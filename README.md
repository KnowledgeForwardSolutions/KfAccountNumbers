# KfAccountNumbers

KfAccountNumbers is a collection of strongly typed business objects for a wide range of government
and commercial account numbers (ex. US Social Security Number, UK National Insurance Number, etc.).

The business objects in KfAccountNumbers all have the following capabilities:

* A constructor that accepts a string representation of the account number. The constructor will throw an exception if the string value is invalid.
* A static Validate method that accepts a string representation of the account number and that returns an enum value that indicates if the string value is valid or the validation rule for the account number that was failed.
* A static Create method that accepts a string representation of the account number and that uses the result pattern to return either an instance of the account number business object or an enum value that indicates the validation rule that was failed.
* Implicit conversion to string and explicit conversion from string.

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

Note that many of the national identifiers supported by KfAccountNumbers embed the person's date of
birth in the identifier. KfAccountNumbers will always validate these dates, but only that the date
exists, and specifically will **NOT** check for future dates. This is to prevent any of the
business objects being required to be aware of the current date/time. If preventing future dates is
a business requirement then you should perform your own validation of the business object's DateOfBirth
property and reject it if the date is in the future.

# Namespace Hierarchy

KfAccountNumbers groups business objects into two broad categories: Commercial and Governmental. The Commercial namespace will contain common types (such as credit card numbers) that are international in scope. The Governmental namespace will contain types for account numbers issued by government authorities (such as US Social Security Numbers, etc.). The Governmental namespace is further subdivided by continent (Africa, Asia, Australia, Europe, North America and South America). The types are named using the two letter ISO country code and the account number name (ex. UsSocialSecurityNumber).

* Commercial (future)
* Governmental
	- Africa (future)
	- Asia (future)
	- Australia (future)
	- Europe
        - [DkPersonnummer](#dkpersonnummer) 
        - [IsKennitala](#iskennitala) 
        - [NoFoedselsnummer](#nofoedselsnummer) 
        - [SePersonnummer](#sepersonnummer)
	- NorthAmerica
		- [CaSocialInsuranceNumber](#casocialinsurancenumber) 
		- [MxCurp](#mxcurp)
        - [UsIndividualTaxpayerIdentificationNumber](#usindividualtaxpayeridentificationnumber)
        - [UsNationalProviderIdentifier](#usnationalprovideridentifier)
		- [UsSocialSecurityNumber](#ussocialsecuritynumber)
	- South America
* Utility

# Business Objects

## CaSocialInsuranceNumber

The `CaSocialInsuranceNumber` type represents a Social Insurance Number (SIN) issued by the Government
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

## DkPersonnummer

The `DkPersonnummer` type represents the national identification number used by Denmark. The Danish
personnummer is often informally called a CPR-nummer.

A Danish personnummer is a ten-digit number structured as DDMMYYSSSS, with the following elements:
* DDMMYY - the person's date of birth in DDMMYY format.
* SSSS - a four digit sequence number used to differentiate between two persons born on the same date.
 The sequence number also encodes additional information. The first digit is used to indicate the century
 of birth (see below) and the final digit indicates the person's gender, with even numbers for females
 and odd numbers for males.

A Danish personnummer may be formatted as a string of 10 consecutive digits (DDMMYYSSSS) or as 11 characters with
a dash ('-') as a separator character separating the date of birth and the remaining four digits (DDMMYY-SSSS).

A Danish personnummer must meet all of the following rules:
* The value may not be null, empty or all whitespace characters.
* The value must be either 10 characters (without separator) or 11 characters (with separator) in length.
* All characters other than the optional separator character must be ASCII digits ('0'-'9').
* The separator character, if included, must be a dash ('-').
* The date of birth, after deriving the century from the century indicator must be a valid date
 between January 1, 1858 and December 31, 2057.

The trailing (right-most) digit of the personnummer was originally a modulus 11 check digit. However, in 2007
the use of the check digit was discontinued since available numbers for several dates were exhausted
(especially January 1, which was often used in cases where immigrants did not know their exact date of
birth). `DkPersonnummer` does not validate a check digit since it is not possible to determine if the
personnummer was issued pre- or post-2007.

Example values:
* 070761-4285 - Date of birth July 7, 1961, gender = male
* 0102036234 - Date of birth February 1, 2003, gender = female

The digit in the seventh character position (zero-based) is used to determine the exact century of
birth, but some digits can span more than one century. The following rules are defined:
* Century indicator = 0-3, then century = 1900
* Century indicator = 4 **AND** year <= 36, then century = 2000
* Century indicator = 4 **AND** year >= 37, then century = 1900
* Century indicator = 5-8 **AND** year <= 57, then century = 2000
* Century indicator = 5-8 **AND** year >= 58, then century = 1800
* Century indicator = 9 **AND** year <= 36, then century = 2000
* Century indicator = 9 **AND** year >= 37, then century = 1900

According to these rules, the valid range for a date of birth is January 1, 1858 to December 31 2057.

See [Wikipedia - Personal identification number (Denmark)](https://en.wikipedia.org/wiki/Personal_identification_number_%28Denmark%29)
and [CPR-nummer](https://da.wikipedia.org/wiki/CPR-nummer) for more info.

## IsKennitala

The `IsKennitala` type represents the national identification number used by Iceland. A kennitala may
be issued to an individual or to a company (see below). `IsKennitala` includes an `IdentifierType`
property which returns an `IsIdentifierType` enumeration value that indicates if the number is assigned
to an individual (Einstaklingur) or to a company (Fyrirtæki).

An Icelandic kennitala is a ten-digit number structured as DDMMYYRRPC, with the following elements:
* DDMMYY - the person's date of birth (for individuals) or the date of registration (for companies) in DDMMYY format.
 The only difference between an Einstaklingur kennitala and a Fyrirtæki kennitala is that 40 is added to the day
 component of the date of birth for the Fyrirtæki kennitala (i.e. 130585 becomes 530585). Day values in the range
 41-71 (inclusive) indicate a Fyrirtæki kennitala.
* RR - two random digits used to differentiate between two persons born on the same date.
* P - a check digit calculated for the DDMMYYRR digits using a weighted modulus 11 algorithm.
* C - a single digit indicating the century of birth. Valid digits are 9 (1900s) and 0 (2000s).

A kennitala may be formatted as a string of 10 consecutive digits (DDMMYYRRPC) or as 11 characters with a
separator character, generally a dash ('-'), separating the date of birth and the remaining four digits (DDMMYY-RRPC).

A valid kennitala must meet all of the following rules:
* The value may not be null, empty or all whitespace characters.
* The value must be either 10 characters (without separator) or 11 characters (with separator) in length.
* All characters (except the optional separator character) must be ASCII digits ('0'-'9').
* The check digit must match the digit calculated using a weighted modulus 11 algorithm.
* The optional separator character, if included, may not be an ASCII digit. Any non-digit character is allowed as a separator.
* The century indicator must be the ASCII character nine ('9') or the ASCII character zero ('0').
* The date of birth, after deriving the century from the century indicator (and if the value is a Fyrirtæki kennitala,
 after subtracting the Fyrirtæki kennitala offset) must be a valid date between January 1, 1900 and December 31, 2099.

Example values:
* 1205854369 - Einstaklingur, date of birth May 12, 1985, check digit = 6
* 120585-4369 - Einstaklingur, date of birth May 12, 1985, check digit = 6
* 5311073810 - Fyrirtæki, date of registration November 13, 2007, check digit 1
* 531107 3810 - Fyrirtæki, date of registration November 13, 2007, check digit 1

See [Wikipedia - Icelandic identification number](https://en.wikipedia.org/wiki/Icelandic_identification_number)
and [kennitala.com](https://kennitala.com/) for more info.

## MxCurp

The `MxCurp` type represents a Clave Única de Registro de Población (CURP) issued by the Government of Mexico,
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
* Characters 0-3 must be alphabetic (A-Z)
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

## NoFoedselsnummer

The `NoFoedselsnummer` type represents a Norwegian national identity number. Like a number of other countries, Norway has
two different identity numbers with identical format, the fødselsnummer (birth number), which is issued to citizens and
long-term residents of Norway and the D-nummer, which is issued to foreign individuals who are not eligible for a
fødselsnummer. (The term "D-nummer" originates from the Norwegian Directorate of Sailors, when the primary group of foreign
born individuals needing an identifier when paying Norwegian taxes were sailors working on Norwegian ships.) The
`NoFoedselsnummer` type includes an `IdentifierType` property which returns a `NoIdentifierType` enumeration value that
indicates the exact type of identifier represented.

Fødselsnummer and D-nummer are both 11 digit numbers formatted as DDMMYYIIICC, with the following elements:
* DDMMYY - the person's date of birth in DDMMYY format. The only difference between a fødselsnummer and a D-nummer is
 that 40 is added to the day component of the person's date of birth (i.e. 130585 becomes 530585). Day values in the range
 41-71 (inclusive) are considered D-nummers.
* III - three digit individual number. All three digits of the individual number are used to derive the century of the
 date of birth and the last digit of the individual number indicates the person's gender, with odd digits assigned to
 males and even digits assigned to females. See below for details on the derivation of the century of the date of birth.
* CC - two separate check digits calculated using a weighted modulus 11 algorithm. The first check digit is calculated
 for the first nine digits (date of birth and individual number) and the second check digit is calculated for the date of
 birth, individual number and first check digit. The use of two different check digits drops the error rate encountered
 during data entry to approximately 1 in 100,000, a figure unattainable by single-digit check algorithms available when
 the fødselsnummer was introduced.

The 11 character value is sometimes formatted for greater readability by inserting a separator character, generally a
space, between the date of birth and the individual number, i.e. DDMMYY IIICC.

A valid fødselsnummer or D-nummer must meet all of the following rules:
* The value may not be null, empty or all whitespace characters.
* The value must be either 11 characters (without separator) or 12 characters (with separator) in length.
* All characters (except the optional separator character) must be ASCII digits (0-9).
* The optional separator character, if included, may not be an ASCII digit. Any non-digit character is allowed as a separator.
* The date of birth, after deriving the century from the individual number (and if the value is a D-nummer,
 after subtracting the D-nummer offset) must be a valid date between January 1, 1854 and December 31, 2039.
* The trailing two characters must be valid weighted modulus 11 check digits.

Example values:
* 13029597140 - fødselsnummer, date of birth February 13, 1995, gender = female, check digits 40
* 130295 97140 - fødselsnummer, date of birth February 13, 1995, gender = female, check digits 40
* 60055029566 - D-nummer, date of birth May 20, 1950, gender = male, check digits 66
* 600550-29566 - D-nummer, date of birth May 20, 1950, gender = male, check digits 66

The rules for deriving the century of birth are somewhat complicated due to additional requirements being layered
upon the individual number element over time. `NoFoedselsnummer` uses rules described in this
[article](https://blog.variant.no/ssns-and-pattern-matching-in-c-9-498f96aa71d4) published on Medium.com. The rules
are:
* Rule 1 - If the individual number is >= 500 and <= 749 **AND** the two digit year is >= 54 then the century = 1800.
* Rule 2 - If the individual number is < 500 then the century = 1900.
* Rule 3 - If the individual number is >= 900 **AND** the two digit year is >= 40 then the century = 1900.
* Rule 4 - If the individual number is>= 500 **AND** the two digit year is <= 39 then the century =2000.
* Rule 5 - Otherwise invalid. Validation will report an invalid date of birth.

According to these rules, the valid range for a date of birth is January 1, 1854 to December 31 2039.

Note that since the rules have overlapping ranges of values (individual number 500 is valid for two different
rules) they must be applied in order to derive the correct century.

Norway plans changes to fødselsnummer values in 2032 due to the expected depletion of available numbers under the
current scheme.

See [Wikipedia - National identity number (Norway)](https://en.wikipedia.org/wiki/National_identity_number_%28Norway%29) for more info.

## SePersonnummer

The `SePersonnummer` type represents either of two identifiers issued by the Swedish Tax Agency that have the
same format and are used for similar purposes. The first, the Personal Identity Number (personnummer)
is issued to persons born in Sweden or who are residents of Sweden for 12 months or longer. The second,
the coordination number (samordningsnummer) is issued to persons who reside in Sweden for less than a year.
`SePersonnummer` includes an `IdentifierType` property which returns a `SeIdentifierType` enumeration value
that indicates the exact type of identifier represented.

Personnummer and samordningsnummer values are both 11 or 13 character strings. The only difference
between the two lengths are the number of digits used to represent the date of birth, either six or
eight. The format of personnummer and samordningsnummer are the same and consist of the following
fields/sections:
* The date of birth, represented by either six or eight digits (YYMMDD format or YYYYMMDD format). Originally six digits
 were used but the eight digit format was introduced in 1997.
* A separator character that separates the date of birth from the remaining four digits. The separator character is
 normally a dash ('-') but when a person turns 100 years old
 the dash is replaced by a plus sign ('+').
* A three digit birth serial number, issued serially as births are recorded for a particular date. The last digit of the
 birth serial number serves an additional purpose of indicating the person's gender, with odd digits assigned to males and
 even digits assigned to females.
* A single check digit calculated using the Luhn algorithm applied to the rightmost six digits of the date of birth and
 to the birth serial number.

The only difference between a personnummer and a samordningsnummer is that the samordningsnummer adds 60 to the day of a
person's date of birth (i.e. 950123 would become 950183).

A valid personnummer or samordningsnummer must meet all of the following rules:
* The value may not be null, empty or all whitespace characters.
* The value must be either 11 or 13 characters long.
* For 11-character strings, the first 6 characters must represent a valid date in the format YYMMDD. For 13-character
 strings, the first 8 characters must represent a valid date in the format YYYYMMDD. Note that the validation specifically
 does **NOT** check for future dates, only that the date exists.
* The date of birth must be followed by a valid separator character. The separator must be either a dash (-) or a plus
 sign (+).
* The separator must be followed by a three digit birth serial number.
* The birth serial number must be followed by a  valid checksum calculated using the Luhn algorithm based on the six digit
 date of birth and the three-digit birth serial number. (The leading two digits of an eight digit date of birth are
 ignored.)

Note that the encoded date of birth may not be the person's actual date of birth. It is possible to run out of birth
serial numbers for a particular day and in this case a day close to the actual date of birth is substituted in its stead.

When determining if a date of birth is valid, values with six digit dates of birth use the separator character to derive
the full four digit year. Year values between 00 and 49 are assumed to be 2000 to 2049 and year values between 50 and 99
are assumed to be 1950 to 1999. If the separator character indicates that the person is at least 100 years of age, then
100 is subtracted from the year, resulting in 00 to 40 meaning 1900 to 1949 and 50 to 99 meaning 1850 to 1899.

The valid range for a date of birth is January 1, 1800 to December 31, 2099. However, when using a six digit date of
birth, the above rule limits the valid range to January 1, 1850 to December 31 2049.

For samordningsnummer values, the value returned by the `DateOfBirth` property is an actual date calculated by
subtracting 60 from the encoded date of birth.

Internally, `SePersonnummer` only stores the 12 digits of the date of birth (in YYYYMMDD format), the birth serial number
and the check digit. The `Value` property will only return those 12 digits. `SePersonnummer`
also exposes `ToShortFormatValue` and `ToLongFormatValue` methods that will return 11 and 13 character strings including a
separator character. The `ToShortFormatValue` and `ToLongFormatValue` methods allow an optional parameter of type `System.TimeProvider`
that can be used to ensure that the separator character used is either a '-' or a '+', depending on the age of the
person according to the time provider. If the time provider parameter is null, then the default separator of dash ('-') is used.
The `ToString` method will return the same result as `ToLongFormatValue` with a null time provider parameter.

When comparing two `SePersonnummer` objects for equality, the internal 12 digit representation is used. This means that
two `SePersonnummer` objects representing the same person will be considered equal, even if one was created using a six
digit date of birth and the other an eight digit date of birth.

Example Values:
* 890201-3286 - Personnummer, date of birth 890201, less than 100 years old, gender = female, check digit = 6.
* 19890201-3286 - Personnummer, date of birth 19890201, less than 100 years old, gender = female, check digit = 6.
(Note that the check digit is the same as for the six digit date of birth version because the two leading characters of
 an eight digit date of birth are ignored when calculating the check digit.)
* 811228+9874 - Personnummer, date of birth 811228, greater than 100 years old, gender = male, check digit = 4.
* 890261-3283 - Samordningsnummer, date of birth 890261 (actual date of birth = 890201), less than 100 years old, gender =
 female, check digit = 3.
* 19890261-3283 - Samordningsnummer, date of birth 19890261 (actual date of birth = 19890201), less than 100 years old, gender =
 female, check digit = 3.
* 811288+9871 - Samordningsnummer, date of birth 811288 (actual date of birth = 811228), greater than 100 years old, gender =
 male, check digit = 1.

See [Wikipedia - Personal identity number (Sweden)](https://en.wikipedia.org/wiki/Personal_identity_number_%28Sweden%29) for more info.

## UsIndividualTaxpayerIdentificationNumber

The `UsIndividualTaxpayerIdentificationNumber` type represents an Individual Taxpayer Identification Number (ITIN)
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
* The fourth and fifth digits must be in the range 50-65, 70-88, 90-92 or 94-99

Note that meeting the above rules is not a guarantee that the value is considered a valid ITIN issued
by the IRS. Determining actual validity of an ITIN requires use of the IRS's Taxpayer Identification
Number (TIN) Matching Program or other IRS services.

See [Wikipedia - Individual Taxpayer Identification Number](https://en.wikipedia.org/wiki/Individual_Taxpayer_Identification_Number) for more info.

## UsNationalProviderIdentifier

The `UsNationalProviderIdentifier` type represents a National Provider Identifier (NPI) issued to health care
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

The `UsSocialSecurityNumber` type represents a Social Security Number (SSN) issued by the US Social Security
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

# Language Notes

We apologize in advance for providing primarily English language documentation
for software with international scope. Unfortunately, the principal developer is
a product of the US educational system, with foreign language skills that fall
somewhere between limited and nonexistent. This has meant that we have had to
rely on mostly English language sources (generally Wikipedia and various AI
tools) for specifications and that the thorough documentation that we wish to
provide is likewise English only. (We have chosen not to use AI translation tools
for documentation because of our inability to review the output for accuracy.)

If you wish to help with language specific documentation for any of the business
objects in KfAccountNumbers, we would be very grateful for your assistance. You
can use the "Contact owners" link on https://www.nuget.org/packages/KfAccountNumbers
to send an email. Or you can log an issue or contribute directly to the Github
repository: https://github.com/KnowledgeForwardSolutions/KfAccountNumbers

# Release History

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
