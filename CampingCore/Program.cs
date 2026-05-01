using System.Reflection;
using CampingCore.Application;
using CampingCore.Infrastructure;
using CampingCore.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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

            **Roles y permisos (REST):** `api/roles` y `api/permissions` exponen CRUD de catálogo; `PUT /api/roles/{id}/permissions` reemplaza por completo la lista de IDs de permisos del rol. Requieren JWT válido.

            **Convención de rutas:** prefijo `api/…`. Donde aplique, el usuario se infiere del claim `sub` del token.

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
