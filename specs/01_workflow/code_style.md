# Code Style

## Purpose
Define the engineering and code-style rules for the Unity project and establish the implementation constraints that all generated or written code must follow.

---

## Scope
This spec defines:
- general engineering principles
- Unity / C# code style rules
- architecture and design expectations
- naming rules
- testing expectations
- refactoring expectations
- MVP vs future-proofing balance
- constraints for all implementation work

This spec does not define:
- exact folder structure
- exact Unity package list
- exact CI/CD pipeline
- exact assembly definition layout
- exact static-analysis tool setup

---

## Core statement
Code must be written for long-term maintainability, safe iteration, and clear reasoning.

The codebase must favor:
- readability over cleverness
- simple designs over speculative abstraction
- explicit intent over hidden behavior
- small safe increments over large rewrites
- strong test coverage around implemented behavior

Primary engineering principles:
- **SOLID**
- **KISS**
- **YAGNI**
- **Clean Code**

---

## Core implementation rule
Default engineering flow:

`implement the smallest correct solution for current requirements -> keep responsibilities clear -> name things explicitly -> cover behavior with tests -> refactor only when it improves clarity without adding speculative complexity`

---

## General engineering principles
- Prefer simple solutions that are easy to read and debug.
- Do not add abstractions before there is a real need.
- Optimize first for correctness and clarity, then for extensibility where it is clearly justified.
- Keep behavior easy to trace from entry point to result.
- Avoid hidden side effects.
- Keep code modular, but do not fragment it into meaningless layers.
- Favor composition over deep inheritance unless inheritance is clearly justified.
- Keep the implementation aligned with current specs, not imagined future features.
- Prefer Unity-friendly architecture that separates scene wiring from gameplay logic.

---

## SOLID application rules
### Single Responsibility Principle
- Each class should have one clear reason to change.
- Do not mix unrelated responsibilities in one class.
- Separate gameplay logic, orchestration, scene wiring, persistence, UI, and asset hookup concerns.

### Open/Closed Principle
- Prefer extension through clear interfaces, strategies, ScriptableObject-driven configuration, or composition when variation is already known.
- Do not introduce abstraction layers only because variation might exist later.

### Liskov Substitution Principle
- Subtypes must behave consistently with their contract.
- Do not create inheritance hierarchies where child behavior breaks parent assumptions.

### Interface Segregation Principle
- Prefer small focused interfaces over large “god interfaces”.
- Do not force classes to implement methods they do not meaningfully support.

### Dependency Inversion Principle
- High-level gameplay logic should depend on clear abstractions where appropriate.
- Use abstractions at meaningful boundaries, not everywhere by default.

---

## KISS rules
- Prefer straightforward control flow.
- Prefer explicit code over overly generic helpers.
- Prefer small understandable methods over dense multi-purpose methods.
- Prefer a few clear gameplay concepts over many micro-abstractions.
- Avoid advanced patterns unless they clearly simplify the current problem.
- Prefer simple Unity scene composition over complicated runtime object-graph magic.

If two designs solve the same problem, prefer the one that is easier to explain to another developer.

---

## YAGNI rules
- Do not implement unused extension points.
- Do not create classes/interfaces “just in case”.
- Do not build generalized systems for features that currently have one use case.
- Do not add settings, flags, or modes that are not required by the current milestone/spec.
- Do not add custom editor tooling unless it clearly improves the current workflow.
- Do not add speculative optimization before profiling or a demonstrated need.

Future compatibility is good. Speculative architecture is not.

---

## Clean Code rules
- Code should read like a clear explanation of intent.
- Every method/class should communicate why it exists.
- Avoid long methods when extraction improves clarity.
- Avoid deep nesting when early return or better decomposition helps.
- Keep conditionals readable and intention-revealing.
- Reduce duplication when the duplication is real and stable, not accidental and premature.
- Prefer domain language from specs in code naming.
- Avoid “magic” behavior hidden in utility classes or MonoBehaviours.

---

## Unity / C# specific rules
- Use C# features only when they improve readability.
- Prefer explicit types when they improve understanding.
- Avoid overusing `var` if it makes the code harder to read.
- Favor immutable data where practical.
- Keep mutable gameplay state controlled and localized.
- Prefer enums for stable gameplay categories.
- Use ScriptableObjects for static/configurable data when they improve data-driven design.
- Do not put heavy gameplay logic directly into MonoBehaviour lifecycle methods if it can live in plain C# classes.
- Keep MonoBehaviours thin when possible: wiring, references, lifecycle bridging, and view updates.
- Put reusable gameplay rules/calculations into plain C# classes/services rather than scene-bound components when appropriate.
- Use Unity events/callbacks carefully; avoid hidden execution chains.
- Keep serialization needs explicit.
- Avoid unnecessary runtime allocations in hot update paths when the path is frequent enough to matter.
- Avoid using `Find`, `GetComponent` repeatedly inside hot loops when references can be cached safely.
- Avoid singletons unless the lifetime/global ownership is truly clear and justified.

---

## Class design rules
- Each class must have a clear role.
- Avoid “god classes”.
- Avoid anemic pass-through classes that add no value.
- Service classes should orchestrate, not own all domain logic.
- Domain logic should live close to the domain it describes.
- Utility classes should be rare and focused.
- MonoBehaviours should not become giant all-purpose controllers.
- Presenter/view/controller-like scene components should not hide important gameplay rules.
- ScriptableObjects should define data/configuration, not become dumping grounds for arbitrary runtime state.

---

## Method design rules
- Methods should do one clear thing.
- Method names must describe intent, not implementation detail.
- Avoid methods with too many arguments.
- If a method needs many parameters, consider whether a small domain object or request object is clearer.
- Keep method side effects obvious.
- Prefer early returns to reduce nesting when it improves clarity.
- Do not hide important state changes inside generic helper methods.

---

## Naming rules
### General naming principles
- Use meaningful names.
- Names must explain gameplay/domain purpose.
- Avoid vague names like `data`, `value`, `item`, `stuff`, `helper`, `manager`, `controller` unless they are truly the best domain term.
- Avoid one-letter names except for trivial loop indexes in very small scopes.
- Prefer full words over abbreviations unless the abbreviation is a stable domain term.
- Use names from the project specs/domain language when possible.

### Variables
- Variable names must be meaningful and specific.
- A variable should tell the reader what it holds and why it exists.
- Prefer names like `currentNodeProgress`, `activeEnemyCount`, `selectedCharacterData`, `pendingRewardSummary` over vague names like `data`, `result`, `tmp`, `val`.

### Methods
- Use verb-based names.
- Method names should describe the action and gameplay meaning.
- Examples: `CalculateNodeProgress`, `UnlockNextNode`, `GrantRunRewards`, `ResolveCombatTick`, `LoadAvailableWorldNodes`.

### Classes
- Use noun-based names.
- A class name should describe the concept or responsibility.
- Examples: `NodeProgressService`, `RunRewardCalculator`, `WorldStateRepository`, `AutoCombatController`.

### Booleans
- Boolean names should read clearly as true/false statements.
- Prefer names like `IsCleared`, `IsAvailable`, `HasUnlockedBranch`, `CanAutoPickUpgrades`.

### Unity assets/scripts
- Script names must match class names.
- ScriptableObject asset types should have names that clearly describe the data they hold.
- Prefab-related scripts should describe role, not only object type.

---

## Folder/module rules
- Group code by gameplay domain or feature when practical, not only by technical layer.
- Keep folder names predictable and stable.
- Avoid dumping unrelated logic into shared/common folders.
- Keep scenes, prefabs, sprites, audio, ScriptableObjects, and scripts organized consistently.
- Shared modules should only exist when code is genuinely reused and conceptually shared.

---

## Comments and documentation rules
- Prefer self-explanatory code over excessive comments.
- Add comments only when they explain **why**, not what obvious code already shows.
- Do not leave stale or misleading comments.
- Avoid noise comments.
- Public APIs and important gameplay boundaries may use concise documentation where helpful.
- If a rule is important enough to require repeated explanation, consider improving naming or decomposition first.

---

## Error-handling rules
- Handle errors explicitly.
- Error messages should be useful and specific.
- Do not swallow exceptions silently.
- Do not catch broad exceptions unless there is a clear recovery or translation purpose.
- Validation failures and impossible states should be distinguishable.
- Fail fast in development when required scene/config references are missing.
- Prefer explicit guard clauses for invalid setup and missing dependencies.

---

## State and side-effect rules
- Keep mutable state as small and localized as possible.
- Make important state transitions explicit.
- Avoid hidden global state.
- Avoid side effects inside code that appears to be pure.
- If a method mutates state, the method contract should make that obvious.
- Avoid mixing transient runtime state with persistent save state in the same object unless the responsibility is truly unified.

---

## Logging and debugging rules
- Log meaningful events and failures.
- Logs should help trace important flow, not flood with noise.
- Do not log every frame/update path unless specifically debugging.
- Prefer structured, contextual logs where possible.
- Log gameplay-significant transitions, failures, and unexpected states.
- Remove temporary debug spam when the milestone is complete unless the logging remains valuable.

---

## Testing rules
Testing is required.

### Core testing rule
Every milestone must include tests for the behavior introduced or changed in that milestone.

### Required testing principles
- Write tests for behavior, not implementation details.
- Tests must prove correctness of the current milestone.
- Tests must be readable and intention-revealing.
- Tests must have deterministic expectations.
- New behavior must not be considered complete without tests, unless explicitly impossible or out of scope.
- Bug fixes should include regression tests when practical.

### Unit test expectations
- Each important rule/service/calculation should have direct unit test coverage.
- Prefer focused unit tests over huge scene/integration-style tests for core logic.
- Test names should explain the scenario and expected result.
- Arrange/Act/Assert structure is preferred when it improves readability.
- Keep gameplay logic testable outside MonoBehaviour where possible.

### Testing priorities
Focus tests on:
- progression rules
- combat calculations
- reward generation
- unlock logic
- automation decisions if deterministic
- persistence boundary behavior
- bug-prone branching logic

### Optional Unity-specific testing
Use play mode/integration tests only when scene interaction, prefab wiring, or engine behavior genuinely needs them.
Do not push ordinary gameplay logic into play mode tests if plain C# unit tests are enough.

### Test naming examples
- `ShouldUnlockNextNodeWhenProgressReachesThreshold`
- `ShouldGrantPartialProgressWhenRunFailsBeforeClear`
- `ShouldReturnOnlyReachableNodesForCurrentWorldState`

### Testing anti-rules
- Do not write fragile tests tied to incidental implementation details.
- Do not skip tests for milestone behavior because “it is simple”.
- Do not create overly broad tests that make failures hard to diagnose.
- Do not duplicate the same coverage shape across many tests without added value.

---

## Refactoring rules
- Refactor to improve readability, naming, structure, or duplication when the change has clear value.
- Do not refactor unrelated areas during a milestone unless necessary for correctness or maintainability.
- Prefer small safe refactorings.
- Preserve behavior unless the task explicitly changes behavior.
- When refactoring, keep or improve test coverage.

Good reasons to refactor:
- class has multiple responsibilities
- method is hard to read
- naming hides intent
- duplication is stable and clearly harmful
- testability is poor because structure is unclear
- MonoBehaviour is carrying too much logic that should live elsewhere

Bad reasons to refactor:
- speculative future design
- style churn with no clarity gain
- pattern introduction for its own sake

---

## Dependency and abstraction rules
- Introduce interfaces at meaningful boundaries.
- Do not create interfaces for every class by default.
- Prefer concrete implementations until variation/testing/design clearly benefit from abstraction.
- Keep dependency graphs simple.
- Avoid cyclic dependencies.
- Prefer constructor injection for plain C# classes where dependency injection is used.
- For MonoBehaviours, prefer explicit serialized references or clear composition over hidden service-location patterns.

---

## Complexity control rules
- Keep branching logic understandable.
- Prefer explicit domain models over encoded primitive combinations when complexity grows.
- Split large flows into named steps when that improves traceability.
- Avoid boolean-flag explosions in public APIs.
- Avoid deeply nested conditionals when a clearer model exists.
- Avoid scattering gameplay rules across many lifecycle callbacks without a clear reason.

---

## Data structure rules
- Choose data structures for clarity first, then efficiency when needed.
- Prefer domain objects over loosely structured dictionaries when behavior matters.
- Use collections intentionally and validate assumptions about emptiness/order/uniqueness when relevant.
- Do not expose mutable internal collections without reason.
- Use ScriptableObject data assets or serializable DTO/config classes when they improve data clarity.

---

## API and boundary rules
- Public methods/classes should expose clear contracts.
- Input validation should happen at appropriate boundaries.
- Boundary code should translate Unity scene/input/data into gameplay-friendly structures.
- Keep scene/view/controller layer thin if applicable.
- Keep persistence layer from leaking directly into higher-level gameplay logic without purpose.

---

## MVP implementation rules
For early milestones, optimize for:
- correctness
- readability
- testability
- low conceptual overhead
- easy iteration
- clear Unity integration points

Do not optimize early for:
- speculative extensibility
- framework cleverness
- generic engines
- advanced optimization without evidence
- over-engineered custom tooling

The first implementation of a feature should be the smallest clean version that satisfies the current spec.

---

## Asset-code boundary rules
- Keep asset references explicit.
- Do not hardcode asset paths in gameplay logic unless there is no better short-term option.
- Separate gameplay data from visual/audio asset hookup where practical.
- Missing required assets should fail clearly, not silently.
- If a milestone depends on missing sprites/audio/animations, request them rather than faking invisible final content.

---

## Consistency rules
- Similar problems should be solved in similar ways.
- Naming, test style, error handling, and structure should remain consistent across the codebase.
- When introducing a new pattern, ensure it is actually better than the existing local pattern.
- Prefer consistency with project rules over personal style preferences.

---

## Constraints for all future implementation
- Do not sacrifice readability for cleverness.
- Do not introduce speculative abstractions without real need.
- Do not leave milestone behavior without tests.
- Do not use vague variable names.
- Do not create god classes or giant methods.
- Do not mix unrelated responsibilities.
- Do not hide important gameplay rules in generic utility code.
- Do not refactor broadly without need.
- Do not bypass the spec language with unclear technical naming.
- Do not push too much gameplay logic into MonoBehaviour lifecycle code when it can live in testable classes.

---

## Extension points
The engineering style must remain compatible with later addition of:
- stricter assembly/module boundaries
- static analysis rules
- architectural tests
- integration/play mode test suites
- contract tests
- performance profiling/optimization rules
- code-generation helpers where justified
- custom editor tooling where justified

These additions must strengthen clarity and maintainability, not replace them.

---

## Out of scope
This spec does not define:
- exact formatter configuration
- exact linter/analyzer ruleset
- exact Unity package selection
- exact assembly definition topology
- exact CI gating rules

---

## Validation checklist
- [ ] Code uses meaningful names for classes, methods, variables, and booleans.
- [ ] Code follows SOLID where it improves clarity and maintainability.
- [ ] Code follows KISS and avoids unnecessary complexity.
- [ ] Code follows YAGNI and avoids speculative abstractions.
- [ ] Core behavior is covered by unit tests for each milestone.
- [ ] Refactorings i