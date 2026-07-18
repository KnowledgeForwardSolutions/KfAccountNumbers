namespace KfAccountNumbers.Utility;

/// <summary>
///   Internal utility object to define and extract a segment of a larger value.
///   Example use: extract the area, group and serial number segments of a US
///   Social Security Number.
/// </summary>
/// <param name="Start">
///   The segment start offset.
/// </param>
/// <param name="End">
///   The segment end offset. Note that a range end is exclusive of the end
///   offset.
/// </param>
/// <remarks>
///   This is for internal use only and does not include any validation of the
///   input values.
/// </remarks>
internal sealed record SegmentRange(Int32 Start, Int32 End)
{
   /// <summary>
   ///   Extract the segment from the supplied <paramref name="value"/>.
   /// </summary>
   /// <param name="value">
   ///   The value that contains the segment to extract.
   /// </param>
   /// <returns>
   ///   The portion of <paramref name="value"/> that is defined by this
   ///   <see cref="SegmentRange"/>.
   /// </returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ReadOnlySpan<Char> Extract(ReadOnlySpan<Char> value) => value[Start..End];

   /// <summary>
   ///   Validate that the segment consists of only ASCII letters (upper-case or
   ///   lower-case).
   /// </summary>
   /// <param name="value">
   ///   The value that contains the segment to validate.
   /// </param>
   /// <param name="invalidCharacterPosition">
   ///   Out parameter. The zero-based index (relative to the entire
   ///   <paramref name="value"/>, not just the segment) of the first non-ASCII
   ///   letter found in the segment or -1 if all characters in the segment are
   ///   ASCII letters.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if the segment only contains ASCII letters;
   ///   otherwise <see langword="false"/>.
   /// </returns>
   public Boolean ValidateAllAsciiLetters(
      ReadOnlySpan<Char> value,
      out Int32 invalidCharacterPosition)
   {
      for (var index = Start; index < End; index++)
      {
         if (!value[index].IsAsciiLetter())
         {
            invalidCharacterPosition = index;
            return false;
         }
      }

      invalidCharacterPosition = -1;
      return true;
   }
}
