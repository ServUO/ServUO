using System;

namespace Server.Items
{
    public class TranslatedGargoyleJournal : BlueBook
    {
        public static readonly BookContent Content = new BookContent(
            "Translated Journal", "Velis",
            new BookPageInfo(
                "This text has been",
                "translated from a",
                "gargoyle's journal",
                "following his capture",
                "and subsequent",
                "reeducation.",
                "",
                "          -Velis"),
            new BookPageInfo(
                "I write this in the",
                "hopes that someday a",
                "soul of pure heart and",
                "mind will read it.  We",
                "are not the evil beings",
                "that our cousin",
                "gargoyles have made",
                "us out to be.  We"),
            new BookPageInfo(
                "consider them",
                "uncivilized and they",
                "have no concept of the",
                "Principles.  To you",
                "who reads this, I beg",
                "for your help in",
                "saving my brethern",
                "and preserving my"),
            new BookPageInfo(
                "race.  We stand at the",
                "edge of destruction as",
                "does the rest of the",
                "world.  Once it was",
                "written law that we",
                "would not allow the",
                "knowledge of our",
                "civilization to spread"),
            new BookPageInfo(
                "into the world, no we",
                "are left with little",
                "choice...contact the",
                "outside world in the hopes",
                "of finding help to save",
                "it or becoming the",
                "unwilling bringers of",
                "its damnation."),
            new BookPageInfo(
                "   I fear my capture is",
                "certain, the",
                "controllers grow ever",
                "closer to my hiding",
                "place and I know if",
                "they discover me, my",
                "fate will be as that of",
                "my brothers."),
            new BookPageInfo(
                "Although we resisted",
                "with all our strength",
                "it is now clear that we",
                "must have assistance",
                "or our people will be",
                "gone.  And if our",
                "oppressor achieves",
                "his goals our race will"),
            new BookPageInfo(
                "surely be joined buy",
                "others.",
                "   Those of us who",
                "have not yet been",
                "taken hope to open a",
                "path from the outside",
                "world into the city.",
                "We believe we have"),
            new BookPageInfo(
                "found weak areas in",
                "the mountains that we",
                "can successfully",
                "knock through with",
                "our limited supplies.",
                "We will have to work",
                "quickly and the risk",
                "of being discovered is"),
            new BookPageInfo(
                "great, but no choice",
                "remains..."),
            new BookPageInfo(),
            new BookPageInfo(),
            new BookPageInfo(
                "Kai Hohiro, 12pm.",
                "10.11.2001",
                "first one to be here"));
        [Constructable]
        public TranslatedGargoyleJournal()
            : base(false)
        {
        }

        public TranslatedGargoyleJournal(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent
        {
            get
            {
                return Content;
            }
        }
        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add("Translated Gargoyle Journal");
        }

        public override void OnSingleClick(Mobile from)
        {
            this.LabelTo(from, "Translated Gargoyle Journal");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}