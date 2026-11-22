SELECT e.Nome AS especialidade, COUNT(*) AS total
FROM Consultas c
JOIN Especialidades e ON e.Id = c.EspecialidadeId
GROUP BY e.Nome
ORDER BY total DESC, e.Nome;
