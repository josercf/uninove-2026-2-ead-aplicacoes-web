using System.Security.Claims;
using ClinicaVida.Api.Dados;
using ClinicaVida.Api.Entidades;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVida.Api.Controllers;

// Corpo do POST de login: os dois campos que o formulário envia.
public record CredenciaisDeLogin(string Email, string Senha);

[ApiController]
[Route("api/auth")]
public class AutenticacaoController(ClinicaContexto banco, IPasswordHasher<Usuario> hasher) : ControllerBase
{
    // POST /api/auth/login: confere a senha e, se estiver certa, abre a sessão com um cookie.
    [HttpPost("login")]
    public async Task<IActionResult> Entrar([FromBody] CredenciaisDeLogin credenciais)
    {
        var usuario = await banco.Usuarios.Include(u => u.Paciente)
            .FirstOrDefaultAsync(u => u.Email == credenciais.Email);

        // VerifyHashedPassword relê o sal de dentro do hash, refaz o PBKDF2 e compara o resultado.
        // E-mail desconhecido e senha errada recebem a mesma resposta, para não revelar quais contas existem.
        var resultado = usuario is null
            ? PasswordVerificationResult.Failed
            : hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, credenciais.Senha);
        if (usuario is null || resultado == PasswordVerificationResult.Failed)
            return Unauthorized(new { erro = "E-mail ou senha inválidos." });

        // Claims: os dados que o cookie leva. O nome do paciente serve à checagem de dono da consulta.
        var identidade = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim("paciente", usuario.Paciente?.Nome ?? ""),
        ], CookieAuthenticationDefaults.AuthenticationScheme);

        // SignInAsync: o middleware de cookie criptografa as claims e envia o Set-Cookie na resposta.
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidade));
        return Ok(new { email = usuario.Email });
    }

    // POST /api/auth/logout: pede ao navegador que apague o cookie.
    [HttpPost("logout")]
    public async Task<IActionResult> Sair()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    // GET /api/auth/eu: devolve quem está logado; 401 sem cookie válido.
    [Authorize]
    [HttpGet("eu")]
    public IActionResult Eu() => Ok(new { email = User.FindFirstValue(ClaimTypes.Email) });
}
