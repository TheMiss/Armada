using Purity.Common.Extensions;
using UnityEngine;

namespace Armageddon.Tests
{
    public class ConvertEnumToString : MonoBehaviour
    {
        public enum MyEnum
        {
            XMLFile,
            NotXML,
            EditorOnly,
            EditorAndRuntime
        }

        [ContextMenu("Tests")]
        private void Start()
        {
            for (int i = (int)MyEnum.XMLFile; i <= (int)MyEnum.EditorAndRuntime; i++)
            {
                var myEnum = (MyEnum)i;

                string humanReadable = myEnum.ToString().ToHumanReadable();

                //Debug.Log(humanReadable);

                Debug.Log(myEnum.ToHumanReadable());
            }
        }
    }
}
