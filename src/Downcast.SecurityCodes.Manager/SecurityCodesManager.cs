using System.Security.Cryptography;

using Downcast.Common.Errors;
using Downcast.SecurityCodes.Model;
using Downcast.SecurityCodes.Repository;

using Microsoft.Extensions.Options;

namespace Downcast.SecurityCodes.Manager;

public class SecurityCodesManager : ISecurityCodesManager
{
    private readonly ISecurityCodesRepository _repository;
    private readonly int _upperLimit;
    private readonly string _format;

    public SecurityCodesManager(ISecurityCodesRepository repository, IOptions<SecurityCodesOptions> options)
    {
        _repository = repository;
        _upperLimit = int.Parse(string.Concat(Enumerable.Repeat('9', options.Value.CodeLength)));
        _format     = string.Concat(Enumerable.Repeat('0', options.Value.CodeLength));
    }

    public async Task ValidateSecurityCode(ValidateSecurityCode code)
    {
        SecurityCode securityCode = await Get(code.Target).ConfigureAwait(false);
        if (!securityCode.Code.Equals(code.Code, StringComparison.Ordinal))
        {
            throw new DcException(ErrorCodes.InvalidSecurityCode);
        }

        await _repository.UpdateConfirmationDate(code.Target, DateTime.UtcNow).ConfigureAwait(false);
    }

    public Task<SecurityCode> Create(SecurityCodeInput securityCode)
    {
        return _repository.Create(GenerateRandomNumber(), securityCode.Target);
    }

    public Task<SecurityCode> Get(string target)
    {
        return _repository.Get(target);
    }

    public Task Delete(string target)
    {
        return _repository.Delete(target);
    }

    private string GenerateRandomNumber()
    {
        return RandomNumberGenerator.GetInt32(0, _upperLimit).ToString(_format);
    }
}