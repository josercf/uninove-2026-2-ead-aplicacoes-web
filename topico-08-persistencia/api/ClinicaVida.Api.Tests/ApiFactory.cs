using ClinicaVida.Api.Dados;
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
