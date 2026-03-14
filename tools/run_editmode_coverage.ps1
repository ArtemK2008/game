param(
    [string]$UnityPath = "",
    [string]$TestResultsPath = "",
    [string]$CoverageResultsPath = "",
    [string]$CoverageHistoryPath = "",
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

if ([string]::IsNullOrWhiteSpace($TestResultsPath)) {
    $TestResultsPath = Join-Path $repoRoot "Logs\coverage\editmode_test_results.xml"
}

if ([string]::IsNullOrWhiteSpace($CoverageResultsPath)) {
    $CoverageResultsPath = Join-Path $repoRoot "Logs\coverage\editmode"
}

if ([string]::IsNullOrWhiteSpace($CoverageHistoryPath)) {
    $CoverageHistoryPath = Join-Path $repoRoot "Logs\coverage\history"
}

if ([string]::IsNullOrWhiteSpace($LogPath)) {
    $LogPath = Join-Path $repoRoot "Logs\coverage\editmode_coverage.log"
}

$directories = @(
    (Split-Path -Parent $TestResultsPath),
    $CoverageResultsPath,
    $CoverageHistoryPath,
    (Split-Path -Parent $LogPath)
) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | Select-Object -Unique

foreach ($directory in $directories) {
    if (-not (Test-Path $directory)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }
}

$coverageOptions = "generateHtmlReport;generateHtmlReportHistory;generateAdditionalReports;generateAdditionalMetrics;generateBadgeReport;verbosity:info"

# Unity Test Framework exits on completion. Passing -quit can stop batch mode before results are written.
& $UnityPath `
    -batchmode `
    -nographics `
    -projectPath $repoRoot `
    -runTests `
    -runSynchronously `
    -testPlatform EditMode `
    -testResults $TestResultsPath `
    -debugCodeOptimization `
    -enableCodeCoverage `
    -coverageResultsPath $CoverageResultsPath `
    -coverageHistoryPath $CoverageHistoryPath `
    -coverageOptions $coverageOptions `
    -logFile $LogPath

exit $LASTEXITCODE
