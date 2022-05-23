using Armageddon.Sheets.Effects;

namespace Armageddon.Mechanics.Combats
{
    public readonly struct AttackModifier
    {
        public readonly Effect Effect;
        public readonly int Level;

        public AttackModifier(Effect effect, int level)
        {
            Effect = effect;
            Level = level;
        }
    }
}
