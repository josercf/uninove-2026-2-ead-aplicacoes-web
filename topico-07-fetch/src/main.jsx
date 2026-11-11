// Ponto de entrada: o React assume a seção da agenda; o resto da página continua HTML fixo.
import { StrictMode } from 'react';
// createRoot: liga o React a um elemento que já existe na página.
import { createRoot } from 'react-dom/client';
import App from './App.jsx';

// A única busca na página: a seção vazia de id agenda-do-dia, no index.html.
const secao = document.querySelector('#agenda-do-dia');
// render desenha o App dentro da seção; StrictMode liga avisos extras durante o desenvolvimento.
createRoot(secao).render(<StrictMode><App /></StrictMode>);
