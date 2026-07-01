// Ignore Spelling: Deserialize Deserialization Json Kf Nif

using LocalCreateResult = KfAccountNumbers.Results.UCreateResult<
   KfAccountNumbers.Governmental.Europe.EsNif,
   KfAccountNumbers.Governmental.Europe.EsNif.ValidationError>;
using LocalValidationError = KfAccountNumbers.Governmental.Europe.EsNif.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.Governmental.Europe.EsNif.ValidationError>;
using LocalValidationResult = KfAccountNumbers.Governmental.Europe.EsNif.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

public class EsNifTests
{
   private const String ValidUnformattedDni = "12345678Z";
   private const String ValidUnformattedLowercaseDni = "12345678z";
   private const String AltValidUnformattedDni = "50487563X";
   private const String ValidFormattedDni = "12345678-Z";
   private const String ValidFormattedLowercaseDni = "12345678-z";
   private const String AltValidFormattedDni = "50487563 X";
   private const String ValidUnformattedNie = "X1234567L";
   private const String ValidUnformattedLowercaseNie = "x1234567l";
   private const String AltValidUnformattedNie = "Y7654321G";
   private const String ValidFormattedNie = "X-1234567-L";
   private const String ValidFormattedLowercaseNie = "x-1234567-l";
   private const String AltValidFormattedNie = "Y 7654321 G";

   private static String GetRawNif(String nif)
      => nif.Length switch
      {
         9 => nif.ToUpperInvariant(),
         10 => (nif[..8] + nif[^1]).ToUpperInvariant(),
         11 => (nif[0] + nif[2..9] + nif[^1]).ToUpperInvariant(),
         _ => throw new InvalidOperationException(),
      };

   public static TheoryData<String> ValidNifValues =>
   [
      ValidUnformattedDni,
      ValidUnformattedLowercaseDni,
      AltValidUnformattedDni,
      ValidFormattedDni,
      ValidFormattedLowercaseDni,
      AltValidFormattedDni,
      ValidUnformattedNie,
      ValidUnformattedLowercaseNie,
      AltValidUnformattedNie,
      ValidFormattedNie,
      ValidFormattedLowercaseNie,
      AltValidFormattedNie,
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "2345678Z",             // DNI Length 8
      "X123567L",             // NIE Length 8
      "X-01234567-L",         // NIE Length 12
      new String('1', 100)    // Very long string
   ];

   public static TheoryData<String> InvalidLengthForTypeValues =>
   [
      "1-2345678-Z",          // DNI Length 11
      "X1234567-L",           // NIE Length 10
   ];

   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted DNI
      { ".2345678Z", 0 },          // Unformatted DNI, non-digit character '.'
      { "1 345678Z", 1 },          // Unformatted DNI, non-digit character ' '
      { "12A45678Z", 2 },          // Unformatted DNI, non-digit character 'A'
      { "123Z5678Z", 3 },          // Unformatted DNI, non-digit character 'Z'
      { "1234^678Z", 4 },          // Unformatted DNI, non-digit character '^'
      { "12345a78Z", 5 },          // Unformatted DNI, non-digit character 'a'
      { "123456z8Z", 6 },          // Unformatted DNI, non-digit character 'z'
      { "1234567~Z", 7 },          // Unformatted DNI, non-digit character '~'
      { "12345678\u0BE6", 8 },     // Unformatted DNI, invalid character unicode Tamil digit 0
      { "12345678U", 8 },          // Unformatted DNI, invalid trailing character 'U'
      { "123456782", 8 },          // Unformatted DNI, invalid trailing character '2'
      { "\u21532345678Z", 0 },     // Unformatted DNI, non-digit character Unicode fraction 1/3
      { "1\u00D6345678Z", 1 },     // Unformatted DNI, invalid character unicode O with umlaut

      // Formatted DNI
      { ".2345678-Z", 0 },         // Formatted DNI, non-digit character '.'
      { "1 345678-Z", 1 },         // Formatted DNI, non-digit character ' '
      { "12A45678-Z", 2 },         // Formatted DNI, non-digit character 'A'
      { "123Z5678-Z", 3 },         // Formatted DNI, non-digit character 'Z'
      { "1234^678-Z", 4 },         // Formatted DNI, non-digit character '^'
      { "12345a78-Z", 5 },         // Formatted DNI, non-digit character 'a'
      { "123456z8-Z", 6 },         // Formatted DNI, non-digit character 'z'
      { "1234567~ Z", 7 },         // Formatted DNI, non-digit character '~'
      { "12345678 \u0BE6", 9 },    // Formatted DNI, invalid character unicode Tamil digit 0
      { "12345678 U", 9 },         // Formatted DNI, invalid trailing character 'U'
      { "12345678 2", 9 },         // Formatted DNI, invalid trailing character '2'
      { "1\u2153345678 Z", 1 },    // Formatted DNI, non-digit character Unicode fraction 1/3
      { "1\u00D6345678 Z", 1 },    // Formatted DNI, invalid character unicode O with umlaut

      // Unformatted NIE
      { "A1234567L", 0 },          // Unformatted NIE, invalid leading character 'A'
      { "w1234567L", 0 },          // Unformatted NIE, invalid leading character 'w'
      { "a1234567L", 0 },          // Unformatted NIE, invalid leading character 'a'
      { "X 234567L", 1 },          // Unformatted NIE, non-digit character ' '
      { "X1A34567L", 2 },          // Unformatted NIE, non-digit character 'A'
      { "X12Z4567L", 3 },          // Unformatted NIE, non-digit character 'Z'
      { "X123^567L", 4 },          // Unformatted NIE, non-digit character '^'
      { "X1234a67L", 5 },          // Unformatted NIE, non-digit character 'a'
      { "X12345z7L", 6 },          // Unformatted NIE, non-digit character 'z'
      { "X123456~L", 7 },          // Unformatted NIE, non-digit character '~'
      { "X1234567\u0BE6", 8 },     // Unformatted NIE, invalid character unicode Tamil digit 0
      { "X1234567U", 8 },          // Unformatted NIE, invalid trailing character 'U'
      { "X12345672", 8 },          // Unformatted NIE, invalid trailing character '2'
      { "\u21531234567L", 0 },     // Unformatted NIE, non-digit character Unicode fraction 1/3
      { "X\u00D6234567L", 1 },     // Unformatted NIE, invalid character unicode O with umlaut

      // Formatted NIE
      { "A-1234567 L", 0 },         // Formatted NIE, invalid leading character 'A'
      { "w-1234567 L", 0 },         // Formatted NIE, invalid leading character 'W'
      { "a-1234567 L", 0 },         // Formatted NIE, invalid leading character 'a'
      { ".-1234567 L", 0 },         // Formatted NIE, non-digit character '.'
      { "X- 234567 L", 2 },         // Formatted NIE, non-digit character ' '
      { "X-1A34567 L", 3 },         // Formatted NIE, non-digit character 'A'
      { "X-12Z4567 L", 4 },         // Formatted NIE, non-digit character 'Z'
      { "X 123^567 L", 5 },         // Formatted NIE, non-digit character '^'
      { "X 1234a67 L", 6 },         // Formatted NIE, non-digit character 'a'
      { "X 12345z7 L", 7 },         // Formatted NIE, non-digit character 'z'
      { "X 123456~ L", 8 },         // Formatted NIE, non-digit character '~'
      { "X 1234567 \u0BE6", 10 },   // Formatted NIE, invalid character unicode Tamil digit 0
      { "X 1234567 U", 10 },        // Formatted NIE, invalid trailing character 'U'
      { "X 1234567 2", 10 },        // Formatted NIE, invalid trailing character '2'
      { "\u2153 1234567 L", 0 },    // Formatted NIE, non-digit character Unicode fraction 1/3
      { "X \u00D6234567 L", 2 },    // Formatted NIE, invalid character unicode O with umlaut
   };

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

   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      // Digit separator in DNI separator location
      { "123456780Z", 8 },
      { "123456781Z", 8 },
      { "123456782Z", 8 },
      { "123456783Z", 8 },
      { "123456784Z", 8 },
      { "123456785Z", 8 },
      { "123456786Z", 8 },
      { "123456787Z", 8 },
      { "123456788Z", 8 },
      { "123456789Z", 8 },

      // Digit separator in first NIE separator location
      { "X01234567-L", 1 },
      { "X11234567-L", 1 },
      { "X21234567-L", 1 },
      { "X31234567-L", 1 },
      { "X41234567-L", 1 },
      { "X51234567-L", 1 },
      { "X61234567-L", 1 },
      { "X71234567-L", 1 },
      { "X81234567-L", 1 },
      { "X91234567-L", 1 },

      // Digit separator in second NIE separator location
      { "X-12345670L", 9 },
      { "X-12345671L", 9 },
      { "X-12345672L", 9 },
      { "X-12345673L", 9 },
      { "X-12345674L", 9 },
      { "X-12345675L", 9 },
      { "X-12345676L", 9 },
      { "X-12345677L", 9 },
      { "X-12345678L", 9 },
      { "X-12345679L", 9 },

      // Mixed separators in NIE
      { "X-1234567 L", 9 },
      { "X 1234567-L", 9 },
   };

   private static InvalidLength GetInvalidLengthResult(
      String value,
      String? message = null)
      => new(
         message ?? Messages.EsNifInvalidLength,
         value.Length,
         EsNif.GetValidLengthDefinitions());

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.EsNifInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.EsNifInvalidCheckDigit,
         EsNif.CheckDigitAlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(Messages.EsNifInvalidSeparator, value[position], position);

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

   // Lowercase.
   [InlineData("00000023t")]
   [InlineData("00000001r")]
   [InlineData("00000002w")]
   [InlineData("00000003a")]
   [InlineData("00000004g")]
   [InlineData("00000005-m")]
   [InlineData("00000006-y")]
   [InlineData("00000007-f")]
   [InlineData("00000008-p")]
   [InlineData("00000009-d")]
   [InlineData("00000010-x")]
   [InlineData("X0000011b")]
   [InlineData("X0000012n")]
   [InlineData("X0000013j")]
   [InlineData("X0000014z")]
   [InlineData("X0000015s")]
   [InlineData("X0000016q")]
   [InlineData("x-0000017-v")]
   [InlineData("x-0000018-h")]
   [InlineData("x-0000019-l")]
   [InlineData("x-0000020-c")]
   [InlineData("x-0000021-k")]
   [InlineData("x-0000022-e")]
   public void EsNif_CheckDigitAlgorithm_ShouldGenerateAllPossibleCharacters(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = EsNif.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData("00000000T")]     // Minimum DNI
   [InlineData("99999999R")]     // Maximum DNI
   [InlineData("X0000000T")]     // Minimum X NIE
   [InlineData("X9999999J")]     // Maximum X NIE
   [InlineData("Y0000000Z")]     // Minimum Y NIE
   [InlineData("Y9999999G")]     // Maximum Y NIE
   [InlineData("Z0000000M")]     // Minimum Z NIE
   [InlineData("Z9999999H")]     // Maximum Z NIE
   [InlineData("00000000t")]     // Minimum DNI
   [InlineData("99999999r")]     // Maximum DNI
   [InlineData("x0000000t")]     // Minimum X NIE
   [InlineData("x9999999j")]     // Maximum X NIE
   [InlineData("y0000000z")]     // Minimum Y NIE
   [InlineData("y9999999g")]     // Maximum Y NIE
   [InlineData("z0000000m")]     // Minimum Z NIE
   [InlineData("z9999999h")]     // Maximum Z NIE
   public void EsNif_CheckDigitAlgorithm_ShouldHandleBoundaryValues(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = EsNif.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Constants Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void EsNif_CheckDigitAlgorithmName_ShouldHaveExpectedValue()
      => EsNif.CheckDigitAlgorithmName.Should().Be("Modulus 23");

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
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new EsNif(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void EsNif_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new EsNif(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidLengthForTypeValues))]
   public void EsNif_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLengthForType(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(
         value,
         Messages.EsNifInvalidLengthForType);

      // Act/assert.
      FluentActions
         .Invoking(() => new EsNif(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void EsNif_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new EsNif(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void EsNif_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new EsNif(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void EsNif_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new EsNif(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region IdentifierType Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidUnformattedDni)]
   [InlineData(ValidFormattedDni)]
   public void EsNif_IdentifierType_ShouldReturnExpectedValue_WhenValueIsDni(String value)
   {
      // Arrange.
      var sut = new EsNif(value);
      EsNif.IdentifierCategory expected = default(EsIdentifierType.Dni);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   [Theory]
   [InlineData(ValidUnformattedNie)]
   [InlineData(ValidFormattedNie)]
   public void EsNif_IdentifierType_ShouldReturnExpectedValue_WhenValueIsNie(String value)
   {
      // Arrange.
      var sut = new EsNif(value);
      EsNif.IdentifierCategory expected = default(EsIdentifierType.Nie);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidNifValues))]
   public void EsNif_Value_ShouldReturnValidatedNif(String value)
   {
      // Arrange.
      var sut = new EsNif(value);
      var expected = GetRawNif(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [InlineData(ValidUnformattedLowercaseDni)]
   [InlineData(ValidUnformattedLowercaseNie)]
   [InlineData(ValidFormattedLowercaseDni)]
   [InlineData(ValidFormattedLowercaseNie)]
   public void EsNif_Value_ShouldReturnNormalizedUppercaseNif(String value)
   {
      // Arrange.
      var sut = new EsNif(value);
      var expected = GetRawNif(value).ToUpperInvariant();

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
      var value = ValidUnformattedDni;
      var sut = new EsNif(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void EsNif_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedNie;
      var sut = new EsNif(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void EsNif_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      EsNif sut = null!;

      // Act.
      String str = sut;

      // Assert.
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

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidNifValues))]
   public void EsNif_ExplicitCastToEsNif_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new EsNif(value);

      // Act.
      var sut = (EsNif)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void EsNif_ExplicitCastToEsNif_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (EsNif)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void EsNif_ExplicitCastToEsNif_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (EsNif)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void EsNif_ExplicitCastToEsNif_ShouldThrowKfValidationException_WhenValueHasInvalidCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (EsNif)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void EsNif_ExplicitCastToEsNif_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (EsNif)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void EsNif_ExplicitCastToEsNif_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (EsNif)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void EsNif_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new EsNif(ValidUnformattedDni);
      var sut2 = new EsNif(ValidUnformattedDni);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void EsNif_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new EsNif(ValidUnformattedDni);
      var sut2 = new EsNif(AltValidUnformattedDni);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void EsNif_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. formatted and unformatted versions for same person should still be equal.
      var sut1 = new EsNif(ValidUnformattedDni);
      var sut2 = new EsNif(ValidFormattedDni);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void EsNif_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new EsNif(ValidFormattedDni);
      var sut2 = new EsNif(ValidFormattedDni.Replace('-', '.'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void EsNif_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new EsNif(ValidFormattedDni.Replace('-', 'A'));
      var sut2 = new EsNif(ValidFormattedDni.Replace('-', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void EsNif_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyByCase()
   {
      // Arrange.
      var sut1 = new EsNif(ValidFormattedDni);
      var sut2 = new EsNif(ValidFormattedLowercaseDni);

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
      var sut1 = new EsNif(ValidUnformattedNie);
      var sut2 = new EsNif(AltValidUnformattedNie);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void EsNif_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. formatted and unformatted versions for same person should still be equal.
      var sut1 = new EsNif(ValidUnformattedNie);
      var sut2 = new EsNif(ValidFormattedNie);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void EsNif_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new EsNif(ValidUnformattedNie);
      var sut2 = new EsNif(ValidUnformattedNie);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void EsNif_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new EsNif(ValidFormattedDni);
      var sut2 = new EsNif(ValidFormattedDni.Replace('-', '.'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void EsNif_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new EsNif(ValidFormattedDni.Replace('-', 'A'));
      var sut2 = new EsNif(ValidFormattedDni.Replace('-', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void EsNif_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyByCase()
   {
      // Arrange.
      var sut1 = new EsNif(ValidFormattedDni);
      var sut2 = new EsNif(ValidFormattedLowercaseDni);

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
      LocalCreateResult expected = new EsNif(value);

      // Act.
      var result = EsNif.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void EsNif_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = EsNif.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void EsNif_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = EsNif.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void EsNif_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasInvalidCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = EsNif.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void EsNif_Create_ShouldReturnInvalidCheckDigitsValidationResult_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = EsNif.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void EsNif_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = EsNif.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void EsNif_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new EsNif(ValidUnformattedDni);
      var sut2 = new EsNif(ValidUnformattedDni);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void EsNif_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new EsNif(ValidUnformattedNie);
      var sut2 = new EsNif(AltValidUnformattedNie);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void EsNif_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. formatted and unformatted versions for same person should still be equal.
      var sut1 = new EsNif(ValidUnformattedDni);
      var sut2 = new EsNif(ValidFormattedDni);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void EsNif_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new EsNif(ValidFormattedDni);
      var sut2 = new EsNif(ValidFormattedDni.Replace('-', '.'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void EsNif_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new EsNif(ValidFormattedDni.Replace('-', 'A'));
      var sut2 = new EsNif(ValidFormattedDni.Replace('-', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void EsNif_Equals_ShouldReturnTrue_WhenValuesDifferOnlyByCase()
   {
      // Arrange.
      var sut1 = new EsNif(ValidFormattedDni);
      var sut2 = new EsNif(ValidFormattedLowercaseDni);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidUnformattedDni, ValidFormattedDni)]
   [InlineData(ValidUnformattedNie, ValidFormattedNie)]
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
      var sut = new EsNif(ValidFormattedNie);
      var mask = "_________";
      var expected = ValidUnformattedNie;

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
      var sut = new EsNif(ValidFormattedNie);
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
      var sut1 = new EsNif(ValidUnformattedNie);
      var sut2 = new EsNif(ValidUnformattedNie);

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
      var sut1 = new EsNif(ValidUnformattedDni);
      var sut2 = new EsNif(AltValidUnformattedDni);

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
      var sut1 = new EsNif(ValidUnformattedDni);
      var sut2 = new EsNif(ValidFormattedDni);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void EsNif_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new EsNif(ValidFormattedDni);
      var sut2 = new EsNif(ValidFormattedDni.Replace('-', '.'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void EsNif_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new EsNif(ValidFormattedDni.Replace('-', 'A'));
      var sut2 = new EsNif(ValidFormattedDni.Replace('-', 'a'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void EsNif_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyByCase()
   {
      // Arrange.
      var sut1 = new EsNif(ValidFormattedDni);
      var sut2 = new EsNif(ValidFormattedLowercaseDni);

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
      var sut1 = new EsNif(ValidUnformattedDni);
      var sut2 = new EsNif(ValidUnformattedDni);

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
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = EsNif.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void EsNif_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = EsNif.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void EsNif_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = EsNif.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void EsNif_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = EsNif.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void EsNif_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = EsNif.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void EsNif_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = EsNif.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void EsNif_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new EsNif(ValidUnformattedDni);

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
      var sut = new EsNif(AltValidFormattedNie);
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
      var foo = new Foo { Nif = new EsNif(ValidUnformattedNie) };
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
      var json = "{\"Nif\":\"12245678Z\"}";  // Invalid check digit
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
