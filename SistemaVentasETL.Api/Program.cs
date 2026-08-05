using Microsoft.EntityFrameworkCore;
using SistemaVentasETL.Data.Context;
using SistemaVentasETL.Api.Repository;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("Sistema_de_Ventas")
    ?? throw new InvalidOperationException(
        "No se encontrÃ³ la cadena de conexiÃ³n Sistema_de_Ventas.");

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

