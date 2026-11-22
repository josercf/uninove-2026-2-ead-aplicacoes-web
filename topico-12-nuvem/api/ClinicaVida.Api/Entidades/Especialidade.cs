namespace ClinicaVida.Api.Entidades;

// Entidade: uma linha da tabela Especialidades. "Cardiologia" é gravada uma vez e referenciada pelas consultas.
public class Especialidade
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
}
