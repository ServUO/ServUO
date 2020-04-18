namespace Server.Items
{
    public class PuzzleBook : BrownBook
    {
        public static readonly BookContent Content = new BookContent(
            "Instructions", "Sir Wilber",
            new BookPageInfo(
                "Greetings Traveler!",
                "I would like to invite",
                "you to a little game.",
                "See the magic key? It "),
            new BookPageInfo(
                "will grant you access to",
                "the Puzzle Room. Be advised",
                "that once you take the key,",
                "you will have no more than",
                "30 minutes to enter the room."),
            new BookPageInfo(
                "and solve the puzzles. If",
                "you fail, you will be",
                "expelled and all your",
                "progress will be lost! ",
                "There are 3 puzzle chests.",
                "Two of them must be completed",
                "first to unlock the third."),
            new BookPageInfo(
                "If successful, you will get a",
                "special item required to enter",
                "my other playground should you",
                "discover its location within",
                "the underworld!!"));

        [Constructable]
        public PuzzleBook() : base(false)
        {
            Movable = false;
            ItemID = 4030;
        }

        public PuzzleBook(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent => Content;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}