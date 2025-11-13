// 模拟用户数据
export const mockUsers = [
  {
    id: 1,
    username: 'admin',
    password: '123456',
    role: 'admin' as const,
    name: '系统管理员',
    email: 'admin@hospital.com',
    phone: '13800138000'
  },
  {
    id: 2,
    username: 'doctor1',
    password: '123456',
    role: 'doctor' as const,
    name: '张医生',
    email: 'doctor1@hospital.com',
    phone: '13800138001',
    department: '内科'
  },
  {
    id: 3,
    username: 'user1',
    password: '123456',
    role: 'user' as const,
    name: '李患者',
    email: 'user1@hospital.com',
    phone: '13800138002'
  }
]

// 模拟医生数据
export const mockDoctors = [
  {
    id: 1,
    name: '张医生',
    department: '内科',
    title: '主任医师',
    phone: '13800138001',
    email: 'doctor1@hospital.com',
    schedule: '周一至周五 8:00-17:00'
  },
  {
    id: 2,
    name: '李医生',
    department: '外科',
    title: '副主任医师',
    phone: '13800138003',
    email: 'doctor2@hospital.com',
    schedule: '周一至周五 8:00-17:00'
  },
  {
    id: 3,
    name: '王医生',
    department: '儿科',
    title: '主治医师',
    phone: '13800138004',
    email: 'doctor3@hospital.com',
    schedule: '周一至周日 8:00-17:00'
  }
]

// 模拟科室数据
export const mockDepartments = [
  { id: 1, name: '内科', description: '内科疾病诊治', location: '1楼东区' },
  { id: 2, name: '外科', description: '外科手术治疗', location: '2楼西区' },
  { id: 3, name: '儿科', description: '儿童疾病诊治', location: '3楼南区' },
  { id: 4, name: '妇产科', description: '妇科产科诊治', location: '4楼北区' },
  { id: 5, name: '骨科', description: '骨科疾病治疗', location: '5楼东区' }
]

// 模拟药品数据
export const mockMedicines = [
  {
    id: 1,
    name: '阿莫西林胶囊',
    specification: '0.25g*24粒',
    price: 15.50,
    stock: 100,
    manufacturer: '华北制药',
    category: '抗生素'
  },
  {
    id: 2,
    name: '布洛芬缓释胶囊',
    specification: '0.3g*20粒',
    price: 12.80,
    stock: 80,
    manufacturer: '中美史克',
    category: '解热镇痛'
  },
  {
    id: 3,
    name: '复方甘草片',
    specification: '100片',
    price: 8.90,
    stock: 150,
    manufacturer: '太极集团',
    category: '止咳化痰'
  }
]

// 模拟通知数据
export const mockNotices = [
  {
    id: 1,
    title: '医院春节放假通知',
    content: '根据国家法定节假日安排，医院春节期间放假时间为...',
    type: 'info',
    publishTime: '2024-01-15 10:00:00',
    isPublished: true
  },
  {
    id: 2,
    title: '新冠疫苗接种通知',
    content: '为做好新冠疫苗接种工作，现将有关事项通知如下...',
    type: 'warning',
    publishTime: '2024-01-10 14:30:00',
    isPublished: true
  },
  {
    id: 3,
    title: '医保政策调整通知',
    content: '根据最新医保政策调整，现将相关变化通知如下...',
    type: 'success',
    publishTime: '2024-01-08 09:15:00',
    isPublished: false
  }
]

// 模拟患者数据
export const mockPatients = [
  {
    id: 1,
    name: '李患者',
    age: 35,
    gender: '男',
    phone: '13800138002',
    idCard: '110101198901010001',
    address: '北京市朝阳区',
    medicalHistory: '高血压病史3年'
  },
  {
    id: 2,
    name: '王女士',
    age: 28,
    gender: '女',
    phone: '13800138005',
    idCard: '110101199601010002',
    address: '北京市海淀区',
    medicalHistory: '无特殊病史'
  }
]

// 模拟预约数据
export const mockAppointments = [
  {
    id: 1,
    patientId: 1,
    patientName: '李患者',
    doctorId: 1,
    doctorName: '张医生',
    department: '内科',
    appointmentDate: '2024-01-20',
    appointmentTime: '09:00',
    status: 'confirmed',
    symptoms: '头痛、发热'
  },
  {
    id: 2,
    patientId: 2,
    patientName: '王女士',
    doctorId: 3,
    doctorName: '王医生',
    department: '儿科',
    appointmentDate: '2024-01-21',
    appointmentTime: '14:00',
    status: 'pending',
    symptoms: '儿童发热'
  }
]