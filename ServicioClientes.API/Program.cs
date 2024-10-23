using Microsoft.EntityFrameworkCore;
using ServicioClientes.Infraestructura.Contextos;
using ServicioClientes.Infraestructura.Repositorios;
using ServicioClientes.Aplicacion.Servicios.Interfaces;
using ServicioClientes.Aplicacion.Servicios.Imp;

var builder = WebApplication.CreateBuilder(args);

var baseDeDatosEnMemoria = builder.Configuration.GetValue<bool>("BaseDeDatosEnMemoria");

if (baseDeDatosEnMemoria)
{
    builder.Services.AddDbContext<ServicioClientesContexto>(options =>
        options.UseInMemoryDatabase("ServicioClientesDB"));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ServicioClientesContexto>(options =>
        options.UseSqlServer(connectionString));
}

// Registrar AutoMapper
builder.Services.AddAutoMapper(typeof(Program));  // Aquí se registra AutoMapper

// Inyección de los servicios para Cliente y Persona
builder.Services.AddScoped<IClienteServicio, ClienteServicio>();
builder.Services.AddScoped<IPersonaServicio, PersonaServicio>();
builder.Services.AddScoped(typeof(IRepositorioGenerico<>), typeof(RepositorioGenerico<>));

// Configuración de CORS
var corsOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirServiciosLocales", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Agregar controladores
builder.Services.AddControllers();

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IClienteServicio, ClienteServicio>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServicioCuentasAPI:BaseUrl"]);
});

builder.Services.AddHttpClient();

var app = builder.Build();

// Configuración del pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServicioClientes.API v1");
    });
}

// Habilitar CORS con la política definida
app.UseCors("PermitirServiciosLocales");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program  // Exponemos esta clase para pruebas
{
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Program>();
            });
}
