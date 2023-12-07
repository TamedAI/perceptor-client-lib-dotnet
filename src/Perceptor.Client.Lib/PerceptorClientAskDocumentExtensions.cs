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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Perceptor.Client.Lib.InputMapping;
using Perceptor.Client.Lib.InternalModels;
using Perceptor.Client.Lib.Models;

namespace Perceptor.Client.Lib
{
	public static class PerceptorClientAskDocumentExtensions
	{
		/// <summary>
		/// Sends instructions for multiple images specified by <paramref name="imagePaths"/>.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imagePaths">Paths to image files</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instructions">List of instructions to perform</param>
		/// <param name="cancellationToken"></param>
		/// <returns>List of<see cref="DocumentImageResult"/> containing instructions and their answers.</returns>
		public static async Task<IReadOnlyList<DocumentImageResult>> AskDocumentImages(this PerceptorClient client,
			IEnumerable<string> imagePaths,
			PerceptorRequest request,
			IEnumerable<string> instructions,
			CancellationToken cancellationToken = default) =>
			await AskDocumentImages(client, request, instructions,
				InstructionContextImageMapper.MapFromFiles(imagePaths), cancellationToken);

		/// <summary>
		/// Sends classify instruction for multiple images specified by <paramref name="imagePaths"/>.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imagePaths">Paths to image files</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instruction">instruction to perform</param>
		/// <param name="classes">list of classes ("document", "invoice" etc.)</param>
		/// <param name="cancellationToken"></param>
		/// <returns>List of<see cref="DocumentImageResult"/> containing instructions and their answers.</returns>
		public static async Task<IReadOnlyList<DocumentImageResult>> ClassifyDocumentImages(this PerceptorClient client,
			IEnumerable<string> imagePaths,
			PerceptorRequest request,
			string instruction,
			IEnumerable<string> classes,
			CancellationToken cancellationToken = default) =>
			await ClassifyDocumentImages(client, request, instruction, classes,
				InstructionContextImageMapper.MapFromFiles(imagePaths), cancellationToken);

		private static Task<IReadOnlyList<DocumentImageResult>> AskDocumentImages(PerceptorClient client,
			PerceptorRequest request, IEnumerable<string> instructions,
			IEnumerable<InstructionContextData> dataContexts,
			CancellationToken cancellationToken) =>
			client.AskInMultipleContexts(dataContexts, request, InstructionMethod.Question, instructions,
				Enumerable.Empty<string>(),
				cancellationToken);

		private static Task<IReadOnlyList<DocumentImageResult>> ClassifyDocumentImages(PerceptorClient client,
			PerceptorRequest request,
			string instruction, IEnumerable<string> classes,
			IEnumerable<InstructionContextData> dataContexts,
			CancellationToken cancellationToken) =>
			client.AskInMultipleContexts(dataContexts, request, InstructionMethod.Classify,
				new[] { instruction },
				classes,
				cancellationToken);

		/// <summary>
		/// Sends instructions for multiple images specified by <paramref name="imageInfos"/>.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imageInfos">List of tuples containing image bytes and file type</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instructions">List of instructions to perform</param>
		/// <param name="cancellationToken"></param>
		/// <returns>List of<see cref="DocumentImageResult"/> containing instructions and their answers.</returns>
		public static async Task<IReadOnlyList<DocumentImageResult>> AskDocumentImages(this PerceptorClient client,
			IEnumerable<(byte[], string)> imageInfos,
			PerceptorRequest request,
			IEnumerable<string> instructions,
			CancellationToken cancellationToken = default)
		{
			IReadOnlyList<InstructionContextData> dataContexts = InstructionContextImageMapper.MapFromBytes(imageInfos);
			return await client.AskInMultipleContexts(dataContexts, request, InstructionMethod.Question, instructions,
				Enumerable.Empty<string>(), cancellationToken);
		}

		/// <summary>
		/// Sends classify instruction for multiple images specified by <paramref name="imageInfos"/>.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imageInfos">List of tuples containing image bytes and file type</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instruction">instruction to perform</param>
		/// <param name="classes">list of classes ("document", "invoice" etc.)</param>
		/// <param name="cancellationToken"></param>
		/// <returns>List of<see cref="DocumentImageResult"/> containing instructions and their answers.</returns>
		public static async Task<IReadOnlyList<DocumentImageResult>> ClassifyDocumentImages(this PerceptorClient client,
			IEnumerable<(byte[], string)> imageInfos,
			PerceptorRequest request,
			string instruction,
			IEnumerable<string> classes,
			CancellationToken cancellationToken = default) =>
			await ClassifyDocumentImages(client, request, instruction, classes,
				InstructionContextImageMapper.MapFromBytes(imageInfos), cancellationToken);

		/// <summary>
		/// Sends instructions for multiple images specified by <paramref name="imageInfos"/>.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imageInfos">List of tuples containing image stream and file type</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instructions">List of instructions to perform</param>
		/// <param name="cancellationToken"></param>
		/// <returns>List of<see cref="DocumentImageResult"/> containing instructions and their answers.</returns>
		public static async Task<IReadOnlyList<DocumentImageResult>> AskDocumentImages(this PerceptorClient client,
			IEnumerable<(Stream, string)> imageInfos,
			PerceptorRequest request,
			IEnumerable<string> instructions,
			CancellationToken cancellationToken = default)
		{
			IReadOnlyList<InstructionContextData> dataContexts =
				await InstructionContextImageMapper.MapFromStreams(imageInfos);
			return await client.AskInMultipleContexts(dataContexts, request, InstructionMethod.Question, instructions,
				Enumerable.Empty<string>(),
				cancellationToken);
		}

		/// <summary>
		/// Sends classify instruction for multiple images specified by <paramref name="imageInfos"/>.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imageInfos">List of tuples containing image stream and file type</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instruction">instruction to perform</param>
		/// <param name="classes">list of classes ("document", "invoice" etc.)</param>
		/// <param name="cancellationToken"></param>
		/// <returns>List of<see cref="DocumentImageResult"/> containing instructions and their answers.</returns>
		public static async Task<IReadOnlyList<DocumentImageResult>> ClassifyDocumentImages(this PerceptorClient client,
			IEnumerable<(Stream, string)> imageInfos,
			PerceptorRequest request,
			string instruction,
			IEnumerable<string> classes,
			CancellationToken cancellationToken = default) =>
			await ClassifyDocumentImages(client, request, instruction, classes,
				await InstructionContextImageMapper.MapFromStreams(imageInfos), cancellationToken);

		/// <summary>
		/// Sends a table instruction for multiple images.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imagePaths">image paths</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instruction">instruction to perform, for example 'GENERATE TABLE Article, Amount, Value GUIDED BY Value'</param>
		/// <param name="cancellationToken"></param>
		/// <returns>List of<see cref="DocumentImageResult"/> containing instructions and their answers.</returns>
		public static Task<IReadOnlyList<DocumentImageResult>> AskTableFromDocumentImages(this PerceptorClient client,
			IEnumerable<string> imagePaths,
			PerceptorRequest request,
			string instruction,
			CancellationToken cancellationToken = default)
		{
			IReadOnlyList<InstructionContextData> dataContexts = InstructionContextImageMapper.MapFromFiles(imagePaths);
			return client.AskTableInMultipleContexts(dataContexts, request, instruction, Enumerable.Empty<string>(),
				cancellationToken);
		}

		/// <summary>
		/// Sends a table instruction for multiple images.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imageStreams">list of tuples containing image stream and file type string</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instruction">instruction to perform, for example 'GENERATE TABLE Article, Amount, Value GUIDED BY Value'</param>
		/// <param name="cancellationToken"></param>
		/// <returns>List of<see cref="DocumentImageResult"/> containing instructions and their answers.</returns>
		public static async Task<IReadOnlyList<DocumentImageResult>> AskTableFromDocumentImages(
			this PerceptorClient client,
			IEnumerable<(Stream, string)> imageStreams,
			PerceptorRequest request,
			string instruction,
			CancellationToken cancellationToken = default)
		{
			IReadOnlyList<InstructionContextData> dataContexts =
				await InstructionContextImageMapper.MapFromStreams(imageStreams);
			return await client.AskTableInMultipleContexts(dataContexts, request, instruction,
				Enumerable.Empty<string>(), cancellationToken);
		}

		/// <summary>
		/// Sends a table instruction for multiple images.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="imageBytes">list of tuples containing image bytes and file type string</param>
		/// <param name="request">Request parameters</param>
		/// <param name="instruction">instruction to perform, for example 'GENERATE TABLE Article, Amount, Value GUIDED BY Value'</param>
		/// <param name="cancellationToken"></param>
		/// <returns>List of<see cref="DocumentImageResult"/> containing instructions and their answers.</returns>
		public static Task<IReadOnlyList<DocumentImageResult>> AskTableFromDocumentImages(this PerceptorClient client,
			IEnumerable<(byte[], string)> imageBytes,
			PerceptorRequest request,
			string instruction,
			CancellationToken cancellationToken = default)
		{
			IReadOnlyList<InstructionContextData> dataContexts = InstructionContextImageMapper.MapFromBytes(imageBytes);
			return client.AskTableInMultipleContexts(dataContexts, request, instruction, Enumerable.Empty<string>(),
				cancellationToken);
		}
	}
}