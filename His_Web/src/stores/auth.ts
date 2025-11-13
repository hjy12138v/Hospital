/**
 * 用户认证状态管理 Store
 * 
 * 功能说明：
 * - 管理用户登录状态
 * - 存储用户信息和 token
 * - 提供登录、登出、权限检查等功能
 * - 支持多角色权限控制（管理员、医生、患者）
 * 
 * 使用 Pinia 进行状态管理，支持 TypeScript 类型检查
 */

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

/**
 * 用户信息接口定义
 * 
 * 包含用户的基本信息和角色权限
 */
export interface User {
  id: number           // 用户唯一标识
  username: string     // 用户名（登录用）
  role: 'admin' | 'doctor' | 'user'  // 用户角色：管理员/医生/患者
  name: string         // 真实姓名
  email?: string       // 邮箱（可选）
  phone?: string       // 电话（可选）
  department?: string  // 科室（医生专用）
}

/**
 * 认证状态管理 Store
 * 
 * 使用 Composition API 风格的 Pinia Store
 */
export const useAuthStore = defineStore('auth', () => {
  // ==================== 状态定义 ====================
  
  /**
   * 当前登录用户信息
   * 初始值为 null，表示未登录状态
   */
  const user = ref<User | null>(null)
  
  /**
   * 用户认证 token
   * 从 localStorage 中读取，支持页面刷新后保持登录状态
   */
  const token = ref<string | null>(localStorage.getItem('token'))

  // ==================== 计算属性 ====================
  
  /**
   * 检查用户是否已登录
   * 同时检查 user 和 token 是否存在
   */
  const isLoggedIn = computed(() => !!user.value && !!token.value)
  
  /**
   * 获取当前用户角色
   * 返回用户角色或 undefined
   */
  const userRole = computed(() => user.value?.role)
  
  /**
   * 检查是否为管理员
   * 管理员拥有最高权限，可以管理所有模块
   */
  const isAdmin = computed(() => user.value?.role === 'admin')
  
  /**
   * 检查是否为医生
   * 医生可以查看患者信息、管理预约等
   */
  const isDoctor = computed(() => user.value?.role === 'doctor')
  
  /**
   * 检查是否为普通用户（患者）
   * 患者只能查看自己的信息和进行预约
   */
  const isUser = computed(() => user.value?.role === 'user')

  // ==================== 方法定义 ====================
  
  /**
   * 用户登录方法
   * 
   * @param userData 用户信息
   * @param authToken 认证 token
   * 
   * 功能：
   * - 保存用户信息到状态中
   * - 保存 token 到 localStorage
   * - 持久化用户信息到 localStorage
   */
  const login = (userData: User, authToken: string) => {
    user.value = userData
    token.value = authToken
    
    // 持久化存储，支持页面刷新后保持登录状态
    localStorage.setItem('token', authToken)
    localStorage.setItem('user', JSON.stringify(userData))
  }

  /**
   * 用户登出方法
   * 
   * 功能：
   * - 清空状态中的用户信息
   * - 清除 localStorage 中的认证信息
   * - 重置所有相关状态
   */
  const logout = () => {
    user.value = null
    token.value = null
    
    // 清除持久化存储
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  /**
   * 初始化认证状态
   * 
   * 功能：
   * - 从 localStorage 恢复用户登录状态
   * - 在应用启动时调用，支持页面刷新后保持登录
   * - 验证存储的数据有效性
   */
  const initAuth = () => {
    const savedUser = localStorage.getItem('user')
    const savedToken = localStorage.getItem('token')
    
    // 检查本地存储中是否有有效的认证信息
    if (savedUser && savedToken) {
      try {
        // 解析用户信息，如果解析失败则清除无效数据
        const parsedUser = JSON.parse(savedUser)
        user.value = parsedUser
        token.value = savedToken
      } catch (error) {
        // 如果用户数据格式错误，清除所有认证信息
        console.error('用户数据格式错误，清除认证信息:', error)
        logout()
      }
    }
  }

  // ==================== 返回 Store 接口 ====================
  
  return {
    // 状态
    user,
    token,
    
    // 计算属性
    isLoggedIn,
    userRole,
    isAdmin,
    isDoctor,
    isUser,
    
    // 方法
    login,
    logout,
    initAuth
  }
})