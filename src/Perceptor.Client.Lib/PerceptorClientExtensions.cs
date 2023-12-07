using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Perceptor.Client.Lib.InputMapping;
using Perceptor.Client.Lib.InternalModels;
using Perceptor.Client.Lib.Models;

namespace Perceptor.Client.Lib
{
	public static class PerceptorClientExtensions
	{

		/// <summary>
		/// Sends instructions for the specified <paramref name="textToAsk"/>
		/// </summary>
		/// <param name="client"></param>
		/// <param name="textToAsk">Text to be processed</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instructions">List of instructions to perform</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>List of <see cref="InstructionWithResult"/> containing instructions and their answers</returns>
		public static Task<IReadOnlyList<InstructionWithResult>> AskText(this PerceptorClient client,
			string textToAsk,
			PerceptorRequest request,
			IEnumerable<string> instructions,
			CancellationToken cancellationToken = default) =>
			client.AskQuestionInContext(InstructionContextData.ForText(textToAsk),
				request,
				instructions,
				cancellationToken);

		/// <summary>
		/// Sends classify instruction for the specified <paramref name="textToAsk"/>
		/// </summary>
		/// <param name="client"></param>
		/// <param name="textToAsk">Text to be processed</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instruction">instruction to perform</param>
		/// <param name="classes">list of classes ("document", "invoice" etc.)</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns><see cref="InstructionWithResult"/> containing instruction and response</returns>
		public static Task<InstructionWithResult> ClassifyText(this PerceptorClient client,
			string textToAsk,
			PerceptorRequest request,
			string instruction,
			IEnumerable<string> classes,
			CancellationToken cancellationToken = default) =>
			client.AskClassifyInContext(InstructionContextData.ForText(textToAsk),
				request,
				instruction,
				classes,
				cancellationToken);
		
		

		/// <summary>
		/// Sends instructions for an image with specified <paramref name="imageFilePath"/>.
		/// Supported image file types are: png, jpg, jpeg.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imageFilePath">Path to the image file</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instructions">List of instructions to perform</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>List of <see cref="InstructionWithResult"/> containing instructions and their answers</returns>
		public static async Task<IReadOnlyList<InstructionWithResult>> AskImage(this PerceptorClient client, string imageFilePath,
			PerceptorRequest request,
			IEnumerable<string> instructions,
			CancellationToken cancellationToken = default) =>
			await client.AskQuestionInContext(InstructionContextImageMapper.MapFromFile(imageFilePath), request, instructions, cancellationToken);

		/// <summary>
		/// Sends classify instruction for the specified <paramref name="imageFilePath"/>
		/// Supported image file types are: png, jpg, jpeg.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imageFilePath">Path to the image file</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instruction">instruction to perform</param>
		/// <param name="classes">list of classes ("document", "invoice" etc.)</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns><see cref="InstructionWithResult"/> containing instruction and response</returns>
		public static async Task<InstructionWithResult> ClassifyImage(this PerceptorClient client, string imageFilePath,
			PerceptorRequest request,
			string instruction,
			IEnumerable<string> classes,
			CancellationToken cancellationToken = default) =>
			await client.AskClassifyInContext(InstructionContextImageMapper.MapFromFile(imageFilePath), request, instruction, classes, cancellationToken);

		/// <summary>
		/// Sends instructions for an image with specified <paramref name="imageStream"/>.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imageStream">Stream containing the image</param>
		/// <param name="fileType">File type. Possible values are: "png", "jpg", "jpeg"</param>
		/// <param name="request">Detailed request parameters</param>
		/// <param name="instructions">List of instructions to perform</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>List of <see cref="InstructionWithResult"/> containing instructions and their answers</returns>
		public static async Task<IReadOnlyList<InstructionWithResult>> AskImage(this PerceptorClient client,
			Stream imageStream,
			string fileType,
			PerceptorRequest request,
			IEnumerable<string> instructions,
			CancellationToken cancellationToken = default) =>
			await client.AskQuestionInContext(await InstructionContextImageMapper.MapFromStream(imageStream, fileType), request, instructions, cancellationToken);

		/// <summary>
		/// Sends classify instruction for the specified <paramref name="imageStream"/>
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imageStream">Stream containing the image</param>
		/// <param name="fileType">File type. Possible values are: "png", "jpg", "jpeg"</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instruction">instruction to perform</param>
		/// <param name="classes">list of classes ("document", "invoice" etc.)</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns><see cref="InstructionWithResult"/> containing instruction and response</returns>
		public static async Task<InstructionWithResult> ClassifyImage(this PerceptorClient client, Stream imageStream,
			string fileType,
			PerceptorRequest request,
			string instruction,
			IEnumerable<string> classes,
			CancellationToken cancellationToken = default) =>
			await client.AskClassifyInContext(await InstructionContextImageMapper.MapFromStream(imageStream, fileType),
				request, instruction, classes, cancellationToken);

		/// <summary>
		/// Sends instructions for an image with specified <paramref name="fileBytes"/>.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="fileBytes">Image contents bytes</param>
		/// <param name="fileType">File type. Possible values are: "png", "jpg", "jpeg"</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instructions">List of instructions to perform</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>List of <see cref="InstructionWithResult"/> containing instructions and their answers</returns>
		public static async Task<IReadOnlyList<InstructionWithResult>> AskImage(this PerceptorClient client,
			byte[] fileBytes,
			string fileType,
			PerceptorRequest request,
			IEnumerable<string> instructions,
			CancellationToken cancellationToken = default) =>
			await client.AskQuestionInContext(InstructionContextImageMapper.MapFromBytes(fileBytes, fileType), request, instructions, cancellationToken);

		/// <summary>
		/// Sends classify instruction for the specified <paramref name="fileBytes"/>
		/// </summary>
		/// <param name="client"></param>
		/// <param name="fileBytes">Image contents bytes</param>
		/// <param name="fileType">File type. Possible values are: "png", "jpg", "jpeg"</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instruction">instruction to perform</param>
		/// <param name="classes">list of classes ("document", "invoice" etc.)</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns><see cref="InstructionWithResult"/> containing instruction and response</returns>
		public static async Task<InstructionWithResult> ClassifyImage(this PerceptorClient client, 
			byte[] fileBytes,
			string fileType,
			PerceptorRequest request,
			string instruction,
			IEnumerable<string> classes,
			CancellationToken cancellationToken = default) =>
			await client.AskClassifyInContext(InstructionContextImageMapper.MapFromBytes(fileBytes, fileType),
				request, instruction, classes, cancellationToken);

		/// <summary>
		/// Sends a table instruction for the specified image
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imageFilePath">image path</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instruction">instruction to perform, for example 'GENERATE TABLE Article, Amount, Value GUIDED BY Value'</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Instance of<see cref="InstructionWithResult"/> instructions and answer/error info.</returns>
		public static Task<InstructionWithResult> AskTableFromImage(this PerceptorClient client,
			string imageFilePath,
			PerceptorRequest request,
			string instruction,
			CancellationToken cancellationToken = default) =>
			client.AskTableInContext(InstructionContextImageMapper.MapFromFile(imageFilePath),
				request,
				instruction,
				cancellationToken
			);

		/// <summary>
		/// Sends a table instruction for the specified image.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imageStream">image stream</param>
		/// <param name="fileType">image file type (e.g. "png", "jpg")</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instruction">instruction to perform, for example 'GENERATE TABLE Article, Amount, Value GUIDED BY Value'</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Instance of<see cref="InstructionWithResult"/> instructions and answer/error info.</returns>
		public static async Task<InstructionWithResult> AskTableFromImage(this PerceptorClient client,
			Stream imageStream,
			string fileType,
			PerceptorRequest request,
			string instruction,
			CancellationToken cancellationToken = default)
		{
			InstructionContextData imageCtxData = await InstructionContextImageMapper.MapFromStream(imageStream, fileType);
			return await client.AskTableInContext(imageCtxData,
				request,
				instruction,
				cancellationToken
			);
		}

		
		/// <summary>
		/// Sends a table instruction for the specified image.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imageBytes">image bytes</param>
		/// <param name="fileType">image file type (e.g. "png", "jpg")</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instruction">instruction to perform, for example 'GENERATE TABLE Article, Amount, Value GUIDED BY Value'</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Instance of<see cref="InstructionWithResult"/> instructions and answer/error info.</returns>
		public static Task<InstructionWithResult> AskTableFromImage(this PerceptorClient client,
			byte[] imageBytes,
			string fileType,
			PerceptorRequest request,
			string instruction,
			CancellationToken cancellationToken = default) =>
			client.AskTableInContext(InstructionContextImageMapper.MapFromBytes(imageBytes, fileType),
				request,
				instruction,
				cancellationToken
			);

	}
}