using MonoAPI.Models;
using MonoAPI.Services;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args); 
        
        // Add services to the container.
        builder.Services.AddControllers();

        builder.Services.Configure<MongoDbSettings>(
            builder.Configuration.GetSection("Mongodb"));
        
        builder.Services.AddSingleton<UsersService>();

       // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        
        // if (app.Environment.IsDevelopment())
        // {
        app.UseSwagger();
        app.UseSwaggerUI();
        // }
        
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

