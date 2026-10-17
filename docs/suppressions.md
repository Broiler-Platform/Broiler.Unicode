# Warning suppressions in the programme's projects

[← Conventions](conventions.md)

Convention 12 suppresses no trim, AOT or single-file warning, and convention 17 makes every warning an
error. This inventory, in the shape of Broiler.VM's rule N36, lists by file every warning suppression in
a source or project file of the projects the shared JavaScript libraries programme governs here
(`src/Unicode.Algorithms`, `src/Unicode.Segmentation`, `src/Unicode.Tables`, `src/Unicode.Generation` and
`probes/Unicode.Probe`) and in the repository's build files they import (`Directory.Build.props`,
`Directory.Packages.props` and `eng/Broiler.Packaging.props`): a `#pragma warning disable`, a
`SuppressMessage` or `UnconditionalSuppressMessage` attribute, a non-empty `NoWarn`, a
`WarningsNotAsErrors` or a `SuppressTrimAnalysisWarnings` element. Rule SL5 scans those files and holds
this section to what it finds: each suppression is listed here, and when the scan finds none the section
says None.

Each of the four libraries sets `NoWarn` in its own project file to `1701;1702`, the compiler's
informational codes the SDK puts in every project. That **replaces** the XML-documentation codes the
packaging file appends, so a public member without its documentation is an error there (convention 15).
It resets the property rather than suppressing anything, and is not listed. The test projects, whose
sources are the rules' own and name every marker the scan looks for, and the witnesses of the rules, which
exist to break them, are not scanned. The packages that existed before the programme are not governed and
are not scanned either.

## Inventory

- `eng/Broiler.Packaging.props`: `NoWarn` appends `CS1570`, `CS1572`, `CS1573`, `CS1574`, `CS1587`,
  `CS1591` and `CS1734`, the XML-documentation warnings, for every project of the repository. The
  libraries reset it in their project files, so it does not reach them. It does reach the probe,
  `probes/Unicode.Probe`, whose project file sets no `NoWarn` of its own. The probe generates no
  documentation file, so none of these warnings can arise there. Rule SL5 still refuses the probe's
  evaluated `NoWarn` until its project file resets it as the libraries' do.
