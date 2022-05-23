using System.Collections.Generic;
using Armageddon.AssetManagement;
using Armageddon.Extensions;
using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.Maps;
using Armageddon.Sheets.Actors;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.UI.MainMenu.World.StartGame
{
    public class StageInfoWindow : Window
    {
        [BoxGroupPrefabs]
        [SerializeField]
        private EnemyElement m_enemyElementPrefab;

        [Required]
        [SerializeField]
        private List<StageInfoRewardBar> m_rewardBars;

        [SerializeField]
        private RectTransform m_possibleEncountersContentTransform;

        private List<EnemyElement> m_enemyElements = new();

        public void SetRewardBars(Stage stage)
        {
            m_possibleEncountersContentTransform.DestroyDesignRemnant();

            m_rewardBars.SetData(stage);
            // for (int i = 0; i < m_rewardBars.Count; i++)
            // {
            //     StageInfoRewardBar rewardBar = m_rewardBars[i];
            //     ScoreRequirementObject scoreRequirement = stage.Object.ScoreRequirements[i];
            //     RewardObject reward = stage.Object.Rewards[i];
            //
            //     rewardBar.SetRewardAsync(reward).Forget();
            //     rewardBar.StarToggle.isOn = i < stage.Object.StarCount;
            // }

            SetPossibleEncounters(stage.Payload.PossibleEncounters);
            // SetPossibleEncountersAsync(stage.Object.possibleEncounters).Forget();
        }

        private void SetPossibleEncounters(List<int> enemySheetIds)
        {
            foreach (EnemyElement enemyElement in m_enemyElements)
            {
                enemyElement.Selected.RemoveAllListeners();
                enemyElement.Deselected.RemoveAllListeners();
                DestroyGameObject(enemyElement);
            }

            m_enemyElements = new List<EnemyElement>();

            foreach (int enemySheetId in enemySheetIds)
            {
                var enemySheet = Assets.LoadSheet<EnemySheet>(enemySheetId);
                EnemyElement enemyElement = Instantiate(m_enemyElementPrefab, m_possibleEncountersContentTransform);
                enemyElement.Initialize(enemySheet);
                enemyElement.Selected.AddListener(OnEnemyElementSelected);
                enemyElement.Deselected.AddListener(OnEnemyElementDeselected);

                m_enemyElements.Add(enemyElement);
            }
        }

        private async UniTask SetPossibleEncountersAsync(List<int> enemySheetIds)
        {
            foreach (EnemyElement enemyElement in m_enemyElements)
            {
                enemyElement.Selected.RemoveAllListeners();
                enemyElement.Deselected.RemoveAllListeners();
                DestroyGameObject(enemyElement);
            }

            m_enemyElements = new List<EnemyElement>();

            foreach (int enemySheetId in enemySheetIds)
            {
                EnemySheet enemySheet = await Assets.LoadEnemySheetAsync(enemySheetId);
                EnemyElement enemyElement = Instantiate(m_enemyElementPrefab, m_possibleEncountersContentTransform);
                enemyElement.Initialize(enemySheet);
                enemyElement.Selected.AddListener(OnEnemyElementSelected);
                enemyElement.Deselected.AddListener(OnEnemyElementDeselected);

                m_enemyElements.Add(enemyElement);
            }
        }

        private void OnEnemyElementSelected(EnemyElement element)
        {
        }

        private void OnEnemyElementDeselected(EnemyElement element)
        {
        }
    }
}
