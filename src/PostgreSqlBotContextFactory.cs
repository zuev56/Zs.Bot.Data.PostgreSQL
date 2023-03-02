using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;

namespace Zs.Bot.Data.PostgreSQL;

public sealed class PostgreSqlBotContextFactory : IDbContextFactory<PostgreSqlBotContext>, IDesignTimeDbContextFactory<PostgreSqlBotContext>
{
    private readonly DbContextOptions<PostgreSqlBotContext> _options = null!;

    public PostgreSqlBotContextFactory()
    {
    }

    public PostgreSqlBotContextFactory(DbContextOptions<PostgreSqlBotContext> options)
    {
        _options = options;
    }

    public PostgreSqlBotContext CreateDbContext() => new(_options);

    // For migrations
    public PostgreSqlBotContext CreateDbContext(string[] args)
    {
        // TODO: exclude hardcoded config file name
        Trace.WriteLineIf(args.Length > 0, string.Join(',', args));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = configuration.GetConnectionString("Default");

        var optionsBuilder = new DbContextOptionsBuilder<PostgreSqlBotContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new PostgreSqlBotContext(optionsBuilder.Options);
    }
}