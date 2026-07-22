using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.National.Europe.DeSteuerIdNr,
   KfAccountNumbers.National.Europe.DeSteuerIdNr.ValidationError>;
using LocalValidationError = KfAccountNumbers.National.Europe.DeSteuerIdNr.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.National.Europe.DeSteuerIdNr.ValidationError>;
using LocalValidationResult = KfAccountNumbers.National.Europe.DeSteuerIdNr.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class DeSteuerIdNrTests
{
   // See https://generator.avris.it/DE for tools to generate test Steuer-IdNr
   private const String ValidUnformattedSteuerIdNr = "43957380212";
   private const String AltValidUnformattedSteuerIdNr = "25986078148";
   private const String ValidFormattedSteuerIdNr = "43 957 380 212";
   private const String AltValidFormattedSteuerIdNr = "25/986/078/148";

   public static TheoryData<String> ValidSteuerIdNrValues =>
   [
      ValidUnformattedSteuerIdNr,
      AltValidUnformattedSteuerIdNr,
      ValidFormattedSteuerIdNr,
      AltValidFormattedSteuerIdNr,
   ];

   public static TheoryData<Char> ValidSeparators =>
   [
      ' ',
      '-',
      'A',
      'z',
      '/',
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "4395738021",           // Length 10
      "439573802123",         // Length 12
      "43 957 380 21",        // Length 13
      "43 957 380 2123",      // Length 15
      new String('1', 100)    // Very long string
   ];

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted values
      { ".3957380212", 0 },            // Non-digit character '.'
      { "4 957380212", 1 },            // Non-digit character ' '
      { "43A57380212", 2 },            // Non-digit character 'A'
      { "439Z7380212", 3 },            // Non-digit character 'Z'
      { "4395^380212", 4 },            // Non-digit character '^'
      { "43957a80212", 5 },            // Non-digit character 'a'
      { "439573z0212", 6 },            // Non-digit character 'z'
      { "4395738~212", 7 },            // Non-digit character '~'
      { "43957380\u215312", 8 },       // Non-digit character Unicode fraction 1/3
      { "439573802\u00D62", 9 },       // Invalid character unicode O with umlaut
      { "4395738021\u0BE6", 10 },      // Invalid character unicode Tamil digit 0

      // Formatted values
      { ".3 957 380 212", 0 },         // Non-digit character '.'
      { "4  957 380 212", 1 },         // Non-digit character ' '
      { "43 A57 380 212", 3 },         // Non-digit character 'A'
      { "43 9Z7 380 212", 4 },         // Non-digit character 'Z'
      { "43 95^ 380 212", 5 },         // Non-digit character '^'
      { "43-957-a80-212", 7 },         // Non-digit character 'a'
      { "43-957-3z0-212", 8 },         // Non-digit character 'z'
      { "43-957-38~-212", 9 },         // Non-digit character '~'
      { "43-957-380-\u215312", 11 },   // Non-digit character Unicode fraction 1/3
      { "43-957-380-2\u00D62", 12 },   // Invalid character unicode O with umlaut
      { "43-957-380-21\u0BE6", 13 },   // Invalid character unicode Tamil digit 0
   };

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "43957389212",       // 43957380212 with single digit transcription error 0 -> 9
      "43947380212",       // 2608832599 with single digit transcription error 5 -> 4
      "25986708148",       // 25986078148 with two digit transposition error 07 -> 70
      "25986087148",       // 25986078148 with two digit transposition error 78 -> 87
      "68223904129",       // 68443904129 with two digit twin error 44 -> 22
      "68443902149",       // 68443904129 with jump transposition error 412 -> 214
      "24395738021",       // 43957380212 with two digit right circular shift error
      "39573802124",       // 43957380212 with two digit left circular shift error

      "43 957 389 212",    // 43957380212 with single digit transcription error 0 -> 9
      "43 947 380 212",    // 2608832599 with single digit transcription error 5 -> 4
      "25 986 708 148",    // 25986078148 with two digit transposition error 07 -> 70
      "25 986 087 148",    // 25986078148 with two digit transposition error 78 -> 87
      "68-223-904-129",    // 68443904129 with two digit twin error 44 -> 22
      "68-443-902-149",    // 68443904129 with jump transposition error 412 -> 214
      "24-395-738-021",    // 43957380212 with two digit right circular shift error
      "39-573-802-124",    // 43957380212 with two digit left circular shift error
   ];

   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      // First separator position
      { "430957 380 212", 2 },
      { "431957 380 212", 2 },
      { "432957 380 212", 2 },
      { "433957 380 212", 2 },
      { "434957 380 212", 2 },
      { "435957 380 212", 2 },
      { "436957 380 212", 2 },
      { "437957 380 212", 2 },
      { "438957 380 212", 2 },
      { "439957 380 212", 2 },

      // Second separator position
      { "43 9570380 212", 6 },
      { "43 9571380 212", 6 },
      { "43 9572380 212", 6 },
      { "43 9573380 212", 6 },
      { "43 9574380 212", 6 },
      { "43 9575380 212", 6 },
      { "43 9576380 212", 6 },
      { "43 9577380 212", 6 },
      { "43 9578380 212", 6 },
      { "43 9579380 212", 6 },

      // Theird separator position
      { "43 957 3800212", 10 },
      { "43 957 3801212", 10 },
      { "43 957 3802212", 10 },
      { "43 957 3803212", 10 },
      { "43 957 3804212", 10 },
      { "43 957 3805212", 10 },
      { "43 957 3806212", 10 },
      { "43 957 3807212", 10 },
      { "43 957 3808212", 10 },
      { "43 957 3809212", 10 },

      // Mixed separators
      { "43 957.380 212", 6 },
      { "43 957 380-212", 10 },
      { "43.957/380-212", 6 },
   };

   private static String GetFormattedValue(String value, Char separator)
      => value[..2] + separator + value[2..5] + separator + value[5..8] + separator +  value[8..];

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.DeSteuerIdNrInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.DeSteuerIdNrInvalidCheckDigit,
         Algorithms.Iso7064Mod11_10.AlgorithmName);

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.DeSteuerIdNrInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(DeSteuerIdNr.UnformattedLength, Messages.DeSteuerIdNrUnormattedLength),
            new ValidLengthDefinition(DeSteuerIdNr.FormattedLength, Messages.DeSteuerIdNrFormattedLength),
         ]);

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(
         Messages.DeSteuerIdNrInvalidSeparator,
         value[position],
         position);

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidSteuerIdNrValues))]
   public void DeSteuerIdNr_Validate_ShouldReturnValidValue_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void DeSteuerIdNr_Validate_ShouldReturnValidationPassed_WhenValueHasValidSeparator(Char separator)
   {
      // Arrange.
      var value = GetFormattedValue(ValidUnformattedSteuerIdNr, separator: separator);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void DeSteuerIdNr_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void DeSteuerIdNr_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void DeSteuerIdNr_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void DeSteuerIdNr_Validate_ShouldReturnInvalidChecksum_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void DeSteuerIdNr_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = DeSteuerIdNr.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion
}
