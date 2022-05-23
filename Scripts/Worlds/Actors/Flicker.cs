using System.Collections;
using UnityEngine;

namespace Armageddon.Worlds.Actors
{
    public class Flicker : MonoBehaviour
    {
        //private const float FlickInterval = 0.05f;

        [Tooltip("Target")]
        [SerializeField]
        private GameObject m_gameObject;

        private Coroutine m_coroutine;

        public void Begin(float flickerInterval)
        {
            m_coroutine = StartCoroutine(PlayFlicker(flickerInterval));
        }

        public void End()
        {
            StopCoroutine(m_coroutine);
            m_gameObject.SetActive(true);
            //m_behaviour.enabled = true;
        }

        private IEnumerator PlayFlicker(float flickerInterval)
        {
            while (true)
            {
                yield return new WaitForSeconds(flickerInterval);
                m_gameObject.SetActive(!m_gameObject.activeInHierarchy);
                //m_behaviour.enabled = !m_behaviour.enabled;
            }
        }
    }
}
