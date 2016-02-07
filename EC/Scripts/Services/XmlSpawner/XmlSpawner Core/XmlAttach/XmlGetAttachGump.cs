using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.Gumps;
using Server.Targeting;
using System.Reflection;
using Server.Commands;
using Server.Commands.Generic;
using CPA = Server.CommandPropertyAttribute;
using System.Xml;
using Server.Spells;
using System.Text;
using Server.Accounting;
using Server.Engines.XmlSpawner2;

/*
** XmlGetAtt
** Version 1.00
** updated 10/24/04
** ArteGordon
**
*/

namespace Server.Mobiles
{
	public class XmlGetAttGump : Gump
	{
		private const int MaxEntries = 18;
		private const int MaxEntriesPerPage = 18;
		
		private object m_TargetObject;

		private bool Dosearchtype;
		private bool Dosearchname;

		private bool Dosearchage;

		private bool Searchagedirection;
		private double Searchage;

		private string Searchtype;
		private string Searchname;

		private bool Sorttype;

		private bool Sortname;

		private Mobile m_From;

		private bool Descendingsort;
		private int Selected;
		private int DisplayFrom;
		private bool [] m_SelectionList;

		private bool SelectAll = false;

		private List<XmlAttachment> m_SearchList;

		 public static void Initialize()
		{
			CommandSystem.Register( "XmlGetAtt", AccessLevel.GameMaster, new CommandEventHandler( XmlGetAtt_OnCommand ) );
		}


		private bool TestAge(object o)
		{
			if(Searchage <= 0) return true;

			if(o is XmlAttachment){
				XmlAttachment a = (XmlAttachment)o;

				if(Searchagedirection)
				{
					// true means allow only mobs greater than the age
					if((DateTime.UtcNow - a.CreationTime) > TimeSpan.FromHours(Searchage)) return true;
				}
				else
				{
					// false means allow only mobs less than the age
					if((DateTime.UtcNow - a.CreationTime) < TimeSpan.FromHours(Searchage)) return true;
				}

			}
			return false;
		}


		private List<XmlAttachment> Search(object target, out string status_str)
		{
			status_str = null;
			List<XmlAttachment> newarray = new List<XmlAttachment>();
			Type  targetType = null;
			// if the type is specified then get the search type
			if(Dosearchtype && Searchtype != null){
				targetType = SpawnerType.GetType( Searchtype );
				if(targetType == null){
					status_str = "Invalid type: " + Searchtype;
					return newarray;
				}
			}

			List<XmlAttachment> attachments = XmlAttach.FindAttachments(target);

			// do the search through attachments
			if(attachments != null)
			foreach(XmlAttachment i in attachments)
			{
				bool hastype = false;
				bool hasname = false;

				if(i == null || i.Deleted ) continue;


				// check for type
				if(Dosearchtype && (i.GetType().IsSubclassOf(targetType) || i.GetType().Equals(targetType)))
				{
					hastype = true;
				}
				if(Dosearchtype && !hastype) continue;

				// check for name
				if (Dosearchname && (i.Name != null) && (Searchname != null) && (i.Name.ToLower().IndexOf(Searchname.ToLower()) >= 0))
				{
					hasname = true;
				}
				if(Dosearchname && !hasname) continue;


				// satisfied all conditions so add it
				newarray.Add(i);
			}

			return newarray;
		}
		
		private class GetAttachTarget : Target
		{
			private CommandEventArgs m_e;

			public GetAttachTarget( CommandEventArgs e) :  base ( 30, false, TargetFlags.None )
			{
			  m_e = e;

			}
			protected override void OnTarget( Mobile from, object targeted )
			{
				if(from == null || targeted == null) return;


				from.SendGump( new XmlGetAttGump(from, targeted, 0,0));
			}
		}

		[Usage( "XmlGetAtt" )]
		[Description( "Gets attachments on an object" )]
		public static void XmlGetAtt_OnCommand( CommandEventArgs e )
		{
			e.Mobile.Target = new GetAttachTarget(e);
		}

		public XmlGetAttGump(Mobile from, object targeted, int x, int y) : this(from, targeted, true, false,
		false, false, false,
		null,null, false, 0,
		null, -1, 0,
		false,false,
		false, null, x, y)
		{

		}

		public XmlGetAttGump( Mobile from, object targeted, bool firststart, bool descend,
			bool dosearchtype, bool dosearchname, bool dosearchage,
			string searchtype, string searchname, bool searchagedirection, double searchage,
			List<XmlAttachment> searchlist, int selected, int displayfrom,
			bool sorttype, bool sortname,
			bool selectall, bool [] selectionlist, int X, int Y ) : base( X,Y )
		{

			m_TargetObject = targeted;
			m_From = from;
			m_SelectionList = selectionlist;
			if(m_SelectionList == null){
				m_SelectionList = new bool[MaxEntries];
			}
			SelectAll = selectall;
			Sorttype = sorttype;
			Sortname = sortname;

			DisplayFrom = displayfrom;
			Selected = selected;

			Descendingsort = descend;
			Dosearchtype = dosearchtype;
			Dosearchname = dosearchname;
			Dosearchage = dosearchage;

			Searchagedirection = searchagedirection;

			Searchage = searchage;
			Searchtype = searchtype;
			Searchname = searchname;

			m_SearchList = searchlist;

			if(firststart)
			{
				string status_str;
				m_SearchList = Search(m_TargetObject,out status_str);
	  		}

			// prepare the page

			AddPage( 0 );

			AddBackground( 0, 0, 640, 474, 5054 );
			AddAlphaRegion( 0, 0, 640, 474 );

			string tnamestr = null;
			if(targeted is Item)
			{
				tnamestr = ((Item)targeted).Name;
			} else
			if(targeted is Mobile)
			{
				tnamestr = ((Mobile)targeted).Name;
			}
			AddLabel( 2, 0, 0x33, String.Format( "Attachments on {0} : {1}", targeted.GetType().Name, tnamestr ) );

			// add the Sort button
			AddButton( 5, 450, 0xFAB, 0xFAD, 700, GumpButtonType.Reply, 0 );
			AddLabel( 38, 450, 0x384, "Sort" );

			 // add the sort direction button
			if(Descendingsort){
					AddButton( 75, 453, 0x15E2, 0x15E6, 701, GumpButtonType.Reply, 0 );
					AddLabel( 100, 450, 0x384, "descend" );
			} else {
				   	AddButton( 75, 453, 0x15E0, 0x15E4, 701, GumpButtonType.Reply, 0 );
				   	AddLabel( 100, 450, 0x384, "ascend" );
			}

			// add the Sort on type toggle
			AddRadio( 155, 450, 0xD2,0xD3, Sorttype, 0 );
			AddLabel( 155, 425, 0x384, "type" );

			// add the Sort on name toggle
			AddRadio( 200, 450, 0xD2,0xD3, Sortname, 1 );
			AddLabel( 200, 425, 0x384, "name" );


			AddLabel( 42, 13, 0x384, "Name" );
			AddLabel( 145, 13, 0x384, "Type" );
			AddLabel( 285, 13, 0x384, "Created" );
			AddLabel( 425, 13, 0x384, "Expires In" );
			AddLabel( 505, 13, 0x384, "Attached By" );

			// add the Delete button
			AddButton( 250, 450, 0xFB1, 0xFB3, 156, GumpButtonType.Reply, 0 );
			AddLabel( 283, 450, 0x384, "Delete" );


			// add the page buttons
			for(int i = 0;i<(int)(MaxEntries/MaxEntriesPerPage);i++){
				//AddButton( 38+i*30, 365, 2206, 2206, 0, GumpButtonType.Page, 1+i );
				AddButton( 418+i*25, 450, 0x8B1+i, 0x8B1+i, 0, GumpButtonType.Page, 1+i );
			}

			// add the advance pageblock buttons
			AddButton( 415+25*(int)(MaxEntries/MaxEntriesPerPage), 450, 0x15E1, 0x15E5, 201, GumpButtonType.Reply, 0 ); // block forward
			AddButton( 395, 450, 0x15E3, 0x15E7, 202, GumpButtonType.Reply, 0 ); // block backward

			// add the displayfrom entry
			AddLabel( 460, 450, 0x384, "Display" );
			AddImageTiled( 500, 450, 60, 21, 0xBBC );
			AddTextEntry( 501, 450, 60, 21, 0, 400, DisplayFrom.ToString() );
			AddButton( 560, 450, 0xFAB, 0xFAD, 9998, GumpButtonType.Reply, 0 );

			// display the item list
			if(m_SearchList != null){
				AddLabel( 320, 425, 68, String.Format("Found {0} attachments",m_SearchList.Count) );
				AddLabel( 500, 425, 68, String.Format("Displaying {0}-{1}",DisplayFrom,
					(DisplayFrom + MaxEntries < m_SearchList.Count ? DisplayFrom + MaxEntries : m_SearchList.Count)) );
			}

			// display the select-all-displayed toggle
			AddButton( 620, 5, 0xD2, 0xD3, 3999, GumpButtonType.Reply, 0 );

			// display the select-all toggle
			AddButton( 600, 5, (SelectAll? 0xD3:0xD2), (SelectAll? 0xD2:0xD3), 3998, GumpButtonType.Reply, 0 );

			for ( int i = 0;  i < MaxEntries; i++ )
			{
				int index = i + DisplayFrom;
				if(m_SearchList == null || index >= m_SearchList.Count) break;
				int page = (int)(i/MaxEntriesPerPage);
				if(i%MaxEntriesPerPage == 0){
					AddPage(page+1);
				}

				// background for search results area
				//AddImageTiled( 235, 22 * (i%MaxEntriesPerPage)  + 30, 386, 23, 0x52 );
				//AddImageTiled( 236, 22 * (i%MaxEntriesPerPage) + 31, 384, 21, 0xBBC );

				// add the Props button for each entry
				AddButton( 5, 22 * (i%MaxEntriesPerPage)  + 30, 0xFAB, 0xFAD, 3000+i, GumpButtonType.Reply, 0 );

				string namestr = null;
				string typestr = null;
				string expirestr = null;
				//string description = null;
				string attachedby = null;
				string created = null;

				int texthue = 0;

				object o = (object)m_SearchList[index];

				if(o is XmlAttachment){
					XmlAttachment a = m_SearchList[index];

					namestr = a.Name;
					typestr = a.GetType().Name;
					expirestr = a.Expiration.ToString();
					//description = a.OnIdentify(m_From);
					created = a.CreationTime.ToString();
					attachedby = a.AttachedBy;
				}

				bool sel=false;
				if(m_SelectionList != null && i < m_SelectionList.Length){
					sel = m_SelectionList[i];
				}
				if(sel) texthue = 33;

				if(i == Selected) texthue = 68;

				// display the name
				AddImageTiled( 36, 22 * (i%MaxEntriesPerPage)  + 31, 102, 21, 0xBBC );
				AddLabelCropped( 38, 22 * (i%MaxEntriesPerPage) + 31, 100, 21, texthue, namestr );

				// display the type
				AddImageTiled( 140, 22 * (i%MaxEntriesPerPage)  + 31, 133, 21, 0xBBC );
				AddLabelCropped( 140, 22 * (i%MaxEntriesPerPage) + 31, 133, 21, texthue, typestr );

				// display the creation time
				AddImageTiled( 275, 22 * (i%MaxEntriesPerPage)  + 31, 138, 21, 0xBBC );
				AddLabelCropped( 275, 22 * (i%MaxEntriesPerPage) + 31, 138, 21, texthue, created );

				// display the expiration
				AddImageTiled( 415, 22 * (i%MaxEntriesPerPage)  + 31, 78, 21, 0xBBC );
				AddLabelCropped( 415, 22 * (i%MaxEntriesPerPage) + 31, 78, 21, texthue, expirestr );

				// display the attachedby
				AddImageTiled( 495, 22 * (i%MaxEntriesPerPage)  + 31, 125, 21, 0xBBC );
				AddLabelCropped( 495, 22 * (i%MaxEntriesPerPage) + 31,105, 21, texthue, attachedby );
				
				// display the descriptio button
				AddButton( 600, 22 * (i%MaxEntriesPerPage)  + 32, 0x5689, 0x568A, 5000+i, GumpButtonType.Reply, 0 );

				// display the selection button
				AddButton( 620, 22 * (i%MaxEntriesPerPage)  + 32, (sel? 0xD3:0xD2), (sel? 0xD2:0xD3), 4000+i, GumpButtonType.Reply, 0 );
			}
		}


		private void DoShowProps(int index)
		{
			if(m_From == null || m_From.Deleted) return;

			if(index < m_SearchList.Count)
			{
				XmlAttachment x = m_SearchList[index];
				if(x == null || x.Deleted ) return;

				m_From.SendGump( new PropertiesGump( m_From, x ) );
			}
		}

		private void SortFindList()
		{
			if(m_SearchList != null && m_SearchList.Count > 0){
				 if(Sorttype){
					this.m_SearchList.Sort( new ListTypeSorter(Descendingsort) );
				 } else
				 if(Sortname){
					this.m_SearchList.Sort( new ListNameSorter(Descendingsort) );
				 }
			}
		}

		private class ListTypeSorter : IComparer<XmlAttachment>
		{
		  private bool Dsort;
			public ListTypeSorter(bool descend) : base ()
			{
			  Dsort = descend;
			}
			public int Compare( XmlAttachment x, XmlAttachment y )
			{
				 string xstr=null;
				 string ystr=null;
				 string str=null;
				 str = x.GetType().ToString();
				 if(str != null){
					string [] arglist = str.Split('.');
					xstr = arglist[arglist.Length-1];
				}

				str = null;
				str = y.GetType().ToString();
				 if(str != null){
					string [] arglist = str.Split('.');
					ystr = arglist[arglist.Length-1];
				}
				 if(Dsort)
				 return String.Compare(ystr, xstr, true);
				 else
				 return String.Compare(xstr, ystr, true);
			}
		}

		private class ListNameSorter : IComparer<XmlAttachment>
		{
		  private bool Dsort;

			public ListNameSorter(bool descend) : base ()
			{
			  Dsort = descend;
			}
			public int Compare( XmlAttachment x, XmlAttachment y )
			{
				 string xstr=null;
				 string ystr=null;

				xstr = x.Name;
				ystr = y.Name;
				if(Dsort)
				return String.Compare(ystr, xstr, true);
				else
				return String.Compare(xstr, ystr, true);
			}
		}

		private void Refresh(NetState state)
		{
			state.Mobile.SendGump( new XmlGetAttGump(this.m_From, this.m_TargetObject, false, this.Descendingsort,
				this.Dosearchtype, this.Dosearchname, this.Dosearchage,
				this.Searchtype, this.Searchname,  this.Searchagedirection, this.Searchage,
				this.m_SearchList, this.Selected, this.DisplayFrom,
				this.Sorttype, this.Sortname,
				this.SelectAll, this.m_SelectionList, this.X, this.Y));
		}


		public override void OnResponse( NetState state, RelayInfo info )
		{
			if(info == null || state == null || state.Mobile == null) return;
			
			int radiostate = -1;
			if(info.Switches.Length > 0){
				radiostate = info.Switches[0];
			}

			// read the text entries for the search criteria

			Searchage = 0;

			TextRelay tr = info.GetTextEntry( 400 );        // displayfrom info
			try{
			DisplayFrom = int.Parse(tr.Text);
			} catch{}

			switch ( info.ButtonID )
			{

				case 0: // Close
				{
					return;
				}

				case 156: // Delete selected items
				{
					Refresh(state);
					int allcount = 0;
					if(m_SearchList != null)
						allcount = m_SearchList.Count;
					state.Mobile.SendGump( new XmlConfirmDeleteGump(state.Mobile, m_TargetObject, m_SearchList, m_SelectionList, DisplayFrom, SelectAll, allcount) );
					return;
				}

				case 201: // forward block
				{
					// clear the selections
					if(m_SelectionList != null && !SelectAll) Array.Clear(m_SelectionList,0,m_SelectionList.Length);
					if(m_SearchList != null && DisplayFrom + MaxEntries < m_SearchList.Count) {
						DisplayFrom += MaxEntries;
						// clear any selection
						Selected = -1;
					}
					break;
				}
				case 202: // backward block
				{
					// clear the selections
					if(m_SelectionList != null && !SelectAll) Array.Clear(m_SelectionList,0,m_SelectionList.Length);
					DisplayFrom -= MaxEntries;
					if(DisplayFrom < 0) DisplayFrom = 0;
					// clear any selection
					Selected = -1;
					break;
				}

				case 700: // Sort
				{
					// clear any selection
					Selected = -1;
					// clear the selections
					if(m_SelectionList != null && !SelectAll) Array.Clear(m_SelectionList,0,m_SelectionList.Length);
					Sorttype = false;
	  				Sortname = false;

					// read the toggle switches that determine the sort
					if ( radiostate == 0 ) // sort by type
	  			   {
	  				   Sorttype = true;
	  			   }
	  			   if ( radiostate == 1 ) // sort by name
	  			   {
	  				   Sortname = true;
	  			   }
					SortFindList();
					break;
				}
				case 701: // descending sort
				{
					Descendingsort = !Descendingsort;
					break;
				}
				case 9998:  // refresh the gump
				{
					// clear any selection
					Selected = -1;
					break;
				}
				default:
				{


					if(info.ButtonID >= 3000 && info.ButtonID < 3000+ MaxEntries){

						Selected = info.ButtonID - 3000;
						// Show the props window
						Refresh(state);

						DoShowProps(info.ButtonID - 3000 + DisplayFrom);
						return;
					}

					if(info.ButtonID == 3998){

						SelectAll = !SelectAll;

						// dont allow individual selection with the selectall button selected
						if(m_SelectionList != null)
						{
							for(int i = 0; i < MaxEntries;i++)
							{
								if(i < m_SelectionList.Length){
									// only toggle the selection list entries for things that actually have entries
									m_SelectionList[i] = SelectAll;
								} else
								{
									break;
								}
							}
						}
					}
					if(info.ButtonID == 3999){

						// dont allow individual selection with the selectall button selected
						if(m_SelectionList != null && m_SearchList != null && !SelectAll)
						{
							for(int i = 0; i < MaxEntries;i++)
							{
								if(i < m_SelectionList.Length){
									// only toggle the selection list entries for things that actually have entries
									if((m_SearchList.Count - DisplayFrom > i)) {
										m_SelectionList[i] = !m_SelectionList[i];
									}
								} else
								{
									break;
								}
							}
						}
					}
					if(info.ButtonID >= 4000 && info.ButtonID < 4000+ MaxEntries){
						int i = info.ButtonID - 4000;
						// dont allow individual selection with the selectall button selected
						if(m_SelectionList != null && i >= 0  && i < m_SelectionList.Length && !SelectAll){
							// only toggle the selection list entries for things that actually have entries
							if(m_SearchList != null && (m_SearchList.Count - DisplayFrom > i)) {
								m_SelectionList[i] = !m_SelectionList[i];
							}
						}
					}
					if(info.ButtonID >= 5000 && info.ButtonID < 5000+ MaxEntries){
						int i = info.ButtonID - 5000;
						// dont allow individual selection with the selectall button selected
						if(m_SelectionList != null && i >= 0  && i < m_SelectionList.Length && !SelectAll){
							// only toggle the selection list entries for things that actually have entries
							if(m_SearchList != null && (m_SearchList.Count - DisplayFrom > i)) {
								XmlAttachment a = m_SearchList[i+DisplayFrom];
								if(a != null)
								{
									state.Mobile.SendMessage(a.OnIdentify(state.Mobile));
								}
							}
						}
					}
					break;
				}
			}
			// Create a new gump
			//m_Spawner.OnDoubleClick( state.Mobile);
			Refresh(state);
		}


	public class XmlConfirmDeleteGump : Gump
	{
		private List<XmlAttachment> SearchList;
		private bool [] SelectedList;
		private Mobile From;
		private int DisplayFrom;
		private bool selectAll;
		private object m_target;

		public XmlConfirmDeleteGump(Mobile from, object target, List<XmlAttachment> searchlist, bool [] selectedlist, int displayfrom, bool selectall, int allcount) : base ( 0, 0 )
		{
			SearchList = searchlist;
			SelectedList = selectedlist;
			DisplayFrom = displayfrom;
			selectAll = selectall;
			m_target = target;
			From = from;
			Closable = false;
			Dragable = true;
			AddPage( 0 );
			AddBackground( 10, 200, 200, 130, 5054 );
			int count = 0;
			if(selectall)
			{
				count = allcount;
			} else
			{
				for(int i =0;i<SelectedList.Length;i++){
					if(SelectedList[i]) count++;
				}
			}

			AddLabel( 20, 225, 33, String.Format("Delete {0} attachments?",count) );
			AddRadio( 35, 255, 9721, 9724, false, 1 ); // accept/yes radio
			AddRadio( 135, 255, 9721, 9724, true, 2 ); // decline/no radio
			AddHtmlLocalized(72, 255, 200, 30, 1049016, 0x7fff , false , false ); // Yes
			AddHtmlLocalized(172, 255, 200, 30, 1049017, 0x7fff , false , false ); // No
			AddButton( 80, 289, 2130, 2129, 3, GumpButtonType.Reply, 0 ); // Okay button

		}
		public override void OnResponse( NetState state, RelayInfo info )
		{

			if(info == null || state == null || state.Mobile == null) return;

			int radiostate = -1;
			if(info.Switches.Length > 0){
				radiostate = info.Switches[0];
			}
			switch(info.ButtonID)
			{

				default:
				{
					if(radiostate == 1 && SearchList != null && SelectedList != null)
					{    // accept
						for(int i = 0;i < SearchList.Count;i++){
							int index = i-DisplayFrom;
							if((index >= 0 && index < SelectedList.Length && SelectedList[index] == true) || selectAll){
								XmlAttachment o = SearchList[i];
								// some objects may not delete gracefully (null map items are particularly error prone) so trap them
								try {
								o.Delete();
								} catch {}
							}
						}
						// refresh the gump
						state.Mobile.CloseGump(typeof(XmlGetAttGump));
						state.Mobile.SendGump(new XmlGetAttGump(state.Mobile,m_target,0,0));
					}
					break;
				}
			}
		}
	}

	}
}
