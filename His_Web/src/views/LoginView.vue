<template>
  <div class="login-container">
    <div class="login-box">
      <div class="login-header">
        <h2>医院便民服务系统</h2>
        <p>Hospital Convenience Service System</p>
      </div>

      <el-form
        ref="loginFormRef"
        :model="loginForm"
        :rules="loginRules"
        class="login-form"
        @submit.prevent="handleLogin"
      >
        <el-form-item prop="username">
          <el-input
            v-model="loginForm.username"
            placeholder="请输入用户名"
            size="large"
            :prefix-icon="User"
          />
        </el-form-item>

        <el-form-item prop="password">
          <el-input
            v-model="loginForm.password"
            type="password"
            placeholder="请输入密码"
            size="large"
            :prefix-icon="Lock"
            show-password
            @keyup.enter="handleLogin"
          />
        </el-form-item>

        <el-form-item prop="role">
          <el-select
            v-model="loginForm.role"
            placeholder="请选择登录身份"
            size="large"
            style="width: 100%"
          >
            <el-option label="管理员" value="admin" />
            <el-option label="医生" value="doctor" />
            <el-option label="患者" value="user" />
          </el-select>
        </el-form-item>

        <el-form-item>
          <el-button
            type="primary"
            size="large"
            style="width: 100%"
            :loading="loading"
            @click="handleLogin"
          >
            登录
          </el-button>
        </el-form-item>
      </el-form>

      <div class="demo-accounts">
        <h4>演示账号</h4>
        <div class="account-list">
          <div class="account-item" @click="fillAccount('admin', '123456')">
            <el-tag type="danger">管理员</el-tag>
            <span>admin / 123456</span>
          </div>
          <div class="account-item" @click="fillAccount('doctor1', '123456')">
            <el-tag type="warning">医生</el-tag>
            <span>doctor1 / 123456</span>
          </div>
          <div class="account-item" @click="fillAccount('user1', '123456')">
            <el-tag type="success">患者</el-tag>
            <span>user1 / 123456</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, type FormInstance } from 'element-plus'
import { User, Lock } from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'
import { authAPI } from '@/api'

const router = useRouter()
const authStore = useAuthStore()

const loginFormRef = ref<FormInstance>()
const loading = ref(false)

const loginForm = reactive({
  username: '',
  password: '',
  role: ''
})

const loginRules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, message: '密码长度不能少于6位', trigger: 'blur' }
  ],
  role: [
    { required: true, message: '请选择登录身份', trigger: 'change' }
  ]
}

const fillAccount = (username: string, password: string) => {
  loginForm.username = username
  loginForm.password = password

  // 根据用户名自动选择角色
  if (username === 'admin') {
    loginForm.role = 'admin'
  } else if (username.startsWith('doctor')) {
    loginForm.role = 'doctor'
  } else if (username.startsWith('user')) {
    loginForm.role = 'user'
  }
}

const handleLogin = async () => {
  if (!loginFormRef.value) return

  try {
    await loginFormRef.value.validate()
    loading.value = true

    const response = await authAPI.login(loginForm.username, loginForm.password)

    // 验证角色是否匹配
    if (response.data.user.role !== loginForm.role) {
      ElMessage.error('登录身份与账号角色不匹配')
      return
    }

    // 保存登录状态
    authStore.login(response.data.user, response.data.token)

    ElMessage.success('登录成功')

    // 根据角色跳转到不同页面
    switch (response.data.user.role) {
      case 'admin':
        router.push('/admin/dashboard')
        break
      case 'doctor':
        router.push('/doctor/dashboard')
        break
      case 'user':
        router.push('/user/dashboard')
        break
      default:
        router.push('/')
    }
  } catch (error: any) {
    ElMessage.error(error.message || '登录失败')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
/* 背景色柔和的蓝绿色渐变，增加轻微噪点提升质感 */
.login-container {
  min-height: 100vh;
  /* 柔和蓝绿色渐变，更符合医疗系统的专业、舒适调性 */
  background: linear-gradient(120deg, #e0f7fa 0%, #b2ebf2 50%, #80deea 100%);
  /* 增加极轻微的噪点，提升视觉质感，避免单调 */
  background-image:
    linear-gradient(120deg, #e0f7fa 0%, #b2ebf2 50%, #80deea 100%),
    url("data:image/svg+xml,%3Csvg width='100' height='100' viewBox='0 0 100 100' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M11 c3.866 0 7-3.134 7-7s-3.134-7-7-7-7 3.134-7 7 3.134 7 7 7zm48 25c3.866 0 7-3.134 7-7s-3.134-7-7-7-7 3.134-7 7 3.134 7 7 7zm-43-7c1.657 0 3-1.343 3-3s-1.343-3-3-3-3 1.343-3 3 1.343 3 3 3zm63 31c1.657 0 3-1.343 3-3s-1.343-3-3-3-3 1.343-3 3 1.343 3 3 3zM34 90c1.657 0 3-1.343 3-3s-1.343-3-3-3-3 1.343-3 3 1.343 3 3 3zm56-76c1.657 0 3-1.343 3-3s-1.343-3-3-3-3 1.343-3 3 1.343 3 3 3zM12 86c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm28-65c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm23-11c2.76 0 5-2.24 5-5s-2.24-5-5-5-5 2.24-5 5 2.24 5 5 5zm-6 60c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm29 22c2.76 0 5-2.24 5-5s-2.24-5-5-5-5 2.24-5 5 2.24 5 5 5zM32 63c2.76 0 5-2.24 5-5s-2.24-5-5-5-5 2.24-5 5 2.24 5 5 5zm57-13c2.76 0 5-2.24 5-5s-2.24-5-5-5-5 2.24-5 5 2.24 5 5 5zm-9-21c1.105 0 2-.895 2-2s-.895-2-2-2-2 .895-2 2 .895 2 2 2zM60 91c1.105 0 2-.895 2-2s-.895-2-2-2-2 .895-2 2 .895 2 2 2zM35 41c1.105 0 2-.895 2-2s-.895-2-2-2-2 .895-2 2 .895 2 2 2zM12 60c1.105 0 2-.895 2-2s-.895-2-2-2-2 .895-2 2 .895 2 2 2z' fill='%23ffffff' fill-opacity='0.05' fill-rule='evenodd'/%3E%3C/svg%3E");
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  /* 增加背景模糊，提升层次感 */
  backdrop-filter: blur(8px);
}

.login-box {
  background: white;
  border-radius: 12px;
  padding: 40px;
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.08); /* 降低阴影透明度，更柔和 */
  width: 100%;
  max-width: 400px;
  /* 增加轻微透明感，与背景呼应 */
  opacity: 0.98;
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
}

.login-header h2 {
  color: #333;
  margin-bottom: 8px;
  font-size: 24px;
  font-weight: 600;
}

.login-header p {
  color: #666;
  font-size: 14px;
  margin: 0;
}

.login-form {
  margin-bottom: 20px;
}

.demo-accounts {
  border-top: 1px solid #eee;
  padding-top: 20px;
}

.demo-accounts h4 {
  margin: 0 0 15px 0;
  color: #666;
  font-size: 14px;
  text-align: center;
}

.account-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.account-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 12px;
  background: #f8f9fa;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.2s;
  font-size: 13px;
}

.account-item:hover {
  background: #e9ecef;
}

.account-item span {
  color: #666;
}
</style>
