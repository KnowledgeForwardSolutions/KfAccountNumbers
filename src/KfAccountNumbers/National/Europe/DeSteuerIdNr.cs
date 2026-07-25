#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Strongly typed business object that represents a German identification
///   number (Steuerliche Identifikationsnummer or Steuer-IdNr).
/// </summary>
/// <remarks>
///   <para>
///      A Steuer-IdNr is an 11-digit number structured as DDDDDDDDDDC, with the
///      following elements:
///      <list type="bullet">
///         <item>
///            <term>DDDDDDDDDD</term>
///            <description>
///               Ten random digits.
///            </description>
///         </item>
///         <item>
///            <term>C</term>
///            <description>
///               Check digit generated using the ISO/IEC 7064, MOD 11,10
///               algorithm.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The 11 character value is sometimes formatted for greater readability
///      by inserting a separator character, generally a space, at positions 2,
///      6 and 10 (zero-based), i.e. DD DDD DDD DDC.
///   </para>
///   <para>
///      When creating a new <see cref="DeSteuerIdNr"/>, the following validation
///      rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The string must be either 11 or 14 characters long.
///            </description>
///         </item>
///         <item>
///            <description>
///               All non-separator characters must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing character must be a valid ISO/IEC 7064 MOD 11,10
///               check digit.
///            </description>
///         </item>
///         <item>
///            <description>
///               If the value has length 14, then characters at positions 2, 6
///               and 10 (zero-based) must not be ASCII digits ('0'-'9') and all
///               separator positions must be the same character
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>43957380212</term>
///            <description>
///               unformatted
///            </description>
///         </item>
///         <item>
///            <term>25 986 078 148</term>
///            <description>
///               formatted
///            </description>
///         </item>
///         <item>
///            <term>91 215 743 612</term>
///            <description>
///               formatted
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      A Steuer-IdNr does not encode any personal information.
///   </para>
///   <para>
///      See <see href="https://de.wikipedia.org/wiki/Steuerliche_Identifikationsnummer">Wikipedia (German) - Steuerliche Identifikationsnummer</see>
///      for more information.
///   </para>
/// </remarks>
[JsonConverter(typeof(DeSteuerIdNrJsonConverter))]
public record DeSteuerIdNr
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new German Steuer-IdNr.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating German Steuer-IdNrs.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator)
   {
   }

   /// <summary>
   ///   The valid length of an unformatted German Steuer-IdNr.
   /// </summary>
   public const Int32 UnformattedLength = 11;

   /// <summary>
   ///   The valid length of a formatted German Steuer-IdNr.
   /// </summary>
   public const Int32 FormattedLength = 14;

   /// <summary>
   ///   The default format to use when formatting <see cref="DeSteuerIdNr"/>
   ///   values.
   /// </summary>
   public const String DefaultFormatMask = "__/___/___/___";

   /// <summary>
   ///   Zero based offset of the first separator.
   /// </summary>
   internal const Int32 FirstSeparatorOffset = 2;

   /// <summary>
   ///   Zero based offset of the second separator.
   /// </summary>
   internal const Int32 SecondSeparatorOffset = 6;

   /// <summary>
   ///   Zero based offset of the third separator.
   /// </summary>
   internal const Int32 ThirdSeparatorOffset = 10;

   /// <summary>
   ///   Initializes a new instance of the <see cref="DeSteuerIdNr"/>
   ///   class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a German Steuer-IdNr.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 11 (or 14 if separator
   ///   characters are used).
   ///   - or -
   ///   <paramref name="value"/> contains a non-digit character in any position
   ///   other than the separator locations.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid ISO/IEC 7064 MOD 11,10
   ///   check digit in the trailing (right-most) character position.
   ///   - or -
   ///   <paramref name="value"/> is 14 characters in length and has an ASCII
   ///   digit ('0'-'9') in a separator location
   ///   - or -
   ///   <paramref name="value"/> is 14 characters in length and has two
   ///   different separator characters.
   /// </exception>
   public DeSteuerIdNr(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="DeSteuerIdNr"/>
   ///   class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private DeSteuerIdNr(String? value, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         ValidationResult validationResult = Validate(value);
         if (validationResult.Value is not ValidValue)
         {
            throw validationResult switch
            {
               EmptyValue emptyValue => new UKfValidationException<ValidationError>(emptyValue),
               InvalidLength invalidLength => new UKfValidationException<ValidationError>(invalidLength),
               InvalidCharacter invalidCharacter => new UKfValidationException<ValidationError>(invalidCharacter),
               InvalidChecksum invalidChecksum => new UKfValidationException<ValidationError>(invalidChecksum),
               InvalidSeparator invalidSeparator => new UKfValidationException<ValidationError>(invalidSeparator),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetRawValue(value!);
   }

   /// <summary>
   ///   Gets the raw Steuer-IdNr value.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="DeSteuerIdNr"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="DeSteuerIdNr"/> to convert.
   /// </param>
   public static implicit operator String(DeSteuerIdNr source)
      => source?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="DeSteuerIdNr"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a German Steuer-IdNr.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid Steuer-IdNr.
   /// </exception>
   public static explicit operator DeSteuerIdNr(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="DeSteuerIdNr"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a German Steuer-IdNr.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{DeSteuerIdNr, ValidationError}"/>. Will
   ///   contain the new <see cref="DeSteuerIdNr"/> if <paramref name="value"/>
   ///   is valid or a <see cref="ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static CreateResult<DeSteuerIdNr, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new DeSteuerIdNr(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the German Steuer-IdNr using the supplied
   ///   <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then <see cref="DefaultFormatMask"/> will be used instead.
   /// </param>
   /// <returns>
   ///   A formatted German Steuer-IdNr.
   /// </returns>
   /// <exception cref="ArgumentNullException">
   ///   <paramref name="mask"/> is <see langword="null"/>.
   /// </exception>
   /// <exception cref="ArgumentException">
   ///   <paramref name="mask"/> is <see cref="String.Empty"/> or all whitespace
   ///   characters.
   /// </exception>
   /// <remarks>
   ///   <see cref="ExtensionMethods.FormatWithMask(String, String)"/> for more
   ///   details on creating a mask to format the German Steuer-IdNr.
   /// </remarks>
   public String Format(String mask = DefaultFormatMask) => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the German Steuer-IdNr.
   /// </summary>
   /// <returns>
   ///   The raw Steuer-IdNr, without separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a valid
   ///   German Steuer-IdNr.
   /// </summary>
   /// <param name="value">
   ///   String representation of a German Steuer-IdNr.
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
         ? ValidateMaskedCheckDigit(value, DeSteuerIdNrNumberCheckDigitMask.Instance)
         : Algorithms.Iso7064Mod11_10.Validate(value);
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

      return default(ValidValue);
   }

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(Messages.DeSteuerIdNrInvalidCharacter, value[position], position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(Messages.DeSteuerIdNrInvalidCheckDigit, Algorithms.Iso7064Mod11_10.AlgorithmName);

   private static InvalidLength GetInvalidLengthResult(ReadOnlySpan<Char> value)
      => new(
         Messages.DeSteuerIdNrInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(UnformattedLength, Messages.DeSteuerIdNrUnformattedLength),
            new ValidLengthDefinition(FormattedLength, Messages.DeSteuerIdNrFormattedLength),
         ]);

   private static InvalidSeparator GetInvalidSeparatorResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(
         Messages.DeSteuerIdNrInvalidSeparator,
         value[position],
         position);

   private static String GetRawValue(String value)
      => value.Length == UnformattedLength
         ? value
         : String.Concat(
            value.AsSpan(0, FirstSeparatorOffset),
            value.AsSpan(FirstSeparatorOffset + 1, 3),
            value.AsSpan(SecondSeparatorOffset + 1, 3),
            value.AsSpan(ThirdSeparatorOffset + 1));

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

         if (!value[index].IsAsciiDigit())
         {
            return index;
         }
      }

      return -1;
   }

   // TODO: Remove this method and replace with call to CheckDigits.Net Iso7064Mod11_10Algorithm once it supports check digit masks
   private static Boolean ValidateMaskedCheckDigit(
      ReadOnlySpan<Char> value,
      DeSteuerIdNrNumberCheckDigitMask mask)
   {
      const Int32 modulus = 10;
      const Int32 modulusPlus1 = 11;

      var product = modulus;
      Int32 num;
      for (var index = 0; index < value.Length - 1; index++)
      {
         if (mask.ExcludeCharacter(index))
         {
            continue;
         }

         num = value[index].ToSingleDigit();
         if (!num.IsValidDigit())
         {
            return false;
         }

         product += num;
         if (product > modulus)
         {
            product -= modulus;
         }

         product *= 2;
         if (product >= modulusPlus1)
         {
            product -= modulusPlus1;
         }
      }

      num = value[^1].ToSingleDigit();
      if (!num.IsValidDigit())
      {
         return false;
      }

      product += num;

      return product % modulus == 1;
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
public class DeSteuerIdNrJsonConverter : JsonConverter<DeSteuerIdNr>
{
   public override DeSteuerIdNr Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new DeSteuerIdNr(str);
   }

   public override void Write(Utf8JsonWriter writer, DeSteuerIdNr value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}

internal class DeSteuerIdNrNumberCheckDigitMask : ICheckDigitMask
{
   private static readonly Lazy<DeSteuerIdNrNumberCheckDigitMask> _instance =
      new(() => new DeSteuerIdNrNumberCheckDigitMask());

   public static DeSteuerIdNrNumberCheckDigitMask Instance => _instance.Value;

   public Boolean ExcludeCharacter(Int32 index)
      => index is DeSteuerIdNr.FirstSeparatorOffset or DeSteuerIdNr.SecondSeparatorOffset or DeSteuerIdNr.ThirdSeparatorOffset;

   public Boolean IncludeCharacter(Int32 index) => !ExcludeCharacter(index);
}
