# Unity Verification Notes

## Mandatory Order
1. Run compile/import first:
   `pwsh -NoLogo -NoProfile -File tools/unity_compile_check.ps1`
2. Only if compile/import is clean, run EditMode verification:
   `pwsh -NoLogo -NoProfile -File tools/unity_editmode_verify.ps1`
3. If the helper verification step fails to emit its expected artifact, rerun verification with a direct waited Unity batch EditMode run and write milestone-specific result/log files.

## Workflow Caveat
- The helper verification script can detach or complete without producing its expected XML artifact in some shell/environment runs.
- Treat that as a workflow caveat, not as evidence that tests passed and not as a product feature.
- Do not modify tooling scripts just to work around this note; use the documented fallback verification path instead.

## Reporting Rule
- Always report the exact compile log path used.
- Always report the exact EditMode result/log paths used, including fallback artifact names.
- Never claim tests passed if compile/import failed.
- Keep milestone verification notes practical: state the command path used, the final result, and the artifact paths.
