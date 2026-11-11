namespace ClinicaVida.Api.Entidades;

// Entidade: uma linha da tabela Consultas. Guarda os números das linhas de Pacientes e de Especialidades, não os nomes.
public class ConsultaAgendada
{
    public int Id { get; set; }

    // Chaves estrangeiras: PacienteId e EspecialidadeId só aceitam números que existem nas outras tabelas.
    public int PacienteId { get; set; }
    public int EspecialidadeId { get; set; }

    // Propriedades de navegação: o EF preenche estes objetos quando a consulta pede o Include.
    public Paciente Paciente { get; set; } = null!;
    public Especialidade Especialidade { get; set; } = null!;

    public DateTime DataHora { get; set; }
}
