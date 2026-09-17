import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// Configuracao do Vite para o topico "cliente-servidor".
// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // Toda chamada do front para "/api" e redirecionada para a API
      // ASP.NET Core, que sobe na porta 5084 (perfil "http" do Kestrel,
      // anotado em api/ClinicaVida.Api/Properties/launchSettings.json).
      '/api': {
        target: 'http://localhost:5084',
        changeOrigin: true,
      },
    },
  },
})
