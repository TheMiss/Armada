using System;
using Armageddon.Backend.Payloads;
using Armageddon.Extensions;
using Armageddon.UI.Base;
using Armageddon.Worlds.Actors.Characters;
using Armageddon.Worlds.Actors.Enemies;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;

namespace Armageddon.Worlds.Misc
{
    public class DropHud : Widget
    {
        public static Vector3 OffsetFromOwner = new Vector2(0, 1.63f);

        [SerializeField]
        private TextMeshProUGUI m_labelText;

        [SerializeField]
        private GameObject m_layoutObject;

        public DropActor Owner { get; set; }

        public void SetText(StageDrop stageDrop, Vector2 position)
        {
            // TODO: Localize
            if (stageDrop.Type == DropType.Item)
            {
                StageDropItem item = stageDrop.Item;
                m_labelText.gameObject.SetActive(true);
                m_labelText.Set($"{item.Sheet.Name} x{item.Quantity}");
            }

            Transform.position = position;
            StopAllCoroutines();
            StartCoroutine(RefreshLayout(m_layoutObject));

            CanTick = true;
        }

        // [Button]
        public void Execute(CharacterActor targetActor, Action<DropActor> targetHitCallback)
        {
            Transform.PlayScaleAnimation();
        }

        public override void Tick()
        {
            if (Owner != null)
            {
                Transform.position = Owner.Transform.position + OffsetFromOwner;
            }
        }
    }
}
