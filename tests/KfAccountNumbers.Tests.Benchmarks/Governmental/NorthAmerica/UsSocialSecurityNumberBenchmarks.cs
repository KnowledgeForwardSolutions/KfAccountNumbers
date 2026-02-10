// Ignore Spelling: Ssn
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1822 // Mark members as static

using KfAccountNumbers.Utility;

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
   [Arguments("012345678")]
   [Arguments("012-34-5678")]
   public void UsSsnCreateMethod(String ssn)
   {
      CreateResult<UsSocialSecurityNumber, UsSocialSecurityNumberValidationResult> validatedSsn =
         UsSocialSecurityNumber.Create(ssn);
   }

   [Benchmark]
   [Arguments("012345678")]
   [Arguments("012-34-5678")]
   public void UsSsnValidateMethod(String ssn)
   {
      UsSocialSecurityNumberValidationResult isValid = UsSocialSecurityNumber.Validate(ssn);
   }
}
