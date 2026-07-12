# MicroDemo — E-Commerce Microservice Demo (.NET 8 · Clean Architecture · CQRS)

Solution de demonstração de um microserviço em **.NET 8** seguindo **Clean Architecture** com **DDD leve**, **CQRS (MediatR)**, **EF Core (Code First + Migrations)**, **FluentValidation** e **Swagger**.

Domínio: **E-COMMERCE** — gestão de **Users** (clientes), **Categories**, **Products**, **Prices** (histórico de preços por produto) e **Orders** com os seus **OrderItems** (linhas com snapshot do preço no momento da compra).

> Esta solution foi **migrada** de um domínio anterior de *Subscriptions* para *E-Commerce*, mantendo intactas a arquitetura, os padrões (Clean Architecture, CQRS/MediatR, EF Core, `Result<T>`, FluentValidation, Swagger) e a organização de testes por tipo de artefato.

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
    └── seed.sql                   → INSERTs com dados de exemplo realistas de e-commerce.
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
| `User` | Cliente/comprador. Tem um histórico de pedidos. |
| `Category` | Categoria de produtos (ex.: Eletrónica, Livros). |
| `Product` | Produto vendável. Pertence a uma `Category` e tem histórico de preços. |
| `Price` | Preço de um `Product` (histórico 1:N). O corrente = ativo com `ValidFromUtc` mais recente. |
| `Order` | Pedido de um `User`. Agrega `OrderItem`s e mantém o total calculado. |
| `OrderItem` | Linha do pedido: liga `Product` + `Order`, com quantidade e **snapshot** do preço unitário. |

### Relacionamentos
- `User` **1:N** `Order`
- `Order` **1:N** `OrderItem`
- `OrderItem` **N:1** `Product` (com snapshot do preço no momento da compra)
- `Product` **N:1** `Category`
- `Product` **1:N** `Price` (histórico de preços)

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

### Users — CRUD completo ✅
| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/api/users?search=&onlyActive=` | Lista utilizadores (filtro opcional) |
| GET    | `/api/users/{id}` | Obtém por id |
| POST   | `/api/users` | Cria (valida e-mail único) |
| PUT    | `/api/users/{id}` | Atualiza |
| DELETE | `/api/users/{id}` | Remove (bloqueia se houver pedidos → `409`) |

### Categories — CRUD completo ✅
| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/api/categories?onlyActive=` | Lista categorias |
| GET    | `/api/categories/{id}` | Obtém por id |
| POST   | `/api/categories` | Cria (valida nome único) |
| PUT    | `/api/categories/{id}` | Atualiza |
| DELETE | `/api/categories/{id}` | Remove (bloqueia se houver produtos → `409`) |

### Products — CRUD completo ✅
| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/api/products?search=&categoryId=&onlyActive=` | Lista produtos (inclui preço corrente) |
| GET    | `/api/products/{id}` | Obtém por id (inclui preço corrente) |
| POST   | `/api/products` | Cria (valida existência da categoria e SKU único) |
| PUT    | `/api/products/{id}` | Atualiza |
| DELETE | `/api/products/{id}` | Remove (bloqueia se presente em pedidos → `409`) |

### Prices — CRUD completo ✅
| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/api/prices?productId=&onlyActive=` | Lista preços (histórico) |
| GET    | `/api/prices/{id}` | Obtém por id |
| POST   | `/api/prices` | Cria (valida existência do produto) |
| PUT    | `/api/prices/{id}` | Atualiza |
| DELETE | `/api/prices/{id}` | Remove |

### Orders
| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/api/orders/{id}` | Obtém por id, incluindo as linhas ✅ |
| POST   | `/api/orders` | **★ RESERVADO PARA IMPLEMENTAÇÃO MANUAL — ver §7** |

---

## 6. ⚠️ Organização CUSTOMIZADA dos testes (particularidade desta solution)

Os testes **não** são organizados por classe/feature (o convencional seria `CreateProductCommandHandlerTests.cs`, `CreateProductCommandValidatorTests.cs`...). Em vez disso, são organizados **por TIPO DE ARTEFATO**, um arquivo por tipo:

| Arquivo | Contém |
|---------|--------|
| `RequestTests.cs`   | **Builders de request** (`UserRequests`, `CategoryRequests`, `ProductRequests`, `PriceRequests`, `OrderRequests`) — a montagem dos commands/queries usados nos testes, com variações válidas/inválidas. |
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

**`POST /api/orders`** → `CreateOrderCommandHandler`
(`src/MicroDemo.Application/Features/Orders/Commands/CreateOrder/`)

O **handler está deliberadamente como stub** (`throw new NotImplementedException(...)`), aguardando implementação manual com a sua skill personalizada. **Já estão prontos**: o `Command` (com múltiplos `CreateOrderItem`), o `Validator` (validação de forma), o endpoint no `OrdersController` e a query de leitura `GET /api/orders/{id}`.

**Por que este endpoint foi escolhido:**
- É o **caso de uso mais rico em regras de negócio** e o único que cruza vários agregados (`User` + `Product` + `Price` + `Order` + `OrderItem`), ao contrário do CRUD trivial dos demais.
- Exercita **validações que dependem do banco** (não cabem no FluentValidator de forma), **resolução do preço corrente + snapshot** no `OrderItem`, **cálculo de totais** e **mapeamento de erros** para o `Result<T>` — ou seja, alto valor didático.

**Checklist sugerido** (também documentado no cabeçalho do handler):
1. User existe? → senão `Result.NotFound`
2. Para cada item: Product existe e está ativo? → senão `Result.NotFound`/`Conflict`
3. Resolver o **preço corrente** do produto (Price ativo com `ValidFromUtc` mais recente) → senão `Result.Conflict`
4. Criar cada `OrderItem` com `UnitPrice` = snapshot do preço, `LineTotal` = `UnitPrice × Quantity`
5. Calcular `Order.TotalAmount` = soma dos `LineTotal`, `Status = Pending`, persistir e retornar `Result<Guid>.Success(id)`
6. Logar via `_logger.LogInformation(...)`

> Observação: por estar como stub, chamar `POST /api/orders` hoje lança `NotImplementedException` (o `ExceptionHandlingMiddleware` responde `500`). Isso é esperado até a implementação. O restante da solution compila e todos os testes passam.

---

## 8. Open questions / decisões tomadas por conta própria

Decisões que tomei sem confirmação durante a **migração para e-commerce** — revise e ajuste conforme necessário:

1. **Endpoint reservado = `POST /api/orders` (criação de Order).** Conforme sugerido, por ser o caso de uso mais rico em regras de negócio. O CRUD de `Order` (update/delete/list) **não** foi criado — só `GET /api/orders/{id}` e o `POST` reservado; posso adicionar se precisar.
2. **`Price` foi religado a `Product`** (era ligado a `Utente` via `Subscription`). Passou a ser um **histórico** (1:N): removi `Name`/`Description`/`BillingPeriod` e adicionei `ProductId` e `ValidFromUtc`. O "preço corrente" é o ativo mais recente (não há período de validade com data-fim). Se preferir `ValidToUtc` explícito, ajusto.
3. **`OrderItem` guarda snapshot** de `UnitPrice` + `Currency` + `LineTotal` (não FK para `Price`), para que alterações de preço não afetem pedidos passados. Por isso, apagar um `Price` é seguro e **não** é bloqueado.
4. **Entidades `Subscription`/`Utente` e enums `BillingPeriod`/`SubscriptionStatus` foram removidos.** `Utente` foi substituído por `User` (nomenclatura em inglês, alinhada ao novo domínio). Ajusto para outro idioma se preferir.
5. **`OrderStatus` redefinido** para e-commerce: `Pending, Paid, Shipped, Delivered, Cancelled` (antes: `Pending, Paid, Failed, Refunded`).
6. **Regras de unicidade:** `User.Email`, `Category.Name` e `Product.Sku` são únicos (índice + verificação no handler). O `SKU` é obrigatório e gerado pelo cliente (não auto-gerado).
7. **Deleção bloqueada (`409`) para preservar integridade:** `User` com pedidos, `Category` com produtos e `Product` presente em pedidos não podem ser removidos (em vez de cascade/soft-delete). FKs no banco: `Category→Product` e `Product→OrderItem` usam `Restrict`; `User→Order`, `Order→OrderItem` e `Product→Price` usam `Cascade`.
8. **Banco de dados = SQL Server (LocalDB).** Mantido do projeto original. Se preferir PostgreSQL/SQLite, troco o provider em `Infrastructure/DependencyInjection.cs` e regenero migrations/scripts.
9. **Envelope `Result<T>` custom, mapeamento manual `ToDto()`, migrations automáticas em Development, `Currency` como `nchar(3)` ISO-4217, JSON simples no middleware (sem `ProblemDetails`)** — todas as decisões do projeto original foram **mantidas** sem alteração.
10. **`ProductDto` inclui `CurrentPrice`/`Currency`** derivados do histórico de preços (comodidade de leitura). Os handlers de leitura de produto fazem `Include(p => p.Prices)` para isso.
11. **Testes de handler usam EF Core InMemory.** Rápido e sem infra, mas **não valida constraints reais** (ex.: índices únicos — por isso os handlers verificam em código). Para fidelidade total, poderíamos usar SQLite in-memory ou Testcontainers.
12. **Versões de pacotes** mantidas: EF 8.0.7, MediatR 12.4.1, FluentValidation 11.9.2, Swashbuckle 6.4.0.

---

## 9. Status atual

- ✅ Build da solution: **sucesso** (0 warnings, 0 errors)
- ✅ Testes: **41/41 passando**
- ✅ Migration `InitialCreate` (e-commerce) gerada
- ✅ `schema.sql` / `seed.sql` atualizados com dados de exemplo de e-commerce
- ⏳ `POST /api/orders` (handler): **stub aguardando implementação manual**
