using AutoFixture;
using FluentAssertions;
using Moq;
using Perceptor.Client.Lib.Configuration;
using Perceptor.Client.Lib.InternalModels;
using Perceptor.Client.Lib.Models;
using Perceptor.Client.Lib.Repository;

namespace Perceptor.Client.Lib.Test.ClientLib;

public class ClientTests
{
	private Mock<IPerceptorRepository> _repositoryMock = null!;
	private PerceptorClient _sut = null!;
	private Fixture _fixture = null!;

	[SetUp]
	public void Setup()
	{
		_repositoryMock = new Mock<IPerceptorRepository>();
		var s = new ClientSettings( "key", "some_url"){WaitTimeout = TimeSpan.FromSeconds(40) };

		_sut = PerceptorClientFactory.CreateForRepository(s, _repositoryMock.Object);

		_fixture = new Fixture();
	}

	[Test]
	public async Task WITH_CustomParameters_When_AskText_THEN_Repository_Called_With_Parameters()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		string[] instructions =
		{
			"not relevant instructions",
			"another instruction"
		};
		PerceptorRequest request = CreatePerceptorRequest();

		IReadOnlyList<InstructionWithResult>? res = await _sut.AskText("some test",
			request, instructions);

		res.Should().HaveCount(instructions.Length);
		
		VerifyMockCalled(request, InstructionMethod.Question, Times.Exactly(instructions.Length));
	}


	[Test]
	public async Task WITH_CustomParameters_WHEN_AskDocumentImages_FromFiles_THEN_Is_Correct()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		PerceptorRequest request = CreatePerceptorRequest();
		string[] instructions =
		{
			"some instruction",
			"another instruction"
		};
		string[] imagePaths =
		{
			TestHelperMethods.GetTestFilePath("binary_file.png"),
			TestHelperMethods.GetTestFilePath("binary_file.png"),
		};
		IReadOnlyList<DocumentImageResult>? res = await _sut.AskDocumentImages(
			imagePaths,
			request,
			instructions);

		res.Should().HaveCount(imagePaths.Length);
		AssertAllPageIndexes(res, imagePaths.Length);

		res.Should().AllSatisfy(singleResult => { singleResult.Results.Should().HaveCount(instructions.Length); });
	}


	[Test]
	public async Task WITH_CustomParameters_WHEN_AskDocumentImages_FromBytes_THEN_Is_Correct()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		string[] instructions =
		{
			"some instruction",
			"another instruction"
		};
		PerceptorRequest request = CreatePerceptorRequest();
		(byte[], string)[] fileTuples =
		{
			(await File.ReadAllBytesAsync(TestHelperMethods.GetTestFilePath("binary_file.png")), "png"),
			(await File.ReadAllBytesAsync(TestHelperMethods.GetTestFilePath("binary_file.png")), "png"),
		};
		IReadOnlyList<DocumentImageResult>? res = await _sut.AskDocumentImages(
			fileTuples,
			request,
			instructions);

		res.Should().HaveCount(fileTuples.Length);
		AssertAllPageIndexes(res, fileTuples.Length);

		AssertAllAnswersAre(res, expectedSuccessResult.Answer);

		VerifyMockCalled(request, InstructionMethod.Question, Times.Exactly(fileTuples.Length * instructions.Length));
	}


	[Test]
	public async Task WITH_CustomParameters_WHEN_AskImage_FromFile_THEN_Repository_Called_With_Parameters()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		string[] instructions =
		{
			"instruction 1",
			"instruction 2",
		};
		PerceptorRequest request = CreatePerceptorRequest();
		IReadOnlyList<InstructionWithResult>? res = await _sut.AskImage(
			TestHelperMethods.GetTestFilePath("binary_file.png"),
			request,
			instructions);

		res.Should().HaveCount(instructions.Length);
		
		VerifyMockCalled(request, InstructionMethod.Question, Times.Exactly(instructions.Length));
	}


	[Test]
	public async Task WITH_CustomParameters_WHEN_AskImage_FromStream_THEN_Repository_Called_With_Parameters()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		string[] instructions =
		{
			"not relevant instructions"
		};

		PerceptorRequest request = CreatePerceptorRequest();
		await using FileStream imageStream = File.OpenRead(TestHelperMethods.GetTestFilePath("binary_file.png"));
		IReadOnlyList<InstructionWithResult>? res = await _sut.AskImage(imageStream, "png",
			request,
			instructions);

		res.Should().HaveCount(instructions.Length);
		
		VerifyMockCalled(request, InstructionMethod.Question, Times.Exactly(instructions.Length));
	}

	[Test]
	public async Task WITH_CustomParameters_WHEN_AskImage_FromBytes_THEN_Repository_Called_With_Parameters()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		string[] instructions =
		{
			"not relevant instructions"
		};

		PerceptorRequest request = CreatePerceptorRequest();
		byte[] imageBytes = await File.ReadAllBytesAsync(TestHelperMethods.GetTestFilePath("binary_file.png"));
		IReadOnlyList<InstructionWithResult>? res = await _sut.AskImage(imageBytes, "png",
			request,
			instructions);

		res.Should().HaveCount(instructions.Length);
		
		VerifyMockCalled(request, InstructionMethod.Question, Times.Exactly(instructions.Length));
	}


	[Test]
	public async Task WITH_CustomParameters_WHEN_AskDocumentImages_FromStreams_THEN_Is_Correct()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		string[] instructions =
		{
			"some instruction",
			"another instruction"
		};
		PerceptorRequest request = CreatePerceptorRequest();
		(Stream, string)[] fileTuples =
		{
			(File.OpenRead(TestHelperMethods.GetTestFilePath("binary_file.png")), "png"),
			(File.OpenRead(TestHelperMethods.GetTestFilePath("binary_file.png")), "png"),
		};
		IReadOnlyList<DocumentImageResult>? res = await _sut.AskDocumentImages(
			fileTuples,
			request,
			instructions);

		res.Should().HaveCount(fileTuples.Length);
		AssertAllPageIndexes(res, fileTuples.Length);
		AssertAllAnswersAre(res, expectedSuccessResult.Answer);
	}


	[Test]
	public async Task WITH_CustomParameters_WHEN_AskTableFromImageFile_THEN_Is_Correct()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		const string tableInstruction = "generate table";

		PerceptorRequest request = CreatePerceptorRequest();

		InstructionWithResult res = await _sut.AskTableFromImage(
			TestHelperMethods.GetTestFilePath("binary_file.png"),
			request,
			"generate table");

		res.InstructionText.Should().Be(tableInstruction);
		AssertResponseText(res, expectedSuccessResult.Answer);

		VerifyMockCalled(request, InstructionMethod.Table, Times.Once());
	}

	[Test]
	public async Task WITH_CustomParameters_WHEN_AskTableFromDocumentImageFiles_THEN_Is_Correct()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		PerceptorRequest request = CreatePerceptorRequest();
		const string tableInstruction = "generate table";

		string[] imageFiles =
		{
			TestHelperMethods.GetTestFilePath("binary_file.png"),
			TestHelperMethods.GetTestFilePath("binary_file.png")
		};

		IReadOnlyList<DocumentImageResult>? res = await _sut.AskTableFromDocumentImages(imageFiles,
			request,
			tableInstruction);

		res.Should().HaveCount(imageFiles.Length);

		AssertAllAnswersAre(res, expectedSuccessResult.Answer);
		VerifyMockCalled(request, InstructionMethod.Table, Times.Exactly(imageFiles.Length));
	}

	[Test]
	public async Task WITH_CustomParameters_WHEN_AskTableFromDocumentImageStreams_THEN_Is_Correct()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		PerceptorRequest request = CreatePerceptorRequest();
		const string tableInstruction = "generate table";
		await using Stream imageStream1 = File.OpenRead(TestHelperMethods.GetTestFilePath("binary_file.png"));
		await using Stream imageStream2 = File.OpenRead(TestHelperMethods.GetTestFilePath("binary_file.png"));

		(Stream, string)[] imageStreams = { (imageStream1, "png"), (imageStream2, "PNG") };

		IReadOnlyList<DocumentImageResult>? res = await _sut.AskTableFromDocumentImages(imageStreams,
			request,
			tableInstruction);

		res.Should().HaveCount(imageStreams.Length);

		AssertAllAnswersAre(res, expectedSuccessResult.Answer);
		VerifyMockCalled(request, InstructionMethod.Table, Times.Exactly(imageStreams.Length));
	}

	[Test]
	public async Task WITH_CustomParameters_WHEN_AskTableFromDocumentImageBytes_THEN_Is_Correct()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		PerceptorRequest request = CreatePerceptorRequest();
		const string tableInstruction = "generate table";
		byte[] imageBytes1 = await File.ReadAllBytesAsync(TestHelperMethods.GetTestFilePath("binary_file.png"));
		byte[] imageBytes2 = await File.ReadAllBytesAsync(TestHelperMethods.GetTestFilePath("binary_file.png"));

		(byte[], string)[] imageStreams = { (imageBytes1, "png"), (imageBytes2, "PNG") };

		IReadOnlyList<DocumentImageResult>? res = await _sut.AskTableFromDocumentImages(imageStreams,
			request,
			tableInstruction);

		res.Should().HaveCount(imageStreams.Length);

		AssertAllAnswersAre(res, expectedSuccessResult.Answer);
		VerifyMockCalled(request, InstructionMethod.Table, Times.Exactly(imageStreams.Length));
	}

	[Test]
	public async Task WITH_CustomParameters_WHEN_AskTableFromImageBytes_THEN_Is_Correct()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		const string tableInstruction = "generate table";
		PerceptorRequest request = CreatePerceptorRequest();
		byte[] imageBytes = await File.ReadAllBytesAsync(TestHelperMethods.GetTestFilePath("binary_file.png"));
		InstructionWithResult res = await _sut.AskTableFromImage(imageBytes, "png",
			request,
			"generate table");

		res.InstructionText.Should().Be(tableInstruction);
		AssertResponseText(res, expectedSuccessResult.Answer);

		VerifyMockCalled(request, InstructionMethod.Table, Times.Once());
	}

	[Test]
	public async Task WITH_CustomParameters_WHEN_AskTableFromImageStream_THEN_Is_Correct()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		const string tableInstruction = "generate table";
		PerceptorRequest request = CreatePerceptorRequest();
		await using FileStream imageStream = File.OpenRead(TestHelperMethods.GetTestFilePath("binary_file.png"));
		InstructionWithResult res = await _sut.AskTableFromImage(imageStream, "png",
			request,
			"generate table");

		res.InstructionText.Should().Be(tableInstruction);
		AssertResponseText(res, expectedSuccessResult.Answer);

		VerifyMockCalled(request, InstructionMethod.Table, Times.Once());
	}

	[Test]
	public async Task WITH_CustomParameters_When_ClassifyText_THEN_Repository_Called_With_Parameters()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		PerceptorRequest request = CreatePerceptorRequest();

		await _sut.ClassifyText("some test",
			request, "not relevant instruction",
			new[] { "class1", "class2" });

		VerifyMockCalled(request, InstructionMethod.Classify, Times.Once());
	}
	
	[Test]
	public async Task WITH_CustomParameters_WHEN_ClassifyImage_FromFile_THEN_Repository_Called_With_Parameters()
	{
		PerceptorRequest request = CreatePerceptorRequest();
		await GetClassifyImageMethod(request, TestHelperMethods.GetTestFilePath("binary_file.png"))();
		VerifyMockCalled(request, InstructionMethod.Classify, Times.Once());
	}
	
	[Test]
	public async Task WITH_CustomParameters_WHEN_ClassifyImage_FromStream_THEN_Repository_Called_With_Parameters()
	{
		await using Stream imageStream1 = File.OpenRead(TestHelperMethods.GetTestFilePath("binary_file.png"));
		PerceptorRequest request = CreatePerceptorRequest();
		await GetClassifyImageMethod(request, (imageStream1, "png") )();
		VerifyMockCalled(request, InstructionMethod.Classify, Times.Once());
	}
	
	[Test]
	public async Task WITH_CustomParameters_WHEN_ClassifyImage_FromBytes_THEN_Repository_Called_With_Parameters()
	{
		byte[] imageBytes1 = await File.ReadAllBytesAsync(TestHelperMethods.GetTestFilePath("binary_file.png"));
		PerceptorRequest request = CreatePerceptorRequest();
		await GetClassifyImageMethod(request, (imageBytes1, "png") )();
		VerifyMockCalled(request, InstructionMethod.Classify, Times.Once());
	}

	private Func<Task> GetClassifyImageMethod(PerceptorRequest request, object methodParams)
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);

		const string instruction = "not relevant instruction";
		var classes = new[] { "class1", "class2" };

		return methodParams switch
		{
			string filePath => () => _sut.ClassifyImage(
				filePath,
				request,
				instruction, classes),

			ValueTuple<Stream, string> fileStream => () => _sut.ClassifyImage(
				fileStream.Item1,
				fileStream.Item2,
				request,
				instruction, classes),
			
			ValueTuple<byte[], string> fileBytes => () => _sut.ClassifyImage(
				fileBytes.Item1,
				fileBytes.Item2,
				request,
				instruction, classes),

			_ => throw new ArgumentException("invalid parameter type")

		};
	}
	[Test]
	public async Task WITH_CustomParameters_WHEN_ClassifyDocumentImages_FromPaths_THEN_Is_Correct()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");

		PerceptorRequest request = CreatePerceptorRequest();

		var imagePaths = new[]
		{
			TestHelperMethods.GetTestFilePath("binary_file.png"),
			TestHelperMethods.GetTestFilePath("binary_file.png")
		};

		var res = await GetClassifyDocumentImagesMethod(request,expectedSuccessResult, imagePaths)();
		res.Should().HaveCount(imagePaths.Length);
		AssertAllPageIndexes(res, imagePaths.Length);
		AssertAllAnswersAre(res, expectedSuccessResult.Answer);
	}
	
	[Test]
	public async Task WITH_CustomParameters_WHEN_ClassifyDocumentImages_FromStreams_THEN_Is_Correct()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);
	
		PerceptorRequest request = CreatePerceptorRequest();
		(Stream, string)[] fileTuples =
		{
			(File.OpenRead(TestHelperMethods.GetTestFilePath("binary_file.png")), "png"),
			(File.OpenRead(TestHelperMethods.GetTestFilePath("binary_file.png")), "png"),
		};
	
		var res = await GetClassifyDocumentImagesMethod(request,expectedSuccessResult, fileTuples)();
	
		res.Should().HaveCount(fileTuples.Length);
		AssertAllPageIndexes(res, fileTuples.Length);
		AssertAllAnswersAre(res, expectedSuccessResult.Answer);
	}
	
	[Test]
	public async Task WITH_CustomParameters_WHEN_ClassifyDocumentImages_FromBytes_THEN_Is_Correct()
	{
		var expectedSuccessResult = new PerceptorSuccessResult("some result");
		SetupRepositoryResult(expectedSuccessResult);
	
		PerceptorRequest request = CreatePerceptorRequest();
		(byte[], string)[] fileTuples =
		{
			(await File.ReadAllBytesAsync(TestHelperMethods.GetTestFilePath("binary_file.png")), "png"),
			(await File.ReadAllBytesAsync(TestHelperMethods.GetTestFilePath("binary_file.png")), "png"),
		};
	
		var res = await GetClassifyDocumentImagesMethod(request,expectedSuccessResult, fileTuples)();
	
		res.Should().HaveCount(fileTuples.Length);
		AssertAllPageIndexes(res, fileTuples.Length);
		AssertAllAnswersAre(res, expectedSuccessResult.Answer);
	}
	
	private Func<Task<IReadOnlyList<DocumentImageResult>>> GetClassifyDocumentImagesMethod(PerceptorRequest request, 
		OneOf<PerceptorSuccessResult, PerceptorError> expectedResult,
		object methodParams)
	{
		SetupRepositoryResult(expectedResult);

		const string instruction = "not relevant instruction";
		var classes = new[] { "class1", "class2" };

		return methodParams switch
		{
			IEnumerable<string> filePaths => () => _sut.ClassifyDocumentImages(
				filePaths,
				request,
				instruction, classes),

			IEnumerable<ValueTuple<Stream, string>> fileStreams => () => _sut.ClassifyDocumentImages(
				fileStreams,
				request,
				instruction, classes),
			
			IEnumerable<ValueTuple<byte[], string>> fileBytes => () => _sut.ClassifyDocumentImages(
				fileBytes,
				request,
				instruction, classes),

			_ => throw new ArgumentException("invalid parameter type")

		};
	}

	
	private void SetupRepositoryResult(OneOf<PerceptorSuccessResult, PerceptorError> toReturn)
	{
		_repositoryMock.Setup(x => x.SendInstruction(It.IsAny<PerceptorRequestPayload>(),
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(toReturn);
	}

	private PerceptorRequest CreatePerceptorRequest()
	{
		return _fixture.Create<PerceptorRequest>();
	}

	private void VerifyMockCalled(PerceptorRequest requestParameters, InstructionMethod method,
		Times times)
	{
		_repositoryMock.Verify(x => x.SendInstruction(
			It.Is<PerceptorRequestPayload>(p =>
				p.Request == requestParameters && p.Method == method),
			It.IsAny<CancellationToken>()), times);
	}

	private static void AssertAllAnswersAre(IEnumerable<DocumentImageResult> collection, string expectedAnswer)
	{
		collection.Should().AllSatisfy(p =>
			p.Results.Should().AllSatisfy(r =>
				{
					r.IsSuccess.Should().BeTrue();
					AssertResponseText(r, expectedAnswer);
				}
			)
		);
	}


	private static void AssertAllPageIndexes(IEnumerable<DocumentImageResult> collection, int count)
	{
		collection.Select(x => x.PageIndex).Should().BeEquivalentTo(Enumerable.Range(0, count),
			o => o.WithStrictOrdering());
	}

	private static void AssertResponseText(InstructionWithResult instructionWithResult, string expected)
	{
		instructionWithResult.Response.Should().ContainKey("text");
		instructionWithResult.Response["text"].Should().Be(expected);
	}
}