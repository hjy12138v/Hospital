<!--
  医生排班管理页面
  
  功能说明：
  - 医生可以查看和设置自己的排班信息
  - 支持按周查看排班安排
  - 可以设置每日的工作时间段
  - 支持请假和调班申请
  - 显示排班统计信息
  
  业务逻辑：
  - 医生只能管理自己的排班
  - 排班变更需要管理员审核
  - 支持重复排班模板设置
  - 与预约系统联动，影响可预约时间
-->

<template>
  <div class="schedule-management">
    <!-- 页面标题和操作区 -->
    <div class="page-header">
      <h2>排班管理</h2>
      <div class="header-actions">
        <el-button type="primary" @click="showAddScheduleDialog">
          <el-icon><Plus /></el-icon>
          添加排班
        </el-button>
        <el-button type="success" @click="showTemplateDialog">
          <el-icon><Setting /></el-icon>
          排班模板
        </el-button>
      </div>
    </div>

    <!-- 排班统计卡片 -->
    <el-row :gutter="20" class="stats-row">
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon work-days">
              <el-icon><Calendar /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ scheduleStats.workDays }}</div>
              <div class="stat-label">本月工作日</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon work-hours">
              <el-icon><Clock /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ scheduleStats.workHours }}</div>
              <div class="stat-label">本月工作时长</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon appointments">
              <el-icon><UserFilled /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ scheduleStats.appointments }}</div>
              <div class="stat-label">预约患者数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon leave-days">
              <el-icon><Warning /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ scheduleStats.leaveDays }}</div>
              <div class="stat-label">请假天数</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 排班日历 -->
    <el-card class="calendar-card">
      <template #header>
        <div class="card-header">
          <span>排班日历</span>
          <div class="calendar-controls">
            <el-date-picker
              v-model="currentWeek"
              type="week"
              placeholder="选择周"
              @change="handleWeekChange"
            />
          </div>
        </div>
      </template>
      
      <!-- 周视图排班表 -->
      <div class="schedule-calendar">
        <div class="calendar-header">
          <div class="time-column">时间</div>
          <div 
            v-for="day in weekDays" 
            :key="day.date.getTime()"
            class="day-column"
            :class="{ 'today': isToday(day.date) }"
          >
            <div class="day-name">{{ day.name }}</div>
            <div class="day-date">{{ formatDate(day.date) }}</div>
          </div>
        </div>
        
        <div class="calendar-body">
          <div 
            v-for="hour in workHours" 
            :key="hour"
            class="time-row"
          >
            <div class="time-cell">{{ hour }}:00</div>
            <div 
              v-for="day in weekDays" 
              :key="`${day.date}-${hour}`"
              class="schedule-cell"
              :class="getScheduleCellClass(day.date, hour)"
              @click="handleCellClick(day.date, hour)"
            >
              <div v-if="getScheduleInfo(day.date, hour)" class="schedule-info">
                {{ getScheduleInfo(day.date, hour) }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </el-card>

    <!-- 排班列表 -->
    <el-card class="schedule-list-card">
      <template #header>
        <span>排班记录</span>
      </template>
      
      <el-table :data="scheduleList" style="width: 100%">
        <el-table-column prop="date" label="日期" width="120">
          <template #default="{ row }">
            {{ formatDateString(row.date) }}
          </template>
        </el-table-column>
        <el-table-column prop="dayOfWeek" label="星期" width="80" />
        <el-table-column prop="startTime" label="开始时间" width="100" />
        <el-table-column prop="endTime" label="结束时间" width="100" />
        <el-table-column prop="type" label="排班类型" width="100">
          <template #default="{ row }">
            <el-tag :type="getScheduleTypeColor(row.type)" size="small">
              {{ getScheduleTypeText(row.type) }}
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
        <el-table-column prop="appointmentCount" label="预约数" width="80" />
        <el-table-column prop="remark" label="备注" />
        <el-table-column label="操作" width="200">
          <template #default="{ row }">
            <el-button 
              type="primary" 
              size="small"
              @click="editSchedule(row)"
              :disabled="!canEditSchedule(row)"
            >
              编辑
            </el-button>
            <el-button 
              type="warning" 
              size="small"
              @click="requestLeave(row)"
              :disabled="!canRequestLeave(row)"
            >
              请假
            </el-button>
            <el-button 
              type="danger" 
              size="small"
              @click="deleteSchedule(row)"
              :disabled="!canDeleteSchedule(row)"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 添加/编辑排班对话框 -->
    <el-dialog
      v-model="scheduleDialogVisible"
      :title="isEditMode ? '编辑排班' : '添加排班'"
      width="600px"
    >
      <el-form
        ref="scheduleFormRef"
        :model="scheduleForm"
        :rules="scheduleRules"
        label-width="100px"
      >
        <el-form-item label="排班日期" prop="date">
          <el-date-picker
            v-model="scheduleForm.date"
            type="date"
            placeholder="选择日期"
            style="width: 100%"
            :disabled-date="disabledDate"
          />
        </el-form-item>
        
        <el-form-item label="开始时间" prop="startTime">
          <el-time-picker
            v-model="scheduleForm.startTime"
            placeholder="选择开始时间"
            style="width: 100%"
          />
        </el-form-item>
        
        <el-form-item label="结束时间" prop="endTime">
          <el-time-picker
            v-model="scheduleForm.endTime"
            placeholder="选择结束时间"
            style="width: 100%"
          />
        </el-form-item>
        
        <el-form-item label="排班类型" prop="type">
          <el-select v-model="scheduleForm.type" placeholder="选择排班类型" style="width: 100%">
            <el-option label="正常班" value="normal" />
            <el-option label="夜班" value="night" />
            <el-option label="急诊班" value="emergency" />
            <el-option label="值班" value="duty" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="备注" prop="remark">
          <el-input
            v-model="scheduleForm.remark"
            type="textarea"
            placeholder="请输入备注信息"
            :rows="3"
          />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="scheduleDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleScheduleSubmit">确定</el-button>
      </template>
    </el-dialog>

    <!-- 排班模板对话框 -->
    <el-dialog
      v-model="templateDialogVisible"
      title="排班模板设置"
      width="800px"
    >
      <div class="template-content">
        <p>设置常用的排班模板，可以快速应用到多个日期</p>
        <!-- TODO: 实现排班模板功能 -->
        <el-empty description="排班模板功能开发中..." />
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
/**
 * 医生排班管理页面逻辑
 * 
 * 主要功能：
 * - 排班日历展示和操作
 * - 排班信息的增删改查
 * - 排班统计数据展示
 * - 请假和调班申请
 */

import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance } from 'element-plus'
import {
  Plus,
  Setting,
  Calendar,
  Clock,
  UserFilled,
  Warning
} from '@element-plus/icons-vue'

// 类型定义
interface Schedule {
  id: number
  date: string
  dayOfWeek: string
  startTime: string
  endTime: string
  type: 'normal' | 'night' | 'emergency' | 'duty'
  status: 'confirmed' | 'pending' | 'cancelled'
  appointmentCount: number
  remark: string
}

interface ScheduleForm {
  id: number
  date: string
  startTime: string
  endTime: string
  type: string
  remark: string
}

// 响应式数据
const currentWeek = ref(new Date())
const scheduleDialogVisible = ref(false)
const templateDialogVisible = ref(false)
const isEditMode = ref(false)
const scheduleFormRef = ref<FormInstance>()

// 排班统计数据
const scheduleStats = ref({
  workDays: 22,      // 本月工作日
  workHours: 176,    // 本月工作时长
  appointments: 156, // 预约患者数
  leaveDays: 2       // 请假天数
})

// 排班表单数据
const scheduleForm = reactive<ScheduleForm>({
  id: 0,
  date: '',
  startTime: '',
  endTime: '',
  type: 'normal',
  remark: ''
})

// 表单验证规则
const scheduleRules = {
  date: [
    { required: true, message: '请选择排班日期', trigger: 'change' }
  ],
  startTime: [
    { required: true, message: '请选择开始时间', trigger: 'change' }
  ],
  endTime: [
    { required: true, message: '请选择结束时间', trigger: 'change' }
  ],
  type: [
    { required: true, message: '请选择排班类型', trigger: 'change' }
  ]
}

// 模拟排班数据
const scheduleList = ref<Schedule[]>([
  {
    id: 1,
    date: '2024-01-22',
    dayOfWeek: '周一',
    startTime: '08:00',
    endTime: '17:00',
    type: 'normal',
    status: 'confirmed',
    appointmentCount: 12,
    remark: '正常门诊'
  },
  {
    id: 2,
    date: '2024-01-23',
    dayOfWeek: '周二',
    startTime: '08:00',
    endTime: '17:00',
    type: 'normal',
    status: 'confirmed',
    appointmentCount: 8,
    remark: ''
  },
  {
    id: 3,
    date: '2024-01-24',
    dayOfWeek: '周三',
    startTime: '18:00',
    endTime: '08:00',
    type: 'night',
    status: 'pending',
    appointmentCount: 0,
    remark: '夜班值班'
  }
])

// 工作时间段（24小时制）
const workHours = Array.from({ length: 24 }, (_, i) => i)

// 计算当前周的日期
const weekDays = computed(() => {
  const days = []
  const startOfWeek = new Date(currentWeek.value)
  startOfWeek.setDate(startOfWeek.getDate() - startOfWeek.getDay() + 1) // 周一开始
  
  for (let i = 0; i < 7; i++) {
    const date = new Date(startOfWeek)
    date.setDate(startOfWeek.getDate() + i)
    days.push({
      date: date,
      name: ['周一', '周二', '周三', '周四', '周五', '周六', '周日'][i]
    })
  }
  return days
})

/**
 * 业务逻辑方法
 */

// 检查是否为今天
const isToday = (date: Date) => {
  const today = new Date()
  return date.toDateString() === today.toDateString()
}

// 格式化日期显示
const formatDate = (date: Date) => {
  return `${date.getMonth() + 1}/${date.getDate()}`
}

// 格式化日期字符串
const formatDateString = (dateStr: string) => {
  return new Date(dateStr).toLocaleDateString()
}

// 获取排班单元格样式
const getScheduleCellClass = (date: Date, hour: number) => {
  const schedule = getScheduleByDateTime(date, hour)
  if (schedule) {
    return `scheduled ${schedule.type}`
  }
  return ''
}

// 获取排班信息
const getScheduleInfo = (date: Date, hour: number) => {
  const schedule = getScheduleByDateTime(date, hour)
  return schedule ? `${schedule.type}班` : ''
}

// 根据日期和时间获取排班信息
const getScheduleByDateTime = (date: Date, hour: number) => {
  const dateStr = date.toISOString().split('T')[0]
  return scheduleList.value.find(schedule => {
    if (schedule.date === dateStr) {
      const startHour = parseInt(schedule.startTime.split(':')[0] || '0')
      const endHour = parseInt(schedule.endTime.split(':')[0] || '0')
      return hour >= startHour && hour < endHour
    }
    return false
  })
}

// 获取排班类型颜色
const getScheduleTypeColor = (type: string) => {
  const colorMap: Record<string, string> = {
    normal: 'success',
    night: 'warning',
    emergency: 'danger',
    duty: 'info'
  }
  return colorMap[type] || 'info'
}

// 获取排班类型文本
const getScheduleTypeText = (type: string) => {
  const textMap: Record<string, string> = {
    normal: '正常班',
    night: '夜班',
    emergency: '急诊班',
    duty: '值班'
  }
  return textMap[type] || type
}

// 获取状态颜色
const getStatusColor = (status: string) => {
  const colorMap: Record<string, string> = {
    confirmed: 'success',
    pending: 'warning',
    cancelled: 'danger'
  }
  return colorMap[status] || 'info'
}

// 获取状态文本
const getStatusText = (status: string) => {
  const textMap: Record<string, string> = {
    confirmed: '已确认',
    pending: '待审核',
    cancelled: '已取消'
  }
  return textMap[status] || status
}

// 检查是否可以编辑排班
const canEditSchedule = (schedule: Schedule) => {
  const scheduleDate = new Date(schedule.date)
  const today = new Date()
  return scheduleDate > today && schedule.status !== 'cancelled'
}

// 检查是否可以请假
const canRequestLeave = (schedule: Schedule) => {
  const scheduleDate = new Date(schedule.date)
  const today = new Date()
  return scheduleDate > today && schedule.status === 'confirmed'
}

// 检查是否可以删除排班
const canDeleteSchedule = (schedule: Schedule) => {
  return schedule.status === 'pending'
}

// 禁用过去的日期
const disabledDate = (time: Date) => {
  return time.getTime() < Date.now() - 8.64e7
}

/**
 * 事件处理方法
 */

// 周变更处理
const handleWeekChange = (date: Date) => {
  currentWeek.value = date
  // TODO: 重新加载该周的排班数据
}

// 日历单元格点击处理
const handleCellClick = (date: Date, hour: number) => {
  const existingSchedule = getScheduleByDateTime(date, hour)
  if (existingSchedule) {
    editSchedule(existingSchedule)
  } else {
    showAddScheduleDialog()
    scheduleForm.date = date.toISOString().split('T')[0]||'0'
    scheduleForm.startTime = `${hour.toString().padStart(2, '0')}:00`
  }
}

// 显示添加排班对话框
const showAddScheduleDialog = () => {
  isEditMode.value = false
  resetScheduleForm()
  scheduleDialogVisible.value = true
}

// 显示排班模板对话框
const showTemplateDialog = () => {
  templateDialogVisible.value = true
}

// 编辑排班
const editSchedule = (schedule: Schedule) => {
  isEditMode.value = true
  Object.assign(scheduleForm, schedule)
  scheduleDialogVisible.value = true
}

// 请假申请
const requestLeave = async (schedule: Schedule) => {
  try {
    await ElMessageBox.confirm(
      `确定要为 ${schedule.date} 的排班申请请假吗？`,
      '请假申请',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    // TODO: 调用请假申请接口
    ElMessage.success('请假申请已提交，等待审核')
  } catch {
    // 用户取消
  }
}

// 删除排班
const deleteSchedule = async (schedule: Schedule) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除 ${schedule.date} 的排班吗？`,
      '确认删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    // TODO: 调用删除排班接口
    const index = scheduleList.value.findIndex(s => s.id === schedule.id)
    if (index !== -1) {
      scheduleList.value.splice(index, 1)
      ElMessage.success('排班删除成功')
    }
  } catch {
    // 用户取消
  }
}

// 提交排班表单
const handleScheduleSubmit = async () => {
  if (!scheduleFormRef.value) return
  
  try {
    await scheduleFormRef.value.validate()
    
    // TODO: 调用保存排班接口
    if (isEditMode.value) {
      ElMessage.success('排班更新成功')
    } else {
      ElMessage.success('排班添加成功')
    }
    
    scheduleDialogVisible.value = false
  } catch {
    ElMessage.error('表单验证失败')
  }
}

// 重置排班表单
const resetScheduleForm = () => {
  Object.assign(scheduleForm, {
    id: 0,
    date: '',
    startTime: '',
    endTime: '',
    type: 'normal',
    remark: ''
  })
  scheduleFormRef.value?.clearValidate()
}

// 组件挂载时加载数据
onMounted(() => {
  // TODO: 加载医生排班数据
  // TODO: 加载排班统计数据
})
</script>

<style scoped>
/**
 * 排班管理页面样式
 */

.schedule-management {
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

.stat-icon.work-days {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.stat-icon.work-hours {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
}

.stat-icon.appointments {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
}

.stat-icon.leave-days {
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

/* 日历卡片 */
.calendar-card {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

/* 排班日历 */
.schedule-calendar {
  border: 1px solid #ebeef5;
  border-radius: 4px;
  overflow: hidden;
}

.calendar-header {
  display: flex;
  background-color: #f5f7fa;
  border-bottom: 1px solid #ebeef5;
}

.time-column {
  width: 80px;
  padding: 10px;
  text-align: center;
  font-weight: bold;
  border-right: 1px solid #ebeef5;
}

.day-column {
  flex: 1;
  padding: 10px;
  text-align: center;
  border-right: 1px solid #ebeef5;
}

.day-column:last-child {
  border-right: none;
}

.day-column.today {
  background-color: #e6f7ff;
  color: #1890ff;
}

.day-name {
  font-weight: bold;
  margin-bottom: 4px;
}

.day-date {
  font-size: 12px;
  color: #666;
}

.calendar-body {
  max-height: 600px;
  overflow-y: auto;
}

.time-row {
  display: flex;
  border-bottom: 1px solid #ebeef5;
}

.time-cell {
  width: 80px;
  padding: 8px;
  text-align: center;
  border-right: 1px solid #ebeef5;
  font-size: 12px;
  color: #666;
}

.schedule-cell {
  flex: 1;
  height: 40px;
  border-right: 1px solid #ebeef5;
  cursor: pointer;
  position: relative;
  transition: background-color 0.2s;
}

.schedule-cell:hover {
  background-color: #f0f9ff;
}

.schedule-cell.scheduled {
  background-color: #e6f7ff;
}

.schedule-cell.scheduled.normal {
  background-color: #f6ffed;
}

.schedule-cell.scheduled.night {
  background-color: #fff7e6;
}

.schedule-cell.scheduled.emergency {
  background-color: #fff2f0;
}

.schedule-cell.scheduled.duty {
  background-color: #f0f5ff;
}

.schedule-info {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-size: 10px;
  color: #333;
  white-space: nowrap;
}

/* 排班列表卡片 */
.schedule-list-card {
  margin-bottom: 20px;
}

/* 模板对话框 */
.template-content {
  text-align: center;
  padding: 20px;
}
</style>