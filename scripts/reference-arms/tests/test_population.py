"""The population generator: determinism, the classes the rule declares, its formats, and its refusals."""

from __future__ import annotations

import hashlib
import json
import os
import shutil
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

HERE = Path(__file__).resolve().parents[1]
if str(HERE) not in sys.path:
    sys.path.insert(0, str(HERE))

import generate_population as P  # noqa: E402
import unicoderef as U  # noqa: E402
from splitmix64 import splitmix64  # noqa: E402

FILES = ["names.tsv", "casing.tsv", "normalization.tsv", "string-properties.tsv", "segmentation.tsv", "limits.tsv", "manifest.json"]
TEXT_FILES = ["casing.tsv", "normalization.tsv", "string-properties.tsv", "segmentation.tsv"]


def ucd() -> U.UCD:
    return U.database()


class DeterminismTests(unittest.TestCase):
    def test_two_runs_write_the_same_bytes(self) -> None:
        # Two processes, with different string hashing, so that no set's order can reach the output.
        def run(out: str, seed: str) -> int:
            environment = dict(os.environ, PYTHONHASHSEED=seed)
            return subprocess.run([sys.executable, "-I", "-B", str(HERE / "generate_population.py"), "--out", out, "--reduced"],
                                  env=environment, capture_output=True).returncode

        with tempfile.TemporaryDirectory() as first, tempfile.TemporaryDirectory() as second:
            self.assertEqual(0, run(first, "1"))
            self.assertEqual(0, run(second, "2"))
            for name in FILES:
                a, b = (Path(first) / name).read_bytes(), (Path(second) / name).read_bytes()
                self.assertEqual(hashlib.sha256(a).hexdigest(), hashlib.sha256(b).hexdigest(), name)
            manifest = json.loads((Path(first) / "manifest.json").read_text(encoding="utf-8"))
            self.assertEqual("reduced", manifest["size"])
            for cls, info in manifest["classes"].items():
                if info.get("file"):
                    data = (Path(first) / info["file"]).read_bytes()
                    self.assertEqual(info["sha256"], hashlib.sha256(data).hexdigest(), cls)
                    self.assertEqual(info["lines"], data.count(b"\n"), cls)

    def test_drawn_strings_are_a_prefix_of_the_longer_draw(self) -> None:
        def drawn(lines: list[str]) -> list[str]:
            return [line for line in lines if line.endswith("\trandom")]

        short = drawn(P.casing_population(ucd(), 8, reduced=True))
        long = drawn(P.casing_population(ucd(), 32, reduced=True))
        self.assertEqual(8, len(short))
        self.assertEqual(short, long[:8])


class FormatTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls) -> None:
        cls.directory = tempfile.TemporaryDirectory()
        cls.out = Path(cls.directory.name)
        assert P.main(["--out", str(cls.out), "--reduced"]) == 0

    @classmethod
    def tearDownClass(cls) -> None:
        cls.directory.cleanup()

    def test_every_text_line_is_hex_units_then_a_tag(self) -> None:
        for name in TEXT_FILES:
            for line in (self.out / name).read_text(encoding="utf-8").split("\n")[:-1]:
                text, tag = line.split("\t")
                self.assertTrue(tag, line)
                for unit in text.split(" "):
                    self.assertRegex(unit, r"^[0-9A-F]{4}$", (name, line))

    def test_names_lines_have_three_fields(self) -> None:
        for line in (self.out / "names.tsv").read_text(encoding="utf-8").split("\n")[:-1]:
            kind, prop, value = line.split("\t")
            self.assertIn(kind, ("lone", "pair"))
            if kind == "lone":
                self.assertEqual("", value)
            self.assertNotIn("/", prop + value)

    def test_limits_lines(self) -> None:
        for line in (self.out / "limits.tsv").read_text(encoding="utf-8").split("\n")[:-1]:
            fields = line.split("\t")
            if fields[0] == "repeat":
                self.assertEqual(5, len(fields))
                int(fields[2])
                self.assertIn(fields[4].split(":")[0], ("casing", "normalization"))
            else:
                self.assertEqual(2, len(fields))
                self.assertIn(fields[1].split(":")[0], ("casing", "normalization"))

    def test_lone_surrogates_never_pair_up_in_the_contexts(self) -> None:
        for line in (self.out / "casing.tsv").read_text(encoding="utf-8").split("\n")[:-1]:
            text, tag = line.split("\t")
            if tag.startswith("surrogate:"):
                lone = int(tag.split(":")[2], 16)
                units = U.parse_units(text)
                self.assertIn(lone, U.code_points(units), line)

    def test_the_manifest_records_u1_implicitly_and_the_seeds(self) -> None:
        manifest = json.loads((self.out / "manifest.json").read_text(encoding="utf-8"))
        self.assertIsNone(manifest["classes"]["U1"]["file"])
        self.assertEqual(0x110000, manifest["classes"]["U1"]["count"])
        self.assertIn("implicit", manifest["classes"]["U5"])
        self.assertEqual({"U2": "0x534a4c372d303031", "U3": "0x534a4c372d303032", "U4": "0x534a4c372d303033",
                          "U6": "0x534a4c372d303034"}, manifest["seeds"])
        self.assertEqual(U.Pins().sha256, manifest["pin"]["sha256"])


class ClassTests(unittest.TestCase):
    def test_u2_holds_the_tables_the_aliases_and_the_refused_set(self) -> None:
        lines, subclasses, alphabet = P.names_population(ucd(), 16)
        names = set(lines)
        for line in ("lone\tASCII\t", "lone\tspace\t", "lone\tRGI_Emoji\t", "lone\tGeneral_Category\t", "lone\tAge\t",
                     "lone\tBlock\t", "lone\tName\t", "lone\tBidi_Class\t", "lone\tLine_Break\t", "lone\tWSpace\t",
                     "pair\tgc\tLu", "pair\tScript_Extensions\tHrkt", "pair\tsc\tKatakana_Or_Hiragana",
                     "pair\tgc\tCombining_Mark", "lone\tlu\t", "lone\tLU\t", "lone\tCasedLetter\t", "lone\tCased Letter\t",
                     "lone\tCased-Letter\t", "lone\tCased_ Letter\t", "lone\tC-ased_Letter\t", "lone\tIsLu\t",
                     "pair\tsc\tIsLatin",
                     "pair\tsc\tLu", "pair\tgc\tLatin", "pair\tASCII\tY", "pair\tAge\t1.1", "pair\tBlock\tBasic_Latin",
                     "pair\tbc\tL", "pair\tlb\tAL", "lone\trgiemoji\t", "pair\tGC\tLu", "pair\tgeneral_category\tLu"):
            self.assertIn(line, names)
        self.assertEqual(" -.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz", alphabet)
        random = next(s for s in subclasses if s["name"] == "random")
        self.assertEqual(32, random["count"])

    def test_u2_draws_read_as_stated(self) -> None:
        lines, subclasses, alphabet = P.names_population(ucd(), 2)
        stream = splitmix64(P.SEED_U2)
        length = 1 + next(stream) % 40
        text = "".join(alphabet[next(stream) % len(alphabet)] for _ in range(length))
        at = next(stream) % (length + 1)
        random = next(s for s in subclasses if s["name"] == "random")
        first = random["first_line"] - 1
        self.assertEqual(f"lone\t{text}\t", lines[first])
        self.assertEqual(f"pair\t{text[:at]}\t{text[at:]}", lines[first + 1])

    def test_u3_contexts_substitute_c_in_each_of_the_eleven(self) -> None:
        self.assertEqual(11, len(P.CONTEXTS))
        self.assertEqual([0x41, 0x1C5, 0x3A3], P.substitute("AcΣ", 0x1C5))
        self.assertEqual([0x4A, 0x10400, 0x300], P.substitute("Jc\u0300", 0x10400))
        targets = set(P.context_code_points(ucd()))
        for cp in (0x49, 0x4A, 0x12E, 0x3A3, 0x130, 0x131, 0xAA, 0x345, 0x2B0, 0x69, 0x1DF1A):
            self.assertIn(cp, targets)
        self.assertNotIn(0xD800, targets)

    def test_u4_random_alphabet_holds_every_kind(self) -> None:
        pools = P.normalization_pools(ucd())
        self.assertIn(0x344, pools["non-starter"])
        self.assertIn(0x212B, pools["singleton"])
        self.assertIn(0x0958, pools["exclusion"])
        self.assertIn(0x1D15E, pools["supplementary"])
        self.assertEqual(0xAC00, pools["LV"][0])
        self.assertEqual(len(set(map(tuple, pools["mark"]))), len(pools["mark"]))
        self.assertEqual(16, len(P.U4_KINDS))

    def test_u5_variants(self) -> None:
        lines = P.string_properties_population(ucd(), reduced=False)
        tags = {line.split("\t")[1].split(":")[0] for line in lines}
        self.assertEqual({"sequence", "removed", "added-start", "added-end", "vs-changed", "cell"}, tags)
        self.assertIn("231A\tsequence:Basic_Emoji", lines)
        self.assertIn("00A9 FE0E\tvs-changed:Basic_Emoji", lines)
        self.assertIn("200D 231A\tadded-start:200D:Basic_Emoji", lines)

    def test_u6_weights_come_from_the_test_files(self) -> None:
        kinds, pools, weights = P.segmentation_kinds(ucd())
        self.assertEqual(14 + 19 + 15, len(kinds) - sum(weights.values()))
        self.assertTrue(all(w >= 1 for w in weights.values()))
        self.assertIn(0x20, pools["space"])
        self.assertIn(0x1F1E6, pools["regional-indicator"])

    def test_u7_giant_inputs_reach_2_24_and_one_more(self) -> None:
        lines = P.limits_population(ucd(), reduced=False)
        for line in lines:
            fields = line.split("\t")
            if fields[0] != "repeat":
                continue
            piece, count, suffix, tag = U.parse_units(fields[1]), int(fields[2]), U.parse_units(fields[3]), fields[4]
            family, what = tag.split(":", 1)
            operations = U.CASING_OPERATIONS if family == "casing" else U.FORMS
            lengths = [U.repeat_result(ucd(), family, op, piece, count, suffix)[0] for op in operations]
            if what.startswith("input-"):
                self.assertEqual(P.GIANT + (1 if "+1" in what else 0), len(piece) * count + len(suffix), tag)
            else:
                self.assertIn(P.GIANT + (1 if "+1" in what else 0), lengths, tag)

    def test_the_repeat_shortcut_is_the_full_computation(self) -> None:
        for line in P.limits_population(ucd(), reduced=False):
            fields = line.split("\t")
            if fields[0] != "repeat":
                continue
            piece, suffix, family = U.parse_units(fields[1]), U.parse_units(fields[3]), fields[4].split(":")[0]
            for count in (3, 4, 7, 300):
                text = U.repeat_units(piece, count, suffix)
                for op in (U.CASING_OPERATIONS if family == "casing" else U.FORMS):
                    if family == "casing":
                        result = U.code_units(ucd().change_case(U.code_points(text), op[0] == "upper", op[1]))
                    else:
                        result = ucd().normalize(text, op)
                    expected = (len(result), hashlib.sha256(U.utf16le(result)).hexdigest())
                    self.assertEqual(expected, U.repeat_result(ucd(), family, op, piece, count, suffix), (line, count, op))

    def test_cells_cover_the_rows(self) -> None:
        rows = {cell["row"] for cell in P.load_cells()}
        for row in ["C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "C10", "C11", "N1", "N2", "N4", "N5", "S1", "S2",
                    "G1", "G2", "G3"]:
            self.assertIn(row, rows)


class RefusalTests(unittest.TestCase):
    def test_the_repository_is_refused_but_artifacts(self) -> None:
        with self.assertRaises(SystemExit):
            P.check_out(U.REPOSITORY / "scripts" / "reference-arms" / "out")
        P.check_out(U.REPOSITORY / "artifacts" / "reference-arms")
        P.check_out(Path(tempfile.gettempdir()) / "population")

    def test_a_changed_pinned_byte_is_refused_before_anything_is_written(self) -> None:
        with tempfile.TemporaryDirectory() as root:
            pins = Path(root) / "data" / "unicode" / "pins"
            shutil.copytree(U.PIN_FILE.parent, pins)
            target = pins / "ucd-17.0.0" / "CaseFolding.txt"
            data = bytearray(target.read_bytes())
            data[-2] ^= 0x01
            target.write_bytes(bytes(data))
            out = Path(root) / "population"
            self.assertEqual(2, P.main(["--out", str(out), "--pins", str(pins / "unicode.pin")]))
            self.assertFalse(out.exists())
            with self.assertRaises(U.PinError):
                U.UCD(U.Pins(pins / "unicode.pin"))

    def test_a_changed_pin_hash_is_refused(self) -> None:
        with tempfile.TemporaryDirectory() as root:
            pins = Path(root) / "data" / "unicode" / "pins"
            shutil.copytree(U.PIN_FILE.parent, pins)
            pin = pins / "unicode.pin"
            text = pin.read_text(encoding="utf-8")
            line = next(l for l in text.split("\n") if "SpecialCasing.txt" in l)
            digest = line.split()[-1]
            pin.write_text(text.replace(digest, ("0" if digest[0] != "0" else "1") + digest[1:]), encoding="utf-8")
            with self.assertRaises(U.PinError) as caught:
                U.UCD(U.Pins(pin))
            self.assertIn("SpecialCasing.txt", str(caught.exception))
            self.assertEqual(2, U.main(["--out", str(Path(root) / "reference"), "--pins", str(pin)]))


if __name__ == "__main__":
    unittest.main()
