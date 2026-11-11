using System.ComponentModel.DataAnnotations;

namespace ClinicaVida.Api.Models;

/// <summary>Uma consulta agendada na Clínica Vida+, com os mesmos campos do tópico 01.</summary>
// Modelo: os dados de uma consulta. Cada propriedade vira uma chave do JSON.
public class Consulta
{
    // Id vira "id" no JSON; quem escolhe o número é o repositório, no POST.
    public int Id { get; set; }

    // [Required]: o [ApiController] responde 400 quando o paciente chega vazio ou ausente.
    [Required(ErrorMessage = "Informe o paciente.")]
    public string Paciente { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a especialidade.")]
    public string Especialidade { get; set; } = string.Empty;

    // DateTime sai no JSON como "2026-10-05T09:00:00", igual ao tópico 01.
    public DateTime DataHora { get; set; }
}
