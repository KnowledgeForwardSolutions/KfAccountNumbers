// Ignore Spelling: Deserialization Deserialize Json Kf

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

using LocalCreateResult = KfAccountNumbers.Results.UCreateResult<
   KfAccountNumbers.Governmental.NorthAmerica.UsNationalProviderIdentifier,
   KfAccountNumbers.Governmental.NorthAmerica.UsNationalProviderIdentifier.ValidationError>;
using LocalValidationError = KfAccountNumbers.Governmental.NorthAmerica.UsNationalProviderIdentifier.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.Governmental.NorthAmerica.UsNationalProviderIdentifier.ValidationError>;
using LocalValidationResult = KfAccountNumbers.Governmental.NorthAmerica.UsNationalProviderIdentifier.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class UsNationalProviderIdentifierTests
{
   private const String ValidNpi = "1245319599";         // Example from www.hippaspace.com
   private const String AltValidNpi = "1234567893";      // Example from Wikipedia article on Luhn algorithm

   public static TheoryData<String> InvalidLengthValues =>
   [
      "124531959",
      "12453195999"
   ];

   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted values
      { ".234567899", 0 },           // Non-digit character '.'
      { "1 34567899", 1 },           // Non-digit character ' '
      { "12A4567899", 2 },           // Non-digit character 'A'
      { "123Z567899", 3 },           // Non-digit character 'Z'
      { "1234^67899", 4 },           // Non-digit character '^'
      { "12345a7899", 5 },           // Non-digit character 'a'
      { "123456z899", 6 },           // Non-digit character 'z'
      { "1234567~99", 7 },           // Non-digit character '~'
      { "12345678\u21539", 8 },      // Non-digit character Unicode fraction 1/3
      { "123456789\u00D6", 9 },      // Invalid character unicode O with umlaut
   };

   // Luhn algorithm is unable to detect these errors so they should pass
   // validation (and the constructor/Create method should create an instance of
   // UsNationalProviderIdentifier).
   public static TheoryData<String> CheckDigitUndetectableErrorValues =>
   [
      "1234569071",           // 1234560971 with two digit transposition 09 -> 90
      "1230967899",           // 1239067899 with two digit transposition 90 -> 09
      "1122334497",           // 1122334497 with two digit twin error 22 -> 55
      "1122337797",           // 1122334497 with two digit twin error 44 -> 77
      "1122664497",           // 1122334497 with two digit twin error 33 -> 66
   ];

   public static TheoryData<String> CheckDigitDetectableErrorValues =>
   [
      "1238560971",           // 1234560971 with single digit transcription error 4 -> 8
      "1243560971",           // 1234560971 with two digit transposition error 34 -> 43
      "4422334497",           // 1122334497 with two digit twin error 11 -> 44
   ];

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.UsNpiInvalidLength,
         value.Length,
         new ValidLengthDefinition(10, Messages.UsNpiValidLength));

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.UsNpiInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.UsNpiInvalidCheckDigit,
         Algorithms.Luhn.AlgorithmName);

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Constructor_ShouldCreateInstance_WhenValueIsValid()
   {
      // Arrange.
      var value = ValidNpi;

      // Act.
      var sut = new UsNationalProviderIdentifier(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(value);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowKfValidationException_WhenValueIsEmpty(String? value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsNationalProviderIdentifier(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsNationalProviderIdentifier(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowKfValidationException_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new UsNationalProviderIdentifier(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String value)
   {
      // Act.
      var sut = new UsNationalProviderIdentifier(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(value);
   }

   [Theory]
   [MemberData(nameof(CheckDigitDetectableErrorValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowKfValidationException_WhenCheckDigitContainsDetectableError(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new UsNationalProviderIdentifier(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNpi)]
   [InlineData(AltValidNpi)]
   public void UsNationalProviderIdentifier_Value_ShouldReturnRawNpi(String value)
   {
      // Arrange.
      var sut = new UsNationalProviderIdentifier(value);

      // Act/assert.
      sut.Value.Should().Be(value);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidNpi;
      var sut = new UsNationalProviderIdentifier(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(value);
   }

   [Fact]
   public void UsNationalProviderIdentifier_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidNpi;
      var sut = new UsNationalProviderIdentifier(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(value);
   }

   [Fact]
   public void UsNationalProviderIdentifier_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      UsNationalProviderIdentifier sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void UsNationalProviderIdentifier_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      UsNationalProviderIdentifier sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void UsNationalProviderIdentifier_ExplicitCastToUsNpi_ShouldCreateInstance_WhenValueIsValid()
   {
      // Arrange.
      var value = ValidNpi;

      // Act.
      var sut = (UsNationalProviderIdentifier)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(value);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsNationalProviderIdentifier_ExplicitCastToUsNpi_ShouldThrowKfValidationException_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (UsNationalProviderIdentifier)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsNationalProviderIdentifier_ExplicitCastToUsNpi_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (UsNationalProviderIdentifier)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsNationalProviderIdentifier_ExplicitCastToUsNpi_ShouldThrowKfValidationException_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (UsNationalProviderIdentifier)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void UsNationalProviderIdentifier_ExplicitCastToUsNpi_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String value)
   {
      // Act.
      var sut = (UsNationalProviderIdentifier)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(value);
   }

   [Theory]
   [MemberData(nameof(CheckDigitDetectableErrorValues))]
   public void UsNationalProviderIdentifier_ExplicitCastToUsNpi_ShouldThrowKfValidationException_WhenCheckDigitContainsDetectableError(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (UsNationalProviderIdentifier)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsNationalProviderIdentifier(ValidNpi);
      var sut2 = new UsNationalProviderIdentifier(ValidNpi);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void UsNationalProviderIdentifier_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new UsNationalProviderIdentifier(ValidNpi);
      var sut2 = new UsNationalProviderIdentifier(AltValidNpi);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new UsNationalProviderIdentifier(ValidNpi);
      var sut2 = new UsNationalProviderIdentifier(AltValidNpi);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void UsNationalProviderIdentifier_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsNationalProviderIdentifier(ValidNpi);
      var sut2 = new UsNationalProviderIdentifier(ValidNpi);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Create_ShouldCreateInstance_WhenValueIsValid()
   {
      // Arrange.
      var value = ValidNpi;
      LocalCreateResult expected = new UsNationalProviderIdentifier(value);

      // Act.
      var result = UsNationalProviderIdentifier.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsNationalProviderIdentifier_Create_ShouldReturnEmptyValue_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = UsNationalProviderIdentifier.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsNationalProviderIdentifier_Create_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = UsNationalProviderIdentifier.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsNationalProviderIdentifier_Create_ShouldReturnInvalidCharacter_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = UsNationalProviderIdentifier.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void UsNationalProviderIdentifier_Create_ShouldCreateInstance_WhenCheckDigitContainsUndetectableError(String value)
   {
      // Arrange.
      LocalCreateResult expected = new UsNationalProviderIdentifier(value);

      // Act.
      var result = UsNationalProviderIdentifier.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(CheckDigitDetectableErrorValues))]
   public void UsNationalProviderIdentifier_Create_ShouldReturnInvalidCheckDigit_WhenCheckDigitContainsDetectableError(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = UsNationalProviderIdentifier.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsNationalProviderIdentifier(ValidNpi);
      var sut2 = new UsNationalProviderIdentifier(ValidNpi);    // Same internal value

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void UsNationalProviderIdentifier_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new UsNationalProviderIdentifier(ValidNpi);
      var sut2 = new UsNationalProviderIdentifier(AltValidNpi);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void UsNationalProviderIdentifier_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new UsNationalProviderIdentifier(ValidNpi);

      // Act/assert.
      sut.Equals(ValidNpi).Should().BeFalse();
   }

   [Fact]
   public void UsNationalProviderIdentifier_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new UsNationalProviderIdentifier(ValidNpi);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new UsNationalProviderIdentifier(ValidNpi);
      var sut2 = new UsNationalProviderIdentifier(ValidNpi);    // Same internal value

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void UsNationalProviderIdentifier_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new UsNationalProviderIdentifier(ValidNpi);
      var sut2 = new UsNationalProviderIdentifier(AltValidNpi);

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

   // UsNationalProviderIdentifier does not override
   // Object.ReferenceEquals, so this test just confirms that two different
   // instances with the same value are not considered reference equal.

   [Fact]
   public void UsNationalProviderIdentifier_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new UsNationalProviderIdentifier(ValidNpi);
      var sut2 = new UsNationalProviderIdentifier(ValidNpi);    // Same internal value

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_ToString_ShouldReturnExpectedValue()
   {
      // Arrange.
      var value = ValidNpi;
      var sut = new UsNationalProviderIdentifier(value);

      // Act/assert.
      sut.ToString().Should().Be(value);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnValidValue_WhenValueIsValid()
   {
      // Arrange.
      var value = ValidNpi;
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = UsNationalProviderIdentifier.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = UsNationalProviderIdentifier.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = UsNationalProviderIdentifier.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidCharacter_WhenValueContainsNonAsciiDigit(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = UsNationalProviderIdentifier.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnValidationPassed_WhenCheckDigitContainsUndetectableError(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = UsNationalProviderIdentifier.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(CheckDigitDetectableErrorValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidCheckDigit_WhenCheckDigitContainsDetectableError(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = UsNationalProviderIdentifier.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new UsNationalProviderIdentifier(ValidNpi);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<UsNationalProviderIdentifier>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void UsNationalProviderIdentifier_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new UsNationalProviderIdentifier(ValidNpi);

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{ValidNpi}\"");  // Simple string, not object
   }

   public class Foo
   {
      public UsNationalProviderIdentifier Npi { get; set; } = null!;
   }

   [Fact]
   public void UsNationalProviderIdentifier_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Npi = new UsNationalProviderIdentifier(ValidNpi) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void UsNationalProviderIdentifier_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Npi\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void UsNationalProviderIdentifier_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Npi\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Npi.Should().BeNull();
   }

   [Fact]
   public void UsNationalProviderIdentifier_JsonDeserialization_ShouldThrowKfValidationException_WhenNpiIsInvalid()
   {
      // Arrange.
      var json = "{\"Npi\":\"1238560971\"}";  // Invalid check digit
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
