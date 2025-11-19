export interface User {
  id: number
  name: string
  username: string
  password?: string
  role: 'user' | 'doctor' | 'admin' | string
  phone: string
  email: string
  department?: string
}

export interface Doctor {
  id: number
  name: string
  department: string
  title: string
  phone: string
  email: string
  schedule: string
}

export interface Department {
  id: number
  name: string
  description: string
  location: string
}

export interface Medicine {
  id: number
  name: string
  description: string
  price: number
  stock: number
  category: string
}

export interface Notice {
  id: number
  title: string
  content: string
  type: 'info' | 'warning' | 'success' | 'error' | string
  isPublished: boolean
  publishTime?: string
}