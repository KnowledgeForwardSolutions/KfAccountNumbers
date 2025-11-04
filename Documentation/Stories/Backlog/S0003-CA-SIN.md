# S0003-CA-SIN

As a developer, I want a business object that represents a Canadian Social Insurance Number (SIN).

# Social Insurance Number Details

* A CA SIN is a nine digit number, formatted as three groups of three digits (i.e. 123-456-789).
* The three groups are typically formatted with dashes between the groups, but other separator characters are possible (next most often is a single space).
* The trailing digit of a CA SIN is a check digits calculated using the Luhn algorithm.

# Links

https://en.wikipedia.org/wiki/Social_insurance_number

# Validation Rules

* Not null, empty or all whitespace characters
* Length 9 (no separator characters) or 11 (includes separator characters)
* All non-separator characters are ASCII digits
* If length 11, then the characters in positions 3 and 7 (zero-based index) must be valid separator characters.
* The trailing digit matches the check digit calculated from the other digits using the Luhn algorithm.

# Requirements

* Create a new business object named CaSocialInsuranceNumber
* CaSocialInsuranceNumber will exist in namespace KfAccountNumbers.Governmental.NorthAmerica
* CaSocialInsuranceNumber will have a public constructor that accepts a String and an optional separator character that defaults to '-'. The constructor will apply all of the validation rules described in [Validation Rules](#validation-rules). If any of the validation rules are failed then the constructor will throw an exception (ArgumentNullException if the string is null, otherwise ArgumentException). The exception will contain a message that describes the error encountered. 
* CaSocialInsuranceNumber will have a public static method named Create that accepts a String and an optional separator character that defaults to '-'. The Create method will use the Result pattern to return either a valid CaSocialInsuranceNumber or an enum value that identifies the validation rule that failed.
* CaSocialInsuranceNumber will have a public static method named Validate that accepts a String and an optional 
separator character that defaults to '-'. The Valdate method will apply all of the validation rules described in [Validation Rules](#validation-rules) and return an enum value that indicates if the String is a valid CA SIN or if not, what validation rule was failed.
* CaSocialInsuranceNumber will be read-only
* CaSocialInsuranceNumber will have an implicit conversion from String. No separator character other than the default ('-') is allowed and the conversion will throw an exception if any validation rules are failed
* CaSocialInsuranceNumber will have an implicit conversion to String. The result will be 9 characters in length
* CaSocialInsuranceNumber will have a ToString method that returns a String of length 9
* CaSocialInsuranceNumber will have a Format method that applies a mask to the value and returns a String

# Deliverables

* New CaSocialInsuranceNumber business object
* New CaSocialInsuranceNumberValidationResult enum with the following values:
	- ValidationPassed = 1
	-	Empty
	- InvalidLength
	- InvalidSeparatorEncountered
	- InvalidCharacterEncountered
	- InvalidCheckDigit
* Unit tests that demonstrate that all of the requirements are met
* Readme updates (including benchmark results)