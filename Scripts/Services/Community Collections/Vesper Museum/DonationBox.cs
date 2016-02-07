using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class VesperDonationBox : BaseCollectionItem
    {
        [Constructable]
        public VesperDonationBox()
            : base(0xE7D)
        {
            this.Hue = 0x48D;
            this.StartTier = 10000000;
            this.NextTier = 5000000;
            this.DailyDecay = 100000;
        }

        public VesperDonationBox(Serial serial)
            : base(serial)
        { 
        }

        public override int LabelNumber
        {
            get
            {
                return 1073407;
            }
        }// Please Contribute to the public Museum of Vesper.
        public override Collection CollectionID
        {
            get
            {
                return Collection.VesperMuseum;
            }
        }
        public override int MaxTier
        {
            get
            {
                return 12;
            }
        }
        public override void Init()
        {
            base.Init();
			
            this.Donations.Add(new CollectionItem(typeof(Gold), 0xEEF, 1073116, 0x0, 0.06666));
            this.Donations.Add(new CollectionItem(typeof(BankCheck), 0x14F0, 1075013, 0x34, 0.06666));
            this.Donations.Add(new CollectionItem(typeof(Board), 0x1BD7, 1015101, 0, 1));
            this.Donations.Add(new CollectionItem(typeof(OakBoard), 0x1BD7, 1075052, 0x7DA, 3));
            this.Donations.Add(new CollectionItem(typeof(AshBoard), 0x1BD7, 1075053, 0x4A7, 6));
            this.Donations.Add(new CollectionItem(typeof(YewBoard), 0x1BD7, 1075054, 0x4A8, 9));
            this.Donations.Add(new CollectionItem(typeof(HeartwoodBoard), 0x1BD7, 1075062, 0x4A9, 12));
            this.Donations.Add(new CollectionItem(typeof(BloodwoodBoard), 0x1BD7, 1075055, 0x4AA, 24));
            this.Donations.Add(new CollectionItem(typeof(FrostwoodBoard), 0x1BD7, 1075056, 0x47F, 48));
            this.Donations.Add(new CollectionItem(typeof(Hinge), 0x1055, 1044172, 0x0, 2));
            this.Donations.Add(new CollectionItem(typeof(Scorp), 0x10E7, 1075057, 0x0, 2));
            this.Donations.Add(new CollectionItem(typeof(DrawKnife), 0x10E4, 1075058, 0x0, 2));
            this.Donations.Add(new CollectionItem(typeof(JointingPlane), 0x1030, 1075059, 0x0, 4));
            this.Donations.Add(new CollectionItem(typeof(MouldingPlane), 0x102C, 1075060, 0x0, 4));
            this.Donations.Add(new CollectionItem(typeof(SmoothingPlane), 0x1032, 1075061, 0x0, 4));
			
            int[] hues = new int[] { 0x581, 0x278, 0x318, 0x2FF };			
            this.Rewards.Add(new CollectionHuedItem(typeof(OdricsRobe), 0x1F03, 1073250, 0x581, 100000.0, hues));
			
            hues = new int[] { 0x229, 0x18E, 0x215, 0xF5 };			
            this.Rewards.Add(new CollectionHuedItem(typeof(MalabellesDress), 0x1516, 1073251, 0x229, 100000.0, hues));
			
            hues = new int[] { 0x281, 0x173, 0x581, 0x300 };	
            this.Rewards.Add(new CollectionHuedItem(typeof(BaronLenshiresCloak), 0x1515, 1073252, 0x281, 100000.0, hues));			
            this.Rewards.Add(new CollectionHuedItem(typeof(Adranath), 0x1541, 1073253, 0x555, 100000.0, hues));
            this.Rewards.Add(new CollectionTitle(1073235, 1073637, 100000.0)); // Vesper Museum Contributor
			
            hues = new int[] { 0x2A, 0x27D, 0xF7, 0x236 };
            this.Rewards.Add(new CollectionHuedItem(typeof(VesperCollectionRing), 0x108A, 1073234, 0x2A, 200000.0, hues));	
            this.Rewards.Add(new CollectionHuedItem(typeof(VesperCollectionNecklace), 0x1088, 1073234, 0x2A, 200000.0, hues));	
            this.Rewards.Add(new CollectionHuedItem(typeof(VesperCollectionBracelet), 0x1086, 1073234, 0x2A, 200000.0, hues));	
            this.Rewards.Add(new CollectionHuedItem(typeof(VesperCollectionEarrings), 0x1087, 1073234, 0x2A, 200000.0, hues));			
            this.Rewards.Add(new CollectionTitle(1073236, 1073638, 200000.0)); // Distinguished Vesper Museum Contributor
            this.Rewards.Add(new CollectionItem(typeof(LordBritishThroneDeed), 0x1F23, 1073243, 0x0, 350000.0));	
            this.Rewards.Add(new CollectionItem(typeof(TrollStatuette), 0x20E9, 1073242, 0x0, 350000.0));	
            this.Rewards.Add(new CollectionItem(typeof(CrystalBallStatuette), 0xE2D, 1073244, 0x0, 350000.0));	
            this.Rewards.Add(new CollectionItem(typeof(DevourerStatuette), 0x2623, 1073245, 0x0, 350000.0));	
            this.Rewards.Add(new CollectionItem(typeof(SnowLadyStatuette), 0x276C, 1075016, 0x0, 350000.0));	
            this.Rewards.Add(new CollectionItem(typeof(GolemStatuette), 0x2610, 1075017, 0x0, 350000.0));	
            this.Rewards.Add(new CollectionItem(typeof(ExodusOverseerStatuette), 0x260C, 1075018, 0x0, 350000.0));	
            this.Rewards.Add(new CollectionItem(typeof(JukaLordStatuette), 0x25FC, 1075019, 0x0, 350000.0));	
            this.Rewards.Add(new CollectionItem(typeof(MeerCaptainStatuette), 0x25FA, 1075020, 0x0, 350000.0));	
            this.Rewards.Add(new CollectionItem(typeof(MeerEternalStatuette), 0x25F8, 1075021, 0x0, 350000.0));	
            this.Rewards.Add(new CollectionItem(typeof(SolenQueenStatuette), 0x2602, 1075022, 0x0, 350000.0));				
            this.Rewards.Add(new CollectionTitle(1073237, 1073639, 350000.0)); // Honored Vesper Museum Contributor
            this.Rewards.Add(new CollectionItem(typeof(MinaxsArmor), 0x1C02, 1073257, 0x281, 550000.0));		
			
            hues = new int[] { 0x281, 0x173, 0x581, 0x300 };	
            this.Rewards.Add(new CollectionHuedItem(typeof(GypsyHeaddress), 0x1544, 1073254, 0x453, 550000.0, hues));	
            this.Rewards.Add(new CollectionHuedItem(typeof(NystulsWizardsHat), 0x1718, 1073255, 0x453, 550000.0, hues));	
            this.Rewards.Add(new CollectionHuedItem(typeof(JesterHatOfChuckles), 0x171C, 1073256, 0x453, 550000.0, hues));		
            this.Rewards.Add(new CollectionItem(typeof(KeeoneansChainMail), 0x13BF, 1073264, 0x84E, 550000.0));			
            this.Rewards.Add(new CollectionTitle(1073238, 1073640, 550000.0)); // Prominent Vesper Museum Contributor	
            this.Rewards.Add(new CollectionItem(typeof(ClaininsSpellbook), 0xEFA, 1073262, 0x84D, 800000.0));	
            this.Rewards.Add(new CollectionItem(typeof(VesperOrderShield), 0x1BC4, 1073258, 0x835, 800000.0));		
            this.Rewards.Add(new CollectionItem(typeof(VesperChaosShield), 0x1BC3, 1073259, 0xFA, 800000.0));		
            this.Rewards.Add(new CollectionItem(typeof(BlackthornsKryss), 0x1401, 1073260, 0x5E5, 800000.0));	
            this.Rewards.Add(new CollectionItem(typeof(SwordOfJustice), 0x13B9, 1073261, 0x47E, 800000.0));	
            this.Rewards.Add(new CollectionItem(typeof(GeoffreysAxe), 0xF45, 1073263, 0x21, 800000.0));		
            this.Rewards.Add(new CollectionItem(typeof(VesperSpecialAchievementReplica), 0x2D4E, 1073265, 0x0, 800000.0));	
            this.Rewards.Add(new CollectionTitle(1073239, 1073641, 800000.0)); // Eminent Vesper Museum Contributor
        }

        public override void IncreaseTier()
        { 
            base.IncreaseTier();
			
            List<object> list = new List<object>();
            Item c;
			
            // don't know names above lev 6
            switch ( this.Tier )
            { 
                case 1:					
                    c = new BookOfChivalry();
                    c.MoveToWorld(new Point3D(2924, 979, -18), this.Map);
                    c.Movable = false;
                    list.Add(c);
					
                    c = new Longsword();
                    c.MoveToWorld(new Point3D(2923, 980, -18), this.Map);
                    c.Movable = false;
                    c.ItemID = 0x26CF;
                    list.Add(c);					
					
                    c = new Shirt();
                    c.MoveToWorld(new Point3D(2924, 978, -18), this.Map);
                    c.Movable = false;
                    c.ItemID = 0x2662;
                    c.Name = "Crisp White Shirt";
                    list.Add(c);
                    break;
                case 2:					
                    c = new GraveDust();
                    c.MoveToWorld(new Point3D(2921, 972, -17), this.Map);
                    c.Movable = false;
                    list.Add(c);	
					
                    c = new NoxCrystal();
                    c.MoveToWorld(new Point3D(2921, 972, -17), this.Map);
                    c.Movable = false;
                    list.Add(c);
					
                    c = new Static(0xF91);
                    c.MoveToWorld(new Point3D(2921, 972, -17), this.Map);
                    c.Movable = false;
                    list.Add(c);
					
                    c = new NecromancerSpellbook();
                    c.MoveToWorld(new Point3D(2922, 972, -18), this.Map);
                    c.Movable = false;
                    list.Add(c);
					
                    c = new AnimateDeadScroll();
                    c.MoveToWorld(new Point3D(2923, 972, -18), this.Map);
                    c.Movable = false;
                    list.Add(c);
					
                    c = new HorrificBeastScroll();
                    c.MoveToWorld(new Point3D(2923, 972, -18), this.Map);
                    c.Movable = false;
                    list.Add(c);
					
                    c = new VampiricEmbraceScroll();
                    c.MoveToWorld(new Point3D(2923, 971, -20), this.Map);
                    c.Movable = false;
                    list.Add(c);		
					
                    c = new Static(0xFDD);
                    c.MoveToWorld(new Point3D(2922, 971, -21), this.Map);
                    list.Add(c);		
					
                    c = new Static(0xFDE);
                    c.MoveToWorld(new Point3D(2923, 971, -21), this.Map);
                    list.Add(c);
					
                    break;
                case 3:				
                    c = new JesterSuit();
                    c.MoveToWorld(new Point3D(2919, 985, -16), this.Map);
                    c.Movable = false;
                    list.Add(c);	
					
                    c = new LocalizedStatic(0xE74, 1073424);
                    c.MoveToWorld(new Point3D(2919, 984, -11), this.Map);
                    c.Movable = false;
                    c.Weight = 50.0;
                    c.Hue = 0x113;
                    list.Add(c);	
					
                    c = new JesterHat();
                    c.MoveToWorld(new Point3D(2919, 983, -13), this.Map);
                    c.Movable = false;
                    c.Hue = 0x113;
                    list.Add(c);	
				
                    break;				
                case 4:						
                    c = new Static(0xD25);
                    c.MoveToWorld(new Point3D(2916, 984, -13), this.Map);
                    c.Movable = false;
                    list.Add(c);		
							
                    c = new Static(0x20D9);
                    c.MoveToWorld(new Point3D(2916, 982, -12), this.Map);
                    c.Name = "Gargoyle";
                    list.Add(c);
					
                    c = new Static(0x2132);
                    c.MoveToWorld(new Point3D(2914, 982, -9), this.Map);
                    list.Add(c);
					
                    c = new Static(0x25B6);
                    c.MoveToWorld(new Point3D(2913, 982, -13), this.Map);
                    list.Add(c);
					
                    c = new Static(0x25B6);
                    c.MoveToWorld(new Point3D(2913, 982, -13), this.Map);
                    list.Add(c);
					
                    c = new Static(0x222E);
                    c.MoveToWorld(new Point3D(2915, 983, -14), this.Map);
                    list.Add(c);
					
                    c = new Static(0x2211);
                    c.MoveToWorld(new Point3D(2914, 984, -13), this.Map);
                    list.Add(c);
					
                    break;
                case 5:									
                    c = new LocalizedStatic(0xE30, 1073421);
                    c.MoveToWorld(new Point3D(2911, 983, -12), this.Map);
                    c.Weight = 10.0;
                    list.Add(c);		
							
                    c = new LocalizedStatic(0x2937, 1073422);
                    c.MoveToWorld(new Point3D(2911, 984, -13), this.Map);
                    list.Add(c);	
							
                    c = new LocalizedStatic(0x12AA, 1073423);
                    c.MoveToWorld(new Point3D(2911, 985, -14), this.Map);
                    list.Add(c);	
					
                    c = new Static(0xEAF);
                    c.MoveToWorld(new Point3D(2910, 985, -21), this.Map);
                    c.Weight = 5.0;
                    list.Add(c);
					
                    c = new Static(0xEAE);
                    c.MoveToWorld(new Point3D(2910, 986, -21), this.Map);
                    c.Weight = 5.0;
                    list.Add(c);
					
                    break;
                case 6:										
                    c = new Tessen();
                    c.MoveToWorld(new Point3D(2910, 966, -17), this.Map);
                    c.Movable = false;
                    list.Add(c);	
									
                    c = new Shuriken();
                    c.MoveToWorld(new Point3D(2910, 965, -17), this.Map);
                    c.Movable = false;
                    list.Add(c);		
					
                    c = new Static(0x2855);
                    c.MoveToWorld(new Point3D(2910, 964, -16), this.Map);
                    c.Weight = 5.0;
                    list.Add(c);		
					
                    c = new Static(0x241D);
                    c.MoveToWorld(new Point3D(2910, 963, -20), this.Map);
                    c.Weight = 5.0;
                    list.Add(c);	
					
                    c = new Static(0x2409);
                    c.MoveToWorld(new Point3D(2910, 963, -17), this.Map);
                    list.Add(c);		
					
                    c = new Static(0x2416);
                    c.MoveToWorld(new Point3D(2909, 965, -17), this.Map);
                    list.Add(c);		
					
                    break;
                case 7:			
                    c = new Static(0x3069);
                    c.MoveToWorld(new Point3D(2914, 964, -21), this.Map);
                    list.Add(c);		
					
                    c = new Static(0x306A);
                    c.MoveToWorld(new Point3D(2913, 964, -21), this.Map);
                    list.Add(c);		
					
                    c = new Static(0x306B);
                    c.MoveToWorld(new Point3D(2912, 964, -21), this.Map);
                    list.Add(c);		
					
                    c = new ElvenLoveseatEastAddon();
                    c.MoveToWorld(new Point3D(2913, 966, -21), this.Map);
                    c.Movable = false;
                    list.Add(c);			
					
                    c = new Static(0x2CFC);
                    c.MoveToWorld(new Point3D(2912, 963, -21), this.Map);
                    list.Add(c);		
					
                    c = new LocalizedStatic(0x2D74, 1073425);
                    c.MoveToWorld(new Point3D(2914, 963, -21), this.Map);
                    list.Add(c);	
					
                    break;
                case 8:								
                    c = new Static(0x2);
                    c.MoveToWorld(new Point3D(2905, 970, -15), this.Map);
                    list.Add(c);		
								
                    c = new Static(0x3);
                    c.MoveToWorld(new Point3D(2905, 969, -15), this.Map);
                    list.Add(c);		
					
                    c = new OrderShield();
                    c.MoveToWorld(new Point3D(2905, 971, -17), this.Map);
                    c.Movable = false;
                    list.Add(c);		
					
                    c = new Static(0x1579);
                    c.MoveToWorld(new Point3D(2904, 971, -21), this.Map);
                    list.Add(c);
					
                    c = new Static(0x1613);
                    c.MoveToWorld(new Point3D(2908, 969, -21), this.Map);
                    list.Add(c);
					
                    c = new Static(0x1614);
                    c.MoveToWorld(new Point3D(2908, 968, -21), this.Map);
                    list.Add(c);				
					
                    break;
                case 9:				
                    c = new Static(0x1526);
                    c.MoveToWorld(new Point3D(2905, 976, -15), this.Map);
                    list.Add(c);		
					
                    c = new Static(0x1527);
                    c.MoveToWorld(new Point3D(2905, 975, -15), this.Map);
                    list.Add(c);							
					
                    c = new Static(0x151A);
                    c.MoveToWorld(new Point3D(2905, 972, -21), this.Map);
                    list.Add(c);					
					
                    c = new Static(0x151A);
                    c.MoveToWorld(new Point3D(2905, 977, -21), this.Map);
                    list.Add(c);					
					
                    c = new Static(0x151A);
                    c.MoveToWorld(new Point3D(2908, 972, -21), this.Map);
                    list.Add(c);					
					
                    c = new Static(0x151A);
                    c.MoveToWorld(new Point3D(2908, 977, -21), this.Map);
                    list.Add(c);	
					
                    c = new Static(0x1514);
                    c.MoveToWorld(new Point3D(2904, 975, -17), this.Map);
                    list.Add(c);	
					
                    break;
                case 10:		
                    c = new Static(0x15C5);
                    c.MoveToWorld(new Point3D(2904, 982, -21), this.Map);
                    list.Add(c);		
					
                    c = new Static(0x15C5);
                    c.MoveToWorld(new Point3D(2904, 979, -21), this.Map);
                    list.Add(c);			
					
                    c = new Static(0x157B);
                    c.MoveToWorld(new Point3D(2904, 981, -21), this.Map);
                    list.Add(c);
					
                    c = new Static(0x14E3);
                    c.MoveToWorld(new Point3D(2905, 980, -21), this.Map);
                    list.Add(c);
					
                    c = new Static(0x14E4);
                    c.MoveToWorld(new Point3D(2905, 981, -21), this.Map);
                    list.Add(c);
					
                    c = new Static(0x14E5);
                    c.MoveToWorld(new Point3D(2906, 981, -21), this.Map);
                    list.Add(c);
					
                    c = new Static(0x14E6);
                    c.MoveToWorld(new Point3D(2906, 980, -21), this.Map);
                    list.Add(c);
					
                    c = new ChaosShield();
                    c.MoveToWorld(new Point3D(2905, 978, -19), this.Map);
                    c.Movable = false;
                    list.Add(c);
					
                    break;
                case 11:					
                    c = new FemaleStuddedChest();
                    c.MoveToWorld(new Point3D(2912, 976, -16), this.Map);
                    c.Movable = false;
                    c.Hue = 0x497;
                    list.Add(c);	
					
                    c = new Static(0x1EA8);
                    c.MoveToWorld(new Point3D(2913, 973, -13), this.Map);
                    c.Hue = 0x497;
                    list.Add(c);		
					
                    c = new Static(0x20F8);
                    c.MoveToWorld(new Point3D(2913, 975, -11), this.Map);
                    c.Hue = 0x113;
                    list.Add(c);		
					
                    c = new Static(0x20E9);
                    c.MoveToWorld(new Point3D(2912, 974, -11), this.Map);
                    c.Name = "Troll";
                    list.Add(c);
					
                    c = new Static(0x2607);
                    c.MoveToWorld(new Point3D(2913, 974, -11), this.Map);
                    list.Add(c);
					
                    c = new Static(0x25F9);
                    c.MoveToWorld(new Point3D(2912, 975, -11), this.Map);
                    list.Add(c);
					
                    break;
                case 12:
                    c = new Static(0x1D8A);
                    c.MoveToWorld(new Point3D(2915, 976, -13), this.Map);
                    list.Add(c);
					
                    c = new Static(0x1D8B);
                    c.MoveToWorld(new Point3D(2916, 976, -13), this.Map);
                    list.Add(c);
					
                    c = new Static(0x234D);
                    c.MoveToWorld(new Point3D(2915, 975, -10), this.Map);
                    list.Add(c);
					
                    c = new WizardsHat();
                    c.MoveToWorld(new Point3D(2915, 974, -13), this.Map);
                    c.Movable = false;
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