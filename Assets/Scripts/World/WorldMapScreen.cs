using System;
using System.Collections.Generic;
using Survivalon.Characters;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.State;
using Survivalon.State.Persistence;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.World
{
    /// <summary>
    /// Показывает placeholder-карту мира и оркестрирует локальные UI-секции без бизнес-логики доступа.
    /// </summary>
    public sealed class WorldMapScreen : MonoBehaviour
    {
        private Canvas canvas;
        private RectTransform panelRectTransform;
        private RectTransform sidebarRectTransform;
        private WorldMapOverviewSectionView overviewSectionView;
        private WorldMapBuildSectionView buildSectionView;
        private WorldMapEntryActionSectionView entryActionSectionView;
        private WorldMapSurfaceSectionView surfaceSectionView;
        private WorldMapScreenController screenController;
        private WorldMapArtResolver artResolver;
        private WorldMapNodeLayoutResolver nodeLayoutResolver;
        private PlayableCharacterWorldIconResolver characterWorldIconResolver;
        private Action<NodeId> onNodeEntryRequested;
        private Action onStopSessionRequested;
        private PersistentGameState gameState;
        private PlayableCharacterSelectionService characterSelectionService;
        private PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService;
        private PlayableCharacterGearAssignmentService gearAssignmentService;
        private WorldMapBuildPreparationInteractionService buildPreparationInteractionService;
        private Action<UiSystemFeedbackSoundId> onFeedbackSoundRequested;
        private Button systemMenuButton;
        private CompactSystemMenuView systemMenuView;
        private UserSettingsState currentSettingsState;
        private Action<UserSettingsState> onSettingsChanged;
        private bool hasQuickRepeatNode;
        private NodeId quickRepeatNodeId;
        private string quickRepeatNodeDisplayName;
        private NodeType quickRepeatNodeType;

        public void Show(
            WorldGraph worldGraph,
            PersistentWorldState worldState,
            Action<NodeId> nodeEntryRequested = null,
            SessionContextState sessionContext = null,
            PersistentGameState gameState = null,
            AccountWideProgressionEffectState progressionEffects = default,
            WorldMapBuildPreparationInteractionService buildPreparationInteractionService = null,
            Action<UiSystemFeedbackSoundId> feedbackSoundRequested = null,
            Action stopSessionRequested = null,
            UserSettingsState settingsState = null,
            Action<UserSettingsState> settingsChanged = null)
        {
            if (worldGraph == null)
            {
                throw new ArgumentNullException(nameof(worldGraph));
            }

            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            screenController = new WorldMapScreenController(
                worldGraph,
                worldState,
                sessionContext: sessionContext,
                progressionEffects: progressionEffects);
            onNodeEntryRequested = nodeEntryRequested;
            onStopSessionRequested = stopSessionRequested;
            onFeedbackSoundRequested = feedbackSoundRequested;
            currentSettingsState = (settingsState ?? UserSettingsState.CreateDefault()).Sanitize();
            onSettingsChanged = settingsChanged;
            this.gameState = gameState;
            characterSelectionService = gameState == null ? null : new PlayableCharacterSelectionService();
            skillPackageAssignmentService = gameState == null
                ? null
                : new PlayableCharacterSkillPackageAssignmentService(characterSelectionService);
            gearAssignmentService = gameState == null
                ? null
                : new PlayableCharacterGearAssignmentService(characterSelectionService);
            this.buildPreparationInteractionService = buildPreparationInteractionService ?? (
                gameState == null
                    ? null
                    : new WorldMapBuildPreparationInteractionService(
                        characterSelectionService: characterSelectionService,
                        skillPackageAssignmentService: skillPackageAssignmentService,
                        gearAssignmentService: gearAssignmentService));
            gameObject.name = "WorldMapScreen";
            artResolver ??= new WorldMapArtResolver();
            nodeLayoutResolver ??= new WorldMapNodeLayoutResolver();
            characterWorldIconResolver ??= new PlayableCharacterWorldIconResolver();

            RuntimeUiSupport.EnsureInputSystemEventSystem();
            EnsureUi();
            Refresh();
        }

        private void Refresh()
        {
            WorldMapWorldStateSummary worldStateSummary = screenController.BuildWorldStateSummary();
            IReadOnlyList<WorldMapNodeOption> nodeOptions = screenController.BuildNodeOptions();
            WorldMapSurfaceLayout surfaceLayout = nodeLayoutResolver.Resolve(nodeOptions);

            overviewSectionView.Refresh(
                "World Map",
                WorldMapScreenTextBuilder.BuildSummaryText(
                    worldStateSummary,
                    screenController.ResolveSelectedNodeDisplayName()));
            RefreshCharacterSelection();
            RefreshBuildAssignment();
            RefreshEntryButton();
            surfaceSectionView.Refresh(
                nodeOptions,
                screenController.WorldGraph.Connections,
                artResolver,
                surfaceLayout,
                HandleNodeSelection);
            RefreshLayout();
        }

        private void HandleNodeSelection(NodeId nodeId)
        {
            if (!screenController.TrySelectNode(nodeId))
            {
                onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiError);
                return;
            }

            onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiClick);
            Debug.Log($"World map node selected: {nodeId}.");
            Refresh();
        }

        private void HandleCharacterSelection(string characterId)
        {
            if (buildPreparationInteractionService == null || gameState == null)
            {
                return;
            }

            if (!buildPreparationInteractionService.TrySelectCharacter(gameState, characterId))
            {
                onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiError);
                return;
            }

            onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiClick);
            Debug.Log($"World map character selected: {characterId}.");
            Refresh();
        }

        private void HandleSkillPackageAssignment(string skillPackageId)
        {
            if (buildPreparationInteractionService == null || gameState == null)
            {
                return;
            }

            if (!buildPreparationInteractionService.TryAssignSkillPackage(gameState, skillPackageId))
            {
                onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiError);
                return;
            }

            onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiConfirm);
            Debug.Log($"World map skill package assigned: {skillPackageId}.");
            Refresh();
        }

        private void HandleGearAssignment(PlayableCharacterGearAssignmentOption gearAssignmentOption)
        {
            if (gearAssignmentOption == null || buildPreparationInteractionService == null || gameState == null)
            {
                return;
            }

            bool changed = gearAssignmentOption.IsEquipped
                ? buildPreparationInteractionService.TryClearGear(gameState, gearAssignmentOption.GearCategory)
                : buildPreparationInteractionService.TryAssignGear(gameState, gearAssignmentOption.GearId);
            if (!changed)
            {
                onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiError);
                return;
            }

            onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiConfirm);
            Debug.Log($"World map gear assignment changed for {gearAssignmentOption.CharacterId}.");
            Refresh();
        }

        private void HandleNodeEntryRequest()
        {
            if (onNodeEntryRequested == null)
            {
                onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiError);
                return;
            }

            if (screenController.TryGetSelectedNodeId(out NodeId selectedNodeId))
            {
                onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiConfirm);
                screenController.SessionContext?.ConsumeReturnToWorldReentryOffer();
                onNodeEntryRequested(selectedNodeId);
                return;
            }

            if (!hasQuickRepeatNode)
            {
                onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiError);
                return;
            }

            onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiConfirm);
            screenController.SessionContext?.ConsumeReturnToWorldReentryOffer();
            onNodeEntryRequested(quickRepeatNodeId);
        }

        private void EnsureUi()
        {
            if (canvas != null)
            {
                return;
            }

            Font uiFont = RuntimeUiSupport.LoadFallbackFont(nameof(WorldMapScreen));

            RectTransform rootRectTransform = RuntimeUiSupport.GetOrAddComponent<RectTransform>(gameObject);
            canvas = RuntimeUiSupport.GetOrAddComponent<Canvas>(gameObject);
            CanvasScaler canvasScaler = RuntimeUiSupport.GetOrAddComponent<CanvasScaler>(gameObject);
            RuntimeUiSupport.GetOrAddComponent<GraphicRaycaster>(gameObject);

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1280f, 720f);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;

            rootRectTransform.anchorMin = Vector2.zero;
            rootRectTransform.anchorMax = Vector2.one;
            rootRectTransform.offsetMin = Vector2.zero;
            rootRectTransform.offsetMax = Vector2.zero;
            rootRectTransform.pivot = new Vector2(0.5f, 0.5f);
            rootRectTransform.localScale = Vector3.one;

            GameObject panelObject = new GameObject(
                "Panel",
                typeof(RectTransform),
                typeof(Image),
                typeof(HorizontalLayoutGroup));
            panelObject.transform.SetParent(transform, false);

            Image panelImage = panelObject.GetComponent<Image>();
            panelImage.color = new Color(0.02f, 0.03f, 0.04f, 0.22f);

            panelRectTransform = panelObject.GetComponent<RectTransform>();
            panelRectTransform.anchorMin = Vector2.zero;
            panelRectTransform.anchorMax = Vector2.one;
            panelRectTransform.offsetMin = new Vector2(20f, 20f);
            panelRectTransform.offsetMax = new Vector2(-20f, -20f);
            panelRectTransform.localScale = Vector3.one;

            HorizontalLayoutGroup panelLayout = panelObject.GetComponent<HorizontalLayoutGroup>();
            panelLayout.padding = new RectOffset(14, 14, 14, 14);
            panelLayout.spacing = 14f;
            panelLayout.childAlignment = TextAnchor.UpperLeft;
            panelLayout.childControlWidth = true;
            panelLayout.childControlHeight = true;
            panelLayout.childForceExpandWidth = false;
            panelLayout.childForceExpandHeight = true;

            surfaceSectionView = WorldMapSurfaceSectionView.Create(panelObject.transform, uiFont);

            GameObject sidebarObject = new GameObject(
                "Sidebar",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup),
                typeof(LayoutElement));
            sidebarObject.transform.SetParent(panelObject.transform, false);

            sidebarRectTransform = sidebarObject.GetComponent<RectTransform>();
            sidebarRectTransform.localScale = Vector3.one;

            Image sidebarImage = sidebarObject.GetComponent<Image>();
            sidebarImage.color = new Color(0.05f, 0.07f, 0.09f, 0.88f);

            LayoutElement sidebarLayoutElement = sidebarObject.GetComponent<LayoutElement>();
            sidebarLayoutElement.preferredWidth = 276f;
            sidebarLayoutElement.minWidth = 248f;
            sidebarLayoutElement.flexibleHeight = 1f;

            VerticalLayoutGroup sidebarLayout = sidebarObject.GetComponent<VerticalLayoutGroup>();
            sidebarLayout.padding = new RectOffset(14, 14, 14, 14);
            sidebarLayout.spacing = 8f;
            sidebarLayout.childAlignment = TextAnchor.UpperLeft;
            sidebarLayout.childControlWidth = true;
            sidebarLayout.childControlHeight = true;
            sidebarLayout.childForceExpandWidth = true;
            sidebarLayout.childForceExpandHeight = false;

            overviewSectionView = WorldMapOverviewSectionView.Create(sidebarObject.transform, uiFont);
            entryActionSectionView = WorldMapEntryActionSectionView.Create(
                sidebarObject.transform,
                uiFont,
                HandleNodeEntryRequest);
            buildSectionView = WorldMapBuildSectionView.Create(sidebarObject.transform, uiFont);
            systemMenuButton = CompactSystemMenuUiFactory.CreateSystemMenuButton(
                transform,
                uiFont,
                HandleSystemMenuRequested);
            systemMenuView = CompactSystemMenuUiFactory.CreateSystemMenuView(transform);
        }

        private void RefreshCharacterSelection()
        {
            IReadOnlyList<PlayableCharacterSelectionOption> selectionOptions = BuildCharacterSelectionOptions();

            buildSectionView.RefreshCharacterSelection(
                WorldMapScreenTextBuilder.BuildCharacterSelectionText(selectionOptions),
                selectionOptions,
                ResolveCharacterSelectionWorldIcons(selectionOptions),
                HandleCharacterSelection);
        }

        private void RefreshBuildAssignment()
        {
            IReadOnlyList<PlayableCharacterSkillPackageOption> skillPackageOptions = BuildSkillPackageOptions();
            IReadOnlyList<PlayableCharacterGearAssignmentOption> gearAssignmentOptions = BuildGearAssignmentOptions();

            buildSectionView.RefreshBuildAssignment(
                WorldMapScreenTextBuilder.BuildAssignmentText(
                    BuildSelectedCharacterDisplayName(),
                    skillPackageOptions,
                    gearAssignmentOptions),
                skillPackageOptions,
                HandleSkillPackageAssignment,
                gearAssignmentOptions,
                HandleGearAssignment);
        }

        private IReadOnlyList<PlayableCharacterSelectionOption> BuildCharacterSelectionOptions()
        {
            if (characterSelectionService == null || gameState == null)
            {
                return Array.Empty<PlayableCharacterSelectionOption>();
            }

            return characterSelectionService.BuildSelectableOptions(gameState);
        }

        private IReadOnlyDictionary<string, Sprite> ResolveCharacterSelectionWorldIcons(
            IReadOnlyList<PlayableCharacterSelectionOption> selectionOptions)
        {
            Dictionary<string, Sprite> worldIconsByCharacterId = new Dictionary<string, Sprite>();
            if (selectionOptions == null || characterWorldIconResolver == null)
            {
                return worldIconsByCharacterId;
            }

            for (int index = 0; index < selectionOptions.Count; index++)
            {
                PlayableCharacterSelectionOption selectionOption = selectionOptions[index];
                if (selectionOption == null ||
                    !characterWorldIconResolver.TryResolveWorldIcon(
                        selectionOption.CharacterId,
                        out Sprite worldIconSprite))
                {
                    continue;
                }

                worldIconsByCharacterId[selectionOption.CharacterId] = worldIconSprite;
            }

            return worldIconsByCharacterId;
        }

        private IReadOnlyList<PlayableCharacterSkillPackageOption> BuildSkillPackageOptions()
        {
            if (skillPackageAssignmentService == null || gameState == null)
            {
                return Array.Empty<PlayableCharacterSkillPackageOption>();
            }

            return skillPackageAssignmentService.BuildOptionsForSelectedCharacter(gameState);
        }

        private IReadOnlyList<PlayableCharacterGearAssignmentOption> BuildGearAssignmentOptions()
        {
            if (gearAssignmentService == null || gameState == null)
            {
                return Array.Empty<PlayableCharacterGearAssignmentOption>();
            }

            List<PlayableCharacterGearAssignmentOption> gearAssignmentOptions =
                new List<PlayableCharacterGearAssignmentOption>();
            AppendGearAssignmentOptions(gearAssignmentOptions, GearCategory.PrimaryCombat);
            AppendGearAssignmentOptions(gearAssignmentOptions, GearCategory.SecondarySupport);
            return gearAssignmentOptions;
        }

        private string BuildSelectedCharacterDisplayName()
        {
            if (characterSelectionService == null || gameState == null)
            {
                return "none";
            }

            return PlayableCharacterCatalog.Get(
                characterSelectionService.ResolveSelectedState(gameState).CharacterId).DisplayName;
        }

        private void AppendGearAssignmentOptions(
            List<PlayableCharacterGearAssignmentOption> targetOptions,
            GearCategory gearCategory)
        {
            IReadOnlyList<PlayableCharacterGearAssignmentOption> categoryOptions =
                gearAssignmentService.BuildOptionsForSelectedCharacter(gameState, gearCategory);

            for (int index = 0; index < categoryOptions.Count; index++)
            {
                targetOptions.Add(categoryOptions[index]);
            }
        }

        private void RefreshEntryButton()
        {
            bool hasSelectedNode = screenController.TryGetSelectedNodeId(out _);
            hasQuickRepeatNode = screenController.TryGetQuickRepeatNode(
                out quickRepeatNodeId,
                out quickRepeatNodeDisplayName,
                out quickRepeatNodeType);
            WorldMapScreenButtonState buttonState = WorldMapScreenStateResolver.ResolveEntryButtonState(
                onNodeEntryRequested != null,
                hasSelectedNode,
                hasSelectedNode
                    ? screenController.ResolveSelectedNodeDisplayName()
                    : null,
                hasQuickRepeatNode,
                quickRepeatNodeDisplayName,
                quickRepeatNodeType);

            entryActionSectionView.Refresh(buttonState);
        }

        private void RefreshLayout()
        {
            Canvas.ForceUpdateCanvases();
            buildSectionView.RefreshLayout();
            LayoutRebuilder.ForceRebuildLayoutImmediate(sidebarRectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRectTransform);
            surfaceSectionView.RefreshLayout();
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRectTransform);
            Canvas.ForceUpdateCanvases();
        }

        private void HandleSystemMenuRequested()
        {
            onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiClick);
            EnsureSystemMenuView().ShowMenu(
                onStopSessionRequested != null,
                HandleSystemMenuResumeRequested,
                onStopSessionRequested != null ? (Action)HandleSystemMenuExitRequested : null,
                currentSettingsState,
                HandleSettingsChangedRequested);
        }

        private void HandleSystemMenuResumeRequested()
        {
            onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiClick);
            systemMenuView.HideMenu();
        }

        private void HandleSystemMenuExitRequested()
        {
            if (onStopSessionRequested == null)
            {
                onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiError);
                return;
            }

            onFeedbackSoundRequested?.Invoke(UiSystemFeedbackSoundId.UiConfirm);
            systemMenuView.HideMenu();
            onStopSessionRequested();
        }

        private void HandleSettingsChangedRequested(UserSettingsState settingsState)
        {
            currentSettingsState = (settingsState ?? UserSettingsState.CreateDefault()).Sanitize();
            onSettingsChanged?.Invoke(currentSettingsState.Clone());
        }

        private CompactSystemMenuView EnsureSystemMenuView()
        {
            if (systemMenuView != null)
            {
                return systemMenuView;
            }

            systemMenuView = CompactSystemMenuUiFactory.CreateSystemMenuView(transform);
            return systemMenuView;
        }
    }
}
