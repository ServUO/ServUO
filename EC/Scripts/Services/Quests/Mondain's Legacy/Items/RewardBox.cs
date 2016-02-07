using System;
using Reward = Server.Engines.Quests.BaseReward;

namespace Server.Items
{
    public class RewardBox : WoodenBox
    {
        [Constructable]	
        public RewardBox()
            : base()
        {
            this.Hue = Reward.StrongboxHue();
			
            while (this.Items.Count < this.Amount)
            { 
                switch ( Utility.Random(4) )
                {
                    case 0:
                        this.DropItem(Reward.Armor());
                        break;
                    case 1:
                        this.DropItem(Reward.RangedWeapon());
                        break;
                    case 2:
                        this.DropItem(Reward.Weapon());
                        break;
                    case 3:
                        this.DropItem(Reward.Jewlery());
                        break;	
                }
            }
			
            if (0.25 > Utility.RandomDouble()) // check
                this.DropItem(Loot.RandomTalisman());
        }

        public RewardBox(Serial serial)
            : base(serial)
        {
        }

        public virtual int ItemAmount
        {
            get
            {
                return 6;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}