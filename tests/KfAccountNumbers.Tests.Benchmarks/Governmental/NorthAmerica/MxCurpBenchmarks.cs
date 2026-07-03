// Ignore Spelling: Curp Mx

#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable IDE0008 // Use explicit type

namespace KfAccountNumbers.Tests.Benchmarks.Governmental.NorthAmerica;

[MemoryDiagnoser]
public class MxCurpBenchmarks
{
   [Benchmark]
   [Arguments("HEGG560427MVZRRL04")]
   [Arguments("hegg560427mvzrll04")]
   public void MxCurpValidateMethod(String curp)
   {
      MxCurp.ValidationResult result = MxCurp.Validate(curp);
   }
}
