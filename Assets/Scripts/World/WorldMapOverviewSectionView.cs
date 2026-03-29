using System;
using Survivalon.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Survivalon.World
{
    /// <summary>
    /// Отвечает за верхний overview-блок карты мира без логики доступа и выбора.
    /// </summary>
    internal sealed class WorldMapOverviewSectionView
    {
        private const float TitlePreferredHeight = 38f;
        private const float SummaryPreferredHeight = 104f;

        private readonly Text titleText;
        private readonly Text summaryText;

        private WorldMapOverviewSectionView(Text titleText, Text summaryText)
        {
            this.titleText = titleText ?? throw new ArgumentNullException(nameof(titleText));
            this.summaryText = summaryText ?? throw new ArgumentNullException(nameof(summaryText));
        }

        public static WorldMapOverviewSectionView Create(Transform parent, Font font)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            Text titleText = RuntimeUiSupport.CreateText(
                parent,
                font,
                "Title",
                24,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                Color.white);
            RuntimeUiSupport.AddLayoutElement(
                titleText.gameObject,
                TitlePreferredHeight,
                flexibleWidth: 1f,
                preferredWidth: 0f);

            Text summaryText = RuntimeUiSupport.CreateText(
                parent,
                font,
                "Summary",
                16,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.88f, 0.90f, 0.94f, 1f));
            RuntimeUiSupport.AddLayoutElement(
                summaryText.gameObject,
                SummaryPreferredHeight,
                flexibleWidth: 1f,
                preferredWidth: 0f);

            return new WorldMapOverviewSectionView(titleText, summaryText);
        }

        public void Refresh(string title, string summary)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Overview title cannot be null or whitespace.", nameof(title));
            }

            if (summary == null)
            {
                throw new ArgumentNullException(nameof(summary));
            }

            titleText.text = title;
            summaryText.text = summary;
        }
    }
}
