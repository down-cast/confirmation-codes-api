using System.ComponentModel.DataAnnotations;

using Downcast.SecurityCodes.Manager;
using Downcast.SecurityCodes.Model;

using Microsoft.AspNetCore.Mvc;

namespace Downcast.SecurityCodes.API.Controllers;

[ApiController]
[Route("api/v1/security-codes")]
public class SecurityCodesController : ControllerBase
{
    private readonly ISecurityCodesManager _manager;

    public SecurityCodesController(ISecurityCodesManager manager)
    {
        _manager = manager;
    }

    /// <summary>
    /// Creates a new 6 digit security code associated with a target email address.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<SecurityCode>> CreateSecurityCode(SecurityCodeInput code)
    {
        SecurityCode createdSecurityCode = await _manager.Create(code).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetSecurityCode), new { code.Target }, createdSecurityCode);
    }

    /// <summary>
    /// Tries to validate a security code for a given email address.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpPost("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task ValidateSecurityCode(ValidateSecurityCode code)
    {
        return _manager.ValidateSecurityCode(code);
    }


    /// <summary>
    /// Retrieves the security code for a given target.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    [HttpGet("{target}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<SecurityCode> GetSecurityCode([Required] [EmailAddress] string target)
    {
        return _manager.Get(target);
    }

    /// <summary>
    /// Deletes a security code for a given target.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    [HttpDelete("{target}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task DeleteSecurityCode([Required] [EmailAddress] string target)
    {
        return _manager.Delete(target);
    }
}