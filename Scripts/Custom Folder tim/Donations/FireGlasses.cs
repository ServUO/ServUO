//Script by zite

using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class FireGlasses : Item
	{
		public int m_hue;
		public int m_maxhue;
		public int m_minhue;
		public int m_updown;
		public float m_speed;

		[CommandProperty( AccessLevel.GameMaster )]
		public int hue
		{
			get{ return m_hue; }
			set{ m_hue = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int maxhue
		{
			get{ return m_maxhue; }
			set{ m_maxhue = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int minhue
		{
			get{ return m_minhue; }
			set{ m_minhue = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public float speed
		{
			get{ return m_speed; }
			set
            {
                if (value >= 0 && value < TimeSpan.MaxValue.TotalSeconds)
                    m_speed = value;
            }
		}

		[Constructable]
		public FireGlasses() : base( 0x2FB8 )
		{
			Name = "fire glasses";
			Hue = 1258;
			Layer = Layer.Earrings;
			m_hue = 1258;
			m_maxhue = 1260;
			m_minhue = 1256;
			m_updown = 3;
		}

		public FireGlasses( Serial serial ) : base( serial )
		{}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( this.RootParent == from )
			{
				if (this.m_updown == 3)
				{
					this.m_updown = 1;
					new InternalTimer( this ).Start();
				}
				else
				{
					this.m_updown = 3;
					from.EndAction( typeof(FireGlasses) );
				}
			}
		}

		private class InternalTimer : Timer
		{
			private FireGlasses m_item;

			public InternalTimer( FireGlasses item ) : base( TimeSpan.FromSeconds( 1.0 ) )
			{
				if (item.m_updown == 1)
				{
					if (item.m_hue >= item.m_maxhue)
					{
						item.m_updown = 2;
						item.m_hue = item.m_maxhue;
						item.m_hue--;
					}
					else
						item.m_hue++;
				}
				else if (item.m_updown == 2)
				{
					if (item.m_hue <= item.m_minhue)
					{
						item.m_updown = 1;
						item.m_hue = item.m_minhue;
						item.m_hue++;
					}
					else
						item.m_hue--;
				}
				m_item = item;
			}

			protected override void OnTick()
			{
				m_item.Hue = m_item.m_hue;
				new InternalTimer( m_item ).Start();
			}
		}
	}
}