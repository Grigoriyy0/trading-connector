using Connector.Infrastructure;
using Microsoft.AspNetCore.WebSockets;

namespace Connector;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddHttpClient();
        builder.Services.AddInfrastructureServices();
        builder.Services.AddSwaggerGen();
        
        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseWebSockets();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}