namespace KfAccountNumbers.Governmental.Europe;

internal class SePersonNumberShortFormatCheckDigitMask : ICheckDigitMask
{
   private static readonly Lazy<SePersonNumberShortFormatCheckDigitMask> _instance =
      new(() => new SePersonNumberShortFormatCheckDigitMask());

   public static SePersonNumberShortFormatCheckDigitMask Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index) => index == 6;

   public Boolean IncludeCharacter(Int32 index) => index != 6;
}

internal class SePersonNumberLongFormatCheckDigitMask : ICheckDigitMask
{
   private static readonly Lazy<SePersonNumberLongFormatCheckDigitMask> _instance =
      new(() => new SePersonNumberLongFormatCheckDigitMask());

   public static SePersonNumberLongFormatCheckDigitMask Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index)
      => index == 0 || index == 1 || index == 8;

   public Boolean IncludeCharacter(Int32 index)
      => index != 0 && index != 1 && index != 8;
}
