using System.Net;
using System.Net.Http.Json;
using ClinicaVida.Api.Models;

// Testes de integração: a regra de horário passa pelo controller, pela validação e pela resposta HTTP.
public class HorarioDeAtendimentoTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _cliente;

    // ClienteAutenticado faz o POST de login da Ana e guarda o cookie de sessão para as chamadas seguintes.
    public HorarioDeAtendimentoTests(ApiFactory fabrica) => _cliente = fabrica.ClienteAutenticado();

    private static Consulta NoInstante(DateTime instante) => new()
    {
        Paciente = "Ana Souza",
        Especialidade = "Cardiologia",
        DataHora = instante,
    };

    [Fact]
    public async Task Post_no_sabado_responde_400_com_a_mensagem_da_regra()
    {
        // O POST leva o cookie da Ana e uma consulta num sábado, 10/10/2026, às 09:00.
        var resposta = await _cliente.PostAsJsonAsync("/api/consultas", NoInstante(new DateTime(2026, 10, 10, 9, 0, 0)));

        // O pedido passou pelo login, pelo controller e pela regra: o status é 400.
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        // O corpo, lido como texto, traz a mensagem de RegraDeAgenda.MensagemDeRecusa.
        Assert.Contains("das 08:00 às 18:00", await resposta.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Post_as_18h_responde_400_e_nada_e_gravado()
    {
        var antes = await _cliente.GetFromJsonAsync<List<Consulta>>("/api/consultas");

        var resposta = await _cliente.PostAsJsonAsync("/api/consultas", NoInstante(new DateTime(2026, 10, 14, 18, 0, 0)));
        var depois = await _cliente.GetFromJsonAsync<List<Consulta>>("/api/consultas");

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        Assert.Equal(antes!.Count, depois!.Count);
    }

    [Fact]
    public async Task Put_fora_do_horario_responde_400()
    {
        var resposta = await _cliente.PutAsJsonAsync("/api/consultas/1", NoInstante(new DateTime(2026, 10, 11, 9, 0, 0)));

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Post_as_8h_de_uma_terca_continua_respondendo_201()
    {
        var resposta = await _cliente.PostAsJsonAsync("/api/consultas", NoInstante(new DateTime(2026, 10, 13, 8, 0, 0)));

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
    }
}
