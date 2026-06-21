namespace KfAccountNumbers.Governmental.Europe;

/// <summary>
///   Defines the possible types of identifiers that can be represented with a
///   <see cref="IsKennitala"/> object.
/// </summary>
public class IsIdentifierType
{
   /// <summary>
   ///   Personal identifier.
   /// </summary>
   public struct Einstaklingur { }

   /// <summary>
   ///   Company identifier. Same format as for Einstaklingur, except that
   ///   40 is added to the day component of the day of birth (i.e. 130585
   ///   becomes 530585).
   /// </summary>
   public struct Fyrirtaeki { }
}
