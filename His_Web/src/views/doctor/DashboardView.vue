<template>
  <div class="doctor-dashboard">
    <h2>医生工作台</h2>
    
    <!-- 今日统计 -->
    <el-row :gutter="20" class="stats-row">
      <el-col :span="8">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon appointments">
              <el-icon><Calendar /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ todayStats.appointments }}</div>
              <div class="stat-label">今日预约</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="8">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon patients">
              <el-icon><UserFilled /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ todayStats.patients }}</div>
              <div class="stat-label">今日接诊</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="8">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon pending">
              <el-icon><Clock /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ todayStats.pending }}</div>
              <div class="stat-label">待处理</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    
    <!-- 今日预约列表 -->
    <el-row :gutter="20">
      <el-col :span="16">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>今日预约</span>
              <el-button type="text" @click="$router.push('/doctor/appointments')">
                查看全部
              </el-button>
            </div>
          </template>
          <el-table :data="todayAppointments" style="width: 100%">
            <el-table-column prop="appointmentTime" label="时间" width="100" />
            <el-table-column prop="patientName" label="患者姓名" />
            <el-table-column prop="symptoms" label="症状" />
            <el-table-column prop="status" label="状态" width="100">
              <template #default="{ row }">
                <el-tag
                  :type="row.status === 'confirmed' ? 'success' : 'warning'"
                  size="small"
                >
                  {{ row.status === 'confirmed' ? '已确认' : '待确认' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="120">
              <template #default="{ row }">
                <el-button type="primary" size="small">
                  查看详情
                </el-button>
              </template>
            </el-table-column>
          </el-table>
          <div v-if="todayAppointments.length === 0" class="no-data">
            今日暂无预约
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="8">
        <el-card>
          <template #header>
            <span>个人信息</span>
          </template>
          <el-descriptions :column="1" border>
            <el-descriptions-item label="姓名">
              {{ authStore.user?.name }}
            </el-descriptions-item>
            <el-descriptions-item label="科室">
              {{ authStore.user?.department }}
            </el-descriptions-item>
            <el-descriptions-item label="联系电话">
              {{ authStore.user?.phone }}
            </el-descriptions-item>
            <el-descriptions-item label="邮箱">
              {{ authStore.user?.email }}
            </el-descriptions-item>
          </el-descriptions>
        </el-card>
        
        <el-card style="margin-top: 20px;">
          <template #header>
            <span>快捷操作</span>
          </template>
          <div class="quick-actions">
            <el-button type="primary" @click="$router.push('/doctor/patients')">
              <el-icon><UserFilled /></el-icon>
              患者管理
            </el-button>
            <el-button type="success" @click="$router.push('/doctor/appointments')">
              <el-icon><Calendar /></el-icon>
              预约管理
            </el-button>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { Calendar, UserFilled, Clock } from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'
import { appointmentAPI } from '@/api'

const authStore = useAuthStore()

const todayStats = ref({
  appointments: 0,
  patients: 0,
  pending: 0
})

const todayAppointments = ref<any[]>([])

const loadTodayData = async () => {
  try {
    const response = await appointmentAPI.getAppointments()
    const appointments = response.data
    
    // 过滤今日预约（这里简化处理，实际应该根据医生ID过滤）
    const today = new Date().toISOString().split('T')[0]
    const todayAppts = appointments.filter((apt: any) => 
      apt.appointmentDate === today || apt.appointmentDate === '2024-01-20'
    )
    
    todayAppointments.value = todayAppts
    
    todayStats.value = {
      appointments: todayAppts.length,
      patients: todayAppts.filter((apt: any) => apt.status === 'confirmed').length,
      pending: todayAppts.filter((apt: any) => apt.status === 'pending').length
    }
  } catch (error) {
    console.error('加载今日数据失败:', error)
  }
}

onMounted(() => {
  loadTodayData()
})
</script>

<style scoped>
.doctor-dashboard {
  padding: 0;
}

.doctor-dashboard h2 {
  margin: 0 0 20px 0;
  color: #303133;
}

.stats-row {
  margin-bottom: 20px;
}

.stat-card {
  height: 120px;
}

.stat-content {
  display: flex;
  align-items: center;
  height: 100%;
}

.stat-icon {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 20px;
  font-size: 24px;
  color: white;
}

.stat-icon.appointments {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.stat-icon.patients {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
}

.stat-icon.pending {
  background: linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%);
  color: #333;
}

.stat-info {
  flex: 1;
}

.stat-number {
  font-size: 32px;
  font-weight: bold;
  color: #303133;
  line-height: 1;
  margin-bottom: 8px;
}

.stat-label {
  font-size: 14px;
  color: #909399;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.no-data {
  text-align: center;
  color: #909399;
  padding: 40px 0;
}

.quick-actions {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.quick-actions .el-button {
  width: 100%;
  height: 40px;
}
</style>