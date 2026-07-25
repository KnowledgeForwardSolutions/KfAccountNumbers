#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Strongly typed business object that represents a Swiss Social Security
///   Number (Sozialversicherungsnummer or Neue AVH Nummer).
/// </summary>
/// <remarks>
///   <para>
///      A Sozialversicherungsnummer is an 13-digit number structured as
///      756XXXXXXXXXY , with the following elements:
///      <list type="bullet">
///         <item>
///            <term>756</term>
///            <description>
///               Constant "756", the ISO 3166-1 code for Switzerland
///            </description>
///         </item>
///         <item>
///            <term>XXXXXXXXX</term>
///            <description>
///               Nine random digits.
///            </description>
///         </item>
///         <item>
///            <term>Y</term>
///            <description>
///               Check digit generated using the EAN-13 algorithm.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The 13 character value is often formatted for greater readability by
///      inserting a separator character, generally a period ('.'), at positions
///      3, 8 and 13 (zero-based), i.e. 756.XXXX.XXXX.XY.
///   </para>
///   <para>
///      When creating a new <see cref="ChSozialversicherungsnummer"/>, the
///      following validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The string must be 13 characters long (unformatted) or 16
///               characters long (formatted for readability).
///            </description>
///         </item>
///         <item>
///            <description>
///               All non-separator characters must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing character must be a valid EAN-13 check digit.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the value has length 16, then characters at positions 3, 8
///               and 13 (zero-based) must not be ASCII digits ('0'-'9') and all
///               separator positions must be the same character.
///            </description>
///         </item>
///         <item>
///            <description>
///               The leading three characters must be "756".
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>7560850652826</term>
///            <description>
///               unformatted, check digit = 6
///            </description>
///         </item>
///         <item>
///            <term>756.8814.3009.98</term>
///            <description>
///               formatted, check digit = 8
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A Sozialversicherungsnummer does not encode any personal information.
///   </para>
///   <para>
///      See <see href="https://en.wikipedia.org/wiki/National_identification_number#Switzerland">Wikipedia - National identification number - Switzerland</see>
///      and <see href="https://de.wikipedia.org/wiki/Sozialversicherungsnummer#Versichertennummer">Wikipedia (German) - Sozialversicherungsnummer</see>
///      for more information.
///   </para>
/// </remarks>
public record ChSozialversicherungsnummer
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new Sozialversicherungsnummer.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidPrefix)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating Sozialversicherungsnummer.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidPrefix)
   {
   }

   /// <summary>
   ///   The valid length of an unformatted Sozialversicherungsnummer.
   /// </summary>
   public const Int32 UnformattedLength = 13;

   /// <summary>
   ///   The valid length of a formatted Sozialversicherungsnummer.
   /// </summary>
   public const Int32 FormattedLength = 16;

   /// <summary>
   ///   The names of the check digits algorithm used by
   ///   <see cref="ChSozialversicherungsnummer"/>.
   /// </summary>
   public const String CheckDigitAlgorithmName = "EAN-13";

   /// <summary>
   ///   The default format to use when formatting
   ///   <see cref="ChSozialversicherungsnummer"/> values.
   /// </summary>
   public const String DefaultFormatMask = "___.____.____.__";

   /// <summary>
   ///   Zero based offset of the first separator.
   /// </summary>
   internal const Int32 FirstSeparatorOffset = 3;

   /// <summary>
   ///   Zero based offset of the second separator.
   /// </summary>
   internal const Int32 SecondSeparatorOffset = 8;

   /// <summary>
   ///   Zero based offset of the third separator.
   /// </summary>
   internal const Int32 ThirdSeparatorOffset = 13;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a valid
   ///   Sozialversicherungsnummer.
   /// </summary>
   /// <param name="value">
   ///   String representation of a Sozialversicherungsnummer.
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

      if (value.Length is not UnformattedLength and not FormattedLength)
      {
         return GetInvalidLengthResult(value);
      }

      // After performing basic checks, validate the check digit because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      var validCheckDigit = IsFormatted(value)
         ? MaskedAlgorithms.Modulus10_13.Validate(value, ChSozialversicherungsnummerNumberCheckDigitMask.Instance)
         : Algorithms.Modulus10_13.Validate(value);
      if (!validCheckDigit)
      {
         // Either invalid check digit or invalid character encountered.
         var invalidCharacterOffset = LocateInvalidCharacter(value);
         return invalidCharacterOffset == -1
            ? GetInvalidChecksumResult()
            : GetInvalidCharacterResult(value, invalidCharacterOffset);
      }

      if (!ValidateSeparators(value, out var invalidSeparatorPosition))
      {
         return GetInvalidSeparatorResult(value, invalidSeparatorPosition);
      }

      if (!ValidatePrefix(value))
      {
         return GetInvalidPrefixResult(value);
      }

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.ChSozialversicherungsnummerInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.ChSozialversicherungsnummerInvalidCheckDigit, CheckDigitAlgorithmName);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.ChSozialversicherungsnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(UnformattedLength, Messages.ChSozialversicherungsnummerUnformattedLength),
            new ValidLengthDefinition(FormattedLength, Messages.ChSozialversicherungsnummerFormattedLength),
         ]);

   private static InvalidPrefix GetInvalidPrefixResult(ReadOnlySpan<Char> value)
      => new(
         Messages.ChSozialversicherungsnummerInvalidPrefix,
         value[..3].ToString());

   private static InvalidSeparator GetInvalidSeparatorResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(
         Messages.ChSozialversicherungsnummerInvalidSeparator,
         value[position],
         position);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> value) => value.Length == FormattedLength;

   // Return the zero-based index of the first non-digit character (excluding
   // separators) or -1 if no non-digit characters found.
   private static Int32 LocateInvalidCharacter(ReadOnlySpan<Char> value)
   {
      var isFormatted = IsFormatted(value);
      for (var index = 0; index < value.Length; index++)
      {
         if (isFormatted && index is FirstSeparatorOffset or SecondSeparatorOffset or ThirdSeparatorOffset)
         {
            continue;
         }

         if (!Char.IsAsciiDigit(value[index]))
         {
            return index;
         }
      }

      return -1;
   }

   private static Boolean ValidatePrefix(ReadOnlySpan<Char> value)
   {
      const String requiredPrefix = "756";

      return value[..3].Equals(requiredPrefix, StringComparison.Ordinal);
   }

   private static Boolean ValidateSeparators(
      ReadOnlySpan<Char> value,
      out Int32 invalidSeparatorOffset)
   {
      invalidSeparatorOffset = -1;
      if (value.Length == UnformattedLength)
      {
         return true;
      }

      var firstSeparator = value[FirstSeparatorOffset];
      if (firstSeparator.IsAsciiDigit())
      {
         invalidSeparatorOffset = FirstSeparatorOffset;
         return false;
      }

      if (value[SecondSeparatorOffset] != firstSeparator)
      {
         invalidSeparatorOffset = SecondSeparatorOffset;
         return false;
      }

      if (value[ThirdSeparatorOffset] != firstSeparator)
      {
         invalidSeparatorOffset = ThirdSeparatorOffset;
         return false;
      }

      return true;
   }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
//public class ChSozialversicherungsnummerJsonConverter : JsonConverter<ChSozialversicherungsnummer>
//{
//   public override ChSozialversicherungsnummer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//   {
//      if (reader.TokenType == JsonTokenType.Null)
//      {
//         return null!;
//      }

//      var str = reader.GetString();
//      return new ChSozialversicherungsnummer(str);
//   }

//   public override void Write(Utf8JsonWriter writer, ChSozialversicherungsnummer value, JsonSerializerOptions options)
//      => writer.WriteStringValue(value.Value);
//}

internal class ChSozialversicherungsnummerNumberCheckDigitMask : ICheckDigitMask
{
   private static readonly Lazy<ChSozialversicherungsnummerNumberCheckDigitMask> _instance =
      new(() => new ChSozialversicherungsnummerNumberCheckDigitMask());

   public static ChSozialversicherungsnummerNumberCheckDigitMask Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index)
      => index is ChSozialversicherungsnummer.FirstSeparatorOffset or ChSozialversicherungsnummer.SecondSeparatorOffset or ChSozialversicherungsnummer.ThirdSeparatorOffset;

   public Boolean IncludeCharacter(Int32 index) => !ExcludeCharacter(index);
}
