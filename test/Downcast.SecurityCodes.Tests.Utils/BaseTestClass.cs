using Bogus;

using Downcast.SecurityCodes.Client;
using Downcast.SecurityCodes.Client.Model;
using Downcast.SecurityCodes.Tests.Utils.DataFakers;

using Refit;

using Xunit;

namespace Downcast.SecurityCodes.Tests.Utils;

public class BaseTestClass : IAsyncLifetime
{
    protected ISecurityCodesClient Client { get; }
    protected Faker Faker { get; } = new();

    private readonly List<SecurityCodeInput> _securityCodesGenerated = new();

    public BaseTestClass()
    {
        HttpClient httpClient = new SecurityCodesServerInstance().CreateClient();
        Client = RestService.For<ISecurityCodesClient>(httpClient);
    }

    protected SecurityCodeInput GenerateSecurityCodeInput()
    {
        SecurityCodeInput securityCode = new SecurityCodeInputFaker().Generate();
        _securityCodesGenerated.Add(securityCode);
        return securityCode;
    }

    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.WhenAll(_securityCodesGenerated.Select(code => Client.DeleteSecurityCode(code.Target)));
    }
}