// Ignore Spelling: Curp Mx

#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1822 // Mark members as static

namespace KfAccountNumbers.Tests.Benchmarks.Governmental.NorthAmerica;

[MemoryDiagnoser]
public class MxCurpBenchmarks
{
   [Benchmark()]
   [Arguments("HEGG560427MVZRRL04")]
   [Arguments("hegg560427mvzrll04")]
   public void MxCurpValidateMethod(String curp)
   {
      MxCurpValidationResult result = MxCurp.Validate(curp);
   }
}
