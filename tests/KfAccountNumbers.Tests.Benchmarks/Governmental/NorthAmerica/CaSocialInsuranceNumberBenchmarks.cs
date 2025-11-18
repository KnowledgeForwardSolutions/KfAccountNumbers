namespace KfAccountNumbers.Tests.Benchmarks.Governmental.NorthAmerica;

[MemoryDiagnoser]
public class CaSocialInsuranceNumberBenchmarks
{
   [Benchmark(Baseline = true)]
   [Arguments("558199428")]
   [Arguments("558-199-428")]
   public void CaSinValidateMethod(String sin)
   {
      var isValid = CaSocialInsuranceNumber.Validate(sin);
   }

   //[Benchmark()]
   //[Arguments("558199428")]
   //[Arguments("558-199-428")]
   //public void CaSinValidate2Method(String sin)
   //{
   //   var isValid = CaSocialInsuranceNumber.Validate2(sin);
   //}
}
