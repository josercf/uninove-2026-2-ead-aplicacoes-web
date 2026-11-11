using ClinicaVida.Api.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVida.Api.Dados;

// DbContext: a sessão com o banco. Cada DbSet vira uma tabela, e o OnModelCreating descreve as regras.
public class ClinicaContexto(DbContextOptions<ClinicaContexto> opcoes) : DbContext(opcoes)
{
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Especialidade> Especialidades => Set<Especialidade>();
    public DbSet<ConsultaAgendada> Consultas => Set<ConsultaAgendada>();

    protected override void OnModelCreating(ModelBuilder modelo)
    {
        // HasIndex(...).IsUnique(): vira um índice UNIQUE no banco, que recusa nomes repetidos.
        modelo.Entity<Paciente>().Property(p => p.Nome).IsRequired().HasMaxLength(100);
        modelo.Entity<Paciente>().HasIndex(p => p.Nome).IsUnique();
        modelo.Entity<Especialidade>().Property(e => e.Nome).IsRequired().HasMaxLength(100);
        modelo.Entity<Especialidade>().HasIndex(e => e.Nome).IsUnique();

        // Índice único composto: o mesmo paciente não tem duas consultas no mesmo horário.
        modelo.Entity<ConsultaAgendada>().HasIndex(c => new { c.PacienteId, c.DataHora }).IsUnique();

        // OnDelete(Restrict): o banco recusa apagar um paciente ou uma especialidade que ainda tem consulta.
        modelo.Entity<ConsultaAgendada>().HasOne(c => c.Paciente).WithMany()
            .HasForeignKey(c => c.PacienteId).OnDelete(DeleteBehavior.Restrict);
        modelo.Entity<ConsultaAgendada>().HasOne(c => c.Especialidade).WithMany()
            .HasForeignKey(c => c.EspecialidadeId).OnDelete(DeleteBehavior.Restrict);

        // HasData: as mesmas 3 consultas do tópico 06, gravadas pela migração.
        modelo.Entity<Paciente>().HasData(
            new Paciente { Id = 1, Nome = "Ana Souza" },
            new Paciente { Id = 2, Nome = "Bruno Lima" },
            new Paciente { Id = 3, Nome = "Carla Mendes" });
        modelo.Entity<Especialidade>().HasData(
            new Especialidade { Id = 1, Nome = "Clínica geral" },
            new Especialidade { Id = 2, Nome = "Cardiologia" });
        modelo.Entity<ConsultaAgendada>().HasData(
            new ConsultaAgendada { Id = 1, PacienteId = 1, EspecialidadeId = 1, DataHora = new DateTime(2026, 10, 5, 9, 0, 0) },
            new ConsultaAgendada { Id = 2, PacienteId = 2, EspecialidadeId = 2, DataHora = new DateTime(2026, 10, 5, 10, 30, 0) },
            new ConsultaAgendada { Id = 3, PacienteId = 3, EspecialidadeId = 2, DataHora = new DateTime(2026, 10, 6, 14, 0, 0) });
    }
}
