<template>
  <div class="notice-view">
    <h2>通知公告</h2>
    
    <el-card>
      <div class="notice-list">
        <div
          v-for="notice in notices"
          :key="notice.id"
          class="notice-item"
          @click="showNoticeDetail(notice)"
        >
          <div class="notice-header">
            <h3 class="notice-title">{{ notice.title }}</h3>
            <el-tag
              :type="getNoticeType(notice.type)"
              size="small"
            >
              {{ getNoticeTypeText(notice.type) }}
            </el-tag>
          </div>
          <div class="notice-content">
            {{ notice.content.substring(0, 100) }}...
          </div>
          <div class="notice-footer">
            <span class="notice-time">{{ notice.publishTime }}</span>
          </div>
        </div>
        
        <div v-if="notices.length === 0" class="no-data">
          暂无通知公告
        </div>
      </div>
    </el-card>
    
    <!-- 通知详情对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="selectedNotice?.title"
      width="600px"
    >
      <div v-if="selectedNotice" class="notice-detail">
        <div class="detail-header">
          <el-tag
            :type="getNoticeType(selectedNotice.type)"
            size="small"
          >
            {{ getNoticeTypeText(selectedNotice.type) }}
          </el-tag>
          <span class="detail-time">{{ selectedNotice.publishTime }}</span>
        </div>
        <div class="detail-content">
          {{ selectedNotice.content }}
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { noticeAPI } from '@/api/resources'

const notices = ref<any[]>([])
const dialogVisible = ref(false)
const selectedNotice = ref<any>(null)

const getNoticeType = (type: string) => {
  switch (type) {
    case 'info':
      return 'primary'
    case 'warning':
      return 'warning'
    case 'success':
      return 'success'
    case 'error':
      return 'danger'
    default:
      return 'info'
  }
}

const getNoticeTypeText = (type: string) => {
  switch (type) {
    case 'info':
      return '通知'
    case 'warning':
      return '重要'
    case 'success':
      return '公告'
    case 'error':
      return '紧急'
    default:
      return '通知'
  }
}

const showNoticeDetail = (notice: any) => {
  selectedNotice.value = notice
  dialogVisible.value = true
}

const loadNotices = async () => {
  try {
    const response = await noticeAPI.getNotices()
    notices.value = response.data.filter((notice: any) => notice.isPublished)
  } catch (error) {
    console.error('加载通知列表失败:', error)
  }
}

onMounted(() => {
  loadNotices()
})
</script>

<style scoped>
.notice-view h2 {
  margin: 0 0 20px 0;
  color: #303133;
}

.notice-list {
  max-height: 600px;
  overflow-y: auto;
}

.notice-item {
  padding: 20px;
  border-bottom: 1px solid #f0f0f0;
  cursor: pointer;
  transition: background-color 0.2s;
}

.notice-item:hover {
  background-color: #f8f9fa;
}

.notice-item:last-child {
  border-bottom: none;
}

.notice-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.notice-title {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: #303133;
}

.notice-content {
  color: #606266;
  line-height: 1.6;
  margin-bottom: 10px;
}

.notice-footer {
  display: flex;
  justify-content: flex-end;
}

.notice-time {
  font-size: 12px;
  color: #909399;
}

.no-data {
  text-align: center;
  color: #909399;
  padding: 40px 0;
}

.notice-detail {
  padding: 10px 0;
}

.detail-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  padding-bottom: 10px;
  border-bottom: 1px solid #f0f0f0;
}

.detail-time {
  font-size: 14px;
  color: #909399;
}

.detail-content {
  color: #606266;
  line-height: 1.8;
  white-space: pre-wrap;
}
</style>