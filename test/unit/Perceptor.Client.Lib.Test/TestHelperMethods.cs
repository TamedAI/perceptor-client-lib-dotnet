namespace Perceptor.Client.Lib.Test;

internal static class TestHelperMethods
{
	public static string GetTestFilePath(string fileName)
	{
		return Path.Combine(TestContext.CurrentContext.WorkDirectory,
			"..", "..", "..", "TestFiles", fileName);
	}
}