using System;
using Armageddon.Assistance.BackendDrivers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Armageddon.Games.Actions
{
    public class LogIn : BaseGameActionTask
    {
        protected override async UniTaskVoid OnExecuteAsync()
        {
            Game.UI.WaitForServerResponse.Show();
            Game.UI.TopBar.Hide();
            Game.UI.TabPageBottomBar.Hide();

            AuthenticationType authenticationType = Game.AuthenticationType;

            SignInReply reply = authenticationType switch
            {
                AuthenticationType.Device => await Game.BackendDriver.SignInWithDeviceAsync(),
                AuthenticationType.RememberMe => await Game.BackendDriver.SignInWithRememberMeAsync(Game.RememberMeId),
                _ => null
            };

            // LogInReply reply = await Game.BackendDriver.LogInWithCustomIdAsync();

            DateTime? lastLoginTime = reply?.LastLoginTime;

            if (lastLoginTime != null)
            {
                DateTime localTime = TimeZoneInfo.ConvertTime(lastLoginTime.Value, TimeZoneInfo.Local);
                Debug.Log($"LastLoginTime: {localTime} (UTC: {reply.LastLoginTime})");
            }

            Game.UI.WaitForServerResponse.Hide();
            EndAction();
        }
    }
}
