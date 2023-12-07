using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Perceptor.Client.Lib.InternalModels;

namespace Perceptor.Client.Lib.Repository
{
	internal static class SseEventParser
	{
		private const string _PREFIX_EVENT = "event"; 
		private const string _PREFIX_DATA = "data";
		private const string _EVENT_FINISHED = "finished";
		private static readonly string _eventFinishedLine = $"{_PREFIX_EVENT}: {_EVENT_FINISHED}";
		private static readonly string _dataLinePrefix = $"{_PREFIX_DATA}: ";
		
		public static async Task<IReadOnlyList<SsEvent>> ParseEvents(string eventsString)
		{
			if (eventsString == null)
			{
				return Array.Empty<SsEvent>();
			}
			IReadOnlyList<string> lines = await eventsString.GetLines();
			
			return lines
				.SelectDataLines()
				.Select(dataLine => new SsEvent(dataLine.GetDataFromLine()))
				.ToArray();
		}


		private static IEnumerable<string> SelectDataLines(this IEnumerable<string> lineList)
		{
			var aggregatedDataLines = lineList.Aggregate(
				(Agg: Enumerable.Empty<string>(), IsPredecessorRelevant: false),
				(agg, line) => agg.IsPredecessorRelevant ? 
					(agg.Agg.Append(line), false) : 
					(agg.Agg, line.IsEventFinishedLine())
				);

			return aggregatedDataLines.Agg;
		}
		

		private static string GetDataFromLine(this string input) =>
			input.StartsWith(_dataLinePrefix, StringComparison.InvariantCultureIgnoreCase)
				? input.Substring(_dataLinePrefix.Length)
				: String.Empty;

		private static bool IsEventFinishedLine(this string input) => 
			input?.Equals(_eventFinishedLine, StringComparison.InvariantCultureIgnoreCase) == true;

		private static async Task<IReadOnlyList<string>> GetLines(this string input)
		{
			var resultLines = new List<string>();
			using var lines = new StringReader(input);
			string singleLine;
			bool foundEventFinishedLine = false;
			do
			{
				singleLine = await lines.ReadLineAsync();

				foundEventFinishedLine = foundEventFinishedLine || singleLine.IsEventFinishedLine();
				if (foundEventFinishedLine &&  !String.IsNullOrWhiteSpace(singleLine))
				{
					resultLines.Add(singleLine);
				}
			} while (singleLine != null);

			return resultLines;

		}
	}
}