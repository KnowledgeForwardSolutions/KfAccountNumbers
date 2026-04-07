using KfAccountNumbers.Governmental.Europe;

namespace KfAccountNumbers.Tests.Benchmarks;

[MemoryDiagnoser]
public class OptimizeBenchmarks
{
   //[Benchmark(Baseline = true)]
   //[Arguments("85.07.30-033.28")]
   //[Arguments("17110804680")]
   //public void OriginalValid(String value) => _ = BeRijksregisternummer.Validate(value);

   //[Benchmark]
   //[Arguments("85.07.30-033.28")]
   //[Arguments("17110804680")]
   //public void FunctionalValid(String value) => _ = BeRijksregisternummer.Validate2(value);

   [Benchmark(Baseline = true)]
   [Arguments("230526-034N")]
   [Arguments("160117A275C")]
   public void OriginalValid(String value) => _ = FiHenkilotunnus.Validate(value);

   [Benchmark]
   [Arguments("230526-034N")]
   [Arguments("160117A275C")]
   public void FunctionalValid(String value) => _ = FiHenkilotunnus.Validate2(value);
}
