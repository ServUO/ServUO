using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class ABrokenVaseQuest : BaseQuest
    { 
        public ABrokenVaseQuest()
            : base()

        {
            this.AddObjective(new ObtainObjective(typeof(AncientPotteryFragments), "Ancient Pottery Fragments", 10, 0x223B));
			
            this.AddReward(new BaseReward(typeof(MeagerMuseumBag), 1112993));
        }

        // public override void GiveRewards()
        // {
        // if ( Owner is PlayerMobile ) 
        // ((PlayerMobile)Owner).Exp += 50;

        /*A Broken Vase */
        public override object Title
        {
            get
            {
                return 1112795;
            }
        }
        public override object Description
        {
            get
            {
                return 1112917;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112918;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112919;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112920;
            }
        }
        //// Owner.SendMessage("You have also been awarded 5 Queens Loyalty Points!");

        // base.GiveRewards();
        // }
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

    public class PuttingThePiecesTogetherQuest : BaseQuest
    { 
        public PuttingThePiecesTogetherQuest()
            : base()
        {
            this.AddObjective(new ObtainObjective(typeof(TatteredAncientScroll), "Tattered Ancient Scrolls", 5, 0x2F5F));
			
            this.AddReward(new BaseReward(typeof(DustyMuseumBag), 1112994));
        }

        /* Putting The Pieces Together */
        public override object Title
        {
            get
            {
                return 1112796;
            }
        }
        public override object Description
        {
            get
            {
                return 1112921;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112922;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112923;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112924;
            }
        }
        // public override void GiveRewards()
        // {
        // if (Owner is PlayerMobile)
        // ((PlayerMobile)Owner).Exp += 100;
        // Owner.SendMessage("You have also been awarded 10 Queens Loyalty Points!");
        // base.GiveRewards();
        // }
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

    public class YeOldeGargishQuest : BaseQuest
    { 
        public YeOldeGargishQuest()
            : base()
        {
            this.AddObjective(new ObtainObjective(typeof(UntransTome), "Untranslated Ancient Tome", 1, 0xEFA));
					
            this.AddReward(new BaseReward(typeof(BulgingMuseumBag), 1112995));
        }

        /* Ye Olde Gargish */
        public override object Title
        {
            get
            {
                return 1112797;
            }
        }
        public override object Description
        {
            get
            {
                return 1112925;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112926;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112927;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112928;
            }
        }
        /// public override void GiveRewards()

        // {
        // if (Owner is PlayerMobile)
        // ((PlayerMobile)Owner).Exp += 500;
        // Owner.SendMessage("You have also been awarded 50 Queens Loyalty Points!");
        // base.GiveRewards();
        // }
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

    public class Axem : MondainQuester
    {
        [Constructable]
        public Axem()
            : base("Axem", "the Curator")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Axem(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(ABrokenVaseQuest),
                    typeof(PuttingThePiecesTogetherQuest),
                    typeof(YeOldeGargishQuest),
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