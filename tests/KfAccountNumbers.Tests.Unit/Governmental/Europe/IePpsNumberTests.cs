namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

public class IePpsNumberTests
{
   private const String Valid8CharacterPpsNumber = "1234567T";
   private const String AltValid8CharacterPpsNumber = "7654321G";
   private const String Valid9CharacterPpsNumber = "1234567FA";
   private const String AltValid9CharacterPpsNumber = "7654321PA";
   private const String Valid8CharacterPpsNumberWithWSuffix = "1234567TW";
   private const String LowerCaseValid8CharacterPpsNumber = "1234567t";
   private const String LowerCaseAltValid8CharacterPpsNumber = "7654321g";
   private const String LowerCaseValid9CharacterPpsNumber = "1234567fa";
   private const String LowerCaseAltValid9CharacterPpsNumber = "7654321pa";
   private const String LowerCaseValid8CharacterPpsNumberWithWSuffix = "1234567tw";
   private const String MixedCaseValid9CharacterPpsNumber = "1234567Fa";
   private const String MixedCaseAltValid9CharacterPpsNumber = "7654321pA";

   private static Char GetCheckDigit(String ppsNumber)
   {
      var sum = 0;
      var weight = 8;
      for(var index = 0; index < 7; index++)
      {
         var num = ppsNumber[index] - Chars.DigitZero;
         sum += (num * weight);
         weight--;
      }

      if (ppsNumber.Length == 9)
      {
         var trailingChar = Char.ToUpper(ppsNumber[8], System.Globalization.CultureInfo.InvariantCulture);
         var trailingCharValue = trailingChar switch
         {
            >= Chars.UpperCaseA and <= Chars.UpperCaseI => (trailingChar - Chars.UpperCaseA) + 1,
            Chars.UpperCaseW => 0,
            _ => throw new InvalidOperationException()
         };
         sum += (trailingCharValue * 9);
      }

      var remainder = sum % 23;
      var checkCharacter = "WABCDEFGHIJKLMNOPQRSTUV"[remainder];     // Note leading W because W is used for zero remainder

      return checkCharacter;
   }

   public static TheoryData<String> InvalidLengthValues =>
   [
      "123456T",              // Length 8
      "12345678FA",           // Length 10
      new String('1', 100)    // Very long string
   ];

   public static TheoryData<String> ValidPpsNumberValues =>
   [
      Valid8CharacterPpsNumber,
      AltValid8CharacterPpsNumber,
      Valid9CharacterPpsNumber,
      AltValid9CharacterPpsNumber,
      Valid8CharacterPpsNumberWithWSuffix,
      LowerCaseValid8CharacterPpsNumber,
      LowerCaseAltValid8CharacterPpsNumber,
      LowerCaseValid9CharacterPpsNumber,
      LowerCaseAltValid9CharacterPpsNumber,
      LowerCaseValid8CharacterPpsNumberWithWSuffix,
      MixedCaseValid9CharacterPpsNumber,
      MixedCaseAltValid9CharacterPpsNumber,
   ];

   public static TheoryData<String> InvalidCharacterValues =>
   [
      " 234567T",                // non-digit character ' '
      "1-34567T",                // non-digit character '-'
      "12=4567T",                // non-digit character '='
      "123A567T",                // non-digit character 'A'
      "1234B67T",                // non-digit character 'B'
      "12345C7T",                // non-digit character 'C'
      "12345a7T",                // non-digit character 'a'
      "12345b7T",                // non-digit character 'b'
      "123456~T",                // non-digit character '~'
      "123456\u2153T",           // non-digit character Unicode fraction 1/3  
      "123456\u00D6T",           // non-digit character unicode O with umlaut
      "1234567 ",                // non-letter check character ' '
      "1234567-",                // non-letter check character '-'
      "1234567~",                // non-letter check character '~'
      "1234567\u2153",           // non-letter check character Unicode fraction 1/3 
      "1234567\u00D6",           // non-letter check character unicode O with umlaut
      "1234567F ",               // non-letter trailing character ' '
      "1234567F-",               // non-letter trailing character '-'
      "1234567F~",               // non-letter trailing character '~'
      "1234567F\u2153",          // non-letter trailing character Unicode fraction 1/3 
      "1234567F\u00D6",          // non-letter trailing character unicode O with umlaut
      "1234567FJ",               // invalid letter trailing character 'J'
      "1234567Fj",               // invalid letter trailing character 'j'
      "1234567FV",               // invalid letter trailing character 'V'
      "1234567Fv",               // invalid letter trailing character 'v'
      "1234567FX",               // invalid letter trailing character 'X'
      "1234567Fx",               // invalid letter trailing character 'x'
   ];

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "1224567T",                // 1234567T with single digit transcription error, 3 -> 2
      "7655321G",                // 7654321G with single digit transcription error, 4 -> 5
      "1122334OC",               // 1122334OB with trailing character transcription error, B -> C
      "1122334PB",               // 1122334OB with check digit transcription error, O -> P
      "1235467TW",               // 1234567TW with two digit transposition error, 45 -> 54
      "7564321PA",               // 7654321PA with two digit transposition error, 65 -> 56
      "1357910IG",               // 1357910GI with two character transposition error, GI -> IG
      "1234765T",                // 1234567T with two digit jump transposition, 567 -> 765
      "1122444OB",               // 1122334OB with two digit twin error, 33 -> 44
      "2222334T",                // 1122334T with two digit twin error, 11 -> 22
   ];

   //[Fact]
   //public void CheckDigitTest()
   //{
   //   var value = "1122334T";
   //   var value2 = "1357910GI";

   //   var checkCharacter = GetCheckDigit(value2);
   //}

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPpsNumberValues))]
   public void IePpsNumber_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => IePpsNumber.Validate(value).Should().Be(IePpsNumberValidationResult.ValidationPassed);

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void IePpsNumber_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => IePpsNumber.Validate(value).Should().Be(IePpsNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void IePpsNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => IePpsNumber.Validate(value).Should().Be(IePpsNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void IePpsNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidCharacter(String value)
      => IePpsNumber.Validate(value).Should().Be(IePpsNumberValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void IePpsNumber_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String value)
      => IePpsNumber.Validate(value).Should().Be(IePpsNumberValidationResult.InvalidCheckDigit);

   #endregion
}
