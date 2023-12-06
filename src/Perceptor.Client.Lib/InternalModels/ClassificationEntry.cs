namespace Perceptor.Client.Lib.InternalModels
{
	internal readonly struct ClassificationEntry
	{
		public ClassificationEntry(string value)
		{
			Value = value;
		}

		public string Value { get; }
	}
}