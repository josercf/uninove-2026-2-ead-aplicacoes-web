// Tabela da agenda. Recebe pela prop consultas a lista já filtrada e desenha uma linha por item.
import { formatarDataHora } from './agenda.js';

export default function TabelaConsultas({ consultas }) {
  return (
    <table id="agenda">
      <thead>
        <tr><th scope="col">Id</th><th scope="col">Paciente</th><th scope="col">Especialidade</th><th scope="col">Data e hora</th></tr>
      </thead>
      <tbody>
        {/* map devolve um <tr> por consulta; key identifica a linha entre um desenho e outro. */}
        {consultas.map((consulta) => (
          <tr key={consulta.id}>
            {/* O valor entre chaves entra como texto puro, como o textContent do tópico 04. */}
            <td>{consulta.id}</td>
            <td>{consulta.paciente}</td>
            <td>{consulta.especialidade}</td>
            <td>{formatarDataHora(consulta.dataHora)}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
