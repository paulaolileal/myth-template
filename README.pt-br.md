<img  style="float: right;" src="logo.png" alt="drawing" width="250"/>

# Myth Template API

[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg?style=for-the-badge)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![Myth Framework](https://img.shields.io/badge/Myth-blue.svg?style=for-the-badge)](https://github.com/paulaolileal/myth)
[![License](https://img.shields.io/badge/license-MIT-green.svg?style=for-the-badge)](LICENSE)

> 🚀 **Template de API ASP.NET Core de nível empresarial demonstrando o poder do ecossistema Myth**

Um projeto template pronto para produção que demonstra padrões arquiteturais avançados, princípios de código limpo e as capacidades do **Myth Framework**. Este template serve como um blueprint para construir APIs REST escaláveis e mantíveis com qualidade e boas práticas de nível empresarial.

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Arquitetura](#-arquitetura)
- [Funcionalidades Principais](#-funcionalidades-principais)
- [Stack Tecnológico](#-stack-tecnológico)
- [Primeiros Passos](#-primeiros-passos)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Padrões de Design](#-padrões-de-design)
- [Benefícios do Myth Framework](#-benefícios-do-myth-framework)
- [Documentação da API](#-documentação-da-api)
- [Exemplos](#-exemplos)
- [Melhores Práticas Implementadas](#-melhores-práticas-implementadas)
- [Testes](#-testes)
- [Configuração](#-configuração)
- [Contribuindo](#-contribuindo)
- [Licença](#-licença)

## 🎯 Visão Geral

A **Myth Template API** é uma demonstração abrangente de arquitetura de software empresarial usando o **Myth Framework**. Implementa uma API de Previsão do Tempo com operações CRUD completas, demonstrando:

- **Arquitetura Limpa** com camadas distintas de domínio, aplicação e infraestrutura
- **Princípios de Domain-Driven Design (DDD)** com aggregate roots e value objects
- **Padrão CQRS (Command Query Responsibility Segregation)**
- **Arquitetura Orientada a Eventos** com domain events
- **Padrões Repository e Specification**
- **Validação abrangente** com regras de negócio
- **Mapeamento de objetos type-safe** e transformações
- **Integração com APIs externas** com clientes REST
- **Logging, tratamento de erros e configuração prontos para produção**

### Por que Usar Este Template?

- ✅ **Desenvolvimento Acelerado**: Pule meses de decisões arquiteturais e configurações
- ✅ **Pronto para Produção**: Padrões e configurações testados em batalha
- ✅ **Escalável**: Separação clara de responsabilidades permite escalonamento independente
- ✅ **Mantível**: Princípios SOLID e estrutura clara reduzem débito técnico
- ✅ **Testável**: Injeção de dependência e padrões repository permitem testes fáceis
- ✅ **Qualidade Empresarial**: Validação abrangente, logging e tratamento de erros

## 🏗️ Arquitetura

Este template segue os princípios de **Arquitetura Limpa** com separação clara de responsabilidades:

```
┌─────────────────────────────────────────────────────────┐
│                   Camada da API                         │
│  ┌─────────────────────────────────────────────────┐    │
│  │              Controllers                        │    │
│  │  • Endpoints HTTP                               │    │
│  │  • Transformação Request/Response               │    │
│  │  • Pipelines Myth.Flow                          │    │
│  └─────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│                Camada de Aplicação                      │
│  ┌─────────────────┐ ┌────────────────┐ ┌─────────────┐ │
│  │  Commands       │ │  Queries       │ │  Events     │ │
│  │  • Create       │ │  • GetAll      │ │  • Created  │ │
│  │  • Update       │ │  • GetById     │ │  • Updated  │ │
│  │  • Delete       │ │                │ │  • Deleted  │ │
│  └─────────────────┘ └────────────────┘ └─────────────┘ │
│  ┌────────────────────────────────────────────────────┐ │
│  │                 DTOs & Handlers                    │ │
│  └────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│                  Camada de Domínio                      │
│  ┌────────────────────────────────────────────────────┐ │
│  │  • Aggregate Roots (WeatherForecast)               │ │
│  │  • Value Objects (Summary, DateOnly)               │ │
│  │  • Domain Events                                   │ │
│  │  │  • Regras de Negócio & Specifications           │ │
│  │  • Interfaces de Repository                        │ │
│  └────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│               Camada de Infraestrutura                  │
│  ┌─────────────────┐ ┌─────────────────┐ ┌────────────┐ │
│  │  Data           │ │  External Data  │ │  Tests     │ │
│  │  • EF Context   │ │  • REST Clients │ │  • Unit    │ │
│  │  • Repositories │ │  • APIs Externas│ │  • Integra-| |
|  |  • Mappings     | |  • Adapters     | |  tion      │ │
│  └─────────────────┘ └─────────────────┘ └────────────┘ │
└─────────────────────────────────────────────────────────┘
```

### Benefícios Arquiteturais

| Benefício | Descrição |
|-----------|------------|
| **Separação de Responsabilidades** | Cada camada tem responsabilidades distintas e dependências fluem para dentro |
| **Testabilidade** | Lógica de domínio isolada da infraestrutura; fácil de mockar dependências |
| **Manutenibilidade** | Mudanças em uma camada não afetam outras; fronteiras claras |
| **Escalabilidade** | Camadas podem ser escaladas independentemente; gargalos de performance claros |
| **Flexibilidade** | Fácil de trocar implementações (banco de dados, serviços externos) |

## ✨ Funcionalidades Principais

### 🎯 Domain-Driven Design (DDD)

- **Aggregate Roots**: `WeatherForecast` com lógica de negócio encapsulada
- **Value Objects**: enum `Summary`, `DateOnly` para type safety
- **Domain Events**: Publicação automática de eventos para ações de negócio
- **Specifications**: Lógica de consulta reutilizável e componível
- **Fluent API**: Interações intuitivas com o modelo de domínio

```csharp
// Exemplo: Criando uma previsão do tempo com regras de negócio
var forecast = new WeatherForecast(date, temperatureC, Summary.Warm)
    .ChangeTemperatureC(25)
    .ChangeSummary(Summary.Mild);
```

### 🔄 Implementação CQRS

**Separação completa de Commands (escritas) e Queries (leituras):**

| Operação | Tipo | Handler | Validação | Eventos |
|----------|------|---------|-----------|---------|
| Create | Command | `CreateWeatherForecastCommandHandler` | ✅ Regras de Negócio | ✅ Created Event |
| Update | Command | `UpdateWeatherForecastCommandHandler` | ✅ Verificação de Existência | ✅ Updated Event |
| Delete | Command | `DeleteWeatherForecastCommandHandler` | ✅ Verificação de Existência | ✅ Deleted Event |
| Get All | Query | `GetAllWeatherForecastsQueryHandler` | ✅ Validação de Filtros | ❌ Somente Leitura |
| Get By ID | Query | `GetWeatherForecastsByIdQueryHandler` | ✅ Validação de ID | ❌ Somente Leitura |

### 📊 Filtragem Avançada e Paginação

**Capacidades abrangentes de consulta:**

- 🌡️ **Faixa de Temperatura**: Filtrar por temperatura mínima/máxima (-100°C a 100°C)
- 📅 **Faixa de Data**: Filtrar por período de datas com precisão `DateOnly`
- 🌤️ **Resumo do Clima**: Filtrar por condições climáticas (Freezing, Warm, Hot, etc.)
- 📄 **Paginação**: Número de página e tamanho com contagem total
- 🔄 **Ordenação**: Resultados ordenados por data (mais recentes primeiro)
- 💾 **Cache**: Cache automático de resultados de consulta

```csharp
// Exemplo: Consulta com filtragem avançada
GET /api/v1/weatherforecast?summary=Warm&minimumDate=2024-01-01&maximumDate=2024-12-31&minimumTemperature=15&maximumTemperature=30&pageNumber=1&pageSize=20
```

### 🔔 Arquitetura Orientada a Eventos

**Tratamento automático de domain events:**

```csharp
// Evento publicado automaticamente na criação da previsão
public record WeatherForecastCreatedEvent : DomainEvent
{
    public Guid WeatherForecastId { get; init; }
}

// Handler responde aos eventos (baixo acoplamento)
public class WeatherForecastCreatedEventHandler : IEventHandler<WeatherForecastCreatedEvent>
{
    public async Task HandleAsync(WeatherForecastCreatedEvent @event, CancellationToken cancellationToken)
    {
        // Log da criação
        _logger.LogInformation("Previsão do tempo criada: {Id}", @event.WeatherForecastId);

        // Buscar recomendação aleatória de cervejaria 🍺
        var brewery = await _breweryRepository.GetRandomBreweryAsync(cancellationToken);
        _logger.LogInformation("Cervejaria recomendada: {Name}", brewery.Name);
    }
}
```

### 🛡️ Validação Abrangente

**Validação multicamada com regras de negócio:**

```csharp
// Validação fluente com verificações assíncronas no banco
builder.For(Date, rules => rules
    .Past()                                    // Deve estar no passado
    .GreaterThan(DateOnly.MinValue)           // Data válida
    .RespectAsync(async (date, ct, sp) => {   // Regra de negócio assíncrona
        var repository = sp.GetRequiredService<IWeatherForecastRepository>();
        var spec = SpecBuilder<WeatherForecast>.Create().WithDateNotInUse(date);
        return await repository.AllAsync(spec, ct);
    })
    .WithStatusCode(HttpStatusCode.Conflict)
    .WithMessage("Já existe previsão do tempo para esta data"));

builder.For(TemperatureC, rules => rules
    .Between(-100, 100)                       // Faixa de temperatura realista
    .WithMessage("Temperatura deve estar entre -100°C e 100°C"));
```

### 🔗 Integração com APIs Externas

**Integração de cliente REST pronta para produção:**

```csharp
// Cliente REST configurado com deserialização automática
builder.Services.AddRestConfiguration("brewery", conf => conf
    .WithBaseUrl("https://api.openbrewerydb.org/v1/")
    .WithBodyDeserialization(CaseStrategy.SnakeCase));

// Repository usando API REST fluente
public async Task<BreweryResponseDto> GetRandomBreweryAsync(CancellationToken cancellationToken)
{
    var request = await _client
        .DoGet("breweries/random")
        .OnResult(res => res.UseTypeForSuccess<IEnumerable<BreweryResponseDto>>())
        .OnError(err => err.ThrowForNonSuccess())
        .BuildAsync(cancellationToken);

    return request.GetAs<IEnumerable<BreweryResponseDto>>().First();
}
```

## 🛠️ Stack Tecnológico

### Framework Principal
- **.NET 10.0** - Versão mais recente
- **ASP.NET Core 10.0** - Framework web de alta performance
- **Entity Framework Core 10.0** - Mapeamento objeto-relacional

### Ecossistema Myth Framework (v3.0.5-preview.13)

| Pacote | Propósito | Benefícios Principais |
|--------|-----------|----------------------|
| **Myth.Commons** | Utilitários comuns e extensões | Classes base, métodos auxiliares |
| **Myth.Flow** | Framework de orquestração de pipelines | Pipelines de request/response, middleware |
| **Myth.Flow.Actions** | Dispatch CQRS de commands/queries | Auto-descoberta de handlers, dispatch type-safe |
| **Myth.Guard** | Biblioteca de validação fluente | Regras de negócio, validação assíncrona, erros customizados |
| **Myth.Morph** | Mapeamento de objetos type-safe | Mapeamento baseado em schema, sem overhead de reflection |
| **Myth.Rest** | Factory de cliente REST | Clientes HTTP fluentes, gerenciamento de configuração |
| **Myth.Repository.EntityFramework** | Padrão Repository com EF | Repositories genéricos, specifications, unit of work |

### Dependências Adicionais
- **Swashbuckle.AspNetCore** - Documentação da API (Swagger/OpenAPI)
- **Microsoft.EntityFrameworkCore.InMemory** - Banco em memória para desenvolvimento/testes

## 🚀 Primeiros Passos

### Pré-requisitos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/vs/), [JetBrains Rider](https://www.jetbrains.com/rider/), ou [VS Code](https://code.visualstudio.com/)

### Início Rápido

1. **Clone ou use como template:**
   ```bash
   git clone https://github.com/sua-org/myth-template-api.git
   cd myth-template-api
   ```

2. **Restaurar pacotes:**
   ```bash
   dotnet restore
   ```

3. **Executar a aplicação:**
   ```bash
   dotnet run --project Myth.Template.API
   ```

4. **Explorar a API:**
   - 🌐 **Swagger UI**: [https://localhost:7296/swagger](https://localhost:7296/swagger)
   - 🔍 **Health Check**: [https://localhost:7296/health](https://localhost:7296/health)
   - ⚡ **API de Exemplo**: [https://localhost:7296/api/v1/weatherforecast](https://localhost:7296/api/v1/weatherforecast)

### Configuração do Template

Este repositório é um projeto template. Use o script PowerShell de configuração para personalizá-lo para seu projeto.

#### Pré-requisitos

- PowerShell 5.0 ou superior
- Git instalado
- .NET 10 SDK instalado

#### Uso Básico

```powershell
# Mantém exemplos de WeatherForecast para referência
.\Setup-Template.ps1 -Name "MinhaEmpresa.MeuProjeto"
```

#### Configuração Limpa (remove exemplos)

```powershell
# Remove todos os exemplos de WeatherForecast e cria estrutura limpa
.\Setup-Template.ps1 -Name "MinhaEmpresa.MeuProjeto" -Clean
```

#### Parâmetros

- **`-Name`** (obrigatório): Nome do novo projeto
  - Substitui `Myth.Template` em pastas, namespaces e arquivos
  - Pode usar pontos, hífens e underscores
  - Exemplo: `"MinhaEmpresa.MeuProjeto"`

- **`-Clean`** (opcional): Remove exemplos de WeatherForecast
  - Limpa todos os arquivos relacionados ao WeatherForecast
  - Cria um `AppContext` base para começar
  - Deixa estrutura limpa para desenvolvimento

#### O Que o Script Faz

1. **Renomeação de arquivos e pastas** - Renomeia todas as pastas e arquivos `Myth.Template.*`
2. **Atualização de conteúdo** - Substitui namespaces em arquivos `.cs`, `.csproj`, `.slnx`, `.json`, `.resx`, `.md`
3. **Limpeza (se `-Clean` especificado)** - Remove exemplos de WeatherForecast e cria AppContext base
4. **Reinicialização do Git** - Cria novo repositório com commit inicial
5. **Auto-limpeza** - Remove o script de configuração e arquivos de documentação

#### Exemplo Completo

```bash
# Clone o template
git clone https://github.com/seu-usuario/myth-template.git meu-novo-projeto
cd meu-novo-projeto

# Configure com limpeza
.\Setup-Template.ps1 -Name "MinhaEmpresa.Vendas" -Clean

# Configure repositório remoto
git remote add origin https://github.com/minha-empresa/vendas-api.git

# Verifique se tudo está funcionando
dotnet build
```

#### Resolução de Problemas

**Erro de permissão:**
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

**Verificar resultado:**
Após execução, verifique se:
- Todos os namespaces foram renomeados
- Projeto compila sem erros: `dotnet build`
- Testes passam: `dotnet test` (se não usou `-Clean`)

### Configuração de Desenvolvimento

1. **Banco de desenvolvimento** (Em memória por padrão):
   - Populado automaticamente com 1000 previsões de exemplo
   - Dados históricos dos últimos 1000 dias
   - Pronto para testes imediatos

2. **Documentação da API**:
   - Especificação completa OpenAPI/Swagger
   - Exemplos de request/response
   - Descrições de erros de validação

## 📁 Estrutura do Projeto

```
Myth.Template.API/
├── 🏗️ Myth.Template.API/                    # Camada da API Web
│   ├── Controllers/                         # Endpoints HTTP
│   ├── Program.cs                           # Startup da aplicação
│   └── appsettings.json                     # Configurações
│
├── 🎯 Myth.Template.Domain/                 # Camada de Domínio (Lógica de Negócio)
│   ├── Models/                              # Aggregate roots & value objects
│   │   ├── WeatherForecast.cs               # Aggregate root principal
│   │   └── Summary.cs                       # Value object (enum)
│   ├── Interfaces/                          # Contratos de Repository
│   └── Specifications/                      # Specifications de consulta
│
├── 🔄 Myth.Template.Application/            # Camada de Aplicação (Casos de Uso)
│   ├── WeatherForecasts/
│   │   ├── Commands/                        # Operações de escrita
│   │   │   ├── Create/                      # Criar previsão
│   │   │   ├── Update/                      # Atualizar previsão
│   │   │   └── Delete/                      # Deletar previsão
│   │   ├── Queries/                         # Operações de leitura
│   │   │   ├── GetAll/                      # Listar com filtros
│   │   │   └── GetById/                     # Previsão individual
│   │   ├── Events/                          # Domain events
│   │   └── DTOs/                            # Data transfer objects
│   └── InitializeFakeData.cs                # População de dados de desenvolvimento
│
├── 💾 Myth.Template.Data/                   # Camada de Acesso a Dados
│   ├── Contexts/                            # Contextos do Entity Framework
│   ├── Mappings/                            # Configurações de entidades
│   └── Repositories/                        # Implementações de acesso a dados
│
├── 🌐 Myth.Template.ExternalData/           # Integrações Externas
│   └── Breweries/                           # Exemplo de integração com API externa
│
└── 🧪 Myth.Template.Test/                   # Projetos de Teste
    └── WeatherForecastTests.cs              # Testes unitários
```

### Responsabilidades das Camadas

| Camada | Responsabilidades | Dependências |
|--------|-------------------|--------------|
| **API** | Endpoints HTTP, tratamento de request/response, autenticação | Application |
| **Application** | Casos de uso, handlers de command/query, DTOs, eventos | Domain |
| **Domain** | Lógica de negócio, aggregate roots, domain services, specifications | Nenhuma |
| **Data** | Entity Framework, repositories, mapeamentos de banco | Domain, Application |
| **ExternalData** | Integração com serviços externos, clientes REST | Application |
| **Test** | Testes unitários, testes de integração, fixtures de teste | Todas as camadas |

## 🎨 Padrões de Design

Este template demonstra implementação profissional de padrões de design chave:

### 🏛️ Padrão Repository

```csharp
// Repository genérico com specifications
public interface IWeatherForecastRepository : IReadWriteRepositoryAsync<WeatherForecast>
{
    // Métodos específicos do domínio podem ser adicionados aqui
}

// Implementação com EF Core
public class WeatherForecastRepository : ReadWriteRepositoryAsync<WeatherForecast>, IWeatherForecastRepository
{
    public WeatherForecastRepository(ForecastContext context) : base(context) { }
}

// Uso em handlers
var forecasts = await _repository.SearchPaginatedAsync(specification, cancellationToken);
```

### 📋 Padrão Specification

```csharp
// Lógica de consulta componível e reutilizável
var spec = SpecBuilder<WeatherForecast>
    .Create()
    .WithSummary(query.Summary)                    // Filtro opcional
    .WithDateGreaterThan(query.MinimumDate)        // Filtro opcional
    .WithDateLowerThan(query.MaximumDate)          // Filtro opcional
    .WithTemparatureGreaterThan(query.MinimumTemperature) // Filtro opcional
    .WithTemparatureLowerThan(query.MaximumTemperature)   // Filtro opcional
    .OrderDescending(x => x.Date)                  // Ordenação consistente
    .WithPagination(query);                        // Paginação

// Executar com type safety
var result = await repository.SearchPaginatedAsync(spec, cancellationToken);
```

### 🔄 Padrão Unit of Work

```csharp
// Consistência transacional entre operações
public async Task<WeatherForecastCreatedEvent> HandleAsync(CreateWeatherForecastCommand command, CancellationToken cancellationToken)
{
    var weatherForecast = new WeatherForecast(command.Date, command.TemperatureC, command.Summary);

    await _repository.AddAsync(weatherForecast, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);  // Transação única

    return new WeatherForecastCreatedEvent { WeatherForecastId = weatherForecast.WeatherForecastId };
}
```

### 🎭 Padrão Command (CQRS)

```csharp
// Commands são DTOs imutáveis com validação
public record CreateWeatherForecastCommand : ICommand<WeatherForecastCreatedEvent>, IValidatable
{
    public DateOnly Date { get; init; }
    public int TemperatureC { get; init; }
    public Summary Summary { get; init; }

    public void Validate(ValidationBuilder<CreateWeatherForecastCommand> builder, ValidationContextKey? context = null)
    {
        // Validação fluente com regras de negócio
    }
}

// Handlers têm responsabilidade única
public class CreateWeatherForecastCommandHandler : ICommandHandler<CreateWeatherForecastCommand, WeatherForecastCreatedEvent>
{
    public async Task<WeatherForecastCreatedEvent> HandleAsync(CreateWeatherForecastCommand command, CancellationToken cancellationToken)
    {
        // Implementação
    }
}
```

### 🔔 Padrão Observer (Eventos)

```csharp
// Baixo acoplamento através de domain events
public record WeatherForecastCreatedEvent : DomainEvent
{
    public Guid WeatherForecastId { get; init; }
}

// Múltiplos handlers podem responder ao mesmo evento
public class WeatherForecastCreatedEventHandler : IEventHandler<WeatherForecastCreatedEvent>
{
    public async Task HandleAsync(WeatherForecastCreatedEvent @event, CancellationToken cancellationToken)
    {
        // Efeitos colaterais: logging, notificações, chamadas de API externa
    }
}
```

## 💎 Benefícios do Myth Framework

O Myth Framework fornece vantagens significativas sobre o desenvolvimento ASP.NET Core tradicional:

### 🚀 Velocidade de Desenvolvimento

| Abordagem Tradicional | Com Myth Framework |
|----------------------|-------------------|
| Configuração manual de pipeline | `PipelineExtensions.Start()` |
| Framework de validação customizado | `Myth.Guard` com API fluente |
| Mapeamento manual de objetos | `Myth.Morph` mapeamento type-safe |
| Configuração de HttpClient | `Myth.Rest` clientes REST fluentes |
| Boilerplate de Repository | Repositories genéricos com specifications |
| Tratamento manual de eventos | Descoberta e dispatch automático de eventos |

### 🏗️ Arquitetura de Pipeline

**Controller Tradicional:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateAsync([FromBody] CreateWeatherForecastRequest request)
{
    // Validação manual
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    // Mapeamento manual
    var command = new CreateWeatherForecastCommand
    {
        Date = request.Date,
        TemperatureC = request.TemperatureC,
        Summary = request.Summary
    };

    // Invocação manual do handler
    var result = await _handler.HandleAsync(command);

    // Publicação manual de evento
    await _eventPublisher.PublishAsync(new WeatherForecastCreatedEvent { Id = result.Id });

    return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, result);
}
```

**Com Pipeline Myth:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateAsync([FromBody] CreateWeatherForecastRequest request, CancellationToken cancellationToken)
{
    var result = await PipelineExtensions
        .Start(request.To<CreateWeatherForecastCommand>())                  // Mapeamento type-safe
        .TapAsync(pipeline => _validator.ValidateAsync(pipeline.CurrentRequest!)) // Validação automática
        .Tap(pipeline => _logger.LogDebug("Command validado com sucesso"))        // Efeitos colaterais
        .Process<CreateWeatherForecastCommand, WeatherForecastCreatedEvent>()     // Dispatch de handler
        .Publish()                                                               // Publicação de evento
        .Tap(pipeline => _logger.LogInformation("Previsão criada: {Id}",
            pipeline.CurrentRequest!.WeatherForecastId))                        // Log de sucesso
        .ExecuteAsync(cancellationToken);                                       // Execução assíncrona

    return result.Match(
        success => CreatedAtAction(nameof(GetByIdAsync),
            new { id = success.WeatherForecastId }, success),
        error => StatusCode((int)error.StatusCode, error.Message));
}
```

### 🎯 Detalhamento de Benefícios

| Funcionalidade | Tradicional | Myth Framework | Benefício |
|----------------|-------------|----------------|-----------|
| **Validação** | Verificações manuais do `ModelState` | Automática com `Myth.Guard` | Type-safe, regras de negócio, validação assíncrona |
| **Mapeamento** | Atribuição manual de propriedades | `request.To<Command>()` | Zero configuração, type-safe |
| **Logging** | Chamadas espalhadas de `_logger` | Pipeline `.Tap()` | Logging consistente e estruturado |
| **Tratamento de Erro** | Blocos try-catch | Tratamento de erro integrado no pipeline | Centralizado, respostas consistentes |
| **Publicação de Evento** | Chamadas manuais do dispatcher de evento | Automático com `.Publish()` | Zero configuração, descoberta automática |
| **Caching** | Implementação manual de cache | Configuração `.UseCache()` | Declarativo, configurável |
| **Retries** | Lógica de retry customizada | Configuração `.UseRetry(3)` | Backoff exponencial, circuit breaker |
| **Telemetria** | Rastreamento manual de performance | `.UseTelemetry()` | Métricas automáticas, tracing |

### 🔄 Comparação de Mapeamento de Objetos

**AutoMapper Tradicional:**
```csharp
// Configuração necessária
CreateMap<CreateWeatherForecastRequest, CreateWeatherForecastCommand>();
CreateMap<WeatherForecast, GetWeatherForecastResponse>()
    .ForMember(dest => dest.SummaryDescription, opt => opt.MapFrom(src => Enum.GetName(src.Summary)))
    .ForMember(dest => dest.SummaryId, opt => opt.MapFrom(src => (int)src.Summary));

// Mapeamento em runtime (possíveis erros)
var command = _mapper.Map<CreateWeatherForecastCommand>(request);
```

**Myth.Morph Baseado em Schema:**
```csharp
// Mapeamento type-safe em tempo de compilação
public record CreateWeatherForecastRequest : IMorphableTo<CreateWeatherForecastCommand>
{
    public void MorphTo(Schema<CreateWeatherForecastCommand> schema)
    {
        // Correspondência automática de propriedades, lógica customizada apenas quando necessário
    }
}

public record GetWeatherForecastResponse : IMorphableFrom<WeatherForecast>
{
    public void MorphFrom(Schema<WeatherForecast> schema)
    {
        schema.Bind(() => SummaryDescription, src => Enum.GetName(src.Summary));
        schema.Bind(() => SummaryId, src => (int)src.Summary);
    }
}

// Mapeamento seguro em tempo de compilação
var command = request.To<CreateWeatherForecastCommand>();
var response = weatherForecast.To<GetWeatherForecastResponse>();
```

## 📚 Documentação da API

### Integração Swagger/OpenAPI

Documentação completa da API com:
- 📖 **Documentação Interativa**: Swagger UI com funcionalidade de testar
- 🔍 **Definições de Schema**: Modelos de request/response com regras de validação
- ✅ **Exemplos de Response**: Requests e responses de exemplo para todos os endpoints
- ❌ **Respostas de Erro**: Schemas de erro detalhados com códigos de status

### Visão Geral dos Endpoints da API

| Método | Endpoint | Descrição | Request | Response |
|--------|----------|-----------|---------|----------|
| **POST** | `/api/v1/weatherforecast` | Criar previsão | `CreateWeatherForecastRequest` | `201 Created` + `Location` |
| **GET** | `/api/v1/weatherforecast` | Listar previsões | Parâmetros de query | `IPaginated<GetWeatherForecastResponse>` |
| **GET** | `/api/v1/weatherforecast/{id}` | Obter previsão | GUID na rota | `GetWeatherForecastResponse` |
| **PUT** | `/api/v1/weatherforecast` | Atualizar previsão | `UpdateWeatherForecastRequest` | `204 No Content` |
| **DELETE** | `/api/v1/weatherforecast` | Deletar previsão | `DeleteWeatherForecastRequest` | `204 No Content` |

### Exemplo de Schema de Response

```json
{
  "items": [
    {
      "weatherForecastId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-01-15",
      "temperatureC": 25,
      "temperatureF": 77,
      "summaryId": 6,
      "summaryDescription": "Warm",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": null
    }
  ],
  "totalCount": 150,
  "pageNumber": 1,
  "pageSize": 20,
  "totalPages": 8,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

## 💡 Exemplos

### Criando uma Previsão do Tempo

```bash
curl -X POST "https://localhost:7296/api/v1/weatherforecast" \
  -H "Content-Type: application/json" \
  -d '{
    "date": "2024-01-15",
    "temperatureC": 25,
    "summary": 6
  }'
```

**Response:**
```http
HTTP/1.1 201 Created
Location: /api/v1/weatherforecast/3fa85f64-5717-4562-b3fc-2c963f66afa6
Content-Type: application/json

{
  "weatherForecastId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Consulta com Filtragem Avançada

```bash
# Obter previsões de clima quente do ano passado, paginadas
curl "https://localhost:7296/api/v1/weatherforecast?summary=6&minimumDate=2023-01-01&maximumDate=2023-12-31&minimumTemperature=20&maximumTemperature=30&pageNumber=1&pageSize=10"
```

### Exemplo de Erro de Validação

```bash
curl -X POST "https://localhost:7296/api/v1/weatherforecast" \
  -H "Content-Type: application/json" \
  -d '{
    "date": "2025-01-15",
    "temperatureC": 150,
    "summary": 999
  }'
```

**Response:**
```http
HTTP/1.1 400 Bad Request
Content-Type: application/json

{
  "errors": [
    {
      "field": "Date",
      "message": "Data deve estar no passado",
      "code": "PAST_DATE_REQUIRED"
    },
    {
      "field": "TemperatureC",
      "message": "Temperatura deve estar entre -100°C e 100°C",
      "code": "TEMPERATURE_OUT_OF_RANGE"
    },
    {
      "field": "Summary",
      "message": "Valor de resumo do clima inválido",
      "code": "INVALID_ENUM_VALUE"
    }
  ]
}
```

## 🏆 Melhores Práticas Implementadas

### Princípios SOLID

- ✅ **Single Responsibility**: Cada handler, repository e service tem um propósito claro
- ✅ **Open/Closed**: Extensões de pipeline permitem adicionar comportamento sem modificar lógica central
- ✅ **Liskov Substitution**: Handlers genéricos implementam interfaces padrão consistentemente
- ✅ **Interface Segregation**: Interfaces específicas para cada responsabilidade (repository, validação, mapeamento)
- ✅ **Dependency Inversion**: Todas as dependências injetadas, dependendo de abstrações não de implementações concretas

### Práticas de Clean Code

- ✅ **Nomes Significativos**: `GetAllWeatherForecastsQueryHandler` descreve claramente o propósito
- ✅ **Funções Pequenas**: Comprimento médio de método ~30 linhas, responsabilidade única
- ✅ **Sem Números Mágicos**: Constantes e enums para valores significativos
- ✅ **Princípio DRY**: Lógica compartilhada em specifications e classes base
- ✅ **Formatação Consistente**: `.editorconfig` força padrões da equipe

### Padrões Empresariais

- ✅ **Domain-Driven Design**: Aggregate roots, value objects, domain services
- ✅ **CQRS**: Separação clara de operações de leitura e escrita
- ✅ **Event Sourcing**: Domain events capturam ações de negócio
- ✅ **Padrão Repository**: Abstração sobre acesso a dados com specifications
- ✅ **Unit of Work**: Consistência transacional entre operações

### Segurança e Prontidão para Produção

- ✅ **Validação de Entrada**: Validação abrangente com regras de negócio
- ✅ **Tratamento de Erro**: Respostas de erro consistentes com códigos HTTP apropriados
- ✅ **Logging**: Logging estruturado com IDs de correlação
- ✅ **Health Checks**: Monitoramento de endpoint para deploys em produção
- ✅ **Configuração**: Configurações específicas do ambiente com validação

## 🧪 Testes

### Estrutura de Testes

O template inclui uma base para testes abrangentes:

```
Myth.Template.Test/
├── Unit Tests/
│   ├── Handlers/              # Testes de handlers de command e query
│   ├── Validators/            # Testes de lógica de validação
│   ├── Specifications/        # Testes de specifications de query
│   └── Domain/               # Testes de comportamento de aggregate root
├── Integration Tests/
│   ├── API/                  # Testes de API end-to-end
│   ├── Database/             # Testes de integração de repository
│   └── ExternalServices/     # Testes de integração de API externa
└── TestFixtures/             # Utilitários de teste compartilhados e dados
```

### Benefícios de Teste desta Arquitetura

| Componente | Abordagem de Teste | Benefícios |
|------------|------------------|------------|
| **Modelos de Domínio** | Testes unitários com funções puras | Sem dependências, execução rápida |
| **Handlers** | Testes unitários com repositories mockados | Teste de lógica de negócio isolada |
| **Repositories** | Testes de integração com DB em memória | Teste de acesso a dados real |
| **Specifications** | Testes unitários com dados de exemplo | Verificação de lógica de query |
| **Validators** | Testes unitários com várias entradas | Verificação de regras de negócio |
| **Controllers** | Testes de integração com cliente de teste | Teste de API end-to-end |

### Exemplos de Teste

```csharp
[Test]
public async Task CreateWeatherForecast_ValidData_ReturnsCreatedEvent()
{
    // Arrange
    var command = new CreateWeatherForecastCommand
    {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
        TemperatureC = 25,
        Summary = Summary.Warm
    };

    // Act
    var result = await _handler.HandleAsync(command, CancellationToken.None);

    // Assert
    Assert.That(result.WeatherForecastId, Is.Not.EqualTo(Guid.Empty));
}

[Test]
public async Task CreateWeatherForecast_DuplicateDate_ThrowsValidationException()
{
    // Arrange
    var existingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
    await SeedWeatherForecast(existingDate);

    var command = new CreateWeatherForecastCommand
    {
        Date = existingDate,
        TemperatureC = 20,
        Summary = Summary.Cool
    };

    // Act & Assert
    var exception = await Assert.ThrowsAsync<ValidationException>(() =>
        _handler.HandleAsync(command, CancellationToken.None));

    Assert.That(exception.Errors.First().Code, Is.EqualTo("CONFLICT"));
}
```

## ⚙️ Configuração

### Configurações da Aplicação

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=weather_forecast.db"
  },
  "ExternalApis": {
    "BreweryApi": {
      "BaseUrl": "https://api.openbrewerydb.org/v1/",
      "Timeout": "00:00:30"
    }
  },
  "Cache": {
    "DefaultExpiration": "00:05:00",
    "QueryCacheExpiration": "00:02:00"
  },
  "Pagination": {
    "DefaultPageSize": 20,
    "MaxPageSize": 100
  }
}
```

### Configuração por Ambiente

| Ambiente | Banco de Dados | Nível de Log | Cache | APIs Externas |
|----------|---------------|-------------|-------|---------------|
| **Development** | Em Memória | Debug | Desabilitado | APIs Reais |
| **Testing** | Em Memória | Warning | Desabilitado | Mockadas |
| **Staging** | SQL Server | Information | Redis | APIs Reais |
| **Production** | SQL Server | Warning | Redis | APIs Reais |

### Configuração de Startup

```csharp
// Configuração do banco de dados
builder.Services.AddDbContext<ForecastContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseInMemoryDatabase("WeatherForecastDb");
    else
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configuração do Myth Framework
builder.Services.AddFlow(config => config
    .UseLogging()                              // Logging de request/response
    .UseExceptionFilter<ValidationException>() // Tratamento de erro de validação
    .UseTelemetry()                           // Métricas de performance
    .UseRetry(retryCount: 3)                  // Retry automático com backoff
    .UseCache(defaultExpiration: TimeSpan.FromMinutes(5)) // Cache de resultado
    .UseActions(x => x
        .UseInMemory()                        // Store de action em memória
        .ScanAssemblies(typeof(CreateWeatherForecastCommandHandler).Assembly)));

// Configuração de cliente REST
builder.Services.AddRestFactory()
    .AddRestConfiguration("brewery", conf => conf
        .WithBaseUrl(builder.Configuration["ExternalApis:BreweryApi:BaseUrl"])
        .WithTimeout(TimeSpan.Parse(builder.Configuration["ExternalApis:BreweryApi:Timeout"]))
        .WithBodyDeserialization(CaseStrategy.SnakeCase));
```

## 🤝 Contribuindo

Bem-vindas as contribuições! Este template serve tanto como implementação de referência quanto como ponto de partida para seus próprios projetos.

### Como Contribuir

1. **Fork o repositório**
2. **Crie uma branch de feature**: `git checkout -b feature/funcionalidade-incrivel`
3. **Siga as convenções de código** definidas em `.editorconfig`
4. **Adicione testes** para nova funcionalidade
5. **Atualize a documentação** se necessário
6. **Submeta um pull request**

### Padrões de Código

- Siga os **princípios SOLID** e práticas de **Clean Code**
- Use **nomes de variáveis significativos** e **funções de responsabilidade única**
- Adicione **documentação XML** para todos os métodos e classes públicas
- Escreva **testes abrangentes** para novas funcionalidades
- Garanta **tratamento apropriado de erros** e **validação**

### Áreas para Contribuição

- 🧪 **Exemplos de teste adicionais** e utilitários de teste
- 📚 **Documentação aprimorada** e tutoriais
- 🔧 **Implementações de padrão de design adicionais**
- 🌐 **Mais exemplos de integração com API externa**
- ⚡ **Otimizações de performance** e benchmarks
- 🔒 **Melhorias de segurança** e exemplos de autenticação

## 📄 Licença

Este projeto está licenciado sob a **Licença MIT** - veja o arquivo [LICENSE](LICENSE) para detalhes.

---

## 🎯 Resumo

A **Myth Template API** demonstra arquitetura de software de nível empresarial com:

- ✅ **Arquitetura Limpa** garantindo código mantível e testável
- ✅ **Domain-Driven Design** com modelos de domínio ricos e lógica de negócio
- ✅ **Padrão CQRS** separando operações de leitura e escrita
- ✅ **Arquitetura Orientada a Eventos** permitindo baixo acoplamento e extensibilidade
- ✅ **Validação Abrangente** com regras de negócio e verificações assíncronas
- ✅ **Mapeamento Type-Safe** eliminando erros de mapeamento em runtime
- ✅ **Funcionalidades Prontas para Produção** incluindo logging, tratamento de erro e monitoramento
- ✅ **Integração do Myth Framework** acelerando desenvolvimento com padrões comprovados

**Perfeito para:**
- Desenvolvimento de API empresarial
- Aprender padrões modernos de .NET
- Padrões de arquitetura da equipe
- Base de projeto pronta para produção

**Comece a construir sua próxima API empresarial com o poder do ecossistema Myth! 🚀**

---

*Para perguntas, issues, ou contribuições, visite nosso [repositório GitHub](https://github.com/sua-org/myth-template-api) ou confira a [documentação do Myth Framework](https://docs.mythframework.io).*
