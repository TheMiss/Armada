using Armageddon.Games;
using Purity.Common;
using UnityEngine;

namespace Armageddon.Mechanics
{
    public class GameAccessibleObject : LightContext
    {
        protected static Game Game { private set; get; }

        public static void InjectGame(Game game)
        {
            if (Game != null)
            {
                Debug.LogWarning("SerializableObject.InjectGame() can be called only once");
                return;
            }

            Game = game;
        }

        protected static T CreateIfNull<T>(T obj) where T : new()
        {
            if (obj != null)
            {
                return obj;
            }

            obj = new T();

            return obj;
        }
    }
}
