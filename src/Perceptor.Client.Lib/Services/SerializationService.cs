namespace Perceptor.Client.Lib.Services
{
	internal static class SerializationService
	{
		public static string Serialize<T>(T obj)
		{
			return System.Text.Json.JsonSerializer.Serialize(obj);
		}

		public static T Deserialize<T>(string input)
		{
			return System.Text.Json.JsonSerializer.Deserialize<T>(input)!;
		}
	}
}