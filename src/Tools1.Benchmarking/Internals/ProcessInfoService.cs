using System;
using System.Diagnostics;

namespace Tools1.Benchmarking.Internals
{
	internal class ProcessInfoService : IProcessInfoService
	{

		private Process _currentProcess = Process.GetCurrentProcess();
		private long _startTotalProcessorTicks;
		private long _startTotalAllocatedBytes;

		public void Reset()
		{
			GC.Collect(0, GCCollectionMode.Forced, true);
			_currentProcess.Refresh();
			_startTotalProcessorTicks = _currentProcess.TotalProcessorTime.Ticks;
			_startTotalAllocatedBytes = GetTotalAllocatedBytesInner();
		}

		public long GetTotalAllocatedBytes()
		{
			return GetTotalAllocatedBytesInner() - _startTotalAllocatedBytes;
		}

		public long GetTotalProcessorTicks()
		{
			_currentProcess.Refresh();
			return _currentProcess.TotalProcessorTime.Ticks - _startTotalProcessorTicks;
		}

		private long GetTotalAllocatedBytesInner()
		{
#if NET5_0_OR_GREATER
			return GC.GetTotalAllocatedBytes(true);
#else
			var method = typeof(GC).GetMethod("GetTotalAllocatedBytes");
			if (method is null) throw new NotSupportedException("GC.GetTotalAllocatedBytes not available");
			return (long)method.Invoke(null, new object[] { true });
#endif
		}
	}
}
