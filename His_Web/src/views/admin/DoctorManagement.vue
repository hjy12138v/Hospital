<template>
  <div class="doctor-management">
    <div class="page-header">
      <h2>医生管理</h2>
      <el-button type="primary" @click="showAddDialog">
        <el-icon><Plus /></el-icon>
        添加医生
      </el-button>
    </div>
    
    <!-- 搜索栏 -->
    <el-card class="search-card">
      <el-form :model="searchForm" inline>
        <el-form-item label="医生姓名">
          <el-input
            v-model="searchForm.name"
            placeholder="请输入医生姓名"
            clearable
          />
        </el-form-item>
        <el-form-item label="科室">
          <el-select
            v-model="searchForm.department"
            placeholder="请选择科室"
            clearable
          >
            <el-option
              v-for="dept in departments"
              :key="dept.id"
              :label="dept.name"
              :value="dept.name"
            />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">搜索</el-button>
          <el-button @click="resetSearch">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 医生列表 -->
    <el-card>
      <el-table
        v-loading="loading"
        :data="filteredDoctors"
        style="width: 100%"
      >
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="name" label="姓名" />
        <el-table-column prop="department" label="科室" />
        <el-table-column prop="title" label="职称" />
        <el-table-column prop="phone" label="电话" />
        <el-table-column prop="email" label="邮箱" />
        <el-table-column prop="schedule" label="出诊时间" />
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
      :title="isEdit ? '编辑医生' : '添加医生'"
      width="600px"
    >
      <el-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-width="80px"
      >
        <el-form-item label="姓名" prop="name">
          <el-input v-model="form.name" placeholder="请输入医生姓名" />
        </el-form-item>
        <el-form-item label="科室" prop="department">
          <el-select
            v-model="form.department"
            placeholder="请选择科室"
            style="width: 100%"
          >
            <el-option
              v-for="dept in departments"
              :key="dept.id"
              :label="dept.name"
              :value="dept.name"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="职称" prop="title">
          <el-select
            v-model="form.title"
            placeholder="请选择职称"
            style="width: 100%"
          >
            <el-option label="主任医师" value="主任医师" />
            <el-option label="副主任医师" value="副主任医师" />
            <el-option label="主治医师" value="主治医师" />
            <el-option label="住院医师" value="住院医师" />
          </el-select>
        </el-form-item>
        <el-form-item label="电话" prop="phone">
          <el-input v-model="form.phone" placeholder="请输入联系电话" />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input v-model="form.email" placeholder="请输入邮箱地址" />
        </el-form-item>
        <el-form-item label="出诊时间" prop="schedule">
          <el-input
            v-model="form.schedule"
            type="textarea"
            placeholder="请输入出诊时间安排"
            :rows="3"
          />
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
import { doctorAPI, departmentAPI } from '@/api/resources'
import type { Doctor, Department } from '@/types/models'

const loading = ref(false)
const dialogVisible = ref(false)
const isEdit = ref(false)
const formRef = ref<FormInstance>()

const doctors = ref<Doctor[]>([])
const departments = ref<Department[]>([])

const searchForm = reactive({
  name: '',
  department: ''
})

const form = reactive({
  id: 0,
  name: '',
  department: '',
  title: '',
  phone: '',
  email: '',
  schedule: ''
})

const rules = {
  name: [
    { required: true, message: '请输入医生姓名', trigger: 'blur' }
  ],
  department: [
    { required: true, message: '请选择科室', trigger: 'change' }
  ],
  title: [
    { required: true, message: '请选择职称', trigger: 'change' }
  ],
  phone: [
    { required: true, message: '请输入联系电话', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入邮箱地址', trigger: 'blur' },
    { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
  ],
  schedule: [
    { required: true, message: '请输入出诊时间安排', trigger: 'blur' }
  ]
}

// 过滤后的医生列表
const filteredDoctors = computed(() => {
  return doctors.value.filter(doctor => {
    const nameMatch = !searchForm.name || doctor.name.includes(searchForm.name)
    const deptMatch = !searchForm.department || doctor.department === searchForm.department
    return nameMatch && deptMatch
  })
})

const loadDoctors = async () => {
  try {
    loading.value = true
    const response = await doctorAPI.getDoctors()
    doctors.value = response.data
  } catch (error) {
    ElMessage.error('加载医生列表失败')
  } finally {
    loading.value = false
  }
}

const loadDepartments = async () => {
  try {
    const response = await departmentAPI.getDepartments()
    departments.value = response.data
  } catch (error) {
    ElMessage.error('加载科室列表失败')
  }
}

const showAddDialog = () => {
  isEdit.value = false
  resetForm()
  dialogVisible.value = true
}

const showEditDialog = (row: Doctor) => {
  isEdit.value = true
  Object.assign(form, row)
  dialogVisible.value = true
}

const resetForm = () => {
  Object.assign(form, {
    id: 0,
    name: '',
    department: '',
    title: '',
    phone: '',
    email: '',
    schedule: ''
  })
  formRef.value?.clearValidate()
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    
    if (isEdit.value) {
      await doctorAPI.updateDoctor(form.id, form)
      ElMessage.success('更新成功')
    } else {
      await doctorAPI.addDoctor(form)
      ElMessage.success('添加成功')
    }
    
    dialogVisible.value = false
    loadDoctors()
  } catch (err: unknown) {
    const message = err instanceof Error ? err.message : '操作失败'
    ElMessage.error(message)
  }
}

const handleDelete = async (row: Doctor) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除医生 "${row.name}" 吗？`,
      '确认删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await doctorAPI.deleteDoctor(row.id)
    ElMessage.success('删除成功')
    loadDoctors()
  } catch (err: unknown) {
    if (err !== 'cancel') {
      const message = err instanceof Error ? err.message : '删除失败'
      ElMessage.error(message)
    }
  }
}

const handleSearch = () => {
  // 搜索功能通过计算属性实现，这里可以添加其他逻辑
}

const resetSearch = () => {
  searchForm.name = ''
  searchForm.department = ''
}

onMounted(() => {
  loadDoctors()
  loadDepartments()
})
</script>

<style scoped>
.doctor-management {
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