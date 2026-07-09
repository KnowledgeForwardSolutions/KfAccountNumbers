#pragma warning disable SA1122 // Use string.Empty for empty strings

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class NoIdentityNumberTestsBase
{
   // https://norske-testdata.no/fnr/ is one source for test fødselsnummers
   // https://verktoy.dev/en/personnummer/ is another source for test numbers
   protected const String ValidUnformattedFoedselsnummer = "13029597140";        // male, century of birth = 1900s
   protected const String AltValidUnformattedFoedselsnummer = "20050559433";     // female, century of birth = 2000s
   protected const String ValidFormattedFoedselsnummer = "130295 97140";
   protected const String AltValidFormattedFoedselsnummer = "200505-59433";

   protected const String ValidUnformattedDnummer = "60055029566";               // male, century of birth = 1900s
   protected const String AltValidUnformattedDnummer = "70100567871";            // female, century of birth = 2000s
   protected const String ValidFormattedDnummer = "600550 29566";
   protected const String AltValidFormattedDnummer = "701005-67871";

   public static TheoryData<String> ValidFoedselsnummerValues =>
   [
      ValidUnformattedFoedselsnummer,
      AltValidUnformattedFoedselsnummer,
      ValidFormattedFoedselsnummer,
      AltValidFormattedFoedselsnummer,
   ];

   public static TheoryData<String> ValidDNummerValues =>
   [
      ValidUnformattedDnummer,
      AltValidUnformattedDnummer,
      ValidFormattedDnummer,
      AltValidFormattedDnummer,
   ];

   public static TheoryData<String> ValidSeparators =>
   [
      " ",
      "-",
      "A",
      "z",
      "!",
   ];

   public static TheoryData<String, String, String, String> FoedselsnummerValidDateOfBirthValues = new()
   {
      // See class documentation on how the individual number and two digit year
      // are used to derive the four digit year. Note also that it is possible for
      // certain combinations to be invalid because of the modulus 11 check digit. In
      // those cases, the rows are duplicated and the day is adjusted in one and the
      // individual number is adjusted in the other

      // Minimum valid date = Jan 1, 1854
      // Maximum valid date = Dec 31, 2039

      // Rule 1 - individual number >= 500 and <= 749 and year >= 54 - century = 1800's
      { "010154",  "", "500", "18540101" },     // minimum 6 digit date and minimum individual number
      { "010154", " ", "749", "18540101" },     // minimum 6 digit date and maximum individual number
      { "311299",  "", "500", "18991231" },     // maximum 6 digit date and minimum individual number
      { "301299", " ", "749", "18991230" },     // maximum 6 digit date and maximum individual number (311299 749 adjusted because of check digits)
      { "311299",  "", "748", "18991231" },     // maximum 6 digit date and maximum individual number  "

      // Rule 2 - individual number < 500, year not considered - century = 1900's
      { "020100",  "", "000", "19000102" },     // minimum 6 digit date and minimum individual number (010100 000 adjusted because of check digits)
      { "010100", " ", "001", "19000101" },     // minimum 6 digit date and minimum individual number  "
      { "010100",  "", "499", "19000101" },     // minimum 6 digit date and maximum individual number
      { "301299", " ", "000", "19991230" },     // maximum 6 digit date and minimum individual number (311299 000 adjusted because of check digits)
      { "311299",  "", "001", "19991231" },     // maximum 6 digit date and minimum individual number  "
      { "311299", " ", "499", "19991231" },     // maximum 6 digit date and maximum individual number

      // Rule 3 - individual number >= 900 and year >= 40 - century = 1900's
      { "010140",  "", "900", "19000101" },     // minimum 6 digit date and minimum individual number
      { "010140",  "", "999", "19000101" },     // minimum 6 digit date and maximum individual number
      { "311299",  "", "900", "19991231" },     // maximum 6 digit date and minimum individual number
      { "301299",  "", "999", "19991230" },     // maximum 6 digit date and maximum individual number (311299 999 adjusted because of check digits)
      { "311299",  "", "998", "19991231" },     // maximum 6 digit date and maximum individual number  "

      // Rule 4 - individual number >= 500 and year <= 39 - century - 2000's
      { "010100",  "", "500", "20000101" },     // minimum 6 digit date and minimum individual number
      { "010100",  "", "999", "20000101" },     // minimum 6 digit date and maximum individual number
      { "311239",  "", "500", "20391231" },     // maximum 6 digit date and minimum individual number
      { "311239",  "", "999", "20391231" },     // maximum 6 digit date and maximum individual number

      // Month maximum days
      { "310104",  "", "501", "19040131" },     // maximum days for January, any year
      { "280201",  "", "234", "19040228" },     // maximum days for February, non-leap year
      { "290204",  "", "234", "19040229" },     // maximum days for February, leap year
      { "290200",  "", "500", "20000229" },     // maximum days for February, leap year (2000 is leap-year)
      { "310304",  "", "501", "20040331" },     // maximum days for March, any year
      { "300404",  "", "499", "19040431" },     // maximum days for April, any year
      { "310504",  "", "501", "20040531" },     // maximum days for May, any year
      { "300604",  "", "500", "20040631" },     // maximum days for June, any year
      { "310704",  "", "501", "20040731" },     // maximum days for July, any year
      { "310804",  "", "501", "20040831" },     // maximum days for August, any year
      { "300904",  "", "500", "20040931" },     // maximum days for September, any year
      { "311004",  "", "501", "20041031" },     // maximum days for October, any year
      { "301104",  "", "499", "19041131" },     // maximum days for November, any year
      { "311204",  "", "500", "20041231" },     // maximum days for December, any year
   };

   public static TheoryData<String, String, String, String> DnummerValidDateOfBirthValues = new()
   {
      // Date of birth, separator, individual number, expected date
      { "410100",  "", "001", "19000101" },     // Minimum date that can be represented as a D-nummer
      { "410100", " ", "499", "19000101" },
      { "711299",  "", "001", "19991231" },
      { "711299", " ", "499", "19991231" },
      { "410100",  "", "500", "20000101" },
      { "410100", " ", "999", "20000101" },
      { "711239",  "", "500", "20391231" },     // Maximum date that can be represented as a D-nummer
      { "711239", " ", "999", "20391231" },

      // Month maximum days
      { "710104",  "", "002", "19040131" },     // maximum days for January, any year
      { "680201", " ", "499", "19010228" },     // maximum days for February, non-leap year
      { "690204",  "", "500", "20040229" },     // maximum days for February, leap year
      { "690200", " ", "998", "20000229" },     // maximum days for February, leap year (2000 is leap-year)
      { "710304",  "", "001", "19040331" },     // maximum days for March, any year
      { "700404", " ", "499", "19040430" },     // maximum days for April, any year
      { "710504",  "", "501", "20040531" },     // maximum days for May, any year
      { "700604", " ", "999", "20040630" },     // maximum days for June, any year
      { "710704",  "", "001", "19040731" },     // maximum days for July, any year
      { "710804", " ", "499", "19040831" },     // maximum days for August, any year
      { "700904",  "", "500", "20040930" },     // maximum days for September, any year
      { "711004", " ", "998", "20041031" },     // maximum days for October, any year
      { "701104",  "", "002", "19041130" },     // maximum days for November, any year
      { "711204", " ", "500", "20041231" },     // maximum days for December, any year
   };

   public static TheoryData<String> InvalidLengthValues =>
   [
      "1006127070",        // Length 10
      "100612-707079",     // Length 14
      new String('1', 100) // Very long string
   ];

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted values
      { ".0061270707", 0 },         // Non-digit character '.'
      { "1 061270707", 1 },         // Non-digit character ' '
      { "10A61270707", 2 },         // Non-digit character 'A'
      { "100Z1270707", 3 },         // Non-digit character 'Z'
      { "1006^270707", 4 },         // Non-digit character '^'
      { "10061a70707", 5 },         // Non-digit character 'a'
      { "100612z0707", 6 },         // Non-digit character 'z'
      { "1006127~707", 7 },         // Non-digit character '~'
      { "10061270\u215307", 8 },    // Non-digit character Unicode fraction 1/3
      { "100612707\u00D67", 9 },    // Invalid character unicode O with umlaut
      { "1006127070\u0BE6", 10 },   // Invalid character unicode Tamil digit 0

      // Formatted values
      { ".00612 70707", 0 },        // Non-digit character '.'
      { "1 0612 70707", 1 },        // Non-digit character ' '
      { "10A612 70707", 2 },        // Non-digit character 'A'
      { "100Z12 70707", 3 },        // Non-digit character 'Z'
      { "1006^2 70707", 4 },        // Non-digit character '^'
      { "10061a 70707", 5 },        // Non-digit character 'a'
      { "100612 z0707", 7 },        // Non-digit character 'z'
      { "100612-7~707", 8 },        // Non-digit character '~'
      { "100612-70\u215307", 9 },   // Non-digit character Unicode fraction 1/3
      { "100612-707\u00D67", 10 },  // Invalid character unicode O with umlaut
      { "100612-7070\u0BE6", 11 },  // Invalid character unicode Tamil digit 0
   };

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      // Unformatted values
      "13039597140",          // 13029597140 with single digit transcription error 2 -> 3
      "20055025972",          // 20055029572 with two digit transposition error 95 -> 59
      "21072551149",          // 21072441149 with two digit twin error 44 -> 55
      "16072996853",          // 16079926853 with jump transposition 992 -> 299
      "13072101223",          // 13072101213 with invalid first check digit 1 -> 2
      "11085149989",          // 11085149980 with invalid second check digit 0 -> 9

      // Formatted values
      "130395 97140",         // 130295 97140 with single digit transcription error 2 -> 3
      "200550 25972",         // 200550 29572 with two digit transposition error 95 -> 59
      "210725 51149",         // 210724 41149 with two digit twin error 44 -> 55
      "160729 96853",         // 160799 26853 with jump transposition 992 -> 299
      "130721-01223",         // 130721-01213 with invalid first check digit 1 -> 2
      "110851-49989",         // 110851-49980 with invalid second check digit 0 -> 9
   ];

   public static TheoryData<String> InvalidSeparators =>
   [
      "0",
      "1",
      "2",
      "3",
      "4",
      "5",
      "6",
      "7",
      "8",
      "9",
   ];

   public static TheoryData<String, String> DnummerInvalidDateOfBirthValues = new()
   {
      // Individual number < 500 = 1900's, >= 500 = 2000s.
      { "410140", "600" },       // January 1, 2040, > max year 2039

      // Invalid months
      { "410004", "200" },       // Invalid month = 0
      { "411304", "500" },       // Invalid month = 13

      // Invalid days
      { "400104", "100" },       // Invalid day = 0
      { "720104", "100" },       // Invalid day of month for January, any year
      { "690201", "100" },       // Invalid day of for February, non-leap year
      { "700204", "100" },       // Invalid day of for February, leap year
      { "700200", "500" },       // Invalid day of for February, leap year (2000 is leap-year)
      { "720304", "500" },       // Invalid day of for March, any year
      { "710404", "500" },       // Invalid day of for April, any year
      { "720504", "500" },       // Invalid day of for May, any year
      { "710604", "500" },       // Invalid day of for June, any year
      { "720704", "500" },       // Invalid day of for July, any year
      { "720804", "500" },       // Invalid day of for August, any year
      { "710904", "100" },       // Invalid day of for September, any year
      { "721004", "200" },       // Invalid day of for October, any year
      { "711104", "100" },       // Invalid day of for November, any year
      { "721204", "200" },       // Invalid day of for December, any year
   };

   protected static String GetValueWithValidCheckDigits(
      String dateOfBirth = "130295",
      String separator = "",
      String individualNumber = "971")
   {
      var d1 = dateOfBirth[0] - Chars.DigitZero;
      var d2 = dateOfBirth[1] - Chars.DigitZero;
      var d3 = dateOfBirth[2] - Chars.DigitZero;
      var d4 = dateOfBirth[3] - Chars.DigitZero;
      var d5 = dateOfBirth[4] - Chars.DigitZero;
      var d6 = dateOfBirth[5] - Chars.DigitZero;
      var i1 = individualNumber[0] - Chars.DigitZero;
      var i2 = individualNumber[1] - Chars.DigitZero;
      var i3 = individualNumber[2] - Chars.DigitZero;

      var sum1 = (3 * d1) + (7 * d2) + (6 * d3) + (1 * d4) + (8 * d5) + (9 * d6) + (4 * i1) + (5 * i2) + (2 * i3);
      var c1 = (11 - (sum1 % 11)) % 11;
      if (c1 == 10)
      {
         throw new InvalidOperationException("Invalid checksum");
      }

      var sum2 = (5 * d1) + (4 * d2) + (3 * d3) + (2 * d4) + (7 * d5) + (6 * d6) + (5 * i1) + (4 * i2) + (3 * i3) + (2 * c1);
      var c2 = (11 - (sum2 % 11)) % 11;
      return c2 == 10
         ? throw new InvalidOperationException("Invalid checksum")
         : $"{dateOfBirth}{separator}{individualNumber}{c1}{c2}";
   }

   protected static String GetNormalizedValue(String value)
      => value.Length == 11 ? value : value[..6] + value[7..];
}
