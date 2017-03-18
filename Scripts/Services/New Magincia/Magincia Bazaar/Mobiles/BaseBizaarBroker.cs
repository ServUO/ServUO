using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
	public class BaseBazaarBroker : BaseCreature
	{
		private MaginciaBazaarPlot m_Plot;
		private int m_BankBalance;
		private string m_ShopName;
		private DateTime m_NextFee;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public MaginciaBazaarPlot Plot { get { return m_Plot; } set { m_Plot = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int BankBalance { get { return m_BankBalance; } set { m_BankBalance = value; } }
		
		public virtual int ComissionFee { get { return MaginciaBazaar.DefaultComissionFee; } }

        public override bool IsInvulnerable { get { return true; } }
		
		public BaseBazaarBroker(MaginciaBazaarPlot plot) : base(AIType.AI_Vendor, FightMode.None, 2, 1, 0.5, 2)
		{
			m_Plot = plot;
			m_BankBalance = 0;
			m_NextFee = DateTime.UtcNow + TimeSpan.FromHours(23);
			InitBody();
			InitOutfit();

            Blessed = true;
            CantWalk = true;
            Direction = Direction.East;
		}
		
		public virtual void InitOutfit()
		{
			switch (Utility.Random(3))
			{
				case 0: EquipItem(new FancyShirt(GetRandomHue())); break;
				case 1: EquipItem(new Doublet(GetRandomHue())); break;
				case 2: EquipItem(new Shirt(GetRandomHue())); break;
			}

			switch (Utility.Random(4))
			{
				case 0: EquipItem(new Shoes()); break;
				case 1: EquipItem(new Boots()); break;
				case 2: EquipItem(new Sandals()); break;
				case 3: EquipItem(new ThighBoots()); break;
			}

			if (Female)
			{
				switch (Utility.Random(6))
				{
					case 0: EquipItem(new ShortPants(GetRandomHue())); break;
					case 1:
					case 2: EquipItem(new Kilt(GetRandomHue())); break;
					case 3:
					case 4:
					case 5: EquipItem(new Skirt(GetRandomHue())); break;
				}
			}
			else
			{
				switch (Utility.Random(2))
				{
					case 0: EquipItem(new LongPants(GetRandomHue())); break;
					case 1: EquipItem(new ShortPants(GetRandomHue())); break;
				}
			}
		}
		
		public virtual void InitBody()
		{
			InitStats( 100, 100, 25 );

			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();

			if ( IsInvulnerable && !Core.AOS )
				NameHue = 0x35;

			if ( Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}
			
			Hue = Race.RandomSkinHue();

            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();

            FacialHairItemID = Race.RandomFacialHair(Female);
            if (FacialHairItemID != 0)
                FacialHairHue = Race.RandomHairHue();
            else
                FacialHairHue = 0;
		}
		
		public virtual int GetRandomHue()
		{
			switch ( Utility.Random( 5 ) )
			{
				default:
				case 0: return Utility.RandomBlueHue();
				case 1: return Utility.RandomGreenHue();
				case 2: return Utility.RandomRedHue();
				case 3: return Utility.RandomYellowHue();
				case 4: return Utility.RandomNeutralHue();
			}
		}
		
		public void Dismiss()
		{
			Delete();
		}
		
		public override void Delete()
		{
			if(m_Plot != null && MaginciaBazaar.Instance != null)
				MaginciaBazaar.Instance.AddInventoryToWarehouse(m_Plot.Owner, this);
		
			base.Delete();
		}
		
		public virtual void OnTick()
		{
			if(m_NextFee < DateTime.UtcNow)
			{
				m_BankBalance -= GetWeeklyFee() / 7;
			
				m_NextFee = DateTime.UtcNow + TimeSpan.FromHours(23);
			}
		}
		
		public virtual int GetWeeklyFee()
		{
			return 0;
		}
		
		public void TryWithdrawFunds(Mobile from, int amount)
		{
			if(m_BankBalance < amount || !Banker.Deposit(from, amount))
				from.SendLocalizedMessage(1150214); // Transfer of funds from the broker to your bank box failed. Please check the amount to transfer is available in the broker's account, and make sure your bank box is able to hold the new funds without becoming overloaded.
			else
			{
				m_BankBalance -= amount;
				from.SendMessage("You withdraw {0}gp to your broker.", amount);
                from.PlaySound(0x37);
			}
		}
		
		public void TryDepositFunds(Mobile from, int amount)
		{
			if(Banker.Withdraw(from, amount))
			{
				m_BankBalance += amount;
				from.SendMessage("You deposit {0}gp to your brokers account.", amount);
                from.PlaySound(0x37);
			}
			else
			{
				from.SendLocalizedMessage(1150215); // You have entered an invalid value, or a non-numeric value. Please try again.ase deposit the necessary funds into your bank box and try again.
			}
		}
		
		public BaseBazaarBroker(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			
			writer.Write(m_BankBalance);
			writer.Write(m_NextFee);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			m_BankBalance = reader.ReadInt();
			m_NextFee = reader.ReadDateTime();
		}
	}
}