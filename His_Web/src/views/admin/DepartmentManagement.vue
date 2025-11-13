<template>
  <div class="department-management">
    <div class="page-header">
      <h2>科室管理</h2>
      <el-button type="primary" @click="showAddDialog">
        <el-icon><Plus /></el-icon>
        添加科室
      </el-button>
    </div>
    
    <!-- 搜索栏 -->
    <el-card class="search-card">
      <el-form :model="searchForm" inline>
        <el-form-item label="科室名称">
          <el-input
            v-model="searchForm.name"
            placeholder="请输入科室名称"
            clearable
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">搜索</el-button>
          <el-button @click="resetSearch">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 科室列表 -->
    <el-card>
      <el-table
        v-loading="loading"
        :data="filteredDepartments"
        style="width: 100%"
      >
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="name" label="科室名称" />
        <el-table-column prop="description" label="科室描述" />
        <el-table-column prop="location" label="科室位置" />
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
      :title="isEdit ? '编辑科室' : '添加科室'"
      width="600px"
    >
      <el-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-width="80px"
      >
        <el-form-item label="科室名称" prop="name">
          <el-input v-model="form.name" placeholder="请输入科室名称" />
        </el-form-item>
        <el-form-item label="科室描述" prop="description">
          <el-input
            v-model="form.description"
            type="textarea"
            placeholder="请输入科室描述"
            :rows="3"
          />
        </el-form-item>
        <el-form-item label="科室位置" prop="location">
          <el-input v-model="form.location" placeholder="请输入科室位置" />
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
import { departmentAPI } from '@/api'

const loading = ref(false)
const dialogVisible = ref(false)
const isEdit = ref(false)
const formRef = ref<FormInstance>()

const departments = ref<any[]>([])

const searchForm = reactive({
  name: ''
})

const form = reactive({
  id: 0,
  name: '',
  description: '',
  location: ''
})

const rules = {
  name: [
    { required: true, message: '请输入科室名称', trigger: 'blur' }
  ],
  description: [
    { required: true, message: '请输入科室描述', trigger: 'blur' }
  ],
  location: [
    { required: true, message: '请输入科室位置', trigger: 'blur' }
  ]
}

// 过滤后的科室列表
const filteredDepartments = computed(() => {
  return departments.value.filter(dept => {
    const nameMatch = !searchForm.name || dept.name.includes(searchForm.name)
    return nameMatch
  })
})

const loadDepartments = async () => {
  try {
    loading.value = true
    const response = await departmentAPI.getDepartments()
    departments.value = response.data
  } catch (error) {
    ElMessage.error('加载科室列表失败')
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
    description: '',
    location: ''
  })
  formRef.value?.clearValidate()
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    
    if (isEdit.value) {
      await departmentAPI.updateDepartment(form.id, form)
      ElMessage.success('更新成功')
    } else {
      await departmentAPI.addDepartment(form)
      ElMessage.success('添加成功')
    }
    
    dialogVisible.value = false
    loadDepartments()
  } catch (error: any) {
    ElMessage.error(error.message || '操作失败')
  }
}

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除科室 "${row.name}" 吗？`,
      '确认删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await departmentAPI.deleteDepartment(row.id)
    ElMessage.success('删除成功')
    loadDepartments()
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
}

onMounted(() => {
  loadDepartments()
})
</script>

<style scoped>
.department-management {
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