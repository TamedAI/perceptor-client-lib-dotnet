// /*
// Copyright 2023 TamedAI GmbH
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */

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