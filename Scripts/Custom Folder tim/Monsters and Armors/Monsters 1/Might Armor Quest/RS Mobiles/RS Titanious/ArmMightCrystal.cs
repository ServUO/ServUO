using System;
using Server;
using Server.Gumps;
using Server.Network;
using System.Collections;
using Server.Multis;
using Server.Mobiles;


namespace Server.Items
{

	public class ArmMightCrystal : Item
	{
		[Constructable]
		public ArmMightCrystal() : this( null )
		{
		}

		[Constructable]
		public ArmMightCrystal ( string name ) : base ( 0x1F1C )
		{
			Name = "Crystal of Might (Arms)";
			LootType = LootType.Regular;
			Hue = 2786;
		}

		public ArmMightCrystal ( Serial serial ) : base ( serial )
		{
		}

      		
		public override void OnDoubleClick( Mobile m )

		{
			Item d = m.Backpack.FindItemByType( typeof(ArmsoftheUnderworld) );
			if ( d != null )
			{	
				Item c = m.Backpack.FindItemByType( typeof(ArmsoftheSea) );
				if ( c != null )
				{
					Item n = m.Backpack.FindItemByType( typeof(ArmsoftheHeavens) );
					if ( n != null )
					{
						Item p = m.Backpack.FindItemByType( typeof(TitanEssence) );
						if ( p != null )
						{
							m.AddToBackpack( new ArmsofMight() );
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