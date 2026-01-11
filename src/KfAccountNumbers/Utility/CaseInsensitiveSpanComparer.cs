namespace KfAccountNumbers.Utility;

/// <summary>
///   A case-insensitive, culture insensitive comparer for <see cref="String"/>,
///   with support for alternate key type of <see cref="ReadOnlySpan{Char}"/>.
/// </summary>
public class CaseInsensitiveSpanComparer :
   IEqualityComparer<String>,
   IAlternateEqualityComparer<ReadOnlySpan<Char>, String>
{
   public Boolean Equals(String? x, String? y)
      => String.Equals(x, y, StringComparison.OrdinalIgnoreCase);

   public Int32 GetHashCode(String obj)
      => StringComparer.OrdinalIgnoreCase.GetHashCode(obj);

   public Boolean Equals(ReadOnlySpan<Char> alternateKey, String mainKey)
      => alternateKey.Equals(mainKey.AsSpan(), StringComparison.OrdinalIgnoreCase);

   public Int32 GetHashCode(ReadOnlySpan<Char> alternateKey)
      => String.GetHashCode(alternateKey, StringComparison.OrdinalIgnoreCase);

   public String Create(ReadOnlySpan<Char> alternateKey) => alternateKey.ToString();
}
