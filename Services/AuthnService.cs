using Fido2NetLib;

namespace BlazorApp.Services
{
  public class AuthnService(HttpClient httpClient)
  {
    private readonly HttpClient _httpClient = httpClient;

    public async Task<CredentialCreateOptions> StartRegistrationAsync(string username)
    {
      var response = await _httpClient.PostAsJsonAsync("/api/auth/register", username);
      if (!response.IsSuccessStatusCode)
      {
        throw new ApplicationException($"Error starting registration: {response.ReasonPhrase} [{await response.Content.ReadAsStringAsync()}]");
      }

      var options = await response.Content.ReadFromJsonAsync<CredentialCreateOptions>();
      if (options == null)
      {
        throw new ApplicationException("Invalid response from the server during registration");
      }

      return options;
    }

    public async Task<HttpResponseMessage> VerifyRegistrationAsync(AuthenticatorAttestationRawResponse credential)
    {
      return await _httpClient.PostAsJsonAsync("/api/auth/register/verify", credential);
    }

    public async Task<AssertionOptions> StartLoginAsync(string username)
    {
      var response = await _httpClient.PostAsJsonAsync("/api/auth/login", username);
      if (!response.IsSuccessStatusCode)
      {
        throw new ApplicationException($"Error starting login: {response.ReasonPhrase} [{await response.Content.ReadAsStringAsync()}]");
      }

      var options = await response.Content.ReadFromJsonAsync<AssertionOptions>();
      if (options == null)
      {
        throw new ApplicationException("Invalid response from the server during login");
      }

      return options;
    }

    public async Task<HttpResponseMessage> VerifyLoginAsync(AuthenticatorAssertionRawResponse assertion)
    {
      var response = await _httpClient.PostAsJsonAsync("/api/auth/login/verify", assertion);
      if (response == null)
      {
        throw new ApplicationException("Invalid response from the server during login");
      }

      return response;
    }
  }

  public static class AuthnServiceDependencyInjection
  {
    /// <summary>
    /// Adds the <see cref="AuthnService"/> service to the DI container.
    /// </summary>
    public static IServiceCollection AddAuthn(this IServiceCollection services, Uri apiUri)
    {
      return services.AddScoped(sp =>
      {
        var httpClient = new HttpClient()
        {
          BaseAddress = apiUri
        };
        return new AuthnService(httpClient);
      });
    }
  }
}

