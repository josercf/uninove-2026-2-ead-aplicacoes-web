namespace ClinicaVida.Api.Models;

/// <summary>Uma consulta agendada na Clínica Vida+.</summary>
// Modelo: só os dados de uma consulta. Cada propriedade vira uma chave do JSON.
public class Consulta
{
    // Id vira "id" no JSON; é o número usado em /api/consultas/{id}.
    public int Id { get; set; }
    // = string.Empty: o texto começa vazio, para nunca valer null.
    public string Paciente { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    // DateTime guarda data e hora juntas; no JSON sai como "2026-10-05T09:00:00".
    public DateTime DataHora { get; set; }
}
