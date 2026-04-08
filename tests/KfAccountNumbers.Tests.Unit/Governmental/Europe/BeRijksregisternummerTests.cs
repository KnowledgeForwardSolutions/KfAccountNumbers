// Ignore Spelling: Rijksregisternummer Kf Nummer

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class BeRijksregisternummerTests
{
   private const String ValidRijksregisternummer = "85.07.30-033.28";
   private const String AltValidRijksregisternummer = "17110804680";
   private const String IncompleteDateOfBirthRijksregisternummer = "40 00 00 955-79";
   private const String UnknownDateOfBirthRijksregisternummer = "00 00 01 003-64";
   private const String ValidBisnummer = "17.51.08-046.40";
   private const String AltValidBisnummer = "01430801695";

   private static String GetRawRijksregisternummer(String rijsregisternummer)
   {
      if (rijsregisternummer.Length == 11)
      {
         return rijsregisternummer;
      }

      return rijsregisternummer[0..2] + rijsregisternummer[3..5] + rijsregisternummer[6..8] +
         rijsregisternummer[9..12] + rijsregisternummer[13..];
   }

   private static String GetRijksregisternummerWithValidCheckDigits(
      Int32 year = 1987,
      Int32 month = 7,
      Int32 day = 30,
      Int32 sequenceNumber = 33,
      Boolean formatted = false)
   {
      var temp = $"{(year >= 2000 ? 2 : 0)}{(year % 100):D2}{month:D2}{day:D2}{sequenceNumber:D3}";
      var checkSum = GetCheckSum(temp);
      
      return formatted
         ? $"{(year % 100):D2}{month:D2}{day:D2}{sequenceNumber:D3}{checkSum:D2}"
         : $"{(year % 100):D2}.{month:D2}.{day:D2}-{sequenceNumber:D3}.{checkSum:D2}";
   }

   private static Int32 GetCheckSum(String str)
   {
      var sum = 0L;
      foreach (var ch in str)
      {
         sum *= 10;
         var num = ch - Chars.DigitZero;
         sum += num;
      }

      return (Int32)(97 - (sum % 97));
   }

   public static TheoryData<String> ValidRijksregisternummerValues =>
   [
      ValidRijksregisternummer,
      AltValidRijksregisternummer,
      IncompleteDateOfBirthRijksregisternummer,
      UnknownDateOfBirthRijksregisternummer,
      ValidBisnummer,
      AltValidBisnummer
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "1711080468",           // Length 10
      "171108046801",         // Length 11
      "85.07.30-033.2",       // Length 14
      "85.07.30-033.289",     // Length 16
      new String('1', 100)    // Very long string
   ];

   public static TheoryData<Int32, Int32, Int32, Boolean> ValidDateOfBirthValues = new()
   {
      // false = unformatted
      // rijksregisternummers
      { 1900,  1,  1, false },   // January 1, 1900
      { 1999, 12, 31, false },   // December 31, 1999

      { 2000,  1,  1, false },   // January 1, 2000
      { 2099, 12, 31, false },   // December 31, 2099

      { 1901,  1, 31, false },   // maximum days for January, any year
      { 1991,  2, 28, false },   // maximum days for February, non leap year
      { 1996,  2, 29, false },   // maximum days for February, leap year
      { 2000,  2, 29, false },   // maximum days for February, leap year (2000 is leap-year)
      { 1904,  3, 31, false },   // maximum days for March, any year
      { 1904,  4, 30, false },   // maximum days for April, any year
      { 1904,  5, 31, false },   // maximum days for May, any year
      { 2004,  6, 30, false },   // maximum days for June, any year
      { 2004,  7, 31, false },   // maximum days for July, any year
      { 2004,  8, 31, false },   // maximum days for August, any year
      { 2004,  9, 30, false },   // maximum days for September, any year
      { 2004, 10, 31, false },   // maximum days for October, any year
      { 2004, 11, 30, false },   // maximum days for November, any year
      { 2004, 12, 31, false },   // maximum days for December, any year

      { 1950,  0,  0, false },   // Incomplete date of birth, only year known
      {    0,  0,  1, false },   // Unknown date of birth

      // BIS-nummers
      { 1900, 41,  1, false },   // January 1, 1900
      { 1999, 52, 31, false },   // December 31, 1999

      { 2000, 41,  1, false },   // January 1, 2000
      { 2099, 52, 31, false },   // December 31, 2099

      { 1901, 41, 31, false },   // maximum days for January, any year
      { 1991, 42, 28, false },   // maximum days for February, non leap year
      { 1996, 42, 29, false },   // maximum days for February, leap year
      { 2000, 42, 29, false },   // maximum days for February, leap year (2000 is leap-year)
      { 1904, 43, 31, false },   // maximum days for March, any year
      { 1904, 44, 30, false },   // maximum days for April, any year
      { 1904, 45, 31, false },   // maximum days for May, any year
      { 2004, 46, 30, false },   // maximum days for June, any year
      { 2004, 47, 31, false },   // maximum days for July, any year
      { 2004, 48, 31, false },   // maximum days for August, any year
      { 2004, 49, 30, false },   // maximum days for September, any year
      { 2004, 50, 31, false },   // maximum days for October, any year
      { 2004, 51, 30, false },   // maximum days for November, any year
      { 2004, 52, 31, false },   // maximum days for December, any year

      { 1950, 40,  0, false },   // Incomplete date of birth, only year known
      {    0, 40,  1, false },   // Unknown date of birth

      // BIS-nummers, unknown gender
      { 1900, 21,  1, false },   // January 1, 1900
      { 1999, 32, 31, false },   // December 31, 1999

      { 2000, 21,  1, false },   // January 1, 2000
      { 2099, 32, 31, false },   // December 31, 2099

      { 1901, 21, 31, false },   // maximum days for January, any year
      { 1991, 22, 28, false },   // maximum days for February, non leap year
      { 1996, 22, 29, false },   // maximum days for February, leap year
      { 2000, 22, 29, false },   // maximum days for February, leap year (2000 is leap-year)
      { 1904, 23, 31, false },   // maximum days for March, any year
      { 1904, 24, 30, false },   // maximum days for April, any year
      { 1904, 25, 31, false },   // maximum days for May, any year
      { 2004, 26, 30, false },   // maximum days for June, any year
      { 2004, 27, 31, false },   // maximum days for July, any year
      { 2004, 28, 31, false },   // maximum days for August, any year
      { 2004, 29, 30, false },   // maximum days for September, any year
      { 2004, 30, 31, false },   // maximum days for October, any year
      { 2004, 31, 30, false },   // maximum days for November, any year
      { 2004, 32, 31, false },   // maximum days for December, any year

      { 1950, 20,  0, false },   // Incomplete date of birth, only year known
      {    0, 20,  1, false },   // Unknown date of birth

      // true = formatted
      { 1900,  1,  1, true },    // January 1, 1900
      { 1999, 12, 31, true },    // December 31, 1999
      { 2000,  1,  1, true },    // January 1, 2000
      { 2099, 12, 31, true },    // December 31, 2099

      { 1901,  1, 31, true },    // maximum days for January, any year
      { 1991,  2, 28, true },    // maximum days for February, non leap year
      { 1996,  2, 29, true },    // maximum days for February, leap year
      { 2000,  2, 29, true },    // maximum days for February, leap year (2000 is leap-year)
      { 1904,  3, 31, true },    // maximum days for March, any year
      { 1904,  4, 30, true },    // maximum days for April, any year
      { 1904,  5, 31, true },    // maximum days for May, any year
      { 2004,  6, 30, true },    // maximum days for June, any year
      { 2004,  7, 31, true },    // maximum days for July, any year
      { 2004,  8, 31, true },    // maximum days for August, any year
      { 2004,  9, 30, true },    // maximum days for September, any year
      { 2004, 10, 31, true },    // maximum days for October, any year
      { 2004, 11, 30, true },    // maximum days for November, any year
      { 2004, 12, 31, true },    // maximum days for December, any year

      { 1950,  0,  0, true },    // Incomplete date of birth, only year known
      {    0,  0,  1, true },    // Unknown date of birth

      // BIS-nummers
      { 1900, 41,  1, true },    // January 1, 1900
      { 1999, 52, 31, true },    // December 31, 1999

      { 2000, 41,  1, true },    // January 1, 2000
      { 2099, 52, 31, true },    // December 31, 2099

      { 1901, 41, 31, true },    // maximum days for January, any year
      { 1991, 42, 28, true },    // maximum days for February, non leap year
      { 1996, 42, 29, true },    // maximum days for February, leap year
      { 2000, 42, 29, true },    // maximum days for February, leap year (2000 is leap-year)
      { 1904, 43, 31, true },    // maximum days for March, any year
      { 1904, 44, 30, true },    // maximum days for April, any year
      { 1904, 45, 31, true },    // maximum days for May, any year
      { 2004, 46, 30, true },    // maximum days for June, any year
      { 2004, 47, 31, true },    // maximum days for July, any year
      { 2004, 48, 31, true },    // maximum days for August, any year
      { 2004, 49, 30, true },    // maximum days for September, any year
      { 2004, 50, 31, true },    // maximum days for October, any year
      { 2004, 51, 30, true },    // maximum days for November, any year
      { 2004, 52, 31, true },    // maximum days for December, any year

      { 1950, 40,  0, true },    // Incomplete date of birth, only year known
      {    0, 40,  1, true },    // Unknown date of birth

      // BIS-nummers, unknown gender
      { 1900, 21,  1, true },    // January 1, 1900
      { 1999, 32, 31, true },    // December 31, 1999

      { 2000, 21,  1, true },    // January 1, 2000
      { 2099, 32, 31, true },    // December 31, 2099

      { 1901, 21, 31, true },    // maximum days for January, any year
      { 1991, 22, 28, true },    // maximum days for February, non leap year
      { 1996, 22, 29, true },    // maximum days for February, leap year
      { 2000, 22, 29, true },    // maximum days for February, leap year (2000 is leap-year)
      { 1904, 23, 31, true },    // maximum days for March, any year
      { 1904, 24, 30, true },    // maximum days for April, any year
      { 1904, 25, 31, true },    // maximum days for May, any year
      { 2004, 26, 30, true },    // maximum days for June, any year
      { 2004, 27, 31, true },    // maximum days for July, any year
      { 2004, 28, 31, true },    // maximum days for August, any year
      { 2004, 29, 30, true },    // maximum days for September, any year
      { 2004, 30, 31, true },    // maximum days for October, any year
      { 2004, 31, 30, true },    // maximum days for November, any year
      { 2004, 32, 31, true },    // maximum days for December, any year

      { 1950, 20,  0, true },    // Incomplete date of birth, only year known
      {    0, 20,  1, true },    // Unknown date of birth
   };

   public static TheoryData<String> InvalidCharacterValues =>
   [
      "A7110804680",             // Non-digit character 'A'
      "1 110804680",             // Non-digit character ' '
      "17_10804680",             // Non-digit character '-'
      "171=0804680",             // Non-digit character '='
      "1711B804680",             // Non-digit character 'B'
      "17110C04680",             // Non-digit character 'C'
      "171108a4680",             // Non-digit character 'a'
      "1711080b680",             // Non-digit character 'b'
      "17110804~80",             // Non-digit character '~'
      "171108046\u21530",        // Non-digit character Unicode fraction 1/3
      "1711080468\u00D6",        // Invalid character unicode O with umlaut

      "A7.11.08-046.80",         // Non-digit character 'A'
      "1 .11.08-046.80",         // Non-digit character ' '
      "17._1.08-046.80",         // Non-digit character '-'
      "17.1=.08-046.80",         // Non-digit character '='
      "17.11.B8-046.80",         // Non-digit character 'B'
      "17.11.0C-046.80",         // Non-digit character 'C'
      "17 11 08 a46 80",         // Non-digit character 'a'
      "17 11 08 0b6 80",         // Non-digit character 'b'
      "17 11 08 04~ 80",         // Non-digit character '~'
      "17 11 08 046 \u21530",    // Non-digit character Unicode fraction 1/3
      "17 11 08 046 8\u00D6",    // Invalid character unicode O with umlaut
   ];

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "85072003328",             // 85073003328 with single digit transcription error, 3 -> 2
      "17110805680",             // 17110804680 with single digit transcription error, 4 -> 5
      "85072003329",             // 85072003328 with check digit transcription error, 8 -> 9
      "17118005680",             // 17110805680 with two digit transposition error, 08 -> 80
      "85037003328",             // 85073003328 with two digit transposition error, 73 -> 37
      "17110408680",             // 17110804680 with two digit jump transposition, 804 -> 408
      "85073004428",             // 85073003328 with two digit twin error, 33 -> 44
      "17220804680",             // 17110804680 with two digit twin error, 11 -> 22

      "85.07.20-033.28",         // 85073003328 with single digit transcription error, 3 -> 2
      "17.11.08-056.80",         // 17110804680 with single digit transcription error, 4 -> 5
      "85.07.20-033.29",         // 85072003328 with check digit transcription error, 8 -> 9
      "17.11.80-056.80",         // 17110805680 with two digit transposition error, 08 -> 80
      "85 03 70 033 28",         // 85073003328 with two digit transposition error, 73 -> 37
      "17 11 04 086 80",         // 17110804680 with two digit jump transposition, 804 -> 408
      "85 07 30 044 28",         // 85073003328 with two digit twin error, 33 -> 44
      "17 22 08 046 80",         // 17110804680 with two digit twin error, 11 -> 22
   ];

   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "85007.30-033.28",
      "85107.30-033.28",
      "85207.30-033.28",
      "85307.30-033.28",
      "85407.30-033.28",
      "85507.30-033.28",
      "85607.30-033.28",
      "85707.30-033.28",
      "85807.30-033.28",
      "85907.30-033.28",

      "85.07030-033.28",
      "85.07130-033.28",
      "85.07230-033.28",
      "85.07330-033.28",
      "85.07430-033.28",
      "85.07530-033.28",
      "85.07630-033.28",
      "85.07730-033.28",
      "85.07830-033.28",
      "85.07930-033.28",

      "85.07.300033.28",
      "85.07.301033.28",
      "85.07.302033.28",
      "85.07.303033.28",
      "85.07.304033.28",
      "85.07.305033.28",
      "85.07.306033.28",
      "85.07.307033.28",
      "85.07.308033.28",
      "85.07.309033.28",

      "85.07.30-033028",
      "85.07.30-033128",
      "85.07.30-033228",
      "85.07.30-033328",
      "85.07.30-033428",
      "85.07.30-033528",
      "85.07.30-033628",
      "85.07.30-033728",
      "85.07.30-033828",
      "85.07.30-033928",
   ];

   public static TheoryData<Int32, Int32, Int32, Boolean> InvalidDateOfBirthValues = new()
   {
      // rijksregisternummers
      { 1904, 13, 31, false },      // month = 13
      { 1904,  1, 32, false },      // Invalid day of month for January, any year
      { 1901,  2, 29, false },      // Invalid day of for February, non-leap year
      { 1904,  2, 30, false },      // Invalid day of for February, leap year
      { 1904,  2, 30, false },      // Invalid day of for February, leap year (2000 is leap-year)
      { 1904,  3, 32, false },      // Invalid day of for March, any year
      { 1904,  4, 31, false },      // Invalid day of for April, any year
      { 1904,  5, 32, false },      // Invalid day of for May, any year
      { 2004,  6, 31, false },      // Invalid day of for June, any year
      { 2004,  7, 32, false },      // Invalid day of for July, any year
      { 2004,  8, 32, false },      // Invalid day of for August, any year
      { 2004,  9, 31, false },      // Invalid day of for September, any year
      { 2004, 10, 32, false },      // Invalid day of for October, any year
      { 2004, 11, 31, false },      // Invalid day of for November, any year
      { 2004, 12, 32, false },      // Invalid day of for December, any year

      // BIS-nummers
      { 1904, 53, 31, false },      // month = 13
      { 1904, 41, 32, false },      // Invalid day of month for January, any year
      { 1901, 42, 29, false },      // Invalid day of for February, non-leap year
      { 1904, 42, 30, false },      // Invalid day of for February, leap year
      { 1904, 42, 30, false },      // Invalid day of for February, leap year (2000 is leap-year)
      { 1904, 43, 32, false },      // Invalid day of for March, any year
      { 1904, 44, 31, false },      // Invalid day of for April, any year
      { 1904, 45, 32, false },      // Invalid day of for May, any year
      { 2004, 46, 31, false },      // Invalid day of for June, any year
      { 2004, 47, 32, false },      // Invalid day of for July, any year
      { 2004, 48, 32, false },      // Invalid day of for August, any year
      { 2004, 49, 31, false },      // Invalid day of for September, any year
      { 2004, 50, 32, false },      // Invalid day of for October, any year
      { 2004, 51, 31, false },      // Invalid day of for November, any year
      { 2004, 52, 32, false },      // Invalid day of for December, any year

      // BIS-nummers, unknown gender
      { 1904, 33, 31, false },      // month = 13
      { 1904, 21, 32, false },      // Invalid day of month for January, any year
      { 1901, 22, 29, false },      // Invalid day of for February, non-leap year
      { 1904, 22, 30, false },      // Invalid day of for February, leap year
      { 1904, 22, 30, false },      // Invalid day of for February, leap year (2000 is leap-year)
      { 1904, 23, 32, false },      // Invalid day of for March, any year
      { 1904, 24, 31, false },      // Invalid day of for April, any year
      { 1904, 25, 32, false },      // Invalid day of for May, any year
      { 2004, 26, 31, false },      // Invalid day of for June, any year
      { 2004, 27, 32, false },      // Invalid day of for July, any year
      { 2004, 28, 32, false },      // Invalid day of for August, any year
      { 2004, 29, 31, false },      // Invalid day of for September, any year
      { 2004, 30, 32, false },      // Invalid day of for October, any year
      { 2004, 31, 31, false },      // Invalid day of for November, any year
      { 2004, 32, 32, false },      // Invalid day of for December, any year

      // rijksregisternummers
      { 1904, 13, 31, true },       // month = 13
      { 1904,  1, 32, true },       // Invalid day of month for January, any year
      { 1901,  2, 29, true },       // Invalid day of for February, non-leap year
      { 1904,  2, 30, true },       // Invalid day of for February, leap year
      { 1904,  2, 30, true },       // Invalid day of for February, leap year (2000 is leap-year)
      { 1904,  3, 32, true },       // Invalid day of for March, any year
      { 1904,  4, 31, true },       // Invalid day of for April, any year
      { 1904,  5, 32, true },       // Invalid day of for May, any year
      { 2004,  6, 31, true },       // Invalid day of for June, any year
      { 2004,  7, 32, true },       // Invalid day of for July, any year
      { 2004,  8, 32, true },       // Invalid day of for August, any year
      { 2004,  9, 31, true },       // Invalid day of for September, any year
      { 2004, 10, 32, true },       // Invalid day of for October, any year
      { 2004, 11, 31, true },       // Invalid day of for November, any year
      { 2004, 12, 32, true },       // Invalid day of for December, any year

      // BIS-nummers
      { 1904, 53, 31, true },       // month = 13
      { 1904, 41, 32, true },       // Invalid day of month for January, any year
      { 1901, 42, 29, true },       // Invalid day of for February, non-leap year
      { 1904, 42, 30, true },       // Invalid day of for February, leap year
      { 1904, 42, 30, true },       // Invalid day of for February, leap year (2000 is leap-year)
      { 1904, 43, 32, true },       // Invalid day of for March, any year
      { 1904, 44, 31, true },       // Invalid day of for April, any year
      { 1904, 45, 32, true },       // Invalid day of for May, any year
      { 2004, 46, 31, true },       // Invalid day of for June, any year
      { 2004, 47, 32, true },       // Invalid day of for July, any year
      { 2004, 48, 32, true },       // Invalid day of for August, any year
      { 2004, 49, 31, true },       // Invalid day of for September, any year
      { 2004, 50, 32, true },       // Invalid day of for October, any year
      { 2004, 51, 31, true },       // Invalid day of for November, any year
      { 2004, 52, 32, true },       // Invalid day of for December, any year

      // BIS-nummers, unknown gender
      { 1904, 33, 31, true },       // month = 13
      { 1904, 21, 32, true },       // Invalid day of month for January, any year
      { 1901, 22, 29, true },       // Invalid day of for February, non-leap year
      { 1904, 22, 30, true },       // Invalid day of for February, leap year
      { 1904, 22, 30, true },       // Invalid day of for February, leap year (2000 is leap-year)
      { 1904, 23, 32, true },       // Invalid day of for March, any year
      { 1904, 24, 31, true },       // Invalid day of for April, any year
      { 1904, 25, 32, true },       // Invalid day of for May, any year
      { 2004, 26, 31, true },       // Invalid day of for June, any year
      { 2004, 27, 32, true },       // Invalid day of for July, any year
      { 2004, 28, 32, true },       // Invalid day of for August, any year
      { 2004, 29, 31, true },       // Invalid day of for September, any year
      { 2004, 30, 32, true },       // Invalid day of for October, any year
      { 2004, 31, 31, true },       // Invalid day of for November, any year
      { 2004, 32, 32, true },       // Invalid day of for December, any year
   };

   #region Constants Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeRijksregisternummer_BisNummerMonthOffset_ShouldHaveExpectedValue()
      => BeRijksregisternummer.BisNummerMonthOffset.Should().Be(40);

   [Fact]
   public void BeRijksregisternummer_BisNummerUnknownGenderMonthOffset_ShouldHaveExpectedValue()
      => BeRijksregisternummer.BisNummerUnknownGenderMonthOffset.Should().Be(20);

   [Fact]
   public void BeRijksregisternummer_MinimumValidYearOfBirth_ShouldHaveExpectedValue()
      => BeRijksregisternummer.MinimumValidYearOfBirth.Should().Be(1900);

   [Fact]
   public void BeRijksregisternummer_MaximumValidYearOfBirth_ShouldHaveExpectedValue()
      => BeRijksregisternummer.MaximumValidYearOfBirth.Should().Be(2099);

   #endregion

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   public void BeRijksregisternummer_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawRijksregisternummer(value);

      // Act.
      var sut = new BeRijksregisternummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void BeRijksregisternummer_Constructor_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(year, month, day, formatted: formatted);
      var expected = GetRawRijksregisternummer(value);

      // Act.
      var sut = new BeRijksregisternummer(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void BeRijksregisternummer_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
      => FluentActions
         .Invoking(() => new BeRijksregisternummer(value))
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerEmpty + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void BeRijksregisternummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
      => FluentActions
         .Invoking(() => new BeRijksregisternummer(value))
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerInvalidLength + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void BeRijksregisternummer_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(String value)
      => FluentActions
         .Invoking(() => new BeRijksregisternummer(value))
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerInvalidCharacter + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void BeRijksregisternummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
      => FluentActions
         .Invoking(() => new BeRijksregisternummer(value))
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerInvalidCheckDigits + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.InvalidCheckDigits);

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void BeRijksregisternummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(year, month, day, formatted: formatted);

      // Act/assert.
      FluentActions
         .Invoking(() => new BeRijksregisternummer(value))
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerInvalidDateOfBirth + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.InvalidDateOfBirth);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnValidationPassed_WhenValueHasValidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(year, month, day, formatted: formatted);

      // Act/assert.
      BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.ValidationPassed);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacterWhereDigitExpected(String value)
      => BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnInvalidCheckDigits_WhenValueHasInvalidCheckDigits(String value)
      => BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.InvalidCheckDigits);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidInvalidSeparator(String value)
      => BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.InvalidSeparator);

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidInvalidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(year, month, day, formatted: formatted);

      // Act/assert.
      BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.InvalidDateOfBirth);
   }

   #endregion
}
