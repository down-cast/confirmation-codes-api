using Downcast.SecurityCodes.Model;

namespace Downcast.SecurityCodes.Manager;

public interface ISecurityCodesManager
{
    Task ValidateSecurityCode(ValidateSecurityCode code);
    Task Create(SecurityCodeInput securityCode);
    Task<SecurityCode> GetByTarget(string target);
}