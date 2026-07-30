namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class BeIdentityNumberBaseTests
{
   // Valid number variations
   private const String ValidUnformattedRijksregisternummer = "87092100294";                 // Unformatted rijksregisternummer, DOB September 21, 1987, female
   private const String AltValidUnformattedRijksregisternummer = "05031101702";              // Unformatted rijksregisternummer, DOB March 11, 2005, male
   private const String UnformattedRijksregisternummerUnknownDob = "00000118576";            // Unformatted rijksregisternummer, DOB unknown, male
   private const String UnformattedRijksregisternummerYearOnlyDob = "55000007612";           // Unformatted rijksregisternummer, DOB 1955 day/month unknown, female
   private const String UnformattedRijksregisternummerYearMonthOnlyDob = "20070003377";      // Unformatted rijksregisternummer, DOB July 2020, day unknown, male
   private const String UnformattedRijksregisternummerYearDayOnlyDob = "94001300428";        // Unformatted rijksregisternummer, DOB 1994, month unknown, day = 13, female
   private const String UnformattedRijksregisternummerRolloverDob = "00000200136";           // Unformatted rijksregisternummer, DOB unknown with rollover, gender = male

   private const String ValidFormattedRijksregisternummer = "87.09.21-002.94";               // Formatted rijksregisternummer, DOB September 21, 1987, female
   private const String AltValidFormattedRijksregisternummer = "05.03.11-017.02";            // Formatted rijksregisternummer, DOB March 11, 2005, male
   private const String FormattedRijksregisternummerUnknownDob = "00.00.01-185.76";          // Formatted rijksregisternummer, DOB unknown, male
   private const String FormattedRijksregisternummerYearOnlyDob = "55.00.00-076.12";         // Formatted rijksregisternummer, DOB 1955 day/month unknown, female
   private const String FormattedRijksregisternummerYearMonthOnlyDob = "20.07.00-033.77";    // Formatted rijksregisternummer, DOB July 2020, day unknown, male
   private const String FormattedRijksregisternummerYearDayOnlyDob = "94.00.13-004.28";      // Formatted rijksregisternummer, DOB 1994, month unknown, day = 13, female
   private const String FormattedRijksregisternummerRolloverDob = "00.00.02-001.36";         // Formatted rijksregisternummer, DOB unknown with rollover, gender = male

   private const String ValidUnformattedBisnummer = "87492100283";                           // Unformatted BIS-nummer, DOB September 21, 1987, female
   private const String AltValidUnformattedBisnummer = "05431101788";                        // Unformatted BIS-nummer, DOB March 11, 2005, male
   private const String UnformattedBisnummerUnknownDob = "00400118565";                      // Unformatted BIS-nummer, DOB unknown, male
   private const String UnformattedBisnummerYearOnlyDob = "55400007601";                     // Unformatted BIS-nummer, DOB 1955 day/month unknown, female
   private const String UnformattedBisnummerYearMonthOnlyDob = "20470003366";                // Unformatted BIS-nummer, DOB July 2020, day unknown, male
   private const String UnformattedBisnummerYearDayOnlyDob = "94401300417";                  // Unformatted BIS-nummer, DOB 1994, month unknown, day = 13, female
   private const String UnformattedBisnummerRolloverDob = "00400200125";                     // Unformatted BIS-nummer, DOB unknown with rollover, gender = male
   private const String UnformattedBisnummerUnknownGender = "12310100179";                   // Unformatted BIS-nummer, DOB November 1, 2012, gender unknown
   private const String UnformattedBisnummerPartialDobUnknownGender = "34200000276";         // Unformatted BIS-nummer, DOB 1934, day/month unknown, gender unknown
   private const String UnformattedBisnummerUnknownDobUnknownGender = "00200108327";         // Unformatted BIS-nummer, DOB unknown, gender unknown
   private const String UnformattedBisnummerRolloverDobUnknownGender = "00200200179";        // Unformatted BIS-nummer, DOB unknown with rollover, gender unknown

   private const String ValidFormattedBisnummer = "87.49.21-002.83";                         // Formatted BIS-nummer, DOB September 21, 1987, female
   private const String AltValidFormattedBisnummer = "05.43.11-017.88";                      // Formatted BIS-nummer, DOB March 11, 2005, male
   private const String FormattedBisnummerUnknownDob = "00.40.01-185.65";                    // Formatted BIS-nummer, DOB unknown, male
   private const String FormattedBisnummerYearOnlyDob = "55.40.00-076.01";                   // Formatted BIS-nummer, DOB 1955 day/month unknown, female
   private const String FormattedBisnummerYearMonthOnlyDob = "20.47.00-033.66";              // Formatted BIS-nummer, DOB July 2020, day unknown, male
   private const String FormattedBisnummerYearDayOnlyDob = "94.40.13-004.17";                // Formatted BIS-nummer, DOB 1994, month unknown, day = 13, female
   private const String FormattedBisnummerRolloverDob = "00.40.02-001.25";                   // Formatted BIS-nummer, DOB unknown with rollover, gender = male
   private const String FormattedBisnummerUnknownGender = "12.31.01-001.79";                 // Formatted BIS-nummer, DOB November 1, 2012, gender unknown
   private const String FormattedBisnummerPartialDobUnknownGender = "34.20.00-002.76";       // Formatted BIS-nummer, DOB 1934, day/month unknown, gender unknown
   private const String FormattedBisnummerUnknownDobUnknownGender = "00.20.01-083.27";       // Formatted BIS-nummer, DOB unknown, gender unknown
   private const String FormattedBisnummerRolloverDobUnknownGender = "00.20.02-001.79";      // Formatted BIS-nummer, DOB unknown with rollover, gender unknown

   protected static String GetRawValue(String value)
      => value.Length == 11
         ? value
         : value[0..2] + value[3..5] + value[6..8] +
           value[9..12] + value[13..];

   protected static String GetValueWithValidCheckDigits(
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
      ValidUnformattedRijksregisternummer,
      AltValidUnformattedRijksregisternummer,
      UnformattedRijksregisternummerUnknownDob,
      UnformattedRijksregisternummerYearOnlyDob,
      UnformattedRijksregisternummerYearMonthOnlyDob,
      UnformattedRijksregisternummerYearDayOnlyDob,
      UnformattedRijksregisternummerRolloverDob,
      ValidFormattedRijksregisternummer,
      AltValidFormattedRijksregisternummer,
      FormattedRijksregisternummerUnknownDob,
      FormattedRijksregisternummerYearOnlyDob,
      FormattedRijksregisternummerYearMonthOnlyDob,
      FormattedRijksregisternummerYearDayOnlyDob,
      FormattedRijksregisternummerRolloverDob,
   ];

   public static TheoryData<String> ValidBisnummerValues =>
   [
      ValidUnformattedBisnummer,
      ValidFormattedBisnummer,
      UnformattedBisnummerUnknownDob,
      UnformattedBisnummerYearOnlyDob,
      UnformattedBisnummerYearMonthOnlyDob,
      UnformattedBisnummerYearDayOnlyDob,
      UnformattedBisnummerRolloverDob,
      UnformattedBisnummerUnknownGender,
      UnformattedBisnummerPartialDobUnknownGender,
      UnformattedBisnummerUnknownDobUnknownGender,
      UnformattedBisnummerRolloverDobUnknownGender,
      AltValidUnformattedBisnummer,
      AltValidFormattedBisnummer,
      FormattedBisnummerUnknownDob,
      FormattedBisnummerYearOnlyDob,
      FormattedBisnummerYearMonthOnlyDob,
      FormattedBisnummerYearDayOnlyDob,
      FormattedBisnummerRolloverDob,
      FormattedBisnummerUnknownGender,
      FormattedBisnummerPartialDobUnknownGender,
      FormattedBisnummerUnknownDobUnknownGender,
      FormattedBisnummerRolloverDobUnknownGender,
   ];

   public static TheoryData<Int32, Int32, Boolean> ValidSequenceNumberBoundaryValues = new()
   {
      // Unformatted values
      { 1965,   1, false },      // Sequence number lower bound
      { 1965, 998, false },      // Sequence number upper bound
      { 2010,   1, false },
      { 2010, 998, false },

      // Formatted values
      { 1965,   1, true },
      { 1965, 998, true },
      { 2010,   1, true },
      { 2010, 998, true },
   };

   public static TheoryData<Int32, Int32, Int32, Boolean> ValidRijksregisternummerDateOfBirthValues = new()
   {
      // rijksregisternummers, year boundaries, false = unformatted
      { 1900,  1,  1, false },   // January 1, 1900
      { 1999, 12, 31, false },   // December 31, 1999
      { 2000,  1,  1, false },   // January 1, 2000
      { 2099, 12, 31, false },   // December 31, 2099

      // rijksregisternummers, maximum days per month
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

      // rijksregisternummers, incomplete/unknown dates of birth
      { 1950,  0,  0, false },   // Incomplete date of birth, only year known
      { 2010,  0,  1, false },   // Incomplete date of birth, with rollover for too many incomplete dates of birth for known year
      {    0,  0,  1, false },   // Unknown date of birth

      // rijksregisternummers, year boundaries, true = formatted
      { 1900,  1,  1, true },    // January 1, 1900
      { 1999, 12, 31, true },    // December 31, 1999
      { 2000,  1,  1, true },    // January 1, 2000
      { 2099, 12, 31, true },    // December 31, 2099

      // rijksregisternummers, maximum days per month
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

      // rijksregisternummers, incomplete/unknown dates of birth
      { 1950,  0,  0, true },    // Incomplete date of birth, only year known
      { 2010,  0,  1, true },    // Incomplete date of birth,  with rollover for too many incomplete dates of birth for known year
      {    0,  0,  1, true },    // Unknown date of birth
   };

   public static TheoryData<Int32, Int32, Int32, Boolean> ValidBisnummerDateOfBirthValues = new()
   {
      // BIS-nummers, year boundaries, false = unformatted
      { 1900, 41,  1, false },   // January 1, 1900
      { 1999, 52, 31, false },   // December 31, 1999
      { 2000, 41,  1, false },   // January 1, 2000
      { 2099, 52, 31, false },   // December 31, 2099

      // BIS-nummers, maximum days per month
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

      // BIS-nummers, incomplete/unknown dates of birth
      { 1950, 40,  0, false },   // Incomplete date of birth, only year known
      { 2010, 40,  1, false },   // Incomplete date of birth,  with rollover for too many incomplete dates of birth for known year
      {    0, 40,  1, false },   // Unknown date of birth

      // BIS-nummers, unknown gender, year boundaries, false = unformatted
      { 1900, 21,  1, false },   // January 1, 1900
      { 1999, 32, 31, false },   // December 31, 1999
      { 2000, 21,  1, false },   // January 1, 2000
      { 2099, 32, 31, false },   // December 31, 2099

      // BIS-nummers, unknown gender, maximum days per month
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

      // BIS-nummers, unknown gender, incomplete/unknown dates of birth
      { 1950, 20,  0, false },   // Incomplete date of birth, only year known
      { 2010, 20,  1, false },   // Incomplete date of birth,  with rollover for too many incomplete dates of birth for known year
      {    0, 20,  1, false },   // Unknown date of birth

      // BIS-nummers, year boundaries, true = formatted
      { 1900, 41,  1, true },    // January 1, 1900
      { 1999, 52, 31, true },    // December 31, 1999
      { 2000, 41,  1, true },    // January 1, 2000
      { 2099, 52, 31, true },    // December 31, 2099

      // BIS-nummers, maximum days per month
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

      // BIS-nummers, incomplete/unknown dates of birth
      { 1950, 40,  0, true },    // Incomplete date of birth, only year known
      { 2010, 40,  1, true },    // Incomplete date of birth,  with rollover for too many incomplete dates of birth for known year
      {    0, 40,  1, true },    // Unknown date of birth

      // BIS-nummers, unknown gender, year boundaries, true = formatted
      { 1900, 21,  1, true },    // January 1, 1900
      { 1999, 32, 31, true },    // December 31, 1999
      { 2000, 21,  1, true },    // January 1, 2000
      { 2099, 32, 31, true },    // December 31, 2099

      // BIS-nummers, unknown gender, maximum days per month
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

      // BIS-nummers, unknown gender, incomplete/unknown dates of birth
      { 1950, 20,  0, true },    // Incomplete date of birth, only year known
      { 2010, 20,  1, true },    // Incomplete date of birth,  with rollover for too many incomplete dates of birth for known year
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

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted values
      { ".7110804680", 0 },            // Non-digit character '.'
      { "1 110804680", 1 },            // Non-digit character ' '
      { "17A10804680", 2 },            // Non-digit character 'A'
      { "171Z0804680", 3 },            // Non-digit character 'Z'
      { "1711^804680", 4 },            // Non-digit character '^'
      { "17110a04680", 5 },            // Non-digit character 'a'
      { "171108z4680", 6 },            // Non-digit character 'z'
      { "1711080~680", 7 },            // Non-digit character '~'
      { "17110804\u215380", 8 },       // Non-digit character Unicode fraction 1/3
      { "171108046\u00D60", 9 },       // Invalid character unicode O with umlaut
      { "1711080468\u0BE6", 10 },      // Invalid character unicode Tamil digit 0

      // Formatted values
      { ".7.11.08-046.80", 0 },        // Non-digit character '.'
      { "1 .11.08-046.80", 1 },        // Non-digit character ' '
      { "17.A1.08-046.80", 3 },        // Non-digit character 'A'
      { "17.1Z.08-046.80", 4 },        // Non-digit character 'Z'
      { "17.11.^8-046.80", 6 },        // Non-digit character '^'
      { "17.11.0a-046.80", 7 },        // Non-digit character 'a'
      { "17 11 08 z46 80", 9 },        // Non-digit character 'z'
      { "17 11 08 0~6 80", 10 },       // Non-digit character '~'
      { "17 11 08 04\u2153 80", 11 },  // Non-digit character Unicode fraction 1/3
      { "17 11 08 046 \u00D60", 13 },  // Invalid character unicode O with umlaut
      { "17 11 08 046 8\u0BE6", 14 },  // Invalid character unicode Tamil digit 0
   };

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

   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      // First separator location
      { "85007.30-033.28", 2 },
      { "85107.30-033.28", 2 },
      { "85207.30-033.28", 2 },
      { "85307.30-033.28", 2 },
      { "85407.30-033.28", 2 },
      { "85507.30-033.28", 2 },
      { "85607.30-033.28", 2 },
      { "85707.30-033.28", 2 },
      { "85807.30-033.28", 2 },
      { "85907.30-033.28", 2 },

      // Second separator location
      { "85.07030-033.28", 5 },
      { "85.07130-033.28", 5 },
      { "85.07230-033.28", 5 },
      { "85.07330-033.28", 5 },
      { "85.07430-033.28", 5 },
      { "85.07530-033.28", 5 },
      { "85.07630-033.28", 5 },
      { "85.07730-033.28", 5 },
      { "85.07830-033.28", 5 },
      { "85.07930-033.28", 5 },

      // Third separator location
      { "85.07.300033.28", 8 },
      { "85.07.301033.28", 8 },
      { "85.07.302033.28", 8 },
      { "85.07.303033.28", 8 },
      { "85.07.304033.28", 8 },
      { "85.07.305033.28", 8 },
      { "85.07.306033.28", 8 },
      { "85.07.307033.28", 8 },
      { "85.07.308033.28", 8 },
      { "85.07.309033.28", 8 },

      // Fourth separator location
      { "85.07.30-033028", 12 },
      { "85.07.30-033128", 12 },
      { "85.07.30-033228", 12 },
      { "85.07.30-033328", 12 },
      { "85.07.30-033428", 12 },
      { "85.07.30-033528", 12 },
      { "85.07.30-033628", 12 },
      { "85.07.30-033728", 12 },
      { "85.07.30-033828", 12 },
      { "85.07.30-033928", 12 },
   };

}
