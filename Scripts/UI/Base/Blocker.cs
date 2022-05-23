using Purity.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Armageddon.UI.Base
{
    public class Blocker : FastMonoBehaviour, IPointerClickHandler
    {
        public UnityEvent Clicked { set; get; } = new();

        /// <summary>
        ///     use this to host any unmasked objects
        /// </summary>
        public GameObject UnmasksObject { set; get; }

        public GameObject BackgroundObject { set; get; }

        private void Start()
        {
            gameObject.AddComponent<Mask>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke();
        }

        // public void AddUnmaskObject(GameObject unmaskObject)
        // {
        //     unmaskObject.transform.SetParent(UnmasksObject.transform, true);
        //
        //     var unmask = unmaskObject.GetComponent<Unmask>();
        //     var unmaskRaycastFilter = gameObject.AddComponent<UnmaskRaycastFilter>();
        //     unmaskRaycastFilter.targetUnmask = unmask;
        // }

        // public void AddUnmaskObjects(List<GameObject> unmaskObjects)
        // {
        //     foreach (GameObject unmaskObject in unmaskObjects)
        //     {
        //         AddUnmaskObject(unmaskObject);
        //     }
        // }
    }
}
