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
}
