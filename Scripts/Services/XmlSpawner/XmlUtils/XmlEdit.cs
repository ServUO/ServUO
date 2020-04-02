using System;
using System.Data;
using System.IO;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis;
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
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

/*
** XmlEditNPC
** Version 1.00
** updated 5/24/05
** ArteGordon
**
**
*/

namespace Server.Engines.XmlSpawner2
{
	public class XmlEditDialogGump : Gump
	{
		private const int MaxEntries = 8;
		private const int MaxEntriesPerPage = 8;

		private Mobile m_From;
		private string Name;
		private int Selected;
		private int DisplayFrom;
		private string SaveFilename;
		private bool [] m_SelectionList;
		private XmlDialog m_Dialog;

		private bool SelectAll = false;

		private ArrayList m_SearchList;

		public static void Initialize()
		{
			CommandSystem.Register( "XmlEdit", AccessLevel.GameMaster, new CommandEventHandler( XmlEditDialog_OnCommand ) );
		}

		[Usage( "XmlEdit" )]
		[Description( "Edits XmlDialog entries on an object" )]
		public static void XmlEditDialog_OnCommand( CommandEventArgs e )
		{
			if(e == null || e.Mobile == null) return;

			// target an object with the xmldialog attachment
			e.Mobile.Target = new EditDialogTarget();


		}

		private class EditDialogTarget : Target
		{

			public EditDialogTarget() :  base ( 30, true, TargetFlags.None )
			{

			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if(from == null) return;

				// does it have an xmldialog attachment?
				XmlDialog xd = XmlAttach.FindAttachment(targeted, typeof(XmlDialog)) as XmlDialog;

				if(xd == null)
				{
					from.SendMessage("Target has no XmlDialog attachment");

					// TODO: ask whether they would like to add one
					from.SendGump( new XmlConfirmAddGump(from, targeted));

					return;
				}

				from.SendGump( new XmlEditDialogGump(from, true, xd, -1, 0, null, false, null, 0, 0));
			}
		}

		public class XmlConfirmAddGump : Gump
		{
			private Mobile m_From;
			private object m_Targeted;

			public XmlConfirmAddGump(Mobile from, object targeted) : base ( 0, 0 )
			{
				if(from == null || targeted == null) return;

				m_Targeted = targeted;
				m_From = from;

				Closable = false;
				Dragable = true;
				AddPage( 0 );
				AddBackground( 10, 200, 200, 130, 5054 );

				AddLabel( 20, 210, 68, String.Format("Add an XmlDialog to target?") );

				string name = null;
				if(targeted is Item)
				{
					name = ((Item)targeted).Name;
				} 
				else
					if(targeted is Mobile)
				{
					name = ((Mobile)targeted).Name;
				}

				if(name == null)
				{
					name = targeted.GetType().Name;
				}
				AddLabel( 20, 230, 0, String.Format("{0}", name) );

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
				if(info.Switches.Length > 0)
				{
					radiostate = info.Switches[0];
				}
				switch(info.ButtonID)
				{

					default:
					{
						if(radiostate == 1)
						{    // accept

							// add the attachment
							XmlDialog xd = new XmlDialog();
							XmlAttach.AttachTo(state.Mobile, m_Targeted, xd);

							// open the editing gump
							state.Mobile.SendGump( new XmlEditDialogGump(state.Mobile, true, xd, -1, 0, null, false, null, 0, 0));
						}
						break;
					}
				}
			}
		}

		public const int MaxLabelLength = 200;
		public string TruncateLabel(string s)
		{
			if (s == null || s.Length < MaxLabelLength) return s;

			return s.Substring(0,MaxLabelLength);
		}

		public XmlEditDialogGump( Mobile from,  bool firststart, 
			XmlDialog dialog, int selected, int displayfrom, string savefilename,
			bool selectall, bool [] selectionlist, int X, int Y ) : base( X,Y )
		{

			if(from == null || dialog == null) return;

			m_Dialog = dialog;
			m_From = from;

			m_SelectionList = selectionlist;
			if(m_SelectionList == null)
			{
				m_SelectionList = new bool[MaxEntries];
			}
			SaveFilename = savefilename;

			DisplayFrom = displayfrom;
			Selected = selected;

			// fill the list with the XmlDialog entries
			m_SearchList = dialog.SpeechEntries;
			
			// prepare the page
			int height = 500;
			

			AddPage( 0 );

			AddBackground( 0, 0, 755, height, 5054 );
			AddAlphaRegion( 0, 0, 755, height );

			// add the separators
			AddImageTiled( 10, 80, 735, 4, 0xBB9 );
			AddImageTiled( 10, height -212, 735, 4, 0xBB9 );
			AddImageTiled( 10, height -60, 735, 4, 0xBB9 );

			// add the xmldialog properties
			int y = 5;
			int w = 140;
			int x = 5;
			int lw = 0;
			// the dialog name
			AddImageTiled( x, y, w, 21, 0x23F4 );
			// get the name of the object this is attached to
			
			if(m_Dialog.AttachedTo is Item)
			{
				Name = ((Item)m_Dialog.AttachedTo).Name;
			} 
			else
				if(m_Dialog.AttachedTo is Mobile)
			{
				Name = ((Mobile)m_Dialog.AttachedTo).Name;
			}
			if(Name == null && m_Dialog.AttachedTo != null)
			{
				Name = m_Dialog.AttachedTo.GetType().Name;
			}
			AddLabelCropped( x, y, w, 21, 0, Name );


			x += w + lw + 20;
			w = 40;
			lw = 90;
			// add the proximity range
			AddLabel( x, y, 0x384, "ProximityRange" );
			AddImageTiled( x+lw, y, w, 21, 0xBBC );
			AddTextEntry( x+lw, y, w, 21, 0, 140, m_Dialog.ProximityRange.ToString() );

			x += w + lw + 20;
			w = 100;
			lw = 60;
			// reset time
			AddLabel( x, y, 0x384, "ResetTime" );
			AddImageTiled( x+lw, y, w, 21, 0xBBC );
			AddTextEntry( x+lw, y, w, 21, 0, 141, m_Dialog.ResetTime.ToString() );

			x += w + lw + 20;
			w = 40;
			lw = 65;
			// speech pace
			AddLabel( x, y, 0x384, "SpeechPace" );
			AddImageTiled( x+lw, y, w, 21, 0xBBC );
			AddTextEntry( x+lw, y, w, 21, 0, 142, m_Dialog.SpeechPace.ToString() );

			x += w + lw + 20;
			w = 30;
			lw = 55;
			// allow ghost triggering
			AddLabel( x, y, 0x384, "GhostTrig" );
			AddCheck( x+lw, y, 0xD2, 0xD3, m_Dialog.AllowGhostTrig, 260);

			// add the triggeroncarried
			y += 27;
			w = 600;
			x = 10;
			lw = 100;
			AddLabel( 10, y, 0x384, "TrigOnCarried" );
			AddImageTiled( x + lw, y, w, 21, 0xBBC );
			AddTextEntry( x+lw, y, w, 21, 0, 150, TruncateLabel(m_Dialog.TriggerOnCarried) );
			AddButton( 720, y, 0xFAB, 0xFAD, 5005, GumpButtonType.Reply, 0 );

			// add the notriggeroncarried
			y += 22;
			w = 600;
			x = 10;
			lw = 100;
			AddLabel( x, y, 0x384, "NoTrigOnCarried" );
			AddImageTiled( x+lw, y, w, 21, 0xBBC );
			AddTextEntry(x + lw, y, w, 21, 0, 151, TruncateLabel(m_Dialog.NoTriggerOnCarried));
			AddButton( 720, y, 0xFAB, 0xFAD, 5006, GumpButtonType.Reply, 0 );

			y = 88;
			// column labels
			AddLabel( 10, y, 0x384, "Edit" );
			AddLabel( 45, y, 0x384, "#" );
			AddLabel( 95, y, 0x384, "ID" );
			AddLabel( 145, y, 0x384, "Depends" );
			AddLabel( 195, y, 0x384, "Keywords" );
			AddLabel( 295, y, 0x384, "Text" );
			AddLabel( 540, y, 0x384, "Action" );
			AddLabel( 602, y, 0x384, "Condition" );
			AddLabel( 664, y, 0x384, "Gump" );

			// display the select-all-displayed toggle
			AddButton( 730, y, 0xD2, 0xD3, 3999, GumpButtonType.Reply, 0 );

			y -= 10;
			for ( int i = 0;  i < MaxEntries; i++ )
			{
				int index = i + DisplayFrom;
				if(m_SearchList == null || index >= m_SearchList.Count) break;
				int page = (int)(i/MaxEntriesPerPage);
				if(i%MaxEntriesPerPage == 0)
				{
					AddPage(page+1);
					// add highlighted page button
					//AddImageTiled( 235+page*25, 448, 25, 25, 0xBBC );
					//AddImage( 238+page*25, 450, 0x8B1+page );
				}

				// background for search results area
				AddImageTiled( 235, y + 22 * (i%MaxEntriesPerPage)  + 30, 386, 23, 0x52 );
				AddImageTiled( 236, y + 22 * (i%MaxEntriesPerPage) + 31, 384, 21, 0xBBC );


				XmlDialog.SpeechEntry s = (XmlDialog.SpeechEntry)m_SearchList[index];

				if(s == null) continue;

				int texthue = 0;

				bool sel=false;

				if(m_SelectionList != null && i < m_SelectionList.Length)
				{
					sel = m_SelectionList[i];
				}

				// entries with the selection box checked are highlighted in red
				if(sel) texthue = 33;

				// the selected entry is highlighted in green
				if(i == Selected) texthue = 68;

				x = 10;
				w = 35;
				// add the Edit button for each entry
				AddButton( 10, y + 22 * (i%MaxEntriesPerPage)  + 30, 0xFAE, 0xFAF, 1000+i, GumpButtonType.Reply, 0 );

				x += w;
				w = 50;
				// display the entry number
				AddImageTiled( x, y + 22 * (i%MaxEntriesPerPage)  + 31, w, 21, 0xBBC );
				AddLabel( x, y + 22 * (i%MaxEntriesPerPage) + 31, texthue, s.EntryNumber.ToString() );
				
				x += w;
				w = 50;
				// display the entry ID
				AddImageTiled( x, y + 22 * (i%MaxEntriesPerPage)  + 31, w, 21, 0x23F4 );
				AddLabel( x, y + 22 * (i%MaxEntriesPerPage) + 31, texthue, s.ID.ToString() );

				x += w;
				w = 50;
				// display the entry dependson
				AddImageTiled( x, y + 22 * (i%MaxEntriesPerPage)  + 31, w, 21, 0xBBC  );
				AddLabel( x, y + 22 * (i%MaxEntriesPerPage) + 31, texthue, s.DependsOn );

				x += w;
				w = 100;				
				// display the entry keywords
				AddImageTiled( x, y + 22 * (i%MaxEntriesPerPage)  + 31, w, 21, 0x23F4 );
				AddLabelCropped( x, y + 22 * (i%MaxEntriesPerPage) + 31, w-5, 21, texthue, TruncateLabel(s.Keywords) );

				x += w;
				w = 245;
				// display the entry text
				AddImageTiled( x, y + 22 * (i%MaxEntriesPerPage)  + 31, w, 21, 0xBBC );
				AddLabelCropped( x, y + 22 * (i%MaxEntriesPerPage) + 31, w-5, 21, texthue, TruncateLabel(s.Text) );

				x += w;
				w = 62;
				// display the action text
				AddImageTiled( x, y + 22 * (i%MaxEntriesPerPage)  + 31, w, 21, 0x23F4 );
				AddLabelCropped( x, y + 22 * (i%MaxEntriesPerPage) + 31, w-5, 21, texthue, TruncateLabel(s.Action) );

				x += w;
				w = 62;
				// display the condition text
				AddImageTiled( x, y + 22 * (i%MaxEntriesPerPage)  + 31, w, 21, 0xBBC );
				AddLabelCropped( x, y + 22 * (i%MaxEntriesPerPage) + 31, w-5, 21, texthue, TruncateLabel(s.Condition) );

				x += w;
				w = 62;
				// display the gump text
				AddImageTiled( x, y + 22 * (i%MaxEntriesPerPage)  + 31, w, 21, 0x23F4 );
				AddLabelCropped( x, y + 22 * (i%MaxEntriesPerPage) + 31, w-5, 21, texthue, TruncateLabel(s.Gump) );

				// display the selection button
				AddButton( 730, y + 22 * (i%MaxEntriesPerPage)  + 32, (sel? 0xD3:0xD2), (sel? 0xD2:0xD3), 4000+i, GumpButtonType.Reply, 0 );

			}


			// display the selected entry information for editing
			XmlDialog.SpeechEntry sentry = null;
			if(Selected >= 0 && Selected + DisplayFrom >= 0 && Selected + DisplayFrom < m_SearchList.Count)
			{
				sentry = (XmlDialog.SpeechEntry)m_SearchList[Selected+ DisplayFrom];
			}

			if(sentry != null)
			{

				y = height - 200;

				// add the entry parameters
				lw = 15;
				w = 40;
				x = 10;
				int spacing = 11;

				// entry number
				AddLabel( x, y, 0x384, "#" );
				AddImageTiled( x+lw, y, w, 21, 0xBBC );
				AddTextEntry( x+lw, y, w, 21, 0, 200, sentry.EntryNumber.ToString() );

				x += w + lw + spacing;
				w = 40;
				lw = 17;
				// ID number
				AddLabel( x, y, 0x384, "ID" );
				AddImageTiled( x+lw, y, w, 21, 0xBBC );
				AddTextEntry( x+lw, y, w, 21, 0, 201, sentry.ID.ToString() );

				x += w + lw + spacing;
				w = 40;
				lw = 65;
				// depends on 
				AddLabel( x, y, 0x384, "DependsOn" );
				AddImageTiled( x+lw, y, w, 21, 0xBBC );
				AddTextEntry( x+lw, y, w, 21, 0, 202, sentry.DependsOn );

				x += w + lw + spacing;
				w = 35;
				lw = 57;
				// prepause 
				AddLabel( x, y, 0x384, "PrePause" );
				AddImageTiled( x+lw, y, w, 21, 0xBBC );
				AddTextEntry( x+lw, y, w, 21, 0, 203, sentry.PrePause.ToString() );

				x += w + lw + spacing;
				w = 35;
				lw = 37;
				// pause
				AddLabel( x, y, 0x384, "Pause" );
				AddImageTiled( x+lw, y, w, 21, 0xBBC );
				AddTextEntry( x+lw, y, w, 21, 0, 204, sentry.Pause.ToString() );

				x += w + lw + spacing;
				w = 37;
				lw = 26;
				// speech hue
				AddLabel( x, y, 0x384, "Hue" );
				AddImageTiled( x+lw, y, w, 21, 0xBBC );
				AddTextEntry( x+lw, y, w, 21, 0, 205, sentry.SpeechHue.ToString() );

				x += w + lw + spacing;
				w = 20;
				lw = 52;
				// lock conversation
				AddLabel(x, y, 0x384, "IgnoreCar");
				AddCheck(x + lw, y, 0xD2, 0xD3, sentry.IgnoreCarried, 252);

				x += w + lw + spacing;
				w = 20;
				lw = 42;
				// lock conversation
				AddLabel( x, y, 0x384, "LockOn" );
				AddCheck( x+lw, y, 0xD2, 0xD3, sentry.LockConversation, 250);

				x += w + lw + spacing;
				w = 20;
				lw = 54;
				// npctrigger
				AddLabel( x, y, 0x384, "AllowNPC" );
				AddCheck( x+lw, y, 0xD2, 0xD3, sentry.AllowNPCTrigger, 251);


				w = 650;
				x = 70;

				y += 27;
				// add the keyword entry
				AddLabel( 10, y, 0x384, "Keywords" );
				AddImageTiled( x, y, w, 21, 0xBBC );
				AddTextEntry( x+1, y, w, 21, 0, 101, sentry.Keywords );
				AddButton( 720, y, 0xFAB, 0xFAD, 5001, GumpButtonType.Reply, 0 );

				y += 22;
				// add the text entry
				AddLabel( 10, y, 0x384, "Text" );
				AddImageTiled( x, y, w, 21, 0xBBC );
				AddTextEntry( x+1, y, w, 21, 0, 100, sentry.Text );
				AddButton( 720, y, 0xFAB, 0xFAD, 5000, GumpButtonType.Reply, 0 );


				y += 22;
				// add the condition string entry
				AddLabel( 10, y, 0x384, "Condition" );
				AddImageTiled( x, y, w, 21, 0xBBC );
				AddTextEntry( x+1, y, w, 21, 0, 102, sentry.Condition );
				AddButton( 720, y, 0xFAB, 0xFAD, 5002, GumpButtonType.Reply, 0 );

				y += 22;
				// add the action string entry
				AddLabel( 10, y, 0x384, "Action" );
				AddImageTiled( x, y, w, 21, 0xBBC );
				AddTextEntry( x+1, y, w, 21, 0, 103, sentry.Action );
				AddButton( 720, y, 0xFAB, 0xFAD, 5003, GumpButtonType.Reply, 0 );

				y += 22;
				// add the gump string entry
				AddLabel( 10, y, 0x384, "Gump" );
				AddImageTiled( x, y, w, 21, 0xBBC );
				AddTextEntry( x+1, y, w, 21, 0, 104, sentry.Gump );
				AddButton( 720, y, 0xFAB, 0xFAD, 5004, GumpButtonType.Reply, 0 );
			}

			y = height - 50;

			AddLabel( 10, y, 0x384, "Config:" );
			AddImageTiled( 50, y , 120, 19, 0x23F4 );
			AddLabel( 50, y, 0, m_Dialog.ConfigFile );

			if(from.AccessLevel >= XmlSpawner.DiskAccessLevel)
			{
				
				// add the save entry
				AddButton( 185, y , 0xFA8, 0xFAA, 159, GumpButtonType.Reply, 0 );
				AddLabel( 218, y , 0x384, "Save to file:" );
				AddImageTiled( 300, y , 180, 19, 0xBBC );
				AddTextEntry( 300, y, 180, 19, 0, 300, SaveFilename );
			}

			// display the item list
			if(m_SearchList != null)
			{
				AddLabel( 495, y, 68, String.Format("{0} Entries",m_SearchList.Count) );
				int last = DisplayFrom + MaxEntries < m_SearchList.Count ? DisplayFrom + MaxEntries : m_SearchList.Count;
				if(last > 0) 
					AddLabel( 595, y, 68, String.Format("Displaying {0}-{1}",DisplayFrom, last -1) );
			}

			y = height - 25;

			// add run status display
			if(m_Dialog.Running)
			{
				AddButton( 10, y-5, 0x2A4E, 0x2A3A, 100, GumpButtonType.Reply, 0 );
				AddLabel( 43, y, 0x384, "On" );
			} 
			else 
			{
				AddButton( 10, y-5, 0x2A62, 0x2A3A, 100, GumpButtonType.Reply, 0 );
				AddLabel( 43, y, 0x384, "Off" );
			}

			// add the Refresh/Sort button
			AddButton( 80, y, 0xFAB, 0xFAD, 700, GumpButtonType.Reply, 0 );
			AddLabel( 113, y, 0x384, "Refresh" );

			// add the Add button
			AddButton( 185, y, 0xFAB, 0xFAD, 155, GumpButtonType.Reply, 0 );
			AddLabel( 218, y, 0x384, "Add" );

			// add the Delete button
			AddButton( 255, y, 0xFB1, 0xFB3, 156, GumpButtonType.Reply, 0 );
			AddLabel( 283, y, 0x384, "Delete" );

			// add the page buttons
			for(int i = 0;i<(int)(MaxEntries/MaxEntriesPerPage);i++)
			{
				AddButton( 513+i*25, y, 0x8B1+i, 0x8B1+i, 0, GumpButtonType.Page, 1+i );
			}

			// add the advance pageblock buttons
			AddButton( 510+25*(int)(MaxEntries/MaxEntriesPerPage), y, 0x15E1, 0x15E5, 201, GumpButtonType.Reply, 0 ); // block forward
			AddButton( 490, y, 0x15E3, 0x15E7, 202, GumpButtonType.Reply, 0 ); // block backward

			// add the displayfrom entry
			AddLabel( 555, y, 0x384, "Display" );
			AddImageTiled( 595, y, 60, 21, 0xBBC );
			AddTextEntry( 595, y, 60, 21, 0, 400, DisplayFrom.ToString() );
			AddButton( 655, y, 0xFAB, 0xFAD, 9998, GumpButtonType.Reply, 0 );

			//AddLabel( 610, y, 0x384, "Select All" );
			// display the select-all toggle
			//AddButton( 670, y, (SelectAll? 0xD3:0xD2), (SelectAll? 0xD2:0xD3), 3998, GumpButtonType.Reply, 0 );

		}

 

		private void SortFindList()
		{
			if(m_SearchList != null && m_SearchList.Count > 0)
			{

				this.m_SearchList.Sort( new ListSorter(false) );

			}
		}

		private class ListSorter : IComparer
		{
			private bool Dsort;
			public ListSorter(bool descend) : base ()
			{
				Dsort = descend;
			}
			public int Compare( object x, object y )
			{
				int xn = 0;
				int yn = 0;


				xn = ((XmlDialog.SpeechEntry)x).EntryNumber;

				yn = ((XmlDialog.SpeechEntry)y).EntryNumber;

				
				if(Dsort)
					return yn - xn;
				else
					return xn- yn;
			}
		}



		private void SaveList(Mobile from,  string filename)
		{
			if(m_SearchList == null || m_SelectionList == null) return;
		  
			string dirname;
			if( System.IO.Directory.Exists( XmlDialog.DefsDir ) && filename != null && !filename.StartsWith("/") && !filename.StartsWith("\\"))
			{
				// put it in the defaults directory if it exists
				dirname = String.Format("{0}/{1}",XmlDialog.DefsDir,filename);
			} 
			else 
			{
				// otherwise just put it in the main installation dir
				dirname = filename;
			}

			// save it to the file
		}

		private XmlEditDialogGump Refresh(NetState state)
		{
			XmlEditDialogGump g = new XmlEditDialogGump(this.m_From, false, this.m_Dialog, this.Selected, 
				this.DisplayFrom, this.SaveFilename, this.SelectAll, this.m_SelectionList, this.X, this.Y);
			state.Mobile.SendGump(g);
			return g;
		}

		public static void ProcessXmlEditBookEntry(Mobile from, object[] args, string text)
		{

			if(from == null || args == null || args.Length < 6) return;

			XmlDialog dialog = (XmlDialog)args[0];
			XmlDialog.SpeechEntry entry = (XmlDialog.SpeechEntry)args[1];
			int textid = (int)args[2];
			int selected = (int)args[3];
			int displayfrom = (int)args[4];
			string savefile = (string)args[5];

			// place the book text into the entry by type
			switch(textid)
			{
				case 0: // text
					if(entry != null)
						entry.Text = text;
					break;
				case 1: // keywords
					if(entry != null)
						entry.Keywords = text;
					break;
				case 2: // condition
					if(entry != null)
						entry.Condition = text;
					break;
				case 3: // action
					if(entry != null)
						entry.Action = text;
					break;
				case 4: // gump
					if(entry != null)
						entry.Gump = text;
					break;
				case 5: // trigoncarried
					if(dialog != null)
						dialog.TriggerOnCarried = text;
					break;
				case 6: // notrigoncarried
					if(dialog != null)
						dialog.NoTriggerOnCarried = text;
					break;
			}


			from.CloseGump(typeof(XmlEditDialogGump));

			//from.SendGump( new XmlEditDialogGump(from, false, m_Dialog, selected, displayfrom, savefilename, false, null, X, Y) );
			from.SendGump( new XmlEditDialogGump(from, true, dialog, selected, displayfrom, savefile, false, null, 0, 0));
		}


		public override void OnResponse( NetState state, RelayInfo info )
		{
			if(info == null || state == null || state.Mobile == null || m_Dialog == null) return;
            
			int radiostate = -1;
			if(info.Switches.Length > 0)
			{
				radiostate = info.Switches[0];
			}



			TextRelay tr = info.GetTextEntry( 400 );        // displayfrom info
			try
			{
				DisplayFrom = int.Parse(tr.Text);
			} 
			catch{}


			tr = info.GetTextEntry( 300 );        // savefilename info
			if(tr != null)
				SaveFilename = tr.Text;

			if(m_Dialog != null)
			{
				tr = info.GetTextEntry( 140 );        // proximity range
				if(tr != null)
				{
					try
					{
						m_Dialog.ProximityRange = int.Parse(tr.Text);
					} 
					catch{}
				}
				tr = info.GetTextEntry( 141 );        // reset time
				if(tr != null)
				{
					try
					{
						m_Dialog.ResetTime = TimeSpan.Parse(tr.Text);
					} 
					catch{}
				}
				tr = info.GetTextEntry( 142 );        // speech pace
				if(tr != null)
				{
					try
					{
						m_Dialog.SpeechPace = int.Parse(tr.Text);
					} 
					catch{}
				}

				tr = info.GetTextEntry( 150 );        // trig on carried
				if(tr != null && (m_Dialog.TriggerOnCarried == null || m_Dialog.TriggerOnCarried.Length < 230))
				{
					if(tr.Text != null && tr.Text.Trim().Length > 0)
					{
						m_Dialog.TriggerOnCarried = tr.Text;
					} 
					else
					{
						m_Dialog.TriggerOnCarried = null;
					}
				}

				tr = info.GetTextEntry( 151 );        // notrig on carried
				if(tr != null && (m_Dialog.NoTriggerOnCarried == null || m_Dialog.NoTriggerOnCarried.Length < 230))
				{
					if(tr.Text != null && tr.Text.Trim().Length > 0)
					{
						m_Dialog.NoTriggerOnCarried = tr.Text;
					} 
					else
					{
						m_Dialog.NoTriggerOnCarried = null;
					}
				}

				m_Dialog.AllowGhostTrig = info.IsSwitched(260);	// allow ghost triggering
			}

			if(m_SearchList != null && Selected >= 0 && Selected + DisplayFrom >= 0 && Selected + DisplayFrom < m_SearchList.Count)
			{
				// entry information
				XmlDialog.SpeechEntry entry = (XmlDialog.SpeechEntry)m_SearchList[Selected + DisplayFrom]; 

				tr = info.GetTextEntry( 200 );        // entry number
				if(tr != null)
				{
					try
					{
						entry.EntryNumber = int.Parse(tr.Text);
					} 
					catch {}
				}

				tr = info.GetTextEntry( 201 );        // entry id
				if(tr != null)
				{
					try
					{
						entry.ID = int.Parse(tr.Text);
					} 
					catch {}
				}

				tr = info.GetTextEntry( 202 );        // depends on
				if(tr != null)
				{
					try
					{
						entry.DependsOn = tr.Text;
					} 
					catch {}
				}

				tr = info.GetTextEntry( 203 );        // prepause
				if(tr != null)
				{
					try
					{
						entry.PrePause = int.Parse(tr.Text);
					} 
					catch {}
				}

				tr = info.GetTextEntry( 204 );        // pause
				if(tr != null)
				{
					try
					{
						entry.Pause = int.Parse(tr.Text);
					} 
					catch {}
				}

				tr = info.GetTextEntry( 205 );        // hue
				if(tr != null)
				{
					try
					{
						entry.SpeechHue = int.Parse(tr.Text);
					} 
					catch {}
				}

				tr = info.GetTextEntry( 101 );        // keywords
				if(tr != null && (entry.Keywords == null || entry.Keywords.Length < 230))
				{
					if(tr.Text != null && tr.Text.Trim().Length > 0)
					{
						entry.Keywords = tr.Text;
					} 
					else
					{
						entry.Keywords = null;
					}

				}

				tr = info.GetTextEntry( 100 );        // text
				if(tr != null && (entry.Text == null || entry.Text.Length < 230))
				{
					if(tr.Text != null && tr.Text.Trim().Length > 0)
					{
						entry.Text = tr.Text;
					} 
					else
					{
						entry.Text = null;
					}
				}

				tr = info.GetTextEntry( 102 );        // condition
				if(tr != null && (entry.Condition == null || entry.Condition.Length < 230))
				{
					if(tr.Text != null && tr.Text.Trim().Length > 0)
					{
						entry.Condition = tr.Text;
					} 
					else
					{
						entry.Condition = null;
					}
				}

				tr = info.GetTextEntry( 103 );        // action
				if(tr != null && (entry.Action == null || entry.Action.Length < 230))
				{
					if(tr.Text != null && tr.Text.Trim().Length > 0)
					{
						entry.Action = tr.Text;
					} 
					else
					{
						entry.Action = null;
					}
				}

				tr = info.GetTextEntry( 104 );        // gump
				if(tr != null && (entry.Gump == null || entry.Gump.Length < 230))
				{
					if(tr.Text != null && tr.Text.Trim().Length > 0)
					{
						entry.Gump = tr.Text;
					} 
					else
					{
						entry.Gump = null;
					}
				}

				entry.LockConversation = info.IsSwitched(250);	// lock conversation
				entry.AllowNPCTrigger = info.IsSwitched(251);	// allow npc
				entry.IgnoreCarried = info.IsSwitched(252);	// ignorecarried
			}



			switch ( info.ButtonID )
			{

				case 0: // Close
				{

					m_Dialog.DeleteTextEntryBook();

					return;
				}
				case 100: // toggle running status
				{

					m_Dialog.Running = !m_Dialog.Running;

					break;
				}
				case 155: // add new entry
				{

					if(m_SearchList != null)
					{
						// find the last entry
						int lastentry = 0;
						foreach(XmlDialog.SpeechEntry e in m_SearchList)
						{
							if(e.EntryNumber > lastentry)
								lastentry = e.EntryNumber;
						}
						lastentry += 10;
						XmlDialog.SpeechEntry se = new XmlDialog.SpeechEntry();
						se.EntryNumber = lastentry;
						se.ID = lastentry;
						m_SearchList.Add(se);
						Selected = m_SearchList.Count -1;
					}
					break;
				}

				case 156: // Delete selected entries
				{
					XmlEditDialogGump g = Refresh(state);
					int allcount = 0;
					if(m_SearchList != null)
						allcount = m_SearchList.Count;
					state.Mobile.SendGump( new XmlConfirmDeleteGump(state.Mobile, g, m_SearchList, m_SelectionList, DisplayFrom, SelectAll, allcount) );
					return;
				}

				case 159: // save to a .npc file
				{           

					// Create a new gump
					Refresh(state);
					// try to save
					m_Dialog.DoSaveNPC(state.Mobile, SaveFilename, true);

					return;
				}

				case 201: // forward block
				{
					// clear the selections
					if(m_SelectionList != null && !SelectAll) Array.Clear(m_SelectionList,0,m_SelectionList.Length);
					if(m_SearchList != null && DisplayFrom + MaxEntries < m_SearchList.Count) 
					{
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

					SortFindList();
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

					if(info.ButtonID >= 1000 && info.ButtonID < 1000+ MaxEntries)
					{
						// flag the entry selected
						Selected = info.ButtonID - 1000;
					} 
					else
						if(info.ButtonID == 3998)
					{

						SelectAll = !SelectAll;

						// dont allow individual selection with the selectall button selected
						if(m_SelectionList != null)
						{
							for(int i = 0; i < MaxEntries;i++)
							{
								if(i < m_SelectionList.Length)
								{
									// only toggle the selection list entries for things that actually have entries
									m_SelectionList[i] = SelectAll;
								} 
								else 
								{
									break;
								}
							}
						}
					} 
					else
						if(info.ButtonID == 3999)
					{

						// dont allow individual selection with the selectall button selected
						if(m_SelectionList != null && m_SearchList != null && !SelectAll)
						{
							for(int i = 0; i < MaxEntries;i++)
							{
								if(i < m_SelectionList.Length)
								{
									// only toggle the selection list entries for things that actually have entries
									if((m_SearchList.Count - DisplayFrom > i)) 
									{
										m_SelectionList[i] = !m_SelectionList[i];
									}
								} 
								else 
								{
									break;
								}
							}
						}
					} 
					else
						if(info.ButtonID >= 4000 && info.ButtonID < 4000+ MaxEntries)
					{
						int i = info.ButtonID - 4000;
						// dont allow individual selection with the selectall button selected
						if(m_SelectionList != null && i >= 0  && i < m_SelectionList.Length && !SelectAll)
						{
							// only toggle the selection list entries for things that actually have entries
							if(m_SearchList != null && (m_SearchList.Count - DisplayFrom > i)) 
							{
								m_SelectionList[i] = !m_SelectionList[i];
							}
						}
					} 
					else
						if(info.ButtonID >= 5000 && info.ButtonID < 5100)
					{

						
						// text entry book buttons
						int textid = info.ButtonID - 5000;

						// entry information
						XmlDialog.SpeechEntry entry = null;

						if(m_SearchList != null && Selected >= 0 && Selected + DisplayFrom >= 0 && Selected + DisplayFrom < m_SearchList.Count)
						{
							entry = (XmlDialog.SpeechEntry)m_SearchList[Selected + DisplayFrom]; 
						}

						string text = String.Empty;
						string title = String.Empty;
						switch(textid)
						{
							case 0: // text
								if(entry != null)
									text = entry.Text;
								title = "Text";
								break;
							case 1: // keywords
								if(entry != null)
									text = entry.Keywords;
								title = "Keywords";
								break;
							case 2: // condition
								if(entry != null)
									text = entry.Condition;
								title = "Condition";
								break;
							case 3: // action
								if(entry != null)
									text = entry.Action;
								title = "Action";
								break;
							case 4: // gump
								if(entry != null)
									text = entry.Gump;
								title = "Gump";
								break;
							case 5: // trigoncarried
								text = m_Dialog.TriggerOnCarried;
								title = "TrigOnCarried";
								break;
							case 6: // notrigoncarried
								text = m_Dialog.NoTriggerOnCarried;
								title = "NoTrigOnCarried";
								break;
						}

						object [] args = new object[6];

						args[0] = m_Dialog;
						args[1] = entry;
						args[2] = textid;
						args[3] = Selected;
						args[4] = DisplayFrom;
						args[5] = SaveFilename;

						XmlTextEntryBook book = new XmlTextEntryBook(0, String.Empty, m_Dialog.Name, 20, true, new XmlTextEntryBookCallback(ProcessXmlEditBookEntry), args);
						
						if(m_Dialog.m_TextEntryBook == null)
						{
							m_Dialog.m_TextEntryBook = new ArrayList();
						}
						m_Dialog.m_TextEntryBook.Add(book);

						book.Title = title;
						book.Author = Name;

						// fill the contents of the book with the current text entry data
						book.FillTextEntryBook(text);

						// put the book at the location of the player so that it can be opened, but drop it below visible range
						book.Visible = false;
						book.Movable = false;
						book.MoveToWorld(new Point3D(state.Mobile.Location.X,state.Mobile.Location.Y,state.Mobile.Location.Z-100), state.Mobile.Map);

						// Create a new gump
						Refresh(state);

						// and open it
						book.OnDoubleClick(state.Mobile);

						return;

					}
					break;
				}
			}
			// Create a new gump
			Refresh(state);
		}
        
        
		public class XmlConfirmDeleteGump : Gump
		{
			private ArrayList SearchList;
			private bool [] SelectedList;
			private Mobile From;
			private int DisplayFrom;
			private bool selectAll;
			XmlEditDialogGump m_Gump;

			public XmlConfirmDeleteGump(Mobile from, XmlEditDialogGump gump, ArrayList searchlist, bool [] selectedlist, int displayfrom, bool selectall, int allcount) : base ( 0, 0 )
			{
				SearchList = searchlist;
				SelectedList = selectedlist;
				DisplayFrom = displayfrom;
				selectAll = selectall;
				m_Gump = gump;
				From = from;
				Closable = false;
				Dragable = true;
				AddPage( 0 );
				AddBackground( 10, 200, 200, 130, 5054 );
				int count = 0;
				if(selectall)
				{
					count = allcount;
				} 
				else
				{
					for(int i =0;i<SelectedList.Length;i++)
					{
						if(SelectedList[i]) count++;
					}
				}

				AddLabel( 20, 225, 33, String.Format("Delete {0} entries?",count) );
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
				if(info.Switches.Length > 0)
				{
					radiostate = info.Switches[0];
				}
				switch(info.ButtonID)
				{

					default:
					{
						if(radiostate == 1 && SearchList != null && SelectedList != null)
						{    // accept
							ArrayList dlist = new ArrayList();
							for(int i = 0;i < SearchList.Count;i++)
							{
								int index = i-DisplayFrom;
								if((index >= 0 && index < SelectedList.Length && SelectedList[index] == true) || selectAll)
								{
									object o = SearchList[i];
									// delete the entry;
									dlist.Add(o);
								}
							}

							foreach(object o in dlist)
							{
								SearchList.Remove(o);
							}

							// clear the selections
							Array.Clear(SelectedList,0,SelectedList.Length);

							if(m_Gump != null)
							{
								state.Mobile.CloseGump(typeof(XmlEditDialogGump));
								m_Gump.Refresh(state);
							}
						}
						break;
					}
				}
			}
		}
	}
}
