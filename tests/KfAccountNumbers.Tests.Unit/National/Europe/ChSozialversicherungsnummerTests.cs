using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.National.Europe.ChSozialversicherungsnummer,
   KfAccountNumbers.National.Europe.ChSozialversicherungsnummer.ValidationError>;
using LocalValidationError = KfAccountNumbers.National.Europe.ChSozialversicherungsnummer.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.National.Europe.ChSozialversicherungsnummer.ValidationError>;
using LocalValidationResult = KfAccountNumbers.National.Europe.ChSozialversicherungsnummer.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class ChSozialversicherungsnummerTests
{
   private const String ValidUnformattedSozialversicherungsnummer = "7560850652826";           // From https://teaddict.net/swiss-ssn.html
   private const String AltValidUnformattedSozialversicherungsnummer = "7568814300998";
   private const String ValidFormattedSozialversicherungsnummer = "756.0850.6528.26";
   private const String AltValidFormattedSozialversicherungsnummer = "756-8814-3009-98";

   public static TheoryData<String> ValidValues =>
   [
      ValidUnformattedSozialversicherungsnummer,
      AltValidUnformattedSozialversicherungsnummer,
      ValidFormattedSozialversicherungsnummer,
      AltValidFormattedSozialversicherungsnummer,
   ];

   public static TheoryData<Char> ValidSeparators =>
   [
      ' ',
      '-',
      'A',
      'z',
      '/',
   ];

   public static TheoryData<String> UndetectableCheckDigitErrors =>
   [
      "7567905162668",     // 7567905612668 with two digit transposition 61 -> 16 (difference = 5)
      "7560058652826",     // 7560850652826 with two digit jump transcription 850 -> 058
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "756085065282",         // Length 12
      "75608506528267",       // Length 14
      "756.0850.6528.2",      // Length 15
      "756.0850.6528.267",    // Length 17
      new String('1', 100)    // Very long string
   ];

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted values
      { ".560850652826", 0 },          // Non-digit character '.'
      { "7 60850652826", 1 },          // Non-digit character ' '
      { "75A0850652826", 2 },          // Non-digit character 'A'
      { "756Z850652826", 3 },          // Non-digit character 'Z'
      { "7560^50652826", 4 },          // Non-digit character '^'
      { "75608a0652826", 5 },          // Non-digit character 'a'
      { "756085z652826", 6 },          // Non-digit character 'z'
      { "7560858~52826", 7 },          // Non-digit character '~'
      { "75608585\u21532826", 8 },     // Non-digit character Unicode fraction 1/3
      { "756085855\u00D6826", 9 },     // Invalid character unicode O with umlaut
      { "7560858552\u0BE626", 10 },    // Invalid character unicode Tamil digit 0
      { "75608506528.6", 11 },         // Non-digit character '.'
      { "756085065282A", 12 },         // Non-digit character 'A'

      // Formatted values
      { ".56.0850.6528.26", 0 },       // Non-digit character '.'
      { "7 6.0850.6528.26", 1 },       // Non-digit character ' '
      { "75A.0850.6528.26", 2 },       // Non-digit character 'A'
      { "756.Z850.6528.26", 4 },       // Non-digit character 'Z'
      { "756.0^50.6528.26", 5 },       // Non-digit character '^'
      { "756.08a0.6528.26", 6 },       // Non-digit character 'a'
      { "756.085z.6528.26", 7 },       // Non-digit character 'z'
      { "756 0858 ~528.26", 9 },       // Non-digit character '~'
      { "756 0858 5\u215328.26", 10 }, // Non-digit character Unicode fraction 1/3
      { "756 0858 55\u00D68.26", 11 }, // Invalid character unicode O with umlaut
      { "756 0858 552\u0BE6.26", 12 }, // Invalid character unicode Tamil digit 0
      { "756 0850 6528 .6", 14 },      // Non-digit character '.'
      { "756 0850 6528 2A", 15 },      // Non-digit character 'A'
   };

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "7560850653826",     // 7560850652826 with single digit transcription error 2 -> 3
      "7560850552826",     // 7560850652826 with single digit transcription error 6 -> 5
      "7568813400998",     // 7568814300998 with two digit transposition 43 -> 34
      "7658814300998",     // 7568814300998 with two digit transposition 56 -> 65
      "7560850652827",     // 7560850652826 with invalid check digit 6 -> 7
   ];

   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      // First separator position
      { "75600850.6528.26", 3 },
      { "75610850.6528.26", 3 },
      { "75620850.6528.26", 3 },
      { "75630850.6528.26", 3 },
      { "75640850.6528.26", 3 },
      { "75650850.6528.26", 3 },
      { "75660850.6528.26", 3 },
      { "75670850.6528.26", 3 },
      { "75680850.6528.26", 3 },
      { "75690850.6528.26", 3 },

      // Second separator position
      { "756.085006528.26", 8 },
      { "756.085016528.26", 8 },
      { "756.085026528.26", 8 },
      { "756.085036528.26", 8 },
      { "756.085046528.26", 8 },
      { "756.085056528.26", 8 },
      { "756.085066528.26", 8 },
      { "756.085076528.26", 8 },
      { "756.085086528.26", 8 },
      { "756.085096528.26", 8 },

      // Third separator position
      { "756.0850.6528026", 13 },
      { "756.0850.6528126", 13 },
      { "756.0850.6528226", 13 },
      { "756.0850.6528326", 13 },
      { "756.0850.6528426", 13 },
      { "756.0850.6528526", 13 },
      { "756.0850.6528626", 13 },
      { "756.0850.6528726", 13 },
      { "756.0850.6528826", 13 },
      { "756.0850.6528926", 13 },

      // Mixed separators
      { "756.0850 6528.26", 8 },
      { "756.0850.6528-26", 13 },
      { "756.0850/6528-26", 8 },
   };

   public static TheoryData<String> InvalidPrefixValues =>
   [
      "1230850652824",
      "123.0850.6528.24",
   ];

   private static String GetFormattedValue(String value, Char separator)
      => value[..3] + separator + value[3..7] + separator + value[7..11] + separator +  value[11..];

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.ChSozialversicherungsnummerInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.ChSozialversicherungsnummerInvalidCheckDigit,
         ChSozialversicherungsnummer.CheckDigitAlgorithmName);

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.ChSozialversicherungsnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(ChSozialversicherungsnummer.UnformattedLength, Messages.ChSozialversicherungsnummerUnformattedLength),
            new ValidLengthDefinition(ChSozialversicherungsnummer.FormattedLength, Messages.ChSozialversicherungsnummerFormattedLength),
         ]);

   private static InvalidPrefix GetInvalidPrefixResult(String value)
      => new(
         Messages.ChSozialversicherungsnummerInvalidPrefix,
         value[..3]);

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(
         Messages.ChSozialversicherungsnummerInvalidSeparator,
         value[position],
         position);

   private static String GetRawChSozialversicherungsnummer(String value)
      => value.Length == ChSozialversicherungsnummer.UnformattedLength
         ? value
         : value[..3] + value[4..8] + value[9..13] + value[14..];

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void ChSozialversicherungsnummer_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawChSozialversicherungsnummer(value);

      // Act.
      var sut = new ChSozialversicherungsnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void ChSozialversicherungsnummer_Constructor_ShouldCreateInstance_WhenValueHasValidSeparator(Char separator)
   {
      // Arrange.
      var value = GetFormattedValue(ValidUnformattedSozialversicherungsnummer, separator: separator);
      var expected = GetRawChSozialversicherungsnummer(value);

      // Act.
      var sut = new ChSozialversicherungsnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(UndetectableCheckDigitErrors))]
   public void ChSozialversicherungsnummer_Constructor_ShouldCreateInstance_WhenValueHasUndetectableCheckDigitError(String value)
   {
      // Arrange.
      var expected = GetRawChSozialversicherungsnummer(value);

      // Act.
      var sut = new ChSozialversicherungsnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void ChSozialversicherungsnummer_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new ChSozialversicherungsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void ChSozialversicherungsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new ChSozialversicherungsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void ChSozialversicherungsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new ChSozialversicherungsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void ChSozialversicherungsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new ChSozialversicherungsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void ChSozialversicherungsnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new ChSozialversicherungsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixValues))]
   public void ChSozialversicherungsnummer_Constructor_ShouldThrowKfValidationException_WhenValueDoesNotStartWith756(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidPrefixResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new ChSozialversicherungsnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void ChSozialversicherungsnummer_Value_ShouldReturnValidatedSteuerIdNr(String value)
   {
      // Arrange.
      var expected = GetRawChSozialversicherungsnummer(value);
      var sut = new ChSozialversicherungsnummer(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void ChSozialversicherungsnummer_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidUnformattedSozialversicherungsnummer;
      var sut = new ChSozialversicherungsnummer(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void ChSozialversicherungsnummer_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidUnformattedSozialversicherungsnummer;
      var sut = new ChSozialversicherungsnummer(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void ChSozialversicherungsnummer_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      ChSozialversicherungsnummer sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void ChSozialversicherungsnummer_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      ChSozialversicherungsnummer sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void ChSozialversicherungsnummer_ExplicitCastToChSozialversicherungsnummer_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new ChSozialversicherungsnummer(value);

      // Act.
      var sut = (ChSozialversicherungsnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void ChSozialversicherungsnummer_ExplicitCastToChSozialversicherungsnummer_ShouldCreateInstance_WhenValueHasValidSeparator(Char separator)
   {
      // Arrange.
      var value = GetFormattedValue(ValidUnformattedSozialversicherungsnummer, separator: separator);
      var expected = new ChSozialversicherungsnummer(value);

      // Act.
      var sut = (ChSozialversicherungsnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void ChSozialversicherungsnummer_ExplicitCastToChSozialversicherungsnummer_ShouldCreateInstance_WhenValueHasUndetectableCheckDigitError(String value)
   {
      // Arrange.
      var expected = new ChSozialversicherungsnummer(value);

      // Act.
      var sut = (ChSozialversicherungsnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void ChSozialversicherungsnummer_ExplicitCastToChSozialversicherungsnummer_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (ChSozialversicherungsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void ChSozialversicherungsnummer_ExplicitCastToChSozialversicherungsnummer_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (ChSozialversicherungsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void ChSozialversicherungsnummer_ExplicitCastToChSozialversicherungsnummer_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (ChSozialversicherungsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void ChSozialversicherungsnummer_ExplicitCastToChSozialversicherungsnummer_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (ChSozialversicherungsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void ChSozialversicherungsnummer_ExplicitCastToChSozialversicherungsnummer_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (ChSozialversicherungsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixValues))]
   public void ChSozialversicherungsnummer_ExplicitCastToChSozialversicherungsnummer_ShouldThrowKfValidationException_WhenValueDoesNotStartWith756(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidPrefixResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (ChSozialversicherungsnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void ChSozialversicherungsnummer_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void ChSozialversicherungsnummer_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(AltValidUnformattedSozialversicherungsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void ChSozialversicherungsnummer_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 13 and 16 character versions for same person should still be equal.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(ValidFormattedSozialversicherungsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void ChSozialversicherungsnummer_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer.Replace('.', '-'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void ChSozialversicherungsnummer_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer.Replace('.', 'A'));
      var sut2 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer.Replace('.', 'a'));

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void ChSozialversicherungsnummer_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(AltValidUnformattedSozialversicherungsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void ChSozialversicherungsnummer_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 13 and 16 character versions for same person should still be equal.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(ValidFormattedSozialversicherungsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void ChSozialversicherungsnummer_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void ChSozialversicherungsnummer_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer.Replace('.', '-'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void ChSozialversicherungsnummer_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer.Replace('.', 'A'));
      var sut2 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer.Replace('.', 'a'));

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void ChSozialversicherungsnummer_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new ChSozialversicherungsnummer(value);

      // Act.
      var result = ChSozialversicherungsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void ChSozialversicherungsnummer_Create_ShouldCreateInstance_WhenValueHasValidSeparator(Char separator)
   {
      // Arrange.
      var value = GetFormattedValue(ValidUnformattedSozialversicherungsnummer, separator: separator);
      LocalCreateResult expected = new ChSozialversicherungsnummer(value);

      // Act.
      var result = ChSozialversicherungsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(UndetectableCheckDigitErrors))]
   public void ChSozialversicherungsnummer_Create_ShouldCreateInstance_WhenValueHasUndetectableCheckDigitError(String value)
   {
      // Arrange.
      LocalCreateResult expected = new ChSozialversicherungsnummer(value);

      // Act.
      var result = ChSozialversicherungsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void ChSozialversicherungsnummer_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = ChSozialversicherungsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void ChSozialversicherungsnummer_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = ChSozialversicherungsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void ChSozialversicherungsnummer_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = ChSozialversicherungsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void ChSozialversicherungsnummer_Create_ShouldReturnInvalidChecksumValidationResult_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = ChSozialversicherungsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void ChSozialversicherungsnummer_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = ChSozialversicherungsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixValues))]
   public void ChSozialversicherungsnummer_Create_ShouldReturnInvalidPrefixValidationResult_WhenValueDoesNotStartWith756(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidPrefixResult(value);

      // Act.
      var result = ChSozialversicherungsnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void ChSozialversicherungsnummer_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void ChSozialversicherungsnummer_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(AltValidUnformattedSozialversicherungsnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void ChSozialversicherungsnummer_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 13 and 16 character versions for same person should still be equal.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(ValidFormattedSozialversicherungsnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void ChSozialversicherungsnummer_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer.Replace('.', '-'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void ChSozialversicherungsnummer_Equals_ShouldReturnTrue_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer.Replace('.', 'A'));
      var sut2 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer.Replace('.', 'a'));

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void ChSozialversicherungsnummer_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);

      // Act/assert.
      sut.Equals(ValidUnformattedSozialversicherungsnummer).Should().BeFalse();
   }

   [Fact]
   public void ChSozialversicherungsnummer_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void ChSozialversicherungsnummer_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new ChSozialversicherungsnummer(ValidFormattedSozialversicherungsnummer);
      var expected = ValidFormattedSozialversicherungsnummer;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void ChSozialversicherungsnummer_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var mask = "_____________";
      var expected = ValidUnformattedSozialversicherungsnummer;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void ChSozialversicherungsnummer_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new ChSozialversicherungsnummer(AltValidFormattedSozialversicherungsnummer);
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
   public void ChSozialversicherungsnummer_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new ChSozialversicherungsnummer(AltValidFormattedSozialversicherungsnummer);
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
   public void ChSozialversicherungsnummer_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void ChSozialversicherungsnummer_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(AltValidUnformattedSozialversicherungsnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void ChSozialversicherungsnummer_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 13 and 16 character versions for same person should still be equal.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(ValidFormattedSozialversicherungsnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void ChSozialversicherungsnummer_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparators()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer.Replace('.', '-'));

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void ChSozialversicherungsnummer_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyBySeparatorCase()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer.Replace('.', 'A'));
      var sut2 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer.Replace('.', 'a'));

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

   // ChSozialversicherungsnummer does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void ChSozialversicherungsnummer_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);
      var sut2 = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void ChSozialversicherungsnummer_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new ChSozialversicherungsnummer(value);
      var expected = GetRawChSozialversicherungsnummer(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void ChSozialversicherungsnummer_Validate_ShouldReturnValidValue_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = ChSozialversicherungsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void ChSozialversicherungsnummer_Validate_ShouldReturnValidationPassed_WhenValueHasValidSeparator(Char separator)
   {
      // Arrange.
      var value = GetFormattedValue(ValidUnformattedSozialversicherungsnummer, separator: separator);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = ChSozialversicherungsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(UndetectableCheckDigitErrors))]
   public void ChSozialversicherungsnummer_Validate_ShouldReturnValidValue_WhenValueHasUndetectableCheckDigitError(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = ChSozialversicherungsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void ChSozialversicherungsnummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = ChSozialversicherungsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void ChSozialversicherungsnummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = ChSozialversicherungsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void ChSozialversicherungsnummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = ChSozialversicherungsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void ChSozialversicherungsnummer_Validate_ShouldReturnInvalidChecksum_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = ChSozialversicherungsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void ChSozialversicherungsnummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = ChSozialversicherungsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixValues))]
   public void ChSozialversicherungsnummer_Validate_ShouldReturnInvalidPrefix_WhenValueDoesNotStartWith756(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidPrefixResult(value);

      // Act.
      var result = ChSozialversicherungsnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void ChSozialversicherungsnummer_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<ChSozialversicherungsnummer>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void ChSozialversicherungsnummer_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new ChSozialversicherungsnummer(AltValidFormattedSozialversicherungsnummer);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public ChSozialversicherungsnummer Sozialversicherungsnummer { get; set; } = null!;
   }

   [Fact]
   public void ChSozialversicherungsnummer_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Sozialversicherungsnummer = new ChSozialversicherungsnummer(ValidUnformattedSozialversicherungsnummer) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void ChSozialversicherungsnummer_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Sozialversicherungsnummer\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void ChSozialversicherungsnummer_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Sozialversicherungsnummer\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Sozialversicherungsnummer.Should().BeNull();
   }

   [Fact]
   public void ChSozialversicherungsnummer_JsonDeserialization_ShouldThrowKfValidationException_WhenSozialversicherungsnummerIsInvalid()
   {
      // Arrange.
      var json = "{\"Sozialversicherungsnummer\":\"7560850653826\"}";  // Invalid checksum
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
