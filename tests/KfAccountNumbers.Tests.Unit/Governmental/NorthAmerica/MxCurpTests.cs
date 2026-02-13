// Ignore Spelling: Curp Deserialize Deserialization Homoclaves Homoclave Json  Mx

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class MxCurpTests
{
   private const String ValidCurp = "HEGG560427MVZRRL04";      // From https://en.wikipedia.org/wiki/Unique_Population_Registry_Code
   private const String AltValidCurp = "MAAR790213HMNRLF03";   // From https://sayari.com/resources/breaking-down-mexican-national-id/
   private const String AltLowerCaseValidCurp = "maar790213hmnrlf03";   // From https://sayari.com/resources/breaking-down-mexican-national-id/

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

   public static TheoryData<String> ValidFullCurpValues =>
   [
      ValidCurp,
      AltValidCurp,
   ];

   public static TheoryData<String> ValidNameInitials =>
   [
      "HEGG",
      "hegg",
   ];

   public static TheoryData<String, Char> ValidBirthDates => new()
   {
       { "560427", '0' },
       { "000101", '0' },         // Jan 1, 1900 (digit homoclave)
       { "991231", '0' },         // Dec 31, 1999
       { "000101", 'A' },         // Jan 1, 2000 (letter homoclave)
       { "991231", 'A' },         // Dec 31, 2099
       { "040229", 'A' },         // Feb 29, 2004 (leap year)

       { "010131", '0' },         // Max day of month January
       { "010228", '0' },         // Max day of month February (non leap year, 1900's)
       { "010228", 'A' },         // Max day of month February (non leap year, 2000's)
       { "040229", '0' },         // Max day of month February (leap year, 1900's)
       { "040229", 'B' },         // Max day of month February (leap year, 2000's)
       { "000229", 'b' },         // Max day of month February (leap year because homoclave indicates 2000, a century divisible by 400)
       { "010331", '0' },         // Max day of month March
       { "010430", 'A' },         // Max day of month April
       { "010531", 'b' },         // Max day of month May
       { "010630", '0' },         // Max day of month June
       { "010731", 'A' },         // Max day of month July
       { "010831", 'b' },         // Max day of month August
       { "010930", '0' },         // Max day of month September
       { "011031", 'A' },         // Max day of month October
       { "011130", 'b' },         // Max day of month November
       { "011231", '0' }          // Max day of month December
   };

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

   public static TheoryData<String> ValidNameConsonants =>
   [
      "RRL",
      "rrl",
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "MAAR790213HMNRLF0",       // Length 17
      "HEGG560427MVZRRL045",     // Length 19
   ];

   public static TheoryData<String> InvalidNameInitials =>
   [
      " AAR",                    // Space in position 0
      "\u00E1AAR",               // á in position 0
      "\u00C1AAR",               // Á in position 0
      "M1AR",                    // Numeric character in position 1
      "MA!R",                    // ! in position 2
      "MAA\u00F1",               // ñ in position 3
      "MAA\u00D1",               // Ñ in position 3
   ];

   public static TheoryData<String, Char> InvalidBirthDates => new()
   {
      { "A10101", '0' },         // Non-digit character 'A'
      { "0B0101", '0' },         // Non-digit character 'B'
      { "01!101", '0' },         // Non-digit character '!'
      { "010\u00F101", '0' },    // Non-digit character 'ñ'
      { "0101 1", '0' },         // Non-digit character ' '
      { "01010\u2153", '0' },    // Non-digit character Unicode fraction 1/3

      { "010001", '0' },         // Invalid month (too low)
      { "011301", 'A' },         // Invalid month (too high)
      { "010100", 'b' },         // Invalid day of month (too low)
      { "010132", '0' },         // Invalid day of month January
      { "010229", '0' },         // Invalid day of month February (non leap year, 1900's)
      { "010229", 'A' },         // Invalid day of month February (non leap year, 2000's)
      { "040230", '0' },         // Invalid day of month February (leap year, 1900's)
      { "040230", 'b' },         // Invalid day of month February (leap year 2000's)
      { "000229", '0' },         // Invalid day of month February (homoclave indicated 1900, not a leap year)
      { "000230", 'A' },         // Invalid day of month February (leap year because homoclave indicates 2000, a century divisible by 400)
      { "010332", '0' },         // Invalid day of month March
      { "010431", 'A' },         // Invalid day of month April
      { "010532", 'b' },         // Invalid day of month May
      { "010631", '0' },         // Invalid day of month June
      { "010732", 'A' },         // Invalid day of month July
      { "010832", 'b' },         // Invalid day of month August
      { "010931", '0' },         // Invalid day of month September
      { "011032", 'A' },         // Invalid day of month October
      { "011131", 'b' },         // Invalid day of month November
      { "011232", '0' },         // Invalid day of month December
   };

   public static TheoryData<Char> InvalidGenders =>
   [
      'A',           // Invalid gender 'A'
      '0',           // Invalid gender '0'
      '!',           // Invalid gender '!'
      '\u00F1',      // Invalid gender Unicode ñ
      '\u2153',      // Invalid gender Unicode fraction 1/3
   ];

   public static TheoryData<String> InvalidStateCodes =>
   [
      "DC",          // Invalid state code, US District of Columbia
      "A1",
      "1C",
   ];

   public static TheoryData<String> InvalidNameConsonants =>
   [
      " LF",         // Space in position 0
      "\u00E1LF",    // á in position 0
      "\u00C1LF",    // Á in position 0
      "R1F",         // Numeric character in position 1
      "RL!",         // ! in position 2
      "RL\u00F1",    // ñ in position 2
      "RL\u00D1",    // Ñ in position 2
   ];

   public static TheoryData<Char> InvalidHomoclaves =>
   [
      '!',           // Invalid homoclave '!'
      '\u00F1',      // Invalid homoclave Unicode ñ
      '\u2153',      // Invalid homoclave Unicode fraction 1
   ];

   public static TheoryData<Char> InvalidCheckDigits =>
   [
      '!',           // Invalid check digit '!'
      'A',           // Invalid check digit 'A'
      'z',           // Invalid check digit 'z'
      '\u00F1',      // Invalid check digit Unicode ñ
      '\u2153',      // Invalid check digit Unicode fraction 1
   ];

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFullCurpValues))]
   public void MxCurp_Constructor_ShouldCreateInstance_WhenFullCurpValueIsValid(String curp)
   {
      // Arrange.
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = new MxCurp(curp);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidNameInitials))]
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
   [MemberData(nameof(ValidBirthDates))]
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
   [MemberData(nameof(ValidNameConsonants))]
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

   [Fact]
   public void MxCurp_Constructor_ShouldNormalizeValueToUpperCase_WhenValueIsValid()
   {
      // Arrange.
      var curp = ValidCurp.ToLowerInvariant();
      var expected = ValidCurp.ToUpperInvariant();

      // Act.
      var sut = new MxCurp(curp);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void MxCurp_Constructor_ShouldThrowInvalidMxCurpException_WhenValueIsNullOrEmpty(String curp)
      => FluentActions
         .Invoking(() => new MxCurp(curp))
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpEmpty + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void MxCurp_Constructor_ShouldThrowInvalidMxCurpException_WhenValueHasInvalidLength(String curp)
      => FluentActions
         .Invoking(() => new MxCurp(curp))
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidLength + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidNameInitials))]
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
   [MemberData(nameof(InvalidBirthDates))]
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
   [MemberData(nameof(InvalidGenders))]
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
   [MemberData(nameof(InvalidStateCodes))]
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
   [MemberData(nameof(InvalidNameConsonants))]
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
   [MemberData(nameof(InvalidHomoclaves))]
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
   [MemberData(nameof(InvalidCheckDigits))]
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

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidCurp)]
   [InlineData(AltLowerCaseValidCurp)]
   public void MxCurp_Value_ShouldReturnNormalizedCurp(String curp)
   {
      // Arrange.
      var expected = curp.ToUpperInvariant();
      var sut = new MxCurp(curp);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void MxCurp_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
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
   public void MxCurp_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
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
   public void MxCurp_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      MxCurp curp = null!;

      // Act.
      String str = curp;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void MxCurp_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      MxCurp curp = null!;

      // Act.
      String str = curp;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidFullCurpValues))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldCreateInstance_WhenFullCurpValueIsValid(String curp)
   {
      // Arrange.
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = (MxCurp)curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidNameInitials))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldCreateInstance_WhenNameInitialsAreValid(String initials)
   {
      // Arrange.
      var curp = GetCurp(initials);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = (MxCurp)curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidBirthDates))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: dateOfBirth, homoclave: homoclave);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = (MxCurp)curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidGenders))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldCreateInstance_WhenGenderIsValid(Char gender)
   {
      // Arrange.
      var curp = GetCurp(gender: gender);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = (MxCurp)curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidStateCodes))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldCreateInstance_WhenStateCodeIsValid(String stateCode)
   {
      // Arrange.
      var curp = GetCurp(stateCode: stateCode);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = (MxCurp)curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidNameConsonants))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldCreateInstance_WhenNameConsonantsAreValid(String consonants)
   {
      // Arrange.
      var curp = GetCurp(consonants: consonants);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = (MxCurp)curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidHomoclaves))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldCreateInstance_WhenHomoclaveIsValid(Char homoclave)
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
   public void MxCurp_ExplicitCastToMxCurp_ShouldCreateInstance_WhenCheckDigitIsValid(Char checkDigit)
   {
      // Arrange.
      var curp = GetCurp(checkDigit: checkDigit);
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = (MxCurp)curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Fact]
   public void MxCurp_ExplicitCastToMxCurp_ShouldNormalizeValueToUpperCase_WhenValueIsValid()
   {
      // Arrange.
      var curp = ValidCurp.ToLowerInvariant();
      var expected = curp.ToUpperInvariant();

      // Act.
      var sut = (MxCurp)curp;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldThrowInvalidMxCurpException_WhenValueIsNullOrEmpty(String str)
      => FluentActions
         .Invoking(() => _ = (MxCurp)str)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpEmpty + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldThrowInvalidMxCurpException_WhenValueHasInvalidLength(String str)
   => FluentActions
         .Invoking(() => _ = (MxCurp)str)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidLength + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidNameInitials))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldThrowInvalidMxCurpException_WhenNameInitialsAreInvalid(String initials)
   {
      // Arrange.
      var str = GetCurp(initials);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (MxCurp)str)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidAlphabeticCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [MemberData(nameof(InvalidBirthDates))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldThrowInvalidMxCurpException_WhenDateOfBirthIsInvalid(
      String dateOfBirth,
      Char homoclave)
   {
      // Arrange.
      var str = GetCurp(dateOfBirth: dateOfBirth, homoclave: homoclave);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (MxCurp)str)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidDateOfBirth + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidDateOfBirth);
   }

   [Theory]
   [MemberData(nameof(InvalidGenders))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldThrowInvalidMxCurpException_WhenGenderIsInvalidCharacter(Char gender)
   {
      // Arrange.
      var str = GetCurp(gender: gender);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (MxCurp)str)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidGender + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidGender);
   }

   [Theory]
   [MemberData(nameof(InvalidStateCodes))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldThrowInvalidMxCurpException_WhenStateCodeIsInvalid(String stateCode)
   {
      // Arrange.
      var str = GetCurp(stateCode: stateCode);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (MxCurp)str)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidState + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidState);
   }

   [Theory]
   [MemberData(nameof(InvalidNameConsonants))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldThrowInvalidMxCurpException_WhenNameConsonantsAreInvalid(String consonants)
   {
      // Arrange.
      var str = GetCurp(consonants: consonants);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (MxCurp)str)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidAlphabeticCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [MemberData(nameof(InvalidHomoclaves))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldThrowInvalidMxCurpException_WhenHomoclaveIsInvalidCharacter(Char homoclave)
   {
      // Arrange.
      var str = GetCurp(homoclave: homoclave);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (MxCurp)str)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidHomoclave + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidHomoclave);
   }

   [Fact]
   public void MxCurp_ExplicitCastToMxCurp_ShouldThrowInvalidMxCurpException_WhenBothHomoclaveAndDateOfBirthAreInvalid()
   {
      // Arrange.
      var str = GetCurp(dateOfBirth: "010132", homoclave: '!'); // Invalid day of month and invalid homoclave

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (MxCurp)str)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidHomoclave + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidHomoclave);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigits))]
   public void MxCurp_ExplicitCastToMxCurp_ShouldThrowInvalidMxCurpException_WhenCheckDigitIsInvalidCharacter(Char checkDigit)
   {
      // Arrange.
      var str = GetCurp(checkDigit: checkDigit);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (MxCurp)str)
         .Should().Throw<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidCheckDigit);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void MxCurp_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var curp1 = new MxCurp(ValidCurp);
      var curp2 = new MxCurp(ValidCurp.ToLowerInvariant());    // Will normalize to same value

      // Act/assert.
      (curp1 == curp2).Should().BeTrue();
   }

   [Fact]
   public void MxCurp_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var curp1 = new MxCurp(ValidCurp);
      var curp2 = new MxCurp(AltValidCurp);

      // Act/assert.
      (curp1 == curp2).Should().BeFalse();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void MxCurp_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var curp1 = new MxCurp(ValidCurp);
      var curp2 = new MxCurp(AltValidCurp);

      // Act/assert.
      (curp1 != curp2).Should().BeTrue();
   }

   [Fact]
   public void MxCurp_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var curp1 = new MxCurp(ValidCurp);
      var curp2 = new MxCurp(ValidCurp.ToLowerInvariant());    // Will normalize to same value

      // Act/assert.
      (curp1 != curp2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFullCurpValues))]
   public void MxCurp_Create_ShouldCreateInstance_WhenFullCurpIsValueIsValid(String curp)
   {
      // Arrange.
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
   [MemberData(nameof(ValidNameInitials))]
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
   [MemberData(nameof(ValidBirthDates))]
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
   [MemberData(nameof(ValidNameConsonants))]
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

   [Fact]
   public void MxCurp_Create_ShouldNormalizeToUpperCase_WhenValueIsValid()
   {
      // Arrange.
      var curp = ValidCurp.ToLowerInvariant();
      var expectedValue = new MxCurp(curp.ToUpperInvariant());

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void MxCurp_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String curp)
   {
      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.Empty);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void MxCurp_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String curp)
   {
      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidLength);
   }

   [Theory]
   [MemberData(nameof(InvalidNameInitials))]
   public void MxCurp_Create_ShouldReturnInvalidAlphabeticCharacterEncounteredResult_WhenNameInitialsAreInvalid(String initials)
   {
      // Arrange.
      var curp = GetCurp(initials: initials);

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [MemberData(nameof(InvalidBirthDates))]
   public void MxCurp_Create_ShouldReturnInvalidDateOfBirthResult_WhenDateOfBirthIsInvalid(
      String dateOfBirth,
      Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(dateOfBirth: dateOfBirth, homoclave: homoclave);

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidDateOfBirth);
   }

   [Theory]
   [MemberData(nameof(InvalidGenders))]
   public void MxCurp_Create_ShouldReturnInvalidGenderResult_WhenGenderIsInvalidCharacter(Char gender)
   {
      // Arrange.
      var curp = GetCurp(gender: gender);

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidGender);
   }

   [Theory]
   [MemberData(nameof(InvalidStateCodes))]
   public void MxCurp_Create_ShouldReturnInvalidStateResult_WhenStateCodeIsInvalid(String stateCode)
   {
      // Arrange.
      var curp = GetCurp(stateCode: stateCode);

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidState);
   }

   [Theory]
   [MemberData(nameof(InvalidNameConsonants))]
   public void MxCurp_Create_ShouldReturnInvalidAlphabeticCharacterEncounteredResult_WhenNameConsonantsAreInvalid(String consonants)
   {
      // Arrange.
      var curp = GetCurp(consonants: consonants);

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [MemberData(nameof(InvalidHomoclaves))]
   public void MxCurp_Create_ShouldReturnInvalidHomoclaveResult_WhenHomoclaveIsInvalidCharacter(Char homoclave)
   {
      // Arrange.
      var curp = GetCurp(homoclave: homoclave);

      // Act.
      var result = MxCurp.Create(curp);

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
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidHomoclave);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigits))]
   public void MxCurp_Create_ShouldReturnInvalidCheckDigitResult_WhenCheckDigitIsInvalidCharacter(Char checkDigit)
   {
      // Arrange.
      var curp = GetCurp(checkDigit: checkDigit);

      // Act.
      var result = MxCurp.Create(curp);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(MxCurpValidationResult.InvalidCheckDigit);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void MxCurp_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var curp1 = new MxCurp(ValidCurp);
      var curp2 = new MxCurp(ValidCurp.ToLowerInvariant());    // Will normalize to same value

      // Act/assert.
      curp1.Equals(curp2).Should().BeTrue();
   }

   [Fact]
   public void MxCurp_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var curp1 = new MxCurp(ValidCurp);
      var curp2 = new MxCurp(AltValidCurp);

      // Act/assert.
      curp1.Equals(curp2).Should().BeFalse();
   }

   [Fact]
   public void MxCurp_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new MxCurp(ValidCurp);

      // Act/assert.
      sut.Equals(ValidCurp).Should().BeFalse();
   }

   [Fact]
   public void MxCurp_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new MxCurp(ValidCurp);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void MxCurp_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var curp1 = new MxCurp(ValidCurp);
      var curp2 = new MxCurp(ValidCurp);

      // Act.
      var hash1 = curp1.GetHashCode();
      var hash2 = curp2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void MxCurp_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var curp1 = new MxCurp(ValidCurp);
      var curp2 = new MxCurp(AltValidCurp);

      // Act.
      var hash1 = curp1.GetHashCode();
      var hash2 = curp2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   #endregion

   #region ReferenceEquals Method Tests
   // ==========================================================================
   // ==========================================================================

   // MxCurp does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void MxCurp_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var curp1 = new MxCurp(ValidCurp);
      var curp2 = new MxCurp(ValidCurp.ToLowerInvariant());       // Will normalize to same value

      // Act/assert.
      (curp1 == curp2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(curp1, curp2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("HEGG560427MVZRRL04", "HEGG560427MVZRRL04")]
   [InlineData("hegg560427mvzrrl04", "HEGG560427MVZRRL04")]    // Lowercase should be normalized to uppercase
   public void MxCurp_ToString_ShouldReturnExpectedValue(
      String curp,
      String expected)
   {
      // Arrange.
      var sut = new MxCurp(curp);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFullCurpValues))]
   public void MxCurp_Validate_ShouldReturnValidationPassed_WhenFullCurpValueIsValid(String curp)
      => MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(ValidNameInitials))]
   public void MxCurp_Validate_ShouldReturnValidationPassed_WhenNameInitialsAreValid(String initials)
   {
      // Arrange.
      var curp = GetCurp(initials);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.ValidationPassed);
   }

   [Theory]
   [MemberData(nameof(ValidBirthDates))]
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
   [MemberData(nameof(ValidNameConsonants))]
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

   [Fact]
   public void MxCurp_Validate_ShouldReturnValidationPassed_WhenValueIsValidButInLowerCase()
   {
      // Arrange.
      var curp = ValidCurp.ToLowerInvariant();

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.ValidationPassed);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void MxCurp_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String curp)
      => MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void MxCurp_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String curp)
      => MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidNameInitials))]
   public void MxCurp_Validate_ShouldReturnInvalidAlphabeticCharacterEncountered_WhenNameInitialsAreInvalid(String initials)
   {
      // Arrange.
      var curp = GetCurp(initials);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [MemberData(nameof(InvalidBirthDates))]
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
   [MemberData(nameof(InvalidGenders))]
   public void MxCurp_Validate_ShouldReturnInvalidGender_WhenGenderIsInvalidCharacter(Char gender)
   {
      // Arrange.
      var curp = GetCurp(gender: gender);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidGender);
   }

   [Theory]
   [MemberData(nameof(InvalidStateCodes))]
   public void MxCurp_Validate_ShouldReturnInvalidState_WhenStateCodeIsInvalid(String stateCode)
   {
      // Arrange.
      var curp = GetCurp(stateCode: stateCode);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidState);
   }

   [Theory]
   [MemberData(nameof(InvalidNameConsonants))]
   public void MxCurp_Validate_ShouldReturnInvalidAlphabeticCharacterEncountered_WhenNameConsonantsAreInvalid(String consonants)
   {
      // Arrange.
      var curp = GetCurp(consonants: consonants);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidAlphabeticCharacterEncountered);
   }

   [Theory]
   [MemberData(nameof(InvalidHomoclaves))]
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
   [MemberData(nameof(InvalidCheckDigits))]
   public void MxCurp_Validate_ShouldReturnInvalidCheckDigit_WhenCheckDigitIsInvalidCharacter(Char checkDigit)
   {
      // Arrange.
      var curp = GetCurp(checkDigit: checkDigit);

      // Act/assert.
      MxCurp.Validate(curp).Should().Be(MxCurpValidationResult.InvalidCheckDigit);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void MxCurp_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var curp = GetCurp();
      var sut = new MxCurp(curp);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<MxCurp>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void MxCurp_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new MxCurp(ValidCurp);

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{ValidCurp}\"");  // Simple string, not object
   }

   public class Foo
   {
      public MxCurp Curp { get; set; } = null!;
   }

   [Fact]
   public void MxCurp_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Curp = new MxCurp(ValidCurp) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void MxCurp_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Curp\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void MxCurp_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Curp\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Curp.Should().BeNull();
   }

   [Fact]
   public void MxCurp_JsonDeserialization_ShouldThrowInvalidMxCurpException_WhenCurpIsInvalid()
   {
      // Arrange.
      var json = "{\"Curp\":\"MAAR790213HMNRLF0\"}";  // Invalid length

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should()
         .ThrowExactly<InvalidMxCurpException>()
         .WithMessage(Messages.MxCurpInvalidLength + "*")
         .And.ValidationResult.Should().Be(MxCurpValidationResult.InvalidLength);
   }

   #endregion
}
