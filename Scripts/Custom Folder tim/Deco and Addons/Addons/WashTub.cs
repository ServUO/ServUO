using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class WashTub : Item, IWaterSource
	{
		private int m_Quantity;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Quantity
		{
			get
			{
				CheckQuantity();
				//if (ItemID == 0x154D) return 100;

				return m_Quantity;
			}
			set
			{
				int oldValue = m_Quantity;

				if ( oldValue != value )
				{
					m_Quantity = value;
				}
				CheckQuantity();
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if (!from.InRange( this.GetWorldLocation(), 1 ))
			{
				from.SendMessage( "I can't reach that." );
				return;
			}

			if(m_Quantity < 1)
			{
				from.SendMessage("That appears to be empty.");
				return;
			}
			else
			{
				if (from.Thirst < 20)
				{
					from.SendMessage("You drink from the tub.");
					from.Direction = ((from.GetDirectionTo(this)) & Direction.Mask);
					from.PlaySound( 0x30 );
					from.Animate( 32, 5, 1, true, false, 0 );
					from.Emote("*Drinks*");
					m_Quantity -=1;
					from.Thirst +=1;
					CheckQuantity();
				}
				else
				{
					from.SendMessage("You wash yourself.");
					from.Direction = ((from.GetDirectionTo(this)) & Direction.Mask);
					from.PlaySound( 0x4E );
					from.Animate( 32, 5, 1, true, false, 0 );
					from.Emote("*Feeling Fresh*");
				}
			}
		}

		public override bool OnMoveOver( Mobile m )
		{
			return false;
		}

		public virtual void CheckQuantity()
		{
			InvalidateProperties();
			if (m_Quantity > 40) 
			{
				ItemID = 0xE7B;
			}
			else ItemID = 0xE83;

			if (m_Quantity > 25)
			{
				Movable = false;
			}
			else 
			{
				Movable = true;
			}
			if (m_Quantity > 50) m_Quantity = 50;
			Weight = m_Quantity + 15;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add(GetQuantityDescription());
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			LabelTo( from, GetQuantityDescription() );
		}

		public virtual string GetQuantityDescription()
		{
			if ( m_Quantity <= 0 )
				return "It's Empty";
			else if ( m_Quantity <= 15 )
				return "It's nearly empty.";
			else if ( m_Quantity <= 30 )
				return "It's half full.";
			else
				return "It's full.";
		}

		[Constructable]
		public WashTub() : base( 0xE83 )
		{
			Name = "a wash tub";
			Weight = 15;
		}

		public WashTub( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			writer.Write( (int) m_Quantity);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_Quantity = reader.ReadInt();
		}
	}
}
