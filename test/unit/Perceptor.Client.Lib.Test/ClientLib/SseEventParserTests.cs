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
data: Hans Helvetia

event: finished
data: Hans Helvetia
",
			new object[] { new SsEvent("Hans Helvetia") }
		},
		new object[]{"", Array.Empty<object>()},
		new object[]{"event: waiting", Array.Empty<object>()},
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