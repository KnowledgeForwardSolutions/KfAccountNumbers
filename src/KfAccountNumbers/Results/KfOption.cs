namespace KfAccountNumbers.Results;

/// <summary>
///   Represents a value that may or may not be present. Will either contain an
///   instance of type <typeparamref name="TS"/> if the value is present or
///   <see cref="None"/> if the value is not present.
/// </summary>
/// <remarks>
///   <para>
///      This is a discriminated union type that supports implicit conversion
///      from either "some" or "none" values.
///   </para>
///   <para>
///      This struct explicitly implements "non-boxing access pattern" for
///      retrieving the "some" or "none" value without incurring boxing overhead.
///   </para>
/// </remarks>
/// <typeparam name="TS">
///   The type of value that may or may not be present.
/// </typeparam>
public readonly struct KfOption<TS> : IUnion
   where TS : notnull
{
   private readonly TS _some;
   private readonly None _none;

   /// <summary>
   ///   Initializes a new instance of the <see cref="KfOption{TS}"/> struct with
   ///   <paramref name="some"/>, a value other than <see cref="None"/>.
   /// </summary>
   /// <param name="some">
   ///   The expected value. This value must not be <see langword="null"/>.
   /// </param>
   /// <exception cref="ArgumentNullException">
   ///   <paramref name="some"/> is <see langword="null"/>.
   /// </exception>
   public KfOption(TS some)
   {
      _some = some ?? throw new ArgumentNullException(nameof(some));
      _none = default;
      IsSome = true;
      IsNone = false;
   }

   /// <summary>
   ///   Initializes a new instance of the <see cref="KfOption{TS}"/> struct with
   ///   <paramref name="none"/>, which indicates that no value is available.
   /// </summary>
   /// <param name="none">
   ///   Item that represents the absence of a value.
   /// </param>
   public KfOption(None none)
   {
      _none = none;
      _some = default!;
      IsNone = true;
      IsSome = false;
   }

   /// <summary>
   ///   Gets a value indicating whether this instance has a value.
   /// </summary>
   /// <remarks>
   ///   Supports the "non-boxing access pattern" for retrieving the "some" or
   ///   "none" value without incurring boxing overhead.
   /// </remarks>
   public Boolean HasValue => true;

   /// <summary>
   ///   Gets a value indicating whether this instance is None; i.e. it
   ///   represents the absence of a value.
   /// </summary>
   public Boolean IsNone { get; private init; }

   /// <summary>
   ///   Gets a value indicating whether this instance is Some; i.e. it
   ///   represents the presence of a value.
   /// </summary>
   public Boolean IsSome { get; private init; }

   /// <summary>
   ///   Gets the value or <see cref="None"/> to indicate that no value is
   ///   available.
   /// </summary>
   public Object Value => IsSome ? _some : _none;

   /// <summary>
   ///   Implicitly converts a <typeparamref name="TS"/> "some" value to an
   ///   Option.
   /// </summary>
   /// <param name="some">
   ///   The expected value. This value must not be <see langword="null"/>.
   /// </param>
   public static implicit operator KfOption<TS>(TS some) => new(some);

   /// <summary>
   ///   Implicitly converts a <see cref="None"/> instance to an Option.
   /// </summary>
   /// <param name="none">
   ///   Item that represents the absence of a value.
   /// </param>
   public static implicit operator KfOption<TS>(None none) => new(none);

   /// <summary>
   ///   Gets the "some" value if available, or the default value otherwise.
   /// </summary>
   /// <param name="some">
   ///   When this method returns, contains the "some" value if available;
   ///   otherwise, the default value of type TS.</param>
   /// <returns>
   ///   <see langword="true"/> if this instance contains the expected value;
   ///   otherwise <see langword="false"/>.
   /// </returns>
   public Boolean TryGetValue(out TS some)
   {
      some = IsSome ? _some : default!;
      return IsSome;
   }

   /// <summary>
   ///   Gets the "none" value if available, or the default value otherwise.
   /// </summary>
   /// <param name="none">
   ///   When this method returns, contains the "none" value if the Option
   ///   represents the absence of a value; otherwise, the default value.
   /// </param>
   /// <returns>
   ///   <see langword="true"/> if this instance contains "none"; otherwise
   ///   <see langword="false"/>.
   /// </returns>
   public Boolean TryGetValue(out None none)
   {
      none = IsNone ? _none : default;
      return IsNone;
   }
}
