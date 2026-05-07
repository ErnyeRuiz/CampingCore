using System.Reflection;
using CampingCore.Application;
using CampingCore.Authorization;
using CampingCore.Infrastructure;
using CampingCore.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddSwaggerGen(options =>
{
    var assemblyName = Assembly.GetExecutingAssembly().GetName().Name!;
    var xmlFile        = $"{assemblyName}.xml";
    var xmlPath        = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "CampingCore API",
        Version     = "v1",
        Description = """
            API REST para descubrir y gestionar sitios de camping, reseñas, favoritos e itinerarios de viaje.

            **Ubicación (Costa Rica):** `GET /api/ubicacion/…` expone provincias, cantones y distritos para rellenar `IdProvincia`, `IdCanton`, `IdDistrito` al crear o editar un camping.

            **Autenticación:** muchas rutas exigen JWT. Registro: `POST /api/auth/register` asigna el rol `Customer` si existe en base de datos; luego `POST /api/auth/login`. El cuerpo de login incluye `userId`, `name`, `email`, `roleName` y `token`. Pulsa *Authorize* y usa `Bearer {token}`. Los claims incluyen `sub` (id de usuario), rol (`role`/`ClaimTypes.Role`) y uno o más claims `permission` con el nombre del permiso.

            **Autorización por permiso:** crear/editar/eliminar campings (`POST`/`PUT`/`DELETE /api/campsites`) exigen políticas `Permission:create.campsite`, `Permission:update.campsite` y `Permission:delete.campsite` (claim `permission` en el JWT). El rol `SuperUser` satisface cualquier política `Permission:…` sin necesidad de esos claims.

            **Roles y permisos (REST):** `api/roles` y `api/permissions` solo son accesibles con rol `SuperUser` en el JWT (`PUT /api/roles/{id}/permissions` incluido). Otros usuarios reciben **403**.

            **Usuarios (admin):** `GET /api/users` lista todos los usuarios con métricas; `GET` y `PUT /api/users/{id}` cargan y actualizan un usuario (nombre, email único, rol, contraseña opcional). Todo ello solo con rol `SuperUser`; otros roles reciben **403**. El perfil propio sigue en `GET`/`PUT /api/users/me` (cualquier usuario autenticado).

            **Cliente / Admin:** siguen usando claims `permission` según la tabla `RolePermissions`. Falta de token → **401**; token sin permiso/política → **403**.

            **Convención de rutas:** prefijo `api/…`. Donde aplique, el usuario se infiere del claim `sub` del token.

            **Campings:** listado público `GET /api/campsites`; gestión `GET /api/campsites/managed` exige JWT y rol `Admin` o `SuperUser` (Admin ve solo los que creó; SuperUser ve todos). `POST` y `PUT` `/api/campsites` usan `multipart/form-data` (campos del sitio + archivos `images`; en `PUT`, `imageIdsToKeep` repetido por cada id de imagen existente que se conserve).

            **Swagger en no-desarrollo:** se puede activar con la clave de configuración `EnableSwagger: true` en `appsettings` (útil en demos; no se recomienda en producción pública sin autenticación adicional en el propio endpoint de documentación).
            """,
        Contact = new OpenApiContact
        {
            Name  = "CampingCore",
            Email = "api@campingcore.local"
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url  = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Description  = "JWT Bearer (POST /api/auth/login). Respuesta incluye campo `token` junto con `userId`, `name`, `email`, `roleName`. Formato aquí: `Bearer {token}`.",
        In           = ParameterLocation.Header,
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        Reference    = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id   = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",

        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

var showSwagger = app.Environment.IsDevelopment()
    || app.Configuration.GetValue("EnableSwagger", false);

if (showSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CampingCore API v1");
        c.DocumentTitle   = "CampingCore – documentación de API";
        c.DisplayRequestDuration();
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
