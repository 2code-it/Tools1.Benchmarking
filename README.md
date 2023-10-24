[![Build Status](https://dev.azure.com/2code-it/pub/_apis/build/status%2F2code-it.Tools1.Benchmarking?branchName=develop "dev-pack")](https://dev.azure.com/2code-it/pub/_build/latest?definitionId=4&branchName=develop)

# Tools1.Benchmarking
Tool to run benchmarks 

## 1.Create Benchmark class
```
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
```
## 2. Run the test (Console app example)

```
using Tools1.Benchmarking;

Benchmarker benchmarker = Benchmarker.CreateWith(x => { x.InvocationCount = 3; });
var result = await benchmarker.RunAsync<Benchmarks>();
string[] lines = result.Select(x => $"{x.Identifier}\t{x.InvocationNumber}\t{x.Duration:N0}").ToArray();

Console.WriteLine(string.Join("\r\n", lines));

```

## 3. Output
Benchmarks.BenchThis1	1	115,311 
Benchmarks.BenchThis1	2	108,462 
Benchmarks.BenchThis1	3	108,536 
Benchmarks.BenchThis2	1	203,523 
Benchmarks.BenchThis2	2	205,462 
Benchmarks.BenchThis2	3	205,538 
