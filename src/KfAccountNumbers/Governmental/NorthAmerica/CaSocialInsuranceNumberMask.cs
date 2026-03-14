namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Mask that breaks a Canadian Social Insurance Number into groups of three
///   digits with separator characters between the groups.
/// </summary>
internal class CaSocialInsuranceNumberMask : ICheckDigitMask
{
   private static readonly Lazy<CaSocialInsuranceNumberMask> _instance =
      new(() => new CaSocialInsuranceNumberMask());

   public static CaSocialInsuranceNumberMask Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index) => index == 3 || index == 7;

   public Boolean IncludeCharacter(Int32 index) => index != 3 && index != 7;
}

