// This Script was Scripted by Espcevan
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class WeightlessTarget : Target
	{
		private WeightlessDeed m_Deed;

		public WeightlessTarget( WeightlessDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is Item )
			{
				Item item = (Item)target;
				if (item is Item)
				{
					if ( ((Item)item).Weight == 0 ) from.SendMessage( "That item is already weightless!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You can make this weightless!" );
						else
						{
							((Item)item).Weight = 0;
							from.SendMessage( "You magically make your item weightless...." );
							m_Deed.Delete();
						}
					}
				}
			}
			else from.SendMessage( "You can not make that weightless" );
		}
	}

	public class WeightlessDeed : Item
	{
		[Constructable]
		public WeightlessDeed() : base( 0x14F0 )
		{
			Name = "a Weightless deed";
			Hue = 0;
		}

		public WeightlessDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) from.SendLocalizedMessage( 1042001 );
			else
			{
				from.SendMessage("What item would you like to make weightless?"  );
				from.Target = new WeightlessTarget( this );
			 }
		}
	}
}