<template>
  <div class="medicine-management">
    <div class="page-header">
      <h2>药品管理</h2>
      <el-button type="primary" @click="showAddDialog">
        <el-icon><Plus /></el-icon>
        添加药品
      </el-button>
    </div>
    
    <!-- 搜索栏 -->
    <el-card class="search-card">
      <el-form :model="searchForm" inline>
        <el-form-item label="药品名称">
          <el-input
            v-model="searchForm.name"
            placeholder="请输入药品名称"
            clearable
          />
        </el-form-item>
        <el-form-item label="药品分类">
          <el-select
            v-model="searchForm.category"
            placeholder="请选择药品分类"
            clearable
          >
            <el-option label="抗生素" value="抗生素" />
            <el-option label="解热镇痛" value="解热镇痛" />
            <el-option label="止咳化痰" value="止咳化痰" />
            <el-option label="心血管" value="心血管" />
            <el-option label="消化系统" value="消化系统" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">搜索</el-button>
          <el-button @click="resetSearch">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 药品列表 -->
    <el-card>
      <el-table
        v-loading="loading"
        :data="filteredMedicines"
        style="width: 100%"
      >
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="name" label="药品名称" />
        <el-table-column prop="specification" label="规格" />
        <el-table-column prop="price" label="价格" width="100">
          <template #default="{ row }">
            ¥{{ row.price }}
          </template>
        </el-table-column>
        <el-table-column prop="stock" label="库存" width="100">
          <template #default="{ row }">
            <el-tag
              :type="row.stock > 50 ? 'success' : row.stock > 10 ? 'warning' : 'danger'"
              size="small"
            >
              {{ row.stock }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="manufacturer" label="生产厂家" />
        <el-table-column prop="category" label="分类" />
        <el-table-column label="操作" width="200">
          <template #default="{ row }">
            <el-button
              type="primary"
              size="small"
              @click="showEditDialog(row)"
            >
              编辑
            </el-button>
            <el-button
              type="danger"
              size="small"
              @click="handleDelete(row)"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    
    <!-- 添加/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="isEdit ? '编辑药品' : '添加药品'"
      width="600px"
    >
      <el-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-width="80px"
      >
        <el-form-item label="药品名称" prop="name">
          <el-input v-model="form.name" placeholder="请输入药品名称" />
        </el-form-item>
        <el-form-item label="规格" prop="specification">
          <el-input v-model="form.specification" placeholder="请输入药品规格" />
        </el-form-item>
        <el-form-item label="价格" prop="price">
          <el-input-number
            v-model="form.price"
            :precision="2"
            :step="0.1"
            :min="0"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="库存" prop="stock">
          <el-input-number
            v-model="form.stock"
            :min="0"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="生产厂家" prop="manufacturer">
          <el-input v-model="form.manufacturer" placeholder="请输入生产厂家" />
        </el-form-item>
        <el-form-item label="分类" prop="category">
          <el-select
            v-model="form.category"
            placeholder="请选择药品分类"
            style="width: 100%"
          >
            <el-option label="抗生素" value="抗生素" />
            <el-option label="解热镇痛" value="解热镇痛" />
            <el-option label="止咳化痰" value="止咳化痰" />
            <el-option label="心血管" value="心血管" />
            <el-option label="消化系统" value="消化系统" />
          </el-select>
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { medicineAPI } from '@/api'

const loading = ref(false)
const dialogVisible = ref(false)
const isEdit = ref(false)
const formRef = ref<FormInstance>()

const medicines = ref<any[]>([])

const searchForm = reactive({
  name: '',
  category: ''
})

const form = reactive({
  id: 0,
  name: '',
  specification: '',
  price: 0,
  stock: 0,
  manufacturer: '',
  category: ''
})

const rules = {
  name: [
    { required: true, message: '请输入药品名称', trigger: 'blur' }
  ],
  specification: [
    { required: true, message: '请输入药品规格', trigger: 'blur' }
  ],
  price: [
    { required: true, message: '请输入药品价格', trigger: 'blur' }
  ],
  stock: [
    { required: true, message: '请输入库存数量', trigger: 'blur' }
  ],
  manufacturer: [
    { required: true, message: '请输入生产厂家', trigger: 'blur' }
  ],
  category: [
    { required: true, message: '请选择药品分类', trigger: 'change' }
  ]
}

// 过滤后的药品列表
const filteredMedicines = computed(() => {
  return medicines.value.filter(medicine => {
    const nameMatch = !searchForm.name || medicine.name.includes(searchForm.name)
    const categoryMatch = !searchForm.category || medicine.category === searchForm.category
    return nameMatch && categoryMatch
  })
})

const loadMedicines = async () => {
  try {
    loading.value = true
    const response = await medicineAPI.getMedicines()
    medicines.value = response.data
  } catch (error) {
    ElMessage.error('加载药品列表失败')
  } finally {
    loading.value = false
  }
}

const showAddDialog = () => {
  isEdit.value = false
  resetForm()
  dialogVisible.value = true
}

const showEditDialog = (row: any) => {
  isEdit.value = true
  Object.assign(form, row)
  dialogVisible.value = true
}

const resetForm = () => {
  Object.assign(form, {
    id: 0,
    name: '',
    specification: '',
    price: 0,
    stock: 0,
    manufacturer: '',
    category: ''
  })
  formRef.value?.clearValidate()
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    
    if (isEdit.value) {
      await medicineAPI.updateMedicine(form.id, form)
      ElMessage.success('更新成功')
    } else {
      await medicineAPI.addMedicine(form)
      ElMessage.success('添加成功')
    }
    
    dialogVisible.value = false
    loadMedicines()
  } catch (error: any) {
    ElMessage.error(error.message || '操作失败')
  }
}

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除药品 "${row.name}" 吗？`,
      '确认删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await medicineAPI.deleteMedicine(row.id)
    ElMessage.success('删除成功')
    loadMedicines()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || '删除失败')
    }
  }
}

const handleSearch = () => {
  // 搜索功能通过计算属性实现
}

const resetSearch = () => {
  searchForm.name = ''
  searchForm.category = ''
}

onMounted(() => {
  loadMedicines()
})
</script>

<style scoped>
.medicine-management {
  padding: 0;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  color: #303133;
}

.search-card {
  margin-bottom: 20px;
}

.search-card .el-form {
  margin-bottom: 0;
}
</style>