<template>
  <div class="profile-view">
    <h2>个人信息</h2>
    
    <el-card>
      <el-descriptions title="基本信息" :column="2" border>
        <el-descriptions-item label="姓名">
          {{ authStore.user?.name }}
        </el-descriptions-item>
        <el-descriptions-item label="用户名">
          {{ authStore.user?.username }}
        </el-descriptions-item>
        <el-descriptions-item label="联系电话">
          {{ authStore.user?.phone }}
        </el-descriptions-item>
        <el-descriptions-item label="邮箱">
          {{ authStore.user?.email }}
        </el-descriptions-item>
        <el-descriptions-item label="用户类型">
          <el-tag type="success">患者</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="注册时间">
          2024-01-01
        </el-descriptions-item>
      </el-descriptions>
      
      <div style="margin-top: 20px;">
        <el-button type="primary">编辑信息</el-button>
        <el-button>修改密码</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
</script>

<style scoped>
.profile-view h2 {
  margin: 0 0 20px 0;
  color: #303133;
}
</style>