
using System;

namespace Perceptor.Client.Lib.InternalModels
{
	internal readonly struct WaitTimeOut
	{
		public WaitTimeOut(TimeSpan timeSpan)
		{
			TimeSpan = timeSpan;
		}

		public TimeSpan TimeSpan { get; }
	}
}