//Based on three scripts from RunUO Distro 1.0  Thanks RunUO!!!!
//Created by Ashlar, beloved of Morrigan
using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	public class BaseTamingCreature : BaseCreature
	{
		public Mobile m_From;

		public BaseTamingCreature( Mobile from ) : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			m_From = from;
			CantWalk = true;
			Tamable = true;
			ControlSlots = 0;
		}

		public override int Meat{ get{ return 1; } }
		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override FoodType FavoriteFood{ get{ return FoodType.GrainsAndHay; } }

		public override void OnDoubleClick( Mobile from )
		{
			if( from == m_From )
				Delete();
		}

		public override bool CanBeDamaged()
		{
			return false;
		}

		public override void OnDelete()
		{
			base.OnDelete();
			TamersLittleHelper tlh = new TamersLittleHelper();
			m_From.AddToBackpack( tlh );
			m_From.SendGump( new TamersLittleHelperGump( m_From, tlh ) );
		}

		public bool HandlesOnMovement{ get{ return true; } }

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( !InRange( m.Location, 8 ) && !InRange( oldLocation, 7 ) && (m == m_From ) )
			{
				Delete();
			}

			base.OnMovement( m, oldLocation );
		}


		public BaseTamingCreature(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);

			writer.Write( m_From );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			m_From = reader.ReadMobile();
		}
	}
}

