using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class EmbracingHumanityQuest : BaseQuest
    {
        public EmbracingHumanityQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(TreatForDrithen), "treat for Drithen", 1, typeof(Drithen), "Drithen (Umbra)"));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        public override TimeSpan RestartDelay => TimeSpan.FromHours(1);// not there on OSI, but prevents farming

        /* Embracing Humanity */
        public override object Title => 1074349;
        /* Well, I don't mind saying it -- I'm flabbergasted!  Absolutely astonished.  I just heard that some elves want to 
        convert themselves to humans through some magical process.  My cousin Nedrick does whatever needs doing.  I guess you 
        could check it out for yourself if you're curious.  Anyway, I wonder if you'll bring my cousin, Drithen, this here 
        treat my wife baked up for him special. */
        public override object Description => 1074357;
        /* That's okay, I'll find someone else to make the delivery. */
        public override object Refuse => 1074459;
        /* If I knew where my cousin was, I'd make the delivery myself. */
        public override object Uncomplete => 1074460;
        /* Oh, hello there.  What do you have for me? */
        public override object Complete => 1074461;
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

        public override Type[] Quests => new Type[]
                {
                    typeof(EmbracingHumanityQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = 0x8404;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Shoes(0x737));
            AddItem(new LongPants(0x1BB));
            AddItem(new FancyShirt(0x535));
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