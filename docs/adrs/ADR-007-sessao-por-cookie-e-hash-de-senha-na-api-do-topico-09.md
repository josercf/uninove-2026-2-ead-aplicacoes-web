# ADR-007: Sessão por cookie e hash de senha na API do tópico 09

**Data:** 22/11/2026
**Status:** Aceita
**Decisores:** José Romualdo

## Contexto

A aula 08 deixou a API da Clínica Vida+ com banco SQLite e sem nenhum controle de acesso: qualquer cliente lê e altera todas as consultas. A aula 09 precisa de login, de uma rota protegida que responda 401 sem sessão e 200 com sessão, de senha armazenada com hash e sal, de um cookie com `HttpOnly`, `Secure` e `SameSite`, e de um exemplo de XSS vindo do alerta de `innerHTML` da aula 04. O aluno roda tudo sem instalar serviço externo.

## Decisão

O `topico-09-autenticacao` copia a API do `topico-08-persistencia`, acrescenta a tabela `Usuarios` e protege `/api/consultas` com autenticação por cookie do ASP.NET Core (`AddCookie`). A senha é gravada com `PasswordHasher<Usuario>` (PBKDF2, HMAC-SHA512, 100.000 iterações, sal de 16 bytes). O XSS é demonstrado em JavaScript puro, com testes em jsdom.

## Motivações

- **Cookie em vez de JWT:** o cookie com `HttpOnly` fica fora do alcance do JavaScript da página, e o navegador o envia sozinho. O JWT guardado em `localStorage` ficaria legível por um XSS, o que enfraqueceria a demonstração do subtema 3.
- **`PasswordHasher` do ASP.NET Core:** vem no framework (sem pacote novo), gera sal aleatório, embute algoritmo e iterações no próprio texto e permite aumentar o custo no futuro. Não se implementa criptografia própria.
- **Cópia da API:** mesma razão das ADR-005 e ADR-006. Os tópicos 06 a 08 continuam idênticos ao que as aulas citam.
- **401 e 403 em vez de redirecionamento:** os eventos `OnRedirectToLogin` e `OnRedirectToAccessDenied` devolvem o status, porque o cliente de uma API é um `fetch`, e não um navegador seguindo links.
- **Rota `/api/minha-agenda` ao lado de `/api/consultas`:** `/api/consultas` só exige sessão e deixa Ana ler a consulta do Bruno, o que o teste registra como limite da autenticação. `/api/minha-agenda/{id}` compara o dono da consulta com o usuário logado e responde 403.
- **Contas de demonstração criadas ao subir:** Ana e Bruno recebem a mesma senha pública (`Senha@123`), de propósito, para o teste comparar os dois hashes gravados.
- **`SameSite=Lax`:** barra o envio do cookie em POST vindo de outro site e mantém a navegação por link funcionando.

## Riscos conhecidos

- **Senha de exemplo pública no código:** vale só para estudo. Mitigação: o README avisa que um sistema real não cria conta com senha fixa.
- **Sem limite de tentativas de login:** um atacante pode testar senhas sem bloqueio. Mitigação: a aula declara o limite; o exemplo não implementa bloqueio.
- **Logout só apaga o cookie no cliente:** o ticket é autocontido, e uma cópia roubada vale até expirar (30 minutos sem uso). Mitigação: a aula declara o limite.
- **Cookie `Secure` e HTTP:** o Safari não guarda cookie `Secure` vindo de `http://localhost`. Mitigação: o perfil `https` do `launchSettings.json` sobe também em `https://localhost:7084`, e os testes usam endereço `https`.
- **Sem proteção contra CSRF além do `SameSite`:** não há token antifalsificação. Mitigação: a aula declara o limite.
- **Nome de paciente como chave de autorização:** a rota `/api/minha-agenda` compara nomes, porque o contrato JSON do semestre traz o nome em texto. Num sistema real, a comparação seria por id.

## Consequências

Positivas: cada atributo do cookie e cada garantia da senha é medido por teste ou por comando; o limite da autenticação aparece em dois testes lado a lado (200 em `/api/consultas/2`, 403 em `/api/minha-agenda/2`).

Negativas: os 29 testes herdados passam a fazer login antes (`ApiFactory.ClienteAutenticado`); a API tem duas rotas de leitura de consulta; a quarta cópia da API divide a porta 5084 com as anteriores.

## ADRs relacionadas

ADR-004 (repositório atrás de interface), ADR-005 (API do tópico 07 como cópia com CORS), ADR-006 (banco SQLite com EF Core).
