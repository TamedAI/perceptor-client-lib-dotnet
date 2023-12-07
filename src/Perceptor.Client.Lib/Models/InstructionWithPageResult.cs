using System.Collections.Generic;

namespace Perceptor.Client.Lib.Models
{
	public class InstructionWithPageResult
	{
		public InstructionWithPageResult(string instructionText, IReadOnlyList<DocumentPageWithResult> pageResults)
		{
			InstructionText = instructionText;
			PageResults = pageResults;
		}

		/// <summary>
		/// Original instruction text
		/// </summary>
		public string InstructionText { get; }
		
		/// <summary>
		/// List of results for the instruction.
		/// </summary>
		public IReadOnlyList<DocumentPageWithResult> PageResults { get; }
		
	}
}