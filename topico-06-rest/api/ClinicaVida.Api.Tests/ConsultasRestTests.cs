// Testes de integração da API REST de consultas: um teste por verbo e por código de status.
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ClinicaVida.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;

// IDisposable: o xUnit chama Dispose ao fim de cada teste e desliga a API em memória.
public class ConsultasRestTests : IDisposable
{
    // Uma fábrica nova por teste: cada teste começa com as mesmas 3 consultas.
    private readonly WebApplicationFactory<Program> _fabrica = new();
    private readonly HttpClient _cliente;

    // O xUnit cria um objeto desta classe para cada teste; o construtor roda antes de cada um.
    public ConsultasRestTests() => _cliente = _fabrica.CreateClient();

    public void Dispose() => _fabrica.Dispose();

    // Corpo de exemplo para POST e PUT, com os mesmos campos do JSON do tópico 01.
    private static Consulta NovaConsulta(string paciente) => new()
    {
        Paciente = paciente,
        Especialidade = "Dermatologia",
        DataHora = new DateTime(2026, 10, 7, 8, 0, 0),
    };

    [Fact]
    public async Task Get_colecao_responde_200_com_as_3_consultas()
    {
        var resposta = await _cliente.GetAsync("/api/consultas");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var consultas = await resposta.Content.ReadFromJsonAsync<List<Consulta>>();
        Assert.Equal(["Ana Souza", "Bruno Lima", "Carla Mendes"], consultas!.Select(c => c.Paciente));
    }

    [Fact]
    public async Task Get_colecao_mantem_os_nomes_de_campo_do_topico_01()
    {
        // Lê o texto cru: um nome de campo trocado quebraria o front da aula 07.
        var json = await _cliente.GetStringAsync("/api/consultas");
        using var documento = JsonDocument.Parse(json);
        var primeira = documento.RootElement[0];

        Assert.Equal(1, primeira.GetProperty("id").GetInt32());
        Assert.Equal("Ana Souza", primeira.GetProperty("paciente").GetString());
        Assert.Equal("Clínica geral", primeira.GetProperty("especialidade").GetString());
        Assert.Equal("2026-10-05T09:00:00", primeira.GetProperty("dataHora").GetString());
    }

    [Fact]
    public async Task Get_por_id_existente_responde_200_com_a_consulta()
    {
        var consulta = await _cliente.GetFromJsonAsync<Consulta>("/api/consultas/2");

        Assert.Equal("Bruno Lima", consulta!.Paciente);
        Assert.Equal("Cardiologia", consulta.Especialidade);
    }

    [Fact]
    public async Task Get_por_id_inexistente_responde_404()
    {
        var resposta = await _cliente.GetAsync("/api/consultas/99");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task Post_valido_responde_201_com_Location_da_consulta_criada()
    {
        // PostAsJsonAsync converte o objeto em JSON e envia no corpo do POST.
        var resposta = await _cliente.PostAsJsonAsync("/api/consultas", NovaConsulta("Diego Rocha"));

        // 201 Created: o servidor criou um recurso novo.
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        // Location: o cabeçalho com o endereço do recurso criado.
        Assert.Equal("/api/consultas/4", resposta.Headers.Location!.AbsolutePath);
        var criada = await resposta.Content.ReadFromJsonAsync<Consulta>();
        Assert.Equal(4, criada!.Id);
        // O endereço do Location responde com a mesma consulta.
        var lida = await _cliente.GetFromJsonAsync<Consulta>(resposta.Headers.Location);
        Assert.Equal("Diego Rocha", lida!.Paciente);
    }

    [Fact]
    public async Task Post_sem_paciente_responde_400_e_nao_cria_nada()
    {
        var resposta = await _cliente.PostAsJsonAsync("/api/consultas", NovaConsulta(""));

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        var consultas = await _cliente.GetFromJsonAsync<List<Consulta>>("/api/consultas");
        Assert.Equal(3, consultas!.Count);
    }

    [Fact]
    public async Task Put_em_id_existente_responde_204_e_altera_a_consulta()
    {
        // PutAsJsonAsync envia a representação completa que substitui a consulta 1.
        var resposta = await _cliente.PutAsJsonAsync("/api/consultas/1", NovaConsulta("Ana Souza"));

        // 204 No Content: deu certo, e a resposta não tem corpo.
        Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
        var alterada = await _cliente.GetFromJsonAsync<Consulta>("/api/consultas/1");
        Assert.Equal("Dermatologia", alterada!.Especialidade);
    }

    [Fact]
    public async Task Put_em_id_inexistente_responde_404()
    {
        var resposta = await _cliente.PutAsJsonAsync("/api/consultas/99", NovaConsulta("Ana Souza"));

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task Put_sem_paciente_responde_400()
    {
        var resposta = await _cliente.PutAsJsonAsync("/api/consultas/1", NovaConsulta(""));

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Delete_em_id_existente_responde_204_e_remove_a_consulta()
    {
        // DeleteAsync envia DELETE, sem corpo.
        var resposta = await _cliente.DeleteAsync("/api/consultas/3");

        Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
        var depois = await _cliente.GetAsync("/api/consultas/3");
        Assert.Equal(HttpStatusCode.NotFound, depois.StatusCode);
    }

    [Fact]
    public async Task Delete_em_id_inexistente_responde_404()
    {
        var resposta = await _cliente.DeleteAsync("/api/consultas/99");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }
}
