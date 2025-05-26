# RomanNumerals

Simple Roman numerals parser for .NET (8.0).
Implemented using a binary tree.

### Usage

```
using kibitzerCZ.RomanNumerals;

public void Test()
{
  int decimal = RomanNumeral.Parse("MCCXXXIV");		//returns 1234
}
```

The `Parse` function performs basic format checking. When the format is not valid, a `FormatException` is thrown. Thus using a `try-catch` block is recommended.
Or you can use `TryParse` which will return `false` instead of throwing an exception in case the format is not valid.