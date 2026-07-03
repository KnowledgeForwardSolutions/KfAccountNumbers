// Ignore Spelling: Ssn

#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable IDE0008 // Use explicit type

using KfAccountNumbers.Results;

namespace KfAccountNumbers.Tests.Benchmarks.Governmental.NorthAmerica;

[MemoryDiagnoser]
public class UsSocialSecurityNumberBenchmarks
{
   [Benchmark]
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
      CreateResult<UsSocialSecurityNumber, UsSocialSecurityNumber.ValidationError> validatedSsn =
         UsSocialSecurityNumber.Create(ssn);
   }

   [Benchmark]
   [Arguments("012345678")]
   [Arguments("012-34-5678")]
   public void UsSsnValidateMethod(String ssn)
   {
      UsSocialSecurityNumber.ValidationResult isValid = UsSocialSecurityNumber.Validate(ssn);
   }
}
