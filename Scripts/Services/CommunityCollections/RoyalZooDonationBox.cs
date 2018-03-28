using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class MoonglowDonationBox : BaseCollectionItem
    {
        [Constructable]
        public MoonglowDonationBox()
            : base(0x9AA)
        {
            this.Hue = 0x48D;
            this.StartTier = 1000000;
            this.NextTier = 500000;
            this.DailyDecay = 100000;
        }

        public MoonglowDonationBox(Serial serial)
            : base(serial)
        { 
        }

        public override int LabelNumber
        {
            get
            {
                return 1073436;
            }
        }// Donation Box
        public override Collection CollectionID
        {
            get
            {
                return Collection.MoonglowZoo;
            }
        }
        public override int MaxTier
        {
            get
            {
                return 10;
            }
        }
        public override bool HandlesOnSpeech
        {
            get
            {
                return true;
            }
        }
        public override void Init()
        {
            base.Init();
			
            this.Donations.Add(new CollectionItem(typeof(Gold), 0xEEF, 1073116, 0x0, 0.06666));
            if (!Core.TOL)
                this.Donations.Add(new CollectionItem(typeof(BankCheck), 0x14F0, 1075013, 0x34, 0.06666));
            this.Donations.Add(new CollectionItem(typeof(DireWolf), 0x25D0, 1073118, 0x0, 30.0));
            this.Donations.Add(new CollectionItem(typeof(TimberWolf), 0x25D3, 1073118, 0x0, 30.0));
            this.Donations.Add(new CollectionItem(typeof(GreyWolf), 0x25D1, 1073118, 0x0, 30.0));
            this.Donations.Add(new CollectionItem(typeof(WhiteWolf), 0x25D2, 1073118, 0x0, 30.0));
            this.Donations.Add(new CollectionItem(typeof(Slime), 0x20E8, 1073117, 0x0, 45.0));
            this.Donations.Add(new CollectionItem(typeof(PolarBear), 0x20E1, 1073120, 0x0, 45.0));
            this.Donations.Add(new CollectionItem(typeof(Unicorn), 0x25CE, 1074821, 0x0, 250.0));
            this.Donations.Add(new CollectionItem(typeof(Kirin), 0x25A0, 1074821, 0x0, 250.0));
            this.Donations.Add(new CollectionItem(typeof(Nightmare), 0x259C, 1074821, 0x0, 250.0));
            this.Donations.Add(new CollectionItem(typeof(FireSteed), 0x21F1, 1074821, 0x0, 250.0));
            this.Donations.Add(new CollectionItem(typeof(SwampDragon), 0x2619, 1074821, 0x0, 250.0));
            this.Donations.Add(new CollectionItem(typeof(RuneBeetle), 0x276F, 1074820, 0x0, 300.0));
            this.Donations.Add(new CollectionItem(typeof(FireBeetle), 0x260F, 1074820, 0x489, 300.0));
            this.Donations.Add(new CollectionItem(typeof(Beetle), 0x260F, 1074820, 0x0, 300.0));
            this.Donations.Add(new CollectionItem(typeof(Drake), 0x20D6, 1073119, 0x0, 400.0));
            this.Donations.Add(new CollectionItem(typeof(Dragon), 0x20D6, 1073119, 0x0, 400.0));
            this.Donations.Add(new CollectionItem(typeof(Reptalon), 0x2D95, 1073121, 0x0, 550.0));
			
            int[] hues = new int[] 
            {
                0x555, 0xAE, 0x94, 0x278, 0x32, 0x28, 0x327, 0x41A
            };

            Rewards.Add(new CollectionItem(typeof(ForTheLifeOfBritanniaSash), 0x1541, 1075792, 0x0, 5000.0, true));
            this.Rewards.Add(new CollectionHuedItem(typeof(ZooMemberCloak), 0x1515, 1073221, 0x555, 100000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(ZooMemberRobe), 0x1F03, 1073221, 0x555, 100000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(ZooMemberSkirt), 0x1F01, 1073221, 0x555, 100000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(ZooMemberBodySash), 0x1541, 1073221, 0x555, 100000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(ZooMemberThighBoots), 0x1711, 1073221, 0x555, 100000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(ZooMemberFloppyHat), 0x1713, 1073221, 0x555, 100000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(ZooMemberBonnet), 0x1719, 1073221, 0x555, 100000.0, hues));
            this.Rewards.Add(new CollectionTitle(1073201, 1073624, 100000.0)); // Britannia Zoo Contributor
            this.Rewards.Add(new CollectionItem(typeof(QuagmireStatue), 0x2614, 1073195, 0x0, 200000.0));
            this.Rewards.Add(new CollectionItem(typeof(BakeKitsuneStatue), 0x2763, 1073189, 0x0, 200000.0));
            this.Rewards.Add(new CollectionItem(typeof(WolfStatue), 0x25D3, 1073190, 0x0, 200000.0));
            this.Rewards.Add(new CollectionItem(typeof(ChangelingStatue), 0x2D8A, 1073191, 0x0, 200000.0));
            this.Rewards.Add(new CollectionItem(typeof(ReptalonStatue), 0x2D95, 1073192, 0x0, 200000.0));
            this.Rewards.Add(new CollectionItem(typeof(PolarBearStatue), 0x20E1, 1073193, 0x0, 200000.0));
            this.Rewards.Add(new CollectionItem(typeof(SnakeStatue), 0x25C2, 1073194, 0x0, 200000.0));
            this.Rewards.Add(new CollectionTitle(1073202, 1073627, 200000.0)); // Distinguished Britannia Zoo Contributor
			
            hues = new int[]
            {
                0x34, 0x1C2, 0x2A3
            };
			
            this.Rewards.Add(new CollectionItem(typeof(SilverSteedZooStatuette), 0x259D, 1073219, 0x0, 350000.0));
            this.Rewards.Add(new CollectionHuedItem(typeof(QuagmireZooStatuette), 0x2614, 1074848, 0x34, 350000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(BakeKitsuneZooStatuette), 0x2763, 1074849, 0x34, 350000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(DireWolfZooStatuette), 0x25D0, 1073196, 0x34, 350000.0, hues));
            this.Rewards.Add(new CollectionItem(typeof(CraneZooStatuette), 0x2764, 1073197, 0x0, 350000.0));
            this.Rewards.Add(new CollectionHuedItem(typeof(PolarBearZooStatuette), 0x20E1, 1074851, 0x34, 350000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(ChangelingZooStatuette), 0x2D8A, 1074850, 0x34, 350000.0, hues));
            this.Rewards.Add(new CollectionHuedItem(typeof(ReptalonZooStatuette), 0x2D95, 1074852, 0x34, 350000.0, hues));
            this.Rewards.Add(new CollectionTitle(1073203, 1073628, 350000.0)); // Honored Britannia Zoo Contributor
			
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooLeatherLegs), 0x13CB, 1073222, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooLeatherGloves), 0x13C6, 1073222, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooLeatherGorget), 0x13C7, 1073222, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooLeatherArms), 0x13CD, 1073222, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooLeatherChest), 0x13CC, 1073222, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooLeatherFemaleChest), 0x1C06, 1073222, 0x109, 550000.0));			
			
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooStuddedLegs), 0x13DA, 1073223, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooStuddedGloves), 0x13D5, 1073223, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooStuddedGorget), 0x13D6, 1073223, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooStuddedArms), 0x13DC, 1073223, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooStuddedChest), 0x13DB, 1073223, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooStuddedFemaleChest), 0x1C02, 1073223, 0x109, 550000.0));
			
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooPlateLegs), 0x1411, 1073224, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooPlateGloves), 0x1414, 1073224, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooPlateGorget), 0x1413, 1073224, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooPlateArms), 0x1410, 1073224, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooPlateChest), 0x1415, 1073224, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooPlateFemaleChest), 0x1411, 1073224, 0x109, 550000.0));
            this.Rewards.Add(new CollectionItem(typeof(RoyalZooPlateHelm), 0x1412, 1073224, 0x109, 550000.0));
			
            this.Rewards.Add(new CollectionTitle(1073204, 1073629, 550000.0)); // Prominent Britannia Zoo Contributor
            this.Rewards.Add(new CollectionItem(typeof(SpecialAchievementZooStatuette), 0x2FF6, 1073226, 0x109, 800000.0));
            this.Rewards.Add(new CollectionTitle(1073205, 1073630, 800000.0)); // Eminent Britannia Zoo Contribuer
            this.Rewards.Add(new CollectionTitle(1073206, 1073631, 800000.0)); // Royal Subject of the Britannia Zoo
        }

        public override void IncreaseTier()
        { 
            base.IncreaseTier();
			
            List<object> list = new List<object>();
            BaseCreature c;
			
            // haven't got a clue if levels are OSI
            switch ( this.Tier )
            {
                case 1:					
                    c = new Crane();
                    c.MoveToWorld(new Point3D(4500, 1382, 23), this.Map);
                    c.Blessed = true;
                    c.Tamable = false;
                    list.Add(c);
					
                    break;
                case 2:					
                    c = new DireWolf();
                    c.MoveToWorld(new Point3D(4494, 1370, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    c = new DireWolf();
                    c.MoveToWorld(new Point3D(4494, 1360, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    c = new WhiteWolf();
                    c.MoveToWorld(new Point3D(4491, 1366, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    c = new WhiteWolf();
                    c.MoveToWorld(new Point3D(4497, 1366, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    c = new GreyWolf();
                    c.MoveToWorld(new Point3D(4497, 1366, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    break;
                case 3:					
                    c = new Quagmire();
                    c.MoveToWorld(new Point3D(4483, 1392, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
											
                    c = new BogThing();
                    c.MoveToWorld(new Point3D(4486, 1385, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);	
					
                    c = new PlagueBeast();
                    c.MoveToWorld(new Point3D(4486, 1379, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
				
                    break;				
                case 4:					
                    c = new PolarBear();
                    c.MoveToWorld(new Point3D(4513, 1395, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    c = new PolarBear();
                    c.MoveToWorld(new Point3D(4508, 1393, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    break;
                case 5:					
                    c = new Yamandon();
                    c.MoveToWorld(new Point3D(4498, 1393, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    break;
                case 6:					
                    c = new Changeling();
                    c.MoveToWorld(new Point3D(4518, 1358, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    break;
                case 7:					
                    c = new Wyvern();
                    c.MoveToWorld(new Point3D(4512, 1381, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    break;
                case 8:					
                    c = new Dragon();
                    c.MoveToWorld(new Point3D(4511, 1372, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    c = new Drake();
                    c.MoveToWorld(new Point3D(4516, 1371, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    break;
                case 9:					
                    c = new Reptalon();
                    c.MoveToWorld(new Point3D(4530, 1387, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    break;
                case 10:				
                    c = new SilverSteed();
                    c.MoveToWorld(new Point3D(4506, 1358, 23), this.Map);
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add(c);
					
                    /*
                    c = new Sphynx();
                    c.MoveToWorld( new Point3D( 4506, 1358, 23 ), Map );
                    c.Blessed = true;	
                    c.Tamable = false;
                    list.Add( c );*/
					
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
