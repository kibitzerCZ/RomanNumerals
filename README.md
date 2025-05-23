# RomanNumerals

Simple Roman numerals parser.

### Usage

```
using kibitzerCZ.RomanNumerals;

public void Test()
{
	int decimal = RomanNumeral.Parse("MCCXXXIV");		//returns 1234
}
```

The `Parse` function performs basic format checking. When the format is not valid, a `FormatException` is thrown. Thus using a `try-catch` block is recommended.