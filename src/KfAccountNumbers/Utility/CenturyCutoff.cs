namespace KfAccountNumbers.Utility;

/// <summary>
///   Represents a century cutoff value used to determine the century for
///   two-digit years.
/// </summary>
/// <remarks>
///   The cutoff value must be between 1 and 100 (inclusive). When converting
///   two-digit years to four-digit years:
///   <list type="bullet">
///      <item>
///         <description>
///            Years less than the <see cref="Cutoff"/> evaluate as belonging to
///            <see cref="CurrentCentury"/>.
///         </description>
///      </item>
///      <item>
///         <description>
///            Years greater than or equal to the cutoff evaluate as belonging
///            to <see cref="PreviousCentury"/>.
///         </description>
///      </item>
///   </list>
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
   ///   The minimum valid century cutoff value.
   /// </summary>
   internal const Int32 CutoffMinValue = 1;

   /// <summary>
   ///   The maximum valid century cutoff value.
   /// </summary>
   internal const Int32 CutoffMaxValue = 100;

   private static readonly Lazy<CenturyCutoff> _defaultInstance = new(() => new CenturyCutoff());

   /// <summary>
   ///   Initializes a new instance of the <see cref="CenturyCutoff"/> class
   ///   with the specified cutoff value.
   /// </summary>
   /// <param name="cutoff">
   ///   Optional. The century cutoff value. Defaults to 50.
   /// </param>
   /// <param name="currentCentury">
   ///   Optional. The assumed current century, i.e. the century value to apply
   ///   to two-digit years that are less than <paramref name="cutoff"/>.
   ///   Defaults to 2000.
   /// </param>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="cutoff"/> is outside the valid range.
   ///   - or -
   ///   <paramref name="currentCentury"/> is not a multiple of 100, i.e. not
   ///   1900, 2000, etc.
   /// </exception>
   public CenturyCutoff(
      Int32 cutoff = 50,
      Int32 currentCentury = 2000)
   {
      Cutoff = cutoff is >= CutoffMinValue and <= CutoffMaxValue
         ? cutoff
         : throw new ArgumentOutOfRangeException(nameof(cutoff), cutoff, Messages.InvalidCenturyCutoff);

      if (currentCentury % 100 != 0)
      {
         throw new ArgumentOutOfRangeException(
            nameof(currentCentury),
            currentCentury,
            Messages.CurrentCenturyInvalidModulus);
      }

      CurrentCentury = currentCentury;
      PreviousCentury = currentCentury - 100;
   }

   /// <summary>
   ///   Gets the assumed century when a two-digit year is less than the
   ///   <see cref="Cutoff"/>.
   /// </summary>
   /// <remarks>
   ///   <para>
   ///      The default value is 2000.
   ///   </para>
   ///   <para>
   ///      Assuming <see cref="Cutoff"/> = 50 and <see cref="CurrentCentury"/>
   ///      = 1900 then the two-digit year 45 will evaluate to 1945 and the
   ///      two-digit year 90 will evaluate to 1890.
   ///   </para>
   /// </remarks>
   public Int32 CurrentCentury { get; private init; }

   /// <summary>
   ///   Gets the cutoff value.
   /// </summary>
   public Int32 Cutoff { get; private init; }

   /// <summary>
   ///   Gets a lazily created singleton instance of <see cref="CenturyCutoff"/>
   ///   with all default values; i.e. <see cref="Cutoff"/> = 50 and
   ///   <see cref="CurrentCentury"/> = 2000.
   /// </summary>
   public static CenturyCutoff DefaultInstance => _defaultInstance.Value;

   /// <summary>
   ///   Gets the assumed century when a two-digit year is greater than or equal
   ///   to the <see cref="Cutoff"/>.
   /// </summary>
   /// <remarks>
   ///   <see cref="PreviousCentury"/> is automatically initialized to be
   ///   <see cref="CurrentCentury"/> - 100.
   /// </remarks>
   public Int32 PreviousCentury { get; private init; }

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
   ///   between <see cref="PreviousCentury"/> and <see cref="CurrentCentury"/>
   ///   + 99. Year values between 1000 and 9999 will return the year value
   ///   unchanged. Year values outside these ranges will throw an
   ///   <see cref="ArgumentOutOfRangeException"/>.
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
   ///      With the default <see cref="Cutoff"/> of 50 and default
   ///      <see cref="CurrentCentury"/> of 2000, then:
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
   ///      0 will always convert to <see cref="CurrentCentury"/>.
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
