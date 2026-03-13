using System;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.Runtime
{
    public sealed class CombatShellView : MonoBehaviour
    {
        private Image backgroundImage;
        private Text titleText;
        private Text summaryText;
        private Image playerEntityCardImage;
        private Text playerEntityCardText;
        private Image enemyEntityCardImage;
        private Text enemyEntityCardText;
        private Font uiFont;

        public void Show(CombatShellContext combatContext)
        {
            if (combatContext == null)
            {
                throw new ArgumentNullException(nameof(combatContext));
            }

            gameObject.name = "CombatShellView";
            EnsureUi();
            titleText.text = $"Combat Shell: {combatContext.NodeId.Value}";
            summaryText.text = "Combat shell active. One player-side entity and one enemy-side entity are spawned.";
            ApplyEntityCard(
                playerEntityCardImage,
                playerEntityCardText,
                combatContext.PlayerEntity,
                new Color(0.18f, 0.38f, 0.68f, 1f));
            ApplyEntityCard(
                enemyEntityCardImage,
                enemyEntityCardText,
                combatContext.EnemyEntity,
                new Color(0.62f, 0.22f, 0.22f, 1f));
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void EnsureUi()
        {
            if (backgroundImage != null)
            {
                return;
            }

            uiFont = RuntimeUiSupport.LoadFallbackFont(nameof(CombatShellView));

            RectTransform rootRectTransform = RuntimeUiSupport.GetOrAddComponent<RectTransform>(gameObject);
            backgroundImage = RuntimeUiSupport.GetOrAddComponent<Image>(gameObject);
            VerticalLayoutGroup layoutGroup = RuntimeUiSupport.GetOrAddComponent<VerticalLayoutGroup>(gameObject);

            rootRectTransform.localScale = Vector3.one;

            backgroundImage.color = new Color(0.10f, 0.11f, 0.15f, 0.95f);

            layoutGroup.padding = new RectOffset(18, 18, 18, 18);
            layoutGroup.spacing = 10f;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;

            titleText = RuntimeUiSupport.CreateText(
                transform,
                uiFont,
                "CombatShellTitle",
                24,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                Color.white);
            RuntimeUiSupport.AddLayoutElement(titleText.gameObject, 32f);

            summaryText = RuntimeUiSupport.CreateText(
                transform,
                uiFont,
                "CombatShellSummary",
                16,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.88f, 0.90f, 0.94f, 1f));
            RuntimeUiSupport.AddLayoutElement(summaryText.gameObject, 46f);

            GameObject entityRowObject = new GameObject(
                "CombatEntityRow",
                typeof(RectTransform),
                typeof(HorizontalLayoutGroup),
                typeof(LayoutElement));
            entityRowObject.transform.SetParent(transform, false);

            HorizontalLayoutGroup entityRowLayout = entityRowObject.GetComponent<HorizontalLayoutGroup>();
            entityRowLayout.spacing = 12f;
            entityRowLayout.childAlignment = TextAnchor.MiddleCenter;
            entityRowLayout.childControlWidth = true;
            entityRowLayout.childControlHeight = true;
            entityRowLayout.childForceExpandWidth = true;
            entityRowLayout.childForceExpandHeight = true;

            LayoutElement entityRowLayoutElement = entityRowObject.GetComponent<LayoutElement>();
            entityRowLayoutElement.minHeight = 92f;
            entityRowLayoutElement.preferredHeight = 92f;

            playerEntityCardImage = CreateEntityCard(
                entityRowObject.transform,
                "PlayerCombatEntity",
                out playerEntityCardText);
            enemyEntityCardImage = CreateEntityCard(
                entityRowObject.transform,
                "EnemyCombatEntity",
                out enemyEntityCardText);
        }

        private Image CreateEntityCard(
            Transform parent,
            string objectName,
            out Text entityCardText)
        {
            GameObject cardObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(Image),
                typeof(LayoutElement));
            cardObject.transform.SetParent(parent, false);

            RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();
            cardRectTransform.localScale = Vector3.one;

            LayoutElement cardLayoutElement = cardObject.GetComponent<LayoutElement>();
            cardLayoutElement.minHeight = 92f;
            cardLayoutElement.preferredHeight = 92f;
            cardLayoutElement.flexibleWidth = 1f;

            Image cardImage = cardObject.GetComponent<Image>();

            entityCardText = RuntimeUiSupport.CreateText(
                cardObject.transform,
                uiFont,
                "Label",
                18,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);

            RectTransform textRectTransform = entityCardText.rectTransform;
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = new Vector2(12f, 8f);
            textRectTransform.offsetMax = new Vector2(-12f, -8f);
            textRectTransform.localScale = Vector3.one;

            return cardImage;
        }

        private static void ApplyEntityCard(
            Image cardImage,
            Text cardText,
            CombatEntityState combatEntity,
            Color backgroundColor)
        {
            cardImage.color = backgroundColor;
            cardText.text =
                $"{combatEntity.DisplayName}\n" +
                $"Side: {combatEntity.Side}\n" +
                $"HP: {FormatStat(combatEntity.BaseStats.MaxHealth)} | ATK: {FormatStat(combatEntity.BaseStats.AttackPower)}\n" +
                $"Rate: {FormatStat(combatEntity.BaseStats.AttackRate)}/s | DEF: {FormatStat(combatEntity.BaseStats.Defense)}\n" +
                $"Alive: {FormatYesNo(combatEntity.IsAlive)} | Active: {FormatYesNo(combatEntity.IsActive)}";
        }

        private static string FormatYesNo(bool value)
        {
            return value ? "Yes" : "No";
        }

        private static string FormatStat(float value)
        {
            return value.ToString("0.##");
        }
    }
}
