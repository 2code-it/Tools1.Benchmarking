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
        Taks.Delay(500).Wait();
    }

    public void BenchThis2()
    {
        Taks.Delay(1000).Wait();
    }

    //etc...
}
```
## 2. Run the test (Console app example)

```
// See https://aka.ms/new-console-template for more information

using Tools1.Benchmarking;

Benchmarker.Shared.Run<Benchmarks>(5);
```

## 3. Output
BenchThis1  500  
BenchThis2  1000
