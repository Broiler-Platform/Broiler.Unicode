"""SplitMix64 against its published reference outputs, and the four ways a draw is read.

The rule (docs/evidence/sjl-7-001/reference-arms.md, configuration) asks for the generator's SplitMix64 to be
"tested against the generator's published reference outputs" before the population is trusted.
"""

from __future__ import annotations

import sys
import unittest
from pathlib import Path

HERE = Path(__file__).resolve().parents[1]
if str(HERE) not in sys.path:
    sys.path.insert(0, str(HERE))

from splitmix64 import between, choice, second, splitmix64, third  # noqa: E402


class SplitMix64Tests(unittest.TestCase):
    def test_the_published_reference_outputs(self) -> None:
        # Seed 1234567, the first five outputs of Vigna's splitmix64.c, as the reference implementations in
        # other languages publish them, and as Broiler.JS's tests/test_reference_population.py checks them.
        stream = splitmix64(1234567)
        self.assertEqual([next(stream) for _ in range(5)],
                         [6457827717110365317, 3203168211198807973, 9817491932198370423,
                          4593380528125082431, 16408922859458223821])

    def test_seed_zero(self) -> None:
        # splitmix64.c from state 0: the first output is the mix of the golden-ratio increment.
        self.assertEqual(next(splitmix64(0)), 0xE220A8397B1DCDAF)

    def test_outputs_are_64_bit_and_the_seed_is_masked(self) -> None:
        stream = splitmix64(-1)
        self.assertEqual([next(stream) for _ in range(3)], [next(s) for s in [splitmix64(0xFFFFFFFFFFFFFFFF)] for _ in range(3)])
        self.assertTrue(all(0 <= next(stream) < 1 << 64 for _ in range(1000)))

    def test_the_draw_readings(self) -> None:
        draw = 0x0123456789ABCDEF
        self.assertEqual(choice(draw, 10), draw % 10)
        self.assertEqual(between(draw, 1, 40), 1 + draw % 40)
        self.assertEqual(second(draw, 1000), (draw >> 16) % 1000)
        self.assertEqual(third(draw, 7), (draw >> 32) % 7)

    def test_the_rule_s_seeds_spell_the_bundle(self) -> None:
        for seed, text in ((0x534A4C372D303031, b"SJL7-001"), (0x534A4C372D303032, b"SJL7-002"),
                           (0x534A4C372D303033, b"SJL7-003"), (0x534A4C372D303034, b"SJL7-004")):
            self.assertEqual(seed.to_bytes(8, "big"), text)


if __name__ == "__main__":
    unittest.main()
