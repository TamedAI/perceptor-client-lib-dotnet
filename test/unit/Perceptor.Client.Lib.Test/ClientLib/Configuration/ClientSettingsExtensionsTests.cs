using FluentAssertions;
using Perceptor.Client.Lib.Configuration;

namespace Perceptor.Client.Lib.Test.ClientLib.Configuration;

public class ClientSettingsExtensionsTests
{
	private static readonly IEnumerable<object[]> _validationTestData = new[]
	{
		new object[] { new ClientSettings(apiKey:"1", url: "invalid_url")},
		new object[] { new ClientSettings( " ",  "http://some.url")},
		new object[] { new ClientSettings("abc", "http://some.url"){RetryCount = -1}},
		new object[] { new ClientSettings("abc",  "http://some.url"){MaximalNumberOfParallelRequests = 0}},
	};
	
	[TestCaseSource(nameof(_validationTestData))]
	public void GIVEN_Invalid_ClientSettings_WHEN_Validate_THEN_ErrorText_Is_NotEmpty(ClientSettings settings)
	{
		settings.Validate().Should().NotBeEmpty();
	}

	[Test]
	public void GIVEN_Valid_ClientSettings_WHEN_Validate_THEN_ErrorText_IsEmpty()
	{
		new ClientSettings("1", "http://some.url").Validate().Should().BeEmpty();
	}
}