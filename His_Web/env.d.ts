/**
 * 环境类型声明文件
 * 
 * 功能说明：
 * - 为 Vite 客户端提供类型声明
 * - 为 Vue 单文件组件提供 TypeScript 类型支持
 * - 解决 TypeScript 无法识别 .vue 文件的问题
 */

/// <reference types="vite/client" />

/**
 * Vue 单文件组件模块声明
 * 
 * 作用：
 * - 告诉 TypeScript 如何处理 .vue 文件
 * - 使 import 语句能够正确识别 Vue 组件
 * - 提供基本的组件类型定义
 */
declare module '*.vue' {
  import type { DefineComponent } from 'vue'
  // 定义 Vue 组件的基本类型结构
  const component: DefineComponent<{}, {}, any>
  export default component
}