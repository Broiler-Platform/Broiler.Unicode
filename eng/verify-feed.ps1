# Prove that a consumer can restore the complete release using its destination feed.
[CmdletBinding()]
param(
    [ValidateSet('nuget')][string] $Target = 'nuget',
    [string] $Packages = 'artifacts'
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
Add-Type -AssemblyName System.IO.Compression.FileSystem
$packagePath = (Resolve-Path $Packages).Path
$archives = @(Get-ChildItem -LiteralPath $packagePath -Filter *.nupkg)
if (!$archives.Count) { throw 'No packages to validate.' }
$scratch = Join-Path ([IO.Path]::GetTempPath()) ("broiler-feed-check-" + [guid]::NewGuid())
New-Item -ItemType Directory -Path $scratch | Out-Null
$references = @()
$mappings = @()
# The consumer targets every framework a package ships. A package that ships fewer, such as the
# net10.0-only packages of the shared JavaScript libraries' Unicode area, is referenced only from the
# frameworks it has, so the restore checks each package where it can be consumed.
$consumerFrameworks = @('net8.0', 'net10.0')
foreach ($archive in $archives) {
    $zip = [IO.Compression.ZipFile]::OpenRead($archive.FullName)
    try {
        $entry = @($zip.Entries | Where-Object FullName -like '*.nuspec')[0]
        $reader = [IO.StreamReader]::new($entry.Open())
        try { [xml] $nuspec = $reader.ReadToEnd() } finally { $reader.Dispose() }
        $id = [Security.SecurityElement]::Escape($nuspec.package.metadata.id)
        $version = [Security.SecurityElement]::Escape($nuspec.package.metadata.version)
        $frameworks = @($zip.Entries | Where-Object FullName -like 'lib/*/*' | ForEach-Object { $_.FullName.Split('/')[1] } | Select-Object -Unique)
        $supported = @($consumerFrameworks | Where-Object { $frameworks -contains $_ })
        if (!$supported.Count) { throw "$id ships none of $($consumerFrameworks -join ', ')." }
        $condition = ''
        if ($supported.Count -ne $consumerFrameworks.Count) {
            $condition = ' Condition="' + (($supported | ForEach-Object { "'`$(TargetFramework)' == '$_'" }) -join ' Or ') + '"'
        }
        $references += "    <PackageReference Include=`"$id`" Version=`"[$version]`"$condition />"
        $mappings += "      <package pattern=`"$id`" />"
    } finally { $zip.Dispose() }
}
$escapedPath = [Security.SecurityElement]::Escape($packagePath)
@"
<configuration>
  <packageSources>
    <clear />
    <add key="release" value="$escapedPath" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <disabledPackageSources><clear /></disabledPackageSources>
  <packageSourceMapping>
    <clear />
    <packageSource key="release">
$($mappings -join "`n")
    </packageSource>
    <packageSource key="nuget.org"><package pattern="*" /></packageSource>
  </packageSourceMapping>
</configuration>
"@ | Set-Content (Join-Path $scratch 'NuGet.config') -Encoding utf8
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net8.0;net10.0</TargetFrameworks>
    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
$($references -join "`n")
  </ItemGroup>
</Project>
"@ | Set-Content (Join-Path $scratch 'Consumer.csproj') -Encoding utf8
# An isolated cache prevents installed developer packages from hiding feed gaps.
& dotnet restore (Join-Path $scratch 'Consumer.csproj') --configfile (Join-Path $scratch 'NuGet.config') --packages (Join-Path $scratch 'packages') --no-http-cache --nologo
if ($LASTEXITCODE -ne 0) { throw "Consumer restore from $Target failed. Check feed access and publish missing dependencies first. Diagnostics: $scratch" }
Write-Host "Consumer restore verified $($archives.Count) packages against $Target."
