using System.Collections.Generic;
using System.Linq;
using Armageddon.Externals.OdinInspector;
using Armageddon.Games;
using Armageddon.UI;
using I2.Loc;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Localization
{
    public class LocalizationSystem : GameContext, ILocalizationParamsManager
    {
        private static readonly int ThaiHashCode = "Thai".GetHashCode();

        [OnValueChanged(nameof(OnLanguageChanged))]
        [ValueDropdown(nameof(AvailableLanguages))]
        public int LanguageIndex;

        [SerializeField]
        private List<Localize> m_localizes;
        
        [ShowInPlayMode]
        private Canvas m_mainCanvas;

        public Canvas MainCanvas
        {
            get
            {
                if (m_mainCanvas == null)
                {
                    m_mainCanvas = FindObjectOfType<UISystem>().GetComponent<Canvas>();
                }

                return m_mainCanvas;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            Localize[] localizes = GetComponentsInChildren<Localize>();

            foreach (Localize localize in localizes)
            {
                Localize foundLocalize = m_localizes.FirstOrDefault(x => x == localize);

                if (foundLocalize == null)
                {
                    Debug.LogWarning($"Better to add {localize.name} to LocalizeRegistry in the scene!");
                }
            }

            RegisterService(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            GetAllLocalizeComponents();

            foreach (Localize localize in m_localizes)
            {
                localize.LocalizeEvent.AddListener(OnBeforeLocalize);
            }

            if (!LocalizationManager.ParamManagers.Contains(this))
            {
                LocalizationManager.ParamManagers.Add(this);
                LocalizationManager.LocalizeAll(true);
            }
        }

        protected override void OnDisable()
        {
            foreach (Localize localize in m_localizes)
            {
                localize.LocalizeEvent.RemoveListener(OnBeforeLocalize);
            }

            LocalizationManager.ParamManagers.Remove(this);

            base.OnDisable();
        }

        public string GetParameterValue(string param)
        {
            // const string sprite = "<sprite>";
            // if (param.Contains(sprite))
            // {
            //     param = param.Replace(sprite, string.Empty);
            // }

            return null;
        }

        [Button(ButtonSizes.Large)]
        [PropertyOrder(-100)]
        [GUIColorDefaultButton]
        private void SetEnglishLanguage()
        {
            SetLanguage(1);
            SetLanguage(0);
        }

        [Button]
        private void GetAllLocalizeComponents()
        {
            m_localizes = MainCanvas.GetComponentsInChildren<Localize>().ToList();
        }

        private void OnBeforeLocalize()
        {
            if (LocalizationManager.CurrentLanguage.GetHashCode() == ThaiHashCode)
            {
                if (string.IsNullOrEmpty(Localize.MainTranslation))
                {
                    return;
                }

                Localize.MainTranslation = ThaiFontAdjuster.Adjust(Localize.MainTranslation);
            }
        }

        public void AddLocalize(Localize localize)
        {
            m_localizes.Add(localize);
        }

        public void RemoveLocalize(Localize localize)
        {
            m_localizes.Remove(localize);
        }

        private void OnLanguageChanged()
        {
            SetLanguage(LanguageIndex);
        }

        public void SetLanguage(int languageIndex)
        {
            string language = LocalizationManager.GetAllLanguages()[languageIndex];
            if (LocalizationManager.HasLanguage(language))
            {
                LocalizationManager.CurrentLanguage = language;
                LanguageIndex = languageIndex;
            }
        }

        private ValueDropdownList<int> AvailableLanguages()
        {
            List<string> languages = LocalizationManager.GetAllLanguages();

            var dropdown = new ValueDropdownList<int>();
            int index = 0;
            foreach (string language in languages)
            {
                dropdown.Add(language, index++);
            }

            return dropdown;
        }
    }
}
