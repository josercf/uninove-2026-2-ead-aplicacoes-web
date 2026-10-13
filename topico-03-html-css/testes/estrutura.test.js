// Testes da estrutura semântica e responsiva da página da Clínica Vida+.
import { test } from 'node:test';
import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';

const html = readFileSync(new URL('../index.html', import.meta.url), 'utf8');
const css = readFileSync(new URL('../estilos.css', import.meta.url), 'utf8');

test('a página declara o idioma português do Brasil', () => {
  assert.match(html, /<html lang="pt-BR">/);
});

test('a página declara a viewport para celular', () => {
  assert.match(html, /<meta name="viewport" content="width=device-width, initial-scale=1">/);
});

test('cada região principal aparece exatamente uma vez', () => {
  for (const tag of ['header', 'nav', 'main', 'footer']) {
    const ocorrencias = html.match(new RegExp(`<${tag}[\\s>]`, 'g')) ?? [];
    assert.equal(ocorrencias.length, 1, `<${tag}> deveria aparecer uma vez`);
  }
});

test('existe um único h1', () => {
  assert.equal((html.match(/<h1[\s>]/g) ?? []).length, 1);
});

test('toda imagem tem texto alternativo', () => {
  const imagens = html.match(/<img\b[^>]*>/g) ?? [];
  assert.ok(imagens.length > 0, 'a página deveria ter ao menos uma imagem');
  for (const img of imagens) assert.match(img, /\balt="[^"]+"/);
});

test('a tabela da agenda tem cabeçalhos com escopo de coluna', () => {
  assert.match(html, /<table id="agenda">/);
  assert.equal((html.match(/<th scope="col">/g) ?? []).length, 4);
});

test('o CSS parte do celular e muda o layout a partir de 768px', () => {
  assert.match(css, /@media \(min-width: 768px\)/);
  assert.match(css, /display: grid/);
});
