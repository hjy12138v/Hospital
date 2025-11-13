<template>
  <div class="appointment-view">
    <h2>预约挂号</h2>
    
    <!-- 预约表单 -->
    <el-card style="margin-bottom: 20px;">
      <template #header>
        <span>新建预约</span>
      </template>
      <el-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-width="100px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="选择科室" prop="department">
              <el-select
                v-model="form.department"
                placeholder="请选择科室"
                style="width: 100%"
                @change="onDepartmentChange"
              >
                <el-option
                  v-for="dept in departments"
                  :key="dept.id"
                  :label="dept.name"
                  :value="dept.name"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="选择医生" prop="doctorId">
              <el-select
                v-model="form.doctorId"
                placeholder="请选择医生"
                style="width: 100%"
              >
                <el-option
                  v-for="doctor in filteredDoctors"
                  :key="doctor.id"
                  :label="`${doctor.name} (${doctor.title})`"
                  :value="doctor.id"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="预约日期" prop="appointmentDate">
              <el-date-picker
                v-model="form.appointmentDate"
                type="date"
                placeholder="选择日期"
                style="width: 100%"
                :disabled-date="disabledDate"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="预约时间" prop="appointmentTime">
              <el-select
                v-model="form.appointmentTime"
                placeholder="请选择时间"
                style="width: 100%"
              >
                <el-option label="09:00" value="09:00" />
                <el-option label="10:00" value="10:00" />
                <el-option label="11:00" value="11:00" />
                <el-option label="14:00" value="14:00" />
                <el-option label="15:00" value="15:00" />
                <el-option label="16:00" value="16:00" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="症状描述" prop="symptoms">
          <el-input
            v-model="form.symptoms"
            type="textarea"
            placeholder="请简要描述症状"
            :rows="3"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSubmit">提交预约</el-button>
          <el-button @click="resetForm">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 我的预约 -->
    <el-card>
      <template #header>
        <span>我的预约</span>
      </template>
      <el-table :data="myAppointments" style="width: 100%">
        <el-table-column prop="appointmentDate" label="预约日期" />
        <el-table-column prop="appointmentTime" label="预约时间" />
        <el-table-column prop="doctorName" label="医生" />
        <el-table-column prop="department" label="科室" />
        <el-table-column prop="symptoms" label="症状" />
        <el-table-column prop="status" label="状态">
          <template #default="{ row }">
            <el-tag
              :type="getStatusType(row.status)"
              size="small"
            >
              {{ getStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作">
          <template #default="{ row }">
            <el-button
              v-if="row.status === 'pending'"
              type="danger"
              size="small"
            >
              取消预约
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, type FormInstance } from 'element-plus'
import { useAuthStore } from '@/stores/auth'
import { appointmentAPI, doctorAPI, departmentAPI } from '@/api'

const authStore = useAuthStore()
const formRef = ref<FormInstance>()

const departments = ref<any[]>([])
const doctors = ref<any[]>([])
const myAppointments = ref<any[]>([])

const form = reactive({
  department: '',
  doctorId: '',
  appointmentDate: '',
  appointmentTime: '',
  symptoms: ''
})

const rules = {
  department: [
    { required: true, message: '请选择科室', trigger: 'change' }
  ],
  doctorId: [
    { required: true, message: '请选择医生', trigger: 'change' }
  ],
  appointmentDate: [
    { required: true, message: '请选择预约日期', trigger: 'change' }
  ],
  appointmentTime: [
    { required: true, message: '请选择预约时间', trigger: 'change' }
  ],
  symptoms: [
    { required: true, message: '请描述症状', trigger: 'blur' }
  ]
}

const filteredDoctors = computed(() => {
  return doctors.value.filter(doctor => doctor.department === form.department)
})

const disabledDate = (time: Date) => {
  return time.getTime() < Date.now() - 8.64e7 // 不能选择今天之前的日期
}

const getStatusType = (status: string) => {
  switch (status) {
    case 'confirmed':
      return 'success'
    case 'pending':
      return 'warning'
    case 'cancelled':
      return 'danger'
    default:
      return 'info'
  }
}

const getStatusText = (status: string) => {
  switch (status) {
    case 'confirmed':
      return '已确认'
    case 'pending':
      return '待确认'
    case 'cancelled':
      return '已取消'
    default:
      return '未知'
  }
}

const onDepartmentChange = () => {
  form.doctorId = '' // 重置医生选择
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    
    const selectedDoctor = doctors.value.find(d => d.id === form.doctorId)
    
    const appointmentData = {
      patientId: authStore.user?.id,
      patientName: authStore.user?.name,
      doctorId: form.doctorId,
      doctorName: selectedDoctor?.name,
      department: form.department,
      appointmentDate: form.appointmentDate,
      appointmentTime: form.appointmentTime,
      symptoms: form.symptoms
    }
    
    await appointmentAPI.addAppointment(appointmentData)
    ElMessage.success('预约提交成功，请等待医生确认')
    
    resetForm()
    loadMyAppointments()
  } catch (error: any) {
    ElMessage.error(error.message || '预约失败')
  }
}

const resetForm = () => {
  Object.assign(form, {
    department: '',
    doctorId: '',
    appointmentDate: '',
    appointmentTime: '',
    symptoms: ''
  })
  formRef.value?.clearValidate()
}

const loadDepartments = async () => {
  try {
    const response = await departmentAPI.getDepartments()
    departments.value = response.data
  } catch (error) {
    console.error('加载科室列表失败:', error)
  }
}

const loadDoctors = async () => {
  try {
    const response = await doctorAPI.getDoctors()
    doctors.value = response.data
  } catch (error) {
    console.error('加载医生列表失败:', error)
  }
}

const loadMyAppointments = async () => {
  try {
    const response = await appointmentAPI.getAppointments()
    myAppointments.value = response.data.filter((apt: any) => 
      apt.patientName === authStore.user?.name
    )
  } catch (error) {
    console.error('加载预约记录失败:', error)
  }
}

onMounted(() => {
  loadDepartments()
  loadDoctors()
  loadMyAppointments()
})
</script>

<style scoped>
.appointment-view h2 {
  margin: 0 0 20px 0;
  color: #303133;
}
</style>