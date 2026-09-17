var builder = WebApplication.CreateBuilder(args);

// Registra os servicos no contêiner de injecao de dependencia.
builder.Services.AddControllers();
// Saiba mais sobre a configuracao do OpenAPI em https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configura o pipeline de requisicoes HTTP.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Classe parcial exposta para que WebApplicationFactory<Program>
// consiga encontrar o ponto de entrada nos testes de integracao.
public partial class Program { }
