// Testes do modelo relacional: chave estrangeira, restrição de unicidade e contagem por especialidade.
using ClinicaVida.Api.Dados;
using ClinicaVida.Api.Entidades;
using Microsoft.EntityFrameworkCore;

public class ModeloRelacionalTests : IDisposable
{
    private readonly ApiFactory _fabrica = new();
    private readonly ClinicaContexto _banco;

    public ModeloRelacionalTests()
    {
        // O contexto só existe depois de a API subir; CreateClient força esse start.
        _fabrica.CreateClient();
        _banco = _fabrica.NovoContexto();
    }

    public void Dispose()
    {
        _banco.Dispose();
        _fabrica.Dispose();
    }

    [Fact]
    public void Migracao_cria_as_tres_tabelas_com_a_agenda_inicial()
    {
        Assert.Equal(3, _banco.Pacientes.Count());
        Assert.Equal(2, _banco.Especialidades.Count());
        Assert.Equal(3, _banco.Consultas.Count());
    }

    [Fact]
    public void Consulta_navega_ate_o_paciente_e_a_especialidade()
    {
        var consulta = _banco.Consultas
            .Include(c => c.Paciente)
            .Include(c => c.Especialidade)
            .Single(c => c.Id == 2);

        Assert.Equal("Bruno Lima", consulta.Paciente.Nome);
        Assert.Equal("Cardiologia", consulta.Especialidade.Nome);
    }

    [Fact]
    public void Especialidade_repetida_em_duas_consultas_ocupa_uma_linha_so()
    {
        // As consultas 2 e 3 são de Cardiologia: as duas apontam para a mesma linha de Especialidade.
        var ids = _banco.Consultas.Where(c => c.Id == 2 || c.Id == 3)
            .Select(c => c.EspecialidadeId).ToList();

        Assert.Equal(ids[0], ids[1]);
        Assert.Equal(1, _banco.Especialidades.Count(e => e.Nome == "Cardiologia"));
    }

    [Fact]
    public void Chave_estrangeira_recusa_consulta_de_paciente_inexistente()
    {
        _banco.Consultas.Add(new ConsultaAgendada
        {
            PacienteId = 999,
            EspecialidadeId = 1,
            DataHora = new DateTime(2026, 10, 9, 8, 0, 0),
        });

        // O SQLite devolve "FOREIGN KEY constraint failed"; o EF o embrulha em DbUpdateException.
        var erro = Assert.Throws<DbUpdateException>(() => _banco.SaveChanges());
        Assert.Contains("FOREIGN KEY constraint failed", erro.InnerException!.Message);
    }

    [Fact]
    public void Unicidade_recusa_o_mesmo_paciente_no_mesmo_horario()
    {
        // A consulta 1 é de Ana Souza (paciente 1) em 05/10/2026 às 09:00.
        _banco.Consultas.Add(new ConsultaAgendada
        {
            PacienteId = 1,
            EspecialidadeId = 2,
            DataHora = new DateTime(2026, 10, 5, 9, 0, 0),
        });

        var erro = Assert.Throws<DbUpdateException>(() => _banco.SaveChanges());
        Assert.Contains("UNIQUE constraint failed", erro.InnerException!.Message);
    }

    [Fact]
    public void Unicidade_recusa_nome_de_especialidade_repetido()
    {
        _banco.Especialidades.Add(new Especialidade { Nome = "Cardiologia" });

        var erro = Assert.Throws<DbUpdateException>(() => _banco.SaveChanges());
        Assert.Contains("UNIQUE constraint failed", erro.InnerException!.Message);
    }

    [Fact]
    public void Remover_especialidade_com_consulta_e_recusado_pela_chave_estrangeira()
    {
        var cardiologia = _banco.Especialidades.Single(e => e.Nome == "Cardiologia");
        _banco.Especialidades.Remove(cardiologia);

        var erro = Assert.Throws<DbUpdateException>(() => _banco.SaveChanges());
        Assert.Contains("FOREIGN KEY constraint failed", erro.InnerException!.Message);
    }

    [Fact]
    public void Contagem_por_especialidade_soma_as_tres_consultas()
    {
        // GROUP BY traduzido pelo EF: uma linha por especialidade, com a contagem de consultas.
        var contagem = _banco.Consultas
            .GroupBy(c => c.Especialidade.Nome)
            .Select(g => new { Especialidade = g.Key, Total = g.Count() })
            .OrderBy(x => x.Especialidade)
            .ToList();

        Assert.Equal(2, contagem.Count);
        Assert.Equal(("Cardiologia", 2), (contagem[0].Especialidade, contagem[0].Total));
        Assert.Equal(("Clínica geral", 1), (contagem[1].Especialidade, contagem[1].Total));
    }
}
