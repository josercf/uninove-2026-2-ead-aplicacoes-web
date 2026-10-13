// Dados fixos da agenda. No tópico de APIs, eles virão do servidor.
// const: o nome consultas não pode receber outro valor depois desta linha.
// export: outros arquivos podem importar esta lista pelo nome consultas.
// [ ] delimita um array, uma lista em ordem; cada { } é um objeto com pares chave: valor.
export const consultas = [
  // Um objeto por consulta; dataHora segue o formato ano-mês-diaThora:minuto:segundo.
  { id: 1, paciente: 'Ana Souza', especialidade: 'Clínica geral', dataHora: '2026-10-05T09:00:00' },
  { id: 2, paciente: 'Bruno Lima', especialidade: 'Cardiologia', dataHora: '2026-10-05T10:30:00' },
  { id: 3, paciente: 'Carla Mendes', especialidade: 'Cardiologia', dataHora: '2026-10-06T14:00:00' },
];
