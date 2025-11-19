<template>
  <div class="notice-management">
    <div class="page-header">
      <h2>通知管理</h2>
      <el-button type="primary" @click="showAddDialog">
        <el-icon><Plus /></el-icon>
        发布通知
      </el-button>
    </div>
    
    <!-- 搜索栏 -->
    <el-card class="search-card">
      <el-form :model="searchForm" inline>
        <el-form-item label="通知标题">
          <el-input
            v-model="searchForm.title"
            placeholder="请输入通知标题"
            clearable
          />
        </el-form-item>
        <el-form-item label="通知类型">
          <el-select
            v-model="searchForm.type"
            placeholder="请选择通知类型"
            clearable
          >
            <el-option label="通知" value="info" />
            <el-option label="重要" value="warning" />
            <el-option label="公告" value="success" />
            <el-option label="紧急" value="error" />
          </el-select>
        </el-form-item>
        <el-form-item label="发布状态">
          <el-select
            v-model="searchForm.isPublished"
            placeholder="请选择发布状态"
            clearable
          >
            <el-option label="已发布" :value="true" />
            <el-option label="未发布" :value="false" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">搜索</el-button>
          <el-button @click="resetSearch">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 通知列表 -->
    <el-card>
      <el-table
        v-loading="loading"
        :data="filteredNotices"
        style="width: 100%"
      >
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="title" label="通知标题" />
        <el-table-column prop="type" label="类型" width="100">
          <template #default="{ row }">
            <el-tag
              :type="getNoticeType(row.type)"
              size="small"
            >
              {{ getNoticeTypeText(row.type) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="publishTime" label="发布时间" width="180">
          <template #default="{ row }">
            <span class="publish-time">{{ formatTime(row.publishTime) }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="isPublished" label="状态" width="100">
          <template #default="{ row }">
            <el-tag
              :type="row.isPublished ? 'success' : 'warning'"
              size="small"
            >
              {{ row.isPublished ? '已发布' : '未发布' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="160">
          <template #default="{ row }">
            <div class="actions">
              <el-button
                type="primary"
                size="small"
                @click="showViewDialog(row)"
              >
                查看
              </el-button>
              <el-dropdown @command="(cmd: string) => onActionCommand(row, cmd)">
                <el-button size="small">
                  更多操作
                  <el-icon style="margin-left:4px"><ArrowDown /></el-icon>
                </el-button>
                <template #dropdown>
                  <el-dropdown-menu>
                    <el-dropdown-item command="edit">编辑</el-dropdown-item>
                    <el-dropdown-item :command="row.isPublished ? 'unpublish' : 'publish'">
                      {{ row.isPublished ? '取消发布' : '发布' }}
                    </el-dropdown-item>
                    <el-dropdown-item command="delete" divided>删除</el-dropdown-item>
                  </el-dropdown-menu>
                </template>
              </el-dropdown>
            </div>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    
    <!-- 添加/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="isEdit ? '编辑通知' : '发布通知'"
      width="800px"
    >
      <el-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-width="80px"
      >
        <el-form-item label="通知标题" prop="title">
          <el-input v-model="form.title" placeholder="请输入通知标题" />
        </el-form-item>
        <el-form-item label="通知类型" prop="type">
          <el-select
            v-model="form.type"
            placeholder="请选择通知类型"
            style="width: 100%"
          >
            <el-option label="通知" value="info" />
            <el-option label="重要" value="warning" />
            <el-option label="公告" value="success" />
            <el-option label="紧急" value="error" />
          </el-select>
        </el-form-item>
        <el-form-item label="通知内容" prop="content">
          <el-input
            v-model="form.content"
            type="textarea"
            placeholder="请输入通知内容"
            :rows="8"
          />
        </el-form-item>
        <el-form-item label="立即发布">
          <el-switch v-model="form.isPublished" />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">确定</el-button>
      </template>
    </el-dialog>
    
    <!-- 查看通知对话框 -->
    <el-dialog
      v-model="viewDialogVisible"
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
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance } from 'element-plus'
import { Plus, ArrowDown } from '@element-plus/icons-vue'
import { noticeAPI } from '@/api/resources'
import type { Notice } from '@/types/models'
import dayjs from 'dayjs'

const loading = ref(false)
const dialogVisible = ref(false)
const viewDialogVisible = ref(false)
const isEdit = ref(false)
const formRef = ref<FormInstance>()

const notices = ref<Notice[]>([])
const selectedNotice = ref<Notice | null>(null)

const searchForm = reactive({
  title: '',
  type: '',
  isPublished: null as boolean | null
})

const form = reactive({
  id: 0,
  title: '',
  content: '',
  type: 'info',
  isPublished: true
})

const rules = {
  title: [
    { required: true, message: '请输入通知标题', trigger: 'blur' }
  ],
  content: [
    { required: true, message: '请输入通知内容', trigger: 'blur' }
  ],
  type: [
    { required: true, message: '请选择通知类型', trigger: 'change' }
  ]
}

// 过滤后的通知列表
const filteredNotices = computed(() => {
  return notices.value.filter(notice => {
    const titleMatch = !searchForm.title || notice.title.includes(searchForm.title)
    const typeMatch = !searchForm.type || notice.type === searchForm.type
    const publishMatch = searchForm.isPublished === null || notice.isPublished === searchForm.isPublished
    return titleMatch && typeMatch && publishMatch
  })
})

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

const formatTime = (value: string | number | Date | undefined) => {
  if (!value) return '-'
  const d = dayjs(value)
  if (!d.isValid()) return '-'
  return d.format('YYYY-MM-DD HH:mm')
}

const loadNotices = async () => {
  try {
    loading.value = true
    const response = await noticeAPI.getNotices()
    notices.value = response.data
  } catch (error) {
    ElMessage.error('加载通知列表失败')
  } finally {
    loading.value = false
  }
}

const showAddDialog = () => {
  isEdit.value = false
  resetForm()
  dialogVisible.value = true
}

const showEditDialog = (row: Notice) => {
  isEdit.value = true
  Object.assign(form, row)
  dialogVisible.value = true
}

const showViewDialog = (row: Notice) => {
  selectedNotice.value = row
  viewDialogVisible.value = true
}

const resetForm = () => {
  Object.assign(form, {
    id: 0,
    title: '',
    content: '',
    type: 'info',
    isPublished: true
  })
  formRef.value?.clearValidate()
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    
    if (isEdit.value) {
      await noticeAPI.updateNotice(form.id, form)
      ElMessage.success('更新成功')
    } else {
      await noticeAPI.addNotice(form)
      ElMessage.success('发布成功')
    }
    
    dialogVisible.value = false
    loadNotices()
  } catch (err: unknown) {
    const message = err instanceof Error ? err.message : '操作失败'
    ElMessage.error(message)
  }
}

const togglePublish = async (row: Notice) => {
  try {
    const action = row.isPublished ? '取消发布' : '发布'
    await ElMessageBox.confirm(
      `确定要${action}通知 "${row.title}" 吗？`,
      `确认${action}`,
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await noticeAPI.updateNotice(row.id, {
      ...row,
      isPublished: !row.isPublished
    })
    
    ElMessage.success(`${action}成功`)
    loadNotices()
  } catch (err: unknown) {
    if (err !== 'cancel') {
      const message = err instanceof Error ? err.message : '操作失败'
      ElMessage.error(message)
    }
  }
}

const onActionCommand = async (row: Notice, cmd: string) => {
  switch (cmd) {
    case 'edit':
      showEditDialog(row)
      break
    case 'publish':
      await togglePublish({ ...row, isPublished: false })
      break
    case 'unpublish':
      await togglePublish({ ...row, isPublished: true })
      break
    case 'delete':
      await handleDelete(row)
      break
  }
}

const handleDelete = async (row: Notice) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除通知 "${row.title}" 吗？`,
      '确认删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await noticeAPI.deleteNotice(row.id)
    ElMessage.success('删除成功')
    loadNotices()
  } catch (err: unknown) {
    if (err !== 'cancel') {
      const message = err instanceof Error ? err.message : '删除失败'
      ElMessage.error(message)
    }
  }
}

const handleSearch = () => {
  // 搜索功能通过计算属性实现
}

const resetSearch = () => {
  searchForm.title = ''
  searchForm.type = ''
  searchForm.isPublished = null
}

onMounted(() => {
  loadNotices()
})
</script>

<style scoped>
.notice-management {
  padding: 0;
}

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

.search-card {
  margin-bottom: 20px;
}

.search-card .el-form {
  margin-bottom: 0;
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

.publish-time {
  color: #606266;
}

.actions {
  display: flex;
  gap: 8px;
  align-items: center;
}

.detail-content {
  color: #606266;
  line-height: 1.8;
  white-space: pre-wrap;
}
</style>