// Testes de componente da agenda diante de uma API protegida: sem login, a API do tópico 09 responde 401.
// userEvent imita a pessoa (foco, clique, teclas); fireEvent, usado no tópico 07, dispara só o evento solto.
import { afterEach, expect, test, vi } from 'vitest';
import { cleanup, render, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import App from '../App.jsx';

// Consultas no formato JSON da API.
const tres = [
  { id: 1, paciente: 'Ana Souza', especialidade: 'Clínica geral', dataHora: '2026-10-05T09:00:00' },
  { id: 2, paciente: 'Bruno Lima', especialidade: 'Cardiologia', dataHora: '2026-10-05T10:30:00' },
  { id: 3, paciente: 'Carla Mendes', especialidade: 'Cardiologia', dataHora: '2026-10-06T14:00:00' },
];

// Devolve o fetch verdadeiro e desmonta a página simulada depois de cada teste.
afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
});

// Troca o fetch global por uma função falsa que devolve a resposta indicada.
function simularApi(resposta) {
  vi.stubGlobal('fetch', vi.fn(() => Promise.resolve(resposta)));
}

test('status 401 pede o login em vez de mostrar o código HTTP', async () => {
  // A API simulada responde 401, como a API do tópico 09 responde a quem chega sem cookie de sessão.
  simularApi({ ok: false, status: 401 });
  // render monta o App numa página simulada (jsdom); o useEffect dispara a busca logo depois.
  render(<App />);
  // role="alert" é como o leitor de tela anuncia o aviso; getByRole procura pelo papel, como a pessoa usa a página.
  // findByRole espera o elemento aparecer, porque a mensagem só surge depois que a Promise do fetch termina.
  const alerta = await screen.findByRole('alert');
  expect(alerta.textContent).toBe('Faça login para ver a agenda.');
  // queryByRole devolve null quando o elemento não existe: com 401, nenhuma tabela é desenhada.
  expect(screen.queryByRole('table')).toBeNull();
});

test('status 403 também tem mensagem própria', async () => {
  simularApi({ ok: false, status: 403 });
  render(<App />);
  const alerta = await screen.findByRole('alert');
  expect(alerta.textContent).toBe('Sua conta não pode ver a agenda.');
});

test('outros erros continuam mostrando o código HTTP', async () => {
  simularApi({ ok: false, status: 500 });
  render(<App />);
  const alerta = await screen.findByRole('alert');
  expect(alerta.textContent).toContain('HTTP 500');
});

test('escolher Cardiologia com o teclado deixa duas linhas na tabela', async () => {
  simularApi({ ok: true, status: 200, json: () => Promise.resolve(tres) });
  // setup cria uma pessoa simulada, que move o foco e dispara os eventos na ordem do navegador.
  const pessoa = userEvent.setup();
  render(<App />);
  // findByRole espera a tabela aparecer; as linhas contam o cabeçalho, por isso 4 antes do filtro.
  const tabela = await screen.findByRole('table');
  // within limita a busca ao interior da tabela; getAllByRole('row') devolve todas as linhas.
  expect(within(tabela).getAllByRole('row')).toHaveLength(4);
  // selectOptions escolhe a opção pelo select achado com o rótulo, com foco e eventos de uma pessoa.
  await pessoa.selectOptions(screen.getByLabelText('Especialidade'), 'Cardiologia');
  // Depois do filtro: o cabeçalho e as 2 consultas de Cardiologia.
  expect(within(screen.getByRole('table')).getAllByRole('row')).toHaveLength(3);
  expect(screen.getByText('2 consulta(s) encontrada(s).')).toBeTruthy();
});
