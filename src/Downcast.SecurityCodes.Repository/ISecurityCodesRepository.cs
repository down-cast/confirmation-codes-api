using Downcast.SecurityCodes.Model;

namespace Downcast.SecurityCodes.Repository;

public interface ISecurityCodesRepository
{
    Task<SecurityCode> Create(string code, string target);

    Task<SecurityCode> Get(string target);

    Task UpdateConfirmationDate(string target, DateTime date);
    
    Task Delete(string target);
}