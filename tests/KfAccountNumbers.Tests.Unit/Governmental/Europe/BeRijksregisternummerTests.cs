// Ignore Spelling: Rijksregisternummer Nummer

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class BeRijksregisternummerTests
{
   private const String ValidRijksregisternummer = "85.07.30-033.28";
   private const String AltValidRijksregisternummer = "17110804680";
   private const String IncompleteDateOfBirthRijksregisternummer = "40 00 00 955-79";
   private const String UnknownDateOfBirthRijksregisternummer = "00 00 01 003-64";
   private const String ValidBisnummer = "17.51.08-046.40";
   private const String AltValidBisnummer = "01430801695";

   private static String GetRijksregisternummerWithValidCheckDigits(
      Int32 year = 1987,
      Int32 month = 7,
      Int32 day = 30,
      Int32 sequenceNumber = 33,
      Boolean formatted = false)
   {
      var temp = $"{(year >= 2000 ? 2 : 0)}{(year % 100):D2}{month:D2}{day:D2}{sequenceNumber:D3}";
      var checkSum = GetCheckSum(temp);
      
      return formatted
         ? $"{(year % 100):D2}{month:D2}{day:D2}{sequenceNumber:D3}{checkSum:D2}"
         : $"{(year % 100):D2}.{month:D2}.{day:D2}-{sequenceNumber:D3}.{checkSum:D2}";
   }

   private static Int32 GetCheckSum(String str)
   {
      var sum = 0L;
      foreach (var ch in str)
      {
         sum *= 10;
         var num = ch - Chars.DigitZero;
         sum += num;
      }

      return (Int32)(97 - (sum % 97));
   }

   public static TheoryData<String> ValidRijksregisternummerValues =>
   [
      ValidRijksregisternummer,
      AltValidRijksregisternummer,
      IncompleteDateOfBirthRijksregisternummer,
      UnknownDateOfBirthRijksregisternummer,
      ValidBisnummer,
      AltValidBisnummer
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "1711080468",        // Length 10
      "171108046801",      // Length 11
      "85.07.30-033.2",      // Length 14
      "85.07.30-033.289",      // Length 16
      new String('1', 100) // Very long string
   ];

   public static TheoryData<Int32, Int32, Int32, Boolean> ValidDateOfBirthValues = new()
   {
      { 1900,  1,  1, false },   // January 1, 1900
      { 1999, 12, 31, false },   // December 31, 1999
      { 2000,  1,  1, false },   // January 1, 2000
      { 2099, 12, 31, false },   // December 31, 2099

      { 1900,  1,  1, true },    // January 1, 1900
      { 1999, 12, 31, true },    // December 31, 1999
      { 2000,  1,  1, true },    // January 1, 2000
      { 2099, 12, 31, true },    // December 31, 2099

   };

   public static TheoryData<String> InvalidCharacterValues =>
   [
      "A7110804680",             // Non-digit character 'A'
      "1 110804680",             // Non-digit character ' '
      "17_10804680",             // Non-digit character '-'
      "171=0804680",             // Non-digit character '='
      "1711B804680",             // Non-digit character 'B'
      "17110C04680",             // Non-digit character 'C'
      "171108a4680",             // Non-digit character 'a'
      "1711080b680",             // Non-digit character 'b'
      "17110804~80",             // Non-digit character '~'
      "171108046\u21530",        // Non-digit character Unicode fraction 1/3
      "1711080468\u00D6",        // Invalid character unicode O with umlaut

      "A7.11.08-046.80",         // Non-digit character 'A'
      "1 .11.08-046.80",         // Non-digit character ' '
      "17._1.08-046.80",         // Non-digit character '-'
      "17.1=.08-046.80",         // Non-digit character '='
      "17.11.B8-046.80",         // Non-digit character 'B'
      "17.11.0C-046.80",         // Non-digit character 'C'
      "17 11 08 a46 80",         // Non-digit character 'a'
      "17 11 08 0b6 80",         // Non-digit character 'b'
      "17 11 08 04~ 80",         // Non-digit character '~'
      "17 11 08 046 \u21530",    // Non-digit character Unicode fraction 1/3
      "17 11 08 046 8\u00D6",    // Invalid character unicode O with umlaut
   ];

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "85072003328",             // 85073003328 with single digit transcription error, 3 -> 2
      "17110805680",             // 17110804680 with single digit transcription error, 4 -> 5
      "85072003329",             // 85072003328 with check digit transcription error, 8 -> 9
      "17118005680",             // 17110805680 with two digit transposition error, 08 -> 80
      "85037003328",             // 85073003328 with two digit transposition error, 73 -> 37
      "17110408680",             // 17110804680 with two digit jump transposition, 804 -> 408
      "85073004428",             // 85073003328 with two digit twin error, 33 -> 44
      "17220804680",             // 17110804680 with two digit twin error, 11 -> 22

      "85.07.20-033.28",         // 85073003328 with single digit transcription error, 3 -> 2
      "17.11.08-056.80",         // 17110804680 with single digit transcription error, 4 -> 5
      "85.07.20-033.29",         // 85072003328 with check digit transcription error, 8 -> 9
      "17.11.80-056.80",         // 17110805680 with two digit transposition error, 08 -> 80
      "85 03 70 033 28",         // 85073003328 with two digit transposition error, 73 -> 37
      "17 11 04 086 80",         // 17110804680 with two digit jump transposition, 804 -> 408
      "85 07 30 044 28",         // 85073003328 with two digit twin error, 33 -> 44
      "17 22 08 046 80",         // 17110804680 with two digit twin error, 11 -> 22
   ];

   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "85007.30-033.28",
      "85107.30-033.28",
      "85207.30-033.28",
      "85307.30-033.28",
      "85407.30-033.28",
      "85507.30-033.28",
      "85607.30-033.28",
      "85707.30-033.28",
      "85807.30-033.28",
      "85907.30-033.28",

      "85.07030-033.28",
      "85.07130-033.28",
      "85.07230-033.28",
      "85.07330-033.28",
      "85.07430-033.28",
      "85.07530-033.28",
      "85.07630-033.28",
      "85.07730-033.28",
      "85.07830-033.28",
      "85.07930-033.28",

      "85.07.300033.28",
      "85.07.301033.28",
      "85.07.302033.28",
      "85.07.303033.28",
      "85.07.304033.28",
      "85.07.305033.28",
      "85.07.306033.28",
      "85.07.307033.28",
      "85.07.308033.28",
      "85.07.309033.28",

      "85.07.30-033028",
      "85.07.30-033128",
      "85.07.30-033228",
      "85.07.30-033328",
      "85.07.30-033428",
      "85.07.30-033528",
      "85.07.30-033628",
      "85.07.30-033728",
      "85.07.30-033828",
      "85.07.30-033928",

   ];

   #region Constants Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeRijksregisternummer_BisNummerMonthOffset_ShouldHaveExpectedValue()
      => BeRijksregisternummer.BisNummerMonthOffset.Should().Be(40);

   [Fact]
   public void BeRijksregisternummer_BisNummerUnknownGenderMonthOffset_ShouldHaveExpectedValue()
      => BeRijksregisternummer.BisNummerUnknownGenderMonthOffset.Should().Be(20);

   [Fact]
   public void BeRijksregisternummer_MinimumValidYearOfBirth_ShouldHaveExpectedValue()
      => BeRijksregisternummer.MinimumValidYearOfBirth.Should().Be(1900);

   [Fact]
   public void BeRijksregisternummer_MaximumValidYearOfBirth_ShouldHaveExpectedValue()
      => BeRijksregisternummer.MaximumValidYearOfBirth.Should().Be(2099);

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnValidationPassed_WhenValueHasValidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(year, month, day, formatted: formatted);

      // Act/assert.
      BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.ValidationPassed);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacterWhereDigitExpected(String value)
      => BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnInvalidCheckDigits_WhenValueHasInvalidCheckDigits(String value)
      => BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.InvalidCheckDigits);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidInvalidSeparator(String value)
      => BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.InvalidSeparator);

   #endregion
}
