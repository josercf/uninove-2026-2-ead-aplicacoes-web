// Componente raiz da agenda: guarda a especialidade escolhida e entrega os dados aos filhos.
// useState: função do React que dá ao componente um valor guardado entre um desenho e outro.
import { useState } from 'react';
import { consultas } from './dados.js';
// A mesma função pura do tópico 04, copiada sem nenhuma mudança.
import { filtrarPorEspecialidade } from './agenda.js';
import FiltroEspecialidade from './FiltroEspecialidade.jsx';
import TabelaConsultas from './TabelaConsultas.jsx';

// Um componente é uma função que devolve o pedaço de tela que ela representa.
export default function App() {
  // especialidade: o valor atual; setEspecialidade: troca o valor e pede um novo desenho.
  // 'todas' é o valor inicial, o mesmo da primeira opção do select.
  const [especialidade, setEspecialidade] = useState('todas');
  // A cada desenho, a lista visível é recalculada a partir do estado atual.
  const visiveis = filtrarPorEspecialidade(consultas, especialidade);

  // O return devolve JSX: tags parecidas com HTML, escritas dentro do JavaScript.
  return (
    // <> e </> agrupam os elementos sem criar uma tag a mais na página.
    <>
      <h2>Agenda do dia</h2>
      {/* valor e aoMudar são props: os dados que o App entrega ao componente filho. */}
      <FiltroEspecialidade valor={especialidade} aoMudar={setEspecialidade} />
      {/* As chaves { } inserem um valor JavaScript no meio do JSX. */}
      <p id="total" aria-live="polite">{visiveis.length} consulta(s) encontrada(s).</p>
      <TabelaConsultas consultas={visiveis} />
    </>
  );
}
