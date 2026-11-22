using ClinicaVida.Api.Dados;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// Fábrica da API para os testes: troca o arquivo SQLite por um banco que vive só na memória.
public class ApiFactory : WebApplicationFactory<Program>
{
    // Um banco SQLite em memória some quando a conexão fecha; por isso esta fica aberta
    // enquanto a fábrica existir, e todas as requisições do teste enxergam o mesmo banco.
    private readonly SqliteConnection _conexao = new("DataSource=:memory:");

    public ApiFactory() => _conexao.Open();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        => builder.ConfigureServices(servicos =>
        {
            // Remove o contexto registrado pelo Program.cs (arquivo clinica.db) e registra outro,
            // apontando para a conexão aberta. O Program.cs aplica as migrações ao subir.
            servicos.RemoveAll<DbContextOptions<ClinicaContexto>>();
            servicos.AddDbContext<ClinicaContexto>(opcoes => opcoes.UseSqlite(_conexao));
        });

    // Opções dos clientes de teste: endereço https, para o navegador simulado aceitar o cookie Secure,
    // e sem redirecionamento automático, para o teste ver o status que a API devolveu de fato.
    private static WebApplicationFactoryClientOptions Opcoes() => new()
    {
        BaseAddress = new Uri("https://localhost"),
        AllowAutoRedirect = false,
        HandleCookies = true,
    };

    // Cliente sem sessão: não passou pelo login, logo não tem cookie.
    public HttpClient ClienteAnonimo() => CreateClient(Opcoes());

    // Cliente que já fez o login; o HttpClient guarda o cookie e o envia nas chamadas seguintes.
    public HttpClient ClienteAutenticado(string email = "ana@vidamais.example")
    {
        var cliente = ClienteAnonimo();
        var resposta = cliente.PostAsJsonAsync("/api/auth/login",
            new { email, senha = ClinicaVida.Api.Seguranca.UsuariosIniciais.SenhaDeExemplo }).GetAwaiter().GetResult();
        resposta.EnsureSuccessStatusCode();
        return cliente;
    }

    // Cria um contexto sobre o mesmo banco da API, para testes que olham as tabelas sem passar pelo HTTP.
    public ClinicaContexto NovoContexto()
    {
        var escopo = Services.CreateScope();
        return escopo.ServiceProvider.GetRequiredService<ClinicaContexto>();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _conexao.Dispose();
    }
}
