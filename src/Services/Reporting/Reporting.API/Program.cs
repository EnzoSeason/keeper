using System.Reflection;
using Microsoft.OpenApi.Models;
using Reporting.API.Utils;
using Reporting.Domain.ReportAggregate;
using Reporting.Domain.StatementAggregate;
using Reporting.Domain.TransactionAggregate;
using Reporting.Infrastructure.Repositories;
using Reporting.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Keeper - Reporting HTTP API",
        Version = "v1"
    });
    
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

// MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// Custom Services
builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();
builder.Services.AddSingleton<IStatementRepository, StatementRepository>();
builder.Services.AddSingleton<IReportRepository, ReportRepository>();
builder.Services.AddSingleton<IClock, Clock>();

// Custom Configurations
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI().UseSwaggerUI(option =>
    {
        option.SwaggerEndpoint("/swagger/v1/swagger.json", "Keeper - Reporting HTTP API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }