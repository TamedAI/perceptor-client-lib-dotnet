// /*
// Copyright 2023 TamedAI GmbH
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */

using FluentAssertions;
using Perceptor.Client.Lib.Configuration;

namespace Perceptor.Client.Lib.Test.ClientLib.Configuration;

public class ClientSettingsExtensionsTests
{
	private static readonly IEnumerable<object[]> _validationTestData = new[]
	{
		new object[] { new ClientSettings(apiKey: "1", url: "invalid_url") },
		new object[] { new ClientSettings(" ", "http://some.url") },
		new object[] { new ClientSettings("abc", "http://some.url") { RetryCount = -1 } },
		new object[] { new ClientSettings("abc", "http://some.url") { MaximalNumberOfParallelRequests = 0 } },
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