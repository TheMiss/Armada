using Purity.Common;
using UnityEngine;

namespace Armageddon.Worlds.Actors
{
    [DisallowMultipleComponent]
    public class Location : FastMonoBehaviour
    {
        private const string IconName = "Position2D-Type1";

        [SerializeField]
        private Color m_color = Color.black;

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, IconName, true, m_color);
        }

        public Vector2 ToVector2()
        {
            return Transform.position;
        }

        public Vector3 ToVector3()
        {
            return Transform.position;
        }

        public static implicit operator Vector2(Location location)
        {
            return location.transform.position;
        }

        public static implicit operator Vector3(Location location)
        {
            return location.transform.position;
        }
    }
}
