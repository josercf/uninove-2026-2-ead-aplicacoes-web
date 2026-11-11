// Testes do que só existe com banco: reaproveitar linhas de apoio, não reutilizar ids e ler o que outro contexto gravou.
using System.Net.Http.Json;
using ClinicaVida.Api.Models;

public class PersistenciaTests : IDisposable
{
    private readonly ApiFactory _fabrica = new();
    private readonly HttpClient _cliente;

    public PersistenciaTests() => _cliente = _fabrica.CreateClient();

    public void Dispose() => _fabrica.Dispose();

    private static Consulta Nova(string paciente, string especialidade, int hora) => new()
    {
        Paciente = paciente,
        Especialidade = especialidade,
        DataHora = new DateTime(2026, 10, 8, hora, 0, 0),
    };

    [Fact]
    public async Task Post_de_paciente_existente_reaproveita_a_linha_de_paciente()
    {
        await _cliente.PostAsJsonAsync("/api/consultas", Nova("Ana Souza", "Cardiologia", 8));

        using var banco = _fabrica.NovoContexto();
        Assert.Equal(3, banco.Pacientes.Count());
        Assert.Equal(4, banco.Consultas.Count());
    }

    [Fact]
    public async Task Post_de_especialidade_nova_cria_uma_linha_de_especialidade()
    {
        await _cliente.PostAsJsonAsync("/api/consultas", Nova("Diego Rocha", "Dermatologia", 9));

        using var banco = _fabrica.NovoContexto();
        Assert.Equal(3, banco.Especialidades.Count());
        Assert.Equal(4, banco.Pacientes.Count());
    }

    [Fact]
    public async Task Id_de_consulta_removida_nao_e_reaproveitado()
    {
        await _cliente.DeleteAsync("/api/consultas/3");

        var resposta = await _cliente.PostAsJsonAsync("/api/consultas", Nova("Diego Rocha", "Dermatologia", 9));

        Assert.Equal("/api/consultas/4", resposta.Headers.Location!.AbsolutePath);
    }

    [Fact]
    public async Task Post_do_mesmo_paciente_no_mesmo_horario_responde_409()
    {
        // Ana Souza já tem consulta em 05/10/2026 às 09:00 (a consulta 1 da agenda inicial).
        var repetida = new Consulta
        {
            Paciente = "Ana Souza",
            Especialidade = "Cardiologia",
            DataHora = new DateTime(2026, 10, 5, 9, 0, 0),
        };

        var resposta = await _cliente.PostAsJsonAsync("/api/consultas", repetida);

        Assert.Equal(System.Net.HttpStatusCode.Conflict, resposta.StatusCode);
    }

    [Fact]
    public async Task Conflito_na_transacao_desfaz_tambem_a_especialidade_nova()
    {
        // O PUT troca a consulta 2 para Ana Souza às 09:00 (conflito com a consulta 1) e pede uma
        // especialidade que ainda não existe. A linha de Neurologia só entraria junto com a consulta.
        var conflito = Nova("Ana Souza", "Neurologia", 9);
        conflito.DataHora = new DateTime(2026, 10, 5, 9, 0, 0);

        var resposta = await _cliente.PutAsJsonAsync("/api/consultas/2", conflito);

        Assert.Equal(System.Net.HttpStatusCode.Conflict, resposta.StatusCode);
        using var banco = _fabrica.NovoContexto();
        Assert.Equal(2, banco.Especialidades.Count());
    }
}
