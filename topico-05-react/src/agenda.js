// Funções puras: recebem dados e devolvem dados, sem tocar na página.

// Devolve só as consultas da especialidade escolhida.
// export: deixa a função disponível para os arquivos que fizerem import dela.
export function filtrarPorEspecialidade(consultas, especialidade) {
  // === compara valor e tipo sem conversão; 'todas' é o value da primeira opção do select.
  // [...consultas] cria um array novo com os mesmos itens; a lista recebida fica intacta.
  if (especialidade === 'todas') return [...consultas];
  // filter percorre o array e devolve um array novo só com os itens que passam no teste.
  // (consulta) => ... é uma arrow function: recebe uma consulta e devolve true ou false.
  return consultas.filter((consulta) => consulta.especialidade === especialidade);
}

// Converte '2026-10-05T09:00:00' em '05/10/2026 09:00'.
export function formatarDataHora(iso) {
  // Expressão regular: cada par de parênteses captura um pedaço da data; \d é um dígito.
  // exec devolve um array com o trecho encontrado e os grupos, ou null se o texto não casar.
  const partes = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2})/.exec(iso);
  // !partes é verdadeiro quando exec devolveu null; throw interrompe a função com um erro.
  // A crase abre um template literal: ${iso} insere o valor recebido no meio do texto.
  if (!partes) throw new Error(`Data inválida: ${iso}`);
  // Desestruturação: a vírgula inicial pula o trecho inteiro; cada nome recebe um grupo.
  const [, ano, mes, dia, hora, minuto] = partes;
  // Monta o texto na ordem brasileira: dia, mês e ano, seguidos de hora e minuto.
  return `${dia}/${mes}/${ano} ${hora}:${minuto}`;
}
