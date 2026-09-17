// Testes de integracao do endpoint de consultas da Clinica Vida+.
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class ConsultasEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _cliente;

    public ConsultasEndpointTests(WebApplicationFactory<Program> fabrica)
        => _cliente = fabrica.CreateClient();

    [Fact]
    public async Task ListarConsultasRespondeDuzentos()
    {
        var resposta = await _cliente.GetAsync("/api/consultas");
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task ListarConsultasDevolveJsonComPeloMenosUmaConsulta()
    {
        var consultas = await _cliente
            .GetFromJsonAsync<List<Consulta>>("/api/consultas");
        Assert.NotNull(consultas);
        Assert.NotEmpty(consultas);
    }

    [Fact]
    public async Task ConsultaInexistenteRespondeQuatrocentosEQuatro()
    {
        var resposta = await _cliente.GetAsync("/api/consultas/9999");
        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }
}
