// Testes de configuração sem segredo: nenhum arquivo versionado pode trazer senha, chave ou token.
using System.Text.RegularExpressions;

// Detector usado pelo teste: devolve cada linha que parece carregar uma credencial.
public static class DetectorDeSegredo
{
    // Cada padrão tem um nome, para a mensagem de falha dizer o que foi encontrado.
    private static readonly (string Nome, Regex Padrao)[] Padroes =
    [
        // Password=abc ou Pwd=abc em connection string; ${VAR} e valor vazio não contam.
        ("senha em connection string", new(@"\b(password|pwd)\s*=\s*(?!\$\{)[^;""'\s]+", RegexOptions.IgnoreCase)),
        // "senha": "abc", "secret": "abc", "apiKey": "abc", "token": "abc" em JSON ou YAML.
        ("chave nomeada como segredo", new(@"(senha|password|secret|segredo|api[_-]?key|token|chave)[A-Za-z_\-]*[""']?\s*[:=]\s*[""']?(?!\$\{)[A-Za-z0-9+/_\-\.!@#$%]{6,}", RegexOptions.IgnoreCase)),
        // usuario:senha@servidor dentro de uma URL.
        ("credencial dentro de URL", new(@"[a-z][a-z0-9+.\-]*://[^/\s:@]+:[^/\s:@$]+@", RegexOptions.IgnoreCase)),
        // Identificador de chave de acesso da AWS.
        ("chave de acesso AWS", new(@"\bAKIA[0-9A-Z]{16}\b")),
        // Cabeçalho de chave privada PEM.
        ("chave privada PEM", new(@"-----BEGIN [A-Z ]*PRIVATE KEY-----")),
    ];

    public static IReadOnlyList<string> Encontrar(string texto)
    {
        var achados = new List<string>();
        var numero = 0;
        foreach (var linha in texto.Split('\n'))
        {
            numero++;
            // Linha de comentário não carrega valor de configuração.
            if (linha.TrimStart().StartsWith('#')) continue;
            foreach (var (nome, padrao) in Padroes)
                if (padrao.IsMatch(linha)) achados.Add($"linha {numero}: {nome}");
        }
        return achados;
    }
}

public class SegredoVersionadoTests
{
    // Pasta que o csproj do teste preenche com os arquivos versionados.
    private static string Pasta => Path.Combine(AppContext.BaseDirectory, "versionados");

    public static TheoryData<string> Arquivos()
    {
        var dados = new TheoryData<string>();
        foreach (var caminho in Directory.GetFiles(Pasta)) dados.Add(Path.GetFileName(caminho));
        return dados;
    }

    [Fact]
    public void Os_arquivos_versionados_foram_copiados_para_o_teste()
    {
        // Sem esta conferência, uma pasta vazia faria o teste abaixo passar sem ler nada.
        var nomes = Directory.GetFiles(Pasta).Select(Path.GetFileName).ToList();
        Assert.Contains("appsettings.json", nomes);
        Assert.Contains("appsettings.Development.json", nomes);
        Assert.Contains("compose.yaml", nomes);
        Assert.Contains("env.exemplo", nomes);
    }

    [Theory]
    [MemberData(nameof(Arquivos))]
    public void Arquivo_versionado_nao_traz_segredo(string nome)
    {
        var texto = File.ReadAllText(Path.Combine(Pasta, nome));

        var achados = DetectorDeSegredo.Encontrar(texto);

        Assert.True(achados.Count == 0, $"{nome}: {string.Join("; ", achados)}");
    }

    [Theory]
    [InlineData("\"Clinica\": \"Server=db;User Id=sa;Password=Abc12345\"", "senha em connection string")]
    [InlineData("\"ChaveDaApi\": \"sk_live_51Habcdef123456\"", "chave nomeada como segredo")]
    [InlineData("\"Jwt\": { \"Secret\": \"meu-segredo-longo\" }", "chave nomeada como segredo")]
    [InlineData("Banco: postgres://admin:hunter2@db.exemplo.org/clinica", "credencial dentro de URL")]
    [InlineData("AWS_ACCESS_KEY_ID=AKIAIOSFODNN7EXAMPLE", "chave de acesso AWS")]
    [InlineData("-----BEGIN RSA PRIVATE KEY-----", "chave privada PEM")]
    public void Detector_reconhece_credencial_em_texto_de_exemplo(string linha, string esperado)
    {
        // Regressão do próprio teste: um detector que nunca acusa nada deixaria passar qualquer segredo.
        Assert.Contains(DetectorDeSegredo.Encontrar(linha), a => a.EndsWith(esperado));
    }

    [Theory]
    [InlineData("\"Clinica\": \"Data Source=/dados/clinica.db\"")]
    [InlineData("ConnectionStrings__Clinica: \"Data Source=/dados/clinica.db\"")]
    [InlineData("Cors__Origem: \"${CORS_ORIGEM:-http://localhost:5173}\"")]
    [InlineData("Password=${SENHA_DO_BANCO}")]
    [InlineData("# Password=abc123 aparece só num comentário")]
    [InlineData("\"AllowedHosts\": \"*\"")]
    public void Detector_aceita_configuracao_sem_credencial(string linha)
    {
        Assert.Empty(DetectorDeSegredo.Encontrar(linha));
    }
}
