using Microsoft.EntityFrameworkCore;

namespace RecruitmentPortal;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDataAccess(connectionString);
        builder.Services.AddIdentity();
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        app.UseApplicationMiddleware();

        await app.RunAsync();
    }
}