using System.Reflection;
using System.Text.Json.Serialization;
using HotelApp.BL;
using HotelApp.DAL;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // El estado de la habitación se envía y se recibe como texto (Available, Occupied...)
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HotelApp API",
        Version = "v1",
        Description = "API REST para la gestión de hoteles, bloques, pisos, habitaciones y tipos de habitación."
    });

    var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xml)) c.IncludeXmlComments(xml);
});

builder.Services.AddDalDependencies(
    builder.Configuration.GetConnectionString("HotelDbContext")!);
builder.Services.AddBlDependencies();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelApp API v1"));

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
