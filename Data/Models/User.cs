namespace BlazorApp.Data.Models
{
  public class User
  {
    /// <summary>
    /// A unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The user's chosen username.
    /// </summary>
    public string Username { get; set; } = string.Empty;
  }
}