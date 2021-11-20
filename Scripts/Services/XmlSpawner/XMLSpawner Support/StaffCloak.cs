using System;
using Server.Mobiles;

/*
** Allows staff to quickly switch between player and their assigned staff levels by equipping or removing the cloak
** Also allows instant teleportation to a specified destination when double-clicked by the staff member.
*/

namespace Server.Items
{
    public class StaffCloak : Cloak
    {
            
        private AccessLevel m_StaffLevel;
        private Point3D m_HomeLoc;
        private Map m_HomeMap;

        [CommandProperty( AccessLevel.Administrator )]
        public AccessLevel StaffLevel { 
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
        public Point3D HomeLoc { get { return m_HomeLoc;} set { m_HomeLoc = value;} }

        [CommandProperty( AccessLevel.GameMaster )]
        public Map HomeMap { get { return m_HomeMap;} set { m_HomeMap = value;} }

        public override void GetProperties( ObjectPropertyList list )
		{

			base.GetProperties(list);

            list.Add( 1060658, "Level\t{0}", StaffLevel ); // ~1_val~: ~2_val~

		}

#if(NEWPARENT)
		public override void OnAdded(IEntity parent)
#else
		public override void OnAdded(object parent)
#endif
        {
            base.OnAdded( parent );

            // delete this if someone without the necessary access level picks it up or tries to equip it
            if(RootParent is Mobile)
            {
                if (((Mobile) RootParent).AccessLevel != StaffLevel)
                {
                    Delete();
                    return;
                }
            }

            // when equipped, change access level to player
            if ( parent is Mobile )
            {
                Mobile m =(Mobile) parent;

                if (m.AccessLevel == StaffLevel)
                {
                    m.AccessLevel = AccessLevel.Player;
                    // and make vuln
                    m.Blessed = false;
                }
            }
        }

#if(NEWPARENT)
        public override void OnRemoved( IEntity parent )
#else
        public override void OnRemoved( object parent )
#endif
        {
            base.OnRemoved( parent );

            // restore access level to the specified level
            if ( parent is Mobile && !Deleted)
            {
                Mobile m = (Mobile) parent;
                
                // restore their assigned staff level
                m.AccessLevel = StaffLevel;
                // and make invuln
                m.Blessed = true;
            }
        }
        
        public override void OnDoubleClick( Mobile from )
        {
            if(from == null) return;

            if(HomeMap != Map.Internal && HomeMap != null && from.AccessLevel == StaffLevel)
            {
                // teleport them to the specific location
                from.MoveToWorld(HomeLoc, HomeMap);
            }
        }

        [Constructable]
        public StaffCloak() : base()
        {

            StaffLevel= AccessLevel.Administrator;  // assign admin staff level by default
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

        public override void Deserialize(GenericReader reader) 
        { 
                base.Deserialize( reader );

                int version = reader.ReadInt();
                
                switch(version)
                {
                case 0:
                    m_StaffLevel = (AccessLevel)reader.ReadInt();
                	m_HomeLoc = reader.ReadPoint3D();
                	string mapname = reader.ReadString();

                	try{
                	m_HomeMap = Map.Parse(mapname);
                	} catch{}

                	break;
                }
        
        }
    }
}
