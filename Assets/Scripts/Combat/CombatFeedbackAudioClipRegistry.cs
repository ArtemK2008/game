using UnityEngine;

namespace Survivalon.Combat
{
    /// <summary>
    /// Stores runtime-safe references to the current prototype's combat feedback clips.
    /// </summary>
    public sealed class CombatFeedbackAudioClipRegistry : ScriptableObject
    {
        private const string ResourceName = "CombatFeedbackAudioClipRegistry";

        [SerializeField] private AudioClip playerAttackClip;
        [SerializeField] private AudioClip enemyAttackClip;
        [SerializeField] private AudioClip playerHitClip;
        [SerializeField] private AudioClip enemyHitClip;
        [SerializeField] private AudioClip enemyDefeatClip;
        [SerializeField] private AudioClip playerDefeatClip;
        [SerializeField] private AudioClip dangerLowHealthClip;
        [SerializeField] private AudioClip burstStrikeClip;

        public static CombatFeedbackAudioClipRegistry LoadOrNull()
        {
            return Resources.Load<CombatFeedbackAudioClipRegistry>(ResourceName);
        }

        public bool TryGetClip(CombatFeedbackSoundId soundId, out AudioClip clip)
        {
            switch (soundId)
            {
                case CombatFeedbackSoundId.PlayerAttack:
                    clip = playerAttackClip;
                    return clip != null;
                case CombatFeedbackSoundId.EnemyAttack:
                    clip = enemyAttackClip;
                    return clip != null;
                case CombatFeedbackSoundId.PlayerHit:
                    clip = playerHitClip;
                    return clip != null;
                case CombatFeedbackSoundId.EnemyHit:
                    clip = enemyHitClip;
                    return clip != null;
                case CombatFeedbackSoundId.EnemyDefeat:
                    clip = enemyDefeatClip;
                    return clip != null;
                case CombatFeedbackSoundId.PlayerDefeat:
                    clip = playerDefeatClip;
                    return clip != null;
                case CombatFeedbackSoundId.DangerLowHealth:
                    clip = dangerLowHealthClip;
                    return clip != null;
                case CombatFeedbackSoundId.BurstStrike:
                    clip = burstStrikeClip;
                    return clip != null;
                default:
                    clip = null;
                    return false;
            }
        }
    }
}
