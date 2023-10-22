using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Tools1.Benchmarking
{
	public class BenchmarkerFactory : IBenchmarkerFactory
	{
		public IBenchmarker Create(BenchmarkerOptions? options = null)
			=> new Benchmarker(options);

		public IBenchmarker Create(Action<BenchmarkerOptions> action)
			=> new Benchmarker().Configure(action);
	}
}
