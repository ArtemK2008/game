namespace Survivalon.Run
{
    /// <summary>
    /// Описывает, какие system feedback-звуки нужно подать в момент показа resolved post-run.
    /// </summary>
    public sealed class PostRunFeedbackSoundState
    {
        public static PostRunFeedbackSoundState None { get; } = new PostRunFeedbackSoundState(false, false);

        public PostRunFeedbackSoundState(bool shouldPlayUnlockSound, bool shouldPlayBossClearSound)
        {
            ShouldPlayUnlockSound = shouldPlayUnlockSound;
            ShouldPlayBossClearSound = shouldPlayBossClearSound;
        }

        public bool ShouldPlayUnlockSound { get; }

        public bool ShouldPlayBossClearSound { get; }
    }
}
