using System;
using Survivalon.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.World
{
    /// <summary>
    /// Отвечает за нижнюю action-кнопку входа в выбранный узел карты мира.
    /// </summary>
    internal sealed class WorldMapEntryActionSectionView
    {
        private const float EntryButtonPreferredHeight = 56f;

        private readonly Button entryButton;
        private readonly Text entryButtonText;

        private WorldMapEntryActionSectionView(Button entryButton, Text entryButtonText)
        {
            this.entryButton = entryButton ?? throw new ArgumentNullException(nameof(entryButton));
            this.entryButtonText = entryButtonText ?? throw new ArgumentNullException(nameof(entryButtonText));
        }

        public static WorldMapEntryActionSectionView Create(
            Transform parent,
            Font font,
            Action onEntryRequested)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            if (onEntryRequested == null)
            {
                throw new ArgumentNullException(nameof(onEntryRequested));
            }

            GameObject buttonObject = new GameObject(
                "EnterSelectedNodeButton",
                typeof(RectTransform),
                typeof(Image),
                typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
            buttonRectTransform.localScale = Vector3.one;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = new Color(0.25f, 0.33f, 0.58f, 1f);

            Button entryButton = buttonObject.GetComponent<Button>();
            entryButton.targetGraphic = buttonImage;
            entryButton.onClick.AddListener(() => onEntryRequested());

            ColorBlock colors = entryButton.colors;
            colors.normalColor = buttonImage.color;
            colors.selectedColor = buttonImage.color;
            colors.highlightedColor = Color.Lerp(buttonImage.color, Color.white, 0.15f);
            colors.pressedColor = Color.Lerp(buttonImage.color, Color.black, 0.15f);
            colors.disabledColor = buttonImage.color * 0.7f;
            entryButton.colors = colors;

            Text entryButtonText = RuntimeUiSupport.CreateText(
                buttonObject.transform,
                font,
                "Label",
                18,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);
            ConfigureButtonLabelRect(entryButtonText.rectTransform);

            RuntimeUiSupport.AddLayoutElement(
                entryButton.gameObject,
                EntryButtonPreferredHeight,
                flexibleWidth: 1f,
                preferredWidth: 0f);
            return new WorldMapEntryActionSectionView(entryButton, entryButtonText);
        }

        public void Refresh(WorldMapScreenButtonState buttonState)
        {
            entryButton.interactable = buttonState.IsInteractable;
            entryButtonText.text = buttonState.Label;
        }

        private static void ConfigureButtonLabelRect(RectTransform labelRectTransform)
        {
            labelRectTransform.anchorMin = Vector2.zero;
            labelRectTransform.anchorMax = Vector2.one;
            labelRectTransform.offsetMin = new Vector2(14f, 8f);
            labelRectTransform.offsetMax = new Vector2(-14f, -8f);
            labelRectTransform.localScale = Vector3.one;
        }
    }
}
