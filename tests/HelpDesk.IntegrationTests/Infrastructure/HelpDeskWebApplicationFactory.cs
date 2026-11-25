using HelpDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace HelpDesk.IntegrationTests.Infrastructure;

public sealed class HelpDeskWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("helpdesk_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    
    private NpgsqlConnection _connection = null!;
    private Respawner _respawner = null!;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        await dbContext.Database.MigrateAsync();

        _connection = new NpgsqlConnection(
            _postgresContainer.GetConnectionString());

        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            _connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,

                SchemasToInclude = ["public"],

                TablesToIgnore =
                [
                    new Respawn.Graph.Table("__EFMigrationsHistory")
                ]
            });
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        await _postgresContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options =>
                options
                    .UseNpgsql(_postgresContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention());
        });
    }

    public Task ResetDatabaseAsync()
    {
        return _respawner.ResetAsync(_connection);
    }
}