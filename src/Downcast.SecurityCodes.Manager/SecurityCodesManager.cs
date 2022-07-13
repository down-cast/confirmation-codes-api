using System.Security.Cryptography;

using Downcast.Common.Errors;
using Downcast.SecurityCodes.Model;
using Downcast.SecurityCodes.Repository;

using Microsoft.Extensions.Options;

namespace Downcast.SecurityCodes.Manager;

public class SecurityCodesManager : ISecurityCodesManager
{
    private readonly ISecurityCodesRepository _repository;
    private readonly IOptions<SecurityCodesOptions> _options;

    public SecurityCodesManager(ISecurityCodesRepository repository, IOptions<SecurityCodesOptions> options)
    {
        _repository = repository;
        _options    = options;
    }

    public async Task ValidateSecurityCode(ValidateSecurityCode code)
    {
        SecurityCode securityCode = await _repository.GetByTarget(code.Target).ConfigureAwait(false);
        if (!securityCode.Code.Equals(code.Code, StringComparison.Ordinal))
        {
            throw new DcException(ErrorCodes.EntityNotFound, "Invalid security code");
        }

        await _repository.UpdateConfirmationDate(code.Target, DateTime.UtcNow).ConfigureAwait(false);
    }

    public Task Create(SecurityCodeInput securityCode)
    {
        return _repository.Create(GenerateRandomNumber(_options.Value.CodeLength), securityCode.Target);
    }


    private static string GenerateRandomNumber(int digits)
    {
        var upperLimit = int.Parse(string.Concat(Enumerable.Repeat('9', digits)));
        var format = string.Concat(Enumerable.Repeat('0', digits));
        return RandomNumberGenerator.GetInt32(0, upperLimit).ToString(format);
    }
}