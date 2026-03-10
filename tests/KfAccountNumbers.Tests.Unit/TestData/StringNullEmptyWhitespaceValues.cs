namespace KfAccountNumbers.Tests.Unit.TestData;

public sealed class StringNullEmptyWhitespaceValues : IEnumerable<TheoryDataRow<String>>
{
   public IEnumerator<TheoryDataRow<String>> GetEnumerator()
   {
      yield return new TheoryDataRow<String>(null!);
      yield return new TheoryDataRow<String>(String.Empty);
      yield return new TheoryDataRow<String>("\t");
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
