using ClinicaVida.Api.Repositorios;

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
// AddSingleton: um único repositório para a API inteira, entregue a quem pedir a interface.
// Trocar ConsultaRepositorioEmMemoria por outra classe muda o armazenamento sem mexer no controller.
builder.Services.AddSingleton<IConsultaRepositorio, ConsultaRepositorioEmMemoria>();

var app = builder.Build();

// UseCors acrescenta os cabeçalhos CORS às respostas e responde sozinho ao preflight (OPTIONS).
app.UseCors();
// MapControllers liga cada rota ([Route], [HttpGet], [HttpPost]...) ao método do controller.
app.MapControllers();
// Run liga o servidor na porta do launchSettings.json (5084) e espera até Ctrl+C.
app.Run();

// Classe parcial exposta para que WebApplicationFactory<Program> encontre o ponto de entrada.
public partial class Program { }
