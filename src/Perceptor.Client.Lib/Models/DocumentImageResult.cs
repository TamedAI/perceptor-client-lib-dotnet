using System.Collections.Generic;

namespace Perceptor.Client.Lib.Models
{
	/// <summary>
	/// Response object for multi page queries
	/// </summary>
	public class DocumentImageResult
	{
		internal DocumentImageResult(int pageIndex, IReadOnlyList<InstructionWithResult> results)
		{
			PageIndex = pageIndex;
			Results = results;
		}

		/// <summary>
		/// Zero based index of original document page
		/// </summary>
		public int PageIndex { get; }
		
		/// <summary>
		/// List of results for the page.
		/// </summary>
		public IReadOnlyList<InstructionWithResult> Results { get; }
	}
}