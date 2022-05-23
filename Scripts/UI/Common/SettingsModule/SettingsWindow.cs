using System.Collections.Generic;
using Armageddon.Localization;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Purity.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Dropdown = Armageddon.UI.Base.Dropdown;

namespace Armageddon.UI.Common.SettingsModule
{
    public class SettingsWindow : Window
    {
        [SerializeField]
        private Button m_closeButton;

        [SerializeField]
        private Dropdown m_languageDropdown;

        [SerializeField]
        private Slider m_musicSlider;

        [SerializeField]
        private Slider m_soundEffectsSlider;

        [SerializeField]
        private Toggle m_vibrationToggle;

        [SerializeField]
        private Toggle m_fpsToggle;

        [SerializeField]
        private Toggle m_damageTextToggle;

        [SerializeField]
        private Toggle m_dailyLogin;

        [SerializeField]
        private Button m_redeemCodeButton;

        [ShowInPlayMode]
        private int m_previousLanguageIndex;
        
        protected override void Awake()
        {
            base.Awake();

            m_closeButton.onClick.AddListener(OnCloseButtonClicked);

            m_languageDropdown.OnDropdownClicked.AddListener(OnLanguageDropdownClicked);
            m_languageDropdown.onValueChanged.AddListener(OnLanguageDropdownValueChanged);

            m_languageDropdown.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>();
            foreach (string language in LocalizationManager.GetAllLanguages())
            {
                var option = new TMP_Dropdown.OptionData($"{language}");
                options.Add(option);
            }

            m_languageDropdown.AddOptions(options);
        }

        private void OnCloseButtonClicked()
        {
            CloseDialog();
        }

        private void OnLanguageDropdownClicked()
        {
            // async UniTask Async()
            // {
            //     string language = LocalizationManager.GetAllLanguages()[m_languageDropdown.value];
            //     string titleText = $"{Texts.UI.Attention}!";
            //     string messageText = Lexicon.ConfirmChangeLanguage(language);
            //     string acceptButtonText = Texts.UI.Yes;
            //     string rejectButtonText = Texts.UI.No;
            //     bool? result = await UI.AlertDialog.ShowInfoDialogAsync(titleText, messageText,
            //         acceptButtonText, rejectButtonText);
            //
            //     if (result == true)
            //     {
            //         UI.Game.Localization.SetLanguage(m_languageDropdown.value);
            //     }
            // }
            //
            // Async().Forget();
        }

        private void OnLanguageDropdownValueChanged(int index)
        {
            async UniTask Async()
            {
                Debug.Log($"{index}");
                
                string language = LocalizationManager.GetAllLanguages()[m_languageDropdown.value];
                string titleText = $"{Texts.UI.Attention}!";
                string messageText = Lexicon.ConfirmChangeLanguage(language);
                string acceptButtonText = Texts.UI.Yes;
                string rejectButtonText = Texts.UI.No;
                bool? result = await UI.AlertDialog.ShowInfoDialogAsync(titleText, messageText,
                    acceptButtonText, rejectButtonText);

                if (result == true)
                {
                    UI.Game.Localization.SetLanguage(index);
                    m_previousLanguageIndex = index;
                }
                else
                {
                    m_languageDropdown.SetValueWithoutNotify(m_previousLanguageIndex);
                }
            }

            Async().Forget();
        }
    }
}
