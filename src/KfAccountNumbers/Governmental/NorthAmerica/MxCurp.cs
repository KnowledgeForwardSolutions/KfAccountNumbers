// Ignore Spelling: Curp Json Mx

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Strongly typed business object for a Clave Única de Registro de Población
///   (CURP).
/// </summary>
/// <remarks>
///   <para>
///      A valid CURP is an 18-character alphanumeric identifier assigned to
///      Mexican citizens by the Registro Nacional de Población (RENAPO).
///      Portions of the string are generated from a person's personal
///      information and the two trailing characters are assigned by RENAPO.
///   </para>
///   <para>
///      A valid CURP meets the following criteria:
///      <list type="bullet">
///         <item>
///            <description>
///               Not <see langword="null"/>, <see cref="String.Empty"/> or all
///               whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               Eighteen (18) characters in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               Has alphabetic characters derived from the person's surname(s)
///               and given name in positions 0-3 and 13-15 (zero-based).
///            </description>
///         </item>
///         <item>
///            <description>
///               Has a valid date of birth (formatted as YYMMDD) in positions
///               4-9 (zero-based).
///            </description>
///         </item>
///         <item>
///            <description>
///               Has a valid gender character in position 10 (zero-based). Must
///               be H (Hombre/male), M (Mujer/female) or X (non-binary).
///            </description>
///         </item>
///         <item>
///            <description>
///               Has a valid state code in positions 11-12 (zero-based).
///            </description>
///         </item>
///         <item>
///            <description>
///               Has an alphanumeric homoclave character in position 16
///               (zero-based). The homoclave is assigned by RENAPO to avoid
///               duplicate CURP values. A digit homoclave character indicates a
///               birth in the 1900-1999 century, while an alphabetic homoclave
///               indicates a birth in the 2000-2099 century.
///            </description>
///         </item>
///         <item>
///            <description>
///               Has a digit check digit character in position 17 (zero-based).
///               The check digit is assigned by RENAPO. (The algorithm used to
///               generate the check digit is not published and no validation
///               other than confirming it is a digit is performed.)
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      <see cref="MxCurp"/> is case-insensitive for validation and
///      parsing purposes. The MxCurp constructor, Create method and
///      implicit string to MxCurp operator will normalize any
///      lowercase letters to uppercase. Equality and inequality comparisons
///      between instances of MxCurp will compare the normalized
///      uppercase versions of the value.
///   </para>
///   <para>
///      Note that YYMMDD date of birth format presents some ambiguity for the
///      century of birth. This has several impacts:
///      <list type="bullet">
///         <item>
///            The <see cref="DateOfBirth"/> property uses the homoclave
///            character to infer the century of birth. In cases where the
///            person was born before 1900 and assigned a CURP when first issued
///            in 1996, the century will be incorrectly reported as 1900's. This
///            was considered an acceptable limitation since there are no known
///            cases of persons born before 1900 being still alive.
///         </item>
///         <item>
///            The validation for leap year dates is impacted. Specifically, the
///            value "000229" will be considered invalid if the homoclave is a
///            digit (since 1900 was not a leap year) but valid if the homoclave
///            is a letter (since 2000 was a leap year).
///         </item>
///         <item>
///            While not specifically connected to the YYMMDD date of birth
///            format, it should be noted that the date of birth validation does
///            not attempt to check for future dates. So a CURP with a date of
///            "991231" and an alphabetic homoclave character would be
///            considered valid, even though it would indicate a birthdate of
///            December 31, 2099.
///         </item>
///      </list>
///   </para>
///   <para>
///      See <see href="https://en.wikipedia.org/wiki/Unique_Population_Registry_Code">Wikipedia - Unique Population Registry Code</see>
///      and <see href="https://es.wikipedia.org/wiki/Clave_%C3%9Anica_de_Registro_de_Poblaci%C3%B3n">Wikipedia - Clave Única de Registro de Población</see>
///      for more details.
///   </para>
/// </remarks>
[JsonConverter(typeof(MxCurpJsonConverter))]
public record MxCurp
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="MxCurp"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidDateOfBirth,
      InvalidGender,
      InvalidStateProvince,
      InvalidMxCurpHomoclave,
      InvalidChecksum)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="MxCurp"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidDateOfBirth,
      InvalidGender,
      InvalidStateProvince,
      InvalidMxCurpHomoclave,
      InvalidChecksum)
   {
   }

   private const Int32 ValidLength = 18;

   private const Int32 GenderOffset = 10;
   private const Int32 HomoclaveOffset = 16;
   private const Int32 CheckDigitOffset = 17;

   private static readonly SegmentRange _initialsRange = new(0, 4);
   private static readonly SegmentRange _dateOfBirthRange = new(4, 10);
   private static readonly SegmentRange _stateRange = new(11, 13);
   private static readonly SegmentRange _consonantsRange = new(13, 16);

   /// <summary>
   ///   Initializes a new instance of the <see cref="MxCurp"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a CURP.
   /// </param>
   /// <remarks>
   ///   Validation of <paramref name="value"/> is performed in a case-insensitive
   ///   manner. However, the <see cref="Value"/> property will normalize the
   ///   CURP to upper-case.
   /// </remarks>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> does not have length of 18.
   ///   - or -
   ///   <paramref name="value"/> contains non-alphabetic characters in positions
   ///   0-3 or 13-15 (zero-based).
   ///   - or -
   ///   <paramref name="value"/> contains an invalid date of birth in positions
   ///   4-9 (zero-based).
   ///   - or -
   ///   <paramref name="value"/> contains an invalid gender character in position
   ///   10 (zero-based). Valid gender characters are H (Hombre/male),
   ///   M (Mujer/female) or X (non-binary).
   ///   - or -
   ///   <paramref name="value"/> contains an invalid state code in positions
   ///   11-12 (zero-based).
   ///   - or -
   ///   <paramref name="value"/> contains a non-alphanumeric homoclave character
   ///   in position 16 (zero-based).
   ///   - or -
   ///   <paramref name="value"/> contains a non-digit check digit character in
   ///   position 17 (zero-based).
   /// </exception>
   public MxCurp(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="MxCurp"/> class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private MxCurp(String? value, ValidationMode validationMode)
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
               InvalidDateOfBirth invalidDateOfBirth => new UKfValidationException<ValidationError>(invalidDateOfBirth),
               InvalidGender invalidGender => new UKfValidationException<ValidationError>(invalidGender),
               InvalidStateProvince invalidState => new UKfValidationException<ValidationError>(invalidState),
               InvalidMxCurpHomoclave invalidHomoclave => new UKfValidationException<ValidationError>(invalidHomoclave),
               InvalidChecksum invalidCheckDigit => new UKfValidationException<ValidationError>(invalidCheckDigit),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = value!.ToUpperInvariant();
   }

   /// <summary>
   ///   Gets the person's date of birth, derived from the YYMMDD date of birth
   ///   and homoclave elements of the CURP.
   /// </summary>
   /// <remarks>
   ///   Homoclave values 0-9 indicate birth in the 1900-1999 century, homoclave
   ///   values A-Z indicate birth in the 2000-2099 century.
   /// </remarks>
   public DateOnly DateOfBirth
   {
      get
      {
         ReadOnlySpan<Char> dateOfBirthSpan = _dateOfBirthRange.Extract(Value);
#pragma warning disable IDE0008 // Use explicit type
         var (year, month, day) = GetYearMonthDay(dateOfBirthSpan);
#pragma warning restore IDE0008 // Use explicit type
         year += Char.IsAsciiDigit(Value[HomoclaveOffset]) ? 1900 : 2000;

         return new DateOnly(year, month, day);
      }
   }

   /// <summary>
   ///   Gets the person's gender, as coded in the CURP.
   /// </summary>
   /// <remarks>
   ///   Gender will be H (Hombre/male), M (Mujer/female) or X (non-binary).
   /// </remarks>
   public Char GenderCode => Value[GenderOffset];

   /// <summary>
   ///   Gets the person's state of birth as coded in the CURP.
   /// </summary>
   public String StateCode => _stateRange.Extract(Value).ToString();

   /// <summary>
   ///   Gets the CURP value.
   /// </summary>
   /// <remarks>
   ///   The CURP value is always normalized to upper-case.
   /// </remarks>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="MxCurp"/> to a <see cref="String"/>,
   ///   returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="MxCurp"/> to convert.
   /// </param>
   public static implicit operator String(MxCurp source)
      => source?.Value ?? String.Empty;     // Handle null CURP object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="MxCurp"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a CURP.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid CURP.
   /// </exception>
   public static explicit operator MxCurp(String value) => new(value);

   /// <summary>
   ///   Get a string representation of the CURP.
   /// </summary>
   /// <returns>
   ///   The CURP value, normalized to upper-case.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Create a new <see cref="MxCurp"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a CURP.
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{MxCurp, ValidationError}"/>. Will
   ///   contain the new <see cref="MxCurp"/> if
   ///   <paramref name="value"/> is valid or a <see cref="ValidationError"/>
   ///   that identifies the validation rule that was failed if
   ///   <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<MxCurp, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new MxCurp(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidDateOfBirth invalidDateOfBirth => (ValidationError)invalidDateOfBirth,
         InvalidGender invalidGender => (ValidationError)invalidGender,
         InvalidStateProvince invalidState => (ValidationError)invalidState,
         InvalidMxCurpHomoclave invalidHomoclave => (ValidationError)invalidHomoclave,
         InvalidChecksum invalidCheckDigit => (ValidationError)invalidCheckDigit,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a valid
   ///   CURP value.
   /// </summary>
   /// <param name="value">
   ///   String representation of a CURP.
   /// </param>
   /// <returns>
   ///   A <see cref="ValidationResult"/> union that indicates if the
   ///   <paramref name="value"/> passed validation or what validation error was
   ///   encountered.
   /// </returns>
   /// <remarks>
   ///   Validation is case-insensitive.
   /// </remarks>
   public static ValidationResult Validate(String? value)
   {
      if (String.IsNullOrWhiteSpace(value))
      {
         return default(EmptyValue);
      }

      if (value.Length != ValidLength)
      {
         return new InvalidLength(
            Messages.MxCurpInvalidLength,
            value.Length,
            new ValidLengthDefinition(ValidLength, Messages.MxCurpValidLength));
      }

      if (!ValidateNameCharacters(value, out var invalidCharacterPosition))
      {
         return new InvalidCharacter(
            Messages.MxCurpInvalidAlphabeticCharacter,
            value[invalidCharacterPosition],
            invalidCharacterPosition);
      }

      if (!Char.IsAsciiLetterOrDigit(value[HomoclaveOffset]))
      {
         // Required to check prior to date of birth validation since homoclave
         // is used to determine the century for leap year date of birth
         // validation.
         return new InvalidMxCurpHomoclave(
            Messages.MxCurpInvalidHomoclave,
            value[HomoclaveOffset]);
      }

      if (!ValidateDateOfBirth(value))
      {
         return new InvalidDateOfBirth(
            Messages.MxCurpInvalidDateOfBirth,
            _dateOfBirthRange.Extract(value).ToString(),
            DateFormatName.YYMMDD);
      }

      if (!ValidateGenderCode(value))
      {
         return new InvalidGender(
            Messages.MxCurpInvalidGender,
            value[GenderOffset].ToString());
      }

      if (!ValidateStateCode(value))
      {
         return new InvalidStateProvince(
            Messages.MxCurpInvalidState,
            _stateRange.Extract(value).ToString());
      }

      if (!Char.IsAsciiDigit(value[CheckDigitOffset]))
      {
         // No algorithm published for CURP check digit, so algorithm name is N/A.
         return new InvalidChecksum(Messages.MxCurpInvalidCheckDigit, "N/A");
      }

      return default(ValidValue);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static (Int32 Year, Int32 Month, Int32 Day) GetYearMonthDay(ReadOnlySpan<Char> dateOfBirthSpan)
   {
      var year = dateOfBirthSpan.ParseTwoDigits();
      var month = dateOfBirthSpan[2..].ParseTwoDigits();
      var day = dateOfBirthSpan[4..].ParseTwoDigits();
      return (year, month, day);
   }

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> value)
   {
      ReadOnlySpan<Char> dateOfBirthSpan = _dateOfBirthRange.Extract(value);

      // Switching from DateTime.TryParseExact to manual validation resulted in
      // a performance improvement from > 60ns to < 20ns per call in benchmark tests.
      foreach (var ch in dateOfBirthSpan)
      {
         if (!Char.IsAsciiDigit(ch))
         {
            return false;
         }
      }

#pragma warning disable IDE0008 // Use explicit type
      var (year, month, day) = GetYearMonthDay(dateOfBirthSpan);
#pragma warning restore IDE0008 // Use explicit type

      if (month is < 1 or > 12)
      {
         return false;
      }

      // Use homoclave value to adjust the year century.
      year += value[HomoclaveOffset].IsAsciiDigit()
         ? 1900
         : 2000;

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }

   private static Boolean ValidateGenderCode(ReadOnlySpan<Char> value)
   {
      var genderCode = value[GenderOffset];

      return genderCode is Chars.UpperCaseH
         or Chars.UpperCaseM
         or Chars.UpperCaseX
         or Chars.LowerCaseH
         or Chars.LowerCaseM
         or Chars.LowerCaseX;
   }

   private static Boolean ValidateNameCharacters(
      ReadOnlySpan<Char> value,
      out Int32 invalidCharacterPosition)
   {
      invalidCharacterPosition = -1;
      for (var index = _initialsRange.Start; index < _initialsRange.End; index++)
      {
         if (!Char.IsAsciiLetter(value[index]))
         {
            invalidCharacterPosition = index;
            return false;
         }
      }

      for (var index = _consonantsRange.Start; index < _consonantsRange.End; index++)
      {
         if (!Char.IsAsciiLetter(value[index]))
         {
            invalidCharacterPosition = index;
            return false;
         }
      }

      return true;
   }

   private static Boolean ValidateStateCode(ReadOnlySpan<Char> value)
   {
      ReadOnlySpan<Char> stateCode = _stateRange.Extract(value);

      return MxCurpStateCodes.ValidateStateCode(stateCode);
   }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class MxCurpJsonConverter : JsonConverter<MxCurp>
{
   public override MxCurp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new MxCurp(str);
   }

   public override void Write(Utf8JsonWriter writer, MxCurp value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
