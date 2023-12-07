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
using Perceptor.Client.Lib.InternalModels;
using Perceptor.Client.Lib.Repository;

namespace Perceptor.Client.Lib.Test.ClientLib;

[TestFixture]
public class SseEventParserTests
{
	private static readonly IEnumerable<object[]> _testCaseData = new[]
	{
		new object[]
		{
			@"event: waiting
data: 

event: waiting
data: 

event: generate
data: Hans

event: generate
data: Hans Mustermann

event: finished
data: Hans Mustermann
",
			new object[] { new SsEvent("Hans Mustermann") }
		},
		new object[] { "", Array.Empty<object>() },
		new object[] { "event: waiting", Array.Empty<object>() },
		new object[]
		{
			@"event: waiting
data: 

event: waiting
data: 

",
			Array.Empty<object>()
		},

		new object[]
		{
			@"
event: finished
data: First Answer

event: finished
data: Another answer
",
			new object[] { new SsEvent("First Answer"), new SsEvent("Another answer") }
		},

		new object[]
		{
			@"
event: finished
data: First Answer

event: waiting
event: finished
data: Another answer
",
			new object[] { new SsEvent("First Answer"), new SsEvent("Another answer") }
		},
	};

	[TestCaseSource(nameof(_testCaseData))]
	public async Task GIVEN_EventsSource_WHEN_Parsing_THEN_EventsResolved(string eventsSource,
		IReadOnlyList<object> expected)
	{
		var result = await SseEventParser.ParseEvents(eventsSource);
		var expectedList = expected.OfType<SsEvent>();
		result.Should().BeEquivalentTo(expectedList);
	}
}