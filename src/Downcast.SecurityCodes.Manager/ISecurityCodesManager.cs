using Downcast.SecurityCodes.Model;

namespace Downcast.SecurityCodes.Manager;

public interface ISecurityCodesManager
{
    Task ValidateSecurityCode(ValidateSecurityCode code);
    Task<SecurityCode> Create(SecurityCodeInput securityCode);
    Task<SecurityCode> Get(string target);
    Task Delete(string target);
}