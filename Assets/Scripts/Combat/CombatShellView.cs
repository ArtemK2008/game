using System;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.Runtime
{
    public sealed class CombatShellView : MonoBehaviour
    {
        public const float PreferredHeight = 252f;

        private const float TitleHeight = 32f;
        private const float SummaryHeight = 64f;
        private const float EntityRowHeight = 104f;

        private Image backgroundImage;
        private Text titleText;
        private Text summaryText;
        private Image playerEntityCardImage;
        private Text playerEntityCardText;
        private Image enemyEntityCardImage;
        private Text enemyEntityCardText;
        private Font uiFont;

        public void Show(CombatEncounterState combatEncounterState)
        {
            if (combatEncounterState == null)
            {
                throw new ArgumentNullException(nameof(combatEncounterState));
            }

            gameObject.name = "CombatShellView";
            EnsureUi();
            titleText.text = $"Combat Shell: {combatEncounterState.CombatContext.NodeId.Value}";
            summaryText.text = BuildSummaryText(combatEncounterState);
            ApplyEntityCard(
                playerEntityCardImage,
                playerEntityCardText,
                combatEncounterState.PlayerEntity,
                new Color(0.18f, 0.38f, 0.68f, 1f));
            ApplyEntityCard(
                enemyEntityCardImage,
                enemyEntityCardText,
                combatEncounterState.EnemyEntity,
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
            RuntimeUiSupport.AddLayoutElement(titleText.gameObject, TitleHeight);

            summaryText = RuntimeUiSupport.CreateText(
                transform,
                uiFont,
                "CombatShellSummary",
                16,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.88f, 0.90f, 0.94f, 1f));
            RuntimeUiSupport.AddLayoutElement(summaryText.gameObject, SummaryHeight);

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
            entityRowLayoutElement.minHeight = EntityRowHeight;
            entityRowLayoutElement.preferredHeight = EntityRowHeight;

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
            cardLayoutElement.minHeight = EntityRowHeight;
            cardLayoutElement.preferredHeight = EntityRowHeight;
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
            CombatEntityRuntimeState combatEntity,
            Color backgroundColor)
        {
            cardImage.color = backgroundColor;
            cardText.text =
                $"{combatEntity.DisplayName}\n" +
                $"{combatEntity.Side} | Alive: {FormatYesNo(combatEntity.IsAlive)} | Act: {FormatYesNo(combatEntity.IsActive)}\n" +
                $"HP: {FormatStat(combatEntity.CurrentHealth)} / {FormatStat(combatEntity.MaxHealth)} | ATK: {FormatStat(combatEntity.CombatEntity.BaseStats.AttackPower)}\n" +
                $"Rate: {FormatStat(combatEntity.CombatEntity.BaseStats.AttackRate)}/s | DEF: {FormatStat(combatEntity.CombatEntity.BaseStats.Defense)}";
        }

        private static string FormatYesNo(bool value)
        {
            return value ? "Yes" : "No";
        }

        private static string FormatStat(float value)
        {
            return value.ToString("0.##");
        }

        private static string BuildSummaryText(CombatEncounterState combatEncounterState)
        {
            string outcomeText = combatEncounterState.IsResolved
                ? $"Outcome: {combatEncounterState.Outcome}"
                : "Outcome: Ongoing";
            return
                $"Elapsed: {FormatStat(combatEncounterState.ElapsedCombatSeconds)}s | {outcomeText}\n" +
                $"Targeting: {combatEncounterState.PlayerEntity.DisplayName} -> {combatEncounterState.EnemyEntity.DisplayName}; " +
                $"{combatEncounterState.EnemyEntity.DisplayName} -> {combatEncounterState.PlayerEntity.DisplayName}";
        }
    }
}
