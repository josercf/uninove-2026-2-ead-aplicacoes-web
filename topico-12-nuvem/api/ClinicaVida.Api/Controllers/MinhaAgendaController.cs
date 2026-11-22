using ClinicaVida.Api.Models;
using ClinicaVida.Api.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaVida.Api.Controllers;

// Rota com autorização por recurso: estar logado não basta, a consulta precisa ser da pessoa logada.
[Authorize]
[ApiController]
[Route("api/minha-agenda")]
public class MinhaAgendaController(IConsultaRepositorio repositorio) : ControllerBase
{
    // GET /api/minha-agenda/2: 200 para o dono, 403 para outro usuário logado, 404 se a consulta não existe.
    [HttpGet("{id:int}")]
    public ActionResult<Consulta> Buscar(int id)
    {
        var consulta = repositorio.Buscar(id);
        if (consulta is null) return NotFound();

        // Compara o paciente da consulta com a claim gravada no login. Só essa linha separa 200 de 403.
        var dono = User.FindFirst("paciente")?.Value;
        return consulta.Paciente == dono ? Ok(consulta) : Forbid();
    }
}
