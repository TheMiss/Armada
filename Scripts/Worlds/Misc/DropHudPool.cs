using Armageddon.Externals.OdinInspector;
using Purity.Common;
using UnityEngine;

namespace Armageddon.Worlds.Misc
{
    /// <summary>
    ///     The more we separate canvases, the better
    /// </summary>
    public class DropHudPool : ObjectPool<DropHud>
    {
        [BoxGroupPrefabs]
        [SerializeField]
        private DropHud m_dropHudPrefab;

        public void Create(int startupSize)
        {
            var batch = new Entry
            {
                SourceObject = m_dropHudPrefab,
                StartupSize = startupSize,
                WillExpand = true,
                ExpandingSize = startupSize / 2
            };

            AddEntry(batch);
        }

        public DropHud Get()
        {
            return Get<DropHud>(m_dropHudPrefab);
        }
    }
}
