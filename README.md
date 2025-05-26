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

#### Options

Both `Parse` and `TryParse` functions accept optional `Options` parameter. This is a class that allow you to modify the parsing process.
At the moment only one option called `MaximumRepetitions` is supported. This option controls the maximum allowed consecutive repetitions of
the same numeral. The default maximum is 3 (e.g. III or VIII), but during history (and in clock industry) four repetitions were used (e.g. IIII or VIIII).

#### Vinculum

_Vinculum_ (multiplication by 1000) is not currently supported (and will probably never be, because of the non-standard characters it uses).