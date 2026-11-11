// Componente raiz da agenda: busca as consultas na API, guarda a especialidade escolhida e entrega os dados aos filhos.
// useState: dá ao componente um valor guardado entre um desenho e outro.
// useEffect: roda um trecho de código depois do primeiro desenho, o lugar certo para chamar a API.
import { useEffect, useState } from 'react';
// buscarConsultas faz o fetch; carregarConsultas traduz a busca em carregando, pronto ou erro.
import { buscarConsultas } from './api.js';
import { carregarConsultas } from './carregar.js';
// A mesma função pura do tópico 04, copiada sem nenhuma mudança.
import { filtrarPorEspecialidade } from './agenda.js';
import FiltroEspecialidade from './FiltroEspecialidade.jsx';
import TabelaConsultas from './TabelaConsultas.jsx';

export default function App() {
  const [especialidade, setEspecialidade] = useState('todas');
  // carga guarda o estado da busca; começa em carregando, antes de qualquer resposta.
  const [carga, setCarga] = useState({ estado: 'carregando' });

  // O array vazio [] manda rodar uma vez, depois do primeiro desenho.
  useEffect(() => {
    // Cada novo estado da busca vira um novo desenho da tela.
    carregarConsultas(buscarConsultas, setCarga);
  }, []);

  // Enquanto a resposta não chega, a tela mostra só o aviso de carregamento.
  if (carga.estado === 'carregando') return <p>Carregando a agenda...</p>;
  // role="alert": o leitor de tela anuncia a mensagem assim que ela aparece.
  if (carga.estado === 'erro') return <p role="alert">Não foi possível carregar a agenda ({carga.mensagem}).</p>;

  // Daqui em diante o estado é pronto: carga.consultas tem a lista recebida da API.
  const visiveis = filtrarPorEspecialidade(carga.consultas, especialidade);

  return (
    <>
      <h2>Agenda do dia</h2>
      <FiltroEspecialidade valor={especialidade} aoMudar={setEspecialidade} />
      <p id="total" aria-live="polite">{visiveis.length} consulta(s) encontrada(s).</p>
      <TabelaConsultas consultas={visiveis} />
    </>
  );
}
