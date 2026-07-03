namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Abstract base class for either of the identifiers issued by the Swedish
///   Tax Agency, the personnummer issued to permanent residents of Sweden and
///   the samordningsnummer issued to temporary residents.
/// </summary>
public abstract record SeIdentityNumberBase
{
   /// <summary>
   ///   The latest year of birth supported by Swedish identity numbers.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2099;

   /// <summary>
   ///   The earliest year of birth supported by Swedish identity numbers.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1800;

   /// <summary>
   ///   Represents the day offset used to distinguish Swedish coordination
   ///   numbers (samordningsnummer) from personnummers.
   /// </summary>
   /// <remarks>
   ///   In Swedish personal identity numbers, a Samordningsnummer is indicated
   ///   by adding 60 to the day component of the date of birth.
   /// </remarks>
   public const Int32 SamordningsnummerDayOffset = 60;

   private const Int32 InternalRepresentationLength = 12;      // YYYYMMDD + birth serial number + check digit
   private const Int32 ShortFormatLength = 11;
   private const Int32 LongFormatLength = 13;

   // These offsets are measured from the end of the string instead of the start
   // because the date of birth has variable length.
   private const Int32 SeparatorOffset = 5;
   private const Int32 GenderOffset = 2;

}
