using FluentAssertions;

namespace Perceptor.Client.Lib.Test.ClientLib;

[TestFixture]
public class StructuredResponseParserTests
{
	[TestCase("direct_answer_text")]
	[TestCase("2")]
	[TestCase("")]
	[TestCase("  ")]
	public void GIVEN_NonJsonText_WHEN_Parsed_THEN_TextElementIsWholeText(string input)
	{
		var result = StructuredResponseParser.ParseStructuredText(input);
		result.Should().ContainKey(StructuredResponseParser.KeyNameText);
		result[StructuredResponseParser.KeyNameText].Should().Be(input.Trim());
	}

	[TestCase("{\"scores\": {\"score\": 0.643, \"n_tokens\": 5}}")]
	[TestCase("{\"entry\": {\"n_tokens\": 5}}")]
	[TestCase("{\"results\": [{\"n_tokens\": 5}]}")]
	public void GIVEN_Json_Without_Text_Element_WHEN_Parsed_THEN_TextIsEmpty(string input)
	{
		var result = StructuredResponseParser.ParseStructuredText(input);
		result.Should().ContainKey(StructuredResponseParser.KeyNameText);
		result[StructuredResponseParser.KeyNameText].ToString().Should().BeEmpty();
	}

	[TestCase("{\"text\": \"direct_answer\", \"scores\": {\"score\": 0.23, \"n_tokens\": 5}}", "direct_answer")]
	public void GIVEN_Json_With_TextElement_WHEN_Parsed_THEN_TextIsFound(string input, string expected)
	{
		var result = StructuredResponseParser.ParseStructuredText(input);
		result.Should().ContainKey(StructuredResponseParser.KeyNameText);
		result[StructuredResponseParser.KeyNameText].ToString().Should().Be(expected);
	}

	[Test]
	public void GIVEN_Json_With_Entries_WHEN_Parsed_THEN_TextAndEntriesFound()
	{
		const string jsonText =
			"{\"text\": \"direct_answer\", \"scores\": {\"score\": 0.64, \"n_tokens\": 5}}";
		var result = StructuredResponseParser.ParseStructuredText(jsonText);
		result.Should().ContainKey(StructuredResponseParser.KeyNameText);
		result.Should().ContainKey("scores");
		var scores = result["scores"];
		scores.Should().BeAssignableTo<IReadOnlyDictionary<string, object>>();
		var scoresDict = scores as IDictionary<string, object>;
		scoresDict!["score"].Should().Be("0.64");
		scoresDict["n_tokens"].Should().Be("5");
	}

	[Test]
	public void GIVEN_Json_With_Array_WHEN_Parsed_THEN_ListIsPresent()
	{
		const string jsonText = "{\"valarray\": [{\"n_tokens\": 5}]}";
		var result = StructuredResponseParser.ParseStructuredText(jsonText);
		result.Should().ContainKey("valarray");
		result["valarray"].Should().BeAssignableTo<List<object>>();
	}
}