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