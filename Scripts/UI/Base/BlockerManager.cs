using System.Collections;
using System.Collections.Generic;
using Armageddon.Extensions;
using Purity.Common;
using Purity.Common.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Armageddon.UI.Base
{
    public class BlockerManager : FastMonoBehaviour
    {
        [ShowInPlayMode]
        public static float OpenBlockerDuration = 0.25f;

        [ShowInPlayMode]
        public static float CloseBlockerDuration = 0.25f;

        [ShowInPlayMode]
        public static Color BlockerColor = new(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);

        public static Texture2D BackgroundTexture;

        [SerializeField]
        private bool m_useZeroAlphaForBlocker;

        [ShowInPlayMode]
        private readonly Stack<Blocker> m_blockers = new();

        [ShowInPlayMode]
        public Color ClearColor => GetClearColor();

        public Transform Parent { set; get; }
        public Canvas TopmostCanvas { set; get; }

        private Color GetClearColor()
        {
#if UNITY_EDITOR
            float alpha = 0.2f;

            if (m_useZeroAlphaForBlocker)
            {
                alpha = 0.0f;
            }
#else
            const float alpha = 0.0f;
#endif
            return Color.red.WithAlpha(alpha);
        }

        public Blocker AddBlocker(Transform parent, int siblingIndex, Color? blockerColor = null,
            UnityAction onClickCallback = null)
        {
            Blocker blocker = CreateBlocker(parent, siblingIndex, blockerColor, onClickCallback);
            m_blockers.Push(blocker);

            return blocker;
        }

        private Blocker CreateBlocker(Transform parent, int siblingIndex, Color? blockerColor = null,
            UnityAction onClickCallback = null)
        {
            if (BackgroundTexture == null)
            {
                BackgroundTexture = new Texture2D(1, 1);
                BackgroundTexture.SetPixel(0, 0, Color.white);
                BackgroundTexture.Apply();
            }

            if (TopmostCanvas == null)
            {
                Debug.LogWarning("BlockerManager needs Canvas!");
                return null;
            }

            var blockerObject = new GameObject("Blocker");
            var blocker = blockerObject.AddComponent<Blocker>();
            {
                var rect = new Rect(0, 0, BackgroundTexture.width, BackgroundTexture.height);
                var sprite = Sprite.Create(BackgroundTexture, rect, new Vector2(0.5f, 0.5f), 1);
                var image = blockerObject.AddComponent<Image>();
                image.material.mainTexture = BackgroundTexture;
                image.sprite = sprite;
                image.color = new Color(1, 1, 1, 1 / 255f);
                image.canvasRenderer.SetAlpha(0.0f);
                image.raycastTarget = false; // Let the BackgroundObject handle this
                image.CrossFadeAlpha(1.0f, OpenBlockerDuration, false);

                // Can use RectTransform Extensions...
                blockerObject.transform.localScale = new Vector3(1, 1, 1);
                var rectTransform = blockerObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = TopmostCanvas.GetComponent<RectTransform>().sizeDelta;
                rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                rectTransform.ResetAnchorToStretchAll();

                // If parent is null set it to a global Parent
                if (parent == null)
                {
                    parent = Parent;
                }

                blockerObject.transform.SetParent(parent, false);
                blockerObject.transform.SetSiblingIndex(siblingIndex);
            }
            {
                var unmasksObject = new GameObject("Unmasks");
                unmasksObject.transform.localScale = new Vector3(1, 1, 1);
                var rectTransform = unmasksObject.AddComponent<RectTransform>();
                rectTransform.ResetAnchorToStretchAll();
                unmasksObject.transform.SetParent(blockerObject.transform, false);
                //unmasksObject.transform.SetSiblingIndex(transform.GetSiblingIndex());

                blocker.UnmasksObject = unmasksObject;
            }
            {
                var backgroundObject = new GameObject("Background");
                var rect = new Rect(0, 0, BackgroundTexture.width, BackgroundTexture.height);
                var sprite = Sprite.Create(BackgroundTexture, rect, new Vector2(0.5f, 0.5f), 1);
                var image = backgroundObject.AddComponent<Image>();
                image.material.mainTexture = BackgroundTexture;
                image.sprite = sprite;
                image.color = blockerColor ?? BlockerColor;
                image.canvasRenderer.SetAlpha(1 / 255f);
                image.CrossFadeAlpha(1.0f, OpenBlockerDuration, false);

                // Can use RectTransform Extensions...
                backgroundObject.transform.localScale = new Vector3(1, 1, 1);
                var rectTransform = backgroundObject.GetComponent<RectTransform>();
                rectTransform.ResetAnchorToStretchAll();
                backgroundObject.transform.SetParent(blockerObject.transform, false);
                //backgroundObject.transform.SetSiblingIndex(transform.GetSiblingIndex());

                blocker.BackgroundObject = backgroundObject;
            }

            if (onClickCallback != null)
            {
                blocker.Clicked.AddListener(onClickCallback);
            }

            return blocker;
        }

        public void RemoveBlocker()
        {
            if (m_blockers.Count == 0)
            {
                Debug.LogWarning("Nothing to remove :(");
                return;
            }

            Blocker blocker = m_blockers.Pop();
            var image = blocker.GetComponent<Image>();
            if (image != null)
            {
                image.CrossFadeAlpha(0.0f, CloseBlockerDuration, false);
            }

            blocker.StartCoroutine(DeleteBlockerDelayed(blocker.gameObject, CloseBlockerDuration));
        }

        private static IEnumerator DeleteBlockerDelayed(Object blockerObject, float duration)
        {
            while (duration >= 0)
            {
                duration -= Time.deltaTime;
                yield return null;
            }

            Destroy(blockerObject);
        }
    }
}
