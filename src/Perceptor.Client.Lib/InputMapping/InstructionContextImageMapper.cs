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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Perceptor.Client.Lib.InternalModels;

namespace Perceptor.Client.Lib.InputMapping
{
	internal static class InstructionContextImageMapper
	{
		private static readonly HashSet<string> _allowedFileExtensions = new HashSet<string>(new[]
		{
			"png",
			"jpg",
			"jpeg"
		}, StringComparer.InvariantCultureIgnoreCase);


		internal static InstructionContextData MapFromFile(string filePath)
		{
			string fileExtension = GetFileExtension(filePath);
			AssertIsValidFileType(fileExtension);

			byte[] loadedBytes = File.ReadAllBytes(filePath);
			return MapBytesArray(loadedBytes, fileExtension);
		}

		internal static IReadOnlyList<InstructionContextData> MapFromFiles(IEnumerable<string> filePaths) =>
			filePaths.Select(MapFromFile)
				.ToArray();


		internal static async Task<IReadOnlyList<InstructionContextData>> MapFromStreams(
			IEnumerable<(Stream Str, string FileType)> imageStreams)
		{
			List<InstructionContextData> res = new List<InstructionContextData>();
			foreach ((Stream Str, string FileType) file in imageStreams)
			{
				res.Add(await MapFromStream(file.Str, file.FileType));
			}

			return res;
		}

		internal static async Task<InstructionContextData> MapFromStream(Stream imageStream, string fileType)
		{
			AssertIsValidFileType(fileType);
			using var memoryStream = new MemoryStream();
			await imageStream.CopyToAsync(memoryStream);
			byte[] loadedBytes = memoryStream.ToArray();
			return MapBytesArray(loadedBytes, fileType);
		}

		internal static IReadOnlyList<InstructionContextData> MapFromBytes(
			IEnumerable<(byte[] imageBytes, string fileType)> imageInfos) =>
			imageInfos.Select(tuple => MapFromBytes(tuple.imageBytes, tuple.fileType))
				.ToArray();

		internal static InstructionContextData MapFromBytes(byte[] imageBytes, string fileType)
		{
			AssertIsValidFileType(fileType);
			return MapBytesArray(imageBytes, fileType);
		}

		private static InstructionContextData MapBytesArray(byte[] bytes, string fileExtension)
		{
			string base64 = Convert.ToBase64String(bytes);
			return InstructionContextData.ForImage($"data:image/{fileExtension};base64,{base64}");
		}


		internal static string GetFileExtension(string filePath)
		{
			string extension = Path.GetExtension(filePath);
			return extension.Length > 0 ? extension.Substring(1) : String.Empty;
		}

		internal static bool IsValidFileType(string extension)
		{
			return _allowedFileExtensions.Contains(extension);
		}

		private static readonly string _invalidFileTypeMessage =
			$"invalid file type, allowed: {String.Join(",", _allowedFileExtensions)}";

		private static void AssertIsValidFileType(string extension)
		{
			if (!IsValidFileType(extension))
			{
				throw new ArgumentException(_invalidFileTypeMessage);
			}
		}
	}
}