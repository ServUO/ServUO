#region Using directives

using System;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using Server;
using Server.Gumps;
using Server.Commands;
using Server.Targeting;

#endregion

namespace Carding.Mobiles
{
	[ Serializable, XmlInclude( typeof( MustangInfo ) ) ]
	/// <summary>
	/// Collection of mustang information for all the mustangs on the shard
	/// </summary>
	public class MustangCollection
	{
		#region Mustang Gump

		private class MustangGump : Gump
		{
			private int m_Page;
			private const int LabelHue = 0x480;
			private const int GreenHue = 0x40;

			public MustangGump( Mobile m, int page ) : base( 100, 100 )
			{
				m.CloseGump( typeof( MustangGump ) );
				m_Page = page;
				MakeGump();
			}

			private void MakeGump()
			{
				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;
				this.AddPage(0);
				this.AddBackground(0, 0, 160, 240, 9400);
				this.AddButton(15, 10, 5601, 5605, 11, GumpButtonType.Reply, 0);
				this.AddLabel(35, 8, GreenHue, @"Load");

				for ( int i = 0; i < 10 && ( 10 * m_Page ) + i < Mustangs.Count; i++ )
				{
					AddButton( 15, 30 + i * 20, 5601, 5605, i + 1, GumpButtonType.Reply, 0 );
					MustangInfo info = Mustangs[ 10 * m_Page + i ] as MustangInfo;
					AddLabel( 35, 33 + i * 20, LabelHue, info.ID );
				}

				if ( m_Page > 0 )
                    this.AddButton(115, 10, 5603, 5607, 12, GumpButtonType.Reply, 0);

				int totPages = Mustangs.Count / 10;

				if ( Mustangs.Count % 10 == 0 && Mustangs.Count != 0 )
					totPages--;

				if ( m_Page < totPages )
					this.AddButton(135, 10, 5601, 5605, 13, GumpButtonType.Reply, 0);
			}

			public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
			{
				switch ( info.ButtonID )
				{
					case 11:

						if ( sender.Mobile.AccessLevel >= AccessLevel.Administrator )
						{
							Load();
							sender.Mobile.SendGump( new MustangGump( sender.Mobile, 0 ) );
						}
						else
						{
							sender.Mobile.SendGump( new MustangGump( sender.Mobile, m_Page ) );
							sender.Mobile.SendMessage( "Only administrators can do that" );
						}
						break;

					case 12: // Previous page

						sender.Mobile.SendGump( new MustangGump( sender.Mobile, --m_Page ) );
						break;
						
					case 13: // Next page

						sender.Mobile.SendGump( new MustangGump( sender.Mobile, ++m_Page ) );
						break;

					default:

						int index = info.ButtonID - 1;

						if ( index >= 0 && index < 10 )
						{
							int i = m_Page * 10 + index;
							
							if ( i >= 0 && i < Mustangs.Count )
							{
								MustangInfo m = Mustangs[ i ] as MustangInfo;
                                sender.Mobile.Target = new MustangTarget(new object[] { m_Page, m });
								sender.Mobile.SendMessage( "Where do you wish to create the mustang?" );
							}
						}
						break;						  
				}
			}

            public class MustangTarget : Target
            {
                private object[] m_state;

                public MustangTarget(object[] state)
                    : base(-1, true, TargetFlags.None)
                {
                    m_state = state;
                }

                protected override void OnTarget(Mobile from, object targeted)
                {
                    IPoint3D p = targeted as IPoint3D;
                    int page = (int)(m_state)[0];
                    MustangInfo info = (MustangInfo)(m_state)[1];

                    if (p == null)
                    {
                        from.SendMessage("Cannot create there");
                        from.SendGump(new MustangGump(from, page));
                        return;
                    }

                    Mustang m = new Mustang(info);
                    m.MoveToWorld(new Point3D(p), from.Map);
                    from.SendGump(new MustangGump(from, page));
                }
            }
		}

		#endregion

		private static MustangCollection m_Mustangs;
		private static ArrayList Mustangs { get { return m_Mustangs.List; } }

		/// <summary>
		/// Creates a random mustang
		/// </summary>
		public static MustangInfo Randomize()
		{
			return Mustangs[ Utility.Random( Mustangs.Count ) ] as MustangInfo;
		}

		/// <summary>
		/// Creates a mustang from its staff ID
		/// </summary>
		/// <param name="name">This can be either the staff id, or a number of ids concatenated by dots:
		/// Copper.Dull.Sky</param>
		public static MustangInfo FromString( string name )
		{
			string[] names = name.Split( '.' );
			ArrayList list = new ArrayList();

			foreach ( string s in names )
			{
				MustangInfo info = Parse( s );

				if ( info != null )
					list.Add( info );
			}

			if ( list.Count == 0 )
				return MustangInfo.Default;
			else
				return list[ Utility.Random( list.Count ) ] as MustangInfo;
		}

		/// <summary>
		/// Finds the MustangInfo corresponding to a staff ID
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static MustangInfo Parse( string name )
		{
			foreach( MustangInfo info in Mustangs )
			{
				if ( info.ID.ToLower() == name.ToLower() )
					return info;
			}

			return null;
		}

		/// <summary>
		/// Creates a mustang whose tame skill is between the range specified
		/// </summary>
		/// <param name="minSkill"></param>
		/// <param name="maxSkill"></param>
		/// <param name="mustang"></param>
		public static MustangInfo FromSkills( int minSkill, int maxSkill )
		{
			if ( minSkill > maxSkill )
			{
				int temp = minSkill;
				minSkill = maxSkill;
				maxSkill = temp;
			}

			ArrayList list = new ArrayList();

			foreach ( MustangInfo info in Mustangs )
			{
				if ( minSkill <= info.Skill && maxSkill >= info.Skill )
					list.Add( info );
			}

			MustangInfo selected = null;

			if ( list.Count > 0 )
			{
				selected = list[ Utility.Random( list.Count ) ] as MustangInfo;
			}

			if ( selected == null )
				selected = MustangInfo.Default;

			return selected;
		}

		public static void Initialize()
		{
			Load();
			Server.Commands.CommandHandlers.Register( "Mustang", AccessLevel.GameMaster, new CommandEventHandler( OnMustang ) );
		}

        [Usage("Mustang"), Description("Brings up a gump that allows to add the defined mustangs")]
        private static void OnMustang(CommandEventArgs e)
        {
            if (m_Mustangs != null)
                e.Mobile.SendGump(new MustangGump(e.Mobile, 0));
            else
                e.Mobile.SendAsciiMessage("Error, the mustang list is not loaded");
        }

	    private static void Load()
		{
			string path = Path.Combine( Core.BaseDirectory, @"Data\mustangs.xml" );

			if ( ! File.Exists( path ) )
				return;

			FileStream stream = null;

			try
			{
				stream = new FileStream( path, FileMode.Open, FileAccess.Read );
				XmlSerializer serializer = new XmlSerializer( typeof( MustangCollection ) );
				m_Mustangs = serializer.Deserialize( stream ) as MustangCollection;
			}
			catch{}
			finally
			{
				if ( stream != null )
					stream.Close();
			}

			if ( m_Mustangs == null )
				m_Mustangs = new MustangCollection();

			m_Mustangs.List.Insert( 0, MustangInfo.Default );
		}

		public MustangCollection()
		{
			m_List = new ArrayList();
		}

		private ArrayList m_List;

		/// <summary>
		/// The full listing of mustangs (MustangInfo) available on the shard
		/// </summary>
		/// <value></value>
		public ArrayList List
		{
			get { return m_List; }
			set { m_List = value; }
		}

	}

	/// <summary>
	/// Defines the features of a mustang type
	/// </summary>
	public class MustangInfo
	{
		private static MustangInfo m_Default;

		public static MustangInfo Default
		{
			get
			{
				if ( m_Default == null )
				{
					m_Default = new MustangInfo();
					m_Default.ID = "Mustang";
					m_Default.Name = "Mustang";
					m_Default.Hue = 2101;
					m_Default.Skill = 55.0;
					m_Default.BodyVal = 0xCC;
				}

				return m_Default;
			}
		}

		public MustangInfo()
		{
		}

		public void ApplyTo( Mustang mustang )
		{
			try
			{
				mustang.Body = (Body) GetBody();
				mustang.Hue = m_Hue;
				mustang.MinTameSkill = m_Skill;
				mustang.Name = m_Name;
				mustang.ItemID = GetItemID( (int) mustang.Body );
			}
			catch
			{
				//World.Broadcast( 0x20, false, ID );
				//throw new Exception();
			}
		}

		public int GetBody()
		{
			if ( m_BodyVal == 200 | m_BodyVal == 204 | m_BodyVal == 226 | m_BodyVal == 228 )
				return m_BodyVal;
            else
			    return 200;
		}

		private static int GetItemID( int body )
		{
			if ( body == 200 )
				return 0x3E9F;
			else if ( body == 226 )
				return 0x3EA0;
			else if ( body == 228 )
				return 0x3EA1;
			else
				return 0x3EA2;
		}

		public override string ToString()
		{
			return m_ID;
		}

		private string m_ID;
		private string m_Name;
		private int m_Hue;
		private double m_Skill;
        private int m_BodyVal;

		/// <summary>
		/// The body value in hex
		/// </summary>
		/// <value></value>
        [XmlAttribute]
        public int BodyVal
        {
            get { return m_BodyVal; }
            set { m_BodyVal = value; }
        }

		/// <summary>
		/// The minimum skill required to tame this mustang
		/// </summary>
		/// <value></value>
		[ XmlAttribute ]
		public double Skill
		{
			get { return m_Skill; }
			set { m_Skill = value; }
		}

		/// <summary>
		/// The hue of this mustang
		/// </summary>
		/// <value></value>
		[ XmlAttribute ]
		public int Hue
		{
			get { return m_Hue; }
			set { m_Hue = value; }
		}

		/// <summary>
		/// The name of this mustang (as seen in game)
		/// </summary>
		/// <value></value>
		[ XmlAttribute ]
		public string Name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}

		/// <summary>
		/// The ID of this mustang, used by staff to spawn it
		/// </summary>
		/// <value></value>
		[ XmlAttribute ]
		public string ID
		{
			get { return m_ID; }
			set { m_ID = value; }
		}
	}
}
