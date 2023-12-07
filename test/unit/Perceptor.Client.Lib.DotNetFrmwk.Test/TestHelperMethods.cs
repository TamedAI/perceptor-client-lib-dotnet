using System.IO;
using NUnit.Framework;

namespace Perceptor.Client.Lib.DotNetFrmwk.Test
{
	internal static class TestHelperMethods
	{
		public static string GetTestFilePath(string fileName)
		{
			return Path.Combine(TestContext.CurrentContext.WorkDirectory,
				"TestFiles", fileName);
		}
	}	
}
