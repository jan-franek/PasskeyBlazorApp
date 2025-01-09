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
      var fido2ServerDomain = fido2Config["ServerDomain"];
      var fido2Origins = fido2Config["Origins"]?.Split(';', StringSplitOptions.RemoveEmptyEntries);
      if (fido2ServerDomain == null || fido2Origins == null) throw new ApplicationException("Invalid configuration for FIDO2 service.");
      builder.Services.AddSingleton(sp => new Fido2Service(fido2ServerDomain, fido2Origins));

      // Add communication with the backend for registration and login
      builder.Services.AddScoped<AuthService>(sp =>
      {
        //FORTESTING: Trust all certificates
        var handler = new HttpClientHandler
        {
          ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
        };
        var httpClient = new HttpClient(handler)
        {
          BaseAddress = new Uri(fido2Origins[0]) // Use the first origin as the base address
        };
        return new AuthService(httpClient);
      });

      builder.Logging.ClearProviders();
      builder.Logging.AddConsole(); // Add console logging for better diagnostics

      builder.Services.AddServerSideBlazor()
        .AddCircuitOptions(options =>
        {
          options.DetailedErrors = true; // Enable detailed error reporting
        });

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

      //FORDEBUG
      app.Use(async (context, next) =>
      {
        Console.WriteLine($"Request Path: {context.Request.Path}, Method: {context.Request.Method}");
        await next.Invoke();
      });
      //ENDFORDEBUG

      app.Run();
    }
  }
}