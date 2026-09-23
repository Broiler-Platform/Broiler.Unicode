# Publishing previews

The **Publish** GitHub Actions workflow validates and publishes all three runtime
packages together. Generators, data tools, tests, and benchmarks are excluded.
Each package includes its README, icon, third-party notices, XML API documentation,
and assemblies for `net8.0` and `net10.0`; `.snupkg` symbol packages are also produced.

## Version selection

Every run reads all shipping package IDs from **both nuget.org and GitHub Packages**,
regardless of the destination. For the configured `VersionPrefix`, it chooses one
more than the highest published `preview.N`, with the configured `VersionSuffix`
as a minimum. For example, GitHub `0.1.0-preview.3` plus nuget.org
`0.1.0-preview.2` produces **`0.1.0-preview.4`** on either destination.
Unlisted versions and partial releases contribute to this calculation.

An optional `version-suffix` must be `preview.N` and at least this calculated next
version. A pushed tag must be `vX.Y.Z-preview.N` and meet the same rule. Existing
previews cannot be reused to copy a release between feeds. Lookup failures stop
the run, including dry runs, rather than risk choosing an already used number.
Dry runs do not reserve a number; a subsequent publish resolves the feeds again.
The workflow serializes publish runs in this repository. Publications outside
this workflow must be coordinated to avoid races and feed indexing delays.

## One-time setup

1. Provide a nuget.org API key permitted to push all three IDs listed in the root
   README through an accessible `NUGET_API_KEY` Actions secret or the shared
   organization `NUGET_TOKEN` secret. `NUGET_API_KEY` takes precedence if both are
   set. Actual nuget.org publication requires a key; dry runs do not need it.
2. Ensure this repository's `GITHUB_TOKEN` can read the corresponding GitHub
   packages, including for nuget.org runs. The workflow already requests
   `packages: read` for resolution and `packages: write` for publishing to GitHub.
   Existing packages associated with another repository may need this repository
   added to their Actions access settings. See
   [GitHub's NuGet authentication documentation](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry).
3. Confirm the human review record in [HUMAN_REVIEW.md](../HUMAN_REVIEW.md) covers
   the revision to release. Its existing attestation is revision-scoped; code
   changes require renewed human review under that record's conditions.

## Validate and publish

1. Run **Actions → Publish → Run workflow**, choose the intended ref and `target`
   (`nuget` or `github`), leave `version-suffix` empty, and keep `dry-run` enabled.
2. Review the resolved version, CI results, and `nuget-packages` artifact. CI builds
   and tests on Linux and Windows. The publish job restores a fresh consumer for
   both supported frameworks from the packed artifacts and destination dependency
   sources, using an isolated package cache.
3. Run the same workflow with `dry-run` disabled when ready to publish. Nuget.org
   receives packages and symbols; GitHub receives the `.nupkg` files. Alternatively,
   pushing a valid preview tag starts an actual nuget.org publication immediately.

Preview communication must state that this is preview software, developer-operated
data tools download and process Unicode/CLDR definitions, and the runtime libraries
use committed data without downloading it at runtime.

To perform the package checks locally, using .NET SDK 10, the .NET 8 runtime,
Node.js, and PowerShell 7:

```powershell
node --test eng/resolve-preview-version.test.mjs
dotnet build Broiler.Unicode.slnx -c Release
dotnet test Broiler.Unicode.slnx -c Release --no-build
./eng/pack.ps1 -Version 0.1.0-preview.4 -Output artifacts/preview-check
./eng/verify-feed.ps1 -Target nuget -Packages artifacts/preview-check
```

Use an empty output directory and replace the sample version as appropriate. For
a live version lookup, set `GITHUB_REPOSITORY_OWNER`, `GITHUB_ACTOR`, and
`GITHUB_TOKEN` (with package read access), then run
`node eng/resolve-preview-version.mjs`. This reads feeds without publishing.
