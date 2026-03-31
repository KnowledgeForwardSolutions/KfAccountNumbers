// Ignore Spelling: Burgerservicenummer

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Strongly typed business object that represents a Dutch Burgerservicenummer
///   or BSN.
/// </summary>
/// <remarks>
///   <para>
///      A burgerservicenummer is a nine-digit number without embedded personal
///      information or attributes other than a trailing check digit calculated
///      using a variation of the modulus 11 algorithm. The number is typically
///      displayed as nine consecutive digits or formatted as NNNN-NN-NNN.
///   </para>
///   <para>
///      When creating a new <see cref="NlBurgerservicenummer"/>, the following
///      validation rules are applied:
///      <list type="bullet">
///         <item>
///            <description>
///               The value may not be null, empty or all whitespace characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The value must be either 9 characters (without separators) or 11
///               characters (with separators) in length.
///            </description>
///         </item>
///         <item>
///            <description>
///               All characters (except the optional separator characters) must be
///               ASCII digits ('0'-'9').
///            </description>
///         </item>
///         <item>
///            <description>
///               If the length of the value is 11 characters, then character
///               positions 4 and 7 (zero-based) must be valid separator characters.
///               Valid separator characters are any non-ASCII digit characters.
///               The same character must be used for both separator characters.
///            </description>
///         </item>
///         <item>
///            <description>
///               The trailing (right-most) character position must be a valid
///               check digit according to the variant modulus 11 algorithm.
///            </description>
///         </item>
///      </list>
///   </para>
///   <para>
///      The variant modulus 11 algorithm used for burgerservicenummer assigns a
///      weight of -1 to the check digit instead of the weight of 1 that is
///      normally used for modulus 11 check digits.
///   </para>
///   <para>
///      See https://nl.wikipedia.org/wiki/Burgerservicenummer for more info.
///   </para>
/// </remarks>
public record NlBurgerservicenummer
{
   private const Int32 UnformattedLength = 9;
   private const Int32 FormattedLength = 11;

   private const Int32 FirstSeparatorOffset = 4;
   private const Int32 SecondSeparatorOffset = 7;

   private const Int32 CheckDigitOffset = 1;    // Measured from end of value

   /// <summary>
   ///   Initialize a new instance of the <see cref="NlBurgerservicenummer"/> class.
   /// </summary>
   /// <param name="burgerservicenummer">
   ///   String representation of a Dutch burgerservicenummer.
   /// </param>
   /// <exception cref="KfValidationException{NlBurgerservicenummerValidationResult}">
   ///   <paramref name="burgerservicenummer"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="burgerservicenummer"/> is not length 9 (or 11 if a separator
   ///   character is used).
   ///   - or -
   ///   <paramref name="burgerservicenummer"/> contains a non-digit character in
   ///   any position other than the separator locations.
   ///   - or -
   ///   <paramref name="burgerservicenummer"/> is 11 characters in length and has an
   ///   ASCII digit ('0'-'9') in a separator location
   ///   - or -
   ///   <paramref name="burgerservicenummer"/> is 11 characters in length and has
   ///   two different separator characters.
   ///   - or -
   ///   <paramref name="burgerservicenummer"/> contains an invalid modulus 11 (11-proef)
   ///   check digit in the trailing (right-most) character position.
   /// </exception>
   public NlBurgerservicenummer(String? burgerservicenummer)
      : this(burgerservicenummer, ValidationMode.ValidationRequired) { }

   /// <summary>
   ///   Private constructor that actually does the work. Supports bypassing
   ///   validation when creating a new instance from a value that has already
   ///   been validated.
   /// </summary>
   private NlBurgerservicenummer(String? burgerservicenummer, ValidationMode validationMode)
   {
      if (validationMode == ValidationMode.ValidationRequired)
      {
         NlBurgerservicenummerValidationResult validationResult = Validate(burgerservicenummer);
         if (validationResult != NlBurgerservicenummerValidationResult.ValidationPassed)
         {
            throw validationResult.ToValidationException();
         }
      }

      Value = GetRawValue(burgerservicenummer!);
   }

   /// <summary>
   ///   The raw burgerservicenummer value.
   /// </summary>
   public String Value { get; private init; }

   public static implicit operator String(NlBurgerservicenummer burgerservicenummer)
      => burgerservicenummer?.Value ?? String.Empty;      // Handle null object gracefully by returning empty string

   // Explicit conversion from String to avoid unintentional conversions that may throw exceptions.
   public static explicit operator NlBurgerservicenummer(String? burgerservicenummer) => new(burgerservicenummer);

   /// <summary>
   ///   Create a new <see cref="NlBurgerservicenummer"/> using the Result pattern.
   /// </summary>
   /// <param name="burgerservicenummer">
   ///   String representation of a Dutch burgerservicenummer.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{NlBurgerservicenummer, NlBurgerservicenummerValidationResult}"/>.
   ///   Will contain the new <see cref="NlBurgerservicenummer"/> if 
   ///   <paramref name="burgerservicenummer"/> is valid or an
   ///   <see cref="NlBurgerservicenummerValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="burgerservicenummer"/> is 
   ///   invalid.
   /// </returns>
   public static CreateResult<NlBurgerservicenummer, NlBurgerservicenummerValidationResult> Create(String? burgerservicenummer)
   {
      NlBurgerservicenummerValidationResult validationResult = Validate(burgerservicenummer);
      return validationResult == NlBurgerservicenummerValidationResult.ValidationPassed
         ? new NlBurgerservicenummer(burgerservicenummer, validationMode: ValidationMode.BypassValidation)
         : validationResult;
   }

   /// <summary>
   ///   Check the <paramref name="burgerservicenummer"/> to determine if it contains a
   ///   valid Dutch burgerservicenummer.
   /// </summary>
   /// <param name="burgerservicenummer">
   ///   String representation of a Dutch burgerservicenummer.
   /// </param>
   /// <returns>
   ///   A <see cref="NlBurgerservicenummerValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="burgerservicenummer"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static NlBurgerservicenummerValidationResult Validate(String? burgerservicenummer)
   {
      if (String.IsNullOrWhiteSpace(burgerservicenummer))
      {
         return NlBurgerservicenummerValidationResult.Empty;
      }
      else if (burgerservicenummer.Length is not UnformattedLength and not FormattedLength)
      {
         return NlBurgerservicenummerValidationResult.InvalidLength;
      }

      // After performing basic checks, validate the check digit because the
      // most common source of errors will be data entry errors. Then validate
      // the subcomponents of the value.
      NlBurgerservicenummerValidationResult validationResult = ValidateCheckDigit(burgerservicenummer);
      if (validationResult != NlBurgerservicenummerValidationResult.ValidationPassed)
      {
         // Could be either InvalidCharacter or InvalidCheckDigit.
         return validationResult;
      }
      else if (!ValidateSeparator(burgerservicenummer))
      {
         return NlBurgerservicenummerValidationResult.InvalidSeparator;
      }


      return NlBurgerservicenummerValidationResult.ValidationPassed;
   }

   private static String GetRawValue(String burgerservicenummer)
      => burgerservicenummer.Length == UnformattedLength
         ? burgerservicenummer
         : String.Concat(
            burgerservicenummer.AsSpan(0, FirstSeparatorOffset),
            burgerservicenummer.AsSpan(FirstSeparatorOffset + 1, 2),
            burgerservicenummer.AsSpan(SecondSeparatorOffset + 1));

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static Boolean IsFormatted(ReadOnlySpan<Char> burgerservicenummer)
      => burgerservicenummer.Length == FormattedLength;

   private static NlBurgerservicenummerValidationResult ValidateCheckDigit(ReadOnlySpan<Char> burgerservicenummer)
   {
      var sum = 0;
      var weight = 9;
      var isFormatted = IsFormatted(burgerservicenummer);
      var processLength = burgerservicenummer.Length - 1;      // Handle check digit separately

      for (var index = 0; index < processLength; index ++)
      {
         if (isFormatted && (index == FirstSeparatorOffset || index == SecondSeparatorOffset))
         {
            continue;
         }

         var num = burgerservicenummer[index] - Chars.DigitZero;
         if (num < 0 || num > 9)
         {
            return NlBurgerservicenummerValidationResult.InvalidCharacter;
         }

         sum += (num * weight);
         weight--;
      }

      var checkDigit = burgerservicenummer[^CheckDigitOffset] - Chars.DigitZero;
      if (checkDigit < 0 || checkDigit > 9)
      {
         return NlBurgerservicenummerValidationResult.InvalidCharacter;
      }

      sum -= checkDigit;

      return sum % 11 == 0
         ? NlBurgerservicenummerValidationResult.ValidationPassed
         : NlBurgerservicenummerValidationResult.InvalidCheckDigit;
   }

   private static Boolean ValidateSeparator(ReadOnlySpan<Char> burgerservicenummer)
   {
      if (burgerservicenummer.Length == UnformattedLength)
      {
         return true;
      }

      var s1 = burgerservicenummer[FirstSeparatorOffset];
      var s2 = burgerservicenummer[SecondSeparatorOffset];

      return s1 == s2 && !s1.IsAsciiDigit();
   }
}
