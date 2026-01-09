#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1822 // Mark members as static

namespace KfAccountNumbers.Tests.Benchmarks.Governmental.NorthAmerica;

[MemoryDiagnoser]
public class CaSocialInsuranceNumberBenchmarks
{
   [Benchmark()]
   [Arguments("558199428")]
   [Arguments("558-199-428")]
   public void CaSinConstructor(String sin)
   {
      var validatedSin = new CaSocialInsuranceNumber(sin);
   }

   [Benchmark]
   [Arguments("558199428", '.')]
   [Arguments("558.199.428", '.')]
   public void CaSinConstructorWithSeparator(
      String sin,
      Char separator)
   {
      var validatedSin = new CaSocialInsuranceNumber(sin, separator);
   }

   [Benchmark]
   [Arguments("558199428")]
   [Arguments("558-199-428")]
   public void CaSinCreateMethod(String sin)
   {
      var validatedSin = CaSocialInsuranceNumber.Create(sin);
   }

   [Benchmark]
   [Arguments("558199428", '.')]
   [Arguments("558.199.428", '.')]
   public void CaSinCreateMethodWithCustomSeparator(
      String sin,
      Char separator)
   {
      var validatedSin = CaSocialInsuranceNumber.Create(sin, separator);
   }

   [Benchmark()]
   [Arguments("558199428")]
   [Arguments("558-199-428")]
   public void CaSinValidateMethod(String sin)
   {
      var isValid = CaSocialInsuranceNumber.Validate(sin);
   }

   [Benchmark()]
   [Arguments("558199428", '.')]
   [Arguments("558.199.428", '.')]
   public void CaSinValidateMethodWithCustomSeparator(
      String sin,
      Char separator)
   {
      var isValid = CaSocialInsuranceNumber.Validate(sin, separator);
   }
}
