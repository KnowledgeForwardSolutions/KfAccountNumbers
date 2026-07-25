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
   private const String AltValidFormattedSozialversicherungsnummer = "756.8814.3009.98";

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
}
