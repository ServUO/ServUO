using System;
using Server.Mobiles;

namespace Server.Items
{
    public class SkeletonKey : BaseCollectionMobile
    {
        [Constructable]
        public SkeletonKey()
            : base("Enrico", "the thief")
        {
            this.StartTier = 10000000;
            this.DailyDecay = 100000;
			
            this.DonationLabel = 1073443; // Skeleton Key Section Donation Representative.
        }

        public SkeletonKey(Serial serial)
            : base(serial)
        { 
        }

        public override Collection CollectionID
        {
            get
            {
                return Collection.SkeletonKey;
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
			
            this.Hue = 0x83FA;
            this.HairItemID = 0x2045;
            this.HairHue = 0x46C;
            this.FacialHairItemID = 0x204C;
            this.FacialHairHue = 0x46C;
        }

        public override void InitOutfit()
        { 
            this.AddItem(new Backpack());						
            this.AddItem(new Dagger());				
            this.AddItem(new Sandals());				
            this.AddItem(new FancyShirt(0x6CC));				
            this.AddItem(new LongPants(0x5E8));	
        }

        public override void Init()
        {
            base.Init();
			
            this.Donations.Add(new CollectionItem(typeof(Gold), 0xEEF, 1073116, 0x0, 0.06666));
            this.Donations.Add(new CollectionItem(typeof(BankCheck), 0x14F0, 1075013, 0x34, 0.06666));				
            this.Donations.Add(new CollectionItem(typeof(Lockpick), 0x14FC, 1025371, 0x0, 1));	
            this.Donations.Add(new CollectionItem(typeof(LesserPoisonPotion), 0xF0A, 1015017, 0x0, 1));	
            this.Donations.Add(new CollectionItem(typeof(DeadlyPoisonPotion), 0xF0A, 1015020, 0x0, 5));												
            this.Donations.Add(new CollectionItem(typeof(ClothNinjaHood), 0x278F, 1074911, 0x0, 11));		
            this.Donations.Add(new CollectionItem(typeof(LeatherNinjaHood), 0x278E, 1074912, 0x0, 16));				
            this.Donations.Add(new CollectionItem(typeof(BrownBook), 0xFEF, 1074906, 0x0, 3));
            this.Donations.Add(new CollectionItem(typeof(TanBook), 0xFF0, 1074906, 0x0, 3));								
			
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
            this.Rewards.Add(new CollectionItem(typeof(PoisonedGlasses), 0x2FB8, 1073376, 0x55C, 800000.0));				
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