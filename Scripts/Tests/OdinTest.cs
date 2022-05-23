using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Tests
{
    [Serializable]
    public class OdinTable
    {
        [ShowIf(nameof(Toggled))]
        public string MyName;

        [ShowIf(nameof(Toggled2))]
        public string MyName2;

        public string MyName3;

        public bool Toggled2;

        public virtual bool Toggled { set; get; }
    }

    [Serializable]
    public class OdinTableA : OdinTable
    {
        public int FuckCount;

        public override bool Toggled { get; set; } = true;
    }

    public class OdinTest : MonoBehaviour
    {
        public string Test1;
        public OdinTableA OdinTableA;
    }
}
