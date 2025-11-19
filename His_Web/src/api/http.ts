import axios from 'axios'
import { unwrapAndNormalize } from './transform'

// 统一 HTTP 客户端
// - 从环境变量读取后端基础地址：`VITE_API_BASE_URL`
// - 自动附加 Authorization 头（如果存在 token）
// - 响应统一返回原始 data，便于在 API 模块中封装为 { data }

const baseURL = import.meta.env.VITE_API_BASE_URL || '/'

export const http = axios.create({
  baseURL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

http.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers = config.headers || {}
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error),
)

http.interceptors.response.use(
  (response) => unwrapAndNormalize(response.data),
  (error) => {
    // 可选：在此处处理 401/403 等通用错误
    return Promise.reject(error)
  },
)

export default http