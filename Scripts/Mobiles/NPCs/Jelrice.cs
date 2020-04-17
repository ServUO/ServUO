using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class SomethingToWailAboutQuest : BaseQuest
    {
        public SomethingToWailAboutQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(WailingBanshee), "wailing banshees", 12));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Something to Wail About */
        public override object Title => 1073071;
        /* Can you hear them? The never-ending howling? The incessant wailing? These banshees, they never cease! Never! They haunt 
        my nights. Please, I beg you -- will you silence them? I would be ever so grateful. */
        public override object Description => 1073561;
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse => 1073580;
        /* Until you kill 12 Wailing Banshees, there will be no peace. */
        public override object Uncomplete => 1073581;
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

    public class RunawaysQuest : BaseQuest
    {
        public RunawaysQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(FrenziedOstard), "frenzied ostards", 12));

            AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Runaways! */
        public override object Title => 1072993;
        /* You've got to help me out! Those wild ostards have been causing absolute havok around here.  Kill them 
        off before they destroy my land.  There are around twelve of them. */
        public override object Description => 1073026;
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse => 1072270;
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete => 1072271;
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

    public class ViciousPredatorQuest : BaseQuest
    {
        public ViciousPredatorQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(DireWolf), "dire wolves ", 10));

            AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Vicious Predator */
        public override object Title => 1072994;
        /* You've got to help me out! Those dire wolves have been causing absolute havok around here.  Kill them off 
        before they destroy my land.  They run around in a pack of around ten.<br> */
        public override object Description => 1073028;
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse => 1072270;
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete => 1072271;
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

        public override Type[] Quests => new Type[]
                {
                    typeof(SomethingToWailAboutQuest),
                    typeof(RunawaysQuest),
                    typeof(ViciousPredatorQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Human;

            Hue = 0x8410;
            HairItemID = 0x2047;
            HairHue = 0x471;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Shoes(0x1BB));
            AddItem(new Skirt(0xD));
            AddItem(new FancyShirt(0x65F));
        }

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