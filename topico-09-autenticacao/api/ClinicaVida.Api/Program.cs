using ClinicaVida.Api.Dados;
using ClinicaVida.Api.Entidades;
using ClinicaVida.Api.Repositorios;
using ClinicaVida.Api.Seguranca;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
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
    .AllowAnyMethod()
    // AllowCredentials: sem ele o navegador não envia nem aceita o cookie de sessão em chamadas entre origens.
    .AllowCredentials()));
// AddDbContext: registra o contexto do EF Core, apontando para o arquivo SQLite da configuração.
// O padrão é clinica.db na pasta de execução; o arquivo nasce na primeira subida da API.
builder.Services.AddDbContext<ClinicaContexto>(opcoes => opcoes.UseSqlite(
    builder.Configuration.GetConnectionString("Clinica") ?? "Data Source=clinica.db"));
// AddScoped: um repositório por requisição, porque o contexto do banco também vive uma requisição.
// O controller continua pedindo IConsultaRepositorio; só a classe registrada mudou.
builder.Services.AddScoped<IConsultaRepositorio, ConsultaRepositorioEf>();

// AddAuthentication + AddCookie: a sessão vive num cookie que a própria API emite e o navegador devolve.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opcoes =>
    {
        opcoes.Cookie.Name = "clinica.sessao";
        // HttpOnly: o JavaScript da página não lê o cookie, o que limita o estrago de um XSS.
        opcoes.Cookie.HttpOnly = true;
        // Secure: o navegador só envia o cookie por HTTPS, e a rede aberta não o expõe.
        opcoes.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        // SameSite=Lax: o navegador deixa de enviar o cookie em requisições disparadas por outro site, exceto ao seguir um link; configurado contra o CSRF, sem teste no repositório.
        opcoes.Cookie.SameSite = SameSiteMode.Lax;
        // A sessão expira após 30 minutos sem uso; passada metade do prazo, a próxima requisição recebe um cookie renovado.
        opcoes.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        opcoes.SlidingExpiration = true;
        // Em API, a falta de sessão vira 401 e a falta de permissão vira 403, em vez do redirecionamento padrão para uma página de login.
        opcoes.Events.OnRedirectToLogin = contexto => { contexto.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; };
        opcoes.Events.OnRedirectToAccessDenied = contexto => { contexto.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; };
    });
// AddAuthorization: habilita o atributo [Authorize] dos controllers.
builder.Services.AddAuthorization();
// PasswordHasher: gera e confere hashes de senha com PBKDF2, sal aleatório e várias iterações.
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

var app = builder.Build();

// Migrate cria o banco, se ainda não existe, e aplica as migrações pendentes antes de a API atender.
using (var escopo = app.Services.CreateScope())
{
    var banco = escopo.ServiceProvider.GetRequiredService<ClinicaContexto>();
    banco.Database.Migrate();
    // As contas de demonstração nascem junto com o banco, com a senha já em hash.
    UsuariosIniciais.Garantir(banco, escopo.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>());
}

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
// UseAuthentication lê o cookie e monta o usuário da requisição; UseAuthorization aplica o [Authorize].
// A ordem importa: autenticar antes de autorizar, e os dois depois do CORS.
app.UseAuthentication();
app.UseAuthorization();
// MapControllers liga cada rota ([Route], [HttpGet], [HttpPost]...) ao método do controller.
app.MapControllers();
// Run liga o servidor na porta do launchSettings.json (5084) e espera até Ctrl+C.
app.Run();

// Classe parcial exposta para que WebApplicationFactory<Program> encontre o ponto de entrada.
public partial class Program { }
