import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./setupTests.js'],
  },
  server: {
    proxy: {
      '/v1/tiny-url': {
        target: 'http://localhost:9090',
        changeOrigin: true,
      },
    },
  },
})
