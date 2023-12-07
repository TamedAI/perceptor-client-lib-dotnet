namespace Perceptor.Client.Lib.InternalModels
{
	internal readonly struct Instruction
	{
		public Instruction(string content)
		{
			Content = content;
		}

		public string Content { get; }

		public override string ToString()
		{
			return Content;
		}
	}
}