// Ignore Spelling: Fi

namespace KfAccountNumbers.National.Europe;

/// <summary>
///   Defines the possible types of identifiers that can be represented with a
///   <see cref="FiHenkilotunnus"/> object.
/// </summary>
public class FiIdentifierType
{
   /// <summary>
   ///   Henkilötunnus assigned to a person born in Finland or a permanent resident.
   /// </summary>
   public struct PermanentResident { }

   /// <summary>
   /// Temporary henkilötunnus assigned to a person not eligible for a permanent
   /// identifier. For example, a hospital patient where the official henkilötunnus
   /// is unknown.
   /// </summary>
   public struct Temporary { }
}
