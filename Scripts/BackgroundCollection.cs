using System;
using System.Collections.Generic;
using UnityEngine;

namespace Armageddon
{
    [Serializable]
    public class BackgroundInfo
    {
        public Sprite Sprite;
    }

    //[CreateAssetMenu(fileName = "BackgroundCollection", menuName = "ScriptableObjects/BackgroundCollection", order = 1)]
    public class BackgroundCollection : ScriptableObject
    {
        [SerializeField]
        private List<BackgroundInfo> m_backgrounds;

        public List<BackgroundInfo> Backgrounds => m_backgrounds;
    }
}
