// Testes de src/api.js e src/carregar.js com node --test e um servidor HTTP falso, sem rede real.
import assert from 'node:assert/strict';
import { createServer } from 'node:http';
import { after, before, describe, test } from 'node:test';
import { buscarConsultas } from '../src/api.js';
import { carregarConsultas } from '../src/carregar.js';

// Três consultas no formato JSON que a API do tópico 06 devolve.
const tres = [
  { id: 1, paciente: 'Ana Souza', especialidade: 'Clínica geral', dataHora: '2026-10-05T09:00:00' },
  { id: 2, paciente: 'Bruno Lima', especialidade: 'Cardiologia', dataHora: '2026-10-05T10:30:00' },
  { id: 3, paciente: 'Carla Mendes', especialidade: 'Cardiologia', dataHora: '2026-10-06T14:00:00' },
];

// O servidor falso escolhe a resposta pelo caminho; atrasa a rota /lenta em 150 ms.
const pedidos = [];
const servidor = createServer((req, res) => {
  pedidos.push(`${req.method} ${req.url}`);
  if (req.url === '/api/consultas') {
    res.writeHead(200, { 'Content-Type': 'application/json' });
    res.end(JSON.stringify(tres));
  } else if (req.url === '/lenta/api/consultas') {
    setTimeout(() => {
      res.writeHead(200, { 'Content-Type': 'application/json' });
      res.end(JSON.stringify(tres));
    }, 150);
  } else if (req.url === '/quebrada/api/consultas') {
    res.writeHead(500);
    res.end('erro interno');
  } else {
    res.writeHead(200, { 'Content-Type': 'application/json' });
    res.end('isto não é JSON');
  }
});
let base;
before(() => new Promise((ok) => servidor.listen(0, '127.0.0.1', () => {
  base = `http://127.0.0.1:${servidor.address().port}`;
  ok();
})));
after(() => servidor.close());

describe('buscarConsultas', () => {
  test('sucesso: devolve as 3 consultas na ordem da API e pede GET /api/consultas', async () => {
    const consultas = await buscarConsultas(base);
    assert.deepEqual(consultas.map((c) => c.paciente), ['Ana Souza', 'Bruno Lima', 'Carla Mendes']);
    assert.ok(pedidos.includes('GET /api/consultas'));
  });

  test('a promessa só se resolve depois da resposta atrasada (tempo medido)', async () => {
    const inicio = performance.now();
    const consultas = await buscarConsultas(`${base}/lenta`);
    const ms = performance.now() - inicio;
    assert.equal(consultas.length, 3);
    assert.ok(ms >= 140, `esperava pelo menos 140 ms, medido ${ms.toFixed(0)} ms`);
  });

  test('erro: status 500 rejeita com o código na mensagem', async () => {
    await assert.rejects(buscarConsultas(`${base}/quebrada`), /HTTP 500/);
  });

  test('erro: servidor fora do ar rejeita', async () => {
    const fora = createServer();
    await new Promise((ok) => fora.listen(0, '127.0.0.1', ok));
    const porta = fora.address().port;
    await new Promise((ok) => fora.close(ok));
    await assert.rejects(buscarConsultas(`http://127.0.0.1:${porta}`));
  });

  test('erro: corpo que não é JSON rejeita', async () => {
    await assert.rejects(buscarConsultas(`${base}/outra`));
  });
});

describe('carregarConsultas', () => {
  test('carregamento: informa "carregando" antes de "pronto" e entrega as consultas', async () => {
    const estados = [];
    const espera = carregarConsultas(() => buscarConsultas(`${base}/lenta`), (e) => estados.push(e));
    // Logo após a chamada, a resposta atrasada ainda não chegou.
    assert.deepEqual(estados, [{ estado: 'carregando' }]);
    await espera;
    assert.equal(estados.length, 2);
    assert.equal(estados[1].estado, 'pronto');
    assert.equal(estados[1].consultas.length, 3);
  });

  test('erro: informa "carregando" e depois "erro" com a mensagem', async () => {
    const estados = [];
    await carregarConsultas(() => buscarConsultas(`${base}/quebrada`), (e) => estados.push(e));
    assert.deepEqual(estados.map((e) => e.estado), ['carregando', 'erro']);
    assert.match(estados[1].mensagem, /HTTP 500/);
  });
});
