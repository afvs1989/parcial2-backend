# Backend — CineHub API (.NET 10)

API REST en **.NET 10** para un **Sistema de Gestión de Películas**: administra películas, actores y la
relación (reparto) entre ambos. Conserva la autenticación por cookies y el CRUD de usuarios.

Sigue una **arquitectura hexagonal** (puertos y adaptadores):

- **Domain**: entidades del negocio (`Pelicula`, `Actor`, `Reparto`, `Usuario`), sin dependencias externas.
- **Application**: puertos (interfaces `I*Repository`, `I*Service`) y DTOs. Define el contrato del dominio.
- **Infrastructure**: adaptadores (EF Core, repositorios, servicios, seeders) que implementan los puertos.
- **Api**: adaptador de entrada (controllers HTTP, middleware, `Program.cs`).

## Estructura

```
backend/
├── src/
│   ├── CineApi.Api/            # Controllers, Program.cs, middleware (adaptador de entrada)
│   ├── CineApi.Application/    # DTOs e interfaces (puertos)
│   ├── CineApi.Domain/         # Entidades del negocio
│   └── CineApi.Infrastructure/ # EF Core, repositorios, servicios (adaptadores de salida)
├── tests/
│   └── CineApi.Tests/          # Pruebas unitarias
├── CineApi.slnx
└── CineApi.http                # Colección de requests HTTP
```

## Modelo de datos

- **Peliculas** (`pelicula_id`, `titulo`, `genero`, `anio`, `director`)
- **Actores** (`actor_id`, `nombre`, `apellido`, `fecha_nacimiento`, `nacionalidad`)
- **Reparto** (relación muchos a muchos entre películas y actores, con el `personaje` interpretado)

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server en `localhost:1433` con base de datos `cine`

## Configuración

Archivo: `src/CineApi.Api/appsettings.json`

| Clave | Descripción |
|---|---|
| `ConnectionStrings:DefaultConnection` | Cadena de conexión a SQL Server |
| `Cors:AllowedOrigin` | Origen permitido del cliente (`http://localhost:4200`) |
| `CookieSettings:Secure` | `false` en desarrollo HTTP; `true` en producción con HTTPS |

## Ejecutar

```bash
cd backend
dotnet restore
dotnet run --project src/CineApi.Api
```

La API queda en `http://localhost:5121`.

Las migraciones y seeders (usuarios, actores, películas y reparto de ejemplo) se aplican automáticamente al iniciar.

## Credenciales de prueba

| Usuario | Contraseña | Rol |
|---|---|---|
| `admin` | `Admin123!` | Administrador |
| `maria.garcia` | `Maria2024!` | Usuario |
| `carlos.lopez` | `Carlos2024!` | Usuario |
| `ana.rodriguez` | `Ana2024!` | Administrador |
| `juan.perez` | `Juan2024!` | Usuario |

## Endpoints principales

| Método | Ruta | Descripción | Auth |
|---|---|---|---|
| POST | `/api/auth/login` | Iniciar sesión | No |
| POST | `/api/auth/logout` | Cerrar sesión | Cookie |
| GET | `/api/auth/status` | Estado de sesión | No |
| GET | `/api/usuarios` | Listar usuarios | Cookie |
| POST/PUT/DELETE | `/api/usuarios/{id}` | Gestionar usuarios | Admin |
| GET | `/api/peliculas` | Listar películas (con reparto) | Cookie |
| POST/PUT/DELETE | `/api/peliculas/{id}` | Gestionar películas | Admin |
| POST | `/api/peliculas/{id}/reparto` | Asignar un actor a la película | Admin |
| DELETE | `/api/peliculas/{id}/reparto/{actorId}` | Quitar un actor del reparto | Admin |
| GET | `/api/actores` | Listar actores (con sus películas) | Cookie |
| POST/PUT/DELETE | `/api/actores/{id}` | Gestionar actores | Admin |

## Seguridad

- Cookie de sesión `LoginSession` (HttpOnly, SameSite=Strict, Secure configurable)
- Protección CSRF con token en header `X-XSRF-TOKEN`
- Contraseñas hasheadas con BCrypt

También puede probar la API con `CineApi.http` (REST Client en VS Code).
