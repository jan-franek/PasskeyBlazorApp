namespace BlazorApp.Data.Models
{
  public class Passkey
  {
    /// <summary>
    /// Unique identifier for the passkey.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Foreign key linking to the User.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Base64-encoded credential ID.
    /// </summary>
    public string CredentialId { get; set; } = string.Empty;

    /// <summary>
    /// Base64-encoded public key.
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Base64-encoded user handle.
    /// </summary>
    public string UserHandle { get; set; } = string.Empty;

    /// <summary>
    /// Used to prevent replay attacks.
    /// </summary>
    public uint Counter { get; set; } = 0;
  }
}