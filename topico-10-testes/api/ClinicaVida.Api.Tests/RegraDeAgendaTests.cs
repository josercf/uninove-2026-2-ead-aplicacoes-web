using ClinicaVida.Api.Agenda;

// Testes de unidade: chamam a regra direto, sem servidor, sem banco e sem rede. São os mais rápidos da pirâmide.
public class RegraDeAgendaTests
{
    // Theory roda o mesmo método uma vez por InlineData; cada linha é um caso de borda do horário de atendimento.
    [Theory]
    [InlineData(2026, 10, 5, 8, 0, true)]    // segunda, abertura: o limite inferior vale
    [InlineData(2026, 10, 5, 17, 59, true)]  // segunda, um minuto antes do fechamento
    [InlineData(2026, 10, 5, 7, 59, false)]  // segunda, um minuto antes da abertura
    [InlineData(2026, 10, 5, 18, 0, false)]  // segunda, fechamento: o limite superior não vale
    [InlineData(2026, 10, 9, 12, 0, true)]   // sexta, meio-dia
    [InlineData(2026, 10, 10, 9, 0, false)]  // sábado
    [InlineData(2026, 10, 11, 9, 0, false)]  // domingo
    public void Horario_de_atendimento_vai_de_segunda_a_sexta_das_8h_as_18h(int ano, int mes, int dia, int hora, int minuto, bool esperado)
    {
        var instante = new DateTime(ano, mes, dia, hora, minuto, 0);

        Assert.Equal(esperado, RegraDeAgenda.EstaNoHorarioDeAtendimento(instante));
    }

    [Fact]
    public void Mensagem_de_recusa_informa_a_janela_de_atendimento()
    {
        Assert.Equal("Consultas de segunda a sexta, das 08:00 às 18:00.", RegraDeAgenda.MensagemDeRecusa);
    }
}
