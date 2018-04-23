//BY: SHAMBAMPOW
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class LanternToShieldTarget : Target
	{
		private LanternToShieldDeed m_Deed;

		public LanternToShieldTarget( LanternToShieldDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is Lantern )
			{
				Item lantern = (Item)target;

				if( lantern.RootParent != from )
				{
					from.SendMessage( "The lantern must be with you!" );
				}
				else
				{
					BaseArmor shield = new Buckler();
					shield.ItemID = lantern.ItemID;
					shield.Name = lantern.Name;
					shield.Hue = lantern.Hue;
					shield.LootType = lantern.LootType;

					shield.Attributes.SpellChanneling = 1;
					shield.PoisonBonus = -1;
					shield.MaxHitPoints = 255;
					shield.HitPoints = 255;

					if( lantern.Insured )
						shield.Insured = true;


				
					from.AddToBackpack( shield );
					lantern.Delete();
					from.SendMessage( "You magically convert the lantern into a lantern-shield..." );

					m_Deed.Delete();
				}
			}
			else
			{
				from.SendMessage( "That is not a lantern." );
			}
		}
	}

	public class LanternToShieldDeed : Item 
	{
		[Constructable]
		public LanternToShieldDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "Lantern To Shield deed";
			LootType = LootType.Blessed;
			Hue = 1177;
		}

		public LanternToShieldDeed( Serial serial ) : base( serial )
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
				from.SendMessage("What lantern would you like to convert into a lantern-shield?"  );
				from.Target = new LanternToShieldTarget( this ); // Call our target
			 }
		}	
	}
}


