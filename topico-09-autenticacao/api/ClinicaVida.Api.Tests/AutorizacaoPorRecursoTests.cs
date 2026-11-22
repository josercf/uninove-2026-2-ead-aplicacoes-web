// Testes do limite da autenticação: estar logado prova quem é a pessoa, e não o que ela pode ler.
using System.Net;
using System.Net.Http.Json;
using ClinicaVida.Api.Models;

public class AutorizacaoPorRecursoTests : IDisposable
{
    private readonly ApiFactory _fabrica = new();

    public void Dispose() => _fabrica.Dispose();

    [Fact]
    public async Task Autenticacao_sozinha_deixa_Ana_ler_a_consulta_do_Bruno()
    {
        var ana = _fabrica.ClienteAutenticado("ana@vidamais.example");

        // A consulta 2 é do Bruno. /api/consultas só exige sessão, e Ana tem uma: a API responde 200.
        var resposta = await ana.GetAsync("/api/consultas/2");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal("Bruno Lima", (await resposta.Content.ReadFromJsonAsync<Consulta>())!.Paciente);
    }

    [Fact]
    public async Task Rota_com_autorizacao_por_recurso_responde_200_ao_dono()
    {
        var ana = _fabrica.ClienteAutenticado("ana@vidamais.example");

        var resposta = await ana.GetAsync("/api/minha-agenda/1");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task Rota_com_autorizacao_por_recurso_responde_403_a_quem_nao_e_dono()
    {
        var ana = _fabrica.ClienteAutenticado("ana@vidamais.example");

        var resposta = await ana.GetAsync("/api/minha-agenda/2");

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task Rota_com_autorizacao_por_recurso_responde_401_sem_sessao_e_404_para_id_inexistente()
    {
        Assert.Equal(HttpStatusCode.Unauthorized, (await _fabrica.ClienteAnonimo().GetAsync("/api/minha-agenda/1")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _fabrica.ClienteAutenticado().GetAsync("/api/minha-agenda/99")).StatusCode);
    }
}
