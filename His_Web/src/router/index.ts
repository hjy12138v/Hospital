/**
 * Vue Router 路由配置文件
 * 
 * 功能说明：
 * - 定义应用程序的所有路由规则
 * - 实现基于角色的权限控制
 * - 配置路由守卫进行认证检查
 * - 支持多角色用户的页面访问控制
 * 
 * 路由结构：
 * - /login: 登录页面（公开访问）
 * - /admin/*: 管理员页面（需要管理员权限）
 * - /doctor/*: 医生页面（需要医生权限）
 * - /user/*: 患者页面（需要患者权限）
 * 
 * 权限控制：
 * - 通过 meta.requiresAuth 控制是否需要登录
 * - 通过 meta.role 控制角色权限
 * - 路由守卫自动进行权限验证和重定向
 */

import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

/**
 * 路由配置
 * 
 * 每个路由对象包含：
 * - path: 路由路径
 * - name: 路由名称（用于编程式导航）
 * - component: 懒加载的组件
 * - meta: 路由元信息（权限控制等）
 * - children: 子路由（嵌套路由）
 */
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/login'
    },
    
    // 登录页面 - 公开访问
    {
      path: '/login',
      name: 'Login',
      component: () => import('@/views/LoginView.vue'),
      meta: { requiresAuth: false }
    },
    
    // 管理员模块 - 需要管理员权限
    {
      path: '/admin',
      name: 'Admin',
      component: () => import('@/layouts/AdminLayout.vue'),
      meta: { requiresAuth: true, role: 'admin' },
      children: [
        {
          path: 'dashboard',
          name: 'AdminDashboard',
          component: () => import('@/views/admin/DashboardView.vue')
        },
        {
          path: 'doctors',
          name: 'DoctorManagement',
          component: () => import('@/views/admin/DoctorManagement.vue')
        },
        {
          path: 'users',
          name: 'UserManagement',
          component: () => import('@/views/admin/UserManagement.vue')
        },
        {
          path: 'medicines',
          name: 'MedicineManagement',
          component: () => import('@/views/admin/MedicineManagement.vue')
        },
        {
          path: 'departments',
          name: 'DepartmentManagement',
          component: () => import('@/views/admin/DepartmentManagement.vue')
        },
        {
          path: 'notices',
          name: 'NoticeManagement',
          component: () => import('@/views/admin/NoticeManagement.vue')
        }
      ]
    },
    
    // 医生模块 - 需要医生权限
    {
      path: '/doctor',
      name: 'Doctor',
      component: () => import('@/layouts/DoctorLayout.vue'),
      meta: { requiresAuth: true, role: 'doctor' },
      children: [
        {
          path: 'dashboard',
          name: 'DoctorDashboard',
          component: () => import('@/views/doctor/DashboardView.vue')
        },
        {
          path: 'patients',
          name: 'PatientList',
          component: () => import('@/views/doctor/PatientList.vue')
        },
        {
          path: 'appointments',
          name: 'AppointmentList',
          component: () => import('@/views/doctor/AppointmentList.vue')
        },
        {
          path: 'schedule',
          name: 'ScheduleManagement',
          component: () => import('@/views/doctor/ScheduleManagement.vue')
        },
        {
          path: 'follow-up',
          name: 'FollowUpManagement',
          component: () => import('@/views/doctor/FollowUpManagement.vue')
        }
      ]
    },
    
    // 患者模块 - 需要患者权限
    {
      path: '/user',
      name: 'User',
      component: () => import('@/layouts/UserLayout.vue'),
      meta: { requiresAuth: true, role: 'user' },
      children: [
        {
          path: 'dashboard',
          name: 'UserDashboard',
          component: () => import('@/views/user/DashboardView.vue')
        },
        {
          path: 'profile',
          name: 'UserProfile',
          component: () => import('@/views/user/ProfileView.vue')
        },
        {
          path: 'appointments',
          name: 'UserAppointments',
          component: () => import('@/views/user/AppointmentView.vue')
        },
        {
          path: 'notices',
          name: 'UserNotices',
          component: () => import('@/views/user/NoticeView.vue')
        },
        {
          path: 'follow-up',
          name: 'UserFollowUp',
          component: () => import('@/views/user/FollowUpView.vue')
        }
      ]
    }
  ]
})

/**
 * 全局前置路由守卫
 * 
 * 功能：
 * - 初始化用户认证状态
 * - 检查路由是否需要登录
 * - 验证用户角色权限
 * - 处理登录重定向逻辑
 * 
 * 执行流程：
 * 1. 初始化认证状态（从 localStorage 恢复）
 * 2. 检查目标路由是否需要认证
 * 3. 验证用户登录状态
 * 4. 检查用户角色权限
 * 5. 根据验证结果进行路由跳转或拦截
 */
router.beforeEach((to, from, next) => {
  // 获取认证状态管理实例
  const authStore = useAuthStore()
  
  // 初始化认证状态（从 localStorage 恢复用户信息）
  authStore.initAuth()
  
  // 检查目标路由是否需要认证
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth !== false)
  
  // 如果不需要认证，直接放行
  if (!requiresAuth) {
    next()
    return
  }
  
  // 检查用户是否已登录
  if (!authStore.isLoggedIn) {
    // 未登录，重定向到登录页面
    next('/login')
    return
  }
  
  // 检查角色权限
  const requiredRole = to.matched.find(record => record.meta.role)?.meta.role
  
  if (requiredRole && authStore.userRole !== requiredRole) {
    // 角色不匹配，根据用户角色重定向到对应的首页
    switch (authStore.userRole) {
      case 'admin':
        next('/admin/dashboard')
        break
      case 'doctor':
        next('/doctor/dashboard')
        break
      case 'user':
        next('/user/dashboard')
        break
      default:
        // 未知角色，重定向到登录页面
        next('/login')
    }
    return
  }
  
  // 权限验证通过，允许访问
  next()
})

export default router
