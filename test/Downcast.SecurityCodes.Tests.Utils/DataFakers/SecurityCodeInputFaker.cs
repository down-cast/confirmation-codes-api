using Bogus;

using Downcast.SecurityCodes.Client.Model;

namespace Downcast.SecurityCodes.Tests.Utils.DataFakers;

public sealed class SecurityCodeInputFaker : Faker<SecurityCodeInput>
{
    public SecurityCodeInputFaker()
    {
        RuleFor(c => c.Target, faker => faker.Internet.Email());
    }
}