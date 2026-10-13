namespace ClinicaVida.Api.Models;

/// <summary>Uma consulta agendada na Clínica Vida+.</summary>
public class Consulta
{
    public int Id { get; set; }
    public string Paciente { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
}
