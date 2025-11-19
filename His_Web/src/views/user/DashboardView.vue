<template>
  <div class="user-dashboard">
    <div class="welcome-section">
      <h2>欢迎回来，{{ authStore.user?.name }}</h2>
      <p>今天是 {{ currentDate }}，祝您身体健康！</p>
    </div>
    
    <!-- 快捷服务 -->
    <el-row :gutter="20" class="service-row">
      <el-col :span="6">
        <el-card class="service-card" @click="$router.push('/user/appointments')">
          <div class="service-content">
            <div class="service-icon appointment">
              <el-icon><Calendar /></el-icon>
            </div>
            <div class="service-info">
              <div class="service-title">预约挂号</div>
              <div class="service-desc">在线预约医生</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="service-card" @click="$router.push('/user/profile')">
          <div class="service-content">
            <div class="service-icon profile">
              <el-icon><User /></el-icon>
            </div>
            <div class="service-info">
              <div class="service-title">个人信息</div>
              <div class="service-desc">管理个人资料</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="service-card" @click="$router.push('/user/notices')">
          <div class="service-content">
            <div class="service-icon notice">
              <el-icon><Bell /></el-icon>
            </div>
            <div class="service-info">
              <div class="service-title">通知公告</div>
              <div class="service-desc">查看医院通知</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="service-card">
          <div class="service-content">
            <div class="service-icon report">
              <el-icon><Document /></el-icon>
            </div>
            <div class="service-info">
              <div class="service-title">检查报告</div>
              <div class="service-desc">查看检查结果</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    
    <!-- 我的预约 -->
    <el-row :gutter="20">
      <el-col :span="16">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>我的预约</span>
              <el-button type="text" @click="$router.push('/user/appointments')">
                查看全部
              </el-button>
            </div>
          </template>
          <el-table :data="myAppointments" style="width: 100%">
            <el-table-column prop="appointmentDate" label="预约日期" />
            <el-table-column prop="appointmentTime" label="预约时间" />
            <el-table-column prop="doctorName" label="医生" />
            <el-table-column prop="department" label="科室" />
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
          </el-table>
          <div v-if="myAppointments.length === 0" class="no-data">
            暂无预约记录
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="8">
        <el-card>
          <template #header>
            <span>最新通知</span>
          </template>
          <div class="notice-list">
            <div
              v-for="notice in recentNotices"
              :key="notice.id"
              class="notice-item"
            >
              <div class="notice-title">{{ notice.title }}</div>
              <div class="notice-time">{{ notice.publishTime }}</div>
            </div>
            <div v-if="recentNotices.length === 0" class="no-data">
              暂无通知
            </div>
          </div>
        </el-card>
        
        <el-card style="margin-top: 20px;">
          <template #header>
            <span>健康提醒</span>
          </template>
          <div class="health-tips">
            <div class="tip-item">
              <el-icon class="tip-icon"><Warning /></el-icon>
              <span>定期体检，关注健康</span>
            </div>
            <div class="tip-item">
              <el-icon class="tip-icon"><Sunny /></el-icon>
              <span>保持良好作息，适量运动</span>
            </div>
            <div class="tip-item">
              <el-icon class="tip-icon"><Coffee /></el-icon>
              <span>均衡饮食，多喝水</span>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { Calendar, User, Bell, Document, Warning, Sunny, Coffee } from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'
import { appointmentAPI } from '@/api'
import { noticeAPI } from '@/api/resources'

const authStore = useAuthStore()

const myAppointments = ref<any[]>([])
const recentNotices = ref<any[]>([])

const currentDate = computed(() => {
  return new Date().toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    weekday: 'long'
  })
})

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

const loadMyAppointments = async () => {
  try {
    const response = await appointmentAPI.getAppointments()
    // 过滤当前用户的预约（这里简化处理）
    myAppointments.value = response.data.filter((apt: any) => 
      apt.patientName === authStore.user?.name
    ).slice(0, 5) // 只显示最近5条
  } catch (error) {
    console.error('加载预约记录失败:', error)
  }
}

const loadRecentNotices = async () => {
  try {
    const response = await noticeAPI.getNotices()
    recentNotices.value = response.data
      .filter((notice: any) => notice.isPublished)
      .slice(0, 3)
  } catch (error) {
    console.error('加载通知失败:', error)
  }
}

onMounted(() => {
  loadMyAppointments()
  loadRecentNotices()
})
</script>

<style scoped>
.user-dashboard {
  padding: 0;
}

.welcome-section {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 30px;
  border-radius: 8px;
  margin-bottom: 20px;
}

.welcome-section h2 {
  margin: 0 0 8px 0;
  font-size: 24px;
  font-weight: 600;
}

.welcome-section p {
  margin: 0;
  opacity: 0.9;
}

.service-row {
  margin-bottom: 20px;
}

.service-card {
  cursor: pointer;
  transition: all 0.3s ease;
  height: 120px;
}

.service-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}

.service-content {
  display: flex;
  align-items: center;
  height: 100%;
}

.service-icon {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 15px;
  font-size: 20px;
  color: white;
}

.service-icon.appointment {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.service-icon.profile {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
}

.service-icon.notice {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
}

.service-icon.report {
  background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%);
}

.service-info {
  flex: 1;
}

.service-title {
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  margin-bottom: 4px;
}

.service-desc {
  font-size: 12px;
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

.notice-list {
  max-height: 200px;
  overflow-y: auto;
}

.notice-item {
  padding: 12px 0;
  border-bottom: 1px solid #f0f0f0;
}

.notice-item:last-child {
  border-bottom: none;
}

.notice-title {
  font-size: 14px;
  color: #303133;
  margin-bottom: 4px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.notice-time {
  font-size: 12px;
  color: #909399;
}

.health-tips {
  padding: 10px 0;
}

.tip-item {
  display: flex;
  align-items: center;
  margin-bottom: 12px;
  font-size: 14px;
  color: #606266;
}

.tip-item:last-child {
  margin-bottom: 0;
}

.tip-icon {
  margin-right: 8px;
  color: #409EFF;
}
</style>