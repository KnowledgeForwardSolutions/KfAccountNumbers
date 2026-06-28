# KfAccountNumbers

KfAccountNumbers is a collection of strongly typed business objects for a wide range of government
and commercial account numbers (ex. US Social Security Number, UK National Insurance Number, etc.).

The business objects in KfAccountNumbers all have the following capabilities:

* A constructor that accepts a string representation of the account number. The constructor will throw an exception if the string value is invalid.
* A static Validate method that accepts a string representation of the account number and that returns an enum value that indicates if the string value is valid or the validation rule for the account number that was failed.
* A static Create method that accepts a string representation of the account number and that uses the result pattern to return either an instance of the account number business object or an enum value that indicates the validation rule that was failed.
* Implicit conversion to string and explicit conversion from string.

If the business object represents an account number that has a defined format (ex. US Social Security
Number, etc.), the constructor, Create and Validate methods and explicit string to business object
operator will accept either a string that consists of only the characters in the account number or a
string that includes format characters (ex. dashes, spaces, etc.) in the appropriate places. The
business object will also implement a Format method that returns a string representation of the
account number with the appropriate format characters in the appropriate places.

If the business object represents an account number that normally has no formatting other than the
raw characters of the account number then the business object constructor, Create and Validate methods
and explicit string to business object operator will only accept strings that consist of the raw
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
		- [BeRijksregisternummer](#berijksregisternummer) 
		- [DkPersonnummer](#dkpersonnummer)
		- [EsNif](#esnif)
		- [FiHenkilotunnus](#fihenkilotunnus)
		- [FrInseeNumber](#frinseenumber)
		- [Gb National Health Service Patient Numbers (GbChiNumber, GbHcNumber, GbNhsNumber, GbPatientNumber)](#gb-national-health-service-patient-numbers-gbchinumber-gbhcnumber-gbnhsnumber-gbpatientnumber)
		- [GbNationalInsuranceNumber](#gbnationalinsurancenumber)
		- [IePpsNumber](#ieppsnumber)
		- [IsKennitala](#iskennitala)
		- [NlBurgerservicenummer](#nlburgerservicenummer)
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

## BeRijksregisternummer

The `BeRijksregisternummer` type represents two possible national identification numbers used by Belgium.
The Belgian National Register Number (Rijksregisternummer in Dutch, Numéro de registre national in French)
is a unique identifier assigned to all persons (Belgian citizens and foreign residents) who are registered
in Belgium's National Register (Rijksregister/Registre national). Persons who are not in the National
register, but who still need a Belgian national identifier (such as cross-border workers) are assigned
a BIS number (bis-nummer or numéro bis). The `BeRijksregisternummer` type includes an `IdentifierType`
property which returns a `BeIdentifierType` enumeration value that indicates the exact type of identifier
represented.

A Belgian rijksregisternummer is an eleven-digit identifier structured as YYMMDDXXXCC with the following
elements:
* YYMMDD - the person's date of birth in YYMMDD format. A BIS number is differentiated from a
  rijksregisternummer by the addition of a constant value (40 or 20, see below) to the month component
  of the date of birth.
* XXX - a three digit sequence number used to differentiate between persons born on the same date.
  The sequence number also indicates gender with odd numbers for males and even numbers for females.
* CC - two digit modulus 97 check sum calculated for the YYMMDD and XXX elements. The check sum
  is also used to indicate century of birth. If CC is equal to the normal modulus 97 check sum then
  the persons' century of birth is 1900-1999. If CC is equal to the modulus 97 check sum calculated by
  first prefixing YYMMDDXXX with the digit 2 (i.e. 2YYMMDDXXX) then the person's century of birth
  is 2000-2099.

A Belgian rijksregisternummer may be formatted as a string of 11 consecutive digits (YYMMDDXXXCC) or
as a 15 character string with characters separating the individual elements. YY.MM.DD-XXX.CC is the
typical display format.

A Belgian rijksregisternummer must meet all of the following rules:
* The value may not be null, empty or all whitespace characters.
* The value must be either 11 characters (without separators) or 15 characters (with separators) in length.
* All characters other than the optional separator characters must be ASCII digits ('0'-'9').
* The separator characters, if included, must not be ASCII digits ('0'-'9').
* The two trailing (right-most) characters must be a valid modulus 97 check sum (taking into account
  the possibility of a person born in the year 2000 or later).
* The date of birth, after deriving the century of birth from the check sum and taking into account the
  BIS number offset, must be a valid date between January 1, 1900 and December 31, 2099. **OR** the
  date of birth may use zeros to indicate that some or all of the person's date of birth is unknown
  (see below for more details).
* The sequence number may not be 000 or 999.

The date of birth can be adjusted in a variety of ways:
* If the person's date of birth is incomplete, then the two digit year is used and zeros are used for
  month and year (for example, 40.00.00-955.69).
* If there are too many people with incomplete dates of birth for a particular year than can be represented
  by a three digit sequence number (i.e. more than 499 males with incomplete dates of birth for the year 1940),
  then 01 is used for the day of birth and the sequence number rolls over to 001 (ex. 40.00.01-001.33).
  (Note that `BeRijksregisternummer` does not enforce an upper limit on the day component in cases of
  rollover, though multiple rollovers in a single year should be rare.)
* If the person's date of birth is unknown, then the constant 00.00.01 is used.
* As noted above, if the value is a BIS number then 40 is added to the month component of the date of birth.
* If the value is a BIS number **AND** the person's gender is unknown at the time the number is issued then
  **20** is added to the month component of the date of birth.

For cases of a BIS number for a person with an incomplete or unknown date of birth, `BeRijksregisternummer`
stacks the appropriate rules. For example, 87.40.00-023.47 would be the BIS number for a person with an
incomplete date of birth born in 1987.

Example values:
* 85.07.30-033.28 - rijksregisternummer, date of birth July 30, 1985, gender = male, check digit calculation 97 - (850730033 mod 97) = 97 - 69 = 28
* 17110804680 - rijksregisternummer, date of birth November 11, 2017, gender = female, check digit calculation 97 - (2171108046 mod 97) = 97 - 17 = 80
* 40 00 00 955-79 - rijksregisternummer, date of birth incomplete, year of birth = 1940, gender = male, check digit calculation 97 - (400000955 mod 97) = 97 - 18 = 79
* 00 00 01 003-64 - rijksregisternummer, date of birth unknown, gender = male, check digit calculation 97 - (000001003 mod 97) = 97 - 33 = 64
* 17.51.08-046.40 - BIS number, date of birth November 11, 1917, gender = female, check digit calculation 97 - (175108046 mod 97) = 97 - 57 = 40
* 09 20 00 002 65 - BIS number, date of birth incomplete, year of birth 2009, gender unknown, check digit calculation 97 - (2092000002 mod 97) = 97 - 32 = 65

See [Wikipedia (French) - Numéro de registre national](https://fr.wikipedia.org/wiki/Num%C3%A9ro_de_registre_national)
for more info.

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
* SSSS - a four digit sequence number used to differentiate between persons born on the same date.
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

The first digit following the date of birth is used to determine the exact century of
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

## EsNif

The `EsNif` type represents a Spanish tax identifier, officially known as the Número de Identificación Fiscal (NIF). `EsNif`
actually supports two different identifiers, the documento nacional de identidad (DNI) issued to Spanish citizens or the
número de identificación de extranjero (NIE) issued to foreigners residing in Spain. `EsNif` includes an `IdentifierType`
property which returns an `EsIdentifierType` enumeration value that indicates if the number is a DNI or NIE.

DNI and NIE are both nine-digit numbers with similar, but slightly different structures. A DNI has the
structure DDDDDDDDC while a NIE uses PDDDDDDDC, where:
* D is a digit (0-9).
* C is an alphabetic character representing the modulus 23 check digit calculated from the previous eight digits.
* P is one of the letters X, Y or Z (when calculating the check digit, X = 0, Y = 1 and Z = 2).

The only difference between a DNI and a NIE is if the leading (left-most) character is a digit or the letter X, Y or Z.
Both values may be formatted as a sequence of nine characters or may be formatted for greater readability by using
separators. For a DNI, a separator (generally a dash '-') is placed between the digits and the trailing alphabetic 
character. For a NIE, separators are placed between the leading letter and the digits, and between the digits and
the trailing alphabetic character.

A NIF must meet all of the following rules:
* The value may not be null, empty or all whitespace characters.
* The value must be 9 characters in length (without separators) or 10 characters (DNI with one separator) or
  11 characters (NIE with two separators). Additionally, if a value has length 10, it must be a DNI (starts with
  a digit) and if a value has length 11, it must be a NIE (starts with X, Y or Z).
* All characters other than the leading and trailing characters (and the optional separators) must be ASCII digits
  ('0'-'9'). The leading character must be either an ASCII digit or X, Y, or Z.
* The trailing character must be a valid modulus 23 check character. Valid characters are "TRWAGMYFPDXBNJZSQVHLCKE"
  (where T represents a remainder of 0 and E represents a remainder of 22).
* The optional separator character(s), if included, may not be an ASCII digit. Any non-digit character is allowed as a separator.
  For a DNI, the separator must be in character position 8 (zero-based). For a NIE, the separators must be in character
  positions 1 and 9 (zero-based) and both separator characters must be the same.

Note that the `EsNif` constructor and Create/Validate methods are case-sensitive and require that alphabetic characters
be upper-case.

Example values:
* 12345678Z - DNI
* 50487563-X - DNI
* X1234567L - NIE
* Y-7654321-G - NIE

See [Wikipedia - National Identity Card (Spain)](https://en.wikipedia.org/wiki/National_Identity_Card_%28Spain%29)
and [Wikipedia (Spanish) - Número de identificación fiscal](https://es.wikipedia.org/wiki/N%C3%BAmero_de_identificaci%C3%B3n_fiscal)

## FiHenkilotunnus

The `FiHenkilotunnus` type represents the national identification number used by Finland. KfAccountNumbers'
implementation of `FiHenkilotunnus` supports the [2023 separator reform](https://dvv.fi/en/reform-of-personal-identity-code)
which extended the allowable century indicator characters to prevent exhausting available henkilötunnus for particular
dates of birth.

A Finnish henkilötunnus is an 11 character value structured as DDMMYYCZZZQ with the following elements:
* DDMMYY - the person's date of birth in DDMMYY format.
* C - the century indicator, with `+` indicating 1800s, `-, U, V, W, X or Y` indicating 1900s and `A, B, C, D, E, F` indicating 2000s.
* ZZZ - a three digit individual number used to differentiate between persons born on the same date. Odd individual numbers
  indicate males and even numbers indicate females. Values from 002-899 indicate persons born in Finland or permanent residents
  and values from 900-999 indicate a temporary value (for example, a hospital patient where the official henkilötunnus is unknown).
  Individual numbers less than 002 are not valid.
* Q - a modulus 31 check digit (or check character, actually) calculated from the digits of the date of birth and the individual
  number. (The century indicator is excluded from the calculation.) The check character will be one of 31 alphanumeric
  characters, `0123456789ABCDEFHJKLMNPRSTUVWXY` (the letters `G, I, O, Q and Z` are excluded to avoid possible confusion with
  digit characters).

A henkilötunnus must meet all of the following rules:
* The value may not be null, empty or all whitespace characters.
* The value must be 11 characters in length.
* The date of birth and individual number elements (DDMMYY and ZZZ elements) must be ASCII digits ('0'-'9').
* The century indicator must be `+, -, U, V, W, X, Y, A, B, C, D, E or F`.
* The date of birth, after deriving the century of birth from the century indicator, must be a valid date between January 1, 1800 and December 31, 2099.
* The individual number must be greater than or equal to 002.
* The check character must be a valid modulus 31 check character calculated from the date of birth and the individual number.

FiHenkilotunnus is case-insensitive for validation and parsing purposes. The FiHenkilotunnus constructor, Create
method and explicit string to FiHenkilotunnus operator will normalize any lowercase letters to uppercase. Equality and
inequality comparisons between instances of FiHenkilotunnus will compare the normalized uppercase versions of the value.

Example values:
* 230526-034N - date of birth May 23, 1926, gender = female, permanent resident
* 160117A275C - date of birth January 16, 2017, gender = male, permanent resident
* 020508D929B - date of birth May 2, 2008, gender = male, temporary/test value

See [Wikipedia - National identification number - Finland](https://en.wikipedia.org/wiki/National_identification_number#Finland)
for more info. Also see [Henkilötunnus](https://kenda.fi/tools/hetu/) for tools to generate test henkilötunnus values.

## FrInseeNumber

The `FrInseeNumber` type represents the national identification number used by France, managed by the Institut national de la
statistique et des études économiques (INSEE). INSEE numbers are sometimes abbreviated as NIR (numéro d'inscription au répertoire
des personnes physiques)

A French INSEE number is a 15-digit number structured as SYYMMLLOOOKKKCC with the following elements:
* S - the person's gender, where 1 = male and 2 = female. Temporary INSEE numbers use 7 = male and 8 = female instead.
* YY - two-digit year of birth.
* MM - two-digit month of birth.
* LLOOO - five-digit INSEE COG (Code officiel géographique) identifying the person's department and commune of birth.
  The COG is the combination of department and commune of birth. There are three possible patterns for COG:
  * For persons born in metropolitan France, 2-digit department + 3-digit commune (including Corsican departments 2A and 2B).
  * For persons born in overseas departments, 3-digit department + 2-digit commune.
  * For persons born abroad, fixed 2-digit department of 99 + three-digit ISO 3166-1 country code.
* KKK - three digits used to distinguish between people born in the same year/month/department/commune.
* CC - two-digit modulus 97 check sum calculated for the preceding 13 digits. When calculating the checksum, department code "2A"
  is replaced by 19, and department code "2B" is replaced by 18.

An INSEE number may be formatted as 15 consecutive digits or as 21 characters with spaces separating the different
elements, i.e. "S YY MM LL OOO KKK CC".

A valid INSEE number must meet all of the following rules:
* The value may not be null, empty or all whitespace characters.
* The value must be either 15 characters (without separators) or 21 characters (with separators) in length.
* All characters (except the optional separator characters or Corsican department codes) must be ASCII digits ('0'-'9').
* The two trailing (right-most) characters must be a valid modulus 97 check sum.
* The separator characters (if used) may not be ASCII digits ('0'-'9'). All separator characters must be the same character.
* The leading gender indicator (S) must be 1, 2, 7 or 8.
* The month element (MM) must be a number between 01 and 12 (for known dates) or 13, 20-42, 50-99 (for persons with unknown or
  incomplete date of birth documentation).
* The COG element (LLOOO) must start with a valid department code, or 99 for persons born abroad. For departments
  with alphabetic characters (Corsica 2A, 2B), the alphabetic character may be uppercase or lowercase.

FrInseeNumber is case-insensitive for validation and parsing purposes. The FrInseeNumber constructor, Create
method and explicit string to FrInseeNumber operator will normalize any lowercase letters to uppercase.
Equality and inequality comparisons between instances of FrInseeNumber will compare the normalized
uppercase versions of the value.

Example values:
* 188121884813236 - gender = male, year of birth = 88, month of birth = 12, department = 18 (Cher)
* 255102445387701 - gender = female, year of birth = 55, month of birth = 10, department = 24 (Dordogne)
* 112072A28806058 - gender = male, year of birth = 12, month of birth = 07, department = 2A (Corse-du-Sud)
* 821099901013371 - temporary INSEE, gender = female, year of birth = 21, month of birth = 09, department = 99 (born abroad)

See [Wikipedia - INSEE code](https://en.wikipedia.org/wiki/INSEE_code) and
[Wikipedia (French) - Numéro de sécurité sociale en France](https://fr.wikipedia.org/wiki/Num%C3%A9ro_de_s%C3%A9curit%C3%A9_sociale_en_France) for more info.

## Gb National Health Service Patient Numbers (GbChiNumber, GbHcNumber, GbNhsNumber, GbPatientNumber)

The multiple public health services in the United Kingdom use patient numbers with significant similarities so the
various types are all described in this one section.

The three public health services in the United Kingdom are:
* The National Health Service (NHS) of England, Wales and the Isle of Man.
* The Scottish Community Health Index (CHI).
* Northern Ireland Health and Care (H&C).

Each of the services uses a 10-digit patient number, with the first nine digits assigned by the
service and the trailing (right-most) digit calculated using the Modulus 11 check digit algorithm.
The patient numbers used by NHS and H&C have no embedded patient information while CHI numbers
embed the patient date of birth and gender (see below).

The patient numbers can be displayed as a string of 10 digits or formatted with separator characters
for readability. NHS and H&C numbers are formatted as three groups of digits in a '3 3 4' pattern
(e.g. "123 456 7890") while CHI numbers are formatted as two groups of digits in a '6 4' pattern
(e.g. "123456 7890"). The six-digit grouping in CHI numbers corresponds to the first six digits
representing the patient's date of birth (see below). The optional separator characters can be any
character that is not an ASCII digit ('0' - '9'), but for NHS and H&C numbers both separator
characters must be the same. The typical separator character is a space (' ').

Each of the services is allocated one or more non-overlapping blocks of nine-digit numbers so it is
possible to determine what service issued the number by comparing the number to a list of valid
ranges for each service. There is also a block of numbers that is reserved for test purposes and is
not issued to the public.

The current assigned blocks of numbers are:
* 010 000 000 to 311 299 999 - Scottish CHI
* 320 000 000 to 399 999 999 - Northern Irish H&C
* 400 000 000 to 499 999 999 - NHS
* 600 000 000 to 799 999 999 - NHS
* 900 000 000 to 999 999 999 - Test

The actual patient number is a nine-digit number selected from the block of numbers allocated to the
issuing service, to which a Modulus 11 check digit is appended, resulting in a 10-digit final number.
Note that the Modulus 11 algorithm can generate a final checksum equal to 10, which can not be
represented with a single decimal digit. In this case, the number is not issued and another number
is selected. In practice this means that approximately 9.09% of all possible numbers are never
issued.

A valid patient number for any of the UK public health services must meet all of the following rules:
* The value may not be null, empty or all whitespace characters.
* The value must be either 10 characters long (without separators) or 11 characters (CHI number with separator) or 12 characters long (NHS and H&C numbers with separators).
* All characters (except the optional separator characters) must be ASCII digits ('0' - '9').
* The trailing (right-most) digit must be a valid Modulus 11 check digit.
* If the value is 11 characters long, character position 6 (zero-based) must not be an ASCII digit ('0' - '9').
* If the value is 12 characters long, character positions 3 and 7 (zero-based) must not be ASCII digits ('0' - '9'). The same character must be used in each separator position.
* The first nine digits must be in a range appropriate to the issuing service (or the test range in the case of NHS or H&C).

KfAccountNumbers includes types for the patient numbers for each of the UK public health services and an additional type
that can represent a patient number from any of the health services.

### GbNhsNumber

`GbNhsNumber` represents a patient number issued by the NHS. The actual number may be from one of the two blocks of numbers
allocated to the NHS or a number from the test block. A NHS patient number does not encode any additional information such
as date of birth or gender in the number.

### GbHcNumber

`GbHcNumber` represents a patient number issued by H&C. The actual number may be from the block of numbers allocated to
H&C or a number from the test block. A H&C patient number does not encode any additional information such
as date of birth or gender in the number.

### GbChiNumber

`GbChiNumber` represents a patient number issued by CHI. A CHI patient number encodes both date of birth and gender in the
patient number and is structured as DDMMYYNNGC where
* DDMMYY is the patient date of birth encoded in DDMMYY format.
* NNG are three digits used to differentiate between two persons born on the same day. The third digit (G) also indicates
 the person's gender, where odd numbers = male and even numbers = female.
* C is the Modulus 11 check digit.

`GbChiNumber` has an additional validation rule where the initial six digits of the number must be a valid date. `GbChiNumber`
does not allow numbers from the test block because they would fail date of birth validation.

`GbChiNumber` has an additional property for retrieving the patient's gender and a method for retrieving the patient's date
of birth.

### GbPatientNumber

`GbPatientNumber` represents a patient number issued by any of the UK public health services. `GbPatientNumber` uses all
of the validation rules described above, including ensuring that the length of formatted values is appropriate for the
specific number block (length 11 for values in the CHI block and 12 for values in other blocks) and ensuring that date
of birth is valid if the number is in the CHI block.

`GbPatientNumber` has an `IdentifierType` property that allows the user to determine the service that issued the number.
`GbPatientNumber` allows implicit conversion from `GbNhsNumber`, `GbHcNumber` and `GbChiNumber` and implements 
`ToGbNhsNumber`, `ToGbHcNumber` and `ToGbChiNumber` methods that support converting `GbPatientNumber` to a more specific
type using the option pattern. For example, if `GbPatientNumber` contains a CHI number and you want to get the patient's
gender, you would use the `ToGbChiNumber` method to convert to an instance of `GbChiNumber` from which you can use the
Gender property.

Example values:
* 4000000004 - unformatted NHS number
* 400 000 0004 - formatted NHS number
* 3200000007 - unformatted H&C number
* 320 000 0007 - formatted H&C number
* 3112999991 - unformatted CHI number, date of birth December 31, 1999, gender = male
* 311299 9991 - formatted CHI number, date of birth December 31, 1999, gender = male
* 9000000009 - unformatted test number
* 900 000 0009 - formatted test number

See [Wikipedia - NHS Number](https://en.wikipedia.org/wiki/NHS_number),
[NHS Data Model and Dictionary](https://www.datadictionary.nhs.uk/attributes/nhs_number.html) and
[Allocated Ranges for NHS Numbers](https://webarchive.nationalarchives.gov.uk/ukgwa/20231221081503/https://digital.nhs.uk/about-nhs-digital/contact-us/freedom-of-information/freedom-of-information-disclosure-log/december-2022/nic-690159-k8h4z)

## GbNationalInsuranceNumber

The `GbNationalInsuranceNumber` type represents a National Insurance Number of the United Kingdom, Guernsey, the Isle of Man,
and Jersey. While not defined as such, it effectively serves as national identifier in the UK.

A National Insurance Number consists of nine characters structured as PPDDDDDDS, where:
* PP is a two-letter prefix. See below for valid prefix characters.
* DDDDDD is a six-digit sequentially assigned number.
* S is a single suffix letter, either A, B, C, or D. The suffix can be omitted if it is unknown as the suffix does not contribute
  to the uniqueness of the value.

A National Insurance Number is typically displayed as a single string of nine characters but can be formatted for readability
as groups of two characters with a separator character, typically a space (i.e. PP DD DD DD S). `GbNationalInsuranceNumber`
is case-sensitive and requires the prefix and suffix characters to be uppercase letters.

A valid National Insurance Number must meet all of the following rules:
* The value may not be null, empty or all whitespace characters.
* The value must be one of the following lengths:
  * 8 characters (unformatted, without suffix character)
  * 9 characters (unformatted, with suffix character)
  * 11 characters (formatted, without suffix character)
  * 13 characters (formatted, with suffix character)
* The leading (left-most) two characters may not be BG, GB, NK, KN, TN, NT, or ZZ.
* Character position 0 (zero-based) must be an uppercase letter, A-C, E, G, H, J-P, R-T, W-Z. The letters D, F, I, Q, U and V are not allowed.
* Character position 1 (zero-based) must be an uppercase letter, A-C, E, G, H, J-N, P, R-T, W-Z. The letters D, F, I, O, Q, U and V are not allowed. (Note O is the only additional excluded character.)
* Character positions 2-7 (zero-based) must be ASCII digits ('0'-'9').
* Character position 8 (zero-based), if present, must be an uppercase letter, A-D.
* Separator characters, if present, may not be ASCII digits ('0'-'9') or uppercase or lowercase letters (A-Z, a-z).
* The same character must be used in every separator position.

Note that National Insurance Numbers do not include a check digit.

Also note that since suffix characters do not contribute to the uniqueness of National Insurance numbers, then it is
technically accurate to say that two values that differ only by one having a suffix character and the other not should
be considered equal. However, if `GbNationalInsuranceNumber` were to override the normal record equality to support this
case there would be other implications, such as hashing or equality where two values have suffix character but only differ
by suffix character. In the end, `GbNationalInsuranceNumber` uses normal record equality and two values that differ only
by the presence or absence of a suffix character will still not be considered equal. But `GbNationalInsuranceNumber` does
attempt to support this case by including an `EqualsNonSuffix` method that performs an equality check only on the first
eight characters (two prefix characters and six digits) of both values.

Example values:
* AB123456C - unformatted, with suffix character
* GG000123 - unformatted, without suffix character
* AB 12 34 56 C - formatted, with suffix character
* GG 00 01 23 - formatted, without suffix character

See [Wikipedia - National Insurance number](https://en.wikipedia.org/wiki/National_Insurance_number) for more info.

## IePpsNumber

The `IePpsNumber` type represents an Irish Personal Public Service number, the national identification number used by Ireland.

An Irish PPS Number consists of seven digits followed by an alphabetic check character and sometimes one additional
letter. The optional second letter was made permanent in 2013 to allow for expansion of the number of PPS numbers issued.
A PPS Number is structured as DDDDDDDC or DDDDDDDCE where
* D is a digit (0-9).
* C is an alphabetic character representing the weighted modulus 23 check character calculated from the previous seven digits and the
  second letter, if present.
* E is an optional letter in the range of A-I or W, made permanent in 2013 to expand the available number space for PPS number issuance.

An Irish PPS Number is typically displayed as a single string of eight or nine characters, without any separator characters.

A valid PPS Number must meet all of the following rules:
* The value may not be null, empty or all whitespace characters.
* The value must be either 8 or 9 characters in length.
* The characters in positions 0-6 (zero-based) must be ASCII digits ('0'-'9').
* The character in position 7 (zero-based) must be a valid weighted modulus 23 check character in the range of A-W.
* The character in position 8 (zero-based), if present, must be a letter in the range of A-I or W.

`IePpsNumber` is case-insensitive and will accept both upper-case and lower-case letters in the two trailing positions
however the value will be normalized to upper-case internally.

Example values:
* 1234567T - Check character = T
* 1234567FA - Check character = F, second letter = A
* 7654321PA - Check character = P, second letter = A  
* 9876543QA - Check character = Q, second letter = A

See [Wikipedia - Personal Public Service Number](https://en.wikipedia.org/wiki/Personal_Public_Service_Number) for more info.

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
* RR - two random digits used to differentiate between persons born on the same date.
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
method and explicit string to MxCurp operator will normalize any lowercase letters to uppercase.
Equality and inequality comparisons between instances of MxCurp will compare the normalized
uppercase versions of the value.

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

## NlBurgerservicenummer

The `NlBurgerservicenummer` type represents a Dutch Burgerservicenummer (BSN), issued to all residents.

A burgerservicenummer consists of nine digits, without embedded personal information or attributes other
than a trailing check digit calculated using a variation of the modulus 11 algorithm. The number is
typically displayed as nine consecutive digits (NNNNNNNNN) or formatted with separators (NNNN-NN-NNN).

A valid burgerservicenummer must meet all of the following rules:
* The value may not be null, empty or all whitespace characters.
* The value must be either nine characters (without separators) or eleven characters (with separators) in length.
* All characters (except the optional separator characters) must be ASCII digits (0-9).
* The optional separator character, if included, may not be an ASCII digit. Any non-digit character is allowed as a separator.
* If separator characters are present, they must be located in character positions 4 and 7 (zero-based) and the same
  non-digit character must be used in both positions.
* The trailing (right-most) character must be a valid check digit according to the variant modulus 11 algorithm.

Example values:
* 123456782
* 1234-56-782

The variant modulus 11 algorithm used for burgerservicenummer assigns a weight of -1 to the check digit
instead of the weight of 1 that is normally used for modulus 11 check digits.

See [Wikipedia (Dutch) - Burgerservicenummer](https://nl.wikipedia.org/wiki/Burgerservicenummer) for more info.
Also see [National ID Number Tool](https://nationalidnumber.com/burgerservicenummer-id) for tools to generate test BSN values.

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
