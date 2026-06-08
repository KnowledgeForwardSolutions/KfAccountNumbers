// Ignore Spelling: Deserialize Deserialization Json Kf Nif

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

public class EsNifTests
{
   private const String Valid9CharacterDni = "12345678Z";
   private const String AltValid9CharacterDni = "50487563X";
   private const String Valid10CharacterDni = "12345678-Z";
   private const String AltValid10CharacterDni = "50487563 X";
   private const String Valid9CharacterNie = "X1234567L";
   private const String AltValid9CharacterNie = "Y7654321G";
   private const String Valid11CharacterNie = "X-1234567-L";
   private const String AltValid11CharacterNie = "Y 7654321 G";

   private static String GetRawNif(String nif)
      => nif.Length switch
      {
         9 => nif,
         10 => nif[..8] + nif[^1],
         11 => nif[0] + nif[2..9] + nif[^1],
         _ => throw new InvalidOperationException(),
      };

   public static TheoryData<String> ValidNifValues =>
   [
      Valid9CharacterDni,
      AltValid9CharacterDni,
      Valid10CharacterDni,
      AltValid10CharacterDni,
      Valid9CharacterNie,
      AltValid9CharacterNie,
      Valid11CharacterNie,
      AltValid11CharacterNie,
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "2345678Z",             // DNI Length 8
      "012345678-Z",          // DNI Length 11
      "X123567L",             // NIE Length 8
      "X1234567-L",           // NIE Length 10
      "X-01234567-L",         // NIE Length 12
      new String('1', 100)    // Very long string
   ];

   public static TheoryData<String> InvalidCharacterValues =>
   [
      "1 345678Z",               // Unformatted DNI, non-digit character ' '
      "1-345678Z",               // Unformatted DNI, non-digit character '-'
      "12=45678Z",               // Unformatted DNI, non-digit character '='
      "123A5678Z",               // Unformatted DNI, non-digit character 'A'
      "1234B678Z",               // Unformatted DNI, non-digit character 'B'
      "12345C78Z",               // Unformatted DNI, non-digit character 'C'
      "123456a8Z",               // Unformatted DNI, non-digit character 'a'
      "1234567bZ",               // Unformatted DNI, non-digit character 'b'
      "12345678~",               // Unformatted DNI, non-digit character '~'
      "12345678U",               // Unformatted DNI, invalid trailing character 'U'
      "12345678t",               // Unformatted DNI, invalid trailing character 't'
      "123456782",               // Unformatted DNI, invalid trailing character '2'
      "\u21532345678Z",          // Unformatted DNI, non-digit character Unicode fraction 1/3
      "1\u00D6345678Z",          // Unformatted DNI, invalid character unicode O with umlaut

      "1 345678-Z",              // Formatted DNI, non-digit character ' '
      "1-345678-Z",              // Formatted DNI, non-digit character '-'
      "12=45678-Z",              // Formatted DNI, non-digit character '='
      "123A5678-Z",              // Formatted DNI, non-digit character 'A'
      "1234B678-Z",              // Formatted DNI, non-digit character 'B'
      "12345C78-Z",              // Formatted DNI, non-digit character 'C'
      "123456a8-Z",              // Formatted DNI, non-digit character 'a'
      "1234567b Z",              // Formatted DNI, non-digit character 'b'
      "12345678 ~",              // Formatted DNI, non-digit character '~'
      "12345678 U",              // Formatted DNI, invalid trailing character 'U'
      "12345678 t",              // Formatted DNI, invalid trailing character 't'
      "12345678 2",              // Formatted DNI, invalid trailing character '2'
      "1\u2153345678 Z",         // Formatted DNI, non-digit character Unicode fraction 1/3
      "1\u00D6345678 Z",         // Formatted DNI, invalid character unicode O with umlaut

      "A1234567L",               // Unformatted NIE, invalid leading character 'A'
      "W1234567L",               // Unformatted NIE, invalid leading character 'B'
      "x1234567L",               // Unformatted NIE, invalid leading character 'x'
      "y1234567L",               // Unformatted NIE, invalid leading character 'y'
      "z1234567L",               // Unformatted NIE, invalid leading character 'z'
      "X-234567L",               // Unformatted NIE, non-digit character '-'
      "X1=34567L",               // Unformatted NIE, non-digit character '='
      "X12A4567L",               // Unformatted NIE, non-digit character 'A'
      "X123B567L",               // Unformatted NIE, non-digit character 'B'
      "X1234C67L",               // Unformatted NIE, non-digit character 'C'
      "X12345a7L",               // Unformatted NIE, non-digit character 'a'
      "X123456bL",               // Unformatted NIE, non-digit character 'b'
      "X1234567~",               // Unformatted NIE, non-digit character '~'
      "X1234567U",               // Unformatted NIE, invalid trailing character 'U'
      "X1234567t",               // Unformatted NIE, invalid trailing character 't'
      "X12345672",               // Unformatted NIE, invalid trailing character '2'
      "\u21531234567L",          // Unformatted NIE, non-digit character Unicode fraction 1/3
      "X\u00D6234567L",          // Unformatted NIE, invalid character unicode O with umlaut

      "A-1234567 L",             // Formatted NIE, invalid leading character 'A'
      "W-1234567 L",             // Formatted NIE, invalid leading character 'B'
      "x-1234567 L",             // Formatted NIE, invalid leading character 'x'
      "y-1234567 L",             // Formatted NIE, invalid leading character 'y'
      "z-1234567 L",             // Formatted NIE, invalid leading character 'z'
      "X--234567 L",             // Formatted NIE, non-digit character '-'
      "X-1=34567 L",             // Formatted NIE, non-digit character '='
      "X-12A4567 L",             // Formatted NIE, non-digit character 'A'
      "X 123B567 L",             // Formatted NIE, non-digit character 'B'
      "X 1234C67 L",             // Formatted NIE, non-digit character 'C'
      "X 12345a7 L",             // Formatted NIE, non-digit character 'a'
      "X 123456b L",             // Formatted NIE, non-digit character 'b'
      "X 1234567 ~",             // Formatted NIE, non-digit character '~'
      "X 1234567 U",             // Formatted NIE, invalid trailing character 'U'
      "X 1234567 t",             // Formatted NIE, invalid trailing character 't'
      "X 1234567 2",             // Formatted NIE, invalid trailing character '2'
      "\u2153 1234567 L",        // Formatted NIE, non-digit character Unicode fraction 1/3
      "X \u00D6234567 L",        // Formatted NIE, invalid character unicode O with umlaut
   ];

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "12245678Z",             // 12345678Z with single digit transcription error, 3 -> 2
      "50587563X",             // 50487563X with single digit transcription error, 4 -> 5
      "11223344C",             // 11223344B with check digit transcription error, B -> C
      "12354678Z",             // 12345678Z with two digit transposition error, 45 -> 54
      "50487653X",             // 50487563X with two digit transposition error, 56 -> 65
      "50784563X",             // 50487563X with two digit jump transposition, 487 -> 784
      "11224444B",             // 11223344B with two digit twin error, 33 -> 44
      "22223344B",             // 11223344B with two digit twin error, 11 -> 22

      "X1224567L",             // X1234567L with single digit transcription error, 3 -> 2
      "Y7655321G",             // Y7654321G with single digit transcription error, 4 -> 5
      "X1122334B",             // X1122334A with check digit transcription error, A -> B
      "X1235467L",             // X1234567L with two digit transposition error, 45 -> 54
      "Y7564321G",             // Y7654321G with two digit transposition error, 65 -> 56
      "X1432567L",             // X1234567L with two digit jump transposition, 234 -> 432
      "X1122444A",             // X1122334A with two digit twin error, 33 -> 44
      "X2222334A",             // X1122334A with two digit twin error, 11 -> 22

      "12245678-Z",            // 12345678Z with single digit transcription error, 3 -> 2
      "50587563-X",            // 50487563X with single digit transcription error, 4 -> 5
      "11223344-C",            // 11223344B with check digit transcription error, B -> C
      "12354678-Z",            // 12345678Z with two digit transposition error, 45 -> 54
      "50487653 X",            // 50487563X with two digit transposition error, 56 -> 65
      "50784563 X",            // 50487563X with two digit jump transposition, 487 -> 784
      "11224444 B",            // 11223344B with two digit twin error, 33 -> 44
      "22223344 B",            // 11223344B with two digit twin error, 11 -> 22

      "X-1224567-L",           // X1234567L with single digit transcription error, 3 -> 2
      "Y-7655321-G",           // Y7654321G with single digit transcription error, 4 -> 5
      "X-1122334-B",           // X1122334A with check digit transcription error, A -> B
      "X-1235467-L",           // X1234567L with two digit transposition error, 45 -> 54
      "Y 7564321 G",           // Y7654321G with two digit transposition error, 65 -> 56
      "X 1432567 L",           // X1234567L with two digit jump transposition, 234 -> 432
      "X 1122444 A",           // X1122334A with two digit twin error, 33 -> 44
      "X 2222334 A",           // X1122334A with two digit twin error, 11 -> 22
   ];

   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "123456780Z",        // Digit separator
      "123456781Z",
      "123456782Z",
      "123456783Z",
      "123456784Z",
      "123456785Z",
      "123456786Z",
      "123456787Z",
      "123456788Z",
      "123456789Z",

      "X01234567-L",
      "X11234567-L",
      "X21234567-L",
      "X31234567-L",
      "X41234567-L",
      "X51234567-L",
      "X61234567-L",
      "X71234567-L",
      "X81234567-L",
      "X91234567-L",

      "X-12345670L",
      "X-12345671L",
      "X-12345672L",
      "X-12345673L",
      "X-12345674L",
      "X-12345675L",
      "X-12345676L",
      "X-12345677L",
      "X-12345678L",
      "X-12345679L",

      "X-1234567 L",       // Different separator
      "X 1234567-L",
   ];

   #region Check Digit Algorithm Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   // Values below are valid DNI and NIE values, and designed to produce all
   // possible check characters. Valid check characters are not in alphabetic
   // sequence, so we want to confirm that all possible values are supported.
   [InlineData("00000023T")]
   [InlineData("00000001R")]
   [InlineData("00000002W")]
   [InlineData("00000003A")]
   [InlineData("00000004G")]
   [InlineData("00000005-M")]
   [InlineData("00000006-Y")]
   [InlineData("00000007-F")]
   [InlineData("00000008-P")]
   [InlineData("00000009-D")]
   [InlineData("00000010-X")]
   [InlineData("X0000011B")]
   [InlineData("X0000012N")]
   [InlineData("X0000013J")]
   [InlineData("X0000014Z")]
   [InlineData("X0000015S")]
   [InlineData("X0000016Q")]
   [InlineData("X-0000017-V")]
   [InlineData("X-0000018-H")]
   [InlineData("X-0000019-L")]
   [InlineData("X-0000020-C")]
   [InlineData("X-0000021-K")]
   [InlineData("X-0000022-E")]
   public void EsNif_CheckDigitAlgorithm_ShouldGenerateAllPossibleCharacters(String value)
      => EsNif.Validate(value).Should().Be(EsNifValidationResult.ValidationPassed);

   [Theory]
   [InlineData("00000000T")]     // Minimum DNI
   [InlineData("99999999R")]     // Maximum DNI
   [InlineData("X0000000T")]     // Minimum X NIE
   [InlineData("X9999999J")]     // Maximum X NIE
   [InlineData("Y0000000Z")]     // Minimum Y NIE
   [InlineData("Y9999999G")]     // Maximum Y NIE
   [InlineData("Z0000000M")]     // Minimum Z NIE
   [InlineData("Z9999999H")]     // Maximum Z NIE
   public void EsNif_CheckDigitAlgorithm_ShouldHandleBoundaryValues(String value)
      => EsNif.Validate(value).Should().Be(EsNifValidationResult.ValidationPassed);

   #endregion

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidNifValues))]
   public void EsNif_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawNif(value);

      // Act.
      var sut = new EsNif(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void EsNif_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
      => FluentActions
         .Invoking(() => new EsNif(value))
         .Should().Throw<KfValidationException<EsNifValidationResult>>()
         .WithMessage(Messages.EsNifEmpty + "*")
         .And.ValidationResult.Should().Be(EsNifValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void EsNif_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
      => FluentActions
         .Invoking(() => new EsNif(value))
         .Should().Throw<KfValidationException<EsNifValidationResult>>()
         .WithMessage(Messages.EsNifInvalidLength + "*")
         .And.ValidationResult.Should().Be(EsNifValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void EsNif_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCharacter(String value)
      => FluentActions
         .Invoking(() => new EsNif(value))
         .Should().Throw<KfValidationException<EsNifValidationResult>>()
         .WithMessage(Messages.EsNifInvalidCharacter + "*")
         .And.ValidationResult.Should().Be(EsNifValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void EsNif_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
      => FluentActions
         .Invoking(() => new EsNif(value))
         .Should().Throw<KfValidationException<EsNifValidationResult>>()
         .WithMessage(Messages.EsNifInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(EsNifValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void EsNif_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String value)
      => FluentActions
         .Invoking(() => new EsNif(value))
         .Should().Throw<KfValidationException<EsNifValidationResult>>()
         .WithMessage(Messages.EsNifInvalidSeparator + "*")
         .And.ValidationResult.Should().Be(EsNifValidationResult.InvalidSeparator);

   #endregion

   #region IdentifierType Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(Valid9CharacterDni, EsIdentifierType.Dni)]
   [InlineData(Valid10CharacterDni, EsIdentifierType.Dni)]
   [InlineData(Valid9CharacterNie, EsIdentifierType.Nie)]
   [InlineData(Valid11CharacterNie, EsIdentifierType.Nie)]
   public void EsNif_IdentifierType_ShouldReturnExpectedValue(
      String value,
      EsIdentifierType expectedIdentifierType)
   {
      // Arrange.
      var sut = new EsNif(value);

      // Act/assert.
      sut.IdentifierType.Should().Be(expectedIdentifierType);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(Valid9CharacterDni, Valid9CharacterDni)]
   [InlineData(Valid10CharacterDni, Valid9CharacterDni)]
   [InlineData(Valid9CharacterNie, Valid9CharacterNie)]
   [InlineData(Valid11CharacterNie, Valid9CharacterNie)]
   public void EsNif_Value_ShouldReturnValidatedNif(
      String value,
      String expected)
   {
      // Arrange.
      var sut = new EsNif(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void EsNif_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = Valid9CharacterDni;
      var sut = new EsNif(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(value);
   }

   [Fact]
   public void EsNif_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = Valid11CharacterNie;
      var sut = new EsNif(value);
      var expected = GetRawNif(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Fact]
   public void EsNif_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      EsNif sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void EsNif_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      EsNif sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidNifValues))]
   public void EsNif_ExplicitCastToEsNif_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawNif(value);

      // Act.
      var sut = (EsNif)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void EsNif_ExplicitCastToEsNif_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
      => FluentActions
         .Invoking(() => _ = (EsNif)value)
         .Should().Throw<KfValidationException<EsNifValidationResult>>()
         .WithMessage(Messages.EsNifEmpty + "*")
         .And.ValidationResult.Should().Be(EsNifValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void EsNif_ExplicitCastToEsNif_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
      => FluentActions
         .Invoking(() => _ = (EsNif)value)
         .Should().Throw<KfValidationException<EsNifValidationResult>>()
         .WithMessage(Messages.EsNifInvalidLength + "*")
         .And.ValidationResult.Should().Be(EsNifValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void EsNif_ExplicitCastToEsNif_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(String value)
      => FluentActions
         .Invoking(() => _ = (EsNif)value)
         .Should().Throw<KfValidationException<EsNifValidationResult>>()
         .WithMessage(Messages.EsNifInvalidCharacter + "*")
         .And.ValidationResult.Should().Be(EsNifValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void EsNif_ExplicitCastToEsNif_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
      => FluentActions
         .Invoking(() => _ = (EsNif)value)
         .Should().Throw<KfValidationException<EsNifValidationResult>>()
         .WithMessage(Messages.EsNifInvalidCheckDigit + "*")
         .And.ValidationResult.Should().Be(EsNifValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void EsNif_ExplicitCastToEsNif_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String value)
      => FluentActions
         .Invoking(() => _ = (EsNif)value)
         .Should().Throw<KfValidationException<EsNifValidationResult>>()
         .WithMessage(Messages.EsNifInvalidSeparator + "*")
         .And.ValidationResult.Should().Be(EsNifValidationResult.InvalidSeparator);

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void EsNif_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new EsNif(Valid9CharacterDni);
      var sut2 = new EsNif(Valid9CharacterDni);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void EsNif_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new EsNif(Valid9CharacterDni);
      var sut2 = new EsNif(AltValid9CharacterDni);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void EsNif_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. formatted and unformatted versions for same person should still be equal.
      var sut1 = new EsNif(Valid9CharacterDni);
      var sut2 = new EsNif(Valid10CharacterDni);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void EsNif_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new EsNif(Valid9CharacterNie);
      var sut2 = new EsNif(AltValid9CharacterNie);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void EsNif_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. formatted and unformatted versions for same person should still be equal.
      var sut1 = new EsNif(Valid9CharacterNie);
      var sut2 = new EsNif(Valid11CharacterNie);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void EsNif_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new EsNif(Valid9CharacterNie);
      var sut2 = new EsNif(Valid9CharacterNie);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidNifValues))]
   public void EsNif_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expectedValue = new EsNif(value);

      // Act.
      var result = EsNif.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void EsNif_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Act.
      var result = EsNif.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(EsNifValidationResult.Empty);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void EsNif_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Act.
      var result = EsNif.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(EsNifValidationResult.InvalidLength);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void EsNif_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasInvalidCharacter(String value)
   {
      // Act.
      var result = EsNif.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(EsNifValidationResult.InvalidCharacter);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void EsNif_Create_ShouldReturnInvalidCheckDigitsValidationResult_WhenValueHasInvalidCheckDigits(String value)
   {
      // Act.
      var result = EsNif.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(EsNifValidationResult.InvalidCheckDigit);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void EsNif_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(String value)
   {
      // Act.
      var result = EsNif.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(EsNifValidationResult.InvalidSeparator);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void EsNif_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new EsNif(Valid9CharacterDni);
      var sut2 = new EsNif(Valid9CharacterDni);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void EsNif_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new EsNif(Valid9CharacterNie);
      var sut2 = new EsNif(AltValid9CharacterNie);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void EsNif_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. formatted and unformatted versions for same person should still be equal.
      var sut1 = new EsNif(Valid9CharacterDni);
      var sut2 = new EsNif(Valid10CharacterDni);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(Valid9CharacterDni, Valid10CharacterDni)]
   [InlineData(Valid9CharacterNie, Valid11CharacterNie)]
   public void EsNif_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed(
      String value,
      String expected)
   {
      // Arrange.
      var sut = new EsNif(value);

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void EsNif_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new EsNif(Valid11CharacterNie);
      var mask = "_________";
      var expected = Valid9CharacterNie;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Theory]
   [InlineData("")]
   [InlineData("\t")]
   public void EsNif_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new EsNif(Valid11CharacterNie);
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
   public void EsNif_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new EsNif(Valid9CharacterNie);
      var sut2 = new EsNif(Valid9CharacterNie);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void EsNif_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new EsNif(Valid9CharacterDni);
      var sut2 = new EsNif(AltValid9CharacterDni);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void EsNif_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. formatted and unformatted versions for same person should still be equal.
      var sut1 = new EsNif(Valid9CharacterDni);
      var sut2 = new EsNif(Valid10CharacterDni);

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

   // EsNif does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void EsNif_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new EsNif(Valid9CharacterDni);
      var sut2 = new EsNif(Valid9CharacterDni);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidNifValues))]
   public void EsNif_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new EsNif(value);
      var expected = GetRawNif(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidNifValues))]
   public void EsNif_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => EsNif.Validate(value).Should().Be(EsNifValidationResult.ValidationPassed);

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void EsNif_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => EsNif.Validate(value).Should().Be(EsNifValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void EsNif_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => EsNif.Validate(value).Should().Be(EsNifValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void EsNif_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidCharacter(String value)
      => EsNif.Validate(value).Should().Be(EsNifValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void EsNif_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String value)
      => EsNif.Validate(value).Should().Be(EsNifValidationResult.InvalidCheckDigit);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void EsNif_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String value)
      => EsNif.Validate(value).Should().Be(EsNifValidationResult.InvalidSeparator);

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void EsNif_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new EsNif(Valid9CharacterDni);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<EsNif>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void EsNif_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new EsNif(AltValid11CharacterNie);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public EsNif Nif { get; set; } = null!;
   }

   [Fact]
   public void EsNif_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Nif = new EsNif(Valid9CharacterNie) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void EsNif_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Nif\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void EsNif_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Nif\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Nif.Should().BeNull();
   }

   [Fact]
   public void EsNif_JsonDeserialization_ShouldThrowKfValidationException_WhenNifIsInvalid()
   {
      // Arrange.
      var json = "{\"Nif\":\"012345678-Z\"}";  // Invalid length

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should()
         .ThrowExactly<KfValidationException<EsNifValidationResult>>()
         .WithMessage(Messages.EsNifInvalidLength + "*")
         .And.ValidationResult.Should().Be(EsNifValidationResult.InvalidLength);
   }

   #endregion
}
