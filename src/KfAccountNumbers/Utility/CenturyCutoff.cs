namespace KfAccountNumbers.Utility;

/// <summary>
///   Represents a century cutoff value used to determine the century for
///   two-digit years.
/// </summary>
/// <remarks>
///   The cutoff value must be between 1 and 100 (inclusive). When converting
///   two-digit years to four-digit years:
///   - Years less than the cutoff receive a century prefix of 2000
///   - Years greater than or equal to the cutoff receive a century prefix of 1900.
///   <para>
///      Common cutoff values:
///      <list type="bullet">
///         <item>
///            <description>
///               50 (default): Years 00-49 → 2000-2049; Years 50-99 → 1950-1999.
///               (This matches SQL Server conversion of two-digit dates.)
///            </description>
///         </item>
///         <item>
///            <description>
///               30: Years 00-29 → 2000-2029; Years 30-99 → 1930-1999
///            </description>
///         </item>
///         <item>
///            <description>
///               1: Years 00 → 2000; Years 01-99 → 1901-1999 (maximum range in
///               the 1900s)
///            </description>
///         </item>
///         <item>
///            <description>
///               100: Years 00-99 → 2000-2099 (all years in the 2000s)
///            </description>
///         </item>
///      </list>
///   </para>
/// </remarks>
public record CenturyCutoff
{
   /// <summary>
   ///   The default century cutoff used when the cutoff is not specified.
   /// </summary>
   public const Int32 DefaultCutoff = 50;

   /// <summary>
   ///   The minimum valid century cutoff value.
   /// </summary>
   internal const Int32 CutoffMinValue = 1;

   /// <summary>
   ///   The maximum valid century cutoff value.
   /// </summary>
   internal const Int32 CutoffMaxValue = 100;

   /// <summary>
   ///   Initializes a new instance of the <see cref="CenturyCutoff"/> class
   ///   with the specified cutoff value.
   /// </summary>
   /// <param name="cutoff">
   ///   Optional. The century cutoff value. Defaults to <see cref="DefaultCutoff"/>.
   /// </param>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="cutoff"/> is outside the valid range.
   /// </exception>
   public CenturyCutoff(Int32 cutoff = DefaultCutoff)
      => Cutoff = cutoff is >= CutoffMinValue and <= CutoffMaxValue
         ? cutoff
         : throw new ArgumentOutOfRangeException(nameof(cutoff), cutoff, Messages.InvalidCenturyCutoff);

   /// <summary>
   ///   Gets the cutoff value.
   /// </summary>
   public Int32 Cutoff { get; private init; }

   /// <summary>
   ///   Explicitly converts an <see cref="Int32"/> to a
   ///   <see cref="CenturyCutoff"/>.
   /// </summary>
   /// <param name="cutoff">
   ///   The century cutoff value.
   /// </param>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="cutoff"/> is outside the valid range.
   /// </exception>
   public static explicit operator CenturyCutoff(Int32 cutoff) => new(cutoff);

   /// <summary>
   ///   Converts the specified year to a four-digit year. Year values between
   ///   00 and 99 will apply the century cutoff and return a four-digit year
   ///   between 1900 and 2099. Year values between 1000 and 9999 will return
   ///   the year value unchanged. Year values outside these ranges will throw
   ///   an <see cref="ArgumentOutOfRangeException"/>.
   /// </summary>
   /// <param name="year">
   ///   A 2-digit (00-99) or 4-digit (1000-9999) integer that represents the
   ///   year to convert.
   /// </param>
   /// <returns>
   ///   A 4-digit integer that represents <paramref name="year"/>.
   /// </returns>
   /// <remarks>
   ///   <para>
   ///      With the default cutoff of 50:
   ///      <list type="bullet">
   ///         <item>ToFourDigitYear(0) returns 2000</item>
   ///         <item>ToFourDigitYear(25) returns 2025</item>
   ///         <item>ToFourDigitYear(50) returns 1950</item>
   ///         <item>ToFourDigitYear(99) returns 1999</item>
   ///         <item>ToFourDigitYear(2025) returns 2025</item>
   ///      </list>
   ///   </para>
   ///   <para>
   ///      Note that the minimum valid cutoff value is 1, so a year value of
   ///      0 will always convert to 2000.
   ///   </para>
   /// </remarks>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="year"/> is not between 0-99 or 1000-9999.
   /// </exception>
   public Int32 ToFourDigitYear(Int32 year)
      => year switch
      {
         >= 0 and <= 99 => year + (year < Cutoff ? 2000 : 1900),
         >= 1000 and <= 9999 => year,
         _ => throw new ArgumentOutOfRangeException(nameof(year), year, Messages.InvalidYearForYyToYyyyConversion),
      };
}
