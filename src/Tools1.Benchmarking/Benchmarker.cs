using System.Diagnostics;
using System.Reflection;

namespace Tools1.Benchmarking
{
	public static class Benchmarker
	{
		private const string _setupMethodName = "Setup";
		private const string _cleanupMethodName = "Cleanup";

		public static IDictionary<string, List<long>> Run<T>(T source, int passes = 1)
		{
			if (source is null) throw new InvalidOperationException("Source is null");
			return Run(typeof(T), source, passes);
		}

		public static IDictionary<string, List<long>> Run<T>(int passes = 1)
		{
			return Run(typeof(T), passes);
		}

		public static IDictionary<string, List<long>> Run(Type benchType, int passes = 1)
		{
			object? instance = Activator.CreateInstance(benchType);
			if (instance == null) throw new InvalidOperationException($"Can't create an instance of {benchType.Name}");
			return Run(benchType, instance, passes);
		}

		public static IDictionary<string, List<long>> Run(Type benchType, object source, int passes = 1)
		{
			RunSetup(source);

			string[] excludedMethodNames = typeof(object).GetMethods().Select(x => x.Name).Union(new[] { _setupMethodName, _cleanupMethodName }).ToArray();
			MethodInfo[] benchMethods = benchType.GetMethods().Where(x => x.IsPublic && !x.GetParameters().Any() && !excludedMethodNames.Contains(x.Name)).ToArray();

			IDictionary<string, List<long>> results = benchMethods.ToDictionary(x => x.Name, y => new List<long>());
			Stopwatch stopWatch = new Stopwatch();

			for (int i = 1; i <= passes; i++)
			{
				foreach (MethodInfo method in benchMethods)
				{
					stopWatch.Restart();
					method.Invoke(source, null);
					stopWatch.Stop();
					results[method.Name].Add(stopWatch.ElapsedMilliseconds);
				}
			}

			RunCleanup(source);
			return results;
		}

		public static async Task<IDictionary<string, List<long>>> RunAsync<T>(T source, int passes = 1)
		{
			return await Task.Run(() => Run<T>(source, passes));
		}

		public static async Task<IDictionary<string, List<long>>> RunAsync<T>(int passes = 1)
		{
			return await Task.Run(() => Run<T>(passes));
		}

		public static async Task<IDictionary<string, List<long>>> RunAsync(Type benchType, int passes = 1)
		{
			return await Task.Run(() => Run(benchType, passes));
		}

		public static async Task<IDictionary<string, List<long>>> RunAsync(Type benchType, object source, int passes = 1)
		{
			return await Task.Run(() => Run(benchType, source, passes));
		}

		private static void RunSetup<T>(T source)
		{
			MethodInfo? method = typeof(T).GetMethods().FirstOrDefault(x => x.Name == _setupMethodName);
			if (method is null) return;
			method.Invoke(source, null);
		}

		private static void RunCleanup<T>(T source)
		{
			MethodInfo? method = typeof(T).GetMethods().FirstOrDefault(x => x.Name == _cleanupMethodName);
			if (method is null) return;
			method.Invoke(source, null);
		}
	}
}