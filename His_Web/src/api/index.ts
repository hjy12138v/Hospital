import axios from 'axios'
import { mockUsers, mockDoctors, mockDepartments, mockMedicines, mockNotices, mockPatients, mockAppointments } from './mock'

// 创建axios实例
const api = axios.create({
  baseURL: '/api',
  timeout: 5000
})

// 请求拦截器
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// 响应拦截器
api.interceptors.response.use(
  (response) => {
    return response.data
  },
  (error) => {
    return Promise.reject(error)
  }
)

// 模拟API延迟
const delay = (ms: number) => new Promise(resolve => setTimeout(resolve, ms))

// 认证相关API
export const authAPI = {
  // 登录
  async login(username: string, password: string) {
    await delay(500)
    const user = mockUsers.find(u => u.username === username && u.password === password)
    if (user) {
      const { password: _, ...userInfo } = user
      const token = `mock_token_${user.id}_${Date.now()}`
      return {
        code: 200,
        message: '登录成功',
        data: {
          user: userInfo,
          token
        }
      }
    } else {
      throw new Error('用户名或密码错误')
    }
  },

  // 获取用户信息
  async getUserInfo() {
    await delay(300)
    const token = localStorage.getItem('token')
    if (token) {
      const tokenParts = token.split('_')
      if (tokenParts.length >= 3) {
        const userId = parseInt(tokenParts[2]||'0')
        if (!isNaN(userId)) {
          const user = mockUsers.find(u => u.id === userId)
          if (user) {
            const { password: _, ...userInfo } = user
            return {
              code: 200,
              data: userInfo
            }
          }
        }
      }
    }
    throw new Error('未授权')
  }
}

// 医生管理API
export const doctorAPI = {
  // 获取医生列表
  async getDoctors() {
    await delay(300)
    return {
      code: 200,
      data: mockDoctors
    }
  },

  // 添加医生
  async addDoctor(doctor: any) {
    await delay(500)
    const newDoctor = {
      id: mockDoctors.length + 1,
      ...doctor
    }
    mockDoctors.push(newDoctor)
    return {
      code: 200,
      message: '添加成功',
      data: newDoctor
    }
  },

  // 更新医生
  async updateDoctor(id: number, doctor: any) {
    await delay(500)
    const index = mockDoctors.findIndex(d => d.id === id)
    if (index !== -1) {
      mockDoctors[index] = { ...mockDoctors[index], ...doctor }
      return {
        code: 200,
        message: '更新成功',
        data: mockDoctors[index]
      }
    }
    throw new Error('医生不存在')
  },

  // 删除医生
  async deleteDoctor(id: number) {
    await delay(500)
    const index = mockDoctors.findIndex(d => d.id === id)
    if (index !== -1) {
      mockDoctors.splice(index, 1)
      return {
        code: 200,
        message: '删除成功'
      }
    }
    throw new Error('医生不存在')
  }
}

// 科室管理API
export const departmentAPI = {
  async getDepartments() {
    await delay(300)
    return {
      code: 200,
      data: mockDepartments
    }
  },

  async addDepartment(department: any) {
    await delay(500)
    const newDepartment = {
      id: mockDepartments.length + 1,
      ...department
    }
    mockDepartments.push(newDepartment)
    return {
      code: 200,
      message: '添加成功',
      data: newDepartment
    }
  },

  async updateDepartment(id: number, department: any) {
    await delay(500)
    const index = mockDepartments.findIndex(d => d.id === id)
    if (index !== -1) {
      mockDepartments[index] = { ...mockDepartments[index], ...department }
      return {
        code: 200,
        message: '更新成功',
        data: mockDepartments[index]
      }
    }
    throw new Error('科室不存在')
  },

  async deleteDepartment(id: number) {
    await delay(500)
    const index = mockDepartments.findIndex(d => d.id === id)
    if (index !== -1) {
      mockDepartments.splice(index, 1)
      return {
        code: 200,
        message: '删除成功'
      }
    }
    throw new Error('科室不存在')
  }
}

// 药品管理API
export const medicineAPI = {
  async getMedicines() {
    await delay(300)
    return {
      code: 200,
      data: mockMedicines
    }
  },

  async addMedicine(medicine: any) {
    await delay(500)
    const newMedicine = {
      id: mockMedicines.length + 1,
      ...medicine
    }
    mockMedicines.push(newMedicine)
    return {
      code: 200,
      message: '添加成功',
      data: newMedicine
    }
  },

  async updateMedicine(id: number, medicine: any) {
    await delay(500)
    const index = mockMedicines.findIndex(m => m.id === id)
    if (index !== -1) {
      mockMedicines[index] = { ...mockMedicines[index], ...medicine }
      return {
        code: 200,
        message: '更新成功',
        data: mockMedicines[index]
      }
    }
    throw new Error('药品不存在')
  },

  async deleteMedicine(id: number) {
    await delay(500)
    const index = mockMedicines.findIndex(m => m.id === id)
    if (index !== -1) {
      mockMedicines.splice(index, 1)
      return {
        code: 200,
        message: '删除成功'
      }
    }
    throw new Error('药品不存在')
  }
}

// 通知管理API
export const noticeAPI = {
  async getNotices() {
    await delay(300)
    return {
      code: 200,
      data: mockNotices
    }
  },

  async addNotice(notice: any) {
    await delay(500)
    const newNotice = {
      id: mockNotices.length + 1,
      publishTime: new Date().toLocaleString(),
      ...notice
    }
    mockNotices.push(newNotice)
    return {
      code: 200,
      message: '添加成功',
      data: newNotice
    }
  },

  async updateNotice(id: number, notice: any) {
    await delay(500)
    const index = mockNotices.findIndex(n => n.id === id)
    if (index !== -1) {
      mockNotices[index] = { ...mockNotices[index], ...notice }
      return {
        code: 200,
        message: '更新成功',
        data: mockNotices[index]
      }
    }
    throw new Error('通知不存在')
  },

  async deleteNotice(id: number) {
    await delay(500)
    const index = mockNotices.findIndex(n => n.id === id)
    if (index !== -1) {
      mockNotices.splice(index, 1)
      return {
        code: 200,
        message: '删除成功'
      }
    }
    throw new Error('通知不存在')
  }
}

// 患者管理API
export const patientAPI = {
  async getPatients() {
    await delay(300)
    return {
      code: 200,
      data: mockPatients
    }
  }
}

// 预约管理API
export const appointmentAPI = {
  async getAppointments() {
    await delay(300)
    return {
      code: 200,
      data: mockAppointments
    }
  },

  async addAppointment(appointment: any) {
    await delay(500)
    const newAppointment = {
      id: mockAppointments.length + 1,
      status: 'pending',
      ...appointment
    }
    mockAppointments.push(newAppointment)
    return {
      code: 200,
      message: '预约成功',
      data: newAppointment
    }
  }
}

/**
 * 医生排班管理 API
 * 
 * 包含功能：
 * - 获取排班列表
 * - 添加排班
 * - 更新排班
 * - 删除排班
 * - 请假申请
 */
export const scheduleAPI = {
  /**
   * 获取医生排班列表
   * 
   * @param params 查询参数
   * 
   * TODO: 对接后端排班列表接口
   * - 接口地址: GET /api/schedules
   * - 查询参数: { doctorId?: number, startDate?: string, endDate?: string, status?: string }
   * - 返回数据: { code: number, data: Schedule[] }
   * - 需要验证医生权限，医生只能查看自己的排班
   */
  async getSchedules(params?: any) {
    await delay(300)
    // 模拟排班数据
    const mockSchedules = [
      {
        id: 1,
        doctorId: 1,
        date: '2024-01-22',
        dayOfWeek: '周一',
        startTime: '08:00',
        endTime: '17:00',
        type: 'normal',
        status: 'confirmed',
        appointmentCount: 12,
        remark: '正常门诊'
      }
    ]
    return {
      code: 200,
      data: mockSchedules
    }
  },

  /**
   * 添加医生排班
   * 
   * @param schedule 排班信息
   * 
   * TODO: 对接后端添加排班接口
   * - 接口地址: POST /api/schedules
   * - 请求体: Schedule 对象
   * - 返回数据: { code: number, message: string, data: Schedule }
   * - 需要验证医生权限
   * - 需要检查时间冲突
   */
  async addSchedule(schedule: any) {
    await delay(500)
    return {
      code: 200,
      message: '排班添加成功',
      data: { id: Date.now(), ...schedule }
    }
  },

  /**
   * 更新医生排班
   * 
   * @param id 排班 ID
   * @param schedule 更新的排班信息
   * 
   * TODO: 对接后端更新排班接口
   * - 接口地址: PUT /api/schedules/{id}
   * - 需要验证医生权限
   * - 需要检查是否有预约冲突
   */
  async updateSchedule(id: number, schedule: any) {
    await delay(500)
    return {
      code: 200,
      message: '排班更新成功',
      data: { id, ...schedule }
    }
  },

  /**
   * 删除医生排班
   * 
   * @param id 排班 ID
   * 
   * TODO: 对接后端删除排班接口
   * - 接口地址: DELETE /api/schedules/{id}
   * - 需要验证医生权限
   * - 需要检查是否有预约
   */
  async deleteSchedule(id: number) {
    await delay(500)
    return {
      code: 200,
      message: '排班删除成功'
    }
  },

  /**
   * 医生请假申请
   * 
   * @param leaveRequest 请假申请信息
   * 
   * TODO: 对接后端请假申请接口
   * - 接口地址: POST /api/schedules/leave
   * - 需要验证医生权限
   * - 需要通知管理员审核
   */
  async requestLeave(leaveRequest: any) {
    await delay(500)
    return {
      code: 200,
      message: '请假申请已提交，等待审核'
    }
  }
}

/**
 * 回访管理 API
 * 
 * 包含功能：
 * - 获取回访列表
 * - 添加回访计划
 * - 执行回访
 * - 患者反馈
 */
export const followUpAPI = {
  /**
   * 获取回访列表
   * 
   * @param params 查询参数
   * 
   * TODO: 对接后端回访列表接口
   * - 接口地址: GET /api/follow-ups
   * - 查询参数: { doctorId?: number, patientId?: number, status?: string, startDate?: string, endDate?: string }
   * - 返回数据: { code: number, data: FollowUp[] }
   * - 需要根据用户角色返回不同数据
   */
  async getFollowUps(params?: any) {
    await delay(300)
    // 模拟回访数据
    const mockFollowUps = [
      {
        id: 1,
        doctorId: 1,
        doctorName: '张医生',
        doctorDepartment: '心内科',
        patientId: 1,
        patientName: '张三',
        patientPhone: '13800138001',
        followUpDate: '2024-01-25 14:00:00',
        method: 'phone',
        status: 'pending',
        priority: 'high',
        reason: '术后恢复情况跟踪',
        remark: '需要关注伤口愈合情况',
        createdAt: '2024-01-16 09:00:00'
      }
    ]
    return {
      code: 200,
      data: mockFollowUps
    }
  },

  /**
   * 添加回访计划
   * 
   * @param followUp 回访信息
   * 
   * TODO: 对接后端添加回访接口
   * - 接口地址: POST /api/follow-ups
   * - 请求体: FollowUp 对象
   * - 返回数据: { code: number, message: string, data: FollowUp }
   * - 需要验证医生权限
   */
  async addFollowUp(followUp: any) {
    await delay(500)
    return {
      code: 200,
      message: '回访计划添加成功',
      data: { id: Date.now(), ...followUp }
    }
  },

  /**
   * 执行回访
   * 
   * @param id 回访 ID
   * @param executeData 执行数据
   * 
   * TODO: 对接后端执行回访接口
   * - 接口地址: PUT /api/follow-ups/{id}/execute
   * - 请求体: { result: string, content: string, nextFollowUpDate?: string }
   * - 返回数据: { code: number, message: string }
   * - 需要验证医生权限
   */
  async executeFollowUp(id: number, executeData: any) {
    await delay(500)
    return {
      code: 200,
      message: '回访记录保存成功'
    }
  },

  /**
   * 患者添加反馈
   * 
   * @param id 回访 ID
   * @param feedback 患者反馈
   * 
   * TODO: 对接后端患者反馈接口
   * - 接口地址: PUT /api/follow-ups/{id}/feedback
   * - 请求体: { content: string, rating?: number }
   * - 返回数据: { code: number, message: string }
   * - 需要验证患者权限
   */
  async addPatientFeedback(id: number, feedback: any) {
    await delay(500)
    return {
      code: 200,
      message: '反馈提交成功'
    }
  },

  /**
   * 获取回访统计数据
   * 
   * @param doctorId 医生 ID（可选）
   * 
   * TODO: 对接后端回访统计接口
   * - 接口地址: GET /api/follow-ups/stats
   * - 查询参数: { doctorId?: number }
   * - 返回数据: { code: number, data: FollowUpStats }
   */
  async getFollowUpStats(doctorId?: number) {
    await delay(300)
    return {
      code: 200,
      data: {
        total: 45,
        pending: 12,
        completed: 28,
        overdue: 5
      }
    }
  }
}

export default api