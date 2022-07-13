using Downcast.SecurityCodes.Manager;
using Downcast.SecurityCodes.Model;

using Microsoft.AspNetCore.Mvc;

namespace Downcast.SecurityCodes.API.Controllers;

[ApiController]
[Route("api/v1/confirmation-codes")]
public class SecurityCodesController : ControllerBase
{
    private readonly ISecurityCodesManager _manager;

    public SecurityCodesController(ISecurityCodesManager manager)
    {
        _manager = manager;
    }


    [HttpPost]
    public Task CreateConfirmationCode(SecurityCodeInput code)
    {
        return _manager.Create(code);
    }

    [HttpPost("validate")]
    public Task ValidateConfirmationCode(ValidateSecurityCode code)
    {
        return _manager.ValidateSecurityCode(code);
    }
}