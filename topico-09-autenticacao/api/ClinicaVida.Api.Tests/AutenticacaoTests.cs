// Testes da sessão por cookie: login, rota protegida (401 sem sessão, 200 com sessão) e atributos do cookie.
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

public class AutenticacaoTests(ITestOutputHelper saida) : IDisposable
{
    // Uma fábrica por teste, com banco novo e as duas contas de demonstração.
    private readonly ApiFactory _fabrica = new();

    public void Dispose() => _fabrica.Dispose();

    private ApiFactory Fabrica => _fabrica;

    private static object Credenciais(string email, string senha) => new { email, senha };

    [Fact]
    public async Task Rota_protegida_sem_sessao_responde_401()
    {
        var anonimo = Fabrica.ClienteAnonimo();

        var resposta = await anonimo.GetAsync("/api/consultas");

        // Sem o redirecionamento padrão para uma página de login: uma API devolve o status, e o front decide o que fazer.
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Login_com_senha_certa_responde_200_e_libera_a_rota_protegida()
    {
        var cliente = Fabrica.ClienteAnonimo();

        var login = await cliente.PostAsJsonAsync("/api/auth/login", Credenciais("ana@vidamais.example", "Senha@123"));
        var protegida = await cliente.GetAsync("/api/consultas");

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.Equal(HttpStatusCode.OK, protegida.StatusCode);
    }

    [Fact]
    public async Task Login_com_senha_errada_responde_401_e_nao_cria_cookie()
    {
        var cliente = Fabrica.ClienteAnonimo();

        var login = await cliente.PostAsJsonAsync("/api/auth/login", Credenciais("ana@vidamais.example", "outra-senha"));

        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
        Assert.False(login.Headers.Contains("Set-Cookie"));
        Assert.Equal(HttpStatusCode.Unauthorized, (await cliente.GetAsync("/api/consultas")).StatusCode);
    }

    [Fact]
    public async Task Email_desconhecido_e_senha_errada_recebem_a_mesma_resposta()
    {
        var cliente = Fabrica.ClienteAnonimo();

        var desconhecido = await cliente.PostAsJsonAsync("/api/auth/login", Credenciais("ninguem@vidamais.example", "Senha@123"));
        var errada = await cliente.PostAsJsonAsync("/api/auth/login", Credenciais("ana@vidamais.example", "x"));

        // Resposta igual nos dois casos: quem tenta adivinhar não descobre quais e-mails têm conta.
        Assert.Equal(await desconhecido.Content.ReadAsStringAsync(), await errada.Content.ReadAsStringAsync());
        Assert.Equal(desconhecido.StatusCode, errada.StatusCode);
    }

    [Fact]
    public async Task Cookie_de_sessao_tem_HttpOnly_Secure_e_SameSite()
    {
        var cliente = Fabrica.ClienteAnonimo();

        var login = await cliente.PostAsJsonAsync("/api/auth/login", Credenciais("ana@vidamais.example", "Senha@123"));
        var cabecalho = login.Headers.GetValues("Set-Cookie").Single();
        // A linha literal do cabeçalho vai para a saída do teste: a aula a cita.
        saida.WriteLine("Set-Cookie: " + cabecalho);

        var atributos = cabecalho.Split(';').Select(parte => parte.Trim().ToLowerInvariant()).ToList();
        Assert.StartsWith("clinica.sessao=", cabecalho);
        Assert.Contains("httponly", atributos);
        Assert.Contains("secure", atributos);
        Assert.Contains("samesite=lax", atributos);
    }

    [Fact]
    public async Task Valor_do_cookie_nao_contem_o_email_nem_o_nome_do_paciente()
    {
        var cliente = Fabrica.ClienteAnonimo();

        var login = await cliente.PostAsJsonAsync("/api/auth/login", Credenciais("ana@vidamais.example", "Senha@123"));
        var valor = login.Headers.GetValues("Set-Cookie").Single().Split(';')[0];

        // O ticket vai criptografado: o navegador carrega o cookie, mas não lê as claims dentro dele.
        Assert.DoesNotContain("ana", valor, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Souza", valor);
    }

    [Fact]
    public async Task Logout_apaga_o_cookie_e_a_rota_volta_a_responder_401()
    {
        var cliente = Fabrica.ClienteAutenticado();

        var saiu = await cliente.PostAsync("/api/auth/logout", null);
        var depois = await cliente.GetAsync("/api/consultas");

        Assert.Equal(HttpStatusCode.NoContent, saiu.StatusCode);
        // O cookie expirado no Set-Cookie faz o cliente descartá-lo.
        Assert.Contains("expires=Thu, 01 Jan 1970", saiu.Headers.GetValues("Set-Cookie").Single());
        Assert.Equal(HttpStatusCode.Unauthorized, depois.StatusCode);
    }

    [Fact]
    public async Task Eu_devolve_o_email_de_quem_esta_logado()
    {
        var cliente = Fabrica.ClienteAutenticado("bruno@vidamais.example");

        var corpo = await cliente.GetStringAsync("/api/auth/eu");

        Assert.Contains("bruno@vidamais.example", corpo);
    }
}
