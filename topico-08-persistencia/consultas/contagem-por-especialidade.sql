-- SELECT: cada linha do resultado traz o nome da especialidade e a contagem de consultas.
SELECT e.Nome AS especialidade, COUNT(*) AS total
-- FROM: lê a tabela Consultas, com o apelido c.
FROM Consultas c
-- JOIN: junta cada consulta à linha de Especialidades cujo Id é igual ao EspecialidadeId da consulta.
JOIN Especialidades e ON e.Id = c.EspecialidadeId
-- GROUP BY: reúne num grupo as linhas com o mesmo nome; COUNT(*) conta as linhas de cada grupo.
GROUP BY e.Nome
-- ORDER BY: maior total primeiro; no empate, ordem alfabética do nome.
ORDER BY total DESC, e.Nome;
