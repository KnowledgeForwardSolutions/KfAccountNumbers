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
}
