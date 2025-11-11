# Fluxo de Caixa - Sistema de Controle Financeiro

Sistema de controle de fluxo de caixa desenvolvido com arquitetura de microserviços, seguindo os princípios de Clean Architecture, CQRS e Event-Driven Architecture.

---

## Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Arquitetura](#arquitetura)
- [Requisitos do Desafio](#requisitos-do-desafio)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Pré-requisitos](#pré-requisitos)
- [Como Executar](#como-executar)
- [Endpoints da API](#endpoints-da-api)
- [Autenticação](#autenticação)
- [Rate Limiting](#rate-limiting)
- [Testes](#testes)
- [Decisões Arquiteturais](#decisões-arquiteturais)

---

## Sobre o Projeto

Sistema de controle de lançamentos financeiros (débitos e créditos) com consolidação diária automática. O projeto foi desenvolvido seguindo boas práticas de desenvolvimento, padrões arquiteturais modernos e princípios SOLID.

### Características Principais

- Microserviços independentes (Lançamentos e Consolidado)
- Comunicação assíncrona via RabbitMQ
- Event-Driven Architecture com MassTransit
- CQRS com MediatR
- Clean Architecture em 4 camadas
- Domain-Driven Design (DDD)
- Autenticação JWT compartilhada
- Rate Limiting (50 req/s com 5% de perda)
- Validações com FluentValidation
- Resiliência com Retry Policies
- Containerização com Docker

---

## Arquitetura

### Visão Geral
###Fluxo Geral
<img width="2421" height="2459" alt="FluxoGeral" src="https://github.com/user-attachments/assets/a685fb1c-64f1-46ec-ac49-b75f19c76b42" />

### Clean Architecture (4 Camadas)

Cada microserviço segue a estrutura:

API (Controllers)              -> Camada de Apresentação
Application (CQRS)             -> Camada de Aplicação
  • Commands (Write)           -> Casos de Uso
  • Queries (Read)             -> DTOs
  • Handlers                   -> Validações
  • Validators (FluentValidation)

Domain (Entities)              -> Camada de Domínio
  • Entities                   -> Regras de Negócio
  • Value Objects              -> Lógica Pura
  • Domain Events              -> Sem Dependências
  • Repository Interfaces

Infrastructure (EF Core)       -> Camada de Infraestrutura
  • DbContext                  -> Acesso a Dados
  • Repositories               -> Integrações
  • Migrations                 -> Versionamento
  • Consumers (MassTransit)    -> Eventos


### Fluxo de Criação de Lançamento

1. Cliente faz `POST /api/lancamentos` (com JWT Token)
2. `LancamentosController` recebe a requisição
3. FluentValidation valida os dados
4. MediatR envia `CriarLancamentoCommand`
5. `CriarLancamentoCommandHandler` cria entidade de domínio, salva via repository e publica `LancamentoCriadoEvent` no RabbitMQ
6. `LancamentoCriadoConsumer` (API Consolidado) consome o evento, atualiza/insere o consolidado diário
7. Cliente consulta `GET /api/consolidado/diario/{data}` e obtém o consolidado atualizado

---

## Requisitos do Desafio

### Requisitos Funcionais Implementados

| # | Requisito | Status | Detalhes |
|---|-----------|--------|----------|
| 1 | Serviço de Lançamentos | Implementado | CRUD completo com débitos e créditos |
| 2 | Serviço de Consolidado | Implementado | Consultas por data e período |
| 3 | Consolidação Automática | Implementado | Via eventos assíncronos (RabbitMQ) |
| 4 | Cálculo de Saldo | Implementado | Total Créditos - Total Débitos |
| 5 | Relatórios | Implementado | Estatísticas do período implementadas |

### Requisitos Não-Funcionais Implementados

| # | Requisito | Status | Detalhes |
|---|-----------|--------|----------|
| 1 | Microserviços Independentes | Implementado | Lançamentos não depende de Consolidado |
| 2 | Comunicação Assíncrona | Implementado | RabbitMQ + MassTransit |
| 3 | Rate Limiting | Implementado | 50 req/s com 5% de perda (Consolidado) |
| 4 | Resiliência | Implementado | Retry Policy (3 tentativas com backoff) |
| 5 | Autenticação | Implementado | JWT com roles (Admin, Operador, Viewer) |
| 6 | Validações | Implementado | FluentValidation em todos Commands |
| 7 | Persistência | Implementado | SQL Server com EF Core 8 |
| 8 | Clean Architecture | Implementado | 4 camadas bem definidas |
| 9 | CQRS | Implementado | Separação de Commands e Queries |
| 10 | Documentação | Implementado | Swagger com autenticação JWT |

#### Requisito Específico: Rate Limiting

"Em dias de picos, o serviço de consolidado diário recebe 50 requisições por segundo, com no máximo 5% de perda de requisições."

**Implementação:**
- Limite: 50 requisições/segundo
- Fila: 2 requisições (5% de 50)
- Resposta: HTTP 429 (Too Many Requests) quando exceder

---

## Tecnologias Utilizadas

### Backend
- .NET 8.0
- C# 12.0
- ASP.NET Core Web API

### Arquitetura e Padrões
- Clean Architecture
- CQRS
- Event-Driven Architecture
- Domain-Driven Design (DDD)
- Repository Pattern
- MediatR

### Persistência
- Entity Framework Core 8
- SQL Server 2022
- EF Core Migrations

### Mensageria
- RabbitMQ 3.x
- MassTransit 8.x

### Validação e Segurança
- FluentValidation
- JWT (JSON Web Token)
- ASP.NET Core Identity
- Rate Limiting

### Infraestrutura
- Docker & Docker Compose
- Swagger/OpenAPI

### Bibliotecas Adicionais
- AutoMapper

## Pré-requisitos

Antes de executar o projeto, certifique-se de ter instalado:

- .NET 8 SDK (versão 8.0 ou superior)
- Docker Desktop (para Windows/Mac) ou Docker Engine (Linux)
- Git
- Editor de código: Visual Studio 2022 ou Visual Studio Code

### Verificar Instalações

```bash
# Verificar .NET SDK
dotnet --version
# Esperado: 8.0.x ou superior

# Verificar Docker
docker --version
docker-compose --version

# Verificar Git
git --version
```

---

## Como Executar

### Passo 1: Clonar o Repositório

```bash
git clone https://github.com/ejlunelli-ship-it/FluxoCaixa.git
cd FluxoCaixa
```

### Passo 2: Configurar Infraestrutura (Docker)

Subir SQL Server e RabbitMQ:

```bash
docker-compose up -d
```

Verificar se os containers estão rodando:

```bash
docker-compose ps
```

### Passo 3: Executar as APIs

Terminal 1 - API Lançamentos:
```bash
cd src/Services/Lancamentos/FluxoCaixa.Lancamentos.API
dotnet run
```
Terminal 2 - API Consolidado:
```bash
cd src/Services/Consolidado/FluxoCaixa.Consolidado.API
dotnet run
```

### Passo 4: Acessar as APIs

| Serviço | URL | Descrição |
|---------|-----|-----------|
| Lançamentos API | http://localhost:5172/swagger | Gestão de lançamentos |
| Consolidado API | http://localhost:5184/swagger | Consultas de consolidado |
| RabbitMQ Management | http://localhost:15672 | Monitoramento de filas (guest/guest) |
| SQL Server | localhost:1433 | Banco de dados (sa/FluxoCaixa@2025) |

### Passo 5: Configurar Autenticação no Swagger

1. Acesse: http://localhost:5172/swagger
2. Execute `POST /api/auth/login`:
```json
{
  "username": "admin",
  "password": "Admin@123"
}
```
3. Copie o token da resposta
4. Clique no botão "Authorize" no Swagger
5. Cole o token (apenas o valor, sem "Bearer")
6. Clique em "Authorize" e depois "Close"

---

## Endpoints da API

### API Lançamentos (Porta 5172)

#### Autenticação

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| POST   | /api/auth/login | Fazer login e obter JWT token | Pública |
| GET    | /api/auth/me    | Obter dados do usuário autenticado | Requerida |

#### Lançamentos

| Método | Endpoint | Descrição | Roles |
|--------|----------|-----------|-------|
| POST   | /api/lancamentos | Criar novo lançamento | Admin, Operador |
| GET    | /api/lancamentos/{id} | Obter lançamento por ID | Admin, Operador |
| GET    | /api/lancamentos | Listar lançamentos por período | Admin, Operador |
| PUT    | /api/lancamentos/{id} | Atualizar lançamento | Admin, Operador |
| DELETE | /api/lancamentos/{id} | Remover lançamento | Admin |

Exemplo de criação de lançamento:

```json
POST /api/lancamentos
{
  "dataLancamento": "2025-01-11T10:00:00",
  "tipo": 1,
  "valor": 250.50,
  "descricao": "Venda de produto",
  "observacao": "Pagamento à vista"
}
```

Tipos de lançamento:
- 1 = Crédito (entrada)
- 2 = Débito (saída)

### API Consolidado (Porta 5184)

| Método | Endpoint | Descrição | Roles | Rate Limit |
|--------|----------|-----------|-------|------------|
| GET | /api/consolidado/diario/{data} | Consolidado de uma data | Viewer | 50 req/s |
| GET | /api/consolidado/periodo | Consolidado de um período | Viewer | 50 req/s |
| GET | /api/consolidado/periodo/estatisticas | Estatísticas do período | Viewer | 50 req/s |
| GET | /api/consolidado/rate-limit-info | Info sobre Rate Limiting | Pública | - |
| GET | /health | Health check | Pública | - |

Exemplo de consulta de consolidado:

```bash
GET /api/consolidado/diario/2025-01-11
```

Resposta de exemplo:

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "data": "2025-01-11",
  "totalCreditos": 1500.00,
  "totalDebitos": 750.00,
  "saldoFinal": 750.00,
  "quantidadeLancamentos": 25
}
```

Exemplo de estatísticas:

```bash
GET /api/consolidado/periodo/estatisticas?dataInicio=2025-01-01&dataFim=2025-01-31
```

Resposta de exemplo:

```json
{
  "periodo": {
    "dataInicio": "2025-01-01",
    "dataFim": "2025-01-31"
  },
  "totalDias": 31,
  "totalCreditos": 45000.00,
  "totalDebitos": 32000.00,
  "saldoFinalPeriodo": 13000.00,
  "totalLancamentos": 750,
  "mediaSaldoDiario": 419.35,
  "maiorSaldo": 2500.00,
  "menorSaldo": -500.00,
  "diasPositivos": 28,
  "diasNegativos": 2,
  "diasZerados": 1
}
```

---

## Autenticação

### Usuários de Teste

| Username | Password | Roles | Permissões |
|----------|----------|-------|------------|
| admin    | Admin@123 | Admin, Operador | Acesso total (criar, editar, deletar) |
| operador | Oper@123  | Operador        | Criar e editar lançamentos |
| viewer   | View@123  | Viewer          | Apenas consultar |

### Fluxo de Autenticação

1. Login na API Lançamentos
2. Receber token JWT válido por 1 hora
3. Usar o mesmo token em ambas as APIs
4. Token expira em 1 hora — fazer login novamente

### Configuração JWT (exemplo)

```json
{
  "Jwt": {
    "Key": "ChaveSecreta_FluxoCaixa2025_DeveSerMaiorQue32Caracteres!@#",
    "Issuer": "FluxoCaixa.Lancamentos.API",
    "Audience": "FluxoCaixa.Clients",
    "ExpiresInMinutes": 60
  }
}
```
---

## Rate Limiting

### Configuração

A API de Consolidado possui Rate Limiting configurado:

- Limite: 50 requisições por segundo
- Janela: 1 segundo (Fixed Window)
- Fila: 2 requisições (5% de perda)
- Resposta: HTTP 429 (Too Many Requests)

### Como Funciona

```
Requisição 1-50  -> Processadas imediatamente (HTTP 200)
Requisição 51-52 -> Enfileiradas e processadas quando possível
Requisição 53+   -> Rejeitadas com HTTP 429
```

### Resposta de Rate Limit Excedido

```json
HTTP/1.1 429 Too Many Requests
{
  "error": "Rate limit exceeded",
  "message": "Limite de 50 requisições por segundo atingido. Por favor, aguarde antes de tentar novamente.",
  "retryAfter": 1
}
```

## Testes

TODO: Implementar testes automatizados

---

## Decisões Arquiteturais

### Por que Microserviços?

- Independência: Lançamentos e Consolidado podem evoluir separadamente  
- Escalabilidade: Escalar apenas o serviço sob carga  
- Resiliência: Falha em um não afeta o outro  
- Tecnologia: Liberdade para escolher diferentes stacks por serviço  

### Por que Event-Driven Architecture?

- Desacoplamento: APIs não se chamam diretamente  
- Assíncrono: Lançamentos não espera consolidação  
- Escalável: RabbitMQ gerencia múltiplos consumidores  
- Resiliente: Retry automático em caso de falha  

### Por que CQRS?

- Separação: Operações de escrita vs leitura  
- Performance: Otimizações específicas para cada operação  
- Escalabilidade: Escalar leitura e escrita independentemente  
- Manutenibilidade: Código mais organizado e testável  

### Por que Clean Architecture?

- Testabilidade: Domínio testável sem infraestrutura  
- Independência: Regras de negócio isoladas  
- Manutenibilidade: Mudanças localizadas por camada  
- Flexibilidade: Trocar tecnologias facilmente  

### Por que SQL Server?

- ACID: Transações confiáveis para dados financeiros  
- Maturidade: Banco robusto e amplamente usado  
- EF Core: Excelente integração  
- Performance: Índices e otimizações para queries  

### Por que RabbitMQ?

- Confiabilidade: Garantia de entrega de mensagens  
- Maturidade: Battle-tested em produção  
- MassTransit: Abstração poderosa  
- Dead Letter Queue: Tratamento de mensagens com falha  

---

## Considerações Arquiteturais e Trade-offs

### Escalabilidade

#### Implementado

- Dimensionamento Horizontal: Microservicos independentes podem ser replicados
- Mensageria Assincrona: RabbitMQ permite adicionar consumidores conforme demanda
- Banco de Dados Separados: Cada servico tem seu proprio banco (database per service)
- Stateless APIs: Sem sessao no servidor, facilitando load balancing

#### Trade-offs

**Vantagens:**
- Facil escalar servicos individualmente baseado em metricas especificas
- RabbitMQ atua como buffer em picos de carga
- Adicionar instancias nao requer mudancas de codigo

**Desvantagens:**
- Complexidade operacional aumentada (gerenciar multiplas instancias)
- Necessidade de load balancer em producao
- Monitoramento distribuido mais complexo
- Custos de infraestrutura maiores

### Resiliencia

#### Implementado

- Servicos Independentes: Falha em Consolidado nao impede criacao de lancamentos
- Retry Policy: 3 tentativas com backoff exponencial no RabbitMQ
- Circuit Breaker: Implicitamente via MassTransit retry policy
- Health Checks: Endpoints /health em ambas APIs
- Dead Letter Queue: Mensagens com falha vao para DLQ automaticamente
- Connection Retry: EF Core tenta reconectar ao banco (3 tentativas)

#### Trade-offs

**Vantagens:**
- Sistema continua funcionando parcialmente mesmo com falhas
- Mensagens nao sao perdidas em caso de falha temporaria
- Recuperacao automatica sem intervencao manual

**Desvantagens:**
- Eventual Consistency: Consolidado pode estar levemente desatualizado
- Complexidade de debugging (traces distribuidos)
- Necessidade de compensacao em caso de falhas persistentes
- Duplicacao eventual se retry tiver sucesso apos timeout

### Seguranca

#### Implementado

- Autenticacao JWT: Token-based authentication
- Autorizacao por Roles: Admin, Operador, Viewer
- HTTPS: Redirecionamento forcado em producao
- SQL Injection Protection: EF Core com parametrizacao automatica
- Validacao de Entrada: FluentValidation em todos Commands
- Rate Limiting: Protecao contra DDoS (50 req/s)


### Padroes Arquiteturais

#### Escolhas Realizadas

**Microservicos vs Monolito**

Escolhido: Microservicos

**Justificativa:**
- Escalabilidade independente (Consolidado recebe mais leituras)
- Evolucao separada (times diferentes podem trabalhar em paralelo)
- Tecnologia flexivel (poderia usar linguagem diferente por servico)

**Trade-offs:**
- PRO: Escalabilidade, resiliencia, independencia
- CONTRA: Complexidade operacional, latencia de rede, debugging complexo

**CQRS vs CRUD Tradicional**

Escolhido: CQRS

**Justificativa:**
- Separacao clara entre escrita (Lancamentos) e leitura (Consolidado)
- Otimizacoes especificas (indices diferentes para read/write)
- Escalabilidade (mais replicas de leitura que escrita)

**Trade-offs:**
- PRO: Performance, escalabilidade, otimizacao especifica
- CONTRA: Eventual consistency, mais codigo, curva de aprendizado

**Event-Driven vs REST Sincrono**

Escolhido: Event-Driven

**Justificativa:**
- Desacoplamento total entre servicos
- Resiliencia (mensagens nao se perdem)
- Performance (nao bloqueia o usuario esperando consolidacao)

**Trade-offs:**
- PRO: Desacoplamento, resiliencia, performance
- CONTRA: Eventual consistency, complexidade, debugging dificil

**Clean Architecture vs Arquitetura em Camadas Tradicional**

Escolhido: Clean Architecture (4 camadas)

**Justificativa:**
- Testabilidade (dominio puro, sem dependencias)
- Flexibilidade (trocar ORM ou banco sem afetar dominio)
- Manutenibilidade (mudancas localizadas)

**Trade-offs:**
- PRO: Testabilidade, manutenibilidade, flexibilidade
- CONTRA: Mais codigo, mais arquivos, curva de aprendizado

### Integracao

#### Implementado

**Comunicacao Assincrona:**
- RabbitMQ como Message Broker
- MassTransit como abstracao
- Eventos de integracao (LancamentoCriadoEvent)
- JSON como formato de mensagem

**Comunicacao Sincrona:**
- REST APIs com JSON
- Swagger/OpenAPI para documentacao
- HTTP/HTTPS como protocolo

#### Trade-offs

**Assincrona (RabbitMQ):**
- PRO: Desacoplamento, buffer em picos, resiliencia
- CONTRA: Eventual consistency, complexidade, latencia adicional

**Sincrona (REST):**
- PRO: Simples, resposta imediata, facil debug
- CONTRA: Acoplamento,  falhas cascatas

### Requisitos Nao-Funcionais

#### Performance

**Implementado:**
- Queries otimizadas com EF Core
- Indices no banco de dados
- Async/Await em toda stack

**Trade-offs:**
- Eventual consistency em troca de performance
- Mais infraestrutura em troca de menor latencia

#### Observabilidade

**Implementado:**
- Logs estruturados (ILogger)
- Health checks basicos
- Swagger para documentacao

#### Disponibilidade

**Implementado:**
- Retry policies
- Health checks
- Docker para deploy consistente

#### Manutenibilidade

**Implementado:**
- Clean Architecture (separacao de responsabilidades)
- SOLID principles
- DDD (linguagem ubiqua)
- Documentacao (README, Swagger, comentarios)
---

## Comparacao de Alternativas

### Alternativa 1: Monolito

**Seria adequado se:**
- Sistema pequeno com poucos usuarios
- Time pequeno (1-3 devs)
- Sem necessidade de escalar partes independentemente

**Trade-offs:**
- PRO: Mais simples, menos infraestrutura, debugging facil
- CONTRA: Nao escala bem, acoplamento, deploy all-or-nothing

### Alternativa 2: Serverless (Azure Functions)

**Seria adequado se:**
- Carga extremamente variavel (de 0 a 1000 req/s)
- Nao quer gerenciar infraestrutura
- Custo otimizado por execucao

**Trade-offs:**
- PRO: Escala automaticamente, paga por uso, zero ops
- CONTRA: Cold start, limitacoes de runtime, vendor lock-in
---

