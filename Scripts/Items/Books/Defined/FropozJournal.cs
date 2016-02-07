using System;

namespace Server.Items
{
    public class FropozJournal : RedBook
    {
        public static readonly BookContent Content = new BookContent(
            "Journal", "Fropoz",
            new BookPageInfo(
                "I have done as my",
                "Master has",
                "instructed me.",
                "",
                "The painted humans",
                "have been driven into",
                "Britannia and are even",
                "now wreaking havoc"),
            new BookPageInfo(
                "across the land,",
                "providing us with the",
                "distraction my Master",
                "requested.  We",
                "have provided them",
                "with the masks",
                "necessary to defeat",
                "the orcs, thus"),
            new BookPageInfo(
                "causing even more",
                "distress for the people",
                "of Britannia.  The",
                "unsuspecting fools",
                "are too busy dealing",
                "with the orc hordes to",
                "continue their",
                "exploration of our"),
            new BookPageInfo(
                "lands.  We are",
                "safe...for now.",
                "     ----",
                "The attacks",
                "continue exactly as",
                "planned.  My Master",
                "is pleased with my",
                "work and we are"),
            new BookPageInfo(
                "closer to our goals than",
                "ever before.  The",
                "gargoyles have proven",
                "to be more troublesome",
                "than we first",
                "anticipated, but I",
                "believe we can",
                "subjugate them fully"),
            new BookPageInfo(
                "given enough time.  It's",
                "unfortunate that we",
                "did not discover their",
                "knowledge sooner.",
                "Even now they",
                "prepare our armies",
                "for battle, but not",
                "without resistance."),
            new BookPageInfo(
                "Now that some of",
                "them know of the",
                "other lands and of",
                "humans, they will",
                "double their efforts to",
                "seek help.  This",
                "cannot be allowed.",
                "    -----"),
            new BookPageInfo(
                "Damn them!!  The",
                "humans proved",
                "more resourcefull than",
                "we thought them",
                "capable of.  Already",
                "their homes are free",
                "of orcs and savages",
                "and they once again"),
            new BookPageInfo(
                "are treading in our",
                "lands.  We may have to",
                "move sooner than we",
                "thought.  I will",
                "prepar my brethern",
                "and our golems.",
                "Hopefully, we can",
                "buy our Master some"),
            new BookPageInfo(
                "more time before the",
                "humans discover us.",
                "     -----",
                "It's too late.  The",
                "gargoyles whom have",
                "evaded our capture",
                "have opened the doors",
                "to our land."),
            new BookPageInfo(
                "They pray the",
                "humans will help",
                "them, despite the",
                "actions of their",
                "cousins in Britannia.  I",
                "fear they are right.",
                "I must go to warn",
                "the MastKai Hohiro,"),
            new BookPageInfo(),
            new BookPageInfo(
                "10.11.2001",
                "first one to be here",
                "",
                "Congrats. I didn't really",
                "care to log on earlier,",
                "nor did I come straight",
                "here. 2pm, Magus"));
        [Constructable]
        public FropozJournal()
            : base(false)
        {
        }

        public FropozJournal(Serial serial)
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
            list.Add("Fropoz's Journal");
        }

        public override void OnSingleClick(Mobile from)
        {
            this.LabelTo(from, "Fropoz's Journal");
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