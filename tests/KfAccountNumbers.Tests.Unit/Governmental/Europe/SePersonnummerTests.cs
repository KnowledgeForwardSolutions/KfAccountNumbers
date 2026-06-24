// Ignore Spelling: Deserialize Deserialization Json Kf Personnummer Samordningsnummer

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

using Microsoft.Extensions.Time.Testing;

using LocalCreateResult = KfAccountNumbers.Results.UCreateResult<
   KfAccountNumbers.Governmental.Europe.SePersonnummer,
   KfAccountNumbers.Governmental.Europe.SePersonnummer.ValidationError>;
using LocalValidationError = KfAccountNumbers.Governmental.Europe.SePersonnummer.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.Governmental.Europe.SePersonnummer.ValidationError>;
using LocalValidationResult = KfAccountNumbers.Governmental.Europe.SePersonnummer.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class SePersonnummerTests
{
   private const String Valid11CharacterDashPersonnummer = "811228-9874";        // From Wikipedia, https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
   private const String Valid11CharacterPlusPersonnummer = "811228+9874";
   private const String Valid13CharacterDashPersonnummer = "19670919-9530";
   private const String Valid13CharacterPlusPersonnummer = "20670919+9530";      // Future date, but valid format and checksum

   private const String Valid11CharacterDashSamordningsnummer = "811288-9871";
   private const String Valid11CharacterPlusSamordningsnummer = "811288+9871";
   private const String Valid13CharacterDashSamordningsnummer = "19670979-9537";
   private const String Valid13CharacterPlusSamordningsnummer = "20670979+9537"; // Future date, but valid format and checksum

   private static String GetPersonnummerWithValidCheckDigit(
      String dateOfBirth = "811228",
      Char separator = '-',
      String birthSerialNumber = "987")
   {
      var partialPersonnummer = $"{dateOfBirth[^6..]}{birthSerialNumber}";
      _ = Algorithms.Luhn.TryCalculateCheckDigit(partialPersonnummer, out var checkDigit);

      return $"{dateOfBirth}{separator}{birthSerialNumber}{checkDigit}";
   }

   private static String GetInternalRepresentation(String personnummer)
   {
      if (personnummer.Length == 13)
      {
         return personnummer[..8] + personnummer[^4..];
      }

      var year = ((personnummer[0] - Chars.DigitZero) * 10)
         + (personnummer[1] - Chars.DigitZero);
      year = CenturyCutoff.DefaultInstance.ToFourDigitYear(year);
      if (personnummer[6] == Chars.Plus)
      {
         year -= 100;
      }

      return $"{year}{personnummer[2..6]}{personnummer[^4..]}";
   }

   public static TheoryData<String> ValidPersonnummerValues =>
   [
      Valid11CharacterDashPersonnummer,
      Valid11CharacterPlusPersonnummer,
      Valid13CharacterDashPersonnummer,
      Valid13CharacterPlusPersonnummer,
   ];

   public static TheoryData<String> ValidSamordningsnummerValues =>
   [
      Valid11CharacterDashSamordningsnummer,
      Valid11CharacterPlusSamordningsnummer,
      Valid13CharacterDashSamordningsnummer,
      Valid13CharacterPlusSamordningsnummer
   ];

   public static TheoryData<String> UndetectableCheckDigitErrors =>
   [
      "010430-0918",       // 010430-9018 with two digit transposition 90 -> 09
      "880411+2558",       // 880411+2228 with two digit twin error 22 -> 55
      "20010430-0918",     // 20010430-9018 with two digit transposition 90 -> 09
      "19880411-2558",     // 19880411+2228 with two digit twin error 22 -> 55

      "010490-0915",       // 010490-9015 with two digit transposition 90 -> 09
      "880471+2555",       // 880471+2225 with two digit twin error 22 -> 55
      "20010490-0915",     // 20010490-9015 with two digit transposition 90 -> 09
      "19880471-2555",     // 19880471+2225 with two digit twin error 22 -> 55
   ];

   public static TheoryData<String, Char> ValidDateOfBirthValues = new()
   {
      // Personnummer values
      // Min/max/boundary values
      { "500101", '+' },         // Minimum valid six digit date, January 1, 1850
      { "991231", '+' },         // December 31, 1899
      { "000101", '+' },         // January 1, 1900
      { "991231", '-' },         // December 31 1999
      { "000101", '-' },         // January 1, 2000
      { "491231", '-' },         // Maximum valid six digit date, December 31, 2049

      // Month max days
      { "010131", '-' },         // Max day of month January 2001
      { "910228", '+' },         // Max day of month February 1891 (non-leap year)
      { "960229", '+' },         // Max day of month February 1896 (leap year)
      { "000228", '+' },         // Max day of month February 1900 (non-leap year)
      { "520229", '-' },         // Max day of month February 1952 (leap year)
      { "000229", '-' },         // Max day of month February 2000 (leap year, century divisible by 400)
      { "010228", '-' },         // Max day of month February 2001 (non-leap year)
      { "530331", '+' },         // Max day of month March 1853
      { "080430", '-' },         // Max day of month April 2008
      { "150531", '+' },         // Max day of month May 1915
      { "600630", '-' },         // Max day of month June 1960
      { "750731", '+' },         // Max day of month July 1875
      { "810831", '-' },         // Max day of month August 1981
      { "090930", '+' },         // Max day of month September 1909
      { "101031", '-' },         // Max day of month October 2010
      { "111130", '+' },         // Max day of month November 1911
      { "121231", '-' },         // Max day of month December 2012

      // Min/max/boundary values
      { "18500101", '+' },       // Minimum valid six digit date, January 1, 1850
      { "18991231", '+' },       // December 31, 1899
      { "19000101", '+' },       // January 1, 1900
      { "19991231", '-' },       // December 31 1999
      { "20000101", '-' },       // January 1, 2000
      { "20491231", '-' },       // Maximum valid six digit date, December 31, 2049

      // Month max days
      { "20010131", '-' },       // Max day of month January 2001
      { "18910228", '+' },       // Max day of month February 1891 (non-leap year)
      { "18960229", '+' },       // Max day of month February 1896 (leap year)
      { "19000228", '+' },       // Max day of month February 1900 (non-leap year)
      { "19520229", '-' },       // Max day of month February 1952 (leap year)
      { "20000229", '-' },       // Max day of month February 2000 (leap year, century divisible by 400)
      { "20010228", '-' },       // Max day of month February 2001 (non-leap year)
      { "18530331", '+' },       // Max day of month March 1853
      { "20080430", '-' },       // Max day of month April 2008
      { "19150531", '+' },       // Max day of month May 1915
      { "19600630", '-' },       // Max day of month June 1960
      { "18750731", '+' },       // Max day of month July 1875
      { "19810831", '-' },       // Max day of month August 1981
      { "19090930", '+' },       // Max day of month September 1909
      { "20101031", '-' },       // Max day of month October 2010
      { "19111130", '+' },       // Max day of month November 1911
      { "20121231", '-' },       // Max day of month December 2012

      // Samordningsnummer values
      // Min/max/boundary values
      { "500161", '+' },         // Minimum valid six digit date, January 1, 1850
      { "991291", '+' },         // December 31, 1899
      { "000161", '+' },         // January 1, 1900
      { "991291", '-' },         // December 31 1999
      { "000161", '-' },         // January 1, 2000
      { "491291", '-' },         // Maximum valid six digit date, December 31, 2049

      // Month max days
      { "010191", '-' },         // Max day of month January 2001
      { "910288", '+' },         // Max day of month February 1891 (non-leap year)
      { "960289", '+' },         // Max day of month February 1896 (leap year)
      { "000288", '+' },         // Max day of month February 1900 (non-leap year)
      { "520289", '-' },         // Max day of month February 1952 (leap year)
      { "000289", '-' },         // Max day of month February 2000 (leap year, century divisible by 400)
      { "010288", '-' },         // Max day of month February 2001 (non-leap year)
      { "530391", '+' },         // Max day of month March 1853
      { "080490", '-' },         // Max day of month April 2008
      { "150591", '+' },         // Max day of month May 1915
      { "600690", '-' },         // Max day of month June 1960
      { "750791", '+' },         // Max day of month July 1875
      { "810891", '-' },         // Max day of month August 1981
      { "090990", '+' },         // Max day of month September 1909
      { "101091", '-' },         // Max day of month October 2010
      { "111190", '+' },         // Max day of month November 1911
      { "121291", '-' },         // Max day of month December 2012

      // Min/max/boundary values
      { "18500161", '+' },       // Minimum valid six digit date, January 1, 1850
      { "18991291", '+' },       // December 31, 1899
      { "19000161", '+' },       // January 1, 1900
      { "19991291", '-' },       // December 31 1999
      { "20000161", '-' },       // January 1, 2000
      { "20491291", '-' },       // Maximum valid six digit date, December 31, 2049

      // Month max days
      { "20010191", '-' },       // Max day of month January 2001
      { "18910288", '+' },       // Max day of month February 1891 (non-leap year)
      { "18960289", '+' },       // Max day of month February 1896 (leap year)
      { "19000288", '+' },       // Max day of month February 1900 (non-leap year)
      { "19520289", '-' },       // Max day of month February 1952 (leap year)
      { "20000289", '-' },       // Max day of month February 2000 (leap year, century divisible by 400)
      { "20010288", '-' },       // Max day of month February 2001 (non-leap year)
      { "18530391", '+' },       // Max day of month March 1853
      { "20080490", '-' },       // Max day of month April 2008
      { "19150591", '+' },       // Max day of month May 1915
      { "19600690", '-' },       // Max day of month June 1960
      { "18750791", '+' },       // Max day of month July 1875
      { "19810891", '-' },       // Max day of month August 1981
      { "19090990", '+' },       // Max day of month September 1909
      { "20101091", '-' },       // Max day of month October 2010
      { "19111190", '+' },       // Max day of month November 1911
      { "20121291", '-' },       // Max day of month December 2012
   };

   public static TheoryData<String> InvalidLengthValues =>
   [
      "811228-987",        // Length 10
      "811228-98745",      // Length 12
      "19811228-98745",    // Length 14
      new String('1', 100) // Very long string
   ];

   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Short format values
      { "A11228-9874", 0 },         // Non-digit character 'A'
      { "8B1228-9874", 1 },         // Non-digit character 'B'
      { "81!228-9874", 2 },         // Non-digit character '!'
      { "811 28-9874", 3 },         // Non-digit character ' '
      { "8112a8-9874", 4 },         // Non-digit character 'a'
      { "81122\u2153-9874", 5 },    // Non-digit character Unicode fraction 1/3
      { "811228-+874", 7 },         // Non-digit character '+'
      { "811228-9.74", 8 },         // Non-digit character '.'
      { "811228-98:4", 9 },         // Non-digit character ':'
      { "811228-987~", 10 },        // Non-digit character '~'

      // Long format values
      { "z9811228-9874", 0 },       // Non-digit character 'z'
      { "1^811228-9874", 1 },       // Non-digit character '^'
      { "19A11228-9874", 2 },       // Non-digit character 'A'
      { "198B1228-9874", 3 },       // Non-digit character 'B'
      { "1981!228-9874", 4 },       // Non-digit character '!'
      { "19811 28-9874", 5 },       // Non-digit character ' '
      { "198112a8-9874", 6 },       // Non-digit character 'a'
      { "1981122\u2153-9874", 7 },  // Non-digit character Unicode fraction 1/3
      { "19811228-+874", 9 },       // Non-digit character '+'
      { "19811228-9.74", 10 },      // Non-digit character '.'
      { "19811228-98:4", 11 },      // Non-digit character ':'
      { "19811228-987~", 12 },      // Non-digit character '~'
   };

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "811228-9875",       // 811228-9874 with invalid check digit 4 -> 5
      "811227-9874",       // 811228-9874 with single digit transcription error 8 -> 7
      "821228-9874",       // 811228-9874 with single digit transcription error 1 -> 2
      "181228-9874",       // 811228-9874 with two digit transcription error 81 -> 18
      "811228-9847",       // 811228-9874 with two digit transcription error 74 -> 47
      "880422+1238",       // 880411+1238 with two digit twin error 11 -> 22
      "880411+3328",       // 880411+2228 with two digit twin error 22 -> 33
      "19811228-9875",     // 19811228-9874 with invalid check digit 4 -> 5
      "19811227-9874",     // 19811228-9874 with single digit transcription error 8 -> 7
      "20821228-9874",     // 20811228-9874 with single digit transcription error 1 -> 2
      "20181228-9874",     // 20811228-9874 with two digit transcription error 81 -> 18
      "19811228-9847",     // 19811228-9874 with two digit transcription error 74 -> 47
      "19880422+1238",     // 19880411+1238 with two digit twin error 11 -> 22
      "19880411+3328",     // 19880411+2228 with two digit twin error 22 -> 33
   ];

   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      { "811228*9874", 6 },
      { "19811228*9874", 8 },
   };

   public static TheoryData<String, Char> InvalidDateOfBirthValues = new()
   {
      // Personnummer values
      // Invalid month
      { "010001", '+' },      // Invalid month (too low)
      { "011301", '-' },      // Invalid month (too high)

      // Invalid day
      { "010100", '+' },      // Invalid day of month (too low)
      { "010132", '-' },      // Invalid day of month January
      { "010229", '+' },      // Invalid day of month February (non leap year)
      { "000229", '+' },      // Invalid day of month February (1900 is not a leap year)
      { "040230", '+' },      // Invalid day of month February (leap year)
      { "010332", '-' },      // Invalid day of month March
      { "010431", '+' },      // Invalid day of month April
      { "010532", '-' },      // Invalid day of month May
      { "010631", '+' },      // Invalid day of month June
      { "010732", '-' },      // Invalid day of month July
      { "010832", '+' },      // Invalid day of month August
      { "010931", '-' },      // Invalid day of month September
      { "011032", '+' },      // Invalid day of month October
      { "011131", '-' },      // Invalid day of month November
      { "011232", '+' },      // Invalid day of month December

      // Invalid year
      { "17991232", '+' },    // Invalid year (too low)
      { "21010101", '-' },    // Invalid year (too high)

      // Invalid month
      { "19010001", '+' },    // Invalid month (too low)
      { "20011301", '-' },    // Invalid month (too high)

      // Invalid day
      { "19010100", '+' },    // Invalid day of month (too low)
      { "20010132", '-' },    // Invalid day of month January
      { "19010229", '+' },    // Invalid day of month February (non leap year)
      { "19000229", '+' },    // Invalid day of month February (1900 is not a leap year)
      { "19040230", '+' },    // Invalid day of month February (leap year)
      { "20010332", '-' },    // Invalid day of month March
      { "19010431", '+' },    // Invalid day of month April
      { "20010532", '-' },    // Invalid day of month May
      { "19010631", '+' },    // Invalid day of month June
      { "20010732", '-' },    // Invalid day of month July
      { "19010832", '+' },    // Invalid day of month August
      { "20010931", '-' },    // Invalid day of month September
      { "19011032", '+' },    // Invalid day of month October
      { "20011131", '-' },    // Invalid day of month November
      { "19011232", '+' },    // Invalid day of month December

      // Samordningsnummer values
      // Invalid month
      { "010061", '+' },      // Invalid month (too low)
      { "011361", '-' },      // Invalid month (too high)

      // Invalid day
      { "010160", '+' },      // Invalid day of month (too low)
      { "010192", '-' },      // Invalid day of month January
      { "010289", '+' },      // Invalid day of month February (non leap year)
      { "000289", '+' },      // Invalid day of month February (1900 is not a leap year)
      { "040290", '+' },      // Invalid day of month February (leap year)
      { "010392", '-' },      // Invalid day of month March
      { "010491", '+' },      // Invalid day of month April
      { "010592", '-' },      // Invalid day of month May
      { "010691", '+' },      // Invalid day of month June
      { "010792", '-' },      // Invalid day of month July
      { "010892", '+' },      // Invalid day of month August
      { "010991", '-' },      // Invalid day of month September
      { "011092", '+' },      // Invalid day of month October
      { "011191", '-' },      // Invalid day of month November
      { "011292", '+' },      // Invalid day of month December

      // Invalid year
      { "17991292", '+' },    // Invalid year (too low)
      { "21010161", '-' },    // Invalid year (too high)

      // Invalid month
      { "19010061", '+' },    // Invalid month (too low)
      { "20011361", '-' },    // Invalid month (too high)

      // Invalid day
      { "19010160", '+' },    // Invalid day of month (too low)
      { "20010192", '-' },    // Invalid day of month January
      { "19010289", '+' },    // Invalid day of month February (non leap year)
      { "19000289", '+' },    // Invalid day of month February (1900 is not a leap year)
      { "19040290", '+' },    // Invalid day of month February (leap year)
      { "20010392", '-' },    // Invalid day of month March
      { "19010491", '+' },    // Invalid day of month April
      { "20010592", '-' },    // Invalid day of month May
      { "19010691", '+' },    // Invalid day of month June
      { "20010792", '-' },    // Invalid day of month July
      { "19010892", '+' },    // Invalid day of month August
      { "20010991", '-' },    // Invalid day of month September
      { "19011092", '+' },    // Invalid day of month October
      { "20011191", '-' },    // Invalid day of month November
      { "19011292", '+' },    // Invalid day of month December
   };

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.SePersonnummerInvalidLength,
         value.Length,
         SePersonnummer.GetValidLengthDefinitions());

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.SePersonnummerInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.SePersonnummerInvalidCheckDigit,
         Algorithms.Luhn.AlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(
         Messages.SePersonnummerInvalidSeparator,
         value[position],
         position);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => value.Length == 11
         ? new InvalidDateOfBirth(Messages.SePersonnummerInvalidDateOfBirth, value[..6], DateFormatName.YYMMDD)
         : new InvalidDateOfBirth(Messages.SePersonnummerInvalidDateOfBirth, value[..8], DateFormatName.YYYYMMDD);

   #region Constants Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_MinimumValidYearOfBirth_ShouldHaveExpectedValue()
      => SePersonnummer.MinimumValidYearOfBirth.Should().Be(1800);

   [Fact]
   public void SePersonnummer_MaximumValidYearOfBirth_ShouldHaveExpectedValue()
      => SePersonnummer.MaximumValidYearOfBirth.Should().Be(2099);

   [Fact]
   public void SePersonnummer_SamordningsnummerOffset_ShouldHaveExpectedValue()
      => SePersonnummer.SamordningsnummerDayOffset.Should().Be(60);

   #endregion

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetInternalRepresentation(value);

      // Act.
      var sut = new SePersonnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(UndetectableCheckDigitErrors))]
   public void SePersonnummer_Constructor_ShouldCreateInstance_WhenCheckDigitHasUndetectableError(String value)
   {
      // Arrange.
      var expected = GetInternalRepresentation(value);

      // Act.
      var sut = new SePersonnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void SePersonnummer_Constructor_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetPersonnummerWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      var expected = GetInternalRepresentation(value);

      // Act.
      var sut = new SePersonnummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void SePersonnummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetPersonnummerWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new SePersonnummer(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("500101", '+', "18500101")]      // Minimum six digit date of birth, January 1, 1850
   [InlineData("491231", '-', "20491231")]      // Maximum six digit date of birth, December 31, 2049

   [InlineData("991231", '+', "18991231")]      // December 31, 1899
   [InlineData("000101", '+', "19000101")]      // January 1, 1900
   [InlineData("991231", '-', "19991231")]      // December 31, 1999
   [InlineData("000101", '-', "20000101")]      // January 1, 2000

   [InlineData("910228", '+', "18910228")]      // Max day of month February 1891 (non-leap year)
   [InlineData("960229", '+', "18960229")]      // Max day of month February 1896 (leap year)
   [InlineData("000228", '+', "19000228")]      // Max day of month February 1900 (non-leap year)
   [InlineData("520229", '-', "19520229")]      // Max day of month February 1952 (leap year)
   [InlineData("000229", '-', "20000229")]      // Max day of month February 2000 (leap year, century divisible by 400)
   [InlineData("010228", '-', "20010228")]      // Max day of month February 2001 (non-leap year)

   [InlineData("18000101", '+', "18000101")]    // Minimum eight digit date of birth, January 1, 1800
   [InlineData("20991231", '+', "20991231")]    // Maximum eight digit date of birth, December 31, 2099

   [InlineData("18991231", '+', "18991231")]    // December 31, 1899
   [InlineData("19000101", '+', "19000101")]    // January 1, 1900
   [InlineData("19991231", '-', "19991231")]    // December 31, 1999
   [InlineData("20000101", '-', "20000101")]    // January 1, 2000

   [InlineData("18910228", '+', "18910228")]    // Max day of month February 1891 (non-leap year)
   [InlineData("18960229", '+', "18960229")]    // Max day of month February 1896 (leap year)
   [InlineData("19000228", '+', "19000228")]    // Max day of month February 1900 (non-leap year)
   [InlineData("19520229", '-', "19520229")]    // Max day of month February 1952 (leap year)
   [InlineData("20000229", '-', "20000229")]    // Max day of month February 2000 (leap year, century divisible by 400)
   [InlineData("20010228", '-', "20010228")]    // Max day of month February 2001 (non-leap year)

   // Samordningsnummer values
   [InlineData("500161", '+', "18500101")]      // Minimum six digit date of birth, January 1, 1850
   [InlineData("491291", '-', "20491231")]      // Maximum six digit date of birth, December 31, 2049

   [InlineData("991291", '+', "18991231")]      // December 31, 1899
   [InlineData("000161", '+', "19000101")]      // January 1, 1900
   [InlineData("991291", '-', "19991231")]      // December 31, 1999
   [InlineData("000161", '-', "20000101")]      // January 1, 2000

   [InlineData("910288", '+', "18910228")]      // Max day of month February 1891 (non-leap year)
   [InlineData("960289", '+', "18960229")]      // Max day of month February 1896 (leap year)
   [InlineData("000288", '+', "19000228")]      // Max day of month February 1900 (non-leap year)
   [InlineData("520289", '-', "19520229")]      // Max day of month February 1952 (leap year)
   [InlineData("000289", '-', "20000229")]      // Max day of month February 2000 (leap year, century divisible by 400)
   [InlineData("010288", '-', "20010228")]      // Max day of month February 2001 (non-leap year)

   [InlineData("18000161", '+', "18000101")]    // Minimum eight digit date of birth, January 1, 1800
   [InlineData("20991291", '+', "20991231")]    // Maximum eight digit date of birth, December 31, 2099

   [InlineData("18991291", '+', "18991231")]    // December 31, 1899
   [InlineData("19000161", '+', "19000101")]    // January 1, 1900
   [InlineData("19991291", '-', "19991231")]    // December 31, 1999
   [InlineData("20000161", '-', "20000101")]    // January 1, 2000

   [InlineData("18910288", '+', "18910228")]    // Max day of month February 1891 (non-leap year)
   [InlineData("18960289", '+', "18960229")]    // Max day of month February 1896 (leap year)
   [InlineData("19000288", '+', "19000228")]    // Max day of month February 1900 (non-leap year)
   [InlineData("19520289", '-', "19520229")]    // Max day of month February 1952 (leap year)
   [InlineData("20000289", '-', "20000229")]    // Max day of month February 2000 (leap year, century divisible by 400)
   [InlineData("20010288", '-', "20010228")]    // Max day of month February 2001 (non-leap year)
   public void SePersonnummer_DateOfBirth_ShouldReturnExpectedValue(
      String dateOfBirth,
      Char separator,
      String expectedDateOfBirth)
   {
      // Arrange.
      var value = GetPersonnummerWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      var sut = new SePersonnummer(value);
      var expected = DateOnly.ParseExact(
         expectedDateOfBirth,
         "yyyyMMdd",
         CultureInfo.InvariantCulture);

      // Act/assert.
      sut.DateOfBirth.Should().Be(expected);
   }

   #endregion

   #region Gender Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("811228", '1')]
   [InlineData("811228", '3')]
   [InlineData("811228", '5')]
   [InlineData("811228", '7')]
   [InlineData("811228", '9')]
   [InlineData("19811228", '1')]
   [InlineData("19811228", '3')]
   [InlineData("19811228", '5')]
   [InlineData("19811228", '7')]
   [InlineData("19811228", '9')]
   public void SePersonnummer_Gender_ShouldReturnMale_ForValuesWithOddGenderIndicator(
      String dateOfBirth,
      Char digit)
   {
      // Arrange.
      var birthSerialNumber = $"54{digit}";
      var value = GetPersonnummerWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         birthSerialNumber: birthSerialNumber);
      var sut = new SePersonnummer(value);
      Gender.BinaryGender expected = default(Gender.Male);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData("811228", '0')]
   [InlineData("811228", '2')]
   [InlineData("811228", '4')]
   [InlineData("811228", '6')]
   [InlineData("811228", '8')]
   [InlineData("19811228", '0')]
   [InlineData("19811228", '2')]
   [InlineData("19811228", '4')]
   [InlineData("19811228", '6')]
   [InlineData("19811228", '8')]
   public void SePersonnummer_Gender_ShouldReturnFemale_ForValuesWithEvenGenderIndicator(
      String dateOfBirth,
      Char digit)
   {
      // Arrange.
      var birthSerialNumber = $"54{digit}";
      var value = GetPersonnummerWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         birthSerialNumber: birthSerialNumber);
      var sut = new SePersonnummer(value);
      Gender.BinaryGender expected = default(Gender.Female);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region IdentifierType Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("890301")]
   [InlineData("890311")]
   [InlineData("890321")]
   [InlineData("890331")]
   [InlineData("20050301")]
   [InlineData("20050311")]
   [InlineData("20050321")]
   [InlineData("20050331")]
   public void SePersonnummer_IdentifierType_ShouldReturnExpectedValue_WhenValueIsPersonnummer(String dateOfBirth)
   {
      // Arrange.
      var personnummer = GetPersonnummerWithValidCheckDigit(dateOfBirth: dateOfBirth);
      var sut = new SePersonnummer(personnummer);
      SePersonnummer.IdentifierCategory expected = default(SeIdentifierType.Personnummer);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   [Theory]
   [InlineData("890361")]
   [InlineData("890371")]
   [InlineData("890381")]
   [InlineData("890391")]
   [InlineData("20050361")]
   [InlineData("20050371")]
   [InlineData("20050381")]
   [InlineData("20050391")]
   public void SePersonnummer_IdentifierType_ShouldReturnExpectedValue_WhenValueIsSamordningsnummer(String dateOfBirth)
   {
      // Arrange.
      var personnummer = GetPersonnummerWithValidCheckDigit(dateOfBirth: dateOfBirth);
      var sut = new SePersonnummer(personnummer);
      SePersonnummer.IdentifierCategory expected = default(SeIdentifierType.Samordningsnummer);

      // Act/assert.
      sut.IdentifierType.Should().Be(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_Value_ShouldReturnValidatedPersonnummer(String personnummer)
   {
      // Arrange.
      var sut = new SePersonnummer(personnummer);
      var expected = GetInternalRepresentation(personnummer);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = Valid11CharacterDashPersonnummer;
      var sut = new SePersonnummer(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void SePersonnummer_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = Valid11CharacterDashPersonnummer;
      var sut = new SePersonnummer(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void SePersonnummer_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      SePersonnummer sut = null!;

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void SePersonnummer_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      SePersonnummer sut = null!;

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new SePersonnummer(value);

      // Act.
      var sut = (SePersonnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(UndetectableCheckDigitErrors))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldCreateInstance_WhenCheckDigitHasUndetectableError(String value)
   {
      // Arrange.
      var expected = new SePersonnummer(value);

      // Act.
      var sut = (SePersonnummer)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetPersonnummerWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      var sut = (SePersonnummer)value;
      var expected = new SePersonnummer(value);

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void SePersonnummer_ExplicitCastToSePersonnummer_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetPersonnummerWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (SePersonnummer)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(Valid11CharacterDashSamordningsnummer);
      var sut2 = new SePersonnummer(Valid11CharacterDashSamordningsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void SePersonnummer_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(Valid13CharacterDashPersonnummer);
      var sut2 = new SePersonnummer(Valid13CharacterDashSamordningsnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var sut2 = new SePersonnummer("19" + Valid11CharacterDashPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(Valid13CharacterDashPersonnummer);
      var sut2 = new SePersonnummer(Valid13CharacterDashSamordningsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void SePersonnummer_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var sut2 = new SePersonnummer("19" + Valid11CharacterDashPersonnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(Valid11CharacterDashSamordningsnummer);
      var sut2 = new SePersonnummer(Valid11CharacterDashSamordningsnummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new SePersonnummer(value);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(UndetectableCheckDigitErrors))]
   public void SePersonnummer_Create_ShouldCreateInstance_WhenValueHasUndetectableCheckDigitError(String value)
   {
      // Arrange.
      LocalCreateResult expected = new SePersonnummer(value);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void SePersonnummer_Create_ShouldCreateInstance_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetPersonnummerWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalCreateResult expected = new SePersonnummer(value);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SePersonnummer_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidCheckDigitValidationResult_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void SePersonnummer_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetPersonnummerWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = SePersonnummer.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(Valid13CharacterPlusPersonnummer);
      var sut2 = new SePersonnummer(Valid13CharacterPlusPersonnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(Valid13CharacterPlusPersonnummer);     // +
      var sut2 = new SePersonnummer(Valid13CharacterDashPersonnummer);     // -

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var sut2 = new SePersonnummer("19" + Valid11CharacterDashPersonnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new SePersonnummer(Valid13CharacterPlusPersonnummer);

      // Act/assert.
      sut.Equals(Valid13CharacterPlusPersonnummer).Should().BeFalse();
   }

   [Fact]
   public void SePersonnummer_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new SePersonnummer(Valid13CharacterPlusPersonnummer);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var sut2 = new SePersonnummer(Valid11CharacterDashPersonnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void SePersonnummer_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var sut2 = new SePersonnummer(Valid11CharacterDashSamordningsnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void SePersonnummer_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 13 character versions for same person should still be equal.
      var sut1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var sut2 = new SePersonnummer("19" + Valid11CharacterDashPersonnummer);

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

   // SePersonnummer does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void SePersonnummer_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new SePersonnummer(Valid11CharacterDashPersonnummer);
      var sut2 = new SePersonnummer(Valid11CharacterDashPersonnummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToLongFormat Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_ToLongFormat_ShouldReturnExpectedValue_WhenTimeProviderIsNull(String value)
   {
      // Arrange.
      var sut = new SePersonnummer(value);
      var expected = sut.Value[..8] + '-' + sut.Value[^4..];

      // Act/assert.
      sut.ToLongFormatValue().Should().Be(expected);
   }

   [Theory]
   [InlineData(Valid11CharacterDashPersonnummer, -150, 0, '-')]
   [InlineData(Valid11CharacterDashPersonnummer, -50, 0, '-')]
   [InlineData(Valid11CharacterDashPersonnummer, 50, 0, '-')]
   [InlineData(Valid11CharacterDashPersonnummer, 100, -1, '-')]
   [InlineData(Valid11CharacterDashPersonnummer, 100, 0, '+')]
   [InlineData(Valid11CharacterDashPersonnummer, 100, 1, '+')]
   [InlineData(Valid13CharacterDashPersonnummer, 100, -1, '-')]
   [InlineData(Valid13CharacterDashPersonnummer, 100, 0, '+')]
   [InlineData(Valid13CharacterDashPersonnummer, 100, 1, '+')]
   [InlineData(Valid11CharacterDashSamordningsnummer, 100, -1, '-')]
   [InlineData(Valid11CharacterDashSamordningsnummer, 100, 0, '+')]
   [InlineData(Valid11CharacterDashSamordningsnummer, 100, 1, '+')]
   [InlineData(Valid13CharacterDashSamordningsnummer, 100, -1, '-')]
   [InlineData(Valid13CharacterDashSamordningsnummer, 100, 0, '+')]
   [InlineData(Valid13CharacterDashSamordningsnummer, 100, 1, '+')]
   public void SePersonnummer_ToLongFormat_ShouldReturnExpectedValue_WhenTimeProviderIsSupplied(
      String value,
      Int32 years,
      Int32 days,
      Char expectedSeparator)
   {
      // Arrange.
      var sut = new SePersonnummer(value);
      var currentDate = sut.DateOfBirth.AddYears(years).AddDays(days).ToDateTime(TimeOnly.MinValue);
      var timeProvider = new FakeTimeProvider(currentDate);
      var expected = $"{sut.Value[..8]}{expectedSeparator}{sut.Value[^4..]}";

      // Act.
      var result = sut.ToLongFormatValue(timeProvider);

      // Assert.
      result.Should().Be(expected);
   }

   #endregion

   #region ToShortFormat Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_ToShortFormat_ShouldReturnExpectedValue_WhenTimeProviderIsNull(String value)
   {
      // Arrange.
      var sut = new SePersonnummer(value);
      var expected = sut.Value[2..8] + '-' + sut.Value[^4..];

      // Act/assert.
      sut.ToShortFormatValue().Should().Be(expected);
   }

   [Theory]
   [InlineData(Valid11CharacterDashPersonnummer, -150, 0, '-')]
   [InlineData(Valid11CharacterDashPersonnummer, -50, 0, '-')]
   [InlineData(Valid11CharacterDashPersonnummer, 50, 0, '-')]
   [InlineData(Valid11CharacterDashPersonnummer, 100, -1, '-')]
   [InlineData(Valid11CharacterDashPersonnummer, 100, 0, '+')]
   [InlineData(Valid11CharacterDashPersonnummer, 100, 1, '+')]
   [InlineData(Valid13CharacterDashPersonnummer, 100, -1, '-')]
   [InlineData(Valid13CharacterDashPersonnummer, 100, 0, '+')]
   [InlineData(Valid13CharacterDashPersonnummer, 100, 1, '+')]
   [InlineData(Valid11CharacterDashSamordningsnummer, 100, -1, '-')]
   [InlineData(Valid11CharacterDashSamordningsnummer, 100, 0, '+')]
   [InlineData(Valid11CharacterDashSamordningsnummer, 100, 1, '+')]
   [InlineData(Valid13CharacterDashSamordningsnummer, 100, -1, '-')]
   [InlineData(Valid13CharacterDashSamordningsnummer, 100, 0, '+')]
   [InlineData(Valid13CharacterDashSamordningsnummer, 100, 1, '+')]
   public void SePersonnummer_ToShortFormat_ShouldReturnExpectedValue_WhenTimeProviderIsSupplied(
      String value,
      Int32 years,
      Int32 days,
      Char expectedSeparator)
   {
      // Arrange.
      var sut = new SePersonnummer(value);
      var currentDate = sut.DateOfBirth.AddYears(years).AddDays(days).ToDateTime(TimeOnly.MinValue);
      var timeProvider = new FakeTimeProvider(currentDate);
      var expected = $"{sut.Value[2..8]}{expectedSeparator}{sut.Value[^4..]}";

      // Act.
      var result = sut.ToShortFormatValue(timeProvider);

      // Assert.
      result.Should().Be(expected);
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new SePersonnummer(value);
      var expected = sut.ToLongFormatValue();

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   [MemberData(nameof(ValidSamordningsnummerValues))]
   public void SePersonnummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(UndetectableCheckDigitErrors))]
   public void SePersonnummer_Validate_ShouldReturnValidationPassed_WhenValueHasUndetectableCheckDigitError(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void SePersonnummer_Validate_ShouldReturnValidationPassed_WhenDateOfBirthIsValid(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetPersonnummerWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SePersonnummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
      String dateOfBirth,
      Char separator)
   {
      // Arrange.
      var value = GetPersonnummerWithValidCheckDigit(
         dateOfBirth: dateOfBirth,
         separator: separator);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = SePersonnummer.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void SePersonnummer_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new SePersonnummer(Valid11CharacterDashPersonnummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<SePersonnummer>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void SePersonnummer_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new SePersonnummer(Valid11CharacterDashSamordningsnummer);
      var expected = sut.ToLongFormatValue();

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public SePersonnummer Personnummer { get; set; } = null!;
   }

   [Fact]
   public void SePersonnummer_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Personnummer = new SePersonnummer(Valid13CharacterDashPersonnummer) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void SePersonnummer_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Personnummer\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void SePersonnummer_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Personnummer\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Personnummer.Should().BeNull();
   }

   [Fact]
   public void SePersonnummer_JsonDeserialization_ShouldThrowKfValidationException_WhenPersonnummerIsInvalid()
   {
      // Arrange.
      var json = "{\"Personnummer\":\"811228-9875\"}";  // Invalid check digit
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
