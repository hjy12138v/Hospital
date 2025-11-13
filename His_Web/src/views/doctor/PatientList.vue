<template>
  <div class="patient-list">
    <h2>患者管理</h2>
    
    <el-card>
      <el-table :data="patients" style="width: 100%">
        <el-table-column prop="name" label="姓名" />
        <el-table-column prop="age" label="年龄" />
        <el-table-column prop="gender" label="性别" />
        <el-table-column prop="phone" label="联系电话" />
        <el-table-column prop="medicalHistory" label="病史" />
        <el-table-column label="操作">
          <template #default="{ row }">
            <el-button type="primary" size="small">查看详情</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { patientAPI } from '@/api'

const patients = ref<any[]>([])

const loadPatients = async () => {
  try {
    const response = await patientAPI.getPatients()
    patients.value = response.data
  } catch (error) {
    console.error('加载患者列表失败:', error)
  }
}

onMounted(() => {
  loadPatients()
})
</script>

<style scoped>
.patient-list h2 {
  margin: 0 0 20px 0;
  color: #303133;
}
</style>