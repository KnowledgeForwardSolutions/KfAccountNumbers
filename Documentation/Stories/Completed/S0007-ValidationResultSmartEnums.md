# S0007-ValidationResultSmartEnums: Refactor Validation Result Enums to Smart Enum Pattern

## Title
Convert ValidationResult enums to smart enum classes with shared base class for improved extensibility and maintainability

## User Story
As a **.NET developer maintaining the KfAccountNumbers library**,  
I want **validation result enums refactored to smart enum classes with a common base**,  
so that **I can add rich behavior to validation results, improve error messaging, eliminate code duplication, and enable polymorphic validation result handling across different account number types**.

## Story abandoned
Creating a base smart enum with derived versions for each business object proved impractical using Ardalis.SmartEnum. The smart enum portion of the story was abandoned and replaced by a smaller effort to use a generic KfValidationException class instead.