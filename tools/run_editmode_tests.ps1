param(
    [string]$UnityPath = "",
    [string]$ResultsPath = "",
    [string]$LogPath = ""
)

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$projectVersionFile = Join-Path $repoRoot "ProjectSettings\ProjectVersion.txt"
$projectVersionMatch = Select-String -Path $projectVersionFile -Pattern "^m_EditorVersion:\s*(.+)$"

if ($projectVersionMatch.Count -eq 0) {
    throw "Unable to read Unity editor version from '$projectVersionFile'."
}

$projectVersion = $projectVersionMatch[0].Matches[0].Groups[1].Value

if ([string]::IsNullOrWhiteSpace($UnityPath)) {
    $UnityPath = Join-Path $env:ProgramFiles "Unity\Hub\Editor\$projectVersion\Editor\Unity.exe"
}

if (-not (Test-Path $UnityPath)) {
    throw "Unity editor not found at '$UnityPath'."
}

if ([string]::IsNullOrWhiteSpace($ResultsPath)) {
    $ResultsPath = Join-Path $repoRoot "Logs\editmode_results.xml"
}

if ([string]::IsNullOrWhiteSpace($LogPath)) {
    $LogPath = Join-Path $repoRoot "Logs\editmode_tests.log"
}

$resultsDirectory = Split-Path -Parent $ResultsPath
$logDirectory = Split-Path -Parent $LogPath

if (-not [string]::IsNullOrWhiteSpace($resultsDirectory) -and -not (Test-Path $resultsDirectory)) {
    New-Item -ItemType Directory -Path $resultsDirectory -Force | Out-Null
}

if (-not [string]::IsNullOrWhiteSpace($logDirectory) -and -not (Test-Path $logDirectory)) {
    New-Item -ItemType Directory -Path $logDirectory -Force | Out-Null
}

# Unity Test Framework exits on completion. Passing -quit can stop batch mode before results are written.
& $UnityPath `
    -batchmode `
    -nographics `
    -projectPath $repoRoot `
    -runTests `
    -runSynchronously `
    -testPlatform EditMode `
    -testResults $ResultsPath `
    -logFile $LogPath

exit $LASTEXITCODE
