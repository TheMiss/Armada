using System.Numerics;
using Armageddon.Backend.Attributes;

namespace Armageddon.Mechanics.Characters
{
    [Exchange]
    public enum CharacterSize
    {
        Small,
        Medium,
        Large,
        Gigantic
    }

    public static class EnemyScaleExtensions
    {
        public static float ToFloat(this CharacterSize size)
        {
            return 1.0f;
        }

        public static Vector2 ToVector2(this CharacterSize size)
        {
            float scale = size.ToFloat();
            return new Vector2(scale, scale);
        }

        public static Vector3 ToVector3(this CharacterSize size)
        {
            float scale = size.ToFloat();
            return new Vector3(scale, scale, scale);
        }
    }
}
