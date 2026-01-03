namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Mask that breaks a Canadian Social Insurance Number into groups of three
///   digits with separator characters between the groups.
/// </summary>
internal class CaSocialInsuranceNumberMask : ICheckDigitMask
{
   public Boolean ExcludeCharacter(Int32 index) => index == 3 || index == 7;

   public Boolean IncludeCharacter(Int32 index) => index != 3 && index != 7;
}

