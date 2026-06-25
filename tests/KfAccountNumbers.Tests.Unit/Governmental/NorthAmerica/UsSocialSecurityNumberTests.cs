// Ignore Spelling: Deserialization Deserialize Json Kf

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

using LocalCreateResult = KfAccountNumbers.Results.UCreateResult<
   KfAccountNumbers.Governmental.NorthAmerica.UsSocialSecurityNumber,
   KfAccountNumbers.Governmental.NorthAmerica.UsSocialSecurityNumber.ValidationError>;
using LocalValidationError = KfAccountNumbers.Governmental.NorthAmerica.UsSocialSecurityNumber.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.Governmental.NorthAmerica.UsSocialSecurityNumber.ValidationError>;
using LocalValidationResult = KfAccountNumbers.Governmental.NorthAmerica.UsSocialSecurityNumber.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class UsSocialSecurityNumberTests
{
   private const String ValidUnformattedSsn = "078051120";        // Actual SSN used in a 1930's advertising campaign
   private const String AltValidUnformattedSsn = "721074426";     // Another SSN rescinded by the Social Security Administration
   private const String ValidFormattedSsn = "078-05-1120";
   private const String AltValidFormattedSsn = "721 07 4426";

   // Values that will successfully create a UsSocialSecurityNumber object
   public static TheoryData<String> ValidValues =>
   [
      ValidUnformattedSsn,
      ValidFormattedSsn,
      AltValidFormattedSsn
   ];

   public static TheoryData<String> ValidAreaNumberBoundaryValues =>
   [
      "899123456",     // Just before ITIN range
      "001123456",     // Just after 000
      "665123456",     // Just before 666
      "667123456",     // Just after 666
   ];

   // Values that will report an invalid length
   public static TheoryData<String> InvalidLengthValues =>
   [
      "07805112",       // Length 8, too short
      "0780511201",     // Length 10, too long if not formatted
      "078-05-112",     // Length 10, too short if formatted
      "078 05 11201",   // Length 12, too long
      new String('1', 100),   // Very long value
   ];

   // Values that will report an invalid separator character
   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      // First separator position
      { "078010 1120", 3 },
      { "078110 1120", 3 },
      { "078210 1120", 3 },
      { "078310 1120", 3 },
      { "078410 1120", 3 },
      { "078510 1120", 3 },
      { "078610 1120", 3 },
      { "078710 1120", 3 },
      { "078810 1120", 3 },
      { "078910 1120", 3 },

      // Second separator position
      { "078 1001120", 6 },
      { "078 1011120", 6 },
      { "078 1021120", 6 },
      { "078 1031120", 6 },
      { "078 1041120", 6 },
      { "078 1051120", 6 },
      { "078 1061120", 6 },
      { "078 1071120", 6 },
      { "078 1081120", 6 },
      { "078 1091120", 6 },

      // Mixed separators
      { "078 10-1120", 6 },
      { "078-10 1120", 6 },
   };

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted values
      { ".12345678", 0 },           // Non-digit character '.'
      { "0 2345678", 1 },           // Non-digit character ' '
      { "01A345678", 2 },           // Non-digit character 'A'
      { "012Z45678", 3 },           // Non-digit character 'Z'
      { "0123^5678", 4 },           // Non-digit character '^'
      { "01234a078", 5 },           // Non-digit character 'a'
      { "012345z78", 6 },           // Non-digit character 'z'
      { "0123456~8", 7 },           // Non-digit character '~'
      { "01234567\u2153", 8 },      // Non-digit character Unicode fraction 1/3
      { "01234567\u00D6", 8 },      // Invalid character unicode O with umlaut

      // Formatted values
      { ".12 34 5678", 0 },           // Non-digit character '.'
      { "0 2 34 5678", 1 },           // Non-digit character ' '
      { "01A 34 5678", 2 },           // Non-digit character 'A'
      { "012 Z4 5678", 4 },           // Non-digit character 'Z'
      { "012 3^ 5678", 5 },           // Non-digit character '^'
      { "012-34-a078", 7 },           // Non-digit character 'a'
      { "012-34-5z78", 8 },           // Non-digit character 'z'
      { "012-34-56~8", 9 },           // Non-digit character '~'
      { "012-34-567\u2153", 10 },     // Non-digit character Unicode fraction 1/3
      { "012-34-567\u00D6", 10 },     // Invalid character unicode O with umlaut
   };

   // Values that will report an invalid area number
   public static TheoryData<String> InvalidAreaNumberValues =>
   [
      "000123456",
      "666123456",
      "900123456",
      "999123456",
      "000-12-3456",
      "666-12-3456",
      "900-12-3456",
      "999-12-3456",
      "000 12 3456",
      "666 12 3456",
      "900 12 3456",
      "999 12 3456",
   ];

   // Values that will report an invalid group number
   public static TheoryData<String> InvalidGroupNumberValues =>
   [
      "012005678",
      "012-00-5678",
      "012 00 5678",
   ];

   // Values that will report an invalid serial number
   public static TheoryData<String> InvalidSerialNumberValues =>
   [
      "012340000",
      "012-34-0000",
      "012 34 0000",
   ];

   // Values that will report an invalid all identical digits
   public static TheoryData<String> AllIdenticalDigitsValues =>
   [
      "111111111",         // Note that missing cases ("000000000", "666666666" and "999999999"
      "222222222",         // will fail the validation for area number before reaching the
      "333333333",         // validation for identical digits
      "444444444",
      "555555555",
      "777777777",
      "888888888",
      "111 11 1111",
      "222 22 2222",
      "333 33 3333",
      "444 44 4444",
      "555 55 5555",
      "777 77 7777",
      "888 88 8888",
   ];

   // Values that will report an invalid run of consecutive digits
   public static TheoryData<String> InvalidRunValues =>
   [
      "123456789",
      "123-45-6789",
      "123 45 6789",
   ];

   private static String GetAreaNumber(String value) => value[..3];

   private static String GetGroupNumber(String value)
      => value.Length switch
      {
         9 => value[3..5],
         11 => value[4..6],
         _ => throw new ArgumentException("Input must be 9 or 11 characters", nameof(value)),
      };

   /// <summary>
   /// Extracts unformatted SSN value. If SSN is 9 characters then value is
   /// returned unchanged. If an 11-character formatted SSN then assumes
   /// separators at positions 3 and 6.
   /// </summary>
   private static String GetRawValue(String value)
      => value.Length switch
      {
         9 => value,
         11 => value[..3] + value[4..6] + value[7..],
         _ => throw new ArgumentException("Input must be 9 or 11 characters", nameof(value)),
      };

   private static String GetSerialNumber(String value)
      => value.Length switch
      {
         9 => value[5..],
         11 => value[7..],
         _ => throw new ArgumentException("Input must be 9 or 11 characters", nameof(value)),
      };

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.UsSsnInvalidLength,
         value.Length,
         UsSocialSecurityNumber.GetValidLengthDefinitions());

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(
         Messages.UsSsnInvalidSeparator,
         value[position],
         position);

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.UsSsnInvalidCharacter,
         value[position],
         position);

   private static InvalidUsTinAreaNumber GetInvalidAreaNumberResult(String value)
      => new(Messages.UsSsnInvalidAreaNumber, GetAreaNumber(value));

   private static InvalidUsTinGroupNumber GetInvalidGroupNumberResult(String value)
      => new(Messages.UsSsnInvalidGroupNumber, GetGroupNumber(value));

   private static InvalidUsSsnSerialNumber GetInvalidSerialNumberResult(String value)
      => new(Messages.UsSsnInvalidSerialNumber, GetSerialNumber(value));

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidAreaNumberBoundaryValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = new UsSocialSecurityNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueIsEmpty(String? value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsSocialSecurityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsSocialSecurityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsSocialSecurityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsSocialSecurityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidAreaNumber(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidAreaNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsSocialSecurityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGroupNumberValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidGroupNumber(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidGroupNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsSocialSecurityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSerialNumberValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSerialNumber(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSerialNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsSocialSecurityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(AllIdenticalDigitsValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHas9IdenticalDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = default(InvalidUsSsnAllIdenticalDigits);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsSocialSecurityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRunValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasConsecutiveRun(String value)
   {
      // Arrange.
      LocalValidationError expected = default(InvalidUsSsnRun);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsSocialSecurityNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidUnformattedSsn)]
   [InlineData(ValidFormattedSsn)]
   public void UsSocialSecurityNumber_Value_ShouldReturnRawSsn(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);
      var sut = new UsSocialSecurityNumber(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidAreaNumberBoundaryValues))]
   public void UsSocialSecurityNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);
      var sut = new UsSocialSecurityNumber(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void UsSocialSecurityNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);
      var sut = new UsSocialSecurityNumber(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Fact]
   public void UsSocialSecurityNumber_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      UsSocialSecurityNumber sut = null!;

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void UsSocialSecurityNumber_CastUsSsnToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      UsSocialSecurityNumber sut = null!;

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidAreaNumberBoundaryValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new UsSocialSecurityNumber(value);

      // Act.
      var sut = (UsSocialSecurityNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueIsEmpty(String? value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsSocialSecurityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsSocialSecurityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsSocialSecurityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsSocialSecurityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueHasInvalidAreaNumber(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidAreaNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsSocialSecurityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGroupNumberValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueHasInvalidGroupNumber(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidGroupNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsSocialSecurityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSerialNumberValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueHasInvalidSerialNumber(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSerialNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsSocialSecurityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(AllIdenticalDigitsValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueHas9IdenticalDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = default(InvalidUsSsnAllIdenticalDigits);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsSocialSecurityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRunValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueHasConsecutiveRun(String value)
   {
      // Arrange.
      LocalValidationError expected = default(InvalidUsSsnRun);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsSocialSecurityNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsSocialSecurityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var sut2 = new UsSocialSecurityNumber(ValidUnformattedSsn);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void UsSocialSecurityNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var sut2 = new UsSocialSecurityNumber(AltValidUnformattedSsn);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void UsSocialSecurityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void UsSocialSecurityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidFormattedSsn);
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn.Replace('-', '.'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void UsSocialSecurityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidFormattedSsn.Replace('-', 'A'));
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn.Replace('-', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsSocialSecurityNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var sut2 = new UsSocialSecurityNumber(AltValidUnformattedSsn);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void UsSocialSecurityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void UsSocialSecurityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn);    // Same internal value

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void UsSocialSecurityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidFormattedSsn);
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn.Replace('-', '.'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void UsSocialSecurityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidFormattedSsn.Replace('-', 'A'));
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn.Replace('-', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidAreaNumberBoundaryValues))]
   public void UsSocialSecurityNumber_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new UsSocialSecurityNumber(value);

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnEmptyValue_WhenValueIsEmpty(String? value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidCharacter_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidAreaNumber_WhenValueHasInvalidAreaNumber(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidAreaNumberResult(value);

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGroupNumberValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidGroupNumber_WhenValueHasInvalidGroupNumber(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidGroupNumberResult(value);

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSerialNumberValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidSerialNumber_WhenValueHasInvalidSerialNumber(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSerialNumberResult(value);

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(AllIdenticalDigitsValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnAllIdenticalDigits_WhenValueHas9IdenticalDigits(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(InvalidUsSsnAllIdenticalDigits);

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRunValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidRun_WhenValueHasConsecutiveRun(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(InvalidUsSsnRun);

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsSocialSecurityNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var sut2 = new UsSocialSecurityNumber(ValidUnformattedSsn);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void UsSocialSecurityNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var sut2 = new UsSocialSecurityNumber(AltValidUnformattedSsn);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void UsSocialSecurityNumber_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void UsSocialSecurityNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidFormattedSsn);
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn.Replace('-', '.'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void UsSocialSecurityNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidFormattedSsn.Replace('-', 'A'));
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn.Replace('-', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void UsSocialSecurityNumber_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidUnformattedSsn);

      // Act/assert.
      sut.Equals(ValidUnformattedSsn).Should().BeFalse();
   }

   [Fact]
   public void UsSocialSecurityNumber_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidUnformattedSsn);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsSocialSecurityNumber_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var expected = ValidFormattedSsn;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void UsSocialSecurityNumber_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(AltValidUnformattedSsn);
      var mask = "___ __ ____";
      var expected = AltValidFormattedSsn;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void UsSocialSecurityNumber_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidUnformattedSsn);
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
   public void UsSocialSecurityNumber_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidUnformattedSsn);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = sut.Format(mask))
         .Should()
         .ThrowExactly<ArgumentException>()
         .WithParameterName(nameof(mask))
         .WithMessage(Messages.FormatMaskEmpty + "*");
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsSocialSecurityNumber_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var sut2 = new UsSocialSecurityNumber(ValidUnformattedSsn);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void UsSocialSecurityNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var sut2 = new UsSocialSecurityNumber(AltValidUnformattedSsn);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void UsSocialSecurityNumber_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void UsSocialSecurityNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidFormattedSsn);
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn.Replace('-', '.'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void UsSocialSecurityNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidFormattedSsn.Replace('-', 'A'));
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn.Replace('-', 'a'));

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

   // UsSocialSecurityNumber does not override Object.ReferenceEquals, so this
   // test just confirms that two different instances with the same value are
   // not considered reference equal.

   [Fact]
   public void UsSocialSecurityNumber_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidUnformattedSsn);
      var sut2 = new UsSocialSecurityNumber(ValidFormattedSsn);    // Same internal value

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidUnformattedSsn, ValidUnformattedSsn)]
   [InlineData(ValidFormattedSsn, ValidUnformattedSsn)]
   public void UsSocialSecurityNumber_ToString_ShouldReturnExpectedValue(
      String value,
      String expected)
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidAreaNumberBoundaryValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnValidValue_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = UsSocialSecurityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnEmptyValue_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = UsSocialSecurityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = UsSocialSecurityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = UsSocialSecurityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = UsSocialSecurityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidAreaNumber_WhenValueHasInvalidAreaNumber(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidAreaNumberResult(value);

      // Act.
      var result = UsSocialSecurityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGroupNumberValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidGroupNumber_WhenValueHasInvalidGroupNumber(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidGroupNumberResult(value);

      // Act.
      var result = UsSocialSecurityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSerialNumberValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidSerialNumber_WhenValueHasInvalidSerialNumber(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSerialNumberResult(value);

      // Act.
      var result = UsSocialSecurityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(AllIdenticalDigitsValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnAllIdenticalDigits_WhenValueHas9IdenticalDigits(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(InvalidUsSsnAllIdenticalDigits);

      // Act.
      var result = UsSocialSecurityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRunValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidRun_WhenValueHasConsecutiveRun(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(InvalidUsSsnRun);

      // Act.
      var result = UsSocialSecurityNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsSocialSecurityNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidUnformattedSsn);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<UsSocialSecurityNumber>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void UsSocialSecurityNumber_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidUnformattedSsn);

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{ValidUnformattedSsn}\"");  // Simple string, not object
   }

   public class Foo
   {
      public UsSocialSecurityNumber Ssn { get; set; } = null!;
   }

   [Fact]
   public void UsSocialSecurityNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Ssn = new UsSocialSecurityNumber(ValidUnformattedSsn) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void UsSocialSecurityNumber_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Ssn\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void UsSocialSecurityNumber_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Ssn\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Ssn.Should().BeNull();
   }

   [Fact]
   public void UsSocialSecurityNumber_JsonDeserialization_ShouldThrowKfValidationException_WhenSsnIsInvalid()
   {
      // Arrange.
      var json = "{\"Ssn\":\"666123456\"}";  // Invalid area number
      LocalValidationError expected = GetInvalidAreaNumberResult("666123456");

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
