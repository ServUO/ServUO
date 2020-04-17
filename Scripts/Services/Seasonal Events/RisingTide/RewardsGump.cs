using Server.Engines.Points;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.RisingTide
{
    public class RisingTideRewardGump : BaseRewardGump
    {
        public override int PointsName => 1158916;  // Your Doubloons
        public override int RewardLabel => 1158917;  // What ye buyin' Matey?

        public RisingTideRewardGump(Mobile owner, PlayerMobile user)
            : base(owner, user, Rewards, 1158918)
        {
        }

        public override int GetYOffset(int id)
        {
            if (id == 0xA2C6)
            {
                return 70;
            }

            if (id == 0xA2C8)
            {
                return 50;
            }

            if (id == 0xA28B)
            {
                return 15;
            }

            return 20;
        }

        public override double GetPoints(Mobile m)
        {
            return PointsSystem.RisingTide.GetPoints(m);
        }

        public override void RemovePoints(double points)
        {
            PointsSystem.RisingTide.DeductPoints(User, points);
        }

        public override void OnItemCreated(Item item)
        {
            if (item is DecorativeWoodCarving)
            {
                ((DecorativeWoodCarving)item).AssignRandomName();
            }
            else if (item is ShoulderParrot)
            {
                ((ShoulderParrot)item).MasterName = User.Name;
            }
        }

        public static List<CollectionItem> Rewards { get; set; }

        public static void Initialize()
        {
            Rewards = new List<CollectionItem>();

            Rewards.Add(new CollectionItem(typeof(DragonCannonDeed), 0x14EF, 1158926, 0, 120000));
            Rewards.Add(new CollectionItem(typeof(BlundercannonDeed), 0x14F2, 1158942, 1126, 25000));
            Rewards.Add(new CollectionItem(typeof(PirateWallMap), 0xA2C8, 1158938, 0, 45000));
            Rewards.Add(new CollectionItem(typeof(MysteriousStatue), 0xA2C6, 1158935, 0, 35000));
            Rewards.Add(new CollectionItem(typeof(ShoulderParrot), 0xA2CA, 1158928, 0, 100000));
            Rewards.Add(new CollectionItem(typeof(DecorativeWoodCarving), 0x4C26, 1158943, 2968, 15000));
            Rewards.Add(new CollectionItem(typeof(QuartermasterRewardDeed), 0x14EF, 0, 0, 25000));
            Rewards.Add(new CollectionItem(typeof(SailingMasterRewardDeed), 0x14EF, 0, 0, 20000));
            Rewards.Add(new CollectionItem(typeof(BotswainRewardDeed), 0x14EF, 0, 0, 15000));
            Rewards.Add(new CollectionItem(typeof(PowderMonkeyRewardDeed), 0x14EF, 0, 0, 10000));
            Rewards.Add(new CollectionItem(typeof(SpikedWhipOfPlundering), 0xA28B, 0, 0, 180000));
            Rewards.Add(new CollectionItem(typeof(BladedWhipOfPlundering), 0xA28B, 0, 0, 180000));
            Rewards.Add(new CollectionItem(typeof(BarbedWhipOfPlundering), 0xA28B, 0, 0, 180000));
            Rewards.Add(new CollectionItem(typeof(TritonStatue), 0xA2D8, 0, 2713, 140000));
        }
    }
}
