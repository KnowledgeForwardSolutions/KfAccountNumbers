using KfAccountNumbers.National.Europe;

#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable IDE0008 // Use explicit type

namespace KfAccountNumbers.Tests.Benchmarks;

[MemoryDiagnoser]
public class OptimizeBenchmarks
{
   [Benchmark(Baseline = true)]
   [Arguments("85.07.30-033.28")]
   [Arguments("17110804680")]
   public void OriginalValid(String value) => _ = BeRijksregisternummer.Validate(value);
}
