// Configuração do Vite e do Vitest para o tópico 05, a agenda em React.
import react from '@vitejs/plugin-react';
import { defineConfig } from 'vite';

export default defineConfig({
  // O plugin do React converte o JSX dos arquivos .jsx em JavaScript comum.
  plugins: [react()],
  test: {
    // jsdom simula no Node a página do navegador, para os testes de componente.
    environment: 'jsdom',
    // Só os testes de componente; os de tests/ rodam com node --test.
    include: ['src/**/*.test.{js,jsx}'],
  },
});
