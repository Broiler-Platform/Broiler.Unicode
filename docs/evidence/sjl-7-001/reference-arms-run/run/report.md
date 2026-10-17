# Bundle sjl-7-001's reference-arm comparison, classified

16022956 differences: 27380 (a'), 15985359 (a), 10217 (b).
0 of them unnamed by the correspondence.

| Verdict | Row | Arm | Count |
|---|---|---|---|
| (a') | C2 | J:JSString.ToLocaleLowerCase(undefined) | 760 |
| (a') | C2 | J:JSString.ToLowerCase and ToLocaleLowerCase(locale) | 2227 |
| (a') | C3 | J:JSString.ToLocaleLowerCase(undefined) | 2560 |
| (a') | C3 | J:JSString.ToLocaleUpperCase(undefined) | 2537 |
| (a') | C3 | J:JSString.ToLowerCase and ToLocaleLowerCase(locale) | 7216 |
| (a') | C3 | J:JSString.ToUpperCase and ToLocaleUpperCase(locale) | 7611 |
| (a') | G3 | J:JSIntlSegments.Compute | 348 |
| (a') | N2 | J:JSIntlCollator.Compare's NFD | 864 |
| (a') | N2 | J:JSString.Normalize | 3257 |
| (a) | C1 | J:JSString.ToLocaleLowerCase(undefined) | 2838 |
| (a) | C1 | J:JSString.ToLowerCase and ToLocaleLowerCase(locale) | 8513 |
| (a) | C12 | V:JsRealm.StringChangeCase and TransformCase | 10372611 |
| (a) | C4 | J:JSString.ToLowerCase and ToLocaleLowerCase(locale) | 3606 |
| (a) | C5 | J:JSString.ToUpperCase and ToLocaleUpperCase(locale) | 22 |
| (a) | C6 | J:JSString.ToLowerCase and ToLocaleLowerCase(locale) | 6083 |
| (a) | C6 | J:JSString.ToUpperCase and ToLocaleUpperCase(locale) | 244 |
| (a) | C7 | J:JSString.ToLowerCase and ToLocaleLowerCase(locale) | 2 |
| (a) | C7 | J:JSString.ToUpperCase and ToLocaleUpperCase(locale) | 2 |
| (a) | G1 | J:JSIntlSegments.Compute | 42646 |
| (a) | G2 | J:JSIntlSegments.Compute | 33576 |
| (a) | I1 | J:CharExtensions.IsIdentifierPart | 1 |
| (a) | I1 | J:CharExtensions.IsIdentifierStart | 1 |
| (a) | I3 | J:JSRegExp.IsRegExpIdentifierPart | 4700 |
| (a) | I3 | J:JSRegExp.IsRegExpIdentifierStart | 4648 |
| (a) | N1 | J:JSIntlCollator.Compare's NFD | 23208 |
| (a) | N1 | J:JSString.Normalize | 92832 |
| (a) | N6 | V:JsRealm.NormalizeText | 5295672 |
| (a) | P1 | J:JSRegExp.ResolvePropertyOperand | 27967 |
| (a) | P1 | J:JSRegExp.TranslateUnicodeProperty | 65879 |
| (a) | P2 | J:JSRegExp.ResolvePropertyOperand | 8 |
| (a) | P2 | J:JSRegExp.TranslateUnicodeProperty | 16 |
| (a) | P4 | J:JSRegExp.ResolvePropertyOperand | 283 |
| (a) | W1 | J:FastScanner.IsWhiteSpace | 1 |
| (b) | G4 | J:JSIntlSegments.IsWordLike | 10217 |

## charge, arm V:JsRealm.NormalizeText: (a), row N6, 377592

correction of Broiler.VM: the maximum result length is checked on the result, not the decomposition; S charges row N7's units for the decomposition it composes, which arm V refused after its first charge.

- `U4` input `00A8` `normalize NFKC;max=1`: S `8`, arm `1`, arbiter `-`
- `U4` input `00AF` `normalize NFKC;max=1`: S `8`, arm `1`, arbiter `-`
- `U4` input `00B4` `normalize NFKC;max=1`: S `8`, arm `1`, arbiter `-`
- `U4` input `00B8` `normalize NFKC;max=1`: S `8`, arm `1`, arbiter `-`
- `U4` input `00BC` `normalize NFKC;max=2`: S `11`, arm `1`, arbiter `-`
- `U4` input `00BD` `normalize NFKC;max=2`: S `11`, arm `1`, arbiter `-`
- `U4` input `00BE` `normalize NFKC;max=2`: S `11`, arm `1`, arbiter `-`
- `U4` input `0132` `normalize NFKC;max=1`: S `8`, arm `1`, arbiter `-`
- `U4` input `0133` `normalize NFKC;max=1`: S `8`, arm `1`, arbiter `-`
- `U4` input `013F` `normalize NFKC;max=1`: S `8`, arm `1`, arbiter `-`
- `U4` input `0140` `normalize NFKC;max=1`: S `8`, arm `1`, arbiter `-`
- `U4` input `0149` `normalize NFKC;max=1`: S `8`, arm `1`, arbiter `-`
- `U4` input `01C4` `normalize NFKC;max=1`: S `11`, arm `1`, arbiter `-`
- `U4` input `01C4` `normalize NFKC;max=2`: S `11`, arm `1`, arbiter `-`
- `U4` input `01C5` `normalize NFKC;max=1`: S `11`, arm `1`, arbiter `-`
- `U4` input `01C5` `normalize NFKC;max=2`: S `11`, arm `1`, arbiter `-`
- `U4` input `01C6` `normalize NFKC;max=1`: S `11`, arm `1`, arbiter `-`
- `U4` input `01C6` `normalize NFKC;max=2`: S `11`, arm `1`, arbiter `-`
- `U4` input `01C7` `normalize NFKC;max=1`: S `8`, arm `1`, arbiter `-`
- `U4` input `01C8` `normalize NFKC;max=1`: S `8`, arm `1`, arbiter `-`

## grapheme, arm J:JSIntlSegments.Compute: (a'), row G3, 348

correction of Broiler.JS: Broiler.JS's graphemes follow the runtime's data (SJL-D-6 (c)).

- `U6` input `2701 200D 2701` `-`: S `0 2 3`, arm `0 3`, arbiter `0 2 3`
- `U6` input `0915 094D 0924` `-`: S `0 3`, arm `0 2 3`, arbiter `0 3`
- `U6` input `0915 094D 094D 0924` `-`: S `0 4`, arm `0 3 4`, arbiter `0 4`
- `U6` input `0915 094D 200D 0924` `-`: S `0 4`, arm `0 3 4`, arbiter `0 4`
- `U6` input `0915 093C 200D 094D 0924` `-`: S `0 5`, arm `0 4 5`, arbiter `0 5`
- `U6` input `0915 093C 094D 200D 0924` `-`: S `0 5`, arm `0 4 5`, arbiter `0 5`
- `U6` input `0915 094D 0924 094D 092F` `-`: S `0 5`, arm `0 2 4 5`, arbiter `0 5`
- `U6` input `0915 094D 094D 0924` `-`: S `0 4`, arm `0 3 4`, arbiter `0 4`
- `U6` input `0AB8 0AFB 0ACD 0AB8 0AFB` `-`: S `0 5`, arm `0 3 5`, arbiter `0 5`
- `U6` input `1019 1039 1018 102C 1037` `-`: S `0 3 5`, arm `0 2 3 5`, arbiter `0 3 5`
- `U6` input `1004 103A 1039 1011 1039 1011` `-`: S `0 6`, arm `0 3 5 6`, arbiter `0 6`
- `U6` input `1B12 1B01 1B32 1B44 1B2F 1B32 1B44 1B22 1B44 1B2C 1B32 1B44 1B22 1B38` `-`: S `0 2 5 10 14`, arm `0 2 4 5 7 9 10 12 14`, arbiter `0 2 5 10 14`
- `U6` input `179F 17D2 178F 17D2 179A 17B8` `-`: S `0 6`, arm `0 2 4 6`, arbiter `0 6`
- `U6` input `1B26 1B17 1B44 1B13` `-`: S `0 1 4`, arm `0 1 3 4`, arbiter `0 1 4`
- `U6` input `1B27 1B13 1B44 1B0B 1B0B 1B04` `-`: S `0 1 4 6`, arm `0 1 3 4 6`, arbiter `0 1 4 6`
- `U6` input `1795 17D2 17AF 1798` `-`: S `0 3 4`, arm `0 2 3 4`, arbiter `0 3 4`
- `U6` input `17A0 17D2 17AB 1791 17D0 1799` `-`: S `0 3 5 6`, arm `0 2 3 5 6`, arbiter `0 3 5 6`
- `U6` input `2701 200D 2701` `-`: S `0 2 3`, arm `0 3`, arbiter `0 2 3`
- `U6` input `1929 1944 2006 1128 5319 FE68 202F D806 DF66 D835 DC8F 30EE` `-`: S `0 1 2 3 4 5 6 9 11 12`, arm `0 1 2 3 4 5 6 7 9 11 12`, arbiter `0 1 2 3 4 5 6 9 11 12`
- `U6` input `D802 DF3E D83C DDE6 1AE8` `-`: S `0 2 5`, arm `0 2 4 5`, arbiter `0 2 5`

## lexical, arm J:CharExtensions.IsIdentifierPart: (a), row I1, 1

correction of Broiler.JS: IdentifierStartChar is ID_Start, which subtracts Pattern_Syntax (UAX #31).

- `U1` input `2E2F` `IdentifierPartChar`: S `false`, arm `true`, arbiter `false`

## lexical, arm J:CharExtensions.IsIdentifierStart: (a), row I1, 1

correction of Broiler.JS: IdentifierStartChar is ID_Start, which subtracts Pattern_Syntax (UAX #31).

- `U1` input `2E2F` `IdentifierStartChar`: S `false`, arm `true`, arbiter `false`

## lexical, arm J:FastScanner.IsWhiteSpace: (a), row W1, 1

correction of Broiler.JS: U+0085 is Cc and in neither WhiteSpace nor LineTerminator.

- `U1` input `0085` `WhiteSpace`: S `false`, arm `true`, arbiter `false`

## lexical, arm J:JSRegExp.IsRegExpIdentifierPart: (a), row I3, 4700

correction of Broiler.JS: group names copy the identifier tests without Pattern_Syntax and without the Unicode 17 tables.

- `U1` input `088F` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `0C5C` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `0CDC` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1ACF` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1AD0` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1AD1` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1AD2` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1AD3` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1AD4` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1AD5` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1AD6` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1AD7` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1AD8` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1AD9` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1ADA` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1ADB` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1ADC` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1ADD` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1AE0` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1AE1` `RegExpIdentifierPart`: S `true`, arm `false`, arbiter `true`

## lexical, arm J:JSRegExp.IsRegExpIdentifierStart: (a), row I3, 4648

correction of Broiler.JS: group names copy the identifier tests without Pattern_Syntax and without the Unicode 17 tables.

- `U1` input `088F` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `0C5C` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `0CDC` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `2E2F` `RegExpIdentifierStart`: S `false`, arm `true`, arbiter `false`
- `U1` input `A7CE` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `A7CF` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `A7D2` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `A7D4` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `A7F1` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `10940` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `10941` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `10942` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `10943` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `10944` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `10945` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `10946` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `10947` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `10948` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `10949` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`
- `U1` input `1094A` `RegExpIdentifierStart`: S `true`, arm `false`, arbiter `true`

## lookup, arm J:JSRegExp.ResolvePropertyOperand: (a), row P1, 27967

correction of Broiler.JS: UnicodeMatchProperty and UnicodeMatchPropertyValue admit the tables' names and aliases exactly.

- `U2` input `lone/E_Base/` `v`: S `refused`, arm `Emoji_Modifier_Base`, arbiter `refused`
- `U2` input `lone/Space/` `v`: S `refused`, arm `White_Space`, arbiter `refused`
- `U2` input `lone/Di/` `v`: S `refused`, arm `Default_Ignorable_Code_Point`, arbiter `refused`
- `U2` input `lone/ascii/` `v`: S `refused`, arm `ASCII`, arbiter `refused`
- `U2` input `lone/aSCII/` `v`: S `refused`, arm `ASCII`, arbiter `refused`
- `U2` input `lone/ascii_hex_digit/` `v`: S `refused`, arm `ASCII_Hex_Digit`, arbiter `refused`
- `U2` input `lone/ASCII_HEX_DIGIT/` `v`: S `refused`, arm `ASCII_Hex_Digit`, arbiter `refused`
- `U2` input `lone/ascii_hEX_dIGIT/` `v`: S `refused`, arm `ASCII_Hex_Digit`, arbiter `refused`
- `U2` input `lone/aSCII_Hex_Digit/` `v`: S `refused`, arm `ASCII_Hex_Digit`, arbiter `refused`
- `U2` input `lone/ahex/` `v`: S `refused`, arm `ASCII_Hex_Digit`, arbiter `refused`
- `U2` input `lone/AHEX/` `v`: S `refused`, arm `ASCII_Hex_Digit`, arbiter `refused`
- `U2` input `lone/ahEX/` `v`: S `refused`, arm `ASCII_Hex_Digit`, arbiter `refused`
- `U2` input `lone/aHex/` `v`: S `refused`, arm `ASCII_Hex_Digit`, arbiter `refused`
- `U2` input `lone/alphabetic/` `v`: S `refused`, arm `Alphabetic`, arbiter `refused`
- `U2` input `lone/ALPHABETIC/` `v`: S `refused`, arm `Alphabetic`, arbiter `refused`
- `U2` input `lone/aLPHABETIC/` `v`: S `refused`, arm `Alphabetic`, arbiter `refused`
- `U2` input `lone/alpha/` `v`: S `refused`, arm `Alphabetic`, arbiter `refused`
- `U2` input `lone/ALPHA/` `v`: S `refused`, arm `Alphabetic`, arbiter `refused`
- `U2` input `lone/aLPHA/` `v`: S `refused`, arm `Alphabetic`, arbiter `refused`
- `U2` input `lone/any/` `v`: S `refused`, arm `Any`, arbiter `refused`

## lookup, arm J:JSRegExp.ResolvePropertyOperand: (a), row P2, 8

correction of Broiler.JS: Hrkt is a value of Script in PropertyValueAliases.txt and in the specification's table.

- `U2` input `pair/Script/Hrkt` `v`: S `Script=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script=Katakana_Or_Hiragana`
- `U2` input `pair/Script/Katakana_Or_Hiragana` `v`: S `Script=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script=Katakana_Or_Hiragana`
- `U2` input `pair/sc/Hrkt` `v`: S `Script=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script=Katakana_Or_Hiragana`
- `U2` input `pair/sc/Katakana_Or_Hiragana` `v`: S `Script=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script=Katakana_Or_Hiragana`
- `U2` input `pair/Script_Extensions/Hrkt` `v`: S `Script_Extensions=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script_Extensions=Katakana_Or_Hiragana`
- `U2` input `pair/Script_Extensions/Katakana_Or_Hiragana` `v`: S `Script_Extensions=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script_Extensions=Katakana_Or_Hiragana`
- `U2` input `pair/scx/Hrkt` `v`: S `Script_Extensions=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script_Extensions=Katakana_Or_Hiragana`
- `U2` input `pair/scx/Katakana_Or_Hiragana` `v`: S `Script_Extensions=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script_Extensions=Katakana_Or_Hiragana`

## lookup, arm J:JSRegExp.ResolvePropertyOperand: (a), row P4, 283

correction of Broiler.JS: General_Category resolves inside a v-mode set operation.

- `U2` input `lone/N/` `v`: S `General_Category=Number`, arm `refused`, arbiter `General_Category=Number`
- `U2` input `lone/No/` `v`: S `General_Category=Other_Number`, arm `refused`, arbiter `General_Category=Other_Number`
- `U2` input `lone/Unassigned/` `v`: S `General_Category=Unassigned`, arm `refused`, arbiter `General_Category=Unassigned`
- `U2` input `lone/Paragraph_Separator/` `v`: S `General_Category=Paragraph_Separator`, arm `refused`, arbiter `General_Category=Paragraph_Separator`
- `U2` input `lone/L/` `v`: S `General_Category=Letter`, arm `refused`, arbiter `General_Category=Letter`
- `U2` input `lone/Nonspacing_Mark/` `v`: S `General_Category=Nonspacing_Mark`, arm `refused`, arbiter `General_Category=Nonspacing_Mark`
- `U2` input `lone/S/` `v`: S `General_Category=Symbol`, arm `refused`, arbiter `General_Category=Symbol`
- `U2` input `lone/Private_Use/` `v`: S `General_Category=Private_Use`, arm `refused`, arbiter `General_Category=Private_Use`
- `U2` input `lone/Punctuation/` `v`: S `General_Category=Punctuation`, arm `refused`, arbiter `General_Category=Punctuation`
- `U2` input `lone/C/` `v`: S `General_Category=Other`, arm `refused`, arbiter `General_Category=Other`
- `U2` input `lone/Other/` `v`: S `General_Category=Other`, arm `refused`, arbiter `General_Category=Other`
- `U2` input `lone/Cc/` `v`: S `General_Category=Control`, arm `refused`, arbiter `General_Category=Control`
- `U2` input `lone/Control/` `v`: S `General_Category=Control`, arm `refused`, arbiter `General_Category=Control`
- `U2` input `lone/cntrl/` `v`: S `General_Category=Control`, arm `refused`, arbiter `General_Category=Control`
- `U2` input `lone/Cf/` `v`: S `General_Category=Format`, arm `refused`, arbiter `General_Category=Format`
- `U2` input `lone/Format/` `v`: S `General_Category=Format`, arm `refused`, arbiter `General_Category=Format`
- `U2` input `lone/Cn/` `v`: S `General_Category=Unassigned`, arm `refused`, arbiter `General_Category=Unassigned`
- `U2` input `lone/Co/` `v`: S `General_Category=Private_Use`, arm `refused`, arbiter `General_Category=Private_Use`
- `U2` input `lone/Cs/` `v`: S `General_Category=Surrogate`, arm `refused`, arbiter `General_Category=Surrogate`
- `U2` input `lone/Surrogate/` `v`: S `General_Category=Surrogate`, arm `refused`, arbiter `General_Category=Surrogate`

## lookup, arm J:JSRegExp.TranslateUnicodeProperty: (a), row P1, 65879

correction of Broiler.JS: UnicodeMatchProperty and UnicodeMatchPropertyValue admit the tables' names and aliases exactly.

- `U2` input `lone/sc/` `u`: S `refused`, arm `General_Category=Currency_Symbol`, arbiter `refused`
- `U2` input `lone/sc/` `v`: S `refused`, arm `General_Category=Currency_Symbol`, arbiter `refused`
- `U2` input `lone/cf/` `u`: S `refused`, arm `General_Category=Format`, arbiter `refused`
- `U2` input `lone/cf/` `v`: S `refused`, arm `General_Category=Format`, arbiter `refused`
- `U2` input `lone/lc/` `u`: S `refused`, arm `General_Category=Cased_Letter`, arbiter `refused`
- `U2` input `lone/lc/` `v`: S `refused`, arm `General_Category=Cased_Letter`, arbiter `refused`
- `U2` input `lone/CS/` `u`: S `refused`, arm `General_Category=Surrogate`, arbiter `refused`
- `U2` input `lone/CS/` `v`: S `refused`, arm `General_Category=Surrogate`, arbiter `refused`
- `U2` input `lone/c/` `u`: S `refused`, arm `General_Category=Other`, arbiter `refused`
- `U2` input `lone/c/` `v`: S `refused`, arm `General_Category=Other`, arbiter `refused`
- `U2` input `lone/n/` `u`: S `refused`, arm `General_Category=Number`, arbiter `refused`
- `U2` input `lone/n/` `v`: S `refused`, arm `General_Category=Number`, arbiter `refused`
- `U2` input `lone/CN/` `u`: S `refused`, arm `General_Category=Unassigned`, arbiter `refused`
- `U2` input `lone/CN/` `v`: S `refused`, arm `General_Category=Unassigned`, arbiter `refused`
- `U2` input `lone/E_Base/` `u`: S `refused`, arm `Emoji_Modifier_Base`, arbiter `refused`
- `U2` input `lone/E_Base/` `v`: S `refused`, arm `Emoji_Modifier_Base`, arbiter `refused`
- `U2` input `lone/SM/` `u`: S `refused`, arm `General_Category=Math_Symbol`, arbiter `refused`
- `U2` input `lone/SM/` `v`: S `refused`, arm `General_Category=Math_Symbol`, arbiter `refused`
- `U2` input `lone/SpacingMark/` `u`: S `refused`, arm `General_Category=Spacing_Mark`, arbiter `refused`
- `U2` input `lone/SpacingMark/` `v`: S `refused`, arm `General_Category=Spacing_Mark`, arbiter `refused`

## lookup, arm J:JSRegExp.TranslateUnicodeProperty: (a), row P2, 16

correction of Broiler.JS: Hrkt is a value of Script in PropertyValueAliases.txt and in the specification's table.

- `U2` input `pair/Script/Hrkt` `u`: S `Script=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script=Katakana_Or_Hiragana`
- `U2` input `pair/Script/Hrkt` `v`: S `Script=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script=Katakana_Or_Hiragana`
- `U2` input `pair/Script/Katakana_Or_Hiragana` `u`: S `Script=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script=Katakana_Or_Hiragana`
- `U2` input `pair/Script/Katakana_Or_Hiragana` `v`: S `Script=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script=Katakana_Or_Hiragana`
- `U2` input `pair/sc/Hrkt` `u`: S `Script=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script=Katakana_Or_Hiragana`
- `U2` input `pair/sc/Hrkt` `v`: S `Script=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script=Katakana_Or_Hiragana`
- `U2` input `pair/sc/Katakana_Or_Hiragana` `u`: S `Script=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script=Katakana_Or_Hiragana`
- `U2` input `pair/sc/Katakana_Or_Hiragana` `v`: S `Script=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script=Katakana_Or_Hiragana`
- `U2` input `pair/Script_Extensions/Hrkt` `u`: S `Script_Extensions=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script_Extensions=Katakana_Or_Hiragana`
- `U2` input `pair/Script_Extensions/Hrkt` `v`: S `Script_Extensions=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script_Extensions=Katakana_Or_Hiragana`
- `U2` input `pair/Script_Extensions/Katakana_Or_Hiragana` `u`: S `Script_Extensions=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script_Extensions=Katakana_Or_Hiragana`
- `U2` input `pair/Script_Extensions/Katakana_Or_Hiragana` `v`: S `Script_Extensions=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script_Extensions=Katakana_Or_Hiragana`
- `U2` input `pair/scx/Hrkt` `u`: S `Script_Extensions=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script_Extensions=Katakana_Or_Hiragana`
- `U2` input `pair/scx/Hrkt` `v`: S `Script_Extensions=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script_Extensions=Katakana_Or_Hiragana`
- `U2` input `pair/scx/Katakana_Or_Hiragana` `u`: S `Script_Extensions=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script_Extensions=Katakana_Or_Hiragana`
- `U2` input `pair/scx/Katakana_Or_Hiragana` `v`: S `Script_Extensions=Katakana_Or_Hiragana`, arm `refused`, arbiter `Script_Extensions=Katakana_Or_Hiragana`

## lower, arm J:JSString.ToLocaleLowerCase(undefined): (a'), row C2, 760

correction of Broiler.JS: Final_Sigma reads Cased and Case_Ignorable from the runtime's data, which does not know code points Unicode 16 and 17 add (SJL-D-6 (c)).

- `U3` input `0041 1ACF 03A3` `default`: S `0061 1ACF 03C2`, arm `0061 1ACF 03C3`, arbiter `0061 1ACF 03C2`
- `U3` input `0041 1AD0 03A3` `default`: S `0061 1AD0 03C2`, arm `0061 1AD0 03C3`, arbiter `0061 1AD0 03C2`
- `U3` input `0041 1AD1 03A3` `default`: S `0061 1AD1 03C2`, arm `0061 1AD1 03C3`, arbiter `0061 1AD1 03C2`
- `U3` input `0041 1AD2 03A3` `default`: S `0061 1AD2 03C2`, arm `0061 1AD2 03C3`, arbiter `0061 1AD2 03C2`
- `U3` input `0041 1AD3 03A3` `default`: S `0061 1AD3 03C2`, arm `0061 1AD3 03C3`, arbiter `0061 1AD3 03C2`
- `U3` input `0041 1AD4 03A3` `default`: S `0061 1AD4 03C2`, arm `0061 1AD4 03C3`, arbiter `0061 1AD4 03C2`
- `U3` input `0041 1AD5 03A3` `default`: S `0061 1AD5 03C2`, arm `0061 1AD5 03C3`, arbiter `0061 1AD5 03C2`
- `U3` input `0041 1AD6 03A3` `default`: S `0061 1AD6 03C2`, arm `0061 1AD6 03C3`, arbiter `0061 1AD6 03C2`
- `U3` input `0041 1AD7 03A3` `default`: S `0061 1AD7 03C2`, arm `0061 1AD7 03C3`, arbiter `0061 1AD7 03C2`
- `U3` input `0041 1AD8 03A3` `default`: S `0061 1AD8 03C2`, arm `0061 1AD8 03C3`, arbiter `0061 1AD8 03C2`
- `U3` input `0041 1AD9 03A3` `default`: S `0061 1AD9 03C2`, arm `0061 1AD9 03C3`, arbiter `0061 1AD9 03C2`
- `U3` input `0041 1ADA 03A3` `default`: S `0061 1ADA 03C2`, arm `0061 1ADA 03C3`, arbiter `0061 1ADA 03C2`
- `U3` input `0041 1ADB 03A3` `default`: S `0061 1ADB 03C2`, arm `0061 1ADB 03C3`, arbiter `0061 1ADB 03C2`
- `U3` input `0041 1ADC 03A3` `default`: S `0061 1ADC 03C2`, arm `0061 1ADC 03C3`, arbiter `0061 1ADC 03C2`
- `U3` input `0041 1ADD 03A3` `default`: S `0061 1ADD 03C2`, arm `0061 1ADD 03C3`, arbiter `0061 1ADD 03C2`
- `U3` input `0041 1AE0 03A3` `default`: S `0061 1AE0 03C2`, arm `0061 1AE0 03C3`, arbiter `0061 1AE0 03C2`
- `U3` input `0041 1AE1 03A3` `default`: S `0061 1AE1 03C2`, arm `0061 1AE1 03C3`, arbiter `0061 1AE1 03C2`
- `U3` input `0041 1AE2 03A3` `default`: S `0061 1AE2 03C2`, arm `0061 1AE2 03C3`, arbiter `0061 1AE2 03C2`
- `U3` input `0041 1AE3 03A3` `default`: S `0061 1AE3 03C2`, arm `0061 1AE3 03C3`, arbiter `0061 1AE3 03C2`
- `U3` input `0041 1AE4 03A3` `default`: S `0061 1AE4 03C2`, arm `0061 1AE4 03C3`, arbiter `0061 1AE4 03C2`

## lower, arm J:JSString.ToLocaleLowerCase(undefined): (a'), row C3, 2128

correction of Broiler.JS: Broiler.JS's default mappings come from the runtime's data and hand patches (SJL-D-6 (c)).

- `U3` input `D81B DEA0` `default`: S `D81B DEBB`, arm `D81B DEA0`, arbiter `D81B DEBB`
- `U3` input `D81B DEA1` `default`: S `D81B DEBC`, arm `D81B DEA1`, arbiter `D81B DEBC`
- `U3` input `D81B DEA2` `default`: S `D81B DEBD`, arm `D81B DEA2`, arbiter `D81B DEBD`
- `U3` input `D81B DEA3` `default`: S `D81B DEBE`, arm `D81B DEA3`, arbiter `D81B DEBE`
- `U3` input `D81B DEA4` `default`: S `D81B DEBF`, arm `D81B DEA4`, arbiter `D81B DEBF`
- `U3` input `D81B DEA5` `default`: S `D81B DEC0`, arm `D81B DEA5`, arbiter `D81B DEC0`
- `U3` input `D81B DEA6` `default`: S `D81B DEC1`, arm `D81B DEA6`, arbiter `D81B DEC1`
- `U3` input `D81B DEA7` `default`: S `D81B DEC2`, arm `D81B DEA7`, arbiter `D81B DEC2`
- `U3` input `D81B DEA8` `default`: S `D81B DEC3`, arm `D81B DEA8`, arbiter `D81B DEC3`
- `U3` input `D81B DEA9` `default`: S `D81B DEC4`, arm `D81B DEA9`, arbiter `D81B DEC4`
- `U3` input `D81B DEAA` `default`: S `D81B DEC5`, arm `D81B DEAA`, arbiter `D81B DEC5`
- `U3` input `D81B DEAB` `default`: S `D81B DEC6`, arm `D81B DEAB`, arbiter `D81B DEC6`
- `U3` input `D81B DEAC` `default`: S `D81B DEC7`, arm `D81B DEAC`, arbiter `D81B DEC7`
- `U3` input `D81B DEAD` `default`: S `D81B DEC8`, arm `D81B DEAD`, arbiter `D81B DEC8`
- `U3` input `D81B DEAE` `default`: S `D81B DEC9`, arm `D81B DEAE`, arbiter `D81B DEC9`
- `U3` input `D81B DEAF` `default`: S `D81B DECA`, arm `D81B DEAF`, arbiter `D81B DECA`
- `U3` input `D81B DEB0` `default`: S `D81B DECB`, arm `D81B DEB0`, arbiter `D81B DECB`
- `U3` input `D81B DEB1` `default`: S `D81B DECC`, arm `D81B DEB1`, arbiter `D81B DECC`
- `U3` input `D81B DEB2` `default`: S `D81B DECD`, arm `D81B DEB2`, arbiter `D81B DECD`
- `U3` input `D81B DEB3` `default`: S `D81B DECE`, arm `D81B DEB3`, arbiter `D81B DECE`

## lower, arm J:JSString.ToLocaleLowerCase(undefined): (a'), row C3, 432

correction of Broiler.JS: Broiler.JS's default mappings come from the runtime's data and hand patches, and Final_Sigma reads Cased and Case_Ignorable from the same data, or Cased as row C1 reads it (SJL-D-6 (c)).

- `U3` input `0041 D81B DEA0 03A3` `default`: S `0061 D81B DEBB 03C2`, arm `0061 D81B DEA0 03C3`, arbiter `0061 D81B DEBB 03C2`
- `U3` input `0041 03A3 D81B DEA0` `default`: S `0061 03C3 D81B DEBB`, arm `0061 03C2 D81B DEA0`, arbiter `0061 03C3 D81B DEBB`
- `U3` input `D81B DEA0 03A3` `default`: S `D81B DEBB 03C2`, arm `D81B DEA0 03C3`, arbiter `D81B DEBB 03C2`
- `U3` input `0041 D81B DEA1 03A3` `default`: S `0061 D81B DEBC 03C2`, arm `0061 D81B DEA1 03C3`, arbiter `0061 D81B DEBC 03C2`
- `U3` input `0041 03A3 D81B DEA1` `default`: S `0061 03C3 D81B DEBC`, arm `0061 03C2 D81B DEA1`, arbiter `0061 03C3 D81B DEBC`
- `U3` input `D81B DEA1 03A3` `default`: S `D81B DEBC 03C2`, arm `D81B DEA1 03C3`, arbiter `D81B DEBC 03C2`
- `U3` input `0041 D81B DEA2 03A3` `default`: S `0061 D81B DEBD 03C2`, arm `0061 D81B DEA2 03C3`, arbiter `0061 D81B DEBD 03C2`
- `U3` input `0041 03A3 D81B DEA2` `default`: S `0061 03C3 D81B DEBD`, arm `0061 03C2 D81B DEA2`, arbiter `0061 03C3 D81B DEBD`
- `U3` input `D81B DEA2 03A3` `default`: S `D81B DEBD 03C2`, arm `D81B DEA2 03C3`, arbiter `D81B DEBD 03C2`
- `U3` input `0041 D81B DEA3 03A3` `default`: S `0061 D81B DEBE 03C2`, arm `0061 D81B DEA3 03C3`, arbiter `0061 D81B DEBE 03C2`
- `U3` input `0041 03A3 D81B DEA3` `default`: S `0061 03C3 D81B DEBE`, arm `0061 03C2 D81B DEA3`, arbiter `0061 03C3 D81B DEBE`
- `U3` input `D81B DEA3 03A3` `default`: S `D81B DEBE 03C2`, arm `D81B DEA3 03C3`, arbiter `D81B DEBE 03C2`
- `U3` input `0041 D81B DEA4 03A3` `default`: S `0061 D81B DEBF 03C2`, arm `0061 D81B DEA4 03C3`, arbiter `0061 D81B DEBF 03C2`
- `U3` input `0041 03A3 D81B DEA4` `default`: S `0061 03C3 D81B DEBF`, arm `0061 03C2 D81B DEA4`, arbiter `0061 03C3 D81B DEBF`
- `U3` input `D81B DEA4 03A3` `default`: S `D81B DEBF 03C2`, arm `D81B DEA4 03C3`, arbiter `D81B DEBF 03C2`
- `U3` input `0041 D81B DEA5 03A3` `default`: S `0061 D81B DEC0 03C2`, arm `0061 D81B DEA5 03C3`, arbiter `0061 D81B DEC0 03C2`
- `U3` input `0041 03A3 D81B DEA5` `default`: S `0061 03C3 D81B DEC0`, arm `0061 03C2 D81B DEA5`, arbiter `0061 03C3 D81B DEC0`
- `U3` input `D81B DEA5 03A3` `default`: S `D81B DEC0 03C2`, arm `D81B DEA5 03C3`, arbiter `D81B DEC0 03C2`
- `U3` input `0041 D81B DEA6 03A3` `default`: S `0061 D81B DEC1 03C2`, arm `0061 D81B DEA6 03C3`, arbiter `0061 D81B DEC1 03C2`
- `U3` input `0041 03A3 D81B DEA6` `default`: S `0061 03C3 D81B DEC1`, arm `0061 03C2 D81B DEA6`, arbiter `0061 03C3 D81B DEC1`

## lower, arm J:JSString.ToLocaleLowerCase(undefined): (a), row C1, 2838

correction of Broiler.JS: Final_Sigma reads Cased from DerivedCoreProperties.txt (Other_Lowercase, Other_Uppercase).

- `U3` input `0041 00AA 03A3` `default`: S `0061 00AA 03C2`, arm `0061 00AA 03C3`, arbiter `0061 00AA 03C2`
- `U3` input `0041 03A3 00AA` `default`: S `0061 03C3 00AA`, arm `0061 03C2 00AA`, arbiter `0061 03C3 00AA`
- `U3` input `00AA 03A3` `default`: S `00AA 03C2`, arm `00AA 03C3`, arbiter `00AA 03C2`
- `U3` input `0041 00BA 03A3` `default`: S `0061 00BA 03C2`, arm `0061 00BA 03C3`, arbiter `0061 00BA 03C2`
- `U3` input `0041 03A3 00BA` `default`: S `0061 03C3 00BA`, arm `0061 03C2 00BA`, arbiter `0061 03C3 00BA`
- `U3` input `00BA 03A3` `default`: S `00BA 03C2`, arm `00BA 03C3`, arbiter `00BA 03C2`
- `U3` input `0041 2160 03A3` `default`: S `0061 2170 03C2`, arm `0061 2170 03C3`, arbiter `0061 2170 03C2`
- `U3` input `0041 03A3 2160` `default`: S `0061 03C3 2170`, arm `0061 03C2 2170`, arbiter `0061 03C3 2170`
- `U3` input `2160 03A3` `default`: S `2170 03C2`, arm `2170 03C3`, arbiter `2170 03C2`
- `U3` input `0041 2161 03A3` `default`: S `0061 2171 03C2`, arm `0061 2171 03C3`, arbiter `0061 2171 03C2`
- `U3` input `0041 03A3 2161` `default`: S `0061 03C3 2171`, arm `0061 03C2 2171`, arbiter `0061 03C3 2171`
- `U3` input `2161 03A3` `default`: S `2171 03C2`, arm `2171 03C3`, arbiter `2171 03C2`
- `U3` input `0041 2162 03A3` `default`: S `0061 2172 03C2`, arm `0061 2172 03C3`, arbiter `0061 2172 03C2`
- `U3` input `0041 03A3 2162` `default`: S `0061 03C3 2172`, arm `0061 03C2 2172`, arbiter `0061 03C3 2172`
- `U3` input `2162 03A3` `default`: S `2172 03C2`, arm `2172 03C3`, arbiter `2172 03C2`
- `U3` input `0041 2163 03A3` `default`: S `0061 2173 03C2`, arm `0061 2173 03C3`, arbiter `0061 2173 03C2`
- `U3` input `0041 03A3 2163` `default`: S `0061 03C3 2173`, arm `0061 03C2 2173`, arbiter `0061 03C3 2173`
- `U3` input `2163 03A3` `default`: S `2173 03C2`, arm `2173 03C3`, arbiter `2173 03C2`
- `U3` input `0041 2164 03A3` `default`: S `0061 2174 03C2`, arm `0061 2174 03C3`, arbiter `0061 2174 03C2`
- `U3` input `0041 03A3 2164` `default`: S `0061 03C3 2174`, arm `0061 03C2 2174`, arbiter `0061 03C3 2174`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a'), row C2, 2227

correction of Broiler.JS: Final_Sigma reads Cased and Case_Ignorable from the runtime's data, which does not know code points Unicode 16 and 17 add (SJL-D-6 (c)).

- `U3` input `0041 1ACF 03A3` `default`: S `0061 1ACF 03C2`, arm `0061 1ACF 03C3`, arbiter `0061 1ACF 03C2`
- `U3` input `0041 1ACF 03A3` `turkic`: S `0061 1ACF 03C2`, arm `0061 1ACF 03C3`, arbiter `0061 1ACF 03C2`
- `U3` input `0041 1ACF 03A3` `lithuanian`: S `0061 1ACF 03C2`, arm `0061 1ACF 03C3`, arbiter `0061 1ACF 03C2`
- `U3` input `0041 1AD0 03A3` `default`: S `0061 1AD0 03C2`, arm `0061 1AD0 03C3`, arbiter `0061 1AD0 03C2`
- `U3` input `0041 1AD0 03A3` `turkic`: S `0061 1AD0 03C2`, arm `0061 1AD0 03C3`, arbiter `0061 1AD0 03C2`
- `U3` input `0041 1AD0 03A3` `lithuanian`: S `0061 1AD0 03C2`, arm `0061 1AD0 03C3`, arbiter `0061 1AD0 03C2`
- `U3` input `0041 1AD1 03A3` `default`: S `0061 1AD1 03C2`, arm `0061 1AD1 03C3`, arbiter `0061 1AD1 03C2`
- `U3` input `0041 1AD1 03A3` `turkic`: S `0061 1AD1 03C2`, arm `0061 1AD1 03C3`, arbiter `0061 1AD1 03C2`
- `U3` input `0041 1AD1 03A3` `lithuanian`: S `0061 1AD1 03C2`, arm `0061 1AD1 03C3`, arbiter `0061 1AD1 03C2`
- `U3` input `0041 1AD2 03A3` `default`: S `0061 1AD2 03C2`, arm `0061 1AD2 03C3`, arbiter `0061 1AD2 03C2`
- `U3` input `0041 1AD2 03A3` `turkic`: S `0061 1AD2 03C2`, arm `0061 1AD2 03C3`, arbiter `0061 1AD2 03C2`
- `U3` input `0041 1AD2 03A3` `lithuanian`: S `0061 1AD2 03C2`, arm `0061 1AD2 03C3`, arbiter `0061 1AD2 03C2`
- `U3` input `0041 1AD3 03A3` `default`: S `0061 1AD3 03C2`, arm `0061 1AD3 03C3`, arbiter `0061 1AD3 03C2`
- `U3` input `0041 1AD3 03A3` `turkic`: S `0061 1AD3 03C2`, arm `0061 1AD3 03C3`, arbiter `0061 1AD3 03C2`
- `U3` input `0041 1AD3 03A3` `lithuanian`: S `0061 1AD3 03C2`, arm `0061 1AD3 03C3`, arbiter `0061 1AD3 03C2`
- `U3` input `0041 1AD4 03A3` `default`: S `0061 1AD4 03C2`, arm `0061 1AD4 03C3`, arbiter `0061 1AD4 03C2`
- `U3` input `0041 1AD4 03A3` `turkic`: S `0061 1AD4 03C2`, arm `0061 1AD4 03C3`, arbiter `0061 1AD4 03C2`
- `U3` input `0041 1AD4 03A3` `lithuanian`: S `0061 1AD4 03C2`, arm `0061 1AD4 03C3`, arbiter `0061 1AD4 03C2`
- `U3` input `0041 1AD5 03A3` `default`: S `0061 1AD5 03C2`, arm `0061 1AD5 03C3`, arbiter `0061 1AD5 03C2`
- `U3` input `0041 1AD5 03A3` `turkic`: S `0061 1AD5 03C2`, arm `0061 1AD5 03C3`, arbiter `0061 1AD5 03C2`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a'), row C3, 6352

correction of Broiler.JS: Broiler.JS's default mappings come from the runtime's data and hand patches (SJL-D-6 (c)).

- `U3` input `D81B DEA0` `default`: S `D81B DEBB`, arm `D81B DEA0`, arbiter `D81B DEBB`
- `U3` input `D81B DEA0` `lithuanian`: S `D81B DEBB`, arm `D81B DEA0`, arbiter `D81B DEBB`
- `U3` input `D81B DEA1` `default`: S `D81B DEBC`, arm `D81B DEA1`, arbiter `D81B DEBC`
- `U3` input `D81B DEA1` `lithuanian`: S `D81B DEBC`, arm `D81B DEA1`, arbiter `D81B DEBC`
- `U3` input `D81B DEA2` `default`: S `D81B DEBD`, arm `D81B DEA2`, arbiter `D81B DEBD`
- `U3` input `D81B DEA2` `lithuanian`: S `D81B DEBD`, arm `D81B DEA2`, arbiter `D81B DEBD`
- `U3` input `D81B DEA3` `default`: S `D81B DEBE`, arm `D81B DEA3`, arbiter `D81B DEBE`
- `U3` input `D81B DEA3` `lithuanian`: S `D81B DEBE`, arm `D81B DEA3`, arbiter `D81B DEBE`
- `U3` input `D81B DEA4` `default`: S `D81B DEBF`, arm `D81B DEA4`, arbiter `D81B DEBF`
- `U3` input `D81B DEA4` `lithuanian`: S `D81B DEBF`, arm `D81B DEA4`, arbiter `D81B DEBF`
- `U3` input `D81B DEA5` `default`: S `D81B DEC0`, arm `D81B DEA5`, arbiter `D81B DEC0`
- `U3` input `D81B DEA5` `lithuanian`: S `D81B DEC0`, arm `D81B DEA5`, arbiter `D81B DEC0`
- `U3` input `D81B DEA6` `default`: S `D81B DEC1`, arm `D81B DEA6`, arbiter `D81B DEC1`
- `U3` input `D81B DEA6` `lithuanian`: S `D81B DEC1`, arm `D81B DEA6`, arbiter `D81B DEC1`
- `U3` input `D81B DEA7` `default`: S `D81B DEC2`, arm `D81B DEA7`, arbiter `D81B DEC2`
- `U3` input `D81B DEA7` `lithuanian`: S `D81B DEC2`, arm `D81B DEA7`, arbiter `D81B DEC2`
- `U3` input `D81B DEA8` `default`: S `D81B DEC3`, arm `D81B DEA8`, arbiter `D81B DEC3`
- `U3` input `D81B DEA8` `lithuanian`: S `D81B DEC3`, arm `D81B DEA8`, arbiter `D81B DEC3`
- `U3` input `D81B DEA9` `default`: S `D81B DEC4`, arm `D81B DEA9`, arbiter `D81B DEC4`
- `U3` input `D81B DEA9` `lithuanian`: S `D81B DEC4`, arm `D81B DEA9`, arbiter `D81B DEC4`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a'), row C3, 864

correction of Broiler.JS: Broiler.JS's default mappings come from the runtime's data and hand patches, and Final_Sigma reads Cased and Case_Ignorable from the same data, or Cased as row C1 reads it (SJL-D-6 (c)).

- `U3` input `0041 D81B DEA0 03A3` `default`: S `0061 D81B DEBB 03C2`, arm `0061 D81B DEA0 03C3`, arbiter `0061 D81B DEBB 03C2`
- `U3` input `0041 D81B DEA0 03A3` `lithuanian`: S `0061 D81B DEBB 03C2`, arm `0061 D81B DEA0 03C3`, arbiter `0061 D81B DEBB 03C2`
- `U3` input `0041 03A3 D81B DEA0` `default`: S `0061 03C3 D81B DEBB`, arm `0061 03C2 D81B DEA0`, arbiter `0061 03C3 D81B DEBB`
- `U3` input `0041 03A3 D81B DEA0` `lithuanian`: S `0061 03C3 D81B DEBB`, arm `0061 03C2 D81B DEA0`, arbiter `0061 03C3 D81B DEBB`
- `U3` input `D81B DEA0 03A3` `default`: S `D81B DEBB 03C2`, arm `D81B DEA0 03C3`, arbiter `D81B DEBB 03C2`
- `U3` input `D81B DEA0 03A3` `lithuanian`: S `D81B DEBB 03C2`, arm `D81B DEA0 03C3`, arbiter `D81B DEBB 03C2`
- `U3` input `0041 D81B DEA1 03A3` `default`: S `0061 D81B DEBC 03C2`, arm `0061 D81B DEA1 03C3`, arbiter `0061 D81B DEBC 03C2`
- `U3` input `0041 D81B DEA1 03A3` `lithuanian`: S `0061 D81B DEBC 03C2`, arm `0061 D81B DEA1 03C3`, arbiter `0061 D81B DEBC 03C2`
- `U3` input `0041 03A3 D81B DEA1` `default`: S `0061 03C3 D81B DEBC`, arm `0061 03C2 D81B DEA1`, arbiter `0061 03C3 D81B DEBC`
- `U3` input `0041 03A3 D81B DEA1` `lithuanian`: S `0061 03C3 D81B DEBC`, arm `0061 03C2 D81B DEA1`, arbiter `0061 03C3 D81B DEBC`
- `U3` input `D81B DEA1 03A3` `default`: S `D81B DEBC 03C2`, arm `D81B DEA1 03C3`, arbiter `D81B DEBC 03C2`
- `U3` input `D81B DEA1 03A3` `lithuanian`: S `D81B DEBC 03C2`, arm `D81B DEA1 03C3`, arbiter `D81B DEBC 03C2`
- `U3` input `0041 D81B DEA2 03A3` `default`: S `0061 D81B DEBD 03C2`, arm `0061 D81B DEA2 03C3`, arbiter `0061 D81B DEBD 03C2`
- `U3` input `0041 D81B DEA2 03A3` `lithuanian`: S `0061 D81B DEBD 03C2`, arm `0061 D81B DEA2 03C3`, arbiter `0061 D81B DEBD 03C2`
- `U3` input `0041 03A3 D81B DEA2` `default`: S `0061 03C3 D81B DEBD`, arm `0061 03C2 D81B DEA2`, arbiter `0061 03C3 D81B DEBD`
- `U3` input `0041 03A3 D81B DEA2` `lithuanian`: S `0061 03C3 D81B DEBD`, arm `0061 03C2 D81B DEA2`, arbiter `0061 03C3 D81B DEBD`
- `U3` input `D81B DEA2 03A3` `default`: S `D81B DEBD 03C2`, arm `D81B DEA2 03C3`, arbiter `D81B DEBD 03C2`
- `U3` input `D81B DEA2 03A3` `lithuanian`: S `D81B DEBD 03C2`, arm `D81B DEA2 03C3`, arbiter `D81B DEBD 03C2`
- `U3` input `0041 D81B DEA3 03A3` `default`: S `0061 D81B DEBE 03C2`, arm `0061 D81B DEA3 03C3`, arbiter `0061 D81B DEBE 03C2`
- `U3` input `0041 D81B DEA3 03A3` `lithuanian`: S `0061 D81B DEBE 03C2`, arm `0061 D81B DEA3 03C3`, arbiter `0061 D81B DEBE 03C2`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a), row C1, 8513

correction of Broiler.JS: Final_Sigma reads Cased from DerivedCoreProperties.txt (Other_Lowercase, Other_Uppercase).

- `U3` input `0041 00AA 03A3` `default`: S `0061 00AA 03C2`, arm `0061 00AA 03C3`, arbiter `0061 00AA 03C2`
- `U3` input `0041 00AA 03A3` `turkic`: S `0061 00AA 03C2`, arm `0061 00AA 03C3`, arbiter `0061 00AA 03C2`
- `U3` input `0041 00AA 03A3` `lithuanian`: S `0061 00AA 03C2`, arm `0061 00AA 03C3`, arbiter `0061 00AA 03C2`
- `U3` input `0041 03A3 00AA` `default`: S `0061 03C3 00AA`, arm `0061 03C2 00AA`, arbiter `0061 03C3 00AA`
- `U3` input `0041 03A3 00AA` `turkic`: S `0061 03C3 00AA`, arm `0061 03C2 00AA`, arbiter `0061 03C3 00AA`
- `U3` input `0041 03A3 00AA` `lithuanian`: S `0061 03C3 00AA`, arm `0061 03C2 00AA`, arbiter `0061 03C3 00AA`
- `U3` input `00AA 03A3` `default`: S `00AA 03C2`, arm `00AA 03C3`, arbiter `00AA 03C2`
- `U3` input `00AA 03A3` `turkic`: S `00AA 03C2`, arm `00AA 03C3`, arbiter `00AA 03C2`
- `U3` input `00AA 03A3` `lithuanian`: S `00AA 03C2`, arm `00AA 03C3`, arbiter `00AA 03C2`
- `U3` input `0041 00BA 03A3` `default`: S `0061 00BA 03C2`, arm `0061 00BA 03C3`, arbiter `0061 00BA 03C2`
- `U3` input `0041 00BA 03A3` `turkic`: S `0061 00BA 03C2`, arm `0061 00BA 03C3`, arbiter `0061 00BA 03C2`
- `U3` input `0041 00BA 03A3` `lithuanian`: S `0061 00BA 03C2`, arm `0061 00BA 03C3`, arbiter `0061 00BA 03C2`
- `U3` input `0041 03A3 00BA` `default`: S `0061 03C3 00BA`, arm `0061 03C2 00BA`, arbiter `0061 03C3 00BA`
- `U3` input `0041 03A3 00BA` `turkic`: S `0061 03C3 00BA`, arm `0061 03C2 00BA`, arbiter `0061 03C3 00BA`
- `U3` input `0041 03A3 00BA` `lithuanian`: S `0061 03C3 00BA`, arm `0061 03C2 00BA`, arbiter `0061 03C3 00BA`
- `U3` input `00BA 03A3` `default`: S `00BA 03C2`, arm `00BA 03C3`, arbiter `00BA 03C2`
- `U3` input `00BA 03A3` `turkic`: S `00BA 03C2`, arm `00BA 03C3`, arbiter `00BA 03C2`
- `U3` input `00BA 03A3` `lithuanian`: S `00BA 03C2`, arm `00BA 03C3`, arbiter `00BA 03C2`
- `U3` input `0041 2160 03A3` `default`: S `0061 2170 03C2`, arm `0061 2170 03C3`, arbiter `0061 2170 03C2`
- `U3` input `0041 2160 03A3` `turkic`: S `0061 2170 03C2`, arm `0061 2170 03C3`, arbiter `0061 2170 03C2`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a), row C4, 3121

correction of Broiler.JS: the Turkic lower-case path skips the class's table, so the runtime's older data shows.

- `U3` input `1C89` `turkic`: S `1C8A`, arm `1C89`, arbiter `1C8A`
- `U3` input `A7CB` `turkic`: S `0264`, arm `A7CB`, arbiter `0264`
- `U3` input `A7CC` `turkic`: S `A7CD`, arm `A7CC`, arbiter `A7CD`
- `U3` input `A7CE` `turkic`: S `A7CF`, arm `A7CE`, arbiter `A7CF`
- `U3` input `A7D2` `turkic`: S `A7D3`, arm `A7D2`, arbiter `A7D3`
- `U3` input `A7D4` `turkic`: S `A7D5`, arm `A7D4`, arbiter `A7D5`
- `U3` input `A7DA` `turkic`: S `A7DB`, arm `A7DA`, arbiter `A7DB`
- `U3` input `A7DC` `turkic`: S `019B`, arm `A7DC`, arbiter `019B`
- `U3` input `D803 DD50` `turkic`: S `D803 DD70`, arm `D803 DD50`, arbiter `D803 DD70`
- `U3` input `D803 DD51` `turkic`: S `D803 DD71`, arm `D803 DD51`, arbiter `D803 DD71`
- `U3` input `D803 DD52` `turkic`: S `D803 DD72`, arm `D803 DD52`, arbiter `D803 DD72`
- `U3` input `D803 DD53` `turkic`: S `D803 DD73`, arm `D803 DD53`, arbiter `D803 DD73`
- `U3` input `D803 DD54` `turkic`: S `D803 DD74`, arm `D803 DD54`, arbiter `D803 DD74`
- `U3` input `D803 DD55` `turkic`: S `D803 DD75`, arm `D803 DD55`, arbiter `D803 DD75`
- `U3` input `D803 DD56` `turkic`: S `D803 DD76`, arm `D803 DD56`, arbiter `D803 DD76`
- `U3` input `D803 DD57` `turkic`: S `D803 DD77`, arm `D803 DD57`, arbiter `D803 DD77`
- `U3` input `D803 DD58` `turkic`: S `D803 DD78`, arm `D803 DD58`, arbiter `D803 DD78`
- `U3` input `D803 DD59` `turkic`: S `D803 DD79`, arm `D803 DD59`, arbiter `D803 DD79`
- `U3` input `D803 DD5A` `turkic`: S `D803 DD7A`, arm `D803 DD5A`, arbiter `D803 DD7A`
- `U3` input `D803 DD5B` `turkic`: S `D803 DD7B`, arm `D803 DD5B`, arbiter `D803 DD7B`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a), row C4, 485

correction of Broiler.JS: the Turkic lower-case path skips the class's table, so the runtime's older data shows, in the mappings and in Final_Sigma, whose Cased is read from the same data or as row C1 reads it.

- `U3` input `0041 A7CE 03A3` `turkic`: S `0061 A7CF 03C2`, arm `0061 A7CE 03C3`, arbiter `0061 A7CF 03C2`
- `U3` input `0041 03A3 A7CE` `turkic`: S `0061 03C3 A7CF`, arm `0061 03C2 A7CE`, arbiter `0061 03C3 A7CF`
- `U3` input `A7CE 03A3` `turkic`: S `A7CF 03C2`, arm `A7CE 03C3`, arbiter `A7CF 03C2`
- `U3` input `0041 A7D2 03A3` `turkic`: S `0061 A7D3 03C2`, arm `0061 A7D2 03C3`, arbiter `0061 A7D3 03C2`
- `U3` input `0041 03A3 A7D2` `turkic`: S `0061 03C3 A7D3`, arm `0061 03C2 A7D2`, arbiter `0061 03C3 A7D3`
- `U3` input `A7D2 03A3` `turkic`: S `A7D3 03C2`, arm `A7D2 03C3`, arbiter `A7D3 03C2`
- `U3` input `0041 A7D4 03A3` `turkic`: S `0061 A7D5 03C2`, arm `0061 A7D4 03C3`, arbiter `0061 A7D5 03C2`
- `U3` input `0041 03A3 A7D4` `turkic`: S `0061 03C3 A7D5`, arm `0061 03C2 A7D4`, arbiter `0061 03C3 A7D5`
- `U3` input `A7D4 03A3` `turkic`: S `A7D5 03C2`, arm `A7D4 03C3`, arbiter `A7D5 03C2`
- `U3` input `0041 D81B DEA0 03A3` `turkic`: S `0061 D81B DEBB 03C2`, arm `0061 D81B DEA0 03C3`, arbiter `0061 D81B DEBB 03C2`
- `U3` input `0041 03A3 D81B DEA0` `turkic`: S `0061 03C3 D81B DEBB`, arm `0061 03C2 D81B DEA0`, arbiter `0061 03C3 D81B DEBB`
- `U3` input `D81B DEA0 03A3` `turkic`: S `D81B DEBB 03C2`, arm `D81B DEA0 03C3`, arbiter `D81B DEBB 03C2`
- `U3` input `0041 D81B DEA1 03A3` `turkic`: S `0061 D81B DEBC 03C2`, arm `0061 D81B DEA1 03C3`, arbiter `0061 D81B DEBC 03C2`
- `U3` input `0041 03A3 D81B DEA1` `turkic`: S `0061 03C3 D81B DEBC`, arm `0061 03C2 D81B DEA1`, arbiter `0061 03C3 D81B DEBC`
- `U3` input `D81B DEA1 03A3` `turkic`: S `D81B DEBC 03C2`, arm `D81B DEA1 03C3`, arbiter `D81B DEBC 03C2`
- `U3` input `0041 D81B DEA2 03A3` `turkic`: S `0061 D81B DEBD 03C2`, arm `0061 D81B DEA2 03C3`, arbiter `0061 D81B DEBD 03C2`
- `U3` input `0041 03A3 D81B DEA2` `turkic`: S `0061 03C3 D81B DEBD`, arm `0061 03C2 D81B DEA2`, arbiter `0061 03C3 D81B DEBD`
- `U3` input `D81B DEA2 03A3` `turkic`: S `D81B DEBD 03C2`, arm `D81B DEA2 03C3`, arbiter `D81B DEBD 03C2`
- `U3` input `0041 D81B DEA3 03A3` `turkic`: S `0061 D81B DEBE 03C2`, arm `0061 D81B DEA3 03C3`, arbiter `0061 D81B DEBE 03C2`
- `U3` input `0041 03A3 D81B DEA3` `turkic`: S `0061 03C3 D81B DEBE`, arm `0061 03C2 D81B DEA3`, arbiter `0061 03C3 D81B DEBE`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a), row C6, 15

correction of Broiler.JS: After_I, Before_Dot, After_Soft_Dotted and More_Above admit no intervening character of combining class 0 or 230; IsContextTransparent reads a mark not of class 230.

- `U3` input `0049 0903 0307` `turkic`: S `0131 0903 0307`, arm `0069 0903`, arbiter `0131 0903 0307`
- `U3` input `D800 0049 0903 0307` `turkic`: S `D800 0131 0903 0307`, arm `D800 0069 0903`, arbiter `D800 0131 0903 0307`
- `U3` input `DC00 0049 0903 0307` `turkic`: S `DC00 0131 0903 0307`, arm `DC00 0069 0903`, arbiter `DC00 0131 0903 0307`
- `U3` input `0049 0903 0307 D800` `turkic`: S `0131 0903 0307 D800`, arm `0069 0903 D800`, arbiter `0131 0903 0307 D800`
- `U3` input `0049 0903 0307 DC00` `turkic`: S `0131 0903 0307 DC00`, arm `0069 0903 DC00`, arbiter `0131 0903 0307 DC00`
- `U3` input `000F 1ADB 0049 D804 DEE1 0307 0ABC 0130 D806 DCA7 DB40 DDB7 0052` `turkic`: S `000F 1ADB 0131 D804 DEE1 0307 0ABC 0069 D806 DCC7 DB40 DDB7 0072`, arm `000F 1ADB 0069 D804 DEE1 0ABC 0069 D806 DCC7 DB40 DDB7 0072`, arbiter `000F 1ADB 0131 D804 DEE1 0307 0ABC 0069 D806 DCC7 DB40 DDB7 0072`
- `U3` input `001B 0041 DB40 DC76 0049 17BD 0307` `turkic`: S `001B 0061 DB40 DC76 0131 17BD 0307`, arm `001B 0061 DB40 DC76 0069 17BD`, arbiter `001B 0061 DB40 DC76 0131 17BD 0307`
- `U3` input `000E 1AD2 0041 0131 0049 D836 DE9C 0307 0037 030D 1DDE 04C7` `turkic`: S `000E 1AD2 0061 0131 0131 D836 DE9C 0307 0037 030D 1DDE 04C8`, arm `000E 1AD2 0061 0131 0069 D836 DE9C 0037 030D 1DDE 04C8`, arbiter `000E 1AD2 0061 0131 0131 D836 DE9C 0307 0037 030D 1DDE 04C8`
- `U3` input `001D 0031 A8F1 0049 D806 DE91 0307 0F83 003B D838 DD36 D836 DE9F 000A` `turkic`: S `001D 0031 A8F1 0131 D806 DE91 0307 0F83 003B D838 DD36 D836 DE9F 000A`, arm `001D 0031 A8F1 0069 D806 DE91 0F83 003B D838 DD36 D836 DE9F 000A`, arbiter `001D 0031 A8F1 0131 D806 DE91 0307 0F83 003B D838 DD36 D836 DE9F 000A`
- `U3` input `05C1 0049 0C56 DB40 DD4B 0307 D804 DCB9 1D84 0354 1F6A` `turkic`: S `05C1 0131 0C56 DB40 DD4B 0307 D804 DCB9 1D84 0354 1F62`, arm `05C1 0069 0C56 DB40 DD4B D804 DCB9 1D84 0354 1F62`, arbiter `05C1 0131 0C56 DB40 DD4B 0307 D804 DCB9 1D84 0354 1F62`
- `U3` input `0300 D801 DF9A D805 DDBA D818 DD21 0035 08CA 003D 0049 0CF3 0307 0F37 0DD3` `turkic`: S `0300 D801 DF9A D805 DDBA D818 DD21 0035 08CA 003D 0131 0CF3 0307 0F37 0DD3`, arm `0300 D801 DF9A D805 DDBA D818 DD21 0035 08CA 003D 0069 0CF3 0F37 0DD3`, arbiter `0300 D801 DF9A D805 DDBA D818 DD21 0035 08CA 003D 0131 0CF3 0307 0F37 0DD3`
- `U3` input `1ED7 0069 0011 004A 0059 0059 031E 0131 0049 D836 DE00 0307` `turkic`: S `1ED7 0069 0011 006A 0079 0079 031E 0131 0131 D836 DE00 0307`, arm `1ED7 0069 0011 006A 0079 0079 031E 0131 0069 D836 DE00`, arbiter `1ED7 0069 0011 006A 0079 0079 031E 0131 0131 D836 DE00 0307`
- `U3` input `13A4 0049 0FB4 1ACA D802 DE39 D806 DE07 0307` `turkic`: S `AB74 0131 0FB4 1ACA D802 DE39 D806 DE07 0307`, arm `AB74 0069 0FB4 1ACA D802 DE39 D806 DE07`, arbiter `AB74 0131 0FB4 1ACA D802 DE39 D806 DE07 0307`
- `U3` input `1DE8 A698 0049 D805 DCBF 0307 D804 DF70 0046 D802 DEE6 1EA2 20D1 059C D838 DC21` `turkic`: S `1DE8 A699 0131 D805 DCBF 0307 D804 DF70 0066 D802 DEE6 1EA3 20D1 059C D838 DC21`, arm `1DE8 A699 0069 D805 DCBF D804 DF70 0066 D802 DEE6 1EA3 20D1 059C D838 DC21`, arbiter `1DE8 A699 0131 D805 DCBF 0307 D804 DF70 0066 D802 DEE6 1EA3 20D1 059C D838 DC21`
- `U3` input `D80D DC34 D801 DDAB 0049 1938 20D2 0307 006B` `turkic`: S `D80D DC34 D801 DDAB 0131 1938 20D2 0307 006B`, arm `D80D DC34 D801 DDAB 0069 1938 20D2 006B`, arbiter `D80D DC34 D801 DDAB 0131 1938 20D2 0307 006B`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a), row C6, 6068

correction of Broiler.JS: More_Above: no intervening character of combining class 0 or 230.

- `U3` input `0049 0903 0307` `lithuanian`: S `0069 0903 0307`, arm `0069 0307 0903 0307`, arbiter `0069 0903 0307`
- `U3` input `004A 0903 0307` `lithuanian`: S `006A 0903 0307`, arm `006A 0307 0903 0307`, arbiter `006A 0903 0307`
- `U3` input `012E 0903 0307` `lithuanian`: S `012F 0903 0307`, arm `012F 0307 0903 0307`, arbiter `012F 0903 0307`
- `U3` input `004A 034F 0300` `lithuanian`: S `006A 034F 0300`, arm `006A 0307 034F 0300`, arbiter `006A 034F 0300`
- `U3` input `004A 0488 0300` `lithuanian`: S `006A 0488 0300`, arm `006A 0307 0488 0300`, arbiter `006A 0488 0300`
- `U3` input `004A 0489 0300` `lithuanian`: S `006A 0489 0300`, arm `006A 0307 0489 0300`, arbiter `006A 0489 0300`
- `U3` input `004A 07A6 0300` `lithuanian`: S `006A 07A6 0300`, arm `006A 0307 07A6 0300`, arbiter `006A 07A6 0300`
- `U3` input `004A 07A7 0300` `lithuanian`: S `006A 07A7 0300`, arm `006A 0307 07A7 0300`, arbiter `006A 07A7 0300`
- `U3` input `004A 07A8 0300` `lithuanian`: S `006A 07A8 0300`, arm `006A 0307 07A8 0300`, arbiter `006A 07A8 0300`
- `U3` input `004A 07A9 0300` `lithuanian`: S `006A 07A9 0300`, arm `006A 0307 07A9 0300`, arbiter `006A 07A9 0300`
- `U3` input `004A 07AA 0300` `lithuanian`: S `006A 07AA 0300`, arm `006A 0307 07AA 0300`, arbiter `006A 07AA 0300`
- `U3` input `004A 07AB 0300` `lithuanian`: S `006A 07AB 0300`, arm `006A 0307 07AB 0300`, arbiter `006A 07AB 0300`
- `U3` input `004A 07AC 0300` `lithuanian`: S `006A 07AC 0300`, arm `006A 0307 07AC 0300`, arbiter `006A 07AC 0300`
- `U3` input `004A 07AD 0300` `lithuanian`: S `006A 07AD 0300`, arm `006A 0307 07AD 0300`, arbiter `006A 07AD 0300`
- `U3` input `004A 07AE 0300` `lithuanian`: S `006A 07AE 0300`, arm `006A 0307 07AE 0300`, arbiter `006A 07AE 0300`
- `U3` input `004A 07AF 0300` `lithuanian`: S `006A 07AF 0300`, arm `006A 0307 07AF 0300`, arbiter `006A 07AF 0300`
- `U3` input `004A 07B0 0300` `lithuanian`: S `006A 07B0 0300`, arm `006A 0307 07B0 0300`, arbiter `006A 07B0 0300`
- `U3` input `004A 0900 0300` `lithuanian`: S `006A 0900 0300`, arm `006A 0307 0900 0300`, arbiter `006A 0900 0300`
- `U3` input `004A 0901 0300` `lithuanian`: S `006A 0901 0300`, arm `006A 0307 0901 0300`, arbiter `006A 0901 0300`
- `U3` input `004A 0902 0300` `lithuanian`: S `006A 0902 0300`, arm `006A 0307 0902 0300`, arbiter `006A 0902 0300`

## lower, arm J:JSString.ToLowerCase and ToLocaleLowerCase(locale): (a), row C7, 2

correction of Broiler.JS: ECMA-402's TransformCase canonicalizes the list and takes its first element.

- `U3` input `0069` `tr,en`: S `0069`, arm `exception:RangeError`, arbiter `0069`
- `U3` input `0069` `tr,en`: S `0069`, arm `exception:RangeError`, arbiter `0069`

## lower, arm V:JsRealm.StringChangeCase and TransformCase: (a), row C12, 5188301

correction of Broiler.VM: the maximum result length is checked on the whole result.

- `U3` input `0000` `default;max=0`: S `over`, arm `0000`, arbiter `over`
- `U3` input `0000` `turkic;max=0`: S `over`, arm `0000`, arbiter `over`
- `U3` input `0000` `lithuanian;max=0`: S `over`, arm `0000`, arbiter `over`
- `U3` input `0001` `default;max=0`: S `over`, arm `0001`, arbiter `over`
- `U3` input `0001` `turkic;max=0`: S `over`, arm `0001`, arbiter `over`
- `U3` input `0001` `lithuanian;max=0`: S `over`, arm `0001`, arbiter `over`
- `U3` input `0002` `default;max=0`: S `over`, arm `0002`, arbiter `over`
- `U3` input `0002` `turkic;max=0`: S `over`, arm `0002`, arbiter `over`
- `U3` input `0002` `lithuanian;max=0`: S `over`, arm `0002`, arbiter `over`
- `U3` input `0003` `default;max=0`: S `over`, arm `0003`, arbiter `over`
- `U3` input `0003` `turkic;max=0`: S `over`, arm `0003`, arbiter `over`
- `U3` input `0003` `lithuanian;max=0`: S `over`, arm `0003`, arbiter `over`
- `U3` input `0004` `default;max=0`: S `over`, arm `0004`, arbiter `over`
- `U3` input `0004` `turkic;max=0`: S `over`, arm `0004`, arbiter `over`
- `U3` input `0004` `lithuanian;max=0`: S `over`, arm `0004`, arbiter `over`
- `U3` input `0005` `default;max=0`: S `over`, arm `0005`, arbiter `over`
- `U3` input `0005` `turkic;max=0`: S `over`, arm `0005`, arbiter `over`
- `U3` input `0005` `lithuanian;max=0`: S `over`, arm `0005`, arbiter `over`
- `U3` input `0006` `default;max=0`: S `over`, arm `0006`, arbiter `over`
- `U3` input `0006` `turkic;max=0`: S `over`, arm `0006`, arbiter `over`

## normalize, arm J:JSIntlCollator.Compare's NFD: (a'), row N2, 864

correction of Broiler.JS: the host's ICU carries older Unicode data (SJL-D-6 (c)).

- `U4` input `D801 DDC9` `NFD`: S `D801 DDD2 0307`, arm `D801 DDC9`, arbiter `D801 DDD2 0307`
- `U4` input `D801 DDE4` `NFD`: S `D801 DDDA 0307`, arm `D801 DDE4`, arbiter `D801 DDDA 0307`
- `U4` input `D804 DF83` `NFD`: S `D804 DF82 D804 DFC9`, arm `D804 DF83`, arbiter `D804 DF82 D804 DFC9`
- `U4` input `D804 DF85` `NFD`: S `D804 DF84 D804 DFBB`, arm `D804 DF85`, arbiter `D804 DF84 D804 DFBB`
- `U4` input `D804 DF8E` `NFD`: S `D804 DF8B D804 DFC2`, arm `D804 DF8E`, arbiter `D804 DF8B D804 DFC2`
- `U4` input `D804 DF91` `NFD`: S `D804 DF90 D804 DFC9`, arm `D804 DF91`, arbiter `D804 DF90 D804 DFC9`
- `U4` input `D804 DFC5` `NFD`: S `D804 DFC2 D804 DFC2`, arm `D804 DFC5`, arbiter `D804 DFC2 D804 DFC2`
- `U4` input `D804 DFC7` `NFD`: S `D804 DFC2 D804 DFB8`, arm `D804 DFC7`, arbiter `D804 DFC2 D804 DFB8`
- `U4` input `D804 DFC8` `NFD`: S `D804 DFC2 D804 DFC9`, arm `D804 DFC8`, arbiter `D804 DFC2 D804 DFC9`
- `U4` input `D818 DD21` `NFD`: S `D818 DD1E D818 DD1E`, arm `D818 DD21`, arbiter `D818 DD1E D818 DD1E`
- `U4` input `D818 DD22` `NFD`: S `D818 DD1E D818 DD29`, arm `D818 DD22`, arbiter `D818 DD1E D818 DD29`
- `U4` input `D818 DD23` `NFD`: S `D818 DD1E D818 DD1F`, arm `D818 DD23`, arbiter `D818 DD1E D818 DD1F`
- `U4` input `D818 DD24` `NFD`: S `D818 DD29 D818 DD1F`, arm `D818 DD24`, arbiter `D818 DD29 D818 DD1F`
- `U4` input `D818 DD25` `NFD`: S `D818 DD1E D818 DD20`, arm `D818 DD25`, arbiter `D818 DD1E D818 DD20`
- `U4` input `D818 DD26` `NFD`: S `D818 DD1E D818 DD1E D818 DD1F`, arm `D818 DD26`, arbiter `D818 DD1E D818 DD1E D818 DD1F`
- `U4` input `D818 DD27` `NFD`: S `D818 DD1E D818 DD29 D818 DD1F`, arm `D818 DD27`, arbiter `D818 DD1E D818 DD29 D818 DD1F`
- `U4` input `D818 DD28` `NFD`: S `D818 DD1E D818 DD1E D818 DD20`, arm `D818 DD28`, arbiter `D818 DD1E D818 DD1E D818 DD20`
- `U4` input `D81B DD68` `NFD`: S `D81B DD67 D81B DD67`, arm `D81B DD68`, arbiter `D81B DD67 D81B DD67`
- `U4` input `D81B DD69` `NFD`: S `D81B DD63 D81B DD67`, arm `D81B DD69`, arbiter `D81B DD63 D81B DD67`
- `U4` input `D81B DD6A` `NFD`: S `D81B DD63 D81B DD67 D81B DD67`, arm `D81B DD6A`, arbiter `D81B DD63 D81B DD67 D81B DD67`

## normalize, arm J:JSIntlCollator.Compare's NFD: (a), row N1, 23207

correction of Broiler.JS: a lone surrogate is a code point of class 0 without a decomposition.

- `U4` input `D800` `NFD`: S `D800`, arm `exception:ArgumentException`, arbiter `D800`
- `U4` input `D801` `NFD`: S `D801`, arm `exception:ArgumentException`, arbiter `D801`
- `U4` input `D802` `NFD`: S `D802`, arm `exception:ArgumentException`, arbiter `D802`
- `U4` input `D803` `NFD`: S `D803`, arm `exception:ArgumentException`, arbiter `D803`
- `U4` input `D804` `NFD`: S `D804`, arm `exception:ArgumentException`, arbiter `D804`
- `U4` input `D805` `NFD`: S `D805`, arm `exception:ArgumentException`, arbiter `D805`
- `U4` input `D806` `NFD`: S `D806`, arm `exception:ArgumentException`, arbiter `D806`
- `U4` input `D807` `NFD`: S `D807`, arm `exception:ArgumentException`, arbiter `D807`
- `U4` input `D808` `NFD`: S `D808`, arm `exception:ArgumentException`, arbiter `D808`
- `U4` input `D809` `NFD`: S `D809`, arm `exception:ArgumentException`, arbiter `D809`
- `U4` input `D80A` `NFD`: S `D80A`, arm `exception:ArgumentException`, arbiter `D80A`
- `U4` input `D80B` `NFD`: S `D80B`, arm `exception:ArgumentException`, arbiter `D80B`
- `U4` input `D80C` `NFD`: S `D80C`, arm `exception:ArgumentException`, arbiter `D80C`
- `U4` input `D80D` `NFD`: S `D80D`, arm `exception:ArgumentException`, arbiter `D80D`
- `U4` input `D80E` `NFD`: S `D80E`, arm `exception:ArgumentException`, arbiter `D80E`
- `U4` input `D80F` `NFD`: S `D80F`, arm `exception:ArgumentException`, arbiter `D80F`
- `U4` input `D810` `NFD`: S `D810`, arm `exception:ArgumentException`, arbiter `D810`
- `U4` input `D811` `NFD`: S `D811`, arm `exception:ArgumentException`, arbiter `D811`
- `U4` input `D812` `NFD`: S `D812`, arm `exception:ArgumentException`, arbiter `D812`
- `U4` input `D813` `NFD`: S `D813`, arm `exception:ArgumentException`, arbiter `D813`

## normalize, arm J:JSIntlCollator.Compare's NFD: (a), row N1, 1

correction of Broiler.JS: normalize reads code points; the runtime's normalizer refuses U+FFFE as it refuses a lone surrogate.

- `U4` input `FFFE` `NFD`: S `FFFE`, arm `exception:ArgumentException`, arbiter `FFFE`

## normalize, arm J:JSString.Normalize: (a'), row N2, 3257

correction of Broiler.JS: the host's ICU carries older Unicode data (SJL-D-6 (c)).

- `U4` input `A7F1` `NFKC`: S `0053`, arm `A7F1`, arbiter `0053`
- `U4` input `A7F1` `NFKD`: S `0053`, arm `A7F1`, arbiter `0053`
- `U4` input `D801 DDC9` `NFD`: S `D801 DDD2 0307`, arm `D801 DDC9`, arbiter `D801 DDD2 0307`
- `U4` input `D801 DDC9` `NFKD`: S `D801 DDD2 0307`, arm `D801 DDC9`, arbiter `D801 DDD2 0307`
- `U4` input `D801 DDE4` `NFD`: S `D801 DDDA 0307`, arm `D801 DDE4`, arbiter `D801 DDDA 0307`
- `U4` input `D801 DDE4` `NFKD`: S `D801 DDDA 0307`, arm `D801 DDE4`, arbiter `D801 DDDA 0307`
- `U4` input `D804 DF83` `NFD`: S `D804 DF82 D804 DFC9`, arm `D804 DF83`, arbiter `D804 DF82 D804 DFC9`
- `U4` input `D804 DF83` `NFKD`: S `D804 DF82 D804 DFC9`, arm `D804 DF83`, arbiter `D804 DF82 D804 DFC9`
- `U4` input `D804 DF85` `NFD`: S `D804 DF84 D804 DFBB`, arm `D804 DF85`, arbiter `D804 DF84 D804 DFBB`
- `U4` input `D804 DF85` `NFKD`: S `D804 DF84 D804 DFBB`, arm `D804 DF85`, arbiter `D804 DF84 D804 DFBB`
- `U4` input `D804 DF8E` `NFD`: S `D804 DF8B D804 DFC2`, arm `D804 DF8E`, arbiter `D804 DF8B D804 DFC2`
- `U4` input `D804 DF8E` `NFKD`: S `D804 DF8B D804 DFC2`, arm `D804 DF8E`, arbiter `D804 DF8B D804 DFC2`
- `U4` input `D804 DF91` `NFD`: S `D804 DF90 D804 DFC9`, arm `D804 DF91`, arbiter `D804 DF90 D804 DFC9`
- `U4` input `D804 DF91` `NFKD`: S `D804 DF90 D804 DFC9`, arm `D804 DF91`, arbiter `D804 DF90 D804 DFC9`
- `U4` input `D804 DFC5` `NFD`: S `D804 DFC2 D804 DFC2`, arm `D804 DFC5`, arbiter `D804 DFC2 D804 DFC2`
- `U4` input `D804 DFC5` `NFKD`: S `D804 DFC2 D804 DFC2`, arm `D804 DFC5`, arbiter `D804 DFC2 D804 DFC2`
- `U4` input `D804 DFC7` `NFD`: S `D804 DFC2 D804 DFB8`, arm `D804 DFC7`, arbiter `D804 DFC2 D804 DFB8`
- `U4` input `D804 DFC7` `NFKD`: S `D804 DFC2 D804 DFB8`, arm `D804 DFC7`, arbiter `D804 DFC2 D804 DFB8`
- `U4` input `D804 DFC8` `NFD`: S `D804 DFC2 D804 DFC9`, arm `D804 DFC8`, arbiter `D804 DFC2 D804 DFC9`
- `U4` input `D804 DFC8` `NFKD`: S `D804 DFC2 D804 DFC9`, arm `D804 DFC8`, arbiter `D804 DFC2 D804 DFC9`

## normalize, arm J:JSString.Normalize: (a), row N1, 92828

correction of Broiler.JS: a lone surrogate is a code point of class 0 without a decomposition.

- `U4` input `D800` `NFC`: S `D800`, arm `exception:ArgumentException`, arbiter `D800`
- `U4` input `D800` `NFD`: S `D800`, arm `exception:ArgumentException`, arbiter `D800`
- `U4` input `D800` `NFKC`: S `D800`, arm `exception:ArgumentException`, arbiter `D800`
- `U4` input `D800` `NFKD`: S `D800`, arm `exception:ArgumentException`, arbiter `D800`
- `U4` input `D801` `NFC`: S `D801`, arm `exception:ArgumentException`, arbiter `D801`
- `U4` input `D801` `NFD`: S `D801`, arm `exception:ArgumentException`, arbiter `D801`
- `U4` input `D801` `NFKC`: S `D801`, arm `exception:ArgumentException`, arbiter `D801`
- `U4` input `D801` `NFKD`: S `D801`, arm `exception:ArgumentException`, arbiter `D801`
- `U4` input `D802` `NFC`: S `D802`, arm `exception:ArgumentException`, arbiter `D802`
- `U4` input `D802` `NFD`: S `D802`, arm `exception:ArgumentException`, arbiter `D802`
- `U4` input `D802` `NFKC`: S `D802`, arm `exception:ArgumentException`, arbiter `D802`
- `U4` input `D802` `NFKD`: S `D802`, arm `exception:ArgumentException`, arbiter `D802`
- `U4` input `D803` `NFC`: S `D803`, arm `exception:ArgumentException`, arbiter `D803`
- `U4` input `D803` `NFD`: S `D803`, arm `exception:ArgumentException`, arbiter `D803`
- `U4` input `D803` `NFKC`: S `D803`, arm `exception:ArgumentException`, arbiter `D803`
- `U4` input `D803` `NFKD`: S `D803`, arm `exception:ArgumentException`, arbiter `D803`
- `U4` input `D804` `NFC`: S `D804`, arm `exception:ArgumentException`, arbiter `D804`
- `U4` input `D804` `NFD`: S `D804`, arm `exception:ArgumentException`, arbiter `D804`
- `U4` input `D804` `NFKC`: S `D804`, arm `exception:ArgumentException`, arbiter `D804`
- `U4` input `D804` `NFKD`: S `D804`, arm `exception:ArgumentException`, arbiter `D804`

## normalize, arm J:JSString.Normalize: (a), row N1, 4

correction of Broiler.JS: normalize reads code points; the runtime's normalizer refuses U+FFFE as it refuses a lone surrogate.

- `U4` input `FFFE` `NFC`: S `FFFE`, arm `exception:ArgumentException`, arbiter `FFFE`
- `U4` input `FFFE` `NFD`: S `FFFE`, arm `exception:ArgumentException`, arbiter `FFFE`
- `U4` input `FFFE` `NFKC`: S `FFFE`, arm `exception:ArgumentException`, arbiter `FFFE`
- `U4` input `FFFE` `NFKD`: S `FFFE`, arm `exception:ArgumentException`, arbiter `FFFE`

## normalize, arm V:JsRealm.NormalizeText: (a), row N6, 4918080

correction of Broiler.VM: the maximum result length is checked on the result, not the decomposition.

- `U4` input `0000` `NFC;max=0`: S `over`, arm `0000`, arbiter `over`
- `U4` input `0000` `NFD;max=0`: S `over`, arm `0000`, arbiter `over`
- `U4` input `0000` `NFKC;max=0`: S `over`, arm `0000`, arbiter `over`
- `U4` input `0000` `NFKD;max=0`: S `over`, arm `0000`, arbiter `over`
- `U4` input `0001` `NFC;max=0`: S `over`, arm `0001`, arbiter `over`
- `U4` input `0001` `NFD;max=0`: S `over`, arm `0001`, arbiter `over`
- `U4` input `0001` `NFKC;max=0`: S `over`, arm `0001`, arbiter `over`
- `U4` input `0001` `NFKD;max=0`: S `over`, arm `0001`, arbiter `over`
- `U4` input `0002` `NFC;max=0`: S `over`, arm `0002`, arbiter `over`
- `U4` input `0002` `NFD;max=0`: S `over`, arm `0002`, arbiter `over`
- `U4` input `0002` `NFKC;max=0`: S `over`, arm `0002`, arbiter `over`
- `U4` input `0002` `NFKD;max=0`: S `over`, arm `0002`, arbiter `over`
- `U4` input `0003` `NFC;max=0`: S `over`, arm `0003`, arbiter `over`
- `U4` input `0003` `NFD;max=0`: S `over`, arm `0003`, arbiter `over`
- `U4` input `0003` `NFKC;max=0`: S `over`, arm `0003`, arbiter `over`
- `U4` input `0003` `NFKD;max=0`: S `over`, arm `0003`, arbiter `over`
- `U4` input `0004` `NFC;max=0`: S `over`, arm `0004`, arbiter `over`
- `U4` input `0004` `NFD;max=0`: S `over`, arm `0004`, arbiter `over`
- `U4` input `0004` `NFKC;max=0`: S `over`, arm `0004`, arbiter `over`
- `U4` input `0004` `NFKD;max=0`: S `over`, arm `0004`, arbiter `over`

## sentence, arm J:JSIntlSegments.Compute: (a), row G1, 42646

correction of Broiler.JS: UAX #29's sentence rules.

- `U6` input `000D 000D` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`
- `U6` input `000D 0308 000D` `-`: S `0 1 3`, arm `0 3`, arbiter `0 1 3`
- `U6` input `000D 0308 000A` `-`: S `0 1 3`, arm `0 3`, arbiter `0 1 3`
- `U6` input `000D 0000` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`
- `U6` input `000D 0308 0000` `-`: S `0 1 3`, arm `0 3`, arbiter `0 1 3`
- `U6` input `000D 094D` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`
- `U6` input `000D 0308 094D` `-`: S `0 1 3`, arm `0 3`, arbiter `0 1 3`
- `U6` input `000D 0300` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`
- `U6` input `000D 0308 0300` `-`: S `0 1 3`, arm `0 3`, arbiter `0 1 3`
- `U6` input `000D 200C` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`
- `U6` input `000D 0308 200C` `-`: S `0 1 3`, arm `0 3`, arbiter `0 1 3`
- `U6` input `000D 200D` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`
- `U6` input `000D 0308 200D` `-`: S `0 1 3`, arm `0 3`, arbiter `0 1 3`
- `U6` input `000D D83C DDE6` `-`: S `0 1 3`, arm `0 3`, arbiter `0 1 3`
- `U6` input `000D 0308 D83C DDE6` `-`: S `0 1 4`, arm `0 4`, arbiter `0 1 4`
- `U6` input `000D 06DD` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`
- `U6` input `000D 0308 06DD` `-`: S `0 1 3`, arm `0 3`, arbiter `0 1 3`
- `U6` input `000D 0903` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`
- `U6` input `000D 0308 0903` `-`: S `0 1 3`, arm `0 3`, arbiter `0 1 3`
- `U6` input `000D 1100` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`

## upper, arm J:JSString.ToLocaleUpperCase(undefined): (a'), row C3, 2537

correction of Broiler.JS: Broiler.JS's default mappings come from the runtime's data and hand patches (SJL-D-6 (c)).

- `U3` input `D81B DEBB` `default`: S `D81B DEA0`, arm `D81B DEBB`, arbiter `D81B DEA0`
- `U3` input `D81B DEBC` `default`: S `D81B DEA1`, arm `D81B DEBC`, arbiter `D81B DEA1`
- `U3` input `D81B DEBD` `default`: S `D81B DEA2`, arm `D81B DEBD`, arbiter `D81B DEA2`
- `U3` input `D81B DEBE` `default`: S `D81B DEA3`, arm `D81B DEBE`, arbiter `D81B DEA3`
- `U3` input `D81B DEBF` `default`: S `D81B DEA4`, arm `D81B DEBF`, arbiter `D81B DEA4`
- `U3` input `D81B DEC0` `default`: S `D81B DEA5`, arm `D81B DEC0`, arbiter `D81B DEA5`
- `U3` input `D81B DEC1` `default`: S `D81B DEA6`, arm `D81B DEC1`, arbiter `D81B DEA6`
- `U3` input `D81B DEC2` `default`: S `D81B DEA7`, arm `D81B DEC2`, arbiter `D81B DEA7`
- `U3` input `D81B DEC3` `default`: S `D81B DEA8`, arm `D81B DEC3`, arbiter `D81B DEA8`
- `U3` input `D81B DEC4` `default`: S `D81B DEA9`, arm `D81B DEC4`, arbiter `D81B DEA9`
- `U3` input `D81B DEC5` `default`: S `D81B DEAA`, arm `D81B DEC5`, arbiter `D81B DEAA`
- `U3` input `D81B DEC6` `default`: S `D81B DEAB`, arm `D81B DEC6`, arbiter `D81B DEAB`
- `U3` input `D81B DEC7` `default`: S `D81B DEAC`, arm `D81B DEC7`, arbiter `D81B DEAC`
- `U3` input `D81B DEC8` `default`: S `D81B DEAD`, arm `D81B DEC8`, arbiter `D81B DEAD`
- `U3` input `D81B DEC9` `default`: S `D81B DEAE`, arm `D81B DEC9`, arbiter `D81B DEAE`
- `U3` input `D81B DECA` `default`: S `D81B DEAF`, arm `D81B DECA`, arbiter `D81B DEAF`
- `U3` input `D81B DECB` `default`: S `D81B DEB0`, arm `D81B DECB`, arbiter `D81B DEB0`
- `U3` input `D81B DECC` `default`: S `D81B DEB1`, arm `D81B DECC`, arbiter `D81B DEB1`
- `U3` input `D81B DECD` `default`: S `D81B DEB2`, arm `D81B DECD`, arbiter `D81B DEB2`
- `U3` input `D81B DECE` `default`: S `D81B DEB3`, arm `D81B DECE`, arbiter `D81B DEB3`

## upper, arm J:JSString.ToUpperCase and ToLocaleUpperCase(locale): (a'), row C3, 7611

correction of Broiler.JS: Broiler.JS's default mappings come from the runtime's data and hand patches (SJL-D-6 (c)).

- `U3` input `D81B DEBB` `default`: S `D81B DEA0`, arm `D81B DEBB`, arbiter `D81B DEA0`
- `U3` input `D81B DEBB` `turkic`: S `D81B DEA0`, arm `D81B DEBB`, arbiter `D81B DEA0`
- `U3` input `D81B DEBB` `lithuanian`: S `D81B DEA0`, arm `D81B DEBB`, arbiter `D81B DEA0`
- `U3` input `D81B DEBC` `default`: S `D81B DEA1`, arm `D81B DEBC`, arbiter `D81B DEA1`
- `U3` input `D81B DEBC` `turkic`: S `D81B DEA1`, arm `D81B DEBC`, arbiter `D81B DEA1`
- `U3` input `D81B DEBC` `lithuanian`: S `D81B DEA1`, arm `D81B DEBC`, arbiter `D81B DEA1`
- `U3` input `D81B DEBD` `default`: S `D81B DEA2`, arm `D81B DEBD`, arbiter `D81B DEA2`
- `U3` input `D81B DEBD` `turkic`: S `D81B DEA2`, arm `D81B DEBD`, arbiter `D81B DEA2`
- `U3` input `D81B DEBD` `lithuanian`: S `D81B DEA2`, arm `D81B DEBD`, arbiter `D81B DEA2`
- `U3` input `D81B DEBE` `default`: S `D81B DEA3`, arm `D81B DEBE`, arbiter `D81B DEA3`
- `U3` input `D81B DEBE` `turkic`: S `D81B DEA3`, arm `D81B DEBE`, arbiter `D81B DEA3`
- `U3` input `D81B DEBE` `lithuanian`: S `D81B DEA3`, arm `D81B DEBE`, arbiter `D81B DEA3`
- `U3` input `D81B DEBF` `default`: S `D81B DEA4`, arm `D81B DEBF`, arbiter `D81B DEA4`
- `U3` input `D81B DEBF` `turkic`: S `D81B DEA4`, arm `D81B DEBF`, arbiter `D81B DEA4`
- `U3` input `D81B DEBF` `lithuanian`: S `D81B DEA4`, arm `D81B DEBF`, arbiter `D81B DEA4`
- `U3` input `D81B DEC0` `default`: S `D81B DEA5`, arm `D81B DEC0`, arbiter `D81B DEA5`
- `U3` input `D81B DEC0` `turkic`: S `D81B DEA5`, arm `D81B DEC0`, arbiter `D81B DEA5`
- `U3` input `D81B DEC0` `lithuanian`: S `D81B DEA5`, arm `D81B DEC0`, arbiter `D81B DEA5`
- `U3` input `D81B DEC1` `default`: S `D81B DEA6`, arm `D81B DEC1`, arbiter `D81B DEA6`
- `U3` input `D81B DEC1` `turkic`: S `D81B DEA6`, arm `D81B DEC1`, arbiter `D81B DEA6`

## upper, arm J:JSString.ToUpperCase and ToLocaleUpperCase(locale): (a), row C5, 22

correction of Broiler.JS: Soft_Dotted from PropList.txt: the hand set lacks U+1DF1A, U+1E04C, U+1E04D and U+1E068.

- `U3` input `D837 DF1A 0307` `lithuanian`: S `D837 DF1A`, arm `D837 DF1A 0307`, arbiter `D837 DF1A`
- `U3` input `D838 DC4C 0307` `lithuanian`: S `D838 DC4C`, arm `D838 DC4C 0307`, arbiter `D838 DC4C`
- `U3` input `D838 DC4D 0307` `lithuanian`: S `D838 DC4D`, arm `D838 DC4D 0307`, arbiter `D838 DC4D`
- `U3` input `D838 DC68 0307` `lithuanian`: S `D838 DC68`, arm `D838 DC68 0307`, arbiter `D838 DC68`
- `U3` input `D800 D837 DF1A 0307` `lithuanian`: S `D800 D837 DF1A`, arm `D800 D837 DF1A 0307`, arbiter `D800 D837 DF1A`
- `U3` input `DC00 D837 DF1A 0307` `lithuanian`: S `DC00 D837 DF1A`, arm `DC00 D837 DF1A 0307`, arbiter `DC00 D837 DF1A`
- `U3` input `D837 DF1A 0307 D800` `lithuanian`: S `D837 DF1A D800`, arm `D837 DF1A 0307 D800`, arbiter `D837 DF1A D800`
- `U3` input `D837 DF1A 0307 DC00` `lithuanian`: S `D837 DF1A DC00`, arm `D837 DF1A 0307 DC00`, arbiter `D837 DF1A DC00`
- `U3` input `D800 D838 DC4C 0307` `lithuanian`: S `D800 D838 DC4C`, arm `D800 D838 DC4C 0307`, arbiter `D800 D838 DC4C`
- `U3` input `DC00 D838 DC4C 0307` `lithuanian`: S `DC00 D838 DC4C`, arm `DC00 D838 DC4C 0307`, arbiter `DC00 D838 DC4C`
- `U3` input `D838 DC4C 0307 D800` `lithuanian`: S `D838 DC4C D800`, arm `D838 DC4C 0307 D800`, arbiter `D838 DC4C D800`
- `U3` input `D838 DC4C 0307 DC00` `lithuanian`: S `D838 DC4C DC00`, arm `D838 DC4C 0307 DC00`, arbiter `D838 DC4C DC00`
- `U3` input `D800 D838 DC4D 0307` `lithuanian`: S `D800 D838 DC4D`, arm `D800 D838 DC4D 0307`, arbiter `D800 D838 DC4D`
- `U3` input `DC00 D838 DC4D 0307` `lithuanian`: S `DC00 D838 DC4D`, arm `DC00 D838 DC4D 0307`, arbiter `DC00 D838 DC4D`
- `U3` input `D838 DC4D 0307 D800` `lithuanian`: S `D838 DC4D D800`, arm `D838 DC4D 0307 D800`, arbiter `D838 DC4D D800`
- `U3` input `D838 DC4D 0307 DC00` `lithuanian`: S `D838 DC4D DC00`, arm `D838 DC4D 0307 DC00`, arbiter `D838 DC4D DC00`
- `U3` input `D800 D838 DC68 0307` `lithuanian`: S `D800 D838 DC68`, arm `D800 D838 DC68 0307`, arbiter `D800 D838 DC68`
- `U3` input `DC00 D838 DC68 0307` `lithuanian`: S `DC00 D838 DC68`, arm `DC00 D838 DC68 0307`, arbiter `DC00 D838 DC68`
- `U3` input `D838 DC68 0307 D800` `lithuanian`: S `D838 DC68 D800`, arm `D838 DC68 0307 D800`, arbiter `D838 DC68 D800`
- `U3` input `D838 DC68 0307 DC00` `lithuanian`: S `D838 DC68 DC00`, arm `D838 DC68 0307 DC00`, arbiter `D838 DC68 DC00`

## upper, arm J:JSString.ToUpperCase and ToLocaleUpperCase(locale): (a), row C6, 244

correction of Broiler.JS: After_I, Before_Dot, After_Soft_Dotted and More_Above admit no intervening character of combining class 0 or 230; IsContextTransparent reads a mark not of class 230.

- `U3` input `0069 0903 0307` `lithuanian`: S `0049 0903 0307`, arm `0049 0903`, arbiter `0049 0903 0307`
- `U3` input `006A 0903 0307` `lithuanian`: S `004A 0903 0307`, arm `004A 0903`, arbiter `004A 0903 0307`
- `U3` input `012F 0903 0307` `lithuanian`: S `012E 0903 0307`, arm `012E 0903`, arbiter `012E 0903 0307`
- `U3` input `0249 0903 0307` `lithuanian`: S `0248 0903 0307`, arm `0248 0903`, arbiter `0248 0903 0307`
- `U3` input `0268 0903 0307` `lithuanian`: S `0197 0903 0307`, arm `0197 0903`, arbiter `0197 0903 0307`
- `U3` input `029D 0903 0307` `lithuanian`: S `A7B2 0903 0307`, arm `A7B2 0903`, arbiter `A7B2 0903 0307`
- `U3` input `02B2 0903 0307` `lithuanian`: S `02B2 0903 0307`, arm `02B2 0903`, arbiter `02B2 0903 0307`
- `U3` input `03F3 0903 0307` `lithuanian`: S `037F 0903 0307`, arm `037F 0903`, arbiter `037F 0903 0307`
- `U3` input `0456 0903 0307` `lithuanian`: S `0406 0903 0307`, arm `0406 0903`, arbiter `0406 0903 0307`
- `U3` input `0458 0903 0307` `lithuanian`: S `0408 0903 0307`, arm `0408 0903`, arbiter `0408 0903 0307`
- `U3` input `1D62 0903 0307` `lithuanian`: S `1D62 0903 0307`, arm `1D62 0903`, arbiter `1D62 0903 0307`
- `U3` input `1D96 0903 0307` `lithuanian`: S `1D96 0903 0307`, arm `1D96 0903`, arbiter `1D96 0903 0307`
- `U3` input `1DA4 0903 0307` `lithuanian`: S `1DA4 0903 0307`, arm `1DA4 0903`, arbiter `1DA4 0903 0307`
- `U3` input `1DA8 0903 0307` `lithuanian`: S `1DA8 0903 0307`, arm `1DA8 0903`, arbiter `1DA8 0903 0307`
- `U3` input `1E2D 0903 0307` `lithuanian`: S `1E2C 0903 0307`, arm `1E2C 0903`, arbiter `1E2C 0903 0307`
- `U3` input `1ECB 0903 0307` `lithuanian`: S `1ECA 0903 0307`, arm `1ECA 0903`, arbiter `1ECA 0903 0307`
- `U3` input `2071 0903 0307` `lithuanian`: S `2071 0903 0307`, arm `2071 0903`, arbiter `2071 0903 0307`
- `U3` input `2148 0903 0307` `lithuanian`: S `2148 0903 0307`, arm `2148 0903`, arbiter `2148 0903 0307`
- `U3` input `2149 0903 0307` `lithuanian`: S `2149 0903 0307`, arm `2149 0903`, arbiter `2149 0903 0307`
- `U3` input `2C7C 0903 0307` `lithuanian`: S `2C7C 0903 0307`, arm `2C7C 0903`, arbiter `2C7C 0903 0307`

## upper, arm J:JSString.ToUpperCase and ToLocaleUpperCase(locale): (a), row C7, 2

correction of Broiler.JS: ECMA-402's TransformCase canonicalizes the list and takes its first element.

- `U3` input `0069` `tr,en`: S `0130`, arm `exception:RangeError`, arbiter `0130`
- `U3` input `0069` `tr,en`: S `0130`, arm `exception:RangeError`, arbiter `0130`

## upper, arm V:JsRealm.StringChangeCase and TransformCase: (a), row C12, 5184310

correction of Broiler.VM: the maximum result length is checked on the whole result.

- `U3` input `0000` `default;max=0`: S `over`, arm `0000`, arbiter `over`
- `U3` input `0000` `turkic;max=0`: S `over`, arm `0000`, arbiter `over`
- `U3` input `0000` `lithuanian;max=0`: S `over`, arm `0000`, arbiter `over`
- `U3` input `0001` `default;max=0`: S `over`, arm `0001`, arbiter `over`
- `U3` input `0001` `turkic;max=0`: S `over`, arm `0001`, arbiter `over`
- `U3` input `0001` `lithuanian;max=0`: S `over`, arm `0001`, arbiter `over`
- `U3` input `0002` `default;max=0`: S `over`, arm `0002`, arbiter `over`
- `U3` input `0002` `turkic;max=0`: S `over`, arm `0002`, arbiter `over`
- `U3` input `0002` `lithuanian;max=0`: S `over`, arm `0002`, arbiter `over`
- `U3` input `0003` `default;max=0`: S `over`, arm `0003`, arbiter `over`
- `U3` input `0003` `turkic;max=0`: S `over`, arm `0003`, arbiter `over`
- `U3` input `0003` `lithuanian;max=0`: S `over`, arm `0003`, arbiter `over`
- `U3` input `0004` `default;max=0`: S `over`, arm `0004`, arbiter `over`
- `U3` input `0004` `turkic;max=0`: S `over`, arm `0004`, arbiter `over`
- `U3` input `0004` `lithuanian;max=0`: S `over`, arm `0004`, arbiter `over`
- `U3` input `0005` `default;max=0`: S `over`, arm `0005`, arbiter `over`
- `U3` input `0005` `turkic;max=0`: S `over`, arm `0005`, arbiter `over`
- `U3` input `0005` `lithuanian;max=0`: S `over`, arm `0005`, arbiter `over`
- `U3` input `0006` `default;max=0`: S `over`, arm `0006`, arbiter `over`
- `U3` input `0006` `turkic;max=0`: S `over`, arm `0006`, arbiter `over`

## word, arm J:JSIntlSegments.Compute: (a), row G2, 33576

correction of Broiler.JS: UAX #29's word rules, without a dictionary.

- `U6` input `000D 000D` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`
- `U6` input `000D 0020` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`
- `U6` input `000A 000D` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`
- `U6` input `000A 000A` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`
- `U6` input `000A 0020` `-`: S `0 1 2`, arm `0 2`, arbiter `0 1 2`
- `U6` input `0000 0308 000D` `-`: S `0 2 3`, arm `0 1 2 3`, arbiter `0 2 3`
- `U6` input `0000 0308 000A` `-`: S `0 2 3`, arm `0 1 2 3`, arbiter `0 2 3`
- `U6` input `0000 0308 0000` `-`: S `0 2 3`, arm `0 1 2 3`, arbiter `0 2 3`
- `U6` input `0000 094D` `-`: S `0 2`, arm `0 1 2`, arbiter `0 2`
- `U6` input `0000 0308 094D` `-`: S `0 3`, arm `0 1 3`, arbiter `0 3`
- `U6` input `0000 0300` `-`: S `0 2`, arm `0 1 2`, arbiter `0 2`
- `U6` input `0000 0308 0300` `-`: S `0 3`, arm `0 1 3`, arbiter `0 3`
- `U6` input `0000 200C` `-`: S `0 2`, arm `0 1 2`, arbiter `0 2`
- `U6` input `0000 0308 200C` `-`: S `0 3`, arm `0 1 3`, arbiter `0 3`
- `U6` input `0000 200D` `-`: S `0 2`, arm `0 1 2`, arbiter `0 2`
- `U6` input `0000 0308 200D` `-`: S `0 3`, arm `0 1 3`, arbiter `0 3`
- `U6` input `0000 0308 D83C DDE6` `-`: S `0 2 4`, arm `0 1 2 4`, arbiter `0 2 4`
- `U6` input `0000 0308 06DD` `-`: S `0 2 3`, arm `0 1 2 3`, arbiter `0 2 3`
- `U6` input `0000 0903` `-`: S `0 2`, arm `0 1 2`, arbiter `0 2`
- `U6` input `0000 0308 0903` `-`: S `0 3`, arm `0 1 3`, arbiter `0 3`

## word-status, arm J:JSIntlSegments.IsWordLike: (b), row G4, 10217

implementation-defined: S answers its own word status.

- `U6` input `000D 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `000D 0308 06DD` `-`: S `NNY`, arm `NNN`, arbiter `-`
- `U6` input `000A 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `000A 0308 06DD` `-`: S `NNY`, arm `NNN`, arbiter `-`
- `U6` input `0000 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `0000 0308 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `094D 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `094D 0308 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `0300 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `0300 0308 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `200C 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `200C 0308 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `200D 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `200D 0308 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `D83C DDE6 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `D83C DDE6 0308 06DD` `-`: S `NY`, arm `NN`, arbiter `-`
- `U6` input `06DD 000D` `-`: S `YN`, arm `NN`, arbiter `-`
- `U6` input `06DD 0308 000D` `-`: S `YN`, arm `NN`, arbiter `-`
- `U6` input `06DD 000A` `-`: S `YN`, arm `NN`, arbiter `-`
- `U6` input `06DD 0308 000A` `-`: S `YN`, arm `NN`, arbiter `-`
