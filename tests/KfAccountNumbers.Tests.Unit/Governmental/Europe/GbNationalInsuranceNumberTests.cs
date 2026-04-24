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

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidNationalInsuranceNumberValues))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => GbNationalInsuranceNumber.Validate(value).Should().Be(GbNationalInsuranceNumberValidationResult.ValidationPassed);

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

   #endregion
}
