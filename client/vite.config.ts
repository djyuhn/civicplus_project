import { configDefaults, defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react-swc'
import config from './config.json'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    host: true,
    strictPort: true,
    port: config.vitePort,
  },
  test: {
    globals: true,
    environment: 'jsdom',
    css: true,
    exclude: [...configDefaults.exclude, '**/e2e/**'], // Example: Exclude e2e tests
    coverage: {
      provider: 'v8', // Use Vite's default coverage provider
      reporter: ['text', 'json', 'html'],
    },
  },
})
