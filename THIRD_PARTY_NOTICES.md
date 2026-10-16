# Third-party notices

This repository contains generated tables based on data published by the Unicode
Consortium, including Unicode emoji and CLDR data. Those data files and derived tables
remain subject to the [Unicode Terms of Use](https://www.unicode.org/terms_of_use.html).

The Apache License 2.0 in [`LICENSE`](LICENSE) applies to Broiler's source code and does
not replace the Unicode Terms of Use for Unicode-provided data.

Every package of this repository carries this file.

## The pinned Unicode archive, and the tables derived from it

*Added 2026-10-16, at work package SJL-7.2 of the shared JavaScript libraries programme.*

**What is archived.** [`data/unicode/pins/`](data/unicode/pins/README.md) holds the following, unmodified:

- the Unicode Character Database 17.0.0 files the algorithm tables are generated from;
- the auxiliary break data and its tests;
- the emoji 17.0 sequence files;
- the Unicode licence text, as `unicode-LICENSE.txt`.

[`unicode.pin`](data/unicode/pins/unicode.pin) records each file's length and SHA-256. The files are
© Unicode, Inc., under the Unicode License v3 (SPDX `Unicode-3.0`), reproduced below. They were copied
byte for byte from Broiler.VM's archive, which records how each was retrieved from unicode.org.

**What is derived, and where it ships.**

- `src/Unicode.Generation` writes `src/Unicode.Tables/UnicodeTableData.g.cs` from those files. It compiles
  into `Broiler.Unicode.Tables` and ships in that package.
- `Broiler.Unicode.Algorithms` and `Broiler.Unicode.Segmentation` carry no table. Each reads its tables
  from the provider its caller passes.
- An engine that embeds the generated tables carries Unicode data in its own assembly and its own notices,
  under decision SJL-D-6 (c).
- `Broiler.Unicode.Generation` carries this repository's pin file, which names Unicode files by hash and
  holds no Unicode data.

The tables are Unicode data under the Unicode License v3. The generator and the lookup code are
Broiler's source code under the Apache License 2.0.

## ECMA-262's property tables

*Added 2026-10-16, at work package SJL-7.2.*

[`data/unicode/pins/ecma262/`](data/unicode/pins/ecma262) holds three tables of the ECMAScript Language
Specification, ECMA-262, 17th edition (ES2026), unmodified, from the `tc39/ecma262` repository at
`0248456c`:

- `table-binary-unicode-properties.html`;
- `table-nonbinary-unicode-properties.html`;
- `table-binary-unicode-properties-of-strings.html`.

They are © Ecma International. That repository licenses its natural-language text under the **Alternative
copyright notice of the Ecma text copyright policy**, which is retained in full beside them at
[`ECMA-alternative-copyright-notice.txt`](data/unicode/pins/ecma262/ECMA-alternative-copyright-notice.txt).

The generator reads them for the set of property names, values and aliases ECMAScript admits. Those names
are encoded into the name tables of `UnicodeTableData.g.cs`, and so into `Broiler.Unicode.Tables`. No prose
of the specification is copied. The notice of changes the Ecma notice's condition (iii) asks for is:

Broiler.Unicode includes material derived from the ECMAScript Language Specification, ECMA-262,
17th edition (ES2026), https://ecma-international.org/publications-and-standards/standards/ecma-262/.
Copyright (c) Ecma International.

Condition (i) asks for the notice's full text where users of the derivative work can see it. This file
travels in every package, so the notice is reproduced here:

```
ALTERNATIVE COPYRIGHT NOTICE AND COPYRIGHT LICENSE

(c) Ecma International

By obtaining and/or copying this work, you (the licensee) agree that you have read, understood,
and will comply with the following terms and conditions.

Permission under Ecma's copyright to copy, modify, prepare derivative works of, and distribute
this work, with or without modification, for any purpose and without fee or royalty is hereby
granted, provided that you include the following on ALL copies of the work or portions thereof,
including modifications:

(i)   The full text of this COPYRIGHT NOTICE AND COPYRIGHT LICENSE in a location viewable to
      users of the redistributed or derivative work.

(ii)  Any pre-existing intellectual property disclaimers, notices, or terms and conditions. If
      none exist, the Ecma alternative copyright notice should be included.

(iii) Notice of any changes or modifications, through a copyright statement on the document such
      as "This document includes material copied from or derived from [title and URL of the Ecma
      document]. Copyright (c) Ecma International."

DISCLAIMERS

THIS WORK IS PROVIDED "AS IS," AND COPYRIGHT HOLDERS MAKE NO REPRESENTATIONS OR WARRANTIES,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO, WARRANTIES OF MERCHANTABILITY OR FITNESS FOR
ANY PARTICULAR PURPOSE OR THAT THE USE OF THE DOCUMENT WILL NOT INFRINGE ANY THIRD PARTY PATENTS,
COPYRIGHTS, TRADEMARKS OR OTHER RIGHTS.

COPYRIGHT HOLDERS WILL NOT BE LIABLE FOR ANY DIRECT, INDIRECT, SPECIAL OR CONSEQUENTIAL DAMAGES
ARISING OUT OF ANY USE OF THE DOCUMENT.

The name and trademarks of copyright holders may NOT be used in advertising or publicity
pertaining to the work without specific, written prior permission. Title to copyright in this work
will at all times remain with copyright holders.
```

## Code ported from Broiler.VM

*Added 2026-10-16, at work package SJL-7.2.*

The following are ports of Broiler.VM's code, and their assurance annotations say `Origin=Ported`:

- the generator in `src/Unicode.Generation`;
- the table formats;
- the algorithms of `src/Unicode.Algorithms` and `src/Unicode.Segmentation`.

Broiler.VM is a Broiler component under the same Apache License 2.0 and the same copyright holder, so the
port brings no third-party term.

## The Unicode licence text

As archived at `data/unicode/pins/unicode-LICENSE.txt`:

```
UNICODE LICENSE V3

COPYRIGHT AND PERMISSION NOTICE

Copyright © 1991-2026 Unicode, Inc.

NOTICE TO USER: Carefully read the following legal agreement. BY
DOWNLOADING, INSTALLING, COPYING OR OTHERWISE USING DATA FILES, AND/OR
SOFTWARE, YOU UNEQUIVOCALLY ACCEPT, AND AGREE TO BE BOUND BY, ALL OF THE
TERMS AND CONDITIONS OF THIS AGREEMENT. IF YOU DO NOT AGREE, DO NOT
DOWNLOAD, INSTALL, COPY, DISTRIBUTE OR USE THE DATA FILES OR SOFTWARE.

Permission is hereby granted, free of charge, to any person obtaining a
copy of data files and any associated documentation (the "Data Files") or
software and any associated documentation (the "Software") to deal in the
Data Files or Software without restriction, including without limitation
the rights to use, copy, modify, merge, publish, distribute, and/or sell
copies of the Data Files or Software, and to permit persons to whom the
Data Files or Software are furnished to do so, provided that either (a)
this copyright and permission notice appear with all copies of the Data
Files or Software, or (b) this copyright and permission notice appear in
associated Documentation.

THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS", WITHOUT WARRANTY OF ANY
KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF
THIRD PARTY RIGHTS.

IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS NOTICE
BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL DAMAGES,
OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS,
WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION,
ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THE DATA
FILES OR SOFTWARE.

Except as contained in this notice, the name of a copyright holder shall
not be used in advertising or otherwise to promote the sale, use or other
dealings in these Data Files or Software without prior written
authorization of the copyright holder.
```
