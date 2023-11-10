# To collect code coverage, you will need the following environment setup:
#
# - A "GODOT" environment variable pointing to the Godot executable
# - ReportGenerator installed
#
#     dotnet tool install -g dotnet-reportgenerator-globaltool
#
# - A version of coverlet > 3.2.0.
#
#   As of Jan 2023, this is not yet released.
#
#   The included `nuget.config` file will allow you to install a nightly
#   version of coverlet from the coverlet nightly nuget feed.
#
#     dotnet tool install --global coverlet.console --prerelease.
#
#   You can build coverlet yourself, but you will need to edit the path to
#   coverlet below to point to your local build of the coverlet dll.
dotnet build --no-restore

coverlet `
  "./.godot/mono/temp/bin/Debug" --verbosity detailed `
  --target $env:GODOT `
  --targetargs "--run-tests --coverage --quit-on-finish" `
  --format "opencover" `
  --output "./coverage/coverage.xml" `
  --exclude-by-file "**/test/**/*.cs" `
  --exclude-by-file "**/*Microsoft.NET.Test.Sdk.Program.cs" `
  --exclude-by-file "**/Godot.SourceGenerators/**/*.cs" `
  --exclude-assemblies-without-sources "missingall"

# Projects included via <ProjectReference> will be collected in code coverage.
# If you want to exclude them, replace the string below with the names of
# the assemblies to ignore. e.g.,
# $ASSEMBLIES_TO_REMOVE="-AssemblyToRemove1;-AssemblyToRemove2"
$ASSEMBLIES_TO_REMOVE=""

reportgenerator `
  -reports:"./coverage/coverage.xml" `
  -targetdir:"./coverage/report" `
  "-assemblyfilters:$ASSEMBLIES_TO_REMOVE" `
  "-classfilters:-GodotPlugins.Game.Main;-GameDemo.Main" `
  -reporttypes:"Html;Badges"

# Copy badges into their own folder. The badges folder should be included in
# source control so that the README.md in the root can reference the badges.
If (!(Test-Path -Path "./badges")) {
    New-Item -ItemType directory -Path "./badges"
}
Move-Item "./coverage/report/badge_branchcoverage.svg" "./badges/branch_coverage.svg" -Force
Move-Item "./coverage/report/badge_linecoverage.svg" "./badges/line_coverage.svg" -Force

Invoke-Expression ("cmd /c start coverage/report/index.htm")
