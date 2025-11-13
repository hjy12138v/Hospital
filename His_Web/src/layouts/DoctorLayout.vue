<!--
  医生工作台布局组件
  
  功能说明：
  - 提供医生工作台的整体布局结构
  - 包含医生专用的导航菜单和功能模块
  - 显示医生个人信息和科室信息
  - 支持医生日常工作流程管理
  
  组件结构：
  - 左侧：医生专用导航菜单（工作台、患者管理、预约管理等）
  - 顶部：面包屑导航 + 医生信息展示
  - 主体：医生工作相关的页面内容
  
  权限说明：
  - 仅医生角色可以访问
  - 显示医生所属科室信息
  - 提供医生日常工作所需的功能入口
-->

<template>
  <el-container class="doctor-layout">
    <!-- 左侧导航栏 -->
    <el-aside width="250px" class="sidebar">
      <!-- Logo 区域 -->
      <div class="logo">
        <h3>医生工作台</h3>
      </div>
      
      <!-- 医生专用导航菜单 -->
      <el-menu
        :default-active="$route.path"
        class="sidebar-menu"
        router
        background-color="#2c3e50"
        text-color="#bfcbd9"
        active-text-color="#409EFF"
      >
        <!-- 工作台 - 医生日常工作概览 -->
        <el-menu-item index="/doctor/dashboard">
          <el-icon><House /></el-icon>
          <span>工作台</span>
        </el-menu-item>
        
        <!-- 患者管理 - 查看和管理患者信息 -->
        <el-menu-item index="/doctor/patients">
          <el-icon><UserFilled /></el-icon>
          <span>患者管理</span>
        </el-menu-item>
        
        <!-- 预约管理 - 处理患者预约请求 -->
        <el-menu-item index="/doctor/appointments">
          <el-icon><Calendar /></el-icon>
          <span>预约管理</span>
        </el-menu-item>
        
        <!-- 排班管理 - 医生个人排班设置 -->
        <el-menu-item index="/doctor/schedule">
          <el-icon><Clock /></el-icon>
          <span>排班管理</span>
        </el-menu-item>
        
        <!-- 回访管理 - 患者回访和跟踪 -->
        <el-menu-item index="/doctor/follow-up">
          <el-icon><ChatDotRound /></el-icon>
          <span>回访管理</span>
        </el-menu-item>
      </el-menu>
    </el-aside>
    
    <!-- 右侧主体区域 -->
    <el-container>
      <!-- 顶部导航栏 -->
      <el-header class="header">
        <!-- 左侧：面包屑导航 -->
        <div class="header-left">
          <el-breadcrumb separator="/">
            <el-breadcrumb-item>医生</el-breadcrumb-item>
            <el-breadcrumb-item>{{ getBreadcrumbTitle() }}</el-breadcrumb-item>
          </el-breadcrumb>
        </div>
        
        <!-- 右侧：医生信息和操作 -->
        <div class="header-right">
          <!-- 医生信息下拉菜单 -->
          <el-dropdown @command="handleCommand">
            <span class="user-info">
              <el-icon><Avatar /></el-icon>
              {{ authStore.user?.name }}
              <!-- 显示医生所属科室 -->
              <el-tag type="warning" size="small" style="margin-left: 8px;">
                {{ authStore.user?.department }}
              </el-tag>
              <el-icon class="el-icon--right"><ArrowDown /></el-icon>
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="profile">个人信息</el-dropdown-item>
                <el-dropdown-item command="logout" divided>退出登录</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </el-header>
      
      <!-- 主内容区域 -->
      <!-- 这里显示医生工作相关的页面内容 -->
      <el-main class="main-content">
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup lang="ts">
/**
 * 医生布局组件逻辑
 * 
 * 主要功能：
 * - 动态生成医生页面的面包屑导航
 * - 处理医生用户操作（个人信息、退出登录）
 * - 显示医生专属信息（科室等）
 * - 管理医生工作台的导航状态
 */

import { useRouter, useRoute } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  House,
  UserFilled,
  Calendar,
  Clock,
  ChatDotRound,
  Avatar,
  ArrowDown
} from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'

// 路由相关
const router = useRouter()
const route = useRoute()

// 认证状态管理
const authStore = useAuthStore()

/**
 * 获取医生页面面包屑导航标题
 * 
 * 根据当前路由路径返回对应的医生功能页面标题
 * 
 * @returns {string} 当前医生页面的中文标题
 */
const getBreadcrumbTitle = () => {
  const routeMap: Record<string, string> = {
    '/doctor/dashboard': '工作台',
    '/doctor/patients': '患者管理',
    '/doctor/appointments': '预约管理',
    '/doctor/schedule': '排班管理',
    '/doctor/follow-up': '回访管理'
  }
  return routeMap[route.path] || '未知页面'
}

/**
 * 处理医生用户操作命令
 * 
 * 处理医生下拉菜单中的各种操作
 * 包括查看个人信息、退出登录等
 * 
 * @param {string} command 操作命令类型
 */
const handleCommand = async (command: string) => {
  switch (command) {
    case 'profile':
      // TODO: 实现医生个人信息页面
      ElMessage.info('个人信息功能开发中...')
      break
      
    case 'logout':
      try {
        // 确认退出操作
        await ElMessageBox.confirm('确定要退出登录吗？', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        })
        
        // 执行退出登录
        authStore.logout()
        
        // 跳转到登录页面
        router.push('/login')
        
        // 显示成功提示
        ElMessage.success('已退出登录')
      } catch {
        // 用户取消操作，不做任何处理
      }
      break
  }
}
</script>

<style scoped>
/**
 * 医生布局样式
 * 
 * 设计理念：
 * - 采用深绿色调，体现医疗专业性
 * - 与管理员布局保持一致的结构
 * - 突出医生工作的专业性和严谨性
 */

/* 整体布局容器 */
.doctor-layout {
  height: 100vh; /* 占满整个视口高度 */
}

/* 侧边栏样式 */
.sidebar {
  background-color: #2c3e50; /* 深绿灰色背景，体现医疗专业性 */
  overflow: hidden; /* 隐藏溢出内容 */
}

/* Logo 区域样式 */
.logo {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #233240; /* 比侧边栏稍深的背景色 */
  color: white;
  margin-bottom: 0;
}

.logo h3 {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
}

/* 侧边栏菜单样式 */
.sidebar-menu {
  border: none; /* 移除默认边框 */
  height: calc(100vh - 60px); /* 减去 Logo 区域高度 */
  overflow-y: auto; /* 菜单项过多时显示滚动条 */
}

.sidebar-menu .el-menu-item {
  height: 50px; /* 菜单项高度 */
  line-height: 50px; /* 文字垂直居中 */
}

/* 顶部导航栏样式 */
.header {
  background: white;
  border-bottom: 1px solid #e4e7ed; /* 底部分割线 */
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 20px;
}

.header-left {
  flex: 1; /* 占据剩余空间 */
}

.header-right {
  display: flex;
  align-items: center;
}

/* 医生信息区域样式 */
.user-info {
  display: flex;
  align-items: center;
  cursor: pointer;
  padding: 8px 12px;
  border-radius: 4px;
  transition: background-color 0.2s; /* 悬停效果过渡 */
}

.user-info:hover {
  background-color: #f5f7fa; /* 悬停时的背景色 */
}

.user-info .el-icon {
  margin-right: 8px;
}

.user-info .el-icon--right {
  margin-left: 8px;
  margin-right: 0;
}

/* 主内容区域样式 */
.main-content {
  background-color: #f0f2f5; /* 浅灰色背景 */
  padding: 20px; /* 内边距 */
}
</style>