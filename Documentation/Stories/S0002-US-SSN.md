# S0002-US-SSN

As a developer, I want a business object that represents a US Social Security Number (SSN).

# Social Security Number Details

* A US SSN is a nine digit number, in the format AAA-GG-SSSS (AAA = area number, GG = group number, SSSS = serial number; refer to the linked Wikipedia article for more details)
* SSNs do not contain a check digit

# Links

https://en.wikipedia.org/wiki/Social_Security_number

# Validation Rules

Not all nine digit sequences are valid SSNs

* Not null, empty or all whitespace characters
* Length 9 (no separator characters) or 11 (includes separator characters)
* All non-separator characters are ASCII digits
* If length 11, then the characters in positions 3 and 6 (zero-based index) must be valid separator characters.
* Area numbers in the 900-999 range are reserved for Individual Taxpayer Identification Numbers
* Area, group or serial numbers that are all zeros (000-##-####, ###-00-####, ###-##-0000) are not allowed
* Area number 666 is not allowed
* Nine identical digits are not allowed
* The sequence 123456789 is not allowed

# Requirements

* Create a new business object named UsSocialSecurityNumber
* UsSocialSecurityNumber will exist in namespace KfAccountNumbers.Governmental.NorthAmerica
* UsSocialSecurityNumber will have a public constructor that accepts a String and an optional separator character that defaults to '-'. The constructor will apply the validation rules described in [Validation Rules](#validation-rules). If any of the validation rules are failed then the constructor will throw an InvalidUsSocialSecurityNumberException. The exception will contain an enum and message that describes the error encountered. 
* UsSocialSecurityNumber will have a public static method named Create that accepts a String and an optional separator character that defaults to '-'. The Create method will use the Result pattern to return either a valid UsSocialSecurityNumber or an enum value that identifies the validation rule that failed.
* UsSocialSecurityNumber will have a public static method named Validate that accepts a String and an optional 
separator character that defaults to '-'. The Valdate method will apply all of the validation rules described in [Validation Rules](#validation-rules) and return an enum value that indicates if the String is a valid SSN or if not, what validation rule was failed.
* UsSocialSecurityNumber will be read-only
* UsSocialSecurityNumber will have an implicit conversion from String. No separator character other than the default ('-') is allowed and the conversion will throw an exception if any validation rules are failed
* UsSocialSecurityNumber will have an implicit conversion to String. The result will be 9 characters in length
* UsSocialSecurityNumber will have a ToString method that returns a String of length 9
* UsSocialSecurityNumber will have a Format method that applies a mask to the value and returns a String (details to follow)

# Deliverables

* New UsSocialSecurityNumber business object
* New UsSocialSecurityNumberValidationResult enum with the following values:
	- ValidationPassed = 1
	-	Empty
	- InvalidLength
	- InvalidSeparatorEncountered
	- InvalidCharacterEncountered
	- InvalidAreaNumber
	- InvalidGroupNumber
	- InvalidSerialNumber
	- AllIdenticalDigits
	- InvalidRun
* New InvalidUsSocialSecurityNumberException class
* Unit tests that demonstrate that all of the requirements are met
* Readme updates