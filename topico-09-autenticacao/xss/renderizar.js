// Duas formas de desenhar a tabela da agenda. A primeira tem a falha do XSS; a segunda é a correção.

// ERRADO: innerHTML lê o texto como HTML. Se o nome do paciente trouxer uma tag, o navegador a cria.
export function desenharComInnerHtml(corpo, consultas) {
  // map transforma cada consulta numa linha de HTML; join junta as linhas num texto só.
  // O nome entra no meio do texto sem nenhum tratamento: é aqui que o ataque passa.
  corpo.innerHTML = consultas.map((c) =>
    `<tr><td>${c.id}</td><td>${c.paciente}</td><td>${c.especialidade}</td><td>${c.dataHora}</td></tr>`,
  ).join('');
}

// CERTO: textContent grava o valor como texto puro. Um < no nome aparece na tela e não vira tag.
export function desenharComTextContent(corpo, consultas) {
  // ownerDocument é o document dono do tbody; createElement cria os elementos sem passar por HTML.
  const documento = corpo.ownerDocument;
  corpo.replaceChildren(...consultas.map((c) => {
    const linha = documento.createElement('tr');
    for (const valor of [c.id, c.paciente, c.especialidade, c.dataHora]) {
      const celula = documento.createElement('td');
      celula.textContent = valor;
      linha.append(celula);
    }
    return linha;
  }));
}
