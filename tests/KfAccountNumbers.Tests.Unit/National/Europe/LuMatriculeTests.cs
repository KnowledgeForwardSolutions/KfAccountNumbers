#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using LocalCreateResult = KfAccountNumbers.Results.CreateResult<
   KfAccountNumbers.National.Europe.LuMatricule,
   KfAccountNumbers.National.Europe.LuMatricule.ValidationError>;
using LocalValidationError = KfAccountNumbers.National.Europe.LuMatricule.ValidationError;
using LocalValidationException = KfAccountNumbers.UKfValidationException<
   KfAccountNumbers.National.Europe.LuMatricule.ValidationError>;
using LocalValidationResult = KfAccountNumbers.National.Europe.LuMatricule.ValidationResult;

namespace KfAccountNumbers.Tests.Unit.National.Europe;

public class LuMatriculeTests
{
   private const String ValidMatricule = "1960090900163";         // From https://www.lbr.lu/mjrcs/jsp/webapp/static/mjrcs/en/mjrcs/pdf/FAQ_National_Identification_Number.pdf
   private const String AltValidMatricule = "1985011500173";      // From https://tin-validate.com/docs/countries/lu

   public static TheoryData<String> ValidValues =>
   [
      ValidMatricule,
      AltValidMatricule,
   ];

   public static TheoryData<String, String> ValidDateOfBirthValues = new()
   {
      // Century boundary values
      { "18000101", "18000101" },      // January 1, 1800
      { "18991231", "18991231" },      // December 31, 1899
      { "19000101", "19000101" },      // January 1, 1900
      { "19991231", "19991231" },      // December 31 1999
      { "20000101", "20000101" },      // January 1, 2000
      { "20991231", "20991231" },      // December 31, 2099

      // Month max days
      { "20010131", "20010131" },      // Max day of month January 2001
      { "18910228", "18910228" },      // Max day of month February 1891 (non-leap year)
      { "18960229", "18960229" },      // Max day of month February 1896 (leap year)
      { "19000228", "19000228" },      // Max day of month February 1900 (non-leap year)
      { "19520229", "19520229" },      // Max day of month February 1952 (leap year)
      { "20000229", "20000229" },      // Max day of month February 2000 (leap year, century divisible by 400)
      { "20010228", "20010228" },      // Max day of month February 2001 (non-leap year)
      { "18530331", "18530331" },      // Max day of month March 1853
      { "20080430", "20080430" },      // Max day of month April 2008
      { "19150531", "19150531" },      // Max day of month May 1915
      { "19600630", "19600630" },      // Max day of month June 1960
      { "18750731", "18750731" },      // Max day of month July 1875
      { "19810831", "19810831" },      // Max day of month August 1981
      { "19090930", "19090930" },      // Max day of month September 1909
      { "20101031", "20101031" },      // Max day of month October 2010
      { "19111130", "19111130" },      // Max day of month November 1911
      { "20121231", "20121231" },      // Max day of month December 2012
   };

   public static TheoryData<String> InvalidLengthValues =>
   [
      "196009090016",         // Length 12
      "19600909001634",       // Length 14
      new String('1', 100)    // Very long string
   ];

   // Values that will report an invalid character encountered
   public static TheoryData<String, Int32> InvalidCharacterValues = new()
   {
      { ".960090900163", 0 },          // Non-digit character '.'
      { "1 60090900163", 1 },          // Non-digit character ' '
      { "19A0090900163", 2 },          // Non-digit character 'A'
      { "196Z090900163", 3 },          // Non-digit character 'Z'
      { "1960^90900163", 4 },          // Non-digit character '^'
      { "19600a0900163", 5 },          // Non-digit character 'a'
      { "196009z900163", 6 },          // Non-digit character 'z'
      { "1960090~00163", 7 },          // Non-digit character '~'
      { "19600909\u21530163", 8 },     // Non-digit character Unicode fraction 1/3
      { "196009090\u00D6163", 9 },     // Invalid character unicode O with umlaut
      { "1960090900\u0BE663", 10 },    // Invalid character unicode Tamil digit 0
      { "19600909001.3", 11 },         // Non-digit character '.'
      { "196009090016A", 12 },         // Non-digit character 'A'
   };

   public static TheoryData<String> InvalidCheckDigitValues =>
   [
      "1970090900163",     // 1960090900163 with single digit transcription error, 6 -> 7
      "1985011500273",     // 1985011500173 with single digit transcription error, 1 -> 2
      "1690090900163",     // 1960090900163 with two digit transposition error, 96 -> 69
      "1960900900163",     // 1960090900163 with two digit transposition error, 09 -> 90, missed by Luhn, caught by Verhoeff
      "1985022500173",     // 1985011500173 with two digit twin error, 11 -> 22
      "1966042900111",     // 1933042900111 with two digit twin error, 33 -> 66, missed by Luhn, caught by Verhoeff
      "1589011500173",     // 1985011500173 with jump transposition, 985 -> 589, missed by Luhn, caught by Verhoeff
      "1960090900173",     // 1960090900163 with invalid Luhn check digit, 6 -> 7
      "1985011500174",     // 1985011500173 with invalid Verhoeff check digit, 3 -> 4
   ];

   public static TheoryData<String> InvalidDateOfBirthValues =>
   [
      // Invalid month
      "00000101",          // Invalid year (too low)

      // Invalid month
      "19010001",          // Invalid month (too low)
      "20011301",          // Invalid month (too high)

      // Invalid day
      "19010100",          // Invalid day of month (too low)
      "20010132",          // Invalid day of month January
      "19010229",          // Invalid day of month February (non leap year)
      "19000229",          // Invalid day of month February (1900 is not a leap year)
      "19040230",          // Invalid day of month February (leap year)
      "20010332",          // Invalid day of month March
      "19010431",          // Invalid day of month April
      "20010532",          // Invalid day of month May
      "19010631",          // Invalid day of month June
      "20010732",          // Invalid day of month July
      "19010832",          // Invalid day of month August
      "20010931",          // Invalid day of month September
      "19011032",          // Invalid day of month October
      "20011131",          // Invalid day of month November
      "19011232",          // Invalid day of month December
   ];

   private static InvalidCharacter GetInvalidCharacterResult(
      String value,
      Int32 position)
      => new(
         Messages.LuMatriculeInvalidCharacter,
         value[position],
         position);

   private static InvalidChecksum GetInvalidChecksumResult()
      => new(
         Messages.LuMatriculeInvalidCheckDigits,
         LuMatricule.CheckDigitAlgorithmNames);

   private static InvalidDateOfBirth GetInvalidDateOfBirthResult(String value)
      => new(
         Messages.LuMatriculeInvalidDateOfBirth,
         value[..8],
         DateFormatName.YYYYMMDD);

   private static InvalidLength GetInvalidLengthResult(String value)
      => new(
         Messages.LuMatriculeInvalidLength,
         value.Length,
         [
            new ValidLengthDefinition(LuMatricule.ValidLength, Messages.LuMatriculeLength),
         ]);

   private static String GetValue(
      String dateOfBirth = "20011231",
      String individualNumber = "001")
   {
      var temp = dateOfBirth + individualNumber;
      var luhnResult = Algorithms.Luhn.TryCalculateCheckDigit(temp, out var luhn);
      var verhoeffResult = Algorithms.Verhoeff.TryCalculateCheckDigit(temp, out var verhoeff);

#pragma warning disable IDE0046 // Convert to conditional expression
      if (!luhnResult || !verhoeffResult)
      {
         throw new InvalidOperationException("Invalid checksum");
      }
#pragma warning restore IDE0046 // Convert to conditional expression

      return $"{temp}{luhn}{verhoeff}";
   }

   #region Constructor Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void LuMatricule_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Act.
      var sut = new LuMatricule(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(value);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void LuMatricule_Constructor_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String _)
   {
      // Act.
      var value = GetValue(dateOfBirth);
      var sut = new LuMatricule(value);

      // Assert.
      sut.Should().NotBeNull();
      sut.Value.Should().Be(value);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void LuMatricule_Constructor_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => new LuMatricule(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void LuMatricule_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new LuMatricule(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void LuMatricule_Constructor_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => new LuMatricule(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void LuMatricule_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => new LuMatricule(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void LuMatricule_Constructor_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(String dateOfBirth)
   {
      // Arrange.
      var value = GetValue(dateOfBirth);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => new LuMatricule(value))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region DateOfBirth Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void LuMatricule_DateOfBirth_ShouldReturnExpectedValue(
      String dateOfBirth,
      String expectedString)
   {
      // Arrange.
      var value = GetValue(dateOfBirth);
      var sut = new LuMatricule(value);
      var expected = DateOnly.ParseExact(
         expectedString,
         "yyyyMMdd",
         CultureInfo.InvariantCulture);

      // Act/assert.
      sut.DateOfBirth.Should().Be(expected);
   }

   #endregion

   #region Value Property Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void LuMatricule_Value_ShouldReturnValidatedMatricule(String value)
   {
      // Arrange.
      var sut = new LuMatricule(value);

      // Act/assert.
      sut.Value.Should().Be(value);
   }

   #endregion

   #region Conversion Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void LuMatricule_ImplicitToStringConversion_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidMatricule;
      var sut = new LuMatricule(value);

      // Act.
      String str = sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void LuMatricule_CastToString_ShouldReturnExpectedValue_WhenValueIsNotNull()
   {
      // Arrange.
      var value = ValidMatricule;
      var sut = new LuMatricule(value);

      // Act.
      var str = (String)sut;

      // Assert.
      str.Should().Be(sut.Value);
   }

   [Fact]
   public void LuMatricule_ImplicitToStringConversion_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      LuMatricule sut = null!;

      // Act.
      String str = sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Fact]
   public void LuMatricule_CastToString_ShouldReturnEmptyString_WhenValueIsNull()
   {
      // Arrange.
      LuMatricule sut = null!;

      // Act.
      var str = (String)sut;

      // Act/assert.
      str.Should().NotBeNull();
      str.Should().BeEmpty();
   }

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void LuMatricule_ExplicitCastToLuMatricule_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      var expected = new LuMatricule(value);

      // Act.
      var sut = (LuMatricule)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void LuMatricule_ExplicitCastToLuMatricule_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String _)
   {
      // Arrange.
      var value = GetValue(dateOfBirth);
      var expected = new LuMatricule(value);

      // Act.
      var sut = (LuMatricule)value;

      // Assert.
      sut.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void LuMatricule_ExplicitCastToLuMatricule_ShouldThrowKfValidationException_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationError expected = default(EmptyValue);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (LuMatricule)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void LuMatricule_ExplicitCastToLuMatricule_ShouldThrowKfValidationException_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidLengthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (LuMatricule)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected, options => options        // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
            .ComparingByMembers<LocalValidationError>()
            .ComparingByMembers<ValidLengthDefinition>()
            .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void LuMatricule_ExplicitCastToLuMatricule_ShouldThrowKfValidationException_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidCharacterResult(value, position);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (LuMatricule)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void LuMatricule_ExplicitCastToLuMatricule_ShouldThrowKfValidationException_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (LuMatricule)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void LuMatricule_ExplicitCastToLuMatricule_ShouldThrowKfValidationException_WhenValueHasInvalidDateOfBirth(String dateOfBirth)
   {
      // Arrange.
      var value = GetValue(dateOfBirth);
      LocalValidationError expected = GetInvalidDateOfBirthResult(value);

      // Act/assert.
      FluentActions
         .Invoking(() => _ = (LuMatricule)value)
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void LuMatricule_EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new LuMatricule(ValidMatricule);
      var sut2 = new LuMatricule(ValidMatricule);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();
   }

   [Fact]
   public void LuMatricule_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new LuMatricule(ValidMatricule);
      var sut2 = new LuMatricule(AltValidMatricule);

      // Act/assert.
      (sut1 == sut2).Should().BeFalse();
   }

   #endregion

   #region Inequality Operator Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void LuMatricule_InequalityOperator_ShouldReturnTrue_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new LuMatricule(ValidMatricule);
      var sut2 = new LuMatricule(AltValidMatricule);

      // Act/assert.
      (sut1 != sut2).Should().BeTrue();
   }

   [Fact]
   public void LuMatricule_InequalityOperator_ShouldReturnFalse_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new LuMatricule(ValidMatricule);
      var sut2 = new LuMatricule(ValidMatricule);

      // Act/assert.
      (sut1 != sut2).Should().BeFalse();
   }

   #endregion

   #region Create Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void LuMatricule_Create_ShouldCreateInstance_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalCreateResult expected = new LuMatricule(value);

      // Act.
      var result = LuMatricule.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void LuMatricule_Create_ShouldCreateInstance_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String _)
   {
      // Arrange.
      var value = GetValue(dateOfBirth);
      LocalCreateResult expected = new LuMatricule(value);

      // Act.
      var result = LuMatricule.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void LuMatricule_Create_ShouldReturnEmptyValidationResult_WhenValueIsEmpty(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)default(EmptyValue);

      // Act.
      var result = LuMatricule.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void LuMatricule_Create_ShouldReturnInvalidLengthValidationResult_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidLengthResult(value);

      // Act.
      var result = LuMatricule.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options                         // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalCreateResult>()
         .ComparingByMembers<LocalValidationError>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void LuMatricule_Create_ShouldReturnInvalidCharacterValidationResult_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidCharacterResult(value, position);

      // Act.
      var result = LuMatricule.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void LuMatricule_Create_ShouldReturnInvalidCheckDigitsValidationResult_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalCreateResult expected = (LocalValidationError)GetInvalidChecksumResult();

      // Act.
      var result = LuMatricule.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void LuMatricule_Create_ShouldReturnInvalidDateOfBirthValidationResult_WhenValueHasInvalidDateOfBirth(String dateOfBirth)
   {
      // Arrange.
      var value = GetValue(dateOfBirth);
      LocalCreateResult expected = (LocalValidationError)GetInvalidDateOfBirthResult(value);

      // Act.
      var result = LuMatricule.Create(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Equals Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void LuMatricule_Equals_ShouldReturnTrue_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new LuMatricule(ValidMatricule);
      var sut2 = new LuMatricule(ValidMatricule);

      // Act/assert.
      sut1.Equals(sut2).Should().BeTrue();
   }

   [Fact]
   public void LuMatricule_Equals_ShouldReturnFalse_WhenValuesAreNotEqual()
   {
      // Arrange.
      var sut1 = new LuMatricule(ValidMatricule);
      var sut2 = new LuMatricule(AltValidMatricule);

      // Act/assert.
      sut1.Equals(sut2).Should().BeFalse();
   }

   [Fact]
   public void LuMatricule_Equals_ShouldReturnFalse_WhenComparedToDifferentType()
   {
      // Arrange.
      var sut = new LuMatricule(ValidMatricule);

      // Act/assert.
      sut.Equals(ValidMatricule).Should().BeFalse();
   }

   [Fact]
   public void LuMatricule_Equals_ShouldReturnFalse_WhenComparedWithNull()
   {
      // Arrange.
      var sut = new LuMatricule(ValidMatricule);

      // Act/assert.
      sut.Equals(null).Should().BeFalse();
   }

   #endregion

   #region GetHashCode Method Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void LuMatricule_GetHashCode_ShouldBeConsistent_WhenValuesAreEqual()
   {
      // Arrange.
      var sut1 = new LuMatricule(ValidMatricule);
      var sut2 = new LuMatricule(ValidMatricule);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().Be(hash2);
   }

   [Fact]
   public void LuMatricule_GetHashCode_ShouldReturnDifferentValues_WhenValuesAreDifferent()
   {
      // Arrange.
      var sut1 = new LuMatricule(ValidMatricule);
      var sut2 = new LuMatricule(AltValidMatricule);

      // Act.
      var hash1 = sut1.GetHashCode();
      var hash2 = sut2.GetHashCode();

      // Assert.
      hash1.Should().NotBe(hash2);
   }

   #endregion

   #region ReferenceEquals Method Tests
   // ==========================================================================
   // ==========================================================================

   // LuMatricule does not override Object.ReferenceEquals, so this test just
   // confirms that two different instances with the same value are not
   // considered reference equal.

   [Fact]
   public void LuMatricule_ObjectReferenceEquals_ShouldReturnFalse_WhenValuesAreEqualButInstancesAreDifferent()
   {
      // Arrange.
      var sut1 = new LuMatricule(ValidMatricule);
      var sut2 = new LuMatricule(ValidMatricule);

      // Act/assert.
      (sut1 == sut2).Should().BeTrue();                         // Value equality should be true
      ReferenceEquals(sut1, sut2).Should().BeFalse();
   }

   #endregion

   #region ToString Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void LuMatricule_ToString_ShouldReturnExpectedValue(String value)
   {
      // Arrange.
      var sut = new LuMatricule(value);

      // Act/assert.
      sut.ToString().Should().Be(sut.Value);
   }

   #endregion

   #region Validate Method Tests
   // ==========================================================================
   // ==========================================================================

   [Theory]
   [MemberData(nameof(ValidValues))]
   public void LuMatricule_Validate_ShouldReturnValidValue_WhenValueIsValid(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = LuMatricule.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(ValidDateOfBirthValues))]
   public void LuMatricule_Validate_ShouldReturnValidValue_WhenValueHasValidDateOfBirth(
      String dateOfBirth,
      String _)
   {
      // Arrange.
      var value = GetValue(dateOfBirth);
      LocalValidationResult expected = default(ValidValue);

      // Act.
      var result = LuMatricule.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [ClassData(typeof(StringNullEmptyWhitespaceValues))]
   public void LuMatricule_Validate_ShouldReturnEmpty_WhenValueIsNullOrEmpty(String value)
   {
      // Arrange.
      LocalValidationResult expected = default(EmptyValue);

      // Act.
      var result = LuMatricule.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidLengthValues))]
   public void LuMatricule_Validate_ShouldReturnInvalidLength_WhenValueHasInvalidLength(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidLengthResult(value);

      // Act.
      var result = LuMatricule.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected, options => options    // Options necessary because FluentAssertions gets lost comparing the ValidLengthDefinition array in InvalidLength type
         .ComparingByMembers<LocalValidationResult>()
         .ComparingByMembers<ValidLengthDefinition>()
         .WithoutStrictOrdering());
   }

   [Theory]
   [MemberData(nameof(InvalidCharacterValues))]
   public void LuMatricule_Validate_ShouldReturnInvalidCharacter_WhenValueHasNonDigitCharacter(
      String value,
      Int32 position)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidCharacterResult(value, position);

      // Act.
      var result = LuMatricule.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidCheckDigitValues))]
   public void LuMatricule_Validate_ShouldReturnInvalidChecksum_WhenValueHasInvalidCheckDigits(String value)
   {
      // Arrange.
      LocalValidationResult expected = GetInvalidChecksumResult();

      // Act.
      var result = LuMatricule.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   [Theory]
   [MemberData(nameof(InvalidDateOfBirthValues))]
   public void LuMatricule_Validate_ShouldReturnInvalidDateOfBirth_WhenValueHasInvalidDateOfBirth(String dateOfBirth)
   {
      // Arrange.
      var value = GetValue(dateOfBirth);
      LocalValidationResult expected = GetInvalidDateOfBirthResult(value);

      // Act.
      var result = LuMatricule.Validate(value);

      // Assert.
      result.Should().BeEquivalentTo(expected);
   }

   #endregion

   #region Json Serialization Tests
   // ==========================================================================
   // ==========================================================================

   [Fact]
   public void LuMatricule_JsonSerialization_ShouldRoundTripSuccessfully()
   {
      // Arrange.
      var sut = new LuMatricule(ValidMatricule);

      // Act.
      var json = JsonSerializer.Serialize(sut);
      var result = JsonSerializer.Deserialize<LuMatricule>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(sut);
   }

   [Fact]
   public void LuMatricule_JsonSerialization_ShouldSerializeAsStringInsteadOfObject()
   {
      // Arrange.
      var sut = new LuMatricule(ValidMatricule);
      var expected = sut.Value;

      // Act.
      var json = JsonSerializer.Serialize(sut);

      // Assert.
      json.Should().Be($"\"{expected}\"");  // Simple string, not object
   }

   public class Foo
   {
      public LuMatricule Matricule { get; set; } = null!;
   }

   [Fact]
   public void LuMatricule_JsonSerialization_ShouldDeserializeComplexObject()
   {
      // Arrange.
      var foo = new Foo { Matricule = new LuMatricule(ValidMatricule) };
      var json = JsonSerializer.Serialize(foo);

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(foo);
   }

   [Fact]
   public void LuMatricule_JsonSerialization_ShouldSerializeNullGracefully()
   {
      // Arrange.
      var expected = /*lang=json,strict*/ "{\"Matricule\":null}";
      var foo = new Foo();

      // Act.
      var json = JsonSerializer.Serialize(foo);

      // Assert.
      json.Should().Be(expected);
   }

   [Fact]
   public void LuMatricule_JsonDeserialization_ShouldDeserializeNullGracefully()
   {
      // Arrange.
      var json = "{\"Matricule\":null}";

      // Act.
      var result = JsonSerializer.Deserialize<Foo>(json);

      // Assert.
      result.Should().NotBeNull();
      result!.Matricule.Should().BeNull();
   }

   [Fact]
   public void LuMatricule_JsonDeserialization_ShouldThrowKfValidationException_WhenMatriculeIsInvalid()
   {
      // Arrange.
      var json = "{\"Matricule\":\"1970090900163\"}";  // Invalid checksum
      LocalValidationError expected = GetInvalidChecksumResult();

      // Act/assert.
      FluentActions
         .Invoking(() => JsonSerializer.Deserialize<Foo>(json))
         .Should().ThrowExactly<LocalValidationException>()
         .And.ValidationError.Should().BeEquivalentTo(expected);
   }

   #endregion
}
