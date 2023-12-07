using FluentAssertions;
using Perceptor.Client.Lib.Models;

namespace Perceptor.Client.Lib.Test.ClientLib;

[TestFixture]
public class UtilsTests
{
	[Test]
	public void GIVEN_EmptyList_WHEN_Grouped_THEN_Empty_List_Returned()
	{
		DocumentImageResult[] emptyInput = Array.Empty<DocumentImageResult>();
		IReadOnlyList<InstructionWithPageResult> mapped = emptyInput.GroupByInstruction();
		mapped.Should().BeEmpty();
	}
	
	[Test]
	public void GIVEN_MultiplePagesResponse_WHEN_Grouped_THEN_Instructions_Match()
	{
		var instructions = new[]
		{
			"inst_1",
			"inst_2",
			"inst_3",
			"inst_4",
		};
		var toMap = Enumerable.Range(1, 5)
				.Select(i => CreateDocumentImageResult(i, instructions))
				.ToList();

		var mapped = Utils.GroupByInstruction(toMap);

		mapped.Should().HaveSameCount(instructions);
		mapped.Select(x => x.InstructionText).Should().BeEquivalentTo(instructions,
			o => o.WithStrictOrdering());
	}
	
	[Test]
	public void GIVEN_MultiplePagesResponse_WHEN_Grouped_THEN_Responses_Match()
	{
		var instructions = new[]
		{
			"inst_1",
			"inst_2",
			"inst_3",
			"inst_4",
			"inst_5"
		};
		var toMap = Enumerable.Range(1, 5)
			.Select(i => CreateDocumentImageResult(i, instructions))
			.ToList();

		var allResponses = toMap.SelectMany(x => x.Results);
		
		var mapped = Utils.GroupByInstruction(toMap);
		mapped.Should().AllSatisfy(m =>
		{
			m.PageResults.Should().HaveSameCount(toMap);

			m.PageResults.Should().BeInAscendingOrder(x => x.PageIndex);
			
			InstructionWithResult[] responsesToCompare = allResponses.Where(x => x.InstructionText == m.InstructionText).ToArray();
			
			m.PageResults.Select(x => x.IsSuccess).Should().BeEquivalentTo(
				responsesToCompare.Select(r => r.IsSuccess));
			
			m.PageResults.Select(x => x.Response).Should().BeEquivalentTo(
				responsesToCompare.Select(r => r.Response));
			
			m.PageResults.Select(x => x.ErrorText).Should().BeEquivalentTo(
				responsesToCompare.Select(r => r.ErrorText));
		});
		
	}

	private static InstructionWithResult CreateInstructionWithResult(string instructionText, int pageIndex) =>
		pageIndex % 2 == 0
			? InstructionWithResult.Success(instructionText,
				StructuredResponseParser.ParseStructuredText($"resp_{instructionText}_page_{pageIndex}"))
			: InstructionWithResult.Error(instructionText, $"error_{instructionText}_page_{pageIndex}");

	private static DocumentImageResult CreateDocumentImageResult(int pageIndex, IEnumerable<string> instructions) =>
		new(pageIndex,
			instructions.Select(i => CreateInstructionWithResult(i, pageIndex)).ToList()
			);
}