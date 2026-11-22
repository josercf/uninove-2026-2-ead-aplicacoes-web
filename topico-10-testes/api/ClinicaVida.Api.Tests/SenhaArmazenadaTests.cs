// Testes da senha armazenada: o banco guarda hash com sal, nunca a senha.
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using ClinicaVida.Api.Entidades;
using ClinicaVida.Api.Seguranca;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

public class SenhaArmazenadaTests(ITestOutputHelper saida) : IDisposable
{
    private readonly ApiFactory _fabrica = new();

    public void Dispose() => _fabrica.Dispose();

    // Lê as duas contas direto da tabela Usuarios, sem passar pela API.
    private Dictionary<string, string> HashesGravados()
    {
        _fabrica.ClienteAnonimo(); // força a API a subir e a semear as contas
        using var banco = _fabrica.NovoContexto();
        return banco.Usuarios.ToDictionary(u => u.Email, u => u.SenhaHash);
    }

    [Fact]
    public void Duas_contas_com_a_mesma_senha_gravam_hashes_diferentes()
    {
        var hashes = HashesGravados();

        // As duas contas foram criadas com Senha@123; só o sal aleatório explica a diferença.
        Assert.Equal(2, hashes.Count);
        Assert.NotEqual(hashes["ana@vidamais.example"], hashes["bruno@vidamais.example"]);
        saida.WriteLine("ana   : " + hashes["ana@vidamais.example"]);
        saida.WriteLine("bruno : " + hashes["bruno@vidamais.example"]);
    }

    [Fact]
    public void Banco_nao_guarda_a_senha_em_texto()
    {
        foreach (var hash in HashesGravados().Values)
            Assert.DoesNotContain(UsuariosIniciais.SenhaDeExemplo, hash);
    }

    [Fact]
    public void Cada_hash_confere_com_a_senha_original_e_recusa_outra()
    {
        var hashes = HashesGravados();
        var hasher = _fabrica.Services.GetRequiredService<IPasswordHasher<Usuario>>();
        var usuario = new Usuario();

        foreach (var hash in hashes.Values)
        {
            Assert.Equal(PasswordVerificationResult.Success, hasher.VerifyHashedPassword(usuario, hash, "Senha@123"));
            Assert.Equal(PasswordVerificationResult.Failed, hasher.VerifyHashedPassword(usuario, hash, "senha@123"));
        }
    }

    [Fact]
    public void Hash_carrega_o_algoritmo_as_iteracoes_e_um_sal_de_16_bytes()
    {
        // Formato v3 do PasswordHasher: 1 byte de versão, 4 do PRF, 4 das iterações, 4 do tamanho do sal, sal e subchave.
        var bytes = Convert.FromBase64String(HashesGravados()["ana@vidamais.example"]);
        var iteracoes = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(5, 4));
        var tamanhoDoSal = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(9, 4));
        saida.WriteLine($"versao={bytes[0]} prf={BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(1, 4))} iteracoes={iteracoes} sal={tamanhoDoSal} bytes={bytes.Length}");

        Assert.Equal(0x01, bytes[0]);
        Assert.True(iteracoes >= 100_000, $"iterações: {iteracoes}");
        Assert.Equal(16, tamanhoDoSal);
    }

    [Fact]
    public void Mesma_conta_gera_hash_novo_a_cada_chamada_porque_o_sal_muda()
    {
        var hasher = new PasswordHasher<Usuario>();
        var usuario = new Usuario();

        Assert.NotEqual(hasher.HashPassword(usuario, "Senha@123"), hasher.HashPassword(usuario, "Senha@123"));
    }

    [Fact]
    public void Contraste_hash_sem_sal_repete_o_mesmo_resultado_para_a_mesma_senha()
    {
        // SHA-256 puro, sem sal: é o que NÃO se faz. Duas contas com a mesma senha ficam idênticas na tabela.
        static string Sha256(string texto) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(texto)));

        Assert.Equal(Sha256("Senha@123"), Sha256("Senha@123"));
    }
}
