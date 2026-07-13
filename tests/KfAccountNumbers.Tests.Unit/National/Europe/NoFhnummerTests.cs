using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.National.Europe.NoFhnummer,
   KfAccountNumbers.National.Europe.NoFhnummer.ValidationError>;
using LocalValidationError = KfAccountNumbers.National.Europe.NoFhnummer.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.National.Europe.NoFhnummer.ValidationError>;
using LocalValidationResult = KfAccountNumbers.National.Europe.NoFhnummer.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class NoFhnummerTests : NoIdentityNumberTestsBase
{
   public static TheoryData<Char, String> InvalidPrefixValues = new()
   {
      { '0', "001" },
      { '1', "001" },
      { '2', "001" },
      { '3', "001" },
      { '4', "003" },      // Individual number adjusted to ensure that valid check digits are generated
      { '5', "001" },
      { '6', "001" },
      { '7', "001" },
   };

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.NoFhnummerInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(NoIdentityNumberBase.UnformattedLength, Messages.NoFhnummerUnformattedLength),
            new ValidLengthDefinition(NoIdentityNumberBase.FormattedLength, Messages.NoFhnummerFormattedLength),
         ]);

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.NoFhnummerInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.NoFhnummerInvalidCheckDigits,
         NoIdentityNumberBase.CheckDigitAlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(String value)
      => new(Messages.NoFhnummerInvalidSeparator, value[6], 6);

   private static InvalidPrefix GetInvalidPrefixResult(String value)
      => new(Messages.NoFhnummerInvalidPrefix, value[0].ToString());

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidFhnummerValues))]
   public void NoFhnummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NoFhnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSeparators))]
   public void NoFhnummer_Validate_ShouldReturnValidationPassed_WhenValueHasValidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("912345", separator, "678");
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = NoFhnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void NoFhnummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = NoFhnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void NoFhnummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = NoFhnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void NoFhnummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = NoFhnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void NoFhnummer_Validate_ShouldReturnInvalidCheckDigits_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = NoFhnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparators))]
   public void NoFhnummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String separator)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits("912345", separator, "678");
      LocalValidationResult expected = GetInvalidSeparatorResult(value);

      // Act.
      var result = NoFhnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixValues))]
   public void NoFhnummer_Validate_ShouldReturnInvalidPrefix_WhenValueHasInvalidInitialDigit(
      Char leadingDigit,
      String individualNumber)
   {
      // Arrange.
      var value = GetValueWithValidCheckDigits(
         $"{leadingDigit}00001",
         individualNumber: individualNumber);
      LocalValidationResult expected = GetInvalidPrefixResult(value);

      // Act.
      var result = NoFhnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion
}
