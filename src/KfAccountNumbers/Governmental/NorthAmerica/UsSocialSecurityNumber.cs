// Ignore Spelling: ssn

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Strongly typed business object for a US Social Security Number.
/// </summary>
public record UsSocialSecurityNumber
{
   private readonly String _ssn;

   private const Int32 AREA_SEPARATOR_OFFSET = 3;
   private const Int32 FORMATTED_LENGTH = 11;
   private const Int32 GROUP_SEPARATOR_OFFSET = 6;
   private const Int32 NONFORMATTED_LENGTH = 9;

   /// <summary>
   ///   Initialize a new <see cref="UsSocialSecurityNumber"/>.
   /// </summary>
   /// <param name="ssn">
   ///   String representation of a Social Security Number.
   /// </param>
   /// <param name="separator">
   ///   Optional. If the <paramref name="ssn"/> is 11 characters in length, 
   ///   then <paramref name="separator"/> identifies the character used to
   ///   separate the different sections of the SSN. This parameter is ignored 
   ///   if the <paramref name="ssn"/> is 9 characters in length. Defaults to '-'.
   /// </param>
   /// <exception cref="ArgumentNullException">
   ///   <paramref name="ssn"/> is <see langword="null"/>.
   /// </exception>
   /// <exception cref="ArgumentException">
   ///   <paramref name="ssn"/> is <see cref="String.Empty"/> or all whitespace
   ///   characters.
   ///   - or -
   ///   <paramref name="ssn"/> does not have length of 9 or 11.
   ///   - or -
   ///   <paramref name="ssn"/> contains a non-ASCII digit (not 0-9).
   ///   - or -
   ///   <paramref name="ssn"/> is 11 characters in length and contains an 
   ///   invalid separator character.
   ///   - or -
   ///   <paramref name="ssn"/> contains an invalid area number (000, 666 or 
   ///   900-999).
   /// </exception>
   public UsSocialSecurityNumber(String ssn, Char separator = '-')
   {
      _ = ssn.RequiresNotNullOrWhiteSpace(Messages.UsSsnEmpty);
      if (!ValidateLength(ssn, out var exception))
      {
         throw exception;
      }

      if (!TryGetSsnValue(ssn, separator, out var value, out exception))
      {
         throw exception;
      }

      _ssn = value;
      var ssnSpan = _ssn.AsSpan();

      if (!ValidateAreaNumber(ssnSpan, out exception))
      {
         throw exception;
      }
   }

   public static implicit operator String(UsSocialSecurityNumber ssn) => ssn._ssn;

   private static Boolean TryGetSsnValue(
      String ssn, 
      Char separator,
      out String value,
      out Exception exception)
   {
      value = String.Empty;
      exception = default!;

      var validCharacters = new Char[NONFORMATTED_LENGTH];
      var inputOffset = 0;
      var resultOffset = 0;

      while(inputOffset < ssn.Length)
      {
         var currentChar = ssn[inputOffset];
         if (ssn.Length == FORMATTED_LENGTH 
            && (inputOffset == AREA_SEPARATOR_OFFSET || inputOffset == GROUP_SEPARATOR_OFFSET))
         {
            if (currentChar != separator)
            {
               var message = String.Format(
                  Messages.UsSsnInvalidSeparator,
                  inputOffset,
                  separator,
                  currentChar);
               exception = new ArgumentException(message, nameof(ssn));
               return false;
            }
         }
         else
         {
            if (!currentChar.IsAsciiDigit())
            {
               var message = String.Format(
                  Messages.UsSsnInvalidCharacter,
                  inputOffset,
                  currentChar);
               exception = new ArgumentException(message, nameof(ssn));
               return false;
            }

            validCharacters[resultOffset] = currentChar;
            resultOffset++;
         }
         inputOffset++;
      }

      value = new String(validCharacters);
      return true;
   }

   private static Boolean ValidateAreaNumber(
      ReadOnlySpan<Char> span,
      out Exception exception)
   {
      const Int32 AREA_LENGTH = 3;
      const String INVALID_AREA_000 = "000";
      const String INVALID_AREA_666 = "666";

      if (span[0] == Chars.DigitNine
         || MemoryExtensions.Equals(span[..AREA_LENGTH], INVALID_AREA_000.AsSpan(), StringComparison.Ordinal)
         || MemoryExtensions.Equals(span[..AREA_LENGTH], INVALID_AREA_666.AsSpan(), StringComparison.Ordinal))
      {
         exception = new ArgumentException(Messages.UsSsnInvalidAreaNumber, "ssn");
         return false;
      }

      exception = default!;
      return true;
   }

   private static Boolean ValidateLength(String ssn, out Exception exception)
   {
      if (ssn?.Length == NONFORMATTED_LENGTH || ssn?.Length == FORMATTED_LENGTH)
      {
         exception = default!;
         return true;
      }

      exception = new ArgumentException(Messages.UsSsnInvalidLength, nameof(ssn));
      return false;
   }
}
