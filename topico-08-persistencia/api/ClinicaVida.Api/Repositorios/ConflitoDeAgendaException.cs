namespace ClinicaVida.Api.Repositorios;

// Lançada quando o banco recusa uma consulta por violar uma restrição de unicidade
// (o mesmo paciente já tem consulta naquele horário). O Program.cs a traduz em 409.
public class ConflitoDeAgendaException(Exception causa)
    : Exception("O paciente já tem uma consulta nesse horário.", causa);
