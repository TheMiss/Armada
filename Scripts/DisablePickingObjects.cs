using System.Collections.Generic;
using Purity.Common;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Armageddon
{
    public class DisablePickingObjects : FastMonoBehaviour
    {
        [SerializeField]
        private List<GameObject> m_disablingPickingObjects;

#if UNITY_EDITOR
        private void Awake()
        {
            foreach (GameObject disablingPickingObject in m_disablingPickingObjects)
            {
                SceneVisibilityManager.instance.DisablePicking(disablingPickingObject, true);
            }
        }
#endif
    }
}
