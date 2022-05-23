using Armageddon.Worlds;
using Purity.Common;
using Sirenix.OdinInspector;

namespace Armageddon.Design
{
    public class SandboxContext : Context
    {
        [BoxGroup(nameof(SandboxContext))]
        [HideInEditorMode]
        [ShowInInspector]
        private static World m_world;

        [BoxGroup(nameof(SandboxContext))]
        [HideInEditorMode]
        [ShowInInspector]
        private static Sandbox m_sandbox;

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

        protected Sandbox Sandbox
        {
            get
            {
                if (m_sandbox == null)
                {
                    m_sandbox = GetService<Sandbox>();
                }

                return m_sandbox;
            }
        }
    }
}
