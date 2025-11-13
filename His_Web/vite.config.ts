/**
 * Vite 构建工具配置文件
 * 
 * 功能说明：
 * - 配置 Vue 3 项目的构建和开发服务器
 * - 设置路径别名 @ 指向 src 目录
 * - 集成 Vue 开发工具和 JSX 支持
 * 
 * 主要配置：
 * - vue(): Vue 3 单文件组件支持
 * - vueJsx(): Vue JSX 语法支持
 * - vueDevTools(): Vue 开发者工具集成
 * - alias: 路径别名配置，@/ 映射到 src/ 目录
 */

import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueJsx from '@vitejs/plugin-vue-jsx'
import vueDevTools from 'vite-plugin-vue-devtools'

// Vite 配置 - 详见官方文档: https://vite.dev/config/
export default defineConfig({
  // 插件配置
  plugins: [
    vue(),           // Vue 3 单文件组件支持
    vueJsx(),        // Vue JSX 语法支持
    vueDevTools(),   // Vue 开发者工具
  ],
  // 模块解析配置
  resolve: {
    alias: {
      // 设置路径别名：@/ 指向 src/ 目录
      // 使用示例：import Component from '@/components/Component.vue'
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
})
