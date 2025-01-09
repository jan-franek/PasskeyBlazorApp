namespace BlazorApp.Services
{
  public class AuthService(HttpClient httpClient)
  {
    private readonly HttpClient _httpClient = httpClient;

    public async Task<object> StartRegistrationAsync(string username)
    {
      var response = await _httpClient.PostAsJsonAsync("/api/auth/register", username);
      if (!response.IsSuccessStatusCode)
      {
        throw new ApplicationException($"Error starting registration: {response.ReasonPhrase} [{response.Content.ReadAsStringAsync()}]");
      }

      var options = await response.Content.ReadFromJsonAsync<object>();
      if (options == null)
      {
        throw new ApplicationException("Invalid response from the server during registration");
      }

      return options;
    }

    public async Task<HttpResponseMessage> VerifyRegistrationAsync(object credential)
    {
      return await _httpClient.PostAsJsonAsync("/api/auth/register/verify", credential);
    }

    public async Task<object> StartLoginAsync(string username)
    {
      var response = await _httpClient.PostAsJsonAsync("/api/auth/login", username);
      if (!response.IsSuccessStatusCode)
      {
        throw new ApplicationException($"Error starting login: {response.ReasonPhrase} [{response.Content.ReadAsStringAsync()}]");
      }

      var options = await response.Content.ReadFromJsonAsync<object>();
      if (options == null)
      {
        throw new ApplicationException("Invalid response from the server during login");
      }

      return options;
    }

    public async Task<HttpResponseMessage> VerifyLoginAsync(object assertion)
    {
      var response = await _httpClient.PostAsJsonAsync("/api/auth/login/verify", assertion);
      if (response == null)
      {
        throw new ApplicationException("Invalid response from the server during login");
      }

      return response;
    }
  }
}

