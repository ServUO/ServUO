using Server;
using System;

namespace Server.Items
{
    public class FishingGuideBook5 : BaseBook
    {
        [Constructable]
		public FishingGuideBook5() : base( Utility.Random( 0xFF1, 2 ), false )
		{
            Name = "Volume 5 - Enchanted Sea Creatures";
		}

        public static readonly BookContent Content = new BookContent
        (
            null, "Cpt. Piddlewash",

            new BookPageInfo
            (
                "Autumn Dragonfish:",

                "This beauty be",
                "found in Ilshenar.",
                "If prepared ",
                "correctly and eaten,",
                "it be improvin' yer",
                "ability to meditate."
            ),

            new BookPageInfo
            (
                "Bull Fish:",

                "The bull fish be ",
                "found in the ",
                "labrynth o' Malas.",
                "If prepared ",
                "correctly and eaten,",
                "it be increasin'",
                "power to yer sword hand."
            ),

            new BookPageInfo
            (
                "Crystal Fish:",

                "This mystical fish",
                "be found in the ",
                "prism o' light. If",
                "prepared correctly",
                "and eaten, it be",
                "protectin' a sailor",
                "from lightning. "
            ),

            new BookPageInfo
            (
                "Fairy Salmon:",

                "This daring fish",
                "swims the rivers o'",
                String.Format("{0}. If prepared", FishInfo.GetFishLocation(typeof(FairySalmon))),
                "correctly, and eaten,",
                "it be helpin' improve",
                "a sailor's concentration",
                "when casting spells."
            ),

            new BookPageInfo
            (
                "Fire Fish:",

                "This fish be found",
                "in the dungeon o' ",
                "Shame. If prepared",
                "correctly and eaten,",
                "it be protecting a",
                "sailor from fire."

            ),

            new BookPageInfo
            (
                "Giant Koi:",

                "This fish be found in",
                "deep waters o' Tokuno.",
                "If prepared correctly",
                "and eaten, it be givin",
                "a sailor it's ability",
                "to dodge."
            ),

            new BookPageInfo
            (
                "Great Barracuda:",

                "This fish be found in",
                "the deep waters o'",
                "Felucca. If prepared",
                "correctly and eaten,",
                "it be increasin' yer",
                "accuracy with weapons."
            ),

            new BookPageInfo
            (
                "Holy Mackerel:",

                "This fish be found in",
                "the spirit filled ",
                "waters o' Malas. If",
                "prepared correctly and",
                "eaten, it be making ye",
                "gain mana more quickly."
            ),

            new BookPageInfo
            (
                "Lava Fish:",

                "This fish be found in",
                "the lava rivers o' the",
                String.Format("{0}. When ", FishInfo.GetFishLocation(typeof(LavaFish))),
                "prepared correctly and",
                "eaten, it be increasin'",
                "yer mana when ye be ",
                "injured."
            ),

            new BookPageInfo
            (
                "Reaper Fish:",

                "This fish be found in",
                "the lakes o' dungeon",
                "Doom. If prepared ",
                "correctly and eaten ",
                "it be protectin' ye ",
                "from poison damage."
            ),

            new BookPageInfo
            (
                "Summer Dragonfish:",

                "This beautiful fish ",
                "be found in the pools",
                "o' dungeon Destard. ",
                "If prepared correctly",
                "and eaten, it will ",
                "increase spell damage."
            ),

            new BookPageInfo
            (
                "Unicorn Fish:",

                "This great fish be ",
                "found in the Twisted ",
                "Weald. If prepared ",
                "correctly and eaten, ",
                "ye will recover from ",
                "fatigue more quickly."
            ),

            new BookPageInfo
            (
                "Yellowtail Barracuda:",

                "This devil be found ",
                "in the deep waters o'",
                "Trammel. If prepared ",
                "correctly and eaten,",
                "ye will heal more",
                "quickly."
            ),

            new BookPageInfo
            (
                "Blue Lobster:",

                "This lobster be exclusive",
                "to the Ice Dungeon. If",
                "prepared correctly and",
                "eaten, it protects ye",
                "from damage due to cold."
            ),

            new BookPageInfo
            (
                "Spider Crab:",

                "Found in the waters o'",
                "Terathan Keep. If ",
                "prepared correctly and",
                "eaten, it be improvin",
                "yer ability to focus."
            ),

            new BookPageInfo
            (
                "Stone Crab:",

                "This tough customer",
                "be ound in the deep ",
                "sea o' the Lost Lands.",
                "If prepared correctly",
                "and eaten, it makes yer",
                "skin tougher."
            )
        );

        public override BookContent DefaultContent{ get{ return Content; } }

        public FishingGuideBook5( Serial serial ) : base( serial )
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