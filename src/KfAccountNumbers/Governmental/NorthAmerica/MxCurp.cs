// Ignore Spelling: Curp Json Mx

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
///               Has alphabetic characters derived the persons surname(s) and
///               given name in positions 0-3 and 13-15 (zero-based).
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
///               duplicate CURP values. A digit homoclave character indicatesa
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
   private const Int32 ValidLength = 18;

   private const Int32 GenderOffset = 10;
   private const Int32 HomoclaveOffset = 16;
   private const Int32 CheckDigitOffset = 17;

   private enum CurpSection
   {
      Initials,
      DateOfBirth,
      GenderCode,
      StateCode,
      InternalConsonants,
      Homoclave,
      CheckDigit,
   }

   /// <summary>
   ///   Initialize a new instance of the <see cref="MxCurp"/> class.
   /// </summary>
   /// <param name="curp">
   ///   String representation of a CURP.
   /// </param>
   /// <remarks>
   ///   Validation of <paramref name="curp"/> is performed in a case-insensitive
   ///   manner. However, the <see cref="Value"/> property will normalize the
   ///   CURP to upper-case.
   /// </remarks>
   /// <exception cref="KfValidationException{MxCurpValidationResult}">
   ///   <paramref name="curp"/> is <see langword="null"/>, empty or all
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="curp"/> does not have length of 18.
   ///   - or -
   ///   <paramref name="curp"/> contains non-alphabetic characters in positions
   ///   0-3 or 13-15 (zero-based).
   ///   - or -
   ///   <paramref name="curp"/> contains an invalid date of birth in positions
   ///   4-9 (zero-based).
   ///   - or -
   ///   <paramref name="curp"/> contains an invalid gender character in position
   ///   10 (zero-based). Valid gender characters are H (Hombre/male),
   ///   M (Mujer/female) or X (non-binary).
   ///   - or -
   ///   <paramref name="curp"/> contains an invalid state code in positions
   ///   11-12 (zero-based).
   ///   - or -
   ///   <paramref name="curp"/> contains a non-alphanumeric homoclave character
   ///   in position 16 (zero-based).
   ///   - or -
   ///   <paramref name="curp"/> contains a non-digit check digit character in
   ///   position 17 (zero-based).
   /// </exception>
   public MxCurp(String? curp) : this(curp, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has already
   ///   been validated.
   /// </summary>
   private MxCurp(String? curp, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         MxCurpValidationResult validationResult = Validate(curp);
         if (validationResult != MxCurpValidationResult.ValidationPassed)
         {
            throw validationResult.ToValidationException();
         }
      }

      Value = curp!.ToUpperInvariant();
   }

   /// <summary>
   ///   The person's date of birth, derived from the YYMMDD date of birth and
   ///   homoclave elements of the CURP.
   /// </summary>
   /// <remarks>
   ///   Homoclave values 0-9 indicate birth in the 1900-1999 century, homoclave
   ///   values A-Z indicate birth in the 2000-2099 century.
   /// </remarks>
   public DateOnly DateOfBirth
   {
      get
      {
         ReadOnlySpan<Char> dateOfBirthSpan = GetSectionSpan(Value, CurpSection.DateOfBirth);
#pragma warning disable IDE0008 // Use explicit type
         var (year, month, day) = GetYearMonthDay(dateOfBirthSpan);
#pragma warning restore IDE0008 // Use explicit type
         year += Char.IsAsciiDigit(Value[HomoclaveOffset]) ? 1900 : 2000;

         return new DateOnly(year, month, day);
      }
   }

   /// <summary>
   ///   The person's gender, as coded in the CURP.
   /// </summary>
   /// <remarks>
   ///   Gender will be H (Hombre/male), M (Mujer/female) or X (non-binary).
   /// </remarks>
   public Char GenderCode => Value[GenderOffset];

   /// <summary>
   ///   The person's state of birth as coded in the CURP.
   /// </summary>
   public String StateCode => GetSectionSpan(Value, CurpSection.StateCode).ToString();

   /// <summary>
   ///   The raw CURP value.
   /// </summary>
   /// <remarks>
   ///   The CURP value is always normalized to upper-case.
   /// </remarks>
   public String Value { get; private init; }

   public static implicit operator String(MxCurp curp)
      => curp?.Value ?? String.Empty;     // Handle null CURP object gracefully by returning empty string

   // Explicit conversion from String to avoid unintentional conversions that may throw exceptions.
   public static explicit operator MxCurp(String curp) => new(curp);

   /// <summary>
   ///   Get a string representation of the CURP.
   /// </summary>
   public override String ToString() => Value;

   /// <summary>
   ///   Create a new <see cref="MxCurp"/> using the Result pattern.
   /// </summary>
   /// <param name="curp">
   ///   String representation of a CURP.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{MxCurp, MxCurpValidationResult}"/>.
   ///   Will contain the new <see cref="MxCurpValidationResult"/> if
   ///   <paramref name="curp"/> is valid or
   ///   <see cref="MxCurpValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="curp"/> is
   ///   invalid.
   /// </returns>
   public static CreateResult<MxCurp, MxCurpValidationResult> Create(String? curp)
   {
      MxCurpValidationResult validationResult = Validate(curp);
      return validationResult == MxCurpValidationResult.ValidationPassed
         ? new MxCurp(curp, validationMode: ValidationMode.BypassValidation)
         : validationResult;
   }

   /// <summary>
   ///   Check the <paramref name="curp"/> to determine if it contains a valid
   ///   CURP value.
   /// </summary>
   /// <param name="curp">
   ///   String representation of a CURP.
   /// </param>
   /// <returns>
   ///   A <see cref="MxCurpValidationResult"/> enumeration
   ///   value that indicates if the <paramref name="curp"/> passed validation
   ///   or what validation error was encountered.
   /// </returns>
   /// <remarks>
   ///   Validation is case-insensitive.
   /// </remarks>
   public static MxCurpValidationResult Validate(String? curp)
   {
      if (String.IsNullOrWhiteSpace(curp))
      {
         return MxCurpValidationResult.Empty;
      }
      else if (curp.Length != ValidLength)
      {
         return MxCurpValidationResult.InvalidLength;
      }
      else if (!ValidateNameCharacters(curp))
      {
         return MxCurpValidationResult.InvalidAlphabeticCharacterEncountered;
      }
      else if (!Char.IsAsciiLetterOrDigit(curp[HomoclaveOffset]))
      {
         // Check moved prior to date of birth validation since homoclave is used to determine
         // the century for leap year date of birth validation.
         return MxCurpValidationResult.InvalidHomoclave;
      }
      else if (!ValidateDateOfBirth(curp))
      {
         return MxCurpValidationResult.InvalidDateOfBirth;
      }
      else if (!ValidateGenderCode(curp))
      {
         return MxCurpValidationResult.InvalidGender;
      }
      else if (!ValidateStateCode(curp))
      {
         return MxCurpValidationResult.InvalidState;
      }
      else if (!Char.IsAsciiDigit(curp[CheckDigitOffset]))
      {
         return MxCurpValidationResult.InvalidCheckDigit;
      }

      return MxCurpValidationResult.ValidationPassed;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static ReadOnlySpan<Char> GetSectionSpan(ReadOnlySpan<Char> curp, CurpSection section)
      => section switch
      {
         CurpSection.Initials => curp[..4],
         CurpSection.DateOfBirth => curp[4..10],
         CurpSection.GenderCode => curp[10..11],
         CurpSection.StateCode => curp[11..13],
         CurpSection.InternalConsonants => curp[13..16],
         CurpSection.Homoclave => curp[16..17],
         CurpSection.CheckDigit => curp[17..],
         _ => throw new SwitchExpressionException(section),
      };

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static (Int32 year, Int32 month, Int32 day) GetYearMonthDay(ReadOnlySpan<Char> dateOfBirthSpan)
   {
      var year = dateOfBirthSpan.ParseTwoDigits();
      var month = dateOfBirthSpan[2..].ParseTwoDigits();
      var day = dateOfBirthSpan[4..].ParseTwoDigits();
      return (year, month, day);
   }

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> curp)
   {
      ReadOnlySpan<Char> dateOfBirthSpan = GetSectionSpan(curp, CurpSection.DateOfBirth);

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
      year += curp[HomoclaveOffset].IsAsciiDigit()
         ? 1900
         : 2000;

      return day >= 1 && day <= DateTime.DaysInMonth(year, month);
   }

   private static Boolean ValidateGenderCode(ReadOnlySpan<Char> curp)
   {
      var genderCode = curp[GenderOffset];

      return genderCode is Chars.UpperCaseH
         or Chars.UpperCaseM
         or Chars.UpperCaseX
         or Chars.LowerCaseH
         or Chars.LowerCaseM
         or Chars.LowerCaseX;
   }

   private static Boolean ValidateNameCharacters(ReadOnlySpan<Char> curp)
   {
      ReadOnlySpan<Char> initials = GetSectionSpan(curp, CurpSection.Initials);
      foreach (var c in initials)
      {
         if (!Char.IsAsciiLetter(c))
         {
            return false;
         }
      }

      ReadOnlySpan<Char> internalConsonants = GetSectionSpan(curp, CurpSection.InternalConsonants);
      foreach (var c in internalConsonants)
      {
         if (!Char.IsAsciiLetter(c))
         {
            return false;
         }
      }

      return true;
   }

   private static Boolean ValidateStateCode(ReadOnlySpan<Char> curp)
   {
      ReadOnlySpan<Char> stateCode = GetSectionSpan(curp, CurpSection.StateCode);

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
