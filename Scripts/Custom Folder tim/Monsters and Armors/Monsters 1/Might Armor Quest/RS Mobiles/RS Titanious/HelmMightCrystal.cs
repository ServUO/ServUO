using System;
using Server;
using Server.Gumps;
using Server.Network;
using System.Collections;
using Server.Multis;
using Server.Mobiles;


namespace Server.Items
{

	public class HelmMightCrystal : Item
	{
		[Constructable]
		public HelmMightCrystal() : this( null )
		{
		}

		[Constructable]
		public HelmMightCrystal ( string name ) : base ( 0x1F1C )
		{
			Name = "Crystal of Might (Helm)";
			LootType = LootType.Regular;
			Hue = 2786;
		}

		public HelmMightCrystal ( Serial serial ) : base ( serial )
		{
		}

      		
		public override void OnDoubleClick( Mobile m )

		{
			Item d = m.Backpack.FindItemByType( typeof(HelmoftheUnderworld) );
			if ( d != null )
			{	
				Item c = m.Backpack.FindItemByType( typeof(HelmoftheSea) );
				if ( c != null )
				{
					Item n = m.Backpack.FindItemByType( typeof(HelmoftheHeavens) );
					if ( n != null )
					{
						Item p = m.Backpack.FindItemByType( typeof(TitanEssence) );
						if ( p != null )
						{
							m.AddToBackpack( new HelmofMight() );
							d.Delete();
							c.Delete();
							n.Delete();
							p.Delete();
							m.SendMessage( "You Combine the Power Of The Gods!!!" );
						}
					}
					else
					{
						m.SendMessage( "You Are Missing Something..." );
					}
				}
			}
			
		}

		public override void Serialize ( GenericWriter writer)
		{
			base.Serialize ( writer );

			writer.Write ( (int) 0);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize ( reader );

			int version = reader.ReadInt();
		}
	}
}