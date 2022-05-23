using System;
using System.Linq;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    public enum StageType
    {
        Main,
        Special
    }

    [Exchange]
    [Serializable]
    public class SpawnPayload
    {
        public EnemyPayload[] Enemies;
        public float Delay;
    }

    [Exchange]
    [Serializable]
    public class WavePayload
    {
        public SpawnPayload[] Spawns;
    }

    [Exchange]
    [Serializable]
    public class BattleStagePayload
    {
        public StageType Type;
        public int StageId;
        public WavePayload[] Waves;
    }

    public static class BattleStagePayloadExtensions
    {
        public static int GetTotalEnemyCount(this BattleStagePayload stage)
        {
            return stage.Waves.Sum(wave => wave.Spawns.Sum(spawn => spawn.Enemies.Length));
        }
    }
}
