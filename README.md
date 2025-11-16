# KfAccountNumbers

KfAccountNumbers is a collection of strongly typed business objects for a wide range of government and commercial account numbers (ex. US Social Security Number, UK National Insurance Number, etc.).

The business objects in KfAccountNumbers all have the following capabilities:

* A constructor that accepts a string representation of the account number. The constructor will throw an exception if the string value is invalid.
* A static Validate method that accepts a string representation of the account number and that returns an enum value that indicates if the string value is valid or the validation rule for the account number that was failed.
* A static Create method that accepts a string representation of the account number and that uses the result pattern to return either an instance of the account number business object or an enum value that indicates the validation rule that was failed.
* Implicit conversion to/from string.

# Namespace Hierarchy

KfAccountNumbers groups business objects into two broad categories: Commercial and Governmental. The Commercial namespace will contain common types (such as credit card numbers) that are international in scope. The Governmental namespace will contain types for account numbers issued by government authorities (such as US Social Security Numbers, etc.). The Governmental namespace is further subdivided by continent (Africa, Asia, Australia, Europe, North America and South America). The types are named using the two letter ISO country code and the account number name (ex. UsSocialSecurityNumber).

* Commercial (future)
* Governmental
	- Africa (future)
	- Asia (future)
	- Australia (future)
	- Europe (future)
	- NorthAmerica
	- South America
* Utility

# Business Objects

## UsSocialSecurityNumber

UsSocialSecurityNumber represents a Social Security Number (SSN) issued by the US Social Security Administration.

A US SSN consists of 9 digits, arranged in three groups (AAA-GG-SSSS). The first three digits are the area number, the second two digits are the area number and the final four digits are the serial number. Originally, the area number was tied to a geographic region and the group number represented a sub-grouping within the area association to a geographic region was eliminated in 2011.

SSNs are commonly formatted with dashes separating the three groups, though spaces are sometimes used. The UsSocialSecurityNumber constructor will accept either 9 character strings (all digits) or eleven character strings that include separator characters. The default separator character is dash ('-'), though the default can be overridden by any non-digit character.

Not all 9 digit numbers are valid SSNs. A valid SSN must meet all of the following rules:

* Must consist of 9 integer digits
* The area number must not be 000, 666 or 900-999. Area numbers 000 and 666 are never issued by the Social Security Administration and area numbers 900-999 are reserved for use as Individual Taxpayer Identification Numbers issued by the US Internal Revenue Service.
* The group number must not be 00.
* The serial number must not be 0000.
* The SSN must not be 9 identical digits (i.e. not 222-22-2222).
* The SSN must not be a consecutive run of digits from 1 to 9 (i.e. not 123-45-6789).

Note that meeting the above rules is not a guarantee that the value is considered a valid SSN issued by the Social Security Administration. Determining actual validity a SSN of requires use of the Social Security Number Verification Service offered by the Social Security Administration.

Note also that there are several actual SSNs that have been used in advertising campaigns or other publications and that have been rescinded by the Social Security Administration (Examples: 078-05-1120, used in 1930's era advertising campaign, 721-07-4426, 219-09-9999). KfAccountNumbers does not maintain a list of rescinded SSNs and will not detect a validation failure for those SSNs. 

See [Wikipedia - Social Security Number](https://en.wikipedia.org/wiki/Social_Security_number) for more info.
