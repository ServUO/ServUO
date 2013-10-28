using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class EmbracingHumanityQuest : BaseQuest
    { 
        public EmbracingHumanityQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(TreatForDrithen), "treat for Drithen", 1, typeof(Drithen), "Drithen (Umbra)"));
						
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromHours(1);
            }
        }// not there on OSI, but prevents farming

        /* Embracing Humanity */
        public override object Title
        {
            get
            {
                return 1074349;
            }
        }
        /* Well, I don't mind saying it -- I'm flabbergasted!  Absolutely astonished.  I just heard that some elves want to 
        convert themselves to humans through some magical process.  My cousin Nedrick does whatever needs doing.  I guess you 
        could check it out for yourself if you're curious.  Anyway, I wonder if you'll bring my cousin, Drithen, this here 
        treat my wife baked up for him special. */
        public override object Description
        {
            get
            {
                return 1074357;
            }
        }
        /* That's okay, I'll find someone else to make the delivery. */
        public override object Refuse
        {
            get
            {
                return 1074459;
            }
        }
        /* If I knew where my cousin was, I'd make the delivery myself. */
        public override object Uncomplete
        {
            get
            {
                return 1074460;
            }
        }
        /* Oh, hello there.  What do you have for me? */
        public override object Complete
        {
            get
            {
                return 1074461;
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

    public class Ioseph : MondainQuester
    {
        [Constructable]
        public Ioseph()
            : base("Ioseph", "the exporter")
        { 
        }

        public Ioseph(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(EmbracingHumanityQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Human;
			
            this.Hue = 0x8404;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Shoes(0x737));
            this.AddItem(new LongPants(0x1BB));
            this.AddItem(new FancyShirt(0x535));
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