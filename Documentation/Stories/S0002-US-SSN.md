# S0002-US-SSN

As a developer, I want a business object that represents a US Social Security Number (SSN).

# Social Security Number Details

* A US SSN is a nine digit number, in the format AAA-GG-SSSS (AAA = area number, GG = group number, SSSS = serial number; refer to the linked Wikipedia article for more details)
* Not all nine digit sequences are valid SSNs
	* Area numbers in the 900-999 range are reserved for Individual Taxpayer Identification Numbers
	* Area, group or serial numbers that are all zeros (000-##-####, ###-00-####, ###-##-0000) are not allowed
	* Area number 666 is not allowed
	* Nine identical digits are not allowed
	* The sequence 123456789 is not allowed
* SSNs do not contain a check digit

# Links

https://en.wikipedia.org/wiki/Social_Security_number

# Requirements

* Create a new business object named UsSocialSecurityNumber
* UsSocialSecurityNumber will exist in namespace KfAccountNumbers.Governmental.NorthAmerica
* UsSocialSecurityNumber will have a public constructor that accepts a ReadOnlySpan<Char> and an optional separator character that defaults to '-'. The ReadOnlySpan<Char> parameter must be of length 9 or 11. If length = 9, then no format characters are allowed. If length is 11, then the separator character is expected in positions 3 and 6 (zero-based index). The constructor will validate the SSN for length, all numeric digits (excluding allowed format character) and that the number meets all of the details described above. If the SSN is invalid then the constructor will throw an ArgumentException with a message that describes the first validation rule that fails
* UsSocialSecurityNumber will have a public static method named Create that accepts a ReadOnlySpan<Char> and an optional format character that defaults to '-'. The CreateMethod will use the Result pattern to return either a valid UsSocialSecurityNumber or an error object containing a collection of validation rule failures. The validation rule failures collection will contain entries for each validation rule failed, not just the first rule failure encountered
* UsSocialSecurityNumber will be read-only
* UsSocialSecurityNumber will have an implicit conversion from ReadOnlySpan<Char>. No format character is allowed and the conversion will throw an exception if any validation rules are failed
* UsSocialSecurityNumber will have an implicit conversion to ReadOnlySpan<Char>. The result will be 9 characters in length
* UsSocialSecurityNumber will have a ToString method that returns a String of length 9
* UsSocialSecurityNumber will have a Format method that applies a mask to the value and returns a String (details to follow)

# Deliverables

* New UsSocialSecurityNumber business object
* Unit tests that demonstrate that all of the requirements are met
* Readme updates