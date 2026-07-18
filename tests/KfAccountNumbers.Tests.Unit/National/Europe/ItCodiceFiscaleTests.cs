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

   public static TheoryData<String> ValidYearValues =>
   [
      "00",
      "99",

      // Valid omocodia substitutions
      "L0",
      "M1",
      "N2",
      "P3",
      "Q4",
      "R5",
      "S6",
      "T7",
      "U8",
      "V9",
      "0l",
      "1m",
      "2n",
      "3p",
      "4q",
      "5r",
      "6s",
      "7t",
      "8u",
      "9v",
   ];

   public static TheoryData<Char> ValidMonthValues =>
   [
      'A',
      'B',
      'C',
      'D',
      'E',
      'H',
      'L',
      'M',
      'P',
      'R',
      'S',
      'T',
      'a',
      'b',
      'c',
      'd',
      'e',
      'h',
      'l',
      'm',
      'p',
      'r',
      's',
      't',
   ];

   public static TheoryData<String, Char, String> ValidDayValues = new()
   {
      // Valid omocodia substitutions
      { "00", 'A', "L1" },
      { "00", 'A', "M1" },
      { "00", 'A', "N1" },
      { "00", 'A', "P1" },
      { "00", 'A', "1Q" },
      { "00", 'A', "1R" },
      { "00", 'A', "1S" },
      { "00", 'A', "1T" },
      { "00", 'A', "1U" },
      { "00", 'A', "1V" },
      { "00", 'A', "l1" },
      { "00", 'A', "m1" },
      { "00", 'A', "n1" },
      { "00", 'A', "1p" },
      { "00", 'A', "1q" },
      { "00", 'A', "1r" },
      { "00", 'A', "1s" },
      { "00", 'A', "1t" },
      { "00", 'A', "1u" },
      { "00", 'A', "1v" },

      // Digit day
      { "04", 'A', "31" },       // Valid day of month for January, any year, within of bounds for gender = male
      { "01", 'B', "28" },       // Valid day of for February, non-leap year
      { "04", 'B', "29" },       // Valid day of for February, leap year
      { "00", 'B', "29" },       // Valid day of for February, leap year (2000 is leap-year)
      { "04", 'C', "31" },       // Valid day of for March, any year
      { "04", 'D', "30" },       // Valid day of for April, any year
      { "04", 'E', "31" },       // Valid day of for May, any year
      { "04", 'H', "90" },       // Valid day of for June, any year, within of bounds for gender = female
      { "04", 'L', "91" },       // Valid day of for July, any year
      { "04", 'M', "91" },       // Valid day of for August, any year
      { "04", 'P', "90" },       // Valid day of for September, any year
      { "04", 'R', "91" },       // Valid day of for October, any year
      { "04", 'S', "90" },       // Valid day of for November, any year
      { "04", 'T', "91" },       // Valid day of for December, any year

      // Omocodia day
      { "04", 'A', "PM" },       // Valid day of month for January, any year, within of bounds for gender = male
      { "01", 'B', "NU" },       // Valid day of for February, non-leap year
      { "04", 'B', "NR" },       // Valid day of for February, leap year
      { "00", 'B', "NR" },       // Valid day of for February, leap year (2000 is leap-year)
      { "04", 'C', "pM" },       // Valid day of for March, any year
      { "04", 'D', "pL" },       // Valid day of for April, any year
      { "04", 'E', "pM" },       // Valid day of for May, any year
      { "04", 'H', "VL" },       // Valid day of for June, any year, within of bounds for gender = female
      { "04", 'L', "Vm" },       // Valid day of for July, any year
      { "04", 'M', "Vm" },       // Valid day of for August, any year
      { "04", 'P', "Vl" },       // Valid day of for September, any year
      { "04", 'R', "vM" },       // Valid day of for October, any year
      { "04", 'S', "vL" },       // Valid day of for November, any year
      { "04", 'T', "vM" },       // Valid day of for December, any year

      // Mixed digit and omocodia day
      { "04", 'A', "3M" },       // Valid day of month for January, any year, within of bounds for gender = male
      { "01", 'B', "2U" },       // Valid day of for February, non-leap year
      { "04", 'B', "2R" },       // Valid day of for February, leap year
      { "00", 'B', "2R" },       // Valid day of for February, leap year (2000 is leap-year)
      { "04", 'C', "3m" },       // Valid day of for March, any year
      { "04", 'D', "3l" },       // Valid day of for April, any year
      { "04", 'E', "3m" },       // Valid day of for May, any year
      { "04", 'H', "P0" },       // Valid day of for June, any year, within of bounds for gender = female
      { "04", 'L', "P1" },       // Valid day of for July, any year
      { "04", 'M', "P1" },       // Valid day of for August, any year
      { "04", 'P', "P0" },       // Valid day of for September, any year
      { "04", 'R', "v1" },       // Valid day of for October, any year
      { "04", 'S', "v0" },       // Valid day of for November, any year
      { "04", 'T', "v1" },       // Valid day of for December, any year
   };

   public static TheoryData<String> ValidComuneValues =>
   [
      "A001",
      "Z404",

      // Valid omocodia substitutions
      "AL01",
      "AM11",
      "AN21",
      "AP31",
      "AQ41",
      "AR51",
      "AS61",
      "A1T7",
      "A1U8",
      "A1V9",
      "A10l",
      "A11m",
      "A12n",
      "A13p",
      "A14q",
      "A51r",
      "A61s",
      "A71t",
      "A81u",
      "A91v",

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

      // Invalid omocodia substitution
      { "00", 'A', "A1" },
      { "00", 'A', "B2" },
      { "00", 'A', "C3" },
      { "00", 'A', "D4" },
      { "00", 'A', "E5" },
      { "00", 'A', "F6" },
      { "00", 'A', "G7" },
      { "00", 'A', "H8" },
      { "00", 'A', "1I" },
      { "00", 'A', "1J" },
      { "00", 'A', "1K" },
      { "00", 'A', "1O" },
      { "00", 'A', "1W" },
      { "00", 'A', "1X" },
      { "00", 'A', "1Y" },
      { "00", 'A', "1Z" },
      { "00", 'A', "AA" },
      { "00", 'A', "a1" },
      { "00", 'A', "b2" },
      { "00", 'A', "c3" },
      { "00", 'A', "d4" },
      { "00", 'A', "e5" },
      { "00", 'A', "f6" },
      { "00", 'A', "g7" },
      { "00", 'A', "h8" },
      { "00", 'A', "1i" },
      { "00", 'A', "1j" },
      { "00", 'A', "1k" },
      { "00", 'A', "1o" },
      { "00", 'A', "1w" },
      { "00", 'A', "1x" },
      { "00", 'A', "1y" },
      { "00", 'A', "1z" },
      { "00", 'A', "aa" },

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

   public static TheoryData<String> InvalidComuneValues =>
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

   private static Int32 MapEvenCharacter(Char ch)
      => ch switch
      {
         var d when d is >= Chars.DigitZero and <= Chars.DigitNine => d - Chars.DigitZero,
         var c when c is >= Chars.UpperCaseA and <= Chars.UpperCaseZ => c - Chars.UpperCaseA,
         _ => -1,
      };

   private static Int32 MapOddCharacter(Char ch)
      => ch switch
      {
         // Map from https://en.wikipedia.org/wiki/Italian_fiscal_code
         '0' => 1,
         '1' => 0,
         '2' => 5,
         '3' => 7,
         '4' => 9,
         '5' => 13,
         '6' => 15,
         '7' => 17,
         '8' => 19,
         '9' => 21,
         'A' => 1,
         'B' => 0,
         'C' => 5,
         'D' => 7,
         'E' => 9,
         'F' => 13,
         'G' => 15,
         'H' => 17,
         'I' => 19,
         'J' => 21,
         'K' => 2,
         'L' => 4,
         'M' => 18,
         'N' => 20,
         'O' => 11,
         'P' => 3,
         'Q' => 6,
         'R' => 8,
         'S' => 12,
         'T' => 14,
         'U' => 16,
         'V' => 10,
         'W' => 22,
         'X' => 25,
         'Y' => 24,
         'Z' => 23,
         _ => -1,
      };


   private static readonly Int32[] _evenCharacterMap = [.. Enumerable.Range('0', 'Z' - '0' + 1).Select(ch => MapEvenCharacter((Char)ch))];

   private static readonly Int32[] _oddCharacterMap = [.. Enumerable.Range('0', 'Z' - '0' + 1).Select(ch => MapOddCharacter((Char)ch))];

   private static String GetValue(
      String surname = "ABC",
      String givenName = "DEF",
      String year = "83",
      Char month = 'M',
      String day = "26",
      String comune = "A123")
   {
      var temp = $"{surname}{givenName}{year}{month}{day}{comune}";

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
      => new(Messages.ItCodiceFiscaleInvalidComune, value[11..15]);

   private static InvalidMonth GetInvalidMonthResult(String value)
      => new(Messages.ItCodiceFiscaleInvalidMonth, value[8..9]);

   private static InvalidSurname GetInvalidSurnameResult(String value)
      => new(Messages.ItCodiceFiscaleInvalidSurname, value[..3]);

   private static InvalidYear GetInvalidYearResult(String value)
      => new(Messages.ItCodiceFiscaleInvalidYear, value[6..8]);

   #region Check Digit Algorithm Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]   // e e e e e e e e
   [InlineData("BABABA10B01A101A")]    // BABABA10B01A evaluates to zero, so only the three digits of the comune have any effect on the check digit calculation
   [InlineData("BABABA10B01A111B")]    // Remainder = 1
   [InlineData("BABABA10B01A121C")]    // Remainder = 2
   [InlineData("BABABA10B01A131D")]    // Remainder = 3
   [InlineData("BABABA10B01A141E")]    // Remainder = 4
   [InlineData("BABABA10B01A151F")]    // Remainder = 5
   [InlineData("BABABA10B01A161G")]    // Remainder = 6
   [InlineData("BABABA10B01A171H")]    // Remainder = 7
   [InlineData("BABABA10B01A181I")]    // Remainder = 8
   [InlineData("BABABA10B01A191J")]    // Remainder = 9
   [InlineData("BABABA10B01A202K")]    // Remainder = 10
   [InlineData("BABABA10B01A212L")]    // Remainder = 11
   [InlineData("BABABA10B01A222M")]    // Remainder = 12
   [InlineData("BABABA10B01A232N")]    // Remainder = 13
   [InlineData("BABABA10B01A242O")]    // Remainder = 14
   [InlineData("BABABA10B01A252P")]    // Remainder = 15
   [InlineData("BABABA10B01A262Q")]    // Remainder = 16
   [InlineData("BABABA10B01A272R")]    // Remainder = 17
   [InlineData("BABABA10B01A282S")]    // Remainder = 18
   [InlineData("BABABA10B01A292T")]    // Remainder = 19
   [InlineData("BABABA10B01A118U")]    // Remainder = 20
   [InlineData("BABABA10B01A128V")]    // Remainder = 21
   [InlineData("BABABA10B01A138W")]    // Remainder = 22
   [InlineData("BABABA10B01A148X")]    // Remainder = 23
   [InlineData("BABABA10B01A158Y")]    // Remainder = 24
   [InlineData("BABABA10B01A168Z")]    // Remainder = 25
   [InlineData("bababa10b01a101a")]    // bababa10b01a evaluates to zero, so only the three digits of the comune have any effect on the check digit calculation
   [InlineData("bababa10b01a111b")]    // Remainder = 1
   [InlineData("bababa10b01a121c")]    // Remainder = 2
   [InlineData("bababa10b01a131d")]    // Remainder = 3
   [InlineData("bababa10b01a141e")]    // Remainder = 4
   [InlineData("bababa10b01a151f")]    // Remainder = 5
   [InlineData("bababa10b01a161g")]    // Remainder = 6
   [InlineData("bababa10b01a171h")]    // Remainder = 7
   [InlineData("bababa10b01a181i")]    // Remainder = 8
   [InlineData("bababa10b01a191j")]    // Remainder = 9
   [InlineData("bababa10b01a202k")]    // Remainder = 10
   [InlineData("bababa10b01a212l")]    // Remainder = 11
   [InlineData("bababa10b01a222m")]    // Remainder = 12
   [InlineData("bababa10b01a232n")]    // Remainder = 13
   [InlineData("bababa10b01a242o")]    // Remainder = 14
   [InlineData("bababa10b01a252p")]    // Remainder = 15
   [InlineData("bababa10b01a262q")]    // Remainder = 16
   [InlineData("bababa10b01a272r")]    // Remainder = 17
   [InlineData("bababa10b01a282s")]    // Remainder = 18
   [InlineData("bababa10b01a292t")]    // Remainder = 19
   [InlineData("bababa10b01a118u")]    // Remainder = 20
   [InlineData("bababa10b01a128v")]    // Remainder = 21
   [InlineData("bababa10b01a138w")]    // Remainder = 22
   [InlineData("bababa10b01a148x")]    // Remainder = 23
   [InlineData("bababa10b01a158y")]    // Remainder = 24
   [InlineData("bababa10b01a168z")]    // Remainder = 25
   public void ItCodiceFiscale_CheckDigitAlgorithm_ShouldValidateAllPossibleCheckDigits(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidCodiceFiscaleValues))]
   public void ItCodiceFiscale_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = value.ToUpperInvariant();

      // Act.
      var sut = new ItCodiceFiscale(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidYearValues))]
   public void ItCodiceFiscale_Constructor_ShouldCreateInstance_WhenYearIsValid(String year)
   {
      // Arrange.
      var value = GetValue(year: year);
      var expected = value.ToUpperInvariant();

      // Act.
      var sut = new ItCodiceFiscale(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidMonthValues))]
   public void ItCodiceFiscale_Constructor_ShouldCreateInstance_WhenMonthIsValid(Char month)
   {
      // Arrange.
      var value = GetValue(month: month);
      var expected = value.ToUpperInvariant();

      // Act.
      var sut = new ItCodiceFiscale(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDayValues))]
   public void ItCodiceFiscale_Constructor_ShouldCreateInstance_WhenDayIsValid(
      String year,
      Char month,
      String day)
   {
      // Arrange.
      var value = GetValue(year: year, month: month, day: day);
      var expected = value.ToUpperInvariant();

      // Act.
      var sut = new ItCodiceFiscale(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidComuneValues))]
   public void ItCodiceFiscale_Constructor_ShouldCreateInstance_WhenComuneIsValid(String comune)
   {
      // Arrange.
      var value = GetValue(comune: comune);
      var expected = value.ToUpperInvariant();

      // Act.
      var sut = new ItCodiceFiscale(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void ItCodiceFiscale_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new ItCodiceFiscale(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void ItCodiceFiscale_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new ItCodiceFiscale(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void ItCodiceFiscale_Constructor_ShouldThrowKfValidationException_WhenValueHasNonAlphanumericCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new ItCodiceFiscale(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckCharacterValues))]
   public void ItCodiceFiscale_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckCharacter(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new ItCodiceFiscale(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidNameValues))]
   public void ItCodiceFiscale_Constructor_ShouldThrowKfValidationException_WhenSurnameHasDigitCharacter(String surname)
   {
      // Arrange.
      var value = GetValue(surname);
      LocalValidationError expected = GetInvalidSurnameResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new ItCodiceFiscale(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidNameValues))]
   public void ItCodiceFiscale_Constructor_ShouldThrowKfValidationException_WhenGivenNameHasDigitCharacter(String givenName)
   {
      // Arrange.
      var value = GetValue(givenName: givenName);
      LocalValidationError expected = GetInvalidGivenNameResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new ItCodiceFiscale(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidYearValues))]
   public void ItCodiceFiscale_Constructor_ShouldThrowKfValidationException_WhenYearHasNonOmocodiaSubstitution(String year)
   {
      // Arrange.
      var value = GetValue(year: year);
      LocalValidationError expected = GetInvalidYearResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new ItCodiceFiscale(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidMonthValues))]
   public void ItCodiceFiscale_Constructor_ShouldThrowKfValidationException_WhenMonthHasInvalidCharacter(Char month)
   {
      // Arrange.
      var value = GetValue(month: month);
      LocalValidationError expected = GetInvalidMonthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new ItCodiceFiscale(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDayValues))]
   public void ItCodiceFiscale_Constructor_ShouldThrowKfValidationException_WhenDayIsInvalid(
      String year,
      Char month,
      String day)
   {
      // Arrange.
      var value = GetValue(year: year, month: month, day: day);
      LocalValidationError expected = GetInvalidDayResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new ItCodiceFiscale(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidComuneValues))]
   public void ItCodiceFiscale_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidComune(String comune)
   {
      // Arrange.
      var value = GetValue(comune: comune);
      LocalValidationError expected = GetInvalidLocationCodeResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new ItCodiceFiscale(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region BelfioreCode Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidComuneValues))]
   public void ItCodiceFiscale_BelfioreCode_ShouldReturnExpectedValue(String comune)
   {
      // Arrange.
      var value = GetValue(comune: comune);
      var sut = new ItCodiceFiscale(value);
      var expected = sut.Value[11..15];

      // Act/assert.
      sut.BelfioreCode.Should().Be(expected);
   }

   #endregion

   #region Gender Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("01")]
   [InlineData("31")]
   [InlineData("LM")]
   [InlineData("PM")]
   [InlineData("lm")]
   [InlineData("pm")]
   public void ItCodiceFiscale_Gender_ShouldReturnMale_WhenDayIsLessThan32(String day)
   {
      // Arrange.
      var value = GetValue(day: day);
      var sut = new ItCodiceFiscale(value);
      Gender.BinaryGender expected = default(Gender.Male);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [InlineData("61")]
   [InlineData("91")]
   [InlineData("SM")]
   [InlineData("VM")]
   [InlineData("sm")]
   [InlineData("vm")]
   public void ItCodiceFiscale_Gender_ShouldReturnFemale_WhenDayIsGreaterThan60(String day)
   {
      // Arrange.
      var value = GetValue(day: day);
      var sut = new ItCodiceFiscale(value);
      Gender.BinaryGender expected = default(Gender.Female);

      // Act/assert.
      sut.Gender.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidCodiceFiscaleValues))]
   public void ItCodiceFiscale_Value_ShouldReturnValidatedInseeNumber(String value)
   {
      // Arrange.
      var sut = new ItCodiceFiscale(value);
      var expected = value.ToUpperInvariant();

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void ItCodiceFiscale_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidUpperCaseCodiceFiscale;
      var sut = new ItCodiceFiscale(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void ItCodiceFiscale_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidUpperCaseCodiceFiscale;
      var sut = new ItCodiceFiscale(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void ItCodiceFiscale_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      ItCodiceFiscale sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void ItCodiceFiscale_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      ItCodiceFiscale sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidCodiceFiscaleValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new ItCodiceFiscale(value);

      // Act.
      var sut = (ItCodiceFiscale)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidYearValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldCreateInstance_WhenYearIsValid(String year)
   {
      // Arrange.
      var value = GetValue(year: year);
      var expected = new ItCodiceFiscale(value);

      // Act.
      var sut = (ItCodiceFiscale)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidMonthValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldCreateInstance_WhenMonthIsValid(Char month)
   {
      // Arrange.
      var value = GetValue(month: month);
      var expected = new ItCodiceFiscale(value);

      // Act.
      var sut = (ItCodiceFiscale)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDayValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldCreateInstance_WhenDayIsValid(
      String year,
      Char month,
      String day)
   {
      // Arrange.
      var value = GetValue(year: year, month: month, day: day);
      var expected = new ItCodiceFiscale(value);

      // Act.
      var sut = (ItCodiceFiscale)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidComuneValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldCreateInstance_WhenComuneIsValid(String comune)
   {
      // Arrange.
      var value = GetValue(comune: comune);
      var expected = new ItCodiceFiscale(value);

      // Act.
      var sut = (ItCodiceFiscale)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => (ItCodiceFiscale)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (ItCodiceFiscale)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldThrowKfValidationException_WhenValueHasNonAlphanumericCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => (ItCodiceFiscale)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckCharacterValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldThrowKfValidationException_WhenValueHasInvalidCheckCharacter(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => (ItCodiceFiscale)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidNameValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldThrowKfValidationException_WhenSurnameHasDigitCharacter(String surname)
   {
      // Arrange.
      var value = GetValue(surname);
      LocalValidationError expected = GetInvalidSurnameResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (ItCodiceFiscale)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidNameValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldThrowKfValidationException_WhenGivenNameHasDigitCharacter(String givenName)
   {
      // Arrange.
      var value = GetValue(givenName: givenName);
      LocalValidationError expected = GetInvalidGivenNameResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (ItCodiceFiscale)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidYearValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldThrowKfValidationException_WhenYearHasNonOmocodiaSubstitution(String year)
   {
      // Arrange.
      var value = GetValue(year: year);
      LocalValidationError expected = GetInvalidYearResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (ItCodiceFiscale)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidMonthValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldThrowKfValidationException_WhenMonthHasInvalidCharacter(Char month)
   {
      // Arrange.
      var value = GetValue(month: month);
      LocalValidationError expected = GetInvalidMonthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (ItCodiceFiscale)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDayValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldThrowKfValidationException_WhenDayIsInvalid(
      String year,
      Char month,
      String day)
   {
      // Arrange.
      var value = GetValue(year: year, month: month, day: day);
      LocalValidationError expected = GetInvalidDayResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (ItCodiceFiscale)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidComuneValues))]
   public void ItCodiceFiscale_ExplicitCastToItCodiceFiscale_ShouldThrowKfValidationException_WhenValueHasInvalidComune(String comune)
   {
      // Arrange.
      var value = GetValue(comune: comune);
      LocalValidationError expected = GetInvalidLocationCodeResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => (ItCodiceFiscale)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void ItCodiceFiscale_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var sut2 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void ItCodiceFiscale_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var sut2 = new ItCodiceFiscale(AltValidUpperCaseCodiceFiscale);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void ItCodiceFiscale_EqualityOperator_ShouldReturnTrue_WhenValuesDifferOnlyByCase()
   {
      // Arrange.
      var sut1 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var sut2 = new ItCodiceFiscale(ValidLowerCaseCodiceFiscale);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void ItCodiceFiscale_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var sut2 = new ItCodiceFiscale(AltValidUpperCaseCodiceFiscale);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void ItCodiceFiscale_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var sut2 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void ItCodiceFiscale_InequalityOperator_ShouldReturnFalse_WhenValuesDifferOnlyByCase()
   {
      // Arrange.
      var sut1 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var sut2 = new ItCodiceFiscale(ValidLowerCaseCodiceFiscale);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidCodiceFiscaleValues))]
   public void ItCodiceFiscale_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new ItCodiceFiscale(value);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidYearValues))]
   public void ItCodiceFiscale_Create_ShouldCreateInstance_WhenYearIsValid(String year)
   {
      // Arrange.
      var value = GetValue(year: year);
      LocalCreateResult expected = new ItCodiceFiscale(value);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidMonthValues))]
   public void ItCodiceFiscale_Create_ShouldCreateInstance_WhenMonthIsValid(Char month)
   {
      // Arrange.
      var value = GetValue(month: month);
      LocalCreateResult expected = new ItCodiceFiscale(value);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDayValues))]
   public void ItCodiceFiscale_Create_ShouldCreateInstance_WhenDayIsValid(
      String year,
      Char month,
      String day)
   {
      // Arrange.
      var value = GetValue(year: year, month: month, day: day);
      LocalCreateResult expected = new ItCodiceFiscale(value);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidComuneValues))]
   public void ItCodiceFiscale_Create_ShouldCreateInstance_WhenComuneIsValid(String comune)
   {
      // Arrange.
      var value = GetValue(comune: comune);
      LocalCreateResult expected = new ItCodiceFiscale(value);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void ItCodiceFiscale_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void ItCodiceFiscale_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void ItCodiceFiscale_Create_ShouldThrowKfValidationException_WhenValueHasNonAlphanumericCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckCharacterValues))]
   public void ItCodiceFiscale_Create_ShouldThrowKfValidationException_WhenValueHasInvalidCheckCharacter(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidNameValues))]
   public void ItCodiceFiscale_Create_ShouldThrowKfValidationException_WhenSurnameHasDigitCharacter(String surname)
   {
      // Arrange.
      var value = GetValue(surname);
      LocalCreateResult expected = (LocalValidationError)GetInvalidSurnameResult(value);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidNameValues))]
   public void ItCodiceFiscale_Create_ShouldThrowKfValidationException_WhenGivenNameHasDigitCharacter(String givenName)
   {
      // Arrange.
      var value = GetValue(givenName: givenName);
      LocalCreateResult expected = (LocalValidationError)GetInvalidGivenNameResult(value);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidYearValues))]
   public void ItCodiceFiscale_Create_ShouldThrowKfValidationException_WhenYearHasNonOmocodiaSubstitution(String year)
   {
      // Arrange.
      var value = GetValue(year: year);
      LocalCreateResult expected = (LocalValidationError)GetInvalidYearResult(value);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidMonthValues))]
   public void ItCodiceFiscale_Create_ShouldThrowKfValidationException_WhenMonthHasInvalidCharacter(Char month)
   {
      // Arrange.
      var value = GetValue(month: month);
      LocalCreateResult expected = (LocalValidationError)GetInvalidMonthResult(value);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDayValues))]
   public void ItCodiceFiscale_Create_ShouldThrowKfValidationException_WhenDayIsInvalid(
      String year,
      Char month,
      String day)
   {
      // Arrange.
      var value = GetValue(year: year, month: month, day: day);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDayResult(value);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidComuneValues))]
   public void ItCodiceFiscale_Create_ShouldThrowKfValidationException_WhenValueHasInvalidComune(String comune)
   {
      // Arrange.
      var value = GetValue(comune: comune);
      LocalCreateResult expected = (LocalValidationError)GetInvalidLocationCodeResult(value);

      // Act.
      var result = ItCodiceFiscale.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void ItCodiceFiscale_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var sut2 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void ItCodiceFiscale_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var sut2 = new ItCodiceFiscale(AltValidUpperCaseCodiceFiscale);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void ItCodiceFiscale_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);

      // Act/assert.
      sut.Equals(ValidUpperCaseCodiceFiscale).Should().BeFalse();
   }

   [Fact]
   public void ItCodiceFiscale_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   [Fact]
   public void ItCodiceFiscale_Equals_ShouldReturnTrue_WhenValuesDifferOnlyByCase()
   {
      // Arrange.
      var sut1 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var sut2 = new ItCodiceFiscale(ValidLowerCaseCodiceFiscale);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void ItCodiceFiscale_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var sut2 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void ItCodiceFiscale_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var sut2 = new ItCodiceFiscale(AltValidUpperCaseCodiceFiscale);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void ItCodiceFiscale_GetHashCode_ShouldBeConsistent_WhenValuesDifferOnlyByCase()
   {
      // Arrange.
      var sut1 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var sut2 = new ItCodiceFiscale(ValidLowerCaseCodiceFiscale);

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

   // ItCodiceFiscale does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void ItCodiceFiscale_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var sut2 = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidCodiceFiscaleValues))]
   public void ItCodiceFiscale_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new ItCodiceFiscale(value);
      var expected = value.ToUpperInvariant();

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidCodiceFiscaleValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnValidValue_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidYearValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnValidValue_WhenYearIsValid(String year)
   {
      // Arrange.
      var value = GetValue(year: year);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidMonthValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnValidValue_WhenMonthIsValid(Char month)
   {
      // Arrange.
      var value = GetValue(month: month);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDayValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnValidValue_WhenDayIsValid(
      String year,
      Char month,
      String day)
   {
      // Arrange.
      var value = GetValue(year: year, month: month, day: day);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidComuneValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnValidValue_WhenComuneIsValid(String comune)
   {
      // Arrange.
      var value = GetValue(comune: comune);
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
   [MemberData(nameof(InvalidComuneValues))]
   public void ItCodiceFiscale_Validate_ShouldReturnInvalidLocationCode_WhenValueHasInvalidComune(String comune)
   {
      // Arrange.
      var value = GetValue(comune: comune);
      LocalValidationResult expected = GetInvalidLocationCodeResult(value);

      // Act.
      var result = ItCodiceFiscale.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void ItCodiceFiscale_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<ItCodiceFiscale>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void ItCodiceFiscale_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public ItCodiceFiscale CodiceFiscale { get; set; } = null!;
   }

   [Fact]
   public void ItCodiceFiscale_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { CodiceFiscale = new ItCodiceFiscale(ValidUpperCaseCodiceFiscale) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void ItCodiceFiscale_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"CodiceFiscale\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void ItCodiceFiscale_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"CodiceFiscale\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.CodiceFiscale.Should().BeNull();
   }

   [Fact]
   public void ItCodiceFiscale_JsonDeserialization_ShouldThrowKfValidationException_WhenValueIsInvalid()
   {
      // Arrange.
      var json = "{\"CodiceFiscale\":\"MRTMST91D08F205J\"}";  // Invalid check digits
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
