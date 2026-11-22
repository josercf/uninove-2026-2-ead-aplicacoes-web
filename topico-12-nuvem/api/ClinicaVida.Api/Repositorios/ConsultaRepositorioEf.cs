using ClinicaVida.Api.Dados;
using ClinicaVida.Api.Entidades;
using ClinicaVida.Api.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVida.Api.Repositorios;

// Implementação da mesma interface do tópico 06, agora com EF Core e SQLite: os dados sobrevivem ao reinício da API.
public class ConsultaRepositorioEf(ClinicaContexto banco) : IConsultaRepositorio
{
    // Projeção das três tabelas para o Consulta do contrato JSON, que continua com paciente e especialidade em texto.
    // O EF traduz o Select em um SELECT com JOIN; nenhuma entidade sai daqui.
    private IQueryable<Consulta> Projetar() => banco.Consultas.AsNoTracking()
        .OrderBy(c => c.Id)
        .Select(c => new Consulta
        {
            Id = c.Id,
            Paciente = c.Paciente.Nome,
            Especialidade = c.Especialidade.Nome,
            DataHora = c.DataHora,
        });

    public IReadOnlyList<Consulta> Listar() => Projetar().ToList();

    public Consulta? Buscar(int id) => Projetar().FirstOrDefault(c => c.Id == id);

    public Consulta Adicionar(Consulta nova)
    {
        var registro = new ConsultaAgendada
        {
            Paciente = PacienteDe(nova.Paciente),
            Especialidade = EspecialidadeDe(nova.Especialidade),
            DataHora = nova.DataHora,
        };
        banco.Consultas.Add(registro);
        Salvar();
        // Depois do SaveChanges, o registro já tem o Id que o banco escolheu.
        nova.Id = registro.Id;
        return nova;
    }

    public bool Atualizar(int id, Consulta dados)
    {
        // Find busca pela chave primária; devolve null quando a linha não existe.
        var registro = banco.Consultas.Find(id);
        if (registro is null) return false;
        registro.Paciente = PacienteDe(dados.Paciente);
        registro.Especialidade = EspecialidadeDe(dados.Especialidade);
        registro.DataHora = dados.DataHora;
        Salvar();
        return true;
    }

    public bool Remover(int id)
    {
        var registro = banco.Consultas.Find(id);
        if (registro is null) return false;
        banco.Consultas.Remove(registro);
        Salvar();
        return true;
    }

    // Reaproveita a linha do paciente com esse nome ou prepara uma nova. Entre o SELECT e o INSERT,
    // outra requisição pode criar o mesmo nome; o índice único recusa o segundo e a API responde 409.
    private Paciente PacienteDe(string nome) =>
        banco.Pacientes.FirstOrDefault(p => p.Nome == nome) ?? new Paciente { Nome = nome };

    private Especialidade EspecialidadeDe(string nome) =>
        banco.Especialidades.FirstOrDefault(e => e.Nome == nome) ?? new Especialidade { Nome = nome };

    // SaveChanges grava tudo o que mudou numa única transação: ou entram a consulta e as linhas de apoio, ou nada entra.
    private void Salvar()
    {
        try { banco.SaveChanges(); }
        // 2067 é o código estendido do SQLite para UNIQUE constraint failed.
        catch (DbUpdateException e) when (e.InnerException is SqliteException { SqliteExtendedErrorCode: 2067 })
        {
            throw new ConflitoDeAgendaException(e);
        }
    }
}
