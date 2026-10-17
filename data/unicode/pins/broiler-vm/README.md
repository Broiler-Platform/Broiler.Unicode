# Broiler.VM's Unicode pin, as this repository compares it

[`unicode.pin`](unicode.pin) is Broiler.VM's `src/tests/unicode/pins/unicode.pin`, copied byte for byte
from Broiler.VM at `7b44dc7` on 2026-10-16. The test project compares this repository's pin with it, by
file name, length and SHA-256 (bundle [`sjl-7-001`](../../../../docs/evidence/sjl-7-001/decision-rule.md),
part B), because a CI run here cannot read Broiler.VM. Broiler.VM's rule `I4` makes the same comparison
from the other side, against the pin `Broiler.Unicode.Generation` carries.

**It moves only with Broiler.VM's pin**: a dated decision that moves the Unicode version moves both
repositories' pins and this copy in the same change. Nothing here is generated from it.
