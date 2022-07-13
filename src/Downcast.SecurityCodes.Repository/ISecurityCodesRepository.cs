using Downcast.SecurityCodes.Model;

namespace Downcast.SecurityCodes.Repository;

public interface ISecurityCodesRepository
{
    Task<string> Create(string code, string target);

    Task<Model.SecurityCode> GetByTarget(string target);

    Task UpdateConfirmationDate(string target, DateTime date);
}