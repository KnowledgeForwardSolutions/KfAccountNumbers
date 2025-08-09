using System.Runtime.CompilerServices;

namespace KfAccountNumbers.Utility;

public static class ExtensionMethods
{
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
}
