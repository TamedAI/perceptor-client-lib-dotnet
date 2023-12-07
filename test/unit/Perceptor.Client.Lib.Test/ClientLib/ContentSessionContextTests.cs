using FluentAssertions;
using Moq;
using Perceptor.Client.Lib.InternalModels;
using Perceptor.Client.Lib.Models;
using Perceptor.Client.Lib.Repository;
using Perceptor.Client.Lib.Services;

namespace Perceptor.Client.Lib.Test.ClientLib;

public class ContentSessionContextTests
{
	private readonly TaskLimiterService _taskLimiterService = new(10);

	private readonly Mock<IPerceptorRepository> _repositoryMock = new();

	[SetUp]
	public void Setup()
	{
	}

	private void SetupRepositoryMockResult(OneOf<PerceptorSuccessResult, PerceptorError> toReturn)
	{
		_repositoryMock
			.Setup(x => x.SendInstruction(It.IsAny<PerceptorRequestPayload>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(toReturn);
	}

	private ContentSessionContext CreateContextForData(InstructionContextData data)
	{
		return new ContentSessionContext(_taskLimiterService,
			_repositoryMock.Object,
			data);
	}

	[Test]
	public async Task Successful_Response_Is_Mapped()
	{
		const string expectedAnswer = "answer value";
		SetupRepositoryMockResult(new PerceptorSuccessResult(expectedAnswer));

		var context = CreateContextForData(InstructionContextData.ForImage("some_image_data"));

		var instruction = new Instruction("some question");
		var r = await context.ProcessInstructionRequest(PerceptorRequest.WithFlavor("original"),
			InstructionMethod.Question,
			new[]
			{
				instruction
			}, 
			Array.Empty<ClassificationEntry>(),
			CancellationToken.None);

		var responseModel = r[0];

		responseModel.Instruction.Should().Be(instruction);
		responseModel.Result.Use(successResult => { successResult.Answer.Should().Be(expectedAnswer); },
			error => Assert.Fail(error.ErrorText));
	}

	[Test]
	public async Task WHEN_InsufficientClassEntries_THEN_ExceptionIsThrown()
	{
		SetupRepositoryMockResult(new PerceptorSuccessResult("not relevant"));

		var context = CreateContextForData(InstructionContextData.ForImage("some_image_data"));

		
		Func<Task> toCall = async ()=> await context.ProcessInstructionRequest(PerceptorRequest.WithFlavor("original"),
			InstructionMethod.Classify,
			new[] { new Instruction("not relevant") }, 
			new []{new ClassificationEntry("single class")},
			CancellationToken.None);

		await toCall.Should().ThrowAsync<ArgumentException>();
	}

	[Test]
	public async Task Error_Response_Is_Mapped()
	{
		const string expectedErrorText = "some error text";
		SetupRepositoryMockResult(new PerceptorError(expectedErrorText));

		var sut = CreateContextForData(InstructionContextData.ForImage("some_image_data"));

		var instruction = new Instruction("some question");
		//var classes = new[] { "class1", "class2" }.Select(x => new ClassificationEntry(x)).ToArray();
		var r = await sut.ProcessInstructionRequest(PerceptorRequest.WithFlavor("original"),
			InstructionMethod.Question,
			new[]
			{
				instruction
			},
			Array.Empty<ClassificationEntry>(),
			CancellationToken.None);

		var responseModel = r[0];
		responseModel.Instruction.Should().Be(instruction);

		responseModel.Result.Use(_ => Assert.Fail("expected error"),
			error => error.ErrorText.Should().Be(expectedErrorText));
	}
}