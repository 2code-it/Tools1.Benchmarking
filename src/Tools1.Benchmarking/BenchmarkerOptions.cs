using System;
using System.Collections.Generic;
using System.Text;

namespace Tools1.Benchmarking
{
	public class BenchmarkerOptions
	{
		public bool OrderRunByInvocationNumber { get; set; }
		public int InvocationCount { get; set; }
		public bool StopOnFailure { get; set; }
	}
}
