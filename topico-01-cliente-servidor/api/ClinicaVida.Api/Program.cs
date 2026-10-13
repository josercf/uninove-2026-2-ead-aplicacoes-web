// Fase 1, montagem: o builder reúne configuração, logs e serviços antes de a API subir.
// CreateBuilder lê appsettings.json, appsettings.{ambiente}.json e as variáveis de ambiente.
var builder = WebApplication.CreateBuilder(args);

// Registra os serviços no contêiner de injeção de dependência.
// AddControllers: habilita os controllers do projeto, como ConsultasController.
builder.Services.AddControllers();
// AddOpenApi: gera a descrição da API em JSON (https://aka.ms/aspnet/openapi).
builder.Services.AddOpenApi();

// Build encerra a montagem e cria o objeto app, que vai atender as requisições.
var app = builder.Build();

// Fase 2, pipeline: o que acontece com cada requisição, na ordem em que está escrito.
// IsDevelopment vale true com ASPNETCORE_ENVIRONMENT=Development, do launchSettings.json.
if (app.Environment.IsDevelopment())
{
    // MapOpenApi publica a descrição da API no caminho /openapi/v1.json.
    app.MapOpenApi();
}
// UseHttpsRedirection responde 307 a um pedido http, apontando o endereço https.
app.UseHttpsRedirection();
// UseAuthorization confere as regras de acesso; o tópico 01 não declara nenhuma.
app.UseAuthorization();
// MapControllers liga cada rota ([Route], [HttpGet]) ao método do controller.
app.MapControllers();
// Run liga o servidor Kestrel nas portas do launchSettings.json e espera até Ctrl+C.
app.Run();

// Classe parcial exposta para que WebApplicationFactory<Program>
// consiga encontrar o ponto de entrada nos testes de integração.
public partial class Program { }
