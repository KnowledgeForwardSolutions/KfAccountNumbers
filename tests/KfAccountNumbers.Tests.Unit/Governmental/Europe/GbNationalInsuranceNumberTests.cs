namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

public class GbNationalInsuranceNumberTests
{
   private const String Valid8CharacterValue = "AB123456";
   private const String Valid9CharacterValue = "AB123456C";
   private const String Valid11CharacterValue = "AB 12 34 56";
   private const String Valid13CharacterValue = "AB 12 34 56 C";
   private const String AltValid8CharacterValue = "GG000123";
   private const String AltValid9CharacterValue = "GG000123B";
   private const String AltValid11CharacterValue = "GG 00 01 23";
   private const String AltValid13CharacterValue = "GG 00 01 23 B";

   public static String GetNationalInsuranceNumber(
      String prefix = "AB",
      String digits = "123456",
      String suffix = "",
      Boolean formatted = false)
      => formatted
         ? $"{prefix} {digits[..2]} {digits[2..4]} {digits[4..]}{(suffix != String.Empty ? " " : "")}{suffix}"
         : $"{prefix}{digits}{suffix}";

   public static String GetNationalInsuranceNumber(
      Char prefix1 = 'A',
      Char prefix2 = 'C',
      String digits = "123456",
      String suffix = "",
      Boolean formatted = false)
      => GetNationalInsuranceNumber($"{prefix1}{prefix2}", digits, suffix, formatted);

   public static TheoryData<String> ValidNationalInsuranceNumberValues =>
   [
      Valid8CharacterValue,
      Valid9CharacterValue,
      Valid11CharacterValue,
      Valid13CharacterValue,
      AltValid8CharacterValue,
      AltValid9CharacterValue,
      AltValid11CharacterValue,
      AltValid13CharacterValue,
   ];

   public static TheoryData<Char, Boolean> ValidPrefixFirstCharacters = new()
   {
      { 'A', false },
      { 'B', false },
      { 'C', false },
      { 'E', false },
      { 'G', false },
      { 'H', false },
      { 'J', false },
      { 'K', false },
      { 'L', false },
      { 'M', false },
      { 'N', false },
      { 'O', false },
      { 'P', false },
      { 'R', false },
      { 'S', false },
      { 'T', false },
      { 'W', false },
      { 'X', false },
      { 'Y', false },
      { 'Z', false },
      { 'A', true },
      { 'B', true },
      { 'C', true },
      { 'E', true },
      { 'G', true },
      { 'H', true },
      { 'J', true },
      { 'K', true },
      { 'L', true },
      { 'M', true },
      { 'N', true },
      { 'O', true },
      { 'P', true },
      { 'R', true },
      { 'S', true },
      { 'T', true },
      { 'W', true },
      { 'X', true },
      { 'Y', true },
      { 'Z', true },
   };

   public static TheoryData<Char, Boolean> ValidPrefixSecondCharacters = new()
   {
      { 'A', false },
      { 'B', false },
      { 'C', false },
      { 'E', false },
      { 'G', false },
      { 'H', false },
      { 'J', false },
      { 'K', false },
      { 'L', false },
      { 'M', false },
      { 'N', false },
      { 'P', false },
      { 'R', false },
      { 'S', false },
      { 'T', false },
      { 'W', false },
      { 'X', false },
      { 'Y', false },
      { 'Z', false },
      { 'A', true },
      { 'B', true },
      { 'C', true },
      { 'E', true },
      { 'G', true },
      { 'H', true },
      { 'J', true },
      { 'K', true },
      { 'L', true },
      { 'M', true },
      { 'N', true },
      { 'P', true },
      { 'R', true },
      { 'S', true },
      { 'T', true },
      { 'W', true },
      { 'X', true },
      { 'Y', true },
      { 'Z', true },
   };

   public static TheoryData<String, Boolean> ValidSuffixCharacters = new()
   {
      { "", false },
      { "A", false },
      { "B", false },
      { "C", false },
      { "D", false },
      { "", true },
      { "A", true },
      { "B", true },
      { "C", true },
      { "D", true },
   };

   public static TheoryData<String> InvalidLengthValues =>
   [
      "AB12345",              // Length 7
      "AB123456CB",           // Length 10
      "AB 12 34 5",           // Length 10
      "AB 12 34 56C",         // Length 12
      "AB 12 34 56 CB",       // Length 14
      new String('1', 100)    // Very long string
   ];

   public static TheoryData<String, Boolean> InvalidPrefixValues = new()
   {
      { "BG", false },
      { "GB", false },
      { "NK", false },
      { "KN", false },
      { "TN", false },
      { "NT", false },
      { "ZZ", false },
      { "BG", true },
      { "GB", true },
      { "NK", true },
      { "KN", true },
      { "TN", true },
      { "NT", true },
      { "ZZ", true },
   };

   public static TheoryData<Char, Boolean> InvalidPrefixFirstCharacters = new()
   {
      { 'D', false },
      { 'F', false },
      { 'I', false },
      { 'Q', false },
      { 'U', false },
      { 'V', false },
      { 'D', true },
      { 'F', true },
      { 'I', true },
      { 'Q', true },
      { 'U', true },
      { 'V', true },
   };

   public static TheoryData<Char, Boolean> InvalidPrefixSecondCharacters = new()
   {
      { 'D', false },
      { 'F', false },
      { 'I', false },
      { 'O', false },
      { 'Q', false },
      { 'U', false },
      { 'V', false },
      { 'D', true },
      { 'F', true },
      { 'I', true },
      { 'O', true },
      { 'Q', true },
      { 'U', true },
      { 'V', true },
   };

   public static TheoryData<String, Boolean> InvalidDigits = new()
   {
      { " 12345", false },
      { "0-2345", false },
      { "01=345", false },
      { "012A45", false },
      { "0123b5", false },
      { "01234~", false },
      { "01234\u2153", false },       // Unicode fraction 1/3  
      { "01234\u00D6", false },       // unicode O with umlaut
      { " 12345", true },
      { "0-2345", true },
      { "01=345", true },
      { "012A45", true },
      { "0123b5", true },
      { "01234~", true },
      { "01234\u2153", true },        // Unicode fraction 1/3  
      { "01234\u00D6", true },        // unicode O with umlaut
   };

   public static TheoryData<String, Boolean> InvalidSuffixCharacters = new()
   {
      { "a", false },
      { "b", false },
      { "c", false },
      { "d", false },
      { " ", false },
      { "=", false },
      { "1", false },
      { "0", false },
      { "\u2153", false },       // Unicode fraction 1/3  
      { "\u00D6", false },       // unicode O with umlaut
      { "a", true },
      { "b", true },
      { "c", true },
      { "d", true },
      { " ", true },
      { "=", true },
      { "1", true },
      { "0", true },
      { "\u2153", true },       // Unicode fraction 1/3  
      { "\u00D6", true },       // unicode O with umlaut
   };

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidNationalInsuranceNumberValues))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => GbNationalInsuranceNumber.Validate(value).Should().Be(GbNationalInsuranceNumberValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(ValidPrefixFirstCharacters))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidFirstPrefixCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(ch, formatted: formatted);

      // Act/assert.
      GbNationalInsuranceNumber.Validate(value).Should().Be(GbNationalInsuranceNumberValidationResult.ValidationPassed);
   }

   [Theory]
   [MemberData(nameof(ValidPrefixSecondCharacters))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidSecondPrefixCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix2: ch, formatted: formatted);

      // Act/assert.
      GbNationalInsuranceNumber.Validate(value).Should().Be(GbNationalInsuranceNumberValidationResult.ValidationPassed);
   }

   [Theory]
   [MemberData(nameof(ValidSuffixCharacters))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidSuffixCharacter(
      String suffix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", suffix: suffix, formatted: formatted);

      // Act/assert.
      GbNationalInsuranceNumber.Validate(value).Should().Be(GbNationalInsuranceNumberValidationResult.ValidationPassed);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => GbNationalInsuranceNumber.Validate(value).Should().Be(GbNationalInsuranceNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => GbNationalInsuranceNumber.Validate(value).Should().Be(GbNationalInsuranceNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidPrefixValues))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnInvalidPrefix_WhenValueHasInvalidPrefix(
      String prefix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix, formatted: formatted);

      // Act/assert.
      GbNationalInsuranceNumber.Validate(value).Should().Be(GbNationalInsuranceNumberValidationResult.InvalidPrefix);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixFirstCharacters))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidPrefixFirstCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(ch, formatted: formatted);

      // Act/assert.
      GbNationalInsuranceNumber.Validate(value).Should().Be(GbNationalInsuranceNumberValidationResult.InvalidCharacter);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixSecondCharacters))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidPrefixSecondCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix2: ch, formatted: formatted);

      // Act/assert.
      GbNationalInsuranceNumber.Validate(value).Should().Be(GbNationalInsuranceNumberValidationResult.InvalidCharacter);
   }

   [Theory]
   [MemberData(nameof(InvalidDigits))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidDigitCharacters(
      String digits,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", digits: digits, formatted: formatted);

      // Act/assert.
      GbNationalInsuranceNumber.Validate(value).Should().Be(GbNationalInsuranceNumberValidationResult.InvalidCharacter);
   }

   [Theory]
   [MemberData(nameof(InvalidSuffixCharacters))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidSuffixCharacter(
      String suffix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", suffix: suffix, formatted: formatted);

      // Act/assert.
      GbNationalInsuranceNumber.Validate(value).Should().Be(GbNationalInsuranceNumberValidationResult.InvalidCharacter);
   }

   #endregion
}
