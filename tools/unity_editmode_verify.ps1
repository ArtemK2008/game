param(
    [string]$UnityPath = "$env:ProgramFiles\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe",
    [string]$ProjectPath = "C:\IT_related\myGame\Survivalon",
    [string]$CompileLogPath = "C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log",
    [string]$ResultsPath = "C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml",
    [string]$TestLogPath = "C:\IT_related\myGame\Survivalon\Logs\editmode.log"
)

& "$PSScriptRoot\unity_compile_check.ps1" `
    -UnityPath $UnityPath `
    -ProjectPath $ProjectPath `
    -LogPath $CompileLogPath

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

$resultsDirectory = Split-Path -Path $ResultsPath -Parent
if (-not (Test-Path $resultsDirectory)) {
    New-Item -ItemType Directory -Path $resultsDirectory -Force | Out-Null
}

if (Test-Path $ResultsPath) {
    Remove-Item $ResultsPath -Force
}

if (Test-Path $TestLogPath) {
    Remove-Item $TestLogPath -Force
}

$arguments = @(
    '-batchmode',
    '-nographics',
    '-projectPath', $ProjectPath,
    '-runTests',
    '-runSynchronously',
    '-testPlatform', 'EditMode',
    '-testResults', $ResultsPath,
    '-logFile', $TestLogPath,
    '-quit'
)

$process = Start-Process `
    -FilePath $UnityPath `
    -ArgumentList $arguments `
    -Wait `
    -NoNewWindow `
    -PassThru

if ($process.ExitCode -ne 0) {
    Write-Error "Unity EditMode run exited with code $($process.ExitCode). See log: $TestLogPath"
    exit 20
}

if (-not (Test-Path $ResultsPath)) {
    Write-Error "EditMode results file was not created: $ResultsPath"
    exit 3
}

Write-Host "EditMode verification finished."
Write-Host "Results: $ResultsPath"
Write-Host "Log: $TestLogPath"
exit 0