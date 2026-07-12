# MicroDemo — Microservice Demo (.NET 8 · Clean Architecture · CQRS)

Solution de demonstração de um microserviço em **.NET 8** seguindo **Clean Architecture** com **DDD leve**, **CQRS (MediatR)**, **EF Core (Code First + Migrations)**, **FluentValidation** e **Swagger**.

Domínio: gestão de **Utenti** (usuários/clientes) e **Prices** (preços/planos), com o relacionamento entre eles modelado por **Subscriptions**, além de **Orders** (pedidos/cobranças).

---

## 1. Arquitetura e camadas

```
MicroDemo.sln
├── src/
│   ├── MicroDemo.Domain          → Entidades, enums, regras invariantes. Sem dependências externas.
│   ├── MicroDemo.Application      → Casos de uso (CQRS): Commands/Queries, Handlers,
│   │                                Validators, DTOs, Result<T>, behaviors do MediatR.
│   ├── MicroDemo.Infrastructure   → EF Core: AppDbContext, configurations, migrations, DI.
│   └── MicroDemo.Api              → ASP.NET Core Web API: controllers, middleware, Swagger.
├── tests/
│   └── MicroDemo.UnitTests        → xUnit + FluentAssertions + Moq (organização customizada, ver §6).
└── sql/
    ├── schema.sql                 → CREATE DATABASE + CREATE TABLE de todo o domínio.
    └── seed.sql                   → INSERTs com dados de exemplo realistas.
```

**Regra de dependência** (aponta sempre para dentro):
`Api → Application → Domain` e `Api → Infrastructure → Application → Domain`.
A camada Application não conhece a Infrastructure — depende apenas da abstração `IAppDbContext`, implementada pelo `AppDbContext` da Infrastructure.

### Padrões aplicados
- **CQRS com MediatR**: cada operação é um `Command` ou `Query` com seu próprio `Handler`.
- **Pipeline behaviors**: `LoggingBehavior` (log de request/response) e `ValidationBehavior` (executa os `FluentValidation` antes do handler).
- **Result / envelope padronizado**: `Result` e `Result<T>` (`Common/Models/Result.cs`) evitam exceções para fluxos de negócio esperados; o `ApiControllerBase` mapeia o `ResultErrorType` para o status HTTP (`404`, `409`, `400`...).
- **DTOs + mapeamento manual** (extensões `ToDto()`), sem AutoMapper, para manter a demo enxuta.
- **Logging** via `ILogger<T>` nativo.

### Entidades do domínio
| Entidade | Descrição |
|----------|-----------|
| `Utente` | Usuário/cliente. Pode ter um `DefaultPrice` (plano padrão). |
| `Price` | Preço/plano (valor, moeda, periodicidade de cobrança). |
| `Subscription` | Associação temporal entre um `Utente` e um `Price` (entidade central do relacionamento). |
| `Order` | Pedido/cobrança de um utente, opcionalmente ligado a uma assinatura. |

---

## 2. Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- **SQL Server** — LocalDB (padrão no Windows/Visual Studio) ou uma instância SQL Server/Express.
- Ferramenta EF Core CLI (para migrations manuais):
  ```bash
  dotnet tool install --global dotnet-ef --version 8.0.7
  ```

---

## 3. Connection string

Definida em `src/MicroDemo.Api/appsettings.json` → `ConnectionStrings:DefaultConnection`:

```json
"Server=(localdb)\\MSSQLLocalDB;Database=MicroDemoDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Ajuste `Server=` conforme sua instância. Para não versionar segredos, prefira sobrescrever via **User Secrets** ou variável de ambiente:

```bash
# Variável de ambiente (Windows PowerShell)
$env:ConnectionStrings__DefaultConnection = "Server=...;Database=MicroDemoDb;User Id=...;Password=...;TrustServerCertificate=True"
```

---

## 4. Como rodar

### Opção A — Migrations do EF Core (recomendado)
O banco é criado/atualizado pelas migrations. Em ambiente **Development**, a API aplica as migrations automaticamente ao subir (`db.Database.Migrate()` no `Program.cs`).

```bash
# na raiz da solution
dotnet build

# aplicar migrations manualmente (opcional — a API já faz isso em Development)
dotnet ef database update --project src/MicroDemo.Infrastructure --startup-project src/MicroDemo.Api

# rodar a API
dotnet run --project src/MicroDemo.Api
```

Acesse o **Swagger UI** em: `https://localhost:<porta>/swagger` (a porta aparece no console).

Para popular dados de exemplo, rode o `sql/seed.sql` na base criada (ver Opção B).

### Opção B — Scripts SQL (sem migrations)
Se preferir criar tudo por SQL puro:

```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -i sql/schema.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -i sql/seed.sql
```

> Os scripts são idempotentes (dropam/limpam antes de recriar/inserir).

### Comandos úteis de migration
```bash
# criar nova migration
dotnet ef migrations add <Nome> --project src/MicroDemo.Infrastructure --startup-project src/MicroDemo.Api --output-dir Persistence/Migrations

# gerar script SQL a partir das migrations
dotnet ef migrations script --project src/MicroDemo.Infrastructure --startup-project src/MicroDemo.Api
```

---

## 5. Endpoints

Base: `/api`. Todos retornam JSON e usam o envelope `Result<T>` mapeado para status HTTP.

### Utenti — CRUD completo ✅
| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/api/utenti?search=&onlyActive=` | Lista utenti (filtro opcional) |
| GET    | `/api/utenti/{id}` | Obtém por id |
| POST   | `/api/utenti` | Cria (valida e-mail único e existência do `DefaultPriceId`) |
| PUT    | `/api/utenti/{id}` | Atualiza |
| DELETE | `/api/utenti/{id}` | Remove |

### Prices — CRUD completo ✅
| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/api/prices?onlyActive=` | Lista preços |
| GET    | `/api/prices/{id}` | Obtém por id |
| POST   | `/api/prices` | Cria |
| PUT    | `/api/prices/{id}` | Atualiza |
| DELETE | `/api/prices/{id}` | Remove (bloqueia se houver assinaturas → `409`) |

### Subscriptions
| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/api/subscriptions/{id}` | Obtém por id ✅ |
| POST   | `/api/subscriptions` | **★ RESERVADO PARA IMPLEMENTAÇÃO MANUAL — ver §7** |

---

## 6. ⚠️ Organização CUSTOMIZADA dos testes (particularidade desta solution)

Os testes **não** são organizados por classe/feature (o convencional seria `CreatePriceCommandHandlerTests.cs`, `CreatePriceCommandValidatorTests.cs`...). Em vez disso, são organizados **por TIPO DE ARTEFATO**, um arquivo por tipo:

| Arquivo | Contém |
|---------|--------|
| `RequestTests.cs`   | **Builders de request** (`PriceRequests`, `UtenteRequests`, `SubscriptionRequests`) — a montagem dos commands/queries usados nos testes, com variações válidas/inválidas. |
| `ResponseTests.cs`  | **Builders de response/DTO esperados** e helpers de asserção sobre o `Result<T>` (`ResultAssertions`). |
| `HandlerTests.cs`   | **Todos os testes dos handlers do MediatR** (EF Core InMemory + Moq para `ILogger`). |
| `ValidatorTests.cs` | **Todos os testes dos validadores** (FluentValidation `TestValidate`). |

**Por que assim?** Centralizar os requests e responses em arquivos próprios permite **reutilizá-los** entre `HandlerTests` e `ValidatorTests` sem duplicar a montagem de objetos — há um único ponto para ajustar os "dados válidos padrão". Os arquivos `RequestTests`/`ResponseTests` mantêm o sufixo `Tests` (e um punhado de smoke-tests) por consistência de nomenclatura e para que os builders sejam validados pelo próprio runner.

Rodar os testes:
```bash
dotnet test
```

---

## 7. ★ Endpoint reservado para implementação manual

**`POST /api/subscriptions`** → `CreateSubscriptionCommandHandler`
(`src/MicroDemo.Application/Features/Subscriptions/Commands/CreateSubscription/`)

O **handler está deliberadamente como stub** (`throw new NotImplementedException(...)`), aguardando implementação manual com a sua skill personalizada. **Já estão prontos**: o `Command`, o `Validator` (validação de forma), o endpoint no `SubscriptionsController` e a query de leitura `GET /api/subscriptions/{id}`.

**Por que este endpoint foi escolhido:**
- É o **único caso de uso com lógica de negócio cruzada entre agregados** (`Utente` + `Price`), ao contrário do CRUD trivial dos demais.
- Exercita **validações que dependem do banco** (não cabem no FluentValidator de forma), **regras de estado** (impedir assinatura ativa sobreposta) e **mapeamento de erros** para o `Result<T>` — ou seja, alto valor didático.

**Checklist sugerido** (também documentado no cabeçalho do handler):
1. Utente existe? → senão `Result.NotFound`
2. Price existe? → senão `Result.NotFound`
3. Price está ativo? → senão `Result.Failure`/`Conflict`
4. Não há assinatura **ativa sobreposta** para o mesmo utente → senão `Result.Conflict`
5. Criar `Subscription` (Status = Active), persistir e retornar `Result<Guid>.Success(id)`
6. Logar via `_logger.LogInformation(...)`

> Observação: por estar como stub, chamar `POST /api/subscriptions` hoje lança `NotImplementedException` (o `ExceptionHandlingMiddleware` responde `500`). Isso é esperado até a implementação. O restante da solution compila e todos os testes passam.

---

## 8. Open questions / decisões tomadas por conta própria

Decisões que tomei sem confirmação — revise e ajuste conforme necessário:

1. **Banco de dados = SQL Server (LocalDB).** Escolhido por ser o mais comum no ecossistema .NET/Windows e por os scripts `CREATE TABLE` pedidos serem naturalmente T-SQL. Se preferir PostgreSQL/SQLite, troco o provider em `Infrastructure/DependencyInjection.cs` e regenero migrations/scripts.
2. **Entidades adicionais = `Subscription` e `Order`.** `Subscription` materializa o relacionamento Utente↔Price pedido; `Order` dá realismo (cobranças). Posso remover `Order` se quiser a demo mais enxuta.
3. **Envelope de resposta = `Result<T>` custom** (em vez de `ApiResponse<T>` ou biblioteca como Ardalis.Result). Simples e sem dependências. Nas respostas de sucesso, a API retorna o **payload puro** (não o envelope) e usa o status HTTP para o significado; os erros retornam `{ error, errors }`.
4. **Sem AutoMapper.** Mapeamento manual via extensões `ToDto()` para reduzir dependências. Posso adicionar AutoMapper se for padrão do time.
5. **Migrations aplicadas automaticamente em Development** (`db.Database.Migrate()` no startup). Conveniente para demo; em produção o correto é aplicar via pipeline/CI. Fácil de remover.
6. **`Currency` como texto livre (`nchar(3)`, ISO-4217)** com validação de tamanho, não uma tabela/enum de moedas. Suficiente para a demo.
7. **Regras de unicidade:** e-mail de `Utente` é único (índice + verificação no handler). Não há verificação de nome único em `Price`.
8. **Exclusão de `Price`** é bloqueada (`409`) se houver assinaturas associadas (evita órfãos), em vez de cascade ou soft-delete.
9. **Testes de handler usam EF Core InMemory.** Rápido e sem infra, mas **não valida constraints reais** (ex.: índice único de e-mail — por isso o handler verifica em código). Para fidelidade total, poderíamos usar SQLite in-memory ou Testcontainers.
10. **Nomenclatura:** solution/namespaces em `MicroDemo`; usei o termo do domínio `Utenti`/`Utente` (italiano) como você indicou. Ajusto para `Users`/`Customers` se preferir.
11. **`ExceptionHandlingMiddleware`** retorna JSON simples (`{ error, errors }`). Não usei `ProblemDetails` (RFC 7807) para manter consistência com o envelope `Result`; posso migrar para `ProblemDetails` se preferir o padrão.
12. **Versões de pacotes** fixadas em 8.0.7 (EF), MediatR 12.4.1, FluentValidation 11.9.2, Swashbuckle 6.4.0. MediatR 12.x ainda é livre; note que versões mais novas mudaram de licença.

---

## 9. Status atual

- ✅ Build da solution: **sucesso** (0 warnings, 0 errors)
- ✅ Testes: **24/24 passando**
- ✅ Migration `InitialCreate` gerada
- ⏳ `POST /api/subscriptions` (handler): **stub aguardando implementação manual**
