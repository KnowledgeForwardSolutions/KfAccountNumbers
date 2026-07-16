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
      // Invalid omocodia substitution
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
      // Letter out of range for month indicator
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

      // Month indicator is not digit
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

   public static TheoryData<String, Char, String> InvalidDayValues = new()
   {
      // Need to include year and month info to test for day > than is valid for month.
      { "00", 'A', "00" },       // Invalid day = 0
      { "00", 'A', "LL" },       // Invalid day = 0
      { "00", 'A', "ll" },       // Invalid day = 0
      { "00", 'A', "0L" },       // Invalid day = 0
      { "00", 'A', "l0" },       // Invalid day = 0
      { "87", 'C', "60" },       // Invalid day = 60
      { "87", 'C', "99" },       // Invalid day = 99
      { "87", 'C', "SL" },       // Invalid day = 60
      { "87", 'C', "VV" },       // Invalid day = 99
      { "87", 'C', "S0" },       // Invalid day = 60
      { "87", 'C', "9V" },       // Invalid day = 99

      // Digit day
      { "04", 'A', "32" },       // Invalid day of month for January, any year, out of bounds for gender = male
      { "01", 'B', "29" },       // Invalid day of for February, non-leap year
      { "04", 'B', "30" },       // Invalid day of for February, leap year
      { "00", 'B', "30" },       // Invalid day of for February, leap year (2000 is leap-year)
      { "04", 'C', "32" },       // Invalid day of for March, any year
      { "04", 'D', "31" },       // Invalid day of for April, any year
      { "04", 'E', "32" },       // Invalid day of for May, any year
      { "04", 'H', "91" },       // Invalid day of for June, any year, out of bounds for gender = female
      { "04", 'L', "92" },       // Invalid day of for July, any year
      { "04", 'M', "92" },       // Invalid day of for August, any year
      { "04", 'P', "91" },       // Invalid day of for September, any year
      { "04", 'R', "92" },       // Invalid day of for October, any year
      { "04", 'S', "91" },       // Invalid day of for November, any year
      { "04", 'T', "92" },       // Invalid day of for December, any year

      // Omocodia day
      { "04", 'A', "PN" },       // Invalid day of month for January, any year, out of bounds for gender = male
      { "01", 'B', "NV" },       // Invalid day of for February, non-leap year
      { "04", 'B', "Pl" },       // Invalid day of for February, leap year
      { "00", 'B', "Pl" },       // Invalid day of for February, leap year (2000 is leap-year)
      { "04", 'C', "pN" },       // Invalid day of for March, any year
      { "04", 'D', "pM" },       // Invalid day of for April, any year
      { "04", 'E', "pN" },       // Invalid day of for May, any year
      { "04", 'H', "VM" },       // Invalid day of for June, any year, out of bounds for gender = female
      { "04", 'L', "Vn" },       // Invalid day of for July, any year
      { "04", 'M', "Vn" },       // Invalid day of for August, any year
      { "04", 'P', "VM" },       // Invalid day of for September, any year
      { "04", 'R', "vN" },       // Invalid day of for October, any year
      { "04", 'S', "vM" },       // Invalid day of for November, any year
      { "04", 'T', "vN" },       // Invalid day of for December, any year

      // Mixed digit and omocodia day
      { "04", 'A', "3N" },       // Invalid day of month for January, any year, out of bounds for gender = male
      { "01", 'B', "2V" },       // Invalid day of for February, non-leap year
      { "04", 'B', "3L" },       // Invalid day of for February, leap year
      { "00", 'B', "3L" },       // Invalid day of for February, leap year (2000 is leap-year)
      { "04", 'C', "3n" },       // Invalid day of for March, any year
      { "04", 'D', "3m" },       // Invalid day of for April, any year
      { "04", 'E', "3n" },       // Invalid day of for May, any year
      { "04", 'H', "P1" },       // Invalid day of for June, any year, out of bounds for gender = female
      { "04", 'L', "P2" },       // Invalid day of for July, any year
      { "04", 'M', "P2" },       // Invalid day of for August, any year
      { "04", 'P', "P1" },       // Invalid day of for September, any year
      { "04", 'R', "v2" },       // Invalid day of for October, any year
      { "04", 'S', "v1" },       // Invalid day of for November, any year
      { "04", 'T', "v2" },       // Invalid day of for December, any year
   };

   public static TheoryData<String> InvalidTownOfBirthValues =>
   [
      // Leading digit instead of alpha
      "0001",
      "1001",
      "2001",
      "3001",
      "4001",
      "5001",
      "6001",
      "7001",
      "8001",
      "9001",

      // Invalid omocodia substitution
      "AA11",
      "AB21",
      "AC31",
      "A1D4",
      "A1E5",
      "A1F6",
      "A17G",
      "A18H",
      "AI11",
      "AJ11",
      "AK11",
      "A1O1",
      "A1W1",
      "A1X1",
      "A11Y",
      "A11Z",
      "AAAA",
      "Aa11",
      "Ab21",
      "Ac31",
      "A1d1",
      "A1e1",
      "A1f1",
      "A11g",
      "A11h",
      "Ai11",
      "Aj11",
      "Ak11",
      "A1o1",
      "A1w1",
      "A1x1",
      "A11y",
      "A11z",
      "AaaA",
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

   private static InvalidDay GetInvalidDayResult(String value)
      => new(Messages.ItCodiceFiscaleInvalidDay, value[9..11]);

   private static InvalidGivenName GetInvalidGivenNameResult(String value)
      => new(Messages.ItCodiceFiscaleInvalidGivenName, value[3..6]);

   private static InvalidLocationCode GetInvalidLocationCodeResult(String value)
      => new(Messages.ItCodiceFiscaleInvalidTownOfBirth, value[11..15]);

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

   [Theory]
   [MemberData(nameof(InvalidDayValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnInvalidDay_WhenDayIsInvalid(
      String year,
      Char month,
      String day)
   {
      // Arrange.
      var value = GetValue(year: year, month: month, day: day);
      LocalValidationResult expected = GetInvalidDayResult(value);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidTownOfBirthValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnInvalidLocationCode_WhenMonthHasInvalidTownOfBirth(String townOfBirth)
   {
      // Arrange.
      var value = GetValue(townOfBirth: townOfBirth);
      LocalValidationResult expected = GetInvalidLocationCodeResult(value);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion
}
