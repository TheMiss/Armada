using UnityEngine;

namespace Armageddon
{
    public static class Layers
    {
        public static readonly int Hero = LayerMask.NameToLayer("Hero");
        public static readonly int Player = LayerMask.NameToLayer("Player");
        public static readonly int PlayerBullet = LayerMask.NameToLayer("PlayerBullet");
        public static readonly int Companion = LayerMask.NameToLayer("Companion");
        public static readonly int Enemy = LayerMask.NameToLayer("Enemy");
        public static readonly int EnemyBullet = LayerMask.NameToLayer("EnemyBullet");
    }
}
