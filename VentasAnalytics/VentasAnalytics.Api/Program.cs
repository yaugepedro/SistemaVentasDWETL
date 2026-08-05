using Microsoft.EntityFrameworkCore;
using VentasAnalytics.Data.Context;
using VentasAnalytics.Api.Repository;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("VentasDB")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión VentasDB.");

builder.Services.AddDbContext<VentasDBContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();