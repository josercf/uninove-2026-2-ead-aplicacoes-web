// Busca as consultas na API do tópico 07, cópia da API do tópico 06 com CORS. No tópico 05 elas vinham do dados.js.
// A API sobe em http://localhost:5084 (launchSettings.json da pasta api/).
export const URL_API = 'http://localhost:5084';

// async: a função devolve uma Promise, um objeto que representa um resultado que ainda vai chegar.
export async function buscarConsultas(base = URL_API) {
  // fetch envia GET /api/consultas e devolve uma Promise; await espera a resposta chegar.
  const resposta = await fetch(`${base}/api/consultas`);
  // ok é false para status fora de 200 a 299; o fetch não rejeita sozinho nesse caso.
  // O status vai junto no erro, para a tela distinguir 401 (sem login) de 500 (falha do servidor).
  if (!resposta.ok) throw Object.assign(new Error(`HTTP ${resposta.status}`), { status: resposta.status });
  // json() lê o corpo e o converte em array; await espera a leitura terminar.
  return resposta.json();
}
