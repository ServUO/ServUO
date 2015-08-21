using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using Server.Targeting;
using Server.Engines.XmlSpawner2;

/*
** Changelog
**
** 3/25/04
** added party status information
** 3/23/04
** changed bottom border location in the quest status gump for 3dclient compatibility
*/
namespace Server.Gumps
{

	public class XmlConfirmDeleteGump : Gump
	{
		Item m_Item;
		Mobile m_From;

		public XmlConfirmDeleteGump(Mobile from, Item item) : base ( 0, 0 )
		{
			m_Item = item;
			m_From =  from;

			Closable = false;
			Dragable = true;
			AddPage( 0 );
			AddBackground( 10, 185, 200, 130, 5054 );

			if(item is XmlQuestBook)
			{
				AddLabel( 20, 190, 33, String.Format("Delete this questbook?") );
				AddLabel( 20, 210, 33, String.Format("{0} quest(s) will be lost.", item.TotalItems) );
				AddLabel( 20, 230, 53, item.Name );
			} 
			else if(item is IXmlQuest)
			{
				AddLabel( 20, 210, 33, String.Format("Delete this quest?") );
				AddLabel( 20, 230, 53, item.Name );
			} 
			else
			{
				AddLabel( 20, 210, 33, String.Format("Delete this item?") );
				AddLabel( 20, 230, 53, item.Name );
			}
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
					if(radiostate == 1 && m_Item != null )
					{    // accept
						if(m_Item is IXmlQuest)
						{
							((IXmlQuest)m_Item).Invalidate();
						} 
						else
							if(m_Item is XmlQuestBook)
						{
							((XmlQuestBook)m_Item).Invalidate();
						} 
						else
						{
							m_Item.Delete();
						}
					}
					else
						if(m_From != null && m_Item != null && !m_Item.Deleted)
					{
						m_From.AddToBackpack(m_Item);
					}
					break;
				}
			}
		}
	}


	public class XmlSimpleGump : Gump
	{
		public static string Color( string text, string color )
		{
			return String.Format( "<BASEFONT COLOR=#{0}>{1}</BASEFONT>", color, text );
		}
		private int m_gumptype;
		private object m_invoker;
		private BaseXmlSpawner.KeywordTag m_keywordtag;
		private XmlGumpCallback m_gumpcallback;
		private List<GumpSelection> gumpSelections = new List<GumpSelection>();

		private class GumpSelection
		{
			public string Selection;
			public string Response;
			public int GumpItemType; // 1=textentry

			public GumpSelection(string s, string r)
			{
				Selection = s;
				Response = r;
			}
		}

		void LocalAddHtml(string text, int x, int y, int width, int height, int color, bool background, bool scrollbar)
		{
			if (text == null) return;

			// check for cliloc specification
			if (text.StartsWith("#"))
			{
				int cliloc = 0;
				if(int.TryParse(text.Substring(1, text.Length - 1), out cliloc))
					AddHtmlLocalized(x, y, width, height, cliloc, color, background, scrollbar);
			}
			else
			{
				try
				{
					string colorstring = String.Format("{0:X}",color);
					AddHtml(x, y, width, height, XmlSimpleGump.Color(text, colorstring), background, scrollbar);
				}
				catch { }
			}
		}

		private string ParseGumpText(string text)
		{

			string maintext = text;

			// format for multiple selection specifications is 
			// maintext ; selection0 ; response0 ; selection1 ; response1 ....

			string [] args = text.Split(';');

			// the first arg is the maintext
			if(args.Length > 0)
			{
				maintext = args[0];
				// fill the selection and responses with the remaining args
				for(int i = 1;i<args.Length;i += 2)
				{
					GumpSelection s = new GumpSelection("","");
					if(i < args.Length) s.Selection = args[i].Trim();
					if(i+1 < args.Length) s.Response = args[i+1].Trim();

					gumpSelections.Add(s);
				}
			}

			return maintext;
		}

		public XmlSimpleGump( object invoker, string gumptext, string gumptitle, int gumptype, BaseXmlSpawner.KeywordTag tag, Mobile from) : this(  invoker,  gumptext,  gumptitle,  gumptype,  tag, from, null)
		{
			
		}

		public XmlSimpleGump( object invoker, string gumptext, string gumptitle, int gumptype, BaseXmlSpawner.KeywordTag tag, Mobile from, XmlGumpCallback gumpcallback) : base( 0, 0 )
		{

			string maintext = gumptext;
			int nselections = 0;
			int height = 400;
			int width = 369;

			Closable = false;
			Dragable = true;
			m_gumptype = gumptype;

			m_invoker = invoker;
			m_keywordtag = tag;
			m_gumpcallback = gumpcallback;

			AddPage( 0 );


			// for the multiple selection gump, parse the gumptext for selections and responses
			if(gumptype == 4)
			{
				maintext = ParseGumpText(gumptext);
				nselections = gumpSelections.Count;
				height = height + nselections*40;
			}
			if(gumptype == 5)
			{
				maintext = ParseGumpText(gumptext);
				nselections = gumpSelections.Count;
				// the maintext in this case is a width,height specifier so parse it
				string [] args = maintext.Split(',');
				if(args != null && args.Length>1)
				{
					int.TryParse(args[0].Trim(), out width);
					int.TryParse(args[1].Trim(), out height);
				}
			}

			AddImageTiled(  54, 33, width, height, 2624 );
			AddAlphaRegion( 54, 33, width, height );

			AddImageTiled( width + 47, 39, 44, height-11, 203 );

			

			AddImageTiled( 58, 39, 29, height - 10, 10460 ); // left hand border
			AddImageTiled( width + 43, 37, 31, height - 11, 10460 ); // right hand border

			AddImageTiled( 40, 38, 17, height - 9, 9263 ); // leftmost border

			

			//AddImageTiled( 94, 25, width - 27, 15, 10304 );  // top border
			AddImageTiled( 40, 25, width + 48, 15, 10304 );  // top border
			AddImageTiled( 40, height + 27, width + 46, 16, 10304 ); // bottom border

			if(gumptype != 5)
			{
				AddImage( width + 61, 9, 10441); // dragon borders
				AddImage( 6, 25, 10421 );
				AddImage( 34, 12, 10420 );
				AddImage( -10, height - 86, 10402 );
				AddImage( 56, 150, 10411 );

				AddImage( 136, 84, 96 ); // divider
				AddImage( width + 3, 57, 1417); // quest icons
				AddImage( width + 12, 66, 5576);

				AddButton( width - 31, height - 8, 2130, 2129, 3, GumpButtonType.Reply, 0 ); // Okay button
			} 
			else
			{
				AddButton( width + 70, 25, 0x138b, 0x138b, 0, GumpButtonType.Reply, 0 ); // closegump button
			}


			if(gumptitle != null && gumptitle.Length > 0 && gumptype != 5)
			{ // display the title if it is there
				AddImage( 156, 126, 2103 ); // bullet
				LocalAddHtml(gumptitle, 174, 121, 200, 40, 0x00FF42, false, false);
			}

			if(gumptype == 0)
			{ // simple message gump

				LocalAddHtml(maintext, 105, 159, 299, 182, 0xEFEF5A, false, true);

			} else
			if(gumptype == 1)
			{ // Yes/no type gump
				AddRadio( 101, height - 45, 9721, 9724, true, 1 ); // accept/yes radio
				AddRadio( 101, height - 11, 9721, 9724, false, 2 ); // decline/no radio
				AddHtmlLocalized(137, height - 41, 200, 30, 1049016, 0x7fff , false , false ); // Yes
				AddHtmlLocalized(137, height - 7, 200, 30, 1049017, 0x7fff , false , false ); // No

				LocalAddHtml(maintext, 105, 159, 299, 182, 0xEFEF5A, false, true);
			} 
			else
				if(gumptype == 2)
			{ // reply type gump
				AddImageTiled( 134, height - 7, 159, 23, 0x52 );
				AddImageTiled( 135, height - 6, 157, 21, 0xBBC );
				AddHtmlLocalized(105, height - 7, 200, 30, 3002006, 0x7fff , false , false ); // Say:
				AddTextEntry( 135, height - 7, 150, 21, 0, 99, null );

				LocalAddHtml(maintext, 105, 159, 299, 182, 0xEFEF5A, false, true);
			} 
			else
				if(gumptype == 3)
			{ // Quest type gump
				AddImage( 97, 49, 9005 ); // quest ribbon
				AddRadio( 101, height - 45, 9721, 9724, true, 1 ); // accept/yes radio
				AddRadio( 101, height - 11, 9721, 9724, false, 2 ); // decline/no radio
				AddHtmlLocalized( 139, 59, 200, 30, 1046013, 0x7fff, false , false ); // Quest Offer
				AddHtmlLocalized(137, height - 41, 200, 30, 1049011, 0x7fff , false , false ); // I accept!
				AddHtmlLocalized(137, height - 7, 200, 30, 1049012, 0x7fff , false , false ); // No thanks, I decline.

				LocalAddHtml(maintext, 105, 159, 299, 182, 0xEFEF5A, false, true);
			} 
			else
				if(gumptype == 4)
			{ // multiple selection type gump
				// parse the gump text to get the selections and responses

				for(int i=0;i < gumpSelections.Count; i++)
				{
					int y = 360 + i*40;
					AddRadio( 101, y, 9721, 9724, i==0 ? true: false, i ); // accept/yes radio
					AddHtml( 137, y+4, 250, 40, XmlSimpleGump.Color( gumpSelections[i].Selection, "FFFFFF" ), false, false );
				}

				LocalAddHtml(maintext, 105, 159, 299, 182, 0xEFEF5A, false, true);

			} 
			else
				if(gumptype == 5)
			{
				// parse the gump text to get the selections and responses

				for(int i=0;i < gumpSelections.Count; i++)
				{
					string selection = gumpSelections[i].Selection;
					string response = gumpSelections[i].Response;

					int gx = 0;
					int gy = 0;
					int gwidth = 0;
					int gheight = 0;
					string label = null;
					string [] args = null;
					int gumpid = 0;
					int color = 0;

					if(selection != null)
					{
						args = selection.Split(',');
					}

					// process the gumpitem specifications
					if(args.Length > 1)
					{
						for(int j=0;j<args.Length;j++)
						{
							args[j] = args[j].Trim();
						}

						if(args[0].ToLower() == "button")
						{
							// syntax is button,gumpid,x,y
							if(args.Length > 3)
							{
								if(args[1].StartsWith("0x"))
								{
									int.TryParse(args[1].Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out gumpid);
								} 
								else
								{
									int.TryParse(args[1], out gumpid);
								}
								int.TryParse(args[2], out gx);
								int.TryParse(args[3], out gy);

								int buttonid = 1000 + i;

								// add the button
								AddButton( gx, gy, gumpid, gumpid, buttonid, GumpButtonType.Reply, 0 ); 
							}
						} 
						else
							if(args[0].ToLower() == "label")
						{
							// syntax is label,x,y,label[,color]
							if(args.Length > 3)
							{
								int.TryParse(args[1], out gx);
								int.TryParse(args[2], out gy);

								label = args[3];
							}
							// set the default label color
							color = 0x384;
							if(args.Length > 4)
							{
								int.TryParse(args[4], out color);
							}

							// add the label
							AddLabel( gx, gy, color, label );
						} 
						else
							if(args[0].ToLower() == "html")
						{
							// reparse the specification to allow for the possibility of commas in the html text
							args = selection.Split(new char[] {','},6);
							color = 0xEFEF5A;

							// syntax is html,x,y,width,height,text[,hue] * hue has to be in HEX format, ex: 0xFF00AA (lenght of 8 mandatory!)
							if(args.Length > 5)
							{
								int.TryParse(args[1].Trim(), out gx);
								int.TryParse(args[2].Trim(), out gy);
								int.TryParse(args[3].Trim(), out gwidth);
								int.TryParse(args[4].Trim(), out gheight);
								if(args.Length>6 && args[5].StartsWith("0x") && args[5].Trim().Length==8)
								{
									if(!int.TryParse(args[5].Trim().Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out color))
										color=0xEFEF5A;
									label = args[6];
								}
								else
									label = args[5];
							}

							// add the html area
							//AddHtml( gx, gy, gwidth, gheight, label, false, true );
							LocalAddHtml(label, gx, gy, gwidth, gheight, color, false, true);
						}
						else
							if(args[0].ToLower() == "textentry")
						{
							gumpSelections[i].GumpItemType = 1;

							// syntax is textentry,x,y,width,height[,textcolor][,text]
							if(args.Length > 4)
							{
								int.TryParse(args[1].Trim(), out gx);
								int.TryParse(args[2].Trim(), out gy);
								int.TryParse(args[3].Trim(), out gwidth);
								int.TryParse(args[4].Trim(), out gheight);						
							}
							
							if(args.Length > 5)
							{
								label = args[5];
							}

							// set the default textentry color
							color = 0x384;
							if(args.Length > 6)
							{
								int.TryParse(args[6], out color);
							}

							AddTextEntry( gx, gy, gwidth, gheight, color, i, label );
						} 
						else
							if(args[0].ToLower() == "radio")
						{
							int gumpid1 = 0;
							int gumpid2 = 0;

							// syntax is radio,gumpid1,gumpid2,x,y[,initialstate]
							if(args.Length > 4)
							{
								int.TryParse(args[1].Trim(), out gumpid1);
								int.TryParse(args[2].Trim(), out gumpid2);
								int.TryParse(args[3].Trim(), out gx);
								int.TryParse(args[4].Trim(), out gy);
							}

							bool initial = false;
							if(args.Length > 5)
							{
								bool.TryParse(args[5], out initial);
							}

							AddRadio( gx, gy, gumpid1, gumpid2, initial, i);

						}
						else
							if(args[0].ToLower() == "image")
						{
							// syntax is image,gumpid,x,y[,hue]
							if(args.Length > 3)
							{
								if(args[1].StartsWith("0x"))
								{
									int.TryParse(args[1].Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out gumpid);
								} 
								else
								{
									int.TryParse(args[1], out gumpid);
								}
								int.TryParse(args[2], out gx);
								int.TryParse(args[3], out gy);

								if(args.Length > 4)
								{
									int.TryParse(args[4], out color);
								}

								// add the image
								AddImage( gx, gy, gumpid, color );
							}

						} 
						else
							if(args[0].ToLower() == "imagetiled")
						{
							// syntax is imagetiled,gumpid,x,y,width,height
							if(args.Length > 5)
							{
								if(args[1].StartsWith("0x"))
								{
									int.TryParse(args[1].Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out gumpid);
								} 
								else
								{
									int.TryParse(args[1], out gumpid);
								}
								int.TryParse(args[2], out gx);
								int.TryParse(args[3], out gy);
								int.TryParse(args[4], out gwidth);
								int.TryParse(args[5], out gheight);

								// add the image
								AddImageTiled( gx, gy, gwidth, gheight, gumpid );
							}
						} 
						else
							if(args[0].ToLower() == "item")
						{
							// syntax is item,itemid,x,y[,hue]
							if(args.Length > 3)
							{
								if(args[1].StartsWith("0x"))
								{
									int.TryParse(args[1].Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out gumpid);
								} 
								else
								{
									int.TryParse(args[1], out gumpid);
								}
								int.TryParse(args[2], out gx);
								int.TryParse(args[3], out gy);

								if(args.Length > 4)
								{
									int.TryParse(args[4], out color);
								}

								// add the image
								AddItem( gx, gy, gumpid, color );
							}

						}
					}
				}
			}
		}

		public override void OnResponse( Server.Network.NetState state, RelayInfo info )
		{
			if(info == null || state == null || state.Mobile == null) return;
            
			Mobile from = state.Mobile;

			

			if(m_gumpcallback != null)
			{

				if(info.ButtonID == 0)
				{
					m_gumpcallback( from, m_invoker, String.Empty);

				} else

				switch(m_gumptype)
				{
					case 0:	// simple acknowledgement gump
						m_gumpcallback( from, m_invoker, "done");
						break;
					case 1:				// yes/no gump
						if(info.Switches != null && info.Switches.Length > 0)
						{
							if ( info.Switches[0] == 1 )
							{
								m_gumpcallback( from, m_invoker, "yes");
							} 
							else
							{
								m_gumpcallback( from, m_invoker, "no");
							}
						}
						break;
					case 2: // text entry gump
						TextRelay entry = info.GetTextEntry( 99 );
						if ( entry != null && entry.Text.Length > 0 )
						{
							// return the response string
							m_gumpcallback( from, m_invoker, entry.Text);
						}
						break;
					case 3: // accept/decline gump
						if(info.Switches != null && info.Switches.Length > 0)
						{
							if ( info.Switches[0] == 1 )
							{
								from.SendLocalizedMessage( 1049019 ); // You have accepted the Quest.

								m_gumpcallback( from, m_invoker, "accept");
							} 
							else
							{
								from.SendLocalizedMessage( 1049018 ); // You have declined the Quest.

								m_gumpcallback( from, m_invoker, "decline");

							}
						}
						break;
					case 4: // multiple option gump
						if(info.Switches != null && info.Switches.Length > 0)
						{
							int select = info.Switches[0];

							if(select >= 0 && select < gumpSelections.Count)
							{
								// return the response string for that selection
								m_gumpcallback( from, m_invoker, gumpSelections[select].Response);
							}
						}
						break;
					case 5:

						string buttonresponse = String.Empty;
						string radioresponse = String.Empty;
						string textresponse = String.Empty;

						if(info.ButtonID >= 1000)
						{
							int select = info.ButtonID - 1000;
							// get the gump response associated with the button
							if(select >= 0 && select < gumpSelections.Count)
							{
								// return the response string for that selection
								buttonresponse = gumpSelections[select].Response;
							}
						}

						if(info.Switches != null && info.Switches.Length > 0)
						{
							int radiostate = info.Switches[0];

							if(radiostate >= 0 && radiostate < gumpSelections.Count)
							{
								radioresponse = gumpSelections[radiostate].Response;
							}
						}

						// check for any textentries
						for(int j=0;j<gumpSelections.Count;j++)
						{
							if(gumpSelections[j].GumpItemType == 1)
							{
								try
								{
									TextRelay te = info.GetTextEntry( j );
									if ( te != null && te.Text.Length > 0 )
									{
										textresponse += te.Text + " ";
									}
								}
								catch {}
							}
						}

						// build the composite reponse string
						string responsestring = null;
						if(buttonresponse != null && buttonresponse.Length > 0)
						{
							responsestring = buttonresponse;
						}
						if(radioresponse != null && radioresponse.Length > 0)
						{
							responsestring += " " + radioresponse;
						}
						if(textresponse != null && textresponse.Length > 0)
						{
							responsestring += " " + textresponse;
						}

						m_gumpcallback( from, m_invoker, responsestring);
						break;
				}
			}
			// get rid of any temporary gump keyword tokens
			if(m_invoker is XmlSpawner)
				((XmlSpawner)m_invoker).DeleteTag(m_keywordtag);
		}
	}
}

