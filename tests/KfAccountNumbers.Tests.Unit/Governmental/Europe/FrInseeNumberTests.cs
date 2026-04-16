// Ignore Spelling: Insee

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

public class FrInseeNumberTests
{
   private const String Valid15CharacterInseeNumber = "188121884813236";
   private const String AltValid15CharacterInseeNumber = "255102445387701";
   private const String Valid15CharacterInseeNumberCorsica = "112072A28806058";
   private const String AltValid15CharacterInseeNumberCorsica = "112072B28806085";
   private const String Valid15CharacterTemporaryInseeNumber = "821099901013371";
   private const String Valid21CharacterInseeNumber = "1 88 12 18 848 132 36";
   private const String AltValid21CharacterInseeNumber = "2 55 10 24 453 877 01";
   private const String Valid21CharacterInseeNumberCorsica = "1 12 07 2A 288 060 58";
   private const String AltValid21CharacterInseeNumberCorsica = "1 12 07 2B 288 060 85";
   private const String Valid21CharacterTemporaryInseeNumber = "8-21-09-99-010-133-71";

   private static String GetInseeWithValidCheckDigits(
      Char gender = Chars.DigitOne,
      String year = "92",
      String month = "06",
      String department = "24",
      String commune = "549",
      String registrationOrder = "437",
      Boolean formatted = false)
   {
      var effectiveDepartment = department switch
      {
         "2A" => "19",
         "2B" => "18",
         _ => department
      };
      var effectiveCommune = department.Length == 3 ? commune[..2] : commune;    // Ensure default 3 char commune is valid for overseas departments
      var work = $"{gender}{year}{month}{effectiveDepartment}{effectiveCommune}{registrationOrder}";
      var checkSum = GetCheckSum(work);

      var insee = $"{gender}{year}{month}{department}{effectiveCommune}{registrationOrder}{checkSum:D2}";

      return formatted
         ? $"{insee[0]} {insee[1..3]} {insee[3..5]} {insee[5..7]} {insee[7..10]} {insee[10..13]} {insee[13..]}"
         : insee;
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

   public static TheoryData<String> ValidInseeNumbers =>
   [
      Valid15CharacterInseeNumber,
      AltValid15CharacterInseeNumber,
      Valid15CharacterInseeNumberCorsica,
      AltValid15CharacterInseeNumberCorsica,
      Valid15CharacterTemporaryInseeNumber,
      Valid21CharacterInseeNumber,
      AltValid21CharacterInseeNumber,
      Valid21CharacterInseeNumberCorsica,
      AltValid21CharacterInseeNumberCorsica,
      Valid21CharacterTemporaryInseeNumber,

      "295109912611193",
   ];

   public static TheoryData<Char> ValidGenders =>
   [
      Chars.DigitOne,
      Chars.DigitTwo,
      Chars.DigitSeven,
      Chars.DigitEight,
   ];

   public static TheoryData<String, Boolean> ValidDepartmentCodes
   {
      get
      {
         var data = new TheoryData<String, Boolean>();
         var formatOptions = new Boolean[] { true, false };
         var departmentCodes = FrDepartmentCodes.GetAllDepartmentCodes();

         foreach(var opt in formatOptions)
         {
            foreach(var code in departmentCodes)
            {
               data.Add(code, opt);
            }
         }

         return data;
      }
   }

   public static TheoryData<String, Boolean> ValidMonths = new()
   {
      { "01", false },
      { "12", false },
      { "13", false },
      { "20", false },
      { "42", false },
      { "50", false },
      { "99", false },

      { "01", true },
      { "12", true },
      { "13", true },
      { "20", true },
      { "42", true },
      { "50", true },
      { "99", true },
   };

   public static TheoryData<String> InvalidLengthValues =>
   [
      "18812188481326",                // Length 14
      "1881218848132613",              // Length 16
      "1 88 12 18 848 132 6",          // Length 20
      "1 88 12 18 848 132 613",        // Length 22
      new String('1', 100)    // Very long string
   ];

   public static TheoryData<String> InvalidCharacterValues =>
   [
      "A88121884813236",               // Non-digit character 'A'
      "1 8121884813236",               // Non-digit character ' '
      "18-121884813236",               // Non-digit character '-'
      "188=21884813236",               // Non-digit character '='
      "1881B1884813236",               // Non-digit character 'B'
      "18812C884813236",               // Non-digit character 'C'
      "188121a84813236",               // Non-digit character 'a' - 'A' would be valid in this location but not 'a'
      "1881218b4813236",               // Non-digit character 'b'
      "18812188~813236",               // Non-digit character '~'
      "188121884\u215313236",          // Non-digit character Unicode fraction 1/3
      "1881218848\u00D63236",          // Invalid character unicode O with umlaut
      "18812188481A236",               // Non-digit character 'A'
      "188121884813 36",               // Non-digit character ' '
      "1881218848132-6",               // Non-digit character '-'
      "18812188481323=",               // Non-digit character '='

      "A 88 12 18 848 132 36",         // Non-digit character 'A'
      "1  8 12 18 848 132 36",         // Non-digit character ' '
      "1 8- 12 18 848 132 36",         // Non-digit character '-'
      "1 88 =2 18 848 132 36",         // Non-digit character '='
      "1 88 1B 18 848 132 36",         // Non-digit character 'B'
      "1 88 12 C8 848 132 36",         // Non-digit character 'C'
      "1 88 12 1a 848 132 36",         // Non-digit character 'a' - 'A' would be valid in this location but not 'a'
      "1 88 12 18 b48 132 36",         // Non-digit character 'b'
      "1 88 12 18 8~8 132 36",         // Non-digit character '~'
      "1 88 12 18 84\u2153 132 36",    // Non-digit character Unicode fraction 1/3
      "1 88 12 18 848 \u00D632 36",    // Invalid character unicode O with umlaut
      "1 88 12 18 848 1A2 36",         // Non-digit character 'A'
      "1 88 12 18 848 13  36",         // Non-digit character ' '
      "1 88 12 18 848 132 -6",         // Non-digit character '-'
      "1 88 12 18 848 132 3=",         // Non-digit character '='
   ];

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "188121884812236",               // 188121884813236 with single digit transcription error, 3 -> 2
      "255102545387701",               // 255102445387701 with single digit transcription error, 4 -> 5
      "112072A28806059",               // 112072A28806058 with check digit transcription error, 8 -> 9
      "188128184813236",               // 188121884813236 with two digit transposition error, 18 -> 81
      "255102445387071",               // 255102445387701 with two digit transposition error, 70 -> 07
      "117022A28806058",               // 112072A28806058 with two digit jump transposition, 207 -> 702
      "188121994813236",               // 188121884813236 with two digit twin error, 88 -> 99
      "255102445388801",               // 255102445387701 with two digit twin error, 77 -> 88
      "112072A28806000",               // 112072A28806058 with invalid check digits -> 00
      "188121884813298",               // 188121884813236 with invalid check digits -> 98
      "255102445387799",               // 255102445387701 with invalid check digits -> 99

      "1 88 12 18 848 122 36",         // 188121884813236 with single digit transcription error, 3 -> 2
      "2 55 10 25 453 877 01",         // 255102445387701 with single digit transcription error, 4 -> 5
      "1 12 07 2A 288 060 59",         // 112072A28806058 with check digit transcription error, 8 -> 9
      "1 88 12 81 848 132 36",         // 188121884813236 with two digit transposition error, 18 -> 81
      "2 55 10 24 453 870 71",         // 255102445387701 with two digit transposition error, 70 -> 07
      "1 17 02 2A 288 060 58",         // 112072A28806058 with two digit jump transposition, 207 -> 702
      "1 88 12 19 948 132 36",         // 188121884813236 with two digit twin error, 88 -> 99
      "2 55 10 24 453 888 01",         // 255102445387701 with two digit twin error, 77 -> 88
      "1 12 07 2A 288 060 00",         // 112072A28806058 with invalid check digits -> 00
      "1 88 12 18 848 132 98",         // 188121884813236 with invalid check digits -> 98
      "2 55 10 24 453 877 99",         // 255102445387701 with invalid check digits -> 99
   ];

   public static TheoryData<String> InvalidSeparatorValues =>
   [
      "1088 12 18 848 132 36",
      "1188 12 18 848 132 36",
      "1288 12 18 848 132 36",
      "1388 12 18 848 132 36",
      "1488 12 18 848 132 36",
      "1588 12 18 848 132 36",
      "1688 12 18 848 132 36",
      "1788 12 18 848 132 36",
      "1888 12 18 848 132 36",
      "1988 12 18 848 132 36",

      "1 88012 18 848 132 36",
      "1 88 12018 848 132 36",
      "1 88 12 180848 132 36",
      "1 88 12 18 8480132 36",
      "1 88 12 18 848 132036",

      "1.88-12.18.848.132.36",
      "1 88 12.18 848 132 36",
      "1 88 12 18.848 132 36",
      "1 88 12 18 848.132 36",
      "1 88 12 18 848 132*36",
   ];

   public static TheoryData<Char> InvalidGenderValues =>
   [
      Chars.DigitThree,
      Chars.DigitFour,
      Chars.DigitFive,
      Chars.DigitSix,
      Chars.DigitNine,
   ];

   public static TheoryData<String, Boolean> InvalidMonthValues = new()
   {
      { "00", false },
      { "14", false },
      { "19", false },
      { "43", false },
      { "49", false },

      { "00", false },
      { "14", false },
      { "19", false },
      { "43", false },
      { "49", false },
   };

   public static TheoryData<String, Boolean> InvalidDepartmentCodes = new()
   {
      { "00", false },
      { "20", false },
      { "97", false },
      { "975", false },    // Invalid overseas department

      { "00", true },
      { "20", true },
      { "97", true },
      { "975", true },     // Invalid overseas department
   };

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidInseeNumbers))]
   public void FrInseeNumber_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
      => FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.ValidationPassed);

   [Theory]
   [MemberData(nameof(ValidGenders))]
   public void FrInseeNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidGender(Char gender)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(gender: gender);

      // Act/assert.
      FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.ValidationPassed);
   }

   [Theory]
   [MemberData(nameof(ValidMonths))]
   public void FrInseeNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidMonth(
      String month,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(month: month, formatted: formatted);

      // Act/assert.
      FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.ValidationPassed);
   }

   [Theory]
   [MemberData(nameof(ValidDepartmentCodes))]
   public void FrInseeNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidDepartmentCode(
      String department,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(department: department, formatted: formatted);

      // Act/assert.
      FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.ValidationPassed);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void FrInseeNumber_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
      => FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.Empty);

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
      => FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.InvalidLength);

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacterWhereDigitExpected(String value)
      => FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.InvalidCharacter);

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidCheckDigits_WhenValueHasInvalidCheckDigits(String value)
      => FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.InvalidCheckDigits);

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(String value)
      => FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.InvalidSeparator);

   [Theory]
   [MemberData(nameof(InvalidGenderValues))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidGender_WhenValueHasInvalidGender(Char gender)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(gender: gender);

      // Act/assert.
      FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.InvalidGender);
   }

   [Theory]
   [MemberData(nameof(InvalidMonthValues))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidGender_WhenValueHasInvalidMonth(
      String month,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(month: month, formatted: formatted);

      // Act/assert.
      FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.InvalidMonth);
   }

   [Theory]
   [MemberData(nameof(InvalidDepartmentCodes))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidDepartment_WhenValueHasInvalidMonth(
      String department,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(department: department, formatted: formatted);

      // Act/assert.
      FrInseeNumber.Validate(value).Should().Be(FrInseeNumberValidationResult.InvalidDepartment);
   }

   #endregion
}
