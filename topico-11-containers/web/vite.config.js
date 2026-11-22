// Configuração do Vite do tópico 11: só o plugin do React, sem a parte de testes do tópico 10.
import react from '@vitejs/plugin-react';
import { defineConfig } from 'vite';

export default defineConfig({
  // O plugin do React converte o JSX dos arquivos .jsx em JavaScript comum.
  plugins: [react()],
});
