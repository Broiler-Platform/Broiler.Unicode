// REFERENCE ARM J: a test-only copy of Broiler.JS's implementation before the cut-over, for the
// reference-arm comparison of bundle sjl-7-001 (docs/evidence/sjl-7-001/reference-arms.md). It is never
// referenced outside this test project, and it is retired at SJL-16.
//
// Copied from Broiler.JS at 4d70821e (the engine sources the correspondence of Broiler.VM's
// docs/evidence/sjl-7-001/correspondence.md reads, at Broiler.JS bdb1a3ca):
//   Broiler.JS/Broiler.JavaScript.Parser/FastScanner.cs
//     blob d39b252394b6287f6bd3daf8738a5a109d35713a: the scanner's white-space test, the
//     condition of _ReadToken's skip loop (:379) with the comment above it (:376-378), and the loop's line
//     that reads a line terminator as one (:381-382)
//
// Changes from the source, each marked "ARM CHANGE" where it is made; none changes a result:
//   - the loop's condition is the body of a function of the code unit it reads, IsWhiteSpace(first), in a
//     static class FastScanner of its own; the condition is the source's, its U+FEFF literal included;
//   - the loop's line-terminator test is the function IsLineTerminatorInSkip(first): a code unit the loop
//     consumes is white space to the scanner unless that test reads it as a line terminator, which ends the
//     loop with a LineTerminator token (:388-389). The comparison calls the two as the loop does.

namespace Broiler.JavaScript.Parser;

internal static class FastScanner // ARM CHANGE: a static class of its own; the source's is the scanner
{
    // ARM CHANGE: the skip loop's condition (:379), as a function of the code unit the loop reads.
    internal static bool IsWhiteSpace(char first)
    {
        // U+FEFF ZERO WIDTH NO-BREAK SPACE (ZWNBSP/BOM) is ECMAScript WhiteSpace, but
        // .NET reclassified it out of White_Space so char.IsWhiteSpace returns false
        // for it; treat it as whitespace explicitly.
        return char.IsWhiteSpace(first) || first == '﻿';
    }

    // ARM CHANGE: the loop's line-terminator test (:381-382), as a function of the same code unit.
    internal static bool IsLineTerminatorInSkip(char first)
    {
        var lineTerminator = false; // ARM CHANGE: the loop's local (:373), for one code unit
        if (first.IsLineTerminator())
            lineTerminator = true;
        return lineTerminator; // ARM CHANGE: the local's value, which the source reads after the loop (:388)
    }
}
