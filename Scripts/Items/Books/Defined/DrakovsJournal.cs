using System;

namespace Server.Items
{
    public class DrakovsJournal : BlueBook
    {
        public static readonly BookContent Content = new BookContent(
            "Drakov's Journal", "Drakov",
            new BookPageInfo(
                "My Master",
                "",
                "This journal was",
                "found on one of",
                "our controllers.  It",
                "seems he has lost",
                "faith in you.  Know",
                "that he has been"),
            new BookPageInfo(
                "dealth with and will",
                "never again speak",
                "ill of you or our",
                "cause.",
                "          -Galzon"),
            new BookPageInfo(
                "We have completted",
                "construction of the",
                "devices needed to",
                "build the clockwork",
                "overseers and minions",
                "as per the request of",
                "the Master.  The",
                "gargoyles have been"),
            new BookPageInfo(
                "most useful and their",
                "knowledge of the",
                "techniques for the",
                "construction of these",
                "creatures will serve",
                "us well.",
                "        -----",
                "I am not one to"),
            new BookPageInfo(
                "criticize the Master,",
                "but I believe he may",
                "have erred in his",
                "decision to destroy",
                "the wingless ones.",
                "Already our forces",
                "are weakened by the",
                "constant attacks of"),
            new BookPageInfo(
                "the humans  Their",
                "strength and",
                "unquestioning",
                "compliance would",
                "have made them very",
                "useful in the fight",
                "against the humans.",
                "But the Master felt"),
            new BookPageInfo(
                "their presence to be",
                "an annoyance and",
                "a distraction to the",
                "winged ones.  It was",
                "not difficult at all",
                "to remove them from",
                "this world.  But now",
                "I fear without more"),
            new BookPageInfo(
                "allies, willing or",
                "not, we stand",
                "little chance of",
                "defeating the foul",
                "humans from our",
                "lands.  Perhaps if",
                "the Master had",
                "shown a little"),
            new BookPageInfo(
                "mercy and forsight",
                "we would not be",
                "in such dire peril."));
        [Constructable]
        public DrakovsJournal()
            : base(false)
        {
        }

        public DrakovsJournal(Serial serial)
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