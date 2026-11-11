// Testes do CORS: a API libera só a origem do front de desenvolvimento (Vite, porta 5173).
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

public class CorsTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _fabrica = new();
    private readonly HttpClient _cliente;

    public CorsTests() => _cliente = _fabrica.CreateClient();

    public void Dispose() => _fabrica.Dispose();

    // Monta um GET /api/consultas com o cabeçalho Origin, como o navegador faz em fetch entre origens.
    private static HttpRequestMessage GetComOrigem(string origem)
    {
        var pedido = new HttpRequestMessage(HttpMethod.Get, "/api/consultas");
        pedido.Headers.Add("Origin", origem);
        return pedido;
    }

    [Fact]
    public async Task Origem_do_front_recebe_o_cabecalho_que_libera_a_leitura()
    {
        var resposta = await _cliente.SendAsync(GetComOrigem("http://localhost:5173"));

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal(["http://localhost:5173"], resposta.Headers.GetValues("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task Outra_origem_nao_recebe_o_cabecalho()
    {
        // SendAsync envia o pedido montado por GetComOrigem, com o cabeçalho Origin de uma porta fora da lista.
        var resposta = await _cliente.SendAsync(GetComOrigem("http://localhost:9999"));

        // A API ainda responde 200: quem bloqueia a leitura é o navegador, pela ausência do cabeçalho.
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        // Headers.Contains devolve false quando o cabeçalho não veio na resposta.
        Assert.False(resposta.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task Requisicao_sem_origem_nao_recebe_o_cabecalho()
    {
        var resposta = await _cliente.GetAsync("/api/consultas");

        Assert.False(resposta.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task Preflight_do_DELETE_na_origem_do_front_lista_o_metodo()
    {
        var pedido = new HttpRequestMessage(HttpMethod.Options, "/api/consultas/1");
        pedido.Headers.Add("Origin", "http://localhost:5173");
        pedido.Headers.Add("Access-Control-Request-Method", "DELETE");

        var resposta = await _cliente.SendAsync(pedido);

        Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
        Assert.Equal(["http://localhost:5173"], resposta.Headers.GetValues("Access-Control-Allow-Origin"));
        Assert.Contains("DELETE", string.Join(",", resposta.Headers.GetValues("Access-Control-Allow-Methods")));
    }
}
