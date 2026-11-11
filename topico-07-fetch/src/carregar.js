// Traduz uma busca em três estados que o componente sabe desenhar: carregando, pronto e erro.
// buscar: função que devolve uma Promise com as consultas; aoMudar: recebe cada novo estado.
export async function carregarConsultas(buscar, aoMudar) {
  // O primeiro estado é entregue de forma síncrona, antes de a resposta chegar.
  aoMudar({ estado: 'carregando' });
  try {
    // await pausa a função até a Promise se resolver; a página continua responsiva.
    const consultas = await buscar();
    aoMudar({ estado: 'pronto', consultas });
  } catch (erro) {
    // Uma Promise rejeitada (rede fora do ar, status 500, JSON inválido) cai neste bloco.
    aoMudar({ estado: 'erro', mensagem: erro.message });
  }
}
