using Armageddon.Backend.Payloads;
using Armageddon.Games;
using Armageddon.Localization;
using Armageddon.Mechanics;
using Armageddon.Mechanics.Characters;
using Armageddon.UI.Base;
using Armageddon.Worlds;
using Armageddon.Worlds.Actors.Heroes;
using Cysharp.Threading.Tasks;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.Loadout.HeroSelection
{
    public class InspectHeroWindow : Window
    {
        [SerializeField]
        private GameObject m_selectedObject;

        [SerializeField]
        private Button m_selectButton;

        [SerializeField]
        private Button m_unlockWithBlueCrystalButton;

        [SerializeField]
        private TextMeshProUGUI m_blueCrystalPriceText;

        [SerializeField]
        private Button m_unlockWithRedGemButton;

        [SerializeField]
        private TextMeshProUGUI m_redGemPriceText;

        [SerializeField]
        private HeroListWindow m_heroListWindow;

        [SerializeField]
        private RawImage m_previewDisplay;

        private HeroActor m_selectedHeroActor;

        public Hero InspectingHero { get; private set; }

        public HeroListWindow HeroListWindow => m_heroListWindow;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_selectButton.onClick.AddListener(OnSelectButtonClicked);
            m_unlockWithBlueCrystalButton.onClick.AddListener(OnUnlockByBlueCrystalButtonClicked);
            m_unlockWithRedGemButton.onClick.AddListener(OnUnlockByRedGemButtonClicked);
        }

        protected override void OnDisable()
        {
            m_selectButton.onClick.RemoveListener(OnSelectButtonClicked);
            m_unlockWithBlueCrystalButton.onClick.RemoveListener(OnUnlockByBlueCrystalButtonClicked);
            m_unlockWithRedGemButton.onClick.RemoveListener(OnUnlockByRedGemButtonClicked);

            UI.PreviewManager.HideHeroes(this);

            base.OnDisable();
        }

        public void Initialize()
        {
        }

        private void OnSelectButtonClicked()
        {
            SelectHeroAsync(InspectingHero).Forget();
        }

        private void OnUnlockByBlueCrystalButtonClicked()
        {
            UnlockHeroAsync(InspectingHero, CurrencyType.EvilHeart).Forget();
        }

        private void OnUnlockByRedGemButtonClicked()
        {
            UnlockHeroAsync(InspectingHero, CurrencyType.RedGem).Forget();
        }

        private async UniTaskVoid SelectHeroAsync(Hero hero)
        {
            var player = GetService<Player>();

            UI.WaitForServerResponse.Show();

            await player.SelectHeroAsync(hero);

            SetSelectedHero();
            await InspectHeroAsync(hero);

            UI.WaitForServerResponse.Hide();
        }

        private async UniTaskVoid UnlockHeroAsync(Hero hero, CurrencyType currencyType)
        {
            var player = GetService<Player>();
            if (!player.CanUnlockHero(hero, currencyType))
            {
                string titleText = Lexicon.InsufficientCurrency(currencyType);
                string messageText = Lexicon.InsufficientCurrencyDetails(currencyType);
                string acceptButtonText = Texts.UI.GotIt;

                UI.AlertDialog.ShowInfoDialogAsync(titleText, messageText, acceptButtonText).Forget();
                return;
            }

            bool? dialogResult = await UI.UnlockHeroConfirmDialog.ShowUnlockAsync(hero, currencyType);

            if (dialogResult != null && dialogResult.Value)
            {
                UI.WaitForServerResponse.Show();

                await player.UnlockHeroAsync(hero, currencyType);

                await InspectHeroAsync(hero);
                HeroListWindow.RefreshUnlockedNumber();
                HeroListWindow.FilterBar.FilterHeroes();

                UI.WaitForServerResponse.Hide();
            }
        }

        public void SetSelectedHero()
        {
            var player = GetService<Player>();

            InspectingHero ??= player.CurrentHero;

            m_selectedObject.SetActive(true);
            m_selectButton.gameObject.SetActive(false);
            m_unlockWithBlueCrystalButton.gameObject.SetActive(false);
            m_unlockWithRedGemButton.gameObject.SetActive(false);

            m_heroListWindow.SetSelectedHero(player.CurrentHero);
        }

        public async UniTask InspectHeroAsync(Hero hero)
        {
            if (InspectingHero == hero)
            {
                return;
            }

            var game = GetService<Game>();

            // if (m_selectedHeroActor != null)
            // {
            //     m_selectedHeroActor.gameObject.SetActive(false);
            // }

            // m_selectedHeroActor = await UI.PreviewManager.GetHeroAsync(hero);
            PreviewEntry entry = await UI.PreviewManager.ShowHeroAsync(this, hero);
            m_previewDisplay.texture = entry.Camera.targetTexture;
            InspectingHero = hero;

            if (game.Player.CurrentHero == hero)
            {
                m_selectedObject.SetActive(true);
                m_selectButton.gameObject.SetActive(false);
                m_unlockWithBlueCrystalButton.gameObject.SetActive(false);
                m_unlockWithRedGemButton.gameObject.SetActive(false);
                return;
            }

            m_selectedObject.SetActive(false);

            if (string.IsNullOrEmpty(hero.InstanceId))
            {
                m_selectButton.gameObject.SetActive(false);

                if (hero.Prices.TryGetValue("EH", out uint blueCrystalPrice))
                {
                    m_unlockWithBlueCrystalButton.gameObject.SetActive(true);

                    // TODO: Should we check if it's free or not
                    // if (blueCrystalPrice > 0)
                    {
                        // string text = $"Unlock\n<color=white>{blueCrystalPrice}</color> <sprite name=\"BC\">";
                        string text = Texts.Message.UnlockWithCrystalHeart;
                        text = TagHandler.ReplacePrice(text, CurrencyType.EvilHeart, blueCrystalPrice);
                        m_blueCrystalPriceText.Set(text);
                    }
                }
                else
                {
                    m_unlockWithBlueCrystalButton.gameObject.SetActive(false);
                }

                if (hero.Prices.TryGetValue("RG", out uint redGemPrice))
                {
                    m_unlockWithRedGemButton.gameObject.SetActive(true);

                    // TODO: Should we check if it's free or not
                    // if (redGemPrice > 0)
                    {
                        // string text = $"Unlock\n<color=white>{redGemPrice}</color> <sprite name=\"RG\">";
                        string text = Texts.Message.UnlockWithRedGem;
                        text = TagHandler.ReplacePrice(text, CurrencyType.RedGem, redGemPrice);
                        m_redGemPriceText.Set(text);
                    }
                }
                else
                {
                    m_unlockWithRedGemButton.gameObject.SetActive(false);
                }
            }
            else
            {
                m_selectButton.gameObject.SetActive(true);
                m_unlockWithBlueCrystalButton.gameObject.SetActive(false);
                m_unlockWithRedGemButton.gameObject.SetActive(false);
            }
        }

        public void OnHeroElementSelected(Hero hero)
        {
            if (InspectingHero == hero)
            {
                return;
            }

            InspectHeroAsync(hero).Forget();
        }
    }
}
