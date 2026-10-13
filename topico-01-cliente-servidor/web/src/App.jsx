import { useEffect, useState } from 'react'
import './App.css'

// Formata a data e hora da consulta no padrão brasileiro.
function formatarDataHora(dataHoraIso) {
  const data = new Date(dataHoraIso)
  return data.toLocaleString('pt-BR', {
    dateStyle: 'short',
    timeStyle: 'short',
  })
}

function App() {
  // "carregando": a requisição ainda está em andamento.
  // "consultas": a lista devolvida pela API, depois que a requisição termina.
  // "erro": mensagem para exibir quando a requisição falha.
  const [consultas, setConsultas] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState(null)

  useEffect(() => {
    // Busca a agenda de consultas na API. Abra a aba Network do DevTools
    // antes de recarregar a página para ver esta requisição acontecer.
    fetch('/api/consultas')
      .then((resposta) => {
        if (!resposta.ok) {
          throw new Error(`A API respondeu com status ${resposta.status}`)
        }
        return resposta.json()
      })
      .then((dados) => {
        setConsultas(dados)
        setCarregando(false)
      })
      .catch((erroRequisicao) => {
        setErro(erroRequisicao.message)
        setCarregando(false)
      })
  }, [])

  return (
    <main>
      <h1>Agenda da Clínica Vida+</h1>
      <p>
        Esta página busca a lista de consultas na API ASP.NET Core, pelo
        endereço <code>/api/consultas</code>.
      </p>

      {carregando && <p role="status">Carregando a agenda...</p>}

      {erro && (
        <p role="alert" className="erro">
          Não foi possível carregar a agenda: {erro}
        </p>
      )}

      {!carregando && !erro && (
        <table>
          <thead>
            <tr>
              <th>Id</th>
              <th>Paciente</th>
              <th>Especialidade</th>
              <th>Data e hora</th>
            </tr>
          </thead>
          <tbody>
            {consultas.map((consulta) => (
              <tr key={consulta.id}>
                <td>{consulta.id}</td>
                <td>{consulta.paciente}</td>
                <td>{consulta.especialidade}</td>
                <td>{formatarDataHora(consulta.dataHora)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </main>
  )
}

export default App
