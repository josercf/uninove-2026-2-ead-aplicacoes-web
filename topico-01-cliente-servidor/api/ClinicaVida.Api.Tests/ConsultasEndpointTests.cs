// Testes de integração do endpoint de consultas da Clínica Vida+.
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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

    [Fact]
    public async Task BuscarConsultaExistenteRespondeDuzentosComOsDadosCorretos()
    {
        // Confere o caminho de sucesso de GET /api/consultas/{id}, que até
        // aqui só era exercitado pelo caso de erro (id inexistente).
        var resposta = await _cliente.GetAsync("/api/consultas/1");
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

        var consulta = await resposta.Content.ReadFromJsonAsync<Consulta>();
        Assert.NotNull(consulta);
        Assert.Equal(1, consulta!.Id);
        Assert.Equal("Ana Souza", consulta.Paciente);
        Assert.Equal("Clínica geral", consulta.Especialidade);
        Assert.Equal(new DateTime(2026, 10, 5, 9, 0, 0), consulta.DataHora);
    }

    [Fact]
    public async Task ListarConsultasSerializaComOsNomesDeCampoDaAula()
    {
        // Lê o JSON cru, sem passar pela desserialização para a classe
        // Consulta. Se o nome de um campo mudar na serialização (por
        // exemplo, um atributo JsonPropertyName aplicado por engano), a
        // desserialização normal não acusaria erro algum, só devolveria a
        // propriedade correspondente como valor padrão; o aluno copiaria o
        // nome de campo mostrado na aula sem saber que a API não devolve
        // mais aquele nome. Este teste confere o texto do JSON diretamente,
        // para pegar justamente esse tipo de regressão.
        var jsonCru = await _cliente.GetStringAsync("/api/consultas");

        using var documento = JsonDocument.Parse(jsonCru);
        var primeiraConsulta = documento.RootElement[0];

        Assert.True(primeiraConsulta.TryGetProperty("id", out _));
        Assert.True(primeiraConsulta.TryGetProperty("paciente", out _));
        Assert.True(primeiraConsulta.TryGetProperty("especialidade", out _));
        Assert.True(primeiraConsulta.TryGetProperty("dataHora", out _));
    }
}
