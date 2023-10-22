using System;

namespace Tools1.Benchmarking
{
	public interface IBenchmarkerFactory
	{
		IBenchmarker Create(BenchmarkerOptions? options = null);
		IBenchmarker Create(Action<BenchmarkerOptions> action);
	}
}