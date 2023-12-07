# perceptor-client-lib-dotnet

## Usage in a .net project

Add following dependency (via cli or IDE): "Perceptor.Client"

To create a client instance:

```csharp
ClientSettings clientSettings = new ClientSettings("your_api_key", "your_api_url")
{
	WaitTimeout = TimeSpan.FromSeconds(30),
	RetryCount = 2,
	MaximalNumberOfParallelRequests = 2
};

var client = PerceptorClientFactory.CreateFromSettings(clientSettings);
```

(Replace "your_api_key" and "your_api_url" with your provided values)

### Creating request

Use method _PerceptorRequest.Factory.withFlavor_ to create a request object without additional parameters.
You have to specify the flavor name and binary flag whether the scores are to be calculated or not.
To specify additional parameters use the constructor of _PerceptorRequest_.

Example code to create a client instance and send a instruction for a text:

```csharp
const string textToProcess = @"
Ich melde einen Schaden für meinen Kunden Hans Mustermann. Er hatte einen Schaden durch eine Überschwemmung. 
Er hat Rechnungen in Höhe von 150000 Euro eingereicht. Der Schaden soll in 2 Chargen bezahlt werden. 
Seine  IBAN ist DE02300606010002474689. Versicherungsbeginn war der 01.10.2022. Er ist abgesichert bis 750.000 EUR. Der Ablauf der Versicherung ist der 01.10.2026. 
Der Kunde hat VIP-Kennzeichen und hatte schonmal einen Leitungswasserschaden in Höhe von 3840 Euro. 
Der Kunde möchte eine Antwort heute oder morgen erhalten. 
Der Schaden ist 2021 aufgetreten. Die Anschrift des Kunden ist: Leipzigerstr. 12, 21390 Bonn.
Für Rückfragen möchte ich per Telefon kontaktiert werden. Es ist eine dringende Angelegenheit.
Meine Vermittlernumer ist die 090.100.
";

string[] instructions = {
	"Vorname und Nachname des Kunden?",
	"Ist der Kunde ein VIP? (Ja oder nein)"
};
var result = await client.AskText(textToProcess,
	PerceptorRequest.WithFlavor("original").WithReturnScores(), 
	instructions);

var replies = string.Join(Environment.NewLine, result.Select(r =>
{
	if (r.IsSuccess)
	{
		return $"instruction: {r.InstructionText}, result: {r.Response["text"]}";
	}

	return $"instruction: {r.InstructionText}, result: {r.ErrorText}";
}));
```

Example code to create a client instance and send a classify instruction for an image:

```csharp
		var imagePath = "path_to_image";
		var result = await client.ClassifyImage(imagePath,
			PerceptorRequest.WithFlavor("original").WithReturnScores(),
			"Was ist das für ein Dokument?",
			new []{"Rechnung", "Antrag", "Rezept"}
		);
```

### Reading responses

Basic class containing the processing result is
_InstructionWithResult_ ([see here](src/Perceptor.Client.Lib/Models/InstructionWithResult.cs)).

It contains following properties:<br>
_instruction_ contains the original instruction text<br>
_isSuccess_  set to True if the query was successful<br>
_response_ is a map/dictionary containing at least "text" element (with actual response text) and may contain additional
values (for example scores).<br>
_errorText_ error text (if error occurred)<br>

Following methods return the list of _InstructionWithResult_ instances:<br>
_askText_<br>
_askImage_<br>

Following method(s) return single _InstructionWithResult_ instance:<br>
_askTableFromImage_<br>
_classifyText_<br>
_classifyImage_<br>

Following methods query multiple images (document images), hence return the list of [
_DocumentImageResult_](src/Perceptor.Client.Lib/Models/DocumentImageResult.cs) instances, containing,
beside the _InstructionWithResult_ list, also the original page info:<br>
_askDocumentImagePaths_<br>
_askDocumentImageStreams_<br>
_askDocumentImageBytes_<br>

## Mapping response

If you use the methods returning the list of _DocumentImageResult_ and need to have the responses grouped by instruction
rather than page, you can use the provided utility extension method (_Utils.GroupByInstruction_) to map the response:

```csharp
		IReadOnlyList<DocumentImageResult>? results = await _sut.AskDocumentImages(imagePaths,
			PerceptorRequest.WithFlavor("original").WithReturnScores(),
			new[]
			{
				"What is the invoice number?",
				"What is the invoice date?",
				"To whom is the invoice billed?",
			}
		);

		IReadOnlyList<InstructionWithPageResult> groupedResults = results.GroupByInstruction();
        
        foreach (InstructionWithPageResult instructionWithPageResult in groupedResults)
		{
			foreach (DocumentPageWithResult pageResult in instructionWithPageResult.PageResults)
			{
				Console.WriteLine($"Instruction: {instructionWithPageResult.InstructionText}");
				if (pageResult.IsSuccess)
				{
					Console.WriteLine($"page: {pageResult.PageIndex}, response: {pageResult.Response}");
				}
				else
				{
					Console.WriteLine($"page: {pageResult.PageIndex}, error: {pageResult.ErrorText}");
				}	
			}
		}
```

