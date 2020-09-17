using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class ClockworkPuzzleQuest : BaseQuest
    {
        public ClockworkPuzzleQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(ClockParts), "clock parts", 5, 0x104F));

            AddReward(new BaseReward(typeof(TinkersSatchel), 1074282)); // Craftsman's Satchel
        }

        public override TimeSpan RestartDelay => TimeSpan.FromMinutes(3);
        /* A clockwork puzzle */
        public override object Title => 1075535;
        /* 'Tis a riddle, you see! "What kind of clock is only right twice per day? A broken one!" *laughs heartily* 
        Ah, yes *wipes eye*, that's one of my favorites! Ah... to business. Could you fashion me some clock parts? 
        I wish my own clocks to be right all the day long! You'll need some tinker's tools and some iron ingots, I 
        think, but from there it should be just a matter of working the metal. */
        public override object Description => 1075534;
        /* Or perhaps you'd rather not. */
        public override object Refuse => 1072981;
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete => 1072271;
        /* Wonderful! Tick tock, tick tock, soon all shall be well with grandfather's clock! */
        public override object Complete => 1075536;
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

    public class Nibbet : MondainQuester
    {
        [Constructable]
        public Nibbet()
            : base("Nibbet", "the tinker")
        {
        }

        public Nibbet(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(ClockworkPuzzleQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;
            Race = Race.Human;

            Hue = 0x840C;
            HairItemID = 0x2044;
            HairHue = 0x1;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Boots(0x591));
            AddItem(new ShortPants(0xF8));
            AddItem(new Shirt(0x2D));
            AddItem(new FullApron(0x288));

            Item item;

            item = new PlateGloves
            {
                Hue = 0x21E
            };
            AddItem(item);
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

    public class TinkersSatchel : Backpack
    {
        [Constructable]
        public TinkersSatchel()
            : base()
        {
            Hue = BaseReward.SatchelHue();

            AddItem(new TinkerTools());

            switch (Utility.Random(5))
            {
                case 0:
                    AddItem(new Springs(3));
                    break;
                case 1:
                    AddItem(new Axle(3));
                    break;
                case 2:
                    AddItem(new Hinge(3));
                    break;
                case 3:
                    AddItem(new Key());
                    break;
                case 4:
                    AddItem(new Scissors());
                    break;
            }
        }

        public TinkersSatchel(Serial serial)
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