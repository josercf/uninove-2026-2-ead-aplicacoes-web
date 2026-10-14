using ClinicaVida.Api.Models;
using ClinicaVida.Api.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaVida.Api.Controllers;

// [ApiController]: responde 400 sozinho quando o corpo viola um [Required] do modelo.
[ApiController]
// A rota do recurso é a mesma do tópico 01, escrita por extenso e em minúsculas.
[Route("api/consultas")]
public class ConsultasController : ControllerBase
{
    private readonly IConsultaRepositorio _repositorio;

    // O ASP.NET Core entrega o repositório registrado no Program.cs (injeção de dependência).
    public ConsultasController(IConsultaRepositorio repositorio) => _repositorio = repositorio;

    // GET /api/consultas: a coleção inteira, 200 OK.
    [HttpGet]
    public ActionResult<IEnumerable<Consulta>> Listar() => Ok(_repositorio.Listar());

    // GET /api/consultas/2: uma consulta, 200 OK, ou 404 Not Found.
    [HttpGet("{id:int}")]
    public ActionResult<Consulta> Buscar(int id)
    {
        var consulta = _repositorio.Buscar(id);
        return consulta is null ? NotFound() : Ok(consulta);
    }

    // POST /api/consultas: cria uma consulta, 201 Created.
    [HttpPost]
    // [FromBody]: a consulta chega no corpo da requisição, em JSON.
    public ActionResult<Consulta> Criar([FromBody] Consulta nova)
    {
        var criada = _repositorio.Adicionar(nova);
        // CreatedAtAction: status 201, corpo com a consulta e cabeçalho Location
        // montado a partir da rota da ação Buscar, com o id novo: /api/consultas/4.
        return CreatedAtAction(nameof(Buscar), new { id = criada.Id }, criada);
    }

    // PUT /api/consultas/1: substitui a consulta inteira, 204 No Content, ou 404.
    [HttpPut("{id:int}")]
    public IActionResult Atualizar(int id, [FromBody] Consulta dados)
        // NoContent(): status 204, sem corpo na resposta.
        => _repositorio.Atualizar(id, dados) ? NoContent() : NotFound();

    // DELETE /api/consultas/3: remove a consulta, 204 No Content, ou 404.
    [HttpDelete("{id:int}")]
    public IActionResult Remover(int id)
        => _repositorio.Remover(id) ? NoContent() : NotFound();
}
