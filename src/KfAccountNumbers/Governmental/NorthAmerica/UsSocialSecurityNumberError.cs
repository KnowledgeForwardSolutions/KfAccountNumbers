namespace KfAccountNumbers.Governmental.NorthAmerica;

public enum UsSocialSecurityNumberErrorType
{
   None = 0,

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
   ///   SSN value contains an ASCII digit (0-9) in a separator character
   ///   location.
   /// </summary>
   InvalidSeparatorCharacter,

   /// <summary>
   ///   SSN value contains a non digit character.
   /// </summary>
   InvalidCharacter,

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
///   An error encountered while creating a <see cref="UsSocialSecurityNumber"/>.
/// </summary>
public record UsSocialSecurityNumberError : CreateError<UsSocialSecurityNumberErrorType>
{
   private UsSocialSecurityNumberError(
      UsSocialSecurityNumberErrorType errorType,
      String description) : base(errorType, description) { }

   /// <summary>
   ///   Error that indicates that the string SSN contained 9 identical digit
   ///   characters.
   /// </summary>
   public static UsSocialSecurityNumberError AllIdenticalDigits => new(
      UsSocialSecurityNumberErrorType.AllIdenticalDigits,
      Messages.UsSsnAllIdenticalDigits);

   /// <summary>
   ///   Error that indicates that the string SSN value was 
   ///   <see langword="null"/>, <see cref="String.Empty"/> or all whitespace 
   ///   characters.
   /// </summary>
   public static UsSocialSecurityNumberError Empty => new(
      UsSocialSecurityNumberErrorType.Empty, 
      Messages.UsSsnEmpty);

   /// <summary>
   ///   Error that indicates that the area number segment of the SSN is invalid.
   /// </summary>
   public static UsSocialSecurityNumberError InvalidAreaNumber => new(
      UsSocialSecurityNumberErrorType.InvalidAreaNumber,
      Messages.UsSsnInvalidAreaNumber);

   /// <summary>
   ///   Error that indicates that the group number segment of the SSN is 
   ///   invalid.
   /// </summary>
   public static UsSocialSecurityNumberError InvalidGroupNumber => new(
      UsSocialSecurityNumberErrorType.InvalidGroupNumber, 
      Messages.UsSsnInvalidGroupNumber);

   /// <summary>
   ///   Get an error that indicates that the string SSN contains a non-digit 
   ///   character.
   /// </summary>
   public static UsSocialSecurityNumberError InvalidCharacter(String message)
      => new(UsSocialSecurityNumberErrorType.InvalidCharacter, message);

   /// <summary>
   ///   Error that indicates that the string SSN has an invalid length.
   /// </summary>
   public static UsSocialSecurityNumberError InvalidLength => new(
      UsSocialSecurityNumberErrorType.InvalidLength,
      Messages.UsSsnInvalidLength);

   /// <summary>
   ///   Error that indicates that the string SSN contained a run of 9 
   ///   consecutive digits (123456789);
   /// </summary>
   public static UsSocialSecurityNumberError InvalidRun => new(
      UsSocialSecurityNumberErrorType.InvalidRun,
      Messages.UsSsnInvalidRun);

   /// <summary>
   ///   Get an error that indicates that the string SSN contains an ASCII digit
   ///   character in a separator character location.
   /// </summary>
   public static UsSocialSecurityNumberError InvalidSeparatorCharacter(String message)
      => new(UsSocialSecurityNumberErrorType.InvalidSeparatorCharacter, message);

   /// <summary>
   ///   Error that indicates that the serial number segment of the SSN is 
   ///   invalid.
   /// </summary>
   public static UsSocialSecurityNumberError InvalidSerialNumber => new(
      UsSocialSecurityNumberErrorType.InvalidSerialNumber,
      Messages.UsSsnInvalidSerialNumber);
}
