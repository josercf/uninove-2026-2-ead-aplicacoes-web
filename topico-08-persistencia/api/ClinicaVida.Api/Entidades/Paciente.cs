namespace ClinicaVida.Api.Entidades;

// Entidade: uma linha da tabela Pacientes. Cada paciente aparece uma vez, por mais consultas que tenha.
public class Paciente
{
    // Id é a chave primária: o banco numera as linhas e usa o número para identificá-las.
    public int Id { get; set; }

    // Nome tem restrição de unicidade (configurada no ClinicaContexto): não existem dois pacientes com o mesmo nome.
    public string Nome { get; set; } = string.Empty;
}
