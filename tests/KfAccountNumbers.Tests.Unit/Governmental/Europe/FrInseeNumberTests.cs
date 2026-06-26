// Ignore Spelling: Deserialize Deserialization Insee Json Kf

using LocalCreateResult = KfAccountNumbers.Results.UCreateResult<
   KfAccountNumbers.Governmental.Europe.FrInseeNumber,
   KfAccountNumbers.Governmental.Europe.FrInseeNumber.ValidationError>;
using LocalValidationError = KfAccountNumbers.Governmental.Europe.FrInseeNumber.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.Governmental.Europe.FrInseeNumber.ValidationError>;
using LocalValidationResult = KfAccountNumbers.Governmental.Europe.FrInseeNumber.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.Governmental.Europe;

#pragma warning disable IDE0008 // Use explicit type
#pragma warning disable IDE0058 // Expression value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible

public class FrInseeNumberTests
{
   private const String ValidUnformattedInseeNumber = "188121884813236";
   private const String AltValidUnformattedInseeNumber = "255102445387701";
   private const String ValidUnformattedInseeNumberCorsica = "112072A28806058";
   private const String AltValidUnformattedInseeNumberCorsica = "112072B28806085";
   private const String ValidUnformattedTemporaryInseeNumber = "821099901013371";
   private const String ValidFormattedInseeNumber = "1 88 12 18 848 132 36";
   private const String AltValidFormattedInseeNumber = "2 55 10 24 453 877 01";
   private const String ValidFormattedInseeNumberCorsica = "1 12 07 2A 288 060 58";
   private const String AltValidFormattedInseeNumberCorsica = "1 12 07 2B 288 060 85";
   private const String ValidFormattedTemporaryInseeNumber = "8-21-09-99-010-133-71";

   private static String GetInseeWithValidCheckDigits(
      Char gender = Chars.DigitOne,
      String year = "92",
      String month = "06",
      String department = "24",
      String commune = "549",
      String registrationOrder = "437",
      Boolean formatted = false)
   {
      commune = department.Length == 3 ? commune[..2] : commune;    // Ensure default 3 char commune is valid for overseas departments

      // Calculate the check sum by performing substitutions for Corsican department codes.
      var checksumDepartment = department switch
      {
         "2A" => "19",
         "2B" => "18",
         _ => department,
      };
      var work = $"{gender}{year}{month}{checksumDepartment}{commune}{registrationOrder}";
      var checkSum = GetCheckSum(work);

      // Construct the INSEE using the original department and the calculated checksum.
      var insee = $"{gender}{year}{month}{department}{commune}{registrationOrder}{checkSum:D2}";

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

   private static String GetRawInsee(String insee)
      => insee.Length == 15
      ? insee
      : insee[..1] + insee[2..4] + insee[5..7] + insee[8..10] + insee[11..14] + insee[15..18] + insee[19..];

   public static TheoryData<String> ValidInseeNumbers =>
   [
      ValidUnformattedInseeNumber,
      AltValidUnformattedInseeNumber,
      ValidUnformattedInseeNumberCorsica,
      AltValidUnformattedInseeNumberCorsica,
      ValidUnformattedTemporaryInseeNumber,
      ValidFormattedInseeNumber,
      AltValidFormattedInseeNumber,
      ValidFormattedInseeNumberCorsica,
      AltValidFormattedInseeNumberCorsica,
      ValidFormattedTemporaryInseeNumber,
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

         foreach (var opt in formatOptions)
         {
            foreach (var code in departmentCodes)
            {
               data.Add(code, opt);
            }
         }

         return data;
      }
   }

   public static TheoryData<String, Boolean> ValidMonths = new()
   {
      // Standard months
      { "01", false },
      { "12", false },

      // Special month codes for unknown/incomplete DOB
      { "13", false },     // Unknown month (birth month not known)
      { "20", false },     // Incomplete DOB range start
      { "42", false },     // Incomplete DOB range end
      { "50", false },     // Incomplete DOB range start (alternate)
      { "99", false },     // Incomplete DOB range end (maximum)

      // Standard months
      { "01", true },
      { "12", true },

      // Special month codes for unknown/incomplete DOB
      { "13", true },      // Unknown month (birth month not known)
      { "20", true },      // Incomplete DOB range start
      { "42", true },      // Incomplete DOB range end
      { "50", true },      // Incomplete DOB range start (alternate)
      { "99", true },      // Incomplete DOB range end (maximum)
   };

   public static TheoryData<String> InvalidLengthValues =>
   [
      "18812188481326",                // Length 14
      "1881218848132613",              // Length 16
      "1 88 12 18 848 132 6",          // Length 20
      "1 88 12 18 848 132 613",        // Length 22
      new String('1', 100)    // Very long string
   ];

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      // Unformatted values
      { ".88121884813236", 0 },              // Non-digit character '.'
      { "1 8121884813236", 1 },              // Non-digit character ' '
      { "18A121884813236", 2 },              // Non-digit character 'A'
      { "188Z21884813236", 3 },              // Non-digit character 'Z'
      { "1881^1884813236", 4 },              // Non-digit character '^'
      { "18812a884813236", 5 },              // Non-digit character 'a'
      { "188121z84813236", 6 },              // Non-digit character 'z'
      { "1881218~4813236", 7 },              // Non-digit character '~'
      { "18812188\u2153803236", 8 },         // Non-digit character Unicode fraction 1/3
      { "188121884\u00D603236", 9 },         // Invalid character unicode O with umlaut
      { "1881218848\u0BE63236", 10 },        // Invalid character unicode Tamil digit 0
      { "18812188481.236", 11 },             // Non-digit character '.'
      { "188121884813 36", 12 },             // Non-digit character ' '
      { "1881218848132A6", 13 },             // Non-digit character 'A'
      { "18812188481323Z", 14 },             // Non-digit character 'Z'

      // Formatted values
      { ". 88 12 18 848 132 36", 0 },        // Non-digit character '.'
      { "1  8 12 18 848 132 36", 2 },        // Non-digit character ' '
      { "1 8A 12 18 848 132 36", 3 },        // Non-digit character 'A'
      { "1 88 Z2 18 848 132 36", 5 },        // Non-digit character 'Z'
      { "1 88 1^ 18 848 132 36", 6 },        // Non-digit character '^'
      { "1 88 12 a8 848 132 36", 8 },        // Non-digit character 'a'
      { "1 88 12 1z 848 132 36", 9 },        // Non-digit character 'z'
      { "1 88 12 18 ~48 132 36", 11 },       // Non-digit character '~'
      { "1 88 12 18 8\u21538 032 36", 12 },  // Non-digit character Unicode fraction 1/3
      { "1 88 12 18 84\u00D6 032 36", 13 },  // Invalid character unicode O with umlaut
      { "1 88 12 18 848 \u0BE632 36", 15 },  // Invalid character unicode Tamil digit 0
      { "1 88 12 18 848 1.2 36", 16 },       // Non-digit character '.'
      { "1 88 12 18 848 13  36", 17 },       // Non-digit character ' '
      { "1 88 12 18 848 132 A6", 19 },       // Non-digit character 'A'
      { "1 88 12 18 848 132 3Z", 20 },       // Non-digit character 'Z'

      // Special cases for Corsican departments
      { "112072a28806058", 6 },              // lowercase 'a' in Corsican department position
      { "112072b28806085", 6 },              // lowercase 'b' in Corsican department position
      { "1 12 07 2a 288 060 58", 9 },        // lowercase 'a' formatted
      { "1 12 07 2b 288 060 85", 9 },        // lowercase 'b' formatted
   };

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      // Unformatted
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

      // Formatted
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

   public static TheoryData<String, Int32> InvalidSeparatorValues = new()
   {
      // First separator position
      { "1088 12 18 848 132 36", 1 },
      { "1188 12 18 848 132 36", 1 },
      { "1288 12 18 848 132 36", 1 },
      { "1388 12 18 848 132 36", 1 },
      { "1488 12 18 848 132 36", 1 },
      { "1588 12 18 848 132 36", 1 },
      { "1688 12 18 848 132 36", 1 },
      { "1788 12 18 848 132 36", 1 },
      { "1888 12 18 848 132 36", 1 },
      { "1988 12 18 848 132 36", 1 },

      // Second separator position
      { "1 88012 18 848 132 36", 4 },
      { "1 88112 18 848 132 36", 4 },
      { "1 88212 18 848 132 36", 4 },
      { "1 88312 18 848 132 36", 4 },
      { "1 88412 18 848 132 36", 4 },
      { "1 88512 18 848 132 36", 4 },
      { "1 88612 18 848 132 36", 4 },
      { "1 88712 18 848 132 36", 4 },
      { "1 88812 18 848 132 36", 4 },
      { "1 88912 18 848 132 36", 4 },

      // Third separator position
      { "1 88 12018 848 132 36", 7 },
      { "1 88 12118 848 132 36", 7 },
      { "1 88 12218 848 132 36", 7 },
      { "1 88 12318 848 132 36", 7 },
      { "1 88 12418 848 132 36", 7 },
      { "1 88 12518 848 132 36", 7 },
      { "1 88 12618 848 132 36", 7 },
      { "1 88 12718 848 132 36", 7 },
      { "1 88 12818 848 132 36", 7 },
      { "1 88 12918 848 132 36", 7 },

      // Fourth separator position
      { "1 88 12 180848 132 36", 10 },
      { "1 88 12 181848 132 36", 10 },
      { "1 88 12 182848 132 36", 10 },
      { "1 88 12 183848 132 36", 10 },
      { "1 88 12 184848 132 36", 10 },
      { "1 88 12 185848 132 36", 10 },
      { "1 88 12 186848 132 36", 10 },
      { "1 88 12 187848 132 36", 10 },
      { "1 88 12 188848 132 36", 10 },
      { "1 88 12 189848 132 36", 10 },

      // Fifth separator position
      { "1 88 12 18 8480132 36", 14 },
      { "1 88 12 18 8481132 36", 14 },
      { "1 88 12 18 8482132 36", 14 },
      { "1 88 12 18 8483132 36", 14 },
      { "1 88 12 18 8484132 36", 14 },
      { "1 88 12 18 8485132 36", 14 },
      { "1 88 12 18 8486132 36", 14 },
      { "1 88 12 18 8487132 36", 14 },
      { "1 88 12 18 8488132 36", 14 },
      { "1 88 12 18 8489132 36", 14 },

      // Sixth separator position
      { "1 88 12 18 848 132036", 18 },
      { "1 88 12 18 848 132136", 18 },
      { "1 88 12 18 848 132236", 18 },
      { "1 88 12 18 848 132336", 18 },
      { "1 88 12 18 848 132436", 18 },
      { "1 88 12 18 848 132536", 18 },
      { "1 88 12 18 848 132636", 18 },
      { "1 88 12 18 848 132736", 18 },
      { "1 88 12 18 848 132836", 18 },
      { "1 88 12 18 848 132936", 18 },

      // Mixed separators - digits
      { "1 88012 18 848 132 36", 4 },
      { "1 88 12018 848 132 36", 7 },
      { "1 88 12 180848 132 36", 10 },
      { "1 88 12 18 8480132 36", 14 },
      { "1 88 12 18 848 132036", 18 },

      // Mixed separators
      { "1.88-12.18.848.132.36", 4 },
      { "1 88 12.18 848 132 36", 7 },
      { "1 88 12 18.848 132 36", 10 },
      { "1 88 12 18 848.132 36", 14 },
      { "1 88 12 18 848 132*36", 18 },
   };

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
      // Unformatted
      { "00", false },
      { "14", false },
      { "19", false },
      { "43", false },
      { "49", false },

      // Formatted
      { "00", true },
      { "14", true },
      { "19", true },
      { "43", true },
      { "49", true },
   };

   public static TheoryData<String, Boolean> InvalidDepartmentCodes = new()
   {
      // Unformatted
      { "00", false },
      { "20", false },
      { "97", false },
      { "970", false },    // Invalid overseas department
      { "975", false },    // Invalid overseas department
      { "977", false },    // Invalid overseas department
      { "978", false },    // Invalid overseas department
      { "979", false },    // Invalid overseas department

      // Formatted
      { "00", true },
      { "20", true },
      { "97", true },
      { "970", true },     // Invalid overseas department
      { "975", true },     // Invalid overseas department
      { "977", true },     // Invalid overseas department
      { "978", true },     // Invalid overseas department
      { "979", true },     // Invalid overseas department
   };

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.FrInseeNumberInvalidLength,
         value.Length,
         FrInseeNumber.GetValidLengthDefinitions());

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.FrInseeNumberInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.FrInseeNumberInvalidCheckDigits,
         FrInseeNumber.CheckDigitAlgorithmName);

   private static InvalidSeparator GetInvalidSeparatorResult(
      String value,
      Int32 position)
      => new(Messages.FrInseeNumberInvalidSeparator, value[position], position);

   private static InvalidGender GetInvalidGenderResult(String value)
      => new(
         Messages.FrInseeNumberInvalidGender,
         value[0].ToString());

   private static InvalidMonth GetInvalidMonthResult(String value)
      => new(
         Messages.FrInseeNumberInvalidMonth,
         value.Length == 15 ? value[3..5] : value[5..7]);

   private static InvalidFrInseeDepartment GetInvalidFrInseeDepartmentResult(String value)
      => new(
         Messages.FrInseeNumberInvalidDepartment,
         FrInseeNumber.GetDepartmentCode(value));

   #region Constants Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FrInseeNumber_CheckDigitAlgorithmName_ShouldHaveExpectedValue()
      => FrInseeNumber.CheckDigitAlgorithmName.Should().Be("Modulus 97");

   #endregion

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidInseeNumbers))]
   public void FrInseeNumber_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawInsee(value);

      // Act.
      var sut = new FrInseeNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidGenders))]
   public void FrInseeNumber_Constructor_ShouldCreateInstance_WhenValueHasValidGender(Char gender)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(gender: gender);
      var expected = GetRawInsee(value);

      // Act.
      var sut = new FrInseeNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidMonths))]
   public void FrInseeNumber_Constructor_ShouldCreateInstance_WhenValueHasValidMonth(
      String month,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(month: month, formatted: formatted);
      var expected = GetRawInsee(value);

      // Act.
      var sut = new FrInseeNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDepartmentCodes))]
   public void FrInseeNumber_Constructor_ShouldCreateInstance_WhenValueHasValidDepartmentCode(
      String department,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(department: department, formatted: formatted);
      var expected = GetRawInsee(value);

      // Act.
      var sut = new FrInseeNumber(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void FrInseeNumber_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new FrInseeNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void FrInseeNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new FrInseeNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void FrInseeNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new FrInseeNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void FrInseeNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new FrInseeNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void FrInseeNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new FrInseeNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGenderValues))]
   public void FrInseeNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidGender(Char gender)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(gender: gender);
      LocalValidationError expected = GetInvalidGenderResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new FrInseeNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidMonthValues))]
   public void FrInseeNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidMonth(
      String month,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(month: month, formatted: formatted);
      LocalValidationError expected = GetInvalidMonthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new FrInseeNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDepartmentCodes))]
   public void FrInseeNumber_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDepartment(
      String department,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(department: department, formatted: formatted);
      LocalValidationError expected = GetInvalidFrInseeDepartmentResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new FrInseeNumber(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region BirthMonth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidMonths))]
   public void FrInseeNumber_BirthMonth_ShouldReturnExpectedValue(
      String month,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(month: month, formatted: formatted);
      var sut = new FrInseeNumber(value);
      var expected = Int32.Parse(month, CultureInfo.InvariantCulture);

      // Act/assert.
      sut.BirthMonth.Should().Be(expected);
   }

   #endregion

   #region BirthYear Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("00", false)]
   [InlineData("50", false)]
   [InlineData("99", false)]
   [InlineData("00", true)]
   [InlineData("50", true)]
   [InlineData("99", true)]
   public void FrInseeNumber_BirthYear_ShouldReturnExpectedValue(
      String year,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(year: year, formatted: formatted);
      var sut = new FrInseeNumber(value);
      var expected = Int32.Parse(year, CultureInfo.InvariantCulture);

      // Act/assert.
      sut.BirthYear.Should().Be(expected);
   }

   #endregion

   #region Cog Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidInseeNumbers))]
   public void FrInseeNumber_Cog_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new FrInseeNumber(value);
      var expected = sut.Value[5..10];

      // Act/assert.
      sut.Cog.Should().Be(expected);
   }

   #endregion

   #region Department Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidDepartmentCodes))]
   public void FrInseeNumber_Department_ShouldReturnExpectedValue(
      String department,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(department: department, formatted: formatted);
      var sut = new FrInseeNumber(value);

      // Act/assert.
      sut.Department.Should().Be(department);
   }

   #endregion

   #region Gender Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(Chars.DigitOne, BinaryGender.Male, true)]
   [InlineData(Chars.DigitTwo, BinaryGender.Female, true)]
   [InlineData(Chars.DigitSeven, BinaryGender.Male, true)]
   [InlineData(Chars.DigitEight, BinaryGender.Female, true)]
   [InlineData(Chars.DigitOne, BinaryGender.Male, false)]
   [InlineData(Chars.DigitTwo, BinaryGender.Female, false)]
   [InlineData(Chars.DigitSeven, BinaryGender.Male, false)]
   [InlineData(Chars.DigitEight, BinaryGender.Female, false)]
   public void FrInseeNumber_Gender_ShouldReturnExpectedValue(
      Char gender,
      BinaryGender expectedGender,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(gender, formatted: formatted);
      var sut = new FrInseeNumber(value);

      // Act/arrange
      sut.Gender.Should().Be(expectedGender);
   }

   #endregion

   #region IsBornAbroad Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData("01",  false, false)]
   [InlineData("2A",  false, false)]
   [InlineData("973", false, false)]
   [InlineData("99",   true, false)]
   [InlineData("01",  false, true)]
   [InlineData("2A",  false, true)]
   [InlineData("973", false, true)]
   [InlineData("99",   true, true)]
   public void FrInseeNumber_IsBornAbroad_ShouldReturnExpectedValue(
      String department,
      Boolean expectedResult,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(department: department, formatted: formatted);
      var sut = new FrInseeNumber(value);

      // Act/arrange
      sut.IsBornAbroad.Should().Be(expectedResult);
   }

   #endregion

   #region IsTemporary Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(Chars.DigitOne, false)]
   [InlineData(Chars.DigitTwo, false)]
   [InlineData(Chars.DigitSeven, true)]
   [InlineData(Chars.DigitEight, true)]
   public void FrInseeNumber_IsTemporary_ShouldReturnExpectedValue(
      Char gender,
      Boolean expected)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(gender);
      var sut = new FrInseeNumber(value);

      // Act/arrange
      sut.IsTemporaryInsee.Should().Be(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [InlineData(ValidUnformattedInseeNumber, ValidUnformattedInseeNumber)]
   [InlineData(ValidFormattedInseeNumber, ValidUnformattedInseeNumber)]
   [InlineData(ValidUnformattedInseeNumberCorsica, ValidUnformattedInseeNumberCorsica)]
   [InlineData(ValidFormattedInseeNumberCorsica, ValidUnformattedInseeNumberCorsica)]
   public void FrInseeNumber_Value_ShouldReturnValidatedInseeNumber(
      String value,
      String expected)
   {
      // Arrange.
      var sut = new FrInseeNumber(value);

      // Act/assert.
      sut.Value.Should().Be(expected);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FrInseeNumber_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidUnformattedInseeNumber;
      var sut = new FrInseeNumber(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void FrInseeNumber_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidFormattedInseeNumberCorsica;
      var sut = new FrInseeNumber(value);
      var expected = GetRawInsee(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void FrInseeNumber_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      FrInseeNumber sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void FrInseeNumber_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      FrInseeNumber sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidInseeNumbers))]
   public void FrInseeNumber_ExplicitCastToFrInseeNumber_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = GetRawInsee(value);

      // Act.
      var sut = (FrInseeNumber)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidGenders))]
   public void FrInseeNumber_ExplicitCastToFrInseeNumber_ShouldCreateInstance_WhenValueHasValidGender(Char gender)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(gender: gender);
      var expected = GetRawInsee(value);

      // Act.
      var sut = (FrInseeNumber)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidMonths))]
   public void FrInseeNumber_ExplicitCastToFrInseeNumber_ShouldCreateInstance_WhenValueHasValidMonth(
      String month,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(month: month, formatted: formatted);
      var expected = GetRawInsee(value);

      // Act.
      var sut = (FrInseeNumber)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDepartmentCodes))]
   public void FrInseeNumber_ExplicitCastToFrInseeNumber_ShouldCreateInstance_WhenValueHasValidDepartmentCode(
      String department,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(department: department, formatted: formatted);
      var expected = GetRawInsee(value);

      // Act.
      var sut = (FrInseeNumber)value;

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void FrInseeNumber_ExplicitCastToFrInseeNumber_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FrInseeNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void FrInseeNumber_ExplicitCastToFrInseeNumber_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FrInseeNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void FrInseeNumber_ExplicitCastToFrInseeNumber_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FrInseeNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void FrInseeNumber_ExplicitCastToFrInseeNumber_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FrInseeNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void FrInseeNumber_ExplicitCastToFrInseeNumber_ShouldThrowKfValidationException_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidSeparatorResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FrInseeNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGenderValues))]
   public void FrInseeNumber_ExplicitCastToFrInseeNumber_ShouldThrowKfValidationException_WhenValueHasInvalidGender(Char gender)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(gender: gender);
      LocalValidationError expected = GetInvalidGenderResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FrInseeNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidMonthValues))]
   public void FrInseeNumber_ExplicitCastToFrInseeNumber_ShouldThrowKfValidationException_WhenValueHasInvalidMonth(
      String month,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(month: month, formatted: formatted);
      LocalValidationError expected = GetInvalidMonthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FrInseeNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDepartmentCodes))]
   public void FrInseeNumber_ExplicitCastToFrInseeNumber_ShouldThrowKfValidationException_WhenValueHasInvalidDepartment(
      String department,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(department: department, formatted: formatted);
      LocalValidationError expected = GetInvalidFrInseeDepartmentResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (FrInseeNumber)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FrInseeNumber_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new FrInseeNumber(ValidUnformattedInseeNumber);
      var sut2 = new FrInseeNumber(ValidUnformattedInseeNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void FrInseeNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new FrInseeNumber(ValidUnformattedInseeNumber);
      var sut2 = new FrInseeNumber(AltValidUnformattedInseeNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   [Fact]
   public void FrInseeNumber_EqualityOperator_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 15 and 21 character versions for same person should still be equal.
      var sut1 = new FrInseeNumber(ValidUnformattedInseeNumber);
      var sut2 = new FrInseeNumber(ValidFormattedInseeNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FrInseeNumber_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new FrInseeNumber(ValidUnformattedInseeNumber);
      var sut2 = new FrInseeNumber(AltValidUnformattedInseeNumber);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void FrInseeNumber_InequalityOperator_ShouldReturnFalse_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 15 and 21 character versions for same person should still be equal.
      var sut1 = new FrInseeNumber(ValidUnformattedInseeNumber);
      var sut2 = new FrInseeNumber(ValidFormattedInseeNumber);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   [Fact]
   public void FrInseeNumber_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new FrInseeNumber(ValidUnformattedInseeNumber);
      var sut2 = new FrInseeNumber(ValidUnformattedInseeNumber);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidInseeNumbers))]
   public void FrInseeNumber_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new FrInseeNumber(value);

      // Act.
      var result = FrInseeNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidGenders))]
   public void FrInseeNumber_Create_ShouldCreateInstance_WhenValueHasValidGender(Char gender)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(gender: gender);
      LocalCreateResult expected = new FrInseeNumber(value);

      // Act.
      var result = FrInseeNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidMonths))]
   public void FrInseeNumber_Create_ShouldCreateInstance_WhenValueHasValidMonth(
      String month,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(month: month, formatted: formatted);
      LocalCreateResult expected = new FrInseeNumber(value);

      // Act.
      var result = FrInseeNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDepartmentCodes))]
   public void FrInseeNumber_Create_ShouldCreateInstance_WhenValueHasValidDepartmentCode(
      String department,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(department: department, formatted: formatted);
      LocalCreateResult expected = new FrInseeNumber(value);

      // Act.
      var result = FrInseeNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void FrInseeNumber_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = FrInseeNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void FrInseeNumber_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = FrInseeNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void FrInseeNumber_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = FrInseeNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void FrInseeNumber_Create_ShouldReturnInvalidCheckDigitsValidationResult_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = FrInseeNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void FrInseeNumber_Create_ShouldReturnInvalidSeparatorValidationResult_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidSeparatorResult(value, position);

      // Act.
      var result = FrInseeNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGenderValues))]
   public void FrInseeNumber_Create_ShouldReturnInvalidGenderValidationResult_WhenValueHasInvalidGender(Char gender)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(gender: gender);
      LocalCreateResult expected = (LocalValidationError)GetInvalidGenderResult(value);

      // Act.
      var result = FrInseeNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidMonthValues))]
   public void FrInseeNumber_Create_ShouldReturnInvaliMonthValidationResult_WhenValueHasInvalidMonth(
      String month,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(month: month, formatted: formatted);
      LocalCreateResult expected = (LocalValidationError)GetInvalidMonthResult(value);

      // Act.
      var result = FrInseeNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDepartmentCodes))]
   public void FrInseeNumber_Create_ShouldReturnInvalidDepartmentValidationResult_WhenValueHasInvalidDepartment(
      String department,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(department: department, formatted: formatted);
      LocalCreateResult expected = (LocalValidationError)GetInvalidFrInseeDepartmentResult(value);

      // Act.
      var result = FrInseeNumber.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FrInseeNumber_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new FrInseeNumber(ValidUnformattedInseeNumber);
      var sut2 = new FrInseeNumber(ValidUnformattedInseeNumber);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void FrInseeNumber_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new FrInseeNumber(ValidUnformattedInseeNumber);
      var sut2 = new FrInseeNumber(AltValidUnformattedInseeNumber);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void FrInseeNumber_Equals_ShouldReturnTrue_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 15 and 21 character versions for same person should still be equal.
      var sut1 = new FrInseeNumber(ValidUnformattedInseeNumber);
      var sut2 = new FrInseeNumber(ValidFormattedInseeNumber);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   #endregion

   #region Format Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FrInseeNumber_Format_ShouldReturnExpectedString_WhenDefaultMaskIsUsed()
   {
      // Arrange.
      var sut = new FrInseeNumber(ValidUnformattedInseeNumber);
      var expected = ValidFormattedInseeNumber;

      // Act.
      var str = sut.Format();

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void FrInseeNumber_Format_ShouldReturnExpectedString_WhenCustomMaskIsUsed()
   {
      // Arrange.
      var sut = new FrInseeNumber(ValidUnformattedInseeNumber);
      var mask = "_______________";
      var expected = ValidUnformattedInseeNumber;

      // Act.
      var str = sut.Format(mask);

      // Assert.
      str.Should().Be(expected);
   }

   [Fact]
   public void FrInseeNumber_Format_ShouldThrowArgumentNullException_WhenMaskIsNull()
   {
      // Arrange.
      var sut = new FrInseeNumber(ValidUnformattedInseeNumberCorsica);
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
   public void FrInseeNumber_Format_ShouldThrowArgumentException_WhenMaskIsEmpty(String mask)
   {
      // Arrange.
      var sut = new FrInseeNumber(ValidFormattedInseeNumberCorsica);
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
   public void FrInseeNumber_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new FrInseeNumber(ValidUnformattedInseeNumber);
      var sut2 = new FrInseeNumber(ValidUnformattedInseeNumber);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void FrInseeNumber_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new FrInseeNumber(ValidUnformattedInseeNumber);
      var sut2 = new FrInseeNumber(AltValidUnformattedInseeNumber);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   [Fact]
   public void FrInseeNumber_GetHashCode_ShouldBeConsistent_WhenValuesHaveDifferentLengths()
   {
      // Arrange. 15 and 21 character versions for same person should still be equal.
      var sut1 = new FrInseeNumber(ValidUnformattedInseeNumber);
      var sut2 = new FrInseeNumber(ValidFormattedInseeNumber);

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

   // FrInseeNumber does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void FrInseeNumber_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new FrInseeNumber(ValidUnformattedTemporaryInseeNumber);
      var sut2 = new FrInseeNumber(ValidUnformattedTemporaryInseeNumber);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidInseeNumbers))]
   public void FrInseeNumber_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new FrInseeNumber(value);
      var expected = GetRawInsee(value);

      // Act/assert.
      sut.ToString().Should().Be(expected);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidInseeNumbers))]
   public void FrInseeNumber_Validate_ShouldReturnValidationPassed_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = FrInseeNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidGenders))]
   public void FrInseeNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidGender(Char gender)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(gender: gender);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = FrInseeNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidMonths))]
   public void FrInseeNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidMonth(
      String month,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(month: month, formatted: formatted);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = FrInseeNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDepartmentCodes))]
   public void FrInseeNumber_Validate_ShouldReturnValidationPassed_WhenValueHasValidDepartmentCode(
      String department,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(department: department, formatted: formatted);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = FrInseeNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void FrInseeNumber_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = FrInseeNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = FrInseeNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacterWhereDigitExpected(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = FrInseeNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidCheckDigits_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = FrInseeNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidSeparatorValues))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidSeparator_WhenValueHasInvalidSeparator(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidSeparatorResult(value, position);

      // Act.
      var result = FrInseeNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidGenderValues))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidGender_WhenValueHasInvalidGender(Char gender)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(gender: gender);
      LocalValidationResult expected = GetInvalidGenderResult(value);

      // Act.
      var result = FrInseeNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidMonthValues))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidMonth_WhenValueHasInvalidMonth(
      String month,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(month: month, formatted: formatted);
      LocalValidationResult expected = GetInvalidMonthResult(value);

      // Act.
      var result = FrInseeNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDepartmentCodes))]
   public void FrInseeNumber_Validate_ShouldReturnInvalidDepartment_WhenValueHasInvalidDepartment(
      String department,
      Boolean formatted)
   {
      // Arrange.
      var value = GetInseeWithValidCheckDigits(department: department, formatted: formatted);
      LocalValidationResult expected = GetInvalidFrInseeDepartmentResult(value);

      // Act.
      var result = FrInseeNumber.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void FrInseeNumber_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new FrInseeNumber(ValidUnformattedInseeNumberCorsica);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<FrInseeNumber>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void FrInseeNumber_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new FrInseeNumber(AltValidFormattedInseeNumber);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public FrInseeNumber InseeNumber { get; set; } = null!;
   }

   [Fact]
   public void FrInseeNumber_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { InseeNumber = new FrInseeNumber(ValidUnformattedInseeNumber) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void FrInseeNumber_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"InseeNumber\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void FrInseeNumber_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"InseeNumber\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.InseeNumber.Should().BeNull();
   }

   [Fact]
   public void FrInseeNumber_JsonDeserialization_ShouldThrowKfValidationException_WhenInseeNumberIsInvalid()
   {
      // Arrange.
      var json = "{\"InseeNumber\":\"188121884812236\"}";  // Invalid check digits
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
