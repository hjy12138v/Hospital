import http from './http'

// 用户管理 API
export const userAPI = {
  async getUsers() {
    const res = await http.get('/api/User')
    return { data: Array.isArray(res) ? res : (res?.data ?? res?.data.items ?? []) }
  },
  async getUserById(id: number) {
    const res = await http.get(`/api/User/${id}`)
    return { data: res }
  },
  async addUser(user: unknown) {
    const res = await http.post('/api/User', user)
    return { data: res }
  },
  async updateUser(id: number, user: unknown) {
    const res = await http.put(`/api/User/${id}`, user)
    return { data: res }
  },
  async deleteUser(id: number) {
    const res = await http.delete(`/api/User/${id}`)
    return { data: res }
  },
}

// 医生管理 API
export const doctorAPI = {
  async getDoctors() {
    const rawDoctors = await http.get('/api/Doctor')
    const doctors = Array.isArray(rawDoctors) ? rawDoctors : (rawDoctors?.data ?? [])

    // 取用户与科室字典，做聚合映射
    const [usersRes, departmentsRes] = await Promise.all([
      http.get('/api/User'),
      http.get('/api/Department'),
    ])
    const users = Array.isArray(usersRes) ? usersRes : (usersRes?.data ?? [])
    const departments = Array.isArray(departmentsRes) ? departmentsRes : (departmentsRes?.data ?? [])

    const userMap = new Map<number, any>()
    for (const u of users as any[]) {
      const id = (u.id ?? u.userId) as number
      userMap.set(id, u)
    }

    const deptMap = new Map<number, any>()
    for (const d of departments as any[]) {
      const id = (d.id ?? d.departmentId) as number
      const name = d.name ?? d.departmentName ?? d.deptName ?? ''
      deptMap.set(id, { ...d, name })
    }

    const normalized = (doctors as any[]).map((d) => {
      const id = d.doctorId ?? d.id
      const user = userMap.get(d.userId)
      const dept = deptMap.get(d.departmentId)
      return {
        id: id as number,
        name: user?.name ?? user?.realName ?? '',
        department: dept?.name ?? '',
        title: d.position ?? d.title ?? '',
        phone: user?.phone ?? user?.mobile ?? '',
        email: user?.email ?? '',
        schedule: d.schedule ?? d.dutyTime ?? d.outpatientTime ?? '',
      }
    })

    return { data: normalized }
  },
  async getDoctorById(id: number) {
    const res = await http.get(`/api/Doctor/${id}`)
    return { data: res }
  },
  async addDoctor(doctor: unknown) {
    const res = await http.post('/api/Doctor', doctor)
    return { data: res }
  },
  async updateDoctor(id: number, doctor: unknown) {
    const res = await http.put(`/api/Doctor/${id}`, doctor)
    return { data: res }
  },
  async deleteDoctor(id: number) {
    const res = await http.delete(`/api/Doctor/${id}`)
    return { data: res }
  },
}

// 科室管理 API
export const departmentAPI = {
  async getDepartments() {
    const res = await http.get('/api/Department')
    const list = Array.isArray(res) ? res : (res?.data ?? res?.data.items ?? [])
    const normalized = (list as any[]).map((d) => ({
      id: d.id ?? d.departmentId ?? d.Id,
      name: d.name ?? d.departmentName ?? d.deptName ?? '',
      description: d.description ?? d.desc ?? d.Description ?? '',
      location: d.location ?? d.address ?? d.Location ?? '',
    }))
    return { data: normalized }
  },
  async getDepartmentById(id: number) {
    const res = await http.get(`/api/Department/${id}`)
    return { data: res }
  },
  async addDepartment(department: unknown) {
    const res = await http.post('/api/Department', department)
    return { data: res }
  },
  async updateDepartment(id: number, department: unknown) {
    const res = await http.put(`/api/Department/${id}`, department)
    return { data: res }
  },
  async deleteDepartment(id: number) {
    const res = await http.delete(`/api/Department/${id}`)
    return { data: res }
  },
}

// 药品管理 API
export const medicineAPI = {
  async getMedicines() {
    const res = await http.get('/api/Medicine')
    const list = Array.isArray(res) ? res : (res?.data ?? res?.data.items ?? [])
    const normalized = (list as any[]).map((m) => ({
      id: m.medicineId ?? m.id ?? m.Id,
      name: m.name ?? m.medicineName ?? m.drugName ?? '',
      description: m.description ?? m.Description ?? '',
      price: Number(m.unitPrice ?? m.UnitPrice ?? m.price ?? m.Price ?? 0),
      stock: Number(m.stock ?? m.Stock ?? m.quantity ?? 0),
      category: m.type ?? m.Type ?? m.category ?? m.Category ?? '',
    }))
    return { data: normalized }
  },
  async getMedicineById(id: number) {
    const res = await http.get(`/api/Medicine/${id}`)
    return { data: res }
  },
  async addMedicine(medicine: unknown) {
    const m = medicine as any
    const payload = {
      // MedicineDto fields expected by backend
      name: m.name,
      description: m.description,
      stock: m.stock,
      unitPrice: m.price,
      type: m.category,
    }
    const res = await http.post('/api/Medicine', payload)
    return { data: res }
  },
  async updateMedicine(id: number, medicine: unknown) {
    const m = medicine as any
    const payload = {
      medicineId: id,
      name: m.name,
      description: m.description,
      stock: m.stock,
      unitPrice: m.price,
      type: m.category,
    }
    const res = await http.put(`/api/Medicine/${id}`, payload)
    return { data: res }
  },
  async deleteMedicine(id: number) {
    const res = await http.delete(`/api/Medicine/${id}`)
    return { data: res }
  },
}

// 通知管理 API
export const noticeAPI = {
  async getNotices() {
    const res = await http.get('/api/Notice')
    const list = Array.isArray(res) ? res : (res?.data ?? res?.data.items ?? [])

    // 取用户列表用于 SenderId -> 发布人用户名 的映射
    let users: any[] = []
    try {
      const usersRes = await http.get('/api/User')
      users = Array.isArray(usersRes) ? usersRes : (usersRes?.data ?? usersRes?.data?.items ?? [])
    } catch {
      users = []
    }
    const userMap = new Map<number, any>()
    for (const u of users) {
      const id = (u.id ?? u.userId ?? u.Id) as number
      userMap.set(id, u)
    }

    const normalized = (list as any[]).map((n) => {
      const publishTime = n.publishTime ?? n.PublishTime ?? n.publish_time ?? n.publishDate ?? ''
      const isPublished = (n.isPublished ?? n.isPublish ?? n.IsPublished) ?? Boolean(publishTime)
      const senderId = n.senderId ?? n.SenderId
      const sender = senderId != null ? userMap.get(senderId as number) : undefined
      const senderName = sender?.username ?? sender?.name ?? sender?.realName ?? ''
      return {
        id: n.id ?? n.noticeId ?? n.Id,
        title: n.title ?? n.noticeTitle ?? n.Title ?? '',
        content: n.content ?? n.noticeContent ?? n.Content ?? '',
        type: n.type ?? n.Type ?? '',
        isPublished,
        publishTime,
        senderId: senderId ?? null,
        senderName,
      }
    })
    return { data: normalized }
  },
  async getNoticeById(id: number) {
    const res = await http.get(`/api/Notice/${id}`)
    return { data: res }
  },
  async addNotice(notice: unknown) {
    const res = await http.post('/api/Notice', notice)
    return { data: res }
  },
  async updateNotice(id: number, notice: unknown) {
    const res = await http.put(`/api/Notice/${id}`, notice)
    return { data: res }
  },
  async deleteNotice(id: number) {
    const res = await http.delete(`/api/Notice/${id}`)
    return { data: res }
  },
}
