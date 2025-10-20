# Sistema de Faturamento com Arquitetura de Microserviços

Projeto desenvolvido como parte de um processo seletivo, com o objetivo de criar um sistema para emissão de notas fiscais utilizando uma arquitetura distribuída e resiliente.

## 📝 Sobre o Projeto

A aplicação simula um ambiente de faturamento onde é possível gerenciar produtos e seus respectivos estoques, e emitir notas fiscais. O principal desafio do projeto foi construir uma solução robusta que seguisse os princípios da arquitetura de microserviços, garantindo a consistência dos dados mesmo em cenários de falha.

## ✨ Funcionalidades Principais

* **📦 Gestão de Produtos:**
    * Cadastro, leitura, atualização e exclusão (CRUD) de produtos.
    * Controle de saldo de estoque individual para cada produto.
* **📄 Gestão de Notas Fiscais:**
    * Cadastro de notas fiscais com numeração e status (`Aberta`, `Processando`, `Fechada`, `Cancelada`).
    * Inclusão de múltiplos produtos em uma única nota fiscal.
* **🖨️ Impressão e Faturamento:**
    * Endpoint para "imprimir" (processar) uma nota fiscal.
    * Validação de saldo em estoque de todos os itens antes de confirmar a operação.
    * Baixa automática no estoque e alteração do status da nota para `FECHADA` após o sucesso da operação.
    * Feedback claro ao usuário sobre o sucesso ou a falha do processo.

## 🏛️ Arquitetura e Conceitos Aplicados

Para atender aos desafios propostos, a aplicação foi estruturada sobre os seguintes pilares:

#### 1. Arquitetura de Microserviços
A solução é dividida em dois serviços independentes e autônomos, cada um com sua própria responsabilidade e banco de dados:
* **Serviço de Estoque:** Responsável por gerenciar o CRUD de produtos e controlar o saldo em estoque.
* **Serviço de Faturamento:** Responsável por orquestrar a criação, gerenciamento e processamento das notas fiscais.

Essa abordagem garante baixo acoplamento, escalabilidade e manutenibilidade independentes para cada domínio de negócio.

#### 2. Consistência de Dados e ACID (Padrão Saga)
Garantir a atomicidade em um ambiente distribuído é um desafio. A operação de "imprimir uma nota" precisa criar a nota E dar baixa no estoque. Para resolver isso, foi implementado o **Padrão Saga (Coreografado)**:

1.  O **Serviço de Faturamento** recebe a requisição e cria a nota com status `PROCESSANDO`.
2.  Ele emite um evento/chamada para o **Serviço de Estoque** solicitando a baixa dos itens.
3.  O **Serviço de Estoque** tenta realizar a baixa:
    * **Sucesso:** Confirma a transação e notifica o Faturamento, que altera o status da nota para `FECHADA`.
    * **Falha:** Rejeita a operação e notifica o Faturamento.
4.  Em caso de falha, o **Serviço de Faturamento** executa uma **transação de compensação**, alterando o status da nota para `CANCELADA`, garantindo que o sistema retorne a um estado consistente.

#### 3. Resiliência e Tratamento de Falhas
O sistema foi projetado para ser resiliente. O cenário de falha obrigatório foi implementado da seguinte forma: se o **Serviço de Estoque** estiver indisponível ou retornar um erro durante o processamento da nota, o **Serviço de Faturamento** identifica a falha, executa a transação de compensação (cancela a nota) e retorna um feedback claro ao usuário final informando sobre o problema.

#### 4. Tratamento de Concorrência (Desafio Extra)
Para evitar condições de corrida (race conditions), como duas requisições tentando comprar o último item de um produto ao mesmo tempo, foi implementado um mecanismo de **travamento otimista (optimistic locking)** no serviço de estoque. Isso garante que as atualizações de saldo sejam atômicas e seguras em um ambiente de alta concorrência.

## 💻 Tecnologias Utilizadas

* **Backend (Serviço de Estoque):** C# com .NET 8
* **Backend (Serviço de Faturamento):** C# com .NET 8
* **Frontend:** Angular 20
* **Bancos de Dados:** PostgreSQL
* **Infraestrutura e Orquestração:** Docker e Docker Compose

## 🚀 Como Executar o Projeto

**Pré-requisitos:**
* [Git](https://git-scm.com/)
* [Docker](https://www.docker.com/products/docker-desktop/)
* [Docker Compose](https://docs.docker.com/compose/)

Siga os passos abaixo para executar a aplicação em seu ambiente local:

```bash
# 1. Clone o repositório
git clone [https://github.com/gabrielmd3/processo-seletivo-korp.git](https://github.com/gabrielmd3/processo-seletivo-korp.git)

# 2. Navegue até a raiz do projeto
cd processo-seletivo-korp

# 3. Suba os containers com Docker Compose
# O comando irá construir as imagens e iniciar todos os serviços e bancos de dados.
docker-compose up --build -d
```
Após a execução, as APIs estarão disponíveis nos respectivos endereços:
* **Serviço de Estoque:** `http://localhost:5001`
* **Serviço de Faturamento:** `http://localhost:5002`
* **Frontend (Angular):** `http://localhost:8080`

## ⚙️ Endpoints da API

### Serviço de Estoque
* `GET /api/produtos` - Lista todos os produtos.
* `POST /api/produtos` - Cadastra um novo produto.
* `PUT /api/produtos/{id}/dar-baixa` - Decrementa o saldo do produto.

### Serviço de Faturamento
* `GET /api/notasfiscais` - Lista todas as notas fiscais.
* `POST /api/notasfiscais` - Cria uma nova nota fiscal.
* `POST /api/notasfiscais/{id}/imprimir` - Processa (imprime) uma nota fiscal, disparando o fluxo da Saga.
