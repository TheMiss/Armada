// using System.Collections.Generic;
// using System.Linq;
// using Armageddon.BackendDrivers.Exchanges;
// using Armageddon.BackendDrivers.Functions;
// using Armageddon.Extensions;
// using Armageddon.Persistence;
// using Armageddon.Worlds.Actors.Heroes;
// using ArmageddonAuthoring.Economy;
// using Cysharp.Threading.Tasks;
// using UnityEngine;
//
// namespace ArmageddonAuthoring.Armory
// {
//     public static class ArmoryUtility
//     {
//         public static string GetFakeHeroInstanceId(HeroActor heroActor)
//         {
//             HeroBlock sheet = HeroBlockTable.Instance.Entries.FirstOrDefault(x => x.Sheet.Name == heroActor.name);
//             if (sheet != null)
//             {
//                 GetFakeInstanceId(sheet.Id);
//             }
//
//             Debug.LogError($"Cannot find id for {heroActor.name}");
//             return string.Empty;
//         }
//
//         public static string GetFakeInstanceId(int sheetId)
//         {
//             return ((long)(sheetId + 10000)).ToHex();
//         }
//
//         public static async UniTask<Player> ForgePlayer()
//         {
//             var reply = new LoadPlayerReply 
//             {
//                 balances = new Dictionary<string, CurrencyObject>(),
//                 heroes = new HeroObject[0],
//                 lockedHeroes = new LockedHeroObject[0],
//                 heroInstanceId = ((long)-1).ToHex(),
//                 maxInventoryCount = 0,
//                 items = new ItemObject[0],
//                 heroInventory = new HeroInventoryObject { slots = new SlotObject[6]},
//                 map = new MapObject { stages = new StageObject[0]},
//             };
//             
//             var heroObjects = new List<HeroObject>();
//             foreach (HeroBlock heroBlock in HeroBlockTable.Instance.Entries)
//             {
//                 var heroObject = new HeroObject
//                 {
//                     instanceId = GetFakeInstanceId(heroBlock.Id), 
//                     sheetId = heroBlock.Id,
//                     dyeIds = new int[5],
//                 };
//
//                 heroObjects.Add(heroObject);
//             }
//
//             reply.heroes = heroObjects.ToArray();
//
//             var player = new Player();
//             await player.InitializeAsync(reply);
//
//             return player;
//         }
//     }
// }



