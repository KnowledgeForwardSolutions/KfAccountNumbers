namespace KfAccountNumbers.Governmental.Europe;

internal class SePersonNumberShortFormatCheckDigitMasks : ICheckDigitMask
{
   private static readonly Lazy<SePersonNumberShortFormatCheckDigitMasks> _instance =
      new(() => new SePersonNumberShortFormatCheckDigitMasks());

   public static SePersonNumberShortFormatCheckDigitMasks Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index) => index == 6;

   public Boolean IncludeCharacter(Int32 index) => index != 6;
}

internal class SePersonNumberLongFormatCheckDigitMasks : ICheckDigitMask
{
   private static readonly Lazy<SePersonNumberLongFormatCheckDigitMasks> _instance =
      new(() => new SePersonNumberLongFormatCheckDigitMasks());

   public static SePersonNumberLongFormatCheckDigitMasks Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index)
      => index == 0 || index == 1 || index == 8;

   public Boolean IncludeCharacter(Int32 index)
      => index != 0 && index != 1 && index != 8;
}
