using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
        builder.Services.Configure<ScrumPoker.WebApp.Services.Settings.ScrumPokerAppSettings>(builder.Configuration.GetSection("ScrumPoker"));

        ScrumPoker.WebApp.Services.Settings.ScrumPokerAppSettings settings = builder.Configuration.Get<ScrumPoker.WebApp.Services.Settings.ScrumPokerAppSettings>();
        string? databaseFilePath = settings.dbFile;

        if (string.IsNullOrWhiteSpace(databaseFilePath))
            databaseFilePath = Path.Combine(builder.Environment.WebRootPath, DEFAULT_DB_NAME);
        else
            databaseFilePath = Path.IsPathFullyQualified(databaseFilePath) ? Path.GetFullPath(databaseFilePath) : Path.Combine(builder.Environment.WebRootPath, databaseFilePath);
        builder.Services.AddDbContext<ScrumPoker.WebApp.Services.ScrumPokerContext>(opt =>
            opt.UseSqlite(new SqliteConnectionStringBuilder
            {
                DataSource = databaseFilePath,
                ForeignKeys = true,
                Mode = File.Exists(databaseFilePath) ? SqliteOpenMode.ReadWrite : SqliteOpenMode.ReadWriteCreate
            }.ConnectionString));

        CurrentApp = builder.Build();
        using (var scope = CurrentApp.Services.CreateScope())
        {
            IServiceProvider services = scope.ServiceProvider;
            IOptions<ScrumPoker.WebApp.Services.Settings.ScrumPokerAppSettings> appSettings = services.GetRequiredService<IOptions<ScrumPoker.WebApp.Services.Settings.ScrumPokerAppSettings>>();
            using ScrumPoker.WebApp.Services.ScrumPokerContext context = new(services.GetRequiredService<DbContextOptions<ScrumPoker.WebApp.Services.ScrumPokerContext>>());
            ScrumPoker.WebApp.Models.ColorSchema.SeedData(context, appSettings, services.GetRequiredService<ILogger<ScrumPoker.WebApp.Models.ColorSchema>>());
            ScrumPoker.WebApp.Models.CardDeck.SeedData(context, appSettings, services.GetRequiredService<ILogger<ScrumPoker.WebApp.Models.CardDeck>>());
            if (ScrumPoker.WebApp.Models.UserProfile.SeedData(context, appSettings, services.GetRequiredService<ILogger<ScrumPoker.WebApp.Models.UserProfile>>()))
                context.SaveChanges(true);
        }

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
