// Ignore Spelling: Deserialization Deserialize itin Itin Json Kf Unformatted

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

namespace KfAccountNumbers.Tests.Unit.Governmental.NorthAmerica;

public class UsIndividualTaxpayerIdentificationNumberTests
{
   private const String ValidNineCharItin = "901501234";
   private const String AltValidNineCharItin = "987654321";
   private const String ValidElevenCharItin = "901-50-1234";
   private const String AltValidElevenCharItin = "987 65 4321";

   // Values that will successfully create a UsIndividualTaxpayerIdentificationNumber object
   public static TheoryData<String> ValidValues =>
   [
      ValidNineCharItin,
      ValidElevenCharItin,
      AltValidElevenCharItin
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
   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "901 50-1234",
      "901-50 1234",
      "901050 1234",
      "90105001234",
      "90115011234",
      "90125021234",
      "90135031234",
      "90145041234",
      "90155051234",
      "90165061234",
      "90175071234",
      "90185081234",
      "90195091234",
   ];

   // Values that will report an invalid character encountered
   public static TheoryData<String> InvalidCharacterValues =>
   [
      //"A01501234",    - will fail invalid area number validation before invalid character validation
      "9A1501234",
      "90A501234",
      "901A01234",
      "9015A1234",
      "90150A234",
      "901501A34",
      "9015012A4",
      "90150123A",
      "9;1501234",
      "9\u21531501234",       // Unicode fraction 1/3
      "9\u21671501234",       // Unicode Roman numeral VII
      "9\u0BEF1501234",       // Unicode Tamil number 9
      //"A01-50-1234",  - will fail invalid area number validation before invalid character validation
      "9A1-50-1234",
      "90A-50-1234",
      "901-A0-1234",
      "901-5A-1234",
      "901-50-A234",
      "901-50-1A34",
      "901-50-12A4",
      "901-50-123A",
      "9;1-50-1234",
      "9\u21531 50 1234",       // Unicode fraction 1/3
      "9\u21671 50 1234",       // Unicode Roman numeral VII
      "9\u0BEF1 50 1234",       // Unicode Tamil number 9
   ];

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

   /// <summary>
   /// Extracts unformatted ITIN value. If ITIN is 9 characters then value is
   /// returned unchanged. If an 11-character formatted ITIN then assumes
   /// separators at positions 3 and 6.
   /// </summary>
   private static String GetRawItin(String itin)
      => itin.Length switch
      {
         9 => itin,
         11 => itin[0..3] + itin[4..6] + itin[7..11],
         _ => throw new ArgumentException("Input must be 9 or 11 characters", nameof(itin))
      };

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldCreateObject_WhenValueContainsValidItin(String itin)
   {
      // Arrange.
      var expected = GetRawItin(itin);

      // Act.
      var sut = new UsIndividualTaxpayerIdentificationNumber(itin);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldThrowKfValidationException_WhenValueIsEmpty(String? itin)
      => FluentActions
         .Invoking(() => _ = new UsIndividualTaxpayerIdentificationNumber(itin))
         .Should()
         .ThrowExactly<KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>>()
         .WithMessage(Messages.UsItinEmpty + "*")
         .And.ValidationResult.Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String itin)
      => FluentActions
         .Invoking(() => _ = new UsIndividualTaxpayerIdentificationNumber(itin))
         .Should()
         .ThrowExactly<KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>>()
         .WithMessage(Messages.UsItinInvalidLength + "*")
         .And.ValidationResult.Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidAreaNumber(String itin)
      => FluentActions
         .Invoking(() => _ = new UsIndividualTaxpayerIdentificationNumber(itin))
         .Should()
         .ThrowExactly<KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>>()
         .WithMessage(Messages.UsItinInvalidAreaNumber + "*")
         .And.ValidationResult.Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidAreaNumber);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldThrowKfValidationException_When11CharacterValueContainsInvalidSeparator(String itin)
      => FluentActions
         .Invoking(() => _ = new UsIndividualTaxpayerIdentificationNumber(itin))
         .Should()
         .ThrowExactly<KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>>()
         .WithMessage(Messages.UsItinInvalidSeparatorEncountered + "*")
         .And.ValidationResult.Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldThrowKfValidationException_WhenValueContainsNonAsciiDigit(String itin)
      => FluentActions
         .Invoking(() => _ = new UsIndividualTaxpayerIdentificationNumber(itin))
         .Should()
         .ThrowExactly<KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>>()
         .WithMessage(Messages.UsItinInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidGroupNumber(String itin)
      => FluentActions
         .Invoking(() => _ = new UsIndividualTaxpayerIdentificationNumber(itin))
         .Should()
         .ThrowExactly<KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>>()
         .WithMessage(Messages.UsItinInvalidGroupNumber + "*")
         .And.ValidationResult.Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidGroupNumber);

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull(String itin)
   {
      // Arrange.
      var expected = GetRawItin(itin);
      var sut = new UsIndividualTaxpayerIdentificationNumber(itin);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void UsIndividualTaxpayerIdentificationNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull(String itin)
   {
      // Arrange.
      var expected = GetRawItin(itin);
      var sut = new UsIndividualTaxpayerIdentificationNumber(itin);

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
      UsIndividualTaxpayerIdentificationNumber itin = null!;

      // Act.
      String str = itin;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      UsIndividualTaxpayerIdentificationNumber itin = null!;

      // Act.
      var str = (String)itin;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   [MemberData(nameof(ValidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldCreateObject_WhenValueContainsValidItin(String str)
   {
      // Arrange.
      var expected = GetRawItin(str);

      // Act.
      var sut = (UsIndividualTaxpayerIdentificationNumber)str;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldThrowKfValidationException_WhenValueIsEmpty(String? str)
      => FluentActions
         .Invoking(() => _ = (UsIndividualTaxpayerIdentificationNumber)str)
         .Should()
         .ThrowExactly<KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>>()
         .WithMessage(Messages.UsItinEmpty + "*")
         .And.ValidationResult.Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String str)
      => FluentActions
         .Invoking(() => _ = (UsIndividualTaxpayerIdentificationNumber)str)
         .Should()
         .ThrowExactly<KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>>()
         .WithMessage(Messages.UsItinInvalidLength + "*")
         .And.ValidationResult.Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldThrowKfValidationException_WhenValueHasInvalidAreaNumber(String str)
      => FluentActions
         .Invoking(() => _ = (UsIndividualTaxpayerIdentificationNumber)str)
         .Should()
         .ThrowExactly<KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>>()
         .WithMessage(Messages.UsItinInvalidAreaNumber + "*")
         .And.ValidationResult.Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidAreaNumber);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldThrowKfValidationException_When11CharacterValueContainsInvalidSeparator(String str)
      => FluentActions
         .Invoking(() => _ = (UsIndividualTaxpayerIdentificationNumber)str)
         .Should()
         .ThrowExactly<KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>>()
         .WithMessage(Messages.UsItinInvalidSeparatorEncountered + "*")
         .And.ValidationResult.Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldThrowKfValidationException_WhenValueContainsNonAsciiDigit(String str)
      => FluentActions
         .Invoking(() => _ = (UsIndividualTaxpayerIdentificationNumber)str)
         .Should()
         .ThrowExactly<KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>>()
         .WithMessage(Messages.UsItinInvalidCharacterEncountered + "*")
         .And.ValidationResult.Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_ExplicitCastToUsItin_ShouldThrowKfValidationException_WhenValueHasInvalidGroupNumber(String str)
      => FluentActions
         .Invoking(() => _ = (UsIndividualTaxpayerIdentificationNumber)str)
         .Should()
         .ThrowExactly<KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>>()
         .WithMessage(Messages.UsItinInvalidGroupNumber + "*")
         .And.ValidationResult.Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidGroupNumber);

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var itin1 = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);
      var itin2 = new UsIndividualTaxpayerIdentificationNumber(ValidElevenCharItin);    // Same internal value

      // Act/assert.
      (itin1 == itin2).Should().BeTrue();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var itin1 = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);
      var itin2 = new UsIndividualTaxpayerIdentificationNumber(AltValidNineCharItin);

      // Act/assert.
      (itin1 == itin2).Should().BeFalse();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var itin1 = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);
      var itin2 = new UsIndividualTaxpayerIdentificationNumber(AltValidNineCharItin);

      // Act/assert.
      (itin1 != itin2).Should().BeTrue();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var itin1 = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);
      var itin2 = new UsIndividualTaxpayerIdentificationNumber(ValidElevenCharItin);    // Same internal value

      // Act/assert.
      (itin1 != itin2).Should().BeFalse();
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharItin)]
   [InlineData(ValidElevenCharItin)]
   public void UsIndividualTaxpayerIdentificationNumber_Value_ShouldReturnRawItin(String itin)
   {
      // Arrange.
      var expected = GetRawItin(itin);
      var sut = new UsIndividualTaxpayerIdentificationNumber(itin);

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
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldCreateObject_WhenValueContainsValidItin(String itin)
   {
      // Arrange.
      var expected = GetRawItin(itin);
      var expectedValue = new UsIndividualTaxpayerIdentificationNumber(expected);

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(itin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String? itin)
   {
      // Arrange.
      var expected = UsIndividualTaxpayerIdentificationNumberValidationResult.Empty;

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(itin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String itin)
   {
      // Arrange.
      var expected = UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidLength;

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(itin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldReturnInvalidAreaNumberValidationResult_WhenValueHasInvalidAreaNumber(String itin)
   {
      // Arrange.
      var expected = UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidAreaNumber;

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(itin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldReturnInvalidSeparatorCharacterValidationResult_When11CharacterValueContainsInvalidSeparator(String itin)
   {
      // Arrange.
      var expected = UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidSeparatorEncountered;

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(itin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueContainsNonAsciiDigit(String itin)
   {
      // Arrange.
      var expected = UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidCharacterEncountered;

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(itin);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Create_ShouldReturnInvalidGroupNumberValidationResult_WhenValueHasInvalidGroupNumber(String itin)
   {
      // Arrange.
      var expected = UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidGroupNumber;

      // Act.
      var result = UsIndividualTaxpayerIdentificationNumber.Create(itin);

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
   public void UsIndividualTaxpayerIdentificationNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var itin1 = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);
      var itin2 = new UsIndividualTaxpayerIdentificationNumber(ValidElevenCharItin);    // Same internal value

      // Act/assert.
      itin1.Equals(itin2).Should().BeTrue();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var itin1 = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);
      var itin2 = new UsIndividualTaxpayerIdentificationNumber(AltValidNineCharItin);

      // Act/assert.
      itin1.Equals(itin2).Should().BeFalse();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);

      // Act/assert.
      sut.Equals(ValidNineCharItin).Should().BeFalse();
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);

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
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);
      var expected = ValidElevenCharItin;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(AltValidNineCharItin);
      var mask = "___ __ ____";
      var expected = AltValidElevenCharItin;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);
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
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);

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
      var itin1 = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);
      var itin2 = new UsIndividualTaxpayerIdentificationNumber(ValidElevenCharItin);    // Same internal value

      // Act.
      var hash1 = itin1.GetHashCode();
      var hash2 = itin2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var itin1 = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);
      var itin2 = new UsIndividualTaxpayerIdentificationNumber(AltValidNineCharItin);

      // Act.
      var hash1 = itin1.GetHashCode();
      var hash2 = itin2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
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
      var itin1 = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);
      var itin2 = new UsIndividualTaxpayerIdentificationNumber(ValidElevenCharItin);    // Same internal value

      // Act/assert.
      (itin1 == itin2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(itin1, itin2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidNineCharItin, ValidNineCharItin)]
   [InlineData(ValidElevenCharItin, ValidNineCharItin)]
   public void UsIndividualTaxpayerIdentificationNumber_ToString_ShouldReturnExpectedValue(
      String itin,
      String expected)
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(itin);

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
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnValidationPassed_WhenValueContainsValidItin(String itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.ValidationPassed);

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnEmpty_WhenValueIsEmpty(String? itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidAreaNumberValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidAreaNumber_WhenValueHasInvalidAreaNumber(String itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidAreaNumber);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidSeparatorEncountered_When11CharacterValueContainsAnInvalidSeparator(String itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidSeparatorEncountered);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidCharacterEncountered_WhenValueContainsNonAsciiDigit(String itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidCharacterEncountered);

   [Theory]
   [MemberData(nameof(InvalidGroupNumberBoundaryValues))]
   public void UsIndividualTaxpayerIdentificationNumber_Validate_ShouldReturnInvalidGroupNumber_WhenValueHasInvalidGroupNumber(String itin)
      => UsIndividualTaxpayerIdentificationNumber.Validate(itin).Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidGroupNumber);

   #endregion

   #region Json Serialization/Deserialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);

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
      var sut = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin);

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{ValidNineCharItin}\"");  // Simple string, not object
   }

   public class Foo
   {
      public UsIndividualTaxpayerIdentificationNumber Itin { get; set; } = null!;
   }

   [Fact]
   public void UsIndividualTaxpayerIdentificationNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Itin = new UsIndividualTaxpayerIdentificationNumber(ValidNineCharItin) };
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
      var json = "{\"Itin\":\"123456789\"}";  // Invalid area number

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should()
         .ThrowExactly<KfValidationException<UsIndividualTaxpayerIdentificationNumberValidationResult>>()
         .WithMessage(Messages.UsItinInvalidAreaNumber + "*")
         .And.ValidationResult.Should().Be(UsIndividualTaxpayerIdentificationNumberValidationResult.InvalidAreaNumber);
   }

   #endregion
}
