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
        private Image playerParticipantCardImage;
        private Text playerParticipantCardText;
        private Image enemyParticipantCardImage;
        private Text enemyParticipantCardText;
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
            ApplyParticipantCard(
                playerParticipantCardImage,
                playerParticipantCardText,
                combatContext.PlayerParticipant,
                new Color(0.18f, 0.38f, 0.68f, 1f));
            ApplyParticipantCard(
                enemyParticipantCardImage,
                enemyParticipantCardText,
                combatContext.EnemyParticipant,
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

            GameObject participantRowObject = new GameObject(
                "CombatParticipantRow",
                typeof(RectTransform),
                typeof(HorizontalLayoutGroup),
                typeof(LayoutElement));
            participantRowObject.transform.SetParent(transform, false);

            HorizontalLayoutGroup participantRowLayout = participantRowObject.GetComponent<HorizontalLayoutGroup>();
            participantRowLayout.spacing = 12f;
            participantRowLayout.childAlignment = TextAnchor.MiddleCenter;
            participantRowLayout.childControlWidth = true;
            participantRowLayout.childControlHeight = true;
            participantRowLayout.childForceExpandWidth = true;
            participantRowLayout.childForceExpandHeight = true;

            LayoutElement participantRowLayoutElement = participantRowObject.GetComponent<LayoutElement>();
            participantRowLayoutElement.minHeight = 92f;
            participantRowLayoutElement.preferredHeight = 92f;

            playerParticipantCardImage = CreateParticipantCard(
                participantRowObject.transform,
                "PlayerCombatParticipant",
                out playerParticipantCardText);
            enemyParticipantCardImage = CreateParticipantCard(
                participantRowObject.transform,
                "EnemyCombatParticipant",
                out enemyParticipantCardText);
        }

        private Image CreateParticipantCard(
            Transform parent,
            string objectName,
            out Text participantCardText)
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

            participantCardText = RuntimeUiSupport.CreateText(
                cardObject.transform,
                uiFont,
                "Label",
                18,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);

            RectTransform textRectTransform = participantCardText.rectTransform;
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = new Vector2(12f, 8f);
            textRectTransform.offsetMax = new Vector2(-12f, -8f);
            textRectTransform.localScale = Vector3.one;

            return cardImage;
        }

        private static void ApplyParticipantCard(
            Image cardImage,
            Text cardText,
            CombatShellParticipant participant,
            Color backgroundColor)
        {
            cardImage.color = backgroundColor;
            cardText.text = $"{participant.DisplayName}\nSide: {participant.Side}";
        }
    }
}
