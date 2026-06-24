// Ignore Spelling: Deserialize Deserialization Json Kf Nummer Rijksregisternummer

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class BeRijksregisternummerTests
{
   private const String Valid11CharacterRijksregisternummer = "17110804680";
   private const String AltValid11CharacterRijksregisternummer = "85073003328";
   private const String Valid15CharacterRijksregisternummer = "17.11.08-046.80";
   private const String AltValid15CharacterRijksregisternummer = "85.07.30-033.28";
   private const String IncompleteDateOfBirthRijksregisternummer = "40 00 00 955-79";
   private const String UnknownDateOfBirthRijksregisternummer = "00 00 01 003-64";
   private const String Valid11CharacterBisnummer = "01430801695";
   private const String AltValid11CharacterBisnummer = "17510804640";
   private const String Valid15CharacterBisnummer = "01.43.08-016.95";
   private const String AltValid15CharacterBisnummer = "17.51.08-046.40";
   private const String IncompleteDateOfBirthBisnummer = "01 40 00 955-54";               // 2001
   private const String UnknownDateOfBirthBisnummer = "00 40 01 003-53";
   private const String IncompleteDateOfBirthUnknownGenderBisnummer = "01 20 00 955-11";  // 2001
   private const String UnknownDateOfBirthUnknownGenderBisnummer = "00 20 01 003-10";

   private static String GetRawRijksregisternummer(String rijksregisternummer)
      => rijksregisternummer.Length == 11
         ? rijksregisternummer
         : rijksregisternummer[0..2] + rijksregisternummer[3..5] + rijksregisternummer[6..8] +
         rijksregisternummer[9..12] + rijksregisternummer[13..];

   private static String GetRijksregisternummerWithValidCheckDigits(
      Int32 year = 1987,
      Int32 month = 7,
      Int32 day = 30,
      Int32 sequenceNumber = 33,
      Boolean formatted = false)
   {
      var temp = $"{(year >= 2000 ? 2 : 0)}{year % 100:D2}{month:D2}{day:D2}{sequenceNumber:D3}";
      var checkSum = GetCheckSum(temp);

      return formatted
         ? $"{year % 100:D2}.{month:D2}.{day:D2}-{sequenceNumber:D3}.{checkSum:D2}"
         : $"{year % 100:D2}{month:D2}{day:D2}{sequenceNumber:D3}{checkSum:D2}";
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
      Valid11CharacterRijksregisternummer,
      AltValid11CharacterRijksregisternummer,
      Valid15CharacterRijksregisternummer,
      AltValid15CharacterRijksregisternummer,
      IncompleteDateOfBirthRijksregisternummer,
      UnknownDateOfBirthRijksregisternummer,
      Valid11CharacterBisnummer,
      Valid15CharacterBisnummer,
      AltValid11CharacterBisnummer,
      AltValid15CharacterBisnummer,
      IncompleteDateOfBirthBisnummer,
      UnknownDateOfBirthBisnummer,
      IncompleteDateOfBirthUnknownGenderBisnummer,
      UnknownDateOfBirthUnknownGenderBisnummer,
   ];

   public static TheoryData<Int32, Int32, Boolean> ValidSequenceNumberBoundaryValues = new()
   {
      { 1965,   1, false },      // Sequence number lower bound
      { 1965, 998, false },      // Sequence number upper bound
      { 2010,   1, false },
      { 2010, 998, false },

      { 1965,   1, true },
      { 1965, 998, true },
      { 2010,   1, true },
      { 2010, 998, true },
   };

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
      { 2010,  0,  1, false },   // Incomplete date of birth, with rollover for to many incomplete dates of birth for known year
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
      { 2010, 40,  1, false },   // Incomplete date of birth,  with rollover for to many incomplete dates of birth for known year
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
      { 2010, 20,  1, false },   // Incomplete date of birth,  with rollover for to many incomplete dates of birth for known year
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
      { 2010,  0,  1, true },    // Incomplete date of birth,  with rollover for to many incomplete dates of birth for known year
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
      { 2010, 40,  1, true },    // Incomplete date of birth,  with rollover for to many incomplete dates of birth for known year
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
      { 2010, 20,  1, true },    // Incomplete date of birth,  with rollover for to many incomplete dates of birth for known year
      {    0, 20,  1, true },    // Unknown date of birth
   };

   public static TheoryData<String> InvalidLengthValues =>
   [
      "1711080468",           // Length 10
      "171108046801",         // Length 11
      "85.07.30-033.2",       // Length 14
      "85.07.30-033.289",     // Length 16
      new String('1', 100)    // Very long string
   ];

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
      "85073003300",             // 85073003328 with invalid check digits -> 00
      "17110804698",             // 17110804680 with invalid check digits -> 98
      "85073003399",             // 85073003328 with invalid check digits -> 99

      "85.07.20-033.28",         // 85073003328 with single digit transcription error, 3 -> 2
      "17.11.08-056.80",         // 17110804680 with single digit transcription error, 4 -> 5
      "85.07.20-033.29",         // 85072003328 with check digit transcription error, 8 -> 9
      "17.11.80-056.80",         // 17110805680 with two digit transposition error, 08 -> 80
      "85 03 70 033 28",         // 85073003328 with two digit transposition error, 73 -> 37
      "17 11 04 086 80",         // 17110804680 with two digit jump transposition, 804 -> 408
      "85 07 30 044 28",         // 85073003328 with two digit twin error, 33 -> 44
      "17 22 08 046 80",         // 17110804680 with two digit twin error, 11 -> 22
      "85.07.30-033.00",         // 85073003328 with invalid check digits -> 00
      "17.11.08-046.98",         // 17110804680 with invalid check digits -> 98
      "85.07.30-033.99",         // 85073003328 with invalid check digits -> 99
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

   public static TheoryData<String> InvalidSequenceNumberValues =>
   [
      "17110800097",          // 1917
      "17110899968",          // 1917
      "17.11.08-000.97",      // 1917
      "17.11.08-999.68",      // 1917
      "17110800029",          // 2017
      "17110899997",          // 2017
      "17.11.08-000.29",      // 2017
      "17.11.08-999.97",      // 2017
   ];

   public static TheoryData<Int32, Int32, Int32, Boolean> InvalidDateOfBirthValues = new()
   {
      // rijksregisternummers
      {    0,  0,  0, false },      // Unknown date of birth requires non-zero day
      { 1904, 13, 31, false },      // month = 13
      { 1904,  1,  0, false },      // day = 0
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
      {    0, 40,  0, false },      // Unknown date of birth requires non-zero day
      { 1904, 53, 31, false },      // month = 13
      { 1904, 41,  0, false },      // day = 0
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
      {    0, 20,  0, false },      // Unknown date of birth requires non-zero day
      { 1904, 33, 31, false },      // month = 13
      { 1904, 21,  0, false },      // day = 0
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
      {    0,  0,  0, true },       // Unknown date of birth requires non-zero day
      { 1904, 13, 31, true },       // month = 13
      { 1904,  1,  0, true },       // day = 0
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
      {    0, 40,  0, true },       // Unknown date of birth requires non-zero day
      { 1904, 53, 31, true },       // month = 13
      { 1904, 41,  0, true },       // day = 0
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
      {    0, 20,  0, true },       // Unknown date of birth requires non-zero day
      { 1904, 33, 31, true },       // month = 13
      { 1904, 21,  0, true },       // day = 0
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
   [MemberData(nameof(ValidSequenceNumberBoundaryValues))]
   public void BeRijksregisternummer_Constructor_ShouldCreateInstance_WhenValueHasValidSerialNumber(
      Int32 year,
      Int32 sequenceNumber,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(
         year,
         sequenceNumber: sequenceNumber,
         formatted: formatted);
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
   [MemberData(nameof(InvalidSeparatorValues))]
   public void BeRijksregisternummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String value)
      => FluentActions
         .Invoking(() => new BeRijksregisternummer(value))
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerInvalidSeparator + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.InvalidSeparator);

   [Theory]
   [MemberData(nameof(InvalidSequenceNumberValues))]
   public void BeRijksregisternummer_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSequenceNumber(String value)
      => FluentActions
         .Invoking(() => new BeRijksregisternummer(value))
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerInvalidSequenceNumber + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.InvalidSequenceNumber);

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

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void BeRijksregisternummer_DateOfBirth_ShouldReturnExpectedValue(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(year, month, day, formatted: formatted);
      var sut = new BeRijksregisternummer(value);

      month = month switch
      {
         >= 20 and <= 32 => month - BeRijksregisternummer.BisNummerUnknownGenderMonthOffset,
         >= 40 and <= 52 => month - BeRijksregisternummer.BisNummerMonthOffset,
         _ => month,
      };
      var expected = new DateResult(
         year > 0 ? year : null,
         month > 0 ? month : null,
         month == 0 ? null : day);

      // Act/assert.
      sut.DateOfBirth.Should().Be(expected);
   }

   #endregion

   #region Gender Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(10, 180, BinaryOrUnknownGender.Female, false)]
   [InlineData(10, 181, BinaryOrUnknownGender.Male, false)]
   [InlineData(10, 182, BinaryOrUnknownGender.Female, false)]
   [InlineData(10, 183, BinaryOrUnknownGender.Male, false)]
   [InlineData(10, 184, BinaryOrUnknownGender.Female, false)]
   [InlineData(10, 185, BinaryOrUnknownGender.Male, true)]
   [InlineData(10, 186, BinaryOrUnknownGender.Female, true)]
   [InlineData(10, 187, BinaryOrUnknownGender.Male, true)]
   [InlineData(10, 188, BinaryOrUnknownGender.Female, true)]
   [InlineData(10, 189, BinaryOrUnknownGender.Male, true)]

   [InlineData(21, 181, BinaryOrUnknownGender.Unknown, false)]
   [InlineData(32, 182, BinaryOrUnknownGender.Unknown, true)]
   public void BeRijksregisternummer_Gender_ShouldReturnExpectedValue(
      Int32 month,
      Int32 sequenceNumber,
      BinaryOrUnknownGender expectedGender,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(
         month: month,
         sequenceNumber: sequenceNumber,
         formatted: formatted);
      var sut = new BeRijksregisternummer(value);

      // Act/assert.
      sut.Gender.Should().Be(expectedGender);
   }

   #endregion

   #region IdentifierType Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(Valid11CharacterRijksregisternummer, BeIdentifierType.Rijksregisternummer)]
   [InlineData(AltValid15CharacterRijksregisternummer, BeIdentifierType.Rijksregisternummer)]
   [InlineData(IncompleteDateOfBirthRijksregisternummer, BeIdentifierType.Rijksregisternummer)]
   [InlineData(UnknownDateOfBirthRijksregisternummer, BeIdentifierType.Rijksregisternummer)]
   [InlineData(Valid11CharacterBisnummer, BeIdentifierType.BisNummer)]
   [InlineData(AltValid15CharacterBisnummer, BeIdentifierType.BisNummer)]
   [InlineData(IncompleteDateOfBirthBisnummer, BeIdentifierType.BisNummer)]
   [InlineData(UnknownDateOfBirthBisnummer, BeIdentifierType.BisNummer)]
   [InlineData(UnknownDateOfBirthUnknownGenderBisnummer, BeIdentifierType.BisNummer)]
   public void BeRijksregisternummer_IdentifierType_ShouldReturnExpectedValue(
      String value,
      BeIdentifierType expectedIdentifierType)
   {
      // Arrange.
      var sut = new BeRijksregisternummer(value);

      // Act/assert.
      sut.IdentifierType.Should().Be(expectedIdentifierType);
   }

   [Theory]
   [InlineData(1, BeIdentifierType.Rijksregisternummer)]     // Month 1 = rijksregisternummer
   [InlineData(12, BeIdentifierType.Rijksregisternummer)]    // Month 12 = rijksregisternummer
   [InlineData(20, BeIdentifierType.BisNummer)]              // Month 20 = BIS unknown gender (min)
   [InlineData(32, BeIdentifierType.BisNummer)]              // Month 32 = BIS unknown gender (max)
   [InlineData(40, BeIdentifierType.BisNummer)]              // Month 40 = BIS (min)
   [InlineData(52, BeIdentifierType.BisNummer)]              // Month 52 = BIS (max)
   public void BeRijksregisternummer_IdentifierType_ShouldReturnExpectedValue_ForMonthBoundaries(
      Int32 month,
      BeIdentifierType expectedType)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(month: month);
      var sut = new BeRijksregisternummer(value);

      // Act/assert.
      sut.IdentifierType.Should().Be(expectedType);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(Valid11CharacterRijksregisternummer, Valid11CharacterRijksregisternummer)]
   [InlineData(Valid15CharacterRijksregisternummer, Valid11CharacterRijksregisternummer)]
   [InlineData(Valid11CharacterBisnummer, Valid11CharacterBisnummer)]
   [InlineData(Valid15CharacterBisnummer, Valid11CharacterBisnummer)]
   public void BeRijksregisternummer_Value_ShouldReturnValidatedRijksregisternummer(
      String value,
      String expected)
   {
      // Arrange.
      var sut = new BeRijksregisternummer(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeRijksregisternummer_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = Valid11CharacterRijksregisternummer;
      var sut = new BeRijksregisternummer(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(value);
   }

   [Fact]
   public void BeRijksregisternummer_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = Valid15CharacterBisnummer;
      var sut = new BeRijksregisternummer(value);
      var expected = GetRawRijksregisternummer(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().NotBeNullOrEmpty();
      str.Should().Be(expected);
   }

   [Fact]
   public void BeRijksregisternummer_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      BeRijksregisternummer sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void BeRijksregisternummer_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      BeRijksregisternummer sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   public void BeRijksregisternummer_ExplicitCastToBeRijksregisternummer_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawRijksregisternummer(value);

      // Act.
      var sut = (BeRijksregisternummer)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidSequenceNumberBoundaryValues))]
   public void BeRijksregisternummer_ExplicitCastToBeRijksregisternummer_ShouldCreateInstance_WhenValueHasValidSequenceNumber(
      Int32 year,
      Int32 sequenceNumber,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(
         year,
         sequenceNumber: sequenceNumber,
         formatted: formatted);
      var expected = GetRawRijksregisternummer(value);

      // Act.
      var sut = (BeRijksregisternummer)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void BeRijksregisternummer_ExplicitCastToBeRijksregisternummer_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(year, month, day, formatted: formatted);
      var expected = GetRawRijksregisternummer(value);

      // Act.
      var sut = (BeRijksregisternummer)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void BeRijksregisternummer_ExplicitCastToBeRijksregisternummer_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
      => FluentActions
         .Invoking(() => _ = (BeRijksregisternummer)value)
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerEmpty + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void BeRijksregisternummer_ExplicitCastToBeRijksregisternummer_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
      => FluentActions
         .Invoking(() => _ = (BeRijksregisternummer)value)
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerInvalidLength + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void BeRijksregisternummer_ExplicitCastToBeRijksregisternummer_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(String value)
      => FluentActions
         .Invoking(() => _ = (BeRijksregisternummer)value)
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerInvalidCharacter + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void BeRijksregisternummer_ExplicitCastToBeRijksregisternummer_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
      => FluentActions
         .Invoking(() => _ = (BeRijksregisternummer)value)
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerInvalidCheckDigits + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.InvalidCheckDigits);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void BeRijksregisternummer_ExplicitCastToBeRijksregisternummer_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(String value)
      => FluentActions
         .Invoking(() => _ = (BeRijksregisternummer)value)
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerInvalidSeparator + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.InvalidSeparator);

   [Theory]
   [MemberData(nameof(InvalidSequenceNumberValues))]
   public void BeRijksregisternummer_ExplicitCastToBeRijksregisternummer_ShouldThrowKfValidationException_WhenValueHasInvalidSequenceNumber(String value)
      => FluentActions
         .Invoking(() => _ = (BeRijksregisternummer)value)
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerInvalidSequenceNumber + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.InvalidSequenceNumber);

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void BeRijksregisternummer_ExplicitCastToBeRijksregisternummer_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(year, month, day, formatted: formatted);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (BeRijksregisternummer)value)
         .Should().Throw<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerInvalidDateOfBirth + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.InvalidDateOfBirth);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeRijksregisternummer_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);
      var sut2 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void BeRijksregisternummer_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);
      var sut2 = new BeRijksregisternummer(AltValid11CharacterRijksregisternummer);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void BeRijksregisternummer_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 15 character versions for same person should still be equal.
      var sut1 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);
      var sut2 = new BeRijksregisternummer(Valid15CharacterRijksregisternummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void BeRijksregisternummer_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqualAndHaveIncompleteDateOfBirth()
   {
      var sut1 = new BeRijksregisternummer(IncompleteDateOfBirthRijksregisternummer);
      var sut2 = new BeRijksregisternummer(IncompleteDateOfBirthRijksregisternummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeRijksregisternummer_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);
      var sut2 = new BeRijksregisternummer(AltValid11CharacterRijksregisternummer);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void BeRijksregisternummer_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 15 character versions for same person should still be equal.
      var sut1 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);
      var sut2 = new BeRijksregisternummer(Valid15CharacterRijksregisternummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void BeRijksregisternummer_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);
      var sut2 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void BeRijksregisternummer_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqualAndHaveIncompleteDateOfBirth()
   {
      // Arrange.
      var sut1 = new BeRijksregisternummer(IncompleteDateOfBirthRijksregisternummer);
      var sut2 = new BeRijksregisternummer(IncompleteDateOfBirthRijksregisternummer);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   public void BeRijksregisternummer_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expectedValue = new BeRijksregisternummer(value);

      // Act.
      var result = BeRijksregisternummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(ValidSequenceNumberBoundaryValues))]
   public void BeRijksregisternummer_Create_ShouldCreateInstance_WhenValueHasValidSequenceNumber(
      Int32 year,
      Int32 sequenceNumber,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(
         year,
         sequenceNumber: sequenceNumber,
         formatted: formatted);
      var expectedValue = new BeRijksregisternummer(value);

      // Act.
      var result = BeRijksregisternummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void BeRijksregisternummer_Create_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(year, month, day, formatted: formatted);
      var expectedValue = new BeRijksregisternummer(value);

      // Act.
      var result = BeRijksregisternummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expectedValue);
      result.ValidationFailure.Should().Be(default);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void BeRijksregisternummer_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Act.
      var result = BeRijksregisternummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(BeRijksregisternummerValidationResult.Empty);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void BeRijksregisternummer_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Act.
      var result = BeRijksregisternummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(BeRijksregisternummerValidationResult.InvalidLength);
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void BeRijksregisternummer_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacterWhereDigitExpected(String value)
   {
      // Act.
      var result = BeRijksregisternummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(BeRijksregisternummerValidationResult.InvalidCharacter);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void BeRijksregisternummer_Create_ShouldReturnInvalidCheckDigitsValidationResult_WhenValueHasInvalidCheckDigits(String value)
   {
      // Act.
      var result = BeRijksregisternummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(BeRijksregisternummerValidationResult.InvalidCheckDigits);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void BeRijksregisternummer_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(String value)
   {
      // Act.
      var result = BeRijksregisternummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(BeRijksregisternummerValidationResult.InvalidSeparator);
   }

   [Theory]
   [MemberData(nameof(InvalidSequenceNumberValues))]
   public void BeRijksregisternummer_Create_ShouldReturnInvalidSequenceNumberValidationResult_WhenValueHasInvalidSequenceNumber(String value)
   {
      // Act.
      var result = BeRijksregisternummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(BeRijksregisternummerValidationResult.InvalidSequenceNumber);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void BeRijksregisternummer_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(
      Int32 year,
      Int32 month,
      Int32 day,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(year, month, day, formatted: formatted);

      // Act.
      var result = BeRijksregisternummer.Create(value);

      // Assert.
      result.Should().NotBeNull();
      result.IsSuccess.Should().BeFalse();
      result.Value.Should().Be(null);
      result.ValidationFailure.Should().Be(BeRijksregisternummerValidationResult.InvalidDateOfBirth);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeRijksregisternummer_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);
      var sut2 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void BeRijksregisternummer_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);
      var sut2 = new BeRijksregisternummer(AltValid11CharacterRijksregisternummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void BeRijksregisternummer_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 15 character versions for same person should still be equal.
      var sut1 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);
      var sut2 = new BeRijksregisternummer(Valid15CharacterRijksregisternummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void BeRijksregisternummer_Equals_ShouldReturnTrue_WhenValuesAreEqualAndHaveIncompleteDateOfBirth()
   {
      // Arrange.
      var sut1 = new BeRijksregisternummer(IncompleteDateOfBirthUnknownGenderBisnummer);
      var sut2 = new BeRijksregisternummer(IncompleteDateOfBirthUnknownGenderBisnummer);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeRijksregisternummer_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new BeRijksregisternummer(Valid11CharacterBisnummer);
      var expected = Valid15CharacterBisnummer;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void BeRijksregisternummer_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);
      var mask = "___________";
      var expected = Valid11CharacterRijksregisternummer;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void BeRijksregisternummer_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new BeRijksregisternummer(Valid11CharacterBisnummer);
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
   public void BeRijksregisternummer_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);
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
   public void BeRijksregisternummer_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new BeRijksregisternummer(Valid11CharacterBisnummer);
      var sut2 = new BeRijksregisternummer(Valid11CharacterBisnummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void BeRijksregisternummer_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);
      var sut2 = new BeRijksregisternummer(AltValid11CharacterRijksregisternummer);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void BeRijksregisternummer_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 11 and 15 character versions for same person should still be equal.
      var sut1 = new BeRijksregisternummer(Valid11CharacterRijksregisternummer);
      var sut2 = new BeRijksregisternummer(Valid15CharacterRijksregisternummer);

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

   // BeRijksregisternummer does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void BeRijksregisternummer_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new BeRijksregisternummer(UnknownDateOfBirthRijksregisternummer);
      var sut2 = new BeRijksregisternummer(UnknownDateOfBirthRijksregisternummer);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidRijksregisternummerValues))]
   public void BeRijksregisternummer_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new BeRijksregisternummer(value);
      var expected = GetRawRijksregisternummer(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
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
   [MemberData(nameof(ValidSequenceNumberBoundaryValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnValidationPassed_WhenValueHasValidSequenceNumber(
      Int32 year,
      Int32 sequenceNumber,
      Boolean formatted)
   {
      // Arrange.
      var value = GetRijksregisternummerWithValidCheckDigits(
         year,
         sequenceNumber: sequenceNumber,
         formatted: formatted);

      // Act/assert.
      BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.ValidationPassed);
   }

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
   [MemberData(nameof(InvalidSequenceNumberValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnInvalidSequenceNumber_WhenValueHasInvalidInvalidSequenceNumber(String value)
      => BeRijksregisternummer.Validate(value).Should().Be(BeRijksregisternummerValidationResult.InvalidSequenceNumber);

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void BeRijksregisternummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(
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

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void BeRijksregisternummer_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new BeRijksregisternummer(Valid15CharacterRijksregisternummer);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<BeRijksregisternummer>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void BeRijksregisternummer_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new BeRijksregisternummer(AltValid11CharacterBisnummer);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public BeRijksregisternummer Rijksregisternummer { get; set; } = null!;
   }

   [Fact]
   public void BeRijksregisternummer_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Rijksregisternummer = new BeRijksregisternummer(Valid15CharacterBisnummer) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void BeRijksregisternummer_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Rijksregisternummer\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void BeRijksregisternummer_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Rijksregisternummer\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Rijksregisternummer.Should().BeNull();
   }

   [Fact]
   public void BeRijksregisternummer_JsonDeserialization_ShouldThrowKfValidationException_WhenRijksregisternummerIsInvalid()
   {
      // Arrange.
      var json = "{\"Rijksregisternummer\":\"17.11.08-046.803\"}";  // Invalid length

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should()
         .ThrowExactly<KfValidationException<BeRijksregisternummerValidationResult>>()
         .WithMessage(Messages.BeRijksregisternummerInvalidLength + "*")
         .And.ValidationResult.Should().Be(BeRijksregisternummerValidationResult.InvalidLength);
   }

   #endregion
}
