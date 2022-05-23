using UnityEngine;

namespace Armageddon.UI.MainMenu.PremiumShop
{
    internal class JsonData
    {
        // JSON Fields, ! Case-sensitive

        public string orderId;
        public string packageName;
        public string productId;
        public long purchaseTime;
        public int purchaseState;
        public string purchaseToken;
    }

    internal class PayloadData
    {
        internal JsonData JsonData;

        // JSON Fields, ! Case-sensitive
        public string signature;
        public string json;

        public static PayloadData FromJson(string json)
        {
            var payload = JsonUtility.FromJson<PayloadData>(json);
            payload.JsonData = JsonUtility.FromJson<JsonData>(payload.json);

            return payload;
        }
    }

    internal class GooglePurchase
    {
        public PayloadData PayloadData;

        // JSON Fields, ! Case-sensitive
        public string Store;
        public string TransactionID;
        public string Payload;

        public static GooglePurchase FromJson(string json)
        {
            var purchase = JsonUtility.FromJson<GooglePurchase>(json);
            purchase.PayloadData = PayloadData.FromJson(purchase.Payload);
            return purchase;
        }
    }
}
