using ClinicaVida.Api.Dados;
using ClinicaVida.Api.Repositorios;
using Microsoft.EntityFrameworkCore;

// O builder reúne configuração e serviços antes de a API subir.
var builder = WebApplication.CreateBuilder(args);

// AddControllers: habilita os controllers do projeto, como o ConsultasController.
builder.Services.AddControllers();
// AddCors: define a política que diz ao navegador quais origens podem ler as respostas da API.
// Só a origem do servidor de desenvolvimento do front (Vite, porta 5173) entra na lista.
builder.Services.AddCors(opcoes => opcoes.AddDefaultPolicy(politica => politica
    .WithOrigins("http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()));
// AddDbContext: registra o contexto do EF Core, apontando para o arquivo SQLite da configuração.
// O padrão é clinica.db na pasta de execução; o arquivo nasce na primeira subida da API.
builder.Services.AddDbContext<ClinicaContexto>(opcoes => opcoes.UseSqlite(
    builder.Configuration.GetConnectionString("Clinica") ?? "Data Source=clinica.db"));
// AddScoped: um repositório por requisição, porque o contexto do banco também vive uma requisição.
// O controller continua pedindo IConsultaRepositorio; só a classe registrada mudou.
builder.Services.AddScoped<IConsultaRepositorio, ConsultaRepositorioEf>();

var app = builder.Build();

// Migrate cria o banco, se ainda não existe, e aplica as migrações pendentes antes de a API atender.
using (var escopo = app.Services.CreateScope())
    escopo.ServiceProvider.GetRequiredService<ClinicaContexto>().Database.Migrate();

// Middleware: traduz o conflito de agenda lançado pelo repositório em 409 Conflict.
app.Use(async (contexto, proximo) =>
{
    try { await proximo(contexto); }
    catch (ConflitoDeAgendaException e) when (!contexto.Response.HasStarted)
    {
        contexto.Response.StatusCode = StatusCodes.Status409Conflict;
        await contexto.Response.WriteAsJsonAsync(new { erro = e.Message });
    }
});

// UseCors acrescenta os cabeçalhos CORS às respostas e responde sozinho ao preflight (OPTIONS).
app.UseCors();
// MapControllers liga cada rota ([Route], [HttpGet], [HttpPost]...) ao método do controller.
app.MapControllers();
// Run liga o servidor na porta do launchSettings.json (5084) e espera até Ctrl+C.
app.Run();

// Classe parcial exposta para que WebApplicationFactory<Program> encontre o ponto de entrada.
public partial class Program { }
