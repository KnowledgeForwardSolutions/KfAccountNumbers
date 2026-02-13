// Ignore Spelling: Deserialization Deserialize Json npi

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class UsNationalProviderIdentifierTests
{
   private const String ValidNpi = "1245319599";         // Example from www.hippaspace.com
   private const String AltValidNpi = "1234567893";     // Example from Wikipedia article on Luhn algorithm

   public static TheoryData<String> InvalidLengthValues =>
   [
      "124531959",
      "12453195999"
   ];

   public static TheoryData<String> InvalidCharacterValues =>
   [
      "A245319599",
      "1A45319599",
      "12A5319599",
      "124A319599",
      "1245A19599",
      "12453A9599",
      "124531A599",
      "1245319A99",
      "12453195A9",
      "124531959A",
      "1;45319599",
      "1\u215345319599",      // Unicode fraction 1/3
      "1\u216745319599",      // Unicode Roman numeral VII
      "1\u0BEF45319599",      // Unicode Tamil number 9
   ];

   public static TheoryData<String> CheckDigitUndetectableErrorValues =>
   [
      "1234569071",           // Valid NPI 1234560971 with two digit transposition 09 -> 90
      "1230967899",           // Valid NPI 1239067899 with two digit transposition 90 -> 09
      "1122334497",           // Valid NPI 1122334497 with two digit twin error 22 -> 55
      "1122337797",           // Valid NPI 1122334497 with two digit twin error 44 -> 77
      "1122664497",           // Valid NPI 1122334497 with two digit twin error 33 -> 66
   ];

   public static TheoryData<String> CheckDigitDetectableErrorValues =>
   [
      "1238560971",           // Valid NPI 1234560971 with single digit transcription error 4 -> 8
      "1243560971",           // Valid NPI 1234560971 with two digit transposition error 34 -> 43
      "4422334497",           // Valid NPI 1122334497 with two digit twin error 11 -> 44
   ];

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Constructor_ShouldCreateObject_WhenValueContainsValidNpi()
   {
      // Arrange.
      var npi = ValidNpi;

      // Act.
      var sut = new UsNationalProviderIdentifier(npi);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(npi);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueIsEmpty(String? npi)
      => FluentActions
         .Invoking(() => _ = new UsNationalProviderIdentifier(npi))
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiEmpty + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueHasInvalidLength(String? npi)
      => FluentActions
         .Invoking(() => _ = new UsNationalProviderIdentifier(npi))
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidLength + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueContainsNonAsciiDigit(String npi)
      => FluentActions
         .Invoking(() => _ = new UsNationalProviderIdentifier(npi))
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String npi)
   {
      // Act.
      var sut = new UsNationalProviderIdentifier(npi);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(npi);
   }

   [Theory]
   [MemberData(nameof(CheckDigitDetectableErrorValues))]
   public void UsNationalProviderIdentifier_Constructor_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenCheckDigitContainsDetectableError(String npi)
      => FluentActions
         .Invoking(() => _ = new UsNationalProviderIdentifier(npi))
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCheckDigit);

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNpi)]
   [InlineData(AltValidNpi)]
   public void UsNationalProviderIdentifier_Value_ShouldReturnRawNpi(String npi)
   {
      // Arrange.
      var sut = new UsNationalProviderIdentifier(npi);

      // Act/assert.
      sut.Value.Should().Be(npi);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var npi = ValidNpi;
      var sut = new UsNationalProviderIdentifier(npi);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(npi);
   }

   [Fact]
   public void UsNationalProviderIdentifier_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var npi = ValidNpi;
      var sut = new UsNationalProviderIdentifier(npi);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(npi);
   }

   [Fact]
   public void UsNationalProviderIdentifier_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      UsNationalProviderIdentifier npi = null!;

      // Act.
      String str = npi;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void UsNationalProviderIdentifier_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      UsNationalProviderIdentifier npi = null!;

      // Act.
      String str = npi;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void UsNationalProviderIdentifier_ExplicitCastToUsNpi_ShouldCreateObject_WhenValueContainsValidNpi()
   {
      // Arrange.
      var npi = ValidNpi;

      // Act.
      var sut = (UsNationalProviderIdentifier)npi;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(npi);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsNationalProviderIdentifier_ExplicitCastToUsNpi_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueIsEmpty(String str)
      => FluentActions
         .Invoking(() => _ = (UsNationalProviderIdentifier)str)
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiEmpty + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsNationalProviderIdentifier_ExplicitCastToUsNpi_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueHasInvalidLength(String str)
      => FluentActions
         .Invoking(() => _ = (UsNationalProviderIdentifier)str)
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidLength + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsNationalProviderIdentifier_ExplicitCastToUsNpi_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenValueContainsNonAsciiDigit(String str)
      => FluentActions
         .Invoking(() => _ = (UsNationalProviderIdentifier)str)
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void UsNationalProviderIdentifier_ExplicitCastToUsNpi_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String str)
   {
      // Act.
      var sut = (UsNationalProviderIdentifier)str;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(str);
   }

   [Theory]
   [MemberData(nameof(CheckDigitDetectableErrorValues))]
   public void UsNationalProviderIdentifier_ExplicitCastToUsNpi_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenCheckDigitContainsDetectableError(String str)
      => FluentActions
         .Invoking(() => _ = (UsNationalProviderIdentifier)str)
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCheckDigit);

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var npi1 = new UsNationalProviderIdentifier(ValidNpi);
      var npi2 = new UsNationalProviderIdentifier(ValidNpi);

      // Act/assert.
      (npi1 == npi2).Should().BeTrue();
   }

   [Fact]
   public void UsNationalProviderIdentifier_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var npi1 = new UsNationalProviderIdentifier(ValidNpi);
      var npi2 = new UsNationalProviderIdentifier(AltValidNpi);

      // Act/assert.
      (npi1 == npi2).Should().BeFalse();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var npi1 = new UsNationalProviderIdentifier(ValidNpi);
      var npi2 = new UsNationalProviderIdentifier(AltValidNpi);

      // Act/assert.
      (npi1 != npi2).Should().BeTrue();
   }

   [Fact]
   public void UsNationalProviderIdentifier_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var npi1 = new UsNationalProviderIdentifier(ValidNpi);
      var npi2 = new UsNationalProviderIdentifier(ValidNpi);

      // Act/assert.
      (npi1 != npi2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Create_ShouldCreateObject_WhenValueContainsValidNpi()
   {
      // Arrange.
      var npi = ValidNpi;
      var expected = new UsNationalProviderIdentifier(npi);

      // Act.
      var result = UsNationalProviderIdentifier.Create(npi);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expected);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsNationalProviderIdentifier_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String npi)
   {
      // Arrange.
      var expected = UsNationalProviderIdentifierValidationResult.Empty;

      // Act.
      var result = UsNationalProviderIdentifier.Create(npi);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().BeNull();
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsNationalProviderIdentifier_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String npi)
   {
      // Arrange.
      var expected = UsNationalProviderIdentifierValidationResult.InvalidLength;

      // Act.
      var result = UsNationalProviderIdentifier.Create(npi);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().BeNull();
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsNationalProviderIdentifier_Create_ShouldReturnInvalidCharacterEncounteredResult_WhenValueContainsNonAsciiDigit(String npi)
   {
      // Arrange.
      var expected = UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered;

      // Act.
      var result = UsNationalProviderIdentifier.Create(npi);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().BeNull();
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void UsNationalProviderIdentifier_Create_ShouldCreateObject_WhenCheckDigitContainsUndetectableError(String npi)
   {
      // Arrange.
      var expected = new UsNationalProviderIdentifier(npi);

      // Act.
      var result = UsNationalProviderIdentifier.Create(npi);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expected);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(CheckDigitDetectableErrorValues))]
   public void UsNationalProviderIdentifier_Create_ShouldReturnInvalidCheckDigit_WhenCheckDigitContainsDetectableError(String npi)
   {
      // Arrange.
      var expected = UsNationalProviderIdentifierValidationResult.InvalidCheckDigit;

      // Act.
      var result = UsNationalProviderIdentifier.Create(npi);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().BeNull();
      result.ValidationFailure.Should().Be(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var npi1 = new UsNationalProviderIdentifier(ValidNpi);
      var npi2 = new UsNationalProviderIdentifier(ValidNpi);    // Same internal value

      // Act/assert.
      npi1.Equals(npi2).Should().BeTrue();
   }

   [Fact]
   public void UsNationalProviderIdentifier_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var npi1 = new UsNationalProviderIdentifier(ValidNpi);
      var npi2 = new UsNationalProviderIdentifier(AltValidNpi);

      // Act/assert.
      npi1.Equals(npi2).Should().BeFalse();
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
      var npi1 = new UsNationalProviderIdentifier(ValidNpi);
      var npi2 = new UsNationalProviderIdentifier(ValidNpi);    // Same internal value

      // Act.
      var hash1 = npi1.GetHashCode();
      var hash2 = npi2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void UsNationalProviderIdentifier_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var npi1 = new UsNationalProviderIdentifier(ValidNpi);
      var npi2 = new UsNationalProviderIdentifier(AltValidNpi);

      // Act.
      var hash1 = npi1.GetHashCode();
      var hash2 = npi2.GetHashCode();

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
      var npi1 = new UsNationalProviderIdentifier(ValidNpi);
      var npi2 = new UsNationalProviderIdentifier(ValidNpi);    // Same internal value

      // Act/assert.
      (npi1 == npi2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(npi1, npi2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_ToString_ShouldReturnExpectedValue()
   {
      // Arrange.
      var npi = ValidNpi;
      var sut = new UsNationalProviderIdentifier(npi);

      // Act/assert.
      sut.ToString().Should().Be(npi);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnValidationPassed_WhenValueContainsValidNpi()
      => UsNationalProviderIdentifier.Validate(ValidNpi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.ValidationPassed);

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidCharacterEncountered_WhenValueContainsNonAsciiDigit(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(CheckDigitUndetectableErrorValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnValidationPassed_WhenCheckDigitContainsUndetectableError(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(CheckDigitDetectableErrorValues))]
   public void UsNationalProviderIdentifier_Validate_ShouldReturnInvalidCheckDigit_WhenCheckDigitContainsDetectableError(String npi)
      => UsNationalProviderIdentifier.Validate(npi)
         .Should().Be(UsNationalProviderIdentifierValidationResult.InvalidCheckDigit);

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
   public void UsNationalProviderIdentifier_JsonDeserialization_ShouldThrowInvalidUsNationalProviderIdentifierException_WhenNpiIsInvalid()
   {
      // Arrange.
      var json = "{\"Npi\":\"124531959\"}";  // Invalid length

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should()
         .ThrowExactly<InvalidUsNationalProviderIdentifierException>()
         .WithMessage(Messages.UsNpiInvalidLength + "*")
         .And.ValidationResult.Should().Be(UsNationalProviderIdentifierValidationResult.InvalidLength);
   }

   #endregion
}
