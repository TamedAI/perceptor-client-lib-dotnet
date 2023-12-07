using System.Net;
using FluentAssertions;
using Moq;
using Perceptor.Client.Lib.Configuration;
using Perceptor.Client.Lib.InternalModels;
using Perceptor.Client.Lib.InternalModels.HttpClient;
using Perceptor.Client.Lib.Models;
using Perceptor.Client.Lib.Repository;
using Perceptor.Client.Lib.Repository.HttpClient;

namespace Perceptor.Client.Lib.Test.ClientLib.Repository;

public class PerceptorHttpRepositoryTests
{
	private Mock<IHttpClientFacade> _httpFacadeMock = null!;
	private PerceptorHttpRepository _sut = null!;
	private PerceptorRepositoryHttpClientSettings _settings = null!;

	[SetUp]
	public void Setup()
	{
		_settings = new PerceptorRepositoryHttpClientSettings("someApiKey",
			"request_url",
			new WaitTimeOut(TimeSpan.FromSeconds(10)));

		_httpFacadeMock = new Mock<IHttpClientFacade>();
		_sut = new PerceptorHttpRepository(_settings,
			_httpFacadeMock.Object);
	}

	private static IEnumerable<object?[]> _testCaseDataErrorHttpCodes = new[]
	{
		new object?[] { HttpStatusCode.Forbidden, null, ErrorConstants.InvalidApiKey.ErrorText },
		new object?[] { HttpStatusCode.BadRequest, "input data error", "input data error" },
		new object?[] { HttpStatusCode.InternalServerError, "unknown error", ErrorConstants.UnknownError.ErrorText },
	};

	private static readonly PerceptorRequestPayload _notRelevantRequestPayload = new(PerceptorRequest.WithFlavor("original"),
		InstructionMethod.Question,
		InstructionContextData.ForText("not relevant"),
		new Instruction("some instruction"), 
		Array.Empty<ClassificationEntry>());

	[TestCaseSource(nameof(_testCaseDataErrorHttpCodes))]
	public async Task WHEN_HttpError_THEN_HttpResponse_Mapped_Correctly(HttpStatusCode statusCode, string? responseText, string errorText)
	{
		var expectedResult = new PerceptorError(errorText);
		SetupHttpReturns(statusCode, responseText);

		var responseModel = await _sut.SendInstruction(_notRelevantRequestPayload, CancellationToken.None);

		responseModel.Use(
			_ => Assert.Fail("expected error"),
			perceptorError => perceptorError.Should().Be(expectedResult)
		);
	}

	private static readonly IEnumerable<object[]> _allMethods =
		Enum.GetValues<InstructionMethod>()
			.Select(x => new object[] { x })
			.ToArray();
	
	[TestCaseSource(nameof(_allMethods))]
	public async Task GIVEN_Method_WHEN_HttpSuccess_THEN_ResponseIsPresent(int method)
	{
		string expectedAnswer = "some_answer";
		SetupHttpReturns(HttpStatusCode.OK, $"event: finished\ndata: {expectedAnswer}");
		PerceptorRequestPayload payload = new(PerceptorRequest.WithFlavor("original"),
			(InstructionMethod)method,
			InstructionContextData.ForText("not relevant"),
			new Instruction("some instruction"),
			new ClassificationEntry[]
			{
				new("class1"), new("class2")
			});
		
		var responseModel = await _sut.SendInstruction(_notRelevantRequestPayload, CancellationToken.None);
		
		responseModel.Use(
			success => success.Answer.Should().Be(expectedAnswer),
			perceptorError => Assert.Fail(perceptorError.ErrorText)
		);
		
	}

	[TestCase("first", "second", "first/second")]
	[TestCase("first/", "second", "first/second")]
	[TestCase("first/", "/second", "first/second")]
	[TestCase("first", "/second", "first/second")]
	[TestCase("", "/second", "/second")]
	[TestCase(null, "/second", "/second")]
	[TestCase("", "second", "second")]
	[TestCase("first", "", "first")]
	[TestCase("first", null, "first")]
	[TestCase(null, "", "")]
	public void GIVEN_UrlParts_WHEN_Merging_THEN_PathIsCorrect(string? firstPart, string? secondPart, string expected)
	{
		var result = PerceptorHttpRepository.ConcatUrl(firstPart, secondPart);
		result.Should().Be(expected);
	}


	private void SetupHttpReturns(HttpStatusCode statusCode, string? content)
	{
		var respMessage = new HttpResponseMessage(statusCode);
		if (content != null)
		{
			respMessage.Content = new StringContent(content);
		}

		_httpFacadeMock.Setup(x => x.SendMessage(It.IsAny<Func<HttpRequestMessage>>(),
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ClientHttpMessageResponse(respMessage));
	}
}