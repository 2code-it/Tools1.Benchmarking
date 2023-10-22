using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tools1.Benchmarking;
using System;
using System.Collections.Generic;
using System.Text;
using Tools1.BenchmarkingTests;
using System.Threading.Tasks;
using System.Linq;

namespace Tools1.Benchmarking.Tests
{
	[TestClass]
	public class BenchmarkerTests
	{
		[TestMethod]
		[DataRow (typeof(TestItemGeneric<string>), false)]
		[DataRow(typeof(TestItemCtor), false)]
		[DataRow(typeof(TestItem), true)]
		public void CanRun_When_ProvidedType_Expect_ProvidedResult(Type type, bool result)
		{
			Benchmarker benchmarker = new Benchmarker();

			bool canRun = benchmarker.CanRun(type);

			Assert.AreEqual(result, canRun);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public async Task Error_When_ExceptionThrownAndErrorEventNotSet_Expect_Exception()
		{
			Benchmarker benchmarker = new Benchmarker();

			await benchmarker.RunAsync<TestItemWithError>();
		}

		[TestMethod]
		public async Task Error_When_ExceptionThrownAndErrorEventSet_Expect_Event()
		{
			Benchmarker benchmarker = new Benchmarker();
			Exception? ex = null;
			benchmarker.Error += (s, e) => { ex = (Exception)e.ExceptionObject; };

			await benchmarker.RunAsync<TestItemWithError>();

			Assert.IsNotNull(ex);
		}

		[TestMethod]
		[DataRow(true, 2)]
		[DataRow(false, 3)]
		public async Task Configure_When_StopOnFailureOptionsSet_Expect_BenchmarkResultLength(bool stopOnFailure, int expectedResultLength)
		{
			Benchmarker benchmarker = Benchmarker.CreateWith(x => { x.StopOnFailure = stopOnFailure; });
			benchmarker.Error += (s, e) => { };

			var result = await benchmarker.RunAsync<TestItemWithError>();

			Assert.AreEqual(expectedResultLength, result.Length);
		}

		[TestMethod]
		public async Task Configure_When_RunInvocationCountIsSet_Expect_BenchmarkResultLength()
		{
			int invocationCount = 3;
			int methodCount = 3;
			Benchmarker benchmarker = Benchmarker.CreateWith(x => { x.InvocationCount = invocationCount; });

			var result = await benchmarker.RunAsync<TestItem>();

			Assert.AreEqual(invocationCount * methodCount, result.Length);
		}

		[TestMethod]
		public async Task Configure_When_RunOrderByInvocationNumberIsTrue_Expect_ResultOrderedBySquenceNumber()
		{
			Benchmarker benchmarker = Benchmarker.CreateWith(x => { x.OrderRunByInvocationNumber = true; x.InvocationCount = 3; });

			var result = await benchmarker.RunAsync<TestItem>();

			Assert.AreEqual(1, result[2].InvocationNumber);
			Assert.AreEqual(3, result[6].InvocationNumber);
		}

		[TestMethod]
		public async Task Configure_When_RunOrderByInvocationNumberIsFalse_Expect_ResultOrderedByMethod()
		{
			Benchmarker benchmarker = Benchmarker.CreateWith(x => { x.OrderRunByInvocationNumber = false; x.InvocationCount = 3; });

			var result = await benchmarker.RunAsync<TestItem>();

			Assert.AreEqual(1, result[0].InvocationNumber);
			Assert.AreEqual(2, result[7].InvocationNumber);
		}

		[TestMethod]
		public async Task RunAsync_When_ActionParametersSet_Expect_ResultWithPArameterValues()
		{
			Benchmarker benchmarker = new Benchmarker();
			const string identifier = "id";
			const int invokcationNumber = 1;
			var result = await benchmarker.RunAsync(() => { }, identifier, invokcationNumber);

			Assert.AreEqual(identifier, result.Identifier);
			Assert.AreEqual(invokcationNumber, result.InvocationNumber);


		}

		[TestMethod]
		public async Task RunAsync_Test()
		{
			Benchmarker benchmarker = Benchmarker.CreateWith(x => { x.InvocationCount = 3; });
			var result = await benchmarker.RunAsync<Benchmarks>();
			string[] lines = result.Select(x => $"{x.Identifier}\t{x.InvocationNumber}\t{x.Duration:N0}").ToArray();

			Console.WriteLine(string.Join("\r\n", lines));
		}
	}
}