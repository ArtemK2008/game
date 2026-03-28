namespace Survivalon.Run
{
    /// <summary>
    /// РћРїРёСЃС‹РІР°РµС‚, РєР°РєРёРµ system feedback-Р·РІСѓРєРё РЅСѓР¶РЅРѕ РїРѕРґР°С‚СЊ РІ РјРѕРјРµРЅС‚ РїРѕРєР°Р·Р° resolved post-run.
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
