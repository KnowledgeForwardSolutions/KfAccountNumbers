// Ignore Spelling: Deserialize Deserialization Dk Json Kf Personnummer

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class DkPersonnummerTests
{
   private const String Valid10CharacterPersonnummer = "0707614285";
   private const String Valid11CharacterPersonnummer = "070761-4285";
   private const String AltValid10CharacterPersonnummer = "0102036234";
   private const String AltValid11CharacterPersonnummer = "010203-6234";

   private static String GetRawPersonnummer(String personnummer)
      => personnummer.Length == 10
         ? personnummer
         : personnummer[..6] + personnummer[7..];

   public static TheoryData<String> ValidPersonnummerValues =>
   [
      Valid10CharacterPersonnummer,
      Valid11CharacterPersonnummer,
      AltValid10CharacterPersonnummer,
      AltValid11CharacterPersonnummer,
   ];

   public static TheoryData<String> ValidDateOfBirthValues =>
   [
      "010100-0123",          // January 1, 1900
      "311299-0123",          // December 31, 1999

      "010100-1123",          // January 1, 1900
      "311299-1123",          // December 31, 1999

      "010100-2123",          // January 1, 1900
      "311299-2123",          // December 31, 1999

      "010100-3123",          // January 1, 1900
      "311299-3123",          // December 31, 1999

      "010100-4123",          // January 1, 2000
      "311236-4123",          // December 31, 2036
      "010137-4123",          // January 1, 1937 - note century break between 36 and 37
      "311299-4123",          // December 31, 1999

      "010100-5123",          // January 1, 2000
      "311257-5123",          // December 31, 2057
      "010158-5123",          // January 1, 1858 - note century break between 57 and 58
      "311299-5123",          // December 31, 1899

      "010100-6123",          // January 1, 2000
      "311257-6123",          // December 31, 2057
      "010158-6123",          // January 1, 1858 - note century break between 57 and 58
      "311299-6123",          // December 31, 1899

      "010100-7123",          // January 1, 2000
      "311257-7123",          // December 31, 2057
      "010158-7123",          // January 1, 1858 - note century break between 57 and 58
      "311299-7123",          // December 31, 1899

      "010100-8123",          // January 1, 2000
      "311257-8123",          // December 31, 2057
      "010158-8123",          // January 1, 1858 - note century break between 57 and 58
      "311299-8123",          // December 31, 1899

      "010100-9123",          // January 1, 2000
      "311236-9123",          // December 31, 2036
      "010137-9123",          // January 1, 1937 - note century break between 36 and 37
      "311299-9123",          // December 31, 1999

      "2902040123",           // February 29, leap year
      "2902041123",           // February 29, leap year
      "2902042123",           // February 29, leap year
      "2902043123",           // February 29, leap year
      "2902044123",           // February 29, leap year
      "2902045123",           // February 29, leap year
      "2902046123",           // February 29, leap year
      "2902047123",           // February 29, leap year
      "2902048123",           // February 29, leap year
      "2902049123",           // February 29, leap year

      "2902004123",           // February 29, 2000 (leap year because of century divisible by 400 rule)
      "2902005123",           // February 29, 2000
      "2902006123",           // February 29, 2000
      "2902007123",           // February 29, 2000
      "2902008123",           // February 29, 2000
      "2902009123",           // February 29, 2000
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "070761428",        // Length 9
      "070761-42856",     // Length 13
      new String('1', 100) // Very long string
   ];

   public static TheoryData<String> InvalidCharacterValues =>
   [
      "A707614285",           // Non-digit character 'A'
      "0 07614285",           // Non-digit character ' '
      "07-7614285",           // Non-digit character '-'
      "070=614285",           // Non-digit character '='
      "0707B14285",           // Non-digit character 'B'
      "07076C4285",           // Non-digit character 'C'
      "070761a285",           // Non-digit character 'a'
      "0707614b85",           // Non-digit character 'b'
      "07076142~5",           // Non-digit character '~'
      "070761428\u2153",      // Non-digit character Unicode fraction 1/3

      "A70761-4285",          // Non-digit character 'A'
      "0 0761-4285",          // Non-digit character ' '
      "07-761-4285",          // Non-digit character '-'
      "070=61-4285",          // Non-digit character '='
      "0707B1-4285",          // Non-digit character 'B'
      "07076C-4285",          // Non-digit character 'C'
      "070761-a285",          // Non-digit character 'a'
      "070761-4b85",          // Non-digit character 'b'
      "070761-42~5",          // Non-digit character '~'
      "070761-428\u2153",     // Non-digit character Unicode fraction 1/3
   ];

   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "070761 4285",
      "070761=4285",
      "070761~4285",
      "070761\u21534285",
   ];

   public static TheoryData<String> InvalidDateOfBirthValues =>
   [
      // Given the century indicator rules (0-9), dates outside 1858-2057
      // are impossible to represent without violating century indicator constraints.
      "0100000112",        // Invalid month = 0
      "0113000112",        // Invalid month = 13
      "000100-0112",       // Invalid day = 0

      "3201040112",        // Invalid day of month for January, any year
      "2902031112",        // Invalid day of month for February, non-leap year
      "3002042112",        // Invalid day of month for February, leap year
      "3002004112",        // Invalid day of month for February, leap year (2000 is leap-year)
      "7203043112",        // Invalid day of for March, any year
      "7104045112",        // Invalid day of for April, any year
      "7205046112",        // Invalid day of for May, any year
      "7106047112",        // Invalid day of for June, any year
      "720704-8112",       // Invalid day of for July, any year
      "720804-9112",       // Invalid day of for August, any year
      "710904-0112",       // Invalid day of for September, any year
      "721004-1112",       // Invalid day of for October, any year
      "711104-2112",       // Invalid day of for November, any year
      "721204-3112",       // Invalid day of for December, any year
   ];

   #region Constants Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DkPersonnummer_MinimumValidYearOfBirth_ShouldHaveExpectedValue()
      => DkPersonnummer.MinimumValidYearOfBirth.Should().Be(1858);

   [Fact]
   public void DkPersonnummer_MaximumValidYearOfBirth_ShouldHaveExpectedValue()
      => DkPersonnummer.MaximumValidYearOfBirth.Should().Be(2057);

   #endregion

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void DkPersonnummer_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawPersonnummer(value);

      // Act.
      var sut = new DkPersonnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void DkPersonnummer_Constructor_ShouldCreateInstance_WhenValueHasValidDateOfBirth(String value)
   {
      // Arrange.
      var expected = GetRawPersonnummer(value);

      // Act.
      var sut = new DkPersonnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void DkPersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
      => FluentActions
         .Invoking(() => new DkPersonnummer(value))
         .Should().Throw<KfValidationException<DkPersonnummerValidationResult>>()
         .WithMessage(Messages.DkPersonnummerEmpty + "*")
         .And.ValidationResult.Should().Be(DkPersonnummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DkPersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
      => FluentActions
         .Invoking(() => new DkPersonnummer(value))
         .Should().Throw<KfValidationException<DkPersonnummerValidationResult>>()
         .WithMessage(Messages.DkPersonnummerInvalidLength + "*")
         .And.ValidationResult.Should().Be(DkPersonnummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DkPersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(String value)
      => FluentActions
         .Invoking(() => new DkPersonnummer(value))
         .Should().Throw<KfValidationException<DkPersonnummerValidationResult>>()
         .WithMessage(Messages.DkPersonnummerInvalidCharacter + "*")
         .And.ValidationResult.Should().Be(DkPersonnummerValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DkPersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String value)
      => FluentActions
         .Invoking(() => new DkPersonnummer(value))
         .Should().Throw<KfValidationException<DkPersonnummerValidationResult>>()
         .WithMessage(Messages.DkPersonnummerInvalidSeparator + "*")
         .And.ValidationResult.Should().Be(DkPersonnummerValidationResult.InvalidSeparator);

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void DkPersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(String value)
      => FluentActions
         .Invoking(() => new DkPersonnummer(value))
         .Should().Throw<KfValidationException<DkPersonnummerValidationResult>>()
         .WithMessage(Messages.DkPersonnummerInvalidDateOfBirth + "*")
         .And.ValidationResult.Should().Be(DkPersonnummerValidationResult.InvalidDateOfBirth);

   #endregion

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("010100-0123", "19000101")]      // January 1, 1900
   [InlineData("311299-0123", "19991231")]      // December 31, 1999

   [InlineData("010100-1123", "19000101")]      // January 1, 1900
   [InlineData("311299-1123", "19991231")]      // December 31, 1999

   [InlineData("010100-2123", "19000101")]      // January 1, 1900
   [InlineData("311299-2123", "19991231")]      // December 31, 1999

   [InlineData("010100-3123", "19000101")]      // January 1, 1900
   [InlineData("311299-3123", "19991231")]      // December 31, 1999

   [InlineData("010100-4123", "20000101")]      // January 1, 2000
   [InlineData("311236-4123", "20361231")]      // December 31, 2036
   [InlineData("010137-4123", "19370101")]      // January 1, 1937 - note century break between 36 and 37
   [InlineData("311299-4123", "19991231")]      // December 31, 1999

   [InlineData("010100-5123", "20000101")]      // January 1, 2000
   [InlineData("311257-5123", "20571231")]      // December 31, 2057
   [InlineData("010158-5123", "18580101")]      // January 1, 1858 - note century break between 57 and 58
   [InlineData("311299-5123", "18991231")]      // December 31, 1899

   [InlineData("010100-6123", "20000101")]      // January 1, 2000
   [InlineData("311257-6123", "20571231")]      // December 31, 2057
   [InlineData("010158-6123", "18580101")]      // January 1, 1858 - note century break between 57 and 58
   [InlineData("311299-6123", "18991231")]      // December 31, 1899

   [InlineData("010100-7123", "20000101")]      // January 1, 2000
   [InlineData("311257-7123", "20571231")]      // December 31, 2057
   [InlineData("010158-7123", "18580101")]      // January 1, 1858 - note century break between 57 and 58
   [InlineData("311299-7123", "18991231")]      // December 31, 1899

   [InlineData("010100-8123", "20000101")]      // January 1, 2000
   [InlineData("311257-8123", "20571231")]      // December 31, 2057
   [InlineData("010158-8123", "18580101")]      // January 1, 1858 - note century break between 57 and 58
   [InlineData("311299-8123", "18991231")]      // December 31, 1899

   [InlineData("010100-9123", "20000101")]      // January 1, 2000
   [InlineData("311236-9123", "20361231")]      // December 31, 2036
   [InlineData("010137-9123", "19370101")]      // January 1, 1937 - note century break between 36 and 37
   [InlineData("311299-9123", "19991231")]      // December 31, 1999

   [InlineData("2902040123", "19040229")]       // February 29, leap year
   [InlineData("2902041123", "19040229")]       // February 29, leap year
   [InlineData("2902042123", "19040229")]       // February 29, leap year
   [InlineData("2902043123", "19040229")]       // February 29, leap year
   [InlineData("2902044123", "20040229")]       // February 29, leap year
   [InlineData("2902045123", "20040229")]       // February 29, leap year
   [InlineData("2902046123", "20040229")]       // February 29, leap year
   [InlineData("2902047123", "20040229")]       // February 29, leap year
   [InlineData("2902048123", "20040229")]       // February 29, leap year
   [InlineData("2902049123", "20040229")]       // February 29, leap year

   [InlineData("2902004123", "20000229")]       // February 29, 2000 (leap year because of century divisible by 400 rule)
   [InlineData("2902005123", "20000229")]       // February 29, 2000
   [InlineData("2902006123", "20000229")]       // February 29, 2000
   [InlineData("2902007123", "20000229")]       // February 29, 2000
   [InlineData("2902008123", "20000229")]       // February 29, 2000
   [InlineData("2902009123", "20000229")]       // February 29, 2000
   public void DkPersonnummer_DateOfBirth_ShouldReturnExpectedValue(
      String value,
      String expectedDateOfBirth)
   {
      // Arrange.
      var sut = new DkPersonnummer(value);
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
   public void DkPersonnummer_Gender_ShouldReturnExpectedValue(
      Char genderChar,
      BinaryGender expectedGender)
   {
      // Arrange.
      var personnummer = $"{Valid10CharacterPersonnummer[..9]}{genderChar}";
      var sut = new DkPersonnummer(personnummer);

      // Act/assert.
      sut.Gender.Should().Be(expectedGender);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(Valid10CharacterPersonnummer, Valid10CharacterPersonnummer)]
   [InlineData(Valid11CharacterPersonnummer, Valid10CharacterPersonnummer)]
   [InlineData(AltValid10CharacterPersonnummer, AltValid10CharacterPersonnummer)]
   [InlineData(AltValid11CharacterPersonnummer, AltValid10CharacterPersonnummer)]
   public void DkPersonnummer_Value_ShouldReturnValidatedPersonnummer(
      String value,
      String expected)
   {
      // Arrange.
      var sut = new DkPersonnummer(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DkPersonnummer_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = Valid10CharacterPersonnummer;
      var sut = new DkPersonnummer(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(value);
   }

   [Fact]
   public void DkPersonnummer_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = Valid11CharacterPersonnummer;
      var sut = new DkPersonnummer(value);
      var expected = GetRawPersonnummer(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Fact]
   public void DkPersonnummer_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      DkPersonnummer personnummer = null!;

      // Act.
      String str = personnummer;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void DkPersonnummer_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      DkPersonnummer personnummer = null!;

      // Act.
      var str = (String)personnummer;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawPersonnummer(value);

      // Act.
      var sut = (DkPersonnummer)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldCreateInstance_WhenValueHasValidDateOfBirth(String value)
   {
      // Arrange.
      var expected = GetRawPersonnummer(value);

      // Act.
      var sut = (DkPersonnummer)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
      => FluentActions
         .Invoking(() => _ = (DkPersonnummer)value)
         .Should().Throw<KfValidationException<DkPersonnummerValidationResult>>()
         .WithMessage(Messages.DkPersonnummerEmpty + "*")
         .And.ValidationResult.Should().Be(DkPersonnummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
      => FluentActions
         .Invoking(() => _ = (DkPersonnummer)value)
         .Should().Throw<KfValidationException<DkPersonnummerValidationResult>>()
         .WithMessage(Messages.DkPersonnummerInvalidLength + "*")
         .And.ValidationResult.Should().Be(DkPersonnummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidNonDigitCharacter(String value)
      => FluentActions
         .Invoking(() => _ = (DkPersonnummer)value)
         .Should().Throw<KfValidationException<DkPersonnummerValidationResult>>()
         .WithMessage(Messages.DkPersonnummerInvalidCharacter + "*")
         .And.ValidationResult.Should().Be(DkPersonnummerValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String value)
      => FluentActions
         .Invoking(() => _ = (DkPersonnummer)value)
         .Should().Throw<KfValidationException<DkPersonnummerValidationResult>>()
         .WithMessage(Messages.DkPersonnummerInvalidSeparator + "*")
         .And.ValidationResult.Should().Be(DkPersonnummerValidationResult.InvalidSeparator);

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(String value)
      => FluentActions
         .Invoking(() => _ = (DkPersonnummer)value)
         .Should().Throw<KfValidationException<DkPersonnummerValidationResult>>()
         .WithMessage(Messages.DkPersonnummerInvalidDateOfBirth + "*")
         .And.ValidationResult.Should().Be(DkPersonnummerValidationResult.InvalidDateOfBirth);

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DkPersonnummer_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(Valid10CharacterPersonnummer);
      var sut2 = new DkPersonnummer(Valid10CharacterPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void DkPersonnummer_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(Valid10CharacterPersonnummer);
      var sut2 = new DkPersonnummer(AltValid10CharacterPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void DkPersonnummer_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 11 character versions for same person should still be equal.
      var sut1 = new DkPersonnummer(Valid10CharacterPersonnummer);
      var sut2 = new DkPersonnummer(Valid11CharacterPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DkPersonnummer_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(Valid10CharacterPersonnummer);
      var sut2 = new DkPersonnummer(AltValid10CharacterPersonnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void DkPersonnummer_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 11 character versions for same person should still be equal.
      var sut1 = new DkPersonnummer(Valid10CharacterPersonnummer);
      var sut2 = new DkPersonnummer(Valid11CharacterPersonnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void DkPersonnummer_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(AltValid10CharacterPersonnummer);
      var sut2 = new DkPersonnummer(AltValid10CharacterPersonnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void DkPersonnummer_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expectedValue = new DkPersonnummer(value);

      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void DkPersonnummer_Create_ShouldCreateInstance_WhenValueHasValidDateOfBirth(String value)
   {
      // Arrange.
      var expectedValue = new DkPersonnummer(value);

      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void DkPersonnummer_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(DkPersonnummerValidationResult.Empty);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DkPersonnummer_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(DkPersonnummerValidationResult.InvalidLength);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DkPersonnummer_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacter(String value)
   {
      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(DkPersonnummerValidationResult.InvalidCharacter);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DkPersonnummer_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(String value)
   {
      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(DkPersonnummerValidationResult.InvalidSeparator);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void DkPersonnummer_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(String value)
   {
      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(DkPersonnummerValidationResult.InvalidDateOfBirth);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DkPersonnummer_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(Valid10CharacterPersonnummer);
      var sut2 = new DkPersonnummer(Valid10CharacterPersonnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void DkPersonnummer_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(Valid10CharacterPersonnummer);
      var sut2 = new DkPersonnummer(AltValid10CharacterPersonnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void DkPersonnummer_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 11 character versions for same person should still be equal.
      var sut1 = new DkPersonnummer(Valid10CharacterPersonnummer);
      var sut2 = new DkPersonnummer(Valid11CharacterPersonnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DkPersonnummer_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new DkPersonnummer(Valid10CharacterPersonnummer);
      var expected = Valid11CharacterPersonnummer;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void DkPersonnummer_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new DkPersonnummer(Valid10CharacterPersonnummer);
      var mask = "__________";
      var expected = Valid10CharacterPersonnummer;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void DkPersonnummer_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new DkPersonnummer(Valid10CharacterPersonnummer);
      String mask = null!;

      // Act/assert.
      FluentActions
         .Invoking(() => _ = sut.Format(mask))
         .Should()
         .ThrowExactly<ArgumentNullException>()
         .WithParameterName(nameof(mask))
         .WithMessage(Messages.FormatMaskEmpty + "*");
   }

   [Theory]
   [InlineData("")]
   [InlineData("\t")]
   public void DkPersonnummer_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new DkPersonnummer(Valid10CharacterPersonnummer);
      var expectedMessage = Messages.FormatMaskEmpty + "*";
      var act = () => _ = sut.Format(mask);

      // Act/assert.
      act.Should().ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(mask))
         .WithMessage(expectedMessage);
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DkPersonnummer_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(Valid10CharacterPersonnummer);
      var sut2 = new DkPersonnummer(Valid10CharacterPersonnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void DkPersonnummer_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(Valid10CharacterPersonnummer);
      var sut2 = new DkPersonnummer(AltValid10CharacterPersonnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void DkPersonnummer_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 11 character versions for same person should still be equal.
      var sut1 = new DkPersonnummer(Valid10CharacterPersonnummer);
      var sut2 = new DkPersonnummer(Valid11CharacterPersonnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   #endregion

   #region ReferenceEquals Method Tests
   // ==========================================================================
   // ==========================================================================

   // DkPersonnummer does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void DkPersonnummer_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(Valid10CharacterPersonnummer);
      var sut2 = new DkPersonnummer(Valid10CharacterPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void DkPersonnummer_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new DkPersonnummer(value);
      var expected = GetRawPersonnummer(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void DkPersonnummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void DkPersonnummer_Validate_ShouldReturnValidationPassed_WhenDateOfBirthIsValid(String value)
      => DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.ValidationPassed);

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void DkPersonnummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DkPersonnummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DkPersonnummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(String value)
      => DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DkPersonnummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String value)
      => DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.InvalidSeparator);

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void DkPersonnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(String value)
      => DkPersonnummer.Validate(value).Should().Be(DkPersonnummerValidationResult.InvalidDateOfBirth);

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DkPersonnummer_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new DkPersonnummer(Valid10CharacterPersonnummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<DkPersonnummer>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void DkPersonnummer_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new DkPersonnummer(AltValid11CharacterPersonnummer);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public DkPersonnummer Personnummer { get; set; } = null!;
   }

   [Fact]
   public void DkPersonnummer_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Personnummer = new DkPersonnummer(Valid10CharacterPersonnummer) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void DkPersonnummer_JsonSerialization_ShouldSerializeNullGracefully()
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
   public void DkPersonnummer_JsonDeserialization_ShouldDeserializeNullGracefully()
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
   public void DkPersonnummer_JsonDeserialization_ShouldThrowKfValidationException_WhenPersonnummerIsInvalid()
   {
      // Arrange.
      var json = "{\"Personnummer\":\"100612-707079\"}";  // Invalid length

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should()
         .ThrowExactly<KfValidationException<DkPersonnummerValidationResult>>()
         .WithMessage(Messages.DkPersonnummerInvalidLength + "*")
         .And.ValidationResult.Should().Be(DkPersonnummerValidationResult.InvalidLength);
   }

   #endregion
}
