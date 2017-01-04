using Server;
using System;

namespace Server.Items
{
    public class FishingGuideBook3 : BaseBook
    {
        [Constructable]
		public FishingGuideBook3() : base( Utility.Random( 0xFF1, 2 ), false )
		{
            Name = "Volume 3 - Uncommon Dungeon Fish";
		}

        public static readonly BookContent Content = new BookContent
        (
            null, "Cpt. Piddlewash",

            new BookPageInfo
            (
                "Crag Snapper:",

                "Crag Snapper be fine",
                "eating. Just mind yer",
                "fingers. "
            ),

            new BookPageInfo
            (
                "Cutthrout Trout:",

                "This dungeon menace",
                "'tis the very one that",
                "gave rise to the ol'",
                "saying. 'Ne'er take a",
                "bath in dungeon water. "
            ),

            new BookPageInfo
            (
                "Darkfish:",

                "Ye find this fish in",
                "undergroun rivers and",
                "lakes. But only dark",
                "undergroun rivers and",
                "lakes. "
            ),

            new BookPageInfo
            (
                "Demon Trout:",

                "Beware, this big devil",
                "comes out of the water",
                "spicy. "
            ),

            new BookPageInfo
            (
                "Drakefish:",

                "The smaller cousin o'",
                "the dragonfish, this",
                "beauty be much easier",
                "to catch and thus more",
                "commonly used in cooking."

            ),

            new BookPageInfo
            (
                "Dungeon Chub:",

                "This be the only",
                "subterranean member",
                "o' the chub family. "
            ),

            new BookPageInfo
            (
                "Grim Cisco:",

                "This fish is sought",
                "for medicinal",
                "purposes. They say",
                "it be the best cure",
                "for hysteria. "
            ),

            new BookPageInfo
            (
                "Infernal Tuna:",

                "This fish be deadly",
                "poisonous unless ye",
                "cook it in butter",
                "with a bit o' ",
                "thyme and serve it",
                "will ale. "
            ),

            new BookPageInfo
            (
                "Lurker Fish:",

                "These fish like to",
                "hide up under",
                "corpses floating",
                "in underground rivers."
            ),

            new BookPageInfo
            (
                "Orc Bass:",

                "If ye be ever ",
                "chased by orcs, ",
                "throw one down an",
                "keep runnin! Ever",
                "since I started",
                "tellin' folks this,",
                "I been sellin more",
                "orc bass. "
            ),

            new BookPageInfo
            (
                "Snaggletoth Bass:",

                "This dungeon lurker",
                "be resemblin' a ",
                "large mouth bass",
                "excepting it be",
                "having huge jagged",
                "teeth."
            ),

            new BookPageInfo
            (
                "Tormented Pike:",

                "This pike be hunted",
                "by every monster in",
                "Sosaria except few a",
                "few."
            )
        );

        public override BookContent DefaultContent{ get{ return Content; } }

        public FishingGuideBook3( Serial serial ) : base( serial )
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