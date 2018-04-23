using System;
using Server;
using Server.Items;
using Server.Mobiles;

/*
** Allows staff to quickly switch between player and their assigned staff levels by equipping or removing the cloak
** Also allows instant teleportation to a specified destination when double-clicked by the staff member.
** Author unknown.
*/

namespace Arya.Abay
{
	public class StaffCloak : Cloak
	{
		private AccessLevel m_StaffLevel;
		private Point3D m_HomeLoc;
		private Map m_HomeMap;

		[CommandProperty( AccessLevel.Administrator )]
		public AccessLevel StaffLevel
		{
			get
			{
				return m_StaffLevel;
			}
			set
			{
				m_StaffLevel = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D HomeLoc{ get{ return m_HomeLoc;}set{ m_HomeLoc = value;}}

		[CommandProperty( AccessLevel.GameMaster )]
		public Map HomeMap{ get{ return m_HomeMap;}set{ m_HomeMap = value;}}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			// delete this if someone without the necessary access level picks it up or tries to equip it
			if ( RootParent is PlayerMobile && ( (PlayerMobile)RootParent ).AccessLevel == AccessLevel.Player )
			{
				((PlayerMobile)RootParent).Emote( "*Picks up Staff Cloak and watches it go poof*" );
				Delete();
				return;
			}

			Mobile mobile = parent as Mobile;

			// when equipped, change access level to player
			if ( null != mobile )
			{
				StaffLevel = mobile.AccessLevel;
				mobile.AccessLevel = AccessLevel.Player;
				mobile.Blessed = false;
			}
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			Mobile mobile = parent as Mobile;

			// restore access level to the specified level
			if ( null != mobile && !Deleted )
			{
				mobile.AccessLevel = StaffLevel;
				StaffLevel = AccessLevel.Player;
				mobile.Blessed = true;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( HomeMap != Map.Internal && HomeMap != null && from.AccessLevel > AccessLevel.Player )
			{
				from.MoveToWorld( HomeLoc, HomeMap );
			}
		}

		[Constructable]
		public StaffCloak() : base()
		{
			StaffLevel = AccessLevel.Player;
			LootType = LootType.Blessed;
			Name = "Staff Cloak";
			Weight = 0;
		}

		public StaffCloak( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			// version
			writer.Write( (int) 0 );
			// version 0
			writer.Write( (int) m_StaffLevel );
			writer.Write( m_HomeLoc );
			string mapname = null;
			if(m_HomeMap != null)
			{
				mapname = m_HomeMap.Name;
			}
			writer.Write( mapname );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch(version)
			{
				case 0:
					m_StaffLevel = (AccessLevel)reader.ReadInt();
					m_HomeLoc = reader.ReadPoint3D();
					string mapName = reader.ReadString();

					try
					{
						m_HomeMap = Map.Parse( mapName );
					}
					catch{}

					break;
			}

		}
	}
}
