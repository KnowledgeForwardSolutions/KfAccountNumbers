#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

using CheckDigits.Net.Utility;

namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Strongly typed business object that represents a Luxembourg National
///   Identification Number (numéro d'identification), commonly called the
///   matricule.
/// </summary>
/// <remarks>
///   <para>
///      A matricule is an 13-digit number structured as YYYYMMDDXXXLV, with the
///      following elements:
///      <list type="bullet">
///         <item>
///            <term>YYYYMMDD</term>
///            <description>
///               Eight digit date of birth in YYYYMMDD format.
///            </description>
///         </item>
///         <item>
///            <term>XXX</term>
///            <description>
///               Three digits used to differentiate between persons born on the
///               same day.
///            </description>
///         </item>
///         <item>
///            <term>L</term>
///            <description>
///               Check digit calculated using the Luhn algorithm on the initial
///               11 digits.
///            </description>
///         </item>
///         <item>
///            <term>V</term>
///            <description>
///               Check digit calculated using the Verhoeff algorithm on the
///               initial 11 digits.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      When creating a new <see cref="LuMatricule"/>, the following validation
///      rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The string must be 13 characters long.
///            </description>
///         </item>
///         <item>
///            <description>
///               All characters must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The character at position 11 (zero-based) must be a valid Luhn
///               algorithm check digit.
///            </description>
///         </item>
///         <item>
///            <description>
///               The character at position 12 (zero-based) must be a valid
///               Verhoeff algorithm check digit.
///            </description>
///         </item>
///         <item>
///            <description>
///               The leading eight characters must be a valid date of birth in
///               YYYYMMDD format.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>1960090900163</term>
///            <description>
///               date of birth = September 9, 1960, Luhn checkdigit = 6,
///               Verhoeff check digit = 3
///            </description>
///         </item>
///         <item>
///            <term>1985011500173</term>
///            <description>
///               date of birth = January 15, 1985, Luhn checkdigit = 7,
///               Verhoeff check digit = 3
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Both check digits are calculated independently on the same 11 initial
///      digits, with the Verhoeff algorithm not including the Luhn result in
///      its calculation. The use of two different check digit algorithms
///      results in greater error detection capability than either algorithm
///      alone.
///   </para>
///   <para>
///      See <see href="https://www.lbr.lu/mjrcs/jsp/webapp/static/mjrcs/en/mjrcs/pdf/FAQ_National_Identification_Number.pdf">Luxembourg Business Registers - Frequently Asked Questions</see>
///      and <see href="https://lookuptax.com/docs/tax-identification-number/luxembourg-tax-id-guide">Luxembourg Tax ID Guide - Matricule, No. TVA &amp; RCS Number Explained</see>
///      for more information.
///   </para>
/// </remarks>
public record LuMatricule
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new Luxembourg matricule.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating Luxembourg matricule.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidDateOfBirth)
   {
   }

   /// <summary>
   ///   The valid length of a Luxembourg matricule.
   /// </summary>
   public const Int32 ValidLength = 13;

   /// <summary>
   ///   The names of the two check digits algorithms used by
   ///   <see cref="LuMatricule"/>.
   /// </summary>
   public const String CheckDigitAlgorithmNames = "Luhn, Verhoeff";

   /// <summary>
   ///   Zero-based offset of the Luhn check digit.
   /// </summary>
   internal const Int32 LuhnOffset = 11;

   private const Int32 DayOffset = 6;
   private const Int32 MonthOffset = 4;
   private const Int32 VerhoeffOffset = 12;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a valid
   ///   Luxembourg matricule.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Luxembourg matricule.
   /// </param>
   /// <returns>
   ///   A <see cref="ValidationResult"/> union that indicates if the
   ///   <paramref name="value"/> passed validation or what validation error was
   ///   encountered.
   /// </returns>
   public static ValidationResult Validate(String? value)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         return default(EmptyValue);
      }

      if (value.Length is not ValidLength)
      {
         return GetInvalidLengthResult(value);
      }

      // After performing basic checks, validate the check digit because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      ValidationResult validationResult = ValidateCheckDigits(value);
      if (validationResult is not ValidValue)
      {
         // Could be either InvalidCharacter or InvalidChecksum.
         return validationResult;
      }

      if (!ValidateDateOfBirth(value))
      {
         return GetInvalidDateOfBirthResult(value);
      }

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.LuMatriculeInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.LuMatriculeInvalidCheckDigits, CheckDigitAlgorithmNames);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(ReadOnlySpan<Char> value)
      => new(
            Messages.LuMatriculeInvalidDateOfBirth,
            value[..8].ToString(),
            DateFormatName.YYYYMMDD);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.LuMatriculeInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(ValidLength, Messages.LuMatriculeLength),
         ]);

   private static (Int32 Year, Int32 Month, Int32 Day) GetYearMonthDay(ReadOnlySpan<Char> value)
   {
      var year = value.ParseFourDigits();
      var month = value[MonthOffset..].ParseTwoDigits();
      var day = value[DayOffset..].ParseTwoDigits();

      return (year, month, day);
   }

   // Port CheckDigits.Net Luhn and Verhoeff validation into a single method to
   // perform both validation checks in a single pass.
   private static ValidationResult ValidateCheckDigits(String value)
   {
      VerhoeffPermutationTable verhoeffPermutationTable = VerhoeffPermutationTable.Instance;
      VerhoeffMultiplicationTable verhoeffMultiplicationTable = VerhoeffMultiplicationTable.Instance;

      var vC = 0;       // Verhoeff c parameter
      var vI = 0;       // Verhoeff i parameter
      //var vP = 0;       // Verhoeff p parameter
      var luhnSum = 0;
      var oddPosition = true;

      // Set up the Verhoeff algorithm by processing the Verhoeff check digit.
      var digit = value[VerhoeffOffset].ToSingleDigit();
      if (!digit.IsValidDigit())
      {
         return GetInvalidCharacterResult(value, VerhoeffOffset);
      }

      var vP = verhoeffPermutationTable[vI % 8, digit];
      vC = verhoeffMultiplicationTable[vC, vP];
      vI++;

      // Initialize the Luhn check digit as well.
      var luhnCheckDigit = value[LuhnOffset].ToSingleDigit();
      if (!luhnCheckDigit.IsValidDigit())
      {
         return GetInvalidCharacterResult(value, LuhnOffset);
      }

      // Process the remaining 11 digits for both Luhn and Verhoeff algorithms.
      for (var index = LuhnOffset - 1; index >= 0; index--)
      {
         digit = value[index].ToSingleDigit();
         if (!digit.IsValidDigit())
         {
            return GetInvalidCharacterResult(value, index);
         }

         // Luhn intermediate calculations.
         luhnSum += oddPosition
            ? digit > 4 ? (digit * 2) - 9 : digit * 2
            : digit;
         oddPosition = !oddPosition;

         // Verhoeff intermediate calculations.
         vP = verhoeffPermutationTable[vI % 8, digit];
         vC = verhoeffMultiplicationTable[vC, vP];
         vI++;
      }

      var calculatedLuhnCheckDigit = (10 - (luhnSum % 10)) % 10;

      return calculatedLuhnCheckDigit == luhnCheckDigit && vC == 0
         ? default(ValidValue)
         : GetInvalidChecksumResult();
   }

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> value)
   {
      // Manual validation is faster than using DateTime.TryParseExact.
#pragma warning disable IDE0008 // Use explicit type
      var (year, month, day) = GetYearMonthDay(value);
      #pragma warning restore IDE0008 // Use explicit type

      if (month is < 1 or > 12)
      {
         return false;
      }

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
//public class LuMatriculeJsonConverter : JsonConverter<LuMatricule>
//{
//   public override LuMatricule Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//   {
//      if (reader.TokenType == JsonTokenType.Null)
//      {
//         return null!;
//      }

//      var str = reader.GetString();
//      return new LuMatricule(str);
//   }

//   public override void Write(Utf8JsonWriter writer, LuMatricule value, JsonSerializerOptions options)
//      => writer.WriteStringValue(value.Value);
//}
