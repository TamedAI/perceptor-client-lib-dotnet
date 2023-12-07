using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Perceptor.Client.Lib.Services;

namespace Perceptor.Client.Lib
{
	internal static class StructuredResponseParser
	{
		public const string KeyNameText = "text";

		private static readonly IReadOnlyDictionary<string, object> _emptyResult = new Dictionary<string, object>()
		{
			{ KeyNameText, String.Empty }
		};

		public static IReadOnlyDictionary<string, object> ParseStructuredText(string input)
		{
			if (String.IsNullOrWhiteSpace(input))
			{
				return _emptyResult;
			}

			try
			{
				var deserialized = SerializationService.Deserialize<JsonObject>(input);
				return MapJsonObject(deserialized);
			}
			catch (JsonException)
			{
				return new Dictionary<string, object>()
				{
					{ KeyNameText, input }
				};
			}
		}

		private static IReadOnlyDictionary<string, object> MapJsonObject(JsonObject jsonObject)
		{
			var resultDictionary = jsonObject.ToDictionary(
				kv => kv.Key,
				kv => kv.Value.Map()
			);
			if (!resultDictionary.ContainsKey(KeyNameText))
			{
				resultDictionary.Add(KeyNameText, String.Empty);
			}

			return resultDictionary;
		}

		private static object Map(this JsonNode jsonNode)
		{
			return jsonNode switch
			{
				JsonValue v => v.ToString(),
				JsonObject o => o.ToDictionary(x => x.Key, x => x.Value.Map()),
				JsonArray arr => arr.Select(x => x.Map()).ToList(),
				_ => jsonNode.ToString()
			};
		}
	}
}