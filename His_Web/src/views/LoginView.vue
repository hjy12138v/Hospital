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
        <h4>示例账号</h4>
        <div class="account-list">
          <div class="account-item" @click="fillAccount('赵子龙', '50000005')">
            <el-tag type="danger">管理员</el-tag>
            <span>赵子龙 / 50000005</span>
          </div>
          <div class="account-item" @click="fillAccount('李浩然', '30000003')">
            <el-tag type="warning">医生</el-tag>
            <span>李浩然 / 30000003</span>
          </div>
          <div class="account-item" @click="fillAccount('张晨曦', '20000002')">
            <el-tag type="success">用户</el-tag>
            <span>张晨曦 / 20000002</span>
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
import http from '@/api/http'

const router = useRouter()
const authStore = useAuthStore()

const loginFormRef = ref<FormInstance>()
const loading = ref(false)

const loginForm = reactive({
  username: '',
  password: '',
})

const loginRules = {
  username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, message: '密码长度不能少于6位', trigger: 'blur' },
  ],
}

const fillAccount = (username: string, password: string) => {
  loginForm.username = username
  loginForm.password = password
}

const handleLogin = async () => {
  if (!loginFormRef.value) return

  try {
    await loginFormRef.value.validate()
    loading.value = true

    // 调用后端登录验证接口（返回 UserDto）
    const res = await http.post('http://localhost:5090/api/User/login', {
      name: loginForm.username,
      password: loginForm.password,
    })

    // 规范化后的字段：id, name, gender, phoneNumber, email, roleId, dateOfBirth
    const roleId: number = Number((res as any).roleId ?? 0)
    const role = roleId === 3 ? 'admin' : roleId === 2 ? 'doctor' : 'user'

    // 保存登录状态（无后端token，使用占位）
    authStore.login(
      {
        id: (res as any).id,
        username: (res as any).name,
        role,
        name: (res as any).name,
        email: (res as any).email ?? '',
        phone: (res as any).phoneNumber ?? '',
      },
      'session',
    )

    ElMessage.success('登录成功')

    // 根据角色跳转到不同页面
    if (role === 'admin') {
      router.push('/admin/dashboard')
    } else if (role === 'doctor') {
      router.push('/doctor/dashboard')
    } else {
      router.push('/user/dashboard')
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
