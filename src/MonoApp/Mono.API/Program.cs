using Microsoft.OpenApi.Models;
using Mono.API;
using Mono.API.Services;

var builder = WebApplication.CreateBuilder(args); 
        
// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));
        
builder.Services.AddSingleton<IUsersService, UsersService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Keeper - Mono HTTP API",
        Version = "v1"
    });
});

var app = builder.Build();
        
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI().UseSwaggerUI(option =>
{
    option.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
});
// }
        
app.UseAuthorization();

app.MapControllers();

app.Run();