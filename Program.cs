using BlazorApp.Components;
using BlazorApp.Data;
using BlazorApp.Providers;
using BlazorApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

      // Add support for controllers
      builder.Services.AddControllers();

      // Add support for sessions
      builder.Services.AddDistributedMemoryCache();
      builder.Services.AddSession(options =>
      {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
      });

      // Add support for authentication and authorization
      builder.Services.AddAuthorization();
      builder.Services.AddCascadingAuthenticationState();
      builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

      // Config EF Core to use SQLite
      builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=app.db"));

      // Config FIDO2 Service
      var fido2Config = builder.Configuration.GetSection("Fido2");
      var fido2ServerDomain = fido2Config["ServerDomain"]!;
      var fido2Origins = fido2Config["Origins"]?
        .Split(';', StringSplitOptions.RemoveEmptyEntries)!;
      builder.Services.AddSingleton(sp => new Fido2Service(fido2ServerDomain, fido2Origins));

      // Add service for calling the WebAuthn API from the client
      builder.Services.AddWebAuthn();

      // Add communication with the backend for registration and login
      builder.Services.AddAuthn(new Uri(fido2Origins[0]));

      builder.Logging.ClearProviders();
      builder.Logging.AddConsole();

      // Enable detailed error reporting
      builder.Services.AddServerSideBlazor()
        .AddCircuitOptions(options => options.DetailedErrors = true);

      var app = builder.Build();

      // Configure the HTTP request pipeline
      if (!app.Environment.IsDevelopment())
      {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
      }

      app.UseSession();

      app.UseHttpsRedirection();
      app.MapControllers();

      app.MapStaticAssets();
      app.UseAntiforgery();

      app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

      // Log all requests
      app.Use(async (context, next) =>
      {
        Console.WriteLine($"Request Path: {context.Request.Path}, Method: {context.Request.Method}");
        await next.Invoke();
      });

      app.Run();
    }
  }
}