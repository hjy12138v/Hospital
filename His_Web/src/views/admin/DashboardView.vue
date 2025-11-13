<template>
  <div class="dashboard">
    <h2>管理员仪表盘</h2>
    
    <!-- 统计卡片 -->
    <el-row :gutter="20" class="stats-row">
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon doctor">
              <el-icon><User /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.doctorCount }}</div>
              <div class="stat-label">医生总数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon user">
              <el-icon><UserFilled /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.userCount }}</div>
              <div class="stat-label">用户总数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon department">
              <el-icon><OfficeBuilding /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.departmentCount }}</div>
              <div class="stat-label">科室总数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon medicine">
              <el-icon><FirstAidKit /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.medicineCount }}</div>
              <div class="stat-label">药品总数</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    
    <!-- 快捷操作 -->
    <el-row :gutter="20" class="quick-actions">
      <el-col :span="12">
        <el-card>
          <template #header>
            <span>快捷操作</span>
          </template>
          <div class="action-buttons">
            <el-button type="primary" @click="$router.push('/admin/doctors')">
              <el-icon><User /></el-icon>
              管理医生
            </el-button>
            <el-button type="success" @click="$router.push('/admin/users')">
              <el-icon><UserFilled /></el-icon>
              管理用户
            </el-button>
            <el-button type="warning" @click="$router.push('/admin/medicines')">
              <el-icon><FirstAidKit /></el-icon>
              管理药品
            </el-button>
            <el-button type="info" @click="$router.push('/admin/notices')">
              <el-icon><Bell /></el-icon>
              发布通知
            </el-button>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="12">
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
      </el-col>
    </el-row>
    
    <!-- 系统信息 -->
    <el-row :gutter="20">
      <el-col :span="24">
        <el-card>
          <template #header>
            <span>系统信息</span>
          </template>
          <el-descriptions :column="3" border>
            <el-descriptions-item label="系统版本">v1.0.0</el-descriptions-item>
            <el-descriptions-item label="当前用户">{{ authStore.user?.name }}</el-descriptions-item>
            <el-descriptions-item label="登录时间">{{ loginTime }}</el-descriptions-item>
            <el-descriptions-item label="系统状态">
              <el-tag type="success">正常运行</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="数据库状态">
              <el-tag type="success">连接正常</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="服务器状态">
              <el-tag type="success">运行正常</el-tag>
            </el-descriptions-item>
          </el-descriptions>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { User, UserFilled, OfficeBuilding, FirstAidKit, Bell } from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'
import { doctorAPI, departmentAPI, medicineAPI, noticeAPI, patientAPI } from '@/api'

const authStore = useAuthStore()

const stats = ref({
  doctorCount: 0,
  userCount: 0,
  departmentCount: 0,
  medicineCount: 0
})

const recentNotices = ref<any[]>([])
const loginTime = ref(new Date().toLocaleString())

const loadStats = async () => {
  try {
    const [doctorsRes, departmentsRes, medicinesRes, patientsRes, noticesRes] = await Promise.all([
      doctorAPI.getDoctors(),
      departmentAPI.getDepartments(),
      medicineAPI.getMedicines(),
      patientAPI.getPatients(),
      noticeAPI.getNotices()
    ])
    
    stats.value = {
      doctorCount: doctorsRes.data.length,
      userCount: patientsRes.data.length,
      departmentCount: departmentsRes.data.length,
      medicineCount: medicinesRes.data.length
    }
    
    // 获取最新的3条通知
    recentNotices.value = noticesRes.data
      .filter((notice: any) => notice.isPublished)
      .slice(0, 3)
  } catch (error) {
    console.error('加载统计数据失败:', error)
  }
}

onMounted(() => {
  loadStats()
})
</script>

<style scoped>
.dashboard {
  padding: 0;
}

.dashboard h2 {
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

.stat-icon.doctor {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.stat-icon.user {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
}

.stat-icon.department {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
}

.stat-icon.medicine {
  background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%);
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

.quick-actions {
  margin-bottom: 20px;
}

.action-buttons {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 15px;
}

.action-buttons .el-button {
  height: 50px;
  font-size: 14px;
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

.no-data {
  text-align: center;
  color: #909399;
  padding: 20px 0;
}
</style>