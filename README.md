#  ApiBlingIntegration

API .NET 10 para integração com o Bling ERP, com extração e sincronização automatizada de pedidos de venda, notas fiscais (NF-e e NFS-e), produtos, categorias e naturezas de operação.

---

##  Sobre o Projeto

Este projeto foi desenvolvido para integrar com a **API v3 do Bling ERP**, extraindo dados comerciais e fiscais e armazenando-os em um banco de dados SQL Server local. A API gerencia autenticação OAuth2 automaticamente (com refresh de tokens), trata rate limiting com retry exponencial e sincroniza os dados de forma estruturada e confiável.

### Funcionalidades

- ✅ Sincronização de **produtos** com categorias
- ✅ Sincronização de **pedidos de venda** e itens
- ✅ Sincronização de **notas fiscais (NF-e)** e itens
- ✅ Sincronização de **notas fiscais de serviço (NFS-e)**
- ✅ Sincronização de **categorias** e **naturezas de operação**
- ✅ Sincronização de **situações** de pedidos e notas
- ✅ Gestão automática de tokens OAuth2 (access + refresh)
- ✅ Tratamento de rate limiting (HTTP 429) com Polly

---

##  Arquitetura

O projeto segue o padrão **Clean Architecture** em 4 camadas, separando responsabilidades e facilitando manutenção e testes:

| Camada | Projeto | Responsabilidade |
|--------|---------|-------------------|
| **API** | `ApiBling.Api` | Controllers, configuração da aplicação, Swagger |
| **Application** | `ApiBling.Application` | Serviços de negócio, DTOs, interfaces, integração com a API do Bling |
| **Domain** | `ApiBling.Domain` | Entidades de domínio (modelagem dos dados) |
| **Infrastructure** | `ApiBling.Infrastructure` | Entity Framework Core, migrations, acesso a dados |

```
ApiBlingIntegration/
├── ApiBling.Api/                  # Camada de apresentação
│   ├── Controllers/               # Endpoints REST
│   ├── Properties/                # Configurações de launch
│   ├── Program.cs                 # Entry point e configuração da aplicação
│   └── appsettings.json          # Configurações (sem credenciais)
│
├── ApiBling.Application/          # Camada de aplicação
│   ├── DTOs/                      # Objetos de transferência de dados
│   ├── Interfaces/                # Contratos de serviços (IBlingApiService, ISincronizacaoService)
│   ├── Services/                  # BlingApiService (HTTP) e SincronizacaoService (orquestração)
│   └── Converters/               # Conversores JSON personalizados
│
├── ApiBling.Domain/               # Camada de domínio
│   └── Entities/                  # 14 entidades: Produto, Pedido, NotaFiscal, etc.
│
├── ApiBling.Infrastructure/       # Camada de infraestrutura
│   ├── Data/                     # AppDbContext (EF Core)
│   └── Migrations/               # 20+ migrations do Entity Framework
│
└── ApiBlingIntegration.slnx      # Solution file
```

---

##  Tecnologias

### .NET 10

O **.NET 10** é a plataforma de desenvolvimento da Microsoft, sucessora do .NET 8. É um framework open-source, cross-platform e de alta performance. Este projeto usa .NET 10 por ser a versão mais recente, com melhorias de performance no JIT compiler, melhor suporte a generics e APIs mais enxutas.

- **Por que usar:** Performance nativa, compilação AOT, ecossistema maduro
- **Como é usado:** Toda a aplicação roda sobre o runtime do .NET 10

### ASP.NET Core

O **ASP.NET Core** é o framework web do .NET para construção de APIs REST. Ele fornece o pipeline de middleware, sistema de roteamento, injeção de dependência nativa e configuração por ambiente (Development, Production).

- **Por que usar:** Framework maduro, performático, com documentação extensa
- **Como é usado:** Os controllers em `ApiBling.Api/Controllers/` expõem endpoints REST que disparam as sincronizações com o Bling. O `Program.cs` configura o pipeline: Swagger, HTTPS, autorização e mapeamento de controllers

### Entity Framework Core

O **EF Core** é o ORM (Object-Relational Mapper) da Microsoft para .NET. Ele mapeia classes C# para tabelas no banco de dados, permitindo trabalhar com dados como objetos sem escrever SQL manualmente.

- **Por que usar:** Elimina SQL boilerplate, suporta migrations (versionamento do schema), LINQ para queries type-safe
- **Como é usado:**
  - O `AppDbContext` define 14 `DbSet<T>` mapeando as entidades do domínio
  - Configurações de precisão decimal (`HasPrecision(18, 4)`), índices únicos (`HasIndex`), relacionamentos (`HasOne/WithMany`) e dados de seed (`HasData`)
  - 20+ migrations versionam toda a evolução do schema do banco

### SQL Server

O **Microsoft SQL Server** é o sistema de gerenciamento de banco de dados relacional (SGBD) utilizado para armazenar os dados extraídos do Bling. Neste projeto, é usado em instância local com autenticação Windows (`Trusted_Connection=True`).

- **Por que usar:** Robustez, integração nativa com EF Core, suporte a transações ACID
- **Como é usado:** A string de conexão aponta para o banco `InformacoesBlingMaju`, configurada via User Secrets (não fica no repositório)

### Polly

O **Polly** é uma biblioteca de resiliência para .NET que implementa padrões como retry, circuit breaker, timeout e fallback. Neste projeto, é usado para tratar o **HTTP 429 (Too Many Requests)** que o Bling retorna quando o limite de requisições é excedido.

- **Por que usar:** O Bling ERP impõe rate limiting agressivo. Sem retry, a sincronização falha frequentemente
- **Como é usado:** Configurado no `Program.cs` via `AddPolicyHandler`:
  - Intercepta erros HTTP transientes **e** HTTP 429
  - Retry com backoff exponencial: 2s → 4s → 6s (3 tentativas)
  - Implementação: `WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2 * retryAttempt))`

### Bling ERP API v3

O **Bling** é um sistema de gestão empresarial (ERP) brasileiro. A **API v3** fornece endpoints REST para acessar pedidos, notas fiscais, produtos, categorias, etc. A autenticação é via **OAuth2** com `client_credentials` flow.

- **Como é usado:**
  - O `BlingApiService` implementa a integração: geração de token, refresh de token e chamadas aos endpoints
  - Autenticação OAuth2: `ClientId` e `ClientSecret` em Base64 no header `Authorization: Basic`
  - Token Bearer nas chamadas de dados: `Authorization: Bearer {accessToken}`
  - URL base: `https://www.bling.com.br/Api/v3`

### Swagger / OpenAPI

O **Swagger** (OpenAPI) é o padrão de documentação de APIs REST. O ASP.NET Core gera a documentação automaticamente a partir dos controllers e expõe uma interface interativa (Swagger UI) para testar os endpoints.

- **Como é usado:** Configurado em `Program.cs` com `AddSwaggerGen()` e `UseSwaggerUI()`, ativo apenas em ambiente de desenvolvimento

### .NET User Secrets

O **User Secrets** é um mecanismo do .NET para armazenar credenciais sensíveis fora do repositório. Os segredos ficam em um arquivo JSON na pasta do perfil do usuário (`%APPDATA%\Microsoft\UserSecrets\`), nunca no projeto.

- **Por que usar:** Evita que ClientId, ClientSecret e string de conexão sejam commitados no Git
- **Como é usado:** O `WebApplication.CreateBuilder(args)` carrega os segredos automaticamente em desenvolvimento. As credenciais são lidas via `IConfiguration["Bling:ClientId"]` e `IConfiguration["Bling:ClientSecret"]`

---

##  Configuração

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (Express ou superior)
- Conta no [Bling ERP](https://www.bling.com.br/) com acesso à API
- Credenciais OAuth2 do Bling (ClientId e ClientSecret)

### Passo a passo

1. **Clonar o repositório**

```bash
git clone https://github.com/MuriloRissoDEV/ApiBlingIntegration.git
cd ApiBlingIntegration
```

2. **Restaurar pacotes NuGet**

```bash
dotnet restore
```

3. **Configurar credenciais com User Secrets**

As credenciais do Bling e a string de conexão são armazenadas via .NET User Secrets e **não** ficam no `appsettings.json`.

```bash
cd ApiBling.Api

dotnet user-secrets init

dotnet user-secrets set "Bling:ClientId" "seu_client_id"
dotnet user-secrets set "Bling:ClientSecret" "seu_client_secret"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\SQLEXPRESS;Database=InformacoesBlingMaju;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

4. **Aplicar migrations no banco de dados**

```bash
dotnet ef database update --project ApiBling.Infrastructure --startup-project ApiBling.Api
```

5. **Executar a API**

```bash
cd ApiBling.Api
dotnet run
```

A API ficará disponível em `https://localhost:7xxx` e o Swagger em `/swagger`.

---

## 🔐 Segurança

- As credenciais do Bling (ClientId e ClientSecret) **não estão no repositório**
- A string de conexão com o banco de dados **não está no repositório**
- Todas as credenciais são gerenciadas via .NET User Secrets
- O arquivo `appsettings.json` do repositório contém apenas campos vazios
- O `.gitignore` bloqueia `appsettings.Development.json`, `bin/`, `obj/`, `.vs/` e `secrets.json`

---

## 📌 Endpoints

A API expõe endpoints REST para sincronização e consulta dos dados:

| Controller | Recurso |
|------------|---------|
| `ProdutosController` | Produtos |
| `PedidosController` | Pedidos de venda |
| `NotasFiscaisController` | Notas fiscais (NF-e) |
| `NotasFiscaisGeraisController` | Notas fiscais gerais |
| `NotasFiscaisServicoController` | Notas fiscais de serviço (NFS-e) |
| `CategoriasController` | Categorias |
| `NaturezasOperacoesController` | Naturezas de operação |
| `SituacoesPedidoVendaController` | Situações de pedido de venda |

> A documentação completa dos endpoints está disponível via Swagger UI ao executar o projeto em ambiente de desenvolvimento.

---

## 🤝 Tratamento de Rate Limiting

O Bling ERP impõe limites de requisições (HTTP 429 - Too Many Requests). Este projeto utiliza a biblioteca **Polly** para interceptar esse erro e aplicar retry com backoff exponencial:

| Tentativa | Espera |
|-----------|--------|
| 1ª | 2 segundos |
| 2ª | 4 segundos |
| 3ª | 6 segundos |

Após 3 tentativas, a requisição falha e é registrada para análise.

---

## 📄 Licença

Este projeto é privado e não possui licença de uso aberta.

---
