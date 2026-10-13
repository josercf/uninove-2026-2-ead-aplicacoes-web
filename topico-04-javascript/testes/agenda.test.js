// Testes das funções puras da agenda. Elas não dependem do navegador.
import { test } from 'node:test';
import assert from 'node:assert/strict';
import { filtrarPorEspecialidade, formatarDataHora } from '../agenda.js';

const consultas = [
  { id: 1, paciente: 'Ana Souza', especialidade: 'Clínica geral', dataHora: '2026-10-05T09:00:00' },
  { id: 2, paciente: 'Bruno Lima', especialidade: 'Cardiologia', dataHora: '2026-10-05T10:30:00' },
  { id: 3, paciente: 'Carla Mendes', especialidade: 'Cardiologia', dataHora: '2026-10-06T14:00:00' },
];

test('filtra pela especialidade escolhida', () => {
  const resultado = filtrarPorEspecialidade(consultas, 'Cardiologia');
  assert.deepEqual(resultado.map((c) => c.id), [2, 3]);
});

test('"todas" devolve a lista inteira', () => {
  assert.equal(filtrarPorEspecialidade(consultas, 'todas').length, 3);
});

test('especialidade sem consulta devolve lista vazia', () => {
  assert.deepEqual(filtrarPorEspecialidade(consultas, 'Pediatria'), []);
});

test('não altera a lista original', () => {
  const resultado = filtrarPorEspecialidade(consultas, 'todas');
  assert.notEqual(resultado, consultas);
  assert.equal(consultas.length, 3);
});

test('formata data e hora no padrão brasileiro', () => {
  assert.equal(formatarDataHora('2026-10-05T09:00:00'), '05/10/2026 09:00');
});

test('data inválida gera erro com mensagem em português', () => {
  assert.throws(() => formatarDataHora('ontem'), /Data inválida: ontem/);
});
