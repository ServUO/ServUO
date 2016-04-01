using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class ClockworkPuzzleQuest : BaseQuest
    { 
        public ClockworkPuzzleQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(ClockParts), "clock parts", 5, 0x104F));
						
            this.AddReward(new BaseReward(typeof(TinkersSatchel), 1074282)); // Craftsman's Satchel
        }

        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromMinutes(3);
            }
        }
        /* A clockwork puzzle */
        public override object Title
        {
            get
            {
                return 1075535;
            }
        }
        /* 'Tis a riddle, you see! "What kind of clock is only right twice per day? A broken one!" *laughs heartily* 
        Ah, yes *wipes eye*, that's one of my favorites! Ah... to business. Could you fashion me some clock parts? 
        I wish my own clocks to be right all the day long! You'll need some tinker's tools and some iron ingots, I 
        think, but from there it should be just a matter of working the metal. */
        public override object Description
        {
            get
            {
                return 1075534;
            }
        }
        /* Or perhaps you'd rather not. */
        public override object Refuse
        {
            get
            {
                return 1072981;
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
        /* Wonderful! Tick tock, tick tock, soon all shall be well with grandfather's clock! */
        public override object Complete
        {
            get
            {
                return 1075536;
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

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(ClockworkPuzzleQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.CantWalk = true;
            this.Race = Race.Human;
			
            this.Hue = 0x840C;
            this.HairItemID = 0x2044;
            this.HairHue = 0x1;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());			
            this.AddItem(new Boots(0x591));
            this.AddItem(new ShortPants(0xF8));
            this.AddItem(new Shirt(0x2D));
            this.AddItem(new FullApron(0x288));
			
            Item item;
			
            item = new PlateGloves();
            item.Hue = 0x21E;
            this.AddItem(item);
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

    public class TinkersSatchel : Backpack
    {
        [Constructable]
        public TinkersSatchel()
            : base()
        {
            this.Hue = BaseReward.SatchelHue();
			
            this.AddItem(new TinkerTools());
			
            switch ( Utility.Random(5) )
            { 
                case 0:
                    this.AddItem(new Springs(3));
                    break;
                case 1:
                    this.AddItem(new Axle(3));
                    break;
                case 2:
                    this.AddItem(new Hinge(3));
                    break;
                case 3:
                    this.AddItem(new Key());
                    break;
                case 4:
                    this.AddItem(new Scissors());
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}