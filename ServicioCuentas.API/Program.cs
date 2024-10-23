using Microsoft.EntityFrameworkCore;
using ServicioClientes.Infraestructura.Contextos;
using ServicioClientes.Infraestructura.Repositorios;
using ServicioCuentas.Aplicacion.Servicios.Imp;
using ServicioCuentas.Aplicacion.Servicios.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

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

// Registrar HttpClient para interactuar con ServicioUsuariosAPI
builder.Services.AddHttpClient<ICuentaServicio, CuentaServicio>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServicioUsuariosAPI:BaseUrl"]);
});

// Inyección de los servicios para Cuenta y Movimiento
builder.Services.AddScoped<IMovimientoServicio, MovimientoServicio>();
builder.Services.AddScoped<ICuentaServicio, CuentaServicio>();
builder.Services.AddScoped<IReporteServicio, ReporteServicio>();


builder.Services.AddScoped(typeof(IRepositorioGenerico<>), typeof(RepositorioGenerico<>));

// Configuración de CORS basada en appsettings.json
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

builder.Services.AddHttpClient();

var app = builder.Build();

// Configuración del pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServicioCuentas.API v1");
    });
}

// Habilitar CORS con la política definida
app.UseCors("PermitirServiciosLocales");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
