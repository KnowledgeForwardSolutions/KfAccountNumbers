# Coding Guidelines

Some basic guidelines to promote code uniformity.

## General Guidelines

* Prefer .Net system types over C# language types to promote consistent use of
   Pascal casing for all typenames. I.e. prefer Int32 over int; Double over double.
* Prefer `var` for built-in and apparent types
* Use C# 10 global usings and file-scoped namespaces to reduce code boilerplate.
* Use 3-space indentation, UTF-8, LF line endings
* Add XML code comments for all public methods of interfaces and public classes.
   If a class implements an interface, do not duplicate the XML comments in the
   interface, use the `inheritdoc` tag instead. Also update README.md, including examples
   of usage.
* Include benchmarks and include results in README.

## Validation

When writing validation methods for business objects, the validation should generally
proceed in the following order:

* Test for null, empty or all whitespace characters
* Test for length
* If the business object uses a check digit, then test the check digit next since
 that will detect the most common data entry errors
* Test other elements specific to the business object

## Unit Tests/Naming Conventions

* Tests use xUnit framework.
* Tests use FluentAssertions for assertions. Use `Version="[7.0.0]"` in the test project .csproj to lock down the version to the final free version of FluentAssertions instead of the later commercial versions.

Unit test methods use the following naming convention: 'aaa'_'bbb'_Should'ccc'_When'ddd' where:

* aaa : Class name
* bbb : method name
* ccc : expected result
* ddd : condition being tested

For example, when testing the NextFoo method of class FooBar we would use the following
unit test names.

```text
   FooBar_NextFoo_ShouldReturnFalse_WhenNoFoosRemaining
   FooBar_NextFoo_ShouldReturnTrue_WhenOnlyOneFoo
   FooBar_NextFoo_ShouldThrowFooException_WhenInvokedAfterEndOfList
```

The '_When' component may be dropped in cases where forcing the test name to strictly
adhere to the naming convention results in a test name that is less clear than without
the '_When'. For example, instead of:

```text
    CheckDigitAlgorithm_Validate_ShouldReturnTrue_WhenCharacterWeightsAreCorrectlyApplied
```

use

```text
    CheckDigitAlgirithm_Validate_ShouldApplyCorrectWeightsToEachCharacter
```

Use the name "value" for a string being passed to the object's constructor, Create or Validate method.
Use the term "sut" (for system under test) for the item being tested.

```text
public void SePersonNummer_Constructor_ShouldCreateInstance_WhenValueIsValid(String value)
{
    // Act.
    var sut = new SePersonnummer(value);

    // Assert.
    sut.Should().NotBeNull();
    ...
}

public void CaSocialInsuranceNumber_EqualityOperator_ShouldReturnFalse_WhenValuesAreNotEqual()
{
    // Arrange.
    var sut1 = new CaSocialInsuranceNumber(ValidNineCharSin);
    var sut2 = new CaSocialInsuranceNumber(AltValidNineCharSin);

    // Act/assert.
    (sut1 == sut2).Should().BeFalse();
}
```
