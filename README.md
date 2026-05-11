# CampingCore

API REST en **.NET 8** para descubrir y gestionar sitios de camping en Costa Rica. Permite registrar campings, escribir reseñas, guardar favoritos, crear itinerarios y consultar ubicaciones por provincia, cantón y distrito.

## Stack

- **Runtime:** .NET 8 / ASP.NET Core
- **Arquitectura:** Clean Architecture (Domain → Application → Infrastructure → API)
- **Base de datos:** SQL Server + EF Core (Code First, Migrations)
- **Autenticación:** JWT (HS256) con autorización por roles y permisos
- **Email:** Brevo (correo transaccional)
- **Documentación:** Swagger / OpenAPI

## Estructura del proyecto

```
CampingCore/               # Host: controladores, middleware, CORS, Swagger
CampingCore.Application/   # Casos de uso, MediatR, validación (FluentValidation)
CampingCore.Domain/        # Entidades y contratos de repositorio
CampingCore.Infrastructure/ # EF Core, JWT, email, cliente de API geográfica
```

## Ejecución local

**Requisitos:** .NET 8 SDK y SQL Server.

```bash
dotnet restore
dotnet build
dotnet run --project CampingCore/CampingCore.csproj
```

Configura las variables de entorno mínimas antes de arrancar:

```bash
ConnectionStrings__DefaultConnection="Server=...;Database=CampingCore;..."
JwtSettings__SecretKey="clave-de-al-menos-32-bytes"
Frontend__BaseUrl="http://localhost:4200"
```

Aplica las migraciones:

```bash
dotnet ef database update --project CampingCore.Infrastructure --startup-project CampingCore
```

Con Swagger activo (automático en desarrollo), abre `/swagger`.

## Despliegue

El repositorio incluye un flujo de GitHub Actions (`.github/workflows/deploy.yml`) que compila, publica y despliega vía FTP.

## Licencia

MIT