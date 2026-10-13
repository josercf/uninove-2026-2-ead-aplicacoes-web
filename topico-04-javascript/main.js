// Liga as funções puras à página: lê o filtro e redesenha a tabela.
// import: traz os nomes que o outro arquivo exportou; ./ indica a mesma pasta.
import { consultas } from './dados.js';
import { filtrarPorEspecialidade, formatarDataHora } from './agenda.js';

// querySelector devolve o primeiro elemento que casa com o seletor CSS, ou null.
// '#filtro' casa com o elemento de id="filtro", o select da especialidade.
const filtro = document.querySelector('#filtro');
// '#agenda tbody' casa com o tbody de dentro da tabela de id="agenda".
const corpo = document.querySelector('#agenda tbody');
// '#total' casa com o parágrafo que mostra a contagem de consultas.
const total = document.querySelector('#total');

// Lê a opção do select e refaz as linhas da tabela e o texto do total.
function desenhar() {
  // filtro.value é o value da opção escolhida, como 'Cardiologia' ou 'todas'.
  const visiveis = filtrarPorEspecialidade(consultas, filtro.value);
  // replaceChildren troca todos os filhos do tbody pelas linhas novas de uma vez.
  // map transforma cada consulta numa linha; os ... passam as linhas uma a uma.
  corpo.replaceChildren(...visiveis.map((consulta) => {
    // createElement cria um elemento novo, que ainda não está na página.
    const linha = document.createElement('tr');
    // for...of percorre os quatro valores da consulta, um por volta.
    for (const valor of [consulta.id, consulta.paciente, consulta.especialidade, formatarDataHora(consulta.dataHora)]) {
      const celula = document.createElement('td');
      // textContent grava o valor como texto puro: um < no valor aparece na tela, sem virar tag.
      celula.textContent = valor;
      // append coloca a célula no fim da linha.
      linha.append(celula);
    }
    // A linha pronta volta para o map, que monta o array de linhas.
    return linha;
  }));
  // Template literal: a quantidade de consultas entra no meio da frase.
  total.textContent = `${visiveis.length} consulta(s) encontrada(s).`;
}

// addEventListener: o navegador chama desenhar a cada troca de opção no select.
// desenhar vai sem parênteses: o navegador guarda a função e a executa quando o evento ocorre.
filtro.addEventListener('change', desenhar);
// Primeira chamada: preenche a tabela com todas as consultas ao abrir a página.
desenhar();
