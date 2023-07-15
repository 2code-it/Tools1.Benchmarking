[![Build Status](https://dev.azure.com/2code-it/pub/_apis/build/status%2F2code-it.Tools1.Benchmarking?branchName=develop "dev-pack")](https://dev.azure.com/2code-it/pub/_build/latest?definitionId=4&branchName=develop)

# Tools1.Benchmarking
Tool to run benchmarks 

## 1.Create Benchmark class
```
	public class Benchmarks
	{
		public void Setup()
		{
			//optional: setup the benchmark prerequisites
		}

		public void Cleanup()
		{
			//optional: cleanup benchmark
		}

		public void BenchThis1()
		{
			Task.Delay(500).Wait();
		}

		public void BenchThis2()
		{
			Task.Delay(1000).Wait();
		}

		//etc...
	}
```
## 2. Run the test (Console app example)

```
using Tools1.Benchmarking;

IDictionary<string, List<long>> result = Benchmarker.Run<Benchmarks>(5);
string[] lines = result.ToDictionary(x => x.Key, x => (int)Math.Round(x.Value.Average()))
    .Select(x => $"{x.Key}\t{x.Value}")
    .ToArray();

Console.WriteLine(string.Join("\r\n", lines));

```

## 3. Output
BenchThis1  508  
BenchThis2  1011
