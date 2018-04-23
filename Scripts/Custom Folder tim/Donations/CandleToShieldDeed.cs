//BY: SHAMBAMPOW
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class CandleToShieldTarget : Target
	{
		private CandleToShieldDeed m_Deed;

		public CandleToShieldTarget( CandleToShieldDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is Candle )
			{
				Item candle = (Item)target;

				if( candle.RootParent != from )
				{
					from.SendMessage( "You cannot convert that there!" );
				}
				else
				{
					BaseArmor shield = new Buckler();
					shield.ItemID = candle.ItemID;
					shield.Name = candle.Name;
					shield.Hue = candle.Hue;
					shield.LootType = candle.LootType;

					shield.Attributes.SpellChanneling = 1;
					shield.PoisonBonus = -1;
					shield.MaxHitPoints = 255;
					shield.HitPoints = 255;

					if( candle.Insured )
						shield.Insured = true;


				
					from.AddToBackpack( shield );
					candle.Delete();
					from.SendMessage( "You magically convert the candle into a candle-shield...." );

					m_Deed.Delete();
				}
			}
			else
			{
				from.SendMessage( "That is not a candle." );
			}
		}
	}

	public class CandleToShieldDeed : Item 
	{
		[Constructable]
		public CandleToShieldDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "Candle to Shield Deed";
			LootType = LootType.Blessed;
			Hue = 1177;
		}

		public CandleToShieldDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendMessage("What candle would you like to convert into a candle-shield?"  );
				from.Target = new CandleToShieldTarget( this ); // Call our target
			 }
		}	
	}
}


