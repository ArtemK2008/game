# Milestone 042d - Reorganize EditMode Tests Into Domain Folders

## Delivered
- Created the remaining obvious EditMode domain folders:
  - `Assets/Tests/EditMode/Combat/`
  - `Assets/Tests/EditMode/Run/`
  - `Assets/Tests/EditMode/World/`
  - `Assets/Tests/EditMode/State/`
  - `Assets/Tests/EditMode/State/Persistence/`
  - `Assets/Tests/EditMode/Data/`
- Moved the remaining obvious EditMode tests into those folders so test placement now mirrors runtime ownership much more closely.
- Kept this milestone folder-only:
  - no gameplay changes
  - no namespace-alignment refactor
  - no asmdef rewrite

## Files Moved
- `Combat/`
  - `CombatAutoAdvanceLoopTests`
  - `CombatAutoTargetSelectorTests`
  - `CombatEncounterResolverTests`
  - `CombatEntityStateTests`
  - `CombatShellPresentationTests`
  - `CombatStatCalculatorTests`
- `Run/`
  - `PostRunStateControllerTests`
  - `RunLifecycleControllerTests`
  - `RunLifecycleControllerCombatTests`
  - `RunLifecycleControllerProgressionTests`
  - `RunLifecycleControllerTestData`
  - `RunProgressResolutionServiceTests`
  - `RunResultFactoryTests`
  - `RunRewardGrantServiceTests`
  - `RunRewardPayloadTests`
  - `RunRewardResolutionServiceTests`
- `World/`
  - `BootstrapWorldGraphBuilderTests`
  - `BootstrapWorldStateSeederTests`
  - `BootstrapWorldTestData`
  - `NextNodeUnlockServiceTests`
  - `NodePlaceholderScreenCombatUiTests`
  - `NodePlaceholderScreenPresentationTests`
  - `NodePlaceholderScreenUiTestBase`
  - `NodePlaceholderScreenUiTests`
  - `NodePlaceholderTestData`
  - `NodeReachabilityResolverTests`
  - `PostRunSummaryTextBuilderTests`
  - `RouteChoiceSupportTests`
  - `WorldFlowTestData`
  - `WorldGraphReachabilityTests`
  - `WorldGraphValidationTests`
  - `WorldMapScreenControllerTests`
  - `WorldMapScreenPresentationTests`
  - `WorldMapScreenUiSetupTests`
  - `WorldNodeAccessResolverTests`
  - `WorldNodeEntryFlowControllerTests`
  - `WorldNodeStateResolverTests`
- `State/Persistence/`
  - `AccountWideProgressionBoardServiceTests`
  - `AccountWideProgressionEffectResolverTests`
  - `NodeProgressMeterServiceTests`
  - `PersistentNodeStateFactoryTests`
  - `PersistentPlayableCharacterInitializerTests`
  - `PersistentStateModelTests`
  - `PersistentStateTestData`
  - `SafeResumePersistenceServiceTests`
  - `TrackedNodeProgressRulesTests`
- `State/`
  - `SessionContextStateTests`
- `Data/`
  - `BaseDataContainerTests`
  - `PlayableCharacterResolverTests`

## Why This Improves Readability
- Runtime ownership and test ownership now line up much more clearly.
- A developer can navigate from a runtime domain folder to the matching test folder without scanning a large flat EditMode directory.
- The remaining flat-root EditMode files are now the smaller set of intentionally deferred exceptions instead of the default layout.

## Intentionally Left Unchanged
- Test namespaces
- Runtime namespaces
- `RuntimeAssemblySmokeTests`
- Any remaining ambiguous test placement that spans multiple domains
- Any broader runtime-root cleanup beyond the already-completed `Core` and `Startup` extraction milestones

## Verification
- Unity EditMode batch run passed.
- Result: `288` passed, `0` failed.
- Artifacts:
  - `Logs/editmode_test_domain_reorg_results.xml`
  - `Logs/editmode_test_domain_reorg.log`
