using System;
using Armageddon.Games;
using Armageddon.Mechanics.Maps;
using Armageddon.Worlds.Actors.Characters;
using Purity.Common;
using Sirenix.OdinInspector;

namespace Armageddon.Worlds
{
    public class BattleLog : GameContext
    {
        [HideInEditorMode]
        [ShowInInspector]
        public string CurrentLogFilePath { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            RegisterService(this);
        }

        [Button]
        private void OpenFolder()
        {
        }

        public void CreateLog(Stage stage)
        {
            // string fileName = $"{DateTime.Now}_{stage.Id}";
            // string filePath = $"BattleLogs/{fileName}";

            string fileName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";
            CurrentLogFilePath = $"BattleLogs/{fileName}";

            DeviceFile.WriteAllText(CurrentLogFilePath, string.Empty);
        }

        public void LogDamage(CharacterActor characterActor, float damage)
        {
            // string text = $"{characterActor.NameWithInstanceId} got {damage} damage.\n";
            // DeviceFile.AppendAllText(CurrentLogFilePath, text); // Bad idea
        }
    }
}
