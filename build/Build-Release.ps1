param (
	[Parameter(Mandatory=$true)]
	[ValidatePattern("^\d\.\d\.(?:\d\.\d$|\d$)")]
	[string]
	$ReleaseVersionNumber,
	[Parameter(Mandatory=$true)]
	[string]
	[AllowEmptyString()]
	$PreReleaseName
)

$PSScriptFilePath = (Get-Item $MyInvocation.MyCommand.Path);
$RepoRoot = (get-item $PSScriptFilePath).Directory.Parent.FullName;
$SolutionRoot = Join-Path -Path $RepoRoot "src";
$NuGetPackagesPath = Join-Path -Path $SolutionRoot "packages"

#trace
"Solution Root: $SolutionRoot"

$ProgFiles86 = [Environment]::GetEnvironmentVariable("ProgramFiles(x86)");
$MSBuild = "$ProgFiles86\MSBuild\14.0\Bin\MSBuild.exe"

# Make sure we don't have a release folder for this version already
$BuildFolder = Join-Path -Path $RepoRoot -ChildPath "build";
$ReleaseFolder = Join-Path -Path $BuildFolder -ChildPath "Releases\v$ReleaseVersionNumber$PreReleaseName";
if ((Get-Item $ReleaseFolder -ErrorAction SilentlyContinue) -ne $null)
{
	Write-Warning "$ReleaseFolder already exists on your local machine. It will now be deleted."
	Remove-Item $ReleaseFolder -Recurse
}
New-Item $ReleaseFolder -Type directory

# Go get nuget.exe if we don't hae it
$NuGet = "$BuildFolder\nuget.exe"
$FileExists = Test-Path $NuGet 
If ($FileExists -eq $False) {
	$SourceNugetExe = "http://nuget.org/nuget.exe"
	Invoke-WebRequest $SourceNugetExe -OutFile $NuGet
}

#trace
"Release path: $ReleaseFolder"

# Set the version number in SolutionInfo.cs
$AssemblyInfoPath = Join-Path -Path $SolutionRoot -ChildPath "FCNViewer\Properties\AssemblyInfo.cs"
(gc -Path $AssemblyInfoPath) `
	-replace "(?<=AssemblyFileVersion\(`")[.\d]*(?=`"\))", $ReleaseVersionNumber |
	sc -Path $AssemblyInfoPath -Encoding UTF8;
(gc -Path $AssemblyInfoPath) `
	-replace "(?<=AssemblyInformationalVersion\(`")[.\w-]*(?=`"\))", "$ReleaseVersionNumber$PreReleaseName" |
	sc -Path $AssemblyInfoPath -Encoding UTF8;
# Set the copyright
$Copyright = "Copyright � Shannon Deminick " + (Get-Date).year;
(gc -Path $AssemblyInfoPath) `
	-replace "(?<=AssemblyCopyright\(`").*(?=`"\))", $Copyright |
	sc -Path $AssemblyInfoPath -Encoding UTF8;

# Build the solution in release mode
$SolutionPath = Join-Path -Path $SolutionRoot -ChildPath "FCNViewer.sln";

# clean sln for all deploys
& $MSBuild "$SolutionPath" /p:Configuration=Release /maxcpucount /t:Clean
if (-not $?)
{
	throw "The MSBuild process returned an error code."
}

#build
& $MSBuild "$SolutionPath" /p:Configuration=Release /maxcpucount
if (-not $?)
{
	throw "The MSBuild process returned an error code."
}

$include = @('FCNViewer.dll','FCNViewer.pdb')
$BinFolder = Join-Path -Path $SolutionRoot -ChildPath "FCNViewer\bin\Release";

New-Item "$ReleaseFolder\bin\" -Type directory
Copy-Item "$BinFolder\*.*" -Destination "$ReleaseFolder\bin\" -Include $include

# COPY THE README OVER
Copy-Item "$BuildFolder\Readme.txt" -Destination $ReleaseFolder

# COPY OVER THE NUSPEC AND BUILD THE NUGET PACKAGE
Copy-Item "$BuildFolder\FCNViewer.nuspec" -Destination $ReleaseFolder
$NuSpec = Join-Path -Path $ReleaseFolder -ChildPath "FCNViewer.nuspec";

Write-Output "DEBUGGING: " $NuSpec -OutputDirectory $ReleaseFolder -Version $ReleaseVersionNumber$PreReleaseName
& $NuGet pack $NuSpec -OutputDirectory $ReleaseFolder -Version $ReleaseVersionNumber$PreReleaseName -Properties copyrightyear=$Copyright