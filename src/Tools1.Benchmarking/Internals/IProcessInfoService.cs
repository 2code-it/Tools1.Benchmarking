namespace Tools1.Benchmarking.Internals
{
	internal interface IProcessInfoService
	{
		long GetTotalAllocatedBytes();
		long GetTotalProcessorTicks();
		void Reset();
	}
}