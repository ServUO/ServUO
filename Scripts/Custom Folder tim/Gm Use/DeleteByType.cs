using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Targeting;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Commands
{
	public enum DeleteByTypeGumpPage
	{
		start,
		targeting,
		textentry
	}

	public class DeleteByTypeGump : Gump
	{
		private static Mobile m_staff;
		private static ArrayList m_todelete;
		private static int m_count;
		private static Item m_item;
		private static string m_typename;
		private static Type typeofItem = typeof( Item );
		private DeleteByTypeGumpPage m_Page;
		private const int White = 0xFFFFFF;

		public static void Initialize()
		{
			CommandSystem.Register( "DeleteByType", AccessLevel.Administrator, new CommandEventHandler( DeleteByType_OnCommand ) );
		}

		[Usage( "DeleteByType" )]
		[Description( "Opens an interface which allows staff members to do global deletes by Type without the worry of typeing in the wrong information and wiping the server." )]
		public static void DeleteByType_OnCommand( CommandEventArgs e )
		{
			e.Mobile.CloseGump( typeof( DeleteByTypeGump ) );
			e.Mobile.SendGump( new DeleteByTypeGump( m_staff, m_todelete, m_count, m_item, m_typename, DeleteByTypeGumpPage.start, m_count ) );
		}

		public void AddBlackAlpha( int x, int y, int width, int height )
		{
			AddImageTiled( x, y, width, height, 2624 );
			AddAlphaRegion( x, y, width, height );
		}

		public void AddPageButton( int x, int y, int buttonID, string text, DeleteByTypeGumpPage page)
		{
			AddButton( x, y - 1, 4005, 4006, buttonID, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y, 200, 20, Color( text, White ), false, false );
		}

		public void AddButtonLabeled( int x, int y, int buttonID, string text )
		{
			AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y, 240, 20, Color( text, White ), false, false );
		}

		public int GetButtonID( int type, int index )
		{
			return 1 + (index * 15) + type;
		}

		public string Color( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
		}

		public string Center( string text )
		{
			return String.Format( "<CENTER>{0}</CENTER>", text );
		}

		public DeleteByTypeGump(Mobile staff, ArrayList todelete, int count, Item item, string typename, DeleteByTypeGumpPage page, int precount) : base( 0, 0 )
		{
			m_staff = staff;
			m_todelete = todelete;
			m_count = count;
			m_item = item;
			m_typename = typename;

			string precountstring = "You will delete [" + precount + "] Items.";

			m_Page = page;

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			this.AddPage(0);
			this.AddBlackAlpha(150, 130, 270, 143 );
			this.AddBackground(150, 130, 270, 143, 9200);
			this.AddLabel(216, 135, 0, @"Global Delete By Type");
			this.AddLabel(370, 135, 0, @"v1.0");

			switch ( page )
			{
				case DeleteByTypeGumpPage.start:
				{
					this.AddButtonLabeled( 170, 170, GetButtonID( 1, 1 ), "Target Item of Type to Delete" );
					this.AddLabel(274, 192, 0, @"or");
					this.AddLabel(174, 213, 0, @"Enter TypeName of Item to Delete");
					this.AddAlphaRegion(173, 237, 180, 20 );
					this.AddTextEntry(173, 237, 180, 20, 0, 0, @"");
					this.AddButtonLabeled( 360, 237, GetButtonID( 1, 2 ), "" );
					break;
				}
				case DeleteByTypeGumpPage.targeting:
				{
					this.AddLabel(170, 160, 0, @"Delete all Items of the Targeted Type?");
					this.AddHtml(150, 185, 270, 20, Color(Center(precountstring), White), false, false );
					this.AddButtonLabeled( 225, 210, GetButtonID( 1, 3 ), "Yes, Delete!" );
					this.AddPageButton( 225, 235, GetButtonID( 0, 0 ), "No, Cancel", DeleteByTypeGumpPage.start);
					break;
				}
				case DeleteByTypeGumpPage.textentry:
				{
					this.AddLabel(195, 160, 0, @"Delete all Items of this Type?");
					this.AddHtml(150, 185, 270, 20, Color(Center(precountstring), White), false, false );
					this.AddButtonLabeled( 225, 210, GetButtonID( 1, 4 ), "Yes, Delete!" );
					this.AddPageButton( 225, 235, GetButtonID( 0, 0 ), "No, Cancel", DeleteByTypeGumpPage.start);
					break;
				}
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			if (from == null)
				return;

			int val = info.ButtonID - 1;

			if ( val < 0 )
				return;

			int version = val % 15;
			int index = val / 15;

			switch ( version )
			{
				case 0:
				{
					DeleteByTypeGumpPage page;

					switch ( index )
					{
						case 0: page = DeleteByTypeGumpPage.start; break;
						case 1: page = DeleteByTypeGumpPage.targeting; break;
						case 2: page = DeleteByTypeGumpPage.textentry; break;
						default: return;
					}
					from.SendGump( new DeleteByTypeGump(m_staff, m_todelete, m_count, m_item, m_typename, page, m_count) );
					break;
				}

				case 1:
				{
					switch ( index )
					{
						case 0: // close the gump
						{
							from.SendMessage("DeleteByType Gump Closing");
							return;
						}
						case 1: // Targeting
						{
							from.Target = new DeleteByTypeTarget();
							break;
						}
						case 2: // TextEntry
						{
							Mobile staff = from;

							TextRelay entry = info.GetTextEntry( 0 );
							string typename = ( entry == null ? "" : entry.Text.Trim() );

							int count = 0;
							int precount = 0;
							ArrayList ToDelete = new ArrayList();
							Type type = SpawnerType.GetType( typename );

							if(type == null || typename.Length == 0)
							{
								staff.SendMessage( 0x35, "You must enter a Valid TypeName." );
								staff.SendGump( new DeleteByTypeGump(m_staff, m_todelete, m_count, m_item, m_typename, DeleteByTypeGumpPage.start, precount));
								return;
							}

							if ( type == typeofItem || type.IsSubclassOf( typeofItem ) )
							{
								foreach( Item item in World.Items.Values )
								{
									if(item.GetType().Name == type.Name)
									{
										if(item != null && !item.Deleted)
										{
											ToDelete.Add( item );
											precount++;
										}
									}
								}
								staff.SendGump( new DeleteByTypeGump(staff, ToDelete, count, null, typename, DeleteByTypeGumpPage.textentry, precount) );
							}
							break;
						}
						case 3:// Last Chance Targeting Delete
						{
							DeleteByTypeTarget.mToDelete(m_staff, m_todelete, m_count, m_item, null);
							break;
						}
						case 4:// Last Chance TextEntry Delete
						{
							DeleteByTypeTarget.mToDelete(m_staff, m_todelete, m_count, null, m_typename);
							break;
						}
					}
					break;
				}
			}
		}

		public class DeleteByTypeTarget : Target
		{
			private DeleteByTypeGumpPage m_Page;

			public DeleteByTypeTarget() : base( -1, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object o )
			{
				Mobile staff = from;
				if ( o is Item )
				{
	            	Item item = (Item)o;
	            	if(item != null)
	            	{
		            	int count = 0;
		            	int precount = 0;
						ArrayList ToDelete = new ArrayList();
						foreach( Item i in World.Items.Values )
						{
							if( i.GetType() == item.GetType() )
							{
								if(i != null && !i.Deleted)
								{
									ToDelete.Add( i );
									precount++;
								}
							}
						}
						staff.SendGump( new DeleteByTypeGump(staff, ToDelete, count, item, null, DeleteByTypeGumpPage.targeting, precount) );
	            	}
				}
			}

			public static void mToDelete(Mobile staff, ArrayList todelete, int count, Item item, string typename)
			{
				foreach( Item i in todelete)
				{
					if(i != null && !i.Deleted)
					{
						i.Delete();
						count++;
					}
				}

				if(item != null)
					staff.SendMessage("Deleted {0} items of type {1}",count,item);
				if(typename != null)
					staff.SendMessage("Deleted {0} items of type {1}",count,typename);

				staff.SendGump( new DeleteByTypeGump(m_staff, m_todelete, m_count, m_item, m_typename, DeleteByTypeGumpPage.start, m_count) );
			}
		}
	}
}