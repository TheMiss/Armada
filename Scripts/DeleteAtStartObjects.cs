using System.Collections.Generic;
using Purity.Common.Extensions;
using UnityEngine;

namespace Armageddon
{
    public class DeleteAtStartObjects : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> m_deletingObjects;

        [SerializeField]
        private List<GameObject> m_deleteChildrenObjects;

        private void OnEnable()
        {
            foreach (GameObject deletingObject in m_deletingObjects)
            {
                Destroy(deletingObject);
            }

            m_deletingObjects.Clear();

            Destroy(gameObject);

            foreach (GameObject deleteChildrenObject in m_deleteChildrenObjects)
            {
                deleteChildrenObject.transform.DestroyChildren();
            }
        }
    }
}
