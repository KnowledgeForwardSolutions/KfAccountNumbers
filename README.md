# KfAccountNumbers

KfAccountNumbers is a collection of strongly typed business objects for a wide range of government
and commercial account numbers (ex. US Social Security Number, UK National Insurance Number, etc.).

The business objects in KfAccountNumbers all have the following capabilities:

* A constructor that accepts a string representation of the account number. The constructor will throw an exception if the string value is invalid.
* A static Validate method that accepts a string representation of the account number and that returns an enum value that indicates if the string value is valid or the validation rule for the account number that was failed.
* A static Create method that accepts a string representation of the account number and that uses the result pattern to return either an instance of the account number business object or an enum value that indicates the validation rule that was failed.
* Implicit conversion to/from string.

If the business object represents an account number that has a defined format (ex. US Social Security
Number, etc.), the constructor, Create and Validate methods and implicit string to business object
operator will accept either a string that consists of only the characters in the account number or a
string that includes format characters (ex. dashes, spaces, etc.) in the appropriate places. The
business object will also implement a Format method that returns a string representation of the
account number with the appropriate format characters in the appropriate places.

If the business object represents an account number that normally has no formatting other than the
raw characters of the account number then the business object constructor, Create and Validate methods
and implicit string to business object operator will only accept strings that consist of the raw
characters of the account number. Nor will the business object implement a Format method since there
is no formatting to be done.

# Namespace Hierarchy

KfAccountNumbers groups business objects into two broad categories: Commercial and Governmental. The Commercial namespace will contain common types (such as credit card numbers) that are international in scope. The Governmental namespace will contain types for account numbers issued by government authorities (such as US Social Security Numbers, etc.). The Governmental namespace is further subdivided by continent (Africa, Asia, Australia, Europe, North America and South America). The types are named using the two letter ISO country code and the account number name (ex. UsSocialSecurityNumber).

* Commercial (future)
* Governmental
	- Africa (future)
	- Asia (future)
	- Australia (future)
	- Europe (future)
	- NorthAmerica
		- [CaSocialInsuranceNumber](#casocialinsurancenumber) 
		- [MxCurp](#mxcurp)
        - [UsIndividualTaxpayerIdentificationNumber](#usindividualtaxpayeridentificationnumber)
        - [UsNationalProviderNumber](#usnationalprovidernumber)
		- [UsSocialSecurityNumber](#ussocialsecuritynumber)
	- South America
* Utility

# Business Objects

## CaSocialInsuranceNumber

CaSocialInsuranceNumber represents a Canadian Social Insurance Number (SIN) issued by the Government
of Canada.

A Canadian SIN consists of 9 digits, typically formatted as AAA AAA AAA. The CaSocialInsuranceNumber
constructor will accept either 9 character strings (all digits) or eleven character strings that
include separator characters between the three groups of digits. If used, the separator character
must be the same for both separators and must be a non-digit character.

Not all 9 digit numbers are valid SINs. A valid SIN must meet all of the following rules:

* Must consist of 9 integer digits
* Must pass the Luhn algorithm check.
* May not start with the digits 8 or 0, which are reserved for Business Numbers (8) and tax numbers assigned by the Canada Revenue Agency (0).

See [Wikipedia - Social Insurance Number](https://en.wikipedia.org/wiki/Social_Insurance_Number) for more info.

## MxCurp

MxCurp represents a Mexican Clave Única de Registro de Población (CURP) issued by the Government of Mexico,
specifically the Registry Nacional de Poblacion (RENAPO).

A Mexican CURP consists of 18 characters, in the form AAAADDDDDDGLLNNNHC (example: HEGG560427MVZRRL04,
from Wikipedia), where

* AAAA are four alphabetic characters (A-Z) derived from the person's surname(s) and given name.
* DDDDDD is the person's date of birth in YYMMDD format.
* G is the person's gender with H for hombre/male, M for mujer/female and X for non-binary.
* LL is a two alphabetic character (A-Z) code for the person's state of birth or NE (nacido en el extranjero) for persons born abroad.
* NNN are three alphabetic characters (A-Z) derived from the person's surname(s) and given name.
* H is an alphanumeric (A-Z, 0-9) homoclave character issued by RENAPO to prevent duplicate CURP values.
* C is a numeric (0-9) check digit calculated using the previous 17 characters.

The homoclave character is a digit (0-9) for people born before 2000 and a letter (A-Z) for people born
in 2000 or later. The algorithm used for the check digit is not published and the only validation
performed on the check digit is to verify that it is a digit character.

MxCurp has read-only properties for accessing the DateOfBirth, GenderCode and StateCode from the
CURP string. 

A valid CURP must meet all of the following rules (specific character offsets are zero-based):

* Must consist of 18 characters
* Characters 0-3 characters must be alphabetic (A-Z)
* Characters 3-9 must be a valid date in YYMMDD format
* Character 10 must be H, M or X
* Characters 11-12 must be a valid state code or NE (see [Appendix - MxCurp list of valid state codes](#mxcurp-list-of-valid-state-codes))
* Characters 13-15 must be alphabetic (A-Z)
* Character 16 must be alphanumeric (A-Z, 0-9)
* Character 17 must be a digit (0-9)

MxCurp is case-insensitive for validation and parsing purposes. The MxCurp constructor, Create
method and implicit string to MxCurp operator will convert any lowercase letters to uppercase.

Note that the homoclave value is used to determine the century of birth. This has two implications.
First, the DateOfBirth property will return a DateOnly value with a year in the range 1900-1999 for
homoclave values of 0-9 and a year in the range 2000-2099 for homoclave values of A-Z. Second, the
date of birth value 000229 will be considered invalid for homoclave values of 0-9 (indicating a birth
date of February 29, 1900) but valid for homoclave values of A-Z (indicating a birth date of
February 29, 2000 because 1900 is not a leap year but 2000 is a leap year. Also note that date of
birth validation does not check for future dates, so a CURP with a date of birth in the future could
be considered valid if it meets all of the other validation rules.

See [Wikipedia - Unique Population Registry Code](https://en.wikipedia.org/wiki/Unique_Population_Registry_Code) and
[Wikipedia - Clave Única de Registro de Población](https://es.wikipedia.org/wiki/Clave_%C3%9Anica_de_Registro_de_Poblaci%C3%B3n) for more info.

## UsIndividualTaxpayerIdentificationNumber

UsIndividualTaxpayerIdentificationNumber represents an Individual Taxpayer Identification Number (ITIN)
issued by the US Internal Revenue Service (IRS). ITINs are used for tax processing purposes for
individuals who are not eligible for a Social Security Number (SSN) but who have a tax filing
requirement in the United States.

A US ITIN consists of 9 digits, arranged in the format 9XX-XX-XXXX. The first digit is always a 9,
the fourth and fifth digits are in the range 50-65, 70-88, 90-92 or 94-99 and the remaining digits can be any
decimal digit. The UsIndividualTaxpayerIdentificationNumber constructor will accept either 9 character
strings (all digits) or eleven character strings that include separator characters between the three
groups. If used, the separator character must be the same for both separators and must be a non-digit
character.

A valid ITIN must meet all of the following rules:

* Must consist of 9 integer digits
* The first digit must be 9.
* The fourth and fifth digits must be in the range 0-65, 70-88, 90-92 or 94-99

Note that meeting the above rules is not a guarantee that the value is considered a valid ITIN issued
by the IRS. Determining actual validity of an ITIN requires use of the IRS's Taxpayer Identification
Number (TIN) Matching Program or other IRS services.

See [Wikipedia - Individual Taxpayer Identification Number](https://en.wikipedia.org/wiki/Individual_Taxpayer_Identification_Number) for more info.

## UsNationalProviderNumber

UsNationalProviderNumber represents a National Provider Identifier (NPI) issued to health care
providers by the US Centers for Medicare & Medicaid Services (CMS). The NPI is used in administrative
and billing transactions within the U.S. healthcare system.

A US NPI consists of 10 digits, without any formatting characters. The trailing (right-most) digit
is a check digit calculated using the Luhn algorithm with a prefix of '80840' added to the left of
the NPI value.

A valid US NPI must meet all of the following rules:

* Must consist of 10 integer digits
* Must pass the Luhn algorithm check with a prefix of '80840' added to the left of the NPI value.

See [Wikipedia - National Provider Identifier](https://en.wikipedia.org/wiki/National_Provider_Identifier) for more info.

## UsSocialSecurityNumber

UsSocialSecurityNumber represents a Social Security Number (SSN) issued by the US Social Security
Administration.

A US SSN consists of 9 digits, arranged in three groups (AAA-GG-SSSS). The first three digits are the
area number, the second two digits are the group number and the final four digits are the serial number.
Originally, the area number was tied to a geographic region and the group number represented a
sub-grouping within the area association to a geographic region was eliminated in 2011.

SSNs are commonly formatted with dashes separating the three groups, though spaces are sometimes used.
The UsSocialSecurityNumber constructor will accept either 9 character strings (all digits) or eleven
character strings that include separator characters between the three groups. If used, the separator
character must be the same for both separators and must be a non-digit character.

Not all 9 digit numbers are valid SSNs. A valid SSN must meet all of the following rules:

* Must consist of 9 integer digits
* The area number must not be 000, 666 or 900-999. Area numbers 000 and 666 are never issued by the Social Security Administration and area numbers 900-999 are reserved for use as Individual Taxpayer Identification Numbers issued by the US Internal Revenue Service.
* The group number must not be 00.
* The serial number must not be 0000.
* The SSN must not be 9 identical digits (i.e. not 222-22-2222).
* The SSN must not be a consecutive run of digits from 1 to 9 (i.e. not 123-45-6789).

Note that meeting the above rules is not a guarantee that the value is considered a valid SSN issued
by the Social Security Administration. Determining actual validity a SSN of requires use of the
Social Security Number Verification Service offered by the Social Security Administration.

Note also that there are several actual SSNs that have been used in advertising campaigns or other
publications and that have been rescinded by the Social Security Administration (Examples: 078-05-1120,
used in 1930's era advertising campaign, 721-07-4426, 219-09-9999). KfAccountNumbers does not maintain
a list of rescinded SSNs and will not detect a validation failure for those SSNs. 

See [Wikipedia - Social Security Number](https://en.wikipedia.org/wiki/Social_Security_number) for more info.

# Appendices

## MxCurp list of valid state codes

| State Code | State Name              |
|------------|-------------------------|
| AS         | Aguascalientes          |
| BC         | Baja California         |
| BS         | Baja California Sur     |
| CC         | Campeche                |
| CL         | Coahuila de Zaragoza    |
| CM         | Colima                  |
| CS         | Chiapas                 |
| CH         | Chihuahua               |
| DF         | Ciudad de México        |
| DG         | Durango                 | 
| GT         | Guanajuato              | 
| GR         | Guerrero                | 
| HG         | Hidalgo                 | 
| JC         | Jalisco                 | 
| MC         | México                  | 
| MN         | Michoacán de Ocampo     | 
| MS         | Morelos                 | 
| NT         | Nayarit                 | 
| NL         | Nuevo León              | 
| OC         | Oaxaca                  | 
| PL         | Puebla                  | 
| QT         | Querétaro               | 
| QR         | Quintana Roo            | 
| SP         | San Luis Potosí         | 
| SL         | Sinaloa                 | 
| SR         | Sonora                  | 
| TC         | Tabasco                 | 
| TS         | Tamaulipas              | 
| TL         | Tlaxcala                | 
| VZ         | Veracruz                | 
| YN         | Yucatán                 | 
| ZS         | Zacatecas               | 
| NE         | Nacido en el Extranjero | 
