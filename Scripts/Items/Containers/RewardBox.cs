using Reward = Server.Engines.Quests.BaseReward;

namespace Server.Items
{
    public class RewardBox : WoodenBox
    {
        [Constructable]
        public RewardBox()
            : base()
        {
            Hue = Reward.StrongboxHue();

            while (Items.Count < Amount)
            {
                switch (Utility.Random(4))
                {
                    case 0:
                        DropItem(Reward.Armor());
                        break;
                    case 1:
                        DropItem(Reward.RangedWeapon());
                        break;
                    case 2:
                        DropItem(Reward.Weapon());
                        break;
                    case 3:
                        DropItem(Reward.Jewlery());
                        break;
                }
            }

            if (0.25 > Utility.RandomDouble()) // check
                DropItem(Loot.RandomTalisman());
        }

        public RewardBox(Serial serial)
            : base(serial)
        {
        }

        public virtual int ItemAmount => 6;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}