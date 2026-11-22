namespace ClinicaVida.Api.Agenda;

// Regra de negócio sem dependência de banco ou de HTTP: por isso se testa em milissegundos, como unidade.
public static class RegraDeAgenda
{
    // Texto devolvido ao cliente quando a consulta cai fora da janela de atendimento.
    public const string MensagemDeRecusa = "Consultas de segunda a sexta, das 08:00 às 18:00.";

    // A clínica atende de segunda a sexta, a partir das 08:00 (inclusive) e até antes das 18:00 (exclusive).
    public static bool EstaNoHorarioDeAtendimento(DateTime instante)
    {
        // DayOfWeek.Saturday e DayOfWeek.Sunday ficam fora do atendimento.
        if (instante.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) return false;
        // TimeOfDay compara só a hora do dia; >= abre às 08:00 e < fecha às 18:00.
        return instante.TimeOfDay >= TimeSpan.FromHours(8) && instante.TimeOfDay < TimeSpan.FromHours(18);
    }
}
