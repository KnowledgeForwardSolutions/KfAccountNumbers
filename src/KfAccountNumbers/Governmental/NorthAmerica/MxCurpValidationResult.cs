// Ignore Spelling: Curp Mx

namespace KfAccountNumbers.Governmental.NorthAmerica;

/// <summary>
///   Defines the possible results returned when validating a Mexican Clave
///   Unica de Registro de Poblacion (CURP).
/// </summary>
public enum MxCurpValidationResult
{
   /// <summary>
   ///   The value does not contain any validation errors.
   /// </summary>
   ValidationPassed = 1,

   /// <summary>
   ///   CURP value is <see langword="null"/>, <see cref="String.Empty"/> or all
   ///   whitespace characters.
   /// </summary>
   Empty,

   /// <summary>
   ///   CURP value has incorrect length. Must be either 18 characters in length.
   /// </summary>
   InvalidLength,

   /// <summary>
   ///   Zero-based character positions 0-3 or 13-15 of the CURP value are not 
   ///   alphabetic. Characters in these positions are derived from the person's
   ///   surname(s) and first given name.
   /// </summary>
   InvalidAlphabeticCharacterEncountered,

   /// <summary>
   ///   Character positions 4-9 (zero-based) are not a valid 6 digit date of 
   ///   birth (YYYYMMDD).
   /// </summary>
   InvalidDateOfBirth,

   /// <summary>
   ///   Character position 10 (zero-based) is not a valid gender character. It 
   ///   must be H (Hombre), M (Mujer) or X (non-binary).
   /// </summary>
   InvalidGender,

   /// <summary>
   ///   Character positions 11-12 (zero-based) are not a valid state code.
   /// </summary>
   InvalidState,

   /// <summary>
   ///   Character position 16 (zero-based) is not a valid homoclave character. 
   ///   Homeclave characters are alphanumeric characters (A-Z, 0-9) issued by 
   ///   RENAPO (Registro Nacional de Población) to avoid duplicate CURP values.
   /// </summary>
   InvalidHomoclave,
   /// <summary>
   ///   Character position 17 (zero-based) is not a valid check digit character. 
   ///   The check digit algorithn used for CURP is not published by RENAPO, but 
   ///   it is known that it is a single digit character (0-9).
   /// </summary>
   InvalidCheckDigit
}

/// <summary>
///   Additional methods for type <see cref="MxCurpValidationResult"/>.
/// </summary>
public static class MxCurpValidationResultExtensions
{
   public static String ToErrorDescription(this MxCurpValidationResult errorType)
      => errorType switch
      {
         MxCurpValidationResult.Empty => Messages.MxCurpEmpty,
         MxCurpValidationResult.InvalidLength => Messages.MxCurpInvalidLength,
         MxCurpValidationResult.InvalidAlphabeticCharacterEncountered => Messages.MxCurpInvalidAlphabeticCharacterEncountered,
         MxCurpValidationResult.InvalidDateOfBirth => Messages.MxCurpInvalidDateOfBirth,
         MxCurpValidationResult.InvalidGender => Messages.MxCurpInvalidGender,
         MxCurpValidationResult.InvalidState => Messages.MxCurpInvalidState,
         MxCurpValidationResult.InvalidHomoclave => Messages.MxCurpInvalidHomoclave,
         MxCurpValidationResult.InvalidCheckDigit => Messages.MxCurpInvalidCheckDigit,
         MxCurpValidationResult.ValidationPassed => Messages.ValidationPassed,
         _ => throw new SwitchExpressionException()
      };
}