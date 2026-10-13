using ClinicaVida.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaVida.Api.Controllers;

// [ApiController] liga as convenções de API, como o 404 descrito em application/problem+json.
[ApiController]
// [controller] vira "Consultas", o nome da classe sem o sufixo Controller: rota api/Consultas.
[Route("api/[controller]")]
// ControllerBase entrega os métodos Ok() e NotFound(), que montam a resposta HTTP.
public class ConsultasController : ControllerBase
{
    // Dados em memória. A persistência entra em um tópico futuro.
    // static readonly: uma só lista para a API inteira, recriada a cada partida do programa.
    private static readonly List<Consulta> Agenda =
    // Os colchetes criam a lista já com os itens escritos dentro deles.
    [
        // new() cria uma Consulta e preenche, entre chaves, as propriedades do modelo.
        new() { Id = 1, Paciente = "Ana Souza", Especialidade = "Clínica geral",
                DataHora = new DateTime(2026, 10, 5, 9, 0, 0) },
        new() { Id = 2, Paciente = "Bruno Lima", Especialidade = "Cardiologia",
                DataHora = new DateTime(2026, 10, 5, 10, 30, 0) },
    ];

    // GET /api/consultas devolve a agenda inteira.
    [HttpGet]
    public ActionResult<IEnumerable<Consulta>> Listar() => Ok(Agenda);

    // GET /api/consultas/1 devolve uma consulta, ou 404 quando não existe.
    // {id:int}: o trecho depois de /api/consultas/ precisa ser um número inteiro.
    [HttpGet("{id:int}")]
    // O número do caminho chega ao método no parâmetro id.
    public ActionResult<Consulta> Buscar(int id)
    {
        // FirstOrDefault: a primeira consulta com esse Id, ou null se nenhuma tiver.
        var consulta = Agenda.FirstOrDefault(c => c.Id == id);
        // ? : escolhe pela condição: null vira NotFound() (404); o resto, Ok (200).
        return consulta is null ? NotFound() : Ok(consulta);
    }
}
