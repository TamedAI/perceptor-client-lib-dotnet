using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using Perceptor.Client.Lib.Configuration;
using Perceptor.Client.Lib.InternalModels;
using Perceptor.Client.Lib.Models;
using Perceptor.Client.Lib.Repository;
using FluentAssertions;

namespace Perceptor.Client.Lib.DotNetFrmwk.Test
{
	[TestFixture]
	public class ClientTests
	{
		private Mock<IPerceptorRepository> _repositoryMock;
		private PerceptorClient _sut;
		private Fixture _fixture;

		[SetUp]
		public void Setup()
		{
			_repositoryMock = new Mock<IPerceptorRepository>();

			_sut = PerceptorClientFactory.CreateForRepository(
				new ClientSettings("key",  "some_url"){ WaitTimeout = TimeSpan.FromSeconds(40) },
				_repositoryMock.Object);

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

			IReadOnlyList<InstructionWithResult> res = await _sut.AskText("some test",
				request, instructions);

			res.Should().HaveCount(instructions.Length);

			_repositoryMock.Verify(x => x.SendInstruction(It.Is<PerceptorRequestPayload>(p => p.Request == request),
				It.IsAny<CancellationToken>()), Times.Exactly(instructions.Length));
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
			IReadOnlyList<InstructionWithResult> res = await _sut.AskImage(
				TestHelperMethods.GetTestFilePath("binary_file.png"),
				request,
				instructions);

			res.Should().HaveCount(instructions.Length);
			_repositoryMock.Verify(x => x.SendInstruction(It.Is<PerceptorRequestPayload>(p => p.Request == request),
				It.IsAny<CancellationToken>()), Times.Exactly(instructions.Length));
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
			using (FileStream imageStream = File.OpenRead(TestHelperMethods.GetTestFilePath("binary_file.png")))
			{
				IReadOnlyList<InstructionWithResult> res = await _sut.AskImage(imageStream, "png",
					request,
					instructions);

				res.Should().HaveCount(instructions.Length);
				VerifyMockCalled(request, InstructionMethod.Question, Times.Exactly(instructions.Length));
			}
		}
		
	
		[Test]
		public async Task WITH_CustomParameters_WHEN_AskTableFromImageStream_THEN_Is_Correct()
		{
			var expectedSuccessResult = new PerceptorSuccessResult("some result");
			SetupRepositoryResult(expectedSuccessResult);

			const string tableInstruction = "generate table";
			var request = CreatePerceptorRequest();
			using (FileStream imageStream = File.OpenRead(TestHelperMethods.GetTestFilePath("binary_file.png")))
			{
				InstructionWithResult res = await _sut.AskTableFromImage(imageStream, "png",
					request,
					"generate table");
				
				res.InstructionText.Should().Be(tableInstruction);
				AssertResponseText(res, expectedSuccessResult.Answer);
				
				VerifyMockCalled(request, InstructionMethod.Table, Times.Once());
			}
		}


		private void SetupRepositoryResult(OneOf<PerceptorSuccessResult, PerceptorError> toReturn)
		{
			_repositoryMock.Setup(x => x.SendInstruction(It.IsAny<PerceptorRequestPayload>(),
					It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(toReturn));
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
					p.Request == requestParameters && p.Method==method),
				It.IsAny<CancellationToken>()), times);
		}
		
		private static void AssertResponseText(InstructionWithResult instructionWithResult, string expected)
		{
			instructionWithResult.Response.Should().ContainKey("text");
			instructionWithResult.Response["text"].Should().Be(expected);
		}
	}
}