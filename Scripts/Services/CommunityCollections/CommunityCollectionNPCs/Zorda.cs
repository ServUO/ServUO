using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    public class ArtsSection : BaseCollectionMobile
    {
        [Constructable]
        public ArtsSection()
            : base("Zorda", "the artist")
        {
            StartTier = 10000000;
            DailyDecay = 100000;

            DonationLabel = 1073439; // Arts Section Donation Representative.
        }

        public ArtsSection(Serial serial)
            : base(serial)
        {
        }

        public override Collection CollectionID => Collection.ArtsSection;

        public override int MaxTier => 1;

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            CantWalk = true;
            Race = Race.Human;

            Hue = 0x8405;
            HairItemID = 0x2045;
            HairHue = 0x44E;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Sandals(0x72B));
            AddItem(new Shirt(0x6BB));
            AddItem(new HalfApron(0x8FD));
            AddItem(new Skirt(0x38B));
        }

        public override void Init()
        {
            base.Init();

            Donations.Add(new CollectionItem(typeof(Gold), 0xEEF, 1073116, 0x0, 0.06666));
            Donations.Add(new CollectionItem(typeof(ScribesPen), 0x0FBF, 1044352, 0x0, 1));
            Donations.Add(new CollectionItem(typeof(BlankScroll), 0xEF3, 1044377, 0x0, 1));
            Donations.Add(new CollectionItem(typeof(FertileDirt), 0xF81, 1023969, 0x0, 1));
            Donations.Add(new CollectionItem(typeof(Board), 0x1BD7, 1015101, 0, 1));
            Donations.Add(new CollectionItem(typeof(OakBoard), 0x1BD7, 1075052, 0x7DA, 3));
            Donations.Add(new CollectionItem(typeof(AshBoard), 0x1BD7, 1075053, 0x4A7, 6));
            Donations.Add(new CollectionItem(typeof(YewBoard), 0x1BD7, 1075054, 0x4A8, 9));
            Donations.Add(new CollectionItem(typeof(HeartwoodBoard), 0x1BD7, 1075062, 0x4A9, 12));
            Donations.Add(new CollectionItem(typeof(BloodwoodBoard), 0x1BD7, 1075055, 0x4AA, 24));
            Donations.Add(new CollectionItem(typeof(FrostwoodBoard), 0x1BD7, 1075056, 0x47F, 48));
            Donations.Add(new CollectionItem(typeof(BrownBook), 0xFEF, 1074906, 0x0, 3));
            Donations.Add(new CollectionItem(typeof(TanBook), 0xFF0, 1074906, 0x0, 3));

            int[] hues = new int[] { 0x1E0, 0x190, 0x151 };
            Rewards.Add(new CollectionItem(typeof(SpecialPrintingOfVirtue), 0xFF2, 1075793, 0x0, 5000.0, true));
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
            Rewards.Add(new CollectionItem(typeof(GlassesOfTheArts), 0x2FB8, 1073363, 0x73, 800000.0));
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
                    // easel with canvas
                    c = new Static(0xF66);
                    c.MoveToWorld(new Point3D(1417, 1602, 30), Map);
                    list.Add(c);

                    // table
                    c = new Static(0xB6B);
                    c.MoveToWorld(new Point3D(1417, 1606, 30), Map);
                    list.Add(c);

                    c = new Static(0xB6D);
                    c.MoveToWorld(new Point3D(1417, 1605, 30), Map);
                    list.Add(c);

                    c = new Static(0xB6C);
                    c.MoveToWorld(new Point3D(1417, 1604, 30), Map);
                    list.Add(c);

                    // bonsai tree					
                    c = new Static(0x28DC);
                    c.MoveToWorld(new Point3D(1417, 1604, 36), Map);
                    list.Add(c);

                    // bottles		
                    c = new Static(0xE29);
                    c.MoveToWorld(new Point3D(1417, 1605, 37), Map);
                    list.Add(c);

                    c = new Static(0xE28);
                    c.MoveToWorld(new Point3D(1417, 1605, 36), Map);
                    list.Add(c);

                    c = new Static(0xE2C);
                    c.MoveToWorld(new Point3D(1417, 1606, 37), Map);
                    list.Add(c);

                    // pen and ink					
                    c = new Static(0xFBF);
                    c.MoveToWorld(new Point3D(1417, 1606, 36), Map);
                    list.Add(c);

                    // cooking book			
                    c = new Static(0xFBE);
                    c.MoveToWorld(new Point3D(1418, 1606, 42), Map);
                    c.Name = "A Cookbook";
                    c.Weight = 5.0;
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