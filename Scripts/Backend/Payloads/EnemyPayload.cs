using System;
using Armageddon.Backend.Attributes;
using Armageddon.Mechanics.Characters;

namespace Armageddon.Backend.Payloads
{
    [Exchange(AddConvertExtension = true)]
    [Serializable]
    public class EnemyPayload
    {
        public int Id;
        public int Level;
        public int? SheetId;
        public EnemyRank? Rank;
        public int? Dexterity;
        public int? Vitality;
        public int? Perception;
        public int? Leadership;
        public int? Health;
        public int? HealthRegeneration;
        public int? Armor;
        public int? PrimaryDamage;
        public StageDropPayload[] Drops = { };
    }
}
