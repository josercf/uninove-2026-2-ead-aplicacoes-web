// Testes do XSS: o mesmo nome de paciente malicioso, desenhado com innerHTML e com textContent.
import { test } from 'node:test';
import assert from 'node:assert/strict';
import { JSDOM, CookieJar } from 'jsdom';
import { desenharComInnerHtml, desenharComTextContent } from '../renderizar.js';

// Nome digitado por um atacante em um formulário de cadastro: uma imagem quebrada com um manipulador onerror.
const PAYLOAD = '<img src="x" onerror="window.__disparos = (window.__disparos || 0) + 1">';
const consultas = [{ id: 9, paciente: PAYLOAD, especialidade: 'Cardiologia', dataHora: '2026-10-05T09:00:00' }];

// Monta uma página mínima com a tabela da agenda. runScripts: 'dangerously' deixa o jsdom executar
// os atributos onerror, como o navegador faz; sem isso o teste não mediria o ataque.
function novaPagina(opcoes = {}) {
  const dom = new JSDOM('<table id="agenda"><tbody></tbody></table>', {
    url: 'https://localhost:7084/', runScripts: 'dangerously', ...opcoes,
  });
  return { dom, janela: dom.window, corpo: dom.window.document.querySelector('#agenda tbody') };
}

// O navegador dispara o evento error de uma imagem que não carrega. O jsdom não busca a imagem,
// então o teste dispara o mesmo evento em cada <img> criada, que é o que o navegador faria.
function dispararErroDasImagens(janela) {
  for (const imagem of janela.document.querySelectorAll('img')) {
    imagem.dispatchEvent(new janela.Event('error'));
  }
}

test('innerHTML transforma o nome em uma imagem com onerror e o código do atacante roda uma vez', () => {
  const { janela, corpo } = novaPagina();

  desenharComInnerHtml(corpo, consultas);
  dispararErroDasImagens(janela);

  assert.equal(corpo.querySelectorAll('img[onerror]').length, 1);
  assert.equal(janela.__disparos, 1);
});

test('textContent grava o nome como texto: nenhuma imagem nasce e o código não roda', () => {
  const { janela, corpo } = novaPagina();

  desenharComTextContent(corpo, consultas);
  dispararErroDasImagens(janela);

  assert.equal(corpo.querySelectorAll('img').length, 0);
  assert.equal(janela.__disparos, undefined);
  // A célula do paciente mostra o texto do ataque literalmente, como um nome estranho, sem efeito.
  assert.equal(corpo.querySelectorAll('td')[1].textContent, PAYLOAD);
});

test('com textContent o texto do ataque é escapado ao serializar a página', () => {
  const { corpo } = novaPagina();

  desenharComTextContent(corpo, consultas);

  assert.match(corpo.innerHTML, /&lt;img src="x" onerror=/);
});

test('cookie HttpOnly não aparece em document.cookie, e o cookie comum aparece', () => {
  const jar = new CookieJar();
  jar.setCookieSync('clinica.sessao=ticket-secreto; HttpOnly; Secure; SameSite=Lax', 'https://localhost:7084/');
  jar.setCookieSync('tema=escuro', 'https://localhost:7084/');
  const { janela } = novaPagina({ cookieJar: jar });

  // O que um script injetado consegue ler: só o cookie sem HttpOnly.
  assert.equal(janela.document.cookie, 'tema=escuro');
  assert.doesNotMatch(janela.document.cookie, /clinica\.sessao/);
});

test('o ataque com innerHTML rouba o cookie comum e não rouba o HttpOnly', () => {
  const jar = new CookieJar();
  jar.setCookieSync('clinica.sessao=ticket-secreto; HttpOnly; Secure; SameSite=Lax', 'https://localhost:7084/');
  jar.setCookieSync('tema=escuro', 'https://localhost:7084/');
  const { janela, corpo } = novaPagina({ cookieJar: jar });
  const roubo = [{ id: 9, paciente: '<img src="x" onerror="window.__roubado = document.cookie">', especialidade: 'Cardiologia', dataHora: '2026-10-05T09:00:00' }];

  desenharComInnerHtml(corpo, roubo);
  dispararErroDasImagens(janela);

  assert.equal(janela.__roubado, 'tema=escuro');
});
