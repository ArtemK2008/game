using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Survivalon.Core;

namespace Survivalon.Run
{
    /// <summary>
    /// Описывает, какие system feedback-звуки нужно подать в момент показа resolved post-run.
    /// </summary>
    public sealed class PostRunFeedbackSoundState
    {
        private readonly ReadOnlyCollection<UiSystemFeedbackSoundId> requestedSounds;

        public static PostRunFeedbackSoundState None { get; } =
            new PostRunFeedbackSoundState(Array.Empty<UiSystemFeedbackSoundId>());

        public PostRunFeedbackSoundState(params UiSystemFeedbackSoundId[] requestedSounds)
        {
            if (requestedSounds == null)
            {
                throw new ArgumentNullException(nameof(requestedSounds));
            }

            UiSystemFeedbackSoundId[] copiedSounds = new UiSystemFeedbackSoundId[requestedSounds.Length];
            Array.Copy(requestedSounds, copiedSounds, requestedSounds.Length);
            this.requestedSounds = Array.AsReadOnly(copiedSounds);
        }

        public IReadOnlyList<UiSystemFeedbackSoundId> RequestedSounds => requestedSounds;

        public bool HasRequestedSounds => requestedSounds.Count > 0;
    }
}
