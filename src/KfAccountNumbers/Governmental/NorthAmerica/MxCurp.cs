// Ignore Spelling: Curp Mx

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Strongly typed business object for a Clave Única de Registro de Población
///   (CURP).
/// </summary>
/// <remarks>
///   <para>
///      A valid CURP is an 18-character alphanumeric identifier assigned to 
///      Mexican citizens by the Registry Nacional de Poblacion (RENAPO). 
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
///               Has a valid date of birth in positions 4-9 (zero-based).
///            </description>
///         </item>
///         <item>
///            <description>
///               Has a valid gender character in position 10 (zero-based). Must
///               be H (Hombre), M (Mujer) or X (non-binary).
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
/// </remarks>
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
      CheckDigit
   }

   public MxCurp(String curp)
   {
      throw new NotImplementedException();
   }

   /// <summary>
   ///   The person's date of birth, derived from the YYMMDD date of birth and 
   ///   homoclave elements of the CURP.
   /// </summary>
   /// <remarks>
   ///   Homoclave values 0-9 indicate birth century 1900-1999, homoclave values
   ///   A-Z indicate brith century 2000-2099.
   /// </remarks>
   public DateOnly DateOfBirth => throw new NotImplementedException();

   /// <summary>
   ///   The person's gender, as coded in the CURP.
   /// </summary>
   /// <remarks>
   ///   Gender will be H (Hombre/male), M (Mujer/female) or X (non-binary).
   /// </remarks>
   public Char GenderCode => throw new NotImplementedException();

   /// <summary>
   ///   The person's state of birth as coded in the CURP.
   /// </summary>
   public String StateCode => throw new NotImplementedException();

   /// <summary>
   ///   The raw CURP value.
   /// </summary>
   public String Value { get; init; }

   public static implicit operator String(MxCurp curp)
      => curp?.Value ?? throw new ArgumentNullException(nameof(curp), Messages.MxCurpInvalidNullConversionToString);

   public static implicit operator MxCurp(String curp) => new(curp);

   /// <summary>
   ///   Get a string representation of the CURP.
   /// </summary>
   public override String ToString() => Value;

   public CreateResult<MxCurp, MxCurpValidationResult> Create(String curp)
   {
      throw new NotImplementedException();
   }

   /// <summary>
   ///   Check the <paramref name="curp"/> to determine if it contains any 
   ///   validation errors.
   /// </summary>
   /// <param name="curp">
   ///   String representation of a CURP.
   /// </param>
   /// <returns>
   ///   A <see cref="MxCurpValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="curp"/> passed validation
   ///   or what validation error was encountered.
   /// </returns>
   public static MxCurpValidationResult Validate(String curp)
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
      else if (!Char.IsAsciiLetterOrDigit(curp[HomoclaveOffset]))
      {
         return MxCurpValidationResult.InvalidHomoclave;
      }
      else if (!Char.IsAsciiDigit(curp[CheckDigitOffset]))
      {
         return MxCurpValidationResult.InvalidCheckDigit;
      }

      return MxCurpValidationResult.ValidationPassed;
   }

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
         _ => throw new SwitchExpressionException(section)
      };

   private static Boolean ValidateDateOfBirth(ReadOnlySpan<Char> curp)
   {
      ReadOnlySpan<Char> dateOfBirthSpan = GetSectionSpan(curp, CurpSection.DateOfBirth);
      var isValid = DateTime.TryParseExact(
         dateOfBirthSpan,
         "yyMMdd",
         null,
         System.Globalization.DateTimeStyles.None,
         out _);

      return isValid;
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

   private static Boolean ValidateHomoclaveCharacter(ReadOnlySpan<Char> curp)
   {
      var c = curp[HomoclaveOffset];

      return Char.IsAsciiLetterOrDigit(c);
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
