namespace Server.Items
{
    public class FishingGuideBook4 : BaseBook
    {
        [Constructable]
        public FishingGuideBook4() : base(Utility.Random(0xFF1, 2), false)
        {
            Name = "Volume 4 - Uncommon Crustaceans";
        }

        public static readonly BookContent Content = new BookContent
        (
            null, "Cpt. Piddlewash",

            new BookPageInfo
            (
                "apple crab:",

                "Some say the apple",
                "crab be so named",
                "because it makes",
                "a good cider. To",
                "this, I say yuck! "
            ),

            new BookPageInfo
            (
                "Blue crab:",

                "The blue crab can",
                "be identified by",
                "the fact that they",
                "be blue on the ",
                "bottom. "
            ),

            new BookPageInfo
            (
                "Dungeoness crab:",

                "The dungeoness crab",
                "was so named ",
                "because 'twas first",
                "discovered in a ",
                "dungeon, later it",
                "was discovered they",
                "could be found ",
                "anywhere."
            ),

            new BookPageInfo
            (
                "King crab:",

                "The Order is not",
                "sure who made this",
                "rascal king, but",
                "we recon it took",
                "some fast talkin'."
            ),

            new BookPageInfo
            (
                "Rock crab:",

                "The rock crab be",
                "uncommon mostly",
                "because they",
                "often get stepped",
                "on by accident."

            ),

            new BookPageInfo
            (
                "Snow crab:",

                "Contrary to popular",
                "belief, the snow",
                "crab is not found",
                "in snow. They be",
                "found in water",
                "with the rest o'",
                "the crabs."
            ),

            new BookPageInfo
            (
                "Crusty lobster:",

                "Juka like to use",
                "the shell o' this",
                "lobster for pie crust. "
            ),

            new BookPageInfo
            (
                "Fred lobster:",

                "On occasion I be",
                "wonderin' to meself",
                ", who is Fred?",
                "and how did he get",
                "to name a lobster?"
            ),

            new BookPageInfo
            (
                "Hummer lobster:",

                "Some sailors say",
                "they can hear the",
                "hum of a hummer",
                "lobster. But I",
                "don't be seein'",
                "'em catch more",
                "than anyone else."
            ),

            new BookPageInfo
            (
                "Rock lobster:",

                "The rock lobster",
                "be uncommon ",
                "mostly because ",
                "they often get ",
                "st... Wait, I think",
                "I used that one",
                "already."
            ),

            new BookPageInfo
            (
                "Shovel-nose lobster:",

                "The shovel-nose ",
                "lobster has a flat,",
                "shovel like nose ",
                "that it uses to dig",
                "into the sand and hide. "
            ),

            new BookPageInfo
            (
                "Spiney lobster:",

                "Spiney lobsters be",
                "hard on traps, ",
                "sometimes when they",
                "try to get in a ",
                "trap they tear it",
                "to pieces. "
            )
        );

        public override BookContent DefaultContent => Content;

        public FishingGuideBook4(Serial serial) : base(serial)
        {
        }

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