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
        private WorldMapOverviewSectionView overviewSectionView;
        private WorldMapBuildSectionView buildSectionView;
        private WorldMapEntryActionSectionView entryActionSectionView;
        private WorldMapNodeListSectionView nodeListSectionView;
        private WorldMapScreenController screenController;
        private Action<NodeId> onNodeEntryRequested;
        private PersistentGameState gameState;
        private PlayableCharacterSelectionService characterSelectionService;
        private PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService;
        private PlayableCharacterGearAssignmentService gearAssignmentService;
        private WorldMapBuildPreparationInteractionService buildPreparationInteractionService;
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
            WorldMapBuildPreparationInteractionService buildPreparationInteractionService = null)
        {
            if (worldGraph == null)
            {
                throw new ArgumentNullException(nameof(worldGraph));
            }

            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            screenController = new WorldMapScreenController(worldGraph, worldState, sessionContext: sessionContext);
            onNodeEntryRequested = nodeEntryRequested;
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

            RuntimeUiSupport.EnsureInputSystemEventSystem();
            EnsureUi();
            Refresh();
        }

        private void Refresh()
        {
            WorldMapWorldStateSummary worldStateSummary = screenController.BuildWorldStateSummary();
            IReadOnlyList<WorldMapNodeOption> nodeOptions = screenController.BuildNodeOptions();

            overviewSectionView.Refresh(
                "World Map",
                WorldMapScreenTextBuilder.BuildSummaryText(
                    worldStateSummary,
                    screenController.ResolveSelectedNodeDisplayName()));
            RefreshCharacterSelection();
            RefreshBuildAssignment();
            RefreshEntryButton();
            nodeListSectionView.Refresh(nodeOptions, HandleNodeSelection);
            RefreshLayout();
        }

        private void HandleNodeSelection(NodeId nodeId)
        {
            if (!screenController.TrySelectNode(nodeId))
            {
                return;
            }

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
                return;
            }

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
                return;
            }

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
                return;
            }

            Debug.Log($"World map gear assignment changed for {gearAssignmentOption.CharacterId}.");
            Refresh();
        }

        private void HandleNodeEntryRequest()
        {
            if (onNodeEntryRequested == null)
            {
                return;
            }

            if (screenController.TryGetSelectedNodeId(out NodeId selectedNodeId))
            {
                onNodeEntryRequested(selectedNodeId);
                return;
            }

            if (!hasQuickRepeatNode)
            {
                return;
            }

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
                typeof(VerticalLayoutGroup));
            panelObject.transform.SetParent(transform, false);

            Image panelImage = panelObject.GetComponent<Image>();
            panelImage.color = new Color(0.08f, 0.10f, 0.14f, 0.94f);

            panelRectTransform = panelObject.GetComponent<RectTransform>();
            panelRectTransform.anchorMin = Vector2.zero;
            panelRectTransform.anchorMax = Vector2.one;
            panelRectTransform.offsetMin = new Vector2(24f, 24f);
            panelRectTransform.offsetMax = new Vector2(-24f, -24f);
            panelRectTransform.localScale = Vector3.one;

            VerticalLayoutGroup panelLayout = panelObject.GetComponent<VerticalLayoutGroup>();
            panelLayout.padding = new RectOffset(16, 16, 16, 16);
            panelLayout.spacing = 8f;
            panelLayout.childAlignment = TextAnchor.UpperLeft;
            panelLayout.childControlWidth = true;
            panelLayout.childControlHeight = true;
            panelLayout.childForceExpandWidth = true;
            panelLayout.childForceExpandHeight = false;

            overviewSectionView = WorldMapOverviewSectionView.Create(panelObject.transform, uiFont);
            buildSectionView = WorldMapBuildSectionView.Create(panelObject.transform, uiFont);
            entryActionSectionView = WorldMapEntryActionSectionView.Create(
                panelObject.transform,
                uiFont,
                HandleNodeEntryRequest);
            nodeListSectionView = WorldMapNodeListSectionView.Create(panelObject.transform, uiFont);
        }

        private void RefreshCharacterSelection()
        {
            IReadOnlyList<PlayableCharacterSelectionOption> selectionOptions = BuildCharacterSelectionOptions();

            buildSectionView.RefreshCharacterSelection(
                WorldMapScreenTextBuilder.BuildCharacterSelectionText(selectionOptions),
                selectionOptions,
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
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRectTransform);
            nodeListSectionView.RefreshLayout();
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRectTransform);
            Canvas.ForceUpdateCanvases();
        }
    }
}
