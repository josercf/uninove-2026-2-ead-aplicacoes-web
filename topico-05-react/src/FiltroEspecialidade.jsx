// Campo de seleção da especialidade. Não guarda estado: mostra o valor recebido e avisa a troca.
// { valor, aoMudar }: desestruturação das props que o App passou em <FiltroEspecialidade ... />.
export default function FiltroEspecialidade({ valor, aoMudar }) {
  return (
    <>
      {/* htmlFor: no JSX, o atributo for do HTML se escreve htmlFor. */}
      <label htmlFor="filtro">Especialidade</label>
      {/* O JSX descarta a quebra de linha entre tags; {' '} repõe o espaço que o HTML mostrava. */}
      {' '}
      {/* value={valor}: o select mostra a opção que o App guarda no estado. */}
      {/* onChange: o React chama a arrow function a cada troca; evento.target é o select. */}
      <select id="filtro" value={valor} onChange={(evento) => aoMudar(evento.target.value)}>
        <option value="todas">Todas</option>
        <option value="Clínica geral">Clínica geral</option>
        <option value="Cardiologia">Cardiologia</option>
        <option value="Dermatologia">Dermatologia</option>
      </select>
    </>
  );
}
