using System.ComponentModel.DataAnnotations;

namespace Downcast.SecurityCodes.Model;

public class SecurityCodeInput
{
    [Required]
    [EmailAddress]
    public string Target { get; set; } = null!;
}