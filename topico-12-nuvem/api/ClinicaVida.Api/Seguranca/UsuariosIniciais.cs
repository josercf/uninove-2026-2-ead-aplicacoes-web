using ClinicaVida.Api.Dados;
using ClinicaVida.Api.Entidades;
using Microsoft.AspNetCore.Identity;

namespace ClinicaVida.Api.Seguranca;

// Cria as contas de demonstração na primeira subida. A senha de exemplo é pública e só vale para estudo.
public static class UsuariosIniciais
{
    public const string SenhaDeExemplo = "Senha@123";

    public static void Garantir(ClinicaContexto banco, IPasswordHasher<Usuario> hasher)
    {
        // Any: se já existe conta, o banco foi preparado antes e nada é recriado.
        if (banco.Usuarios.Any()) return;

        // Ana e Bruno recebem a MESMA senha de propósito: o teste compara os dois hashes gravados.
        foreach (var (email, pacienteId) in new[] { ("ana@vidamais.example", 1), ("bruno@vidamais.example", 2) })
        {
            var usuario = new Usuario { Email = email, PacienteId = pacienteId };
            // HashPassword sorteia um sal novo a cada chamada e devolve o texto pronto para gravar.
            usuario.SenhaHash = hasher.HashPassword(usuario, SenhaDeExemplo);
            banco.Usuarios.Add(usuario);
        }
        banco.SaveChanges();
    }
}
