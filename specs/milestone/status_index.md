# Milestone Status Index

Current next implementation target: Milestone 097

This file is a navigation index only. Use `specs/00_overview/current_build_state.md` for current shipped behavior and individual milestone notes for full history/detail.

Folder split:
- numbered milestone notes and suffix follow-up notes live in `specs/milestone/`
- refactor notes live in `specs/milestone/refactors/`
- docs-only notes live in `specs/milestone/docs/`

## Legend
Type:
- `feature`: planned implementation milestone that changes delivered scope
- `follow-up`: small cleanup/fix/boundary pass attached to a recent milestone
- `refactor`: behavior-preserving structure/ownership cleanup
- `acceptance`: audit/verification checkpoint or repo-state validation note

Status:
- `completed`: note exists and the work is reflected in repo history
- `deferred`: intentionally left for a later milestone
- `replaced`: superseded by a later note

Current index note:
- The repo history indexed here is currently completion-only, so every listed entry is `completed`.

## Numbered Milestones
id | type | title | status | depends on / note
001 | acceptance | Verify project baseline | completed | phase 0
002 | feature | Align spec folder structure | completed | phase 0
003 | feature | Create core assembly/test structure | completed | phase 0
004 | feature | Create bootstrap scene | completed | phase 0
005 | feature | Create core identifiers and enums | completed | phase 1
006 | feature | Create base data containers | completed | phase 1
007 | feature | Create persistent game state model | completed | phase 1
008 | feature | Create game bootstrap flow | completed | phase 1
009 | feature | Implement world graph model | completed | phase 2
010 | feature | Implement node reachability rules | completed | phase 2
011 | feature | Implement basic world map screen | completed | phase 2
012 | feature | Implement world-to-node entry flow | completed | phase 2
013 | feature | Implement route choice support | completed | phase 2
014 | feature | Implement run lifecycle shell | completed | phase 3
015 | feature | Implement post-run state flow | completed | phase 3
016 | feature | Implement safe stopping/resume baseline | completed | phase 3
017 | feature | Implement session-context helpers | completed | phase 3
018 | feature | Create combat scene/state shell | completed | phase 4
019 | feature | Implement combat entity base model | completed | phase 4
020 | feature | Implement base stat model | completed | phase 4
021 | feature | Implement basic attack resolution | completed | phase 4
022 | feature | Implement enemy defeat and run resolve baseline | completed | phase 4
023 | feature | Implement basic auto-targeting | completed | phase 5
024 | feature | Implement basic auto-attack loop | completed | phase 5
025 | feature | Implement basic enemy hostility behavior | completed | phase 5
026 | acceptance | Implement movement if movement exists in MVP | completed | phase 5
027 | acceptance | Verify no-manual-combat core loop | completed | phase 5
028 | feature | Implement node progress meter | completed | phase 6
029 | feature | Implement map clear threshold rule | completed | phase 6
030 | feature | Implement next-node unlock rule | completed | phase 6
031 | acceptance | Implement partial completion value | completed | phase 6
032 | feature | Implement replayability of cleared nodes | completed | phase 6
033 | feature | Implement reward payload model | completed | phase 7
034 | feature | Implement basic soft currency | completed | phase 7
035 | feature | Implement one material category | completed | phase 7
036 | feature | Implement post-run reward summary UI | completed | phase 7
037 | feature | Differentiate ordinary vs milestone rewards | completed | phase 7
038 | feature | Implement one account-wide progression sink | completed | phase 8
039 | feature | Connect progression sink to combat outcomes | completed | phase 8
040 | feature | Implement one push-oriented upgrade | completed | phase 8
041 | feature | Implement one farm-oriented upgrade | completed | phase 8
042 | feature | Implement persistent character model | completed | phase 9
043 | feature | Implement character selection model placeholder | completed | phase 9
044 | feature | Implement character-linked progression | completed | phase 9
045 | feature | Implement second character only if needed | completed | phase 9
046 | feature | Implement baseline attack as skill-compatible system | completed | phase 10
047 | feature | Implement one passive skill layer | completed | phase 10
048 | feature | Implement one auto-triggered active skill | completed | phase 10
049 | feature | Implement build-facing skill selection or assignment | completed | phase 10
050 | feature | Implement run-time upgrade affecting skills | completed | phase 10
051 | feature | Implement persistent gear data model | completed | phase 11
052 | feature | Implement equipping gear before runs | completed | phase 11
053 | feature | Implement gear stat effect impact | completed | phase 11
054 | feature | Implement second gear slot category | completed | phase 11
055 | feature | Implement standard enemy data variety | completed | phase 12
056 | feature | Implement enemy profile differences in combat | completed | phase 12
057 | acceptance | Map enemy profiles to nodes/locations | completed | phase 12
058 | feature | Implement boss entity/encounter model | completed | phase 13
059 | feature | Connect boss to progression gate | completed | phase 13
060 | feature | Add boss reward differentiation | completed | phase 13
061 | feature | Implement town/service context shell | completed | phase 14
062 | feature | Connect progression sink to town/service layer | completed | phase 14
063 | feature | Connect gear/build preparation to town/service layer | completed | phase 14
064 | feature | Implement one project-style powerup mechanic | completed | phase 15
065 | feature | Implement one conversion/refinement mechanic | completed | phase 15
066 | feature | Connect materials to powerup mechanic | completed | phase 15
067 | feature | Implement two location identities in content data | completed | phase 16
068 | feature | Implement basic location-based enemy/resource differentiation | completed | phase 16
069 | feature | Implement old-location relevance rule in content | completed | phase 16
070 | feature | Implement readable world-state UI | completed | phase 17
071 | feature | Implement readable run HUD baseline | completed | phase 17
072 | feature | Implement upgrade choice UI | completed | phase 17
073 | feature | Improve post-run next-action clarity | completed | phase 17
074 | feature | Implement autosave at resolved post-run boundary | completed | phase 18
075 | feature | Persist build/character/gear state cleanly | completed | phase 18
076 | acceptance | Validate safe resume flow | completed | phase 18
077 | feature | Add minimal offline-progress compatibility hooks | completed | phase 18
078 | feature | Implement low-friction replay flow | completed | phase 19
079 | feature | Implement auto-pick baseline if desired | completed | phase 19
080 | feature | Mark or detect farm-ready content | completed | phase 19
081 | feature | Add one automation comfort upgrade | completed | phase 19
082 | feature | Implement gear-as-loot acquisition | completed | phase 20
083 | acceptance | Implement basic duplicate/conversion handling if needed | completed | phase 20
084 | feature | Implement milestone reward presentation polish | completed | phase 20
085 | feature | Expand boss readability and structure | completed | phase 21
086 | feature | Add one optional challenge/elite path if the core gate-boss works | completed | phase 21
087 | feature | Implement basic UI/system feedback sounds | completed | phase 22
088 | feature | Implement readable combat SFX baseline | completed | phase 22
089 | feature | Implement basic music context split | completed | phase 22
090 | feature | Implement milestone audio cues | completed | phase 22
091 | feature | Implement baseline player/enemy readability visuals | completed | phase 23
092 | feature | Implement two distinct location visual identities | completed | phase 23
093 | feature | Implement service/town visual contrast | completed | phase 23
094 | feature | Implement combat effect readability under autobattle | completed | phase 23
095 | feature | Implement compact main menu | completed | phase 24
096 | feature | Implement compact pause/system menu | completed | phase 24

## Follow-Ups And Acceptance Notes
id | type | title | status | depends on / note
013a | acceptance | Phase 0-2 cleanup and audit | completed | after 013
032b | follow-up | Implement broad cleared-node farm access | completed | after 032
038a | follow-up | Clarify account-wide progression purchase API | completed | after 038
039a | follow-up | Reduce run persistent context parameter growth | completed | after 039
042b | follow-up | Extract startup domain structure | completed | after 042
042c | follow-up | Move core types into core domain folders | completed | after 042
042d | follow-up | Reorganize EditMode tests into domain folders | completed | after 042
042e | follow-up | Align runtime and EditMode namespaces with folder structure | completed | after 042
042f | follow-up | Cleanup namespace alignment redundancy | completed | after 042
042g | follow-up | Remove test-only runtime accessors | completed | after 042
042h | follow-up | Remove redundant runtime namespace segment | completed | after 042
046a | follow-up | Fix world map placeholder scrolling | completed | after 046
046b | follow-up | Fix world map placeholder scroll view layout | completed | after 046
047a | follow-up | Harden passive skill model shape | completed | after 047
049a | follow-up | Deduplicate skill package ids | completed | after 049
049b | follow-up | Move skill package ids into character domain | completed | after 049
049c | follow-up | Restore world map scroll after package assignment | completed | after 049
050a | follow-up | Harden run-time skill upgrade shape | completed | after 050
052a | follow-up | Cleanup world map build summary and gear assignment shape | completed | after 052
056a | follow-up | Cleanup enemy content and profile ownership | completed | after 056
059a | follow-up | Cleanup boss progression gate shape | completed | after 059
061a | follow-up | Cleanup town/service shell routing and layout | completed | after 061
063a | follow-up | Cleanup town build interaction boundaries | completed | after 063
065a | follow-up | Fix town conversion persistence wiring | completed | after 065
067a | follow-up | Cleanup location identity integration and authored-data safety | completed | after 067
068a | follow-up | Cleanup location differentiation boundaries | completed | after 068
069a | follow-up | Harden old-location relevance authoring boundaries | completed | after 069
070a | follow-up | Cleanup world-map path-role boundaries | completed | after 070
072a | follow-up | Clean up run-choice presentation boundaries and world-map readability | completed | after 072
073a | follow-up | Cleanup post-run next-action recommendation boundaries | completed | after 073
073b | follow-up | Cleanup post-run next-action strict push targets | completed | after 073
077a | follow-up | Cleanup offline-progress compatibility boundaries | completed | after 077
078a | follow-up | Tighten low-friction replay offer boundary | completed | after 078
079a | follow-up | Cleanup auto-pick policy boundary | completed | after 079
081a | follow-up | Cleanup automation comfort upgrade boundaries | completed | after 081

## Refactor Milestones
id | type | title | status | depends on / note
refactor01 | refactor | Package structure and test alignment | completed | runtime/test ownership
refactor02 | refactor | Shared UI support boundary | completed | shared UI boundary
refactor03 | refactor | Characters runtime domain split | completed | characters runtime split
refactor04 | refactor | Progression authored-data split | completed | progression authored-data split
refactor05 | refactor | Dormant authoring-model cleanup | completed | prototype cleanup
refactor06 | refactor | Reward authored-data extraction | completed | reward authored-data split
refactor06b | refactor | WorldMapScreen decomposition | completed | world-map screen split
refactor07 | refactor | Shared enum/display-name formatting | completed | shared formatting cleanup

## Docs / Workflow Notes
id | type | title | status | depends on / note
docs01 | follow-up | Milestone navigation and workflow cleanup | completed | docs-only navigation aid
docs02 | follow-up | Milestone note normalization and index cleanup | completed | docs-only normalization pass
docs04 | follow-up | Separate refactor and docs notes from milestone history | completed | docs-only organization cleanup
