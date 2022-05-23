using UnityEngine;
#if ENABLE_FACEBOOK_SIGN_IN
using Facebook.Unity;
#endif

#if ENABLE_GOOGLE_SIGN_IN
using Google;
#endif

namespace Armageddon.Games
{
    public static class SocialPlugins
    {
#if ENABLE_FACEBOOK_SIGN_IN
        public static void InitializeFacebook()
        {
            Debug.Log("InitializeFacebook");

            FB.Init(() => { });
        }
#endif


#if ENABLE_GOOGLE_SIGN_IN
        public static void InitializeGoogleSignIn()
        {
            Debug.Log("InitializeGooglePlay");

            string webClientId = "987098282046-7s6s32fjui164ifu0s7csvgf0bk62geh.apps.googleusercontent.com";
            var configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true
            };
        }
#endif
    }
}
