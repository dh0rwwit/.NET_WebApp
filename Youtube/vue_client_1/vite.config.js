import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    port: 5273, // Vue Vite 개발 서버 포트
    proxy: {
      "/api": {
        target: "http://localhost:5000", // ASP.NET Core API 서버
        changeOrigin: true,
        secure: false,
      },
      "/client1": {
        target: "http://localhost:5274", // YARP 프록시 서버 (Vue)
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/client1/, ""),
      },
    },
  },
});


/*import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
})
*/
