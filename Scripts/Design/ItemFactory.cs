using System.Collections.Generic;
using System.IO;
using Armageddon.Backend.Payloads;
using Armageddon.Extensions;
using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.Items;
using Cysharp.Threading.Tasks;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Design
{
    public class ItemFactory : SandboxContext
    {
        [DisplayAsString]
        public long ItemInstanceId = 100000;

        [ListDrawerSettings(ShowIndexLabels = false)]
        public List<SandboxItem> SandboxItems;

        [ButtonGroup]
        [PropertyOrder(-100)]
        [GUIColorDefaultButton]
        private void LoadSandboxItems()
        {
            // foreach (Transform childTransform in transform)
            // {
            //     DestroyImmediate(childTransform.gameObject);
            // }

            foreach (SandboxItem sandboxItem in SandboxItems)
            {
                if (sandboxItem != null)
                {
                    DestroyImmediate(sandboxItem.gameObject);
                }
            }

            SandboxItems = new List<SandboxItem>();

            string path = $"{Application.persistentDataPath}/{SandboxItem.ItemObjectsPath}";

            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                var itemObject = FileEx.ReadObjectFromJson<ItemPayload>(file);

                var sandboxItemObject = new GameObject();
                var sandboxItem = sandboxItemObject.AddComponent<SandboxItem>();
                sandboxItem.transform.SetParent(Transform);
                sandboxItem.ItemPayload = itemObject;
                sandboxItem.Load();
                sandboxItem.ForceSetName();

                SandboxItems.Add(sandboxItem);
            }

            Debug.Log("Loaded ItemObjects!");
        }

        [ButtonGroup]
        [PropertyOrder(-100)]
        [GUIColorDefaultButton]
        private void SaveSandboxItems()
        {
            foreach (SandboxItem sandboxItem in SandboxItems)
            {
                sandboxItem.Save();
            }

            Debug.Log("Saved SandboxItems!");
        }

        [ButtonGroup]
        [PropertyOrder(-100)]
        [GUIColorDefaultButton]
        private void CreateSandboxItem()
        {
            var itemObject = new ItemPayload { InstanceId = ItemInstanceId.ToHex() };

            ItemInstanceId++;

            var sandboxItemObject = new GameObject("New Sandbox Item");
            var sandboxItem = sandboxItemObject.AddComponent<SandboxItem>();
            sandboxItem.transform.SetParent(Transform);
            sandboxItem.ItemPayload = itemObject;

            SandboxItems.Add(sandboxItem);
        }

        private SandboxItem GetItemObject(long itemInstanceId)
        {
            return SandboxItems.Find(x => x.ItemPayload.InstanceId == itemInstanceId.ToHex());
        }

        public async UniTask<Item> GetItem(long itemInstanceId)
        {
            SandboxItem sandboxItem = GetItemObject(itemInstanceId);

            if (sandboxItem == null)
            {
                return null;
            }

            return await Item.CreateAsync(sandboxItem.ItemPayload);
        }
    }
}
