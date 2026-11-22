// Testes da agenda inteira com a API simulada: fetch é trocado por uma função falsa, sem rede.
import { afterEach, expect, test, vi } from 'vitest';
import { cleanup, fireEvent, render, screen } from '@testing-library/react';
import App from '../App.jsx';

// As 3 consultas no formato JSON da API do tópico 06.
const tres = [
  { id: 1, paciente: 'Ana Souza', especialidade: 'Clínica geral', dataHora: '2026-10-05T09:00:00' },
  { id: 2, paciente: 'Bruno Lima', especialidade: 'Cardiologia', dataHora: '2026-10-05T10:30:00' },
  { id: 3, paciente: 'Carla Mendes', especialidade: 'Cardiologia', dataHora: '2026-10-06T14:00:00' },
];

// Desmonta a página simulada e devolve o fetch verdadeiro depois de cada teste.
afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
});

// Troca o fetch global por uma função que devolve a resposta indicada.
function simularApi(resposta) {
  vi.stubGlobal('fetch', vi.fn(() => Promise.resolve(resposta)));
}

// Resposta falsa com o formato mínimo que api.js usa: ok, status e json().
const respostaOk = { ok: true, status: 200, json: () => Promise.resolve(tres) };

// Devolve os nomes de paciente das linhas do corpo da tabela, na ordem da tela.
function pacientesNaTabela() {
  const linhas = document.querySelectorAll('#agenda tbody tr');
  return [...linhas].map((linha) => linha.children[1].textContent);
}

test('mostra "Carregando" enquanto a resposta não chegou', () => {
  // Uma Promise que nunca se resolve mantém a página no estado de carregamento.
  vi.stubGlobal('fetch', vi.fn(() => new Promise(() => {})));
  render(<App />);
  expect(screen.getByText('Carregando a agenda...')).toBeTruthy();
  expect(document.querySelector('#agenda')).toBeNull();
});

test('depois da resposta, abre com as 3 consultas e o total de 3', async () => {
  simularApi(respostaOk);
  render(<App />);
  expect(await screen.findByText('3 consulta(s) encontrada(s).')).toBeTruthy();
  expect(pacientesNaTabela()).toEqual(['Ana Souza', 'Bruno Lima', 'Carla Mendes']);
  expect(fetch).toHaveBeenCalledWith('http://localhost:5084/api/consultas');
});

test('Cardiologia mostra só as 2 consultas dessa especialidade', async () => {
  simularApi(respostaOk);
  render(<App />);
  await screen.findByText('3 consulta(s) encontrada(s).');
  fireEvent.change(screen.getByLabelText('Especialidade'), { target: { value: 'Cardiologia' } });
  expect(pacientesNaTabela()).toEqual(['Bruno Lima', 'Carla Mendes']);
  expect(screen.getByText('2 consulta(s) encontrada(s).')).toBeTruthy();
});

test('Dermatologia deixa a tabela sem linhas e o total em 0', async () => {
  simularApi(respostaOk);
  render(<App />);
  await screen.findByText('3 consulta(s) encontrada(s).');
  fireEvent.change(screen.getByLabelText('Especialidade'), { target: { value: 'Dermatologia' } });
  expect(pacientesNaTabela()).toEqual([]);
  expect(screen.getByText('0 consulta(s) encontrada(s).')).toBeTruthy();
});

test('a data aparece no formato brasileiro, como no tópico 04', async () => {
  simularApi(respostaOk);
  render(<App />);
  expect(await screen.findByText('05/10/2026 09:00')).toBeTruthy();
});

test('status 500 mostra a mensagem de erro e nenhuma tabela', async () => {
  simularApi({ ok: false, status: 500 });
  render(<App />);
  const alerta = await screen.findByRole('alert');
  expect(alerta.textContent).toContain('Não foi possível carregar a agenda');
  expect(alerta.textContent).toContain('HTTP 500');
  expect(document.querySelector('#agenda')).toBeNull();
});

test('rede fora do ar (fetch rejeitado) mostra a mensagem de erro', async () => {
  vi.stubGlobal('fetch', vi.fn(() => Promise.reject(new TypeError('Failed to fetch'))));
  render(<App />);
  const alerta = await screen.findByRole('alert');
  expect(alerta.textContent).toContain('Failed to fetch');
});
