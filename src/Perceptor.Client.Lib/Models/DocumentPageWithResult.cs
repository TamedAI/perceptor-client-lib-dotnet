using System.Collections.Generic;

namespace Perceptor.Client.Lib.Models
{
	public class DocumentPageWithResult
	{
		public DocumentPageWithResult(int pageIndex, bool isSuccess, IReadOnlyDictionary<string, object> response, string errorText)
		{
			PageIndex = pageIndex;
			IsSuccess = isSuccess;
			Response = response;
			ErrorText = errorText;
		}

		/// <summary>
		/// Zero based index of original document page
		/// </summary>
		public int PageIndex { get; }
		public bool IsSuccess { get; }
		/// <summary>
		/// Response object, containing at least "text" element (with response text) and optionally other elements (like scores).
		/// </summary>
		public IReadOnlyDictionary<string, object> Response { get; }
		/// <summary>
		/// Error text (in case of failed query)
		/// </summary>
		public string ErrorText { get; }
	}
}