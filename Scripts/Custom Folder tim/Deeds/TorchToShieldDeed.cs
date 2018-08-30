//BY: SHAMBAMPOW
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class TorchToShieldTarget : Target
	{
		private TorchToShieldDeed m_Deed;

		public TorchToShieldTarget( TorchToShieldDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is Torch )
			{
				Item torch = (Item)target;

				if( torch.RootParent != from )
				{
					from.SendMessage( "You cannot convert that there!" );
				}
				else
				{
					BaseArmor shield = new Buckler();
					shield.ItemID = torch.ItemID;
					shield.Name = torch.Name;
					shield.Hue = torch.Hue;
					shield.LootType = torch.LootType;

					shield.Attributes.SpellChanneling = 1;
					shield.PoisonBonus = -1;
					shield.MaxHitPoints = 255;
					shield.HitPoints = 255;

					if( torch.Insured )
						shield.Insured = true;


				
					from.AddToBackpack( shield );
					torch.Delete();
					from.SendMessage( "You magically convert the torch into a torch-shield...." );

					m_Deed.Delete();
				}
			}
			else
			{
				from.SendMessage( "That is not a torch." );
			}
		}
	}

	public class TorchToShieldDeed : Item 
	{
		[Constructable]
		public TorchToShieldDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "Torch to Shield Deed";
			LootType = LootType.Blessed;
			Hue = 1177;
		}

		public TorchToShieldDeed( Serial serial ) : base( serial )
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
				from.SendMessage("What torch would you like to convert into a torch-shield?"  );
				from.Target = new TorchToShieldTarget( this ); // Call our target
			 }
		}	
	}
}


