using System.Collections.Generic;
using UnityEngine;

namespace Armageddon
{
    public class DisableAtStartObjects : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> m_disablingObjects;

        private void OnEnable()
        {
            foreach (GameObject disableObject in m_disablingObjects)
            {
                disableObject.SetActive(false);
            }

            Destroy(gameObject);
        }
    }
}
