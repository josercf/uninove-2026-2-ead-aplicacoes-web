namespace ClinicaVida.Api.Entidades;

// Entidade: uma linha da tabela Usuarios. A senha nunca é gravada; o banco guarda só o hash com sal.
public class Usuario
{
    public int Id { get; set; }

    // Email tem restrição de unicidade (configurada no ClinicaContexto): é o nome de login.
    public string Email { get; set; } = string.Empty;

    // SenhaHash: texto em Base64 que o PasswordHasher monta com o algoritmo, o número de iterações,
    // um sal aleatório de 16 bytes e o resultado do PBKDF2. Duas contas com a mesma senha geram textos diferentes.
    public string SenhaHash { get; set; } = string.Empty;

    // PacienteId liga a conta ao paciente dono das consultas; a rota /api/minha-agenda usa essa ligação.
    public int? PacienteId { get; set; }
    public Paciente? Paciente { get; set; }
}
