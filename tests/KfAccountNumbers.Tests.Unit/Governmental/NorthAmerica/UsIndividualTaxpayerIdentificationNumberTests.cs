// Ignore Spelling: Deserialization Deserialize itin Itin Json Kf Unformatted

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

using LocalCreateResult = KfAccountNumbers.Results.UCreateResult<
   KfAccountNumbers.Governmental.NorthAmerica.UsIndividualTaxpayerIdentificationNumber,
   KfAccountNumbers.Governmental.NorthAmerica.UsIndividualTaxpayerIdentificationNumber.ValidationError>;

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class UsIndividualTaxpayerIdentificationNumberTests
{
   private const String ValidUnformattedItin = "901501234";
   private const String AltValidUnformattedItin = "987654321";
   private const String ValidFormattedItin = "901-50-1234";
   private const String AltValidFormattedItin = "987 65 4321";

   // Values that will successfully create a UsIndividualTaxpayerIdentificationNumber object
   public static TheoryData<String> ValidValues =>
   [
      ValidUnformattedItin,
      ValidFormattedItin,
      AltValidFormattedItin
   ];

   public static TheoryData<String> ValidGroupNumberBoundaryValues =>
   [
      "900500123",
      "900650123",
      "900700123",
      "900880123",
      "900900123",
      "900920123",
      "900940123",
      "900990123",
   ];

   // Values that will report an invalid length
   public static TheoryData<String> InvalidLengthValues =>
   [
      "90150123",
      "9015012345",
      "901-50-123",
      "987 65 43210",
   ];

   // Values that will report an invalid area number (first digit must be 9)
   public static TheoryData<String> InvalidAreaNumberValues =>
   [
      "001501234",
      "101501234",
      "201501234",
      "301501234",
      "401501234",
      "501501234",
      "601501234",
      "701501234",
      "801501234",
      "001-50-1234",
      "101-50-1234",
      "201-50-1234",
      "301-50-1234",
      "401-50-1234",
      "501-50-1234",
      "601-50-1234",
      "701-50-1234",
      "801-50-1234",
   ];

   // Values that will report an invalid separator character
   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      // First separator position
      { "901050 1234", 3 },
      { "901150 1234", 3 },
      { "901250 1234", 3 },
      { "901350 1234", 3 },
      { "901450 1234", 3 },
      { "901550 1234", 3 },
      { "901650 1234", 3 },
      { "901750 1234", 3 },
      { "901850 1234", 3 },
      { "901950 1234", 3 },

      // Second separator position
      { "901 5001234", 6 },
      { "901 5011234", 6 },
      { "901 5021234", 6 },
      { "901 5031234", 6 },
      { "901 5041234", 6 },
      { "901 5051234", 6 },
      { "901 5061234", 6 },
      { "901 5071234", 6 },
      { "901 5081234", 6 },
      { "901 5091234", 6 },

      // Mixed separators
      { "901 50-1234", 6 },
      { "901-50 1234", 6 },
   };

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted values
      // { ".01501234", 0 },           // Non-digit character '.' - will fail invalid area number validation before invalid character validation
      { "9 1501234", 1 },           // Non-digit character ' '
      { "90A501234", 2 },           // Non-digit character 'A'
      { "901Z01234", 3 },           // Non-digit character 'Z'
      { "9015^1234", 4 },           // Non-digit character '^'
      { "90150a234", 5 },           // Non-digit character 'a'
      { "901501z34", 6 },           // Non-digit character 'z'
      { "9015012~4", 7 },           // Non-digit character '~'
      { "90150123\u2153", 8 },      // Non-digit character Unicode fraction 1/3
      { "90150123\u00D6", 8 },      // Invalid character unicode O with umlaut

      // Formatted values
      // { ".01 50 1234", 0 },           // Non-digit character '.' - will fail invalid area number validation before invalid character validation
      { "9 1 50 1234", 1 },           // Non-digit character ' '
      { "90A 50 1234", 2 },           // Non-digit character 'A'
      { "901 Z0 1234", 4 },           // Non-digit character 'Z'
      { "901 5^ 1234", 5 },           // Non-digit character '^'
      { "901-50-a234", 7 },           // Non-digit character 'a'
      { "901-50-1z34", 8 },           // Non-digit character 'z'
      { "901-50-12~4", 9 },           // Non-digit character '~'
      { "901-50-123\u2153", 10 },     // Non-digit character Unicode fraction 1/3
      { "901-50-123\u00D6", 10 },     // Invalid character unicode O with umlaut
   };

   // Values that will report an invalid group number (group number must be in the range 50-65, 70-88, 90-92 or 94-99)
   public static TheoryData<String> InvalidGroupNumberBoundaryValues =>
   [
      "900000123",
      "900490123",
      "900660123",
      "900690123",
      "900890123",
      "900930123",
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
   /// Extracts unformatted ITIN value. If ITIN is 9 characters then value is
   /// returned unchanged. If an 11-character formatted ITIN then assumes
   /// separators at positions 3 and 6.
   /// </summary>
   private static String GetRawValue(String value)
      => value.Length switch
      {
         9 => value,
         11 => value[0..3] + value[4..6] + value[7..11],
         _ => throw new ArgumentException("Input must be 9 or 11 characters", nameof(value)),
      };

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.UsItinInvalidLength,
         value.Length,
         UsIndividualTaxpayerIdentificationNumber.GetInvalidLengthDefinitions());

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(
         Messages.UsItinInvalidSeparatorEncountered,
         value[position],
         position);

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.UsItinInvalidCharacterEncountered,
         value[position],
         position);

   private static UsTinInvalidAreaNumber GetInvalidAreaNumberResult(String value)
      => new(Messages.UsItinInvalidAreaNumber, GetAreaNumber(value));

   private static UsTinInvalidGroupNumber GetInvalidGroupNumberResult(String value)
      => new(Messages.UsItinInvalidGroupNumber, GetGroupNumber(value));

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldCreateObject_WhenValueContainsValidItin(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = new UsIndividualTaxpayerIdentificationNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldThrowKfValidationException_WhenValueIsEmpty(String? value)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsIndividualTaxpayerIdentificationNumber(value))
         .Should().ThrowExactly<UKfValidationException<UsIndividualTaxpayerIdentificationNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsIndividualTaxpayerIdentificationNumber(value))
         .Should().ThrowExactly<UKfValidationException<UsIndividualTaxpayerIdentificationNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<UsIndividualTaxpayerIdentificationNumber.ValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidAreaNumber(String value)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationError expected = GetInvalidAreaNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsIndividualTaxpayerIdentificationNumber(value))
         .Should().ThrowExactly<UKfValidationException<UsIndividualTaxpayerIdentificationNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsIndividualTaxpayerIdentificationNumber(value))
         .Should().ThrowExactly<UKfValidationException<UsIndividualTaxpayerIdentificationNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldThrowKfValidationException_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsIndividualTaxpayerIdentificationNumber(value))
         .Should().ThrowExactly<UKfValidationException<UsIndividualTaxpayerIdentificationNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidGroupNumber(String value)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationError expected = GetInvalidGroupNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsIndividualTaxpayerIdentificationNumber(value))
         .Should().ThrowExactly<UKfValidationException<UsIndividualTaxpayerIdentificationNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);
      var sut = new UsIndividualTaxpayerIdentificationNumber(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void UsIndividualTaxpayerIdentificationNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);
      var sut = new UsIndividualTaxpayerIdentificationNumber(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldCreateInstance_WhenValueContainsValidItin(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);

      // Act.
      var sut = (UsIndividualTaxpayerIdentificationNumber)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldThrowKfValidationException_WhenValueIsEmpty(String? value)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsIndividualTaxpayerIdentificationNumber)value)
         .Should().ThrowExactly<UKfValidationException<UsIndividualTaxpayerIdentificationNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsIndividualTaxpayerIdentificationNumber)value)
         .Should().ThrowExactly<UKfValidationException<UsIndividualTaxpayerIdentificationNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<UsIndividualTaxpayerIdentificationNumber.ValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldThrowKfValidationException_WhenValueHasInvalidAreaNumber(String value)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationError expected = GetInvalidAreaNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsIndividualTaxpayerIdentificationNumber)value)
         .Should().ThrowExactly<UKfValidationException<UsIndividualTaxpayerIdentificationNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldThrowKfValidationException_WhenValueContainsInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsIndividualTaxpayerIdentificationNumber)value)
         .Should().ThrowExactly<UKfValidationException<UsIndividualTaxpayerIdentificationNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldThrowKfValidationException_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsIndividualTaxpayerIdentificationNumber)value)
         .Should().ThrowExactly<UKfValidationException<UsIndividualTaxpayerIdentificationNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldThrowKfValidationException_WhenValueHasInvalidGroupNumber(String value)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationError expected = GetInvalidGroupNumberResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsIndividualTaxpayerIdentificationNumber)value)
         .Should().ThrowExactly<UKfValidationException<UsIndividualTaxpayerIdentificationNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(AltValidUnformattedItin);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin.Replace('-', '.'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin.Replace('-', 'A'));
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin.Replace('-', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(AltValidUnformattedItin);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin.Replace('-', '.'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin.Replace('-', 'A'));
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin.Replace('-', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidUnformattedItin)]
   [InlineData(ValidFormattedItin)]
   public void UsIndividualTaxpayerIdentificationNumber_Value_ShouldReturnRawItin(String value)
   {
      // Arrange.
      var expected = GetRawValue(value);
      var sut = new UsIndividualTaxpayerIdentificationNumber(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldCreateInstance_WhenValuIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new UsIndividualTaxpayerIdentificationNumber(value);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldReturnEmptyValue_WhenValueIsEmpty(String? value)
   {
      // Arrange.
      LocalCreateResult expected = (UsIndividualTaxpayerIdentificationNumber.ValidationError)default(EmptyValue);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (UsIndividualTaxpayerIdentificationNumber.ValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<UsIndividualTaxpayerIdentificationNumber.ValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldReturnInvalidAreaNumber_WhenValueHasInvalidAreaNumber(String value)
   {
      // Arrange.
      LocalCreateResult expected = (UsIndividualTaxpayerIdentificationNumber.ValidationError)GetInvalidAreaNumberResult(value);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldReturnInvalidSeparator_WhenValueContainsInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (UsIndividualTaxpayerIdentificationNumber.ValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldReturnInvalidCharacter_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (UsIndividualTaxpayerIdentificationNumber.ValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldReturnInvalidGroupNumber_WhenValueHasInvalidGroupNumber(String value)
   {
      // Arrange.
      LocalCreateResult expected = (UsIndividualTaxpayerIdentificationNumber.ValidationError)GetInvalidGroupNumberResult(value);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(AltValidUnformattedItin);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin.Replace('-', '.'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin.Replace('-', 'A'));
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin.Replace('-', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);

      // Act/assert.
      sut.Equals(ValidUnformattedItin).Should().BeFalse();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var expected = ValidFormattedItin;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(AltValidUnformattedItin);
      var mask = "___ __ ____";
      var expected = AltValidFormattedItin;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
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
   public void UsIndividualTaxpayerIdentificationNumber_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);

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
   public void UsIndividualTaxpayerIdentificationNumber_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(AltValidUnformattedItin);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin.Replace('-', '.'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin.Replace('-', 'A'));
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin.Replace('-', 'a'));

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

   // UsIndividualTaxpayerIdentificationNumber does not override
   // Object.ReferenceEquals, so this test just confirms that two different
   // instances with the same value are not considered reference equal.

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);
      var sut2 = new UsIndividualTaxpayerIdentificationNumber(ValidFormattedItin);    // Same internal value

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidUnformattedItin, ValidUnformattedItin)]
   [InlineData(ValidFormattedItin, ValidUnformattedItin)]
   public void UsIndividualTaxpayerIdentificationNumber_ToString_ShouldReturnExpectedValue(
      String value,
      String expected)
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationResult expected = default(ValidValue);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? value)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationResult expected = default(EmptyValue);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<UsIndividualTaxpayerIdentificationNumber.ValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidAreaNumber_WhenValueHasInvalidAreaNumber(String value)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationResult expected = GetInvalidAreaNumberResult(value);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidSeparator_WhenValueContainsAnInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidCharacter_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidGroupNumber_WhenValueHasInvalidGroupNumber(String value)
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber.ValidationResult expected = GetInvalidGroupNumberResult(value);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization/Deserialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<UsIndividualTaxpayerIdentificationNumber>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin);

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{ValidUnformattedItin}\"");  // Simple string, not object
   }

   public class Foo
   {
      public UsIndividualTaxpayerIdentificationNumber Itin { get; set; } = null!;
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Itin = new UsIndividualTaxpayerIdentificationNumber(ValidUnformattedItin) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Itin\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Itin\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Itin.Should().BeNull();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_JsonDeserialization_ShouldThrowKfValidationException_WhenItinIsInvalid()
   {
      // Arrange.
      var json = "{\"Itin\":\"001501234\"}";  // Invalid area number
      UsIndividualTaxpayerIdentificationNumber.ValidationError expected = GetInvalidAreaNumberResult("001501234");

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<UKfValidationException<UsIndividualTaxpayerIdentificationNumber.ValidationError>>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
