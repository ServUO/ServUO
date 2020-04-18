using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class ThePenIsMightierQuest : BaseQuest
    {
        public ThePenIsMightierQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(RecallScroll), "recall scroll", 5, 0x1F4C));

            AddReward(new BaseReward(typeof(RedLeatherBook), 1075545)); // a book bound in red leather
        }

        public override TimeSpan RestartDelay => TimeSpan.FromMinutes(3);
        /* The Pen is Mightier */
        public override object Title => 1075542;
        /* Do you know anything about 'Inscription?' I've been trying to get my hands on some hand crafted Recall 
        scrolls for a while now, and I could really use some help. I don't have a scribe's pen, let alone a 
        spellbook with Recall in it, or blank scrolls, so there's no way I can do it on my own. How about you 
        though? I could trade you one of my old leather bound books for some. */
        public override object Description => 1075543;
        /* Hmm, thought I had your interest there for a moment. It's not everyday you see a book made from real 
        daemon skin, after all! */
        public override object Refuse => 1075546;
        /* Inscribing... yes, you'll need a scribe's pen, some reagents, some blank scroll, and of course your own 
        magery book. You might want to visit the magery shop if you're lacking some materials. */
        public override object Uncomplete => 1075547;
        /* Ha! Finally! I've had a rune to the waterfalls near Justice Isle that I've been wanting to use for the 
        longest time, and now I can visit at last. Here's that book I promised you... glad to be rid of it, to be 
        honest. */
        public override object Complete => 1075548;
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

    public class Lyle : MondainQuester
    {
        [Constructable]
        public Lyle()
            : base("Lyle", "the mage")
        {
        }

        public Lyle(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(ThePenIsMightierQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;
            Race = Race.Human;

            Hue = 0x83F7;
            HairItemID = 0x204A;
            HairHue = 0x459;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new ThighBoots());
            AddItem(new Robe(0x2FD));
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

    public class RedLeatherBook : BaseBook
    {
        [Constructable]
        public RedLeatherBook()
            : base(0xFF2)
        {
            Hue = 0x485;
        }

        public RedLeatherBook(Serial serial)
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