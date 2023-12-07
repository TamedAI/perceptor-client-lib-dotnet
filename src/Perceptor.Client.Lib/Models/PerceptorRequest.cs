using System.Collections.Generic;

namespace Perceptor.Client.Lib.Models
{
	/// <summary>
	/// Request information
	/// </summary>
	public class PerceptorRequest
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="flavor"></param>
		/// <param name="parameters"></param>
		/// <param name="returnScores"></param>
		public PerceptorRequest(string flavor, IReadOnlyDictionary<string,string> parameters, bool returnScores)
		{
			Parameters = parameters;
			ReturnScores = returnScores;
			Flavor = flavor;
		}

		/// <summary>
		/// Flavor
		/// </summary>
		public string Flavor { get; }
		/// <summary>
		/// Detailed request parameters
		/// </summary>
		public IReadOnlyDictionary<string, string> Parameters { get; }
		/// <summary>
		/// true if scores are to be returned
		/// </summary>
		public bool ReturnScores { get; }
		
		/// <summary>
		/// Creates another instance where <see cref="ReturnScores"/> is set to true
		/// </summary>
		/// <returns></returns>

		public PerceptorRequest WithReturnScores()
		{
			return ReturnScores ? this : new PerceptorRequest(Flavor, Parameters, true);
		}
		
		private static readonly IReadOnlyDictionary<string, string> _emptyParameters = new Dictionary<string, string>();

		/// <summary>
		/// Creates an instance with default parameters and the specified <paramref name="flavor"/>
		/// </summary>
		/// <param name="flavor"></param>
		/// <returns></returns>
		public static PerceptorRequest WithFlavor(string flavor)
		{
			return new PerceptorRequest(flavor, _emptyParameters, false);
		}
		
	}
}