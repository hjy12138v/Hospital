<template>
  <div class="appointment-list">
    <h2>预约管理</h2>
    
    <el-card>
      <el-table :data="appointments" style="width: 100%">
        <el-table-column prop="appointmentDate" label="预约日期" />
        <el-table-column prop="appointmentTime" label="预约时间" />
        <el-table-column prop="patientName" label="患者姓名" />
        <el-table-column prop="symptoms" label="症状" />
        <el-table-column prop="status" label="状态">
          <template #default="{ row }">
            <el-tag
              :type="row.status === 'confirmed' ? 'success' : 'warning'"
              size="small"
            >
              {{ row.status === 'confirmed' ? '已确认' : '待确认' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作">
          <template #default="{ row }">
            <el-button type="primary" size="small">处理</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { appointmentAPI } from '@/api'

const appointments = ref<any[]>([])

const loadAppointments = async () => {
  try {
    const response = await appointmentAPI.getAppointments()
    appointments.value = response.data
  } catch (error) {
    console.error('加载预约列表失败:', error)
  }
}

onMounted(() => {
  loadAppointments()
})
</script>

<style scoped>
.appointment-list h2 {
  margin: 0 0 20px 0;
  color: #303133;
}
</style>