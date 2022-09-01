using System.Net;

using Downcast.SecurityCodes.Client.Model;
using Downcast.SecurityCodes.Tests.Utils;

using FluentAssertions;

using Refit;

namespace Downcast.SecurityCodes.Tests;

public class SecurityCodesTests : BaseTestClass
{
    private const int CodeLength = 6;

    [Fact]
    public async Task CreateSecurityCode_Success()
    {
        SecurityCodeInput securityCode = GenerateSecurityCodeInput();
        ApiResponse<SecurityCode> response = await Client.CreateSecurityCode(securityCode).ConfigureAwait(false);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        response.Content.Should().NotBeNull();
        response.Content!.Target.Should().Be(securityCode.Target);
        response.Content!.Code.Should().HaveLength(CodeLength);
    }

    [Fact]
    public async Task CreateSecurityCode_Success_Idempotent()
    {
        SecurityCodeInput securityCode = GenerateSecurityCodeInput();

        ApiResponse<SecurityCode> response = await Client.CreateSecurityCode(securityCode).ConfigureAwait(false);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        response = await Client.CreateSecurityCode(securityCode).ConfigureAwait(false);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        response = await Client.CreateSecurityCode(securityCode).ConfigureAwait(false);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }


    [Fact]
    public async Task GetSecurityCode_Success()
    {
        SecurityCodeInput securityCode = GenerateSecurityCodeInput();
        ApiResponse<SecurityCode> createCodeResponse =
            await Client.CreateSecurityCode(securityCode).ConfigureAwait(false);
        createCodeResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        ApiResponse<SecurityCode> getCodeResponse =
            await Client.GetSecurityCode(securityCode.Target).ConfigureAwait(false);

        getCodeResponse.IsSuccessStatusCode.Should().BeTrue();
        getCodeResponse.Content!.Should().BeEquivalentTo(createCodeResponse.Content);
    }

    [Fact]
    public async Task GetSecurityCode_NotFound()
    {
        SecurityCodeInput securityCode = GenerateSecurityCodeInput();

        ApiResponse<SecurityCode> getCodeResponse =
            await Client.GetSecurityCode(securityCode.Target).ConfigureAwait(false);

        getCodeResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSecurityCode_Success()
    {
        SecurityCodeInput securityCode = GenerateSecurityCodeInput();

        // Create security code
        ApiResponse<SecurityCode> createCodeResponse =
            await Client.CreateSecurityCode(securityCode).ConfigureAwait(false);
        createCodeResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Ensure security code exists
        ApiResponse<SecurityCode> getCodeResponse =
            await Client.GetSecurityCode(securityCode.Target).ConfigureAwait(false);
        getCodeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Delete security code
        HttpResponseMessage deleteCodeResponse =
            await Client.DeleteSecurityCode(securityCode.Target).ConfigureAwait(false);
        deleteCodeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Ensure security code is deleted
        getCodeResponse =
            await Client.GetSecurityCode(securityCode.Target).ConfigureAwait(false);
        getCodeResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task ValidateCode_Success()
    {
        SecurityCodeInput securityCode = GenerateSecurityCodeInput();

        // Create security code
        ApiResponse<SecurityCode> createCodeResponse =
            await Client.CreateSecurityCode(securityCode).ConfigureAwait(false);
        createCodeResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Validate security code
        HttpResponseMessage validateCodeResponse = await Client.ValidateSecurityCode(new ValidateSecurityCode
        {
            Code   = createCodeResponse.Content!.Code,
            Target = createCodeResponse.Content!.Target
        }).ConfigureAwait(false);

        validateCodeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Ensure confirmation date is defined
        ApiResponse<SecurityCode> getCodeResponse =
            await Client.GetSecurityCode(securityCode.Target).ConfigureAwait(false);
        getCodeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getCodeResponse.Content!.ConfirmationDate.Should().NotBeNull();
        getCodeResponse.Content!.ConfirmationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }


    [Fact]
    public async Task ValidateCode_Invalid_Code()
    {
        SecurityCodeInput securityCode = GenerateSecurityCodeInput();

        // Create security code
        ApiResponse<SecurityCode> createCodeResponse =
            await Client.CreateSecurityCode(securityCode).ConfigureAwait(false);
        createCodeResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Validate security code
        HttpResponseMessage validateCodeResponse = await Client.ValidateSecurityCode(new ValidateSecurityCode
        {
            Code   = createCodeResponse.Content!.Code + "invalid",
            Target = createCodeResponse.Content!.Target
        }).ConfigureAwait(false);

        validateCodeResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task ValidateCode_Non_Existent_Target()
    {
        SecurityCodeInput securityCode = GenerateSecurityCodeInput();

        // Create security code
        ApiResponse<SecurityCode> createCodeResponse =
            await Client.CreateSecurityCode(securityCode).ConfigureAwait(false);
        createCodeResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Validate security code
        HttpResponseMessage validateCodeResponse = await Client.ValidateSecurityCode(new ValidateSecurityCode
        {
            Code   = createCodeResponse.Content!.Code,
            Target = createCodeResponse.Content!.Target + "not-found"
        }).ConfigureAwait(false);

        validateCodeResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}