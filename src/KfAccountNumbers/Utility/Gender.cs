namespace KfAccountNumbers.Utility;

/// <summary>
///   Defines various gender related elements.
/// </summary>
public record struct Gender
{
   /// <summary>
   ///   Represents the female gender.
   /// </summary>
   public record struct Female { }

   /// <summary>
   ///   Represents the male gender.
   /// </summary>
   public record struct Male { }

   /// <summary>
   ///   Gender identity that falls outside the binary male/female understanding
   ///   of gender.
   /// </summary>
   public record struct NonBinary { }

   /// <summary>
   ///   Specifies the gender of an individual as either male or female.
   /// </summary>
   /// <remarks>
   ///   Used in historical cases where only binary gender options are defined.
   /// </remarks>
   public union BinaryGender(Male, Female) { }
}
