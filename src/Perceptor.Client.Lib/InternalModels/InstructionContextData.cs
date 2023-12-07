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

namespace Perceptor.Client.Lib.InternalModels
{
	internal class InstructionContextData
	{
		public string ContextType { get; private set; }
		public string Content { get; private set; }

		public static InstructionContextData ForImage(string content)
		{
			return new InstructionContextData()
			{
				Content = content,
				ContextType = "image"
			};
		}

		public static InstructionContextData ForText(string textContent)
		{
			return new InstructionContextData()
			{
				Content = textContent,
				ContextType = "text"
			};
		}
	}
}