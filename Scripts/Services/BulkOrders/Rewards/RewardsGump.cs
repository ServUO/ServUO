using System;
using Server.Engines.BulkOrders;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.BulkOrders
{
    public class RewardsGump : BaseRewardGump
    {
        public BODType BODType { get; private set; }

        public bool UsingBanked { get; set; }

        public override int YDist { get  { return 15; } }

        public RewardsGump(Mobile owner, PlayerMobile user, BODType type, int points = 0)
            : base(owner, user, BulkOrderSystem.GetRewardCollection(type), 1157082, points == 0 ? BulkOrderSystem.GetPoints(user, type) : points)
        {
            BODType = type;

            if (points > 0)
                BulkOrderSystem.AddToPending(User, type, points);
            else
                UsingBanked = true;

            var entry = new GumpLabel(230, 65, 0x64, GetPoints(user).ToString("0.000000"));
            entry.Parent = this;

            Entries.Insert(10, entry);
        }

        protected override void AddPoints()
        {
        }

        public override double GetPoints(Mobile m)
        {
            if (Points > 0)
                return Points;

            return BulkOrderSystem.GetPoints(m, this.BODType);
        }

        public override void OnConfirmed(CollectionItem citem, int index)
        {
            BODCollectionItem item = citem as BODCollectionItem;

            if (item != null && GetPoints(User) >= item.Points && item.Constructor != null)
            {
                Item i = item.Constructor(item.RewardType);

                if (User.Backpack == null || !User.Backpack.TryDropItem(User, i, false))
                {
                    User.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
                    i.Delete();

                    User.SendGump(new RewardsGump(Owner, User, this.BODType, (int)Points));
                }
                else
                {
                    User.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
                    User.PlaySound(0x5A7);

                    if (UsingBanked)
                    {
                        BulkOrderSystem.DeductPoints(User, this.BODType, item.Points);
                    }
                    else
                    {
                        BulkOrderSystem.RemovePending(User, this.BODType);
                    }
                }
            }
        }
    }
}