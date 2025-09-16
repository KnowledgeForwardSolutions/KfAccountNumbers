// Ignore Spelling: Ssn

namespace KfAccountNumbers.Tests.Benchmarks.Governmental.NorthAmerica;

[MemoryDiagnoser]
public class UsSocialSecurityNumberBenchmarks
{
   [Params("012345678", "012-34-5678")]
   public String Ssn { get; set; } = default!;

   [Benchmark(Baseline = true)]
   public void UsSocialSecurityNumberConstructor()
   {
      var validatedSsn = new UsSocialSecurityNumber(Ssn);
   }

   //[Benchmark]
   //public void UsSsn2Constructor()
   //{
   //   var validatedSsn = new UsSsn2(Ssn);
   //}

   //[Benchmark]
   //public void UsSsn3Constructor()
   //{
   //   var validatedSsn = new UsSsn3(Ssn);
   //}
}
