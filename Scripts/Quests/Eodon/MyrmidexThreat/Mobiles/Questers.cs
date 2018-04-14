using Server;
using System;
using Server.Engines.Quests;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class Yar : MondainQuester
	{
        public override Type[] Quests { get { return new Type[] { typeof(TheZealotryOfZipactriotlQuest) }; } }
																  
		public override bool ChangeRace { get { return false; } }
		 
		[Constructable]
		public Yar() : base("Yar", "the Barrab Tinker")
		{ 
		}		

		public override void InitBody()
		{
			InitStats(100, 100, 25);
			
			Female = false;
            Body = 0x190;
            HairItemID = Race.RandomHair(false);
            Hue = 34214;
		}
		
		public override void InitOutfit()
		{
			SetWearable(new BoneChest(), 1828);
			SetWearable(new DecorativePlateKabuto(), 1828);
			SetWearable(new LeatherHaidate(), 1828);
			SetWearable(new Sandals(), 1828);
			SetWearable(new SledgeHammer(), 1828);
		}

        public override void OnOfferFailed()
        {
            Say(1080107); // I'm sorry, I have nothing for you at this time.
        }

		public Yar(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class Carroll : MondainQuester
	{
		public override Type[] Quests { get { return new Type[] { typeof(HiddenTreasuresQuest) }; } }
																  
		public override bool ChangeRace { get { return false; } }
		
		[Constructable]
		public Carroll() : base("Carroll", "the Gemologist")
		{
		}			
		
		public override void InitBody()
		{
			InitStats(100, 100, 25);
			
			Female = false;
            Body = 0x190;
            HairItemID = Race.RandomHair(false);
            Hue = Race.RandomSkinHue();
		}
		
		public override void InitOutfit()
		{
			SetWearable(new FancyShirt());
			SetWearable(new Doublet(), 105);
			SetWearable(new LongPants(), 107);
			SetWearable(new SilverNecklace());
			SetWearable(new Shoes());
		}
		
		public Carroll(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class Bront : MondainQuester
	{
		public override Type[] Quests { get { return new Type[] { typeof(TheSaltySeaQuest) }; } }
																  
		public override bool ChangeRace { get { return false; } }
		
		[Constructable]
		public Bront() : base("Bront", "the Captain")
		{
		}			
		
		public override void InitBody()
		{
			InitStats(100, 100, 25);
			
			Female = false;
            Body = 0x190;
            HairItemID = Race.RandomHair(false);
            Hue = Race.RandomSkinHue();
		}
		
		public override void InitOutfit()
		{
			SetWearable(new StuddedChest());
			SetWearable(new BodySash(), 128);
			SetWearable(new LongPants());
			SetWearable(new TricorneHat());	
			SetWearable(new Sandals());
		}
		
		public Bront(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class Eriathwen : MondainQuester
	{
		public override Type[] Quests { get { return new Type[] { typeof(ATinkersTaskQuest) }; } }
																  
		public override bool ChangeRace { get { return false; } }
		
		[Constructable]
		public Eriathwen() : base("Eriathwen", "the Golem Maker")
		{
		}

		public override void InitBody()
		{
			InitStats(100, 100, 25);
			
			Race = Race.Elf;
			Female = true;
            Body = 605;
            HairItemID = Race.RandomHair(true);
            Hue = 0x847E;
		}
		
		public override void InitOutfit()
		{
			SetWearable(new ElvenShirt(), 164);
			SetWearable(new ElvenPants(), 1114);
			SetWearable(new ElvenBoots(), 1828);
		}		
		
		public Eriathwen(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class CollectorOfOddities : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

	    public override void InitSBInfo()
        {
            m_SBInfos.Add( new InternalSB() );
        }
	   
		[Constructable]
		public CollectorOfOddities() : base("the collector of oddities")
		{
		}			
		
		public override void InitOutfit()
		{
			SetWearable(new FancyShirt(), 1266);
			SetWearable(new Doublet(), 1266);
			SetWearable(new LongPants(), 1266);
			SetWearable(new Boots());
		}	
		
		private class InternalSB : SBInfo
		{
			private List<GenericBuyInfo> m_BuyInfo;
			private IShopSellInfo m_SellInfo = new InternalSellInfo();

			public InternalSB() : this(null)
			{
			}

			public InternalSB(BaseVendor owner)
			{
				m_BuyInfo = new InternalBuyInfo(owner);
			}

			public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
			public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

			public class InternalBuyInfo : List<GenericBuyInfo>
			{
				public InternalBuyInfo(BaseVendor owner)
				{  
					Add( new GenericBuyInfo( "Stasis Chamber Power Core", typeof( StasisChamberPowerCore ), 101250, 500, 40155, 0 ) );
				}
			}

			public class InternalSellInfo : GenericSellInfo
			{
				public InternalSellInfo()
				{
				}
			}
		}
		
		public CollectorOfOddities(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class EllieRafkin : MondainQuester
	{
        public override Type[] Quests { get { return new Type[] { typeof(ExterminatingTheInfestationQuest) }; } }
																  
		public override bool ChangeRace { get { return false; } }
        public override bool IsActiveVendor { get { return true; } }

        private List<SBInfo> _SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return _SBInfos; } }

        public override void InitSBInfo()
        {
            _SBInfos.Add(new InternalSB());
        }

		[Constructable]
		public EllieRafkin() : base("Ellie Rafkin", "the Professor")
		{
		}

        public override void OnOfferFailed()
        {
            Say(1080107); // I'm sorry, I have nothing for you at this time.
        }

		public override void InitBody()
		{
			InitStats(100, 100, 25);
			
			Female = true;
            Body = 0x191;
            HairItemID = Race.RandomHair(true);
            Hue = Race.RandomSkinHue();
		}
		
		public override void InitOutfit()
		{
			SetWearable(new FancyShirt());		
			SetWearable(new Kilt(), 933);
			SetWearable(new ThighBoots(), 1);
		}

        private class InternalSB : SBInfo
        {
            private List<GenericBuyInfo> m_BuyInfo;
            private IShopSellInfo m_SellInfo = new InternalSellInfo();

            public InternalSB()
                : this(null)
            {
            }

            public InternalSB(BaseVendor owner)
            {
                m_BuyInfo = new InternalBuyInfo(owner);
            }

            public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
            public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

            public class InternalBuyInfo : List<GenericBuyInfo>
            {
                public InternalBuyInfo(BaseVendor owner)
                {
                    Add(new GenericBuyInfo("Unabridged Map of Eodon", typeof(UnabridgedAtlasOfEodon), 62500, 500, 7185, 0));
                }
            }

            public class InternalSellInfo : GenericSellInfo
            {
                public InternalSellInfo()
                {
                }
            }
        }

		public EllieRafkin(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class Foxx : MondainQuester
	{
		public override Type[] Quests { get { return new Type[] { typeof(PestControlQuest) }; } } 
																  
		public override bool ChangeRace { get { return false; } }
		
		[Constructable]
		public Foxx() : base("Foxx", "the Lieutenant")
		{
		}

		public override void InitBody()
		{
			InitStats(100, 100, 25);
			
			Female = false;
            Body = 0x190;
            HairItemID = Race.RandomHair(true);
            Hue = Race.RandomSkinHue();
		}

        public override void InitOutfit()
        {
            SetWearable(new PlateChest());
            SetWearable(new PlateLegs());
            SetWearable(new BodySash(), 1828);
            SetWearable(new OrderShield());
            SetWearable(new Longsword());
        }

        public override void Advertise()
        {
            Say(1156619); // Fall in now! These Myrmidex aren't going to slay themselves! We've got to squash these bugs!
        }

        public Foxx(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class Yero : MondainQuester
	{
		public override Type[] Quests { get { return new Type[] { typeof(GettingEvenQuest) }; } } 
																  
		public override bool ChangeRace { get { return false; } }
		
		[Constructable]
		public Yero() : base("Yero", "the Gambler")
		{
		}

		public override void InitBody()
		{
			InitStats(100, 100, 25);
			
			Female = false;
            Body = 0x190;
            HairItemID = Race.RandomHair(true);
            Hue = Race.RandomSkinHue();
		}
		
		public override void InitOutfit()
		{
			SetWearable(new ShortPants());
			SetWearable(new Sandals());
		}		
		
		public Yero(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class Alida : MondainQuester
	{
		public override Type[] Quests { get { return new Type[] { typeof(OfVorpalsAndLettacesTheGardnerQuest) }; } } 
																  
		public override bool ChangeRace { get { return false; } }
		
		[Constructable]
		public Alida() : base("Alida", "the Gardener")
		{
		}

		public override void InitBody()
		{
			InitStats(100, 100, 25);
			
			Female = true;
            Body = 0x191;
            HairItemID = Race.RandomHair(true);
            Hue = Race.RandomSkinHue();
		}
		
		public override void InitOutfit()
		{
			SetWearable(new Shirt());
			SetWearable(new LongPants(), 1);
			SetWearable(new HalfApron(), 263);
			SetWearable(new LeatherGloves());
            SetWearable(new FloppyHat());
		}		
		
		public Alida(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
}