// Confere que o tópico 05 usa a mesma lógica e os mesmos dados do tópico 04.
import { readFileSync } from 'node:fs';
import { expect, test } from 'vitest';
import { consultas as consultas04 } from '../../../topico-04-javascript/dados.js';
import { consultas as consultas05 } from '../dados.js';

// Lê um arquivo como texto, a partir da pasta deste teste.
function ler(caminho) {
  return readFileSync(new URL(caminho, import.meta.url), 'utf-8');
}

test('o agenda.js do tópico 05 é cópia exata do agenda.js do tópico 04', () => {
  expect(ler('../agenda.js')).toBe(ler('../../../topico-04-javascript/agenda.js'));
});

test('as consultas do tópico 05 são as mesmas do tópico 04', () => {
  expect(consultas05).toEqual(consultas04);
});
