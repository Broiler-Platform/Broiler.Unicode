"""The classifier: each row's predicate on a difference built from the correspondence's own cells, and the
outputs it writes."""

from __future__ import annotations

import gzip
import json
import sys
import tempfile
import unittest
from pathlib import Path

HERE = Path(__file__).resolve().parents[1]
if str(HERE) not in sys.path:
    sys.path.insert(0, str(HERE))

import classify_reference_arms as C  # noqa: E402
import unicoderef as U  # noqa: E402


def ucd() -> U.UCD:
    return U.database()


def verdict(operation: str, input_: str, parameter: str, arm: str, s: str, result: str, cls: str = "U3") -> tuple[str, str, str]:
    v, row, unnamed, _, _ = C.classify(C.Difference([operation, cls, input_, parameter, arm, s, result], ucd()))
    return v, row, unnamed


def giant(operation: str, input_: str, parameter: str) -> str:
    return U.answer(ucd(), operation, input_, parameter)


class RowTests(unittest.TestCase):
    def test_lookups(self) -> None:
        self.assertEqual(("(a)", "P1", "no"), verdict("lookup", "lone/letter/", "u", "J", "refused", "General_Category=Letter", "U2"))
        self.assertEqual(("(a)", "P1", "no"), verdict("lookup", "pair/GC/Lu", "u", "J", "refused", "General_Category=Uppercase_Letter", "U2"))
        self.assertEqual(("(a)", "P2", "no"), verdict("lookup", "pair/sc/Hrkt", "u", "J", "Script=Katakana_Or_Hiragana", "refused", "U2"))
        self.assertEqual(("(a)", "P3", "no"), verdict("lookup", "lone/IsGreek/", "u", "J", "refused", "dotnet:IsGreek", "U2"))
        self.assertEqual(("(a)", "P4", "no"), verdict("lookup", "pair/gc/Lu", "v", "J", "General_Category=Uppercase_Letter", "refused", "U2"))
        self.assertEqual(("(a)", "P1", "no"), verdict("lookup", "lone/rgiemoji/", "v", "J", "refused", "RGI_Emoji", "U2"))

    def test_lexical(self) -> None:
        self.assertEqual(("(a)", "I1", "no"), verdict("lexical", "2E2F", "IdentifierStartChar", "J", "false", "true", "U1"))
        self.assertEqual(("(a)", "I3", "no"), verdict("lexical", "2E2F", "RegExpIdentifierStart", "J", "false", "true", "U1"))
        # Row I2's identifiers of Unicode 14 and 16: U+0870 is ID_Start, U+11B60 (Mn) ID_Continue only.
        self.assertEqual(("(a')", "I2", "no"), verdict("lexical", "0870", "IdentifierStartChar", "J", "true", "false", "U1"))
        self.assertEqual(("(a')", "I2", "no"), verdict("lexical", "11B60", "IdentifierPartChar", "J", "true", "false", "U1"))
        self.assertEqual(("(c)", "-", "-"), verdict("lexical", "11B60", "IdentifierStartChar", "J", "true", "false", "U1"))
        self.assertEqual(("(a)", "W1", "no"), verdict("lexical", "0085", "WhiteSpace", "J", "false", "true", "U1"))
        # A line terminator is not W1's.
        self.assertEqual(("(c)", "-", "yes"), verdict("lexical", "2028", "WhiteSpace", "J", "false", "true", "U1"))

    def test_casing(self) -> None:
        self.assertEqual(("(a)", "C1", "no"), verdict("lower", "00AA 03A3", "default", "J", "00AA 03C2", "00AA 03C3"))
        self.assertEqual(("(a)", "C4", "no"), verdict("lower", "A7CB", "turkic", "J", "0264", "A7CB"))
        self.assertEqual(("(a)", "C5", "no"), verdict("upper", "D837 DF1A 0307", "lithuanian", "J", "D837 DF1A", "D837 DF1A 0307"))
        self.assertEqual(("(a)", "C6", "no"), verdict("lower", "0049 0903 0300", "lithuanian", "J", "0069 0903 0300", "0069 0307 0903 0300"))
        self.assertEqual(("(a)", "C7", "no"), verdict("upper", "0069", "tr,en", "J", "0130", "exception:CultureNotFoundException"))
        self.assertEqual(("(a)", "C8", "no"), verdict("upper", "0069", "default@tr-TR", "J", "0049", "0130"))
        self.assertEqual(("(a')", "C3", "no"), verdict("lower", "D803 DD50", "default", "J", "D803 DD70", "D803 DD50"))
        # A Final_Sigma difference that Cased does not explain is row C2's, which admits no correction.
        self.assertEqual(("(c)", "C2", "no"), verdict("lower", "0391 03A3 00AD", "default", "J", "03B1 03C2 00AD", "03B1 03C3 00AD"))

    def test_normalization(self) -> None:
        self.assertEqual(("(a)", "N1", "no"), verdict("normalize", "D800", "NFC", "J", "D800", "exception:ArgumentException", "U4"))
        self.assertEqual(("(a')", "N2", "no"), verdict("normalize", "D801 DDC9", "NFD", "J", "D801 DDD2 0307", "D801 DDC9", "U4"))
        self.assertEqual(("(a')", "N2", "no"), verdict("normalize", "D818 DD1E D818 DD1E", "NFC", "J", "D818 DD21", "D818 DD1E D818 DD1E", "U4"))
        self.assertEqual(("(a')", "N3", "no"), verdict("normalize", "00E9", "NFD@invariant", "J", "0065 0301", "00E9", "U4"))
        self.assertEqual(("(c)", "-", "yes"), verdict("normalize", "00E9", "NFD", "J", "0065 0301", "0065 0302", "U4"))

    def test_segmentation(self) -> None:
        text = U.format_units([ord(c) for c in "One. Two"])
        self.assertEqual(("(a)", "G1", "no"), verdict("sentence", text, "-", "J", "0 5 8", "0 8", "U6"))
        self.assertEqual(("(a)", "G2", "no"), verdict("word", "0063 0061 006E 0027 0074", "-", "J", "0 5", "0 3 4 5", "U6"))
        self.assertEqual(("(a')", "G3", "no"), verdict("grapheme", "0061 0301", "-", "J", "0 2", "0 1 2", "U6"))
        self.assertEqual(("(b)", "G4", "-"), verdict("word-status", "005F", "-", "J", "N", "Y", "U6"))
        self.assertEqual(("(b)", "G4", "-"), verdict("word-status", "005F", "-", "V", "N", "Y", "U6"))

    def test_arm_v_limits(self) -> None:
        over = "repeat:00DF:8388608:0061"
        self.assertEqual(("(a)", "C12", "no"), verdict("upper", over, "default;max=16777216", "V", "over", "exception:RangeError", "U7"))
        fits = "repeat:00DF:8388608:"
        s = giant("upper", fits, "default;max=16777216")
        self.assertEqual(("(a)", "C12", "no"), verdict("upper", fits, "default;max=16777216", "V", s, "exception:RangeError", "U7"))
        n6 = "repeat:00E9:8388608:0061"
        s = giant("normalize", n6, "NFC;max=16777216")
        self.assertTrue(s.startswith("len:8388609:"))
        self.assertEqual(("(a)", "N6", "no"), verdict("normalize", n6, "NFC;max=16777216", "V", s, "exception:RangeError", "U7"))
        self.assertEqual(("(a)", "C12", "no"), verdict("lower", "0130", "lithuanian;max=1", "V", "over", "0069 0307", "U3"))

    def test_every_other_difference(self) -> None:
        self.assertEqual(("(c)", "-", "-"), verdict("charge", "0041", "upper/default", "V", "12", "13"))
        self.assertEqual(("(c)", "-", "yes"), verdict("lower", "0041", "default", "V", "0061", "0062"))
        self.assertEqual(("(c)", "-", "-"), verdict("lower", "0041", "default", "J", "0062", "0061"))
        self.assertEqual(("(c)", "-", "-"), verdict("lower", "0041", "default", "J", "0061", "0061"))
        self.assertEqual(("(c)", "-", "yes"), verdict("string-property", "231A", "Basic_Emoji", "J", "true", "false", "U5"))
        self.assertEqual(("unclassified", "-", "-"), verdict("gc", "0041", "-", "J-control", "Lu", "Ll", "U1"))
        self.assertEqual(("unclassified", "-", "-"), verdict("charge", "0041", "NFC", "V-control", "1", "2", "U4"))


class OutputTests(unittest.TestCase):
    def run_classifier(self, lines: list[str]) -> tuple[int, Path, tempfile.TemporaryDirectory]:
        directory = tempfile.TemporaryDirectory()
        run = Path(directory.name)
        with gzip.open(run / "differences.tsv.gz", "wt", encoding="utf-8", newline="\n") as stream:
            stream.write("".join(line + "\n" for line in lines))
        return C.main(["--run", str(run)]), run, directory

    def test_outputs_and_status(self) -> None:
        header = "\t".join(C.HEADER)
        status, run, directory = self.run_classifier([header, "lower\tU3\t00AA 03A3\tdefault\tJ\t00AA 03C2\t00AA 03C3",
                                                      "word-status\tU6\t005F\t-\tJ\tN\tY"])
        with directory:
            self.assertEqual(0, status)
            summary = json.loads((run / "classification.json").read_text(encoding="utf-8"))
            self.assertEqual({"(a)": 1, "(b)": 1}, summary["verdicts"])
            self.assertEqual(3, summary["differences.tsv"]["lines"])
            rows = gzip.open(run / "classification.tsv.gz", "rt", encoding="utf-8").read().split("\n")
            self.assertEqual("C1", rows[1].split("\t")[9])
            self.assertIn("(a) | C1 | J | 1", (run / "report.md").read_text(encoding="utf-8"))
            first = (run / "classification.tsv.gz").read_bytes()
            self.assertEqual(0, C.main(["--run", str(run)]))
            self.assertEqual(first, (run / "classification.tsv.gz").read_bytes())

        status, run, directory = self.run_classifier(["charge\tU3\t0041\tupper/default\tV\t12\t13"])
        with directory:
            self.assertEqual(1, status)

    def test_recorded_answers_are_checked(self) -> None:
        line = "lower\tU3\t00AA 03A3\tdefault\tJ\t00AA 03C2\t00AA 03C3"
        status, run, directory = self.run_classifier([line])
        with directory, tempfile.TemporaryDirectory() as reference:
            manifest = {"version": U.REFERENCE_VERSION, "pin": {"sha256": ucd().pins.sha256}}
            (Path(reference) / "reference-manifest.json").write_text(json.dumps(manifest), encoding="utf-8")
            cells = ["0041", "0061", "0041", "0061", "0041", "0061"]
            right = ["00AA 03A3", "00AA 03A3", "00AA 03C2", "00AA 03A3", "00AA 03C2", "00AA 03A3", "00AA 03C2"]
            (Path(reference) / "casing-ref.tsv").write_text("\t".join(["0041"] + cells) + "\n" + "\t".join(right) + "\n", encoding="utf-8")
            self.assertEqual(0, C.main(["--run", str(run), "--reference", reference]))
            summary = json.loads((run / "classification.json").read_text(encoding="utf-8"))
            self.assertEqual(1, summary["recorded answers"]["differences cross-checked"])
            wrong = list(right)
            wrong[2] = "00AA 03C3"
            (Path(reference) / "casing-ref.tsv").write_text("\t".join(wrong) + "\n", encoding="utf-8")
            self.assertEqual(3, C.main(["--run", str(run), "--reference", reference]))
            manifest["pin"]["sha256"] = "0" * 64
            (Path(reference) / "reference-manifest.json").write_text(json.dumps(manifest), encoding="utf-8")
            with self.assertRaises(SystemExit):
                C.main(["--run", str(run), "--reference", reference])


if __name__ == "__main__":
    unittest.main()
