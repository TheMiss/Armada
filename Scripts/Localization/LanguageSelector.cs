using Armageddon.Games;
using UnityEngine;

namespace Armageddon.Localization
{
    public class LanguageSelector : GameContext
    {
        protected override void Awake()
        {
            base.Awake();
            CanTick = true;
        }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    Localization.SetLanguage(0);
                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Localization.SetLanguage(1);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Localization.SetLanguage(2);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Localization.SetLanguage(3);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    Localization.SetLanguage(4);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    Localization.SetLanguage(5);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    Localization.SetLanguage(6);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    Localization.SetLanguage(7);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha8))
                {
                    Localization.SetLanguage(8);
                }
                // else if (Input.GetKeyDown(KeyCode.Alpha9))
                // {
                //     SetLanguage(9);
                // }
            }
        }
    }
}
