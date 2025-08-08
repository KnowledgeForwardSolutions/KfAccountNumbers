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
}
