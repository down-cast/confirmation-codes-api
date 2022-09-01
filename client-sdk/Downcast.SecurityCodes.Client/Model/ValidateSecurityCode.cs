namespace Downcast.SecurityCodes.Client.Model;

public class ValidateSecurityCode : SecurityCodeInput
{
    public string Code { get; init; } = null!;
}