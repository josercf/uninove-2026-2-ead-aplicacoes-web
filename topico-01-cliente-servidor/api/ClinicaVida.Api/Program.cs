var builder = WebApplication.CreateBuilder(args);

// Registra os serviços no contêiner de injeção de dependência.
builder.Services.AddControllers();
// Saiba mais sobre a configuração do OpenAPI em https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Classe parcial exposta para que WebApplicationFactory<Program>
// consiga encontrar o ponto de entrada nos testes de integração.
public partial class Program { }
