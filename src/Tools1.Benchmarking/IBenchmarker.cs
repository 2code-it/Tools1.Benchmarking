using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tools1.Benchmarking
{
	public interface IBenchmarker
	{
		event EventHandler<BenchmarkResultEventArgs>? BenchmarkCompleted;
		event EventHandler<UnhandledExceptionEventArgs>? Error;

		bool CanRun(Type type);
		Benchmarker Configure(Action<BenchmarkerOptions> optionsAction);

		Task<BenchmarkResult> RunAsync(Action action, string? identifier = null, int invocationNumber = 1, CancellationToken? cancellationToken = null);
		Task<BenchmarkResult[]> RunAsync(Type[] types, CancellationToken? cancellationToken = null);
		Task<BenchmarkResult[]> RunAsync(Type type, CancellationToken? cancellationToken = null);
		Task<BenchmarkResult[]> RunAsync<T>(CancellationToken? cancellationToken = null);
	}
}