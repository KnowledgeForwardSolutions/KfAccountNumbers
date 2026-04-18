// Ignore Spelling: Insee

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a French INSEE number.
/// </summary>
/// <remarks>
///   <para>
///      A French INSEE number is a 15-digit number structured as SYYMMLLOOOKKKCC
///      with the following elements:
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
///               Three digits used to distinguish between people born in the same
///               year/month/department/commune.
///            </description>
///         </item>
///         <item>
///            <term>CC</term>
///            <description>
///               Two-digit modulus 97 check sum calculated for the preceding 13 digits.
///               When calculating the checksum, department code "2A" is replaced by 19,
///               and department code "2B" is replaced by 18.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      An INSEE number may be formatted as 15 consecutive digits or as 21 characters
///      with spaces separating the different elements, i.e. "S YY MM LL OOO KKK CC".
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
///               The value must be either 15 characters (without separators) or 21
///               characters (with separators) in length.
///            </description>
///         </item>
///         <item>
///            <description>
///              All characters (except the optional separator characters or Corsican
///              department codes) must be ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               The two trailing (right-most) characters must be a valid modulus 97
///               check sum.
///            </description>
///         </item>
///         <item>
///            <description>
///               The separator characters (if used) may not be ASCII digits ('0'-'9').
///               All separator characters must be the same character.
///            </description>
///         </item>
///         <item>
///            <description>
///               The leading gender indicator (S) must be 1, 2, 7 or 8.
///            </description>
///         </item>
///         <item>
///            <description>
///               The month element (MM) must be a number between 01 and 12 (for known
///               dates) or 13, 20-42, 50-99 (for persons with unknown or incomplete date
///               of birth documentation).
///            </description>
///         </item>
///         <item>
///            <description>
///               The COG element (LLOOO) must start with a valid department code, or
///               99 for persons born abroad.  For departments with alphabetic characters
///               (Corsica 2A, 2B), the alphabetic character must be uppercase.
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
///               gender = male, year of birth = 88, month of birth = 12, department = 18 (Cher)
///            </description>
///         </item>
///         <item>
///            <term>255102445387701</term>
///            <description>
///               gender = female, year of birth = 55, month of birth = 10, department = 24 (Dordogne)
///            </description>
///         </item>
///         <item>
///            <term>112072A28806058</term>
///            <description>
///               gender = male, year of birth = 12, month of birth = 07, department = 2A (Corse-du-Sud)
///            </description>
///         </item>
///         <item>
///            <term>821099901013371</term>
///            <description>
///               temporary INSEE, gender = female, year of birth = 21, month of birth = 09, department = 99 (born abroad)
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
   private const Int32 UnformattedLength = 15;
   private const Int32 FormattedLength = 21;

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

   private const Int32 UnformattedCorsicanDepartmentLetterOffset = 6;
   private const Int32 FormattedCorsicanDepartmentLetterOffset = 9;

   // These items are measured from the end of the value.
   private const Int32 CheckDigit1Offset = 2;
   private const Int32 CheckDigit2Offset = 1;

   private const String OverseasDepartmentPrefix = "97";
   private const String BornAbroadDepartment = "99";

   /// <summary>
   ///   Initialize a new instance of the <see cref="FrInseeNumber"/> class.
   /// </summary>
   /// <param name="insee">
   ///   String representation of a French INSEE number.
   /// </param>
   /// <exception cref="KfValidationException{FrInseeNumberValidationResult}">
   ///   <paramref name="insee"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="insee"/> is not length 15 (or 21 if separator
   ///   characters are used).
   ///   - or -
   ///   <paramref name="insee"/> contains a non-digit character in
   ///   any position other than the separator locations. (Exception: Corsican
   ///   departments - 2A and 2B.)
   ///   - or -
   ///   <paramref name="insee"/> has invalid modulus 97 check digit
   ///   characters in the trailing (right-most) character positions.
   ///   - or -
   ///   <paramref name="insee"/> is 21 characters in length and has
   ///   an ASCII digit character ('0'-'9') in a separator location or does not
   ///   use the same separator character in each location.
   ///   - or -
   ///   <paramref name="insee"/> contains an invalid gender value. Valid gender
   ///   values are 1 (male) and 2 (female) or 7 (male) and 8 (female) for
   ///   temporary INSEE numbers.
   ///   - or -
   ///   <paramref name="insee"/> contains an invalid value for month of birth.
   ///   Valid values for month of birth are 01-12 (for known dates of birth)
   ///   and 13, 20-42, 50-99 for persons with unknown or incomplete date of
   ///   birth documentation.
   ///   - or -
   ///   <paramref name="insee"/> contains an invalid department code.
   /// </exception>
   public FrInseeNumber(String? insee)
      : this(insee, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has already
   ///   been validated.
   /// </summary>
   private FrInseeNumber(String? insee, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         FrInseeNumberValidationResult validationResult = Validate(insee);
         if (validationResult != FrInseeNumberValidationResult.ValidationPassed)
         {
            throw validationResult.ToValidationException();
         }
      }

      Value = GetRawValue(insee!);
   }

   /// <summary>
   ///   Get the person's integer month of birth.
   /// </summary>
   /// <remarks>
   ///   Birth month is normally 1-12, but may be other values for persons with
   ///   unknown or incomplete documentation. Other possible values are 13,
   ///   20-42 and 50-99.
   /// </remarks>
   public Int32 BirthMonth => Value.AsSpan(UnformattedMonthOffset..).ParseTwoDigits();

   /// <summary>
   ///   Get the person's two digit year of birth (0-99).
   /// </summary>
   public Int32 BirthYear => Value.AsSpan(UnformattedYearOffset..).ParseTwoDigits();

   /// <summary>
   ///   The five-digit INSEE COG (Code officiel géographique) identifying the
   ///   person's department and commune of birth.
   /// </summary>
   /// <remarks>
   ///   The COG is the combination of department and commune of birth. There
   ///   are three possible patterns for COG:
   ///   <list type="bullet">
   ///      <item>
   ///         <description>
   ///            For persons born in metropolitan France, 2-digit department + 3-digit
   ///            commune (including Corsican departments 2A and 2B).
   ///         </description>
   ///      </item>
   ///      <item>
   ///         <description>
   ///            For persons born in overseas departments, 3-digit department + 2-digit commune.
   ///         </description>
   ///      </item>
   ///      <item>
   ///         <description>
   ///            For persons born abroad, fixed 2-digit department of 99 + three-digit
   ///            ISO 3166-1 country code.
   ///         </description>
   ///      </item>
   ///   </list>
   /// </remarks>
   public String Cog => Value[UnformattedDepartmentOffset..UnformattedSequenceOffset];

   /// <summary>
   ///   Get the INSEE code for the department where the person was born, as
   ///   encoded in the INSEE number.
   /// </summary>
   public String Department
   {
      get
      {
         var endOffset =UnformattedCommuneOffset;
         if (Value.AsSpan(UnformattedDepartmentOffset..endOffset).Equals(OverseasDepartmentPrefix, StringComparison.OrdinalIgnoreCase))
         {
            endOffset ++;
            return Value[UnformattedDepartmentOffset..endOffset];
         }

         // Overseas departments use an additional character for department code.
         return Value[UnformattedDepartmentOffset..endOffset];
      }
   }

   /// <summary>
   ///   The person's gender, as indicated by the leading (left-most) digit in
   ///   the INSEE number.
   /// </summary>
   public BinaryGender Gender
      => Value[GenderOffset] switch
      {
         Chars.DigitOne => BinaryGender.Male,
         Chars.DigitTwo => BinaryGender.Female,
         Chars.DigitSeven => BinaryGender.Male,
         Chars.DigitEight => BinaryGender.Female,
         _ => throw new InvalidOperationException()      // Validation during construction ensures this can never be reached
      };

   /// <summary>
   ///   Indicates if the person was born abroad.
   /// </summary>
   /// <remarks>
   ///   Persons born abroad have a fixed department code of "99".
   /// </remarks>
   public Boolean IsBornAbroad
      => Value.AsSpan(UnformattedDepartmentOffset..UnformattedCommuneOffset).Equals(BornAbroadDepartment, StringComparison.OrdinalIgnoreCase);

   /// <summary>
   ///   Indicates if this INSEE number is temporary or permanent.
   /// </summary>
   /// <remarks>
   ///   Permanent INSEE numbers use gender codes '1' or '2' while temporary
   ///   INSEE numbers use gender codes '7' or '8'.
   /// </remarks>
   public Boolean IsTemporaryInsee
      => Value[GenderOffset] switch
      {
         Chars.DigitOne => false,
         Chars.DigitTwo => false,
         Chars.DigitSeven => true,
         Chars.DigitEight => true,
         _ => throw new InvalidOperationException()      // Validation during construction ensures this can never be reached
      };

   /// <summary>
   ///   The raw INSEE number.
   /// </summary>
   public String Value { get; private init; }

   public static implicit operator String(FrInseeNumber insee)
      => insee?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   // Explicit conversion from String to avoid unintentional conversions that may throw exceptions.
   public static explicit operator FrInseeNumber(String? insee) => new(insee);

   /// <summary>
   ///   Create a new <see cref="FrInseeNumber"/> using the Result pattern.
   /// </summary>
   /// <param name="insee">
   ///   String representation of a French INSEE number.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{FrInseeNumber, FrInseeNumberValidationResult}"/>.
   ///   Will contain the new <see cref="FrInseeNumber"/> if 
   ///   <paramref name="insee"/> is valid or an <see cref="FrInseeNumber"/> that
   ///   identifies the validation rule that was failed if <paramref name="insee"/>
   ///   is invalid.
   /// </returns>
   public static CreateResult<FrInseeNumber, FrInseeNumberValidationResult> Create(String? insee)
   {
      FrInseeNumberValidationResult validationResult = Validate(insee);
      return validationResult == FrInseeNumberValidationResult.ValidationPassed
         ? new FrInseeNumber(insee, validationMode: ValidationMode.BypassValidation)
         : validationResult;
   }

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
   /// <remarks>
   ///   Will return the raw INSEE number, without  separator characters.
   /// </remarks>
   public override String ToString() => Value;

   /// <summary>
   ///   Check the <paramref name="insee"/> to determine if it contains a
   ///   valid French INSEE number.
   /// </summary>
   /// <param name="insee">
   ///   String representation of a French INSEE number.
   /// </param>
   /// <returns>
   ///   A <see cref="FrInseeNumberValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="insee"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static FrInseeNumberValidationResult Validate(String? insee)
   {
      if (String.IsNullOrWhiteSpace(insee))
      {
         return FrInseeNumberValidationResult.Empty;
      }
      else if (insee.Length is not UnformattedLength and not FormattedLength)
      {
         return FrInseeNumberValidationResult.InvalidLength;
      }

      // After performing basic checks, validate the check digits because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      FrInseeNumberValidationResult validationResult = ValidateCheckDigits(insee);
      if (validationResult != FrInseeNumberValidationResult.ValidationPassed)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }
      else if (!ValidateSeparators(insee))
      {
         return FrInseeNumberValidationResult.InvalidSeparator;
      }
      else if (!ValidateGender(insee))
      {
         return FrInseeNumberValidationResult.InvalidGender;
      }
      else if (!ValidateMonth(insee))
      {
         return FrInseeNumberValidationResult.InvalidMonth;
      }
      else if (!ValidateDepartment(insee))
      {
         return FrInseeNumberValidationResult.InvalidDepartment;
      }

      return FrInseeNumberValidationResult.ValidationPassed;
   }

   private static String GetRawValue(String insee)
   {
      if (insee.Length == UnformattedLength)
      {
         return insee;
      }

      var buffer = ArrayPool<Char>.Shared.Rent(UnformattedLength);
      try
      {
         ReadOnlySpan<Char> source = insee.AsSpan();
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

   private static FrInseeNumberValidationResult ValidateCheckDigits(ReadOnlySpan<Char> insee)
   {
      var processLength = insee.Length - 2;      // Exclude check digits from main loop
      var isFormatted = IsFormatted(insee);
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
         var num = insee[index].ToSingleDigit();
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
               corsicanOffset = insee[index] switch
               {
                  Chars.UpperCaseA => 1000000,
                  Chars.UpperCaseB => 2000000,
                  _ => 0L                          // Not a valid Corsican department code
               };
            }

            // If not part of a valid Corsican department code then the character is invalid.
            if (corsicanOffset == 0L)
            {
               return FrInseeNumberValidationResult.InvalidCharacter;
            }

            // If using corsican offset then num is ignored for this character.
            num = 0;
         }

         sum += num;
      }

      sum -= corsicanOffset;
      var remainder = 97 - (sum % 97);

      var c1 = insee[^CheckDigit1Offset].ToSingleDigit();
      var c2 = insee[^CheckDigit2Offset].ToSingleDigit();
      if (!c1.IsValidDigit() || !c2.IsValidDigit())
      {
         return FrInseeNumberValidationResult.InvalidCharacter;
      }
      var checkSum = (c1 * 10) + c2;

      return checkSum == remainder
         ? FrInseeNumberValidationResult.ValidationPassed
         : FrInseeNumberValidationResult.InvalidCheckDigits;
   }

   private static Boolean ValidateDepartment(ReadOnlySpan<Char> insee)
   {
      var start = IsFormatted(insee)
         ? FormattedDepartmentOffset
         : UnformattedDepartmentOffset;
      var end = start + 2;

      ReadOnlySpan<Char> department = insee[start..end];
      if (FrDepartmentCodes.ValidateDepartmentCode(department))
      {
         return true;
      }
      else if (department.Equals(OverseasDepartmentPrefix, StringComparison.OrdinalIgnoreCase))
      {
         // Possible overseas department.
         ReadOnlySpan<Char> extendedDepartment = IsFormatted(insee)
            ? [ ..department, insee[end + 1] ]                       // If formatted, we have to skip over separator between department and commune
            : insee[start..(end + 1)];                               // If not formatted, simply extend the slice

         return FrDepartmentCodes.ValidateDepartmentCode(extendedDepartment);
      }

      return false;
   }

   private static Boolean ValidateGender(ReadOnlySpan<Char> insee)
      => insee[GenderOffset] is Chars.DigitOne or Chars.DigitTwo or Chars.DigitSeven or Chars.DigitEight;

   private static Boolean ValidateMonth(ReadOnlySpan<Char> insee)
   {
      var month = IsFormatted(insee)
         ? insee[FormattedMonthOffset..].ParseTwoDigits()
         : insee[UnformattedMonthOffset..].ParseTwoDigits();

      return month switch
      {
         >= 1 and <= 12 => true,
         13 => true,
         >= 20 and <= 42 => true,
         >= 50 => true,
         _ => false
      };
   }

   private static Boolean ValidateSeparators(ReadOnlySpan<Char> insee)
   {
      if (insee.Length == UnformattedLength)
      {
         return true;
      }

      var separator = insee[Separator1Offset];

      // Separator may not be an ASCII digit and all separators must be the same.
      return !separator.IsAsciiDigit()
             && insee[Separator2Offset] == separator
             && insee[Separator3Offset] == separator
             && insee[Separator4Offset] == separator
             && insee[Separator5Offset] == separator
             && insee[Separator6Offset] == separator;
   }
}

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
