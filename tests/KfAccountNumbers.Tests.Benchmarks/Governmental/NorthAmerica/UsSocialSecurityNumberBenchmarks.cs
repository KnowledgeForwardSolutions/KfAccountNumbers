// Ignore Spelling: Ssn
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1822 // Mark members as static

namespace KfAccountNumbers.Tests.Benchmarks.Governmental.NorthAmerica;

[MemoryDiagnoser]
public class UsSocialSecurityNumberBenchmarks
{
   [Benchmark()]
   [Arguments("012345678")]
   [Arguments("012-34-5678")]
   public void UsSsnConstructor(String ssn)
   {
      var validatedSsn = new UsSocialSecurityNumber(ssn);
   }

   [Benchmark]
   [Arguments("012345678", '.')]
   [Arguments("012.34.5678", '.')]
   public void UsSsnConstructorWithSeparator(
      String ssn,
      Char separator)
   {
      var validatedSsn = new UsSocialSecurityNumber(ssn, separator);
   }

   [Benchmark]
   [Arguments("012345678")]
   [Arguments("012-34-5678")]
   public void UsSsnCreateMethod(String ssn)
   {
      var validatedSsn = UsSocialSecurityNumber.Create(ssn);
   }

   [Benchmark]
   [Arguments("012345678", '.')]
   [Arguments("012.34.5678", '.')]
   public void UsSsnCreateMethodWithCustomSeparator(
      String ssn,
      Char separator)
   {
      var validatedSsn = UsSocialSecurityNumber.Create(ssn, separator);
   }

   [Benchmark]
   [Arguments("012345678")]
   [Arguments("012-34-5678")]
   public void UsSsnValidateMethod(String ssn)
   {
      var isValid = UsSocialSecurityNumber.Validate(ssn);
   }

   [Benchmark]
   [Arguments("012345678", '.')]
   [Arguments("012.34.5678", '.')]
   public void UsSsnValidateMethodWithCustomSeparator(
      String ssn,
      Char separator)
   {
      var isValid = UsSocialSecurityNumber.Validate(ssn, separator);
   }
}
