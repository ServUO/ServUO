using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    public class Trades : BaseCollectionMobile
    {
        [Constructable]
        public Trades()
            : base("Bonnie", "the master of trades")
        {
            StartTier = 10000000;
            DailyDecay = 100000;

            DonationLabel = 1073438; // Trades Section Donation Representative.
        }

        public Trades(Serial serial)
            : base(serial)
        {
        }

        public override Collection CollectionID => Collection.Trades;

        public override int MaxTier => 1;

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            CantWalk = true;
            Race = Race.Human;

            Hue = 0x83ED;
            HairItemID = 0x203B;
            HairHue = 0x479;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Boots());
            AddItem(new HalfApron(0x65F));
            AddItem(new Doublet(0x3B3));
            AddItem(new Kilt(0x724));
            AddItem(new LeatherDo());
            AddItem(new LeatherHiroSode());
        }

        public override void Init()
        {
            base.Init();

            Donations.Add(new CollectionItem(typeof(Gold), 0xEEF, 1073116, 0x0, 0.06666));
            Donations.Add(new CollectionItem(typeof(BrownBook), 0xFEF, 1074906, 0x0, 3));
            Donations.Add(new CollectionItem(typeof(TanBook), 0xFF0, 1074906, 0x0, 3));
            Donations.Add(new CollectionItem(typeof(IronIngot), 0x1BF2, 1074904, 0x0, 1));
            Donations.Add(new CollectionItem(typeof(DullCopperIngot), 0x1BF2, 1074916, 0x973, 2));
            Donations.Add(new CollectionItem(typeof(ShadowIronIngot), 0x1BF2, 1074917, 0x966, 4));
            Donations.Add(new CollectionItem(typeof(CopperIngot), 0x1BF2, 1074918, 0x96D, 8));
            Donations.Add(new CollectionItem(typeof(BronzeIngot), 0x1BF2, 1074919, 0x972, 12));
            Donations.Add(new CollectionItem(typeof(GoldIngot), 0x1BF2, 1074920, 0x8A5, 18));
            Donations.Add(new CollectionItem(typeof(AgapiteIngot), 0x1BF2, 1074921, 0x979, 24));
            Donations.Add(new CollectionItem(typeof(VeriteIngot), 0x1BF2, 1074922, 0x89F, 31));
            Donations.Add(new CollectionItem(typeof(ValoriteIngot), 0x1BF2, 1074923, 0x8AB, 39));
            Donations.Add(new CollectionItem(typeof(Leather), 0x1081, 1074929, 0x0, 2));
            Donations.Add(new CollectionItem(typeof(SpinedLeather), 0x1081, 1074930, 0x8AC, 5));
            Donations.Add(new CollectionItem(typeof(HornedLeather), 0x1081, 1074931, 0x845, 10));
            Donations.Add(new CollectionItem(typeof(BarbedLeather), 0x1081, 1074932, 0x851, 15));
            Donations.Add(new CollectionItem(typeof(Hides), 0x1079, 1074924, 0x0, 2));
            Donations.Add(new CollectionItem(typeof(SpinedLeather), 0x1079, 1074925, 0x8AC, 5));
            Donations.Add(new CollectionItem(typeof(HornedLeather), 0x1079, 1074926, 0x845, 10));
            Donations.Add(new CollectionItem(typeof(BarbedLeather), 0x1079, 1074927, 0x851, 15));

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
            Rewards.Add(new CollectionItem(typeof(TradesGlasses), 0x2FB8, 1073362, 0x0, 800000.0));
        }

        public override bool CanDonate(PlayerMobile player)
        {
            bool can = player.LibraryFriend;

            if (!can)
                player.SendLocalizedMessage(1074273); // You must speak with Librarian Verity before you can donate to this collection. 

            return can;
        }

        public override void IncreaseTier()
        {
            base.IncreaseTier();

            List<object> list = new List<object>();
            Item c;

            switch (Tier)
            {
                case 1:
                    // dishing stump
                    c = new Static(0x1865);
                    c.MoveToWorld(new Point3D(1417, 1590, 30), Map);
                    list.Add(c);

                    // pickaxe
                    c = new Pickaxe();
                    c.MoveToWorld(new Point3D(1414, 1588, 47), Map);
                    c.Movable = false;
                    list.Add(c);

                    // sewing kit
                    c = new SewingKit();
                    c.MoveToWorld(new Point3D(1414, 1592, 42), Map);
                    c.Movable = false;
                    list.Add(c);

                    // empty tool box
                    c = new Static(0x1EB7);
                    c.MoveToWorld(new Point3D(1415, 1588, 47), Map);
                    list.Add(c);

                    // silver wire
                    c = new Static(0x1EB7)
                    {
                        Weight = 5.0
                    };
                    c.MoveToWorld(new Point3D(1417, 1592, 46), Map);
                    list.Add(c);

                    // gold wire
                    c = new Static(0x1EB7)
                    {
                        Weight = 5.0
                    };
                    c.MoveToWorld(new Point3D(1417, 1592, 47), Map);
                    list.Add(c);

                    // silver ingots
                    c = new Static(0x1BF6);
                    c.MoveToWorld(new Point3D(1416, 1592, 44), Map);
                    list.Add(c);
                    break;
            }

            if (list.Count > 0)
                Tiers.Add(list);
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