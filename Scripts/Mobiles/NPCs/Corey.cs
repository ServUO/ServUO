using Server.Mobiles;

namespace Server.Items
{
    public class LightAndMight : BaseCollectionMobile
    {
        [Constructable]
        public LightAndMight()
            : base("Corey", "the paladin")
        {
            StartTier = 10000000;
            DailyDecay = 100000;

            DonationLabel = 1073446; // Light and Might Section Donation Representative.
        }

        public LightAndMight(Serial serial)
            : base(serial)
        {
        }

        public override Collection CollectionID => Collection.LightAndMight;

        public override int MaxTier => 1;

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;
            Race = Race.Human;

            Hue = 0x8405;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Halberd());
            AddItem(new Cloak(0x4E6));

            Item item;

            item = new PlateLegs
            {
                Hue = 0x8A6
            };
            AddItem(item);

            item = new PlateArms
            {
                Hue = 0x8A6
            };
            AddItem(item);

            item = new PlateChest
            {
                Hue = 0x8A6
            };
            AddItem(item);

            item = new PlateGloves
            {
                Hue = 0x8A6
            };
            AddItem(item);

            item = new PlateHelm
            {
                Hue = 0x8A6
            };
            AddItem(item);

            item = new PlateGorget
            {
                Hue = 0x8A6
            };
            AddItem(item);
        }

        public override void Init()
        {
            base.Init();

            Donations.Add(new CollectionItem(typeof(Gold), 0xEEF, 1073116, 0x0, 0.06666));
            Donations.Add(new CollectionItem(typeof(BrownBook), 0xFEF, 1074906, 0x0, 3));
            Donations.Add(new CollectionItem(typeof(TanBook), 0xFF0, 1074906, 0x0, 3));
            Donations.Add(new CollectionItem(typeof(BookOfChivalry), 0x2252, 1061666, 0x0, 12));

            int[] hues = new int[] { 0x1E0, 0x190, 0x151 };
            Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendBodySash), 0x1541, 1073346, 0x190, 100000.0, hues));
            Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendFeatheredHat), 0x171A, 1073347, 0x190, 100000.0, hues));
            Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendSurcoat), 0x1FFD, 1073348, 0x190, 100000.0, hues));
            Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendPants), 0x1539, 1073349, 0x190, 100000.0, hues));
            Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendCloak), 0x1515, 1073350, 0x190, 100000.0, hues));
            Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendDoublet), 0x1F7B, 1073351, 0x190, 100000.0, hues));
            Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendSkirt), 0x1537, 1073352, 0x190, 100000.0, hues));
            Rewards.Add(new CollectionTitle(1073341, 1073859, 100000.0)); // Britain Public Library Contributor

            hues = new int[] { 0x0, 0x1C2, 0x320, 0x190, 0x1E0 };
            Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendLantern), 0xA25, 1073339, 0x1C2, 200000.0, hues));
            Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendReadingChair), 0x2DEB, 1073340, 0x1C2, 200000.0, hues));
            Rewards.Add(new CollectionTitle(1073342, 1073860, 200000.0)); // Distinguished Library Contributor
            Rewards.Add(new CollectionHuedItem(typeof(SherryTheMouseQuotes), 0xFBD, 1073300, 0x1C2, 350000.0, hues));
            Rewards.Add(new CollectionHuedItem(typeof(WyrdBeastmasterQuotes), 0xFBD, 1073310, 0x1C2, 350000.0, hues));
            Rewards.Add(new CollectionHuedItem(typeof(MercenaryJustinQuotes), 0xFBD, 1073317, 0x1C2, 350000.0, hues));
            Rewards.Add(new CollectionHuedItem(typeof(HeigelOfMoonglowQuotes), 0xFBD, 1073327, 0x1C2, 350000.0, hues));
            Rewards.Add(new CollectionHuedItem(typeof(TraderHoraceQuotes), 0xFBD, 1073338, 0x1C2, 350000.0, hues));
            Rewards.Add(new CollectionTitle(1073343, 1073861, 350000.0)); // Honored Library Contributor			
            Rewards.Add(new CollectionItem(typeof(TreatiseonAlchemyTalisman), 0x2F58, 1073353, 0x0, 550000.0));
            Rewards.Add(new CollectionItem(typeof(PrimerOnArmsTalisman), 0x2F59, 1073354, 0x0, 550000.0));
            Rewards.Add(new CollectionItem(typeof(MyBookTalisman), 0x2F5A, 1073355, 0x0, 550000.0));
            Rewards.Add(new CollectionItem(typeof(TalkingtoWispsTalisman), 0x2F5B, 1073356, 0x0, 550000.0));
            Rewards.Add(new CollectionItem(typeof(GrammarOfOrchishTalisman), 0x2F59, 1073358, 0x0, 550000.0));
            Rewards.Add(new CollectionItem(typeof(BirdsofBritanniaTalisman), 0x2F5A, 1073359, 0x0, 550000.0));
            Rewards.Add(new CollectionItem(typeof(TheLifeOfTravelingMinstrelTalisman), 0x2F5A, 1073360, 0x0, 550000.0));
            Rewards.Add(new CollectionTitle(1073344, 1073862, 550000.0)); // Prominent Library Contributor
            Rewards.Add(new CollectionTitle(1073345, 1073863, 800000.0)); // Eminent Library Contributor
            Rewards.Add(new CollectionItem(typeof(LightOfWayGlasses), 0x2FB8, 1073378, 0x256, 800000.0));
        }

        public override bool CanDonate(PlayerMobile player)
        {
            bool can = player.LibraryFriend;

            if (!can)
                player.SendLocalizedMessage(1074273); // You must speak with Librarian Verity before you can donate to this collection. 

            return can;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}