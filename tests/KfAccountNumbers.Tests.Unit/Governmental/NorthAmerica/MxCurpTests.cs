// Ignore Spelling: Curp Mx Homoclaves Homoclave

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class MxCurpTests
{
   private const String ValidCurp = "HEGG560427MVZRRL04";      // From https://en.wikipedia.org/wiki/Unique_Population_Registry_Code

   /// <summary>
   ///   Generate a test CURP value. Full defaults will result in the example CURP described in https://en.wikipedia.org/wiki/Unique_Population_Registry_Code
   /// </summary>
   private static String GetCurp(
      String initials = "HEGG",
      String dateOfBirth = "560427",
      Char gender = 'M',
      String stateCode = "VZ",
      String consonants = "RRL",
      Char homoclave = '0',
      Char checkDigit = '4')
      => $"{initials}{dateOfBirth}{gender}{stateCode}{consonants}{homoclave}{checkDigit}";

   public static TheoryData<Char> ValidCheckDigits
   {
      get
      {
         var data = new TheoryData<Char>();
         data.AddRange(CharacterData.NumericCharacters);

         return data;
      }
   }

   public static TheoryData<Char> ValidGenders
   {
      get
      {
         var data = new TheoryData<Char>();
         data.AddRange(['H', 'M', 'X', 'h', 'm', 'x']);

         return data;
      }
   }

   public static TheoryData<Char> ValidHomoclaves
   {
      get
      {
         var data = new TheoryData<Char>();

         data.AddRange(CharacterData.AsciiUpperAndLowercaseAlphanumericCharacters);

         return data;
      }
   }

   public static TheoryData<String> ValidStateCodes
   {
      get
      {
         var data = new TheoryData<String>();
         var stateCodes = MxCurpStateCodes.GetAllStateCodes();
         data.AddRange(stateCodes);
         data.AddRange(stateCodes.Select(c => c.ToLower()).ToArray());
         return data;
      }
   }

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("HEGG")]
   [InlineData("hegg")]
   public void MxCurp_Validate_ShouldReturnValidationPassed_WhenNameInitialsAreValid(String initials)
   {
      // Arrange.
      var curp = GetCurp(initials);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.ValidationPassed);
   }

   [Theory]
   [InlineData("560427")]
   [InlineData("010101")]
   [InlineData("121231")]
   public void MxCurp_Validate_ShouldReturnValidationPassed_WhenDateOfBirthIsValid(String dateOfBirth)
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: dateOfBirth);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.ValidationPassed);
   }

   [Theory]
   [MemberData(nameof(ValidGenders))]
   public void MxCurp_Validate_ShouldReturnValidationPassed_WhenGenderIsValid(Char gender)
   {
      // Arrange.
      var curp = GetCurp(gender: gender);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.ValidationPassed);
   }

   [Theory]
   [MemberData(nameof(ValidStateCodes))]
   public void MxCurp_Validate_ShouldReturnValidationPassed_WhenStateCodeIsValid(String stateCode)
   {
      // Arrange.
      var curp = GetCurp(stateCode: stateCode);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.ValidationPassed);
   }

   [Theory]
   [InlineData("RRL")]
   [InlineData("rrl")]
   public void MxCurp_Validate_ShouldReturnValidationPassed_WhenNameConsonantsAreValid(String consonants)
   {
      // Arrange.
      var curp = GetCurp(consonants: consonants);
      var expected = MxCurpValidationResult.ValidationPassed;

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidHomoclaves))]
   public void MxCurp_Validate_ShouldReturnValidationPassed_WhenHomoclaveIsValid(Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(homoclave: homoclave);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.ValidationPassed);
   }

   [Theory]
   [MemberData(nameof(ValidCheckDigits))]
   public void MxCurp_Validate_ShouldReturnValidationPassed_WhenCheckDigitIsValid(Char checkDigit)
   {
      // Arrange.
      var curp = GetCurp(checkDigit: checkDigit);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.ValidationPassed);
   }

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void MxCurp_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String curp)
      => MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.Empty);

   [Theory]
   [InlineData("MAAR790213HMNRLF0")]      // Length 17
   [InlineData("HEGG560427MVZRRL045")]    // Length 19
   public void MxCurp_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String curp)
      => MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidLength);

   [Theory]
   [InlineData(" AAR")]          // Space in position 0
   [InlineData("\u00E1AAR")]     // á in position 0
   [InlineData("\u00C1AAR")]     // Á in position 0
   [InlineData("M1AR")]          // Numeric character in position 1
   [InlineData("MA!R")]          // ! in position 2
   [InlineData("MAA\u00F1")]     // ñ in position 3
   [InlineData("MAA\u00D1")]     // Ñ in position 3
   public void MxCurp_Validate_ShouldReturnInvalidAlphabeticCharacterEncountered_WhenNameInitialsAreInvalid(String initials)
   {
      // Arrange.
      var curp = GetCurp(initials);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [InlineData("790232")]        // Invalid day
   [InlineData("791413")]        // Invalid month
   [InlineData("A90213")]        // Non-digit character
   [InlineData("7B0213")]        // Non-digit character
   [InlineData("79!213")]        // Non-digit character
   [InlineData("790~13")]        // Non-digit character
   [InlineData("7902 3")]        // Non-digit character
   [InlineData("79021\u2153")]   // Unicode fraction 1/3
   public void MxCurp_Validate_ShouldReturnInvalidDateOfBirth_WhenDateOfBirthIsInvalid(String dateOfBirth)
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: dateOfBirth);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidDateOfBirth);
   }

   [Theory]
   [InlineData('A')]             // Invalid gender 'A'
   [InlineData('0')]             // Invalid gender '0'
   [InlineData('!')]             // Invalid gender '!'
   [InlineData('\u00F1')]        // Invalid gender Unicode ñ
   [InlineData('\u2153')]        // Invalid gender Unicode fraction 1/3
   public void MxCurp_Validate_ShouldReturnInvalidGender_WhenGenderIsInvalidCharacter(Char gender)
   {
      // Arrange.
      var curp = GetCurp(gender: gender);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidGender);
   }

   [Theory]
   [InlineData("DC")] // Invalid state code, US District of Columbia
   [InlineData("A1")]
   [InlineData("1C")]
   public void MxCurp_Validate_ShouldReturnInvalidGender_WhenStateCodeIsInvalid(String stateCode)
   {
      // Arrange.
      var curp = GetCurp(stateCode: stateCode);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidState);
   }

   [Theory]
   [InlineData(" LF")]           // Space in position 0
   [InlineData("\u00E1LF")]      // á in position 0
   [InlineData("\u00C1LF")]      // Á in position 0
   [InlineData("R1F")]           // Numeric character in position 1
   [InlineData("RL!")]           // ! in position 2
   [InlineData("RL\u00F1")]      // ñ in position 2
   [InlineData("RL\u00D1")]      // Ñ in position 2
   public void MxCurp_Validate_ShouldReturnInvalidAlphabeticCharacterEncountered_WhenNameConsonantsAreInvalid(String consonants)
   {
      // Arrange.
      var curp = GetCurp(consonants: consonants);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [InlineData('!')]             // Invalid homoclave '!'
   [InlineData('\u00F1')]        // Invalid homoclave Unicode ñ
   [InlineData('\u2153')]        // Invalid homoclave Unicode fraction 1
   public void MxCurp_Validate_ShouldReturnInvalidHomoclave_WhenHomoclaveIsInvalidCharacter(Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(homoclave: homoclave);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidHomoclave);
   }

   [Theory]
   [InlineData('!')]             // Invalid check digit '!'
   [InlineData('A')]             // Invalid check digit 'A'
   [InlineData('z')]             // Invalid check digit 'z'
   [InlineData('\u00F1')]        // Invalid check digit Unicode ñ
   [InlineData('\u2153')]        // Invalid check digit Unicode fraction 1
   public void MxCurp_Validate_ShouldReturnInvalidCheckDigit_WhenCheckDigitIsInvalidCharacter(Char checkDigit)
   {
      // Arrange.
      var curp = GetCurp(checkDigit: checkDigit);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidCheckDigit);
   }

   #endregion
}
