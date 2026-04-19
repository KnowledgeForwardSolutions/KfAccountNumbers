// Ignore Spelling: Nif

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

public class EsNifTests
{
   private const String Valid9CharacterDni = "12345678Z";
   private const String AltValid9CharacterDni = "50487563X";
   private const String Valid10CharacterDni = "12345678-Z";
   private const String AltValid10CharacterDni = "50487563 X";
   private const String Valid9CharacterNie = "X1234567L";
   private const String AltValid9CharacterNie = "Y7654321G";
   private const String Valid11CharacterNie = "X-1234567-L";
   private const String AltValid11CharacterNie = "Y 7654321 G";

   public static TheoryData<String> ValidNifValues =>
   [
      Valid9CharacterDni,
      AltValid9CharacterDni,
      Valid10CharacterDni,
      AltValid10CharacterDni,
      Valid9CharacterNie,
      AltValid9CharacterNie,
      Valid11CharacterNie,
      AltValid11CharacterNie,
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "2345678Z",             // DNI Length 8
      "012345678-Z",          // DNI Length 11
      "X123567L",             // NIE Length 8 
      "X1234567-L",           // NIE Length 10
      "X-01234567-L",         // NIE Length 12
      new String('1', 100)    // Very long string
   ];

   public static TheoryData<String> InvalidCharacterValues =>
   [
      "1 345678Z",               // Unformatted DNI, non-digit character ' '
      "1_345678Z",               // Unformatted DNI, non-digit character '-'
      "12=45678Z",               // Unformatted DNI, non-digit character '='
      "123A5678Z",               // Unformatted DNI, non-digit character 'A'
      "1234B678Z",               // Unformatted DNI, non-digit character 'B'
      "12345C78Z",               // Unformatted DNI, non-digit character 'C'
      "123456a8Z",               // Unformatted DNI, non-digit character 'a'
      "1234567bZ",               // Unformatted DNI, non-digit character 'b'
      "12345678~",               // Unformatted DNI, non-digit character '~'
      "12345678U",               // Unformatted DNI, invalid trailing character 'U'
      "12345678t",               // Unformatted DNI, invalid trailing character 't'
      "\u21532345678Z",          // Unformatted DNI, non-digit character Unicode fraction 1/3              
      "1\u00D6345678Z",          // Unformatted DNI, invalid character unicode O with umlaut               

      "1 345678-Z",              // Formatted DNI, non-digit character ' '
      "1_345678-Z",              // Formatted DNI, non-digit character '-'
      "12=45678-Z",              // Formatted DNI, non-digit character '='
      "123A5678-Z",              // Formatted DNI, non-digit character 'A'
      "1234B678-Z",              // Formatted DNI, non-digit character 'B'
      "12345C78-Z",              // Formatted DNI, non-digit character 'C'
      "123456a8-Z",              // Formatted DNI, non-digit character 'a'
      "1234567b Z",              // Formatted DNI, non-digit character 'b'
      "12345678 ~",              // Formatted DNI, non-digit character '~'
      "12345678 U",              // Formatted DNI, invalid trailing character 'U'
      "12345678 t",              // Formatted DNI, invalid trailing character 't'
      "1\u2153345678 Z",         // Formatted DNI, non-digit character Unicode fraction 1/3              
      "1\u00D6345678 Z",         // Formatted DNI, invalid character unicode O with umlaut               

      "A1234567L",               // Unformatted NIE, invalid leading character 'A'
      "W1234567L",               // Unformatted NIE, invalid leading character 'B'
      "x1234567L",               // Unformatted NIE, invalid leading character 'x'
      "y1234567L",               // Unformatted NIE, invalid leading character 'y'
      "z1234567L",               // Unformatted NIE, invalid leading character 'z'
      "X_234567L",               // Unformatted NIE, non-digit character '-'
      "X1=34567L",               // Unformatted NIE, non-digit character '='
      "X12A4567L",               // Unformatted NIE, non-digit character 'A'
      "X123B567L",               // Unformatted NIE, non-digit character 'B'
      "X1234C67L",               // Unformatted NIE, non-digit character 'C'
      "X12345a7L",               // Unformatted NIE, non-digit character 'a'
      "X123456bL",               // Unformatted NIE, non-digit character 'b'
      "X1234567~",               // Unformatted NIE, non-digit character '~'
      "X1234567U",               // Unformatted NIE, invalid trailing character 'U'
      "X1234567t",               // Unformatted NIE, invalid trailing character 't'
      "\u21531234567L",          // Unformatted NIE, non-digit character Unicode fraction 1/3              
      "X\u00D6234567L",          // Unformatted NIE, invalid character unicode O with umlaut               

      "A-1234567 L",             // Formatted NIE, invalid leading character 'A'
      "W-1234567 L",             // Formatted NIE, invalid leading character 'B'
      "x-1234567 L",             // Formatted NIE, invalid leading character 'x'
      "y-1234567 L",             // Formatted NIE, invalid leading character 'y'
      "z-1234567 L",             // Formatted NIE, invalid leading character 'z'
      "X-_234567 L",             // Formatted NIE, non-digit character '-'
      "X-1=34567 L",             // Formatted NIE, non-digit character '='
      "X-12A4567 L",             // Formatted NIE, non-digit character 'A'
      "X 123B567 L",             // Formatted NIE, non-digit character 'B'
      "X 1234C67 L",             // Formatted NIE, non-digit character 'C'
      "X 12345a7 L",             // Formatted NIE, non-digit character 'a'
      "X 123456b L",             // Formatted NIE, non-digit character 'b'
      "X 1234567 ~",             // Formatted NIE, non-digit character '~'
      "X 1234567 U",             // Formatted NIE, invalid trailing character 'U'
      "X 1234567 t",             // Formatted NIE, invalid trailing character 't'
      "\u2153 1234567 L",        // Formatted NIE, non-digit character Unicode fraction 1/3              
      "X \u00D6234567 L",        // Formatted NIE, invalid character unicode O with umlaut               
   ];

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidNifValues))]
   public void EsNif_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => EsNif.Validate(value).Should().Be(EsNifValidationResult.ValidationPassed);

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void EsNif_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => EsNif.Validate(value).Should().Be(EsNifValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void EsNif_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => EsNif.Validate(value).Should().Be(EsNifValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void EsNif_Validate_ShouldReturnInvalidCharacter_WhenValueHasInvalidCharacter(String value)
      => EsNif.Validate(value).Should().Be(EsNifValidationResult.InvalidCharacter);

   #endregion
}
