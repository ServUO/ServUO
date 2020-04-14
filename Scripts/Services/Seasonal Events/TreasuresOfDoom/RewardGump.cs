using Server.Engines.Points;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.TreasuresOfDoom
{
    public class DoomRewardGump : BaseRewardGump
    {
        public DoomRewardGump(Mobile owner, PlayerMobile user)
            : base(owner, user, DoomRewards.Rewards, 1155595)
        {
        }

        public override int GetYOffset(int id)
        {
            return 15;
        }

        public override double GetPoints(Mobile m)
        {
            return PointsSystem.TreasuresOfDoom.GetPoints(m);
        }

        public override void OnConfirmed(CollectionItem citem, int index)
        {
            Item item = null;

            if (citem.Type == typeof(TreasuresOfDoomRewardDeed))
            {
                item = new TreasuresOfKotlRewardDeed(citem.Tooltip);
            }
            else
            {
                item = Loot.Construct(citem.Type);
            }

            if (item != null)
            {
                if (item is LanternOfLight)
                {
                    ((LanternOfLight)item).OwnerName = User.Name;
                }

                if (User.Backpack == null || !User.Backpack.TryDropItem(User, item, false))
                {
                    User.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
                    item.Delete();
                }
                else
                {
                    User.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
                    User.PlaySound(0x5A7);
                }
            }
            else
            {
                base.OnConfirmed(citem, index);
            }

            PointsSystem.TreasuresOfDoom.DeductPoints(User, citem.Points);
        }
    }
}