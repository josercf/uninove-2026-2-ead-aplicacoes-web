// Devolve a primeira consulta cujo horário é igual ou posterior ao instante dado, ou null se não houver.
// consultas: lista já ordenada por dataHora; agora: texto ISO como '2026-10-05T09:00:00'.
export function proximaConsulta(consultas, agora) {
  for (const consulta of consultas) {
    // O texto ISO em ordem alfabética coincide com a ordem do tempo; >= inclui o instante exato.
    if (consulta.dataHora >= agora) return consulta;
  }
  return null;
}
