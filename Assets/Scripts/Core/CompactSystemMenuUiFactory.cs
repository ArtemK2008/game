using System;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.Core
{
    public static class CompactSystemMenuUiFactory
    {
        public static Button CreateSystemMenuButton(
            Transform parent,
            Font font,
            Action onClick)
        {
            GameObject buttonObject = new GameObject(
                "SystemMenuButton",
                typeof(RectTransform),
                typeof(Image),
                typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.transform.SetAsLastSibling();

            RectTransform buttonRectTransform = buttonObject.GetComponent<RectTransform>();
            buttonRectTransform.anchorMin = new Vector2(1f, 1f);
            buttonRectTransform.anchorMax = new Vector2(1f, 1f);
            buttonRectTransform.pivot = new Vector2(1f, 1f);
            buttonRectTransform.sizeDelta = new Vector2(132f, 42f);
            buttonRectTransform.anchoredPosition = new Vector2(-24f, -24f);
            buttonRectTransform.localScale = Vector3.one;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = new Color(0.18f, 0.24f, 0.32f, 0.96f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(() => onClick?.Invoke());

            Text buttonText = RuntimeUiSupport.CreateText(
                buttonObject.transform,
                font,
                "Label",
                16,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);
            buttonText.text = "System";

            RectTransform labelRectTransform = buttonText.rectTransform;
            labelRectTransform.anchorMin = Vector2.zero;
            labelRectTransform.anchorMax = Vector2.one;
            labelRectTransform.offsetMin = new Vector2(10f, 6f);
            labelRectTransform.offsetMax = new Vector2(-10f, -6f);
            labelRectTransform.localScale = Vector3.one;

            return button;
        }

        public static CompactSystemMenuView CreateSystemMenuView(Transform parent)
        {
            GameObject systemMenuObject = new GameObject("SystemMenuOverlay");
            systemMenuObject.transform.SetParent(parent, false);
            CompactSystemMenuView systemMenuView = systemMenuObject.AddComponent<CompactSystemMenuView>();
            systemMenuView.HideMenu();
            return systemMenuView;
        }
    }
}
