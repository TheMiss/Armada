using System;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    [Serializable]
    public class CharacterPayload
    {
        // public string instanceId;
        public int SheetId;

        // public PartDataObject[] parts;
        public int[] DyeIds;

        public int DyeId;
        // public CharacterDataType type;
    }

    [Exchange(AddConvertExtension = true)]
    [Serializable]
    public class HeroPayload : CharacterPayload
    {
        public string InstanceId;
    }

    // [Serializable]
    // [SuppressMessage("ReSharper", "InconsistentNaming")]
    // public class PartDataObject
    // {
    //     [JsonProperty]
    //     public int VariantIndex { set; get; }
    //
    //     [JsonProperty]
    //     public int DyeId { set; get; }
    // }
}
