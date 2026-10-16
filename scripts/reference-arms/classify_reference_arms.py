#!/usr/bin/env python3
"""Classify the differences of bundle sjl-7-001's reference-arm comparison against the arbiter.

    python3 scripts/reference-arms/classify_reference_arms.py --run <comparison output directory> [--examples 20]

The comparison (this repository's test project) classifies nothing: it lists, in differences.tsv.gz, every
input on which an arm's answer differs from S's. This script is the owner's classification. It is written
before the judged run, from docs/evidence/sjl-7-001/reference-arms.md's admitted classes and the
correspondence's rows, and is applied mechanically, so that every class can be checked against the arbiter's
value line by line. The arbiter is unicoderef.py, written from the standards' text and reading only the
pinned files; the agreement of the arms decides nothing.

**Input.** `<run>/differences.tsv.gz`, UTF-8, one difference per line, optionally after a header line that
starts with `operation\\t`:

    <operation>\\t<class>\\t<input>\\t<parameter>\\t<arm>\\t<S result>\\t<arm result>

in the notation README.md section "differences.tsv" gives (unicoderef.answer computes the arbiter's answer
in the same notation). A `charge` line compares S's total charge with arm V's.

**The classes**, decided for each line in this order:

1. an arm whose name contains "control" is a negative control: reported, never classified (`unclassified`);
2. a `charge` line is class (c): Broiler.VM's units are the charge's specification (JSD-0074, decision 4);
3. a `word-status` line is class (b), row G4: the status is implementation-defined, and S answers its own;
4. the arbiter computes the answer; where it cannot (an operation it does not know), class (c);
5. S's answer is not the arbiter's: class (c), a defect of S;
6. S's answer and the arm's are both the arbiter's, yet listed as different: class (c), a defect of the
   comparison's notation;
7. S's answer is the arbiter's and the arm's is not: the arm's row predicates (ROWS_J, ROWS_V below) are tried
   in order, and the first that holds gives the class and the row: (a) a correction, (a') a correction by data
   decision SJL-D-6 (c). Where none holds the difference is class (c) and flagged **unnamed**: the
   correspondence names no such correction, which falsifies JSD-0074 whatever the arbiter says (the rule's
   "How it is read"). Against arm V only C12 and N6 can hold; every other difference with V is class (c).

**The arbiter's recorded answers.** With `--reference <unicoderef.py's --out>`, the classifier reads, for the
inputs the differences name, the answers the arbiter recorded for the population (names-ref.tsv, casing-ref.tsv,
normalization-ref.tsv, string-properties-ref.tsv, segmentation-ref.tsv), refuses a reference written under another
pin or arbiter version, and checks each recorded answer against the arbiter's direct one: any disagreement is
printed and nothing is written (exit 3). The code-point operations and the repeat inputs, recorded as sets, maps
and digests, are answered directly.

**Outputs**, in the run directory: classification.tsv.gz (every difference with the arbiter's answer, the
verdict, the row, the unnamed flag and the reason; a field longer than 256 characters is shown by its length
and SHA-256), classification.json (counts per verdict, per row, per operation and arm, and the SHA-256 and line
count of both listings' uncompressed text) and report.md (the counts and the first --examples lines of each
group). The exit status is 0 when no difference is class (c), 1 otherwise, 2 when a pinned file is refused and 3
when a recorded answer is not the arbiter's.
"""

from __future__ import annotations

import argparse
import gzip
import hashlib
import json
import sys
from collections import Counter, defaultdict
from functools import lru_cache
from pathlib import Path

HERE = Path(__file__).resolve().parent
if str(HERE) not in sys.path:
    sys.path.insert(0, str(HERE))

import unicoderef as U  # noqa: E402

CLASSIFIER_VERSION = "sjl-7-001-classifier@1"
SHOWN = 256
HEADER = ["operation", "class", "input", "parameter", "arm", "S", "result"]
CASING = ("upper", "lower")
NORMALIZATION = ("normalize", "NFC", "NFD", "NFKC", "NFKD")
IDENTIFIER_SETS = ("IdentifierStartChar", "IdentifierPartChar", "ID_Start-Pattern_Syntax", "ID_Continue-Pattern_Syntax",
                   "ID_Start", "ID_Continue")
REGEXP_IDENTIFIER_SETS = ("RegExpIdentifierStart", "RegExpIdentifierPart")
WHITE_SPACE_SETS = ("WhiteSpace", "White_Space")


# --------------------------------------------------------------------------------------------------
# Reading a difference.

class Difference:
    def __init__(self, fields: list[str], ucd: U.UCD) -> None:
        self.fields = fields
        operation, self.cls, self.input, self.parameter, self.arm, self.s, self.result = fields
        self.operation = U.OPERATION_ALIASES.get(operation, operation)
        self.ucd = ucd

    @property
    def arm_letter(self) -> str:
        return self.arm.strip()[:1].upper()

    def units(self) -> list[int] | None:
        units, repeat = U.parse_text_input(self.input)
        return units

    def language(self) -> str:
        return U.parse_language(self.parameter)

    def form(self) -> str:
        if self.operation != "normalize":
            return self.operation
        return self.parameter.split(";", 1)[0].split("@", 1)[0].strip().upper()


def is_refusal(value: str) -> bool:
    return value == U.OVER or value.startswith(("exception", "error", "refused", "throw"))


def text_of(value: str) -> list[int] | None:
    """A listed text result as code units, or None when it is a refusal or a digest."""
    if is_refusal(value) or value.startswith("len:"):
        return None
    try:
        return U.parse_units(value)
    except ValueError:
        return None


# --------------------------------------------------------------------------------------------------
# What the predicates need: Final_Sigma under arm J's approximation, the casing and normalization that
# older data would give.

def final_sigma_with(ucd: U.UCD, cps, i: int, cased, ignorable) -> bool:
    j = i - 1
    while j >= 0 and ignorable(cps[j]):
        j -= 1
    if j < 0 or not cased(cps[j]):
        return False
    j = i + 1
    while j < len(cps) and ignorable(cps[j]):
        j += 1
    return not (j < len(cps) and cased(cps[j]))


def sigma_only(ref: list[int], arm: list[int]) -> bool:
    """The two texts differ only where one has U+03C3 and the other U+03C2."""
    return len(ref) == len(arm) and ref != arm and all(a == b or {a, b} == {0x3C3, 0x3C2} for a, b in zip(ref, arm))


def sigma_explained_by_cased(ucd: U.UCD, cps) -> bool:
    """Whether some Σ's Final_Sigma decision changes when Cased is read as General_Category Lu, Ll or Lt,
    arm J's approximation (correspondence row C1)."""
    def approximate(c: int) -> bool:
        return ucd.general_category(c) in ("Lu", "Ll", "Lt")

    def real_cased(c: int) -> bool:
        return bool(ucd.cased[c])

    def real_ignorable(c: int) -> bool:
        return bool(ucd.case_ignorable[c])

    return any(final_sigma_with(ucd, cps, i, real_cased, real_ignorable) != final_sigma_with(ucd, cps, i, approximate, real_ignorable)
               for i, c in enumerate(cps) if c == 0x3A3)


def explained_by_unmapped(ucd: U.UCD, cps, upper: bool, language: str, arm: list[int]) -> bool:
    """Whether the arm's text is the reference's with some code points left as they are: data that does not
    know their mappings (row C3's host data, row C4's runtime path)."""
    options = []
    for i, c in enumerate(cps):
        mapped = tuple(U.code_units(ucd.map_at(cps, i, upper, language)))
        kept = tuple(U.code_units([c]))
        options.append({mapped, kept})

    @lru_cache(maxsize=None)
    def matches(i: int, at: int) -> bool:
        if i == len(options):
            return at == len(arm)
        return any(tuple(arm[at:at + len(o)]) == o and matches(i + 1, at + len(o)) for o in options[i])

    return matches(0, 0)


def normalize_without(ucd: U.UCD, units, form: str, unknown: set[int]) -> list[int]:
    """The normal form when the code points of `unknown` are unassigned: no decomposition, class 0, and in
    no composition (the reading of older data, row N2)."""
    compat = form in ("NFKC", "NFKD")
    table = ucd.nfkd_map if compat else ucd.nfd_map
    out: list[int] = []
    for c in U.code_points(units):
        d = None if c in unknown else table.get(c)
        out.extend(d if d is not None and not (set(d) & unknown) else (c,))

    def klass(c: int) -> int:
        return 0 if c in unknown else ucd.ccc[c]

    i = 0
    while i < len(out):
        if klass(out[i]) == 0:
            i += 1
            continue
        j = i + 1
        while j < len(out) and klass(out[j]) != 0:
            j += 1
        out[i:j] = sorted(out[i:j], key=klass)
        i = j
    if form in ("NFD", "NFKD") or not out:
        return U.code_units(out)
    result = [out[0]]
    starter = 0
    last = klass(out[0]) or 256
    for c in out[1:]:
        k = klass(c)
        composite = None
        if (last < k or last == 0) and c not in unknown and result[starter] not in unknown:
            composite = ucd.compose_pair(result[starter], c)
            if composite in unknown:
                composite = None
        if composite is not None:
            result[starter] = composite
            continue
        if k == 0:
            starter = len(result)
        last = k
        result.append(c)
    return U.code_units(result)


_FIRSTS: dict[int, frozenset[int]] = {}


def _first_elements(ucd: U.UCD) -> frozenset[int]:
    if id(ucd) not in _FIRSTS:
        _FIRSTS[id(ucd)] = frozenset(a for a, _ in ucd.primary) | frozenset(range(U.L_BASE, U.L_BASE + U.L_COUNT))
    return _FIRSTS[id(ucd)]


def explained_by_older_data(ucd: U.UCD, units, form: str, arm: list[int]) -> bool:
    """Whether the arm's normal form is the one data without some of the input's code points gives: all of
    those that normalization reads, or any one of them."""
    firsts = _first_elements(ucd)
    involved = [c for c in dict.fromkeys(U.code_points(units))
                if c in ucd.nfkd_map or ucd.ccc[c] or c in ucd.second_elements or c in firsts]
    candidates = [set(involved)] + [{c} for c in involved]
    return any(normalize_without(ucd, units, form, unknown) == arm for unknown in candidates if unknown)


# --------------------------------------------------------------------------------------------------
# The rows. Each predicate sees a difference whose S answer is the arbiter's and whose arm answer is not.

def _lookup(d: Difference) -> bool:
    return d.operation == "lookup"


def _cp(d: Difference) -> int:
    return U.parse_code_point(d.input)


def p3(d: Difference, ref: str) -> bool:
    return _lookup(d) and d.s == U.REFUSED and d.result.lower().startswith(("dotnet", ".net", "fallthrough", "verbatim"))


def p1(d: Difference, ref: str) -> bool:
    return _lookup(d) and d.s == U.REFUSED and not is_refusal(d.result)


def p2(d: Difference, ref: str) -> bool:
    return _lookup(d) and d.s.endswith("=Katakana_Or_Hiragana") and is_refusal(d.result)


def p4(d: Difference, ref: str) -> bool:
    return _lookup(d) and d.parameter.strip() in ("v", "unicodeSets", "v-flag") and d.s.startswith("General_Category=") \
        and is_refusal(d.result)


def i1(d: Difference, ref: str) -> bool:
    return d.operation == "lexical" and d.parameter in IDENTIFIER_SETS and d.ucd.has("Pattern_Syntax", _cp(d))


def i3(d: Difference, ref: str) -> bool:
    return d.operation == "lexical" and d.parameter in REGEXP_IDENTIFIER_SETS


def i2(d: Difference, ref: str) -> bool:
    return d.operation == "lexical" and d.parameter in IDENTIFIER_SETS and not d.ucd.has("Pattern_Syntax", _cp(d))


def w1(d: Difference, ref: str) -> bool:
    if d.operation != "lexical" or d.parameter not in WHITE_SPACE_SETS:
        return False
    cp = _cp(d)
    terminator = U.ranges_contain(d.ucd.sets()["lexical:LineTerminator"], cp)
    return d.result == "true" and ref == "false" and not terminator


def c7(d: Difference, ref: str) -> bool:
    return d.operation in CASING and is_refusal(d.result) and "," in d.parameter


def c8(d: Difference, ref: str) -> bool:
    return d.operation in CASING and "@" in d.parameter and d.language() == "default"


def c5(d: Difference, ref: str) -> bool:
    arm, expected = text_of(d.result), text_of(ref)
    return d.operation == "upper" and d.language() == "lithuanian" and arm is not None and expected is not None \
        and arm.count(0x307) > expected.count(0x307)


def c6(d: Difference, ref: str) -> bool:
    arm, expected, units = text_of(d.result), text_of(ref), d.units()
    return d.operation == "lower" and d.language() == "lithuanian" and arm is not None and expected is not None \
        and units is not None and any(u in (0x49, 0x4A, 0x12E) for u in units) and arm.count(0x307) != expected.count(0x307)


def c4(d: Difference, ref: str) -> bool:
    arm, units = text_of(d.result), d.units()
    if d.operation != "lower" or d.language() != "turkic" or arm is None or units is None:
        return False
    if any(u in (0x49, 0x69, 0x130, 0x131, 0x307) for u in units):
        return False
    return explained_by_unmapped(d.ucd, U.code_points(units), False, "turkic", arm)


def c1(d: Difference, ref: str) -> bool:
    arm, expected, units = text_of(d.result), text_of(ref), d.units()
    return d.operation == "lower" and arm is not None and expected is not None and units is not None \
        and sigma_only(expected, arm) and sigma_explained_by_cased(d.ucd, U.code_points(units))


def c3(d: Difference, ref: str) -> bool:
    arm, units = text_of(d.result), d.units()
    if d.operation not in CASING or arm is None or units is None:
        return False
    return explained_by_unmapped(d.ucd, U.code_points(units), d.operation == "upper", d.language(), arm)


def n1(d: Difference, ref: str) -> bool:
    units = d.units()
    return d.operation in NORMALIZATION and is_refusal(d.result) and units is not None \
        and any(0xD800 <= c <= 0xDFFF for c in U.code_points(units))


def n3(d: Difference, ref: str) -> bool:
    return d.operation in NORMALIZATION and "invariant" in d.parameter.lower() and text_of(d.result) == d.units()


def n2(d: Difference, ref: str) -> bool:
    arm, units = text_of(d.result), d.units()
    return d.operation in NORMALIZATION and arm is not None and units is not None \
        and explained_by_older_data(d.ucd, units, d.form(), arm)


def g1(d: Difference, ref: str) -> bool:
    units = d.units()
    return d.operation == "sentence" and units is not None and d.result.strip() == f"0 {len(units)}"


def g2(d: Difference, ref: str) -> bool:
    return d.operation == "word"


def g3(d: Difference, ref: str) -> bool:
    return d.operation == "grapheme"


def limit(family: tuple[str, ...]):
    def predicate(d: Difference, ref: str) -> bool:
        bounded = U.parse_max(d.parameter) is not None or d.cls == "U7"
        return d.operation in family and bounded and (is_refusal(d.result) or is_refusal(d.s) or ref == U.OVER)
    return predicate


# (row, class, predicate, what the correspondence says), tried in order. Written before the judged run.
ROWS_J = [
    ("P3", "(a)", p3, "a name Broiler.Regex refuses is left verbatim for .NET's \\p; the tables admit only their names"),
    ("P1", "(a)", p1, "UnicodeMatchProperty and UnicodeMatchPropertyValue admit the tables' names and aliases exactly"),
    ("P2", "(a)", p2, "Hrkt is a value of Script in PropertyValueAliases.txt and in the specification's table"),
    ("P4", "(a)", p4, "General_Category resolves inside a v-mode set operation"),
    ("I1", "(a)", i1, "IdentifierStartChar is ID_Start, which subtracts Pattern_Syntax (UAX #31)"),
    ("I3", "(a)", i3, "group names copy the identifier tests without Pattern_Syntax and without the Unicode 17 tables"),
    ("I2", "(a')", i2, "Broiler.JS's identifier classes follow the runtime's data plus hand lists (SJL-D-6 (c))"),
    ("W1", "(a)", w1, "U+0085 is Cc and in neither WhiteSpace nor LineTerminator"),
    ("C7", "(a)", c7, "ECMA-402's TransformCase canonicalizes the list and takes its first element"),
    ("C8", "(a)", c8, "casing no longer depends on the host culture"),
    ("C5", "(a)", c5, "Soft_Dotted from PropList.txt: the hand set lacks U+1DF1A, U+1E04C, U+1E04D and U+1E068"),
    ("C6", "(a)", c6, "More_Above: no intervening character of combining class 0 or 230"),
    ("C4", "(a)", c4, "the Turkic lower-case path skips the class's table, so the runtime's older data shows"),
    ("C1", "(a)", c1, "Final_Sigma reads Cased from DerivedCoreProperties.txt (Other_Lowercase, Other_Uppercase)"),
    ("C3", "(a')", c3, "Broiler.JS's default mappings come from the runtime's data and hand patches (SJL-D-6 (c))"),
    ("N1", "(a)", n1, "a lone surrogate is a code point of class 0 without a decomposition"),
    ("N3", "(a')", n3, "under invariant globalization the runtime's Normalize does nothing (SJL-D-6 (c))"),
    ("N2", "(a')", n2, "the host's ICU carries older Unicode data (SJL-D-6 (c))"),
    ("G1", "(a)", g1, "UAX #29's sentence rules"),
    ("G2", "(a)", g2, "UAX #29's word rules, without a dictionary"),
    ("G3", "(a')", g3, "Broiler.JS's graphemes follow the runtime's data (SJL-D-6 (c))"),
]
ROWS_V = [
    ("C12", "(a)", limit(CASING), "the maximum result length is checked on the whole result"),
    ("N6", "(a)", limit(NORMALIZATION), "the maximum result length is checked on the result, not the decomposition"),
]
# A row the correspondence names whose difference is not admitted for the arm: listed, class (c).
NAMED_NOT_ADMITTED_J = [
    ("C2", lambda d, ref: d.operation == "lower" and (lambda a, e: a is not None and e is not None and sigma_only(e, a))(text_of(d.result), text_of(ref)),
     "Broiler.JS's Case_Ignorable is an approximation (named as an agreement, not as a correction)"),
]


def classify(d: Difference) -> tuple[str, str, str, str, str]:
    """(verdict, row, unnamed, reason, arbiter's answer) of one difference."""
    if "control" in d.arm.lower():
        return "unclassified", "-", "-", "a negative control: reported, not classified", "-"
    if d.operation == "charge":
        return "(c)", "-", "-", "a charge of S differs from arm V's: always class (c)", "-"
    if d.operation == "word-status":
        return "(b)", "G4", "-", "implementation-defined: S answers its own word status", "-"
    try:
        ref = U.answer(d.ucd, d.operation, d.input, d.parameter)
    except Exception as error:  # noqa: BLE001 - the arbiter failing to judge is itself the reason
        return "(c)", "-", "-", f"the arbiter cannot judge the input: {type(error).__name__}: {error}", "-"
    if ref is None:
        return "(c)", "-", "-", "the arbiter has no answer for the operation", "-"
    if d.s != ref:
        return "(c)", "-", "-", "S is not the arbiter's: a defect of S", ref
    if d.result == ref:
        return "(c)", "-", "-", "S and the arm both give the arbiter's answer yet are listed as different", ref
    letter = d.arm_letter
    rows = ROWS_J if letter == "J" else ROWS_V if letter == "V" else []
    for row, verdict, predicate, what in rows:
        if predicate(d, ref):
            engine = "Broiler.JS" if letter == "J" else "Broiler.VM"
            return verdict, row, "no", f"correction of {engine}: {what}", ref
    if letter == "J":
        for row, predicate, what in NAMED_NOT_ADMITTED_J:
            if predicate(d, ref):
                return "(c)", row, "no", f"S is the arbiter's; row {row} names this but admits no correction: {what}", ref
    if letter == "V":
        return "(c)", "-", "yes", "S is the arbiter's, but a difference with arm V outside C12's and N6's limits is class (c)", ref
    return "(c)", "-", "yes", "S is the arbiter's, but no row of the correspondence names this correction", ref


# --------------------------------------------------------------------------------------------------
# The arbiter's recorded answers (unicoderef.py --out), cross-checked against its direct answers.

SEGMENTATION_COLUMNS = {"grapheme": 1, "word": 2, "sentence": 3, "word-status": 4}


class Recorded:
    """The answers unicoderef.py recorded for the population, read for the inputs the differences name only.
    A reference written under another pin or another version of the arbiter is refused."""

    def __init__(self, directory: Path, ucd: U.UCD, differences: list[Difference]) -> None:
        manifest_path = directory / "reference-manifest.json"
        manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
        if manifest["pin"]["sha256"] != ucd.pins.sha256 or manifest["version"] != U.REFERENCE_VERSION:
            raise SystemExit(f"refused: {directory} was written under pin {manifest['pin']['sha256']} by {manifest['version']}; "
                             f"this is pin {ucd.pins.sha256} and {U.REFERENCE_VERSION}")
        self.manifest_sha256 = hashlib.sha256(manifest_path.read_bytes()).hexdigest()
        needed: dict[str, set[str]] = defaultdict(set)
        for d in differences:
            place = self.place(d)
            if place is not None:
                needed[place[0]].add(place[1])
        self.lines: dict[str, dict[str, list[str]]] = {}
        for name, keys in needed.items():
            found: dict[str, list[str]] = {}
            path = directory / name
            if path.exists():
                width = 3 if name == "names-ref.tsv" else 1
                with open(path, encoding="utf-8", newline="\n") as stream:
                    for line in stream:
                        fields = line.rstrip("\n").split("\t")
                        key = "\t".join(fields[:width])
                        if key in keys:
                            found[key] = fields
            self.lines[name] = found

    @staticmethod
    def place(d: Difference) -> tuple[str, str, int] | None:
        """(file, key, column) of a difference's recorded answer, or None where none is recorded (the code-point
        operations, whose answers are sets and maps, and the repeat inputs)."""
        if d.operation == "lookup":
            kind, prop, value = U.parse_name_input(d.input)
            return "names-ref.tsv", "\t".join([kind, prop, value]), 4 if d.parameter.strip() in ("v", "unicodeSets", "v-flag") else 3
        if d.operation not in CASING + NORMALIZATION + ("string-property",) + tuple(SEGMENTATION_COLUMNS):
            return None
        units, repeat = U.parse_text_input(d.input)
        if repeat is not None:
            return None
        key = U.format_units(units)
        if d.operation in CASING:
            operation = (d.operation, d.language())
            return "casing-ref.tsv", key, 1 + U.CASING_OPERATIONS.index(operation)
        if d.operation in NORMALIZATION:
            return ("normalization-ref.tsv", key, 1 + U.FORMS.index(d.form())) if d.form() in U.FORMS else None
        if d.operation == "string-property":
            return "string-properties-ref.tsv", key, 1
        return "segmentation-ref.tsv", key, SEGMENTATION_COLUMNS[d.operation]

    def answer(self, d: Difference) -> str | None:
        place = self.place(d)
        if place is None:
            return None
        fields = self.lines.get(place[0], {}).get(place[1])
        if fields is None:
            return None
        value = fields[place[2]]
        if d.operation == "string-property":
            return "true" if d.parameter in value.split(",") else "false"
        maximum = U.parse_max(d.parameter)
        if maximum is not None and (d.operation in CASING or d.operation in NORMALIZATION):
            return U.text_result(U.parse_units(value), maximum)
        return value


# --------------------------------------------------------------------------------------------------

def shown(text: str) -> str:
    if len(text) <= SHOWN:
        return text
    return f"len:{len(text)}:sha256:{hashlib.sha256(text.encode('utf-8')).hexdigest()}"


def read_differences(path: Path) -> tuple[list[str], list[list[str]]]:
    with gzip.open(path, "rt", encoding="utf-8", newline="\n") as stream:
        lines = [line for line in stream.read().split("\n") if line]
    rows = []
    for line in lines:
        if line.startswith("operation\t"):
            continue
        fields = line.split("\t")
        if len(fields) != 7:
            raise SystemExit(f"a difference of {len(fields)} fields, not 7: {line[:200]!r}")
        rows.append(fields)
    return lines, rows


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--run", required=True, type=Path, help="the comparison's output directory (differences.tsv.gz)")
    parser.add_argument("--examples", type=int, default=20)
    parser.add_argument("--pins", type=Path, default=U.PIN_FILE)
    parser.add_argument("--reference", type=Path,
                        help="unicoderef.py's output directory: its recorded answers are checked against the direct ones")
    args = parser.parse_args(argv)
    try:
        ucd = U.UCD(U.Pins(args.pins))
    except U.PinError as error:
        print(f"refused: {error}", file=sys.stderr)
        return 2
    lines, rows = read_differences(args.run / "differences.tsv.gz")
    recorded = Recorded(args.reference, ucd, [Difference(f, ucd) for f in rows]) if args.reference else None
    checked, disagreeing = 0, []

    out_lines = ["\t".join(HEADER + ["arbiter", "verdict", "row", "unnamed", "reason"])]
    counts: Counter = Counter()
    by_row: Counter = Counter()
    by_operation: Counter = Counter()
    unnamed = 0
    examples: dict = defaultdict(list)
    for fields in rows:
        d = Difference(fields, ucd)
        verdict, row, flag, reason, ref = classify(d)
        if recorded is not None:
            written = recorded.answer(d)
            if written is not None:
                checked += 1
                direct = U.answer(ucd, d.operation, d.input, d.parameter)
                if written != direct:
                    disagreeing.append((fields, written, direct))
        out_lines.append("\t".join([shown(f) for f in fields] + [shown(ref), verdict, row, flag, reason]))
        key = (d.operation, d.arm, verdict, row, reason)
        counts[key] += 1
        by_row[(verdict, row, d.arm)] += 1
        by_operation[(d.operation, d.arm, verdict)] += 1
        unnamed += flag == "yes"
        if len(examples[key]) < args.examples:
            examples[key].append((fields, ref))

    if disagreeing:
        for fields, written, direct in disagreeing[:20]:
            print(f"the recorded answer {written!r} is not the arbiter's {direct!r}: {fields}", file=sys.stderr)
        print(f"refused: {len(disagreeing)} recorded answers are not the arbiter's; nothing is written", file=sys.stderr)
        return 3
    text = "".join(line + "\n" for line in out_lines)
    with open(args.run / "classification.tsv.gz", "wb") as raw:
        with gzip.GzipFile(filename="", mode="wb", fileobj=raw, mtime=0, compresslevel=9) as target:
            target.write(text.encode("utf-8"))
    differences_text = "".join(line + "\n" for line in lines)
    by_verdict: Counter = Counter()
    for (_, _, verdict, _, _), n in counts.items():
        by_verdict[verdict] += n
    summary = {
        "classifier": "scripts/reference-arms/classify_reference_arms.py", "version": CLASSIFIER_VERSION,
        "reference": U.REFERENCE_VERSION, "pin_sha256": ucd.pins.sha256,
        "recorded answers": None if recorded is None else {"directory": str(args.reference), "manifest_sha256": recorded.manifest_sha256,
                                                           "differences cross-checked": checked},
        "differences": len(rows),
        "verdicts": dict(sorted(by_verdict.items())),
        "unnamed by the correspondence": unnamed,
        "rows": [{"verdict": v, "row": r, "arm": a, "count": n} for (v, r, a), n in sorted(by_row.items())],
        "operations": [{"operation": o, "arm": a, "verdict": v, "count": n} for (o, a, v), n in sorted(by_operation.items())],
        "groups": [{"operation": o, "arm": a, "verdict": v, "row": r, "reason": why, "count": n}
                   for (o, a, v, r, why), n in sorted(counts.items())],
        "differences.tsv": {"lines": len(lines), "sha256": hashlib.sha256(differences_text.encode("utf-8")).hexdigest()},
        "classification.tsv": {"lines": len(out_lines), "sha256": hashlib.sha256(text.encode("utf-8")).hexdigest()},
    }
    (args.run / "classification.json").write_text(json.dumps(summary, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")

    report = ["# Bundle sjl-7-001's reference-arm comparison, classified", "",
              f"{len(rows)} differences: " + (", ".join(f"{n} {v}" for v, n in sorted(by_verdict.items())) or "none") + ".",
              f"{unnamed} of them unnamed by the correspondence.", "",
              "| Verdict | Row | Arm | Count |", "|---|---|---|---|"]
    report += [f"| {v} | {r} | {a} | {n} |" for (v, r, a), n in sorted(by_row.items())]
    report.append("")
    for (operation, arm, verdict, row, reason), n in sorted(counts.items()):
        report += [f"## {operation}, arm {arm}: {verdict}, row {row}, {n}", "", f"{reason}.", ""]
        for fields, ref in examples[(operation, arm, verdict, row, reason)]:
            report.append(f"- `{fields[1]}` input `{shown(fields[2])}` `{fields[3]}`: S `{shown(fields[5])}`, "
                          f"arm `{shown(fields[6])}`, arbiter `{shown(ref)}`")
        report.append("")
    (args.run / "report.md").write_text("\n".join(report), encoding="utf-8")
    print(json.dumps(summary["verdicts"]))
    return 0 if "(c)" not in by_verdict else 1


if __name__ == "__main__":
    sys.exit(main())
