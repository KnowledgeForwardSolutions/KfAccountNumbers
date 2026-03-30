// Ignore Spelling: Fi

namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible types of identifiers that can be represented with a
///   <see cref="FiHenkilotunnus"/> object.
/// </summary>
public enum FiIdentifierType
{
   /// <summary>
   ///   Henkilötunnus assigned to a person born in Finland or a permanent resident.
   /// </summary>
   PermanentResident = 0,

   /// <summary>
   /// Temporary henkilötunnus assigned to a person not eligible for a permanent
   /// identifier. For example, a hospital patient where the official henkilötunnus
   /// is unknown.
   /// </summary>
   Temporary = 1
}
