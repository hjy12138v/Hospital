<!--
  管理员布局组件
  
  功能说明：
  - 提供管理员后台的整体布局结构
  - 包含侧边栏导航、顶部导航栏、主内容区域
  - 实现权限控制和用户信息展示
  - 支持响应式布局和主题切换
  
  组件结构：
  - 左侧：导航菜单（科室管理、医生管理等）
  - 顶部：面包屑导航 + 用户信息下拉菜单
  - 主体：路由视图内容区域
  
  权限说明：
  - 仅管理员角色可以访问
  - 通过路由守卫进行权限验证
-->

<template>
  <el-container class="admin-layout">
    <!-- 左侧导航栏 -->
    <el-aside width="250px" class="sidebar">
      <!-- Logo 区域 -->
      <div class="logo">
        <h3>管理员控制台</h3>
      </div>
      
      <!-- 导航菜单 -->
      <!-- 
        菜单配置说明：
        - router: 启用路由模式，点击菜单项自动跳转
        - default-active: 当前激活的菜单项（基于路由路径）
        - background-color: 菜单背景色
        - text-color: 菜单文字颜色
        - active-text-color: 激活菜单项的文字颜色
      -->
      <el-menu
        :default-active="$route.path"
        class="sidebar-menu"
        router
        background-color="#304156"
        text-color="#bfcbd9"
        active-text-color="#409EFF"
      >
        <!-- 仪表盘 - 数据概览和快捷操作 -->
        <el-menu-item index="/admin/dashboard">
          <el-icon><House /></el-icon>
          <span>仪表盘</span>
        </el-menu-item>
        
        <!-- 医生管理 - 医生信息的增删改查 -->
        <el-menu-item index="/admin/doctors">
          <el-icon><User /></el-icon>
          <span>医生管理</span>
        </el-menu-item>
        
        <!-- 用户管理 - 患者和系统用户管理 -->
        <el-menu-item index="/admin/users">
          <el-icon><UserFilled /></el-icon>
          <span>用户管理</span>
        </el-menu-item>
        
        <!-- 科室管理 - 医院科室信息管理 -->
        <el-menu-item index="/admin/departments">
          <el-icon><OfficeBuilding /></el-icon>
          <span>科室管理</span>
        </el-menu-item>
        
        <!-- 药品管理 - 药品库存和信息管理 -->
        <el-menu-item index="/admin/medicines">
          <el-icon><FirstAidKit /></el-icon>
          <span>药品管理</span>
        </el-menu-item>
        
        <!-- 通知管理 - 系统通知和公告发布 -->
        <el-menu-item index="/admin/notices">
          <el-icon><Bell /></el-icon>
          <span>通知管理</span>
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
            <el-breadcrumb-item>管理员</el-breadcrumb-item>
            <el-breadcrumb-item>{{ getBreadcrumbTitle() }}</el-breadcrumb-item>
          </el-breadcrumb>
        </div>
        
        <!-- 右侧：用户信息和操作 -->
        <div class="header-right">
          <!-- 用户信息下拉菜单 -->
          <el-dropdown @command="handleCommand">
            <span class="user-info">
              <el-icon><Avatar /></el-icon>
              {{ authStore.user?.name }}
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
      <!-- 这里显示各个管理页面的内容 -->
      <el-main class="main-content">
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup lang="ts">
/**
 * 管理员布局组件逻辑
 * 
 * 主要功能：
 * - 动态生成面包屑导航
 * - 处理用户操作（个人信息、退出登录）
 * - 管理用户认证状态
 */

import { useRouter, useRoute } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  House,
  User,
  UserFilled,
  OfficeBuilding,
  FirstAidKit,
  Bell,
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
 * 获取面包屑导航标题
 * 
 * 根据当前路由路径返回对应的页面标题
 * 用于顶部面包屑导航显示
 * 
 * @returns {string} 当前页面的中文标题
 */
const getBreadcrumbTitle = () => {
  const routeMap: Record<string, string> = {
    '/admin/dashboard': '仪表盘',
    '/admin/doctors': '医生管理',
    '/admin/users': '用户管理',
    '/admin/departments': '科室管理',
    '/admin/medicines': '药品管理',
    '/admin/notices': '通知管理'
  }
  return routeMap[route.path] || '未知页面'
}

/**
 * 处理用户操作命令
 * 
 * 处理用户下拉菜单中的各种操作
 * 包括查看个人信息、退出登录等
 * 
 * @param {string} command 操作命令类型
 */
const handleCommand = async (command: string) => {
  switch (command) {
    case 'profile':
      // TODO: 实现个人信息页面
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
 * 管理员布局样式
 * 
 * 设计理念：
 * - 采用深色侧边栏 + 浅色主体的经典后台布局
 * - 响应式设计，适配不同屏幕尺寸
 * - 统一的色彩搭配和间距规范
 */

/* 整体布局容器 */
.admin-layout {
  height: 100vh; /* 占满整个视口高度 */
}

/* 侧边栏样式 */
.sidebar {
  background-color: #304156; /* 深蓝灰色背景 */
  overflow: hidden; /* 隐藏溢出内容 */
}

/* Logo 区域样式 */
.logo {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #2b3a4b; /* 比侧边栏稍深的背景色 */
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

/* 用户信息区域样式 */
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