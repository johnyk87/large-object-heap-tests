namespace LohPoc.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Engines;
    using BenchmarkDotNet.Jobs;
    using LohPoc.Lib;

    [MemoryDiagnoser]
    [Config(typeof(DontForceGcCollectionsConfig))] // we don't want to interfere with GC, we want to include it's impact
    public partial class AllocationBenchmark
    {
        [Params(1000, 10000, 100000, 1000000)]
        public int ArraySize { get; set; }

        [Benchmark]
        public void ArrayT()
        {
            DeadCodeEliminationHelper.KeepAliveWithoutBoxing(
                new SomeReferenceType[ArraySize]);
        }

        [Benchmark]
        public void ShardedArrayT()
        {
            DeadCodeEliminationHelper.KeepAliveWithoutBoxing(
                new ShardedArray<SomeReferenceType>(ArraySize));
        }

        [Benchmark]
        public void RentedArrayT()
        {
            using var _ = new RentedArray<SomeReferenceType>(ArraySize);
        }
    }

    public class DontForceGcCollectionsConfig : ManualConfig
    {
        public DontForceGcCollectionsConfig()
        {
            AddJob(Job.Default
                .WithGcMode(new GcMode()
                {
                    Force = false // tell BenchmarkDotNet not to force GC collections after every iteration
                }));
        }
    }
}