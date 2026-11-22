// Configuração do Vite e do Vitest para o tópico 10, a agenda em React com testes e cobertura.
import react from '@vitejs/plugin-react';
import { defineConfig } from 'vite';

export default defineConfig({
  // O plugin do React converte o JSX dos arquivos .jsx em JavaScript comum.
  plugins: [react()],
  test: {
    // jsdom simula no Node a página do navegador, para os testes de componente.
    environment: 'jsdom',
    // Relatório de cobertura: npm run cobertura. Mede só o código de src, sem os testes.
    coverage: { provider: 'v8', include: ['src/**/*.{js,jsx}'], exclude: ['src/__tests__/**', 'src/main.jsx'], reporter: ['text', 'json-summary'] },
    // Só os testes de componente; os de tests/ rodam com node --test.
    include: ['src/**/*.test.{js,jsx}'],
  },
});
