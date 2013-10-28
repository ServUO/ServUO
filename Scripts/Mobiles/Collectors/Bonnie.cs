using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class Trades : BaseCollectionMobile
    {
        [Constructable]
        public Trades()
            : base("Bonnie", "the master of trades")
        {
            this.StartTier = 10000000;
            this.DailyDecay = 100000;
			
            this.DonationLabel = 1073438; // Trades Section Donation Representative.
        }

        public Trades(Serial serial)
            : base(serial)
        { 
        }

        public override Collection CollectionID
        {
            get
            {
                return Collection.Trades;
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
			
            this.Hue = 0x83ED;
            this.HairItemID = 0x203B;
            this.HairHue = 0x479;
        }

        public override void InitOutfit()
        { 
            this.AddItem(new Backpack());						
            this.AddItem(new Boots());				
            this.AddItem(new HalfApron(0x65F));				
            this.AddItem(new Doublet(0x3B3));				
            this.AddItem(new Kilt(0x724));				
            this.AddItem(new LeatherDo());				
            this.AddItem(new LeatherHiroSode());
        }

        public override void Init()
        {
            base.Init();
			
            this.Donations.Add(new CollectionItem(typeof(Gold), 0xEEF, 1073116, 0x0, 0.06666));
            this.Donations.Add(new CollectionItem(typeof(BankCheck), 0x14F0, 1075013, 0x34, 0.06666));	
            this.Donations.Add(new CollectionItem(typeof(BrownBook), 0xFEF, 1074906, 0x0, 3));
            this.Donations.Add(new CollectionItem(typeof(TanBook), 0xFF0, 1074906, 0x0, 3));			
            this.Donations.Add(new CollectionItem(typeof(IronIngot), 0x1BF2, 1074904, 0x0, 1));										
            this.Donations.Add(new CollectionItem(typeof(DullCopperIngot), 0x1BF2, 1074916, 0x973, 2));				
            this.Donations.Add(new CollectionItem(typeof(ShadowIronIngot), 0x1BF2, 1074917, 0x966, 4));				
            this.Donations.Add(new CollectionItem(typeof(CopperIngot), 0x1BF2, 1074918, 0x96D, 8));				
            this.Donations.Add(new CollectionItem(typeof(BronzeIngot), 0x1BF2, 1074919, 0x972, 12));				
            this.Donations.Add(new CollectionItem(typeof(GoldIngot), 0x1BF2, 1074920, 0x8A5, 18));				
            this.Donations.Add(new CollectionItem(typeof(AgapiteIngot), 0x1BF2, 1074921, 0x979, 24));				
            this.Donations.Add(new CollectionItem(typeof(VeriteIngot), 0x1BF2, 1074922, 0x89F, 31));				
            this.Donations.Add(new CollectionItem(typeof(ValoriteIngot), 0x1BF2, 1074923, 0x8AB, 39));							
            this.Donations.Add(new CollectionItem(typeof(Leather), 0x1081, 1074929, 0x0, 2));	
            this.Donations.Add(new CollectionItem(typeof(SpinedLeather), 0x1081, 1074930, 0x8AC, 5));	
            this.Donations.Add(new CollectionItem(typeof(HornedLeather), 0x1081, 1074931, 0x845, 10));	
            this.Donations.Add(new CollectionItem(typeof(BarbedLeather), 0x1081, 1074932, 0x851, 15));							
            this.Donations.Add(new CollectionItem(typeof(Hides), 0x1079, 1074924, 0x0, 2));	
            this.Donations.Add(new CollectionItem(typeof(SpinedLeather), 0x1079, 1074925, 0x8AC, 5));	
            this.Donations.Add(new CollectionItem(typeof(HornedLeather), 0x1079, 1074926, 0x845, 10));	
            this.Donations.Add(new CollectionItem(typeof(BarbedLeather), 0x1079, 1074927, 0x851, 15));	
			
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
            this.Rewards.Add(new CollectionItem(typeof(TradesGlasses), 0x2FB8, 1073362, 0x0, 800000.0));				
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
						
            switch ( this.Tier )
            { 
                case 1:
                    // dishing stump
                    c = new Static(0x1865);
                    c.MoveToWorld(new Point3D(1417, 1590, 30), this.Map);
                    list.Add(c);
					
                    // pickaxe
                    c = new Pickaxe();
                    c.MoveToWorld(new Point3D(1414, 1588, 47), this.Map);
                    c.Movable = false;
                    list.Add(c);
					
                    // sewing kit
                    c = new SewingKit();
                    c.MoveToWorld(new Point3D(1414, 1592, 42), this.Map);
                    c.Movable = false;
                    list.Add(c);
					
                    // empty tool box
                    c = new Static(0x1EB7);
                    c.MoveToWorld(new Point3D(1415, 1588, 47), this.Map);
                    list.Add(c);
					
                    // silver wire
                    c = new Static(0x1EB7);
                    c.Weight = 5.0;
                    c.MoveToWorld(new Point3D(1417, 1592, 46), this.Map);
                    list.Add(c);
					
                    // gold wire
                    c = new Static(0x1EB7);
                    c.Weight = 5.0;
                    c.MoveToWorld(new Point3D(1417, 1592, 47), this.Map);
                    list.Add(c);
					
                    // silver ingots
                    c = new Static(0x1BF6);
                    c.MoveToWorld(new Point3D(1416, 1592, 44), this.Map);
                    list.Add(c);				
                    break;				
            }
			
            if (list.Count > 0)
                this.Tiers.Add(list);
        }

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