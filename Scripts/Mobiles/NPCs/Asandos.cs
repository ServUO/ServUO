using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class BakersDozenQuest : BaseQuest
    {
        public BakersDozenQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(CookieMix), "cookie mix", 5, 0x103F));

            AddReward(new BaseReward(typeof(ChefsSatchel), 1074282)); // Craftsman's Satchel
        }

        /* Baker's Dozen */
        public override object Title => 1075478;
        /* You there! Do you know much about the ways of cooking? If you help me out, I'll show you a thing or two about 
        how it's done. Bring me some cookie mix, about 5 batches will do it, and I will reward you. Although, I don't 
        think you can buy it, you can make some in a snap! First get a rolling pin or frying pan or even a flour sifter. 
        Then you mix one pinch of flour with some water and you've got some dough! Take that dough and add one dollop of 
        honey and you've got sweet dough. add one more drop of honey and you've got cookie mix. See? Nothing to it! Now 
        get to work! */
        public override object Description => 1075479;
        /* Argh, I absolutely must have more of these 'cookies!' Come back if you change your mind. */
        public override object Refuse => 1075480;
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete => 1072271;
        /* Thank you! I haven't been this excited about food in months! */
        public override object Complete => 1075481;
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

    public class Asandos : MondainQuester
    {
        [Constructable]
        public Asandos()
            : base("Asandos", "the chef")
        {
        }

        public Asandos(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(BakersDozenQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;
            Race = Race.Human;

            Hue = 0x83FF;
            HairItemID = 0x2044;
            HairHue = 0x1;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Boots(0x901));
            AddItem(new ShortPants());
            AddItem(new Shirt());
            AddItem(new Cap());
            AddItem(new HalfApron(0x28));
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

    public class ChefsSatchel : Backpack
    {
        [Constructable]
        public ChefsSatchel()
            : base()
        {
            Hue = BaseReward.SatchelHue();

            AddItem(new SackFlour());
            AddItem(new Skillet());
        }

        public ChefsSatchel(Serial serial)
            : base(serial)
        {
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