using KfAccountNumbers.Governmental.Europe;

namespace KfAccountNumbers.Tests.Benchmarks;

[MemoryDiagnoser]
public class OptimizeBenchmarks
{
   [Benchmark(Baseline = true)]
   [Arguments("85.07.30-033.28")]
   [Arguments("17110804680")]
   public void OriginalValid(String value) => _ = BeRijksregisternummer.Validate(value);
}
