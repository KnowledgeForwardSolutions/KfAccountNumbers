namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class SeIdentityNumberTestsBase
{
   protected const String ValidShortFormatDashPersonnummer = "811228-9874";        // From Wikipedia, https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
   protected const String ValidShortFormatPlusPersonnummer = "811228+9874";
   protected const String AltValidShortFormatDashPersonnummer = "670919-9530";
   protected const String AltValidShortFormatPlusPersonnummer = "670919+9530";
   protected const String ValidLongFormatDashPersonnummer = "19670919-9530";
   protected const String ValidLongFormatPlusPersonnummer = "20670919+9530";      // Future date, but valid format and checksum

   protected const String ValidShortFormatDashSamordningsnummer = "811288-9871";
   protected const String ValidShortFormatPlusSamordningsnummer = "811288+9871";
   protected const String AltValidShortFormatDashSamordningsnummer = "670979-9537";
   protected const String AltValidShortFormatPlusSamordningsnummer = "670979+9537";
   protected const String ValidLongFormatDashSamordningsnummer = "19670979-9537";
   protected const String ValidLongFormatPlusSamordningsnummer = "20670979+9537"; // Future date, but valid format and checksum

   public static TheoryData<String> ValidPersonnummerValues =>
   [
      ValidShortFormatDashPersonnummer,
      ValidShortFormatPlusPersonnummer,
      AltValidShortFormatDashPersonnummer,
      AltValidShortFormatPlusPersonnummer,
      ValidLongFormatDashPersonnummer,
      ValidLongFormatPlusPersonnummer
   ];

   public static TheoryData<String> ValidSamordningsnummerValues =>
   [
      ValidShortFormatDashSamordningsnummer,
      ValidShortFormatPlusSamordningsnummer,
      AltValidShortFormatDashSamordningsnummer,
      AltValidShortFormatPlusSamordningsnummer,
      ValidLongFormatDashSamordningsnummer,
      ValidLongFormatPlusSamordningsnummer
   ];

   public static TheoryData<String> PersonnummerUndetectableCheckDigitErrors =>
   [
      "010430-0918",       // 010430-9018 with two digit transposition 90 -> 09
      "880411+2558",       // 880411+2228 with two digit twin error 22 -> 55
      "20010430-0918",     // 20010430-9018 with two digit transposition 90 -> 09
      "19880411-2558",     // 19880411+2228 with two digit twin error 22 -> 55
   ];

   public static TheoryData<String> SamordningsnummerUndetectableCheckDigitErrors =>
   [
      "010490-0915",       // 010490-9015 with two digit transposition 90 -> 09
      "880471+2555",       // 880471+2225 with two digit twin error 22 -> 55
      "20010490-0915",     // 20010490-9015 with two digit transposition 90 -> 09
      "19880471-2555",     // 19880471+2225 with two digit twin error 22 -> 55
   ];

   public static TheoryData<String, Char, String> PersonnummerValidDateOfBirthValues = new()
   {
      // Min/max/boundary values
      { "500101", '+', "18500101" },      // Minimum valid six digit date, January 1, 1850
      { "991231", '+', "18991231" },      // December 31, 1899
      { "000101", '+', "19000101" },      // January 1, 1900
      { "991231", '-', "19991231" },      // December 31 1999
      { "000101", '-', "20000101" },      // January 1, 2000
      { "491231", '-', "20491231" },      // Maximum valid six digit date, December 31, 2049

      // Month max days
      { "010131", '-', "20010131" },      // Max day of month January 2001
      { "910228", '+', "18910228" },      // Max day of month February 1891 (non-leap year)
      { "960229", '+', "18960229" },      // Max day of month February 1896 (leap year)
      { "000228", '+', "19000228" },      // Max day of month February 1900 (non-leap year)
      { "520229", '-', "19520229" },      // Max day of month February 1952 (leap year)
      { "000229", '-', "20000229" },      // Max day of month February 2000 (leap year, century divisible by 400)
      { "010228", '-', "20010228" },      // Max day of month February 2001 (non-leap year)
      { "530331", '+', "18530331" },      // Max day of month March 1853
      { "080430", '-', "20080430" },      // Max day of month April 2008
      { "150531", '+', "19150531" },      // Max day of month May 1915
      { "600630", '-', "19600630" },      // Max day of month June 1960
      { "750731", '+', "18750731" },      // Max day of month July 1875
      { "810831", '-', "19810831" },      // Max day of month August 1981
      { "090930", '+', "19090930" },      // Max day of month September 1909
      { "101031", '-', "20101031" },      // Max day of month October 2010
      { "111130", '+', "19111130" },      // Max day of month November 1911
      { "121231", '-', "20121231" },      // Max day of month December 2012

      // Min/max/boundary values
      { "18000101", '+', "18000101" },    // Minimum valid eight digit date, January 1, 1800
      { "18991231", '+', "18991231" },    // December 31, 1899
      { "19000101", '+', "19000101" },    // January 1, 1900
      { "19991231", '-', "19991231" },    // December 31 1999
      { "20000101", '-', "20000101" },    // January 1, 2000
      { "20991231", '-', "20991231" },    // Maximum valid six digit date, December 31, 2099

      // Month max days
      { "20010131", '-', "20010131" },    // Max day of month January 2001
      { "18910228", '+', "18910228" },    // Max day of month February 1891 (non-leap year)
      { "18960229", '+', "18960229" },    // Max day of month February 1896 (leap year)
      { "19000228", '+', "19000228" },    // Max day of month February 1900 (non-leap year)
      { "19520229", '-', "19520229" },    // Max day of month February 1952 (leap year)
      { "20000229", '-', "20000229" },    // Max day of month February 2000 (leap year, century divisible by 400)
      { "20010228", '-', "20010228" },    // Max day of month February 2001 (non-leap year)
      { "18530331", '+', "18530331" },    // Max day of month March 1853
      { "20080430", '-', "20080430" },    // Max day of month April 2008
      { "19150531", '+', "19150531" },    // Max day of month May 1915
      { "19600630", '-', "19600630" },    // Max day of month June 1960
      { "18750731", '+', "18750731" },    // Max day of month July 1875
      { "19810831", '-', "19810831" },    // Max day of month August 1981
      { "19090930", '+', "19090930" },    // Max day of month September 1909
      { "20101031", '-', "20101031" },    // Max day of month October 2010
      { "19111130", '+', "19111130" },    // Max day of month November 1911
      { "20121231", '-', "20121231" },    // Max day of month December 2012
   };

   public static TheoryData<String, Char, String> SamordningsnummerValidDateOfBirthValues = new()
   {
      // Min/max/boundary values, short format
      { "500161", '+', "18500101" },      // Minimum valid six digit date, January 1, 1850
      { "991291", '+', "18991231" },      // December 31, 1899
      { "000161", '+', "19000101" },      // January 1, 1900
      { "991291", '-', "19991231" },      // December 31 1999
      { "000161", '-', "20000101" },      // January 1, 2000
      { "491291", '-', "20491231" },      // Maximum valid six digit date, December 31, 2049

      // Month max days, short format
      { "010191", '-', "20010131" },      // Max day of month January 2001
      { "910288", '+', "18910228" },      // Max day of month February 1891 (non-leap year)
      { "960289", '+', "18960229" },      // Max day of month February 1896 (leap year)
      { "000288", '+', "19000228" },      // Max day of month February 1900 (non-leap year)
      { "520289", '-', "19520229" },      // Max day of month February 1952 (leap year)
      { "000289", '-', "20000229" },      // Max day of month February 2000 (leap year, century divisible by 400)
      { "010288", '-', "20010228" },      // Max day of month February 2001 (non-leap year)
      { "530391", '+', "18530331" },      // Max day of month March 1853
      { "080490", '-', "20080430" },      // Max day of month April 2008
      { "150591", '+', "19150531" },      // Max day of month May 1915
      { "600690", '-', "19600630" },      // Max day of month June 1960
      { "750791", '+', "18750731" },      // Max day of month July 1875
      { "810891", '-', "19810831" },      // Max day of month August 1981
      { "090990", '+', "19090930" },      // Max day of month September 1909
      { "101091", '-', "20101031" },      // Max day of month October 2010
      { "111190", '+', "19111130" },      // Max day of month November 1911
      { "121291", '-', "20121231" },      // Max day of month December 2012

      // Min/max/boundary values, long format
      { "18000161", '+', "18000101" },    // Minimum valid eight digit date, January 1, 1800
      { "18991291", '+', "18991231" },    // December 31, 1899
      { "19000161", '+', "19000101" },    // January 1, 1900
      { "19991291", '-', "19991231" },    // December 31 1999
      { "20000161", '-', "20000101" },    // January 1, 2000
      { "20991291", '-', "20991231" },    // Maximum valid eight digit date, December 31, 2099

      // Month max days, long format
      { "20010191", '-', "20010131" },    // Max day of month January 2001
      { "18910288", '+', "18910228" },    // Max day of month February 1891 (non-leap year)
      { "18960289", '+', "18960229" },    // Max day of month February 1896 (leap year)
      { "19000288", '+', "19000228" },    // Max day of month February 1900 (non-leap year)
      { "19520289", '-', "19520229" },    // Max day of month February 1952 (leap year)
      { "20000289", '-', "20000229" },    // Max day of month February 2000 (leap year, century divisible by 400)
      { "20010288", '-', "20010228" },    // Max day of month February 2001 (non-leap year)
      { "18530391", '+', "18530331" },    // Max day of month March 1853
      { "20080490", '-', "20080430" },    // Max day of month April 2008
      { "19150591", '+', "19150531" },    // Max day of month May 1915
      { "19600690", '-', "19600630" },    // Max day of month June 1960
      { "18750791", '+', "18750731" },    // Max day of month July 1875
      { "19810891", '-', "19810831" },    // Max day of month August 1981
      { "19090990", '+', "19090930" },    // Max day of month September 1909
      { "20101091", '-', "20101031" },    // Max day of month October 2010
      { "19111190", '+', "19111130" },    // Max day of month November 1911
      { "20121291", '-', "20121231" },    // Max day of month December 2012
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

   public static TheoryData<String, Char> PersonnummerInvalidDateOfBirthValues = new()
   {
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
      { "17991231", '+' },    // Invalid year (too low)
      { "21000101", '-' },    // Invalid year (too high)

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
   };

   public static TheoryData<String, Char> SamordningsnummerInvalidDateOfBirthValues = new()
   {
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
      { "17991291", '+' },    // Invalid year (too low)
      { "21000161", '-' },    // Invalid year (too high)

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

   public static TheoryData<Int32, Int32, Char> FormatValueTestData = new()
   {
      // Year offset, day offset, expected separator
      { -150,  0, '-' },
      {  -50,  0, '-' },
      {   50,  0, '-' },
      {  100, -1, '-' },
      {  100,  0, '+' },
      {  100,  1, '+' },
      {  150,  0, '+' },
   };

   protected static String GetValueWithValidCheckDigit(
      String dateOfBirth = "811228",
      Char separator = '-',
      String birthSerialNumber = "987")
   {
      var partialValue = $"{dateOfBirth[^6..]}{birthSerialNumber}";
      _ = Algorithms.Luhn.TryCalculateCheckDigit(partialValue, out var checkDigit);

      return $"{dateOfBirth}{separator}{birthSerialNumber}{checkDigit}";
   }

   protected static String GetNormalizedValue(String value)
   {
      if (value.Length == 13)
      {
         return value[..8] + value[^4..];
      }

      var year = ((value[0] - Chars.DigitZero) * 10)
                 + (value[1] - Chars.DigitZero);
      year = CenturyCutoff.DefaultInstance.ToFourDigitYear(year);
      if (value[6] == Chars.Plus)
      {
         year -= 100;
      }

      return $"{year}{value[2..6]}{value[^4..]}";
   }
}
