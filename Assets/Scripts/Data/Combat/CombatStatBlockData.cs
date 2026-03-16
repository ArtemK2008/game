using System;
using UnityEngine;

namespace Survivalon.Runtime.Data.Combat
{
    [Serializable]
    public sealed class CombatStatBlockData
    {
        [SerializeField]
        private float maxHealth;

        [SerializeField]
        private float attackPower;

        [SerializeField]
        private float attackRate;

        [SerializeField]
        private float defense;

        public float MaxHealth => maxHealth;

        public float AttackPower => attackPower;

        public float AttackRate => attackRate;

        public float Defense => defense;
    }
}
