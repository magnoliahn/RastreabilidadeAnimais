using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using rastreabilidadeAnimais.Data;
using rastreabilidadeAnimais.DTOs;
using rastreabilidadeAnimais.Repositories;
using rastreabilidadeAnimais.Repositories.Interfaces;
using rastreabilidadeAnimais.Services;
using rastreabilidadeAnimais.Services.Interfaces;
using rastreabilidadeAnimais.Validators;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IUnidadeExploracaoRepository, UnidadeExploracaoRepository>();
builder.Services.AddScoped<ISaidaAnimaisRepository, SaidaAnimaisRepository>();

// Register services
builder.Services.AddScoped<IUnidadeExploracaoService, UnidadeExploracaoService>();
builder.Services.AddScoped<ISaidaAnimaisService, SaidaAnimaisService>();

// Register validators
builder.Services.AddScoped<CreateUnidadeExploracaoValidator>();
builder.Services.AddScoped<UpdateUnidadeExploracaoValidator>();
builder.Services.AddScoped<CreateSaidaAnimaisValidator>();
builder.Services.AddScoped<UpdateSaidaAnimaisValidator>();
builder.Services.AddScoped<IValidator<CreateSaidaAnimaisDTO>, CreateSaidaAnimaisValidator>();
builder.Services.AddScoped<IValidator<UpdateSaidaAnimaisDTO>, UpdateSaidaAnimaisValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Initialize database with seed data
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();