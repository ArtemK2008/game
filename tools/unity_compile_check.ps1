param(
    [string]$UnityPath = "$env:ProgramFiles\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe",
    [string]$ProjectPath = "C:\IT_related\myGame\Survivalon",
    [string]$LogPath = "C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log"
)

$arguments = @(
    '-batchmode',
    '-nographics',
    '-quit',
    '-projectPath', $ProjectPath,
    '-accept-apiupdate',
    '-ignorecompilererrors',
    '-logFile', $LogPath
)

Start-Process -FilePath $UnityPath -ArgumentList $arguments -Wait -NoNewWindow

if (-not (Test-Path $LogPath)) {
    Write-Error "Compile log was not created: $LogPath"
    exit 2
}

$logText = Get-Content -Path $LogPath -Raw

# Keep this simple and biased toward catching real compiler failures.
$hasCompilerErrors =
    $logText -match 'error CS\d+' -or
    $logText -match 'Scripts have compiler errors' -or
    $logText -match 'error[s]?\s*\n' -or
    $logText -match 'Compilation failed'

if ($hasCompilerErrors) {
    Write-Host "Unity compile/import pass found compiler errors. See log:"
    Write-Host $LogPath
    exit 1
}

Write-Host "Unity compile/import pass completed without detected compiler errors."
Write-Host $LogPath
exit 0