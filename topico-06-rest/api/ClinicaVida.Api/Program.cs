using ClinicaVida.Api.Repositorios;

// O builder reúne configuração e serviços antes de a API subir.
var builder = WebApplication.CreateBuilder(args);

// AddControllers: habilita os controllers do projeto, como o ConsultasController.
builder.Services.AddControllers();
// AddSingleton: um único repositório para a API inteira, entregue a quem pedir a interface.
// Trocar ConsultaRepositorioEmMemoria por outra classe muda o armazenamento sem mexer no controller.
builder.Services.AddSingleton<IConsultaRepositorio, ConsultaRepositorioEmMemoria>();

var app = builder.Build();

// MapControllers liga cada rota ([Route], [HttpGet], [HttpPost]...) ao método do controller.
app.MapControllers();
// Run liga o servidor na porta do launchSettings.json (5084) e espera até Ctrl+C.
app.Run();

// Classe parcial exposta para que WebApplicationFactory<Program> encontre o ponto de entrada.
public partial class Program { }
