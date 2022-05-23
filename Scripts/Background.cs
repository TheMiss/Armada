using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Background : MonoBehaviour
    {
        [SerializeField]
        private BackgroundCollection m_backgroundCollection;

        [SerializeField]
        private SpriteRenderer m_spriteRenderer;

        private void Start()
        {
            AdjustScaleToFitScreen();
        }

        public void Change(int index)
        {
            BackgroundInfo backgroundInfo = m_backgroundCollection.Backgrounds[index];
            m_spriteRenderer.sprite = backgroundInfo.Sprite;

            AdjustScaleToFitScreen();
        }

        [Button]
        private void AdjustScaleToFitScreen()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();

            float worldScreenHeight = Camera.main.orthographicSize * 2;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            transform.localScale = new Vector3(
                worldScreenWidth / spriteRenderer.sprite.bounds.size.x,
                worldScreenHeight / spriteRenderer.sprite.bounds.size.y, 1);
        }
    }
}
