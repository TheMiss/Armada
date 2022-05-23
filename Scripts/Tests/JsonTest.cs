using System;
using System.Collections.Generic;
using System.IO;
using Armageddon.Mechanics.Characters;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Tests
{
    public class JsonTest : MonoBehaviour
    {
        public enum CharacterType
        {
            Alien,
            Predator
        }

        [Button]
        private void RunTest()
        {
            // var player = new PlayerData();
            // // string json = JsonUtility.ToJson(player);
            // string json = JsonConvert.SerializeObject(player);
            //
            // // Debug.Log(json);
            //
            // File.WriteAllText(@"D:\MyData.json", json);

            var player = new PlayerData();
            player.Age = 31;
            player.Gold = -999;
            player.Characters = new List<CharacterData>
            {
                new CharacterData { Health = 2000, Mana = 500, Type = CharacterType.Alien },
                new CharacterData { Health = 3500, Mana = 200, Type = CharacterType.Predator }
            };
            string json = JsonConvert.SerializeObject(player);

            File.WriteAllText(@"D:\MyData.json", json);

            Debug.Log(json);
        }

        [Button]
        private void RunTest2()
        {
            string json = File.ReadAllText(@"D:\MyData.json");
            var player = JsonConvert.DeserializeObject<PlayerData>(json);
            Debug.Log("RunTest2");
        }

        [Button]
        private void RunTest3()
        {
            var test = new List<PartData>();
            var part = new PartData { DyeId = 56, VariantIndex = 2 };

            test.Add(part);
            // string json = JsonConvert.SerializeObject(part);
            string json = JsonConvert.SerializeObject(test);

            File.WriteAllText(@"D:\MyData.json", json);
        }

        [Button]
        private void RunTest4()
        {
            var player = new PlayerData
            {
                Age = 31,
                Gold = -999,
                Characters = new List<CharacterData>
                {
                    new CharacterData { Health = 2000, Mana = 500, Type = CharacterType.Alien },
                    new CharacterData { Health = 3500, Mana = 200, Type = CharacterType.Predator }
                }
            };

            player.AddTest();

            string json = JsonConvert.SerializeObject(player);

            File.WriteAllText(@"D:\MyData.json", json);
        }

        [Button]
        private void RunTest5()
        {
            string json = File.ReadAllText(@"D:\MyData.json");
            var player = JsonConvert.DeserializeObject<PlayerData>(json);
            Debug.Log("RunTest5");
        }

        [Button]
        private void RunTest6()
        {
            var stageProgression = new StageProgressionEntity();
            stageProgression.SelectedStageIndex = 99;

            for (int i = 0; i < 100; i++)
            {
                int seed = 9999999;
                stageProgression.Seeds.Add(seed);
            }

            string jsonObjectString = JsonConvert.SerializeObject(stageProgression);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "FileSize.json"), jsonObjectString);
        }

        [Button]
        private void RunTest7()
        {
            int test = unchecked((int)-3364283419284786149);
            Debug.Log($"test = {test}");
        }

        [Serializable]
        private class StageProgressionEntity
        {
            public int SelectedStageIndex;
            public List<int> Seeds = new List<int>();
        }

        [Serializable]
        public class PlayerData
        {
            public int Gold;

            public int Age;

            // public int Level { pget; }
            public float AddedField1;
            public float AddedField2;

            public List<CharacterData> Characters;

            private List<CharacterData> m_list = new List<CharacterData>();

            public IReadOnlyList<CharacterData> ReadOnlyList
            {
                set => m_list = value as List<CharacterData>;
                get => m_list;
            }

            public void AddTest()
            {
                {
                    var newCharacter = new CharacterData
                    {
                        Health = 888,
                        Mana = 777,
                        Type = CharacterType.Alien,
                        AddedArmor = 555,
                        AddedDamage = 666
                    };

                    m_list.Add(newCharacter);
                }
                {
                    var newCharacter = new CharacterData
                    {
                        Health = 123,
                        Mana = 456,
                        Type = CharacterType.Predator,
                        AddedArmor = 555,
                        AddedDamage = 666
                    };

                    m_list.Add(newCharacter);
                }
            }
        }

        // [JsonObject(MemberSerialization.OptIn)]
        [Serializable]
        public class CharacterData
        {
            public int Health;
            public int Mana;
            public int AddedArmor;
            public int AddedDamage;
            public CharacterType Type;
        }
    }
}
