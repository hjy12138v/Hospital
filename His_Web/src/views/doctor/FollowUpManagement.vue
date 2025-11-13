<!--
  医生回访管理页面
  
  功能说明：
  - 医生可以查看需要回访的患者列表
  - 支持创建和管理回访计划
  - 记录回访内容和结果
  - 支持多种回访方式（电话、短信、面诊）
  - 统计回访完成情况
  
  业务逻辑：
  - 系统自动生成回访提醒
  - 医生可以手动添加回访计划
  - 回访记录需要详细记录
  - 支持回访结果分类和统计
-->

<template>
  <div class="follow-up-management">
    <!-- 页面标题和操作区 -->
    <div class="page-header">
      <h2>回访管理</h2>
      <div class="header-actions">
        <el-button type="primary" @click="showAddFollowUpDialog">
          <el-icon><Plus /></el-icon>
          添加回访
        </el-button>
        <el-button type="success" @click="exportFollowUpData">
          <el-icon><Download /></el-icon>
          导出数据
        </el-button>
      </div>
    </div>

    <!-- 回访统计卡片 -->
    <el-row :gutter="20" class="stats-row">
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon total">
              <el-icon><UserFilled /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ followUpStats.total }}</div>
              <div class="stat-label">总回访数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
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
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon completed">
              <el-icon><CircleCheck /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ followUpStats.completed }}</div>
              <div class="stat-label">已完成</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon overdue">
              <el-icon><Warning /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ followUpStats.overdue }}</div>
              <div class="stat-label">已逾期</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 筛选和搜索 -->
    <el-card class="filter-card">
      <el-form :model="filterForm" inline>
        <el-form-item label="患者姓名">
          <el-input
            v-model="filterForm.patientName"
            placeholder="请输入患者姓名"
            clearable
          />
        </el-form-item>
        
        <el-form-item label="回访状态">
          <el-select
            v-model="filterForm.status"
            placeholder="请选择状态"
            clearable
          >
            <el-option label="待回访" value="pending" />
            <el-option label="已完成" value="completed" />
            <el-option label="已逾期" value="overdue" />
            <el-option label="已取消" value="cancelled" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="回访方式">
          <el-select
            v-model="filterForm.method"
            placeholder="请选择方式"
            clearable
          >
            <el-option label="电话回访" value="phone" />
            <el-option label="短信回访" value="sms" />
            <el-option label="面诊回访" value="visit" />
            <el-option label="在线咨询" value="online" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="计划时间">
          <el-date-picker
            v-model="filterForm.dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
          />
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" @click="handleSearch">搜索</el-button>
          <el-button @click="resetFilter">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 回访列表 -->
    <el-card class="follow-up-list-card">
      <template #header>
        <div class="card-header">
          <span>回访列表</span>
          <div class="list-actions">
            <el-button-group>
              <el-button 
                :type="viewMode === 'list' ? 'primary' : ''"
                @click="viewMode = 'list'"
              >
                <el-icon><List /></el-icon>
                列表视图
              </el-button>
              <el-button 
                :type="viewMode === 'calendar' ? 'primary' : ''"
                @click="viewMode = 'calendar'"
              >
                <el-icon><Calendar /></el-icon>
                日历视图
              </el-button>
            </el-button-group>
          </div>
        </div>
      </template>
      
      <!-- 列表视图 -->
      <div v-if="viewMode === 'list'">
        <el-table :data="filteredFollowUpList" style="width: 100%">
          <el-table-column prop="patientName" label="患者姓名" width="120" />
          <el-table-column prop="patientPhone" label="联系电话" width="130" />
          <el-table-column prop="lastVisitDate" label="最后就诊" width="120">
            <template #default="{ row }">
              {{ formatDate(row.lastVisitDate) }}
            </template>
          </el-table-column>
          <el-table-column prop="followUpDate" label="计划回访" width="120">
            <template #default="{ row }">
              {{ formatDate(row.followUpDate) }}
            </template>
          </el-table-column>
          <el-table-column prop="method" label="回访方式" width="100">
            <template #default="{ row }">
              <el-tag :type="getMethodColor(row.method)" size="small">
                {{ getMethodText(row.method) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="status" label="状态" width="100">
            <template #default="{ row }">
              <el-tag :type="getStatusColor(row.status)" size="small">
                {{ getStatusText(row.status) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="priority" label="优先级" width="100">
            <template #default="{ row }">
              <el-tag :type="getPriorityColor(row.priority)" size="small">
                {{ getPriorityText(row.priority) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="reason" label="回访原因" />
          <el-table-column label="操作" width="200">
            <template #default="{ row }">
              <el-button 
                type="primary" 
                size="small"
                @click="startFollowUp(row)"
                :disabled="row.status === 'completed'"
              >
                开始回访
              </el-button>
              <el-button 
                type="success" 
                size="small"
                @click="viewFollowUpDetail(row)"
              >
                查看详情
              </el-button>
              <el-button 
                type="warning" 
                size="small"
                @click="editFollowUp(row)"
                :disabled="row.status === 'completed'"
              >
                编辑
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>
      
      <!-- 日历视图 -->
      <div v-else class="calendar-view">
        <el-calendar v-model="calendarDate">
          <template #date-cell="{ data }">
            <div class="calendar-cell">
              <div class="date-number">{{ data.day.split('-').slice(-1)[0] }}</div>
              <div class="follow-up-items">
                <div 
                  v-for="item in getFollowUpsByDate(data.day)"
                  :key="item.id"
                  class="follow-up-item"
                  :class="item.status"
                  @click="viewFollowUpDetail(item)"
                >
                  {{ item.patientName }}
                </div>
              </div>
            </div>
          </template>
        </el-calendar>
      </div>
    </el-card>

    <!-- 添加/编辑回访对话框 -->
    <el-dialog
      v-model="followUpDialogVisible"
      :title="isEditMode ? '编辑回访' : '添加回访'"
      width="700px"
    >
      <el-form
        ref="followUpFormRef"
        :model="followUpForm"
        :rules="followUpRules"
        label-width="100px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="患者姓名" prop="patientName">
              <el-input v-model="followUpForm.patientName" placeholder="请输入患者姓名" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="联系电话" prop="patientPhone">
              <el-input v-model="followUpForm.patientPhone" placeholder="请输入联系电话" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="计划时间" prop="followUpDate">
              <el-date-picker
                v-model="followUpForm.followUpDate"
                type="datetime"
                placeholder="选择回访时间"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="回访方式" prop="method">
              <el-select v-model="followUpForm.method" placeholder="选择回访方式" style="width: 100%">
                <el-option label="电话回访" value="phone" />
                <el-option label="短信回访" value="sms" />
                <el-option label="面诊回访" value="visit" />
                <el-option label="在线咨询" value="online" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="优先级" prop="priority">
              <el-select v-model="followUpForm.priority" placeholder="选择优先级" style="width: 100%">
                <el-option label="高" value="high" />
                <el-option label="中" value="medium" />
                <el-option label="低" value="low" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="最后就诊" prop="lastVisitDate">
              <el-date-picker
                v-model="followUpForm.lastVisitDate"
                type="date"
                placeholder="选择就诊日期"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-form-item label="回访原因" prop="reason">
          <el-input
            v-model="followUpForm.reason"
            type="textarea"
            placeholder="请输入回访原因"
            :rows="3"
          />
        </el-form-item>
        
        <el-form-item label="备注信息">
          <el-input
            v-model="followUpForm.remark"
            type="textarea"
            placeholder="请输入备注信息"
            :rows="2"
          />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="followUpDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleFollowUpSubmit">确定</el-button>
      </template>
    </el-dialog>

    <!-- 回访执行对话框 -->
    <el-dialog
      v-model="executeDialogVisible"
      title="执行回访"
      width="600px"
    >
      <el-form
        ref="executeFormRef"
        :model="executeForm"
        :rules="executeRules"
        label-width="100px"
      >
        <el-form-item label="回访结果" prop="result">
          <el-select v-model="executeForm.result" placeholder="选择回访结果" style="width: 100%">
            <el-option label="成功联系" value="success" />
            <el-option label="无人接听" value="no_answer" />
            <el-option label="号码错误" value="wrong_number" />
            <el-option label="拒绝回访" value="refused" />
            <el-option label="需要复诊" value="need_revisit" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="回访内容" prop="content">
          <el-input
            v-model="executeForm.content"
            type="textarea"
            placeholder="请详细记录回访内容"
            :rows="5"
          />
        </el-form-item>
        
        <el-form-item label="下次回访">
          <el-date-picker
            v-model="executeForm.nextFollowUpDate"
            type="datetime"
            placeholder="选择下次回访时间（可选）"
            style="width: 100%"
          />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="executeDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleExecuteSubmit">完成回访</el-button>
      </template>
    </el-dialog>

    <!-- 回访详情对话框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="回访详情"
      width="800px"
    >
      <div v-if="selectedFollowUp" class="follow-up-detail">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="患者姓名">{{ selectedFollowUp.patientName }}</el-descriptions-item>
          <el-descriptions-item label="联系电话">{{ selectedFollowUp.patientPhone }}</el-descriptions-item>
          <el-descriptions-item label="最后就诊">{{ formatDate(selectedFollowUp.lastVisitDate) }}</el-descriptions-item>
          <el-descriptions-item label="计划回访">{{ formatDate(selectedFollowUp.followUpDate) }}</el-descriptions-item>
          <el-descriptions-item label="回访方式">{{ getMethodText(selectedFollowUp.method) }}</el-descriptions-item>
          <el-descriptions-item label="优先级">{{ getPriorityText(selectedFollowUp.priority) }}</el-descriptions-item>
          <el-descriptions-item label="状态">{{ getStatusText(selectedFollowUp.status) }}</el-descriptions-item>
          <el-descriptions-item label="创建时间">{{ formatDate(selectedFollowUp.createdAt) }}</el-descriptions-item>
        </el-descriptions>
        
        <div class="detail-section">
          <h4>回访原因</h4>
          <p>{{ selectedFollowUp.reason }}</p>
        </div>
        
        <div v-if="selectedFollowUp.executeContent" class="detail-section">
          <h4>回访记录</h4>
          <p>{{ selectedFollowUp.executeContent }}</p>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
/**
 * 医生回访管理页面逻辑
 * 
 * 主要功能：
 * - 回访计划的创建和管理
 * - 回访执行和记录
 * - 回访数据统计和展示
 * - 多种视图模式支持
 */

import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance } from 'element-plus'
import {
  Plus,
  Download,
  UserFilled,
  Clock,
  CircleCheck,
  Warning,
  List,
  Calendar
} from '@element-plus/icons-vue'

// 响应式数据
const viewMode = ref('list') // 视图模式：list | calendar
const calendarDate = ref(new Date())
const followUpDialogVisible = ref(false)
const executeDialogVisible = ref(false)
const detailDialogVisible = ref(false)
const isEditMode = ref(false)
const selectedFollowUp = ref<any>(null)

const followUpFormRef = ref<FormInstance>()
const executeFormRef = ref<FormInstance>()

// 回访统计数据
const followUpStats = ref({
  total: 45,      // 总回访数
  pending: 12,    // 待回访
  completed: 28,  // 已完成
  overdue: 5      // 已逾期
})

// 筛选表单
const filterForm = reactive({
  patientName: '',
  status: '',
  method: '',
  dateRange: null as any
})

// 回访表单
const followUpForm = reactive({
  id: 0,
  patientName: '',
  patientPhone: '',
  followUpDate: '',
  method: '',
  priority: 'medium',
  lastVisitDate: '',
  reason: '',
  remark: ''
})

// 执行回访表单
const executeForm = reactive({
  result: '',
  content: '',
  nextFollowUpDate: ''
})

// 表单验证规则
const followUpRules = {
  patientName: [
    { required: true, message: '请输入患者姓名', trigger: 'blur' }
  ],
  patientPhone: [
    { required: true, message: '请输入联系电话', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ],
  followUpDate: [
    { required: true, message: '请选择回访时间', trigger: 'change' }
  ],
  method: [
    { required: true, message: '请选择回访方式', trigger: 'change' }
  ],
  reason: [
    { required: true, message: '请输入回访原因', trigger: 'blur' }
  ]
}

const executeRules = {
  result: [
    { required: true, message: '请选择回访结果', trigger: 'change' }
  ],
  content: [
    { required: true, message: '请记录回访内容', trigger: 'blur' }
  ]
}

// 模拟回访数据
const followUpList = ref([
  {
    id: 1,
    patientName: '张三',
    patientPhone: '13800138001',
    lastVisitDate: '2024-01-15',
    followUpDate: '2024-01-25',
    method: 'phone',
    status: 'pending',
    priority: 'high',
    reason: '术后恢复情况跟踪',
    remark: '需要关注伤口愈合情况',
    createdAt: '2024-01-16',
    executeContent: ''
  },
  {
    id: 2,
    patientName: '李四',
    patientPhone: '13800138002',
    lastVisitDate: '2024-01-10',
    followUpDate: '2024-01-20',
    method: 'sms',
    status: 'completed',
    priority: 'medium',
    reason: '慢性病用药指导',
    remark: '',
    createdAt: '2024-01-11',
    executeContent: '患者恢复良好，按时服药，无不良反应'
  },
  {
    id: 3,
    patientName: '王五',
    patientPhone: '13800138003',
    lastVisitDate: '2024-01-08',
    followUpDate: '2024-01-18',
    method: 'visit',
    status: 'overdue',
    priority: 'high',
    reason: '复查结果异常',
    remark: '需要面诊详细检查',
    createdAt: '2024-01-09',
    executeContent: ''
  }
])

// 过滤后的回访列表
const filteredFollowUpList = computed(() => {
  return followUpList.value.filter(item => {
    const nameMatch = !filterForm.patientName || item.patientName.includes(filterForm.patientName)
    const statusMatch = !filterForm.status || item.status === filterForm.status
    const methodMatch = !filterForm.method || item.method === filterForm.method
    
    let dateMatch = true
    if (filterForm.dateRange && filterForm.dateRange.length === 2) {
      const itemDate = new Date(item.followUpDate)
      const startDate = new Date(filterForm.dateRange[0])
      const endDate = new Date(filterForm.dateRange[1])
      dateMatch = itemDate >= startDate && itemDate <= endDate
    }
    
    return nameMatch && statusMatch && methodMatch && dateMatch
  })
})

/**
 * 业务逻辑方法
 */

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
    phone: '电话',
    sms: '短信',
    visit: '面诊',
    online: '在线'
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

// 获取优先级颜色
const getPriorityColor = (priority: string) => {
  const colorMap: Record<string, string> = {
    high: 'danger',
    medium: 'warning',
    low: 'success'
  }
  return colorMap[priority] || 'info'
}

// 获取优先级文本
const getPriorityText = (priority: string) => {
  const textMap: Record<string, string> = {
    high: '高',
    medium: '中',
    low: '低'
  }
  return textMap[priority] || priority
}

// 根据日期获取回访列表
const getFollowUpsByDate = (date: string) => {
  return followUpList.value.filter(item => 
    item.followUpDate.startsWith(date)
  )
}

/**
 * 事件处理方法
 */

// 显示添加回访对话框
const showAddFollowUpDialog = () => {
  isEditMode.value = false
  resetFollowUpForm()
  followUpDialogVisible.value = true
}

// 编辑回访
const editFollowUp = (followUp: any) => {
  isEditMode.value = true
  Object.assign(followUpForm, followUp)
  followUpDialogVisible.value = true
}

// 开始回访
const startFollowUp = (followUp: any) => {
  selectedFollowUp.value = followUp
  resetExecuteForm()
  executeDialogVisible.value = true
}

// 查看回访详情
const viewFollowUpDetail = (followUp: any) => {
  selectedFollowUp.value = followUp
  detailDialogVisible.value = true
}

// 导出回访数据
const exportFollowUpData = () => {
  // TODO: 实现数据导出功能
  ElMessage.success('数据导出功能开发中...')
}

// 搜索处理
const handleSearch = () => {
  // 搜索逻辑通过计算属性实现
}

// 重置筛选
const resetFilter = () => {
  Object.assign(filterForm, {
    patientName: '',
    status: '',
    method: '',
    dateRange: null
  })
}

// 提交回访表单
const handleFollowUpSubmit = async () => {
  if (!followUpFormRef.value) return
  
  try {
    await followUpFormRef.value.validate()
    
    // TODO: 调用保存回访接口
    if (isEditMode.value) {
      ElMessage.success('回访更新成功')
    } else {
      ElMessage.success('回访添加成功')
    }
    
    followUpDialogVisible.value = false
  } catch (error) {
    ElMessage.error('表单验证失败')
  }
}

// 提交执行回访表单
const handleExecuteSubmit = async () => {
  if (!executeFormRef.value) return
  
  try {
    await executeFormRef.value.validate()
    
    // TODO: 调用执行回访接口
    // 更新回访状态为已完成
    if (selectedFollowUp.value) {
      selectedFollowUp.value.status = 'completed'
      selectedFollowUp.value.executeContent = executeForm.content
    }
    
    ElMessage.success('回访记录保存成功')
    executeDialogVisible.value = false
  } catch (error) {
    ElMessage.error('表单验证失败')
  }
}

// 重置回访表单
const resetFollowUpForm = () => {
  Object.assign(followUpForm, {
    id: 0,
    patientName: '',
    patientPhone: '',
    followUpDate: '',
    method: '',
    priority: 'medium',
    lastVisitDate: '',
    reason: '',
    remark: ''
  })
  followUpFormRef.value?.clearValidate()
}

// 重置执行表单
const resetExecuteForm = () => {
  Object.assign(executeForm, {
    result: '',
    content: '',
    nextFollowUpDate: ''
  })
  executeFormRef.value?.clearValidate()
}

// 组件挂载时加载数据
onMounted(() => {
  // TODO: 加载回访数据
  // TODO: 加载统计数据
})
</script>

<style scoped>
/**
 * 回访管理页面样式
 */

.follow-up-management {
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

.header-actions {
  display: flex;
  gap: 10px;
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

.stat-icon.completed {
  background: linear-gradient(135deg, #a8edea 0%, #fed6e3 100%);
  color: #333;
}

.stat-icon.overdue {
  background: linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%);
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

/* 筛选卡片 */
.filter-card {
  margin-bottom: 20px;
}

/* 回访列表卡片 */
.follow-up-list-card {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

/* 日历视图 */
.calendar-view {
  min-height: 600px;
}

.calendar-cell {
  height: 100px;
  padding: 4px;
}

.date-number {
  font-weight: bold;
  margin-bottom: 4px;
}

.follow-up-items {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.follow-up-item {
  font-size: 10px;
  padding: 2px 4px;
  border-radius: 2px;
  cursor: pointer;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.follow-up-item.pending {
  background-color: #fff7e6;
  color: #fa8c16;
}

.follow-up-item.completed {
  background-color: #f6ffed;
  color: #52c41a;
}

.follow-up-item.overdue {
  background-color: #fff2f0;
  color: #ff4d4f;
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
</style>