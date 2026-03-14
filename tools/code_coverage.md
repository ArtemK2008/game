# EditMode Coverage

Use the Unity Code Coverage package through:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\run_editmode_coverage.ps1
```

Default outputs:

- test results XML: `Logs/coverage/editmode_test_results.xml`
- coverage results/report root: `Logs/coverage/editmode/`
- coverage history: `Logs/coverage/history/`
- Unity log: `Logs/coverage/editmode_coverage.log`

Open the HTML report from:

- `Logs/coverage/editmode/Report/index.htm`

The script uses the Unity editor version from `ProjectSettings/ProjectVersion.txt` and enables:

- HTML report
- HTML history
- additional reports
- additional metrics
- badge report
