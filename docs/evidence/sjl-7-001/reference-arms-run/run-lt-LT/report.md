# Bundle sjl-7-001's reference-arm comparison, classified

183238 differences: 15318 (a'), 167920 (a).
0 of them unnamed by the correspondence.

| Verdict | Row | Arm | Count |
|---|---|---|---|
| (a') | C2 | J:JSString.ToLowerCase and ToLocaleLowerCase(locale) | 1467 |
| (a') | C3 | J:JSString.ToLowerCase and ToLocaleLowerCase(locale) | 4656 |
| (a') | C3 | J:JSString.ToUpperCase and ToLocaleUpperCase(locale) | 5074 |
| (a') | N2 | J:JSIntlCollator.Compare's NFD | 864 |
| (a') | N2 | J:JSString.Normalize | 3257 |
| (a) | C1 | J:JSString.ToLowerCase and ToLocaleLowerCase(locale) | 5675 |
| (a) | C4 | J:JSString.ToLowerCase and ToLocaleLowerCase(locale) | 3606 |
| (a) | C5 | J:JSString.ToUpperCase and ToLocaleUpperCase(locale) | 22 |
| (a) | C6 | J:JSString.ToLowerCase and ToLocaleLowerCase(locale) | 6083 |
| (a) | C6 | J:JSString.ToUpperCase and ToLocaleUpperCase(locale) | 244 |
| (a) | C7 | J:JSString.ToLowerCase and ToLocaleLowerCase(locale) | 2 |
| (a) | C7 | J:JSString.ToUpperCase and ToLocaleUpperCase(locale) | 2 |
| (a) | C8 | J:JSString.ToLocaleLowerCase(undefined) | 24354 |
| (a) | C8 | J:JSString.ToLocaleUpperCase(undefined) | 3197 |
| (a) | C8 | J:JSString.ToLowerCase and ToLocaleLowerCase(locale) | 6158 |
| (a) | C8 | J:JSString.ToUpperCase and ToLocaleUpperCase(locale) | 2537 |
| (a) | N1 | J:JSIntlCollator.Compare's NFD | 23208 |
| (a) | N1 | J:JSString.Normalize | 92832 |

## lower, arm J:JSString.ToLocaleLowerCase(undefined): (a), row C8, 24354

correction of Broiler.JS: casing no longer depends on the host culture.

- `U3` input `00CC` `default@lt-LT`: S `00EC`, arm `0069 0307 0300`, arbiter `00EC`
- `U3` input `00CD` `default@lt-LT`: S `00ED`, arm `0069 0307 0301`, arbiter `00ED`
- `U3` input `0128` `default@lt-LT`: S `0129`, arm `0069 0307 0303`, arbiter `0129`
- `U3` input `D81B DEA0` `default@lt-LT`: S `D81B DEBB`, arm `D81B DEA0`, arbiter `D81B DEBB`
- `U3` input `D81B DEA1` `default@lt-LT`: S `D81B DEBC`, arm `D81B DEA1`, arbiter `D81B DEBC`
- `U3` input `D81B DEA2` `default@lt-LT`: S `D81B DEBD`, arm `D81B DEA2`, arbiter `D81B DEBD`
- `U3` input `D81B DEA3` `default@lt-LT`: S `D81B DEBE`, arm `D81B DEA3`, arbiter `D81B DEBE`
- `U3` input `D81B DEA4` `default@lt-LT`: S `D81B DEBF`, arm `D81B DEA4`, arbiter `D81B DEBF`
- `U3` input `D81B DEA5` `default@lt-LT`: S `D81B DEC0`, arm `D81B DEA5`, arbiter `D81B DEC0`
- `U3` input `D81B DEA6` `default@lt-LT`: S `D81B DEC1`, arm `D81B DEA6`, arbiter `D81B DEC1`
- `U3` input `D81B DEA7` `default@lt-LT`: S `D81B DEC2`, arm `D81B DEA7`, arbiter `D81B DEC2`
- `U3` input `D81B DEA8` `default@lt-LT`: S `D81B DEC3`, arm `D81B DEA8`, arbiter `D81B DEC3`
- `U3` input `D81B DEA9` `default@lt-LT`: S `D81B DEC4`, arm `D81B DEA9`, arbiter `D81B DEC4`
- `U3` input `D81B DEAA` `default@lt-LT`: S `D81B DEC5`, arm `D81B DEAA`, arbiter `D81B DEC5`
- `U3` input `D81B DEAB` `default@lt-LT`: S `D81B DEC6`, arm `D81B DEAB`, arbiter `D81B DEC6`
- `U3` input `D81B DEAC` `default@lt-LT`: S `D81B DEC7`, arm `D81B DEAC`, arbiter `D81B DEC7`
- `U3` input `D81B DEAD` `default@lt-LT`: S `D81B DEC8`, arm `D81B DEAD`, arbiter `D81B DEC8`
- `U3` input `D81B DEAE` `default@lt-LT`: S `D81B DEC9`, arm `D81B DEAE`, arbiter `D81B DEC9`
- `U3` input `D81B DEAF` `default@lt-LT`: S `D81B DECA`, arm `D81B DEAF`, arbiter `D81B DECA`
- `U3` input `D81B DEB0` `default@lt-LT`: S `D81B DECB`, arm `D81B DEB0`, arbiter `D81B DECB`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a'), row C2, 1467

correction of Broiler.JS: Final_Sigma reads Cased and Case_Ignorable from the runtime's data, which does not know code points Unicode 16 and 17 add (SJL-D-6 (c)).

- `U3` input `0041 1ACF 03A3` `turkic@lt-LT`: S `0061 1ACF 03C2`, arm `0061 1ACF 03C3`, arbiter `0061 1ACF 03C2`
- `U3` input `0041 1ACF 03A3` `lithuanian@lt-LT`: S `0061 1ACF 03C2`, arm `0061 1ACF 03C3`, arbiter `0061 1ACF 03C2`
- `U3` input `0041 1AD0 03A3` `turkic@lt-LT`: S `0061 1AD0 03C2`, arm `0061 1AD0 03C3`, arbiter `0061 1AD0 03C2`
- `U3` input `0041 1AD0 03A3` `lithuanian@lt-LT`: S `0061 1AD0 03C2`, arm `0061 1AD0 03C3`, arbiter `0061 1AD0 03C2`
- `U3` input `0041 1AD1 03A3` `turkic@lt-LT`: S `0061 1AD1 03C2`, arm `0061 1AD1 03C3`, arbiter `0061 1AD1 03C2`
- `U3` input `0041 1AD1 03A3` `lithuanian@lt-LT`: S `0061 1AD1 03C2`, arm `0061 1AD1 03C3`, arbiter `0061 1AD1 03C2`
- `U3` input `0041 1AD2 03A3` `turkic@lt-LT`: S `0061 1AD2 03C2`, arm `0061 1AD2 03C3`, arbiter `0061 1AD2 03C2`
- `U3` input `0041 1AD2 03A3` `lithuanian@lt-LT`: S `0061 1AD2 03C2`, arm `0061 1AD2 03C3`, arbiter `0061 1AD2 03C2`
- `U3` input `0041 1AD3 03A3` `turkic@lt-LT`: S `0061 1AD3 03C2`, arm `0061 1AD3 03C3`, arbiter `0061 1AD3 03C2`
- `U3` input `0041 1AD3 03A3` `lithuanian@lt-LT`: S `0061 1AD3 03C2`, arm `0061 1AD3 03C3`, arbiter `0061 1AD3 03C2`
- `U3` input `0041 1AD4 03A3` `turkic@lt-LT`: S `0061 1AD4 03C2`, arm `0061 1AD4 03C3`, arbiter `0061 1AD4 03C2`
- `U3` input `0041 1AD4 03A3` `lithuanian@lt-LT`: S `0061 1AD4 03C2`, arm `0061 1AD4 03C3`, arbiter `0061 1AD4 03C2`
- `U3` input `0041 1AD5 03A3` `turkic@lt-LT`: S `0061 1AD5 03C2`, arm `0061 1AD5 03C3`, arbiter `0061 1AD5 03C2`
- `U3` input `0041 1AD5 03A3` `lithuanian@lt-LT`: S `0061 1AD5 03C2`, arm `0061 1AD5 03C3`, arbiter `0061 1AD5 03C2`
- `U3` input `0041 1AD6 03A3` `turkic@lt-LT`: S `0061 1AD6 03C2`, arm `0061 1AD6 03C3`, arbiter `0061 1AD6 03C2`
- `U3` input `0041 1AD6 03A3` `lithuanian@lt-LT`: S `0061 1AD6 03C2`, arm `0061 1AD6 03C3`, arbiter `0061 1AD6 03C2`
- `U3` input `0041 1AD7 03A3` `turkic@lt-LT`: S `0061 1AD7 03C2`, arm `0061 1AD7 03C3`, arbiter `0061 1AD7 03C2`
- `U3` input `0041 1AD7 03A3` `lithuanian@lt-LT`: S `0061 1AD7 03C2`, arm `0061 1AD7 03C3`, arbiter `0061 1AD7 03C2`
- `U3` input `0041 1AD8 03A3` `turkic@lt-LT`: S `0061 1AD8 03C2`, arm `0061 1AD8 03C3`, arbiter `0061 1AD8 03C2`
- `U3` input `0041 1AD8 03A3` `lithuanian@lt-LT`: S `0061 1AD8 03C2`, arm `0061 1AD8 03C3`, arbiter `0061 1AD8 03C2`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a'), row C3, 4224

correction of Broiler.JS: Broiler.JS's default mappings come from the runtime's data and hand patches (SJL-D-6 (c)).

- `U3` input `D81B DEA0` `lithuanian@lt-LT`: S `D81B DEBB`, arm `D81B DEA0`, arbiter `D81B DEBB`
- `U3` input `D81B DEA1` `lithuanian@lt-LT`: S `D81B DEBC`, arm `D81B DEA1`, arbiter `D81B DEBC`
- `U3` input `D81B DEA2` `lithuanian@lt-LT`: S `D81B DEBD`, arm `D81B DEA2`, arbiter `D81B DEBD`
- `U3` input `D81B DEA3` `lithuanian@lt-LT`: S `D81B DEBE`, arm `D81B DEA3`, arbiter `D81B DEBE`
- `U3` input `D81B DEA4` `lithuanian@lt-LT`: S `D81B DEBF`, arm `D81B DEA4`, arbiter `D81B DEBF`
- `U3` input `D81B DEA5` `lithuanian@lt-LT`: S `D81B DEC0`, arm `D81B DEA5`, arbiter `D81B DEC0`
- `U3` input `D81B DEA6` `lithuanian@lt-LT`: S `D81B DEC1`, arm `D81B DEA6`, arbiter `D81B DEC1`
- `U3` input `D81B DEA7` `lithuanian@lt-LT`: S `D81B DEC2`, arm `D81B DEA7`, arbiter `D81B DEC2`
- `U3` input `D81B DEA8` `lithuanian@lt-LT`: S `D81B DEC3`, arm `D81B DEA8`, arbiter `D81B DEC3`
- `U3` input `D81B DEA9` `lithuanian@lt-LT`: S `D81B DEC4`, arm `D81B DEA9`, arbiter `D81B DEC4`
- `U3` input `D81B DEAA` `lithuanian@lt-LT`: S `D81B DEC5`, arm `D81B DEAA`, arbiter `D81B DEC5`
- `U3` input `D81B DEAB` `lithuanian@lt-LT`: S `D81B DEC6`, arm `D81B DEAB`, arbiter `D81B DEC6`
- `U3` input `D81B DEAC` `lithuanian@lt-LT`: S `D81B DEC7`, arm `D81B DEAC`, arbiter `D81B DEC7`
- `U3` input `D81B DEAD` `lithuanian@lt-LT`: S `D81B DEC8`, arm `D81B DEAD`, arbiter `D81B DEC8`
- `U3` input `D81B DEAE` `lithuanian@lt-LT`: S `D81B DEC9`, arm `D81B DEAE`, arbiter `D81B DEC9`
- `U3` input `D81B DEAF` `lithuanian@lt-LT`: S `D81B DECA`, arm `D81B DEAF`, arbiter `D81B DECA`
- `U3` input `D81B DEB0` `lithuanian@lt-LT`: S `D81B DECB`, arm `D81B DEB0`, arbiter `D81B DECB`
- `U3` input `D81B DEB1` `lithuanian@lt-LT`: S `D81B DECC`, arm `D81B DEB1`, arbiter `D81B DECC`
- `U3` input `D81B DEB2` `lithuanian@lt-LT`: S `D81B DECD`, arm `D81B DEB2`, arbiter `D81B DECD`
- `U3` input `D81B DEB3` `lithuanian@lt-LT`: S `D81B DECE`, arm `D81B DEB3`, arbiter `D81B DECE`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a'), row C3, 432

correction of Broiler.JS: Broiler.JS's default mappings come from the runtime's data and hand patches, and Final_Sigma reads Cased and Case_Ignorable from the same data, or Cased as row C1 reads it (SJL-D-6 (c)).

- `U3` input `0041 D81B DEA0 03A3` `lithuanian@lt-LT`: S `0061 D81B DEBB 03C2`, arm `0061 D81B DEA0 03C3`, arbiter `0061 D81B DEBB 03C2`
- `U3` input `0041 03A3 D81B DEA0` `lithuanian@lt-LT`: S `0061 03C3 D81B DEBB`, arm `0061 03C2 D81B DEA0`, arbiter `0061 03C3 D81B DEBB`
- `U3` input `D81B DEA0 03A3` `lithuanian@lt-LT`: S `D81B DEBB 03C2`, arm `D81B DEA0 03C3`, arbiter `D81B DEBB 03C2`
- `U3` input `0041 D81B DEA1 03A3` `lithuanian@lt-LT`: S `0061 D81B DEBC 03C2`, arm `0061 D81B DEA1 03C3`, arbiter `0061 D81B DEBC 03C2`
- `U3` input `0041 03A3 D81B DEA1` `lithuanian@lt-LT`: S `0061 03C3 D81B DEBC`, arm `0061 03C2 D81B DEA1`, arbiter `0061 03C3 D81B DEBC`
- `U3` input `D81B DEA1 03A3` `lithuanian@lt-LT`: S `D81B DEBC 03C2`, arm `D81B DEA1 03C3`, arbiter `D81B DEBC 03C2`
- `U3` input `0041 D81B DEA2 03A3` `lithuanian@lt-LT`: S `0061 D81B DEBD 03C2`, arm `0061 D81B DEA2 03C3`, arbiter `0061 D81B DEBD 03C2`
- `U3` input `0041 03A3 D81B DEA2` `lithuanian@lt-LT`: S `0061 03C3 D81B DEBD`, arm `0061 03C2 D81B DEA2`, arbiter `0061 03C3 D81B DEBD`
- `U3` input `D81B DEA2 03A3` `lithuanian@lt-LT`: S `D81B DEBD 03C2`, arm `D81B DEA2 03C3`, arbiter `D81B DEBD 03C2`
- `U3` input `0041 D81B DEA3 03A3` `lithuanian@lt-LT`: S `0061 D81B DEBE 03C2`, arm `0061 D81B DEA3 03C3`, arbiter `0061 D81B DEBE 03C2`
- `U3` input `0041 03A3 D81B DEA3` `lithuanian@lt-LT`: S `0061 03C3 D81B DEBE`, arm `0061 03C2 D81B DEA3`, arbiter `0061 03C3 D81B DEBE`
- `U3` input `D81B DEA3 03A3` `lithuanian@lt-LT`: S `D81B DEBE 03C2`, arm `D81B DEA3 03C3`, arbiter `D81B DEBE 03C2`
- `U3` input `0041 D81B DEA4 03A3` `lithuanian@lt-LT`: S `0061 D81B DEBF 03C2`, arm `0061 D81B DEA4 03C3`, arbiter `0061 D81B DEBF 03C2`
- `U3` input `0041 03A3 D81B DEA4` `lithuanian@lt-LT`: S `0061 03C3 D81B DEBF`, arm `0061 03C2 D81B DEA4`, arbiter `0061 03C3 D81B DEBF`
- `U3` input `D81B DEA4 03A3` `lithuanian@lt-LT`: S `D81B DEBF 03C2`, arm `D81B DEA4 03C3`, arbiter `D81B DEBF 03C2`
- `U3` input `0041 D81B DEA5 03A3` `lithuanian@lt-LT`: S `0061 D81B DEC0 03C2`, arm `0061 D81B DEA5 03C3`, arbiter `0061 D81B DEC0 03C2`
- `U3` input `0041 03A3 D81B DEA5` `lithuanian@lt-LT`: S `0061 03C3 D81B DEC0`, arm `0061 03C2 D81B DEA5`, arbiter `0061 03C3 D81B DEC0`
- `U3` input `D81B DEA5 03A3` `lithuanian@lt-LT`: S `D81B DEC0 03C2`, arm `D81B DEA5 03C3`, arbiter `D81B DEC0 03C2`
- `U3` input `0041 D81B DEA6 03A3` `lithuanian@lt-LT`: S `0061 D81B DEC1 03C2`, arm `0061 D81B DEA6 03C3`, arbiter `0061 D81B DEC1 03C2`
- `U3` input `0041 03A3 D81B DEA6` `lithuanian@lt-LT`: S `0061 03C3 D81B DEC1`, arm `0061 03C2 D81B DEA6`, arbiter `0061 03C3 D81B DEC1`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a), row C1, 5675

correction of Broiler.JS: Final_Sigma reads Cased from DerivedCoreProperties.txt (Other_Lowercase, Other_Uppercase).

- `U3` input `0041 00AA 03A3` `turkic@lt-LT`: S `0061 00AA 03C2`, arm `0061 00AA 03C3`, arbiter `0061 00AA 03C2`
- `U3` input `0041 00AA 03A3` `lithuanian@lt-LT`: S `0061 00AA 03C2`, arm `0061 00AA 03C3`, arbiter `0061 00AA 03C2`
- `U3` input `0041 03A3 00AA` `turkic@lt-LT`: S `0061 03C3 00AA`, arm `0061 03C2 00AA`, arbiter `0061 03C3 00AA`
- `U3` input `0041 03A3 00AA` `lithuanian@lt-LT`: S `0061 03C3 00AA`, arm `0061 03C2 00AA`, arbiter `0061 03C3 00AA`
- `U3` input `00AA 03A3` `turkic@lt-LT`: S `00AA 03C2`, arm `00AA 03C3`, arbiter `00AA 03C2`
- `U3` input `00AA 03A3` `lithuanian@lt-LT`: S `00AA 03C2`, arm `00AA 03C3`, arbiter `00AA 03C2`
- `U3` input `0041 00BA 03A3` `turkic@lt-LT`: S `0061 00BA 03C2`, arm `0061 00BA 03C3`, arbiter `0061 00BA 03C2`
- `U3` input `0041 00BA 03A3` `lithuanian@lt-LT`: S `0061 00BA 03C2`, arm `0061 00BA 03C3`, arbiter `0061 00BA 03C2`
- `U3` input `0041 03A3 00BA` `turkic@lt-LT`: S `0061 03C3 00BA`, arm `0061 03C2 00BA`, arbiter `0061 03C3 00BA`
- `U3` input `0041 03A3 00BA` `lithuanian@lt-LT`: S `0061 03C3 00BA`, arm `0061 03C2 00BA`, arbiter `0061 03C3 00BA`
- `U3` input `00BA 03A3` `turkic@lt-LT`: S `00BA 03C2`, arm `00BA 03C3`, arbiter `00BA 03C2`
- `U3` input `00BA 03A3` `lithuanian@lt-LT`: S `00BA 03C2`, arm `00BA 03C3`, arbiter `00BA 03C2`
- `U3` input `0041 2160 03A3` `turkic@lt-LT`: S `0061 2170 03C2`, arm `0061 2170 03C3`, arbiter `0061 2170 03C2`
- `U3` input `0041 2160 03A3` `lithuanian@lt-LT`: S `0061 2170 03C2`, arm `0061 2170 03C3`, arbiter `0061 2170 03C2`
- `U3` input `0041 03A3 2160` `turkic@lt-LT`: S `0061 03C3 2170`, arm `0061 03C2 2170`, arbiter `0061 03C3 2170`
- `U3` input `0041 03A3 2160` `lithuanian@lt-LT`: S `0061 03C3 2170`, arm `0061 03C2 2170`, arbiter `0061 03C3 2170`
- `U3` input `2160 03A3` `turkic@lt-LT`: S `2170 03C2`, arm `2170 03C3`, arbiter `2170 03C2`
- `U3` input `2160 03A3` `lithuanian@lt-LT`: S `2170 03C2`, arm `2170 03C3`, arbiter `2170 03C2`
- `U3` input `0041 2161 03A3` `turkic@lt-LT`: S `0061 2171 03C2`, arm `0061 2171 03C3`, arbiter `0061 2171 03C2`
- `U3` input `0041 2161 03A3` `lithuanian@lt-LT`: S `0061 2171 03C2`, arm `0061 2171 03C3`, arbiter `0061 2171 03C2`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a), row C4, 3121

correction of Broiler.JS: the Turkic lower-case path skips the class's table, so the runtime's older data shows.

- `U3` input `1C89` `turkic@lt-LT`: S `1C8A`, arm `1C89`, arbiter `1C8A`
- `U3` input `A7CB` `turkic@lt-LT`: S `0264`, arm `A7CB`, arbiter `0264`
- `U3` input `A7CC` `turkic@lt-LT`: S `A7CD`, arm `A7CC`, arbiter `A7CD`
- `U3` input `A7CE` `turkic@lt-LT`: S `A7CF`, arm `A7CE`, arbiter `A7CF`
- `U3` input `A7D2` `turkic@lt-LT`: S `A7D3`, arm `A7D2`, arbiter `A7D3`
- `U3` input `A7D4` `turkic@lt-LT`: S `A7D5`, arm `A7D4`, arbiter `A7D5`
- `U3` input `A7DA` `turkic@lt-LT`: S `A7DB`, arm `A7DA`, arbiter `A7DB`
- `U3` input `A7DC` `turkic@lt-LT`: S `019B`, arm `A7DC`, arbiter `019B`
- `U3` input `D803 DD50` `turkic@lt-LT`: S `D803 DD70`, arm `D803 DD50`, arbiter `D803 DD70`
- `U3` input `D803 DD51` `turkic@lt-LT`: S `D803 DD71`, arm `D803 DD51`, arbiter `D803 DD71`
- `U3` input `D803 DD52` `turkic@lt-LT`: S `D803 DD72`, arm `D803 DD52`, arbiter `D803 DD72`
- `U3` input `D803 DD53` `turkic@lt-LT`: S `D803 DD73`, arm `D803 DD53`, arbiter `D803 DD73`
- `U3` input `D803 DD54` `turkic@lt-LT`: S `D803 DD74`, arm `D803 DD54`, arbiter `D803 DD74`
- `U3` input `D803 DD55` `turkic@lt-LT`: S `D803 DD75`, arm `D803 DD55`, arbiter `D803 DD75`
- `U3` input `D803 DD56` `turkic@lt-LT`: S `D803 DD76`, arm `D803 DD56`, arbiter `D803 DD76`
- `U3` input `D803 DD57` `turkic@lt-LT`: S `D803 DD77`, arm `D803 DD57`, arbiter `D803 DD77`
- `U3` input `D803 DD58` `turkic@lt-LT`: S `D803 DD78`, arm `D803 DD58`, arbiter `D803 DD78`
- `U3` input `D803 DD59` `turkic@lt-LT`: S `D803 DD79`, arm `D803 DD59`, arbiter `D803 DD79`
- `U3` input `D803 DD5A` `turkic@lt-LT`: S `D803 DD7A`, arm `D803 DD5A`, arbiter `D803 DD7A`
- `U3` input `D803 DD5B` `turkic@lt-LT`: S `D803 DD7B`, arm `D803 DD5B`, arbiter `D803 DD7B`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a), row C4, 485

correction of Broiler.JS: the Turkic lower-case path skips the class's table, so the runtime's older data shows, in the mappings and in Final_Sigma, whose Cased is read from the same data or as row C1 reads it.

- `U3` input `0041 A7CE 03A3` `turkic@lt-LT`: S `0061 A7CF 03C2`, arm `0061 A7CE 03C3`, arbiter `0061 A7CF 03C2`
- `U3` input `0041 03A3 A7CE` `turkic@lt-LT`: S `0061 03C3 A7CF`, arm `0061 03C2 A7CE`, arbiter `0061 03C3 A7CF`
- `U3` input `A7CE 03A3` `turkic@lt-LT`: S `A7CF 03C2`, arm `A7CE 03C3`, arbiter `A7CF 03C2`
- `U3` input `0041 A7D2 03A3` `turkic@lt-LT`: S `0061 A7D3 03C2`, arm `0061 A7D2 03C3`, arbiter `0061 A7D3 03C2`
- `U3` input `0041 03A3 A7D2` `turkic@lt-LT`: S `0061 03C3 A7D3`, arm `0061 03C2 A7D2`, arbiter `0061 03C3 A7D3`
- `U3` input `A7D2 03A3` `turkic@lt-LT`: S `A7D3 03C2`, arm `A7D2 03C3`, arbiter `A7D3 03C2`
- `U3` input `0041 A7D4 03A3` `turkic@lt-LT`: S `0061 A7D5 03C2`, arm `0061 A7D4 03C3`, arbiter `0061 A7D5 03C2`
- `U3` input `0041 03A3 A7D4` `turkic@lt-LT`: S `0061 03C3 A7D5`, arm `0061 03C2 A7D4`, arbiter `0061 03C3 A7D5`
- `U3` input `A7D4 03A3` `turkic@lt-LT`: S `A7D5 03C2`, arm `A7D4 03C3`, arbiter `A7D5 03C2`
- `U3` input `0041 D81B DEA0 03A3` `turkic@lt-LT`: S `0061 D81B DEBB 03C2`, arm `0061 D81B DEA0 03C3`, arbiter `0061 D81B DEBB 03C2`
- `U3` input `0041 03A3 D81B DEA0` `turkic@lt-LT`: S `0061 03C3 D81B DEBB`, arm `0061 03C2 D81B DEA0`, arbiter `0061 03C3 D81B DEBB`
- `U3` input `D81B DEA0 03A3` `turkic@lt-LT`: S `D81B DEBB 03C2`, arm `D81B DEA0 03C3`, arbiter `D81B DEBB 03C2`
- `U3` input `0041 D81B DEA1 03A3` `turkic@lt-LT`: S `0061 D81B DEBC 03C2`, arm `0061 D81B DEA1 03C3`, arbiter `0061 D81B DEBC 03C2`
- `U3` input `0041 03A3 D81B DEA1` `turkic@lt-LT`: S `0061 03C3 D81B DEBC`, arm `0061 03C2 D81B DEA1`, arbiter `0061 03C3 D81B DEBC`
- `U3` input `D81B DEA1 03A3` `turkic@lt-LT`: S `D81B DEBC 03C2`, arm `D81B DEA1 03C3`, arbiter `D81B DEBC 03C2`
- `U3` input `0041 D81B DEA2 03A3` `turkic@lt-LT`: S `0061 D81B DEBD 03C2`, arm `0061 D81B DEA2 03C3`, arbiter `0061 D81B DEBD 03C2`
- `U3` input `0041 03A3 D81B DEA2` `turkic@lt-LT`: S `0061 03C3 D81B DEBD`, arm `0061 03C2 D81B DEA2`, arbiter `0061 03C3 D81B DEBD`
- `U3` input `D81B DEA2 03A3` `turkic@lt-LT`: S `D81B DEBD 03C2`, arm `D81B DEA2 03C3`, arbiter `D81B DEBD 03C2`
- `U3` input `0041 D81B DEA3 03A3` `turkic@lt-LT`: S `0061 D81B DEBE 03C2`, arm `0061 D81B DEA3 03C3`, arbiter `0061 D81B DEBE 03C2`
- `U3` input `0041 03A3 D81B DEA3` `turkic@lt-LT`: S `0061 03C3 D81B DEBE`, arm `0061 03C2 D81B DEA3`, arbiter `0061 03C3 D81B DEBE`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a), row C6, 15

correction of Broiler.JS: After_I, Before_Dot, After_Soft_Dotted and More_Above admit no intervening character of combining class 0 or 230; IsContextTransparent reads a mark not of class 230.

- `U3` input `0049 0903 0307` `turkic@lt-LT`: S `0131 0903 0307`, arm `0069 0903`, arbiter `0131 0903 0307`
- `U3` input `D800 0049 0903 0307` `turkic@lt-LT`: S `D800 0131 0903 0307`, arm `D800 0069 0903`, arbiter `D800 0131 0903 0307`
- `U3` input `DC00 0049 0903 0307` `turkic@lt-LT`: S `DC00 0131 0903 0307`, arm `DC00 0069 0903`, arbiter `DC00 0131 0903 0307`
- `U3` input `0049 0903 0307 D800` `turkic@lt-LT`: S `0131 0903 0307 D800`, arm `0069 0903 D800`, arbiter `0131 0903 0307 D800`
- `U3` input `0049 0903 0307 DC00` `turkic@lt-LT`: S `0131 0903 0307 DC00`, arm `0069 0903 DC00`, arbiter `0131 0903 0307 DC00`
- `U3` input `000F 1ADB 0049 D804 DEE1 0307 0ABC 0130 D806 DCA7 DB40 DDB7 0052` `turkic@lt-LT`: S `000F 1ADB 0131 D804 DEE1 0307 0ABC 0069 D806 DCC7 DB40 DDB7 0072`, arm `000F 1ADB 0069 D804 DEE1 0ABC 0069 D806 DCC7 DB40 DDB7 0072`, arbiter `000F 1ADB 0131 D804 DEE1 0307 0ABC 0069 D806 DCC7 DB40 DDB7 0072`
- `U3` input `001B 0041 DB40 DC76 0049 17BD 0307` `turkic@lt-LT`: S `001B 0061 DB40 DC76 0131 17BD 0307`, arm `001B 0061 DB40 DC76 0069 17BD`, arbiter `001B 0061 DB40 DC76 0131 17BD 0307`
- `U3` input `000E 1AD2 0041 0131 0049 D836 DE9C 0307 0037 030D 1DDE 04C7` `turkic@lt-LT`: S `000E 1AD2 0061 0131 0131 D836 DE9C 0307 0037 030D 1DDE 04C8`, arm `000E 1AD2 0061 0131 0069 D836 DE9C 0037 030D 1DDE 04C8`, arbiter `000E 1AD2 0061 0131 0131 D836 DE9C 0307 0037 030D 1DDE 04C8`
- `U3` input `001D 0031 A8F1 0049 D806 DE91 0307 0F83 003B D838 DD36 D836 DE9F 000A` `turkic@lt-LT`: S `001D 0031 A8F1 0131 D806 DE91 0307 0F83 003B D838 DD36 D836 DE9F 000A`, arm `001D 0031 A8F1 0069 D806 DE91 0F83 003B D838 DD36 D836 DE9F 000A`, arbiter `001D 0031 A8F1 0131 D806 DE91 0307 0F83 003B D838 DD36 D836 DE9F 000A`
- `U3` input `05C1 0049 0C56 DB40 DD4B 0307 D804 DCB9 1D84 0354 1F6A` `turkic@lt-LT`: S `05C1 0131 0C56 DB40 DD4B 0307 D804 DCB9 1D84 0354 1F62`, arm `05C1 0069 0C56 DB40 DD4B D804 DCB9 1D84 0354 1F62`, arbiter `05C1 0131 0C56 DB40 DD4B 0307 D804 DCB9 1D84 0354 1F62`
- `U3` input `0300 D801 DF9A D805 DDBA D818 DD21 0035 08CA 003D 0049 0CF3 0307 0F37 0DD3` `turkic@lt-LT`: S `0300 D801 DF9A D805 DDBA D818 DD21 0035 08CA 003D 0131 0CF3 0307 0F37 0DD3`, arm `0300 D801 DF9A D805 DDBA D818 DD21 0035 08CA 003D 0069 0CF3 0F37 0DD3`, arbiter `0300 D801 DF9A D805 DDBA D818 DD21 0035 08CA 003D 0131 0CF3 0307 0F37 0DD3`
- `U3` input `1ED7 0069 0011 004A 0059 0059 031E 0131 0049 D836 DE00 0307` `turkic@lt-LT`: S `1ED7 0069 0011 006A 0079 0079 031E 0131 0131 D836 DE00 0307`, arm `1ED7 0069 0011 006A 0079 0079 031E 0131 0069 D836 DE00`, arbiter `1ED7 0069 0011 006A 0079 0079 031E 0131 0131 D836 DE00 0307`
- `U3` input `13A4 0049 0FB4 1ACA D802 DE39 D806 DE07 0307` `turkic@lt-LT`: S `AB74 0131 0FB4 1ACA D802 DE39 D806 DE07 0307`, arm `AB74 0069 0FB4 1ACA D802 DE39 D806 DE07`, arbiter `AB74 0131 0FB4 1ACA D802 DE39 D806 DE07 0307`
- `U3` input `1DE8 A698 0049 D805 DCBF 0307 D804 DF70 0046 D802 DEE6 1EA2 20D1 059C D838 DC21` `turkic@lt-LT`: S `1DE8 A699 0131 D805 DCBF 0307 D804 DF70 0066 D802 DEE6 1EA3 20D1 059C D838 DC21`, arm `1DE8 A699 0069 D805 DCBF D804 DF70 0066 D802 DEE6 1EA3 20D1 059C D838 DC21`, arbiter `1DE8 A699 0131 D805 DCBF 0307 D804 DF70 0066 D802 DEE6 1EA3 20D1 059C D838 DC21`
- `U3` input `D80D DC34 D801 DDAB 0049 1938 20D2 0307 006B` `turkic@lt-LT`: S `D80D DC34 D801 DDAB 0131 1938 20D2 0307 006B`, arm `D80D DC34 D801 DDAB 0069 1938 20D2 006B`, arbiter `D80D DC34 D801 DDAB 0131 1938 20D2 0307 006B`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a), row C6, 6068

correction of Broiler.JS: More_Above: no intervening character of combining class 0 or 230.

- `U3` input `0049 0903 0307` `lithuanian@lt-LT`: S `0069 0903 0307`, arm `0069 0307 0903 0307`, arbiter `0069 0903 0307`
- `U3` input `004A 0903 0307` `lithuanian@lt-LT`: S `006A 0903 0307`, arm `006A 0307 0903 0307`, arbiter `006A 0903 0307`
- `U3` input `012E 0903 0307` `lithuanian@lt-LT`: S `012F 0903 0307`, arm `012F 0307 0903 0307`, arbiter `012F 0903 0307`
- `U3` input `004A 034F 0300` `lithuanian@lt-LT`: S `006A 034F 0300`, arm `006A 0307 034F 0300`, arbiter `006A 034F 0300`
- `U3` input `004A 0488 0300` `lithuanian@lt-LT`: S `006A 0488 0300`, arm `006A 0307 0488 0300`, arbiter `006A 0488 0300`
- `U3` input `004A 0489 0300` `lithuanian@lt-LT`: S `006A 0489 0300`, arm `006A 0307 0489 0300`, arbiter `006A 0489 0300`
- `U3` input `004A 07A6 0300` `lithuanian@lt-LT`: S `006A 07A6 0300`, arm `006A 0307 07A6 0300`, arbiter `006A 07A6 0300`
- `U3` input `004A 07A7 0300` `lithuanian@lt-LT`: S `006A 07A7 0300`, arm `006A 0307 07A7 0300`, arbiter `006A 07A7 0300`
- `U3` input `004A 07A8 0300` `lithuanian@lt-LT`: S `006A 07A8 0300`, arm `006A 0307 07A8 0300`, arbiter `006A 07A8 0300`
- `U3` input `004A 07A9 0300` `lithuanian@lt-LT`: S `006A 07A9 0300`, arm `006A 0307 07A9 0300`, arbiter `006A 07A9 0300`
- `U3` input `004A 07AA 0300` `lithuanian@lt-LT`: S `006A 07AA 0300`, arm `006A 0307 07AA 0300`, arbiter `006A 07AA 0300`
- `U3` input `004A 07AB 0300` `lithuanian@lt-LT`: S `006A 07AB 0300`, arm `006A 0307 07AB 0300`, arbiter `006A 07AB 0300`
- `U3` input `004A 07AC 0300` `lithuanian@lt-LT`: S `006A 07AC 0300`, arm `006A 0307 07AC 0300`, arbiter `006A 07AC 0300`
- `U3` input `004A 07AD 0300` `lithuanian@lt-LT`: S `006A 07AD 0300`, arm `006A 0307 07AD 0300`, arbiter `006A 07AD 0300`
- `U3` input `004A 07AE 0300` `lithuanian@lt-LT`: S `006A 07AE 0300`, arm `006A 0307 07AE 0300`, arbiter `006A 07AE 0300`
- `U3` input `004A 07AF 0300` `lithuanian@lt-LT`: S `006A 07AF 0300`, arm `006A 0307 07AF 0300`, arbiter `006A 07AF 0300`
- `U3` input `004A 07B0 0300` `lithuanian@lt-LT`: S `006A 07B0 0300`, arm `006A 0307 07B0 0300`, arbiter `006A 07B0 0300`
- `U3` input `004A 0900 0300` `lithuanian@lt-LT`: S `006A 0900 0300`, arm `006A 0307 0900 0300`, arbiter `006A 0900 0300`
- `U3` input `004A 0901 0300` `lithuanian@lt-LT`: S `006A 0901 0300`, arm `006A 0307 0901 0300`, arbiter `006A 0901 0300`
- `U3` input `004A 0902 0300` `lithuanian@lt-LT`: S `006A 0902 0300`, arm `006A 0307 0902 0300`, arbiter `006A 0902 0300`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a), row C7, 2

correction of Broiler.JS: ECMA-402's TransformCase canonicalizes the list and takes its first element.

- `U3` input `0069` `tr,en@lt-LT`: S `0069`, arm `exception:RangeError`, arbiter `0069`
- `U3` input `0069` `tr,en@lt-LT`: S `0069`, arm `exception:RangeError`, arbiter `0069`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a), row C8, 6158

correction of Broiler.JS: casing no longer depends on the host culture.

- `U3` input `D81B DEA0` `default@lt-LT`: S `D81B DEBB`, arm `D81B DEA0`, arbiter `D81B DEBB`
- `U3` input `D81B DEA1` `default@lt-LT`: S `D81B DEBC`, arm `D81B DEA1`, arbiter `D81B DEBC`
- `U3` input `D81B DEA2` `default@lt-LT`: S `D81B DEBD`, arm `D81B DEA2`, arbiter `D81B DEBD`
- `U3` input `D81B DEA3` `default@lt-LT`: S `D81B DEBE`, arm `D81B DEA3`, arbiter `D81B DEBE`
- `U3` input `D81B DEA4` `default@lt-LT`: S `D81B DEBF`, arm `D81B DEA4`, arbiter `D81B DEBF`
- `U3` input `D81B DEA5` `default@lt-LT`: S `D81B DEC0`, arm `D81B DEA5`, arbiter `D81B DEC0`
- `U3` input `D81B DEA6` `default@lt-LT`: S `D81B DEC1`, arm `D81B DEA6`, arbiter `D81B DEC1`
- `U3` input `D81B DEA7` `default@lt-LT`: S `D81B DEC2`, arm `D81B DEA7`, arbiter `D81B DEC2`
- `U3` input `D81B DEA8` `default@lt-LT`: S `D81B DEC3`, arm `D81B DEA8`, arbiter `D81B DEC3`
- `U3` input `D81B DEA9` `default@lt-LT`: S `D81B DEC4`, arm `D81B DEA9`, arbiter `D81B DEC4`
- `U3` input `D81B DEAA` `default@lt-LT`: S `D81B DEC5`, arm `D81B DEAA`, arbiter `D81B DEC5`
- `U3` input `D81B DEAB` `default@lt-LT`: S `D81B DEC6`, arm `D81B DEAB`, arbiter `D81B DEC6`
- `U3` input `D81B DEAC` `default@lt-LT`: S `D81B DEC7`, arm `D81B DEAC`, arbiter `D81B DEC7`
- `U3` input `D81B DEAD` `default@lt-LT`: S `D81B DEC8`, arm `D81B DEAD`, arbiter `D81B DEC8`
- `U3` input `D81B DEAE` `default@lt-LT`: S `D81B DEC9`, arm `D81B DEAE`, arbiter `D81B DEC9`
- `U3` input `D81B DEAF` `default@lt-LT`: S `D81B DECA`, arm `D81B DEAF`, arbiter `D81B DECA`
- `U3` input `D81B DEB0` `default@lt-LT`: S `D81B DECB`, arm `D81B DEB0`, arbiter `D81B DECB`
- `U3` input `D81B DEB1` `default@lt-LT`: S `D81B DECC`, arm `D81B DEB1`, arbiter `D81B DECC`
- `U3` input `D81B DEB2` `default@lt-LT`: S `D81B DECD`, arm `D81B DEB2`, arbiter `D81B DECD`
- `U3` input `D81B DEB3` `default@lt-LT`: S `D81B DECE`, arm `D81B DEB3`, arbiter `D81B DECE`

## normalize, arm J:JSIntlCollator.Compare's NFD: (a'), row N2, 864

correction of Broiler.JS: the host's ICU carries older Unicode data (SJL-D-6 (c)).

- `U4` input `D801 DDC9` `NFD@lt-LT`: S `D801 DDD2 0307`, arm `D801 DDC9`, arbiter `D801 DDD2 0307`
- `U4` input `D801 DDE4` `NFD@lt-LT`: S `D801 DDDA 0307`, arm `D801 DDE4`, arbiter `D801 DDDA 0307`
- `U4` input `D804 DF83` `NFD@lt-LT`: S `D804 DF82 D804 DFC9`, arm `D804 DF83`, arbiter `D804 DF82 D804 DFC9`
- `U4` input `D804 DF85` `NFD@lt-LT`: S `D804 DF84 D804 DFBB`, arm `D804 DF85`, arbiter `D804 DF84 D804 DFBB`
- `U4` input `D804 DF8E` `NFD@lt-LT`: S `D804 DF8B D804 DFC2`, arm `D804 DF8E`, arbiter `D804 DF8B D804 DFC2`
- `U4` input `D804 DF91` `NFD@lt-LT`: S `D804 DF90 D804 DFC9`, arm `D804 DF91`, arbiter `D804 DF90 D804 DFC9`
- `U4` input `D804 DFC5` `NFD@lt-LT`: S `D804 DFC2 D804 DFC2`, arm `D804 DFC5`, arbiter `D804 DFC2 D804 DFC2`
- `U4` input `D804 DFC7` `NFD@lt-LT`: S `D804 DFC2 D804 DFB8`, arm `D804 DFC7`, arbiter `D804 DFC2 D804 DFB8`
- `U4` input `D804 DFC8` `NFD@lt-LT`: S `D804 DFC2 D804 DFC9`, arm `D804 DFC8`, arbiter `D804 DFC2 D804 DFC9`
- `U4` input `D818 DD21` `NFD@lt-LT`: S `D818 DD1E D818 DD1E`, arm `D818 DD21`, arbiter `D818 DD1E D818 DD1E`
- `U4` input `D818 DD22` `NFD@lt-LT`: S `D818 DD1E D818 DD29`, arm `D818 DD22`, arbiter `D818 DD1E D818 DD29`
- `U4` input `D818 DD23` `NFD@lt-LT`: S `D818 DD1E D818 DD1F`, arm `D818 DD23`, arbiter `D818 DD1E D818 DD1F`
- `U4` input `D818 DD24` `NFD@lt-LT`: S `D818 DD29 D818 DD1F`, arm `D818 DD24`, arbiter `D818 DD29 D818 DD1F`
- `U4` input `D818 DD25` `NFD@lt-LT`: S `D818 DD1E D818 DD20`, arm `D818 DD25`, arbiter `D818 DD1E D818 DD20`
- `U4` input `D818 DD26` `NFD@lt-LT`: S `D818 DD1E D818 DD1E D818 DD1F`, arm `D818 DD26`, arbiter `D818 DD1E D818 DD1E D818 DD1F`
- `U4` input `D818 DD27` `NFD@lt-LT`: S `D818 DD1E D818 DD29 D818 DD1F`, arm `D818 DD27`, arbiter `D818 DD1E D818 DD29 D818 DD1F`
- `U4` input `D818 DD28` `NFD@lt-LT`: S `D818 DD1E D818 DD1E D818 DD20`, arm `D818 DD28`, arbiter `D818 DD1E D818 DD1E D818 DD20`
- `U4` input `D81B DD68` `NFD@lt-LT`: S `D81B DD67 D81B DD67`, arm `D81B DD68`, arbiter `D81B DD67 D81B DD67`
- `U4` input `D81B DD69` `NFD@lt-LT`: S `D81B DD63 D81B DD67`, arm `D81B DD69`, arbiter `D81B DD63 D81B DD67`
- `U4` input `D81B DD6A` `NFD@lt-LT`: S `D81B DD63 D81B DD67 D81B DD67`, arm `D81B DD6A`, arbiter `D81B DD63 D81B DD67 D81B DD67`

## normalize, arm J:JSIntlCollator.Compare's NFD: (a), row N1, 23207

correction of Broiler.JS: a lone surrogate is a code point of class 0 without a decomposition.

- `U4` input `D800` `NFD@lt-LT`: S `D800`, arm `exception:ArgumentException`, arbiter `D800`
- `U4` input `D801` `NFD@lt-LT`: S `D801`, arm `exception:ArgumentException`, arbiter `D801`
- `U4` input `D802` `NFD@lt-LT`: S `D802`, arm `exception:ArgumentException`, arbiter `D802`
- `U4` input `D803` `NFD@lt-LT`: S `D803`, arm `exception:ArgumentException`, arbiter `D803`
- `U4` input `D804` `NFD@lt-LT`: S `D804`, arm `exception:ArgumentException`, arbiter `D804`
- `U4` input `D805` `NFD@lt-LT`: S `D805`, arm `exception:ArgumentException`, arbiter `D805`
- `U4` input `D806` `NFD@lt-LT`: S `D806`, arm `exception:ArgumentException`, arbiter `D806`
- `U4` input `D807` `NFD@lt-LT`: S `D807`, arm `exception:ArgumentException`, arbiter `D807`
- `U4` input `D808` `NFD@lt-LT`: S `D808`, arm `exception:ArgumentException`, arbiter `D808`
- `U4` input `D809` `NFD@lt-LT`: S `D809`, arm `exception:ArgumentException`, arbiter `D809`
- `U4` input `D80A` `NFD@lt-LT`: S `D80A`, arm `exception:ArgumentException`, arbiter `D80A`
- `U4` input `D80B` `NFD@lt-LT`: S `D80B`, arm `exception:ArgumentException`, arbiter `D80B`
- `U4` input `D80C` `NFD@lt-LT`: S `D80C`, arm `exception:ArgumentException`, arbiter `D80C`
- `U4` input `D80D` `NFD@lt-LT`: S `D80D`, arm `exception:ArgumentException`, arbiter `D80D`
- `U4` input `D80E` `NFD@lt-LT`: S `D80E`, arm `exception:ArgumentException`, arbiter `D80E`
- `U4` input `D80F` `NFD@lt-LT`: S `D80F`, arm `exception:ArgumentException`, arbiter `D80F`
- `U4` input `D810` `NFD@lt-LT`: S `D810`, arm `exception:ArgumentException`, arbiter `D810`
- `U4` input `D811` `NFD@lt-LT`: S `D811`, arm `exception:ArgumentException`, arbiter `D811`
- `U4` input `D812` `NFD@lt-LT`: S `D812`, arm `exception:ArgumentException`, arbiter `D812`
- `U4` input `D813` `NFD@lt-LT`: S `D813`, arm `exception:ArgumentException`, arbiter `D813`

## normalize, arm J:JSIntlCollator.Compare's NFD: (a), row N1, 1

correction of Broiler.JS: normalize reads code points; the runtime's normalizer refuses U+FFFE as it refuses a lone surrogate.

- `U4` input `FFFE` `NFD@lt-LT`: S `FFFE`, arm `exception:ArgumentException`, arbiter `FFFE`

## normalize, arm J:JSString.Normalize: (a'), row N2, 3257

correction of Broiler.JS: the host's ICU carries older Unicode data (SJL-D-6 (c)).

- `U4` input `A7F1` `NFKC@lt-LT`: S `0053`, arm `A7F1`, arbiter `0053`
- `U4` input `A7F1` `NFKD@lt-LT`: S `0053`, arm `A7F1`, arbiter `0053`
- `U4` input `D801 DDC9` `NFD@lt-LT`: S `D801 DDD2 0307`, arm `D801 DDC9`, arbiter `D801 DDD2 0307`
- `U4` input `D801 DDC9` `NFKD@lt-LT`: S `D801 DDD2 0307`, arm `D801 DDC9`, arbiter `D801 DDD2 0307`
- `U4` input `D801 DDE4` `NFD@lt-LT`: S `D801 DDDA 0307`, arm `D801 DDE4`, arbiter `D801 DDDA 0307`
- `U4` input `D801 DDE4` `NFKD@lt-LT`: S `D801 DDDA 0307`, arm `D801 DDE4`, arbiter `D801 DDDA 0307`
- `U4` input `D804 DF83` `NFD@lt-LT`: S `D804 DF82 D804 DFC9`, arm `D804 DF83`, arbiter `D804 DF82 D804 DFC9`
- `U4` input `D804 DF83` `NFKD@lt-LT`: S `D804 DF82 D804 DFC9`, arm `D804 DF83`, arbiter `D804 DF82 D804 DFC9`
- `U4` input `D804 DF85` `NFD@lt-LT`: S `D804 DF84 D804 DFBB`, arm `D804 DF85`, arbiter `D804 DF84 D804 DFBB`
- `U4` input `D804 DF85` `NFKD@lt-LT`: S `D804 DF84 D804 DFBB`, arm `D804 DF85`, arbiter `D804 DF84 D804 DFBB`
- `U4` input `D804 DF8E` `NFD@lt-LT`: S `D804 DF8B D804 DFC2`, arm `D804 DF8E`, arbiter `D804 DF8B D804 DFC2`
- `U4` input `D804 DF8E` `NFKD@lt-LT`: S `D804 DF8B D804 DFC2`, arm `D804 DF8E`, arbiter `D804 DF8B D804 DFC2`
- `U4` input `D804 DF91` `NFD@lt-LT`: S `D804 DF90 D804 DFC9`, arm `D804 DF91`, arbiter `D804 DF90 D804 DFC9`
- `U4` input `D804 DF91` `NFKD@lt-LT`: S `D804 DF90 D804 DFC9`, arm `D804 DF91`, arbiter `D804 DF90 D804 DFC9`
- `U4` input `D804 DFC5` `NFD@lt-LT`: S `D804 DFC2 D804 DFC2`, arm `D804 DFC5`, arbiter `D804 DFC2 D804 DFC2`
- `U4` input `D804 DFC5` `NFKD@lt-LT`: S `D804 DFC2 D804 DFC2`, arm `D804 DFC5`, arbiter `D804 DFC2 D804 DFC2`
- `U4` input `D804 DFC7` `NFD@lt-LT`: S `D804 DFC2 D804 DFB8`, arm `D804 DFC7`, arbiter `D804 DFC2 D804 DFB8`
- `U4` input `D804 DFC7` `NFKD@lt-LT`: S `D804 DFC2 D804 DFB8`, arm `D804 DFC7`, arbiter `D804 DFC2 D804 DFB8`
- `U4` input `D804 DFC8` `NFD@lt-LT`: S `D804 DFC2 D804 DFC9`, arm `D804 DFC8`, arbiter `D804 DFC2 D804 DFC9`
- `U4` input `D804 DFC8` `NFKD@lt-LT`: S `D804 DFC2 D804 DFC9`, arm `D804 DFC8`, arbiter `D804 DFC2 D804 DFC9`

## normalize, arm J:JSString.Normalize: (a), row N1, 92828

correction of Broiler.JS: a lone surrogate is a code point of class 0 without a decomposition.

- `U4` input `D800` `NFC@lt-LT`: S `D800`, arm `exception:ArgumentException`, arbiter `D800`
- `U4` input `D800` `NFD@lt-LT`: S `D800`, arm `exception:ArgumentException`, arbiter `D800`
- `U4` input `D800` `NFKC@lt-LT`: S `D800`, arm `exception:ArgumentException`, arbiter `D800`
- `U4` input `D800` `NFKD@lt-LT`: S `D800`, arm `exception:ArgumentException`, arbiter `D800`
- `U4` input `D801` `NFC@lt-LT`: S `D801`, arm `exception:ArgumentException`, arbiter `D801`
- `U4` input `D801` `NFD@lt-LT`: S `D801`, arm `exception:ArgumentException`, arbiter `D801`
- `U4` input `D801` `NFKC@lt-LT`: S `D801`, arm `exception:ArgumentException`, arbiter `D801`
- `U4` input `D801` `NFKD@lt-LT`: S `D801`, arm `exception:ArgumentException`, arbiter `D801`
- `U4` input `D802` `NFC@lt-LT`: S `D802`, arm `exception:ArgumentException`, arbiter `D802`
- `U4` input `D802` `NFD@lt-LT`: S `D802`, arm `exception:ArgumentException`, arbiter `D802`
- `U4` input `D802` `NFKC@lt-LT`: S `D802`, arm `exception:ArgumentException`, arbiter `D802`
- `U4` input `D802` `NFKD@lt-LT`: S `D802`, arm `exception:ArgumentException`, arbiter `D802`
- `U4` input `D803` `NFC@lt-LT`: S `D803`, arm `exception:ArgumentException`, arbiter `D803`
- `U4` input `D803` `NFD@lt-LT`: S `D803`, arm `exception:ArgumentException`, arbiter `D803`
- `U4` input `D803` `NFKC@lt-LT`: S `D803`, arm `exception:ArgumentException`, arbiter `D803`
- `U4` input `D803` `NFKD@lt-LT`: S `D803`, arm `exception:ArgumentException`, arbiter `D803`
- `U4` input `D804` `NFC@lt-LT`: S `D804`, arm `exception:ArgumentException`, arbiter `D804`
- `U4` input `D804` `NFD@lt-LT`: S `D804`, arm `exception:ArgumentException`, arbiter `D804`
- `U4` input `D804` `NFKC@lt-LT`: S `D804`, arm `exception:ArgumentException`, arbiter `D804`
- `U4` input `D804` `NFKD@lt-LT`: S `D804`, arm `exception:ArgumentException`, arbiter `D804`

## normalize, arm J:JSString.Normalize: (a), row N1, 4

correction of Broiler.JS: normalize reads code points; the runtime's normalizer refuses U+FFFE as it refuses a lone surrogate.

- `U4` input `FFFE` `NFC@lt-LT`: S `FFFE`, arm `exception:ArgumentException`, arbiter `FFFE`
- `U4` input `FFFE` `NFD@lt-LT`: S `FFFE`, arm `exception:ArgumentException`, arbiter `FFFE`
- `U4` input `FFFE` `NFKC@lt-LT`: S `FFFE`, arm `exception:ArgumentException`, arbiter `FFFE`
- `U4` input `FFFE` `NFKD@lt-LT`: S `FFFE`, arm `exception:ArgumentException`, arbiter `FFFE`

## upper, arm J:JSString.ToLocaleUpperCase(undefined): (a), row C8, 3197

correction of Broiler.JS: casing no longer depends on the host culture.

- `U3` input `D81B DEBB` `default@lt-LT`: S `D81B DEA0`, arm `D81B DEBB`, arbiter `D81B DEA0`
- `U3` input `D81B DEBC` `default@lt-LT`: S `D81B DEA1`, arm `D81B DEBC`, arbiter `D81B DEA1`
- `U3` input `D81B DEBD` `default@lt-LT`: S `D81B DEA2`, arm `D81B DEBD`, arbiter `D81B DEA2`
- `U3` input `D81B DEBE` `default@lt-LT`: S `D81B DEA3`, arm `D81B DEBE`, arbiter `D81B DEA3`
- `U3` input `D81B DEBF` `default@lt-LT`: S `D81B DEA4`, arm `D81B DEBF`, arbiter `D81B DEA4`
- `U3` input `D81B DEC0` `default@lt-LT`: S `D81B DEA5`, arm `D81B DEC0`, arbiter `D81B DEA5`
- `U3` input `D81B DEC1` `default@lt-LT`: S `D81B DEA6`, arm `D81B DEC1`, arbiter `D81B DEA6`
- `U3` input `D81B DEC2` `default@lt-LT`: S `D81B DEA7`, arm `D81B DEC2`, arbiter `D81B DEA7`
- `U3` input `D81B DEC3` `default@lt-LT`: S `D81B DEA8`, arm `D81B DEC3`, arbiter `D81B DEA8`
- `U3` input `D81B DEC4` `default@lt-LT`: S `D81B DEA9`, arm `D81B DEC4`, arbiter `D81B DEA9`
- `U3` input `D81B DEC5` `default@lt-LT`: S `D81B DEAA`, arm `D81B DEC5`, arbiter `D81B DEAA`
- `U3` input `D81B DEC6` `default@lt-LT`: S `D81B DEAB`, arm `D81B DEC6`, arbiter `D81B DEAB`
- `U3` input `D81B DEC7` `default@lt-LT`: S `D81B DEAC`, arm `D81B DEC7`, arbiter `D81B DEAC`
- `U3` input `D81B DEC8` `default@lt-LT`: S `D81B DEAD`, arm `D81B DEC8`, arbiter `D81B DEAD`
- `U3` input `D81B DEC9` `default@lt-LT`: S `D81B DEAE`, arm `D81B DEC9`, arbiter `D81B DEAE`
- `U3` input `D81B DECA` `default@lt-LT`: S `D81B DEAF`, arm `D81B DECA`, arbiter `D81B DEAF`
- `U3` input `D81B DECB` `default@lt-LT`: S `D81B DEB0`, arm `D81B DECB`, arbiter `D81B DEB0`
- `U3` input `D81B DECC` `default@lt-LT`: S `D81B DEB1`, arm `D81B DECC`, arbiter `D81B DEB1`
- `U3` input `D81B DECD` `default@lt-LT`: S `D81B DEB2`, arm `D81B DECD`, arbiter `D81B DEB2`
- `U3` input `D81B DECE` `default@lt-LT`: S `D81B DEB3`, arm `D81B DECE`, arbiter `D81B DEB3`

## upper, arm J:JSString.ToUpperCase and ToLocaleUpperCase(locale): (a'), row C3, 5074

correction of Broiler.JS: Broiler.JS's default mappings come from the runtime's data and hand patches (SJL-D-6 (c)).

- `U3` input `D81B DEBB` `turkic@lt-LT`: S `D81B DEA0`, arm `D81B DEBB`, arbiter `D81B DEA0`
- `U3` input `D81B DEBB` `lithuanian@lt-LT`: S `D81B DEA0`, arm `D81B DEBB`, arbiter `D81B DEA0`
- `U3` input `D81B DEBC` `turkic@lt-LT`: S `D81B DEA1`, arm `D81B DEBC`, arbiter `D81B DEA1`
- `U3` input `D81B DEBC` `lithuanian@lt-LT`: S `D81B DEA1`, arm `D81B DEBC`, arbiter `D81B DEA1`
- `U3` input `D81B DEBD` `turkic@lt-LT`: S `D81B DEA2`, arm `D81B DEBD`, arbiter `D81B DEA2`
- `U3` input `D81B DEBD` `lithuanian@lt-LT`: S `D81B DEA2`, arm `D81B DEBD`, arbiter `D81B DEA2`
- `U3` input `D81B DEBE` `turkic@lt-LT`: S `D81B DEA3`, arm `D81B DEBE`, arbiter `D81B DEA3`
- `U3` input `D81B DEBE` `lithuanian@lt-LT`: S `D81B DEA3`, arm `D81B DEBE`, arbiter `D81B DEA3`
- `U3` input `D81B DEBF` `turkic@lt-LT`: S `D81B DEA4`, arm `D81B DEBF`, arbiter `D81B DEA4`
- `U3` input `D81B DEBF` `lithuanian@lt-LT`: S `D81B DEA4`, arm `D81B DEBF`, arbiter `D81B DEA4`
- `U3` input `D81B DEC0` `turkic@lt-LT`: S `D81B DEA5`, arm `D81B DEC0`, arbiter `D81B DEA5`
- `U3` input `D81B DEC0` `lithuanian@lt-LT`: S `D81B DEA5`, arm `D81B DEC0`, arbiter `D81B DEA5`
- `U3` input `D81B DEC1` `turkic@lt-LT`: S `D81B DEA6`, arm `D81B DEC1`, arbiter `D81B DEA6`
- `U3` input `D81B DEC1` `lithuanian@lt-LT`: S `D81B DEA6`, arm `D81B DEC1`, arbiter `D81B DEA6`
- `U3` input `D81B DEC2` `turkic@lt-LT`: S `D81B DEA7`, arm `D81B DEC2`, arbiter `D81B DEA7`
- `U3` input `D81B DEC2` `lithuanian@lt-LT`: S `D81B DEA7`, arm `D81B DEC2`, arbiter `D81B DEA7`
- `U3` input `D81B DEC3` `turkic@lt-LT`: S `D81B DEA8`, arm `D81B DEC3`, arbiter `D81B DEA8`
- `U3` input `D81B DEC3` `lithuanian@lt-LT`: S `D81B DEA8`, arm `D81B DEC3`, arbiter `D81B DEA8`
- `U3` input `D81B DEC4` `turkic@lt-LT`: S `D81B DEA9`, arm `D81B DEC4`, arbiter `D81B DEA9`
- `U3` input `D81B DEC4` `lithuanian@lt-LT`: S `D81B DEA9`, arm `D81B DEC4`, arbiter `D81B DEA9`

## upper, arm J:JSString.ToUpperCase and ToLocaleUpperCase(locale): (a), row C5, 22

correction of Broiler.JS: Soft_Dotted from PropList.txt: the hand set lacks U+1DF1A, U+1E04C, U+1E04D and U+1E068.

- `U3` input `D837 DF1A 0307` `lithuanian@lt-LT`: S `D837 DF1A`, arm `D837 DF1A 0307`, arbiter `D837 DF1A`
- `U3` input `D838 DC4C 0307` `lithuanian@lt-LT`: S `D838 DC4C`, arm `D838 DC4C 0307`, arbiter `D838 DC4C`
- `U3` input `D838 DC4D 0307` `lithuanian@lt-LT`: S `D838 DC4D`, arm `D838 DC4D 0307`, arbiter `D838 DC4D`
- `U3` input `D838 DC68 0307` `lithuanian@lt-LT`: S `D838 DC68`, arm `D838 DC68 0307`, arbiter `D838 DC68`
- `U3` input `D800 D837 DF1A 0307` `lithuanian@lt-LT`: S `D800 D837 DF1A`, arm `D800 D837 DF1A 0307`, arbiter `D800 D837 DF1A`
- `U3` input `DC00 D837 DF1A 0307` `lithuanian@lt-LT`: S `DC00 D837 DF1A`, arm `DC00 D837 DF1A 0307`, arbiter `DC00 D837 DF1A`
- `U3` input `D837 DF1A 0307 D800` `lithuanian@lt-LT`: S `D837 DF1A D800`, arm `D837 DF1A 0307 D800`, arbiter `D837 DF1A D800`
- `U3` input `D837 DF1A 0307 DC00` `lithuanian@lt-LT`: S `D837 DF1A DC00`, arm `D837 DF1A 0307 DC00`, arbiter `D837 DF1A DC00`
- `U3` input `D800 D838 DC4C 0307` `lithuanian@lt-LT`: S `D800 D838 DC4C`, arm `D800 D838 DC4C 0307`, arbiter `D800 D838 DC4C`
- `U3` input `DC00 D838 DC4C 0307` `lithuanian@lt-LT`: S `DC00 D838 DC4C`, arm `DC00 D838 DC4C 0307`, arbiter `DC00 D838 DC4C`
- `U3` input `D838 DC4C 0307 D800` `lithuanian@lt-LT`: S `D838 DC4C D800`, arm `D838 DC4C 0307 D800`, arbiter `D838 DC4C D800`
- `U3` input `D838 DC4C 0307 DC00` `lithuanian@lt-LT`: S `D838 DC4C DC00`, arm `D838 DC4C 0307 DC00`, arbiter `D838 DC4C DC00`
- `U3` input `D800 D838 DC4D 0307` `lithuanian@lt-LT`: S `D800 D838 DC4D`, arm `D800 D838 DC4D 0307`, arbiter `D800 D838 DC4D`
- `U3` input `DC00 D838 DC4D 0307` `lithuanian@lt-LT`: S `DC00 D838 DC4D`, arm `DC00 D838 DC4D 0307`, arbiter `DC00 D838 DC4D`
- `U3` input `D838 DC4D 0307 D800` `lithuanian@lt-LT`: S `D838 DC4D D800`, arm `D838 DC4D 0307 D800`, arbiter `D838 DC4D D800`
- `U3` input `D838 DC4D 0307 DC00` `lithuanian@lt-LT`: S `D838 DC4D DC00`, arm `D838 DC4D 0307 DC00`, arbiter `D838 DC4D DC00`
- `U3` input `D800 D838 DC68 0307` `lithuanian@lt-LT`: S `D800 D838 DC68`, arm `D800 D838 DC68 0307`, arbiter `D800 D838 DC68`
- `U3` input `DC00 D838 DC68 0307` `lithuanian@lt-LT`: S `DC00 D838 DC68`, arm `DC00 D838 DC68 0307`, arbiter `DC00 D838 DC68`
- `U3` input `D838 DC68 0307 D800` `lithuanian@lt-LT`: S `D838 DC68 D800`, arm `D838 DC68 0307 D800`, arbiter `D838 DC68 D800`
- `U3` input `D838 DC68 0307 DC00` `lithuanian@lt-LT`: S `D838 DC68 DC00`, arm `D838 DC68 0307 DC00`, arbiter `D838 DC68 DC00`

## upper, arm J:JSString.ToUpperCase and ToLocaleUpperCase(locale): (a), row C6, 244

correction of Broiler.JS: After_I, Before_Dot, After_Soft_Dotted and More_Above admit no intervening character of combining class 0 or 230; IsContextTransparent reads a mark not of class 230.

- `U3` input `0069 0903 0307` `lithuanian@lt-LT`: S `0049 0903 0307`, arm `0049 0903`, arbiter `0049 0903 0307`
- `U3` input `006A 0903 0307` `lithuanian@lt-LT`: S `004A 0903 0307`, arm `004A 0903`, arbiter `004A 0903 0307`
- `U3` input `012F 0903 0307` `lithuanian@lt-LT`: S `012E 0903 0307`, arm `012E 0903`, arbiter `012E 0903 0307`
- `U3` input `0249 0903 0307` `lithuanian@lt-LT`: S `0248 0903 0307`, arm `0248 0903`, arbiter `0248 0903 0307`
- `U3` input `0268 0903 0307` `lithuanian@lt-LT`: S `0197 0903 0307`, arm `0197 0903`, arbiter `0197 0903 0307`
- `U3` input `029D 0903 0307` `lithuanian@lt-LT`: S `A7B2 0903 0307`, arm `A7B2 0903`, arbiter `A7B2 0903 0307`
- `U3` input `02B2 0903 0307` `lithuanian@lt-LT`: S `02B2 0903 0307`, arm `02B2 0903`, arbiter `02B2 0903 0307`
- `U3` input `03F3 0903 0307` `lithuanian@lt-LT`: S `037F 0903 0307`, arm `037F 0903`, arbiter `037F 0903 0307`
- `U3` input `0456 0903 0307` `lithuanian@lt-LT`: S `0406 0903 0307`, arm `0406 0903`, arbiter `0406 0903 0307`
- `U3` input `0458 0903 0307` `lithuanian@lt-LT`: S `0408 0903 0307`, arm `0408 0903`, arbiter `0408 0903 0307`
- `U3` input `1D62 0903 0307` `lithuanian@lt-LT`: S `1D62 0903 0307`, arm `1D62 0903`, arbiter `1D62 0903 0307`
- `U3` input `1D96 0903 0307` `lithuanian@lt-LT`: S `1D96 0903 0307`, arm `1D96 0903`, arbiter `1D96 0903 0307`
- `U3` input `1DA4 0903 0307` `lithuanian@lt-LT`: S `1DA4 0903 0307`, arm `1DA4 0903`, arbiter `1DA4 0903 0307`
- `U3` input `1DA8 0903 0307` `lithuanian@lt-LT`: S `1DA8 0903 0307`, arm `1DA8 0903`, arbiter `1DA8 0903 0307`
- `U3` input `1E2D 0903 0307` `lithuanian@lt-LT`: S `1E2C 0903 0307`, arm `1E2C 0903`, arbiter `1E2C 0903 0307`
- `U3` input `1ECB 0903 0307` `lithuanian@lt-LT`: S `1ECA 0903 0307`, arm `1ECA 0903`, arbiter `1ECA 0903 0307`
- `U3` input `2071 0903 0307` `lithuanian@lt-LT`: S `2071 0903 0307`, arm `2071 0903`, arbiter `2071 0903 0307`
- `U3` input `2148 0903 0307` `lithuanian@lt-LT`: S `2148 0903 0307`, arm `2148 0903`, arbiter `2148 0903 0307`
- `U3` input `2149 0903 0307` `lithuanian@lt-LT`: S `2149 0903 0307`, arm `2149 0903`, arbiter `2149 0903 0307`
- `U3` input `2C7C 0903 0307` `lithuanian@lt-LT`: S `2C7C 0903 0307`, arm `2C7C 0903`, arbiter `2C7C 0903 0307`

## upper, arm J:JSString.ToUpperCase and ToLocaleUpperCase(locale): (a), row C7, 2

correction of Broiler.JS: ECMA-402's TransformCase canonicalizes the list and takes its first element.

- `U3` input `0069` `tr,en@lt-LT`: S `0130`, arm `exception:RangeError`, arbiter `0130`
- `U3` input `0069` `tr,en@lt-LT`: S `0130`, arm `exception:RangeError`, arbiter `0130`

## upper, arm J:JSString.ToUpperCase and ToLocaleUpperCase(locale): (a), row C8, 2537

correction of Broiler.JS: casing no longer depends on the host culture.

- `U3` input `D81B DEBB` `default@lt-LT`: S `D81B DEA0`, arm `D81B DEBB`, arbiter `D81B DEA0`
- `U3` input `D81B DEBC` `default@lt-LT`: S `D81B DEA1`, arm `D81B DEBC`, arbiter `D81B DEA1`
- `U3` input `D81B DEBD` `default@lt-LT`: S `D81B DEA2`, arm `D81B DEBD`, arbiter `D81B DEA2`
- `U3` input `D81B DEBE` `default@lt-LT`: S `D81B DEA3`, arm `D81B DEBE`, arbiter `D81B DEA3`
- `U3` input `D81B DEBF` `default@lt-LT`: S `D81B DEA4`, arm `D81B DEBF`, arbiter `D81B DEA4`
- `U3` input `D81B DEC0` `default@lt-LT`: S `D81B DEA5`, arm `D81B DEC0`, arbiter `D81B DEA5`
- `U3` input `D81B DEC1` `default@lt-LT`: S `D81B DEA6`, arm `D81B DEC1`, arbiter `D81B DEA6`
- `U3` input `D81B DEC2` `default@lt-LT`: S `D81B DEA7`, arm `D81B DEC2`, arbiter `D81B DEA7`
- `U3` input `D81B DEC3` `default@lt-LT`: S `D81B DEA8`, arm `D81B DEC3`, arbiter `D81B DEA8`
- `U3` input `D81B DEC4` `default@lt-LT`: S `D81B DEA9`, arm `D81B DEC4`, arbiter `D81B DEA9`
- `U3` input `D81B DEC5` `default@lt-LT`: S `D81B DEAA`, arm `D81B DEC5`, arbiter `D81B DEAA`
- `U3` input `D81B DEC6` `default@lt-LT`: S `D81B DEAB`, arm `D81B DEC6`, arbiter `D81B DEAB`
- `U3` input `D81B DEC7` `default@lt-LT`: S `D81B DEAC`, arm `D81B DEC7`, arbiter `D81B DEAC`
- `U3` input `D81B DEC8` `default@lt-LT`: S `D81B DEAD`, arm `D81B DEC8`, arbiter `D81B DEAD`
- `U3` input `D81B DEC9` `default@lt-LT`: S `D81B DEAE`, arm `D81B DEC9`, arbiter `D81B DEAE`
- `U3` input `D81B DECA` `default@lt-LT`: S `D81B DEAF`, arm `D81B DECA`, arbiter `D81B DEAF`
- `U3` input `D81B DECB` `default@lt-LT`: S `D81B DEB0`, arm `D81B DECB`, arbiter `D81B DEB0`
- `U3` input `D81B DECC` `default@lt-LT`: S `D81B DEB1`, arm `D81B DECC`, arbiter `D81B DEB1`
- `U3` input `D81B DECD` `default@lt-LT`: S `D81B DEB2`, arm `D81B DECD`, arbiter `D81B DEB2`
- `U3` input `D81B DECE` `default@lt-LT`: S `D81B DEB3`, arm `D81B DECE`, arbiter `D81B DEB3`
