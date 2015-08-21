using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Regions;
using System.Collections;

namespace Server.Engines.XmlSpawner2
{

	public class ChallengeRegionStone : Item
	{

        public Map m_ChallengeMap;
        private string m_ChallengeRegionName;
        private ChallengeGameRegion m_ChallengeRegion;                    // challenge region
        private MusicName m_Music;
        private int m_Priority;
        private Rectangle3D [] m_ChallengeArea;
        private string m_CopiedRegion;
        private bool m_Disable;

        [CommandProperty( AccessLevel.GameMaster )]
        public Rectangle3D [] ChallengeArea
        {
            get { return m_ChallengeArea; }
            set { m_ChallengeArea = value;}
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool DisableGuards
        {
            get { return m_Disable; }
            set 
            { 
                m_Disable = value;
                if(m_ChallengeRegion != null)
                {
					m_ChallengeRegion.Unregister();
                    m_ChallengeRegion.Disabled = m_Disable;
					m_ChallengeRegion.Register();
                }
            }
        }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public string ChallengeRegionName
        {
            get { return m_ChallengeRegionName; }
            set
            { 
                m_ChallengeRegionName = value; 

                RefreshRegions();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public ChallengeGameRegion ChallengeRegion
        {
            get { return m_ChallengeRegion; }
        }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public Map ChallengeMap
        {
            get { return m_ChallengeMap; }
            set { 
                m_ChallengeMap = value; 

                RefreshRegions();
            }
        }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public MusicName ChallengeMusic
        {
            get { return m_Music; }
            set { 
                m_Music = value;
                if(m_ChallengeRegion != null)
                {
					m_ChallengeRegion.Unregister();
                    m_ChallengeRegion.Music = m_Music;
					m_ChallengeRegion.Register();
                }
            }
        }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public int ChallengePriority
        {
            get { return m_Priority; }
            set {
                m_Priority = value;
                
                RefreshRegions();
            }
        }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public string CopyRegion
        {
            get {
                return m_CopiedRegion;
            }
            set {
                if(value == null)
                {
                    m_CopiedRegion = null;
                } else
                {

                    // find the named region
					Region r = this.Map.Regions[value];
                    
                    if(r != null)
                    {
						m_CopiedRegion = value;

						// copy the coords, map, and music from that region
						m_ChallengeMap = r.Map;
						m_Music = r.Music;
						m_Priority = r.Priority;
						m_ChallengeArea = r.Area;

						RefreshRegions();

    
                    }
                }
            }
        }

        public void RefreshRegions()
        {

			if (m_ChallengeRegion != null)
			{
				m_ChallengeRegion.Unregister();
			}

			if (m_ChallengeMap == null || m_ChallengeArea == null) return;

			// define a new one based on the named region
			m_ChallengeRegion = new ChallengeGameRegion(m_ChallengeRegionName, m_ChallengeMap, m_Priority, m_ChallengeArea);
			// update the new region properties with current values
			m_ChallengeRegion.Music = m_Music;
			m_ChallengeRegion.Disabled = m_Disable;

			m_ChallengeRegion.Register();
        }

		[Constructable]
		public ChallengeRegionStone() : base ( 0xED4 )
		{
			Visible = false;
			Movable = false;
			Name = "Challenge Region Stone";

			ChallengeRegionName = "Challenge Game Region";

			ChallengePriority = 0x90;             // high priority

			m_ChallengeMap = this.Map;
			
		}

		public ChallengeRegionStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
			// version 2
			// updated for RunUO 2.0 region changes in 3D region coordinates
            // version 1
            writer.Write( m_Disable );
            // version 0
			writer.Write( (int)m_Music );
			writer.Write( m_Priority );
			// removed in version 2
			//writer.Write(m_ChallengeArea);

			writer.Write( m_ChallengeRegionName );
			if (m_ChallengeMap != null)
			{
				writer.Write(m_ChallengeMap.Name);
			}
			else
			{
				writer.Write(String.Empty);
			}
			writer.Write( m_CopiedRegion );

			// do the coord list
			if (m_ChallengeRegion != null && m_ChallengeRegion.Area != null && m_ChallengeRegion.Area.Length > 0)
			{
				writer.Write(m_ChallengeRegion.Area.Length);

				for (int i = 0; i < m_ChallengeRegion.Area.Length; i++)
				{
					writer.Write(m_ChallengeRegion.Area[i].Start);
					writer.Write(m_ChallengeRegion.Area[i].End);
				}
			}
			else
			{
				writer.Write((int)0);
			}
		}
		
		public override void GetProperties(ObjectPropertyList list)
		{
            base.GetProperties(list);
            
            list.Add( 1062613, m_ChallengeRegionName);
		}



		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_ChallengeArea = new Rectangle3D[0];

			switch ( version )
			{
				case 2:
			    case 1:
				{
				    m_Disable = reader.ReadBool();
				    goto case 0;
				}
				case 0:
				{
                    m_Music = (MusicName)reader.ReadInt();
                    m_Priority = reader.ReadInt();
					if (version < 2)
					{
						// old region area
						reader.ReadRect2D();
					}
                    m_ChallengeRegionName = reader.ReadString();
                    string mapname = reader.ReadString();
                    try{
					   m_ChallengeMap = Map.Parse(mapname);
					} catch {}
					m_CopiedRegion = reader.ReadString();

					// do the coord list
						int count = reader.ReadInt();
						if (count > 0)
						{
							// the old version used 2D rectangles for the region area.  The new version uses 3D
							if (version < 2)
							{
								Rectangle2D[] area = new Rectangle2D[count];
								for (int i = 0; i < count; i++)
								{
									area[i] = reader.ReadRect2D();
								}
								m_ChallengeArea = Region.ConvertTo3D(area);
							}
							else
							{
								m_ChallengeArea = new Rectangle3D[count];
								for (int i = 0; i < count; i++)
								{
									m_ChallengeArea[i] = new Rectangle3D(reader.ReadPoint3D(), reader.ReadPoint3D());
								}
							}
						}
					break;
				}
			}

			// refresh the region
			Timer.DelayCall(TimeSpan.Zero, new TimerCallback(RefreshRegions));
		}

		public static Region FindRegion(string name)
		{
            if (Region.Regions == null)	return null;

        	foreach (Region region in Region.Regions)
        	{
        		if (string.Compare(region.Name, name, true) == 0)
        		{
        			return region;
        		}
        	}

        	return null;
        }

        public override void OnDoubleClick( Mobile m )
		{
			if( m != null && m.AccessLevel >= AccessLevel.GameMaster)
			{
                m.SendMessage("Define the Challenge area");
                DefineChallengeArea ( m );
			}
		}


		public void DefineChallengeArea( Mobile m )
		{
			BoundingBoxPicker.Begin( m, new BoundingBoxCallback( ChallengeRegionArea_Callback ), this );
		}

		private static void ChallengeRegionArea_Callback( Mobile from, Map map, Point3D start, Point3D end, object state )
		{
            // assign these coords to the region
            ChallengeRegionStone s = state as ChallengeRegionStone;

			if (s != null && from != null)
			{
				s.m_ChallengeArea = new Rectangle3D[1];
				s.m_ChallengeArea[0] = Region.ConvertTo3D(new Rectangle2D(start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1));
				s.m_ChallengeMap = map;

				s.CopyRegion = null;

				s.RefreshRegions();

			}
		}

		public override void OnDelete()
		{
			if( m_ChallengeRegion != null )
				m_ChallengeRegion.Unregister();

			base.OnDelete();
		}
	}
}
