// Testes da agenda inteira: o mesmo comportamento do filtro do tópico 04.
import { afterEach, expect, test } from 'vitest';
import { cleanup, fireEvent, render, screen } from '@testing-library/react';
import App from '../App.jsx';

// Desmonta a página simulada depois de cada teste, para um não afetar o outro.
afterEach(cleanup);

// Devolve os nomes de paciente das linhas do corpo da tabela, na ordem da tela.
function pacientesNaTabela() {
  // querySelectorAll devolve todas as linhas tr do corpo da tabela de id agenda.
  const linhas = document.querySelectorAll('#agenda tbody tr');
  // children[1] é a segunda célula da linha, a do paciente.
  return [...linhas].map((linha) => linha.children[1].textContent);
}

test('abre com as 3 consultas e o total de 3', () => {
  render(<App />);
  expect(pacientesNaTabela()).toEqual(['Ana Souza', 'Bruno Lima', 'Carla Mendes']);
  expect(screen.getByText('3 consulta(s) encontrada(s).')).toBeTruthy();
});

test('Cardiologia mostra só as 2 consultas dessa especialidade', () => {
  // render desenha o App numa página simulada pelo jsdom, sem abrir navegador.
  render(<App />);
  // getByLabelText acha o campo pelo rótulo; fireEvent.change simula a troca de opção.
  fireEvent.change(screen.getByLabelText('Especialidade'), { target: { value: 'Cardiologia' } });
  // toEqual compara o array de pacientes da tela com o esperado, item a item.
  expect(pacientesNaTabela()).toEqual(['Bruno Lima', 'Carla Mendes']);
  // getByText interrompe o teste com erro se nenhum elemento tiver exatamente esse texto.
  expect(screen.getByText('2 consulta(s) encontrada(s).')).toBeTruthy();
});

test('Dermatologia deixa a tabela sem linhas e o total em 0', () => {
  render(<App />);
  fireEvent.change(screen.getByLabelText('Especialidade'), { target: { value: 'Dermatologia' } });
  expect(pacientesNaTabela()).toEqual([]);
  expect(screen.getByText('0 consulta(s) encontrada(s).')).toBeTruthy();
});

test('a data aparece no formato brasileiro, como no tópico 04', () => {
  render(<App />);
  expect(screen.getByText('05/10/2026 09:00')).toBeTruthy();
});
