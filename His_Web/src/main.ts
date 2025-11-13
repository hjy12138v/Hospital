/**
 * 医院便民服务系统 - 主入口文件
 * 
 * 功能说明：
 * - 初始化 Vue 3 应用程序
 * - 配置全局插件和组件
 * - 设置应用程序的基础架构
 * 
 * 技术栈：
 * - Vue 3 (Composition API)
 * - Element Plus (UI 组件库)
 * - Pinia (状态管理)
 * - Vue Router (路由管理)
 * - TypeScript (类型支持)
 */

// Vue 3 核心库导入
import { createApp } from 'vue'
// Pinia 状态管理库
import { createPinia } from 'pinia'
// Element Plus UI 组件库
import ElementPlus from 'element-plus'
// Element Plus 样式文件
import 'element-plus/dist/index.css'
// Element Plus 图标库 - 导入所有图标组件
import * as ElementPlusIconsVue from '@element-plus/icons-vue'

// 应用程序根组件
import App from './App.vue'
// 路由配置
import router from './router'

// 创建 Vue 应用实例
const app = createApp(App)

/**
 * 注册 Element Plus 图标组件
 * 
 * 作用：
 * - 将所有 Element Plus 图标注册为全局组件
 * - 可以在任何组件中直接使用 <el-icon><IconName /></el-icon>
 * - 避免在每个组件中单独导入图标
 */
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

// 安装插件和配置
app.use(createPinia())  // 状态管理 - 用于全局状态共享
app.use(router)         // 路由管理 - 用于页面导航
app.use(ElementPlus)    // UI 组件库 - 提供丰富的 UI 组件

// 挂载应用到 DOM
app.mount('#app')
