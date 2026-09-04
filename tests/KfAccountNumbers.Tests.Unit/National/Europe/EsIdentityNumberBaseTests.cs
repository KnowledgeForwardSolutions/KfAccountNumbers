namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class EsIdentityNumberBaseTests
{
   private const String ValidUnformattedDni = "12345678Z";
   private const String ValidUnformattedLowercaseDni = "12345678z";
   private const String AltValidUnformattedDni = "50487563X";
   private const String ValidFormattedDni = "12345678-Z";
   private const String ValidFormattedLowercaseDni = "12345678-z";
   private const String AltValidFormattedDni = "50487563 X";
   private const String ValidUnformattedNie = "X1234567L";
   private const String ValidUnformattedLowercaseNie = "x1234567l";
   private const String AltValidUnformattedNie = "Y7654321G";
   private const String ValidFormattedNie = "X-1234567-L";
   private const String ValidFormattedLowercaseNie = "x-1234567-l";
   private const String AltValidFormattedNie = "Y 7654321 G";

   protected static String GetNormalizedIdentifier(String value)
      => value.Length switch
      {
         9 => value.ToUpperInvariant(),
         10 => (value[..8] + value[^1]).ToUpperInvariant(),
         11 => (value[0] + value[2..9] + value[^1]).ToUpperInvariant(),
         _ => throw new InvalidOperationException(),
      };

   public static TheoryData<String> ValidDniValues =>
   [
      ValidUnformattedDni,
      ValidUnformattedLowercaseDni,
      AltValidUnformattedDni,
      ValidFormattedDni,
      ValidFormattedLowercaseDni,
      AltValidFormattedDni,
   ];

   public static TheoryData<String> ValidNieValues =>
   [
      ValidUnformattedNie,
      ValidUnformattedLowercaseNie,
      AltValidUnformattedNie,
      ValidFormattedNie,
      ValidFormattedLowercaseNie,
      AltValidFormattedNie,
   ];

   public static TheoryData<String> InvalidDniLengthValues =>
   [
      "2345678Z",             // DNI Length 8
      "1-2345678-Z",          // DNI Length 11
      new String('1', 100)    // Very long string
   ];

   public static TheoryData<String, Int32> InvalidDniCharacterValues = new()
   {
      // Unformatted DNI
      { ".2345678Z", 0 },          // Unformatted DNI, non-digit character '.'
      { "1 345678Z", 1 },          // Unformatted DNI, non-digit character ' '
      { "12A45678Z", 2 },          // Unformatted DNI, non-digit character 'A'
      { "123Z5678Z", 3 },          // Unformatted DNI, non-digit character 'Z'
      { "1234^678Z", 4 },          // Unformatted DNI, non-digit character '^'
      { "12345a78Z", 5 },          // Unformatted DNI, non-digit character 'a'
      { "123456z8Z", 6 },          // Unformatted DNI, non-digit character 'z'
      { "1234567~Z", 7 },          // Unformatted DNI, non-digit character '~'
      { "12345678\u0BE6", 8 },     // Unformatted DNI, invalid character unicode Tamil digit 0
      { "12345678I", 8 },          // Unformatted DNI, invalid trailing character 'I'
      { "12345678O", 8 },          // Unformatted DNI, invalid trailing character 'O'
      { "12345678U", 8 },          // Unformatted DNI, invalid trailing character 'U'
      { "12345678i", 8 },          // Unformatted DNI, invalid trailing character 'i'
      { "12345678o", 8 },          // Unformatted DNI, invalid trailing character 'o'
      { "12345678u", 8 },          // Unformatted DNI, invalid trailing character 'u'
      { "123456782", 8 },          // Unformatted DNI, invalid trailing character '2'
      { "\u21532345678Z", 0 },     // Unformatted DNI, non-digit character Unicode fraction 1/3
      { "1\u00D6345678Z", 1 },     // Unformatted DNI, invalid character unicode O with umlaut

      // Formatted DNI
      { ".2345678-Z", 0 },         // Formatted DNI, non-digit character '.'
      { "1 345678-Z", 1 },         // Formatted DNI, non-digit character ' '
      { "12A45678-Z", 2 },         // Formatted DNI, non-digit character 'A'
      { "123Z5678-Z", 3 },         // Formatted DNI, non-digit character 'Z'
      { "1234^678-Z", 4 },         // Formatted DNI, non-digit character '^'
      { "12345a78-Z", 5 },         // Formatted DNI, non-digit character 'a'
      { "123456z8-Z", 6 },         // Formatted DNI, non-digit character 'z'
      { "1234567~ Z", 7 },         // Formatted DNI, non-digit character '~'
      { "12345678 \u0BE6", 9 },    // Formatted DNI, invalid character unicode Tamil digit 0
      { "12345678 I", 9 },         // Formatted DNI, invalid trailing character 'I'
      { "12345678 O", 9 },         // Formatted DNI, invalid trailing character 'O'
      { "12345678 U", 9 },         // Formatted DNI, invalid trailing character 'U'
      { "12345678 i", 9 },         // Formatted DNI, invalid trailing character 'i'
      { "12345678 o", 9 },         // Formatted DNI, invalid trailing character 'o'
      { "12345678 u", 9 },         // Formatted DNI, invalid trailing character 'u'
      { "12345678 2", 9 },         // Formatted DNI, invalid trailing character '2'
      { "1\u2153345678 Z", 1 },    // Formatted DNI, non-digit character Unicode fraction 1/3
      { "1\u00D6345678 Z", 1 },    // Formatted DNI, invalid character unicode O with umlaut
   };

   public static TheoryData<String, Int32> InvalidNieCharacterValues = new()
   {
      // Unformatted NIE
      { "A1234567L", 0 },          // Unformatted NIE, invalid leading character 'A'
      { "w1234567L", 0 },          // Unformatted NIE, invalid leading character 'w'
      { "a1234567L", 0 },          // Unformatted NIE, invalid leading character 'a'
      { "X 234567L", 1 },          // Unformatted NIE, non-digit character ' '
      { "X1A34567L", 2 },          // Unformatted NIE, non-digit character 'A'
      { "X12Z4567L", 3 },          // Unformatted NIE, non-digit character 'Z'
      { "X123^567L", 4 },          // Unformatted NIE, non-digit character '^'
      { "X1234a67L", 5 },          // Unformatted NIE, non-digit character 'a'
      { "X12345z7L", 6 },          // Unformatted NIE, non-digit character 'z'
      { "X123456~L", 7 },          // Unformatted NIE, non-digit character '~'
      { "X1234567\u0BE6", 8 },     // Unformatted NIE, invalid character unicode Tamil digit 0
      { "X1234567I", 8 },          // Unformatted NIE, invalid trailing character 'I'
      { "X1234567O", 8 },          // Unformatted NIE, invalid trailing character 'O'
      { "X1234567U", 8 },          // Unformatted NIE, invalid trailing character 'U'
      { "X1234567i", 8 },          // Unformatted NIE, invalid trailing character 'i'
      { "X1234567o", 8 },          // Unformatted NIE, invalid trailing character 'o'
      { "X1234567u", 8 },          // Unformatted NIE, invalid trailing character 'u'
      { "X12345672", 8 },          // Unformatted NIE, invalid trailing character '2'
      { "\u21531234567L", 0 },     // Unformatted NIE, non-digit character Unicode fraction 1/3
      { "X\u00D6234567L", 1 },     // Unformatted NIE, invalid character unicode O with umlaut

      // Formatted NIE
      { "A-1234567-L", 0 },         // Formatted NIE, invalid leading character 'A'
      { "w-1234567-L", 0 },         // Formatted NIE, invalid leading character 'W'
      { "a-1234567-L", 0 },         // Formatted NIE, invalid leading character 'a'
      { ".-1234567-L", 0 },         // Formatted NIE, non-digit character '.'
      { "X- 234567-L", 2 },         // Formatted NIE, non-digit character ' '
      { "X-1A34567-L", 3 },         // Formatted NIE, non-digit character 'A'
      { "X-12Z4567-L", 4 },         // Formatted NIE, non-digit character 'Z'
      { "X 123^567 L", 5 },         // Formatted NIE, non-digit character '^'
      { "X 1234a67 L", 6 },         // Formatted NIE, non-digit character 'a'
      { "X 12345z7 L", 7 },         // Formatted NIE, non-digit character 'z'
      { "X 123456~ L", 8 },         // Formatted NIE, non-digit character '~'
      { "X 1234567 \u0BE6", 10 },   // Formatted NIE, invalid character unicode Tamil digit 0
      { "X 1234567 U", 10 },        // Formatted NIE, invalid trailing character 'U'
      { "X 1234567 I", 10 },        // Formatted NIE, invalid trailing character 'I'
      { "X 1234567 O", 10 },        // Formatted NIE, invalid trailing character 'O'
      { "X 1234567 U", 10 },        // Formatted NIE, invalid trailing character 'U'
      { "X 1234567 i", 10 },        // Formatted NIE, invalid trailing character 'i'
      { "X 1234567 o", 10 },        // Formatted NIE, invalid trailing character 'o'
      { "X 1234567 u", 10 },        // Formatted NIE, invalid trailing character 'u'
      { "X 1234567 2", 10 },        // Formatted NIE, invalid trailing character '2'
      { "\u2153 1234567 L", 0 },    // Formatted NIE, non-digit character Unicode fraction 1/3
      { "X \u00D6234567 L", 2 },    // Formatted NIE, invalid character unicode O with umlaut
   };

   public static TheoryData<String> InvalidDniCheckDigitValues =>
   [
      "12245678Z",             // 12345678Z with single digit transcription error, 3 -> 2
      "50587563X",             // 50487563X with single digit transcription error, 4 -> 5
      "11223344C",             // 11223344B with check digit transcription error, B -> C
      "12354678z",             // 12345678z with two digit transposition error, 45 -> 54
      "50487653X",             // 50487563X with two digit transposition error, 56 -> 65
      "50784563X",             // 50487563X with two digit jump transposition, 487 -> 784
      "11224444b",             // 11223344b with two digit twin error, 33 -> 44
      "22223344B",             // 11223344B with two digit twin error, 11 -> 22

      "12245678-Z",            // 12345678Z with single digit transcription error, 3 -> 2
      "50587563-X",            // 50487563X with single digit transcription error, 4 -> 5
      "11223344-C",            // 11223344B with check digit transcription error, B -> C
      "12354678-Z",            // 12345678Z with two digit transposition error, 45 -> 54
      "50487653 X",            // 50487563X with two digit transposition error, 56 -> 65
      "50784563 X",            // 50487563X with two digit jump transposition, 487 -> 784
      "11224444 B",            // 11223344B with two digit twin error, 33 -> 44
      "22223344 B",            // 11223344B with two digit twin error, 11 -> 22
   ];

   public static TheoryData<String> InvalidNieCheckDigitValues =>
   [
      "X1224567L",             // X1234567L with single digit transcription error, 3 -> 2
      "Y7655321G",             // Y7654321G with single digit transcription error, 4 -> 5
      "X1122334B",             // X1122334A with check digit transcription error, A -> B
      "X1235467L",             // X1234567L with two digit transposition error, 45 -> 54
      "Y7564321G",             // Y7654321G with two digit transposition error, 65 -> 56
      "X1432567l",             // X1234567l with two digit jump transposition, 234 -> 432
      "X1122444A",             // X1122334A with two digit twin error, 33 -> 44
      "x2222334A",             // x1122334A with two digit twin error, 11 -> 22

      "X-1224567-L",           // X1234567L with single digit transcription error, 3 -> 2
      "Y-7655321-G",           // Y7654321G with single digit transcription error, 4 -> 5
      "X-1122334-B",           // X1122334A with check digit transcription error, A -> B
      "X-1235467-l",           // X1234567l with two digit transposition error, 45 -> 54
      "Y 7564321 G",           // Y7654321G with two digit transposition error, 65 -> 56
      "X 1432567 L",           // X1234567L with two digit jump transposition, 234 -> 432
      "X 1122444 A",           // X1122334A with two digit twin error, 33 -> 44
      "x 2222334 A",           // x1122334A with two digit twin error, 11 -> 22
   ];

   public static TheoryData<String, Int32> InvalidDniSeparatorValues = new()
   {
      // Digit separator
      { "123456780Z", 8 },
      { "123456781Z", 8 },
      { "123456782Z", 8 },
      { "123456783Z", 8 },
      { "123456784Z", 8 },
      { "123456785Z", 8 },
      { "123456786Z", 8 },
      { "123456787Z", 8 },
      { "123456788Z", 8 },
      { "123456789Z", 8 },

      // Letter separator
      { "12345678aZ", 8 },
      { "12345678bZ", 8 },
      { "12345678YZ", 8 },
      { "12345678ZZ", 8 },
   };

   public static TheoryData<String, Int32> InvalidNieSeparatorValues = new()
   {
      // Digit separator in first NIE separator location
      { "X01234567-L", 1 },
      { "X11234567-L", 1 },
      { "X21234567-L", 1 },
      { "X31234567-L", 1 },
      { "X41234567-L", 1 },
      { "X51234567-L", 1 },
      { "X61234567-L", 1 },
      { "X71234567-L", 1 },
      { "X81234567-L", 1 },
      { "X91234567-L", 1 },

      // Letter separator in first NIE separator location
      { "Xa1234567-L", 1 },
      { "Xb1234567-L", 1 },
      { "XY1234567-L", 1 },
      { "XZ1234567-L", 1 },

      // Digit separator in second NIE separator location
      { "X-12345670L", 9 },
      { "X-12345671L", 9 },
      { "X-12345672L", 9 },
      { "X-12345673L", 9 },
      { "X-12345674L", 9 },
      { "X-12345675L", 9 },
      { "X-12345676L", 9 },
      { "X-12345677L", 9 },
      { "X-12345678L", 9 },
      { "X-12345679L", 9 },

      // Letter separator in second NIE separator location
      { "X-1234567aL", 9 },
      { "X-1234567bL", 9 },
      { "X-1234567YL", 9 },
      { "X-1234567ZL", 9 },

      // Mixed separators in NIE
      { "X-1234567 L", 9 },
      { "X 1234567-L", 9 },
   };

}
