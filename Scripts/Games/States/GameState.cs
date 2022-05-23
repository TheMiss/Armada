namespace Armageddon.Games.States
{
    public class GameState
    {
        public static readonly GameState Empty = new();
        public int Test;

        public override string ToString()
        {
            return $"{GetType().Name}";
            // return $"{GetType().Name.Replace("State", string.Empty)}";
        }
    }
}
