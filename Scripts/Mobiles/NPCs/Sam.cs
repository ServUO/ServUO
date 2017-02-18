using System;
using Server.Mobiles;

namespace Server.Items
{
    public class BritanniaWaters : BaseCollectionMobile
    {
        [Constructable]
        public BritanniaWaters()
            : base("Sam", "the fisher")
        {
            this.StartTier = 10000000;
            this.DailyDecay = 100000;
			
            this.DonationLabel = 1073440; // Britannia Waters Section Donation Representative.
        }

        public BritanniaWaters(Serial serial)
            : base(serial)
        { 
        }

        public override Collection CollectionID
        {
            get
            {
                return Collection.BritanniaWaters;
            }
        }
        public override int MaxTier
        {
            get
            {
                return 1;
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;	
            this.CantWalk = true;
            this.Race = Race.Human;		
			
            this.Hue = 0x8402;
            this.HairItemID = 0x2044;
            this.HairHue = 0x46B;
            this.FacialHairItemID = 0x203F;
            this.FacialHairHue = 0x46B;
        }

        public override void InitOutfit()
        { 
            this.AddItem(new Backpack());						
            this.AddItem(new FishingPole());				
            this.AddItem(new Shoes(0x71F));				
            this.AddItem(new Doublet(0x52E));				
            this.AddItem(new LongPants(0x656));	
        }

        public override void Init()
        {
            base.Init();
			
            this.Donations.Add(new CollectionItem(typeof(Gold), 0xEEF, 1073116, 0x0, 0.06666));
            this.Donations.Add(new CollectionItem(typeof(BankCheck), 0x14F0, 1075013, 0x34, 0.06666));	
			
            this.Donations.Add(new CollectionItem(typeof(RawFishSteak), 0x97A, 1075087, 0x0, 0.25));	
            this.Donations.Add(new CollectionItem(typeof(Fish), 0x9CC, 1074939, 0x0, 1));										
            this.Donations.Add(new CollectionItem(typeof(FishingPole), 0xDC0, 1011406, 0x0, 2));				
            this.Donations.Add(new CollectionItem(typeof(BrownBook), 0xFEF, 1074906, 0x0, 3));
            this.Donations.Add(new CollectionItem(typeof(TanBook), 0xFF0, 1074906, 0x0, 3));

            #region High Seas
            Donations.Add(new CollectionItem(typeof(Lobster), 17619, 1096491, 0x0, 10));
            Donations.Add(new CollectionItem(typeof(Crab), 17617, 1096490, 0x0, 10));
            #endregion

            int[] hues = new int[] { 0x1E0, 0x190, 0x151 };			
            this.Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendBodySash), 0x1541, 1073346, 0x190, 100000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendFeatheredHat), 0x171A, 1073347, 0x190, 100000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendSurcoat), 0x1FFD, 1073348, 0x190, 100000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendPants), 0x1539, 1073349, 0x190, 100000.0, hues));			
            this.Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendCloak), 0x1515, 1073350, 0x190, 100000.0, hues));			
            this.Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendDoublet), 0x1F7B, 1073351, 0x190, 100000.0, hues));			
            this.Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendSkirt), 0x1537, 1073352, 0x190, 100000.0, hues));
            this.Rewards.Add(new CollectionTitle(1073341, 1073859, 100000.0)); // Britain Public Library Contributor
			
            hues = new int[] { 0x0, 0x1C2, 0x320, 0x190, 0x1E0 };
            this.Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendLantern), 0xA25, 1073339, 0x1C2, 200000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(LibraryFriendReadingChair), 0x2DEB, 1073340, 0x1C2, 200000.0, hues));			
            this.Rewards.Add(new CollectionTitle(1073342, 1073860, 200000.0)); // Distinguished Library Contributor
            this.Rewards.Add(new CollectionHuedItem(typeof(SherryTheMouseQuotes), 0xFBD, 1073300, 0x1C2, 350000.0, hues));	
            this.Rewards.Add(new CollectionHuedItem(typeof(WyrdBeastmasterQuotes), 0xFBD, 1073310, 0x1C2, 350000.0, hues));	
            this.Rewards.Add(new CollectionHuedItem(typeof(MercenaryJustinQuotes), 0xFBD, 1073317, 0x1C2, 350000.0, hues));	
            this.Rewards.Add(new CollectionHuedItem(typeof(HeigelOfMoonglowQuotes), 0xFBD, 1073327, 0x1C2, 350000.0, hues));	
            this.Rewards.Add(new CollectionHuedItem(typeof(TraderHoraceQuotes), 0xFBD, 1073338, 0x1C2, 350000.0, hues));				
            this.Rewards.Add(new CollectionTitle(1073343, 1073861, 350000.0)); // Honored Library Contributor			
            this.Rewards.Add(new CollectionItem(typeof(TreatiseonAlchemyTalisman), 0x2F58, 1073353, 0x0, 550000.0));			
            this.Rewards.Add(new CollectionItem(typeof(PrimerOnArmsTalisman), 0x2F59, 1073354, 0x0, 550000.0));		
            this.Rewards.Add(new CollectionItem(typeof(MyBookTalisman), 0x2F5A, 1073355, 0x0, 550000.0));			
            this.Rewards.Add(new CollectionItem(typeof(TalkingtoWispsTalisman), 0x2F5B, 1073356, 0x0, 550000.0));		
            this.Rewards.Add(new CollectionItem(typeof(GrammarOfOrchishTalisman), 0x2F59, 1073358, 0x0, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(BirdsofBritanniaTalisman), 0x2F5A, 1073359, 0x0, 550000.0));	
            this.Rewards.Add(new CollectionItem(typeof(TheLifeOfTravelingMinstrelTalisman), 0x2F5A, 1073360, 0x0, 550000.0));						
            this.Rewards.Add(new CollectionTitle(1073344, 1073862, 550000.0)); // Prominent Library Contributor
            this.Rewards.Add(new CollectionTitle(1073345, 1073863, 800000.0)); // Eminent Library Contributor
            this.Rewards.Add(new CollectionItem(typeof(MaritimeGlasses), 0x2FB8, 1073364, 0x581, 800000.0));				
        }

        public override bool CanDonate(PlayerMobile player)
        {
            bool can = player.LibraryFriend;
		
            if (!can)
                player.SendLocalizedMessage(1074273); // You must speak with Librarian Verity before you can donate to this collection. 
		
            return can;
        }

        /*public override void IncreaseTier()
        {			
        base.IncreaseTier();
			
        List<object> list = new List<object>();
        Item c;
			
        switch ( Tier )
        {								
        }
			
        if ( list.Count > 0 )
        Tiers.Add( list );
        }*/
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }
}