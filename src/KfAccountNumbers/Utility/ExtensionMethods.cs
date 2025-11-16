namespace KfAccountNumbers.Utility;

public static class ExtensionMethods
{
   /// <summary>
   ///   Format a <paramref name="str"/> using a <paramref name="mask"/>.
   /// </summary>
   /// <param name="str">
   ///   The string to format.
   /// </param>
   /// <param name="mask">
   ///   The mask that specified the final output.
   /// </param>
   /// <returns>
   ///   A string that contains the input <paramref name="str"/> after applying
   ///   the <paramref name="mask"/>.
   /// </returns>
   /// <exception cref="ArgumentNullException">
   ///   <paramref name="str"/> is <see langword="null"/>.
   ///   - or -
   ///   <paramref name="mask"/> is <see langword="null"/>.
   /// </exception>
   /// <exception cref="ArgumentException">
   ///   <paramref name="mask"/> is <see cref="String.Empty"/> or all whitespace
   ///   characters.
   /// </exception>
   /// <remarks>
   ///   <para>
   ///      The <paramref name="mask"/> is applied one character at a time,
   ///      moving from left to right. If the character in the mask is an 
   ///      underscore ('_') then the next available character from the 
   ///      <paramref name="str"/> is added to the output. Otherwise the mask 
   ///      character is considered a literal character and it is added to the
   ///      output instead. If there are no remaining <paramref name="str"/>
   ///      characters then additional mask underscore characters are ignored.
   ///   </para>
   ///   <para>
   ///      The backslash character ('\') is used as an escape character. Use 
   ///      the sequence "\_" to add a literal underscore to the output and use
   ///      the sequence "\\" to add a literal backslash to the output.
   ///   </para>
   /// </remarks>
   /// <example>
   ///   str = "012345678", mask = "___-__-____" => "012-34-5678"
   ///   
   ///   str = "8005551212", mask = "(___) ___-____" => "(800) 555-1212"
   ///   
   ///   str = "abcdef", mask = "___ \_ ___" => "abc _ def"
   ///   
   ///   str = "abc", mask = "__.__.__" => "ab.c."
   /// </example>
   public static String FormatWithMask(this String str, String mask)
   {
      if (str is null)
      {
         throw new ArgumentNullException(nameof(str), Messages.FormatStrNull);
      }
      _ = mask.RequiresNotNullOrWhiteSpace(Messages.FormatMaskEmpty);

      var outputChars = new Char[mask.Length];
      var strIndex = 0;
      var maskIndex = 0;
      var outputIndex = 0;

      while(maskIndex < mask.Length)
      {
         var maskChar = mask[maskIndex];
         switch(maskChar)
         {
            case Chars.Backslash:
               maskIndex++;
               outputChars[outputIndex++] = mask[maskIndex];
               break;

            case Chars.Underscore:
               if (strIndex < str.Length)
               {
                  outputChars[outputIndex++] = str[strIndex++];
               }
               break;

            default:
               outputChars[outputIndex++] = maskChar;
               break;
         }
         maskIndex++;
      }

      return outputChars[..outputIndex].AsSpan().ToString();
   }

   /// <summary>
   ///   Indicates if <paramref name="ch"/> is categorized as an ASCII digit.
   /// </summary>
   /// <param name="ch">
   ///   The Unicode character to evaluate.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if <paramref name="ch"/> an ASCII digit; 
   ///   otherwise <see langword="false"/>.
   /// </returns>
   /// <remarks>
   ///   <para>
   ///      This method determines whether the character is in the range '0' 
   ///      through '9', inclusive.
   ///   </para>
   ///   <para>
   ///      Use this method instead of <see cref="Char.IsDigit(Char)"/> because
   ///      IsDigit has some edge cases where Unicode characters for decimal 
   ///      digits in other writing systems (ex. Tamil digit 9, '\u2083') would
   ///      also evaluate as <see langword="true"/>.
   ///   </para>
   /// </remarks>
   public static Boolean IsAsciiDigit(this Char ch) => Char.IsAsciiDigit(ch);

   /// <summary>
   ///   Guard against a String value that is <see langword="null"/>, 
   ///   <see cref="String.Empty"/> or all whitespace characters.
   /// </summary>
   /// <param name="str">
   ///   The String to check.
   /// </param>
   /// <param name="message">
   ///   The message to include if an exception is thrown.
   /// </param>
   /// <returns>
   ///   The <paramref name="str"/> that is verified to not be 
   ///   <see langword="null"/>, <see cref="String.Empty"/> or all whitespace 
   ///   characters.
   /// </returns>
   /// <exception cref="ArgumentNullException">
   ///   <paramref name="str"/> is <see langword="null"/>.
   /// </exception>
   /// <exception cref="ArgumentException">
   ///   <paramref name="str"/> is <see cref="String.Empty"/> or all whitespace
   ///   characters
   /// </exception>
   public static String RequiresNotNullOrWhiteSpace(
      this String str,
      String message,
      [CallerArgumentExpression(nameof(str))] String callerArgumentName = null!)
      => str is null
         ? throw new ArgumentNullException(callerArgumentName, message)
         : String.IsNullOrWhiteSpace(str) 
            ? throw new ArgumentException(message, callerArgumentName) 
            : str;

   /// <summary>
   ///   Validate that String value is not <see langword="null"/>, 
   ///   <see cref="String.Empty"/> or all whitespace characters.
   /// </summary>
   /// <param name="str">
   ///   The String to check.
   /// </param>
   /// <param name="message">
   ///   The message to include if an exception is thrown.
   /// </param>
   /// <exception cref="ArgumentNullException">
   ///   <paramref name="str"/> is <see langword="null"/>.
   /// </exception>
   /// <exception cref="ArgumentException">
   ///   <paramref name="str"/> is <see cref="String.Empty"/> or all whitespace
   ///   characters
   /// </exception>
   public static void ValidateNotNullOrWhiteSpace(
      this String str,
      String message,
      [CallerArgumentExpression(nameof(str))] String callerArgumentName = null!)
   {
      if (str is null)
      {
         throw new ArgumentNullException(callerArgumentName, message);
      }
      if (String.IsNullOrWhiteSpace(str))
      {
         throw new ArgumentException(message, callerArgumentName);
      }
   }
}
