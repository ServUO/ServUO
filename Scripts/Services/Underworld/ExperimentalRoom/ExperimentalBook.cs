namespace Server.Items
{

    public class ExperimentalBook : BrownBook
    {
        public override int LabelNumber => 1113479;  // 

        [Constructable]
        public ExperimentalBook() : base(false)
        {
            ItemID = 4030;
        }

        public static readonly BookContent Content = new BookContent(
            "Read Me!", "Sir Wilber",
            new BookPageInfo(
                "Hello again!",
                "This is Part II of my",
                "experiment. Beyond these",
                "doors is a challenge for",
                "those carrying an ",
                "Experimental Gem."),
            new BookPageInfo(
                "INSTRUCTIONS:",
                "Activate the Gem to start",
                "the self-destruct timer.",
                "You have 30 minutes to",
                "reach the final room."),
            new BookPageInfo(
                "Each room has colored",
                "areas matching various states:",
                "White = Cold",
                "Pink = Warm",
                "Blue = Freezing",
                "Red = Blazing"),
            new BookPageInfo(
                "Green = Poison",
                "Orange = Cure",
                "Dark Green= Lethal",
                "Brown=Greater Cure",
                "Your gem's state will cycle",
                "randomly through these"),
            new BookPageInfo(
                "colors. Each time the gem",
                "shifts, you must enter the",
                "colored region that will",
                "bring your gem back to neutral",
                "state (grey). "),
                new BookPageInfo(
                "Failing when the gem reaches",
                "the extreme of any state get you",
                "expelled from the room and you",
                "will have to start over. The",
                "self-destruct"),
            new BookPageInfo(
                "timer will not reset. Once you",
                "reach the final room, place your",
                "gem in the box to receive",
                "your reward."));

        public override BookContent DefaultContent => Content;

        public ExperimentalBook(Serial serial)
            : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // ver
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}