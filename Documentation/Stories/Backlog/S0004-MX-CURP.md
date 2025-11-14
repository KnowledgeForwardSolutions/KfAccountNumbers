# S0004-MX-CURP

As a developer, I want a business object that represents a Mexican Clave Unica de Registro de Poblacion, also known as a Unique Population Registy Code, abbreviated as CURP. 

# CURP Details

* A MX CURP is a 18 character string issued by a Mexican Federal Government's agency, the Registry Nacional de Poblacion (RENAPO). Portions of the string are generated from a person's personal information and the two trailing characters are assigned by RENAPO. The characters in a CURP number are (by position, zero-based):

* 0 - First letter of paternal last name
* 1 - First vowel of paternal last name
* 2 - First letter of maternal last name
* 3 - First letter of persons first name
* 4-9 - Six digit date of birth in YYMMDD format
* 10 - Gender: H (hombre/male), M (mujer/female) or X (non-binary)
* 11-12 - Two character code for the person's state of birth (see table below) or NE (nacido en el extranjero) for persons born abroad
* 13 - First internal consonant of paternal last name
* 14 - First internal consonant of maternal last name
* 15 - First internal consonant of person's first name
* 16 - Homoclave - character assigned by RENAPO to ensure uniqueness. Will be 0-9 for persons born before 2000 or A-Z for persons born since 2000.
* 17 - Check digit assigned by RENAPO. The algorithm for validating the check digit is not published

There are several exceptions to the above. 
* The Spanish letter N with tilde (considered a separate letter rather than a letter with an accent) is replaced with the letter X. 
* If the person has two given names and the first given name is a very common name (such as Maria or Jose) then the second given name will be used in place of the person's first name.
* If the first four characters form a work considered profane, offensive or pejorative then other characters are used. RENAPO maintains a Catalog of Inappropriate Words (Catalogo de Palabras Inconvenientes) that lists the objectional words and their replacement characters (generally by replacing one of the vowels with the letter X).

The non-digit characters are uppercase latin alphabetic characters (A-Z).

# Links

https://en.wikipedia.org/wiki/Unique_Population_Registry_Code
https://docs.oracle.com/en/cloud/saas/human-resources/24b/faimx/person-national-identifiers-for-mexico.html#Clave-Unica-de-Registro-de-Poblaci%C3%B3n-(CURP)

# Validation Rules

* Not null, empty or all whitespace characters
* Length 18
* Characters 0-3 must be alphabetic characters (A-Z)
* Characters 4-9 must be digits (0-9) and be a valid YYMMDD date
* Character 10 must be H, M or X
* Characters 11-12 must be a valid state code or NE
* Charcters 13-15 must be alphabetic characters (A-Z)
* Character 16 must be an alphanumeric character (A-z, 0-9)
* Character 17 must be a digit (0-9)

# Requirements

* Create a new business object named MxCurp
* MxCurp will exist in namespace KfAccountNumbers.Governmental.NorthAmerica
* MxCurp will have a public constructor that accepts a String. The constructor will apply the validation rules described in [Validation Rules](#validation-rules). If any of the validation rules are failed then the constructor will throw an InvalidMxCurpException. The exception will contain an enum and a message that describes the error encountered. 
* MxCurp will have a public static method named Create that accepts a String. The Create method will use the Result pattern to return either a valid MxCurp or an enum value that identifies the validation rule that failed.
* MxCurp will have a public static method named Validate that accepts a String. The Valdate method will apply all of the validation rules described in [Validation Rules](#validation-rules) and return an enum value that indicates if the String is a valid MxCurp or if not, what validation rule was failed.
* MxCurp will be read-only
* MxCurp will have an implicit conversion from String. The conversion will throw an exception if any validation rules are failed
* MxCurp will have an implicit conversion to String. The result will be 18 characters in length and uppercase
* MxCurp will have a ToString method that returns a String of length 18 and uppercase
* MxCurp will have a read-only Gender property that returns an enum (1 - Hombre/male, 2 - Mujer/female, 3 - non-binary)
* MxCurp will have a read-only DateOfBirth property that returns a Date value. The Homoclave value is used to determine the century of the person's birth (0-9 assumed to to be 1900, A-Z assumed to be 2000)

# Deliverables

* New MxCurp business object
* New MxCurpValidationResult enum with the following values:
	- ValidationPassed = 1
	-	Empty
	- InvalidLength
	- InvalidAlphabeticCharacterEncountered
	- InvalidDateOfBirth
	- InvalidGender
	- InvalidState
	- InvalidHomoclave
	- InvalidCheckDigit
* New InvalidMxCurpException class
* Unit tests that demonstrate that all of the requirements are met
* Readme updates (including benchmark results)