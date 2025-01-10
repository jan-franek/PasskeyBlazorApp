using BlazorApp.Data;
using BlazorApp.Data.Models;
using BlazorApp.Services;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BlazorApp.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController(Fido2Service fido2Service, AppDbContext dbContext) : ControllerBase
  {
    private readonly Fido2 _fido2 = fido2Service.GetFido2Instance();
    private readonly AppDbContext _dbContext = dbContext;

    [HttpPost("register")]
    public IActionResult StartRegistration([FromBody] string username)
    {
      // 1. Retrieve or create user
      var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);
      if (user == null)
      {
        user = new User { Username = username };
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
      }

      // 2. Get user's existing credentials
      var existingKeys = _dbContext.Passkeys
          .Where(p => p.UserId == user.Id)
          .Select(p => new PublicKeyCredentialDescriptor(Convert.FromBase64String(p.CredentialId)))
          .ToList();

      // 3. Create attestation options
      var userAccount = new Fido2User
      {
        Id = Encoding.UTF8.GetBytes(user.Id.ToString()), // byte[] user ID
        Name = user.Username,
        DisplayName = user.Username
      };

      var options = _fido2.RequestNewCredential(
          user: userAccount,
          excludeCredentials: existingKeys,
          authenticatorSelection: AuthenticatorSelection.Default,
          attestationPreference: AttestationConveyancePreference.None
      );

      // 4. Store options temporarily
      HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());

      Console.WriteLine(options.ToJson());

      // 5. Return options to the client
      return Ok(options);
    }

    [HttpPost("register/verify")]
    public async Task<IActionResult> VerifyRegistration([FromBody] AuthenticatorAttestationRawResponse attestationResponse)
    {
      try
      {
        // 1. Retrieve and remove stored options from session
        var jsonOptions = HttpContext.Session.GetString("fido2.attestationOptions");
        if (string.IsNullOrEmpty(jsonOptions))
        {
          return BadRequest("Attestation options not found in session.");
        }

        Console.WriteLine(jsonOptions);

        HttpContext.Session.Remove("fido2.attestationOptions");
        var options = CredentialCreateOptions.FromJson(jsonOptions);

        // 2. Define callback to check if the credential ID is unique to the user
        Task<bool> callback(IsCredentialIdUniqueToUserParams args, CancellationToken cancellationToken)
        {
          var existingCredentials = _dbContext.Passkeys
            .Where(p => p.CredentialId == Convert.ToBase64String(args.CredentialId))
            .ToList();

          return Task.FromResult(existingCredentials.Count == 0);
        }

        // 3. Verify the attestation response and create the new credential
        var success = await _fido2.MakeNewCredentialAsync(attestationResponse, options, callback);

        // 4. Store the new credential in the database
        var userId = Guid.Parse(Encoding.UTF8.GetString(success.Result!.User.Id));
        _dbContext.Passkeys.Add(new Passkey
        {
          UserId = userId,
          CredentialId = Convert.ToBase64String(success.Result.CredentialId),
          PublicKey = Convert.ToBase64String(success.Result.PublicKey),
          UserHandle = Convert.ToBase64String(success.Result.User.Id),
          Counter = success.Result.Counter
        });

        await _dbContext.SaveChangesAsync();

        // 5. Return success to the client
        return Ok(success);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpPost("login")]
    public IActionResult StartLogin([FromBody] string username)
    {
      // 1. Get user from the database
      var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);
      if (user == null)
      {
        return NotFound("Username is not registered");
      }

      // 2. Get registered credentials for the user
      var existingCredentials = _dbContext.Passkeys
          .Where(p => p.UserId == user.Id)
          .Select(p => new PublicKeyCredentialDescriptor(Convert.FromBase64String(p.CredentialId)))
          .ToList();

      // 3. Create assertion options
      var options = _fido2.GetAssertionOptions(
          existingCredentials,
          UserVerificationRequirement.Discouraged
      );

      // 4. Temporarily store options in session
      HttpContext.Session.SetString("fido2.assertionOptions", options.ToJson());

      // 5. Return options to the client
      return Ok(options);
    }

    [HttpPost("login/verify")]
    public async Task<IActionResult> VerifyLogin([FromBody] AuthenticatorAssertionRawResponse clientResponse)
    {
      try
      {
        // 1. Retrieve and remove stored assertion options
        var jsonOptions = HttpContext.Session.GetString("fido2.assertionOptions");
        if (string.IsNullOrEmpty(jsonOptions))
        {
          return BadRequest("Assertion options not found in session.");
        }

        HttpContext.Session.Remove("fido2.assertionOptions");
        var options = AssertionOptions.FromJson(jsonOptions);

        // 2. Get registered credential from the database
        var credentialId = Convert.ToBase64String(clientResponse.Id);
        var storedCredential = _dbContext.Passkeys.FirstOrDefault(p => p.CredentialId == credentialId);

        if (storedCredential == null)
        {
          return BadRequest("Credential not found.");
        }

        var storedPublicKey = Convert.FromBase64String(storedCredential.PublicKey);

        // 3. Get the stored counter
        var storedCounter = storedCredential.Counter;

        // 4. Create callback to check if user handle owns the credential ID
        Task<bool> callback(IsUserHandleOwnerOfCredentialIdParams args, CancellationToken cancellationToken)
        {
          var userHandle = Convert.ToBase64String(args.UserHandle);
          var matchingCredentials = _dbContext.Passkeys
            .Where(p => p.UserHandle == userHandle)
            .ToList();

          return Task.FromResult(matchingCredentials.Any(c => c.CredentialId == Convert.ToBase64String(args.CredentialId)));
        }

        // 5. Verify the assertion
        var result = await _fido2.MakeAssertionAsync(clientResponse, options, storedPublicKey, storedCounter, callback);

        // 6. Update the counter in the database
        storedCredential.Counter = result.Counter;
        _dbContext.SaveChanges();

        // 7. Return success to the client
        return Ok(result);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}
