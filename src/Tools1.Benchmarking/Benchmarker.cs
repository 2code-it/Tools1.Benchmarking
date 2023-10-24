using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Tools1.Benchmarking.Internals;

namespace Tools1.Benchmarking
{
	public class Benchmarker : IBenchmarker
	{
		public Benchmarker() : this(null) { }
		public Benchmarker(BenchmarkerOptions? options) : this(options, new ProcessInfoService()) { }
		internal Benchmarker(BenchmarkerOptions? options, IProcessInfoService processInfoService)
		{
			_processInfoService = processInfoService;
			if (!(options is null)) MapOptions(options, _options);
		}

		private IProcessInfoService _processInfoService;
		private Stopwatch _stopwatch = new Stopwatch();
		private readonly string[] _excludedMethodNames = typeof(object).GetMethods().Select(x => x.Name).ToArray();
		private readonly PropertyInfo[] _optionsProperties = typeof(BenchmarkerOptions).GetProperties().Where(x => x.CanWrite && !(x.GetSetMethod() is null)).ToArray();
		private BenchmarkerOptions _options = CreateDefaultOptions();

		public event EventHandler<BenchmarkResultEventArgs>? BenchmarkCompleted;
		public event EventHandler<UnhandledExceptionEventArgs>? Error;

		public Benchmarker Configure(Action<BenchmarkerOptions> optionsAction)
		{
			var options = CreateDefaultOptions();
			optionsAction(options);
			MapOptions(options, _options);
			return this;
		}

		public bool CanRun(Type type)
			=> type.IsClass && !type.IsGenericType && type.GetConstructors().Any(x => x.GetParameters().Length == 0);

		public async Task<BenchmarkResult[]> RunAsync(Type[] types, CancellationToken? cancellationToken = null)
		{
			List<BenchmarkResult[]> resultsList = new List<BenchmarkResult[]>();
			foreach(Type type in types)
			{
				if(!CanRun(type)) continue;
				resultsList.Add(await RunAsync(type, cancellationToken));
			}
			return resultsList.SelectMany(x=>x).ToArray();
		}

		public async Task<BenchmarkResult[]> RunAsync(Type type, CancellationToken? cancellationToken = null)
		{
			object? source = Activator.CreateInstance(type);
			if (source is null) throw new ArgumentException($"Can't create instance of {type.Name}", nameof(type));

			MethodInfo[] benchMethods = type.GetMethods().Where(BenchmarkMethodFilter).ToArray();

			var runInfos = benchMethods.SelectMany(m => GetBenchmarkRunInfo(m, _options.InvocationCount, source, type)).ToArray();
			if (_options.OrderRunByInvocationNumber) runInfos = runInfos.OrderBy(x => x.invocationNumber).ToArray();

			BenchmarkResult[] results = new BenchmarkResult[runInfos.Length];
			int i = 0;
			do
			{
				results[i] = await RunAsync(runInfos[i].action, runInfos[i].id, runInfos[i].invocationNumber, cancellationToken);
				if (_options.StopOnFailure && results[i].Failed) break;
			}
			while (++i < runInfos.Length);

			return results.Take(i+1).ToArray();
		}

		public async Task<BenchmarkResult> RunAsync(Action action, string? identifier = null, int invocationNumber = 1, CancellationToken? cancellationToken = null)
		{
			long ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;

			BenchmarkResult result = CreateNewResult(identifier);
			_stopwatch.Restart();
			_processInfoService.Reset();

			bool success = await TryRunActionAsync(action, cancellationToken);

			_stopwatch.Stop();
			result.Failed = !success;
			result.InvocationNumber = invocationNumber;
			result.Duration = _stopwatch.ElapsedTicks / ticksPerMicrosecond;
			result.TotalProcessorTime = _processInfoService.GetTotalProcessorTicks() / ticksPerMicrosecond;
			result.TotalAllocatedBytes = _processInfoService.GetTotalAllocatedBytes();

			OnBenchmarkCompleted(result);
			return result;
		}

		public async Task<BenchmarkResult[]> RunAsync<T>(CancellationToken? cancellationToken = null)
			=> await RunAsync(typeof(T), cancellationToken);

		public static BenchmarkerOptions CreateDefaultOptions()
			=> new BenchmarkerOptions { InvocationCount = 1 };

		public static Benchmarker CreateWith(Action<BenchmarkerOptions> optionsAction)
			=> new Benchmarker().Configure(optionsAction);


		private (string id, Action action, int invocationNumber)[] GetBenchmarkRunInfo(MethodInfo method, int invocationCount, object source, Type type)
			=> Enumerable.Range(1, invocationCount)
				.Select(i => ($"{type.Name}.{method.Name}", (Action)Delegate.CreateDelegate(typeof(Action), source, method), i))
				.ToArray();

		private void MapOptions(BenchmarkerOptions optionsSource, BenchmarkerOptions optionsDestination)
		{
			foreach(var property in _optionsProperties)
			{
				property.SetValue(optionsDestination, property.GetValue(optionsSource));
			}
		}

		private bool BenchmarkMethodFilter(MethodInfo method)
			=> !method.IsSpecialName
				&& method.IsPublic
				&& !method.GetParameters().Any()
				&& !_excludedMethodNames.Contains(method.Name);
		
		private BenchmarkResult CreateNewResult(string? identifier, int invocationNumber = 1)
		{
			BenchmarkResult result = new BenchmarkResult();
			result.Created = DateTime.Now;
			result.Identifier = identifier;
			result.InvocationNumber = invocationNumber;
			return result;
		}

		private async Task<bool> TryRunActionAsync(Action action, CancellationToken? cancellationToken)
		{
			try
			{
				await Task.Run(action, cancellationToken ?? CancellationToken.None);
				return true;
			}
			catch(AggregateException ae)
			{
				OnError(ae.InnerException);
			}
			catch (Exception ex)
			{
				OnError(ex);
			}
			return false;
		}

		private void OnBenchmarkCompleted(BenchmarkResult result)
		{
			BenchmarkCompleted?.Invoke(this, new BenchmarkResultEventArgs(result));
		}

		private void OnError(Exception? exception)
		{
			if (exception is null) return;
			if (Error is null) throw exception;
			Error.Invoke(this, new UnhandledExceptionEventArgs(exception, false));
		}
	}
}