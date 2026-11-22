// Mesma função de proximaConsulta.js, com um defeito de limite: > no lugar de >=.
// Uma consulta marcada para o instante exato da pergunta é ignorada e a seguinte é devolvida.
export function proximaConsultaComDefeito(consultas, agora) {
  for (const consulta of consultas) {
    // DEFEITO: com >, a consulta das 09:00 não é "a próxima" quando a pergunta é feita às 09:00.
    if (consulta.dataHora > agora) return consulta;
  }
  return null;
}
