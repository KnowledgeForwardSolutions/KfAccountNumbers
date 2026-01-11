namespace KfAccountNumbers.Tests.Unit.TestData;

internal static class CharacterData
{
   public static Char[] AsciiLowercaseAlphabeticCharacters
      => "abcdefghijklmnopqrstuvwxyz".ToCharArray();

   public static Char[] NumericCharacters 
      => "0123456789".ToCharArray();

   public static Char[] AsciiUppercaseAlphabeticCharacters
      => "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

   public static Char[] AsciiUpperAndLowercaseAlphabeticCharacters
      => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

   public static Char[] AsciiUpperAndLowercaseAlphanumericCharacters
      => "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
}
