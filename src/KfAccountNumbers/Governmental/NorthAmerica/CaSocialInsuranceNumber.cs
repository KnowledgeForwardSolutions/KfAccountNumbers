// Ignore Spelling: Luhn

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Strongly typed business object for a CA Social Insurance Number (SIN).
/// </summary>
/// <remarks>
///   A valid Canadian Social Insurance Number (SIN) consists of nine decimal 
///   digits. The nine digits are commonly separated into three groups of three
///   digits with a separator character (e.g. 123-456-789). The SIN uses the
/// </remarks>
public record CaSocialInsuranceNumber
{
   public const Char DefaultSeparator = Chars.Dash;

   private const Int32 FormattedLength = 11;
   private const Int32 NonFormattedLength = 9;

   private const Int32 FirstSeparatorOffset = 3;
   private const Int32 SecondSeparatorOffset = 7;

   /// <summary>
   ///   Initialize a new <see cref="CaSocialInsuranceNumber"/>.
   /// </summary>
   /// <param name="sin">
   ///   The string representation of a Social Insurance Number.
   /// </param>
   /// <param name="separator">
   ///   Optional. If the <paramref name="sin"/> is 11 characters in length, 
   ///   then <paramref name="separator"/> identifies the character used to
   ///   separate the different sections of the SIN. This parameter is ignored 
   ///   if the <paramref name="sin"/> is 9 characters in length. Defaults to '-'.
   /// </param>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="separator"/> is an ASCII digit (0-9).
   /// </exception>
   /// <exception cref="InvalidCaSocialInsuranceNumberException">
   ///   <paramref name="sin"/> is <see langword="null"/>, empty or all 
   ///   whitespace characters.
   ///   - or -
   ///   <paramref name="sin"/> does not have length of 9 or 11.
   ///   - or -
   ///   <paramref name="sin"/> contains a non-ASCII digit (not 0-9).
   ///   - or -
   ///   <paramref name="sin"/> is 11 characters in length and contains an 
   ///   invalid separator character.
   ///   - or -
   ///   <paramref name="sin"/> fails the Luhn check digit validation.
   ///   - or -
   ///   <paramref name="sin"/> contains an invalid province code (first digit
   ///   may not be zero or eight).
   /// </exception>
   public CaSocialInsuranceNumber(String? sin, Char separator = DefaultSeparator)
   {
      var validationResult = Validate(sin, separator);
      if (validationResult != CaSocialInsuranceNumberValidationResult.ValidationPassed)
      {
         throw new InvalidCaSocialInsuranceNumberException(validationResult);
      }

      Value = GetValidatedSin(sin!);
   }

   /// <summary>
   ///   Private constructor to support <see cref="Create(String?, Char)"/>
   ///   method.
   /// </summary>
   /// <remarks>
   ///   Boolean discard parameter is used to differentiate this constructor
   ///   from the public constructor.
   /// </remarks>
   /// <param name="sin"></param>
   private CaSocialInsuranceNumber(String sin, Boolean _)
   {
      Value = GetValidatedSin(sin);
   }

   /// <summary>
   ///   The raw SSN value.
   /// </summary>
   public String Value { get; init; }

   public static implicit operator String(CaSocialInsuranceNumber sin)
      => sin?.Value ?? throw new ArgumentNullException(nameof(sin), Messages.CaSinInvalidNullConversionToString);

   public static implicit operator CaSocialInsuranceNumber(String? sin) => new(sin, DefaultSeparator);

   /// <summary>
   ///   Create a new <see cref="CaSocialInsuranceNumber"/>.
   /// </summary>
   /// <param name="sin">
   ///   String representation of a Social Insurance Number.
   /// </param>
   /// <param name="separator">
   ///   Optional. If the <paramref name="sin"/> is 11 characters in length, 
   ///   then <paramref name="separator"/> identifies the character used to
   ///   separate the different sections of the SIN. This parameter is ignored 
   ///   if the <paramref name="sin"/> is 9 characters in length. Defaults to '-'.
   /// </param>
   /// <returns>
   ///   A <see cref="CreateResult{CaSocialInsuranceNumber, CaSocialInsuranceNumberValidationResult}"/>.
   ///   Will contain the new <see cref="CaSocialInsuranceNumber"/> if 
   ///   <paramref name="sin"/> is valid or 
   ///   <see cref="CaSocialInsuranceNumberValidationResult"/> that identifies
   ///   the validation rule that was failed if <paramref name="sin"/> is 
   ///   invalid.
   /// </returns>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="separator"/> is an ASCII digit (0-9).
   /// </exception>
   public static CreateResult<CaSocialInsuranceNumber, CaSocialInsuranceNumberValidationResult> Create(
      String? sin,
      Char separator = DefaultSeparator)
   {
      var validationResult = Validate(sin, separator);
      return validationResult == CaSocialInsuranceNumberValidationResult.ValidationPassed
         ? new CaSocialInsuranceNumber(sin!, true)          // Note: invoking private ctor
         : validationResult;
   }

   /// <summary>
   ///   Check the <paramref name="sin"/> to determine if it contains any 
   ///   validation errors.
   /// </summary>
   /// <param name="sin">
   ///   String representation of a Social Insurance Number.
   /// </param>
   /// <param name="separator">
   ///   Optional. If the <paramref name="sin"/> is 11 characters in length, 
   ///   then <paramref name="separator"/> identifies the character used to
   ///   separate the different sections of the SSN. This parameter is ignored 
   ///   if the <paramref name="sin"/> is 9 characters in length. Defaults to '-'.
   /// </param>
   /// <returns>
   ///   A <see cref="CaSocialInsuranceNumberValidationResult"/> enumeration 
   ///   value that indicates if the <paramref name="sin"/> passed validation
   ///   or what validation error was encountered.
   /// </returns>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   <paramref name="separator"/> is an ASCII digit (0-9).
   /// </exception>
   public static CaSocialInsuranceNumberValidationResult Validate(
      String? sin,
      Char separator = DefaultSeparator)
   {
      if (!ValidateSeparatorCharacter(separator))
      {
         throw new ArgumentOutOfRangeException(nameof(separator), separator, Messages.CaSinInvalidCustomSeparatorCharacter);
      }

      // Basic checks for empty/null and length and formatting.
      if (String.IsNullOrWhiteSpace(sin))
      {
         return CaSocialInsuranceNumberValidationResult.Empty;
      }
      else if (sin.Length != NonFormattedLength && sin.Length != FormattedLength)
      {
         return CaSocialInsuranceNumberValidationResult.InvalidLength;
      }
      else if (IsFormattedSin(sin) && !ValidateEmbeddedSeparatorCharacters(sin, separator))
      {
         return CaSocialInsuranceNumberValidationResult.InvalidSeparatorEncountered;
      }

      // Validate the check digit and province code.
      var validCheckDigit = IsFormattedSin(sin)
         ? CheckDigitAlgorithms.Luhn.Validate(sin, CheckDigitMasks.CaSocialInsuranceNumberMask)
         : CheckDigitAlgorithms.Luhn.Validate(sin);
      if (!validCheckDigit)
      {
         // Either invalid check digit or invalid character encountered.
         return ValidateDigits(sin)
            ? CaSocialInsuranceNumberValidationResult.InvalidCheckDigit
            : CaSocialInsuranceNumberValidationResult.InvalidCharacterEncountered;
      }
      else if (!ValidateProvince(sin))
      {
         return CaSocialInsuranceNumberValidationResult.InvalidProvince;
      }

      return CaSocialInsuranceNumberValidationResult.ValidationPassed;
   }

   private static void CopySection(
      ReadOnlySpan<Char> source,
      Span<Char> target,
      Int32 section)
   {
      var start = section * 3;
      var end = (section + 1) * 3;
      var formattedOffset = IsFormattedSin(source) ? section : 0;

      var sourceSpan = source[(start + formattedOffset)..(end + formattedOffset)];
      var targetSpan = target[start..end];

      sourceSpan.CopyTo(targetSpan);
   }

   /// <summary>
   ///   Get an unformatted SIN value from a string that has passed validation.
   ///   If the source string is formatted, then create a new string by merging
   ///   all three SIN sections together without allocating intermediate 
   ///   Strings.
   /// </summary>
   private static String GetValidatedSin(String sin)
   {
      if (sin.Length == NonFormattedLength)
      {
         return sin;
      }

      var buffer = ArrayPool<Char>.Shared.Rent(NonFormattedLength);
      try
      {
         var span = new Span<Char>(buffer);

         CopySection(sin, span, 0);
         CopySection(sin, span, 1);
         CopySection(sin, span, 2);

         return span[..NonFormattedLength].ToString();
      }
      finally
      {
         ArrayPool<Char>.Shared.Return(buffer);
      }
   }

   private static Boolean IsFormattedSin(ReadOnlySpan<Char> sin) => sin.Length == FormattedLength;

   private static Boolean ValidateDigits(String sin)
   {
      var isFormatted = IsFormattedSin(sin);
      for (var index = 0; index < sin.Length; index++)
      {
         if (isFormatted && (index == FirstSeparatorOffset || index == SecondSeparatorOffset))
         {
            continue;
         }
         if (!sin[index].IsAsciiDigit())
         {
            return false;
         }
      }
      return true;
   }

   private static Boolean ValidateEmbeddedSeparatorCharacters(
         ReadOnlySpan<Char> sin,
         Char separator)
      // If SIN is formatted, must contain valid separator character between sections.
      => sin.Length == NonFormattedLength || (sin[FirstSeparatorOffset] == separator && sin[SecondSeparatorOffset] == separator);

   private static Boolean ValidateProvince(ReadOnlySpan<Char> sin)
      => sin[0] != Chars.DigitZero && sin[0] != Chars.DigitEight;

   // A separator character may be any character except ASCII digits (which are
   // valid SIN characters).
   private static Boolean ValidateSeparatorCharacter(Char separator)
      => !separator.IsAsciiDigit();
}
