// Testes de cada componente sozinho, recebendo props montadas no próprio teste.
import { afterEach, expect, test } from 'vitest';
import { cleanup, fireEvent, render, screen } from '@testing-library/react';
import FiltroEspecialidade from '../FiltroEspecialidade.jsx';
import TabelaConsultas from '../TabelaConsultas.jsx';

afterEach(cleanup);

test('TabelaConsultas desenha uma linha por consulta recebida na prop', () => {
  const consultas = [
    { id: 7, paciente: 'Diego Rocha', especialidade: 'Dermatologia', dataHora: '2026-10-07T08:15:00' },
  ];
  render(<TabelaConsultas consultas={consultas} />);
  const linhas = document.querySelectorAll('#agenda tbody tr');
  expect(linhas.length).toBe(1);
  expect(linhas[0].textContent).toBe('7Diego RochaDermatologia07/10/2026 08:15');
});

test('FiltroEspecialidade mostra o valor recebido e avisa a troca pela prop aoMudar', () => {
  const recebidos = [];
  render(<FiltroEspecialidade valor="todas" aoMudar={(valor) => recebidos.push(valor)} />);
  const campo = screen.getByLabelText('Especialidade');
  expect(campo.value).toBe('todas');
  fireEvent.change(campo, { target: { value: 'Clínica geral' } });
  expect(recebidos).toEqual(['Clínica geral']);
});
