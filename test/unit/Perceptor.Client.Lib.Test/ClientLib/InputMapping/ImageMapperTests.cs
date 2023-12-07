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

using FluentAssertions;
using Perceptor.Client.Lib.InputMapping;
using Perceptor.Client.Lib.InternalModels;

namespace Perceptor.Client.Lib.Test.ClientLib.InputMapping;

[TestFixture]
public class ImageMapperTests
{
	[Test]
	public void ImageFile_Mapped_Correctly()
	{
		var filePath = TestHelperMethods.GetTestFilePath("binary_file.png");

		InstructionContextData result = InstructionContextImageMapper.MapFromFile(filePath);

		result.ContextType.Should().Be("image");
		result.Content.Should().Be("data:image/png;base64,MXg=");
	}

	[Test]
	public async Task ImageStream_Mapped_Correctly()
	{
		var filePath = TestHelperMethods.GetTestFilePath("binary_file.png");
		await using FileStream fileStream = File.OpenRead(filePath);

		InstructionContextData result = await InstructionContextImageMapper.MapFromStream(fileStream, "png");

		result.ContextType.Should().Be("image");
		result.Content.Should().Be("data:image/png;base64,MXg=");
	}

	[Test]
	public async Task ImageBytes_Mapped_Correctly()
	{
		var filePath = TestHelperMethods.GetTestFilePath("binary_file.png");
		await using FileStream fileStream = File.OpenRead(filePath);
		using var memoryStream = new MemoryStream();
		await fileStream.CopyToAsync(memoryStream);
		byte[] loadedBytes = memoryStream.ToArray();

		InstructionContextData result = InstructionContextImageMapper.MapFromBytes(loadedBytes, "png");

		result.ContextType.Should().Be("image");
		result.Content.Should().Be("data:image/png;base64,MXg=");
	}

	[Test]
	public void GIVEN_Invalid_FileType_WHEN_Opening_THEN_Exception_Is_Raised()
	{
		var filePath = TestHelperMethods.GetTestFilePath("invalid_file.bmp");
		Action openAction = () => InstructionContextImageMapper.MapFromFile(filePath);
		openAction.Should().Throw<ArgumentException>();
	}

	[TestCase("some-file.png", "png")]
	[TestCase("some-file", "")]
	[TestCase("some-file.", "")]
	[TestCase("path/some-file", "")]
	[TestCase("path/some-file.jpg", "jpg")]
	public void GIVEN_FilePath_WHEN_GetExtension_THEN_Extension_Is_Correct(string filePath, string expected)
	{
		InstructionContextImageMapper.GetFileExtension(filePath).Should().Be(expected);
	}

	[TestCase("jpg", true)]
	[TestCase("JPG", true)]
	[TestCase("JPEG", true)]
	[TestCase("jpeg", true)]
	[TestCase("png", true)]
	[TestCase("PNG", true)]
	[TestCase("", false)]
	[TestCase("x", false)]
	[TestCase("bmp", false)]
	public void GIVEN_FileType_THEN_Validity_Is_Checked(string fileExtension, bool expectedIsValid)
	{
		InstructionContextImageMapper.IsValidFileType(fileExtension).Should().Be(expectedIsValid);
	}
}