using HelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Api.Middleware;
using HelpDesk.Application.Abstractions;
using HelpDesk.Application.Security;
using HelpDesk.Application.Users.Register;
using HelpDesk.Infrastructure.Persistence.Repositories;
using HelpDesk.Infrastructure.Security;
using HelpDesk.Infrastructure.Time;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IPasswordPolicy, PasswordPolicy>();
builder.Services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();

builder.Services.AddScoped<RegisterUserUseCase>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/error-test", () =>
{
    throw new InvalidOperationException("Test exception");
});

app.Run();