using System;
using Reward = Server.Engines.Quests.BaseReward;

namespace Server.Items
{
    public class BaseRewardBag : Bag
    {
        public BaseRewardBag()
            : base()
        {
            this.Hue = Reward.RewardBagHue();
			
            while (this.Items.Count < this.ItemAmount)
            { 
                if (0.05 > Utility.RandomDouble()) // check
                    this.DropItem(Loot.RandomTalisman());
                else 
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
            }
        }

        public BaseRewardBag(Serial serial)
            : base(serial)
        {
        }

        public virtual int ItemAmount
        {
            get
            {
                return 0;
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

    public class SmallTrinketBag : BaseRewardBag
    {
        [Constructable]
        public SmallTrinketBag()
            : base()
        { 
        }

        public SmallTrinketBag(Serial serial)
            : base(serial)
        {
        }

        public override int ItemAmount
        {
            get
            {
                return 1;
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

    public class TrinketBag : BaseRewardBag
    {
        [Constructable]
        public TrinketBag()
            : base()
        { 
        }

        public TrinketBag(Serial serial)
            : base(serial)
        {
        }

        public override int ItemAmount
        {
            get
            {
                return 2;
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

    public class TreasureBag : BaseRewardBag
    {
        [Constructable]
        public TreasureBag()
            : base()
        { 
        }

        public TreasureBag(Serial serial)
            : base(serial)
        {
        }

        public override int ItemAmount
        {
            get
            {
                return 3;
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

    public class LargeTreasureBag : BaseRewardBag
    {
        [Constructable]
        public LargeTreasureBag()
            : base()
        { 
        }

        public LargeTreasureBag(Serial serial)
            : base(serial)
        {
        }

        public override int ItemAmount
        {
            get
            {
                return 4;
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