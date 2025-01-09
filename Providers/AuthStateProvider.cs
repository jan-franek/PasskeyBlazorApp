using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorApp.Providers
{
  public class AuthStateProvider : AuthenticationStateProvider
  {
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      var identity = new ClaimsIdentity();
      var user = new ClaimsPrincipal(identity);

      return Task.FromResult(new AuthenticationState(user));
    }

    public void AuthenticateUser(string userIdentifier)
    {
      var identity = new ClaimsIdentity(
      [
          new Claim(ClaimTypes.Name, userIdentifier),
        ], "Custom Authentication");

      var user = new ClaimsPrincipal(identity);

      NotifyAuthenticationStateChanged(
          Task.FromResult(new AuthenticationState(user)));
    }

    public void LogOut()
    {
      // Clear authentication
      var user = new ClaimsPrincipal(new ClaimsIdentity());

      NotifyAuthenticationStateChanged(
          Task.FromResult(new AuthenticationState(user)));
    }
  }
}
