// Confere que o tópico 07 continua usando o mesmo filtro do tópico 04.
import { readFileSync } from 'node:fs';
import { expect, test } from 'vitest';

// Lê um arquivo como texto, a partir da pasta deste teste.
function ler(caminho) {
  return readFileSync(new URL(caminho, import.meta.url), 'utf-8');
}

test('o agenda.js do tópico 07 é cópia exata do agenda.js do tópico 04', () => {
  expect(ler('../agenda.js')).toBe(ler('../../../topico-04-javascript/agenda.js'));
});
