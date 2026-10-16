# Unicode data pin

**Owner:** the Broiler.Unicode maintainer. **Reviewer:** none.

The Unicode Character Database files, the auxiliary break data and its tests, the two emoji sequence
files, the Unicode licence text and ECMA-262's three property tables that this repository's algorithm
tables are generated from, archived here unmodified. [`unicode.pin`](unicode.pin) records each file's
length and SHA-256.

## Where they came from

**This archive is a copy of Broiler.VM's**, `src/tests/unicode/pins/` in that repository, with the
three ECMAScript tables of its `src/Broiler.VM.Profile.JavaScript/docs/specification/`, taken on
2026-10-16 at work package SJL-7.2 of the shared JavaScript libraries programme. The owner condition
this repository adopted that day (its [roadmap](../../../docs/roadmap.md), "The owner conditions")
says the pin "starts as a copy of Broiler.VM's `src/tests/unicode/pins/unicode.pin`: the same files,
by the same hashes". Every file was copied byte for byte from Broiler.VM at `7b44dc7`, and nothing was
retrieved again. Broiler.VM's pin and its README record how each file was retrieved from unicode.org
and tc39/ecma262, twice and byte-identical, between 2026-09-22 and 2026-10-04.

| | |
|---|---|
| Version | **Unicode 17.0.0** |
| UCD files | under [`ucd-17.0.0/`](ucd-17.0.0) at their UCD paths, `auxiliary/`, `emoji/` and `extracted/` kept |
| Emoji sequence files | under [`emoji-17.0.0/`](emoji-17.0.0), from the release's `emoji/` directory |
| ECMAScript tables | under [`ecma262/`](ecma262), from tc39/ecma262 at `0248456c`; the pin names them by path from the repository root |
| Test-only | `NormalizationTest.txt`, `GraphemeBreakTest.txt`, `WordBreakTest.txt` and `SentenceBreakTest.txt`: no table is generated from them |

## How the pin is enforced

The generator, [`src/Unicode.Generation`](../../../src/Unicode.Generation), reads the pin and hands
the tables nothing it has not verified: every file's length and SHA-256, and every UCD file's version
header. The test project regenerates every table byte for byte and holds this pin equal to
Broiler.VM's, line by line, by file name, length and hash (the programme's rule SL7; bundle
[`sjl-7-001`](../../../docs/evidence/sjl-7-001/decision-rule.md), part B). **The pin moves only by a
dated decision that moves Broiler.VM's in the same change**, never in one repository alone.

**Every archived file is binary in [`.gitattributes`](../../../.gitattributes)**, so no checkout filter
rewrites a line ending and moves a hash on one platform.

## Licence

The Unicode files are © Unicode, Inc., under the Unicode License v3 (SPDX `Unicode-3.0`), whose text
is retained as [`unicode-LICENSE.txt`](unicode-LICENSE.txt) and reproduced in
[`THIRD_PARTY_NOTICES.md`](../../../THIRD_PARTY_NOTICES.md). The ECMAScript tables are © Ecma
International, from the ECMA-262 specification's source. **The files are unmodified**, which their
hashes attest.
