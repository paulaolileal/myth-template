param(
    [Parameter(Mandatory = $true)]
    [string]$Name,
    [switch]$Clean
)

if ($Name -match '[^a-zA-Z0-9._-]') {
    Write-Error "Project name must contain only letters, numbers, dots, hyphens or underscores."
    exit 1
}

Write-Host "Setting up template for project: $Name" -ForegroundColor Green

# Rename folders
Write-Host "Renaming folders..."
Get-ChildItem -Directory | Where-Object { $_.Name -like "*Myth.Template*" } | ForEach-Object {
    $newName = $_.Name.Replace("Myth.Template", $Name)
    Rename-Item $_.Name $newName
    Write-Host "  $($_.Name) -> $newName"
}

# Rename files
Write-Host "Renaming files..."
Get-ChildItem -Recurse -File | Where-Object { $_.Name -like "*Myth.Template*" } | ForEach-Object {
    $newName = $_.Name.Replace("Myth.Template", $Name)
    $newPath = Join-Path $_.Directory $newName
    Rename-Item $_.FullName $newPath
    Write-Host "  $($_.Name) -> $newName"
}

# Update content
Write-Host "Updating content..."
Get-ChildItem -Recurse -Include "*.cs","*.csproj","*.slnx","*.json","*.resx","*.md" | ForEach-Object {
    $content = Get-Content $_.FullName -Raw -Encoding UTF8
    if ($content -like "*Myth.Template*") {
        $newContent = $content.Replace("Myth.Template", $Name)
        Set-Content $_.FullName $newContent -Encoding UTF8
        Write-Host "  Updated: $($_.Name)"
    }
}

# Clean examples if requested
if ($Clean) {
    Write-Host "Cleaning WeatherForecast examples..."

    $filesToRemove = @(
        "$Name.Domain\Models\WeatherForecast.cs",
        "$Name.Domain\Models\Summary.cs",
        "$Name.Domain\Interfaces\IWeatherForecastRepository.cs",
        "$Name.Domain\Specifications\WeatherForecastSpecification.cs",
        "$Name.Data\Contexts\ForecastContext.cs",
        "$Name.Data\Repositories\WeatherForecastRepository.cs",
        "$Name.Data\Mappings\WeatherForecastMap.cs",
        "$Name.Application\InitializeFakeData.cs",
        "$Name.API\Controllers\WeatherForecastController.cs",
        "$Name.Test\WeatherForecastTests.cs"
    )

    foreach ($file in $filesToRemove) {
        if (Test-Path $file) {
            Remove-Item $file -Force
            Write-Host "  Removed: $file"
        }
    }

    if (Test-Path "$Name.Application\WeatherForecasts") {
        Remove-Item "$Name.Application\WeatherForecasts" -Recurse -Force
        Write-Host "  Removed: WeatherForecasts folder"
    }

    # Create simple AppContext
    Write-Host "Creating base AppContext..."
    $contextDir = "$Name.Data\Contexts"
    if (!(Test-Path $contextDir)) {
        New-Item -Path $contextDir -ItemType Directory -Force | Out-Null
    }

    # Simple approach - write lines directly
    $contextFile = "$contextDir\AppContext.cs"
    "using Microsoft.EntityFrameworkCore;" | Out-File $contextFile -Encoding UTF8
    "using Myth.Contexts;" | Out-File $contextFile -Append -Encoding UTF8
    "" | Out-File $contextFile -Append -Encoding UTF8
    "namespace $Name.Data.Contexts;" | Out-File $contextFile -Append -Encoding UTF8
    "" | Out-File $contextFile -Append -Encoding UTF8
    "public class AppContext : BaseContext" | Out-File $contextFile -Append -Encoding UTF8
    "{" | Out-File $contextFile -Append -Encoding UTF8
    "    public AppContext(DbContextOptions<AppContext> options) : base(options)" | Out-File $contextFile -Append -Encoding UTF8
    "    {" | Out-File $contextFile -Append -Encoding UTF8
    "    }" | Out-File $contextFile -Append -Encoding UTF8
    "" | Out-File $contextFile -Append -Encoding UTF8
    "    protected override void OnModelCreating(ModelBuilder modelBuilder)" | Out-File $contextFile -Append -Encoding UTF8
    "    {" | Out-File $contextFile -Append -Encoding UTF8
    "        base.OnModelCreating(modelBuilder);" | Out-File $contextFile -Append -Encoding UTF8
    "        // Configure your models here" | Out-File $contextFile -Append -Encoding UTF8
    "    }" | Out-File $contextFile -Append -Encoding UTF8
    "}" | Out-File $contextFile -Append -Encoding UTF8

    Write-Host "  Created AppContext.cs"
}

# Reset Git
Write-Host "Resetting Git..."
if (Test-Path ".git") {
    Remove-Item ".git" -Recurse -Force
}

git init
git branch -m main
git add .

if ($Clean) {
    git commit -m "feat: setup $Name from myth-template with clean structure"
} else {
    git commit -m "feat: setup $Name from myth-template with examples"
}

Write-Host "Done! Project renamed to: $Name" -ForegroundColor Green

if ($Clean) {
    Write-Host "WeatherForecast examples removed, base AppContext created" -ForegroundColor Cyan
} else {
    Write-Host "WeatherForecast examples kept for reference" -ForegroundColor Cyan
}

Write-Host "`nNext steps:"
Write-Host "1. git remote add origin <repository-url>"
Write-Host "2. Update appsettings.json"
Write-Host "3. Run 'dotnet build' to verify"

# Self-delete script and setup instructions
Write-Host "Setup script completed and will now remove itself." -ForegroundColor Cyan
Remove-Item $PSCommandPath -Force