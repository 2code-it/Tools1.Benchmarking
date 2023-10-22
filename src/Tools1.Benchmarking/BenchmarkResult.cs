using System;

namespace Tools1.Benchmarking
{
	public class BenchmarkResult
	{

		public string? Identifier { get; set; }
		public int InvocationNumber { get; set; }
		public DateTime Created { get; set; }
		public long Duration { get; set; }
		public long TotalProcessorTime { get; set; }
		public long TotalAllocatedBytes { get; set; }
		public bool Failed { get; set; }
	}
}