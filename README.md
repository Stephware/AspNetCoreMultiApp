# AspNetCoreMultiApp (.NET 10)

A three-project solution adapted from [Mark-Batongbacal/AspNetCoreMultiApp](https://github.com/Mark-Batongbacal/AspNetCoreMultiApp) for **.NET 10**.

## Projects
- **AspNetCoreMultiApp.Api** — ASP.NET Core Web API, EF Core / SQL Server, Products endpoints, and Hangfire dashboard.
- **AspNetCoreMultiApp.Web** — ASP.NET Core MVC application.
- **AspNetCoreMultiApp.Worker** — Hangfire background worker; retrieves products from the API and writes JSON reports.

## Requirements
- .NET 10 SDK
- Running Microsoft SQL Server instance (local or remote)
- Access to NuGet during first restore
- `dotnet-ef` 10.x tool for the initial database migration

## Configure SQL Server
Do not commit database passwords. Set the same connection string in the shell used to run the API, EF migrations, and Worker:

```bash
export ConnectionStrings__DefaultConnection='Server=localhost,1433;Database=AspNetCoreMultiAppDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;Encrypt=True'
```

Use an appropriate SQL Server connection string for your environment. On macOS, SQL Server normally runs in a container or on another server.

## Prepare the database
From the repository root:

```bash
dotnet restore AspNetCoreMultiApp.sln
dotnet tool install --global dotnet-ef --version 10.0.12
dotnet ef migrations add InitialCreate --project AspNetCoreMultiApp.Api --startup-project AspNetCoreMultiApp.Api
dotnet ef database update --project AspNetCoreMultiApp.Api --startup-project AspNetCoreMultiApp.Api
dotnet build AspNetCoreMultiApp.sln
```

Only run `migrations add InitialCreate` once; commit its generated migration files. For later schema changes, create a new migration instead.

## Run the solution (three terminals)
Ensure the same SQL Server connection-string environment variable is available in the API and Worker terminals.

**Terminal 1 — API**
```bash
dotnet run --project AspNetCoreMultiApp.Api --launch-profile http
```
API: http://localhost:5173/api/products

**Terminal 2 — MVC**
```bash
dotnet run --project AspNetCoreMultiApp.Web --launch-profile http
```
MVC: http://localhost:5239

**Terminal 3 — Worker**
```bash
export ApiSettings__BaseUrl='http://localhost:5173/'
dotnet run --project AspNetCoreMultiApp.Worker
```
Worker reports: `AspNetCoreMultiApp.Worker/Reports/` (when launched from that project directory), or the working directory's `Reports/` folder otherwise. The Worker schedules a product JSON report every minute.

### API examples
```bash
curl http://localhost:5173/api/products
curl -X POST http://localhost:5173/api/products \
  -H 'Content-Type: application/json' \
  -d '{"name":"Sample Product","price":99.50}'
```

Development OpenAPI document: http://localhost:5173/openapi/v1.json (run in Development environment).

Hangfire dashboard: http://localhost:5173/hangfire — by default, access is restricted to local requests. Configure authentication before publishing the dashboard.

## Notes
- All three projects target `net10.0`.
- EF Core, ASP.NET Core OpenAPI, and Hosting dependencies use the 10.0.12 stable patch.
- Hangfire 1.8.25 is retained from the reference implementation.
- MVC remains a separate project; it is not automatically wired to fetch products from the API.
- Keep `ConnectionStrings__DefaultConnection` and other secrets outside Git.
- The reference repo did not include an explicit LICENSE file; verify reuse/distribution permission with its author before redistributing copied code.
