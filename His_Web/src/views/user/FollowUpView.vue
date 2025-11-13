<!--
  患者回访查看页面
  
  功能说明：
  - 患者可以查看自己的回访记录
  - 显示医生的回访计划和执行情况
  - 支持在线回复医生的回访
  - 查看回访历史和统计信息
  
  业务逻辑：
  - 患者只能查看自己的回访记录
  - 可以对医生的回访进行反馈
  - 支持查看回访详情和历史
  - 提供便民的回访提醒功能
-->

<template>
  <div class="user-follow-up">
    <!-- 页面标题 -->
    <div class="page-header">
      <h2>我的回访记录</h2>
      <div class="header-info">
        <el-tag type="info">
          <el-icon><InfoFilled /></el-icon>
          医生会定期回访您的康复情况
        </el-tag>
      </div>
    </div>

    <!-- 回访统计 -->
    <el-row :gutter="20" class="stats-row">
      <el-col :span="8">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon total">
              <el-icon><ChatDotRound /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ followUpStats.total }}</div>
              <div class="stat-label">总回访次数</div>
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
              <div class="stat-number">{{ followUpStats.pending }}</div>
              <div class="stat-label">待回访</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="8">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon recent">
              <el-icon><Calendar /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ followUpStats.recentDays }}</div>
              <div class="stat-label">最近回访(天前)</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 即将到来的回访 -->
    <el-card v-if="upcomingFollowUps.length > 0" class="upcoming-card">
      <template #header>
        <div class="card-header">
          <span>即将到来的回访</span>
          <el-tag type="warning" size="small">
            <el-icon><Bell /></el-icon>
            {{ upcomingFollowUps.length }} 个待回访
          </el-tag>
        </div>
      </template>
      
      <div class="upcoming-list">
        <div 
          v-for="item in upcomingFollowUps" 
          :key="item.id"
          class="upcoming-item"
        >
          <div class="item-left">
            <div class="doctor-info">
              <el-avatar :size="40" :src="item.doctorAvatar">
                {{ item.doctorName.charAt(0) }}
              </el-avatar>
              <div class="doctor-details">
                <div class="doctor-name">{{ item.doctorName }} 医生</div>
                <div class="doctor-dept">{{ item.doctorDepartment }}</div>
              </div>
            </div>
          </div>
          
          <div class="item-center">
            <div class="follow-up-info">
              <div class="follow-up-time">
                <el-icon><Clock /></el-icon>
                {{ formatDateTime(item.followUpDate) }}
              </div>
              <div class="follow-up-method">
                <el-tag :type="getMethodColor(item.method)" size="small">
                  {{ getMethodText(item.method) }}
                </el-tag>
              </div>
              <div class="follow-up-reason">{{ item.reason }}</div>
            </div>
          </div>
          
          <div class="item-right">
            <el-button 
              type="primary" 
              size="small"
              @click="viewFollowUpDetail(item)"
            >
              查看详情
            </el-button>
          </div>
        </div>
      </div>
    </el-card>

    <!-- 回访历史 -->
    <el-card class="history-card">
      <template #header>
        <div class="card-header">
          <span>回访历史</span>
          <div class="filter-controls">
            <el-select
              v-model="filterStatus"
              placeholder="筛选状态"
              size="small"
              style="width: 120px"
              @change="handleFilterChange"
            >
              <el-option label="全部" value="" />
              <el-option label="已完成" value="completed" />
              <el-option label="待回访" value="pending" />
              <el-option label="已逾期" value="overdue" />
            </el-select>
          </div>
        </div>
      </template>
      
      <el-timeline>
        <el-timeline-item
          v-for="item in filteredFollowUpHistory"
          :key="item.id"
          :timestamp="formatDate(item.followUpDate)"
          :type="getTimelineType(item.status)"
          placement="top"
        >
          <el-card class="timeline-card">
            <div class="timeline-content">
              <div class="timeline-header">
                <div class="doctor-info">
                  <strong>{{ item.doctorName }} 医生</strong>
                  <el-tag size="small" style="margin-left: 8px;">{{ item.doctorDepartment }}</el-tag>
                </div>
                <div class="status-info">
                  <el-tag :type="getStatusColor(item.status)" size="small">
                    {{ getStatusText(item.status) }}
                  </el-tag>
                  <el-tag :type="getMethodColor(item.method)" size="small" style="margin-left: 8px;">
                    {{ getMethodText(item.method) }}
                  </el-tag>
                </div>
              </div>
              
              <div class="timeline-body">
                <div class="follow-up-reason">
                  <strong>回访原因：</strong>{{ item.reason }}
                </div>
                
                <div v-if="item.executeContent" class="follow-up-content">
                  <strong>回访记录：</strong>{{ item.executeContent }}
                </div>
                
                <div v-if="item.patientFeedback" class="patient-feedback">
                  <strong>我的反馈：</strong>{{ item.patientFeedback }}
                </div>
              </div>
              
              <div class="timeline-footer">
                <el-button 
                  type="text" 
                  size="small"
                  @click="viewFollowUpDetail(item)"
                >
                  查看详情
                </el-button>
                <el-button 
                  v-if="item.status === 'completed' && !item.patientFeedback"
                  type="text" 
                  size="small"
                  @click="addFeedback(item)"
                >
                  添加反馈
                </el-button>
              </div>
            </div>
          </el-card>
        </el-timeline-item>
      </el-timeline>
      
      <div v-if="filteredFollowUpHistory.length === 0" class="no-data">
        <el-empty description="暂无回访记录" />
      </div>
    </el-card>

    <!-- 回访详情对话框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="回访详情"
      width="700px"
    >
      <div v-if="selectedFollowUp" class="follow-up-detail">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="医生姓名">{{ selectedFollowUp.doctorName }}</el-descriptions-item>
          <el-descriptions-item label="所属科室">{{ selectedFollowUp.doctorDepartment }}</el-descriptions-item>
          <el-descriptions-item label="回访时间">{{ formatDateTime(selectedFollowUp.followUpDate) }}</el-descriptions-item>
          <el-descriptions-item label="回访方式">{{ getMethodText(selectedFollowUp.method) }}</el-descriptions-item>
          <el-descriptions-item label="回访状态">
            <el-tag :type="getStatusColor(selectedFollowUp.status)">
              {{ getStatusText(selectedFollowUp.status) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="创建时间">{{ formatDateTime(selectedFollowUp.createdAt) }}</el-descriptions-item>
        </el-descriptions>
        
        <div class="detail-section">
          <h4>回访原因</h4>
          <p>{{ selectedFollowUp.reason }}</p>
        </div>
        
        <div v-if="selectedFollowUp.executeContent" class="detail-section">
          <h4>医生回访记录</h4>
          <p>{{ selectedFollowUp.executeContent }}</p>
        </div>
        
        <div v-if="selectedFollowUp.patientFeedback" class="detail-section">
          <h4>我的反馈</h4>
          <p>{{ selectedFollowUp.patientFeedback }}</p>
        </div>
        
        <div v-if="selectedFollowUp.nextFollowUpDate" class="detail-section">
          <h4>下次回访时间</h4>
          <p>{{ formatDateTime(selectedFollowUp.nextFollowUpDate) }}</p>
        </div>
      </div>
    </el-dialog>

    <!-- 添加反馈对话框 -->
    <el-dialog
      v-model="feedbackDialogVisible"
      title="添加反馈"
      width="600px"
    >
      <el-form
        ref="feedbackFormRef"
        :model="feedbackForm"
        :rules="feedbackRules"
        label-width="100px"
      >
        <el-form-item label="反馈内容" prop="content">
          <el-input
            v-model="feedbackForm.content"
            type="textarea"
            placeholder="请详细描述您的康复情况、用药感受或其他需要反馈的信息"
            :rows="6"
            maxlength="500"
            show-word-limit
          />
        </el-form-item>
        
        <el-form-item label="康复评分">
          <el-rate
            v-model="feedbackForm.rating"
            :colors="['#ff9900', '#ff9900', '#67c23a']"
            text-color="#ff9900"
            show-text
          />
          <div class="rating-desc">请为您的康复情况打分</div>
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="feedbackDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleFeedbackSubmit">提交反馈</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
/**
 * 患者回访查看页面逻辑
 * 
 * 主要功能：
 * - 展示患者的回访记录和统计
 * - 提供回访详情查看
 * - 支持患者反馈功能
 * - 显示即将到来的回访提醒
 */

import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, type FormInstance } from 'element-plus'
import {
  InfoFilled,
  ChatDotRound,
  Clock,
  Calendar,
  Bell
} from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'

// 认证状态
const authStore = useAuthStore()

// 响应式数据
const detailDialogVisible = ref(false)
const feedbackDialogVisible = ref(false)
const selectedFollowUp = ref<any>(null)
const filterStatus = ref('')
const feedbackFormRef = ref<FormInstance>()

// 回访统计数据
const followUpStats = ref({
  total: 8,        // 总回访次数
  pending: 2,      // 待回访
  recentDays: 5    // 最近回访天数
})

// 反馈表单
const feedbackForm = reactive({
  content: '',
  rating: 5
})

// 表单验证规则
const feedbackRules = {
  content: [
    { required: true, message: '请输入反馈内容', trigger: 'blur' },
    { min: 10, message: '反馈内容至少10个字符', trigger: 'blur' }
  ]
}

// 即将到来的回访
const upcomingFollowUps = ref([
  {
    id: 1,
    doctorName: '张医生',
    doctorDepartment: '心内科',
    doctorAvatar: '',
    followUpDate: '2024-01-25 14:00:00',
    method: 'phone',
    reason: '术后恢复情况跟踪，了解伤口愈合情况'
  },
  {
    id: 2,
    doctorName: '李医生',
    doctorDepartment: '内分泌科',
    doctorAvatar: '',
    followUpDate: '2024-01-28 10:00:00',
    method: 'sms',
    reason: '糖尿病用药指导和血糖监测'
  }
])

// 回访历史记录
const followUpHistory = ref([
  {
    id: 1,
    doctorName: '张医生',
    doctorDepartment: '心内科',
    followUpDate: '2024-01-20 14:00:00',
    method: 'phone',
    status: 'completed',
    reason: '术后恢复情况跟踪',
    executeContent: '患者恢复良好，伤口愈合正常，无感染迹象。建议继续按时服药，注意休息。',
    patientFeedback: '感谢医生的关心，恢复情况确实不错，会继续按医嘱执行。',
    createdAt: '2024-01-15 09:00:00',
    nextFollowUpDate: '2024-01-25 14:00:00'
  },
  {
    id: 2,
    doctorName: '李医生',
    doctorDepartment: '内分泌科',
    followUpDate: '2024-01-18 10:00:00',
    method: 'sms',
    status: 'completed',
    reason: '糖尿病用药指导',
    executeContent: '血糖控制良好，继续现有治疗方案。',
    patientFeedback: '',
    createdAt: '2024-01-10 16:00:00'
  },
  {
    id: 3,
    doctorName: '王医生',
    doctorDepartment: '骨科',
    followUpDate: '2024-01-15 16:00:00',
    method: 'visit',
    status: 'completed',
    reason: '骨折术后复查',
    executeContent: '骨折愈合良好，可以逐步增加活动量。',
    patientFeedback: '谢谢医生，会按照建议逐步恢复活动。',
    createdAt: '2024-01-08 11:00:00'
  },
  {
    id: 4,
    doctorName: '赵医生',
    doctorDepartment: '呼吸科',
    followUpDate: '2024-01-25 09:00:00',
    method: 'phone',
    status: 'pending',
    reason: '慢性咳嗽治疗效果跟踪',
    executeContent: '',
    patientFeedback: '',
    createdAt: '2024-01-12 14:00:00'
  }
])

// 过滤后的回访历史
const filteredFollowUpHistory = computed(() => {
  if (!filterStatus.value) {
    return followUpHistory.value
  }
  return followUpHistory.value.filter(item => item.status === filterStatus.value)
})

/**
 * 业务逻辑方法
 */

// 格式化日期时间
const formatDateTime = (dateStr: string) => {
  return new Date(dateStr).toLocaleString()
}

// 格式化日期
const formatDate = (dateStr: string) => {
  return new Date(dateStr).toLocaleDateString()
}

// 获取回访方式颜色
const getMethodColor = (method: string) => {
  const colorMap: Record<string, string> = {
    phone: 'primary',
    sms: 'success',
    visit: 'warning',
    online: 'info'
  }
  return colorMap[method] || 'info'
}

// 获取回访方式文本
const getMethodText = (method: string) => {
  const textMap: Record<string, string> = {
    phone: '电话回访',
    sms: '短信回访',
    visit: '面诊回访',
    online: '在线咨询'
  }
  return textMap[method] || method
}

// 获取状态颜色
const getStatusColor = (status: string) => {
  const colorMap: Record<string, string> = {
    pending: 'warning',
    completed: 'success',
    overdue: 'danger',
    cancelled: 'info'
  }
  return colorMap[status] || 'info'
}

// 获取状态文本
const getStatusText = (status: string) => {
  const textMap: Record<string, string> = {
    pending: '待回访',
    completed: '已完成',
    overdue: '已逾期',
    cancelled: '已取消'
  }
  return textMap[status] || status
}

// 获取时间线类型
const getTimelineType = (status: string) => {
  const typeMap: Record<string, string> = {
    pending: 'warning',
    completed: 'success',
    overdue: 'danger',
    cancelled: 'info'
  }
  return typeMap[status] || 'primary'
}

/**
 * 事件处理方法
 */

// 查看回访详情
const viewFollowUpDetail = (followUp: any) => {
  selectedFollowUp.value = followUp
  detailDialogVisible.value = true
}

// 添加反馈
const addFeedback = (followUp: any) => {
  selectedFollowUp.value = followUp
  resetFeedbackForm()
  feedbackDialogVisible.value = true
}

// 筛选状态变更
const handleFilterChange = () => {
  // 筛选逻辑通过计算属性实现
}

// 提交反馈
const handleFeedbackSubmit = async () => {
  if (!feedbackFormRef.value) return
  
  try {
    await feedbackFormRef.value.validate()
    
    // TODO: 调用提交反馈接口
    if (selectedFollowUp.value) {
      selectedFollowUp.value.patientFeedback = feedbackForm.content
    }
    
    ElMessage.success('反馈提交成功，感谢您的配合！')
    feedbackDialogVisible.value = false
  } catch (error) {
    ElMessage.error('表单验证失败')
  }
}

// 重置反馈表单
const resetFeedbackForm = () => {
  Object.assign(feedbackForm, {
    content: '',
    rating: 5
  })
  feedbackFormRef.value?.clearValidate()
}

// 组件挂载时加载数据
onMounted(() => {
  // TODO: 根据当前用户加载回访数据
  // TODO: 加载统计数据
})
</script>

<style scoped>
/**
 * 患者回访查看页面样式
 */

.user-follow-up {
  padding: 0;
}

/* 页面头部 */
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

.header-info .el-tag {
  font-size: 12px;
}

/* 统计卡片 */
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

.stat-icon.total {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.stat-icon.pending {
  background: linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%);
  color: #333;
}

.stat-icon.recent {
  background: linear-gradient(135deg, #a8edea 0%, #fed6e3 100%);
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

/* 即将到来的回访 */
.upcoming-card {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.upcoming-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.upcoming-item {
  display: flex;
  align-items: center;
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 8px;
  border-left: 4px solid #409eff;
}

.item-left {
  margin-right: 20px;
}

.doctor-info {
  display: flex;
  align-items: center;
}

.doctor-details {
  margin-left: 12px;
}

.doctor-name {
  font-weight: bold;
  color: #303133;
  margin-bottom: 4px;
}

.doctor-dept {
  font-size: 12px;
  color: #909399;
}

.item-center {
  flex: 1;
}

.follow-up-info {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.follow-up-time {
  display: flex;
  align-items: center;
  font-weight: bold;
  color: #409eff;
}

.follow-up-time .el-icon {
  margin-right: 4px;
}

.follow-up-reason {
  color: #606266;
  font-size: 14px;
}

/* 回访历史 */
.history-card {
  margin-bottom: 20px;
}

.filter-controls {
  display: flex;
  align-items: center;
  gap: 10px;
}

.timeline-card {
  margin-bottom: 0;
}

.timeline-content {
  padding: 0;
}

.timeline-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.timeline-body {
  margin-bottom: 12px;
}

.timeline-body > div {
  margin-bottom: 8px;
  line-height: 1.6;
}

.timeline-footer {
  display: flex;
  gap: 10px;
}

.patient-feedback {
  background-color: #f0f9ff;
  padding: 8px 12px;
  border-radius: 4px;
  border-left: 3px solid #409eff;
}

/* 回访详情 */
.follow-up-detail {
  padding: 10px 0;
}

.detail-section {
  margin-top: 20px;
}

.detail-section h4 {
  margin: 0 0 10px 0;
  color: #303133;
}

.detail-section p {
  margin: 0;
  color: #606266;
  line-height: 1.6;
}

/* 反馈表单 */
.rating-desc {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}

/* 无数据状态 */
.no-data {
  text-align: center;
  padding: 40px 0;
}
</style>