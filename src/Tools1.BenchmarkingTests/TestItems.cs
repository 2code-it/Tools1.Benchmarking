using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tools1.BenchmarkingTests
{
	internal class TestItem
	{
		public void RunBenchmark1() { }
		public void RunBenchmark2() { }
		public void RunBenchmark3() { }
	}

	internal class TestItemCtor
	{
		public TestItemCtor(string data) { }
		public void RunBenchmark1() { }
	}

	internal class TestItemGeneric<T>
	{
		public T RunBenchmark1() { return default!; }
	}

	internal class TestItemWithError
	{
		public void RunBenchmark1() { }
		public void RunBenchmark2() { throw new InvalidOperationException("Test Failed"); } 
		public void RunBenchmark3() { }
	}

	public class Benchmarks
	{
		public void BenchThis1()
		{
			Task.Delay(100).Wait();
		}

		public void BenchThis2()
		{
			Task.Delay(200).Wait();
		}

		//etc...
	}
}
