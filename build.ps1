param (
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$NugetApiKey
)

# Set variables
$ProjectPath = "src/SharpGameService"
$TestProjectPath = "tests/SharpGameService.Tests"
$OutputPath = "artifacts"
$NugetSource = "https://api.nuget.org/v3/index.json"

# Ensure output directory exists
if (-Not (Test-Path -Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath | Out-Null
}

# Restore, build, and test the project
Write-Host "Restoring dependencies..."
dotnet restore $ProjectPath

Write-Host "Building the project..."
dotnet build $ProjectPath -c Release

Write-Host "Running tests..."
$TestResult = dotnet test $ProjectPath -c Release --no-build --logger "trx;LogFileName=test-results.trx"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Tests failed. Aborting the build process."
    exit 1
}

# Pack the project
Write-Host "Packing the project..."
dotnet pack $ProjectPath -c Release -o $OutputPath /p:Version=$Version

# Find the generated .nupkg file
$PackagePath = Get-ChildItem -Path $OutputPath -Filter "*.nupkg" | Select-Object -ExpandProperty FullName

if (-Not $PackagePath) {
    Write-Error "Failed to find the NuGet package in the output directory."
    exit 1
}

# Push the package to NuGet
Write-Host "Pushing the package to NuGet..."
dotnet nuget push $PackagePath -k $NugetApiKey -s $NugetSource

Write-Host "Package pushed successfully!"