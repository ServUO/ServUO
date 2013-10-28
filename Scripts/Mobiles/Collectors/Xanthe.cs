using System;
using Server.Mobiles;

namespace Server.Items
{
    public class SongsOfNote : BaseCollectionMobile
    {
        [Constructable]
        public SongsOfNote()
            : base("Xanthe", "the musician")
        {
            this.StartTier = 10000000;
            this.DailyDecay = 100000;
			
            this.DonationLabel = 1073450; // Songs of Note Section Donation Representative.
        }

        public SongsOfNote(Serial serial)
            : base(serial)
        { 
        }

        public override Collection CollectionID
        {
            get
            {
                return Collection.SongsOfNote;
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
			
            this.Female = true;	
            this.CantWalk = true;
            this.Race = Race.Human;		
			
            this.Hue = 0x83F4;
            this.HairItemID = 0x2049;
            this.HairHue = 0x460;
        }

        public override void InitOutfit()
        { 
            this.AddItem(new Backpack());						
            this.AddItem(new Shoes(0x74D));				
            this.AddItem(new ShortPants(0x57B));				
            this.AddItem(new FancyShirt(0x725));	
        }

        public override void Init()
        {
            base.Init();
			
            this.Donations.Add(new CollectionItem(typeof(Gold), 0xEEF, 1073116, 0x0, 0.06666));
            this.Donations.Add(new CollectionItem(typeof(BankCheck), 0x14F0, 1075013, 0x34, 0.06666));	
            this.Donations.Add(new CollectionItem(typeof(BrownBook), 0xFEF, 1074906, 0x0, 3));
            this.Donations.Add(new CollectionItem(typeof(TanBook), 0xFF0, 1074906, 0x0, 3));
            this.Donations.Add(new CollectionItem(typeof(Drums), 0xE9C, 1015116, 0x0, 8));		
            this.Donations.Add(new CollectionItem(typeof(Lute), 0xEB3, 1015117, 0x0, 8));				
			
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
            this.Rewards.Add(new CollectionItem(typeof(LyricalGlasses), 0x2FB8, 1073382, 0x47F, 800000.0));				
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