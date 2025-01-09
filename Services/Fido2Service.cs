using Fido2NetLib;

namespace BlazorApp.Services
{
  public class Fido2Service
  {
    private readonly Fido2 _fido2;

    public Fido2Service(string serverDomain, string[] origins)
    {
      _fido2 = new Fido2(new Fido2Configuration
      {
        ServerName = "BlazorApp",
        ServerDomain = serverDomain,
        Origins = new HashSet<string>(origins),
        TimestampDriftTolerance = 1000
      });
    }

    public Fido2 GetFido2Instance() => _fido2;
  }
}
