using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.National.Europe.ItCodiceFiscale,
   KfAccountNumbers.National.Europe.ItCodiceFiscale.ValidationError>;
using LocalValidationError = KfAccountNumbers.National.Europe.ItCodiceFiscale.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.National.Europe.ItCodiceFiscale.ValidationError>;
using LocalValidationResult = KfAccountNumbers.National.Europe.ItCodiceFiscale.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class ItCodiceFiscaleTests
{
   private const String ValidUpperCaseCodiceFiscale = "MRTMTT91D08F205J";
   private const String AltValidUpperCaseCodiceFiscale = "MLLSNT82P65Z404U";
   private const String ValidUpperCaseOmocodiaCodiceFiscale = "RSSNTN86H08G2NST";
   private const String AltValidUpperCaseOmocodiaCodiceFiscale = "RSSNTNUSHLUGNNSZ";      // Previous value with all digits replaced by omocodia substitutions
   private const String ValidLowerCaseCodiceFiscale = "mrtmtt91d08f205j";
   private const String AltValidLowerCaseCodiceFiscale = "mllsnt82p65z404u";
   private const String ValidLowerCaseOmocodiaCodiceFiscale = "rssntn86h08G2nst";
   private const String ValidMixedCaseCodiceFiscale = "MrtMtt91D08F205j";
   private const String AltValidMixedCaseCodiceFiscale = "mllSNT82P65z404U";
   private const String ValidMixedCaseOmocodiaCodiceFiscale = "RSSNTN86H08G2nsT";

   public static TheoryData<String> ValidCodiceFiscaleValues =>
   [
      ValidUpperCaseCodiceFiscale,
      AltValidUpperCaseCodiceFiscale,
      ValidUpperCaseOmocodiaCodiceFiscale,
      AltValidUpperCaseOmocodiaCodiceFiscale,
      ValidLowerCaseCodiceFiscale,
      AltValidLowerCaseCodiceFiscale,
      ValidLowerCaseOmocodiaCodiceFiscale,
      ValidMixedCaseCodiceFiscale,
      AltValidMixedCaseCodiceFiscale,
      ValidMixedCaseOmocodiaCodiceFiscale,
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "MRTMTT91D08F205",               // Length 15
      "MRTMTT91D08F205JK",             // Length 17
      new String('1', 100)    // Very long string
   ];

   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      { ".RTMTT91D08F205J", 0 },          // Non-digit character '.'
      { "M TMTT91D08F205J", 1 },          // Non-digit character ' '
      { "MR^MTT91D08F205J", 2 },          // Non-digit character '^'
      { "MRT~TT91D08F205J", 3 },          // Non-digit character '~'
      { "MRTM\u2153T91D08F205J", 4 },     // Non-digit character Unicode fraction 1/3
      { "MRTMT\u00D691D08F205J", 5 },     // Invalid character unicode O with umlaut
      { "MRTMTT\u0BE61D08F205J", 6 },     // Invalid character unicode Tamil digit 0
      { "MRTMTT9.D08F205J", 7 },          // Non-digit character '.'
      { "MRTMTT91 08F205J", 8 },          // Non-digit character ' '
      { "MRTMTT91D^8F205J", 9 },          // Non-digit character '^'
      { "MRTMTT91D0~F205J", 10 },         // Non-digit character '~'
      { "MRTMTT91D08\u2153205J", 11 },    // Non-digit character Unicode fraction 1/3
      { "MRTMTT91D08F\u00D605J", 12 },    // Invalid character unicode O with umlaut
      { "MRTMTT91D08F2\u0BE65J", 13 },    // Invalid character unicode Tamil digit 0
      { "MRTMTT91D08F20.J", 14 },         // Non-digit character '.'
      { "MRTMTT91D08F205 ", 15 },         // Non-digit character ' '
   };

   public static TheoryData<String> InvalidCheckCharacterValues =>
   [
      "MRTMST91D08F205J",        // MRTMTT91D08F205J with single character transcription error, T -> S
      "MRTMTT92D08F205J",        // MRTMTT91D08F205J with single character transcription error, 1 -> 2
      "MRTMTT91D08F205K",        // MRTMTT91D08F205J with check character transcription error, J -> K
      "MLSLNT82P65Z404U",        // MLLSNT82P65Z404U with two character transposition error, LS -> SL
      "MLLSNT82P56Z404U",        // MLLSNT82P65Z404U with two character transposition error, 65 -> 56
      "MRTMSS91D08F205J",        // MRTMTT91D08F205J with two character twin error, TT -> SS
      "RTTNTN86H08G2NST",        // RSSNTN86H08G2NST with two character twin error, SS -> TT
      "MRTMTT91D08F2050",        // MRTMTT91D08F205J with invalid check character -> 0
      "MLLSNT82P65Z4049",        // MLLSNT82P65Z404U with invalid check character -> 9
   ];

   public static TheoryData<String> InvalidNameValues =>
   [
      "1RT",
      "M2T",
      "MR3",
      "4RT",
      "M5T",
      "MR6",
      "7RT",
      "M8T",
      "MR9",
      "0RT",
   ];

   public static TheoryData<String> InvalidYearValues =>
   [
      "A1",
      "B2",
      "C3",
      "D4",
      "E5",
      "F6",
      "G7",
      "H8",
      "1I",
      "1J",
      "1K",
      "1O",
      "1W",
      "1X",
      "1Y",
      "1Z",
      "AA",
      "a1",
      "b2",
      "c3",
      "d4",
      "e5",
      "f6",
      "g7",
      "h8",
      "1i",
      "1j",
      "1k",
      "1o",
      "1w",
      "1x",
      "1y",
      "1z",
      "aa",
   ];

   public static TheoryData<Char> InvalidMonthValues =>
   [
      'F',
      'G',
      'I',
      'J',
      'K',
      'N',
      'O',
      'Q',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'Z',
      'f',
      'g',
      'i',
      'j',
      'k',
      'n',
      'o',
      'q',
      'u',
      'v',
      'w',
      'x',
      'y',
      'z',
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
   ];

   private static readonly Int32[] _evenCharacterMap = Enumerable.Range('0', 'Z' - '0' + 1)
      .Select(ch => ItCodiceFiscale.MapEvenCharacter((Char)ch))
      .ToArray();

   private static readonly Int32[] _oddCharacterMap = Enumerable.Range('0', 'Z' - '0' + 1)
      .Select(ch => ItCodiceFiscale.MapOddCharacter((Char)ch))
      .ToArray();

   private static String GetValue(
      String surname = "ABC",
      String givenName = "DEF",
      String year = "83",
      Char month = 'M',
      String day = "26",
      String townOfBirth = "A123")
   {
      var temp = $"{surname}{givenName}{year}{month}{day}{townOfBirth}";

      return GetValueWithValidCheckDigit(temp);
   }

   private static String GetValueWithValidCheckDigit(String value)
   {
      var sum = 0;
      var isOdd = true;
      foreach (var ch in value)
      {
         var upper = Char.ToUpperInvariant(ch);
         var num = upper switch
         {
            >= '0' and <= 'Z' => isOdd
               ? _oddCharacterMap[upper - '0']
               : _evenCharacterMap[upper - '0'],
            _ => -1,
         };

         if (num == -1)
         {
            throw new InvalidOperationException("invalid character");
         }

         sum += num;
         isOdd = !isOdd;
      }

      var remainder = sum % 26;
      var checkCharacter = (Char)('A' + remainder);

      return $"{value}{checkCharacter}";
   }

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.ItCodiceFiscaleInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(ItCodiceFiscale.IndividualLength, Messages.ItCodiceFiscaleLength),
         ]);

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.ItCodiceFiscaleInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.ItCodiceFiscaleInvalidCheckCharacter,
         ItCodiceFiscale.CheckDigitAlgorithmName);

   private static InvalidGivenName GetInvalidGivenNameResult(String value)
      => new(Messages.ItCodiceFiscaleInvalidGivenName, value[3..6]);

   private static InvalidMonth GetInvalidMonthResult(String value)
      => new(Messages.ItCodiceFiscaleInvalidMonth, value[8..9]);

   private static InvalidSurname GetInvalidSurnameResult(String value)
      => new(Messages.ItCodiceFiscaleInvalidSurname, value[..3]);

   private static InvalidYear GetInvalidYearResult(String value)
      => new(Messages.ItCodiceFiscaleInvalidYear, value[6..8]);

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   //[Fact]
   //public void CheckDigitTest()
   //{
   //   // Arrange.
   //   var value = "RSSNTNUSHLUGNNS";
   //   var expected = "RSSNTNUSHLUGNNSZ";

   //   // Act.
   //   var result = GetValueWithValidCheckDigit(value);

   //   // Assert.
   //   result.Should().Be(expected);
   //}

   [Theory]
   [MemberData(nameof(ValidCodiceFiscaleValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonAlphanumericCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckCharacterValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnInvalidChecksum_WhenValueHasInvalidCheckCharacter(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidNameValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnInvalidSurname_WhenSurnameHasDigitCharacter(String surname)
   {
      // Arrange.
      var value = GetValue(surname);
      LocalValidationResult expected = GetInvalidSurnameResult(value);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidNameValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnInvalidGivenName_WhenGivenNameHasDigitCharacter(String givenName)
   {
      // Arrange.
      var value = GetValue(givenName: givenName);
      LocalValidationResult expected = GetInvalidGivenNameResult(value);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidYearValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnInvalidYear_WhenYearHasNonOmocodiaSubstitution(String year)
   {
      // Arrange.
      var value = GetValue(year: year);
      LocalValidationResult expected = GetInvalidYearResult(value);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidMonthValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnInvalidMonth_WhenMonthHasInvalidCharacter(Char month)
   {
      // Arrange.
      var value = GetValue(month: month);
      LocalValidationResult expected = GetInvalidMonthResult(value);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion
}
