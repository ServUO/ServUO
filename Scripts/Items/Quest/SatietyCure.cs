using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class SatietyCure : Item
	{
		private int m_Uses;

        public override int LabelNumber { get { return 1080542; } } // Pepta's Satiety Cure

		[CommandProperty( AccessLevel.GameMaster )]
		public int Uses
		{
			get{ return m_Uses; }
			set{ m_Uses = value; }
		}

		[Constructable]
		public SatietyCure() : base( 0xEFC )
		{
			this.Weight = 1.0;
			this.Hue = 235;
            this.LootType = LootType.Blessed;
			m_Uses = 10;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				SendLocalizedMessageTo( from, 1042038 ); // You must have the object in your backpack to use it.
				return;
			}

			if ( m_Uses > 0 )
			{
				from.PlaySound( 0x2D6 );
				from.SendLocalizedMessage( 501206 ); // An awful taste fills your mouth.

                if (from.Hunger > 0)
				{
                    from.Hunger = 0;
					from.SendMessage( "You feel as if you could eat more." ); 
				}

				m_Uses--;
			}
			else
			{
				Delete();
				from.SendLocalizedMessage( 501201 ); // There wasn't enough left to have any effect.
			}
		}

        public SatietyCure(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			writer.WriteEncodedInt( m_Uses );
		}

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Uses = reader.ReadEncodedInt();
        }
	}
}