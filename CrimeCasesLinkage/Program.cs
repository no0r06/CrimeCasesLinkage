using CrimeCasesLinkage.Data;
using CrimeCasesLinkage.Models;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddDbContext<CrimeDbContext>(options =>
    options.UseSqlServer(
        "Server=(localdb)\\MSSQLLocalDB;Database=CrimeCasesLinkDB;Trusted_Connection=True;TrustServerCertificate=True;"
    ));

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/scalar/v1"))
   .ExcludeFromDescription();

app.Run();