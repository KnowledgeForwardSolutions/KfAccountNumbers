// Ignore Spelling: Burgerservicenummer Deserialize Deserialization Json Kf

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class NlBurgerservicenummerTests
{
   private const String ValidBurgerservicenummer = "123456782";
   private const String AltValidBurgerservicenummer = "111222333";
   private const String ValidFormattedBurgerservicenummer = "1234-56-782";
   private const String AltValidFormattedBurgerservicenummer = "1112-22-333";
   private const String AltSeparatorCharBurgerservicenummer = "1638.97.426";

   private static String GetRawBurgerservicenummer(String burgerservicenummer)
      => burgerservicenummer.Length == 9
         ? burgerservicenummer
         : burgerservicenummer[..4] + burgerservicenummer[5..7] + burgerservicenummer[8..];

   public static TheoryData<String> ValidBurgerservicenummerValues =>
   [
      ValidBurgerservicenummer,
      AltValidBurgerservicenummer,
      ValidFormattedBurgerservicenummer,
      AltValidFormattedBurgerservicenummer,
      AltSeparatorCharBurgerservicenummer,
   ];

   public static TheoryData<String> ValidSeparatorValues =>
   [
      "1234-56-782",
      "1234 56 782",
      "1234.56.782",
      "1234~56~782",
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "12345678",          // Length 8
      "1112223334",        // Length 10
      "1234-56-78",        // Length 10
      "1234-56-7823",      // Length 12
      new String('1', 100) // Very long string
   ];

   public static TheoryData<String> InvalidCharacterValues =>
   [
      "A23456782",         // Non-digit character 'A'
      "1 3456782",         // Non-digit character ' '
      "12-456782",         // Non-digit character '-'
      "123=56782",         // Non-digit character '='
      "1234B6782",         // Non-digit character 'B'
      "12345C782",         // Non-digit character 'C'
      "123456a82",         // Non-digit character 'a'
      "1234567b2",         // Non-digit character 'b'
      "12345678~",         // Non-digit character '~'
      "12345678\u2153",    // Non-digit character Unicode fraction 1/3

      "A234 56 782",       // Non-digit character 'A'
      "1 34 56 782",       // Non-digit character ' '
      "12-4 56 782",       // Non-digit character '-'
      "123= 56 782",       // Non-digit character '='
      "1234 B6 782",       // Non-digit character 'B'
      "1234 5C 782",       // Non-digit character 'C'
      "1234 56 a82",       // Non-digit character 'a'
      "1234 56 7b2",       // Non-digit character 'b'
      "1234 56 78~",       // Non-digit character '~'
      "1234 56 78\u2153",  // Non-digit character Unicode fraction 1/3
   ];

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "122456782",         // 123456782 with single digit transcription error, 3 -> 2
      "111223333",         // 111222333 with single digit transcription error, 2 -> 3
      "123456783",         // 123456782 with check digit transcription error, 3 -> 2
      "124356782",         // 123456782 with two digit transposition error, 34 -> 43
      "112122333",         // 111222333 with two digit transposition error, 12 -> 21
      "123458762",         // 123456782 with two digit jump transposition, 678 -> 876
      "100222333",         // 111222333 with two digit twin error, 11 -> 00
      "111222344",         // 111222333 with two digit twin error, 33 -> 44
   ];

   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "1234056-782",
      "1234156-782",
      "1234256-782",
      "1234356-782",
      "1234456-782",
      "1234556-782",
      "1234656-782",
      "1234756-782",
      "1234856-782",
      "1234956-782",

      "1234-560782",
      "1234-561782",
      "1234-562782",
      "1234-563782",
      "1234-564782",
      "1234-565782",
      "1234-566782",
      "1234-567782",
      "1234-568782",
      "1234-569782",

      "1234-56.782",
      "1234.56-782",
   ];

   #region Check Digit Algorithm Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   // Values designed to produce all possible check digits 0-9
                                 // Weights: 9 8 7 6 5 4 3 2 -1 
   [InlineData("110000006")]     //          9 8             -6 = 11, mod 11 = 0             
   [InlineData("101000005")]     //          9   7           -5 
   [InlineData("100100004")]     //          9     6         -4
   [InlineData("100010003")]     //          9       5       -3
   [InlineData("100001002")]     //          9         4     -2
   [InlineData("100000101")]     //          9           3   -1
   [InlineData("110010000")]     //          9 8   6          0
   [InlineData("100110009")]     //          9     6 5       -9
   [InlineData("010110008")]     //            8   6 5       -8
   [InlineData("001110007")]     //              7 6 5       -7
   public void NlBurgerservicenummer_CheckDigitAlgorithm_ShouldValidateAllPossibleCheckDigits(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.ValidationPassed);

   #endregion

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidBurgerservicenummerValues))]
   public void NlBurgerservicenummer_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawBurgerservicenummer(value);

      // Act.
      var sut = new NlBurgerservicenummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparatorValues))]
   public void NlBurgerservicenummer_Constructor_ShouldCreateInstance_WhenValueHasValidSeparator(String value)
   {
      // Arrange.
      var expected = GetRawBurgerservicenummer(value);

      // Act.
      var sut = new NlBurgerservicenummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NlBurgerservicenummer_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
      => FluentActions
         .Invoking(() => new NlBurgerservicenummer(value))
         .Should().Throw<KfValidationException<NlBurgerservicenummerValidationResult>>()
         .WithMessage(Messages.NlBurgerservicenummerEmpty + "*")
         .And.ValidationResult.Should().Be(NlBurgerservicenummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NlBurgerservicenummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
      => FluentActions
         .Invoking(() => new NlBurgerservicenummer(value))
         .Should().Throw<KfValidationException<NlBurgerservicenummerValidationResult>>()
         .WithMessage(Messages.NlBurgerservicenummerInvalidLength + "*")
         .And.ValidationResult.Should().Be(NlBurgerservicenummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NlBurgerservicenummer_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(String value)
      => FluentActions
         .Invoking(() => new NlBurgerservicenummer(value))
         .Should().Throw<KfValidationException<NlBurgerservicenummerValidationResult>>()
         .WithMessage(Messages.NlBurgerservicenummerInvalidCharacter + "*")
         .And.ValidationResult.Should().Be(NlBurgerservicenummerValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NlBurgerservicenummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
      => FluentActions
         .Invoking(() => new NlBurgerservicenummer(value))
         .Should().Throw<KfValidationException<NlBurgerservicenummerValidationResult>>()
         .WithMessage(Messages.NlBurgerservicenummerInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(NlBurgerservicenummerValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void NlBurgerservicenummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String value)
      => FluentActions
         .Invoking(() => new NlBurgerservicenummer(value))
         .Should().Throw<KfValidationException<NlBurgerservicenummerValidationResult>>()
         .WithMessage(Messages.NlBurgerservicenummerInvalidSeparator + "*")
         .And.ValidationResult.Should().Be(NlBurgerservicenummerValidationResult.InvalidSeparator);

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidBurgerservicenummer, ValidBurgerservicenummer)]
   [InlineData(AltValidBurgerservicenummer, AltValidBurgerservicenummer)]
   [InlineData(ValidFormattedBurgerservicenummer, ValidBurgerservicenummer)]
   [InlineData(AltValidFormattedBurgerservicenummer, AltValidBurgerservicenummer)]
   public void NlBurgerservicenummer_Value_ShouldReturnValidatedBurgerservicenummer(
      String value,
      String expected)
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NlBurgerservicenummer_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidBurgerservicenummer;
      var sut = new NlBurgerservicenummer(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(value);
   }

   [Fact]
   public void NlBurgerservicenummer_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedBurgerservicenummer;
      var sut = new NlBurgerservicenummer(value);
      var expected = GetRawBurgerservicenummer(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Fact]
   public void NlBurgerservicenummer_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      NlBurgerservicenummer sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void NlBurgerservicenummer_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      NlBurgerservicenummer sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidBurgerservicenummerValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawBurgerservicenummer(value);

      // Act.
      var sut = (NlBurgerservicenummer)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparatorValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldCreateInstance_WhenValueHasValidSeparator(String value)
   {
      // Arrange.
      var expected = GetRawBurgerservicenummer(value);

      // Act.
      var sut = (NlBurgerservicenummer)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
      => FluentActions
         .Invoking(() => _ = (NlBurgerservicenummer)value)
         .Should().Throw<KfValidationException<NlBurgerservicenummerValidationResult>>()
         .WithMessage(Messages.NlBurgerservicenummerEmpty + "*")
         .And.ValidationResult.Should().Be(NlBurgerservicenummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
      => FluentActions
         .Invoking(() => _ = (NlBurgerservicenummer)value)
         .Should().Throw<KfValidationException<NlBurgerservicenummerValidationResult>>()
         .WithMessage(Messages.NlBurgerservicenummerInvalidLength + "*")
         .And.ValidationResult.Should().Be(NlBurgerservicenummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(String value)
      => FluentActions
         .Invoking(() => _ = (NlBurgerservicenummer)value)
         .Should().Throw<KfValidationException<NlBurgerservicenummerValidationResult>>()
         .WithMessage(Messages.NlBurgerservicenummerInvalidCharacter + "*")
         .And.ValidationResult.Should().Be(NlBurgerservicenummerValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
      => FluentActions
         .Invoking(() => _ = (NlBurgerservicenummer)value)
         .Should().Throw<KfValidationException<NlBurgerservicenummerValidationResult>>()
         .WithMessage(Messages.NlBurgerservicenummerInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(NlBurgerservicenummerValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void NlBurgerservicenummer_ExplicitCastToNlBurgerservicenummer_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String value)
      => FluentActions
         .Invoking(() => _ = (NlBurgerservicenummer)value)
         .Should().Throw<KfValidationException<NlBurgerservicenummerValidationResult>>()
         .WithMessage(Messages.NlBurgerservicenummerInvalidSeparator + "*")
         .And.ValidationResult.Should().Be(NlBurgerservicenummerValidationResult.InvalidSeparator);

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NlBurgerservicenummer_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidBurgerservicenummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void NlBurgerservicenummer_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(AltValidBurgerservicenummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void NlBurgerservicenummer_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NlBurgerservicenummer_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(AltValidBurgerservicenummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void NlBurgerservicenummer_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void NlBurgerservicenummer_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidBurgerservicenummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidBurgerservicenummerValues))]
   public void NlBurgerservicenummer_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expectedValue = new NlBurgerservicenummer(value);

      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(ValidSeparatorValues))]
   public void NlBurgerservicenummer_Create_ShouldCreateInstance_WhenValueHasValidSeparator(String value)
   {
      // Arrange.
      var expectedValue = new NlBurgerservicenummer(value);

      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NlBurgerservicenummer_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(NlBurgerservicenummerValidationResult.Empty);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NlBurgerservicenummer_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(NlBurgerservicenummerValidationResult.InvalidLength);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NlBurgerservicenummer_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacterWhereDigitExpected(String value)
   {
      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(NlBurgerservicenummerValidationResult.InvalidCharacter);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NlBurgerservicenummer_Create_ShouldReturnInvalidCheckDigitValidationResult_WhenValueHasInvalidCheckDigit(String value)
   {
      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(NlBurgerservicenummerValidationResult.InvalidCheckDigit);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void NlBurgerservicenummer_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(String value)
   {
      // Act.
      var result = NlBurgerservicenummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(NlBurgerservicenummerValidationResult.InvalidSeparator);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NlBurgerservicenummer_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidBurgerservicenummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void NlBurgerservicenummer_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(AltValidBurgerservicenummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void NlBurgerservicenummer_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NlBurgerservicenummer_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var expected = ValidFormattedBurgerservicenummer;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void NlBurgerservicenummer_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var mask = "_________";
      var expected = ValidBurgerservicenummer;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void NlBurgerservicenummer_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(ValidBurgerservicenummer);
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
   public void NlBurgerservicenummer_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(ValidBurgerservicenummer);
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
   public void NlBurgerservicenummer_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidBurgerservicenummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void NlBurgerservicenummer_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(AltValidBurgerservicenummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void NlBurgerservicenummer_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 9 and 11 character versions for same person should still be equal.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidFormattedBurgerservicenummer);

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

   // NlBurgerservicenummer does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void NlBurgerservicenummer_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new NlBurgerservicenummer(ValidBurgerservicenummer);
      var sut2 = new NlBurgerservicenummer(ValidBurgerservicenummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidBurgerservicenummerValues))]
   public void NlBurgerservicenummer_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(value);
      var expected = GetRawBurgerservicenummer(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidBurgerservicenummerValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(ValidSeparatorValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnValidationPassed_WhenValueHasValidSeparator(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.ValidationPassed);

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacterWhereDigitExpected(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void NlBurgerservicenummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String value)
      => NlBurgerservicenummer.Validate(value).Should().Be(NlBurgerservicenummerValidationResult.InvalidSeparator);

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void NlBurgerservicenummer_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(ValidBurgerservicenummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<NlBurgerservicenummer>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void NlBurgerservicenummer_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new NlBurgerservicenummer(AltValidFormattedBurgerservicenummer);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public NlBurgerservicenummer Burgerservicenummer { get; set; } = null!;
   }

   [Fact]
   public void NlBurgerservicenummer_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Burgerservicenummer = new NlBurgerservicenummer(AltValidBurgerservicenummer) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void NlBurgerservicenummer_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Burgerservicenummer\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void NlBurgerservicenummer_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Burgerservicenummer\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Burgerservicenummer.Should().BeNull();
   }

   [Fact]
   public void NlBurgerservicenummer_JsonDeserialization_ShouldThrowKfValidationException_WhenBurgerservicenummerIsInvalid()
   {
      // Arrange.
      var json = "{\"Burgerservicenummer\":\"100612-707079\"}";  // Invalid length

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should()
         .ThrowExactly<KfValidationException<NlBurgerservicenummerValidationResult>>()
         .WithMessage(Messages.NlBurgerservicenummerInvalidLength + "*")
         .And.ValidationResult.Should().Be(NlBurgerservicenummerValidationResult.InvalidLength);
   }

   #endregion
}
