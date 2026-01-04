using Microsoft.AspNetCore.Identity;
using RealTimeTradingApp.API.Extensions;
using RealTimeTradingApp.API.Hubs;
using RealTimeTradingApp.Application;
using RealTimeTradingApp.Domain.Entities;
using RealTimeTradingApp.Infrastructure;
using RealTimeTradingApp.Infrastructure.Seed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/api-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApplcationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// Seed Identity data (Roles + Admin)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();

    await RoleSeeder.SeedAsync(roleManager);
    await AdminSeeder.SeedAsync(userManager, roleManager);
}

    // Use global exception middleware as first middleware after build
app.UseGlobalExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<StockPriceHub>("/stockhub");
//app.MapGet("/", () => "Trading API is running");

app.Run();