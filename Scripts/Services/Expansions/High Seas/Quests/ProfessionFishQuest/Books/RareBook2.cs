using Server;
using System;

namespace Server.Items
{
    public class FishingGuideBook2 : BaseBook
    {
        [Constructable]
		public FishingGuideBook2() : base( Utility.Random( 0xFF1, 2 ), false )
		{
            Name = "Volume 2 - Uncommon Sea Fish";
		}

        public static readonly BookContent Content = new BookContent
        (
            null, "Cpt. Piddlewash",

            new BookPageInfo
            (
                "Amberjack:",

                "One o’ these days I’m",
                "going ta hang up me",
                "fishin’ pole and start",
                "me own brewery, and I’m",
                "going to name me brew",
                "amberjack."
            ),

            new BookPageInfo
            (
                "Black sea bass",

                "The black seabass be",
                "a more purpley color in",
                "me personal opinion,",
                "but it wasn’ me what",
                "named it. "
            ),

            new BookPageInfo
            (
                "Blue grouper:",

                "Dont let the green color",
                "scare ye away, it be",
                "delicious! Folks what eat",
                "them say they make yer eyes",
                "turn green."
            ),

            new BookPageInfo
            (
                "Bluefish:",

                "These be uncommon",
                "because most fishermen",
                "mistake them for other",
                "fish cause they ain’t",
                "actually blue. This be",
                "thar natural defense. "
            ),

            new BookPageInfo
            (
                "Bonefish:",

                "This fish be havin ",
                "lots o’ bones. Like",
                "double the normal",
                "amount. I seen some",
                "that couldn’t even move!"

            ),

            new BookPageInfo
            (
                "Bonito:",

                "Bonito be great when",
                "smoked and dried. Tis",
                "a favorite of Tokuno. "
            ),

            new BookPageInfo
            (
                "Cape cod:",

                "This fish be found off",
                "the cape. Way off the",
                "cape. Like, in the",
                "middle o the sea. "
            ),

            new BookPageInfo
            (
                "Captain snook:",

                "Whatever sun baked ",
                "swab named this poor",
                "devil cap’n snook",
                "should be keel hauled!",
                "I knew Cap’n Snook,",
                "this be no Cap’n Snook. "
            ),

            new BookPageInfo
            (
                "Cobia:",

                "Best to not be confusin’",
                "the cobia with the cobra,",
                "the cobra be requiring",
                "a totally different kind",
                "o’ bait."
            ),

            new BookPageInfo
            (
                "Gray snapper:",

                "Old sailors say that",
                "many generations ago",
                "the gray snapper used",
                "to be the blonde snapper."
            ),

            new BookPageInfo
            (
                "Haddock:",

                "When tha’ wind’s in",
                "yar hair, the salt’s",
                "on yar lips and yar",
                "hook’s wrapped around",
                "yar pole, the haddock",
                "be thar."
            ),

            new BookPageInfo
            (
                "Mahi mahi:",

                "They say that the most",
                "persuasive argument is",
                "repetition. This might",
                "explain why mahi mahi",
                "be so popular."
            ),

            new BookPageInfo
            (
                "Red drum:",

                "The red drum is thus",
                "named because o’ the",
                "sound it makes when",
                "you thump it on the",
                "head."
            ),

            new BookPageInfo
            (
                "Red grouper:",

                "Red grouper be extra",
                "good with a dalop o’",
                "Madam Beamy’s hot sauce."
            ),

            new BookPageInfo
            (
                "Red Snook:",

                "This fish be found",
                "anywhere the rest o",
                "the fish in this handbook",
                "be found."
            ),

            new BookPageInfo
            (
                "Shad:",

                "The shad be one o’",
                "me personal favorite",
                "deep sea uncommon fish."
            ),

            new BookPageInfo
            (
                "Tarpon:",

                "This fella once told",
                "me the word ‘Tarpon’ be",
                "derived from the word",
                "‘Tarpaulin’, but I’s",
                "pretty sure he was insane."
            ),

            new BookPageInfo
            (
                "Yellowfin tuna:",

                "The best thing about",
                "tuna is that it tastes",
                "like chicken that was",
                "eaten by a fish."
            )
        );

        public override BookContent DefaultContent{ get{ return Content; } }

        public FishingGuideBook2( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
    }
}