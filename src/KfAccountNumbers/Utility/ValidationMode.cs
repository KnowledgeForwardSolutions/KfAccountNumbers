namespace KfAccountNumbers.Utility;

/// <summary>
///   Specifies the validation behavior to use in the private constructor of
///   a KfAccountNumbers business object.
/// </summary>
/// <remarks>
///   Use this enumeration to indicate whether validation should be performed
///   or bypassed based on prior validation. Selecting
///   <see cref="ValidationRequired"/> enforces validation, while
///   <see cref="BypassValidation"/> allows skipping validation when it is known
///   that the value has already been validated.
/// </remarks>
internal enum ValidationMode
{
   /// <summary>
   ///   Default - validation is needed.
   /// </summary>
   ValidationRequired = 0,

   /// <summary>
   ///   Safe to bypass constructor validation because value was previously
   ///   validated.
   /// </summary>
   BypassValidation,
}
