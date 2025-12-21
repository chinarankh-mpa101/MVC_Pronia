using Microsoft.EntityFrameworkCore;
using Pronia_example.Contexts;

namespace Pronia_example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });
            var app = builder.Build();
            app.UseStaticFiles();
            app.MapDefaultControllerRoute();
            //app.MapGet("/", () => "Hello World!");
            app.Run();
        }
    }
}
