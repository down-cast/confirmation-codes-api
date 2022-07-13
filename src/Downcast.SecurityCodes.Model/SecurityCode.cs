namespace Downcast.SecurityCodes.Model;

public class SecurityCode : SecurityCodeInput
{
    public string Code { get; init; } = null!;
    public DateTime UpdatedDate { get; init; }
    public DateTime ConfirmationDate { get; init; }
}