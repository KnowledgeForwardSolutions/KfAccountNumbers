#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable IDE0008 // Use explicit type

namespace KfAccountNumbers.Tests.Benchmarks.Governmental.NorthAmerica;

[MemoryDiagnoser]
public class CaSocialInsuranceNumberBenchmarks
{
   [Benchmark]
   [Arguments("558199428")]
   [Arguments("558-199-428")]
   public void CaSinConstructor(String sin)
   {
      var validatedSin = new CaSocialInsuranceNumber(sin);
   }

   [Benchmark]
   [Arguments("558199428")]
   [Arguments("558-199-428")]
   public void CaSinCreateMethod(String sin)
   {
      var validatedSin = CaSocialInsuranceNumber.Create(sin);
   }

   [Benchmark]
   [Arguments("558199428")]
   [Arguments("558-199-428")]
   public void CaSinValidateMethod(String sin)
   {
      var isValid = CaSocialInsuranceNumber.Validate(sin);
   }
}
