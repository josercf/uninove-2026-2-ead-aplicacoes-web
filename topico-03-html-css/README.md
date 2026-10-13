# Tópico 03: HTML semântico e CSS responsivo

Este exemplo é uma página estática da Clínica Vida+, escrita só com HTML e CSS. A página marca cada região com a tag que descreve o seu papel (`header`, `nav`, `main`, `section`, `footer`) e traz a agenda do dia numa tabela de dados. O CSS parte da tela do celular e muda o layout a partir de 768 pixels de largura.

Nenhum servidor é necessário: o navegador abre o arquivo direto do disco.

## Arquivos

- `index.html`: a página, com um comentário em português em cada linha relevante.
- `estilos.css`: a folha de estilo, primeiro para o celular e depois para telas de 768 pixels ou mais.
- `logo.svg`: o logotipo da clínica, um círculo verde com uma cruz branca.
- `testes/estrutura.test.js`: sete testes que conferem a estrutura da página e a regra de largura do CSS.
- `package.json`: declara o comando `npm test`. O tópico não instala nenhuma dependência.

## Pré-requisitos

1. Um navegador atualizado (Chrome, Edge ou Firefox).

2. Node.js 18 ou superior, só para rodar os testes.

   ```bash
   node --version
   ```

   Resultado esperado: um número igual ou maior que `v18.0.0`.

## Como executar

### Passo 1: Abrir a página

Na pasta `topico-03-html-css`, dê um duplo clique no arquivo `index.html`.

Resultado esperado: o navegador abre um endereço que começa por `file://` e mostra a faixa verde com o logotipo, o título "Clínica Vida+" e os links Especialidades, Agenda do dia e Contato.

### Passo 2: Ver o layout abaixo de 768 pixels

Estreite a janela do navegador até ela ficar com a largura de um celular. Outra forma é abrir o DevTools (`F12`, no macOS `Cmd+Option+I`) e ligar o modo dispositivo (`Ctrl+Shift+M`, no macOS `Cmd+Shift+M`).

Resultado esperado: os links do menu descem para uma linha abaixo do título, e os três cartões de especialidade ficam um embaixo do outro.

### Passo 3: Ver o layout a partir de 768 pixels

Alargue a janela até passar de 768 pixels de largura.

Resultado esperado: o menu sobe para a mesma linha do título e encosta na direita, os três cartões ficam lado a lado, e o conteúdo para de crescer em 960 pixels, centralizado na janela.

### Passo 4: Seguir os links do menu

Clique em "Agenda do dia" e depois em "Contato".

Resultado esperado: a página rola até a tabela da agenda e depois até o rodapé com o endereço. O endereço do navegador ganha `#agenda-do-dia` e depois `#contato` no fim.

### Passo 5: Rodar os testes

No terminal, a partir da pasta raiz do repositório:

```bash
cd topico-03-html-css
npm test
```

Resultado esperado: sete linhas com `✔`, seguidas de `ℹ pass 7` e `ℹ fail 0`.

## Se der errado

### Erro 1: a página abre sem cor e com a lista de links em coluna

Isso significa que o navegador não encontrou o `estilos.css`. A causa mais comum é o arquivo ter sido movido ou renomeado: o `index.html` procura `estilos.css` na mesma pasta em que ele está.

Correção: devolva o `estilos.css` para a pasta `topico-03-html-css`, ao lado do `index.html`, com esse nome exato, e recarregue a página.

### Erro 2: o logotipo não aparece e, no lugar dele, surge o texto "Logotipo da Clínica Vida+"

Isso significa que o navegador não encontrou o `logo.svg`, e mostrou o texto alternativo do atributo `alt` no lugar da imagem.

Correção: confira se o `logo.svg` está na mesma pasta do `index.html` e recarregue a página.

### Erro 3: `npm test` responde "ENOENT: no such file or directory"

Isso significa que o comando rodou fora da pasta do tópico, ou que o `index.html` ou o `estilos.css` foi apagado ou renomeado.

Correção: entre na pasta com `cd topico-03-html-css`, confira com `ls` (ou `dir` no Windows) que os dois arquivos estão lá e rode `npm test` de novo.

### Erro 4: um teste falha com a mensagem "deveria aparecer uma vez"

Isso significa que uma das regiões `header`, `nav`, `main` ou `footer` foi apagada ou repetida no `index.html`. A mensagem diz qual delas.

Correção: deixe uma única ocorrência de cada região e rode `npm test` de novo.

## Experimente mudar

Faça cada mudança separadamente e desfaça antes da próxima.

### Mudança 1: trocar o ponto de quebra

No `estilos.css`, troque `768px` por `1024px` na linha do `@media`. Recarregue a página e alargue a janela devagar.

Pergunta: em que largura os cartões passam a ficar lado a lado? O que o teste `o CSS parte do celular e muda o layout a partir de 768px` responde depois da troca?

### Mudança 2: apagar a linha da viewport

Apague a linha `<meta name="viewport" ...>` do `index.html` e abra a página no modo dispositivo do DevTools, com a largura de um celular.

Pergunta: o texto fica maior, menor ou igual? Os cartões ficam lado a lado ou um embaixo do outro?

### Mudança 3: tirar o texto alternativo

Apague o atributo `alt` da imagem do logotipo e rode `npm test`.

Pergunta: qual teste falha, e qual mensagem ele mostra?
