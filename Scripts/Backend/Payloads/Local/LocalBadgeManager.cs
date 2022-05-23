using System;
using System.Collections.Generic;
using Armageddon.Mechanics;
using Armageddon.Mechanics.Mails;
using Newtonsoft.Json;
using Purity.Common.Extensions;

namespace Armageddon.Backend.Payloads.Local
{
    [Serializable]
    public class LocalBadge
    {
        public string InstanceId;
        public bool IsNoticed;
    }

    [Serializable]
    public class LocalBadgeManager
    {
        public List<LocalBadge> Mails = new();

        [JsonIgnore]
        public bool IsDataChanged { get; set; }

        private void InitializeLocalsToBadges(List<LocalBadge> localBadges, List<IBadge> badges)
        {
            var removingLocalBadges = new List<LocalBadge>();

            foreach (LocalBadge localBadge in localBadges)
            {
                IBadge badge = badges.Find(x => x.InstanceId == localBadge.InstanceId);
                if (badge != null)
                {
                    badge.IsNoticed = localBadge.IsNoticed;
                }
                else
                {
                    removingLocalBadges.Add(localBadge);
                }
            }

            localBadges.RemoveRange(removingLocalBadges);
        }

        private void SetBadgesToLocals(List<IBadge> badges, List<LocalBadge> localBadges)
        {
            // Clear out the non-existing badges.
            for (int i = localBadges.Count - 1; i >= 0; i--)
            {
                LocalBadge localBadge = localBadges[i];

                if (!badges.Exists(x => x.InstanceId == localBadge.InstanceId))
                {
                    localBadges.RemoveAt(i);
                    IsDataChanged = true;
                }
            }

            // Modify or add a new local badge to the list.
            foreach (IBadge badge in badges)
            {
                LocalBadge localBadge = localBadges.Find(x => x.InstanceId == badge.InstanceId);
                if (localBadge == null)
                {
                    localBadge = new LocalBadge
                    {
                        InstanceId = badge.InstanceId,
                        IsNoticed = badge.IsNoticed
                    };

                    localBadges.Add(localBadge);

                    IsDataChanged = true;
                }
                else
                {
                    if (localBadge.IsNoticed != badge.IsNoticed)
                    {
                        localBadge.IsNoticed = true;
                        IsDataChanged = true;
                    }
                }
            }
        }

        public void InitializeMailBadges(List<Mail> mails)
        {
            InitializeLocalsToBadges(Mails, new List<IBadge>(mails));
        }

        public void SetToLocals(List<Mail> mails)
        {
            SetBadgesToLocals(new List<IBadge>(mails), Mails);
        }
    }
}
