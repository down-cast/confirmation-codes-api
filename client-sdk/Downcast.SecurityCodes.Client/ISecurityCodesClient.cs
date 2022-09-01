using Downcast.SecurityCodes.Client.Model;

using Refit;

namespace Downcast.SecurityCodes.Client;

public interface ISecurityCodesClient
{
    /// <summary>
    /// Retrieves the security code for a given target.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    [Get("/api/v1/security-codes/{target}")]
    Task<ApiResponse<SecurityCode>> GetSecurityCode(string target);

    /// <summary>
    /// Creates a new 6 digit security code associated with a target email address.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [Post("/api/v1/security-codes")]
    Task<ApiResponse<SecurityCode>> CreateSecurityCode(SecurityCodeInput code);

    /// <summary>
    /// Tries to validate a security code for a given email address.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [Post("/api/v1/security-codes/validate")]
    Task<HttpResponseMessage> ValidateSecurityCode(ValidateSecurityCode code);

    /// <summary>
    /// Deletes a security code for a given target.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    [Delete("/api/v1/security-codes/{target}")]
    Task<HttpResponseMessage> DeleteSecurityCode(string target);
}