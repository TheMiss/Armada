using System.Collections.Generic;
using Armageddon.AssetManagement;
using Armageddon.Backend.Payloads;
using Armageddon.Sheets.Actors;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Armageddon.Mechanics.Characters
{
    public class Hero : Character
    {
        public HeroSheet Sheet { private set; get; }

        public Dictionary<string, uint> Prices { set; get; }

        public bool IsUnlocked => !string.IsNullOrEmpty(InstanceId);

        public async UniTask<bool> InitializeAsync(HeroPayload heroPayload)
        {
            Initialize(heroPayload);

            InstanceId = heroPayload.InstanceId;
            Sheet = await Assets.LoadHeroSheetAsync(heroPayload.SheetId);

            if (Sheet == null)
            {
                Debug.LogError($"Wait what?!... No HeroSheet {heroPayload.SheetId}");
                return false;
            }

            return true;
        }

        // private static List<PartData> GetParts(int heroSheetId)
        // {
        //     return HeroActorFactory.GetDefaultParts(heroSheetId);
        // }

        // public override SkinPreset GetSelectedSkinPreset()
        // {
        //     return null;
        //     // return HeroFactory.GetSkinPreset(SchemaId, PresetIndex);
        // }

        public override void SetSelectedSkinPreset(int presetIndex)
        {
            // PresetIndex = presetIndex;
            //
            // Parts = HeroFactory.GetSkinPreset(SchemaId, presetIndex).GetPartVariantDataList();
        }
    }
}
