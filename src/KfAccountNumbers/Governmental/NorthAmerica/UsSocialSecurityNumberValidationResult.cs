namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Defines the possible results returned when validating a US Social Security 
///   Number.
/// </summary>
public enum UsSocialSecurityNumberValidationResult
{
   /// <summary>
   ///   The value does not contain any validation errors.
   /// </summary>
   ValidationPassed = 1,

   /// <summary>
   ///   SSN value is <see langword="null"/>, <see cref="String.Empty"/> or all
   ///   whitespace characters.
   /// </summary>
   Empty,

   /// <summary>
   ///   SSN value has incorrect length. Must be either 9 characters (digits 
   ///   only) or 11 characters (9 digits with two separator characters).
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   SSN value contains an unexpected character in a separator character
   ///   location. 
   /// </summary>
   /// <remarks>
   ///   A SSN with length 11 should have valid separator character between the 
   ///   area, group and serial number sections. When using the default
   ///   separator, the expected separator character is a dash ('-'). When using
   ///   a custom separator, the expected separator character is the custom
   ///   separator.
   /// </remarks>
   InvalidSeparatorEncountered,

   /// <summary>
   ///   SSN value contains a non digit character.
   /// </summary>
   InvalidCharacterEncountered,

   /// <summary>
   ///   SSN area number segment is invalid (is 000, 666 or 900-999).
   /// </summary>
   InvalidAreaNumber,

   /// <summary>
   ///   SSN group number segment is invalid (is 00).
   /// </summary>
   InvalidGroupNumber,

   /// <summary>
   ///   SSN serial number segment is invalid (is 0000).
   /// </summary>
   InvalidSerialNumber,

   /// <summary>
   ///   SSN consists of 9 identical digits.
   /// </summary>
   AllIdenticalDigits,

   /// <summary>
   ///   SSN consists of the consecutive digits 123456789.
   /// </summary>
   InvalidRun,
}

/// <summary>
///   Additional methods for type <see cref="UsSocialSecurityNumberValidationResult"/>.
/// </summary>
public static class UsSocialSecurityNumberValidationResultExtensions
{
   public static String ToErrorDescription(this UsSocialSecurityNumberValidationResult errorType)
      => errorType switch
      {
         UsSocialSecurityNumberValidationResult.Empty => Messages.UsSsnEmpty,
         UsSocialSecurityNumberValidationResult.InvalidLength => Messages.UsSsnInvalidLength,
         UsSocialSecurityNumberValidationResult.InvalidSeparatorEncountered => Messages.UsSsnInvalidSeparatorEncountered,
         UsSocialSecurityNumberValidationResult.InvalidCharacterEncountered => Messages.UsSsnInvalidCharacterEncountered,
         UsSocialSecurityNumberValidationResult.InvalidAreaNumber => Messages.UsSsnInvalidAreaNumber,
         UsSocialSecurityNumberValidationResult.InvalidGroupNumber => Messages.UsSsnInvalidGroupNumber,
         UsSocialSecurityNumberValidationResult.InvalidSerialNumber => Messages.UsSsnInvalidSerialNumber,
         UsSocialSecurityNumberValidationResult.AllIdenticalDigits => Messages.UsSsnAllIdenticalDigits,
         UsSocialSecurityNumberValidationResult.InvalidRun => Messages.UsSsnInvalidRun,
         UsSocialSecurityNumberValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };
}