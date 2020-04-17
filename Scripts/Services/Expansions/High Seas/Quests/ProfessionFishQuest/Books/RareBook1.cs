namespace Server.Items
{
    public class FishingGuideBook1 : BaseBook
    {
        [Constructable]
        public FishingGuideBook1() : base(Utility.Random(0xFF1, 2), false)
        {
            Name = "Volume 1 - Uncommon Shore Fish";
        }

        public static readonly BookContent Content = new BookContent
        (
            null, "Cpt. Piddlewash",

            new BookPageInfo
            (
                "Bluegill Sunfish:",
                "Be wary o these bluegills,",
                "they be a bit snooty but",
                "they be tastin' great on a",
                "cracker."
            ),

            new BookPageInfo
            (
                "Brook trout:",
                "These be found in brooks",
                "mostly, but sometimes",
                "streams, ponds, creeks,",
                "rivers, inlets, fords, and",
                "occasionally puddles."
            ),

            new BookPageInfo
            (
                "Green catfish:",

                "Dont let the green color",
                "scare ye away, it be",
                "delicious! Folks what eat",
                "them say they make yer eyes",
                "turn green."
            ),

            new BookPageInfo
            (
                "Kokanee salmon:",

                "I named this'n fer me",
                "favorite aunt in hopes that",
                "she would leave me her ship.",
                "Then she left it to her",
                "boyfriend so I changed it to",
                "Kokanee. "
            ),

            new BookPageInfo
            (
                "Pike:",

                "This fresh water fish be lookin'",
                "a bit like their ocean cousin",
                "the barracuda. But don't be",
                "fooled, they bite!"

            ),

            new BookPageInfo
            (
                "Pumpkinseed Sunfish:",

                "Found in rivers and other shallow",
                "waters, this fish be so named",
                "because it be first caught by me",
                "friend, Pumpkinseed Smith."
            ),

            new BookPageInfo
            (
                "Rainbow trout:",

                "These trout be colored a bit like",
                "rainbow salmon but they're not,",
                "they're trout."
            ),

            new BookPageInfo
            (
                "Redbelly bream:",

                "The secret ta catchin' these",
                "particular bream is ta be fishin'",
                "near the shores."
            ),

            new BookPageInfo
            (
                "Smallmouth bass:",

                "'Tis believed that this",
                "fish is uncommon simply",
                "because it be a picky 'eater. "
            ),

            new BookPageInfo
            (
                "Uncommon shiner:",

                "This fish is not to be",
                "confused with the common",
                "shiner. The uncommon",
                "shiner tastes way better."
            ),

            new BookPageInfo
            (
                "Walleye:",

                "This be a tricky devil",
                "'cause he can see ye coming.",
                "'Tis best to fish her them",
                "at night or ta be wearin'",
                "a worm costume."
            ),

            new BookPageInfo
            (
                "Yellow perch:",

                "Ye can sometimes see",
                "these swimmin' near rocks",
                "and such. They be easy ta",
                "spot cause they be yellow",
                "somewhere on thar body."
            )
        );

        public override BookContent DefaultContent => Content;

        public FishingGuideBook1(Serial serial) : base(serial)
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