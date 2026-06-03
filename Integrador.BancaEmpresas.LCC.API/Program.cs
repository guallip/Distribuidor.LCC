using Integrador.BancaEmpresas.LCC.Aplicacion.Extensiones;
using Integrador.BancaEmpresas.LCC.Infraestructura.Extensiones;
using Produnet.BFF.Middleware.Extensiones;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#if DEBUG
System.Diagnostics.Debugger.Break();
#endif

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddHealthChecks();
builder.Services.AgregarServiciosAplicacion();
builder.Services.AgregarServiciosInfraestructura(builder.Configuration);
builder.Services.AgregarServicioIdCorrelacion();

// Visor de eventos
#pragma warning disable CA1416
builder.Logging.AddEventLog(configure =>
{
    configure.LogName = builder.Configuration["Logging:EventLog:LogName"];
    configure.SourceName = builder.Configuration["Logging:EventLog:SourceName"];
});
#pragma warning restore CA1416

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/HealthCheck");
app.UsarIdCorrelacionMiddleware();
app.UsarLogMiddleware(opciones =>
{
    opciones.LogRuta = builder.Configuration["LogRuta"];
    opciones.LogBaseDatos = Convert.ToBoolean(builder.Configuration["LogBaseDatos"]);
    opciones.LogTabla = "log_Integrador.BancaEmpresas.LCC.API";
});

app.Run();