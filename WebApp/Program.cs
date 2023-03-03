using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ScrumPoker.WebApp.Services;

internal class Program
{
    private const string DEFAULT_DB_NAME = "ScrumPoker.db";

#pragma warning disable CS8618
    internal static WebApplication CurrentApp { get; private set; }
    
#pragma warning restore CS8618

    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllersWithViews();
        builder.Services.Configure<ScrumPokerAppSettings>(builder.Configuration.GetSection("ScrumPoker"));

        // builder.Services.AddSingleton<SessionTokenService>();

        CurrentApp = builder.Build();

        ScrumPokerAppSettings settings = CurrentApp.Configuration.Get<ScrumPokerAppSettings>();
        string? databaseFilePath = settings.DbFile;

        if (string.IsNullOrWhiteSpace(databaseFilePath))
            databaseFilePath = Path.Combine(builder.Environment.WebRootPath, DEFAULT_DB_NAME);
        else
            databaseFilePath = Path.IsPathFullyQualified(databaseFilePath) ? Path.GetFullPath(databaseFilePath) : Path.Combine(builder.Environment.WebRootPath, databaseFilePath);
        CurrentApp.Logger.LogInformation("Using database {databaseFilePath}", databaseFilePath);
        builder.Services.AddDbContext<ScrumPokerContext>(opt =>
            opt.UseSqlite(new SqliteConnectionStringBuilder
            {
                DataSource = databaseFilePath,
                ForeignKeys = true,
                Mode = File.Exists(databaseFilePath) ? SqliteOpenMode.ReadWrite : SqliteOpenMode.ReadWriteCreate
            }.ConnectionString));

        // Configure the HTTP request pipeline.
        if (!CurrentApp.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            CurrentApp.UseHsts();
        }

        CurrentApp.UseHttpsRedirection();
        CurrentApp.UseStaticFiles();
        CurrentApp.UseRouting();
        CurrentApp.MapControllerRoute(
            name: "default",
            // 
            pattern: "{controller}/{action=Index}/{id?}");

        CurrentApp.MapFallbackToFile("index.html");
        CurrentApp.Run();
    }
}