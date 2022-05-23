using Purity.Common;
using Sirenix.OdinInspector;

namespace Armageddon.Worlds
{
    public abstract class WorldContext : Context
    {
        [BoxGroup(nameof(WorldContext))]
        [HideInEditorMode]
        [ShowInInspector]
        private static World m_world;

        protected World World
        {
            get
            {
                if (m_world == null)
                {
                    m_world = GetService<World>();
                }

                return m_world;
            }
        }
    }
}
