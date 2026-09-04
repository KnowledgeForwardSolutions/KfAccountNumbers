#pragma warning disable IDE0250 // Make struct 'readonly'

namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Abstract base class for Spanish identity numbers.
/// </summary>
public abstract record EsIdentityNumberBase
{
   /// <summary>
   ///   Discriminated union defining the types of Spanish identity numbers.
   /// </summary>
   public union IdentifierCategory(EsIdentifierType.Dni, EsIdentifierType.Nie) { }

   /// <summary>
   ///   Discriminated union defining the possible validation errors that can
   ///   occur when creating a new Spanish identity number.
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
   ///   validating a Spanish identity number.
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
   ///   The name of the check digit algorithm used by Spanish identity numbers.
   /// </summary>
   public const String CheckDigitAlgorithmName = "Modulus 23";

   /// <summary>
   ///   The allowed length of an unformatted Spanish NIF.
   /// </summary>
   public const Int32 UnformattedLength = 9;

   /// <summary>
   ///   The allowed length of a formatted Spanish DNI.
   /// </summary>
   public const Int32 DniFormattedLength = 10;

   /// <summary>
   ///   The allowed length of a formatted Spanish NIE.
   /// </summary>
   public const Int32 NieFormattedLength = 11;

   /// <summary>
   ///   Zero-based offset of the leading NIE separator character.
   /// </summary>
   protected const Int32 LeadingSeparatorOffset = 1;

   /// <summary>
   ///   Offset (measured from end of string) of the DNI separator character and
   ///   the trailing NIE separator character.
   /// </summary>
   protected const Int32 TrailingSeparatorOffset = 2;

   /// <summary>
   ///   Offset (measured from end of string) of the check character.
   /// </summary>
   protected const Int32 CheckCharacterOffset = 1;

   /// <summary>
   ///   Modulus 23 check characters in correct order.
   /// </summary>
   protected const String CheckCharacters = "TRWAGMYFPDXBNJZSQVHLCKE";

   /// <summary>
   ///   Hashset used to identify invalid characters in check chararacter
   ///   position.
   /// </summary>
   protected static readonly HashSet<Char> ValidCheckCharacters = [.. CheckCharacters];

   /// <summary>
   ///   Defines what types of identifiers are accepted during validation.
   /// </summary>
   protected enum AllowedIdentifierType
   {
      /// <summary>
      ///   The value may be any Spanish identity number.
      /// </summary>
      Any = 1,

      /// <summary>
      ///   The value must be a Spanish DNI (formatted length = 10 and leading
      ///   character must be a digit).
      /// </summary>
      Dni,

      /// <summary>
      ///   The value must be a Spanish NIE (formatted length = 11 and leading
      ///   character must be X, Y or Z);
      /// </summary>
      Nie,
   };

   /// <summary>
   ///   Get the normalized identifier value, stripped of separator characters
   ///   and with any lower-case characters converted to upper-case.
   /// </summary>
   /// <param name="value">
   ///   The original identifier value.
   /// </param>
   /// <returns>
   ///   The normalized identifier value.
   /// </returns>
   protected static String GetNormalizedValue(String value)
   {
      var rawValue = value.Length switch
      {
         UnformattedLength => value,
         DniFormattedLength => String.Concat(value.AsSpan(..8), value.AsSpan(^1..)),
         NieFormattedLength => String.Concat(value.AsSpan(..1), value.AsSpan(2..^2), value.AsSpan(^1..)),
         _ => throw new UnreachableException("This branch should never be reached"),
      };

      return rawValue.ToUpperInvariant();
   }


   /// <summary>
   ///   Validate that the supplied <paramref name="value"/> contains a valid
   ///   Modulus 32 check character.
   /// </summary>
   /// <param name="value">
   ///   The value to check.
   /// </param>
   /// <param name="allowedIdentifierType">
   ///   Defines how the leading character is processed (DNI must be digit,
   ///   NIE must be X, Y or Z).
   /// </param>
   /// <param name="invalidCharacterPosition">
   ///   Output. The zero-based index of the first invalid character encountered
   ///   or -1 if no invalid characters were found.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if <paramref name="value"/> contains a valid
   ///   Modulus 23 check character; otherwise <see langword="false"/> to
   ///   indicate that either an invalid character was encountered
   ///   (<paramref name="invalidCharacterPosition"/> != -1) or that the check
   ///   character was invalid.
   /// </returns>
   protected static Boolean ValidateCheckDigit(
      ReadOnlySpan<Char> value,
      AllowedIdentifierType allowedIdentifierType,
      out Int32 invalidCharacterPosition)
   {
      invalidCharacterPosition = -1;

      // Process leading character outside main loop.
      var leadingCharacter = Char.ToUpperInvariant(value[0]);
      var sum = allowedIdentifierType switch
      {
         AllowedIdentifierType.Dni => leadingCharacter.ToSingleDigit(),
         AllowedIdentifierType.Nie => ParseNieInitialDigit(leadingCharacter),
         _ => ParseDniOrNieInitialDigit(leadingCharacter)
      };
      if (!sum.IsValidDigit())
      {
         invalidCharacterPosition = 0;
         return false;
      }

      // Handle inner digits.
      var start = value.Length == NieFormattedLength ? 2 : 1;
      var end = value.Length == NieFormattedLength ? 9 : 8;
      for (var index = start; index < end; index++)
      {
         sum *= 10;
         var num = value[index].ToSingleDigit();
         if (!num.IsValidDigit())
         {
            invalidCharacterPosition = index;
            return false;
         }

         sum += num;
      }

      var remainder = sum % 23;
      var checkCharacter = CheckCharacters[remainder];
      var trailingCharacter = value[^CheckCharacterOffset];
      if (trailingCharacter.Equals(checkCharacter, StringComparison.OrdinalIgnoreCase))
      {
         return true;
      }

      // If check character doesn't match, check for character not in
      // set of valid check characters. If not found then set the invalid
      // character position to indicate that the failure was an invalid
      // character instead of an invalid check digit.
      if (!ValidCheckCharacters.Contains(Char.ToUpperInvariant(trailingCharacter)))
      {
         invalidCharacterPosition = value.Length - 1;
      }

      return false;
   }

   /// <summary>
   ///   Validate that the supplied <paramref name="value"/> is either length 9
   ///   (no separators), length 10 with a single valid separator or length 11
   ///   with two identical valid separator characters.
   /// </summary>
   /// <param name="value">
   ///   The value to check.
   /// </param>
   /// <param name="invalidSeparatorPosition">
   ///   Output. The zero-based index of the invalid separator character or -1
   ///   if no invalid separators are found.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if <paramref name="value"/> does not contain
   ///   separators or all separator characters are valid; otherwise
   ///   <see langword="false"/>.
   /// </returns>
   protected static Boolean ValidateSeparators(
      ReadOnlySpan<Char> value,
      out Int32 invalidSeparatorPosition)
   {
      invalidSeparatorPosition = -1;
      if (value.Length == UnformattedLength)
      {
         return true;  // No separators to validate
      }

      var trailingSeparator = value[^TrailingSeparatorOffset];

      // Separator must not be a digit.
      if (trailingSeparator.IsAsciiDigit() || trailingSeparator.IsAsciiLetter())
      {
         invalidSeparatorPosition = value.Length - TrailingSeparatorOffset;
         return false;
      }

      // DNI has only trailing separator.
      if (value.Length == DniFormattedLength)
      {
         return true;
      }

      // NIE has leading and trailing separators - which must match.
      var leadingSeparator = value[LeadingSeparatorOffset];
      if (leadingSeparator.IsAsciiDigit() || leadingSeparator.IsAsciiLetter())
      {
         invalidSeparatorPosition = LeadingSeparatorOffset;
         return false;
      }

      if (leadingSeparator != trailingSeparator)
      {
         invalidSeparatorPosition = value.Length - TrailingSeparatorOffset;
         return false;
      }

      return true;
   }

   private static Int32 ParseDniOrNieInitialDigit(Char ch)
   {
      // Handle digits first (DNI). If invalid, check for possible NIE leading
      // character.
      var num = ch.ToSingleDigit();
      if (!num.IsValidDigit())
      {
         num = ParseNieInitialDigit(ch);
      }

      return num;
   }

   private static Int32 ParseNieInitialDigit(Char ch)
   {
      // Assumes that ch is already upper-case.
      var num = ch - Chars.UpperCaseX;

      return num is < 0 or > 2 // X = 0, Y = 1, Z = 2
         ? -1
         : num;
   }
}
