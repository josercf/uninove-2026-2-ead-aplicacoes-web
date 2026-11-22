// Experimento do subtema 4: cobertura de 100% com um defeito vivo.
// proximaConsultaComDefeito.js ignora a consulta marcada para o instante exato da pergunta (usa > no lugar de >=).
import { expect, test } from 'vitest';
import { proximaConsulta } from '../cobertura/proximaConsulta.js';
import { proximaConsultaComDefeito } from '../cobertura/proximaConsultaComDefeito.js';

// Duas consultas em ordem de horário.
const agenda = [
  { id: 1, paciente: 'Ana Souza', dataHora: '2026-10-05T09:00:00' },
  { id: 2, paciente: 'Bruno Lima', dataHora: '2026-10-05T10:30:00' },
];

// Teste fraco: executa todas as linhas e todos os ramos da função, e só confere que algo voltou.
test('fraco: proximaConsultaComDefeito devolve alguma coisa para 08:00 e nada depois do fim', () => {
  expect(proximaConsultaComDefeito(agenda, '2026-10-05T08:00:00')).toBeTruthy();
  expect(proximaConsultaComDefeito(agenda, '2026-10-05T23:00:00')).toBeNull();
});

// Teste forte da versão correta: a consulta marcada para o instante exato conta como a próxima.
test('forte: a consulta das 09:00 é a próxima quando a pergunta é feita às 09:00', () => {
  expect(proximaConsulta(agenda, '2026-10-05T09:00:00').id).toBe(1);
});

test('a versão correta passa também nos casos do teste fraco', () => {
  expect(proximaConsulta(agenda, '2026-10-05T08:00:00').id).toBe(1);
  expect(proximaConsulta(agenda, '2026-10-05T09:30:00').id).toBe(2);
  expect(proximaConsulta(agenda, '2026-10-05T23:00:00')).toBeNull();
});

// test.fails passa quando o conteúdo FALHA: registra que o teste forte pega o defeito da versão com defeito.
test.fails('forte: o mesmo teste forte reprova a versão com defeito (devolve 2, e não 1)', () => {
  expect(proximaConsultaComDefeito(agenda, '2026-10-05T09:00:00').id).toBe(1);
});
