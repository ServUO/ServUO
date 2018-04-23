
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections;
using Server.Network;


namespace Server.Items
{

	public class NagsTeleporter : Item
	{
		private Point3D m_DestLoc;
		private Map     m_DestMap;
		private bool    m_AllowCreatures;
		private bool    m_TelePets;

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Location
		{
			get { return m_DestLoc; }
			set { m_DestLoc = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Map Map
		{
			get { return m_DestMap; }
			set { m_DestMap = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool AllowCreatures
		{
			get { return m_AllowCreatures; }
			set { m_AllowCreatures = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool TelePets
		{
			get { return m_TelePets; }
			set { m_TelePets = value; InvalidateProperties(); }
		}

		[Constructable]
		public NagsTeleporter() : base( 3948 )
		{
			Visible = false;
			Hue = 1;
			Movable = false;
			Weight = 0.0;
			Name = "Nags Teleporter";
		}

		public NagsTeleporter( Serial serial ) : base( serial )
		{
		}

		public override bool OnMoveOver( Mobile m )
		{

                        PlayerMobile pm = m as PlayerMobile;

			if( !m_AllowCreatures && !m.Player )
				return true;

			if( pm.Backpack.ConsumeTotal( typeof( NagsSting ), 1 ) )

			{
				if( m_TelePets )
				{
					Server.Mobiles.BaseCreature.TeleportPets( m, m_DestLoc, m_DestMap );
				}

				pm.PlaySound(0x1F7);
				pm.MoveToWorld( m_DestLoc, m_DestMap );
				
				return false;
			}

			pm.SendMessage( " You must have Nag's Sting if you wish to pass." );
			return true;
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

			writer.Write( m_DestLoc );
			writer.Write( m_DestMap );
			writer.Write( m_AllowCreatures );
			writer.Write( m_TelePets );
		}


		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			m_DestLoc = reader.ReadPoint3D();
			m_DestMap = reader.ReadMap();
			m_AllowCreatures = reader.ReadBool();
			m_TelePets = reader.ReadBool();
		}
	}
}