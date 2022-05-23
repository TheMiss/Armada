using Armageddon.Assistance.BackendDrivers;
using Armageddon.UI.MainMenu.Upgrades.Powers;
using Purity.Common;

namespace Armageddon.Configuration
{
    public static class DebugSettings
    {
        public static readonly bool ShowMoveSelection = false;

        static DebugSettings()
        {
#if UNITY_EDITOR
            DeviceFile.WriteUncompressedFile = true;
#endif
            
            // Player
#if UNITY_EDITOR
#endif
            // Player.CompressPlayerProfileFile = true;
            // Player.CompressPlayerInventoryFile = true;
            // Player.CompressLocalBadgeManagerFile = true;
            
            // Other
            PowersSubWindow.LogCreateCardButtonsTimeTaken = true;

            // Backend
            BackendSettings.LogEncryptTimeTaken = false;
            BackendSettings.LogDecryptTimeTaken = false;
            BackendSettings.LogSerializeTimeTaken = false;
            BackendSettings.LogDeserializeTimeTaken = false;
            BackendSettings.LogExecuteFunctionTimeTaken = true;
        }
    }
}
