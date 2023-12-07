using System;
using System.Collections.Generic;
using System.Linq;

namespace Perceptor.Client.Lib.InternalModels
{
	internal readonly struct OneOf<T1, T2>
	{
		private readonly IReadOnlyList<T1> _first;
		private readonly IReadOnlyList<T2> _second;

		private OneOf(T1 first)
		{
			_first = new[] { first };
			_second = Array.Empty<T2>();
		}
		
		private OneOf(T2 second)
		{
			_second = new[] { second };
			_first = Array.Empty<T1>();
		}

		public TR Match<TR>(Func<T1, TR> matchFirst,
			Func<T2, TR> matchSecond)
		{
			return _first.Any() ? matchFirst(_first[0]) : matchSecond(_second[0]);
		}

		internal void Use(Action<T1> useFirst, Action<T2> useSecond)
		{
			if (_first.Any())
			{
				useFirst(_first[0]);
			}
			else
			{
				useSecond(_second[0]);
			}
		}
		
		public static implicit operator OneOf<T1, T2>(T1 first) => new OneOf<T1, T2>(first);
		public static implicit operator OneOf<T1, T2>(T2 second) => new OneOf<T1, T2>(second);
	}
}