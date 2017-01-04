using Server;
using System;

namespace Server.Items
{
    public class FishingGuideBook6 : BaseBook
    {
        [Constructable]
		public FishingGuideBook6() : base( Utility.Random( 0xFF1, 2 ), false )
		{
            Name = "Vplume 6 - Legendary Sea Creatures";
		}

        public static readonly BookContent Content = new BookContent
        (
            null, "Cpt. Piddlewash",

            new BookPageInfo
            (
                "Abyssal Dragonfish:",

                "In the bottomless wells",
                "o' Destard lurk many ",
                "dangers. Some say a",
                "black dragonfish swims",
                "there."
            ),

            new BookPageInfo
            (
                "Black Marlin:",

                "Somewhere out in the ",
                "deep waters o' Felucca,",
                "sailors say they caught",
                "a black marlin but it",
                "got away."
            ),

            new BookPageInfo
            (
                "Blue Marlin:",

                "An old sailor once told",
                "me that he saw a blue",
                "marlin leap from the ",
                "seas o' Trammel. This",
                "hath ne'er been confirmed."
            ),

            new BookPageInfo
            (
                "Dungeon Pike:",

                "A journal was found in",
                "the Terathan Keep by a",
                "pile o' bones. The late",
                "fisherman claimed that",
                "he has caught this fish",
                "thar."
            ),

            new BookPageInfo
            (
                "Giant Samurai Fish:",

                "Tokuno fishermen tell ",
                "stories of ancient ",
                "samurai fish o' legendary",
                "size. None o' their ",
                "stories have ever been",
                "confirmed."
            ),

            new BookPageInfo
            (
                "Golden Tuna:",

                "This fish be only known",
                "in the myth. But come",
                "believe they exist in",
                "the deep waters o' ",
                "Tokuno."
            ),

            new BookPageInfo
            (
                "Winter Dragonfish:",

                "The Ice Dungeon holds",
                "many mysteries, most",
                "of them will kill ye. ",
                "But there be a legend",
                "of a dragonfish that",
                "rules the rivers thar."
            ),

            new BookPageInfo
            (
                "Kingfish",

                "The kingfish be",
                "extraordinarily rare.",
                "They say that Lord ",
                "British caught one ",
                "once, but this ne'er",
                "was confirmed."
            ),

            new BookPageInfo
            (
                "Lantern Fish:",

                "This fish be said to",
                "live in the Prism o'",
                "light. However like",
                "many legends, it has",
                "never been confirmed."
            ),

            new BookPageInfo
            (
                "Rainbow Fish:",

                "The elves tell a tale",
                "of princess who fell",
                "into the river of the",
                "Twisted Weald and was",
                "eaten by this elusive",
                "fish."
            ),

            new BookPageInfo
            (
                "Seeker Fish:",

                "The story o' this fish",
                "is that it wandered ",
                "into the Labyrinth of",
                "Malas and became lost.",
                "'Tis an odd story with",
                "many holes but it might",
                "be thar."
            ),

            new BookPageInfo
            (
                "Spring Dragonfish:",

                "Before one was caught",
                "by Mistress Kegwood ",
                "in Ilshenar, these were",
                "unknown. It hangs in ",
                "the secret hall o' the",
                "Order of the Dragonfish."
            ),

            new BookPageInfo
            (
                "Stone Fish:",

                "The stone harpies worship",
                "a great Stone fish they",
                "say sleeps at the bottom",
                "o' the sea in the Lost ",
                "Lands. Many of our order",
                "seek to catch it."
            ),

            new BookPageInfo
            (
                "Zombie Fish:",

                "'Tis said that there be",
                "an Undead fish in the ",
                "waters o' Malas. Some say",
                "it be an unholy experiment,",
                "some say it be a lie."
            ),

            new BookPageInfo
            (
                "Blood Lobster:",

                "In the depths o' the",
                "dungeon Shame this strange",
                "creature be said to lurk.",
                "Some say it feeds on the",
                "blood o' the fallen."
            ),

            new BookPageInfo
            (
                "Dread Lobster:",

                "'Tis said that this ",
                "lobster is the reason",
                "monsters don't go into",
                "the waters of Doom."
            ),

            new BookPageInfo
            (
                "Tunnel Crab:",

                "This creature be said",
                "to live in the",
                String.Format("{0} beneath Fire", FishInfo.GetFishLocation(typeof(TunnelCrab))),
                "Island. 'Tis a goblin",
                "legend so 'tis a bit",
                "suspect."
            ),

            new BookPageInfo
            (
                "Void Crab:",

                "Some old fisherman in",
                String.Format("{0} say they have", FishInfo.GetFishLocation(typeof(VoidCrab))),
                "seen a crab that ",
                "resembles a void demon",
                "in the rivers. This has",
                "not been confirmed."
            ),

            new BookPageInfo
            (
                "Void Lobster:",

                "The goblins o' the",
                String.Format("{0} tell o' a", FishInfo.GetFishLocation(typeof(VoidLobster))),
                "creature that looks like", 
                "a cross between a void",
                "demon and a lobster. They",
                "say it lives in the lava",
                "therein."
            )
        );

        public override BookContent DefaultContent{ get{ return Content; } }

        public FishingGuideBook6( Serial serial ) : base( serial )
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