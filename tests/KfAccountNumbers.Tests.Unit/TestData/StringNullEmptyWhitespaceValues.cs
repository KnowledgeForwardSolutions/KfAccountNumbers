namespace KfAccountNumbers.Tests.Unit.TestData;

public sealed class StringNullEmptyWhitespaceValues : IEnumerable<Object[]>
{
   public IEnumerator<Object[]> GetEnumerator()
   {
      yield return [null!];
      yield return [String.Empty];
      yield return ["\t"];
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
