using KfAccountNumbers.Governmental.NorthAmerica;

namespace KfAccountNumbers.Utility;

/// <summary>
///   Lazy instantiated singleton instances of masks used to validate the check
///   digits for formatted identifiers.
/// </summary>
internal static class CheckDigitMasks
{
   private static readonly Lazy<CaSocialInsuranceNumberMask> _caSocialInsuranceNumberMask =
      new(() => new CaSocialInsuranceNumberMask());

   /// <summary>
   ///   Mask that breaks a Canadian Social Insurance Number into groups of 
   ///   three digits with separator characters between the groups.
   /// </summary>
   public static CaSocialInsuranceNumberMask CaSocialInsuranceNumberMask => _caSocialInsuranceNumberMask.Value;
}
