// Ignore Spelling: Deserialize Deserialization Dk Json Kf Personnummer

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.National.Europe.DkPersonnummer,
   KfAccountNumbers.National.Europe.DkPersonnummer.ValidationError>;
using LocalValidationError = KfAccountNumbers.National.Europe.DkPersonnummer.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.National.Europe.DkPersonnummer.ValidationError>;
using LocalValidationResult = KfAccountNumbers.National.Europe.DkPersonnummer.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class DkPersonnummerTests
{
   private const String ValidUnformattedPersonnummer = "0707614285";
   private const String ValidFormattedPersonnummer = "070761-4285";
   private const String AltValidUnformattedPersonnummer = "0102036234";
   private const String AltValidFormattedPersonnummer = "010203-6234";

   private static String GetRawPersonnummer(String value)
      => value.Length == 10
         ? value
         : value[..6] + value[7..];

   public static TheoryData<String> ValidPersonnummerValues =>
   [
      ValidUnformattedPersonnummer,
      ValidFormattedPersonnummer,
      AltValidUnformattedPersonnummer,
      AltValidFormattedPersonnummer,
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

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted values
      { ".707614285", 0 },          // Non-digit character '.'
      { "0 07614285", 1 },          // Non-digit character ' '
      { "07A7614285", 2 },          // Non-digit character 'A'
      { "070Z614285", 3 },          // Non-digit character 'Z'
      { "0707^14285", 4 },          // Non-digit character '^'
      { "07076a4285", 5 },          // Non-digit character 'a'
      { "070761z285", 6 },          // Non-digit character 'z'
      { "0707614~85", 7 },          // Non-digit character '~'
      { "07076142\u21535", 8 },     // Non-digit character Unicode fraction 1/3
      { "070761428\u00D6", 9 },     // Invalid character unicode O with umlaut

      // Formatted values
      { ".70761 4285", 0 },         // Non-digit character '.'
      { "0 0761 4285", 1 },         // Non-digit character ' '
      { "07A761 4285", 2 },         // Non-digit character 'A'
      { "070Z61 4285", 3 },         // Non-digit character 'Z'
      { "0707^1 4285", 4 },         // Non-digit character '^'
      { "07076a 4285", 5 },         // Non-digit character 'a'
      { "070761 z285", 7 },         // Non-digit character 'z'
      { "070761-4~85", 8 },         // Non-digit character '~'
      { "070761-42\u21535", 9 },    // Non-digit character Unicode fraction 1/3
      { "070761-428\u00D6", 10 },   // Invalid character unicode O with umlaut
   };

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

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.DkPersonnummerInvalidLength,
         value.Length,
         DkPersonnummer.GetValidLengthDefinitions());

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.DkPersonnummerInvalidCharacter,
         value[position],
         position);

   private static InvalidSeparator GetInvalidSeparatorResult(String value)
      => new(Messages.DkPersonnummerInvalidSeparator, value[6], 6);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(Messages.DkPersonnummerInvalidDateOfBirth, value[..6], DateFormatName.DDMMYY);

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
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new DkPersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DkPersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new DkPersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DkPersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new DkPersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DkPersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new DkPersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void DkPersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new DkPersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

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
         CultureInfo.InvariantCulture);

      // Act/assert.
      sut.DateOfBirth.Should().Be(expected);
   }

   #endregion

   #region Gender Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidUnformattedPersonnummer, '1')]
   [InlineData(ValidUnformattedPersonnummer, '3')]
   [InlineData(ValidUnformattedPersonnummer, '5')]
   [InlineData(ValidUnformattedPersonnummer, '7')]
   [InlineData(ValidUnformattedPersonnummer, '9')]
   [InlineData(ValidFormattedPersonnummer, '1')]
   [InlineData(ValidFormattedPersonnummer, '3')]
   [InlineData(ValidFormattedPersonnummer, '5')]
   [InlineData(ValidFormattedPersonnummer, '7')]
   [InlineData(ValidFormattedPersonnummer, '9')]
   public void DkPersonnummer_Gender_ShouldReturnMale_ForValuesWithOddGenderIndicator(
      String value,
      Char gender)
   {
      // Arrange.
      value = $"{value[..^1]}{gender}";
      var sut = new DkPersonnummer(value);
      Gender.BinaryGender expected = default(Gender.Male);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData(ValidUnformattedPersonnummer, '0')]
   [InlineData(ValidUnformattedPersonnummer, '2')]
   [InlineData(ValidUnformattedPersonnummer, '4')]
   [InlineData(ValidUnformattedPersonnummer, '6')]
   [InlineData(ValidUnformattedPersonnummer, '8')]
   [InlineData(ValidFormattedPersonnummer, '0')]
   [InlineData(ValidFormattedPersonnummer, '2')]
   [InlineData(ValidFormattedPersonnummer, '4')]
   [InlineData(ValidFormattedPersonnummer, '6')]
   [InlineData(ValidFormattedPersonnummer, '8')]
   public void DkPersonnummer_Gender_ShouldReturnFemale_ForValuesWithEvenGenderIndicator(
      String value,
      Char gender)
   {
      // Arrange.
      value = $"{value[..^1]}{gender}";
      var sut = new DkPersonnummer(value);
      Gender.BinaryGender expected = default(Gender.Female);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void DkPersonnummer_Value_ShouldReturnValidatedPersonnummer(String value)
   {
      // Arrange.
      var sut = new DkPersonnummer(value);
      var expected = GetRawPersonnummer(value);

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
      var value = ValidUnformattedPersonnummer;
      var sut = new DkPersonnummer(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void DkPersonnummer_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedPersonnummer;
      var sut = new DkPersonnummer(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void DkPersonnummer_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      DkPersonnummer sut = null!;

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void DkPersonnummer_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      DkPersonnummer sut = null!;

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new DkPersonnummer(value);

      // Act.
      var sut = (DkPersonnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldCreateInstance_WhenValueHasValidDateOfBirth(String value)
   {
      // Arrange.
      var expected = new DkPersonnummer(value);

      // Act.
      var sut = (DkPersonnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (DkPersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (DkPersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (DkPersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (DkPersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void DkPersonnummer_ExplicitCastToDkPersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (DkPersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DkPersonnummer_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(ValidUnformattedPersonnummer);
      var sut2 = new DkPersonnummer(ValidUnformattedPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void DkPersonnummer_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(ValidUnformattedPersonnummer);
      var sut2 = new DkPersonnummer(AltValidUnformattedPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void DkPersonnummer_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 11 character versions for same person should still be equal.
      var sut1 = new DkPersonnummer(ValidUnformattedPersonnummer);
      var sut2 = new DkPersonnummer(ValidFormattedPersonnummer);

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
      var sut1 = new DkPersonnummer(ValidUnformattedPersonnummer);
      var sut2 = new DkPersonnummer(AltValidUnformattedPersonnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void DkPersonnummer_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 11 character versions for same person should still be equal.
      var sut1 = new DkPersonnummer(ValidUnformattedPersonnummer);
      var sut2 = new DkPersonnummer(ValidFormattedPersonnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void DkPersonnummer_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(AltValidUnformattedPersonnummer);
      var sut2 = new DkPersonnummer(AltValidUnformattedPersonnummer);

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
      LocalCreateResult expected = new DkPersonnummer(value);

      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void DkPersonnummer_Create_ShouldCreateInstance_WhenValueHasValidDateOfBirth(String value)
   {
      // Arrange.
      LocalCreateResult expected = new DkPersonnummer(value);

      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void DkPersonnummer_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DkPersonnummer_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DkPersonnummer_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DkPersonnummer_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value);

      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void DkPersonnummer_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = DkPersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DkPersonnummer_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(ValidUnformattedPersonnummer);
      var sut2 = new DkPersonnummer(ValidUnformattedPersonnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void DkPersonnummer_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new DkPersonnummer(ValidUnformattedPersonnummer);
      var sut2 = new DkPersonnummer(AltValidUnformattedPersonnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void DkPersonnummer_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 10 and 11 character versions for same person should still be equal.
      var sut1 = new DkPersonnummer(ValidUnformattedPersonnummer);
      var sut2 = new DkPersonnummer(ValidFormattedPersonnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void DkPersonnummer_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new DkPersonnummer(ValidFormattedPersonnummer);

      // Act/assert.
      sut.Equals(ValidFormattedPersonnummer).Should().BeFalse();
   }

   [Fact]
   public void DkPersonnummer_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new DkPersonnummer(ValidFormattedPersonnummer);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DkPersonnummer_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new DkPersonnummer(ValidUnformattedPersonnummer);
      var expected = ValidFormattedPersonnummer;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void DkPersonnummer_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new DkPersonnummer(ValidUnformattedPersonnummer);
      var mask = "__________";
      var expected = ValidUnformattedPersonnummer;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void DkPersonnummer_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new DkPersonnummer(ValidUnformattedPersonnummer);
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
      var sut = new DkPersonnummer(ValidUnformattedPersonnummer);
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
      var sut1 = new DkPersonnummer(ValidUnformattedPersonnummer);
      var sut2 = new DkPersonnummer(ValidUnformattedPersonnummer);

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
      var sut1 = new DkPersonnummer(ValidUnformattedPersonnummer);
      var sut2 = new DkPersonnummer(AltValidUnformattedPersonnummer);

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
      var sut1 = new DkPersonnummer(ValidUnformattedPersonnummer);
      var sut2 = new DkPersonnummer(ValidFormattedPersonnummer);

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
      var sut1 = new DkPersonnummer(ValidUnformattedPersonnummer);
      var sut2 = new DkPersonnummer(ValidUnformattedPersonnummer);

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
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = DkPersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void DkPersonnummer_Validate_ShouldReturnValidationPassed_WhenDateOfBirthIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = DkPersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void DkPersonnummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = DkPersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DkPersonnummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = DkPersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DkPersonnummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = DkPersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DkPersonnummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value);

      // Act.
      var result = DkPersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void DkPersonnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = DkPersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void DkPersonnummer_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new DkPersonnummer(ValidUnformattedPersonnummer);

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
      var sut = new DkPersonnummer(AltValidFormattedPersonnummer);
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
      var foo = new Foo { Personnummer = new DkPersonnummer(ValidUnformattedPersonnummer) };
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
      var json = "{\"Personnummer\":\"070761 4285\"}";  // Invalid separator
      LocalValidationError expected = GetInvalidSeparatorResult("070761 4285");

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
