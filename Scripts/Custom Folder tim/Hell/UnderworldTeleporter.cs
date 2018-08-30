
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections;
using Server.Network;


namespace Server.Items
{

	public class UnderWorldTeleporter : Item
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
		public UnderWorldTeleporter() : base( 3948 )
		{
			Visible = true;
			Hue = 1;
			Movable = false;
			Weight = 0.0;
			Name = "Door to the lower chambers";
		}

		public UnderWorldTeleporter( Serial serial ) : base( serial )
		{
		}

		public override bool OnMoveOver( Mobile m )
		{

                        //Possible Fix 6-12-10
                        PlayerMobile pm = m as PlayerMobile;


			if( !m_AllowCreatures && !m.Player )
				return true;

			if( pm.Backpack.ConsumeTotal( typeof( SamuraiHelmOfFire ), 1 ) )
			if( pm.Backpack.ConsumeTotal( typeof( ClearVision ), 1 ) )
			if( pm.Backpack.ConsumeTotal( typeof( PChaos ), 1 ) )
			if( pm.Backpack.ConsumeTotal( typeof( PowerCrystalTwo ), 1 ) )
			{
				if( m_TelePets )
				{
					Server.Mobiles.BaseCreature.TeleportPets( m, m_DestLoc, m_DestMap );
				}

				pm.Say("Have mercy on my soul");
				World.Broadcast( 32, true, "A cold wind is felt throughout the land as {0} enters the realm of the dead kings.", m.Name );
				pm.PlaySound(0x1F7);

				pm.MoveToWorld( m_DestLoc, m_DestMap );
				

				return false;
			}

			pm.SendMessage( " You must have all of the required toll to enter the underworld." );
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