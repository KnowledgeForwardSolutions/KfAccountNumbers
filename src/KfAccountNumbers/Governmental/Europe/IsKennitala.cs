// Ignore Spelling: Kennitala

namespace KfAccountNumbers.Governmental.Europe;

public record IsKennitala
{
   /// <summary>
   ///   Represents the day offset used to distinguish personal (Einstaklingur)
   ///   kennitala values from company (Fyrirtaeki) kennitala values.
   /// </summary>
   /// <remarks>
   ///   For Icelandic kennitala numbers, a Fyrirtaeki kennitala is indicated by
   ///   adding 40 to the day component of the date of birth.
   /// </remarks>
   public const Int32 FyrirtaekiDayOffset = 40;

   /// <summary>
   ///   The latest year of birth supported by <see cref="IsKennitala"/>.
   /// </summary>
   public const Int32 MaximumValidYearOfBirth = 2099;

   /// <summary>
   ///   The earliest year of birth supported by <see cref="IsKennitala"/>.
   /// </summary>
   public const Int32 MinimumValidYearOfBirth = 1900;

   private const Int32 NoSeparatorLength = 10;
   private const Int32 SeparatorLength = 11;

   private const Int32 SeparatorOffset = 6;

   /// <summary>
   ///   Check the <paramref name="kennitala"/> to determine if it contains a
   ///   valid Icelandic kennitala number.
   /// </summary>
   /// <param name="kennitala">
   ///   String representation of an Icelandic kennitala number.
   /// </param>
   /// <returns>
   ///   A <see cref="IsKennitala"/> enumeration 
   ///   value that indicates if the <paramref name="kennitala"/> passed
   ///   validation or what validation error was encountered.
   /// </returns>
   public static IsKennitalaValidationResult Validate(String? kennitala)
   {
      if (String.IsNullOrWhiteSpace(kennitala))
      {
         return IsKennitalaValidationResult.Empty;
      }
      else if (kennitala.Length is not NoSeparatorLength and not SeparatorLength)
      {
         return IsKennitalaValidationResult.InvalidLength;
      }

      throw new NotImplementedException();
   }
}
