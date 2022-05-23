using System.Collections;
using System.Collections.Generic;
using Armageddon.Extensions;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Armageddon.UI.Base;
using UnityEngine;

namespace Armageddon.UI.MainMenu.World.StartGame
{
    public class StartingItemsWindow : Window
    {
        [SerializeField]
        private StartingItemElement m_startingItemElementPrefab;

        [SerializeField]
        private StartingItemTooltip m_startingItemTooltip;

        [SerializeField]
        private RectTransform m_contentTransform;

        private readonly List<StartingItemElement> m_startingItems = new();

        public void SetItems(PlayerInventory playerInventory)
        {
            m_contentTransform.DestroyDesignRemnant();
            m_startingItemTooltip.gameObject.SetActive(false);

            foreach (InventorySlot slot in playerInventory.Slots)
            {
                if (slot.Object is Consumable consumable)
                {
                    StartingItemElement startingItemElement = m_startingItems.Find(x => x.Item == consumable);

                    if (startingItemElement == null)
                    {
                        startingItemElement = Instantiate(m_startingItemElementPrefab, m_contentTransform);
                        startingItemElement.Initialize(consumable);
                        startingItemElement.Selected.AddListener(OnStartingItemElementSelected);
                        startingItemElement.Deselected.AddListener(OnStartingItemElementDeselected);

                        m_startingItems.Add(startingItemElement);
                    }

                    startingItemElement.UseToggle.isOn = consumable.UseWhenStartGame;
                }
            }

            // TODO: Reorder when get new consumables.
        }

        private void OnStartingItemElementSelected(StartingItemElement element)
        {
            // Since LayoutGroup doesn't get update immediately so we will use coroutine to update very frame.
            // Also good to update position when dragging in scroll view.
            // Vector3 position = element.Transform.position;
            // position.y += element.GetComponent<RectTransform>().rect.height;
            //
            // m_startingItemTooltip.gameObject.SetActive(true);
            // m_startingItemTooltip.Transform.position = position;
            // m_startingItemTooltip.SetItem(element.Item);

            StopAllCoroutines();
            StartCoroutine(UpdateTooltipPosition(element));
        }

        private void OnStartingItemElementDeselected(StartingItemElement element)
        {
            m_startingItemTooltip.gameObject.SetActive(false);

            StopAllCoroutines();
        }

        private IEnumerator UpdateTooltipPosition(StartingItemElement element)
        {
            m_startingItemTooltip.gameObject.SetActive(true);
            m_startingItemTooltip.SetItem(element.Item);

            var tooltipRectTransform = m_startingItemTooltip.GetComponent<RectTransform>();
            var elementRectTransform = element.GetComponent<RectTransform>();
            var rootRectTransform = UI.GetComponent<RectTransform>();

            while (true)
            {
                Vector3 position = element.Transform.position;

                float tooltipHeight = tooltipRectTransform.rect.height;
                float elementHeight = elementRectTransform.rect.height;

                position.y += tooltipHeight * 0.5f + elementHeight * 0.5f;

                Vector3 rootPosition = rootRectTransform.position;
                Rect rootRect = rootRectTransform.rect;
                float leftBound = rootPosition.x - rootRect.width * 0.5f;
                float rightBound = rootPosition.x + rootRect.width * 0.5f;

                if (position.x - tooltipRectTransform.rect.width * 0.5f < leftBound)
                {
                    const float offsetX = 5;
                    position.x = leftBound + tooltipRectTransform.rect.width * 0.5f + offsetX;
                }
                else if (position.x + tooltipRectTransform.rect.width * 0.5f > rightBound)
                {
                    const float offsetX = 5;
                    position.x = rightBound - tooltipRectTransform.rect.width * 0.5f - offsetX;
                }

                m_startingItemTooltip.SetArrowPositionX(element.Transform.position.x);
                m_startingItemTooltip.Transform.position = position;

                yield return null;
            }
        }
    }
}
