$ErrorActionPreference = "Stop"
Write-Host "Applying Markdown Extended Fix..." -ForegroundColor Cyan

# 1. Setup local target for Mermaid
$localVscodeDir = Join-Path $env:USERPROFILE ".vscode\local"
if (!(Test-Path $localVscodeDir)) { 
    New-Item -ItemType Directory -Force $localVscodeDir | Out-Null 
}

# 2. Copy the Mermaid engine
if (Test-Path ".\patch\mermaid.min.js") {
    Copy-Item ".\patch\mermaid.min.js" -Destination (Join-Path $localVscodeDir "mermaid.min.js") -Force
    Write-Host "  [OK] Copied mermaid.min.js to $localVscodeDir" -ForegroundColor Green
} else {
    Write-Host "  [Error] Could not find .\patch\mermaid.min.js - run this script from the MarkForge folder." -ForegroundColor Red
}

# 3. Patch the extension's shared.js
$extPath = Join-Path $env:USERPROFILE ".vscode\extensions\jebbs.markdown-extended-1.1.4\out\src\services\exporter"
if (Test-Path $extPath) {
    Copy-Item ".\patch\shared.js" -Destination (Join-Path $extPath "shared.js") -Force
    Write-Host "  [OK] Successfully injected patch into jebbs.markdown-extended-1.1.4!" -ForegroundColor Green
} else {
    Write-Host "  [Warning] jebbs.markdown-extended-1.1.4 not found on this machine. Please install the extension first." -ForegroundColor Yellow
}

# 4. Instructions for Settings
$fixedName = $env:USERNAME
Write-Host ""
Write-Host "Almost done! Remember to add these two lines to your global VS Code settings.json:" -ForegroundColor Cyan
Write-Host ""
Write-Host '    "markdownExtended.exportOutDirName": "",'
Write-Host ('    "markdown-pdf.mermaidServer": "file:///C:/Users/' + $fixedName + '/.vscode/local/mermaid.min.js"')
Write-Host ""
Write-Host "Press any key to exit..."
$Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown') | Out-Null