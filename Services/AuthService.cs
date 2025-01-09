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
        throw new ApplicationException($"Error starting registration: {response.ReasonPhrase}");
      }

      var options = await response.Content.ReadFromJsonAsync<object>();
      if (options == null)
      {
        throw new ApplicationException("Invalid response from the server during registration");
      }

      return options;
    }

    public async Task<bool> VerifyRegistrationAsync(object credential)
    {
      var response = await _httpClient.PostAsJsonAsync("/api/auth/register/verify", credential);
      if (!response.IsSuccessStatusCode)
      {
        Console.WriteLine(response.ReasonPhrase);
      }

      return response.IsSuccessStatusCode;
    }

    public async Task<object> StartLoginAsync(string username)
    {
      var response = await _httpClient.PostAsJsonAsync("/api/auth/login", username);
      if (!response.IsSuccessStatusCode)
      {
        throw new ApplicationException($"Error starting login: {response.ReasonPhrase}");
      }

      var options = await response.Content.ReadFromJsonAsync<object>();
      if (options == null)
      {
        throw new ApplicationException("Invalid response from the server during login");
      }

      return options;
    }

    public async Task<bool> VerifyLoginAsync(object assertion)
    {
      var response = await _httpClient.PostAsJsonAsync("/api/auth/login/verify", assertion);
      return response.IsSuccessStatusCode;
    }
  }
}

