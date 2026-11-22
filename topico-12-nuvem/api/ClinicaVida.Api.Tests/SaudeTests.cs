// Testes da verificação de saúde e da configuração por variável de ambiente.
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

public class SaudeTests : IDisposable
{
    private readonly ApiFactory _fabrica = new();

    public void Dispose() => _fabrica.Dispose();

    [Fact]
    public async Task Health_responde_200_sem_login()
    {
        // A plataforma de nuvem chama /health sem cookie; a rota não pode exigir sessão.
        var resposta = await _fabrica.ClienteAnonimo().GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal("Healthy", await resposta.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Rotas_de_negocio_continuam_exigindo_login()
    {
        // Regressão: abrir /health não pode abrir o resto da API.
        var resposta = await _fabrica.ClienteAnonimo().GetAsync("/api/consultas");

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Origem_do_cors_vem_da_configuracao()
    {
        // Cors__Origem no ambiente vira a chave Cors:Origem; UseSetting equivale a ela no teste.
        using var fabrica = _fabrica.WithWebHostBuilder(b => b.UseSetting("Cors:Origem", "https://clinica.exemplo.org"));
        var pedido = new HttpRequestMessage(HttpMethod.Get, "/health");
        pedido.Headers.Add("Origin", "https://clinica.exemplo.org");

        var resposta = await fabrica.CreateClient().SendAsync(pedido);

        Assert.Equal(["https://clinica.exemplo.org"], resposta.Headers.GetValues("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task Origem_antiga_deixa_de_valer_quando_a_configuracao_muda()
    {
        using var fabrica = _fabrica.WithWebHostBuilder(b => b.UseSetting("Cors:Origem", "https://clinica.exemplo.org"));
        var pedido = new HttpRequestMessage(HttpMethod.Get, "/health");
        pedido.Headers.Add("Origin", "http://localhost:5173");

        var resposta = await fabrica.CreateClient().SendAsync(pedido);

        Assert.False(resposta.Headers.Contains("Access-Control-Allow-Origin"));
    }
}
