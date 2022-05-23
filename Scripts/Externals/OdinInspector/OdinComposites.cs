using System;
using Sirenix.OdinInspector;

namespace Armageddon.Externals.OdinInspector
{
    [IncludeMyAttributes]
    [BoxGroup("Prefabs")]
    public class BoxGroupPrefabsAttribute : Attribute
    {
    }

    [IncludeMyAttributes]
    [Button]
    [GUIColorDefaultButton]
    public class DefaultButtonAttribute : Attribute
    {
    }

    /// <summary>
    ///     Default color of GUIColor for button.
    /// </summary>
    [IncludeMyAttributes]
    [GUIColor(0.4f, 0.8f, 1)]
    public class GUIColorDefaultButtonAttribute : Attribute
    {
    }

    /// <summary>
    ///     Color of Common or Regular
    /// </summary>
    [IncludeMyAttributes]
    [GUIColor(255 / 255f, 255 / 255f, 240 / 255f)]
    public class GUIColorCommonAttribute : Attribute
    {
    }

    /// <summary>
    ///     Color of Uncommon or Superior
    /// </summary>
    [IncludeMyAttributes]
    [GUIColor(61 / 255f, 210 / 255f, 11 / 255f)]
    public class GUIColorUncommonAttribute : Attribute
    {
    }

    /// <summary>
    ///     Color of Rare or Champion
    /// </summary>
    [IncludeMyAttributes]
    [GUIColor(47 / 255f, 120 / 255f, 255 / 255f)]
    public class GUIColorRareAttribute : Attribute
    {
    }

    /// <summary>
    ///     Color of Elite
    /// </summary>
    [IncludeMyAttributes]
    [GUIColor(230 / 255f, 65 / 255f, 255 / 255f)]
    public class GUIColorEpicAttribute : Attribute
    {
    }

    /// <summary>
    ///     Color of Legendary or Ruler
    /// </summary>
    [IncludeMyAttributes]
    [GUIColor(254 / 255f, 183 / 255f, 9 / 255f)]
    public class GUIColorLegendaryAttribute : Attribute
    {
    }

    /// <summary>
    ///     Color of Immortal or Demigod
    /// </summary>
    [IncludeMyAttributes]
    [GUIColor(1 / 255f, 246 / 255f, 247 / 255f)]
    public class GUIColorImmortalAttribute : Attribute
    {
    }

    /// <summary>
    ///     Color of Ancient or Deity
    /// </summary>
    [IncludeMyAttributes]
    [GUIColor(235 / 255f, 75 / 255f, 75 / 255f)]
    public class GUIColorAncientAttribute : Attribute
    {
    }
}
