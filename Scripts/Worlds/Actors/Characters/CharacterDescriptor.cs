using Armageddon.Mechanics.Combats;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Characters
{
    /// <summary>
    ///     Formerly know as CharacterDescription. Naming convention is like this
    ///     https://tool.oschina.net/uploads/apidocs/ogre3d/api/html/structOgre_1_1RenderWindowDescription.html#_details
    /// </summary>
    public class CharacterDescriptor
    {
        // TODO: Upgrade to init property in C# 9.0
        public int Id { get; set; }
        public int Level { get; set; }
        public CharacterHud CharacterHud { get; set; }
        public LayerMask ActorLayer { get; set; }
        public LayerMask BulletLayer { get; set; }
        public LayerMask BulletCollisionMask { get; set; }
        public bool AddCollisionHandler { get; set; }
        public int WeaponSlotCount { get; set; } = 2;
        public CombatEntity CombatEntity { get; set; }
    }
}
