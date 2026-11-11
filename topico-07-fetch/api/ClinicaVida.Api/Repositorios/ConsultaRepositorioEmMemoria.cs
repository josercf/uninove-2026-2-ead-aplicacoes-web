using ClinicaVida.Api.Models;

namespace ClinicaVida.Api.Repositorios;

// Implementação da interface com uma lista em memória: os dados somem quando a API para.
public class ConsultaRepositorioEmMemoria : IConsultaRepositorio
{
    // As mesmas 3 consultas do dados.js do tópico 05, que a aula 07 vai buscar por fetch.
    private readonly List<Consulta> _agenda =
    [
        new() { Id = 1, Paciente = "Ana Souza", Especialidade = "Clínica geral",
                DataHora = new DateTime(2026, 10, 5, 9, 0, 0) },
        new() { Id = 2, Paciente = "Bruno Lima", Especialidade = "Cardiologia",
                DataHora = new DateTime(2026, 10, 5, 10, 30, 0) },
        new() { Id = 3, Paciente = "Carla Mendes", Especialidade = "Cardiologia",
                DataHora = new DateTime(2026, 10, 6, 14, 0, 0) },
    ];

    // O próximo id livre; ids removidos não são reaproveitados.
    private int _proximoId = 4;

    // Lock: duas requisições ao mesmo tempo não alteram a lista juntas.
    private readonly Lock _trava = new();

    public IReadOnlyList<Consulta> Listar()
    {
        // lock: só uma requisição por vez executa o bloco entre chaves.
        lock (_trava) return _agenda.ToList();
    }

    public Consulta? Buscar(int id)
    {
        lock (_trava) return _agenda.FirstOrDefault(c => c.Id == id);
    }

    public Consulta Adicionar(Consulta nova)
    {
        lock (_trava)
        {
            // O id enviado pelo cliente é ignorado: quem numera é o servidor.
            nova.Id = _proximoId++;
            _agenda.Add(nova);
            return nova;
        }
    }

    public bool Atualizar(int id, Consulta dados)
    {
        lock (_trava)
        {
            // FindIndex devolve a posição da consulta na lista, ou -1 se ela não existe.
            var posicao = _agenda.FindIndex(c => c.Id == id);
            if (posicao < 0) return false;
            // O id da rota vale sobre o id do corpo; a consulta inteira é substituída.
            dados.Id = id;
            _agenda[posicao] = dados;
            return true;
        }
    }

    public bool Remover(int id)
    {
        // RemoveAll devolve quantas consultas removeu: 1 quando o id existia, 0 quando não.
        lock (_trava) return _agenda.RemoveAll(c => c.Id == id) > 0;
    }
}
