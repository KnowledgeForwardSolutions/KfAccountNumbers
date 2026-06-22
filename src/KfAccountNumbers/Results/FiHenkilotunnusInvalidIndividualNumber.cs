namespace KfAccountNumbers.Results;

/// <summary>
///   The FiHenkilotunnusInvalidIndividualNumber struct represents a validation
///   error indicating that a Finnish henkilötunnus value contains an invalid
///   individual number component.
/// </summary>
/// <param name="Description">
///   Message describing the validation error, indicating that an invalid
///   individual number was found in the input value.
/// </param>
/// <param name="IndividualNumber">
///   The invalid individual number that was found in the input value.
/// </param>
public readonly record struct FiHenkilotunnusInvalidIndividualNumber(
   String Description,
   String IndividualNumber);
