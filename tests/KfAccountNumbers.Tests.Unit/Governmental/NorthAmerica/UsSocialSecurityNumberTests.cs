// Ignore Spelling: Deserialization Deserialize Json Kf ssn

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class UsSocialSecurityNumberTests
{
   private const String ValidNineCharSsn = "078051120";        // Actual SSN used in a 1930's advertising campaign
   private const String AltValidNineCharSsn = "721074426";     // Another SSN rescinded by the Social Security Administration
   private const String ValidElevenCharSsn = "078-05-1120";
   private const String AltValidElevenCharSsn = "721 07 4426";

   // Values that will successfully create a UsSocialSecurityNumber object
   public static TheoryData<String> ValidValues =>
   [
      ValidNineCharSsn,
      ValidElevenCharSsn,
      AltValidElevenCharSsn
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
      "07805112",
      "0780511201",
      "078-05-112",
      "078 05 11201",
   ];

   // Values that will report an invalid separator character
   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "078 05-1120",
      "078-05 1120",
      "078005 1120",
      "07800501120",
      "07810511120",
      "07820521120",
      "07830531120",
      "07840541120",
      "07850551120",
      "07860561120",
      "07870571120",
      "07880581120",
      "07890591120",
   ];

   // Values that will report an invalid character encountered
   public static TheoryData<String> InvalidCharacterValues =>
   [
      "A12345678",
      "0A2345678",
      "01A345678",
      "012A45678",
      "0123A5678",
      "01234A678",
      "012345A78",
      "0123456A8",
      "01234567A",
      "0;2345678",
      "0\u21532345678",       // Unicode fraction 1/3
      "0\u21672345678",       // Unicode Roman numeral VII
      "0\u0BEF2345678",       // Unicode Tamil number 9
      "A12-34-5678",
      "0A2-34-5678",
      "01A-34-5678",
      "012-A4-5678",
      "012-3A-5678",
      "012-34-A678",
      "012-34-5A78",
      "012-34-56A8",
      "012-34-567A",
      "0;2-34-5678",
      "0\u21532-34-5678",     // Unicode fraction 1/3
      "0\u21672-34-5678",     // Unicode Roman numeral VII
      "0\u0BEF2-34-5678",     // Unicode Tamil number 9
   ];

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

   /// <summary>
   /// Extracts unformatted SSN value. If ITTN is 9 characters then value is
   /// returned unchanged. If an 11-character formatted SSN then assumes
   /// separators at positions 3 and 6.
   /// </summary>
   private static String GetRawSsn(String ssn)
      => ssn.Length switch
      {
         9 => ssn,
         11 => ssn[0..3] + ssn[4..6] + ssn[7..11],
         _ => throw new ArgumentException("Input must be 9 or 11 characters", nameof(ssn))
      };

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidAreaNumberBoundaryValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldCreateObject_WhenValueContainsValidSsn(String value)
   {
      // Arrange.
      var expected = GetRawSsn(value);

      // Act.
      var sut = new UsSocialSecurityNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueIsEmpty(String? value)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(value))
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnEmpty + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(value))
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidLength + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_When11CharacterValueContainsInvalidSeparator(String ssn)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(ssn))
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidSeparatorEncountered + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueContainsNonAsciiDigit(String value)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(value))
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidAreaNumber(String value)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(value))
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidAreaNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidAreaNumber);

   [Theory]
   [MemberData(nameof(InvalidGroupNumberValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidGroupNumber(String value)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(value))
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidGroupNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidGroupNumber);

   [Theory]
   [MemberData(nameof(InvalidSerialNumberValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSerialNumber(String value)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(value))
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidSerialNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidSerialNumber);

   [Theory]
   [MemberData(nameof(AllIdenticalDigitsValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHas9IdenticalDigits(String value)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(value))
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnAllIdenticalDigits + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.AllIdenticalDigits);

   [Theory]
   [MemberData(nameof(InvalidRunValues))]
   public void UsSocialSecurityNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasConsecutiveRun(String value)
      => FluentActions
         .Invoking(() => _ = new UsSocialSecurityNumber(value))
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidRun + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidRun);

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn)]
   public void UsSocialSecurityNumber_Value_ShouldReturnRawSsn(String value)
   {
      // Arrange.
      var expected = GetRawSsn(value);
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
      var expected = GetRawSsn(value);
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
      var expected = GetRawSsn(value);
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

      // Act/assert.
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

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidAreaNumberBoundaryValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldCreateObject_WhenValueContainsValidSsn(String value)
   {
      // Arrange.
      var expected = GetRawSsn(value);

      // Act.
      var sut = (UsSocialSecurityNumber)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueIsEmpty(String? value)
      => FluentActions
         .Invoking(() => _ = (UsSocialSecurityNumber)value)
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnEmpty + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
      => FluentActions
         .Invoking(() => _ = (UsSocialSecurityNumber)value)
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidLength + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_When11CharacterValueContainsInvalidSeparator(String value)
      => FluentActions
         .Invoking(() => _ = (UsSocialSecurityNumber)value)
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidSeparatorEncountered + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueContainsNonAsciiDigit(String value)
      => FluentActions
         .Invoking(() => _ = (UsSocialSecurityNumber)value)
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueHasInvalidAreaNumber(String value)
      => FluentActions
         .Invoking(() => _ = (UsSocialSecurityNumber)value)
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidAreaNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidAreaNumber);

   [Theory]
   [MemberData(nameof(InvalidGroupNumberValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueHasInvalidGroupNumber(String value)
      => FluentActions
         .Invoking(() => _ = (UsSocialSecurityNumber)value)
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidGroupNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidGroupNumber);

   [Theory]
   [MemberData(nameof(InvalidSerialNumberValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueHasInvalidSerialNumber(String value)
      => FluentActions
         .Invoking(() => _ = (UsSocialSecurityNumber)value)
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidSerialNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidSerialNumber);

   [Theory]
   [MemberData(nameof(AllIdenticalDigitsValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueHas9IdenticalDigits(String value)
      => FluentActions
         .Invoking(() => _ = (UsSocialSecurityNumber)value)
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnAllIdenticalDigits + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.AllIdenticalDigits);

   [Theory]
   [MemberData(nameof(InvalidRunValues))]
   public void UsSocialSecurityNumber_ExplicitCastToUsSsn_ShouldThrowKfValidationException_WhenValueHasConsecutiveRun(String value)
      => FluentActions
         .Invoking(() => _ = (UsSocialSecurityNumber)value)
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidRun + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidRun);

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsSocialSecurityNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidNineCharSsn);
      var sut2 = new UsSocialSecurityNumber(ValidElevenCharSsn);    // Same internal value

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void UsSocialSecurityNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidNineCharSsn);
      var sut2 = new UsSocialSecurityNumber(AltValidNineCharSsn);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsSocialSecurityNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidNineCharSsn);
      var sut2 = new UsSocialSecurityNumber(AltValidNineCharSsn);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void UsSocialSecurityNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidNineCharSsn);
      var sut2 = new UsSocialSecurityNumber(ValidElevenCharSsn);    // Same internal value

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
   public void UsSocialSecurityNumber_Create_ShouldCreateObject_WhenValueContainsValidSsn(String value)
   {
      // Arrange.
      var expected = GetRawSsn(value);
      var expectedValue = new UsSocialSecurityNumber(expected);

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String? value)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.Empty;

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidLength;

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidSeparatorCharacterValidationResult_When11CharacterValueContainsInvalidSeparator(String value)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered;

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueContainsNonAsciiDigit(String value)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered;

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidAreaNumberValidationResult_WhenValueHasInvalidAreaNumber(String value)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidAreaNumber;

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGroupNumberValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidGroupNumberValidationResult_WhenValueHasInvalidGroupNumber(String value)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidGroupNumber;

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSerialNumberValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidSerialNumberValidationResult_WhenValueHasInvalidSerialNumber(String value)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidSerialNumber;

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(AllIdenticalDigitsValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnAllIdenticalDigitsValidationResult_WhenValueHas9IdenticalDigits(String value)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.AllIdenticalDigits;

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidRunValues))]
   public void UsSocialSecurityNumber_Create_ShouldReturnInvalidRunValidationResult_WhenValueHasConsecutiveRun(String value)
   {
      // Arrange.
      var expected = UsSocialSecurityNumberValidationResult.InvalidRun;

      // Act.
      var result = UsSocialSecurityNumber.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsSocialSecurityNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidNineCharSsn);
      var sut2 = new UsSocialSecurityNumber(ValidElevenCharSsn);    // Same internal value

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void UsSocialSecurityNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new UsSocialSecurityNumber(ValidNineCharSsn);
      var sut2 = new UsSocialSecurityNumber(AltValidNineCharSsn);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void UsSocialSecurityNumber_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidNineCharSsn);

      // Act/assert.
      sut.Equals(ValidNineCharSsn).Should().BeFalse();
   }

   [Fact]
   public void UsSocialSecurityNumber_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidNineCharSsn);

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
      var sut = new UsSocialSecurityNumber(ValidNineCharSsn);
      var expected = ValidElevenCharSsn;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void UsSocialSecurityNumber_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(AltValidNineCharSsn);
      var mask = "___ __ ____";
      var expected = AltValidElevenCharSsn;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void UsSocialSecurityNumber_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidNineCharSsn);
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
      var sut = new UsSocialSecurityNumber(ValidNineCharSsn);

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
      var sut1 = new UsSocialSecurityNumber(ValidNineCharSsn);
      var sut2 = new UsSocialSecurityNumber(ValidElevenCharSsn);    // Same internal value

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
      var sut1 = new UsSocialSecurityNumber(ValidNineCharSsn);
      var sut2 = new UsSocialSecurityNumber(AltValidNineCharSsn);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
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
      var sut1 = new UsSocialSecurityNumber(ValidNineCharSsn);
      var sut2 = new UsSocialSecurityNumber(ValidElevenCharSsn);    // Same internal value

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharSsn, ValidNineCharSsn)]
   [InlineData(ValidElevenCharSsn, ValidNineCharSsn)]
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
   public void UsSocialSecurityNumber_Validate_ShouldReturnValidationPassed_WhenValueContainsValidSsn(String value)
      => UsSocialSecurityNumber.Validate(value).Should().Be(UsSocialSecurityNumberValidationResult.ValidationPassed);

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? value)
      => UsSocialSecurityNumber.Validate(value).Should().Be(UsSocialSecurityNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => UsSocialSecurityNumber.Validate(value).Should().Be(UsSocialSecurityNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidSeparatorEncountered_When11CharacterValueContainsAnInvalidSeparator(String value)
      => UsSocialSecurityNumber.Validate(value).Should().Be(UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidCharacterEncountered_WhenValueContainsNonAsciiDigit(String value)
      => UsSocialSecurityNumber.Validate(value).Should().Be(UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidAreaNumber_WhenValueHasInvalidAreaNumber(String value)
      => UsSocialSecurityNumber.Validate(value).Should().Be(UsSocialSecurityNumberValidationResult.InvalidAreaNumber);

   [Theory]
   [MemberData(nameof(InvalidGroupNumberValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidGroupNumber_WhenValueHasInvalidGroupNumber(String value)
      => UsSocialSecurityNumber.Validate(value).Should().Be(UsSocialSecurityNumberValidationResult.InvalidGroupNumber);

   [Theory]
   [MemberData(nameof(InvalidSerialNumberValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidSerialNumber_WhenValueHasInvalidSerialNumber(String value)
      => UsSocialSecurityNumber.Validate(value).Should().Be(UsSocialSecurityNumberValidationResult.InvalidSerialNumber);

   [Theory]
   [MemberData(nameof(AllIdenticalDigitsValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnAllIdenticalDigits_WhenValueHas9IdenticalDigits(String value)
      => UsSocialSecurityNumber.Validate(value).Should().Be(UsSocialSecurityNumberValidationResult.AllIdenticalDigits);

   [Theory]
   [MemberData(nameof(InvalidRunValues))]
   public void UsSocialSecurityNumber_Validate_ShouldReturnInvalidRun_WhenValueHasConsecutiveRun(String value)
      => UsSocialSecurityNumber.Validate(value).Should().Be(UsSocialSecurityNumberValidationResult.InvalidRun);

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsSocialSecurityNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new UsSocialSecurityNumber(ValidNineCharSsn);

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
      var sut = new UsSocialSecurityNumber(ValidNineCharSsn);

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{ValidNineCharSsn}\"");  // Simple string, not object
   }

   public class Foo
   {
      public UsSocialSecurityNumber Ssn { get; set; } = null!;
   }

   [Fact]
   public void UsSocialSecurityNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Ssn = new UsSocialSecurityNumber(ValidNineCharSsn) };
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

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should()
         .ThrowExactly<KfValidationException<UsSocialSecurityNumberValidationResult>>()
         .WithMessage(Messages.UsSsnInvalidAreaNumber + "*")
         .And.ValidationResult.Should().Be(UsSocialSecurityNumberValidationResult.InvalidAreaNumber);
   }

   #endregion
}
