using ClinicaVida.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaVida.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultasController : ControllerBase
{
    // Dados em memoria. A persistencia entra em um topico futuro.
    private static readonly List<Consulta> Agenda =
    [
        new() { Id = 1, Paciente = "Ana Souza", Especialidade = "Clinica geral",
                DataHora = new DateTime(2026, 10, 5, 9, 0, 0) },
        new() { Id = 2, Paciente = "Bruno Lima", Especialidade = "Cardiologia",
                DataHora = new DateTime(2026, 10, 5, 10, 30, 0) },
    ];

    // GET /api/consultas devolve a agenda inteira.
    [HttpGet]
    public ActionResult<IEnumerable<Consulta>> Listar() => Ok(Agenda);

    // GET /api/consultas/1 devolve uma consulta, ou 404 quando nao existe.
    [HttpGet("{id:int}")]
    public ActionResult<Consulta> Buscar(int id)
    {
        var consulta = Agenda.FirstOrDefault(c => c.Id == id);
        return consulta is null ? NotFound() : Ok(consulta);
    }
}
