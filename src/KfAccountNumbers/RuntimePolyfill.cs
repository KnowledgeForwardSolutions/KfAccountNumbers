namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
   AllowMultiple = false)]
public sealed class UnionAttribute : Attribute;

public interface IUnion
{
   Object? Value { get; }
}
