using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{ 
    public class SomethingToWailAboutQuest : BaseQuest
    { 
        public SomethingToWailAboutQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(WailingBanshee), "wailing banshees", 12));
			
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Something to Wail About */
        public override object Title
        {
            get
            {
                return 1073071;
            }
        }
        /* Can you hear them? The never-ending howling? The incessant wailing? These banshees, they never cease! Never! They haunt 
        my nights. Please, I beg you -- will you silence them? I would be ever so grateful. */
        public override object Description
        {
            get
            {
                return 1073561;
            }
        }
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse
        {
            get
            {
                return 1073580;
            }
        }
        /* Until you kill 12 Wailing Banshees, there will be no peace. */
        public override object Uncomplete
        {
            get
            {
                return 1073581;
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

    public class RunawaysQuest : BaseQuest
    { 
        public RunawaysQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(FrenziedOstard), "frenzied ostards", 12));
			
            this.AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Runaways! */
        public override object Title
        {
            get
            {
                return 1072993;
            }
        }
        /* You've got to help me out! Those wild ostards have been causing absolute havok around here.  Kill them 
        off before they destroy my land.  There are around twelve of them. */
        public override object Description
        {
            get
            {
                return 1073026;
            }
        }
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse
        {
            get
            {
                return 1072270;
            }
        }
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete
        {
            get
            {
                return 1072271;
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

    public class ViciousPredatorQuest : BaseQuest
    { 
        public ViciousPredatorQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(DireWolf), "dire wolves ", 10));
			
            this.AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Vicious Predator */
        public override object Title
        {
            get
            {
                return 1072994;
            }
        }
        /* You've got to help me out! Those dire wolves have been causing absolute havok around here.  Kill them off 
        before they destroy my land.  They run around in a pack of around ten.<br> */
        public override object Description
        {
            get
            {
                return 1073028;
            }
        }
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse
        {
            get
            {
                return 1072270;
            }
        }
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete
        {
            get
            {
                return 1072271;
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

    public class Jelrice : MondainQuester
    {
        [Constructable]
        public Jelrice()
            : base("Jelrice", "the trader")
        { 
        }

        public Jelrice(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(SomethingToWailAboutQuest),
                    typeof(RunawaysQuest),
                    typeof(ViciousPredatorQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Human;
			
            this.Hue = 0x8410;
            this.HairItemID = 0x2047;
            this.HairHue = 0x471;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Shoes(0x1BB));
            this.AddItem(new Skirt(0xD));
            this.AddItem(new FancyShirt(0x65F));
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