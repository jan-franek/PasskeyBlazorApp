// this file is a copy of https://github.com/passwordless-lib/fido2-net-lib/blob/master/Src/Fido2.BlazorWebAssembly/WebAuthn.cs
using Fido2NetLib;
using Microsoft.JSInterop;

namespace BlazorApp.Services
{
  /// <summary>
  /// Module for accessing the browser's WebAuthn API.
  /// </summary>
  public class WebAuthnService
  {
    private IJSObjectReference _jsModule = null!;
    private readonly Task _initializer;

    public WebAuthnService(IJSRuntime js)
    {
      _initializer = Task.Run(async () => _jsModule = await js.InvokeAsync<IJSObjectReference>("import", "./js/webauthn.js"));
    }

    /// <summary>
    /// Wait for this to make sure this module is initialized.
    /// </summary>
    public Task Init() => _initializer;

    /// <summary>
    /// Whether or not this browser supports WebAuthn.
    /// </summary>
    public async Task<bool> IsWebAuthnSupportedAsync() => await _jsModule.InvokeAsync<bool>("isWebAuthnPossible");

    /// <summary>
    /// Creates a new credential.
    /// </summary>
    public async Task<AuthenticatorAttestationRawResponse> CreateCredentialAsync(CredentialCreateOptions options) =>
      await _jsModule.InvokeAsync<AuthenticatorAttestationRawResponse>("createCredential", options);

    /// <summary>
    /// Verifies a credential for login.
    /// </summary>
    public async Task<AuthenticatorAssertionRawResponse> GetAssertionAsync(AssertionOptions options) =>
      await _jsModule.InvokeAsync<AuthenticatorAssertionRawResponse>("getAssertion", options);
  }

  public static class WebAuthnServiceDependencyInjection
  {
    /// <summary>
    /// Adds the <see cref="WebAuthnService"/> service to the DI container.
    /// </summary>
    public static IServiceCollection AddWebAuthn(this IServiceCollection services) =>
      services.AddScoped<WebAuthnService>();
  }
}
