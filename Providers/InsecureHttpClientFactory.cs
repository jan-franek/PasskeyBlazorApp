namespace BlazorApp.Providers
{
  public static class InsecureHttpClientFactory
  {
    public static HttpClient Create()
    {
      var handler = new HttpClientHandler
      {
        ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
      };

      return new HttpClient(handler);
    }
  }
}
