// Ignore Spelling: Insee

#pragma warning disable IDE0250 // Make struct 'readonly'
#pragma warning disable IDE0046 // Convert to conditional expression
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a French INSEE number.
/// </summary>
/// <remarks>
///   <para>
///      A French INSEE number is a 15-digit number structured as
///      SYYMMLLOOOKKKCC with the following elements:
///      <list type="bullet">
///         <item>
///            <term>S</term>
///            <description>
///               The person's gender, where 1 = male and 2 = female. Temporary
///               INSEE numbers use 7 = male and 8 = female instead.
///            </description>
///         </item>
///         <item>
///            <term>YY</term>
///            <description>
///               The person's two-digit year of birth.
///            </description>
///         </item>
///         <item>
///            <term>MM</term>
///            <description>
///               The person's two-digit month of birth. See below for values
///               used for persons with unknown or incomplete date of birth
///               documentation.
///            </description>
///         </item>
///         <item>
///            <term>LLOOO</term>
///            <description>
///               Five-digit INSEE COG (Code officiel géographique) identifying
///               the person's department and commune of birth. (Exception: LL
///               may be "2A" or "2B" for the two departments in Corsica).
///            </description>
///         </item>
///         <item>
///            <term>KKK</term>
///            <description>
///               Three digits used to distinguish between people born in the
///               same year/month/department/commune.
///            </description>
///         </item>
///         <item>
///            <term>CC</term>
///            <description>
///               Two-digit modulus 97 check sum calculated for the preceding 13
///               digits. When calculating the checksum, department code "2A" is
///               replaced by 19, and department code "2B" is replaced by 18.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      An INSEE number may be formatted as 15 consecutive digits or as 21
///      characters with spaces separating the different elements, i.e.
///      "S YY MM LL OOO KKK CC".
///   </para>
///   <para>
///      When creating a new <see cref="FrInseeNumber"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 15 characters (without separators) or
///               21 characters (with separators) in length.
///            </description>
///         </item>
///         <item>
///            <description>
///              All characters (except the optional separator characters or
///              Corsican department codes) must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The two trailing (right-most) characters must be a valid
///               modulus 97 check sum.
///            </description>
///         </item>
///         <item>
///            <description>
///               The separator characters (if used) may not be ASCII digits
///               ('0'-'9'). All separator characters must be the same character.
///            </description>
///         </item>
///         <item>
///            <description>
///               The leading gender indicator (S) must be 1, 2, 7 or 8.
///            </description>
///         </item>
///         <item>
///            <description>
///               The month element (MM) must be a number between 01 and 12 (for
///               known dates) or 13, 20-42, 50-99 (for persons with unknown or
///               incomplete date of birth documentation).
///            </description>
///         </item>
///         <item>
///            <description>
///               The COG element (LLOOO) must start with a valid department
///               code, or 99 for persons born abroad.  For departments with
///               alphabetic characters (Corsica 2A, 2B), the alphabetic
///               character must be uppercase.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      Example values:
///      <list type="bullet">
///         <item>
///            <term>188121884813236</term>
///            <description>
///               gender = male, year of birth = 88, month of birth = 12,
///               department = 18 (Cher)
///            </description>
///         </item>
///         <item>
///            <term>255102445387701</term>
///            <description>
///               gender = female, year of birth = 55, month of birth = 10,
///               department = 24 (Dordogne)
///            </description>
///         </item>
///         <item>
///            <term>112072A28806058</term>
///            <description>
///               gender = male, year of birth = 12, month of birth = 07,
///               department = 2A (Corse-du-Sud)
///            </description>
///         </item>
///         <item>
///            <term>821099901013371</term>
///            <description>
///               temporary INSEE, gender = female, year of birth = 21,
///               month of birth = 09, department = 99 (born abroad)
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      See https://en.wikipedia.org/wiki/INSEE_code and
///      https://fr.wikipedia.org/wiki/Num%C3%A9ro_de_s%C3%A9curit%C3%A9_sociale_en_France (French) for more info.
///   </para>
/// </remarks>
[JsonConverter(typeof(FrInseeNumberJsonConverter))]
public record FrInseeNumber
{
   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new <see cref="FrInseeNumber"/>.
   /// </summary>
   public union ValidationError(
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidGender,
      InvalidMonth,
      InvalidFrInseeDepartment)
   {
   }

   /// <summary>
   ///   Discriminated union defining the possible results that can occur when
   ///   validating a <see cref="FrInseeNumber"/>.
   /// </summary>
   public union ValidationResult(
      ValidValue,
      EmptyValue,
      InvalidLength,
      InvalidCharacter,
      InvalidChecksum,
      InvalidSeparator,
      InvalidGender,
      InvalidMonth,
      InvalidFrInseeDepartment)
   {
   }

   /// <summary>
   ///   The name of the check digit algorithm used by INSEE numbers.
   /// </summary>
   public const String CheckDigitAlgorithmName = "Modulus 97";

   private const Int32 UnformattedLength = 15;
   private const Int32 FormattedLength = 21;

   // Used to validate formatted values.
   private static readonly SegmentRange _formattedDepartment = new(8, 10);
   private static readonly SegmentRange _formattedMonth = new(5, 7);

   // Used to validate unformatted values or to extract elements from raw values
   // post validation.
   private static readonly SegmentRange _department = new(5, 7);
   private static readonly SegmentRange _month = new(3, 5);
   private static readonly SegmentRange _overseasDepartment = new(5, 8);

   private const Int32 GenderOffset = 0;
   private const Int32 FormattedMonthOffset = 5;
   private const Int32 FormattedDepartmentOffset = 8;
   private const Int32 UnformattedYearOffset = 1;
   private const Int32 UnformattedMonthOffset = 3;
   private const Int32 UnformattedDepartmentOffset = 5;
   private const Int32 UnformattedCommuneOffset = 7;
   private const Int32 UnformattedSequenceOffset = 10;

   private const Int32 Separator1Offset = 1;
   private const Int32 Separator2Offset = 4;
   private const Int32 Separator3Offset = 7;
   private const Int32 Separator4Offset = 10;
   private const Int32 Separator5Offset = 14;
   private const Int32 Separator6Offset = 18;

   private static readonly Int32[] _separatorOffsets =
   [
      Separator2Offset,             // Skip Separator1Offset because it's handled slightly differently.
      Separator3Offset,             // See ValidateSeparators.
      Separator4Offset,
      Separator5Offset,
      Separator6Offset
   ];

   private const Int32 UnformattedCorsicanDepartmentLetterOffset = 6;
   private const Int32 FormattedCorsicanDepartmentLetterOffset = 9;

   // These items are measured from the end of the value.
   private const Int32 CheckDigit1Offset = 2;
   private const Int32 CheckDigit2Offset = 1;

   private const String OverseasDepartmentPrefix = "97";
   private const String BornAbroadDepartment = "99";

   /// <summary>
   ///   Initializes a new instance of the <see cref="FrInseeNumber"/> class.
   /// </summary>
   /// <param name="value">
   ///   String representation of a French INSEE number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="value"/> is not length 15 (or 21 if separator
   ///   characters are used).
   ///   - or -
   ///   <paramref name="value"/> contains a non-digit character in
   ///   any position other than the separator locations. (Exception: Corsican
   ///   departments - 2A and 2B.)
   ///   - or -
   ///   <paramref name="value"/> has invalid modulus 97 check digit
   ///   characters in the trailing (right-most) character positions.
   ///   - or -
   ///   <paramref name="value"/> is 21 characters in length and has
   ///   an ASCII digit character ('0'-'9') in a separator location or does not
   ///   use the same separator character in each location.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid gender value. Valid gender
   ///   values are 1 (male) and 2 (female) or 7 (male) and 8 (female) for
   ///   temporary INSEE numbers.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid value for month of birth.
   ///   Valid values for month of birth are 01-12 (for known dates of birth)
   ///   and 13, 20-42, 50-99 for persons with unknown or incomplete date of
   ///   birth documentation.
   ///   - or -
   ///   <paramref name="value"/> contains an invalid department code.
   /// </exception>
   public FrInseeNumber(String? value)
      : this(value, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Initializes a new instance of the <see cref="FrInseeNumber"/> class.
   /// </summary>
   /// <remarks>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has
   ///   already been validated.
   /// </remarks>
   private FrInseeNumber(String? value, ValidationMode validationMode)
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
               InvalidGender invalidGender => new UKfValidationException<ValidationError>(invalidGender),
               InvalidMonth invalidMonth => new UKfValidationException<ValidationError>(invalidMonth),
               InvalidFrInseeDepartment invalidDepartment => new UKfValidationException<ValidationError>(invalidDepartment),
               _ => new UnreachableException("This branch should never be reached"),
            };
         }
      }

      Value = GetRawValue(value!);
   }

   /// <summary>
   ///   Gets the person's integer month of birth.
   /// </summary>
   /// <remarks>
   ///   Birth month is normally 1-12, but may be other values for persons with
   ///   unknown or incomplete documentation. Other possible values are 13,
   ///   20-42 and 50-99.
   /// </remarks>
   public Int32 BirthMonth => Value.AsSpan(UnformattedMonthOffset..).ParseTwoDigits();

   /// <summary>
   ///   Gets the person's two digit year of birth (0-99).
   /// </summary>
   public Int32 BirthYear => Value.AsSpan(UnformattedYearOffset..).ParseTwoDigits();

   /// <summary>
   ///   Gets the five-digit INSEE COG (Code officiel géographique) identifying
   ///   the person's department and commune of birth.
   /// </summary>
   /// <remarks>
   ///   The COG is the combination of department and commune of birth. There
   ///   are three possible patterns for COG:
   ///   <list type="bullet">
   ///      <item>
   ///         <description>
   ///            For persons born in metropolitan France, 2-digit department +
   ///            3-digit commune (including Corsican departments 2A and 2B).
   ///         </description>
   ///      </item>
   ///      <item>
   ///         <description>
   ///            For persons born in overseas departments, 3-digit department +
   ///            2-digit commune.
   ///         </description>
   ///      </item>
   ///      <item>
   ///         <description>
   ///            For persons born abroad, fixed 2-digit department of 99 +
   ///            three-digit ISO 3166-1 country code.
   ///         </description>
   ///      </item>
   ///   </list>
   /// </remarks>
   public String Cog => Value[UnformattedDepartmentOffset..UnformattedSequenceOffset];

   /// <summary>
   ///   Gets the INSEE code for the department where the person was born, as
   ///   encoded in the INSEE number.
   /// </summary>
   public String Department
   {
      get
      {
         var endOffset = UnformattedCommuneOffset;
         if (Value.AsSpan(UnformattedDepartmentOffset..endOffset).Equals(OverseasDepartmentPrefix, StringComparison.OrdinalIgnoreCase))
         {
            // Overseas departments use an additional character for department code.
            endOffset++;
         }

         return Value[UnformattedDepartmentOffset..endOffset];
      }
   }

   /// <summary>
   ///   Gets the person's gender, as indicated by the leading (left-most) digit
   ///   in the INSEE number.
   /// </summary>
   public Gender.BinaryGender Gender
      => Value[GenderOffset] % 2 == 0 ? default(Gender.Female) : default(Gender.Male);    // This works because the ASCII character values for digits have the same odd/even pattern

   /// <summary>
   ///   Gets a value indicating whether the person was born abroad.
   ///   <see langword="true"/> if the person was born abroad; otherwise
   ///   <see langword="false"/>.
   /// </summary>
   /// <remarks>
   ///   Persons born abroad have a fixed department code of "99".
   /// </remarks>
   public Boolean IsBornAbroad
      => Value.AsSpan(UnformattedDepartmentOffset..UnformattedCommuneOffset).Equals(BornAbroadDepartment, StringComparison.OrdinalIgnoreCase);

   /// <summary>
   ///   Gets a value indicating whether this INSEE number is temporary or
   ///   permanent. <see langword="true"/> if this INSEE is temporary; otherwise
   ///   <see langword="false"/>.
   /// </summary>
   /// <remarks>
   ///   Permanent INSEE numbers use gender codes '1' or '2' while temporary
   ///   INSEE numbers use gender codes '7' or '8'.
   /// </remarks>
   public Boolean IsTemporaryInsee
      => Value[GenderOffset] is Chars.DigitSeven or Chars.DigitEight;

   /// <summary>
   ///   Gets the raw INSEE number.
   /// </summary>
   public String Value { get; private init; }

   /// <summary>
   ///   Implicitly converts a <see cref="FrInseeNumber"/> to a
   ///   <see cref="String"/>, returning an empty string if the source is null.
   /// </summary>
   /// <param name="source">
   ///   The <see cref="FrInseeNumber"/> to convert.
   /// </param>
   public static implicit operator String(FrInseeNumber source)
      => source?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   /// <summary>
   ///   Defines an explicit conversion of a string to a <see cref="FrInseeNumber"/>.
   /// </summary>
   /// <param name="value">
   ///   String representation of a French INSEE number.
   /// </param>
   /// <exception cref="UKfValidationException{ValidationError}">
   ///   <paramref name="value"/> is not a valid INSEE number.
   /// </exception>
   public static explicit operator FrInseeNumber(String? value) => new(value);

   /// <summary>
   ///   Create a new <see cref="FrInseeNumber"/> using the Result pattern.
   /// </summary>
   /// <param name="value">
   ///   String representation of a French INSEE number.
   /// </param>
   /// <returns>
   ///   A <see cref="UCreateResult{BeRijksregisternummer, ValidationError}"/>. Will
   ///   contain the new <see cref="BeRijksregisternummer"/> if <paramref name="value"/>
   ///   is valid or a <see cref="ValidationError"/> that identifies the
   ///   validation rule that was failed if <paramref name="value"/> is invalid.
   /// </returns>
   public static UCreateResult<FrInseeNumber, ValidationError> Create(String? value)
      => Validate(value) switch
      {
         ValidValue => new FrInseeNumber(value, ValidationMode.BypassValidation),
         EmptyValue emptyValue => (ValidationError)emptyValue,
         InvalidLength invalidLength => (ValidationError)invalidLength,
         InvalidCharacter invalidCharacter => (ValidationError)invalidCharacter,
         InvalidChecksum invalidChecksum => (ValidationError)invalidChecksum,
         InvalidSeparator invalidSeparator => (ValidationError)invalidSeparator,
         InvalidGender invalidGender => (ValidationError)invalidGender,
         InvalidMonth invalidMonth => (ValidationError)invalidMonth,
         InvalidFrInseeDepartment invalidDepartment => (ValidationError)invalidDepartment,
         _ => throw new UnreachableException("This branch should never be reached"),
      };

   /// <summary>
   ///   Format the INSEE number using the supplied <paramref name="mask"/>.
   /// </summary>
   /// <param name="mask">
   ///   Optional. The mask that specifies the final output. If not supplied
   ///   then the default mask "_ __ __ __ ___ ___ __" will be used instead.
   /// </param>
   /// <returns>
   ///   A formatted INSEE number.
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
   ///   details on creating a mask to format the INSEE number.
   /// </remarks>
   public String Format(String mask = "_ __ __ __ ___ ___ __") => Value.FormatWithMask(mask);

   /// <summary>
   ///   Get a string representation of the INSEE number.
   /// </summary>
   /// <returns>
   ///   The raw INSEE number, without  separator characters.
   /// </returns>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="value"/> to determine if it contains a
   ///   valid French INSEE number.
   /// </summary>
   /// <param name="value">
   ///   String representation of a French INSEE number.
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
         return new InvalidLength(
            Messages.FrInseeNumberInvalidLength,
            value.Length,
            GetValidLengthDefinitions());
      }

      // After performing basic checks, validate the check digits because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      ValidationResult validationResult = ValidateCheckDigits(value);
      if (validationResult is not ValidValue)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }

      if (!ValidateSeparators(value, out var invalidSeparatorPosition))
      {
         return new InvalidSeparator(
            Messages.FrInseeNumberInvalidSeparator,
            value[invalidSeparatorPosition],
            invalidSeparatorPosition);
      }

      if (!ValidateGender(value))
      {
         return new InvalidGender(
            Messages.FrInseeNumberInvalidGender,
            value[GenderOffset].ToString());
      }

      if (!ValidateMonth(value))
      {
         return new InvalidMonth(
            Messages.FrInseeNumberInvalidMonth,
            GetMonth(value).ToString());
      }

      if (!ValidateDepartment(value))
      {
         return new InvalidFrInseeDepartment(
            Messages.FrInseeNumberInvalidDepartment,
            GetDepartmentCode(value));
      }

      return default(ValidValue);
   }

   /// <summary>
   ///   Gets the department code from the supplied value.
   /// </summary>
   /// <param name="value">
   ///   String representation of a French INSEE number.
   /// </param>
   /// <returns>
   ///   The department code from the supplied value.
   /// </returns>
   internal static String GetDepartmentCode(ReadOnlySpan<Char> value)
   {
      var isFormatted = IsFormatted(value);
      ReadOnlySpan<Char> department =
         (isFormatted ? _formattedDepartment : _department).Extract(value);
      if (department.Equals(OverseasDepartmentPrefix, StringComparison.OrdinalIgnoreCase))
      {
         return isFormatted
            ? $"{department}{value[_formattedDepartment.End + 1]}"
            : _overseasDepartment.Extract(value).ToString();
      }

      return department.ToString();
   }

   /// <summary>
   ///   Gets an array of details about valid lengths accepted for an INSEE
   ///   number.
   /// </summary>
   /// <returns>
   ///   An array of <see cref="ValidLengthDefinition"/>s.
   /// </returns>
   internal static ValidLengthDefinition[] GetValidLengthDefinitions()
      =>
      [
         new ValidLengthDefinition(UnformattedLength, Messages.FrInseeNumberUnformattedLength),
         new ValidLengthDefinition(FormattedLength, Messages.FrInseeNumberFormattedLength),
      ];

   private static InvalidCharacter GetInvalidCharacterResult(
      ReadOnlySpan<Char> value,
      Int32 position)
      => new(
         Messages.FrInseeNumberInvalidCharacter,
         value[position],
         position);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static ReadOnlySpan<Char> GetMonth(ReadOnlySpan<Char> value)
      => (IsFormatted(value) ? _formattedMonth : _month).Extract(value);

   private static String GetRawValue(String value)
   {
      if (value.Length == UnformattedLength)
      {
         return value;
      }

      var buffer = ArrayPool<Char>.Shared.Rent(UnformattedLength);
      try
      {
         ReadOnlySpan<Char> source = value.AsSpan();
         var span = new Span<Char>(buffer);

         ReadOnlySpan<Int32> segmentLengths = [1, 2, 2, 2, 3, 3, 2];
         var sourceOffset = 0;
         var targetOffset = 0;
         foreach (var length in segmentLengths)
         {
            ReadOnlySpan<Char> sourceSpan = source[sourceOffset..(sourceOffset + length)];
            Span<Char> targetSpan = span[targetOffset..(targetOffset + length)];

            sourceSpan.CopyTo(targetSpan);

            sourceOffset += length + 1;
            targetOffset += length;
         }

         return span[..UnformattedLength].ToString();
      }
      finally
      {
         ArrayPool<Char>.Shared.Return(buffer);
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> insee)
      => insee.Length == FormattedLength;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsSeparatorLocation(Int32 index)
      => index is Separator1Offset or Separator2Offset or Separator3Offset or
            Separator4Offset or Separator5Offset or Separator6Offset;

   private static ValidationResult ValidateCheckDigits(ReadOnlySpan<Char> value)
   {
      var processLength = value.Length - 2;      // Exclude check digits from main loop
      var isFormatted = IsFormatted(value);
      var letterOffset = isFormatted ? FormattedCorsicanDepartmentLetterOffset : UnformattedCorsicanDepartmentLetterOffset;
      var corsicanOffset = 0L;

      var sum = 0L;        // Long required because 13 digits exceeds int capacity
      for (var index = 0; index < processLength; index++)
      {
         if (isFormatted && IsSeparatorLocation(index))
         {
            continue;
         }

         sum *= 10;
         var num = value[index].ToSingleDigit();
         if (!num.IsValidDigit())
         {
            // Handle possible valid letter character in department code (2A or 2B are valid for Corsica).
            // Note that there are two possible ways to handle Corsican department codes. The common
            // option is to substitute "19" for "2A" and "18" for "2B" and process normally. The other
            // option (described in footnote "F" on https://fr.wikipedia.org/wiki/Num%C3%A9ro_de_s%C3%A9curit%C3%A9_sociale_en_France
            // is to set the value for the letter character to zero and then subtracting 1,000,000 (for A)
            // or 2,000,000 (for B) from the sum before calculating the remainder. The second option,
            // which we call the "corsican offset", is used here because it does not involve creating
            // a temporary copy of the INSEE with only digits or require looking ahead to the second
            // department character if the department code starts with 2.
            if (index == letterOffset)
            {
               corsicanOffset = value[index] switch
               {
                  Chars.UpperCaseA => 1000000,
                  Chars.UpperCaseB => 2000000,
                  _ => 0L,                         // Not a valid Corsican department code
               };
            }

            // If not part of a valid Corsican department code then the character is invalid.
            if (corsicanOffset == 0L)
            {
               return GetInvalidCharacterResult(value, index);
            }

            // If using corsican offset then num is ignored for this character.
            num = 0;
         }

         sum += num;
      }

      sum -= corsicanOffset;
      var remainder = 97 - (sum % 97);

      var c1 = value[^CheckDigit1Offset].ToSingleDigit();
      if (!c1.IsValidDigit())
      {
         return GetInvalidCharacterResult(value, value.Length - CheckDigit1Offset);
      }

      var c2 = value[^CheckDigit2Offset].ToSingleDigit();
      if (!c2.IsValidDigit())
      {
         return GetInvalidCharacterResult(value, value.Length - CheckDigit2Offset);
      }

      var checkSum = (c1 * 10) + c2;

      return checkSum == remainder
         ? default(ValidValue)
         : new InvalidChecksum(
            Messages.FrInseeNumberInvalidCheckDigits,
            CheckDigitAlgorithmName);
   }

   private static Boolean ValidateDepartment(ReadOnlySpan<Char> value)
   {
      var isFormatted = IsFormatted(value);
      ReadOnlySpan<Char> department =
         (isFormatted ? _formattedDepartment : _department).Extract(value);
      if (FrDepartmentCodes.ValidateDepartmentCode(department))
      {
         return true;
      }
      else if (department.Equals(OverseasDepartmentPrefix, StringComparison.OrdinalIgnoreCase))
      {
         // Possible overseas department. Check three character department code
         // that includes the first character of the commune.
         ReadOnlySpan<Char> extendedDepartment = IsFormatted(value)
            ? [.. department, value[_formattedDepartment.End + 1]]                        // If formatted, we have to skip over separator between department and commune
            : value[_department.Start..(_department.End + 1)];                            // If not formatted, simply extend the slice

         return FrDepartmentCodes.ValidateDepartmentCode(extendedDepartment);
      }

      return false;
   }

   private static Boolean ValidateGender(ReadOnlySpan<Char> value)
      => value[GenderOffset] is Chars.DigitOne or Chars.DigitTwo or Chars.DigitSeven or Chars.DigitEight;

   private static Boolean ValidateMonth(ReadOnlySpan<Char> value)
   {
      var month = GetMonth(value).ParseTwoDigits();

      return month switch
      {
         >= 1 and <= 12 => true,
         13 => true,
         >= 20 and <= 42 => true,
         >= 50 => true,
         _ => false,
      };
   }

   private static Boolean ValidateSeparators(
      ReadOnlySpan<Char> value,
      out Int32 invalidSeparatorPosition)
   {
      invalidSeparatorPosition = -1;
      if (value.Length == UnformattedLength)
      {
         return true;
      }

      var separator = value[Separator1Offset];
      if (separator.IsAsciiDigit())
      {
         invalidSeparatorPosition = Separator1Offset;
         return false;
      }

      // All separators must be the same.
      foreach (var offset in _separatorOffsets)
      {
         if (value[offset] != separator)
         {
            invalidSeparatorPosition = offset;
            return false;
         }
      }

      return true;
   }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
public class FrInseeNumberJsonConverter : JsonConverter<FrInseeNumber>
{
   public override FrInseeNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         return null!;
      }

      var str = reader.GetString();
      return new FrInseeNumber(str);
   }

   public override void Write(Utf8JsonWriter writer, FrInseeNumber value, JsonSerializerOptions options)
      => writer.WriteStringValue(value.Value);
}
