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
        private static readonly Vector2 SideEffectSize = new Vector2(120f, 120f);
        private static readonly Vector2 CenterEffectSize = new Vector2(168f, 112f);
        private static readonly Color BackgroundArtTint = new Color(1f, 1f, 1f, 0.40f);
        private static readonly Color SideEffectTint = new Color(1f, 1f, 1f, 0.82f);
        private static readonly Color CenterEffectTint = new Color(1f, 1f, 1f, 0.88f);

        private Image backgroundImage;
        private Image backgroundArtImage;
        private Text titleText;
        private Text summaryText;
        private Image playerEntityCardImage;
        private Image playerEntitySpriteImage;
        private Text playerEntityCardText;
        private Image enemyEntityCardImage;
        private Image enemyEntitySpriteImage;
        private Text enemyEntityCardText;
        private Image playerEffectImage;
        private Image enemyEffectImage;
        private Image centerEffectImage;
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
            ApplyBackgroundArt(presentationState.BackgroundSprite);
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
            ApplyCombatEffects(presentationState);
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

            backgroundArtImage = CreateBackgroundArtImage();

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

            playerEffectImage = CreateOverlayEffectImage(
                entityRowObject.transform,
                "PlayerCombatEffectArt",
                new Vector2(0.25f, 0.5f),
                SideEffectSize);
            enemyEffectImage = CreateOverlayEffectImage(
                entityRowObject.transform,
                "EnemyCombatEffectArt",
                new Vector2(0.75f, 0.5f),
                SideEffectSize);
            centerEffectImage = CreateOverlayEffectImage(
                entityRowObject.transform,
                "CombatShellCenterEffectArt",
                new Vector2(0.5f, 0.5f),
                CenterEffectSize);
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
            entitySpriteImage.preserveAspect = true;

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

        private static Image CreateOverlayEffectImage(
            Transform parent,
            string objectName,
            Vector2 anchor,
            Vector2 size)
        {
            GameObject effectObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(Image),
                typeof(LayoutElement));
            effectObject.transform.SetParent(parent, false);

            RectTransform effectRectTransform = effectObject.GetComponent<RectTransform>();
            effectRectTransform.anchorMin = anchor;
            effectRectTransform.anchorMax = anchor;
            effectRectTransform.pivot = new Vector2(0.5f, 0.5f);
            effectRectTransform.anchoredPosition = Vector2.zero;
            effectRectTransform.sizeDelta = size;
            effectRectTransform.localScale = Vector3.one;

            LayoutElement effectLayoutElement = effectObject.GetComponent<LayoutElement>();
            effectLayoutElement.ignoreLayout = true;

            Image effectImage = effectObject.GetComponent<Image>();
            effectImage.enabled = false;
            effectImage.raycastTarget = false;
            effectImage.preserveAspect = true;
            return effectImage;
        }

        private Image CreateBackgroundArtImage()
        {
            GameObject backgroundArtObject = new GameObject(
                "CombatShellBackgroundArt",
                typeof(RectTransform),
                typeof(Image),
                typeof(LayoutElement));
            backgroundArtObject.transform.SetParent(transform, false);
            backgroundArtObject.transform.SetAsFirstSibling();

            RectTransform backgroundArtRectTransform = backgroundArtObject.GetComponent<RectTransform>();
            backgroundArtRectTransform.anchorMin = Vector2.zero;
            backgroundArtRectTransform.anchorMax = Vector2.one;
            backgroundArtRectTransform.offsetMin = Vector2.zero;
            backgroundArtRectTransform.offsetMax = Vector2.zero;
            backgroundArtRectTransform.localScale = Vector3.one;

            LayoutElement backgroundArtLayoutElement = backgroundArtObject.GetComponent<LayoutElement>();
            backgroundArtLayoutElement.ignoreLayout = true;

            Image resolvedBackgroundArtImage = backgroundArtObject.GetComponent<Image>();
            resolvedBackgroundArtImage.color = BackgroundArtTint;
            resolvedBackgroundArtImage.raycastTarget = false;
            return resolvedBackgroundArtImage;
        }

        private void ApplyBackgroundArt(Sprite backgroundSprite)
        {
            backgroundArtImage.sprite = backgroundSprite;
            backgroundArtImage.enabled = backgroundSprite != null;
            backgroundArtImage.color = BackgroundArtTint;
        }

        private void ApplyCombatEffects(CombatShellPresentationState presentationState)
        {
            ApplyCombatEffect(playerEffectImage, presentationState.PlayerEffectSprite, SideEffectTint);
            ApplyCombatEffect(enemyEffectImage, presentationState.EnemyEffectSprite, SideEffectTint);
            ApplyCombatEffect(centerEffectImage, presentationState.CenterEffectSprite, CenterEffectTint);
        }

        private static void ApplyCombatEffect(Image effectImage, Sprite effectSprite, Color tint)
        {
            effectImage.sprite = effectSprite;
            effectImage.enabled = effectSprite != null;
            effectImage.color = tint;
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
