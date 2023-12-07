using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Perceptor.Client.Lib.Configuration;
using Perceptor.Client.Lib.Models;
using Perceptor.Client.Lib.Services;
// ReSharper disable StringLiteralTypo

namespace Perceptor.Client.Lib.Test.ClientLib;

[Ignore("Integration tests, only to run in dev mode")]
public class ClientIntegrationTests
{
	private PerceptorClient _sut = null!;

	[SetUp]
	public void Setup()
	{
		IConfigurationBuilder builder = new ConfigurationBuilder()
			.AddJsonFile("client-config.json");

		var config = builder.Build();

		var apiKey = config.GetSection("TAI_PERCEPTOR_API_KEY").Value;
		var apiUrl = config.GetSection("TAI_PERCEPTOR_BASE_URL").Value;
		
		
		var settings = new ClientSettings(apiKey, apiUrl)
		{
			WaitTimeout = TimeSpan.FromSeconds(30),
			RetryCount = 2,
			MaximalNumberOfParallelRequests = 2
		};
		_sut = PerceptorClientFactory.CreateFromSettings(settings);
	}

	private const string _TEXT_TO_PROCESS = @"
Ich melde einen Schaden für meinen Kunden Hans Mustermann. Er hatte einen Schaden durch eine Überschwemmung. 
Er hat Rechnungen in Höhe von 150000 Euro eingereicht. Der Schaden soll in 2 Chargen bezahlt werden. 
Seine  IBAN ist DE02300606010002474689. Versicherungsbeginn war der 01.10.2022. Er ist abgesichert bis 750.000 EUR. Der Ablauf der Versicherung ist der 01.10.2026. 
Der Kunde hat VIP-Kennzeichen und hatte schonmal einen Leitungswasserschaden in Höhe von 3840 Euro. 
Der Kunde möchte eine Antwort heute oder morgen erhalten. 
Der Schaden ist 2021 aufgetreten. Die Anschrift des Kunden ist: Leipzigerstr. 12, 21390 Bonn.
Für Rückfragen möchte ich per Telefon kontaktiert werden. Es ist eine dringende Angelegenheit.
Meine Vermittlernumer ist die 090.100.
";

	[Test]
	public async Task AskText()
	{
		string[] instructions = {
			"Vorname und Nachname des Kunden?",
			"Ist der Kunde ein VIP? (Ja oder nein)",
			"was ist die IBAN?",
			"Wie hoch sind seine Rechnungen?",
			"Ist er abgesichert?",
			"wann läuft die Versicherung ab?",
			"wie wiele Chargen?",
			"wie ist der Schaden entstanden?",
			"wie lautet die Anschrift?",
			"die Vermittlernummer?",
			"hatte er schon mal Schaden?",
			"wann will der Kunde Antwort?",
			"wie soll ich kontaktiert werden?",
			"ist es dringend?"
		};

		var results = await _sut.AskText(_TEXT_TO_PROCESS,
			PerceptorRequest.WithFlavor("original").WithReturnScores(), instructions);

		results.Should().HaveSameCount(instructions);
		
		LogAndAssertResponse(results);
	}

	[Test]
	public async Task ClassifyText()
	{
		InstructionWithResult? result = await _sut.ClassifyText(_TEXT_TO_PROCESS,
			PerceptorRequest.WithFlavor("original").WithReturnScores(),
			"was ist das für ein Text?",
			new [] {
				"versicherung",
				"Schadenmeldung",
				"letter",
				"brief"
			}
			);

		LogAndAssertResponse(result);
	}

	private static void LogAndAssertResponse(InstructionWithResult result)
	{
		LogText(result.IsSuccess
			? $"Instruction: '{result.InstructionText}', response: '{DumpResponseDictionary(result.Response)}'"
			: $"Instruction: '{result.InstructionText}', error response: '{result.ErrorText}'");

		result.IsSuccess.Should().BeTrue(because: result.ErrorText);
	}

	[Test]
	public async Task AskImage()
	{
		var imagePath = TestHelperMethods.GetTestFilePath("invoice.jpg");
		var results = await _sut.AskImage(imagePath,
			PerceptorRequest.WithFlavor("original").WithReturnScores(),
			new[]
			{
				"What is the invoice number?",
				"What is the invoice date?",
				"To whom is the invoice billed?",
			}
		);
		
		LogAndAssertResponse(results);
	}

	[Test]
	public async Task ClassifyImage()
	{
		var imagePath = TestHelperMethods.GetTestFilePath("invoice.jpg");
		var results = await _sut.ClassifyImage(imagePath,
			PerceptorRequest.WithFlavor("original").WithReturnScores(),
			"Was ist das für ein Dokument?",
			new []{"Rechnung", "Antrag", "Rezept"}
		);
		
		LogAndAssertResponse(results);
	}
	
	[Test]
	public async Task ClassifyDocumentImages()
	{
		var imagePath = TestHelperMethods.GetTestFilePath("invoice.jpg");
		var results = await _sut.ClassifyDocumentImages(new[]{imagePath, imagePath},
			PerceptorRequest.WithFlavor("original").WithReturnScores(),
			"Was ist das für ein Dokument?",
			new []{"Rechnung", "Antrag", "Rezept"}
		);
		
		LogAndAssertResponse(results);
	}

	private static void LogAndAssertResponse(IReadOnlyList<InstructionWithResult> results)
	{
		foreach (InstructionWithResult singleResult in results)
		{
			LogText(singleResult.IsSuccess
				? $"Instruction: '{singleResult.InstructionText}', response: '{DumpResponseDictionary(singleResult.Response)}'"
				: $"Instruction: '{singleResult.InstructionText}', error response: '{singleResult.ErrorText}'");
		}

		results.Should().AllSatisfy(r => r.IsSuccess.Should().BeTrue(because: r.ErrorText));
	}

	private static void LogAndAssertResponse(IReadOnlyList<DocumentImageResult> results)
	{
		foreach (DocumentImageResult pageResult in results)
		{
			foreach (InstructionWithResult singleResult in pageResult.Results)
			{
				LogText(singleResult.IsSuccess
					? $"Instruction: '{singleResult.InstructionText}', response: '{DumpResponseDictionary(singleResult.Response)}'"
					: $"Instruction: '{singleResult.InstructionText}', error response: '{singleResult.ErrorText}'");				
			}
		}
		
		results.SelectMany(x=>x.Results).Should().AllSatisfy(r => r.IsSuccess.Should().BeTrue(because: r.ErrorText));
	}

	private static void LogText(string toLog)
	{
		TestContext.WriteLine(toLog);
	}

	private static string DumpResponseDictionary(IReadOnlyDictionary<string, object> input)
	{
		return SerializationService.Serialize(input);
	}
	
}