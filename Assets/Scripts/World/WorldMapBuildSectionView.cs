using System;
using System.Collections.Generic;
using Survivalon.Characters;
using Survivalon.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.World
{
    /// <summary>
    /// Отвечает за build-секцию карты мира: summary выбранного персонажа и кнопки смены build.
    /// </summary>
    internal sealed class WorldMapBuildSectionView
    {
        private const float CharacterSelectionSummaryPreferredHeight = 28f;
        private const float CharacterSelectionButtonPreferredHeight = 42f;
        private const float BuildAssignmentSummaryPreferredHeight = 54f;
        private const float SkillPackageAssignmentButtonPreferredHeight = 34f;
        private const float GearAssignmentButtonPreferredHeight = 34f;
        private const float CharacterWorldIconSize = 26f;
        private const float CharacterWorldIconAnchorX = 24f;
        private const float CharacterButtonLabelInsetWithoutIcon = 14f;
        private const float CharacterButtonLabelInsetWithIcon = 48f;

        private readonly Font uiFont;
        private readonly Text characterSelectionText;
        private readonly RectTransform characterSelectionContainer;
        private readonly Text buildAssignmentText;
        private readonly RectTransform skillPackageAssignmentContainer;
        private readonly RectTransform gearAssignmentContainer;

        private WorldMapBuildSectionView(
            Font uiFont,
            Text characterSelectionText,
            RectTransform characterSelectionContainer,
            Text buildAssignmentText,
            RectTransform skillPackageAssignmentContainer,
            RectTransform gearAssignmentContainer)
        {
            this.uiFont = uiFont ?? throw new ArgumentNullException(nameof(uiFont));
            this.characterSelectionText = characterSelectionText ?? throw new ArgumentNullException(nameof(characterSelectionText));
            this.characterSelectionContainer = characterSelectionContainer ?? throw new ArgumentNullException(nameof(characterSelectionContainer));
            this.buildAssignmentText = buildAssignmentText ?? throw new ArgumentNullException(nameof(buildAssignmentText));
            this.skillPackageAssignmentContainer = skillPackageAssignmentContainer ?? throw new ArgumentNullException(nameof(skillPackageAssignmentContainer));
            this.gearAssignmentContainer = gearAssignmentContainer ?? throw new ArgumentNullException(nameof(gearAssignmentContainer));
        }

        public static WorldMapBuildSectionView Create(Transform parent, Font font)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            Text characterSelectionText = RuntimeUiSupport.CreateText(
                parent,
                font,
                "CharacterSelectionSummary",
                16,
                FontStyle.Bold,
                TextAnchor.UpperLeft,
                new Color(0.88f, 0.90f, 0.94f, 1f));
            RuntimeUiSupport.AddLayoutElement(
                characterSelectionText.gameObject,
                CharacterSelectionSummaryPreferredHeight,
                flexibleWidth: 1f,
                preferredWidth: 0f);

            RectTransform characterSelectionContainer = CreateChoiceListContainer(
                parent,
                "CharacterSelectionList");

            Text buildAssignmentText = RuntimeUiSupport.CreateText(
                parent,
                font,
                "BuildAssignmentSummary",
                16,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.88f, 0.90f, 0.94f, 1f));
            RuntimeUiSupport.AddLayoutElement(
                buildAssignmentText.gameObject,
                BuildAssignmentSummaryPreferredHeight,
                flexibleWidth: 1f,
                preferredWidth: 0f);

            RectTransform skillPackageAssignmentContainer = CreateChoiceListContainer(
                parent,
                "SkillPackageAssignmentList");
            RectTransform gearAssignmentContainer = CreateChoiceListContainer(
                parent,
                "GearAssignmentList");

            return new WorldMapBuildSectionView(
                font,
                characterSelectionText,
                characterSelectionContainer,
                buildAssignmentText,
                skillPackageAssignmentContainer,
                gearAssignmentContainer);
        }

        public void RefreshCharacterSelection(
            string summaryText,
            IReadOnlyList<PlayableCharacterSelectionOption> selectionOptions,
            IReadOnlyDictionary<string, Sprite> worldIconsByCharacterId,
            Action<string> onCharacterSelected)
        {
            if (summaryText == null)
            {
                throw new ArgumentNullException(nameof(summaryText));
            }

            if (selectionOptions == null)
            {
                throw new ArgumentNullException(nameof(selectionOptions));
            }

            if (worldIconsByCharacterId == null)
            {
                throw new ArgumentNullException(nameof(worldIconsByCharacterId));
            }

            if (onCharacterSelected == null)
            {
                throw new ArgumentNullException(nameof(onCharacterSelected));
            }

            characterSelectionText.text = summaryText;
            ClearChildren(characterSelectionContainer);

            for (int index = 0; index < selectionOptions.Count; index++)
            {
                PlayableCharacterSelectionOption selectionOption = selectionOptions[index];
                Color buttonColor = selectionOption.IsSelected
                    ? new Color(0.20f, 0.45f, 0.28f, 1f)
                    : new Color(0.25f, 0.33f, 0.58f, 1f);
                string buttonLabel = WorldMapScreenTextBuilder.BuildCharacterButtonLabel(selectionOption);
                worldIconsByCharacterId.TryGetValue(selectionOption.CharacterId, out Sprite worldIconSprite);

                CreateChoiceButton(
                    characterSelectionContainer,
                    $"{selectionOption.CharacterId}_CharacterButton",
                    buttonLabel,
                    buttonColor,
                    CharacterSelectionButtonPreferredHeight,
                    worldIconSprite,
                    () => onCharacterSelected(selectionOption.CharacterId));
            }
        }

        public void RefreshBuildAssignment(
            string summaryText,
            IReadOnlyList<PlayableCharacterSkillPackageOption> skillPackageOptions,
            Action<string> onSkillPackageAssigned,
            IReadOnlyList<PlayableCharacterGearAssignmentOption> gearAssignmentOptions,
            Action<PlayableCharacterGearAssignmentOption> onGearAssignmentChanged)
        {
            if (summaryText == null)
            {
                throw new ArgumentNullException(nameof(summaryText));
            }

            if (skillPackageOptions == null)
            {
                throw new ArgumentNullException(nameof(skillPackageOptions));
            }

            if (onSkillPackageAssigned == null)
            {
                throw new ArgumentNullException(nameof(onSkillPackageAssigned));
            }

            if (gearAssignmentOptions == null)
            {
                throw new ArgumentNullException(nameof(gearAssignmentOptions));
            }

            if (onGearAssignmentChanged == null)
            {
                throw new ArgumentNullException(nameof(onGearAssignmentChanged));
            }

            buildAssignmentText.text = summaryText;
            RefreshSkillPackageButtons(skillPackageOptions, onSkillPackageAssigned);
            RefreshGearButtons(gearAssignmentOptions, onGearAssignmentChanged);
        }

        public void RefreshLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(characterSelectionContainer);
            LayoutRebuilder.ForceRebuildLayoutImmediate(skillPackageAssignmentContainer);
            LayoutRebuilder.ForceRebuildLayoutImmediate(gearAssignmentContainer);
        }

        private void RefreshSkillPackageButtons(
            IReadOnlyList<PlayableCharacterSkillPackageOption> skillPackageOptions,
            Action<string> onSkillPackageAssigned)
        {
            ClearChildren(skillPackageAssignmentContainer);

            for (int index = 0; index < skillPackageOptions.Count; index++)
            {
                PlayableCharacterSkillPackageOption skillPackageOption = skillPackageOptions[index];
                Color buttonColor = skillPackageOption.IsAssigned
                    ? new Color(0.38f, 0.32f, 0.16f, 1f)
                    : new Color(0.23f, 0.28f, 0.48f, 1f);
                string buttonLabel = WorldMapScreenTextBuilder.BuildSkillPackageButtonLabel(skillPackageOption);

                CreateChoiceButton(
                    skillPackageAssignmentContainer,
                    $"{skillPackageOption.SkillPackageId}_SkillPackageButton",
                    buttonLabel,
                    buttonColor,
                    SkillPackageAssignmentButtonPreferredHeight,
                    null,
                    () => onSkillPackageAssigned(skillPackageOption.SkillPackageId));
            }
        }

        private void RefreshGearButtons(
            IReadOnlyList<PlayableCharacterGearAssignmentOption> gearAssignmentOptions,
            Action<PlayableCharacterGearAssignmentOption> onGearAssignmentChanged)
        {
            ClearChildren(gearAssignmentContainer);

            for (int index = 0; index < gearAssignmentOptions.Count; index++)
            {
                PlayableCharacterGearAssignmentOption gearAssignmentOption = gearAssignmentOptions[index];
                Color buttonColor = gearAssignmentOption.IsEquipped
                    ? new Color(0.18f, 0.44f, 0.28f, 1f)
                    : new Color(0.23f, 0.28f, 0.48f, 1f);
                string buttonLabel = WorldMapScreenTextBuilder.BuildGearAssignmentButtonLabel(gearAssignmentOption);

                CreateChoiceButton(
                    gearAssignmentContainer,
                    $"{gearAssignmentOption.GearId}_GearButton",
                    buttonLabel,
                    buttonColor,
                    GearAssignmentButtonPreferredHeight,
                    null,
                    () => onGearAssignmentChanged(gearAssignmentOption));
            }
        }

        private Button CreateChoiceButton(
            Transform parent,
            string objectName,
            string label,
            Color buttonColor,
            float preferredHeight,
            Sprite leadingIconSprite,
            Action onClick)
        {
            GameObject buttonObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement));
            buttonObject.transform.SetParent(parent, false);

            RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
            buttonRectTransform.localScale = Vector3.one;

            LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
            layoutElement.minWidth = 0f;
            layoutElement.preferredWidth = 0f;
            layoutElement.flexibleWidth = 1f;
            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = buttonColor;

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(() => onClick());

            ColorBlock colors = button.colors;
            colors.normalColor = buttonImage.color;
            colors.selectedColor = buttonImage.color;
            colors.highlightedColor = Color.Lerp(buttonImage.color, Color.white, 0.15f);
            colors.pressedColor = Color.Lerp(buttonImage.color, Color.black, 0.15f);
            colors.disabledColor = buttonImage.color * 0.7f;
            button.colors = colors;

            if (leadingIconSprite != null)
            {
                CreateLeadingIcon(buttonObject.transform, leadingIconSprite);
            }

            Text buttonText = RuntimeUiSupport.CreateText(
                buttonObject.transform,
                uiFont,
                "Label",
                14,
                FontStyle.Bold,
                leadingIconSprite != null ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter,
                Color.white);
            buttonText.text = label;
            buttonText.resizeTextForBestFit = true;
            buttonText.resizeTextMinSize = 11;
            buttonText.resizeTextMaxSize = 14;
            buttonText.horizontalOverflow = HorizontalWrapMode.Wrap;
            buttonText.verticalOverflow = VerticalWrapMode.Truncate;

            ConfigureButtonLabelRect(buttonText.rectTransform, leadingIconSprite != null);
            return button;
        }

        private static void CreateLeadingIcon(Transform parent, Sprite leadingIconSprite)
        {
            GameObject iconObject = new GameObject(
                "WorldIcon",
                typeof(RectTransform),
                typeof(Image));
            iconObject.transform.SetParent(parent, false);

            Image iconImage = iconObject.GetComponent<Image>();
            iconImage.sprite = leadingIconSprite;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            iconImage.color = Color.white;

            RectTransform iconRectTransform = iconObject.GetComponent<RectTransform>();
            iconRectTransform.anchorMin = new Vector2(0f, 0.5f);
            iconRectTransform.anchorMax = new Vector2(0f, 0.5f);
            iconRectTransform.pivot = new Vector2(0.5f, 0.5f);
            iconRectTransform.anchoredPosition = new Vector2(CharacterWorldIconAnchorX, 0f);
            iconRectTransform.sizeDelta = new Vector2(CharacterWorldIconSize, CharacterWorldIconSize);
            iconRectTransform.localScale = Vector3.one;
        }

        private static RectTransform CreateChoiceListContainer(Transform parent, string objectName)
        {
            GameObject rowObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter),
                typeof(LayoutElement));
            rowObject.transform.SetParent(parent, false);

            VerticalLayoutGroup rowLayout = rowObject.GetComponent<VerticalLayoutGroup>();
            rowLayout.spacing = 8f;
            rowLayout.childAlignment = TextAnchor.UpperLeft;
            rowLayout.childControlWidth = true;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = true;
            rowLayout.childForceExpandHeight = false;

            ContentSizeFitter rowFitter = rowObject.GetComponent<ContentSizeFitter>();
            rowFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            rowFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            LayoutElement rowLayoutElement = rowObject.GetComponent<LayoutElement>();
            rowLayoutElement.minWidth = 0f;
            rowLayoutElement.preferredWidth = 0f;
            rowLayoutElement.flexibleWidth = 1f;

            RectTransform rowRectTransform = rowObject.GetComponent<RectTransform>();
            rowRectTransform.anchorMin = new Vector2(0f, 1f);
            rowRectTransform.anchorMax = new Vector2(1f, 1f);
            rowRectTransform.pivot = new Vector2(0.5f, 1f);
            rowRectTransform.localScale = Vector3.one;
            return rowRectTransform;
        }

        private static void ConfigureButtonLabelRect(RectTransform labelRectTransform, bool hasLeadingIcon)
        {
            labelRectTransform.anchorMin = Vector2.zero;
            labelRectTransform.anchorMax = Vector2.one;
            labelRectTransform.offsetMin = new Vector2(
                hasLeadingIcon ? CharacterButtonLabelInsetWithIcon : CharacterButtonLabelInsetWithoutIcon,
                8f);
            labelRectTransform.offsetMax = new Vector2(-14f, -8f);
            labelRectTransform.localScale = Vector3.one;
        }

        private static void ClearChildren(Transform container)
        {
            for (int index = container.childCount - 1; index >= 0; index--)
            {
                GameObject childObject = container.GetChild(index).gameObject;
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(childObject);
                    continue;
                }

                UnityEngine.Object.DestroyImmediate(childObject);
            }
        }
    }
}
