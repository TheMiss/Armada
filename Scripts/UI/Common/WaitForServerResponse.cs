using Armageddon.UI.Base;
using UnityEngine;

namespace Armageddon.UI.Common
{
    public class WaitForServerResponse : Widget
    {
        [SerializeField]
        private GameObject m_loadingObject;

        protected override void Awake()
        {
            base.Awake();

            CanTick = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Reset to 0 degree.
            foreach (Transform childTransform in m_loadingObject.transform)
            {
                childTransform.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        public override void Show(bool animate = true)
        {
            base.Show(animate);

            Transform.SetAsLastSibling();
        }
    }
}
