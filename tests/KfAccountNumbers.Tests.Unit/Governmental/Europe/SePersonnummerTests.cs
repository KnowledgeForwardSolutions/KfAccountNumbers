// Ignore Spelling: Deserialize Deserialization Json Kf Personnummer Samordningsnummer

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class SePersonnummerTests
{
   private const String Valid11CharacterDashPersonnummer = "811228-9874";        // From Wikipedia, https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
   private const String Valid11CharacterPlusPersonnummer = "811228+9874";
   private const String Valid13CharacterDashPersonnummer = "19670919-9530";
   private const String Valid13CharacterPlusPersonnummer = "20670919+9530";      // Future date, but valid format and checksum

   private const String Valid11CharacterDashSamordningsnummer = "811288-9871";
   private const String Valid11CharacterPlusSamordningsnummer = "811288+9871";
   private const String Valid13CharacterDashSamordningsnummer = "19670979-9537";
   private const String Valid13CharacterPlusSamordningsnummer = "20670979+9537"; // Future date, but valid format and checksum

   private static String GetPersonnummer(
      String dateOfBirth = "811228",
      Char separator = '-',
      String birthSerialNumber = "987",
      Char checkDigit = '4')
      => $"{dateOfBirth}{separator}{birthSerialNumber}{checkDigit}";

   private static String GetPersonnummerWithValidCheckDigit(
      String dateOfBirth = "811228",
      Char separator = '-',
      String birthSerialNumber = "987")
   {
      var partialPersonnummer = $"{dateOfBirth[^6..]}{birthSerialNumber}";
      _ = CheckDigits.Net.Algorithms.Luhn.TryCalculateCheckDigit(partialPersonnummer, out var checkDigit);

      return $"{dateOfBirth}{separator}{birthSerialNumber}{checkDigit}";
   }

   public static TheoryData<String> ValidPersonnummerValues =>
   [
      Valid11CharacterDashPersonnummer,
      Valid11CharacterPlusPersonnummer,
      Valid13CharacterDashPersonnummer,
      Valid13CharacterPlusPersonnummer,
   ];

   public static TheoryData<String> ValidSamordningsnummerValues =>
   [
      Valid11CharacterDashSamordningsnummer,
      Valid11CharacterPlusSamordningsnummer,
      Valid13CharacterDashSamordningsnummer,
      Valid13CharacterPlusSamordningsnummer
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "811228-987",        // Length 10
      "811228-98745",      // Length 12
      "19811228-98745",    // Length 14
   ];

   public static TheoryData<String> ValidPersonnummerDateOfBirthValues =>
   [
      // Six digit dates assume 20th century birth years, i.e. 1900 - 1999
      "000101",            // Min valid date, Jan 1, 1900
      "991231",            // Max six digit valid date, Dec 31, 1999

      "010131",            // Max day of month January
      "000228",            // Max day of month February (1900 is non-leap year)
      "040229",            // Max day of month February (leap year)
      "010331",            // Max day of month March
      "010430",            // Max day of month April
      "010531",            // Max day of month May
      "010630",            // Max day of month June
      "010731",            // Max day of month July
      "010831",            // Max day of month August
      "010930",            // Max day of month September
      "011031",            // Max day of month October
      "011130",            // Max day of month November
      "011231",            // Max day of month December

      "19000101",          // Jan 1, 1900
      "20991231",          // Dec 31, 2099

      "19010131",          // Max day of month January
      "19000228",          // Max day of month February (1900 is non-leap year)
      "20000229",          // Max day of month February (2000 is leap year)
      "19040229",          // Max day of month February (leap year)
      "19010331",          // Max day of month March
      "19010430",          // Max day of month April
      "19010531",          // Max day of month May
      "20010630",          // Max day of month June
      "20010731",          // Max day of month July
      "20010831",          // Max day of month August
      "20010930",          // Max day of month September
      "20011031",          // Max day of month October
      "20011130",          // Max day of month November
      "20011231",          // Max day of month December
   ];

   public static TheoryData<String> ValidSamordningsnummerDateOfBirthValues =>
   [
      // Six digit dates assume 20th century birth years, i.e. 1900 - 1999
      "000161",            // Min valid date, Jan 1, 1900
      "991291",            // Max six digit valid date, Dec 31, 1999

      "010191",            // Max day of month January
      "000288",            // Max day of month February (1900 is non-leap year)
      "040289",            // Max day of month February (leap year)
      "010391",            // Max day of month March
      "010490",            // Max day of month April
      "010591",            // Max day of month May
      "010690",            // Max day of month June
      "010791",            // Max day of month July
      "010891",            // Max day of month August
      "010990",            // Max day of month September
      "011091",            // Max day of month October
      "011190",            // Max day of month November
      "011291",            // Max day of month December

      "19000161",          // Jan 1, 1900
      "20991291",          // Dec 31, 2099

      "19010191",          // Max day of month January
      "19000288",          // Max day of month February (1900 is non-leap year)
      "20000289",          // Max day of month February (2000 is leap year)
      "19040289",          // Max day of month February (leap year)
      "19010391",          // Max day of month March
      "19010490",          // Max day of month April
      "19010591",          // Max day of month May
      "20010690",          // Max day of month June
      "20010791",          // Max day of month July
      "20010891",          // Max day of month August
      "20010990",          // Max day of month September
      "20011091",          // Max day of month October
      "20011190",          // Max day of month November
      "20011291",          // Max day of month December
   ];

   public static TheoryData<String> InvalidSixDigitPersonnummerDateOfBirthValues =>
   [
      "A10101",            // Non-digit character 'A'
      "0B0101",            // Non-digit character 'B'
      "01!101",            // Non-digit character '!'
      "0101 1",            // Non-digit character ' '
      "01010\u2153",       // Non-digit character Unicode fraction 1/3

      "010001",            // Invalid month (too low)
      "011301",            // Invalid month (too high)
      "010100",            // Invalid day of month (too low)
      "010132",            // Invalid day of month January
      "010229",            // Invalid day of month February (non leap year)
      "000229",            // Invalid day of month February (1900 is not a leap year)
      "040230",            // Invalid day of month February (leap year)
      "010332",            // Invalid day of month March
      "010431",            // Invalid day of month April
      "010532",            // Invalid day of month May
      "010631",            // Invalid day of month June
      "010732",            // Invalid day of month July
      "010832",            // Invalid day of month August
      "010931",            // Invalid day of month September
      "011032",            // Invalid day of month October
      "011131",            // Invalid day of month November
      "011232",            // Invalid day of month December
   ];

   public static TheoryData<String> InvalidSixDigitSamordningsnummerDateOfBirthValues =>
   [
      "010160",            // Invalid day of month (too low)
      "010192",            // Invalid day of month January
      "010289",            // Invalid day of month February (non leap year)
      "000289",            // Invalid day of month February (1900 is not a leap year)
      "040290",            // Invalid day of month February (leap year)
      "010392",            // Invalid day of month March
      "010491",            // Invalid day of month April
      "010592",            // Invalid day of month May
      "010691",            // Invalid day of month June
      "010792",            // Invalid day of month July
      "010892",            // Invalid day of month August
      "010991",            // Invalid day of month September
      "011092",            // Invalid day of month October
      "011191",            // Invalid day of month November
      "011292",            // Invalid day of month December
   ];

   public static TheoryData<String> InvalidEightDigitPersonnummerDateOfBirthValues =>
   [
      "a9811228",          // Non-digit character 'a'
      "1b811228",          // Non-digit character 'b'
      "1981A228",          // Non-digit character 'A'
      "19810B28",          // Non-digit character 'B'
      "198112!8",          // Non-digit character '!'
      "1981122 ",          // Non-digit character ' '
      "1981122\u2153",     // Non-digit character Unicode fraction 1/3

      "18811228",          // Invalid century (too low)
      "21811228",          // Invalid century (too high)
      "19810028",          // Invalid month (too low)
      "19811328",          // Invalid month (too high)
      "19810100",          // Invalid day of month (too low)
      "19810132",          // Invalid day of month January
      "19810229",          // Invalid day of month February (non leap year)
      "19000229",          // Invalid day of month February (1900 is not a leap year)
      "20040230",          // Invalid day of month February (leap year)
      "19810332",          // Invalid day of month March
      "19810431",          // Invalid day of month April
      "19810532",          // Invalid day of month May
      "20810631",          // Invalid day of month June
      "20810732",          // Invalid day of month July
      "20810832",          // Invalid day of month August
      "20810931",          // Invalid day of month September
      "20811032",          // Invalid day of month October
      "20811131",          // Invalid day of month November
      "20811232",          // Invalid day of month December
   ];

   public static TheoryData<String> InvalidEightDigitSamordningsnummerDateOfBirthValues =>
   [
      "19010160",          // Invalid day of month (too low)
      "19010192",          // Invalid day of month January
      "19010289",          // Invalid day of month February (non leap year)
      "19000289",          // Invalid day of month February (1900 is not a leap year)
      "19040290",          // Invalid day of month February (leap year)
      "19010392",          // Invalid day of month March
      "19010491",          // Invalid day of month April
      "19010592",          // Invalid day of month May
      "20010691",          // Invalid day of month June
      "20010792",          // Invalid day of month July
      "20010892",          // Invalid day of month August
      "20010991",          // Invalid day of month September
      "20011092",          // Invalid day of month October
      "20011191",          // Invalid day of month November
      "20011292",          // Invalid day of month December
   ];

   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "811228*9874",
      "19811228*9874"
   ];

   public static TheoryData<String> InvalidBirthSerialNumberValues =>
   [
      "A87",               // Non-digit character 'A'
      "9B7",               // Non-digit character 'B'
      "98!",               // Non-digit character '!'
      "98 ",               // Non-digit character ' '
      "98\u2153",          // Non-digit character Unicode fraction 1/3
   ];

   public static TheoryData<String> UndetectableCheckDigitErrors =>
   [
      "010430-0918",       // 010430-9018 with two digit transposition 90 -> 09
      "880411+2558",       // 880411+2228 with two digit twin error 22 -> 55
      "20010430-0918",     // 20010430-9018 with two digit transposition 90 -> 09
      "19880411-2558",     // 19880411+2228 with two digit twin error 22 -> 55

      "010490-0915",       // 010490-9015 with two digit transposition 90 -> 09
      "880471+2555",       // 880471+2225 with two digit twin error 22 -> 55
      "20010490-0915",     // 20010490-9015 with two digit transposition 90 -> 09
      "19880471-2555",     // 19880471+2225 with two digit twin error 22 -> 55
   ];

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "811228-9875",       // 811228-9874 with invalid check digit 4 -> 5
      "811227-9874",       // 811228-9874 with single digit transcription error 8 -> 7
      "821228-9874",       // 811228-9874 with single digit transcription error 1 -> 2
      "181228-9874",       // 811228-9874 with two digit transcription error 81 -> 18
      "811228-9847",       // 811228-9874 with two digit transcription error 74 -> 47
      "880422+1238",       // 880411+1238 wtih two digit twin error 11 -> 22
      "880411+3328",       // 880411+2228 with two digit twin error 22 -> 33
      "19811228-9875",     // 19811228-9874 with invalid check digit 4 -> 5
      "19811227-9874",     // 19811228-9874 with single digit transcription error 8 -> 7
      "20821228-9874",     // 20811228-9874 with single digit transcription error 1 -> 2
      "20181228-9874",     // 20811228-9874 with two digit transcription error 81 -> 18
      "19811228-9847",     // 19811228-9874 with two digit transcription error 74 -> 47
      "19880422+1238",     // 19880411+1238 wtih two digit twin error 11 -> 22
      "19880411+3328",     // 19880411+2228 with two digit twin error 22 -> 33
   ];

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_Constructor_ShouldCreateInstance_WhenValueIsValid(String personnummer)
   {
      // Act.
      var sut = new SePersonnummer(personnummer);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(personnummer);
   }

   [Theory]
   [MemberData(nameof(ValidPersonnummerDateOfBirthValues))]
   [MemberData(nameof(ValidSamordningsnummerDateOfBirthValues))]
   public void SePersonnummer_Constructor_ShouldCreateInstance_WhenDateOfBirthIsValid(String dateOfBirth)
   {
      // Arrange.
      var personnummer = GetPersonnummerWithValidCheckDigit(dateOfBirth: dateOfBirth);

      // Act.
      var sut = new SePersonnummer(personnummer);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(personnummer);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String personnummer)
      => FluentActions
         .Invoking(() => new SePersonnummer(personnummer))
         .Should().Throw<KfValidationException<SePersonnummerValidationResult>>()
         .WithMessage(Messages.SePersonnummerEmpty + "*")
         .And.ValidationResult.Should().Be(SePersonnummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String personnummer)
      => FluentActions
         .Invoking(() => new SePersonnummer(personnummer))
         .Should().Throw<KfValidationException<SePersonnummerValidationResult>>()
         .WithMessage(Messages.SePersonnummerInvalidLength + "*")
         .And.ValidationResult.Should().Be(SePersonnummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSixDigitPersonnummerDateOfBirthValues))]
   [MemberData(nameof(InvalidEightDigitPersonnummerDateOfBirthValues))]
   [MemberData(nameof(InvalidSixDigitSamordningsnummerDateOfBirthValues))]
   [MemberData(nameof(InvalidEightDigitSamordningsnummerDateOfBirthValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(String dateOfBirth)
   {
      // Arrange.
      var personnummer = GetPersonnummer(dateOfBirth: dateOfBirth);

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(personnummer))
         .Should().Throw<KfValidationException<SePersonnummerValidationResult>>()
         .WithMessage(Messages.SePersonnummerInvalidDateOfBirth + "*")
         .And.ValidationResult.Should().Be(SePersonnummerValidationResult.InvalidDateOfBirth);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String personnummer)
      => FluentActions
         .Invoking(() => new SePersonnummer(personnummer))
         .Should().Throw<KfValidationException<SePersonnummerValidationResult>>()
         .WithMessage(Messages.SePersonnummerInvalidSeparator + "*")
         .And.ValidationResult.Should().Be(SePersonnummerValidationResult.InvalidSeparator);

   [Theory]
   [MemberData(nameof(InvalidBirthSerialNumberValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidBirthSerialNumber(String birthSerialNumber)
   {
      // Arrange.
      var dateOfBirth = "20010616";
      var personnummer = GetPersonnummer(
         dateOfBirth: dateOfBirth,
         birthSerialNumber: birthSerialNumber);

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(personnummer))
         .Should().Throw<KfValidationException<SePersonnummerValidationResult>>()
         .WithMessage(Messages.SePersonnummerInvalidBirthSerialNumber + "*")
         .And.ValidationResult.Should().Be(SePersonnummerValidationResult.InvalidBirthSerialNumber);
   }

   [Theory]
   [MemberData(nameof(UndetectableCheckDigitErrors))]
   public void SePersonnummer_Constructor_ShouldCreateInstance_WhenCheckDigitHasUndetectableError(String personnummer)
   {
      // Act.
      var sut = new SePersonnummer(personnummer);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(personnummer);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String personnummer)
      => FluentActions
         .Invoking(() => new SePersonnummer(personnummer))
         .Should().Throw<KfValidationException<SePersonnummerValidationResult>>()
         .WithMessage(Messages.SePersonnummerInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(SePersonnummerValidationResult.InvalidCheckDigit);

   #endregion

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("000101", "19000101")]     // Jan 1, 1900
   [InlineData("991231", "19991231")]     // Dec 31, 1999
   [InlineData("000228", "19000228")]     // Max day of month February (non leap year)
   [InlineData("040229", "19040229")]     // Max day of month February (leap year)

   [InlineData("19000101", "19000101")]   // Jan 1, 1900
   [InlineData("19991231", "19991231")]   // Dec 31, 1999
   [InlineData("20000101", "20000101")]   // Jan 1, 2000
   [InlineData("20991231", "20991231")]   // Dec 31, 2099

   [InlineData("19000228", "19000228")]     // Max day of month February (non leap year)
   [InlineData("19040229", "19040229")]     // Max day of month February (leap year)
   [InlineData("20000229", "20000229")]     // Max day of month February (leap year because century divisible by 400)
   [InlineData("20010228", "20010228")]     // Max day of month February (non leap year)
   [InlineData("20040229", "20040229")]     // Max day of month February (leap year)

   // Samordningsnummer values
   [InlineData("000161", "19000101")]     // Jan 1, 1900
   [InlineData("991291", "19991231")]     // Dec 31, 1999
   [InlineData("000288", "19000228")]     // Max day of month February (non leap year)
   [InlineData("040289", "19040229")]     // Max day of month February (leap year)

   [InlineData("19000161", "19000101")]   // Jan 1, 1900
   [InlineData("19991291", "19991231")]   // Dec 31, 1999
   [InlineData("20000161", "20000101")]   // Jan 1, 2000
   [InlineData("20991291", "20991231")]   // Dec 31, 2099

   [InlineData("19000288", "19000228")]     // Max day of month February (non leap year)
   [InlineData("19040289", "19040229")]     // Max day of month February (leap year)
   [InlineData("20000289", "20000229")]     // Max day of month February (leap year because century divisible by 400)
   [InlineData("20010288", "20010228")]     // Max day of month February (non leap year)
   [InlineData("20040289", "20040229")]     // Max day of month February (leap year)
   public void SePersonnummer_DateOfBirth_ShouldReturnExpectedValue(
      String dateOfBirth,
      String expectedDateOfBirth)
   {
      // Arrange.
      var personnummer = GetPersonnummerWithValidCheckDigit(dateOfBirth: dateOfBirth);
      var sut = new SePersonnummer(personnummer);
      var expected = DateOnly.ParseExact(
         expectedDateOfBirth,
         "yyyyMMdd",
         System.Globalization.CultureInfo.InvariantCulture);

      // Act/assert.
      sut.DateOfBirth.Should().Be(expected);
   }

   #endregion

   #region Gender Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData('0', BinaryGender.Female)]
   [InlineData('1', BinaryGender.Male)]
   [InlineData('2', BinaryGender.Female)]
   [InlineData('3', BinaryGender.Male)]
   [InlineData('4', BinaryGender.Female)]
   [InlineData('5', BinaryGender.Male)]
   [InlineData('6', BinaryGender.Female)]
   [InlineData('7', BinaryGender.Male)]
   [InlineData('8', BinaryGender.Female)]
   [InlineData('9', BinaryGender.Male)]
   public void SePersonnummer_Gender_ShouldReturnExpectedValue(
      Char digit,
      BinaryGender expectedGender)
   {
      // Arrange.
      var birthSerialNumber = $"54{digit}";
      var personnummer = GetPersonnummerWithValidCheckDigit(birthSerialNumber: birthSerialNumber);
      var sut = new SePersonnummer(personnummer);

      // Act/assert.
      sut.Gender.Should().Be(expectedGender);
   }

   #endregion

   #region IdentifierType Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("890301")]
   [InlineData("890311")]
   [InlineData("890321")]
   [InlineData("890331")]
   [InlineData("20050301")]
   [InlineData("20050311")]
   [InlineData("20050321")]
   [InlineData("20050331")]
   public void SePersonnummer_IdentifierType_ShouldReturnExpectedValue_WhenValueIsPersonnummer(String dateOfBirth)
   {
      // Arrange.
      var personnummer = GetPersonnummerWithValidCheckDigit(dateOfBirth: dateOfBirth);
      var sut = new SePersonnummer(personnummer);

      // Act/assert.
      sut.IdentifierType.Should().Be(SeIdentifierType.Personnummer);
   }

   [Theory]
   [InlineData("890361")]
   [InlineData("890371")]
   [InlineData("890381")]
   [InlineData("890391")]
   [InlineData("20050361")]
   [InlineData("20050371")]
   [InlineData("20050381")]
   [InlineData("20050391")]
   public void SePersonnummer_IdentifierType_ShouldReturnExpectedValue_WhenValueIsSamordningsnummer(String dateOfBirth)
   {
      // Arrange.
      var personnummer = GetPersonnummerWithValidCheckDigit(dateOfBirth: dateOfBirth);
      var sut = new SePersonnummer(personnummer);

      // Act/assert.
      sut.IdentifierType.Should().Be(SeIdentifierType.Samordningsnummer);
   }

   #endregion

   #region IsCentenarian Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(Valid11CharacterDashPersonnummer, false)]
   [InlineData(Valid11CharacterPlusPersonnummer, true)]
   [InlineData(Valid13CharacterDashPersonnummer, false)]
   [InlineData(Valid13CharacterPlusPersonnummer, true)]
   [InlineData(Valid11CharacterDashSamordningsnummer, false)]
   [InlineData(Valid11CharacterPlusSamordningsnummer, true)]
   [InlineData(Valid13CharacterDashSamordningsnummer, false)]
   [InlineData(Valid13CharacterPlusSamordningsnummer, true)]
   public void SePersonnummer_IsCentenarian_ShouldReturnExpectedValue(
      String personnummer,
      Boolean expected)
   {
      // Arrange.
      var sut = new SePersonnummer(personnummer);

      // Act/assert.
      sut.IsCentenarian.Should().Be(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_Value_ShouldReturnValidatedPersonnummer(String personnummer)
   {
      // Arrange.
      var sut = new SePersonnummer(personnummer);

      // Act/assert.
      sut.Value.Should().Be(personnummer);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var personnummer = Valid11CharacterDashPersonnummer;
      var sut = new SePersonnummer(personnummer);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(personnummer);
   }

   [Fact]
   public void SePersonnummer_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var personnummer = Valid11CharacterDashPersonnummer;
      var sut = new SePersonnummer(personnummer);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(personnummer);
   }

   [Fact]
   public void SePersonnummer_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      SePersonnummer personnummer = null!;

      // Act.
      String str = personnummer;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void SePersonnummer_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      SePersonnummer personnummer = null!;

      // Act.
      String str = personnummer;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldCreateInstance_WhenValueIsValid(String personnummer)
   {
      // Act.
      var sut = (SePersonnummer)personnummer;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(personnummer);
   }

   [Theory]
   [MemberData(nameof(ValidPersonnummerDateOfBirthValues))]
   [MemberData(nameof(ValidSamordningsnummerDateOfBirthValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldCreateInstance_WhenDateOfBirthIsValid(String dateOfBirth)
   {
      // Act.
      var personnummer = GetPersonnummerWithValidCheckDigit(dateOfBirth: dateOfBirth);
      var sut = (SePersonnummer)personnummer;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(personnummer);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String personnummer)
      => FluentActions
         .Invoking(() => _ = (SePersonnummer)personnummer)
         .Should().Throw<KfValidationException<SePersonnummerValidationResult>>()
         .WithMessage(Messages.SePersonnummerEmpty + "*")
         .And.ValidationResult.Should().Be(SePersonnummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String personnummer)
      => FluentActions
         .Invoking(() => _ = (SePersonnummer)personnummer)
         .Should().Throw<KfValidationException<SePersonnummerValidationResult>>()
         .WithMessage(Messages.SePersonnummerInvalidLength + "*")
         .And.ValidationResult.Should().Be(SePersonnummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSixDigitPersonnummerDateOfBirthValues))]
   [MemberData(nameof(InvalidEightDigitPersonnummerDateOfBirthValues))]
   [MemberData(nameof(InvalidSixDigitSamordningsnummerDateOfBirthValues))]
   [MemberData(nameof(InvalidEightDigitSamordningsnummerDateOfBirthValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(String dateOfBirth)
   {
      // Arrange.
      var personnummer = GetPersonnummer(dateOfBirth: dateOfBirth);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)personnummer)
         .Should().Throw<KfValidationException<SePersonnummerValidationResult>>()
         .WithMessage(Messages.SePersonnummerInvalidDateOfBirth + "*")
         .And.ValidationResult.Should().Be(SePersonnummerValidationResult.InvalidDateOfBirth);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String personnummer)
      => FluentActions
         .Invoking(() => _ = (SePersonnummer)personnummer)
         .Should().Throw<KfValidationException<SePersonnummerValidationResult>>()
         .WithMessage(Messages.SePersonnummerInvalidSeparator + "*")
         .And.ValidationResult.Should().Be(SePersonnummerValidationResult.InvalidSeparator);

   [Theory]
   [MemberData(nameof(InvalidBirthSerialNumberValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidBirthSerialNumber(String birthSerialNumber)
   {
      // Arrange.
      var dateOfBirth = "20010616";
      var personnummer = GetPersonnummer(
         dateOfBirth: dateOfBirth,
         birthSerialNumber: birthSerialNumber);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)personnummer)
         .Should().Throw<KfValidationException<SePersonnummerValidationResult>>()
         .WithMessage(Messages.SePersonnummerInvalidBirthSerialNumber + "*")
         .And.ValidationResult.Should().Be(SePersonnummerValidationResult.InvalidBirthSerialNumber);
   }

   [Theory]
   [MemberData(nameof(UndetectableCheckDigitErrors))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldCreateInstance_WhenCheckDigitHasUndetectableError(String personnummer)
   {
      // Act.
      var sut = (SePersonnummer)personnummer;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(personnummer);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String personnummer)
      => FluentActions
         .Invoking(() => _ = (SePersonnummer)personnummer)
         .Should().Throw<KfValidationException<SePersonnummerValidationResult>>()
         .WithMessage(Messages.SePersonnummerInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(SePersonnummerValidationResult.InvalidCheckDigit);

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var personnummer1 = new SePersonnummer(Valid11CharacterDashSamordningsnummer);
      var personnummer2 = new SePersonnummer(Valid11CharacterDashSamordningsnummer);

      // Act/assert.
      (personnummer1 == personnummer2).Should().BeTrue();
   }

   [Fact]
   public void SePersonnummer_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var personnummer1 = new SePersonnummer(Valid13CharacterDashPersonnummer);
      var personnummer2 = new SePersonnummer(Valid13CharacterDashSamordningsnummer);

      // Act/assert.
      (personnummer1 == personnummer2).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_EqualityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still not be equal.
      var personnummer1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var personnummer2 = new SePersonnummer("19" + Valid11CharacterDashPersonnummer);

      // Act/assert.
      (personnummer1 == personnummer2).Should().BeFalse();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var personnummer1 = new SePersonnummer(Valid13CharacterDashPersonnummer);
      var personnummer2 = new SePersonnummer(Valid13CharacterDashSamordningsnummer);

      // Act/assert.
      (personnummer1 != personnummer2).Should().BeTrue();
   }

   [Fact]
   public void SePersonnummer_InequalityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still not be equal.
      var personnummer1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var personnummer2 = new SePersonnummer("19" + Valid11CharacterDashPersonnummer);

      // Act/assert.
      (personnummer1 != personnummer2).Should().BeTrue();
   }

   [Fact]
   public void SePersonnummer_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var personnummer1 = new SePersonnummer(Valid11CharacterDashSamordningsnummer);
      var personnummer2 = new SePersonnummer(Valid11CharacterDashSamordningsnummer);

      // Act/assert.
      (personnummer1 != personnummer2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_Create_ShouldCreateInstance_WhenValueIsValid(String personnummer)
   {
      // Arrange.
      var expectedValue = new SePersonnummer(personnummer);

      // Act.
      var result = SePersonnummer.Create(personnummer);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(ValidPersonnummerDateOfBirthValues))]
   [MemberData(nameof(ValidSamordningsnummerDateOfBirthValues))]
   public void SePersonnummer_Create_ShouldCreateInstance_WhenDateOfBirthIsValid(String dateOfBirth)
   {
      // Arrange.
      var personnummer = GetPersonnummerWithValidCheckDigit(dateOfBirth: dateOfBirth);
      var expectedValue = new SePersonnummer(personnummer);

      // Act.
      var result = SePersonnummer.Create(personnummer);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SePersonnummer_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String personnummer)
   {
      // Act.
      var result = SePersonnummer.Create(personnummer);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(SePersonnummerValidationResult.Empty);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String personnummer)
   {
      // Act.
      var result = SePersonnummer.Create(personnummer);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(SePersonnummerValidationResult.InvalidLength);
   }

   [Theory]
   [MemberData(nameof(InvalidSixDigitPersonnummerDateOfBirthValues))]
   [MemberData(nameof(InvalidEightDigitPersonnummerDateOfBirthValues))]
   [MemberData(nameof(InvalidSixDigitSamordningsnummerDateOfBirthValues))]
   [MemberData(nameof(InvalidEightDigitSamordningsnummerDateOfBirthValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(String dateOfBirth)
   {
      // Arrange.
      var personnummer = GetPersonnummer(dateOfBirth: dateOfBirth);

      // Act.
      var result = SePersonnummer.Create(personnummer);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(SePersonnummerValidationResult.InvalidDateOfBirth);
   }

   [Theory]
   [MemberData(nameof(InvalidBirthSerialNumberValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidBirthSerialNumberValidationResult_WhenValueHasInvalidBirthSerialNumber(String birthSerialNumber)
   {
      // Arrange.
      var personnummer = GetPersonnummer(birthSerialNumber: birthSerialNumber);

      // Act.
      var result = SePersonnummer.Create(personnummer);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(SePersonnummerValidationResult.InvalidBirthSerialNumber);
   }

   [Theory]
   [MemberData(nameof(UndetectableCheckDigitErrors))]
   public void SePersonnummer_Create_ShouldCreateInstance_WhenValueHasUndetectableCheckDigitError(String personnummer)
   {
      // Arrange.
      var expectedValue = new SePersonnummer(personnummer);

      // Act.
      var result = SePersonnummer.Create(personnummer);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidCheckDigitValidationResult_WhenValueHasInvalidCheckDigit(String personnummer)
   {
      // Act.
      var result = SePersonnummer.Create(personnummer);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(SePersonnummerValidationResult.InvalidCheckDigit);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var personnummer1 = new SePersonnummer(Valid13CharacterPlusPersonnummer);
      var personnummer2 = new SePersonnummer(Valid13CharacterPlusPersonnummer);

      // Act/assert.
      personnummer1.Equals(personnummer2).Should().BeTrue();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var personnummer1 = new SePersonnummer(Valid13CharacterPlusPersonnummer);     // +
      var personnummer2 = new SePersonnummer(Valid13CharacterDashPersonnummer);     // -

      // Act/assert.
      personnummer1.Equals(personnummer2).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still not be equal.
      var personnummer1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var personnummer2 = new SePersonnummer("19" + Valid11CharacterDashPersonnummer);

      // Act/assert.
      personnummer1.Equals(personnummer2).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new SePersonnummer(Valid13CharacterPlusPersonnummer);

      // Act/assert.
      sut.Equals(Valid13CharacterPlusPersonnummer).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new SePersonnummer(Valid13CharacterPlusPersonnummer);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var personnummer1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var personnummer2 = new SePersonnummer(Valid11CharacterDashPersonnummer);

      // Act.
      var hash1 = personnummer1.GetHashCode();
      var hash2 = personnummer2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void SePersonnummer_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var personnummer1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var personnummer2 = new SePersonnummer(Valid11CharacterDashSamordningsnummer);

      // Act.
      var hash1 = personnummer1.GetHashCode();
      var hash2 = personnummer2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void SePersonnummer_GetHashCode_ShouldReturnDifferentValues_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still not be equal.
      var personnummer1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var personnummer2 = new SePersonnummer("19" + Valid11CharacterDashPersonnummer);

      // Act.
      var hash1 = personnummer1.GetHashCode();
      var hash2 = personnummer2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   #endregion

   #region ReferenceEquals Method Tests
   // ==========================================================================
   // ==========================================================================

   // SePersonnummer does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void SePersonnummer_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var personnummer1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var personnummer2 = new SePersonnummer(Valid11CharacterDashPersonnummer);

      // Act/assert.
      (personnummer1 == personnummer2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(personnummer1, personnummer2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_ToString_ShouldReturnExpectedValue(String personnummer)
   {
      // Arrange.
      var sut = new SePersonnummer(personnummer);

      // Act/assert.
      sut.ToString().Should().Be(personnummer);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String personnummer)
      => SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(ValidPersonnummerDateOfBirthValues))]
   [MemberData(nameof(ValidSamordningsnummerDateOfBirthValues))]
   public void SePersonnummer_Validate_ShouldReturnValidationPassed_WhenDateOfBirthIsValid(String dateOfBirth)
   {
      // Arrange.
      var personnummer = GetPersonnummerWithValidCheckDigit(dateOfBirth: dateOfBirth);

      // Act/assert.
      SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.ValidationPassed);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SePersonnummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String personnummer)
      => SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String personnummer)
      => SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSixDigitPersonnummerDateOfBirthValues))]
   [MemberData(nameof(InvalidEightDigitPersonnummerDateOfBirthValues))]
   [MemberData(nameof(InvalidSixDigitSamordningsnummerDateOfBirthValues))]
   [MemberData(nameof(InvalidEightDigitSamordningsnummerDateOfBirthValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(String dateOfBirth)
   {
      // Arrange.
      var personnummer = GetPersonnummer(dateOfBirth: dateOfBirth);

      // Act/assert.
      SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.InvalidDateOfBirth);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String personnummer)
      => SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.InvalidSeparator);

   [Theory]
   [MemberData(nameof(InvalidBirthSerialNumberValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidBirthSerialNumber_WhenValueHasInvalidBirthSerialNumber(String birthSerialNumber)
   {
      // Arrange.
      var personnummer = GetPersonnummer(birthSerialNumber: birthSerialNumber);

      // Act/assert.
      SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.InvalidBirthSerialNumber);
   }

   [Theory]
   [MemberData(nameof(UndetectableCheckDigitErrors))]
   public void SePersonnummer_Validate_ShouldReturnValidationPassed_WhenValueHasUndetectableCheckDigitError(String personnummer)
      => SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String personnummer)
      => SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.InvalidCheckDigit);

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new SePersonnummer(Valid11CharacterDashPersonnummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<SePersonnummer>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void SePersonnummer_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new SePersonnummer(Valid11CharacterDashSamordningsnummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{Valid11CharacterDashSamordningsnummer}\"");  // Simple string, not object
   }

   public class Foo
   {
      public SePersonnummer Personnummer { get; set; } = null!;
   }

   [Fact]
   public void SePersonnummer_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Personnummer = new SePersonnummer(Valid13CharacterDashPersonnummer) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void SePersonnummer_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Personnummer\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void SePersonnummer_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Personnummer\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Personnummer.Should().BeNull();
   }

   [Fact]
   public void SePersonnummer_JsonDeserialization_ShouldThrowKfValidationException_WhenPersonnummerIsInvalid()
   {
      // Arrange.
      var json = "{\"Personnummer\":\"811228-987\"}";  // Invalid length

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should()
         .ThrowExactly<KfValidationException<SePersonnummerValidationResult>>()
         .WithMessage(Messages.SePersonnummerInvalidLength + "*")
         .And.ValidationResult.Should().Be(SePersonnummerValidationResult.InvalidLength);
   }

   #endregion
}
