using System.ComponentModel.DataAnnotations;

namespace Downcast.SecurityCodes.Model;

public class ValidateSecurityCode : SecurityCodeInput
{
    [Required]
    public string Code { get; init; } = null!;
}