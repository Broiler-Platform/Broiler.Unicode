#!/usr/bin/env python3
"""Generate the input population of bundle sjl-7-001's reference-arm comparison.

    python3 scripts/reference-arms/generate_population.py --out <directory> [--reduced]

The population is the one docs/evidence/sjl-7-001/reference-arms.md declares, class by class (U1 to U7), with
the seeds it names. The comparison in this repository's test project reads what this writes; nothing here is
an expected value, and the comparison judges no input by it. The data that selects inputs (which code points
are Cased, which have a decomposition, the sequences of the emoji files, the lines of the test files) is read
through unicoderef.py from the pinned files, each checked against unicode.pin; a file whose bytes are not the
pin's is refused and nothing is written.

**Files** (README.md gives every format): names.tsv (U2), casing.tsv (U3), normalization.tsv (U4),
string-properties.tsv (U5), segmentation.tsv (U6), limits.tsv (U7), and manifest.json with each file's line
count, SHA-256 and counts per tag. U1, every code point from U+0000 to U+10FFFF with the surrogates, has no
file: the manifest records it, and the harness enumerates it. U5's "every single code point of U1" is the
same enumeration, recorded in the manifest and not written. A text is its UTF-16 code units in 4-digit
upper-case hexadecimal separated by single spaces, followed by a tab and the tag of the sub-class it comes
from; lines end with LF, and the files are UTF-8.

**A draw** is one output of SplitMix64 (splitmix64.py, tested against the published reference outputs) from
the class's seed. A choice among n things takes the draw modulo n; an integer from [low, high] is low plus the
draw modulo the range's size; a second value from the same draw is the draw shifted right by 16 bits, modulo
its own range, and a third the draw shifted right by 32 bits, modulo its own range. Each drawn class below
states its draws in that form, in the order they are consumed.

The output is deterministic: the same arguments and the same pinned files give the same bytes.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import re
import sys
from collections import Counter
from pathlib import Path

HERE = Path(__file__).resolve().parent
if str(HERE) not in sys.path:
    sys.path.insert(0, str(HERE))

import unicoderef as U  # noqa: E402
from splitmix64 import between, choice, second, splitmix64, third  # noqa: E402

RULE = "docs/evidence/sjl-7-001/reference-arms.md"
GENERATOR = "scripts/reference-arms/generate_population.py"
GENERATOR_VERSION = "sjl-7-001-population@1"
CELLS = HERE / "cells.json"

SEED_U2 = 0x534A4C372D303031  # "SJL7-001"
SEED_U3 = 0x534A4C372D303032
SEED_U4 = 0x534A4C372D303033
SEED_U6 = 0x534A4C372D303034

U2_DRAWS = 1 << 14
U3_DRAWS = 1 << 16
U4_DRAWS = 1 << 16
U6_DRAWS = 1 << 16

# U3: the rule's eleven contexts, c standing for the code point substituted.
CONTEXTS = ["AcΣ", "AΣc", "cΣ", "Σc", "c\u0307", "c\u0301\u0307", "c\u0903\u0307", "Ic", "Jc\u0300", "c\u0345", "cA"]
CONTEXT_EXTRAS = [0x0049, 0x004A, 0x012E, 0x03A3, 0x0130, 0x0131]  # I, J, Į, Σ, İ, ı
LONE_HIGH, LONE_LOW = 0xD800, 0xDC00
U3_SPECIALS = [0x0049, 0x004A, 0x012E, 0x03A3, 0x0130, 0x0131, 0x0069, 0x006A, 0x0307, 0x0300, 0x0301, 0x0345, 0x0903, 0x0041]
U3_KINDS = ["context"] * 4 + ["ascii"] * 4 + ["mark0", "mark220", "mark230", "mark230", "mark-other"] + ["special"] * 3
U4_KINDS = ["starter"] * 3 + ["mark"] * 3 + ["L", "V", "T", "LV", "LVT"] + ["exclusion", "singleton", "non-starter",
                                                                          "supplementary", "surrogate"]
U5_ADDED = [0x200D, 0xFE0F]
U6_EXTRAS = ["space", "punctuation", "digit", "ideograph", "katakana", "regional-indicator"]
BREAK_TESTS = ["GraphemeBreakTest.txt", "WordBreakTest.txt", "SentenceBreakTest.txt"]

REDUCED_DRAW_DIVISOR = 256


# --------------------------------------------------------------------------------------------------
# Writing.

def text_line(units, tag: str) -> str:
    return f"{U.format_units(units)}\t{tag}"


def tag_family(tag: str) -> str:
    """The key a tag is counted under: its line numbers removed (test-file:WordBreakTest.txt:123 counts
    under test-file:WordBreakTest.txt)."""
    return ":".join(part for part in tag.split(":") if not part.isdigit())


class Writer:
    def __init__(self, out: Path) -> None:
        self.out = out
        self.classes: dict[str, dict[str, object]] = {}

    def write(self, cls: str, name: str, lines: list[str], what: str, extra: dict | None = None) -> None:
        data = "".join(line + "\n" for line in lines).encode("utf-8")
        (self.out / name).write_bytes(data)
        entry: dict[str, object] = {"file": name, "lines": len(lines), "sha256": hashlib.sha256(data).hexdigest(), "what": what}
        if name != "names.tsv":
            tags = Counter(tag_family(line.rsplit("\t", 1)[1]) for line in lines)
            entry["tags"] = dict(sorted(tags.items()))
        if extra:
            entry.update(extra)
        self.classes[cls] = entry
        print(f"{cls} {name}: {len(lines)}", file=sys.stderr)


def unique(values):
    seen = set()
    out = []
    for value in values:
        key = tuple(value) if isinstance(value, list) else value
        if key not in seen:
            seen.add(key)
            out.append(value)
    return out


def load_cells() -> list[dict]:
    return json.loads(CELLS.read_text(encoding="utf-8"))["cells"]


def cell_lines(family: str) -> list[str]:
    """The run cells of a family, one line per distinct text of a row."""
    lines = []
    for cell in load_cells():
        if cell["family"] == family:
            lines.append(f"{cell['units']}\tcell:{cell['row']}")
    return unique(lines)


# --------------------------------------------------------------------------------------------------
# U2: names.

class Names:
    """names.tsv: `<kind>\\t<property>\\t<value>`, kind lone (value empty) or pair; no name holds a tab, a
    newline or a '/'. The sub-classes follow one another in the order below; manifest.json records each one's
    first line and count. Lines of the deterministic sub-classes are written once (the first time); the drawn
    ones are written as drawn."""

    def __init__(self) -> None:
        self.lines: list[str] = []
        self.seen: set[tuple[str, str, str]] = set()
        self.subclasses: list[dict[str, object]] = []
        self._start = 0

    def add(self, kind: str, prop: str, value: str = "", dedupe: bool = True) -> None:
        for field in (prop, value):
            assert "\t" not in field and "\n" not in field and "/" not in field, field
        key = (kind, prop, value)
        if dedupe:
            if key in self.seen:
                return
            self.seen.add(key)
        self.lines.append(f"{kind}\t{prop}\t{value}")

    def begin(self) -> None:
        self._start = len(self.lines)

    def end(self, name: str, what: str) -> None:
        self.subclasses.append({"name": name, "first_line": self._start + 1, "count": len(self.lines) - self._start, "what": what})


def mutations(name: str) -> dict[str, list[str]]:
    """The refused set's changes of one name: its letter case changed (lower, upper, swapped, first letter
    swapped), one underscore removed (each in turn), a space or a hyphen inserted (at each position, the ends
    included; and, as a loose matcher reads them alike, each underscore replaced by a space or a hyphen), and
    an Is prefix."""
    out = {
        "case": [name.lower(), name.upper(), name.swapcase(), name[:1].swapcase() + name[1:]],
        "underscore": [name[:k] + name[k + 1:] for k, ch in enumerate(name) if ch == "_"],
        "space-hyphen": [name[:p] + ch + name[p:] for p in range(len(name) + 1) for ch in (" ", "-")]
                        + [name[:k] + ch + name[k + 1:] for k, c in enumerate(name) if c == "_" for ch in (" ", "-")],
        "is": ["Is" + name],
    }
    return {kind: [m for m in unique(values) if m != name] for kind, values in out.items()}


def names_population(ucd: U.UCD, draws: int) -> tuple[list[str], list[dict], str]:
    n = Names()
    binary_aliases = list(ucd.ecma_binary)
    strings = list(ucd.ecma_strings)
    nonbinary = list(ucd.ecma_nonbinary)
    by_property = {canonical: [alias for alias in nonbinary if ucd.ecma_nonbinary[alias] == canonical]
                   for canonical in ("General_Category", "Script", "Script_Extensions")}
    gc_values = [alias for rec in ucd.value_aliases["gc"] for alias in rec]
    sc_values = [alias for rec in ucd.value_aliases["sc"] for alias in rec]
    values_of = {"General_Category": gc_values, "Script": sc_values, "Script_Extensions": sc_values}
    admitted_lone = set(gc_values) | set(binary_aliases) | set(strings)

    n.begin()
    for alias in binary_aliases + strings + nonbinary:
        n.add("lone", alias)
    n.end("ecma262", "every name and alias of ECMA-262's three tables, alone")

    n.begin()
    for rec in ucd.property_aliases:
        for alias in rec:
            n.add("lone", alias)
    n.end("property-aliases", "every name and alias of PropertyAliases.txt, alone")

    n.begin()
    for prop, recs in ucd.value_aliases.items():
        for rec in recs:
            for alias in rec:
                n.add("lone", alias)
    n.end("value-aliases", "every value and alias of PropertyValueAliases.txt, alone")

    n.begin()
    for canonical, props in by_property.items():
        for prop in props:
            for value in values_of[canonical]:
                n.add("pair", prop, value)
    n.end("pairs", "Name=Value for General_Category, Script and Script_Extensions: every alias on both sides")

    representative = {"General_Category": ["Lu", "Uppercase_Letter"], "Script": ["Latn", "Latin"],
                      "Script_Extensions": ["Latn", "Latin"]}
    for kind, what in (("case", "with its letter case changed"), ("underscore", "with an underscore removed"),
                       ("space-hyphen", "with a space or a hyphen inserted"), ("is", "with an Is prefix")):
        n.begin()
        for name in binary_aliases + gc_values + strings:
            for m in mutations(name)[kind]:
                if m not in admitted_lone:
                    n.add("lone", m)
        for canonical, props in by_property.items():
            admitted_values = set(values_of[canonical])
            for prop in props:
                for value in values_of[canonical]:
                    for m in mutations(value)[kind]:
                        if m not in admitted_values:
                            n.add("pair", prop, m)
                for m in mutations(prop)[kind]:
                    if m not in ucd.ecma_nonbinary:
                        for value in representative[canonical]:
                            n.add("pair", m, value)
        n.end(f"refused-{kind}", f"each admitted name, lone and on either side of a pair, {what}")

    n.begin()
    binary_values = sorted({alias for prop, recs in ucd.value_aliases.items()
                            if ucd.property_long.get(prop) in ucd.binary for rec in recs for alias in rec})
    for canonical, props in by_property.items():
        admitted_values = set(values_of[canonical])
        wrong = [v for other, vs in values_of.items() if other != canonical for v in vs] + binary_aliases + strings + nonbinary
        for prop in props:
            for value in unique(wrong):
                if value not in admitted_values:
                    n.add("pair", prop, value)
    for prop in binary_aliases + strings:
        for value in binary_values:
            n.add("pair", prop, value)
    n.end("refused-wrong-property", "values as the value of the wrong property: General_Category's of Script and "
          "Script_Extensions, Script's of General_Category, binary properties, properties of strings and property "
          "names as values, and binary properties and properties of strings given a value")

    n.begin()
    for prop, recs in ucd.value_aliases.items():
        if prop in ("gc", "sc"):
            continue
        record = next((rec for rec in ucd.property_aliases if rec[0] == prop), [prop])
        for alias in record:
            for rec in recs:
                for value in rec:
                    n.add("pair", alias, value)
    n.end("outside-tables", "every other property of PropertyValueAliases.txt (Age, Block, Bidi_Class, Line_Break, "
          "Canonical_Combining_Class, the binary properties' Y/N...) as Name=Value, every alias on both sides; their "
          "names alone are in property-aliases")

    n.begin()
    for cell in load_cells():
        if cell["family"] == "names":
            kind, prop, value = cell["parameter"].split(" ", 1)[0].split("/")
            n.add(kind, prop, value)
    n.end("cells", "the names of correspondence rows S1 and S2")

    alphabet = "".join(sorted({ch for line in n.lines for field in line.split("\t")[1:] for ch in field}))
    n.begin()
    stream = splitmix64(SEED_U2)
    for _ in range(draws):
        length = between(next(stream), 1, 40)
        text = "".join(alphabet[choice(next(stream), len(alphabet))] for _ in range(length))
        at = between(next(stream), 0, length)
        n.add("lone", text, "", dedupe=False)
        n.add("pair", text[:at], text[at:], dedupe=False)
    n.end("random", f"{draws} strings drawn with seed {SEED_U2:#x}: a draw gives the length, 1 to 40; each "
          "character is a draw over the alphabet; a draw gives the split, 0 to the length; each string is written "
          "alone, then split into a pair")
    return n.lines, n.subclasses, alphabet


# --------------------------------------------------------------------------------------------------
# U3: casing.

def context_code_points(ucd: U.UCD) -> list[int]:
    return sorted({c for name in ("Cased", "Case_Ignorable", "Soft_Dotted") for a, b in ucd.binary[name]
                   for c in range(a, b + 1)} | set(CONTEXT_EXTRAS))


def substitute(template: str, c: int) -> list[int]:
    assert template.count("c") == 1
    return [c if ch == "c" else ord(ch) for ch in template]


def every_code_point(reduced: bool) -> range | list[int]:
    if reduced:
        return list(range(0x300)) + list(range(0xD7F0, 0xE010)) + [0x10FFFF]
    return range(U.CODE_POINTS)


def casing_population(ucd: U.UCD, draws: int, reduced: bool) -> list[str]:
    lines = [text_line(U.code_units([cp]), "alone") for cp in every_code_point(reduced)]
    targets = context_code_points(ucd)
    if reduced:
        targets = targets[:32] + CONTEXT_EXTRAS
    for c in targets:
        for template in CONTEXTS:
            lines.append(text_line(U.code_units(substitute(template, c)), f"context:{template}"))
    for c in targets:
        for template in CONTEXTS:
            cps = substitute(template, c)
            for at in range(len(cps) + 1):
                where = "before" if at == 0 else "after" if at == len(cps) else f"inside{at}"
                for lone in (LONE_HIGH, LONE_LOW):
                    units = U.code_units(cps[:at]) + [lone] + U.code_units(cps[at:])
                    lines.append(text_line(units, f"surrogate:{where}:{lone:04X}:{template}"))
    lines += cell_lines("casing")
    marks = [cp for cp in range(U.CODE_POINTS) if ucd.general_category(cp) in ("Mn", "Mc", "Me")]
    pools = {
        "context": context_code_points(ucd),
        "ascii": list(range(0x80)),
        "mark0": [cp for cp in marks if ucd.ccc[cp] == 0],
        "mark220": [cp for cp in range(U.CODE_POINTS) if ucd.ccc[cp] == 220],
        "mark230": [cp for cp in range(U.CODE_POINTS) if ucd.ccc[cp] == 230],
        "mark-other": [cp for cp in range(U.CODE_POINTS) if ucd.ccc[cp] not in (0, 220, 230)],
        "special": U3_SPECIALS,
    }
    stream = splitmix64(SEED_U3)
    for _ in range(draws):
        count = between(next(stream), 1, 12)
        cps = []
        for _ in range(count):
            draw = next(stream)
            pool = pools[U3_KINDS[choice(draw, len(U3_KINDS))]]
            cps.append(pool[second(draw, len(pool))])
        lines.append(text_line(U.code_units(cps), "random"))
    return lines


# --------------------------------------------------------------------------------------------------
# U4: normalization.

def normalization_test_lines(ucd: U.UCD, limit: int | None) -> list[str]:
    lines = []
    for number, line in enumerate(ucd.pins.read("NormalizationTest.txt").split("\n"), 1):
        data = line.split("#", 1)[0].strip()
        if not data or line.startswith("@"):
            continue
        columns = data.split(";")[:5]
        for k, column in enumerate(columns, 1):
            lines.append(text_line(U.code_units(U.code_points_of(column)), f"test-file:NormalizationTest.txt:{number}:c{k}"))
        if limit is not None and len(lines) >= limit:
            break
    return lines


def normalization_pools(ucd: U.UCD) -> dict[str, list]:
    pairs = [d for d in ucd.canonical.values() if len(d) == 2]
    classes = sorted({ucd.ccc[cp] for cp in range(U.CODE_POINTS)} - {0})
    by_class = {k: [cp for cp in range(U.CODE_POINTS) if ucd.ccc[cp] == k] for k in classes}
    decomposed = set(ucd.nfkd_map)
    supplementary = sorted({cp for cp in decomposed if cp >= 0x10000} | {c for d in pairs for c in d if c >= 0x10000})
    return {
        "starter": sorted({d[0] for d in pairs if ucd.ccc[d[0]] == 0} | set(range(0x41, 0x5B)) | set(range(0x61, 0x7B))),
        "mark": [by_class[k] for k in classes],
        "L": list(range(U.L_BASE, U.L_BASE + U.L_COUNT)),
        "V": list(range(U.V_BASE, U.V_BASE + U.V_COUNT)),
        "T": list(range(U.T_BASE + 1, U.T_BASE + U.T_COUNT)),
        "LV": [U.S_BASE + k * U.T_COUNT for k in range(U.L_COUNT * U.V_COUNT)],
        "LVT": [s for s in range(U.S_BASE, U.S_BASE + U.S_COUNT) if (s - U.S_BASE) % U.T_COUNT],
        "exclusion": sorted(ucd.composition_exclusions),
        "singleton": sorted(cp for cp, d in ucd.canonical.items() if len(d) == 1),
        "non-starter": sorted(cp for cp, d in ucd.canonical.items() if ucd.ccc[cp] != 0 or ucd.ccc[d[0]] != 0),
        "supplementary": supplementary,
        "surrogate": list(range(0xD800, 0xE000)),
    }


def normalization_population(ucd: U.UCD, draws: int, reduced: bool) -> list[str]:
    lines = [text_line(U.code_units([cp]), "alone") for cp in every_code_point(reduced)]
    lines += normalization_test_lines(ucd, 250 if reduced else None)
    lines += cell_lines("normalization")
    pools = normalization_pools(ucd)
    stream = splitmix64(SEED_U4)
    for _ in range(draws):
        count = between(next(stream), 1, 12)
        cps = []
        for _ in range(count):
            draw = next(stream)
            kind = U4_KINDS[choice(draw, len(U4_KINDS))]
            if kind == "mark":
                pool = pools["mark"][second(draw, len(pools["mark"]))]
                cps.append(pool[third(draw, len(pool))])
            else:
                pool = pools[kind]
                cps.append(pool[second(draw, len(pool))])
        lines.append(text_line(U.code_units(cps), "random"))
    return lines


# --------------------------------------------------------------------------------------------------
# U5: properties of strings.

def emoji_sequences(ucd: U.UCD) -> list[tuple[str, list[int]]]:
    """Every sequence of the two files in file order, a range expanded into its code points."""
    out = []
    for name in ("emoji-sequences.txt", "emoji-zwj-sequences.txt"):
        for rec in U.records(ucd.pins.read(name)):
            if ".." in rec[0]:
                a, b = U.code_range(rec[0])
                out += [(rec[1], [c]) for c in range(a, b + 1)]
            else:
                out.append((rec[1], list(U.code_points_of(rec[0]))))
    return out


def string_properties_population(ucd: U.UCD, reduced: bool) -> list[str]:
    sequences = emoji_sequences(ucd)
    if reduced:
        sequences = sequences[:40] + sequences[-40:]
    lines = [text_line(U.code_units(seq), f"sequence:{name}") for name, seq in sequences]
    for name, seq in sequences:
        if len(seq) >= 2:
            for k in range(len(seq)):
                lines.append(text_line(U.code_units(seq[:k] + seq[k + 1:]), f"removed:{name}"))
    for name, seq in sequences:
        for added in U5_ADDED:
            lines.append(text_line(U.code_units([added] + seq), f"added-start:{added:04X}:{name}"))
            lines.append(text_line(U.code_units(seq + [added]), f"added-end:{added:04X}:{name}"))
    for name, seq in sequences:
        for k, c in enumerate(seq):
            if c in (0xFE0E, 0xFE0F):
                changed = seq[:k] + [0xFE0E if c == 0xFE0F else 0xFE0F] + seq[k + 1:]
                lines.append(text_line(U.code_units(changed), f"vs-changed:{name}"))
    lines += cell_lines("string-properties")
    return lines


# --------------------------------------------------------------------------------------------------
# U6: segmentation.

def break_test_texts(ucd: U.UCD, name: str) -> list[tuple[int, list[int]]]:
    out = []
    for number, line in enumerate(ucd.pins.read(name).split("\n"), 1):
        data = line.split("#", 1)[0].strip()
        if data:
            out.append((number, [int(t, 16) for t in data.split() if t not in ("÷", "×")]))
    return out


def segmentation_kinds(ucd: U.UCD) -> tuple[list[str], dict[str, list[int]], dict[str, int]]:
    """The drawn alphabet of U6: every value of Grapheme_Cluster_Break, Word_Break and Sentence_Break that has a
    code point, weight 1 each, and six extra kinds weighted as the three test files weigh them: a kind of
    weight w is listed w times, w the kind's share of the code points of the three files' lines times the
    number of values, rounded half up, and at least 1."""
    pools: dict[str, list[int]] = {}
    for prop, names, values in (("GCB", ucd.gcb_names, ucd.gcb), ("WB", ucd.wb_names, ucd.wb), ("SB", ucd.sb_names, ucd.sb)):
        members: dict[int, list[int]] = {}
        for cp in range(U.CODE_POINTS):
            members.setdefault(values[cp], []).append(cp)
        for index in sorted(members, key=lambda i: names[i]):
            pools[f"{prop}={names[index]}"] = members[index]
    values = list(pools)
    extras = {
        "space": [cp for cp in range(U.CODE_POINTS) if ucd.sb_names[ucd.sb[cp]] == "Sp"],
        "punctuation": [cp for cp in range(U.CODE_POINTS) if ucd.general_category(cp).startswith("P")],
        "digit": [cp for cp in range(U.CODE_POINTS) if ucd.general_category(cp) == "Nd"],
        "ideograph": [cp for cp in range(U.CODE_POINTS) if ucd.ideographic[cp]],
        "katakana": [cp for cp in range(U.CODE_POINTS) if ucd.wb_names[ucd.wb[cp]] == "Katakana"],
        "regional-indicator": [cp for a, b in ucd.binary["Regional_Indicator"] for cp in range(a, b + 1)],
    }
    member_of = {kind: set(pool) for kind, pool in extras.items()}
    counts = Counter()
    total = 0
    for name in BREAK_TESTS:
        for _, cps in break_test_texts(ucd, name):
            total += len(cps)
            for c in cps:
                for kind in U6_EXTRAS:
                    if c in member_of[kind]:
                        counts[kind] += 1
    weights = {kind: max(1, (2 * counts[kind] * len(values) + total) // (2 * total)) for kind in U6_EXTRAS}
    kinds = values + [kind for kind in U6_EXTRAS for _ in range(weights[kind])]
    pools.update(extras)
    return kinds, pools, weights


def segmentation_population(ucd: U.UCD, draws: int, reduced: bool) -> tuple[list[str], dict]:
    lines = []
    for name in BREAK_TESTS:
        texts = break_test_texts(ucd, name)
        if reduced:
            texts = texts[:60]
        lines += [text_line(U.code_units(cps), f"test-file:{name}:{number}") for number, cps in texts]
    lines += cell_lines("segmentation")
    kinds, pools, weights = segmentation_kinds(ucd)
    stream = splitmix64(SEED_U6)
    for _ in range(draws):
        count = between(next(stream), 1, 16)
        cps = []
        for _ in range(count):
            draw = next(stream)
            pool = pools[kinds[choice(draw, len(kinds))]]
            cps.append(pool[second(draw, len(pool))])
        lines.append(text_line(U.code_units(cps), "random"))
    return lines, {"kinds": len(kinds), "values": len(kinds) - sum(weights.values()), "weights": weights}


# --------------------------------------------------------------------------------------------------
# U7: limits.

GIANT = 1 << 24


def repeat_line(piece: list[int], count: int, suffix: list[int], tag: str) -> str:
    return f"repeat\t{U.format_units(piece)}\t{count}\t{U.format_units(suffix)}\t{tag}"


def length_changing(ucd: U.UCD) -> list[int]:
    """Every code point whose full case mapping, alone, in some language, has another length in code units.
    Only a code point SpecialCasing.txt names, or whose simple mapping crosses U+FFFF, can."""
    candidates = set(ucd.special_unconditional) | set(ucd.special_conditional)
    for table in (ucd.simple_upper, ucd.simple_lower):
        candidates |= {c for c, m in table.items() if (c > 0xFFFF) != (m > 0xFFFF)}
    out = []
    for cp in sorted(candidates):
        units = U.code_units([cp])
        for upper, language in U.CASING_OPERATIONS:
            mapped = U.code_units(ucd.change_case([cp], upper == "upper", language))
            if len(mapped) != len(units):
                out.append(cp)
                break
    return out


def limits_population(ucd: U.UCD, reduced: bool) -> list[str]:
    lines = []
    for cp in length_changing(ucd):
        lines.append(text_line(U.code_units([cp]), "casing:length-changing"))
    for cps, what in ((([0x49, 0x300]), "longer:lithuanian"), ([0x4A, 0x301], "longer:lithuanian"),
                      ([0x12E, 0x301], "longer:lithuanian"), ([0xCC], "longer:lithuanian"),
                      ([0x69, 0x307], "shorter:lithuanian"), ([0x6A, 0x307, 0x301], "shorter:lithuanian"),
                      ([0x49, 0x307], "shorter:turkic"), ([0x49, 0x316, 0x307], "shorter:turkic"),
                      ([0x41, 0x3A3], "same:final-sigma")):
        lines.append(text_line(U.code_units(cps), f"casing:{what}"))
    for cps, what in (([0xE9], "longer"), ([0x344], "longer"), ([0xF73], "longer"), ([0x1D15E], "longer"),
                      ([0xFB01], "longer"), ([0xFDFA], "longer"), ([0x3300], "longer"), ([0x2474], "longer"),
                      ([0xAC01], "longer"), ([0x65, 0x301], "shorter"), ([0x1100, 0x1161, 0x11A8], "shorter"),
                      ([0x1D400], "shorter"), ([0x2F800], "shorter"), ([0x41, 0x30A], "shorter")):
        lines.append(text_line(U.code_units(cps), f"normalization:{what}"))
    scale = 4096 if reduced else 1
    giant = GIANT // scale
    a = [0x61]
    for piece, count, suffix, what in (
            ([0xDF], giant // 2, [], "result-2^24:upper"), ([0xDF], giant // 2, a, "result-2^24+1:upper"),
            ([0x130], giant // 2, [], "result-2^24:lower"), ([0x130], giant // 2, a, "result-2^24+1:lower"),
            ([0x390], giant // 3, a * (giant % 3), "result-2^24:upper"), ([0x390], giant // 3, a * (giant % 3 + 1), "result-2^24+1:upper"),
            ([0xCC], giant // 3, a * (giant % 3), "result-2^24:lower-lithuanian"), ([0xCC], giant // 3, a * (giant % 3 + 1), "result-2^24+1:lower-lithuanian"),
            (a, giant, [], "input-2^24"), (a, giant, a, "input-2^24+1")):
        lines.append(repeat_line(U.code_units(piece), count, U.code_units(suffix), f"casing:{what}"))
    for piece, count, suffix, what in (
            ([0xE9], giant // 2, [], "result-2^24:NFD"), ([0xE9], giant // 2, a, "result-2^24+1:NFD"),
            ([0x65, 0x301], giant // 2, [], "input-2^24:NFC-shorter"), ([0x65, 0x301], giant // 2, a, "input-2^24+1:NFC-shorter"),
            ([0xFDFA], giant // 18, a * (giant % 18), "result-2^24:NFKD"), ([0xFDFA], giant // 18, a * (giant % 18 + 1), "result-2^24+1:NFKD"),
            ([0x1D15E], giant // 4, [], "result-2^24:NFD"), ([0x1D15E], giant // 4, a, "result-2^24+1:NFD"),
            (a, giant, [], "input-2^24"), (a, giant, a, "input-2^24+1")):
        lines.append(repeat_line(U.code_units(piece), count, U.code_units(suffix), f"normalization:{what}"))
    return lines


# --------------------------------------------------------------------------------------------------

def check_out(out: Path) -> None:
    """The population is never written into the repository, except under artifacts/, which git ignores."""
    target = out.resolve()
    repository = U.REPOSITORY.resolve()
    if target == repository or repository in target.parents:
        allowed = repository / "artifacts"
        if not (target == allowed or allowed in target.parents):
            raise SystemExit(f"refused: {out} is inside the repository; write under {allowed} or outside it")


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--out", required=True, type=Path)
    parser.add_argument("--reduced", action="store_true",
                        help="a small population for the tests (fewer draws and code points); never the judged one")
    parser.add_argument("--pins", type=Path, default=U.PIN_FILE)
    args = parser.parse_args(argv)
    check_out(args.out)
    try:
        ucd = U.UCD(U.Pins(args.pins))
    except U.PinError as error:
        print(f"refused: {error}", file=sys.stderr)
        return 2
    args.out.mkdir(parents=True, exist_ok=True)
    divisor = REDUCED_DRAW_DIVISOR if args.reduced else 1
    w = Writer(args.out)

    names, subclasses, alphabet = names_population(ucd, U2_DRAWS // divisor)
    w.write("U2", "names.tsv", names, "property names, values and aliases, alone and in pairs, the refused set, and drawn strings",
            {"subclasses": subclasses, "alphabet": alphabet, "seed": f"{SEED_U2:#x}", "draws": U2_DRAWS // divisor})
    w.write("U3", "casing.tsv", casing_population(ucd, U3_DRAWS // divisor, args.reduced),
            "every code point alone; the eleven contexts for every Cased, Case_Ignorable or Soft_Dotted code point and "
            "I, J, Į, Σ, İ, ı; U+D800 and U+DC00 before, after and inside each; the run cells of C1-C11; drawn strings",
            {"seed": f"{SEED_U3:#x}", "draws": U3_DRAWS // divisor, "contexts": CONTEXTS,
             "context_code_points": len(context_code_points(ucd)), "kinds": U3_KINDS, "specials": [f"{c:04X}" for c in U3_SPECIALS]})
    w.write("U4", "normalization.tsv", normalization_population(ucd, U4_DRAWS // divisor, args.reduced),
            "every code point alone; every column of every NormalizationTest.txt line; the run cells of N1-N7; drawn strings",
            {"seed": f"{SEED_U4:#x}", "draws": U4_DRAWS // divisor, "kinds": U4_KINDS})
    w.write("U5", "string-properties.tsv", string_properties_population(ucd, args.reduced),
            "every sequence of the two emoji files; each with one code point removed, U+200D and U+FE0F added at either "
            "end, and one variation selector changed; the run cells of S1-S3",
            {"implicit": "every code point from U+0000 to U+10FFFF alone, a surrogate as one code unit, in each of the "
                         "seven properties: not written; sets.tsv of unicoderef.py gives the single code points of each",
             "added": [f"{c:04X}" for c in U5_ADDED]})
    segmentation, alphabet6 = segmentation_population(ucd, U6_DRAWS // divisor, args.reduced)
    w.write("U6", "segmentation.tsv", segmentation,
            "every line of the three break test files (the text); the run cells of G1-G7; drawn strings",
            {"seed": f"{SEED_U6:#x}", "draws": U6_DRAWS // divisor, "alphabet": alphabet6})
    w.write("U7", "limits.tsv", limits_population(ucd, args.reduced),
            "the declared inputs whose result is longer than the input, shorter, and of 2^24 code units and one more; "
            "the maximum-length variants of U3 and U4 are the harness's",
            {"giant": GIANT // (4096 if args.reduced else 1)})

    manifest = {
        "rule": RULE,
        "generator": GENERATOR,
        "generator_version": GENERATOR_VERSION,
        "reference": "scripts/reference-arms/unicoderef.py",
        "size": "reduced" if args.reduced else "full",
        "pin": {"file": "data/unicode/pins/unicode.pin", "sha256": ucd.pins.sha256, "version": ucd.pins.version},
        "inputs": dict(sorted(ucd.pins.read_files.items())),
        "cells": {"file": "scripts/reference-arms/cells.json", "sha256": hashlib.sha256(CELLS.read_bytes()).hexdigest()},
        "draw": "one output of SplitMix64 from the class's seed; a choice among n things is the draw modulo n; an "
                "integer from [low, high] is low plus the draw modulo the range's size; a second value is the draw "
                "shifted right by 16 bits, a third the draw shifted right by 32 bits, each modulo its own range",
        "seeds": {"U2": f"{SEED_U2:#x}", "U3": f"{SEED_U3:#x}", "U4": f"{SEED_U4:#x}", "U6": f"{SEED_U6:#x}"},
        "classes": {"U1": {"file": None, "implicit": "every code point from U+0000 to U+10FFFF, the 2,048 surrogates included "
                                                     "(each as one code unit), in every code-point operation",
                           "count": U.CODE_POINTS}, **w.classes},
    }
    (args.out / "manifest.json").write_text(json.dumps(manifest, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    return 0


if __name__ == "__main__":
    sys.exit(main())
