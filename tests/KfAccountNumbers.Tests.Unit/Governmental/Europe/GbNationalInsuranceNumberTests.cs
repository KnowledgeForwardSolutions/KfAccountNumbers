// Ignore Spelling: Deserialize Deserialization Json Kf

using LocalCreateResult = KfAccountNumbers.Results.UCreateResult<
   KfAccountNumbers.Governmental.Europe.GbNationalInsuranceNumber,
   KfAccountNumbers.Governmental.Europe.GbNationalInsuranceNumber.ValidationError>;
using LocalValidationError = KfAccountNumbers.Governmental.Europe.GbNationalInsuranceNumber.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.Governmental.Europe.GbNationalInsuranceNumber.ValidationError>;
using LocalValidationResult = KfAccountNumbers.Governmental.Europe.GbNationalInsuranceNumber.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

public class GbNationalInsuranceNumberTests
{
   private const String Valid8CharacterValue = "AB123456";
   private const String Valid9CharacterValue = "AB123456C";
   private const String Valid11CharacterValue = "AB 12 34 56";
   private const String Valid13CharacterValue = "AB 12 34 56 C";
   private const String AltValid8CharacterValue = "GG000123";
   private const String AltValid9CharacterValue = "GG000123B";
   private const String AltValid11CharacterValue = "GG 00 01 23";
   private const String AltValid13CharacterValue = "GG 00 01 23 B";

   public static String GetNationalInsuranceNumber(
      String prefix = "AB",
      String digits = "123456",
      String suffix = "",
      Boolean formatted = false)
      => formatted
         ? $"{prefix} {digits[..2]} {digits[2..4]} {digits[4..]}{(suffix != String.Empty ? " " : String.Empty)}{suffix}"
         : $"{prefix}{digits}{suffix}";

   public static String GetNationalInsuranceNumber(
      Char prefix1 = 'A',
      Char prefix2 = 'C',
      String digits = "123456",
      String suffix = "",
      Boolean formatted = false)
      => GetNationalInsuranceNumber($"{prefix1}{prefix2}", digits, suffix, formatted);

   public static String GetRawNationalInsuranceNumber(String str)
      => str.Length switch
      {
         11 => $"{str[..2]}{str[3..5]}{str[6..8]}{str[9..]}",
         13 => $"{str[..2]}{str[3..5]}{str[6..8]}{str[9..11]}{str[12]}",
         _ => str,
      };

   public static TheoryData<String> ValidNationalInsuranceNumberValues =>
   [
      Valid8CharacterValue,
      Valid9CharacterValue,
      Valid11CharacterValue,
      Valid13CharacterValue,
      AltValid8CharacterValue,
      AltValid9CharacterValue,
      AltValid11CharacterValue,
      AltValid13CharacterValue,
   ];

   public static TheoryData<Char, Boolean> ValidPrefixFirstCharacters = new()
   {
      { 'A', false },
      { 'B', false },
      { 'C', false },
      { 'E', false },
      { 'G', false },
      { 'H', false },
      { 'J', false },
      { 'K', false },
      { 'L', false },
      { 'M', false },
      { 'N', false },
      { 'O', false },
      { 'P', false },
      { 'R', false },
      { 'S', false },
      { 'T', false },
      { 'W', false },
      { 'X', false },
      { 'Y', false },
      { 'Z', false },
      { 'A', true },
      { 'B', true },
      { 'C', true },
      { 'E', true },
      { 'G', true },
      { 'H', true },
      { 'J', true },
      { 'K', true },
      { 'L', true },
      { 'M', true },
      { 'N', true },
      { 'O', true },
      { 'P', true },
      { 'R', true },
      { 'S', true },
      { 'T', true },
      { 'W', true },
      { 'X', true },
      { 'Y', true },
      { 'Z', true },
   };

   public static TheoryData<Char, Boolean> ValidPrefixSecondCharacters = new()
   {
      { 'A', false },
      { 'B', false },
      { 'C', false },
      { 'E', false },
      { 'G', false },
      { 'H', false },
      { 'J', false },
      { 'K', false },
      { 'L', false },
      { 'M', false },
      { 'N', false },
      { 'P', false },
      { 'R', false },
      { 'S', false },
      { 'T', false },
      { 'W', false },
      { 'X', false },
      { 'Y', false },
      { 'Z', false },
      { 'A', true },
      { 'B', true },
      { 'C', true },
      { 'E', true },
      { 'G', true },
      { 'H', true },
      { 'J', true },
      { 'K', true },
      { 'L', true },
      { 'M', true },
      { 'N', true },
      { 'P', true },
      { 'R', true },
      { 'S', true },
      { 'T', true },
      { 'W', true },
      { 'X', true },
      { 'Y', true },
      { 'Z', true },
   };

   public static TheoryData<String, Boolean> ValidSuffixCharacters = new()
   {
      { String.Empty, false },
      { "A", false },
      { "B", false },
      { "C", false },
      { "D", false },
      { String.Empty, true },
      { "A", true },
      { "B", true },
      { "C", true },
      { "D", true },
   };

   public static TheoryData<String> InvalidLengthValues =>
   [
      "AB12345",              // Length 7
      "AB123456CB",           // Length 10
      "AB 12 34 5",           // Length 10
      "AB 12 34 56C",         // Length 12
      "AB 12 34 56 CB",       // Length 14
      new String('1', 100)    // Very long string
   ];

   public static TheoryData<String, Boolean> InvalidPrefixValues = new()
   {
      { "BG", false },
      { "GB", false },
      { "NK", false },
      { "KN", false },
      { "TN", false },
      { "NT", false },
      { "ZZ", false },
      { "BG", true },
      { "GB", true },
      { "NK", true },
      { "KN", true },
      { "TN", true },
      { "NT", true },
      { "ZZ", true },
   };

   public static TheoryData<Char, Boolean> InvalidPrefixFirstCharacters = new()
   {
      { 'D', false },
      { 'F', false },
      { 'I', false },
      { 'Q', false },
      { 'U', false },
      { 'V', false },
      { 'D', true },
      { 'F', true },
      { 'I', true },
      { 'Q', true },
      { 'U', true },
      { 'V', true },
   };

   public static TheoryData<Char, Boolean> InvalidPrefixSecondCharacters = new()
   {
      { 'D', false },
      { 'F', false },
      { 'I', false },
      { 'O', false },
      { 'Q', false },
      { 'U', false },
      { 'V', false },
      { 'D', true },
      { 'F', true },
      { 'I', true },
      { 'O', true },
      { 'Q', true },
      { 'U', true },
      { 'V', true },
   };

   public static TheoryData<String, Int32, Boolean> InvalidDigits = new()
   {
      { " 12345", 2, false },
      { "0-2345", 3, false },
      { "01=345", 4, false },
      { "012A45", 5, false },
      { "0123b5", 6, false },
      { "01234~", 7, false },
      { "01234\u2153", 7, false },     // Unicode fraction 1/3
      { "01234\u00D6", 7, false },     // unicode O with umlaut
      { " 12345", 3, true },
      { "0-2345", 4, true },
      { "01=345", 6, true },
      { "012A45", 7, true },
      { "0123b5", 9, true },
      { "01234~", 10, true },
      { "01234\u2153", 10, true },     // Unicode fraction 1/3
      { "01234\u00D6", 10, true },     // unicode O with umlaut
   };

   public static TheoryData<String, Boolean> InvalidSuffixCharacters = new()
   {
      { "a", false },
      { "b", false },
      { "c", false },
      { "d", false },
      { " ", false },
      { "=", false },
      { "1", false },
      { "0", false },
      { "\u2153", false },       // Unicode fraction 1/3
      { "\u00D6", false },       // unicode O with umlaut
      { "a", true },
      { "b", true },
      { "c", true },
      { "d", true },
      { " ", true },
      { "=", true },
      { "1", true },
      { "0", true },
      { "\u2153", true },       // Unicode fraction 1/3
      { "\u00D6", true },       // unicode O with umlaut
   };

   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      { "AB012 34 56 A", 2 },
      { "AB912 34 56 A", 2 },
      { "ABA12 34 56 A", 2 },
      { "ABZ12 34 56 A", 2 },
      { "ABa12 34 56 A", 2 },
      { "ABz12 34 56 A", 2 },

      { "AB 12034 56 A", 5 },
      { "AB 12934 56 A", 5 },
      { "AB 12A34 56 A", 5 },
      { "AB 12Z34 56 A", 5 },
      { "AB 12a34 56 A", 5 },
      { "AB 12z34 56 A", 5 },

      { "AB 12 34056 A", 8 },
      { "AB 12 34956 A", 8 },
      { "AB 12 34A56 A", 8 },
      { "AB 12 34Z56 A", 8 },
      { "AB 12 34a56 A", 8 },
      { "AB 12 34z56 A", 8 },

      { "AB 12 34 560A", 11 },
      { "AB 12 34 569A", 11 },
      { "AB 12 34 56AA", 11 },
      { "AB 12 34 56ZA", 11 },
      { "AB 12 34 56aA", 11 },
      { "AB 12 34 56zA", 11 },

      { "AB012 34 56", 2 },
      { "AB912 34 56", 2 },
      { "ABA12 34 56", 2 },
      { "ABZ12 34 56", 2 },
      { "ABa12 34 56", 2 },
      { "ABz12 34 56", 2 },

      { "AB 12034 56", 5 },
      { "AB 12934 56", 5 },
      { "AB 12A34 56", 5 },
      { "AB 12Z34 56", 5 },
      { "AB 12a34 56", 5 },
      { "AB 12z34 56", 5 },

      { "AB 12 34056", 8 },
      { "AB 12 34956", 8 },
      { "AB 12 34A56", 8 },
      { "AB 12 34Z56", 8 },
      { "AB 12 34a56", 8 },
      { "AB 12 34z56", 8 },
   };

   private static InvalidLength GetInvalidLengthResult(
      String value,
      String? message = null)
      => new(
         message ?? Messages.GbNationalInsuranceNumberInvalidLength,
         value.Length,
         GbNationalInsuranceNumber.GetValidLengthDefinitions());

   private static InvalidGbNationalInsuranceNumberPrefix GetInvalidPrefixResult(String value)
      => new(
         Messages.GbNationalInsuranceNumberInvalidPrefix,
         value[..2]);

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.GbNationalInsuranceNumberInvalidCharacter,
         value[position],
         position);

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(
         Messages.GbNationalInsuranceNumberInvalidSeparator,
         value[position],
         position);

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidNationalInsuranceNumberValues))]
   public void GbNationalInsuranceNumber_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawNationalInsuranceNumber(value);

      // Act.
      var sut = new GbNationalInsuranceNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidPrefixFirstCharacters))]
   public void GbNationalInsuranceNumber_Constructor_ShouldCreateInstance_WhenValueHasValidFirstPrefixCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(ch, formatted: formatted);
      var expected = GetRawNationalInsuranceNumber(value);

      // Act.
      var sut = new GbNationalInsuranceNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidPrefixSecondCharacters))]
   public void GbNationalInsuranceNumber_Constructor_ShouldCreateInstance_WhenValueHasValidSecondPrefixCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix2: ch, formatted: formatted);
      var expected = GetRawNationalInsuranceNumber(value);

      // Act.
      var sut = new GbNationalInsuranceNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSuffixCharacters))]
   public void GbNationalInsuranceNumber_Constructor_ShouldCreateInstance_WhenValueHasValidSuffixCharacter(
      String suffix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", suffix: suffix, formatted: formatted);
      var expected = GetRawNationalInsuranceNumber(value);

      // Act.
      var sut = new GbNationalInsuranceNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbNationalInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNationalInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbNationalInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNationalInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixValues))]
   public void GbNationalInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidPrefix(
      String prefix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix, formatted: formatted);
      LocalValidationError expected = GetInvalidPrefixResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNationalInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixFirstCharacters))]
   public void GbNationalInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidPrefixFirstCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(ch, formatted: formatted);
      LocalValidationError expected = GetInvalidCharacterResult(value, 0);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNationalInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixSecondCharacters))]
   public void GbNationalInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidPrefixSecondCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix2: ch, formatted: formatted);
      LocalValidationError expected = GetInvalidCharacterResult(value, 1);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNationalInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDigits))]
   public void GbNationalInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDigitCharacters(
      String digits,
      Int32 position,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", digits: digits, formatted: formatted);
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNationalInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSuffixCharacters))]
   public void GbNationalInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSuffixCharacter(
      String suffix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", suffix: suffix, formatted: formatted);
      LocalValidationError expected = GetInvalidCharacterResult(value, value.Length - 1);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNationalInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbNationalInsuranceNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new GbNationalInsuranceNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidNationalInsuranceNumberValues))]
   public void GbNationalInsuranceNumber_Value_ShouldReturnValidatedNationalInsuranceNumber(String value)
   {
      // Arrange.
      var sut = new GbNationalInsuranceNumber(value);
      var expected = GetRawNationalInsuranceNumber(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbNationalInsuranceNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = Valid9CharacterValue;
      var sut = new GbNationalInsuranceNumber(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void GbNationalInsuranceNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = Valid11CharacterValue;
      var sut = new GbNationalInsuranceNumber(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void GbNationalInsuranceNumber_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      GbNationalInsuranceNumber sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void GbNationalInsuranceNumber_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      GbNationalInsuranceNumber sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidNationalInsuranceNumberValues))]
   public void GbNationalInsuranceNumber_ExplicitCastToGbNationalInsuranceNumber_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new GbNationalInsuranceNumber(value);

      // Act.
      var sut = (GbNationalInsuranceNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidPrefixFirstCharacters))]
   public void GbNationalInsuranceNumber_ExplicitCastToGbNationalInsuranceNumber_ShouldCreateInstance_WhenValueHasValidFirstPrefixCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(ch, formatted: formatted);
      var expected = new GbNationalInsuranceNumber(value);

      // Act.
      var sut = (GbNationalInsuranceNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidPrefixSecondCharacters))]
   public void GbNationalInsuranceNumber_ExplicitCastToGbNationalInsuranceNumber_ShouldCreateInstance_WhenValueHasValidSecondPrefixCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix2: ch, formatted: formatted);
      var expected = new GbNationalInsuranceNumber(value);

      // Act.
      var sut = (GbNationalInsuranceNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSuffixCharacters))]
   public void GbNationalInsuranceNumber_ExplicitCastToGbNationalInsuranceNumber_ShouldCreateInstance_WhenValueHasValidSuffixCharacter(
      String suffix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", suffix: suffix, formatted: formatted);
      var expected = new GbNationalInsuranceNumber(value);

      // Act.
      var sut = (GbNationalInsuranceNumber)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbNationalInsuranceNumber_ExplicitCastToGbNationalInsuranceNumber_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (GbNationalInsuranceNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbNationalInsuranceNumber_ExplicitCastToGbNationalInsuranceNumber_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (GbNationalInsuranceNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixValues))]
   public void GbNationalInsuranceNumber_ExplicitCastToGbNationalInsuranceNumber_ShouldThrowKfValidationException_WhenValueHasInvalidPrefix(
      String prefix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix, formatted: formatted);
      LocalValidationError expected = GetInvalidPrefixResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (GbNationalInsuranceNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixFirstCharacters))]
   public void GbNationalInsuranceNumber_ExplicitCastToGbNationalInsuranceNumber_ShouldThrowKfValidationException_WhenValueHasInvalidPrefixFirstCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(ch, formatted: formatted);
      LocalValidationError expected = GetInvalidCharacterResult(value, 0);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (GbNationalInsuranceNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixSecondCharacters))]
   public void GbNationalInsuranceNumber_ExplicitCastToGbNationalInsuranceNumber_ShouldThrowKfValidationException_WhenValueHasInvalidPrefixSecondCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix2: ch, formatted: formatted);
      LocalValidationError expected = GetInvalidCharacterResult(value, 1);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (GbNationalInsuranceNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDigits))]
   public void GbNationalInsuranceNumber_ExplicitCastToGbNationalInsuranceNumber_ShouldThrowKfValidationException_WhenValueHasInvalidDigitCharacters(
      String digits,
      Int32 position,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", digits: digits, formatted: formatted);
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (GbNationalInsuranceNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSuffixCharacters))]
   public void GbNationalInsuranceNumber_ExplicitCastToGbNationalInsuranceNumber_ShouldThrowKfValidationException_WhenValueHasInvalidSuffixCharacter(
      String suffix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", suffix: suffix, formatted: formatted);
      LocalValidationError expected = GetInvalidCharacterResult(value, value.Length - 1);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (GbNationalInsuranceNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbNationalInsuranceNumber_ExplicitCastToGbNationalInsuranceNumber_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (GbNationalInsuranceNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbNationalInsuranceNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid9CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(Valid9CharacterValue);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void GbNationalInsuranceNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid9CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(AltValid9CharacterValue);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Theory]
   [InlineData(Valid8CharacterValue, Valid11CharacterValue)]
   [InlineData(Valid9CharacterValue, Valid13CharacterValue)]
   public void GbNationalInsuranceNumber_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentFormats(
      String value1,
      String value2)
   {
      // Arrange. Formatted and unformatted versions for same person should still be equal.
      var sut1 = new GbNationalInsuranceNumber(value1);
      var sut2 = new GbNationalInsuranceNumber(value2);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void GbNationalInsuranceNumber_EqualityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 8 and 9 characters versions for same person are not considered equal.
      var sut1 = new GbNationalInsuranceNumber(Valid8CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(Valid9CharacterValue);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbNationalInsuranceNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid8CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(AltValid8CharacterValue);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void GbNationalInsuranceNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid8CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(Valid8CharacterValue);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Theory]
   [InlineData(Valid8CharacterValue, Valid11CharacterValue)]
   [InlineData(Valid9CharacterValue, Valid13CharacterValue)]
   public void GbNationalInsuranceNumber_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentFormats(
      String value1,
      String value2)
   {
      // Arrange. Formatted and unformatted versions for same person should still be equal.
      var sut1 = new GbNationalInsuranceNumber(value1);
      var sut2 = new GbNationalInsuranceNumber(value2);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void GbNationalInsuranceNumber_InequalityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 8 and 9 characters versions for same person are not considered equal.
      var sut1 = new GbNationalInsuranceNumber(Valid8CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(Valid9CharacterValue);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidNationalInsuranceNumberValues))]
   public void GbNationalInsuranceNumber_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new GbNationalInsuranceNumber(value);

      // Act.
      var result = GbNationalInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidPrefixFirstCharacters))]
   public void GbNationalInsuranceNumber_Create_ShouldCreateInstance_WhenValueHasValidFirstPrefixCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(ch, formatted: formatted);
      LocalCreateResult expected = new GbNationalInsuranceNumber(value);

      // Act.
      var result = GbNationalInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidPrefixSecondCharacters))]
   public void GbNationalInsuranceNumber_Create_ShouldCreateInstance_WhenValueHasValidSecondPrefixCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix2: ch, formatted: formatted);
      LocalCreateResult expected = new GbNationalInsuranceNumber(value);

      // Act.
      var result = GbNationalInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSuffixCharacters))]
   public void GbNationalInsuranceNumber_Create_ShouldCreateInstance_WhenValueHasValidSuffixCharacter(
      String suffix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", suffix: suffix, formatted: formatted);
      LocalCreateResult expected = new GbNationalInsuranceNumber(value);

      // Act.
      var result = GbNationalInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbNationalInsuranceNumber_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = GbNationalInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbNationalInsuranceNumber_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = GbNationalInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixValues))]
   public void GbNationalInsuranceNumber_Create_ShouldReturnInvalidPrefixValidationResult_WhenValueHasInvalidPrefix(
      String prefix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix, formatted: formatted);
      LocalCreateResult expected = (LocalValidationError)GetInvalidPrefixResult(value);

      // Act.
      var result = GbNationalInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixFirstCharacters))]
   public void GbNationalInsuranceNumber_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasInvalidPrefixFirstCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(ch, formatted: formatted);
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, 0);

      // Act.
      var result = GbNationalInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixSecondCharacters))]
   public void GbNationalInsuranceNumber_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasInvalidPrefixSecondCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix2: ch, formatted: formatted);
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, 1);

      // Act.
      var result = GbNationalInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDigits))]
   public void GbNationalInsuranceNumber_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasInvalidDigitCharacters(
      String digits,
      Int32 position,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", digits: digits, formatted: formatted);
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = GbNationalInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSuffixCharacters))]
   public void GbNationalInsuranceNumber_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasInvalidSuffixCharacter(
      String suffix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", suffix: suffix, formatted: formatted);
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, value.Length - 1);

      // Act.
      var result = GbNationalInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbNationalInsuranceNumber_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = GbNationalInsuranceNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbNationalInsuranceNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid9CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(Valid9CharacterValue);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void GbNationalInsuranceNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid9CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(AltValid9CharacterValue);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Theory]
   [InlineData(Valid8CharacterValue, Valid11CharacterValue)]
   [InlineData(Valid9CharacterValue, Valid13CharacterValue)]
   public void GbNationalInsuranceNumber_Equals_ShouldReturnTrue_WhenValuesHaveDifferentFormats(
      String value1,
      String value2)
   {
      // Arrange. Formatted and unformatted versions for same person should still be equal.
      var sut1 = new GbNationalInsuranceNumber(value1);
      var sut2 = new GbNationalInsuranceNumber(value2);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void GbNationalInsuranceNumber_Equals_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 8 and 9 characters versions for same person are not considered equal.
      var sut1 = new GbNationalInsuranceNumber(Valid8CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(Valid9CharacterValue);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   #endregion

   #region EqualsNonSuffix Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbNationalInsuranceNumber_EqualsNonSuffix_ShouldReturnTrue_WhenValuesWithSuffixesAreEqual()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid9CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(Valid9CharacterValue);

      // Act/assert.
      sut1.EqualsNonSuffix(sut2).Should().BeTrue();
   }

   [Fact]
   public void GbNationalInsuranceNumber_EqualsNonSuffix_ShouldReturnTrue_WhenValuesWithoutSuffixesAreEqual()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid8CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(Valid8CharacterValue);

      // Act/assert.
      sut1.EqualsNonSuffix(sut2).Should().BeTrue();
   }

   [Theory]
   [InlineData(Valid9CharacterValue, Valid8CharacterValue)]
   [InlineData(Valid8CharacterValue, Valid9CharacterValue)]
   public void GbNationalInsuranceNumber_EqualsNonSuffix_ShouldReturnTrue_WhenValuesWithAndWithoutSuffixesAreEqual(
      String value1,
      String value2)
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(value1);
      var sut2 = new GbNationalInsuranceNumber(value2);

      // Act/assert.
      sut1.EqualsNonSuffix(sut2).Should().BeTrue();
   }

   [Fact]
   public void GbNationalInsuranceNumber_EqualsNonSuffix_ShouldReturnTrue_WhenValuesDifferOnlyBySuffix()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber("AB123456A");
      var sut2 = new GbNationalInsuranceNumber("AB123456B");

      // Act/assert.
      sut1.EqualsNonSuffix(sut2).Should().BeTrue();
   }

   [Theory]
   [InlineData(Valid9CharacterValue)]
   [InlineData(Valid8CharacterValue)]
   public void GbNationalInsuranceNumber_EqualsNonSuffix_ShouldReturnTrue_WhenValuesAreReferenceEqual(String value)
   {
      // Arrange.
      var sut = new GbNationalInsuranceNumber(value);

      // Act/assert.
      sut.EqualsNonSuffix(sut).Should().BeTrue();
   }

   [Fact]
   public void GbNationalInsuranceNumber_EqualsNonSuffix_ShouldReturnFalse_WhenValuesWithSuffixesAreNotEqual()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid9CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(AltValid9CharacterValue);

      // Act/assert.
      sut1.EqualsNonSuffix(sut2).Should().BeFalse();
   }

   [Fact]
   public void GbNationalInsuranceNumber_EqualsNonSuffix_ShouldReturnFalse_WhenValuesWithoutSuffixesAreNotEqual()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid8CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(AltValid8CharacterValue);

      // Act/assert.
      sut1.EqualsNonSuffix(sut2).Should().BeFalse();
   }

   [Theory]
   [InlineData(Valid9CharacterValue, AltValid8CharacterValue)]
   [InlineData(Valid8CharacterValue, AltValid9CharacterValue)]
   public void GbNationalInsuranceNumber_EqualsNonSuffix_ShouldReturnFalse_WhenValuesWithAndWithoutSuffixesAreNotEqual(
      String value1,
      String value2)
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(value1);
      var sut2 = new GbNationalInsuranceNumber(value2);

      // Act/assert.
      sut1.EqualsNonSuffix(sut2).Should().BeFalse();
   }

   [Fact]
   public void GbNationalInsuranceNumber_EqualsNonSuffix_ShouldReturnFalse_WhenComparedToNull()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid8CharacterValue);
      GbNationalInsuranceNumber? sut2 = null;

      // Act/assert.
      sut1.EqualsNonSuffix(sut2).Should().BeFalse();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(Valid8CharacterValue, Valid11CharacterValue)]
   [InlineData(Valid9CharacterValue, Valid13CharacterValue)]
   [InlineData(Valid11CharacterValue, Valid11CharacterValue)]
   [InlineData(Valid13CharacterValue, Valid13CharacterValue)]
   public void GbNationalInsuranceNumber_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed(
      String value,
      String expected)
   {
      // Arrange.
      var sut = new GbNationalInsuranceNumber(value);

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Theory]
   [InlineData(Valid8CharacterValue, Valid8CharacterValue)]
   [InlineData(Valid9CharacterValue, Valid9CharacterValue)]
   [InlineData(Valid11CharacterValue, Valid8CharacterValue)]
   [InlineData(Valid13CharacterValue, Valid9CharacterValue)]
   public void GbNationalInsuranceNumber_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed(
      String value,
      String expected)
   {
      // Arrange.
      var sut = new GbNationalInsuranceNumber(value);
      var mask = "_________";

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Theory]
   [InlineData("")]
   [InlineData("\t")]
   public void GbNationalInsuranceNumber_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new GbNationalInsuranceNumber(Valid8CharacterValue);
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
   public void GbNationalInsuranceNumber_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid8CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(Valid8CharacterValue);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void GbNationalInsuranceNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid9CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(AltValid9CharacterValue);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Theory]
   [InlineData(Valid8CharacterValue, Valid11CharacterValue)]
   [InlineData(Valid9CharacterValue, Valid13CharacterValue)]
   public void GbNationalInsuranceNumber_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentFormats(
      String value1,
      String value2)
   {
      // Arrange. Formatted and unformatted versions for same person should still be equal.
      var sut1 = new GbNationalInsuranceNumber(value1);
      var sut2 = new GbNationalInsuranceNumber(value2);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void GbNationalInsuranceNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 8 and 9 characters versions for same person are not considered equal.
      var sut1 = new GbNationalInsuranceNumber(Valid8CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(Valid9CharacterValue);

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

   // GbNationalInsuranceNumber does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void GbNationalInsuranceNumber_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new GbNationalInsuranceNumber(Valid9CharacterValue);
      var sut2 = new GbNationalInsuranceNumber(Valid9CharacterValue);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidNationalInsuranceNumberValues))]
   public void GbNationalInsuranceNumber_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new GbNationalInsuranceNumber(value);
      var expected = GetRawNationalInsuranceNumber(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidNationalInsuranceNumberValues))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = GbNationalInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidPrefixFirstCharacters))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidFirstPrefixCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(ch, formatted: formatted);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = GbNationalInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidPrefixSecondCharacters))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidSecondPrefixCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix2: ch, formatted: formatted);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = GbNationalInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSuffixCharacters))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidSuffixCharacter(
      String suffix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", suffix: suffix, formatted: formatted);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = GbNationalInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = GbNationalInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = GbNationalInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixValues))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnInvalidPrefix_WhenValueHasInvalidPrefix(
      String prefix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix, formatted: formatted);
      LocalValidationResult expected = GetInvalidPrefixResult(value);

      // Act.
      var result = GbNationalInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixFirstCharacters))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidPrefixFirstCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(ch, formatted: formatted);
      LocalValidationResult expected = GetInvalidCharacterResult(value, 0);

      // Act.
      var result = GbNationalInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidPrefixSecondCharacters))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidPrefixSecondCharacter(
      Char ch,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber(prefix2: ch, formatted: formatted);
      LocalValidationResult expected = GetInvalidCharacterResult(value, 1);

      // Act.
      var result = GbNationalInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDigits))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidDigitCharacters(
      String digits,
      Int32 position,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", digits: digits, formatted: formatted);
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = GbNationalInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSuffixCharacters))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidSuffixCharacter(
      String suffix,
      Boolean formatted)
   {
      // Arrange.
      var value = GetNationalInsuranceNumber("AB", suffix: suffix, formatted: formatted);
      LocalValidationResult expected = GetInvalidCharacterResult(value, value.Length - 1);

      // Act.
      var result = GbNationalInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void GbNationalInsuranceNumber_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = GbNationalInsuranceNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void GbNationalInsuranceNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new GbNationalInsuranceNumber(Valid9CharacterValue);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<GbNationalInsuranceNumber>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void GbNationalInsuranceNumber_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new GbNationalInsuranceNumber(Valid9CharacterValue);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public GbNationalInsuranceNumber NationalInsuranceNumber { get; set; } = null!;
   }

   [Fact]
   public void GbNationalInsuranceNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { NationalInsuranceNumber = new GbNationalInsuranceNumber(AltValid9CharacterValue) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void GbNationalInsuranceNumber_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"NationalInsuranceNumber\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void GbNationalInsuranceNumber_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"NationalInsuranceNumber\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.NationalInsuranceNumber.Should().BeNull();
   }

   [Fact]
   public void GbNationalInsuranceNumber_JsonDeserialization_ShouldThrowKfValidationException_WhenNationalInsuranceNumberIsInvalid()
   {
      // Arrange.
      var json = "{\"NationalInsuranceNumber\":\"BG123456\"}";  // Invalid prefix
      LocalValidationError expected = GetInvalidPrefixResult("BG123456");

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
