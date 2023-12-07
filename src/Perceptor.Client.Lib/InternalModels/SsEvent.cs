namespace Perceptor.Client.Lib.InternalModels
{
	internal readonly struct SsEvent
	{
		public SsEvent( string data)
		{
			Data = data;
		}

		public string Data { get; }
	}
}