using System;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.Core
{
    /// <summary>
    /// Renders the compact MVP settings surface and raises sanitized setting changes to the owner.
    /// </summary>
    public sealed class CompactSettingsView : MonoBehaviour
    {
        private sealed class VolumeRowControls
        {
            public VolumeRowControls(Text valueText, Button decreaseButton, Button increaseButton)
            {
                ValueText = valueText;
                DecreaseButton = decreaseButton;
                IncreaseButton = increaseButton;
            }

            public Text ValueText { get; }

            public Button DecreaseButton { get; }

            public Button IncreaseButton { get; }
        }

        private VerticalLayoutGroup layoutGroup;
        private Text masterVolumeValueText;
        private Text musicVolumeValueText;
        private Text sfxVolumeValueText;
        private Text displayModeValueText;
        private Button masterVolumeDecreaseButton;
        private Button masterVolumeIncreaseButton;
        private Button musicVolumeDecreaseButton;
        private Button musicVolumeIncreaseButton;
        private Button sfxVolumeDecreaseButton;
        private Button sfxVolumeIncreaseButton;
        private Button displayModeToggleButton;
        private Text displayModeToggleButtonText;
        private Button backButton;
        private Font uiFont;
        private string configuredBackButtonName = "SettingsBackButton";
        private UserSettingsState currentSettingsState;
        private Action<UserSettingsState> onSettingsChanged;
        private Action onBackRequested;

        public void Show(
            UserSettingsState settingsState,
            Action<UserSettingsState> settingsChanged,
            Action backRequested,
            string backButtonName)
        {
            currentSettingsState = (settingsState ?? UserSettingsState.CreateDefault()).Sanitize();
            onSettingsChanged = settingsChanged;
            onBackRequested = backRequested;
            configuredBackButtonName = string.IsNullOrWhiteSpace(backButtonName)
                ? "SettingsBackButton"
                : backButtonName;

            EnsureUi();
            gameObject.SetActive(true);
            Refresh();
        }

        public void Hide()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

        private void EnsureUi()
        {
            if (layoutGroup != null)
            {
                backButton.gameObject.name = configuredBackButtonName;
                return;
            }

            uiFont = RuntimeUiSupport.LoadFallbackFont(nameof(CompactSettingsView));
            layoutGroup = RuntimeUiSupport.GetOrAddComponent<VerticalLayoutGroup>(gameObject);
            layoutGroup.spacing = 8f;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;

            VolumeRowControls masterVolumeRow = CreateVolumeRow(
                "MasterVolumeRow",
                "MasterVolumeDecreaseButton",
                "MasterVolumeValue",
                "MasterVolumeIncreaseButton",
                HandleMasterVolumeDecreaseRequested,
                HandleMasterVolumeIncreaseRequested);
            masterVolumeValueText = masterVolumeRow.ValueText;
            masterVolumeDecreaseButton = masterVolumeRow.DecreaseButton;
            masterVolumeIncreaseButton = masterVolumeRow.IncreaseButton;

            VolumeRowControls musicVolumeRow = CreateVolumeRow(
                "MusicVolumeRow",
                "MusicVolumeDecreaseButton",
                "MusicVolumeValue",
                "MusicVolumeIncreaseButton",
                HandleMusicVolumeDecreaseRequested,
                HandleMusicVolumeIncreaseRequested);
            musicVolumeValueText = musicVolumeRow.ValueText;
            musicVolumeDecreaseButton = musicVolumeRow.DecreaseButton;
            musicVolumeIncreaseButton = musicVolumeRow.IncreaseButton;

            VolumeRowControls sfxVolumeRow = CreateVolumeRow(
                "SfxVolumeRow",
                "SfxVolumeDecreaseButton",
                "SfxVolumeValue",
                "SfxVolumeIncreaseButton",
                HandleSfxVolumeDecreaseRequested,
                HandleSfxVolumeIncreaseRequested);
            sfxVolumeValueText = sfxVolumeRow.ValueText;
            sfxVolumeDecreaseButton = sfxVolumeRow.DecreaseButton;
            sfxVolumeIncreaseButton = sfxVolumeRow.IncreaseButton;

            GameObject displayRowObject = new GameObject(
                "DisplayModeRow",
                typeof(RectTransform),
                typeof(HorizontalLayoutGroup));
            displayRowObject.transform.SetParent(transform, false);
            RuntimeUiSupport.AddLayoutElement(displayRowObject, 44f);

            HorizontalLayoutGroup displayRowLayout = displayRowObject.GetComponent<HorizontalLayoutGroup>();
            displayRowLayout.spacing = 8f;
            displayRowLayout.childAlignment = TextAnchor.MiddleLeft;
            displayRowLayout.childControlWidth = false;
            displayRowLayout.childControlHeight = true;
            displayRowLayout.childForceExpandWidth = false;
            displayRowLayout.childForceExpandHeight = false;

            displayModeValueText = RuntimeUiSupport.CreateText(
                displayRowObject.transform,
                uiFont,
                "DisplayModeValue",
                17,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.90f, 0.92f, 0.96f, 1f));
            RuntimeUiSupport.GetOrAddComponent<LayoutElement>(displayModeValueText.gameObject).flexibleWidth = 1f;

            displayModeToggleButton = CreateInlineButton(
                displayRowObject.transform,
                "DisplayModeToggleButton",
                out displayModeToggleButtonText,
                HandleDisplayModeToggleRequested);

            backButton = CreateWideButton(
                transform,
                configuredBackButtonName,
                "Back",
                HandleBackRequested);
        }

        private void Refresh()
        {
            masterVolumeValueText.text = $"Master volume: {FormatPercent(currentSettingsState.MasterVolume)}";
            musicVolumeValueText.text = $"Music volume: {FormatPercent(currentSettingsState.MusicVolume)}";
            sfxVolumeValueText.text = $"SFX volume: {FormatPercent(currentSettingsState.SfxVolume)}";
            displayModeValueText.text = $"Display mode: {(currentSettingsState.UseFullscreen ? "Fullscreen" : "Windowed")}";
            displayModeToggleButtonText.text = currentSettingsState.UseFullscreen
                ? "Switch To Windowed"
                : "Switch To Fullscreen";

            masterVolumeDecreaseButton.interactable = currentSettingsState.MasterVolume > 0f;
            masterVolumeIncreaseButton.interactable = currentSettingsState.MasterVolume < 1f;
            musicVolumeDecreaseButton.interactable = currentSettingsState.MusicVolume > 0f;
            musicVolumeIncreaseButton.interactable = currentSettingsState.MusicVolume < 1f;
            sfxVolumeDecreaseButton.interactable = currentSettingsState.SfxVolume > 0f;
            sfxVolumeIncreaseButton.interactable = currentSettingsState.SfxVolume < 1f;
            backButton.gameObject.name = configuredBackButtonName;
        }

        private VolumeRowControls CreateVolumeRow(
            string rowObjectName,
            string decreaseButtonObjectName,
            string valueObjectName,
            string increaseButtonObjectName,
            Action onDecreaseRequested,
            Action onIncreaseRequested)
        {
            GameObject rowObject = new GameObject(
                rowObjectName,
                typeof(RectTransform),
                typeof(HorizontalLayoutGroup));
            rowObject.transform.SetParent(transform, false);
            RuntimeUiSupport.AddLayoutElement(rowObject, 44f);

            HorizontalLayoutGroup rowLayout = rowObject.GetComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 8f;
            rowLayout.childAlignment = TextAnchor.MiddleLeft;
            rowLayout.childControlWidth = false;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = false;
            rowLayout.childForceExpandHeight = false;

            Button decreaseButton = CreateInlineButton(
                rowObject.transform,
                decreaseButtonObjectName,
                out _,
                onDecreaseRequested,
                "-",
                48f);
            Text valueText = RuntimeUiSupport.CreateText(
                rowObject.transform,
                uiFont,
                valueObjectName,
                17,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.90f, 0.92f, 0.96f, 1f));
            RuntimeUiSupport.GetOrAddComponent<LayoutElement>(valueText.gameObject).flexibleWidth = 1f;
            Button increaseButton = CreateInlineButton(
                rowObject.transform,
                increaseButtonObjectName,
                out _,
                onIncreaseRequested,
                "+",
                48f);

            return new VolumeRowControls(valueText, decreaseButton, increaseButton);
        }

        private Button CreateInlineButton(
            Transform parent,
            string objectName,
            out Text buttonText,
            Action onClick,
            string label = null,
            float preferredWidth = 168f)
        {
            GameObject buttonObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = new Color(0.18f, 0.24f, 0.32f, 1f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = buttonImage;

            ColorBlock colors = button.colors;
            colors.normalColor = buttonImage.color;
            colors.highlightedColor = new Color(0.25f, 0.32f, 0.42f, 1f);
            colors.pressedColor = new Color(0.14f, 0.19f, 0.26f, 1f);
            colors.disabledColor = new Color(0.12f, 0.14f, 0.17f, 0.85f);
            button.colors = colors;

            RuntimeUiSupport.AddLayoutElement(buttonObject, 40f, preferredWidth: preferredWidth);

            buttonText = RuntimeUiSupport.CreateText(
                buttonObject.transform,
                uiFont,
                "Label",
                16,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);
            buttonText.text = label ?? objectName;

            RectTransform labelRectTransform = buttonText.rectTransform;
            labelRectTransform.anchorMin = Vector2.zero;
            labelRectTransform.anchorMax = Vector2.one;
            labelRectTransform.offsetMin = new Vector2(10f, 6f);
            labelRectTransform.offsetMax = new Vector2(-10f, -6f);

            button.onClick.AddListener(() => onClick?.Invoke());
            return button;
        }

        private Button CreateWideButton(
            Transform parent,
            string objectName,
            string label,
            Action onClick)
        {
            Button button = CreateInlineButton(parent, objectName, out Text buttonText, onClick, label);
            LayoutElement layoutElement = button.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = 0f;
            layoutElement.minWidth = 0f;
            layoutElement.flexibleWidth = 1f;
            buttonText.text = label;
            return button;
        }

        private void HandleMasterVolumeDecreaseRequested()
        {
            ApplySettingsChange(currentSettingsState.WithMasterVolume(currentSettingsState.MasterVolume - UserSettingsState.VolumeStep));
        }

        private void HandleMasterVolumeIncreaseRequested()
        {
            ApplySettingsChange(currentSettingsState.WithMasterVolume(currentSettingsState.MasterVolume + UserSettingsState.VolumeStep));
        }

        private void HandleMusicVolumeDecreaseRequested()
        {
            ApplySettingsChange(currentSettingsState.WithMusicVolume(currentSettingsState.MusicVolume - UserSettingsState.VolumeStep));
        }

        private void HandleMusicVolumeIncreaseRequested()
        {
            ApplySettingsChange(currentSettingsState.WithMusicVolume(currentSettingsState.MusicVolume + UserSettingsState.VolumeStep));
        }

        private void HandleSfxVolumeDecreaseRequested()
        {
            ApplySettingsChange(currentSettingsState.WithSfxVolume(currentSettingsState.SfxVolume - UserSettingsState.VolumeStep));
        }

        private void HandleSfxVolumeIncreaseRequested()
        {
            ApplySettingsChange(currentSettingsState.WithSfxVolume(currentSettingsState.SfxVolume + UserSettingsState.VolumeStep));
        }

        private void HandleDisplayModeToggleRequested()
        {
            ApplySettingsChange(currentSettingsState.WithFullscreen(!currentSettingsState.UseFullscreen));
        }

        private void HandleBackRequested()
        {
            onBackRequested?.Invoke();
        }

        private void ApplySettingsChange(UserSettingsState updatedState)
        {
            currentSettingsState = updatedState.Sanitize();
            onSettingsChanged?.Invoke(currentSettingsState.Clone());
            Refresh();
        }

        private static string FormatPercent(float value)
        {
            return $"{Mathf.RoundToInt(Mathf.Clamp01(value) * 100f)}%";
        }
    }
}
