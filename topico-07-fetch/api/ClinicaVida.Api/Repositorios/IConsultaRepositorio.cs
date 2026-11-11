using ClinicaVida.Api.Models;

namespace ClinicaVida.Api.Repositorios;

// Interface: a lista de operações que o controller usa, sem dizer onde os dados ficam.
// Hoje a implementação guarda a agenda em memória; um tópico futuro troca por banco de dados.
public interface IConsultaRepositorio
{
    // Todas as consultas, em ordem de id.
    IReadOnlyList<Consulta> Listar();
    // Consulta? : a consulta com esse id, ou null quando ela não existe.
    Consulta? Buscar(int id);
    // Guarda uma consulta nova e devolve a mesma consulta, já com o id escolhido.
    Consulta Adicionar(Consulta nova);
    // true quando a consulta existia e foi substituída; false quando o id não existe.
    bool Atualizar(int id, Consulta dados);
    // true quando a consulta existia e foi removida; false quando o id não existe.
    bool Remover(int id);
}
