using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Armageddon.UI.Base
{
    public class Tab : Widget
    {
        [SerializeField]
        private Image m_backgroundImage;

        [SerializeField]
        private GameObject m_iconObject;

        [FormerlySerializedAs("m_window")]
        [FormerlySerializedAs("m_page")]
        [SerializeField]
        private TabWindow m_tabWindow;

        [SerializeField]
        private TextMeshProUGUI m_text;

        [ReadOnly]
        public int Index;

        public TabWindow TabWindow => m_tabWindow;

        public void SetColor(Color backgroundColor, Color textColor)
        {
            m_backgroundImage.color = backgroundColor;
            m_text.color = textColor;
        }

        public override void Tick()
        {
            if (name == "HomeTab")
            {
                Debug.Log(Transform.position);
            }
        }

        public void Select(float iconScale, float textScale, float tweenDuration)
        {
            if (m_iconObject != null)
            {
                var iconScaleVector = new Vector2(iconScale, iconScale);
                m_iconObject.transform.DOScale(iconScaleVector, tweenDuration);
            }

            if (m_text != null)
            {
                var textScaleVector = new Vector2(textScale, textScale);
                m_text.transform.DOScale(textScaleVector, tweenDuration);
            }
        }

        public void Deselect(float tweenDuration)
        {
            if (m_iconObject != null)
            {
                m_iconObject.transform.DOScale(Vector3.one, tweenDuration);
            }

            if (m_text != null)
            {
                m_text.transform.DOScale(Vector3.one, tweenDuration);
            }
        }
    }
}
