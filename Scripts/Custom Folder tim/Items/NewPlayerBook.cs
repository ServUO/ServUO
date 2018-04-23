/*
 * Copyright (c) 2005, Kai Sassmannshausen <kai@sassie.org>
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * - Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *
 * - Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the
 * distribution.
 *
 * - Neither the name of Kai Sassmannshausen, nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,
 * BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * NewPlayerBook and NewPlayerGump
 *
 */

using System;
using System.Collections;
using System.Net;
using Server.Network;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Regions;

namespace Server.Items
{
                public class NewPlayerBook : Item
	        {

		[Constructable]
		public NewPlayerBook() : base( 7187 )
		{
			Movable = true;
			Hue = 0x422;
			Name = "a new player book";
			LootType = LootType.Newbied;
		}

		public NewPlayerBook(Serial serial) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

		}

		public override void OnDoubleClick(Mobile from)
		{

		  if (from == null || from.Deleted || from.Backpack == null || from.Backpack.Deleted )
		    return;

		  PlayerMobile m_PMobile = (PlayerMobile)from;


		  if ( !this.IsChildOf(from.Backpack ))
		  {
			from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			return;
		  }

		  if ( !m_PMobile.Young )
		  {
			from.SendMessage("Only young players are allowed to use and have this.");
			this.Delete();
			return;
		  }

		  from.CloseGump(typeof(NewPlayerGump));
		  from.SendGump(new NewPlayerGump(from));
		}
	}
}

namespace Server.Gumps
{
	public class NewPlayerGump : Gump
	{

		private Mobile m_From;

		private int HeadlineColor = 442; //33;
		private int LinkColor = 54;
		private int TextColor = 248; //902;
		private int BookPage = 0;

		// -- Teleport Locations
		// Elder Forest
		private Point3D loc1 = new Point3D(5530,1629,0);
		private Map loc1map = Map.Felucca;

		// Haven
		private Point3D loc2 = new Point3D(3577,2590,0);
		private Map loc2map = Map.Felucca;

		// Britain Inn
		private Point3D loc3 = new Point3D(1429,1682,20);
		private Map loc3map = Map.Felucca;


		public NewPlayerGump(Mobile from) : base (20, 30)
		{
			m_From = from;
			constructGump(m_From);
		}


		public NewPlayerGump(Mobile from, int page) : base (20, 30)
		{
			m_From = from;
			BookPage = page;
			constructGump(m_From);
		}


	        private void constructGump(Mobile from)
		{
			PlayerMobile m_PMobile = (PlayerMobile)from;

			if (from == null || from.Deleted || !m_PMobile.Young)
				return;

			int y = 0;
			int x = 0;

			AddBackground();

			switch (BookPage)
			{
			case 0:

				AddButton(456, 10, 0x1F6, 0x1F6, 1, GumpButtonType.Reply, 0);

				// Left side of page 1
				y+=30;
				x+=138;

				AddLabel(170, y, HeadlineColor, "Welcome to the");
				y+=15;
				AddLabel(160, y, HeadlineColor, "Defiance Networks!");

				y+= 55;
				AddLabel(x, y, TextColor,     "Please read the following");
				y+=15;
				AddLabel(x, y, TextColor,     "text carefully and complete");
				y+=15;
				AddLabel(x, y, TextColor,     "the steps so that your");
				y+=15;
				AddLabel(x, y, TextColor,     "account is insured.");

				// Right side of page 1
				y= 20;
				x= 325;

				AddLabel(x+15, y, HeadlineColor, "Accounting stuff");
				y+= 30;

				AddLabel(x, y, TextColor,       "1. Visit our homepage");
				y+=15;
				AddLabel(x+20, y, LinkColor,    "www.defianceuo.com");
				y+=15;
				AddLabel(x, y, TextColor,       "and set a magicword for");
				y+=15;
				AddLabel(x, y, TextColor,	"your account under the");
				y+=15;
				AddLabel(x, y, TextColor,	"acc management section.");

				y+=35;
				AddLabel(x, y, TextColor,	"2. Visit our forums at");
				y+=15;
				AddLabel(x+2, y, LinkColor,    "www.defianceuo.com/forums");
				y+=15;
				AddLabel(x, y, TextColor,	"and register a account");
				y+=15;
				AddLabel(x, y, TextColor,	"there.");

				break;

			case 1:

				AddButton(100, 10, 0x1F5, 0x1F5, 0, GumpButtonType.Reply, 0);
				AddButton(456, 10, 0x1F6, 0x1F6, 2, GumpButtonType.Reply, 0);

				// Left side of page 2
				y+= 20;
				x+= 145;

				AddLabel(x+30,y, HeadlineColor, "Starting Location");
				y+= 35;

				AddLabel(x, y, TextColor,       "3. Choose your starting");
				y+=15;
				AddLabel(x, y, TextColor,       "location based on your");
				y+=15;
				AddLabel(x, y, TextColor,	"personal experiance with");
				y+=15;
				AddLabel(x, y, TextColor,	"Ultima Online:");
				y+=35;
				//AddButton(x+15, y+3, 0x4B9, 0x4BA, 11, GumpButtonType.Reply, 0);
				//AddLabel (x+45, y, LinkColor,    "Elder Forest");
				//y+=25;
				//AddButton(x+15, y+3, 0x4B9, 0x4BA, 12, GumpButtonType.Reply, 0);
				//AddLabel (x+45, y, LinkColor,    "Haven");
				//y+=25;
				AddButton(x+15, y+3, 0x4B9, 0x4BA, 13, GumpButtonType.Reply, 0);
				AddLabel (x+45, y, LinkColor,    "Britain City");


				// Right side of page 2
				x = 325;
				y = 22;

				//AddLabel(x, y, LinkColor,	"Elder Forest");
				//y+= 15;
				//AddLabel(x, y, TextColor,	"The Ultima Online Tutorial.");
				//y+= 15;
				//AddLabel(x, y, TextColor,	"Here can you learn some");
				//y+= 15;
				//AddLabel(x, y, TextColor,	"basics. Just take a look.");

				y+= 24;

				//AddLabel(x, y, LinkColor,	"Haven");
				//y+= 15;
				//AddLabel(x, y, TextColor,	"The Beginners Quest.");
				//y+= 15;
				//AddLabel(x, y, TextColor,	"Gain your first experiance");
				//y+= 15;
				//AddLabel(x, y, TextColor,	"with some action.");

				y+= 24;

				AddLabel(x, y, LinkColor,	"Britain");
				y+= 15;
				AddLabel(x, y, TextColor,	"The Capital City.");
				y+= 15;
				AddLabel(x, y, TextColor,	"Adventure throu the lands");
				y+= 15;
				AddLabel(x, y, TextColor,	"of Defiance.");

				break;

			case 2:

				AddButton(100, 10, 0x1F5, 0x1F5, 1, GumpButtonType.Reply, 0);

				// Left side of page 3

				x = 135;
				y = 20;

				AddLabel(x+35, y, HeadlineColor,"Donation Center");
				y+= 30;

				AddLabel(x, y, LinkColor,	"www.defianceuo.com/donate");
				y+= 30;
				AddLabel(x, y, TextColor,	"Become a Donator and get");
				y+= 15;
				AddLabel(x, y, TextColor,	"access to special dungeons");
				y+= 15;
				AddLabel(x, y, TextColor,	"and events. It also contains");
				y+= 15;
				AddLabel(x, y, TextColor,	"Items, character extentions,");
				y+= 15;
				AddLabel(x, y, TextColor,	"custom houses, etc...");
				y+= 30;
				AddLabel(x, y, 147,		"Note that every donation");
				y+= 15;
				AddLabel(x, y, 147,		"helpes to keep DFI online!");


				// Right side of page 3

				x = 325;
				y = 20;

				AddLabel(x+25,y, HeadlineColor, "Additional Service");
				y+= 30;

				AddLabel(x, y, LinkColor,	"Defiance Client Starter");
				y+= 30;
				AddLabel(x, y, TextColor,	"With this tool you can");
				y+= 15;
				AddLabel(x, y, TextColor,	"easy start your UO client");
				y+= 15;
				AddLabel(x, y, TextColor,	"to play on Defiance. It also");
				y+= 15;
				AddLabel(x, y, TextColor,	"shows you the latest news.");
				y+= 25;
				AddLabel(x, y, LinkColor,	"Download available at the");
				y+= 15;
				AddLabel(x, y, LinkColor,	"Defiance Homepage.");
				y+= 35;
				AddButton(x+30, y+3, 0x4B9, 0x4BA, 20, GumpButtonType.Reply, 0);
				AddLabel(x+50, y, 147,		"Close this book");

				break;
			}

		}


		private void AddBackground()
		{
			AddImage(100, 10, 500);
		}

        private bool CanYoungTeleport(PlayerMobile pm)
        {
            if (pm.Region is Server.Regions.Jail || !pm.Region.CanUseStuckMenu(pm))
            {
                pm.SendMessage("You cannot use the young player teleport here.");
                return false;
            }
            return true;
        }

		public override void OnResponse(NetState state, RelayInfo info)
		{
			NetState sender = state;
			m_From = state.Mobile;
			PlayerMobile m_PMobile = (PlayerMobile)m_From;

			if ( m_From == null || m_From.Deleted || !m_PMobile.Young || info.ButtonID < 0 )
				return;

			switch (info.ButtonID)
			{

			case 0:
			case 1:
			case 2:
				sender.Mobile.SendGump(new NewPlayerGump( sender.Mobile, info.ButtonID ));
				break;
			case 11:
				if (CanYoungTeleport(m_PMobile)) m_From.MoveToWorld(loc1, loc1map);
    		    sender.Mobile.SendGump(new NewPlayerGump( sender.Mobile, 2 ));
				break;
			case 12:
				if (CanYoungTeleport(m_PMobile)) m_From.MoveToWorld(loc2, loc2map);
				sender.Mobile.SendGump(new NewPlayerGump( sender.Mobile, 2 ));
				break;
			case 13:
                if (CanYoungTeleport(m_PMobile)) m_From.MoveToWorld(loc3, loc3map);
                sender.Mobile.SendGump(new NewPlayerGump(sender.Mobile, 2));
				break;
			}

			return;

		}

	}
}