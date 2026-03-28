using System;
using UnityEngine;
using UnityEngine.UI;
using Survivalon.Core;

namespace Survivalon.Combat
{
    /// <summary>
    /// Renders the compact placeholder combat shell for the run HUD and entity cards.
    /// </summary>
    public sealed class CombatShellView : MonoBehaviour
    {
        public const float PreferredHeight = 360f;

        private const float TitleHeight = 32f;
        private const float SummaryHeight = 104f;
        private const float EntityRowHeight = 152f;
        private const float EntitySpriteWidth = 72f;

        private Image backgroundImage;
        private Text titleText;
        private Text summaryText;
        private Image playerEntityCardImage;
        private Image playerEntitySpriteImage;
        private Text playerEntityCardText;
        private Image enemyEntityCardImage;
        private Image enemyEntitySpriteImage;
        private Text enemyEntityCardText;
        private Font uiFont;

        public void Show(
            CombatEncounterState combatEncounterState,
            CombatShellPresentationState presentationState,
            string title = null,
            string summary = null)
        {
            if (combatEncounterState == null)
            {
                throw new ArgumentNullException(nameof(combatEncounterState));
            }

            gameObject.name = "CombatShellView";
            EnsureUi();
            titleText.text = string.IsNullOrWhiteSpace(title)
                ? $"Combat Shell: {combatEncounterState.CombatContext.NodeId.Value}"
                : title;
            summaryText.text = string.IsNullOrWhiteSpace(summary)
                ? CombatShellTextBuilder.BuildSummaryText(combatEncounterState)
                : summary;
            ApplyEntityCard(
                playerEntityCardImage,
                playerEntitySpriteImage,
                playerEntityCardText,
                combatEncounterState.PlayerEntity,
                presentationState.PlayerSprite,
                CombatShellStateResolver.ResolveEntityCardColor(combatEncounterState.PlayerEntity.Side));
            ApplyEntityCard(
                enemyEntityCardImage,
                enemyEntitySpriteImage,
                enemyEntityCardText,
                combatEncounterState.EnemyEntity,
                presentationState.EnemySprite,
                CombatShellStateResolver.ResolveEntityCardColor(combatEncounterState.EnemyEntity.Side));
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
                "PlayerCombatEntitySprite",
                out playerEntitySpriteImage,
                out playerEntityCardText);
            enemyEntityCardImage = CreateEntityCard(
                entityRowObject.transform,
                "EnemyCombatEntity",
                "EnemyCombatEntitySprite",
                out enemyEntitySpriteImage,
                out enemyEntityCardText);
        }

        private Image CreateEntityCard(
            Transform parent,
            string objectName,
            string spriteObjectName,
            out Image entitySpriteImage,
            out Text entityCardText)
        {
            GameObject cardObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(Image),
                typeof(LayoutElement),
                typeof(HorizontalLayoutGroup));
            cardObject.transform.SetParent(parent, false);

            RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();
            cardRectTransform.localScale = Vector3.one;

            LayoutElement cardLayoutElement = cardObject.GetComponent<LayoutElement>();
            cardLayoutElement.minHeight = EntityRowHeight;
            cardLayoutElement.preferredHeight = EntityRowHeight;
            cardLayoutElement.flexibleWidth = 1f;

            Image cardImage = cardObject.GetComponent<Image>();

            HorizontalLayoutGroup cardLayout = cardObject.GetComponent<HorizontalLayoutGroup>();
            cardLayout.padding = new RectOffset(10, 10, 10, 10);
            cardLayout.spacing = 12f;
            cardLayout.childAlignment = TextAnchor.MiddleLeft;
            cardLayout.childControlWidth = false;
            cardLayout.childControlHeight = true;
            cardLayout.childForceExpandWidth = false;
            cardLayout.childForceExpandHeight = true;

            GameObject spriteObject = new GameObject(
                spriteObjectName,
                typeof(RectTransform),
                typeof(Image),
                typeof(LayoutElement));
            spriteObject.transform.SetParent(cardObject.transform, false);

            RectTransform spriteRectTransform = spriteObject.GetComponent<RectTransform>();
            spriteRectTransform.localScale = Vector3.one;

            LayoutElement spriteLayoutElement = spriteObject.GetComponent<LayoutElement>();
            spriteLayoutElement.minWidth = EntitySpriteWidth;
            spriteLayoutElement.preferredWidth = EntitySpriteWidth;
            spriteLayoutElement.flexibleWidth = 0f;

            entitySpriteImage = spriteObject.GetComponent<Image>();
            entitySpriteImage.color = Color.white;
            entitySpriteImage.preserveAspect = false;

            entityCardText = RuntimeUiSupport.CreateText(
                cardObject.transform,
                uiFont,
                "Label",
                15,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                Color.white);
            RuntimeUiSupport.AddLayoutElement(
                entityCardText.gameObject,
                EntityRowHeight - 20f,
                flexibleWidth: 1f);

            RectTransform textRectTransform = entityCardText.rectTransform;
            textRectTransform.localScale = Vector3.one;

            return cardImage;
        }

        private static void ApplyEntityCard(
            Image cardImage,
            Image spriteImage,
            Text cardText,
            CombatEntityRuntimeState combatEntity,
            Sprite sprite,
            Color backgroundColor)
        {
            cardImage.color = backgroundColor;
            spriteImage.sprite = sprite;
            cardText.text = CombatShellTextBuilder.BuildEntityCardText(combatEntity);
        }
    }
}
