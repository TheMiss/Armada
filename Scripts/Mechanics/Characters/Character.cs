using System.Collections.Generic;
using System.Linq;
using Armageddon.Backend.Payloads;
using Armageddon.Mechanics.Inventories;
using Armageddon.Worlds.Actors.Characters;
using Newtonsoft.Json;

namespace Armageddon.Mechanics.Characters
{
    public class PartData
    {
        [JsonProperty]
        public int VariantIndex { set; get; }

        [JsonProperty]
        public int DyeId { set; get; }
    }

    public abstract class Character : GameAccessibleObject, IInventoryObject
    {
        public int SheetId { private set; get; }

        public List<PartData> Parts { protected set; get; }

        public int DyeId { set; get; }

        public CharacterDataType Type { private set; get; }

        public List<CharacterSkin> Dyes { private set; get; }

        public string InstanceId { get; set; }

        protected void Initialize(CharacterPayload characterPayload)
        {
            var parts = new List<PartData>();

            SheetId = characterPayload.SheetId;
            Parts = parts;
            DyeId = characterPayload.DyeId;
            // Dyes = ItemFactory.GetDyes(characterObject.dyeIds.ToList());
            // Type = characterObject.type;
        }

        public CharacterSkin GetSelectedDye()
        {
            return Dyes.FirstOrDefault(dye => dye.Id == DyeId);
        }

        // public abstract SkinPreset GetSelectedSkinPreset();

        /// <summary>
        ///     This will also modify Parts as well
        /// </summary>
        /// <param name="presetIndex"></param>
        public abstract void SetSelectedSkinPreset(int presetIndex);
    }
}
