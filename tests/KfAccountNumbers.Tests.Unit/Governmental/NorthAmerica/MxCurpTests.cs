// Ignore Spelling: Curp Mx Homoclaves Homoclave

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

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
         data.AddRange([.. stateCodes.Select(c => c.ToLowerInvariant())]);
         return data;
      }
   }

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("HEGG")]
   [InlineData("hegg")]
   public void MxCurp_Constructor_ShouldCreateInstance_WhenNameInitialsAreValid(String initials)
   {
      // Arrange.
      var curp = GetCurp(initials);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = new MxCurp(curp);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [InlineData("560427", '0')]
   [InlineData("000101", '0')]         // Jan 1, 1900 (digit homoclave)
   [InlineData("991231", '0')]         // Dec 31, 1999
   [InlineData("000101", 'A')]         // Jan 1, 2000 (letter homoclave)
   [InlineData("991231", 'A')]         // Dec 31, 2099
   [InlineData("040229", 'A')]         // Feb 29, 2004 (leap year)

   [InlineData("010131", '0')]         // Max day of month January
   [InlineData("010228", '0')]         // Max day of month February (non leap year, 1900's)
   [InlineData("010228", 'A')]         // Max day of month February (non leap year, 2000's)
   [InlineData("040229", '0')]         // Max day of month February (leap year, 1900's)
   [InlineData("040229", 'B')]         // Max day of month February (leap year, 2000's)
   [InlineData("000229", 'b')]         // Max day of month February (leap year because homoclave indicates 2000, a century divisible by 400)
   [InlineData("010331", '0')]         // Max day of month March
   [InlineData("010430", 'A')]         // Max day of month April
   [InlineData("010531", 'b')]         // Max day of month May
   [InlineData("010630", '0')]         // Max day of month June
   [InlineData("010731", 'A')]         // Max day of month July
   [InlineData("010831", 'b')]         // Max day of month August
   [InlineData("010930", '0')]         // Max day of month September
   [InlineData("011031", 'A')]         // Max day of month October
   [InlineData("011130", 'b')]         // Max day of month November
   [InlineData("011231", '0')]         // Max day of month December
   public void MxCurp_Constructor_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: dateOfBirth, homoclave: homoclave);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = new MxCurp(curp);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidGenders))]
   public void MxCurp_Constructor_ShouldCreateInstance_WhenGenderIsValid(Char gender)
   {
      // Arrange.
      var curp = GetCurp(gender: gender);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = new MxCurp(curp);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidStateCodes))]
   public void MxCurp_Constructor_ShouldCreateInstance_WhenStateCodeIsValid(String stateCode)
   {
      // Arrange.
      var curp = GetCurp(stateCode: stateCode);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = new MxCurp(curp);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [InlineData("RRL")]
   [InlineData("rrl")]
   public void MxCurp_Constructor_ShouldCreateInstance_WhenNameConsonantsAreValid(String consonants)
   {
      // Arrange.
      var curp = GetCurp(consonants: consonants);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = new MxCurp(curp);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidHomoclaves))]
   public void MxCurp_Constructor_ShouldCreateInstance_WhenHomoclaveIsValid(Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(homoclave: homoclave);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = new MxCurp(curp);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidCheckDigits))]
   public void MxCurp_Constructor_ShouldCreateInstance_WhenCheckDigitIsValid(Char checkDigit)
   {
      // Arrange.
      var curp = GetCurp(checkDigit: checkDigit);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = new MxCurp(curp);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void MxCurp_Constructor_ShouldThrowInvalidMxCurpException_WhenValueIsNullOrEmpty(String? curp)
      => FluentActions
         .Invoking(() => new MxCurp(curp!))
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpEmpty + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.Empty);

   [Theory]
   [InlineData("MAAR790213HMNRLF0")]      // Length 17
   [InlineData("HEGG560427MVZRRL045")]    // Length 19
   public void MxCurp_Constructor_ShouldThrowInvalidMxCurpException_WhenValueHasInvalidLength(String curp)
      => FluentActions
         .Invoking(() => new MxCurp(curp))
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidLength + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidLength);

   [Theory]
   [InlineData(" AAR")]          // Space in position 0
   [InlineData("\u00E1AAR")]     // á in position 0
   [InlineData("\u00C1AAR")]     // Á in position 0
   [InlineData("M1AR")]          // Numeric character in position 1
   [InlineData("MA!R")]          // ! in position 2
   [InlineData("MAA\u00F1")]     // ñ in position 3
   [InlineData("MAA\u00D1")]     // Ñ in position 3
   public void MxCurp_Constructor_ShouldThrowInvalidMxCurpException_WhenNameInitialsAreInvalid(String initials)
   {
      // Arrange.
      var curp = GetCurp(initials);

      // Act/assert.
      FluentActions
         .Invoking(() => new MxCurp(curp))
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidAlphabeticCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [InlineData("A10101", '0')]         // Non-digit character 'A'
   [InlineData("0B0101", '0')]         // Non-digit character 'B'
   [InlineData("01!101", '0')]         // Non-digit character '!'
   [InlineData("010\u00F101", '0')]    // Non-digit character 'ñ'
   [InlineData("0101 1", '0')]         // Non-digit character ' '
   [InlineData("01010\u2153", '0')]    // Non-digit character Unicode fraction 1/3

   [InlineData("010001", '0')]         // Invalid month (too low)
   [InlineData("011301", 'A')]         // Invalid month (too high)
   [InlineData("010100", 'b')]         // Invalid day of month (too low)
   [InlineData("010132", '0')]         // Invalid day of month January
   [InlineData("010229", '0')]         // Invalid day of month February (non leap year, 1900's)
   [InlineData("010229", 'A')]         // Invalid day of month February (non leap year, 2000's)
   [InlineData("040230", '0')]         // Invalid day of month February (leap year, 1900's)
   [InlineData("040230", 'b')]         // Invalid day of month February (leap year 2000's)
   [InlineData("000229", '0')]         // Invalid day of month February (homoclave indicated 1900, not a leap year)
   [InlineData("000230", 'A')]         // Invalid day of month February (leap year because homoclave indicates 2000, a century divisible by 400)
   [InlineData("010332", '0')]         // Invalid day of month March
   [InlineData("010431", 'A')]         // Invalid day of month April
   [InlineData("010532", 'b')]         // Invalid day of month May
   [InlineData("010631", '0')]         // Invalid day of month June
   [InlineData("010732", 'A')]         // Invalid day of month July
   [InlineData("010832", 'b')]         // Invalid day of month August
   [InlineData("010931", '0')]         // Invalid day of month September
   [InlineData("011032", 'A')]         // Invalid day of month October
   [InlineData("011131", 'b')]         // Invalid day of month November
   [InlineData("011232", '0')]         // Invalid day of month December
   public void MxCurp_Constructor_ShouldThrowInvalidMxCurpException_WhenDateOfBirthIsInvalid(
      String dateOfBirth,
      Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: dateOfBirth, homoclave: homoclave);

      // Act/assert.
      FluentActions
         .Invoking(() => new MxCurp(curp))
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidDateOfBirth + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidDateOfBirth);
   }

   [Theory]
   [InlineData('A')]             // Invalid gender 'A'
   [InlineData('0')]             // Invalid gender '0'
   [InlineData('!')]             // Invalid gender '!'
   [InlineData('\u00F1')]        // Invalid gender Unicode ñ
   [InlineData('\u2153')]        // Invalid gender Unicode fraction 1/3
   public void MxCurp_Constructor_ShouldThrowInvalidMxCurpException_WhenGenderIsInvalidCharacter(Char gender)
   {
      // Arrange.
      var curp = GetCurp(gender: gender);

      // Act/assert.
      FluentActions
         .Invoking(() => new MxCurp(curp))
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidGender + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidGender);
   }

   [Theory]
   [InlineData("DC")] // Invalid state code, US District of Columbia
   [InlineData("A1")]
   [InlineData("1C")]
   public void MxCurp_Constructor_ShouldThrowInvalidMxCurpException_WhenStateCodeIsInvalid(String stateCode)
   {
      // Arrange.
      var curp = GetCurp(stateCode: stateCode);

      // Act/assert.
      FluentActions
         .Invoking(() => new MxCurp(curp))
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidState + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidState);
   }

   [Theory]
   [InlineData(" LF")]           // Space in position 0
   [InlineData("\u00E1LF")]      // á in position 0
   [InlineData("\u00C1LF")]      // Á in position 0
   [InlineData("R1F")]           // Numeric character in position 1
   [InlineData("RL!")]           // ! in position 2
   [InlineData("RL\u00F1")]      // ñ in position 2
   [InlineData("RL\u00D1")]      // Ñ in position 2
   public void MxCurp_Constructor_ShouldThrowInvalidMxCurpException_WhenNameConsonantsAreInvalid(String consonants)
   {
      // Arrange.
      var curp = GetCurp(consonants: consonants);

      // Act/assert.
      FluentActions
         .Invoking(() => new MxCurp(curp))
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidAlphabeticCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [InlineData('!')]             // Invalid homoclave '!'
   [InlineData('\u00F1')]        // Invalid homoclave Unicode ñ
   [InlineData('\u2153')]        // Invalid homoclave Unicode fraction 1
   public void MxCurp_Constructor_ShouldThrowInvalidMxCurpException_WhenHomoclaveIsInvalidCharacter(Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(homoclave: homoclave);

      // Act/assert.
      FluentActions
         .Invoking(() => new MxCurp(curp))
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidHomoclave + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidHomoclave);
   }

   [Fact]
   public void MxCurp_Constructor_ShouldThrowInvalidMxCurpException_WhenBothHomoclaveAndDateOfBirthAreInvalid()
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: "010132", homoclave: '!'); // Invalid day of month and invalid homoclave

      // Act/assert.
      FluentActions
         .Invoking(() => new MxCurp(curp))
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidHomoclave + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidHomoclave);
   }

   [Theory]
   [InlineData('!')]             // Invalid check digit '!'
   [InlineData('A')]             // Invalid check digit 'A'
   [InlineData('z')]             // Invalid check digit 'z'
   [InlineData('\u00F1')]        // Invalid check digit Unicode ñ
   [InlineData('\u2153')]        // Invalid check digit Unicode fraction 1
   public void MxCurp_Constructor_ShouldThrowInvalidMxCurpException_WhenCheckDigitIsInvalidCharacter(Char checkDigit)
   {
      // Arrange.
      var curp = GetCurp(checkDigit: checkDigit);

      // Act/assert.
      FluentActions
         .Invoking(() => new MxCurp(curp))
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidCheckDigit);
   }

   #endregion

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("000101", '0')]         // Jan 1, 1900 (digit homoclave)
   [InlineData("991231", '0')]         // Dec 31, 1999
   [InlineData("000101", 'A')]         // Jan 1, 2000 (letter homoclave)
   [InlineData("991231", 'A')]         // Dec 31, 2099
   [InlineData("040229", 'A')]         // Feb 29, 2004 (leap year)

   [InlineData("010228", 'A')]         // Max day of month February (non leap year)
   [InlineData("040229", '0')]         // Max day of month February (leap year)
   [InlineData("000229", 'b')]         // Max day of month February (leap year because century divisible by 400)
   public void MxCurp_DateOfBirth_ShouldReturnExpectedValue(
      String dateOfBirth,
      Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: dateOfBirth, homoclave: homoclave);
      var expected = DateOnly.ParseExact(
         (Char.IsAsciiDigit(homoclave) ? "19" : "20") + dateOfBirth,
         "yyyyMMdd",
         System.Globalization.CultureInfo.InvariantCulture);
      var sut = new MxCurp(curp);

      // Act/assert.
      sut.DateOfBirth.Should().Be(expected);
   }

   #endregion

   #region GenderCode Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidGenders))]
   public void MxCurp_Gender_ShouldReturnExpectedValue(Char gender)
   {
      // Arrange.
      var curp = GetCurp(gender: gender);
      var expected = Char.ToUpperInvariant(gender);
      var sut = new MxCurp(curp);

      // Act/assert.
      sut.GenderCode.Should().Be(expected);
   }

   #endregion

   #region StateCode Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidStateCodes))]
   public void MxCurp_StateCode_ShouldReturnExpectedValue(String stateCode)
   {
      // Arrange.
      var curp = GetCurp(stateCode: stateCode);
      var expected = stateCode.ToUpperInvariant();
      var sut = new MxCurp(curp);

      // Act/assert.
      sut.StateCode.Should().Be(expected);
   }

   #endregion

   #region Implicit Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void MxCurp_ImplicitMxCurpToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var sut = new MxCurp(ValidCurp);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(ValidCurp);
   }

   [Fact]
   public void MxCurp_CastMxCurpToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var sut = new MxCurp(ValidCurp);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(ValidCurp);
   }

   [Fact]
   public void MxCurp_ImplicitCastMxCurpToString_ShouldThrowArgumentNullException_WhenValueIsNull()
   {
      // Arrange.
      MxCurp curp = null!;
      String str;

      // Act/assert.
      FluentActions
         .Invoking(() => str = curp)
         .Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(curp))
         .WithMessage(Messages.MxCurpInvalidNullConversionToString + "*");
   }

   [Fact]
   public void MxCurp_CastMxCurpToString_ShouldThrowArgumentNullException_WhenValueIsNull()
   {
      // Arrange.
      MxCurp curp = null!;

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (String)curp)
         .Should().ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(curp))
         .WithMessage(Messages.MxCurpInvalidNullConversionToString + "*");
   }

   [Theory]
   [InlineData("HEGG")]
   [InlineData("hegg")]
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldCreateInstance_WhenNameInitialsAreValid(String initials)
   {
      // Arrange.
      var curp = GetCurp(initials);
      var expected = curp.ToUpperInvariant();

      // Act.
      MxCurp sut = curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [InlineData("560427", '0')]
   [InlineData("000101", '0')]         // Jan 1, 1900 (digit homoclave)
   [InlineData("991231", '0')]         // Dec 31, 1999
   [InlineData("000101", 'A')]         // Jan 1, 2000 (letter homoclave)
   [InlineData("991231", 'A')]         // Dec 31, 2099
   [InlineData("040229", 'A')]         // Feb 29, 2004 (leap year)

   [InlineData("010131", '0')]         // Max day of month January
   [InlineData("010228", '0')]         // Max day of month February (non leap year, 1900's)
   [InlineData("010228", 'A')]         // Max day of month February (non leap year, 2000's)
   [InlineData("040229", '0')]         // Max day of month February (leap year, 1900's)
   [InlineData("040229", 'B')]         // Max day of month February (leap year, 2000's)
   [InlineData("000229", 'b')]         // Max day of month February (leap year because homoclave indicates 2000, a century divisible by 400)
   [InlineData("010331", '0')]         // Max day of month March
   [InlineData("010430", 'A')]         // Max day of month April
   [InlineData("010531", 'b')]         // Max day of month May
   [InlineData("010630", '0')]         // Max day of month June
   [InlineData("010731", 'A')]         // Max day of month July
   [InlineData("010831", 'b')]         // Max day of month August
   [InlineData("010930", '0')]         // Max day of month September
   [InlineData("011031", 'A')]         // Max day of month October
   [InlineData("011130", 'b')]         // Max day of month November
   [InlineData("011231", '0')]         // Max day of month December
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: dateOfBirth, homoclave: homoclave);
      var expected = curp.ToUpperInvariant();

      // Act.
      MxCurp sut = curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidGenders))]
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldCreateInstance_WhenGenderIsValid(Char gender)
   {
      // Arrange.
      var curp = GetCurp(gender: gender);
      var expected = curp.ToUpperInvariant();

      // Act.
      MxCurp sut = curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidStateCodes))]
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldCreateInstance_WhenStateCodeIsValid(String stateCode)
   {
      // Arrange.
      var curp = GetCurp(stateCode: stateCode);
      var expected = curp.ToUpperInvariant();

      // Act.
      MxCurp sut = curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [InlineData("RRL")]
   [InlineData("rrl")]
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldCreateInstance_WhenNameConsonantsAreValid(String consonants)
   {
      // Arrange.
      var curp = GetCurp(consonants: consonants);
      var expected = curp.ToUpperInvariant();

      // Act.
      MxCurp sut = curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidHomoclaves))]
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldCreateInstance_WhenHomoclaveIsValid(Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(homoclave: homoclave);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = new MxCurp(curp);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidCheckDigits))]
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldCreateInstance_WhenCheckDigitIsValid(Char checkDigit)
   {
      // Arrange.
      var curp = GetCurp(checkDigit: checkDigit);
      var expected = curp.ToUpperInvariant();

      // Act.
      MxCurp sut = curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldThrowInvalidMxCurpException_WhenValueIsNullOrEmpty(String? curp)
   {
      // Arrange.
      MxCurp sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = curp!)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpEmpty + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.Empty);
   }

   [Theory]
   [InlineData("MAAR790213HMNRLF0")]      // Length 17
   [InlineData("HEGG560427MVZRRL045")]    // Length 19
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldThrowInvalidMxCurpException_WhenValueHasInvalidLength(String curp)
   {
      // Arrange.
      MxCurp sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = curp)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidLength + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidLength);
   }

   [Theory]
   [InlineData(" AAR")]          // Space in position 0
   [InlineData("\u00E1AAR")]     // á in position 0
   [InlineData("\u00C1AAR")]     // Á in position 0
   [InlineData("M1AR")]          // Numeric character in position 1
   [InlineData("MA!R")]          // ! in position 2
   [InlineData("MAA\u00F1")]     // ñ in position 3
   [InlineData("MAA\u00D1")]     // Ñ in position 3
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldThrowInvalidMxCurpException_WhenNameInitialsAreInvalid(String initials)
   {
      // Arrange.
      var curp = GetCurp(initials);
      MxCurp sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = curp)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidAlphabeticCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [InlineData("A10101", '0')]         // Non-digit character 'A'
   [InlineData("0B0101", '0')]         // Non-digit character 'B'
   [InlineData("01!101", '0')]         // Non-digit character '!'
   [InlineData("010\u00F101", '0')]    // Non-digit character 'ñ'
   [InlineData("0101 1", '0')]         // Non-digit character ' '
   [InlineData("01010\u2153", '0')]    // Non-digit character Unicode fraction 1/3

   [InlineData("010001", '0')]         // Invalid month (too low)
   [InlineData("011301", 'A')]         // Invalid month (too high)
   [InlineData("010100", 'b')]         // Invalid day of month (too low)
   [InlineData("010132", '0')]         // Invalid day of month January
   [InlineData("010229", '0')]         // Invalid day of month February (non leap year, 1900's)
   [InlineData("010229", 'A')]         // Invalid day of month February (non leap year, 2000's)
   [InlineData("040230", '0')]         // Invalid day of month February (leap year, 1900's)
   [InlineData("040230", 'b')]         // Invalid day of month February (leap year 2000's)
   [InlineData("000229", '0')]         // Invalid day of month February (homoclave indicated 1900, not a leap year)
   [InlineData("000230", 'A')]         // Invalid day of month February (leap year because homoclave indicates 2000, a century divisible by 400)
   [InlineData("010332", '0')]         // Invalid day of month March
   [InlineData("010431", 'A')]         // Invalid day of month April
   [InlineData("010532", 'b')]         // Invalid day of month May
   [InlineData("010631", '0')]         // Invalid day of month June
   [InlineData("010732", 'A')]         // Invalid day of month July
   [InlineData("010832", 'b')]         // Invalid day of month August
   [InlineData("010931", '0')]         // Invalid day of month September
   [InlineData("011032", 'A')]         // Invalid day of month October
   [InlineData("011131", 'b')]         // Invalid day of month November
   [InlineData("011232", '0')]         // Invalid day of month December
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldThrowInvalidMxCurpException_WhenDateOfBirthIsInvalid(
      String dateOfBirth,
      Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: dateOfBirth, homoclave: homoclave);

      // Act/assert.
      FluentActions
         .Invoking(() => new MxCurp(curp))
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidDateOfBirth + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidDateOfBirth);
   }

   [Theory]
   [InlineData('A')]             // Invalid gender 'A'
   [InlineData('0')]             // Invalid gender '0'
   [InlineData('!')]             // Invalid gender '!'
   [InlineData('\u00F1')]        // Invalid gender Unicode ñ
   [InlineData('\u2153')]        // Invalid gender Unicode fraction 1/3
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldThrowInvalidMxCurpException_WhenGenderIsInvalidCharacter(Char gender)
   {
      // Arrange.
      var curp = GetCurp(gender: gender);
      MxCurp sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = curp)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidGender + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidGender);
   }

   [Theory]
   [InlineData("DC")] // Invalid state code, US District of Columbia
   [InlineData("A1")]
   [InlineData("1C")]
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldThrowInvalidMxCurpException_WhenStateCodeIsInvalid(String stateCode)
   {
      // Arrange.
      var curp = GetCurp(stateCode: stateCode);
      MxCurp sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = curp)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidState + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidState);
   }

   [Theory]
   [InlineData(" LF")]           // Space in position 0
   [InlineData("\u00E1LF")]      // á in position 0
   [InlineData("\u00C1LF")]      // Á in position 0
   [InlineData("R1F")]           // Numeric character in position 1
   [InlineData("RL!")]           // ! in position 2
   [InlineData("RL\u00F1")]      // ñ in position 2
   [InlineData("RL\u00D1")]      // Ñ in position 2
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldThrowInvalidMxCurpException_WhenNameConsonantsAreInvalid(String consonants)
   {
      // Arrange.
      var curp = GetCurp(consonants: consonants);
      MxCurp sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = curp)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidAlphabeticCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [InlineData('!')]             // Invalid homoclave '!'
   [InlineData('\u00F1')]        // Invalid homoclave Unicode ñ
   [InlineData('\u2153')]        // Invalid homoclave Unicode fraction 1
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldThrowInvalidMxCurpException_WhenHomoclaveIsInvalidCharacter(Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(homoclave: homoclave);
      MxCurp sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = curp)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidHomoclave + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidHomoclave);
   }

   [Fact]
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldThrowInvalidMxCurpException_WhenBothHomoclaveAndDateOfBirthAreInvalid()
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: "010132", homoclave: '!'); // Invalid day of month and invalid homoclave
      MxCurp sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = curp)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidHomoclave + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidHomoclave);
   }

   [Theory]
   [InlineData('!')]             // Invalid check digit '!'
   [InlineData('A')]             // Invalid check digit 'A'
   [InlineData('z')]             // Invalid check digit 'z'
   [InlineData('\u00F1')]        // Invalid check digit Unicode ñ
   [InlineData('\u2153')]        // Invalid check digit Unicode fraction 1
   public void MxCurp_ImplicitStringToMxCurpConversion_ShouldThrowInvalidMxCurpException_WhenCheckDigitIsInvalidCharacter(Char checkDigit)
   {
      // Arrange.
      var curp = GetCurp(checkDigit: checkDigit);
      MxCurp sut;

      // Act/assert.
      FluentActions
         .Invoking(() => sut = curp)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidCheckDigit);
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("HEGG")]
   [InlineData("hegg")]
   public void MxCurp_Create_ShouldCreateInstance_WhenNameInitialsAreValid(String initials)
   {
      // Arrange.
      var curp = GetCurp(initials);
      var expectedValue = new MxCurp(curp);

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [InlineData("560427", '0')]
   [InlineData("000101", '0')]         // Jan 1, 1900 (digit homoclave)
   [InlineData("991231", '0')]         // Dec 31, 1999
   [InlineData("000101", 'A')]         // Jan 1, 2000 (letter homoclave)
   [InlineData("991231", 'A')]         // Dec 31, 2099
   [InlineData("040229", 'A')]         // Feb 29, 2004 (leap year)

   [InlineData("010131", '0')]         // Max day of month January
   [InlineData("010228", '0')]         // Max day of month February (non leap year, 1900's)
   [InlineData("010228", 'A')]         // Max day of month February (non leap year, 2000's)
   [InlineData("040229", '0')]         // Max day of month February (leap year, 1900's)
   [InlineData("040229", 'B')]         // Max day of month February (leap year, 2000's)
   [InlineData("000229", 'b')]         // Max day of month February (leap year because homoclave indicates 2000, a century divisible by 400)
   [InlineData("010331", '0')]         // Max day of month March
   [InlineData("010430", 'A')]         // Max day of month April
   [InlineData("010531", 'b')]         // Max day of month May
   [InlineData("010630", '0')]         // Max day of month June
   [InlineData("010731", 'A')]         // Max day of month July
   [InlineData("010831", 'b')]         // Max day of month August
   [InlineData("010930", '0')]         // Max day of month September
   [InlineData("011031", 'A')]         // Max day of month October
   [InlineData("011130", 'b')]         // Max day of month November
   [InlineData("011231", '0')]         // Max day of month December
   public void MxCurp_Create_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: dateOfBirth, homoclave: homoclave);
      var expectedValue = new MxCurp(curp);

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(ValidGenders))]
   public void MxCurp_Create_ShouldCreateInstance_WhenGenderIsValid(Char gender)
   {
      // Arrange.
      var curp = GetCurp(gender: gender);
      var expectedValue = new MxCurp(curp);

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(ValidStateCodes))]
   public void MxCurp_Create_ShouldCreateInstance_WhenStateCodeIsValid(String stateCode)
   {
      // Arrange.
      var curp = GetCurp(stateCode: stateCode);
      var expectedValue = new MxCurp(curp);

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [InlineData("RRL")]
   [InlineData("rrl")]
   public void MxCurp_Create_ShouldCreateInstance_WhenNameConsonantsAreValid(String consonants)
   {
      // Arrange.
      var curp = GetCurp(consonants: consonants);
      var expectedValue = new MxCurp(curp);

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(ValidHomoclaves))]
   public void MxCurp_Create_ShouldCreateInstance_WhenHomoclaveIsValid(Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(homoclave: homoclave);
      var expectedValue = new MxCurp(curp);

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(ValidCheckDigits))]
   public void MxCurp_Create_ShouldCreateInstance_WhenCheckDigitIsValid(Char checkDigit)
   {
      // Arrange.
      var curp = GetCurp(checkDigit: checkDigit);
      var expectedValue = new MxCurp(curp);

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [InlineData(null)]
   [InlineData("")]
   [InlineData("\t")]
   public void MxCurp_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String? curp)
   {
      // Act.
      var result = MxCurp.Create(curp!);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.Empty);
   }

   [Theory]
   [InlineData("MAAR790213HMNRLF0")]      // Length 17
   [InlineData("HEGG560427MVZRRL045")]    // Length 19
   public void MxCurp_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String curp)
   {
      // Act.
      var result = MxCurp.Create(curp!);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidLength);
   }

   [Theory]
   [InlineData(" AAR")]          // Space in position 0
   [InlineData("\u00E1AAR")]     // á in position 0
   [InlineData("\u00C1AAR")]     // Á in position 0
   [InlineData("M1AR")]          // Numeric character in position 1
   [InlineData("MA!R")]          // ! in position 2
   [InlineData("MAA\u00F1")]     // ñ in position 3
   [InlineData("MAA\u00D1")]     // Ñ in position 3
   public void MxCurp_Create_ShouldReturnInvalidAlphabeticCharacterEncounteredResult_WhenNameInitialsAreInvalid(String initials)
   {
      // Arrange.
      var curp = GetCurp(initials: initials);

      // Act.
      var result = MxCurp.Create(curp!);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [InlineData("A10101", '0')]         // Non-digit character 'A'
   [InlineData("0B0101", '0')]         // Non-digit character 'B'
   [InlineData("01!101", '0')]         // Non-digit character '!'
   [InlineData("010\u00F101", '0')]    // Non-digit character 'ñ'
   [InlineData("0101 1", '0')]         // Non-digit character ' '
   [InlineData("01010\u2153", '0')]    // Non-digit character Unicode fraction 1/3

   [InlineData("010001", '0')]         // Invalid month (too low)
   [InlineData("011301", 'A')]         // Invalid month (too high)
   [InlineData("010100", 'b')]         // Invalid day of month (too low)
   [InlineData("010132", '0')]         // Invalid day of month January
   [InlineData("010229", '0')]         // Invalid day of month February (non leap year, 1900's)
   [InlineData("010229", 'A')]         // Invalid day of month February (non leap year, 2000's)
   [InlineData("040230", '0')]         // Invalid day of month February (leap year, 1900's)
   [InlineData("040230", 'b')]         // Invalid day of month February (leap year 2000's)
   [InlineData("000229", '0')]         // Invalid day of month February (homoclave indicated 1900, not a leap year)
   [InlineData("000230", 'A')]         // Invalid day of month February (leap year because homoclave indicates 2000, a century divisible by 400)
   [InlineData("010332", '0')]         // Invalid day of month March
   [InlineData("010431", 'A')]         // Invalid day of month April
   [InlineData("010532", 'b')]         // Invalid day of month May
   [InlineData("010631", '0')]         // Invalid day of month June
   [InlineData("010732", 'A')]         // Invalid day of month July
   [InlineData("010832", 'b')]         // Invalid day of month August
   [InlineData("010931", '0')]         // Invalid day of month September
   [InlineData("011032", 'A')]         // Invalid day of month October
   [InlineData("011131", 'b')]         // Invalid day of month November
   [InlineData("011232", '0')]         // Invalid day of month December
   public void MxCurp_Create_ShouldReturnInvalidDateOfBirthResult_WhenDateOfBirthIsInvalid(
      String dateOfBirth,
      Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: dateOfBirth, homoclave: homoclave);

      // Act.
      var result = MxCurp.Create(curp!);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidDateOfBirth);
   }

   [Theory]
   [InlineData('A')]             // Invalid gender 'A'
   [InlineData('0')]             // Invalid gender '0'
   [InlineData('!')]             // Invalid gender '!'
   [InlineData('\u00F1')]        // Invalid gender Unicode ñ
   [InlineData('\u2153')]        // Invalid gender Unicode fraction 1/3
   public void MxCurp_Create_ShouldReturnInvalidGenderResult_WhenGenderIsInvalidCharacter(Char gender)
   {
      // Arrange.
      var curp = GetCurp(gender: gender);

      // Act.
      var result = MxCurp.Create(curp!);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidGender);
   }

   [Theory]
   [InlineData("DC")] // Invalid state code, US District of Columbia
   [InlineData("A1")]
   [InlineData("1C")]
   public void MxCurp_Create_ShouldReturnInvalidStateResult_WhenStateCodeIsInvalid(String stateCode)
   {
      // Arrange.
      var curp = GetCurp(stateCode: stateCode);

      // Act.
      var result = MxCurp.Create(curp!);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidState);
   }

   [Theory]
   [InlineData(" LF")]           // Space in position 0
   [InlineData("\u00E1LF")]      // á in position 0
   [InlineData("\u00C1LF")]      // Á in position 0
   [InlineData("R1F")]           // Numeric character in position 1
   [InlineData("RL!")]           // ! in position 2
   [InlineData("RL\u00F1")]      // ñ in position 2
   [InlineData("RL\u00D1")]      // Ñ in position 2
   public void MxCurp_Create_ShouldReturnInvalidAlphabeticCharacterEncounteredResult_WhenNameConsonantsAreInvalid(String consonants)
   {
      // Arrange.
      var curp = GetCurp(consonants: consonants);

      // Act.
      var result = MxCurp.Create(curp!);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [InlineData('!')]             // Invalid homoclave '!'
   [InlineData('\u00F1')]        // Invalid homoclave Unicode ñ
   [InlineData('\u2153')]        // Invalid homoclave Unicode fraction 1
   public void MxCurp_Create_ShouldReturnInvalidHomoclaveResult_WhenHomoclaveIsInvalidCharacter(Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(homoclave: homoclave);

      // Act.
      var result = MxCurp.Create(curp!);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidHomoclave);
   }

   [Fact]
   public void MxCurp_Create_ShouldReturnInvalidHomoclaveResult_WhenBothHomoclaveAndDateOfBirthAreInvalid()
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: "010132", homoclave: '!'); // Invalid day of month and invalid homoclave

      // Act.
      var result = MxCurp.Create(curp!);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidHomoclave);
   }

   [Theory]
   [InlineData('!')]             // Invalid check digit '!'
   [InlineData('A')]             // Invalid check digit 'A'
   [InlineData('z')]             // Invalid check digit 'z'
   [InlineData('\u00F1')]        // Invalid check digit Unicode ñ
   [InlineData('\u2153')]        // Invalid check digit Unicode fraction 1
   public void MxCurp_Create_ShouldReturnInvalidCheckDigitResult_WhenCheckDigitIsInvalidCharacter(Char checkDigit)
   {
      // Arrange.
      var curp = GetCurp(checkDigit: checkDigit);

      // Act.
      var result = MxCurp.Create(curp!);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidCheckDigit);
   }

   #endregion

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
   [InlineData("560427", '0')]
   [InlineData("000101", '0')]         // Jan 1, 1900 (digit homoclave)
   [InlineData("991231", '0')]         // Dec 31, 1999
   [InlineData("000101", 'A')]         // Jan 1, 2000 (letter homoclave)
   [InlineData("991231", 'A')]         // Dec 31, 2099
   [InlineData("040229", 'A')]         // Feb 29, 2004 (leap year)

   [InlineData("010131", '0')]         // Max day of month January
   [InlineData("010228", '0')]         // Max day of month February (non leap year, 1900's)
   [InlineData("010228", 'A')]         // Max day of month February (non leap year, 2000's)
   [InlineData("040229", '0')]         // Max day of month February (leap year, 1900's)
   [InlineData("040229", 'B')]         // Max day of month February (leap year, 2000's)
   [InlineData("000229", 'b')]         // Max day of month February (leap year because homoclave indicates 2000, a century divisible by 400)
   [InlineData("010331", '0')]         // Max day of month March
   [InlineData("010430", 'A')]         // Max day of month April
   [InlineData("010531", 'b')]         // Max day of month May
   [InlineData("010630", '0')]         // Max day of month June
   [InlineData("010731", 'A')]         // Max day of month July
   [InlineData("010831", 'b')]         // Max day of month August
   [InlineData("010930", '0')]         // Max day of month September
   [InlineData("011031", 'A')]         // Max day of month October
   [InlineData("011130", 'b')]         // Max day of month November
   [InlineData("011231", '0')]         // Max day of month December
   public void MxCurp_Validate_ShouldReturnValidationPassed_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: dateOfBirth, homoclave: homoclave);

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
   public void MxCurp_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String? curp)
      => MxCurp.Validate(curp!).Should().Be(MxCurpValidationResult.Empty);

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
   [InlineData("A10101", '0')]         // Non-digit character 'A'
   [InlineData("0B0101", '0')]         // Non-digit character 'B'
   [InlineData("01!101", '0')]         // Non-digit character '!'
   [InlineData("010\u00F101", '0')]    // Non-digit character 'ñ'
   [InlineData("0101 1", '0')]         // Non-digit character ' '
   [InlineData("01010\u2153", '0')]    // Non-digit character Unicode fraction 1/3

   [InlineData("010001", '0')]         // Invalid month (too low)
   [InlineData("011301", 'A')]         // Invalid month (too high)
   [InlineData("010100", 'b')]         // Invalid day of month (too low)
   [InlineData("010132", '0')]         // Invalid day of month January
   [InlineData("010229", '0')]         // Invalid day of month February (non leap year, 1900's)
   [InlineData("010229", 'A')]         // Invalid day of month February (non leap year, 2000's)
   [InlineData("040230", '0')]         // Invalid day of month February (leap year, 1900's)
   [InlineData("040230", 'b')]         // Invalid day of month February (leap year 2000's)
   [InlineData("000229", '0')]         // Invalid day of month February (homoclave indicated 1900, not a leap year)
   [InlineData("000230", 'A')]         // Invalid day of month February (leap year because homoclave indicates 2000, a century divisible by 400)
   [InlineData("010332", '0')]         // Invalid day of month March
   [InlineData("010431", 'A')]         // Invalid day of month April
   [InlineData("010532", 'b')]         // Invalid day of month May
   [InlineData("010631", '0')]         // Invalid day of month June
   [InlineData("010732", 'A')]         // Invalid day of month July
   [InlineData("010832", 'b')]         // Invalid day of month August
   [InlineData("010931", '0')]         // Invalid day of month September
   [InlineData("011032", 'A')]         // Invalid day of month October
   [InlineData("011131", 'b')]         // Invalid day of month November
   [InlineData("011232", '0')]         // Invalid day of month December
   public void MxCurp_Validate_ShouldReturnInvalidDateOfBirth_WhenDateOfBirthIsInvalid(
      String dateOfBirth,
      Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: dateOfBirth, homoclave: homoclave);

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
   public void MxCurp_Validate_ShouldReturnInvalidState_WhenStateCodeIsInvalid(String stateCode)
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

   [Fact]
   public void MxCurp_Validate_ShouldReturnInvalidHomoclave_WhenBothHomoclaveAndDateOfBirthAreInvalid()
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: "010132", homoclave: '!'); // Invalid day of month and invalid homoclave

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
