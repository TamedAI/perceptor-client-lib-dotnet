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

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Perceptor.Client.Lib.InternalModels;
using Perceptor.Client.Lib.Models;
using Perceptor.Client.Lib.Repository;

namespace Perceptor.Client.Lib.Test.ClientLib.Repository;

[TestFixture]
public class RepositoryHelpersTests
{
	[Test]
	public void GIVEN_Flavour_WHEN_MapToPayloadString_THEN_Is_Mapped()
	{
		const string someFlavour = "some flavor";

		var notRelevantTimeout = new WaitTimeOut(TimeSpan.FromSeconds(100));
		var result = RepositoryHelpers.MapToPayloadString(
			new PerceptorRequestPayload(PerceptorRequest.WithFlavor(someFlavour),
				InstructionMethod.Question,
				InstructionContextData.ForText("not relevant"),
				new Instruction("not relevant"),
				Array.Empty<ClassificationEntry>()),
			notRelevantTimeout
		);

		VerifyContains(result, "flavor", someFlavour);
	}

	[Test]
	public void GIVEN_Classes_WHEN_MapToPayloadString_THEN_Is_Mapped()
	{
		var notRelevantTimeout = new WaitTimeOut(TimeSpan.FromSeconds(100));
		var classes = new[] { "class_1", "class_2" };

		var result = RepositoryHelpers.MapToPayloadString(
			new PerceptorRequestPayload(PerceptorRequest.WithFlavor("not relevant"),
				InstructionMethod.Classify,
				InstructionContextData.ForText("not relevant"),
				new Instruction("not relevant"),
				classes.Select(c => new ClassificationEntry(c)).ToArray()
			),
			notRelevantTimeout
		);

		VerifyContains(result, "classes", classes);
	}


	[Test]
	public void GIVEN_Instruction_WHEN_MapToPayloadString_THEN_Is_Mapped()
	{
		const string instructionString = "some instruction";

		var notRelevantTimeout = new WaitTimeOut(TimeSpan.FromSeconds(100));
		var result = RepositoryHelpers.MapToPayloadString(
			new PerceptorRequestPayload(PerceptorRequest.WithFlavor("not relevant"),
				InstructionMethod.Question,
				InstructionContextData.ForText("not relevant"),
				new Instruction(instructionString),
				Array.Empty<ClassificationEntry>()),
			notRelevantTimeout
		);

		VerifyContains(result, "instruction", instructionString);
	}

	[Test]
	public void GIVEN_Timeout_WHEN_MapToPayloadString_THEN_IsMapped()
	{
		var waitTimeout = new WaitTimeOut(TimeSpan.FromSeconds(495));
		var result = RepositoryHelpers.MapToPayloadString(
			new PerceptorRequestPayload(PerceptorRequest.WithFlavor("not relevant"),
				InstructionMethod.Question,
				InstructionContextData.ForText("not relevant"),
				new Instruction("not relevant"),
				Array.Empty<ClassificationEntry>()),
			waitTimeout
		);

		VerifyContains(result, "waitTimeout", waitTimeout.TimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture));
	}

	[Test]
	public void GIVEN_Parameters_WHEN_MapToPayloadString_THEN_IsMapped()
	{
		IReadOnlyDictionary<string, string> paramsDict = new Dictionary<string, string>()
		{
			{ "par1", "val1" },
			{ "par2", "val2" }
		};

		var request = new PerceptorRequest("not relevant flavor",
			paramsDict, false);

		var notRelevantTimeout = new WaitTimeOut(TimeSpan.FromSeconds(100));
		var result = RepositoryHelpers.MapToPayloadString(
			new PerceptorRequestPayload(request,
				InstructionMethod.Question,
				InstructionContextData.ForText("not relevant"),
				new Instruction("not relevant"),
				Array.Empty<ClassificationEntry>()),
			notRelevantTimeout
		);

		var des = Deserialize<Dictionary<string, object>>(result);
		var paramsElements = (JsonElement)des["params"];
		var asDictionary = paramsElements.Deserialize<Dictionary<string, string>>();
		paramsDict.Should().BeSubsetOf(asDictionary);
	}

	[TestCase(true, "true")]
	[TestCase(false, "false")]
	public void GIVEN_ReturnScores_WHEN_MapToPayloadString_THEN_ReturnScoresIsSet(bool returnScores,
		string expectedToFind)
	{
		IReadOnlyDictionary<string, string> paramsDict = new Dictionary<string, string>()
		{
			{ "par1", "val1" },
			{ "par2", "val2" }
		};
		var request = new PerceptorRequest("not relevant flavor",
			paramsDict, returnScores);

		var notRelevantTimeout = new WaitTimeOut(TimeSpan.FromSeconds(100));
		var result = RepositoryHelpers.MapToPayloadString(
			new PerceptorRequestPayload(request,
				InstructionMethod.Question,
				InstructionContextData.ForText("not relevant"),
				new Instruction("not relevant"),
				Array.Empty<ClassificationEntry>()),
			notRelevantTimeout
		);

		var des = Deserialize<Dictionary<string, object>>(result);
		var paramsElements = (JsonElement)des["params"];
		var asDictionary = paramsElements.Deserialize<Dictionary<string, string>>();
		asDictionary.Should().ContainKey("returnScores");
		asDictionary!["returnScores"].Should().Be(expectedToFind);
	}

	private const string _RESPONSE_WITH_DETAIL = @"{  ""detail"": ""detail_text""}";
	private const string _RESPONSE_WITH_DETAIL_2 = @"{  ""detail"": [""detail_text""]}";
	private const string _RESPONSE_WITH_OTHER_TEXT = "some other response";

	[TestCase(_RESPONSE_WITH_DETAIL, "detail_text")]
	[TestCase(_RESPONSE_WITH_DETAIL_2, "[\"detail_text\"]")]
	[TestCase(_RESPONSE_WITH_OTHER_TEXT, _RESPONSE_WITH_OTHER_TEXT)]
	public void GIVEN_ResponseText_WHEN_Parsing_THEN_Details_Are_Parsed(string input, string expected)
	{
		var mapped = input.MapBadRequestContentString();
		mapped.Should().Be(expected);
	}

	private static void VerifyContains(string input, string key, string value)
	{
		var deserialized = DeserializeDictionary(input);
		deserialized[key].ToString().Should().Be(value);
	}

	private static void VerifyContains(string input, string key, IEnumerable<string> values)
	{
		var deserialized = DeserializeDictionary(input);
		var arrayElement = (JsonElement)deserialized[key];
		var entries = arrayElement.EnumerateArray().Select(x => x.ToString());
		entries.Should().BeEquivalentTo(values);
	}

	private static T Deserialize<T>(string input)
	{
		return JsonSerializer.Deserialize<T>(input)!;
	}

	private static Dictionary<string, object> DeserializeDictionary(string input)
	{
		return Deserialize<Dictionary<string, object>>(input);
	}
}