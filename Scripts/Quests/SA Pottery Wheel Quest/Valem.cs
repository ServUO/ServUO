using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class PotteryWheelQuest : BaseQuest
    {
        public PotteryWheelQuest()
            : base()
        {
            this.AddObjective(new ObtainObjective(typeof(AncientPotteryFragments), "Ancient Pottery Fragments", 15, 0x223B));                                                      
            
            this.AddReward(new BaseReward(typeof(PotteryWheelAddonDeed), "Pottery Wheel Addon Deed"));
        }

        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
        public override object Title
        {
            get
            {
                return "Pottery Wheel Quest";
            }
        }
        public override object Description
        {
            get
            {
                return "My grandfather has passed away and I need for someone to go and gather Ancient pottery fragments for me. They can be found on Toxic and Stone Sliths, also on Raptors or Enraged earth elementals. I will reward you with a pottery wheel deed, so that you too, may learn this wonderful craft.";
            }
        }
        public override object Refuse
        {
            get
            {
                return "That's a shame, I can't get the fragments myself.";
            }
        }
        public override object Uncomplete
        {
            get
            {
                return "You haven't brought me enough pottery fragments yet..";
            }
        }
        public override object Complete
        {
            get
            {
                return "Oh Wonderful! Here is the reward I promised!";
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

    public class Valem : MondainQuester
    {
        [Constructable]
        public Valem()
            : base("Valem", "the Pottery Dealer")
        {
        }

        public Valem(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(PotteryWheelQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.CantWalk = true;

            this.Body = 666;
            this.HairItemID = 16987;
            this.HairHue = 1801;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());

            this.AddItem(new GargishClothChest(Utility.RandomNeutralHue()));
            this.AddItem(new GargishClothKilt(Utility.RandomNeutralHue()));
            this.AddItem(new GargishClothLegs(Utility.RandomNeutralHue()));
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