using System.Reflection;
using CampingCore.Application;
using CampingCore.Infrastructure;
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

            **Autenticación:** la mayoría de operaciones requieren un token JWT. Obténlo con `POST /api/auth/login` o regístrate con `POST /api/auth/register` y luego inicia sesión. En esta interfaz, pulsa *Authorize* e introduce: `Bearer {tu_token}`.

            **Convención de rutas:** prefijo `api/…`. Los recursos vinculados al usuario usan el identificador del token (claim `sub`).

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
        Description  = "JWT Bearer. Formato: `Bearer {token}` (obtenido de POST /api/auth/login).",
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

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
