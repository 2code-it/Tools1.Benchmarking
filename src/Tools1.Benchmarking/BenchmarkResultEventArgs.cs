using System;

namespace Tools1.Benchmarking
{
	public class BenchmarkResultEventArgs : EventArgs
	{
		public BenchmarkResultEventArgs(BenchmarkResult result)
		{
			Result = result;
		}

		public BenchmarkResult Result { get; private set; }
	}
}
