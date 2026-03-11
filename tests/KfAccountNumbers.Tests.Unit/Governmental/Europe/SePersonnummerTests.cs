// Ignore Spelling: Personnummer

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

public class SePersonnummerTests
{
   private const String Valid11CharacterDashPersonnummer = "811228-9874";        // From Wikipedia, https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
   private const String Valid11CharacterPlusPersonnummer = "811228+9874";
   private const String Valid13CharacterDashPersonnummer = "19670919-9530";
   private const String Valid13CharacterPlusPersonnummer = "20670919+9530";      // Future date, but valid format and checksum

   private static String GetPersonnummer(
      String dateOfBirth = "811228",
      Char separator = '-',
      String birthSerialNumber = "987",
      Char checkDigit = '4')
      => $"{dateOfBirth}{separator}{birthSerialNumber}{checkDigit}";

   public static TheoryData<String> ValidPersonnummerValues =>
   [
      Valid11CharacterDashPersonnummer,
      Valid11CharacterPlusPersonnummer,
      Valid13CharacterDashPersonnummer,
      Valid13CharacterPlusPersonnummer,
   ];

   public static TheoryData<String> InvalidLengthValues =>
   [
      "811228-987",        // Length 10
      "811228-98745",      // Length 12
      "19811228-98745",    // Length 14
   ];

   public static TheoryData<String> InvalidSixDigitDateOfBirthValues =>
   [
      "A10101",            // Non-digit character 'A'
      "0B0101",            // Non-digit character 'B'
      "01!101",            // Non-digit character '!'
      "0101 1",            // Non-digit character ' '
      "01010\u2153",       // Non-digit character Unicode fraction 1/3

      "010001",            // Invalid month (too low)
      "011301",            // Invalid month (too high)
      "010100",            // Invalid day of month (too low)
      "010132",            // Invalid day of month January
      "010229",            // Invalid day of month February (non leap year)
      "000229",            // Invalid day of month February (1900 is not a leap year)
      "040230",            // Invalid day of month February (leap year)
      "010332",            // Invalid day of month March
      "010431",            // Invalid day of month April
      "010532",            // Invalid day of month May
      "010631",            // Invalid day of month June
      "010732",            // Invalid day of month July
      "010832",            // Invalid day of month August
      "010931",            // Invalid day of month September
      "011032",            // Invalid day of month October
      "011131",            // Invalid day of month November
      "011232",            // Invalid day of month December
   ];

   public static TheoryData<String> InvalidEightDigitDateOfBirthValues =>
   [
      "a9811228",          // Non-digit character 'a'
      "1b811228",          // Non-digit character 'b'
      "1981A228",          // Non-digit character 'A'
      "19810B28",          // Non-digit character 'B'
      "198112!8",          // Non-digit character '!'
      "1981122 ",          // Non-digit character ' '
      "1981122\u2153",     // Non-digit character Unicode fraction 1/3

      "18811228",          // Invalid century (too low)
      "21811228",          // Invalid century (too high)
      "19810028",          // Invalid month (too low)
      "19811328",          // Invalid month (too high)
      "19810100",          // Invalid day of month (too low)
      "19810132",          // Invalid day of month January
      "19810229",          // Invalid day of month February (non leap year)
      "19000229",          // Invalid day of month February (1900 is not a leap year)
      "20040230",          // Invalid day of month February (leap year)
      "19810332",          // Invalid day of month March
      "19810431",          // Invalid day of month April
      "19810532",          // Invalid day of month May
      "20810631",          // Invalid day of month June
      "20810732",          // Invalid day of month July
      "20810832",          // Invalid day of month August
      "20810931",          // Invalid day of month September
      "20811032",          // Invalid day of month October
      "20811131",          // Invalid day of month November
      "20811232",          // Invalid day of month December
   ];

   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "811228*9874",
      "19811228*9874"
   ];

   public static TheoryData<String> InvalidBirthSerialNumberValues =>
   [
      "98A",               // Non-digit character 'A'
      "9B7",               // Non-digit character 'B'
      "98!",               // Non-digit character '!'
      "98 ",               // Non-digit character ' '
      "98\u2153",          // Non-digit character Unicode fraction 1/3
   ];

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "811228-9875",       // 811228-9874 with invalid check digit 4 -> 5
      "811227-9874",       // 811228-9874 with single digit transcription error 8 -> 7
      "821228-9874",       // 811228-9874 with single digit transcription error 1 -> 2
      "181228-9874",       // 811228-9874 with two digit transcription error 81 -> 18
      "811228-9847",       // 811228-9874 with two digit transcription error 74 -> 47
      "880422+1238",       // 880411+1238 wtih two digit twin error 11 -> 22
      "880411+3328",       // 880411+2228 with two digit twin error 22 -> 33
      "19811228-9875",     // 19811228-9874 with invalid check digit 4 -> 5
      "19811227-9874",     // 19811228-9874 with single digit transcription error 8 -> 7
      "20821228-9874",     // 20811228-9874 with single digit transcription error 1 -> 2
      "20181228-9874",     // 20811228-9874 with two digit transcription error 81 -> 18
      "19811228-9847",     // 19811228-9874 with two digit transcription error 74 -> 47
      "19880422+1238",     // 19880411+1238 wtih two digit twin error 11 -> 22
      "19880411+3328",     // 19880411+2228 with two digit twin error 22 -> 33
   ];

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidPersonnummerValues))]
   public void SePersonnummer_Validate_ShouldReturnValidationPassed_WhenFullPersonnummerValueIsValid(String personnummer)
      => SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.ValidationPassed);

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void SePersonnummer_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String personnummer)
      => SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String personnummer)
      => SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidSixDigitDateOfBirthValues))]
   [MemberData(nameof(InvalidEightDigitDateOfBirthValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(String dateOfBirth)
   {
      // Arrange.
      var personnummer = GetPersonnummer(dateOfBirth: dateOfBirth);

      // Act/assert.
      SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.InvalidDateOfBirth);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String personnummer)
      => SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.InvalidSeparator);

   [Theory]
   [MemberData(nameof(InvalidBirthSerialNumberValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidBirthSerialNumber_WhenValueHasInvalidBirthSerialNumber(String birthSerialNumber)
   {
      // Arrange.
      var personnummer = GetPersonnummer(birthSerialNumber: birthSerialNumber);

      // Act/assert.
      SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.InvalidBirthSerialNumber);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void SePersonnummer_Validate_ShouldReturnInvalidCheckDigit_WhenValueHasInvalidCheckDigit(String personnummer)
      => SePersonnummer.Validate(personnummer).Should().Be(SePersonnummerValidationResult.InvalidCheckDigit);

   #endregion
}
