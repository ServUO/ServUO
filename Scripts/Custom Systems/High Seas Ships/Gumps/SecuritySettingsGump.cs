using System;
using System.Collections;
using System.Collections.Generic;

using Server;
using Server.Engines;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;

namespace Server.Multis
{

    public enum SecuritySettingsGumpPage
    {
		Default,
		Public,
		Party,
		Guild,
		AccessListDefault,
		AccessListPlayer,
		AccessListPage
    }

    public class SecuritySettingsGump : Gump
    {	
	
        private BaseGalleon _falleon;
		private SecuritySettingsGumpPage _page;
		private Dictionary<int, PlayerMobile> _playersAboard;

        private const int LabelColor = 0x7FFF;
        private const int SelectedColor = 0x421F;
        private const int DisabledColor = 0x4210;
        private const int WarningColor = 0x7E10;

        private const int LabelHue = 0x481;
        private const int HighlightedLabelHue = 0x64;
		
		private int _currentAccessListPage;
		
		private PlayerMobile _selectedPlayer;

        private string GetOwnerName()
        {
            Mobile m = _falleon.Owner;

            if (m == null || m.Deleted)
                return "(unowned)";

            string name;

            if ((name = m.Name) == null || (name = name.Trim()).Length <= 0)
                name = "(no name)";

            return name;
        }

        public int CurrentAccessListPage{ get{ return _currentAccessListPage; } }
		
		public Dictionary<int, PlayerMobile> PlayersAboard{ get{ return _playersAboard; } }
		
        public int GetButtonID(int type, int index)
        {
            return 1 + (index * 15) + type;
        }	

        public void AddButtonLabeled(int x, int y, int buttonID, int number)
        {
            AddButtonLabeled(x, y, buttonID, number, true);
        }

        public void AddButtonLabeled(int x, int y, int buttonID, int number, bool enabled)
        {
            if (enabled)
                AddButton(x, y, 4005, 4007, buttonID, GumpButtonType.Reply, 0);

            AddHtmlLocalized(x + 35, y, 240, 20, number, enabled ? LabelColor : DisabledColor, false, false);
        }		

		public SecuritySettingsGump(SecuritySettingsGumpPage page, Mobile from, BaseGalleon galleon, Dictionary<int, PlayerMobile> playersAboard, int currentAccessListPage, PlayerMobile selectedPlayer) 
			: base(50, 40)		
		{
            _falleon = galleon;
			_page = page;
			_playersAboard = playersAboard;
			_currentAccessListPage = currentAccessListPage;
			_selectedPlayer = selectedPlayer;

            from.CloseGump(typeof(SecuritySettingsGump));

			bool isOwner = false;
			if (from == _falleon.Owner)
				isOwner = true;
			
			if (!isOwner)
			{
				if (_falleon.CanModifySecurity == 0)
					return;
				else if (_falleon.CanModifySecurity == 1)
					if (from.Party != _falleon.Owner.Party)
						return;
					else if (((Party)(_falleon.Owner.Party)).Leader != _falleon.Owner)
							return;
				else if (_falleon.CanModifySecurity == 2)
					if (from.Party != _falleon.Owner.Party)
						return;
			}
			
			AddPage(0);	
				
			AddImageTiled(0, 0, 20, 18, 0xA3C);
			AddImageTiled(20, 0, 270, 18, 0xA3D);
			AddImageTiled(286, 0, 20, 18, 0xA3E);
			AddImageTiled(0, 18, 22, 210, 0xA3F);
			AddImageTiled(20, 18, 266, 206, 0xA40);
			AddImageTiled(286, 18, 20, 210, 0xA41);
			AddImageTiled(0, 224, 22, 210, 0xA3F);
			AddImageTiled(20, 224, 266, 206, 0xA40);
			AddImageTiled(286, 224, 20, 210, 0xA41);
			AddImageTiled(0, 430, 20, 18, 0xA42);
			AddImageTiled(20, 430, 270, 18, 0xA43);
			AddImageTiled(286, 430, 20, 18, 0xA44);
			
			AddLabel(85, 20, 53, "Passenger and Crew Manifest");
			AddLabel(10, 50, LabelHue, String.Format("Ship: {0}", (_falleon.ShipName != null)?_falleon.ShipName:"unnamed ship"));
			AddLabel(10, 70, LabelHue, String.Format("Owner: {0}", _falleon.Owner.Name));

			if ((page == SecuritySettingsGumpPage.Default) || (page == SecuritySettingsGumpPage.Public) || (page == SecuritySettingsGumpPage.Party) || (page == SecuritySettingsGumpPage.Guild))
			{
			
			
				AddLabel(10, 100, LabelHue, String.Format("Party membership modifies access to this ship: "));

				
				switch (_falleon.CanModifySecurity)
				{
					case (0):
						{
							AddButton(60, 120, 0xFA6, 0xFA6, GetButtonID(0, 0), GumpButtonType.Reply, 0); 
							AddHtmlLocalized(95, 120, 240, 20, 1149778, LabelColor, false, false);// Never
							AddButton(60, 140, 0xFA5, 0xFA6, GetButtonID(0, 1), GumpButtonType.Reply, 0); 
							AddHtmlLocalized(95, 140, 240, 20, 1149744, LabelColor, false, false);// When I am a Party Leader
							AddButton(60, 160, 0xFA5, 0xFA6, GetButtonID(0, 2), GumpButtonType.Reply, 0); 
							AddHtmlLocalized(95, 160, 240, 20, 1149745, LabelColor, false, false);// When I am a Party Member
								
							break;					
						}
						
					case (1):
						{
							AddButton(60, 120, 0xFA5, 0xFA6, GetButtonID(0, 0), GumpButtonType.Reply, 0); 
							AddHtmlLocalized(95, 120, 240, 20, 1149778, LabelColor, false, false);// Never
							AddButton(60, 140, 0xFA6, 0xFA6, GetButtonID(0, 1), GumpButtonType.Reply, 0); 
							AddHtmlLocalized(95, 140, 240, 20, 1149744, LabelColor, false, false);// When I am a Party Leader
							AddButton(60, 160, 0xFA5, 0xFA6, GetButtonID(0, 2), GumpButtonType.Reply, 0); 
							AddHtmlLocalized(95, 160, 240, 20, 1149745, LabelColor, false, false);// When I am a Party Member
								
							break;
						}
						
					case (2):
						{
							AddButton(60, 120, 0xFA5, 0xFA6, GetButtonID(0, 0), GumpButtonType.Reply, 0); 
							AddHtmlLocalized(95, 120, 240, 20, 1149778, LabelColor, false, false);// Never
							AddButton(60, 140, 0xFA5, 0xFA6, GetButtonID(0, 1), GumpButtonType.Reply, 0); 
							AddHtmlLocalized(95, 140, 240, 20, 1149744, LabelColor, false, false);// When I am a Party Leader
							AddButton(60, 160, 0xFA6, 0xFA6, GetButtonID(0, 2), GumpButtonType.Reply, 0); 
							AddHtmlLocalized(95, 160, 240, 20, 1149745, LabelColor, false, false);// When I am a Party Member

							break;
						}
				}	

				AddLabel(10, 180, LabelHue, String.Format("Public Access: "));			
				
				if (page == SecuritySettingsGumpPage.Public)
					AddButton(120, 180, 0xFA6, 0xFA6, GetButtonID(1, 0), GumpButtonType.Reply, 0);
				else
					AddButton(120, 180, 0xFA5, 0xFA6, GetButtonID(1, 0), GumpButtonType.Reply, 0);
				
				switch (_falleon.Public)
				{
					case (0):
						{
							AddLabel(155, 180, 906, String.Format("N/A"));
								
							break;					
						}
						
					case (1):
						{
							AddLabel(155, 180, 98, String.Format("PASSENGER"));
								
							break;
						}
						
					case (2):
						{
							AddLabel(155, 180, 68, String.Format("CREW"));

							break;
						}
						
					case (3):
						{
							AddLabel(155, 180, 53, String.Format("OFFICER"));

							break;
						}
		
					case (4):
						{
							AddLabel(155, 180, 906, String.Format("CAPTAIN"));

							break;
						}
						
					case (5):
						{
							AddLabel(155, 180, 38, String.Format("DENY ACCESS"));

							break;
						}
				}
				
				AddLabel(10, 200, LabelHue, String.Format("Party Access: "));		

				if (page == SecuritySettingsGumpPage.Party)
					AddButton(120, 200, 0xFA6, 0xFA6, GetButtonID(1, 1), GumpButtonType.Reply, 0);
				else
					AddButton(120, 200, 0xFA5, 0xFA6, GetButtonID(1, 1), GumpButtonType.Reply, 0);			
				
				switch (_falleon.Party)
				{
					case (0):
						{
							AddLabel(155, 200, 906, String.Format("N/A"));
								
							break;					
						}
						
					case (1):
						{
							AddLabel(155, 200, 98, String.Format("PASSENGER"));
								
							break;
						}
						
					case (2):
						{
							AddLabel(155, 200, 68, String.Format("CREW"));

							break;
						}
						
					case (3):
						{
							AddLabel(155, 200, 53, String.Format("OFFICER"));

							break;
						}
		
					case (4):
						{
							AddLabel(155, 200, 906, String.Format("CAPTAIN"));

							break;
						}
						
					case (5):
						{
							AddLabel(155, 200, 38, String.Format("DENY ACCESS"));

							break;
						}
				}
							
				AddLabel(10, 220, LabelHue, String.Format("Guild Access: "));

				if (page == SecuritySettingsGumpPage.Guild)
					AddButton(120, 220, 0xFA6, 0xFA6, GetButtonID(1, 2), GumpButtonType.Reply, 0); 
				else
					AddButton(120, 220, 0xFA5, 0xFA6, GetButtonID(1, 2), GumpButtonType.Reply, 0); 
					
				switch (_falleon.Guild)
				{
					case (0):
						{
							AddLabel(155, 220, 906, String.Format("N/A"));
								
							break;					
						}
						
					case (1):
						{
							AddLabel(155, 220, 98, String.Format("PASSENGER"));
								
							break;
						}
						
					case (2):
						{
							AddLabel(155, 220, 68, String.Format("CREW"));

							break;
						}
						
					case (3):
						{
							AddLabel(155, 220, 53, String.Format("OFFICER"));

							break;
						}
		
					case (4):
						{
							AddLabel(155, 220, 906, String.Format("CAPTAIN"));

							break;
						}
						
					case (5):
						{
							AddLabel(155, 220, 38, String.Format("DENY ACCESS"));

							break;
						}
				}			
				
				switch ( page )
				{	
					case SecuritySettingsGumpPage.Public:
						{	
							switch (_falleon.Public)
							{
								case (0):
									{
										AddLabel(20, 260, LabelHue, String.Format("Public Access: "));
										AddButton(20, 280, 0xFA6, 0xFA6, GetButtonID(2, 0), GumpButtonType.Reply, 0); 
										AddLabel(55, 280, LabelHue, "N/A");
										AddButton(20, 300, 0xFA5, 0xFA6, GetButtonID(2, 1), GumpButtonType.Reply, 0); 
										AddLabel(55, 300, LabelHue, "PASSENGER");
										AddButton(20, 320, 0xFA5, 0xFA6, GetButtonID(2, 2), GumpButtonType.Reply, 0); 
										AddLabel(55, 320, LabelHue, "CREW");
										AddButton(20, 340, 0xFA5, 0xFA6, GetButtonID(2, 3), GumpButtonType.Reply, 0); 
										AddLabel(55, 340, LabelHue, "OFFICER");
										AddButton(20, 360, 0xFA5, 0xFA6, GetButtonID(2, 4), GumpButtonType.Reply, 0); 
										AddLabel(55, 360, LabelHue, "DENY ACCESS");
											
										break;					
									}
									
								case (1):
									{
										AddLabel(20, 260, LabelHue, String.Format("Public Access: "));
										AddButton(20, 280, 0xFA5, 0xFA6, GetButtonID(2, 0), GumpButtonType.Reply, 0); 
										AddLabel(55, 280, LabelHue, "N/A");
										AddButton(20, 300, 0xFA6, 0xFA6, GetButtonID(2, 1), GumpButtonType.Reply, 0); 
										AddLabel(55, 300, 98, "PASSENGER");
										AddButton(20, 320, 0xFA5, 0xFA6, GetButtonID(2, 2), GumpButtonType.Reply, 0); 
										AddLabel(55, 320, LabelHue, "CREW");
										AddButton(20, 340, 0xFA5, 0xFA6, GetButtonID(2, 3), GumpButtonType.Reply, 0); 
										AddLabel(55, 340, LabelHue, "OFFICER");
										AddButton(20, 360, 0xFA5, 0xFA6, GetButtonID(2, 4), GumpButtonType.Reply, 0); 
										AddLabel(55, 360, LabelHue, "DENY ACCESS");
											
										break;
									}
									
								case (2):
									{
										AddLabel(20, 260, LabelHue, String.Format("Public Access: "));
										AddButton(20, 280, 0xFA5, 0xFA6, GetButtonID(2, 0), GumpButtonType.Reply, 0); 
										AddLabel(55, 280, LabelHue, "N/A");
										AddButton(20, 300, 0xFA5, 0xFA6, GetButtonID(2, 1), GumpButtonType.Reply, 0); 
										AddLabel(55, 300, LabelHue, "PASSENGER");
										AddButton(20, 320, 0xFA6, 0xFA6, GetButtonID(2, 2), GumpButtonType.Reply, 0); 
										AddLabel(55, 320, 68, "CREW");
										AddButton(20, 340, 0xFA5, 0xFA6, GetButtonID(2, 3), GumpButtonType.Reply, 0); 
										AddLabel(55, 340, LabelHue, "OFFICER");
										AddButton(20, 360, 0xFA5, 0xFA6, GetButtonID(2, 4), GumpButtonType.Reply, 0); 
										AddLabel(55, 360, LabelHue, "DENY ACCESS");

										break;
									}
									
								case (3):
									{
										AddLabel(20, 260, LabelHue, String.Format("Public Access: "));
										AddButton(20, 280, 0xFA5, 0xFA6, GetButtonID(2, 0), GumpButtonType.Reply, 0); 
										AddLabel(55, 280, LabelHue, "N/A");
										AddButton(20, 300, 0xFA5, 0xFA6, GetButtonID(2, 1), GumpButtonType.Reply, 0); 
										AddLabel(55, 300, LabelHue, "PASSENGER");
										AddButton(20, 320, 0xFA5, 0xFA6, GetButtonID(2, 2), GumpButtonType.Reply, 0); 
										AddLabel(55, 320, LabelHue, "CREW");
										AddButton(20, 340, 0xFA6, 0xFA6, GetButtonID(2, 3), GumpButtonType.Reply, 0); 
										AddLabel(55, 340, 53, "OFFICER");
										AddButton(20, 360, 0xFA5, 0xFA6, GetButtonID(2, 4), GumpButtonType.Reply, 0); 
										AddLabel(55, 360, LabelHue, "DENY ACCESS");

										break;
									}
					
								case (4):
									{
										AddLabel(20, 260, LabelHue, String.Format("Public Access: "));
										AddButton(20, 280, 0xFA5, 0xFA6, GetButtonID(2, 0), GumpButtonType.Reply, 0); 
										AddLabel(55, 280, LabelHue, "N/A");
										AddButton(20, 300, 0xFA5, 0xFA6, GetButtonID(2, 1), GumpButtonType.Reply, 0); 
										AddLabel(55, 300, LabelHue, "PASSENGER");
										AddButton(20, 320, 0xFA5, 0xFA6, GetButtonID(2, 2), GumpButtonType.Reply, 0); 
										AddLabel(55, 320, LabelHue, "CREW");
										AddButton(20, 340, 0xFA5, 0xFA6, GetButtonID(2, 3), GumpButtonType.Reply, 0); 
										AddLabel(55, 340, LabelHue, "OFFICER");
										AddButton(20, 360, 0xFA5, 0xFA6, GetButtonID(2, 4), GumpButtonType.Reply, 0); 
										AddLabel(55, 360, LabelHue, "DENY ACCESS");

										break;
									}
									
								case (5):
									{
										AddLabel(20, 260, LabelHue, String.Format("Public Access: "));
										AddButton(20, 280, 0xFA5, 0xFA6, GetButtonID(2, 0), GumpButtonType.Reply, 0); 
										AddLabel(55, 280, LabelHue, "N/A");
										AddButton(20, 300, 0xFA5, 0xFA6, GetButtonID(2, 1), GumpButtonType.Reply, 0); 
										AddLabel(55, 300, LabelHue, "PASSENGER");
										AddButton(20, 320, 0xFA5, 0xFA6, GetButtonID(2, 2), GumpButtonType.Reply, 0); 
										AddLabel(55, 320, LabelHue, "CREW");
										AddButton(20, 340, 0xFA5, 0xFA6, GetButtonID(2, 3), GumpButtonType.Reply, 0); 
										AddLabel(55, 340, LabelHue, "OFFICER");
										AddButton(20, 360, 0xFA6, 0xFA6, GetButtonID(2, 4), GumpButtonType.Reply, 0); 
										AddLabel(55, 360, 38, "DENY ACCESS");

										break;
									}
							}																																
							break;
						}									
						
					case SecuritySettingsGumpPage.Party:
						{

							switch (_falleon.Party)
							{
								case (0):
									{
										AddLabel(80, 260, LabelHue, String.Format("Party Access: "));
										AddButton(80, 280, 0xFA6, 0xFA6, GetButtonID(3, 0), GumpButtonType.Reply, 0); 
										AddLabel(115, 280, LabelHue, "N/A");
										AddButton(80, 300, 0xFA5, 0xFA6, GetButtonID(3, 1), GumpButtonType.Reply, 0); 
										AddLabel(115, 300, LabelHue, "PASSENGER");
										AddButton(80, 320, 0xFA5, 0xFA6, GetButtonID(3, 2), GumpButtonType.Reply, 0); 
										AddLabel(115, 320, LabelHue, "CREW");
										AddButton(80, 340, 0xFA5, 0xFA6, GetButtonID(3, 3), GumpButtonType.Reply, 0); 
										AddLabel(115, 340, LabelHue, "OFFICER");
										AddButton(80, 360, 0xFA5, 0xFA6, GetButtonID(3, 4), GumpButtonType.Reply, 0); 
										AddLabel(115, 360, LabelHue, "DENY ACCESS");

											
										break;					
									}
									
								case (1):
									{
										AddLabel(80, 260, LabelHue, String.Format("Party Access: "));
										AddButton(80, 280, 0xFA5, 0xFA6, GetButtonID(3, 0), GumpButtonType.Reply, 0); 
										AddLabel(115, 280, LabelHue, "N/A");
										AddButton(80, 300, 0xFA6, 0xFA6, GetButtonID(3, 1), GumpButtonType.Reply, 0); 
										AddLabel(115, 300, 98, "PASSENGER");
										AddButton(80, 320, 0xFA5, 0xFA6, GetButtonID(3, 2), GumpButtonType.Reply, 0); 
										AddLabel(115, 320, LabelHue, "CREW");
										AddButton(80, 340, 0xFA5, 0xFA6, GetButtonID(3, 3), GumpButtonType.Reply, 0); 
										AddLabel(115, 340, LabelHue, "OFFICER");
										AddButton(80, 360, 0xFA5, 0xFA6, GetButtonID(3, 4), GumpButtonType.Reply, 0); 
										AddLabel(115, 360, LabelHue, "DENY ACCESS");

											
										break;
									}
									
								case (2):
									{
										AddLabel(80, 260, LabelHue, String.Format("Party Access: "));
										AddButton(80, 280, 0xFA5, 0xFA6, GetButtonID(3, 0), GumpButtonType.Reply, 0); 
										AddLabel(115, 280, LabelHue, "N/A");
										AddButton(80, 300, 0xFA5, 0xFA6, GetButtonID(3, 1), GumpButtonType.Reply, 0); 
										AddLabel(115, 300, LabelHue, "PASSENGER");
										AddButton(80, 320, 0xFA6, 0xFA6, GetButtonID(3, 2), GumpButtonType.Reply, 0); 
										AddLabel(115, 320, 68, "CREW");
										AddButton(80, 340, 0xFA5, 0xFA6, GetButtonID(3, 3), GumpButtonType.Reply, 0); 
										AddLabel(115, 340, LabelHue, "OFFICER");
										AddButton(80, 360, 0xFA5, 0xFA6, GetButtonID(3, 4), GumpButtonType.Reply, 0); 
										AddLabel(115, 360, LabelHue, "DENY ACCESS");


										break;
									}
									
								case (3):
									{
										AddLabel(80, 260, LabelHue, String.Format("Party Access: "));
										AddButton(80, 280, 0xFA5, 0xFA6, GetButtonID(3, 0), GumpButtonType.Reply, 0); 
										AddLabel(115, 280, LabelHue, "N/A");
										AddButton(80, 300, 0xFA5, 0xFA6, GetButtonID(3, 1), GumpButtonType.Reply, 0); 
										AddLabel(115, 300, LabelHue, "PASSENGER");
										AddButton(80, 320, 0xFA5, 0xFA6, GetButtonID(3, 2), GumpButtonType.Reply, 0); 
										AddLabel(115, 320, LabelHue, "CREW");
										AddButton(80, 340, 0xFA6, 0xFA6, GetButtonID(3, 3), GumpButtonType.Reply, 0); 
										AddLabel(115, 340, 53, "OFFICER");
										AddButton(80, 360, 0xFA5, 0xFA6, GetButtonID(3, 4), GumpButtonType.Reply, 0); 
										AddLabel(115, 360, LabelHue, "DENY ACCESS");


										break;
									}
					
								case (4):
									{
										AddLabel(80, 260, LabelHue, String.Format("Party Access: "));
										AddButton(80, 280, 0xFA5, 0xFA6, GetButtonID(3, 0), GumpButtonType.Reply, 0); 
										AddLabel(115, 280, LabelHue, "N/A");
										AddButton(80, 300, 0xFA5, 0xFA6, GetButtonID(3, 1), GumpButtonType.Reply, 0); 
										AddLabel(115, 300, LabelHue, "PASSENGER");
										AddButton(80, 320, 0xFA5, 0xFA6, GetButtonID(3, 2), GumpButtonType.Reply, 0); 
										AddLabel(115, 320, LabelHue, "CREW");
										AddButton(80, 340, 0xFA5, 0xFA6, GetButtonID(3, 3), GumpButtonType.Reply, 0); 
										AddLabel(115, 340, LabelHue, "OFFICER");
										AddButton(80, 360, 0xFA5, 0xFA6, GetButtonID(3, 4), GumpButtonType.Reply, 0); 
										AddLabel(115, 360, LabelHue, "DENY ACCESS");


										break;
									}
									
								case (5):
									{
										AddLabel(80, 260, LabelHue, String.Format("Party Access: "));
										AddButton(80, 280, 0xFA5, 0xFA6, GetButtonID(3, 0), GumpButtonType.Reply, 0); 
										AddLabel(115, 280, LabelHue, "N/A");
										AddButton(80, 300, 0xFA5, 0xFA6, GetButtonID(3, 1), GumpButtonType.Reply, 0); 
										AddLabel(115, 300, LabelHue, "PASSENGER");
										AddButton(80, 320, 0xFA5, 0xFA6, GetButtonID(3, 2), GumpButtonType.Reply, 0); 
										AddLabel(115, 320, LabelHue, "CREW");
										AddButton(80, 340, 0xFA5, 0xFA6, GetButtonID(3, 3), GumpButtonType.Reply, 0); 
										AddLabel(115, 340, LabelHue, "OFFICER");
										AddButton(80, 360, 0xFA6, 0xFA6, GetButtonID(3, 4), GumpButtonType.Reply, 0); 
										AddLabel(115, 360, 38, "DENY ACCESS");

										break;
									}
							}										
							break;
						}
						
					case SecuritySettingsGumpPage.Guild:
						{
							switch (_falleon.Guild)
							{
								case (0):
									{
										AddLabel(140, 260, LabelHue, String.Format("Guild Access: "));
										AddButton(140, 280, 0xFA6, 0xFA6, GetButtonID(4, 0), GumpButtonType.Reply, 0); 
										AddLabel(175, 280, LabelHue, "N/A");
										AddButton(140, 300, 0xFA5, 0xFA6, GetButtonID(4, 1), GumpButtonType.Reply, 0); 
										AddLabel(175, 300, LabelHue, "PASSENGER");
										AddButton(140, 320, 0xFA5, 0xFA6, GetButtonID(4, 2), GumpButtonType.Reply, 0); 
										AddLabel(175, 320, LabelHue, "CREW");
										AddButton(140, 340, 0xFA5, 0xFA6, GetButtonID(4, 3), GumpButtonType.Reply, 0); 
										AddLabel(175, 340, LabelHue, "OFFICER");
										AddButton(140, 360, 0xFA5, 0xFA6, GetButtonID(4, 4), GumpButtonType.Reply, 0); 
										AddLabel(175, 360, LabelHue, "DENY ACCESS");
											
										break;					
									}
									
								case (1):
									{
										AddLabel(140, 260, LabelHue, String.Format("Guild Access: "));
										AddButton(140, 280, 0xFA5, 0xFA6, GetButtonID(4, 0), GumpButtonType.Reply, 0); 
										AddLabel(175, 280, LabelHue, "N/A");
										AddButton(140, 300, 0xFA6, 0xFA6, GetButtonID(4, 1), GumpButtonType.Reply, 0); 
										AddLabel(175, 300, 98, "PASSENGER");
										AddButton(140, 320, 0xFA5, 0xFA6, GetButtonID(4, 2), GumpButtonType.Reply, 0); 
										AddLabel(175, 320, LabelHue, "CREW");
										AddButton(140, 340, 0xFA5, 0xFA6, GetButtonID(4, 3), GumpButtonType.Reply, 0); 
										AddLabel(175, 340, LabelHue, "OFFICER");
										AddButton(140, 360, 0xFA5, 0xFA6, GetButtonID(4, 4), GumpButtonType.Reply, 0); 
										AddLabel(175, 360, LabelHue, "DENY ACCESS");

										break;
									}
									
								case (2):
									{
										AddLabel(140, 260, LabelHue, String.Format("Guild Access: "));
										AddButton(140, 280, 0xFA5, 0xFA6, GetButtonID(4, 0), GumpButtonType.Reply, 0); 
										AddLabel(175, 280, LabelHue, "N/A");
										AddButton(140, 300, 0xFA5, 0xFA6, GetButtonID(4, 1), GumpButtonType.Reply, 0); 
										AddLabel(175, 300, LabelHue, "PASSENGER");
										AddButton(140, 320, 0xFA6, 0xFA6, GetButtonID(4, 2), GumpButtonType.Reply, 0); 
										AddLabel(175, 320, 68, "CREW");
										AddButton(140, 340, 0xFA5, 0xFA6, GetButtonID(4, 3), GumpButtonType.Reply, 0); 
										AddLabel(175, 340, LabelHue, "OFFICER");
										AddButton(140, 360, 0xFA5, 0xFA6, GetButtonID(4, 4), GumpButtonType.Reply, 0); 
										AddLabel(175, 360, LabelHue, "DENY ACCESS");

										break;
									}
									
								case (3):
									{
										AddLabel(140, 260, LabelHue, String.Format("Guild Access: "));
										AddButton(140, 280, 0xFA5, 0xFA6, GetButtonID(4, 0), GumpButtonType.Reply, 0); 
										AddLabel(175, 280, LabelHue, "N/A");
										AddButton(140, 300, 0xFA5, 0xFA6, GetButtonID(4, 1), GumpButtonType.Reply, 0); 
										AddLabel(175, 300, LabelHue, "PASSENGER");
										AddButton(140, 320, 0xFA5, 0xFA6, GetButtonID(4, 2), GumpButtonType.Reply, 0); 
										AddLabel(175, 320, LabelHue, "CREW");
										AddButton(140, 340, 0xFA6, 0xFA6, GetButtonID(4, 3), GumpButtonType.Reply, 0); 
										AddLabel(175, 340, 53, "OFFICER");
										AddButton(140, 360, 0xFA5, 0xFA6, GetButtonID(4, 4), GumpButtonType.Reply, 0); 
										AddLabel(175, 360, LabelHue, "DENY ACCESS");

										break;
									}
					
								case (4):
									{
										AddLabel(140, 260, LabelHue, String.Format("Guild Access: "));
										AddButton(140, 280, 0xFA5, 0xFA6, GetButtonID(4, 0), GumpButtonType.Reply, 0); 
										AddLabel(175, 280, LabelHue, "N/A");
										AddButton(140, 300, 0xFA5, 0xFA6, GetButtonID(4, 1), GumpButtonType.Reply, 0); 
										AddLabel(175, 300, LabelHue, "PASSENGER");
										AddButton(140, 320, 0xFA5, 0xFA6, GetButtonID(4, 2), GumpButtonType.Reply, 0); 
										AddLabel(175, 320, LabelHue, "CREW");
										AddButton(140, 340, 0xFA5, 0xFA6, GetButtonID(4, 3), GumpButtonType.Reply, 0); 
										AddLabel(175, 340, LabelHue, "OFFICER");
										AddButton(140, 360, 0xFA5, 0xFA6, GetButtonID(4, 4), GumpButtonType.Reply, 0); 
										AddLabel(175, 360, LabelHue, "DENY ACCESS");

										break;
									}
									
								case (5):
									{
										AddLabel(140, 260, LabelHue, String.Format("Guild Access: "));
										AddButton(140, 280, 0xFA5, 0xFA6, GetButtonID(4, 0), GumpButtonType.Reply, 0); 
										AddLabel(175, 280, LabelHue, "N/A");
										AddButton(140, 300, 0xFA5, 0xFA6, GetButtonID(4, 1), GumpButtonType.Reply, 0); 
										AddLabel(175, 300, LabelHue, "PASSENGER");
										AddButton(140, 320, 0xFA5, 0xFA6, GetButtonID(4, 2), GumpButtonType.Reply, 0); 
										AddLabel(175, 320, LabelHue, "CREW");
										AddButton(140, 340, 0xFA5, 0xFA6, GetButtonID(4, 3), GumpButtonType.Reply, 0); 
										AddLabel(175, 340, LabelHue, "OFFICER");
										AddButton(140, 360, 0xFA6, 0xFA6, GetButtonID(4, 4), GumpButtonType.Reply, 0); 
										AddLabel(175, 360, 38, "DENY ACCESS");

										break;
									}
							}		

						

							
							break;
						}					
				}
				
				AddButtonLabeled(160, 410, GetButtonID(6, 1), 1149734); // Access List
				
			}
			else
			{
				switch( page )
				{
					case (SecuritySettingsGumpPage.AccessListDefault):
						{
						
							if (playersAboard == null)
								break;
								
							int PlayersCounter = playersAboard.Count;				

							int currentPage = 1;
							int currentLine = 0;
							int currentButton = 0;
							
							AddPage(1);
							
							foreach(KeyValuePair<int, PlayerMobile> entry in playersAboard)
							{
																
								AddButton(10, 100 + currentLine * 20, 0xFA5, 0xFA6, GetButtonID(5, currentButton++), GumpButtonType.Reply, 0);
								AddLabel(45, 100 + currentLine * 20, LabelHue, (entry.Value).Name);
								
								byte listedPlayerAccess = 0;
								if (_falleon.PlayerAccess != null)
									foreach(KeyValuePair<PlayerMobile, byte> entry2 in _falleon.PlayerAccess)							
										if (entry.Value == entry2.Key)
											listedPlayerAccess = entry2.Value;								

								switch (listedPlayerAccess)
								{
									case (0):
										{
											AddLabel(120, 100 + currentLine * 20, LabelHue, "N/A");
												
											break;					
										}
										
									case (1):
										{
											AddLabel(120, 100 + currentLine * 20, 98, "PASSENGER");
												
											break;					
										}
									
									case (2):
										{
											AddLabel(120, 100 + currentLine * 20, 68, "CREW");
												
											break;					
										}
										
									case (3):
										{
											AddLabel(120, 100 + currentLine * 20, 53, "OFFICER");
												
											break;					
										}
										
									case (4):
										{
											AddLabel(120, 100 + currentLine * 20, 43, "CAPTAIN");
												
											break;					
										}
										
									case (5):
										{
											AddLabel(120, 100 + currentLine * 20, 38, "DENY ACCESS");
												
											break;					
										}
								}
								
								++currentLine;
								
								if (currentLine == 10)
								{
									currentLine = 0;
									currentButton = 0;
									AddPage(currentPage++);
									
									_currentAccessListPage = currentPage;																		
								}
								
								if (currentPage > _currentAccessListPage)
									break;
							}
							
							AddButton(10, 410, 0xFA5, 0xFA6, GetButtonID(6, 0), GumpButtonType.Reply, 0);
							AddLabel(45, 410, LabelHue, "MAIN MENU");	
							
							if (_currentAccessListPage > 1)							
								AddButton(160, 410, 0xFAE, 0xFAF,GetButtonID(6, 2), GumpButtonType.Reply, 0);
							
							
							if (currentLine == 0) 
								AddButton(200, 410, 0xFA5, 0xFA6,GetButtonID(6, 3), GumpButtonType.Reply, 0);
								
							
						}	

						break;					
						
					case (SecuritySettingsGumpPage.AccessListPlayer):
						{
						
							if (selectedPlayer == null)
								break;

							AddLabel(10, 120, LabelHue, selectedPlayer.Name);
								
							byte selectedPlayerAccess = 0;
							if (_falleon.PlayerAccess != null)
								foreach(KeyValuePair<PlayerMobile, byte> entry in _falleon.PlayerAccess)							
									if (selectedPlayer == entry.Key)
										selectedPlayerAccess = entry.Value;								

							switch (selectedPlayerAccess)
							{
								case (0):
									{
										AddLabel(80, 120, LabelHue, "N/A");
										AddLabel(80, 260, LabelHue, String.Format(selectedPlayer.Name + " Access: "));
										AddButton(80, 280, 0xFA6, 0xFA6, GetButtonID(7, 0), GumpButtonType.Reply, 0); 
										AddLabel(115, 280, LabelHue, "N/A");
										AddButton(80, 300, 0xFA5, 0xFA6, GetButtonID(7, 1), GumpButtonType.Reply, 0); 
										AddLabel(115, 300, LabelHue, "PASSENGER");
										AddButton(80, 320, 0xFA5, 0xFA6, GetButtonID(7, 2), GumpButtonType.Reply, 0); 
										AddLabel(115, 320, LabelHue, "CREW");
										AddButton(80, 340, 0xFA5, 0xFA6, GetButtonID(7, 3), GumpButtonType.Reply, 0); 
										AddLabel(115, 340, LabelHue, "OFFICER");
										AddButton(80, 360, 0xFA5, 0xFA6, GetButtonID(7, 4), GumpButtonType.Reply, 0); 
										AddLabel(115, 360, LabelHue, "CAPTAIN");
										AddButton(80, 380, 0xFA5, 0xFA6, GetButtonID(7, 5), GumpButtonType.Reply, 0); 
										AddLabel(115, 380, LabelHue, "DENY ACCESS");

											
										break;					
									}
									
								case (1):
									{
										AddLabel(80, 120, 98, "PASSENGER");
										AddLabel(80, 260, LabelHue, String.Format(selectedPlayer.Name + " Access: "));
										AddButton(80, 280, 0xFA5, 0xFA6, GetButtonID(7, 0), GumpButtonType.Reply, 0); 
										AddLabel(115, 280, LabelHue, "N/A");
										AddButton(80, 300, 0xFA6, 0xFA6, GetButtonID(7, 1), GumpButtonType.Reply, 0); 
										AddLabel(115, 300, 98, "PASSENGER");
										AddButton(80, 320, 0xFA5, 0xFA6, GetButtonID(7, 2), GumpButtonType.Reply, 0); 
										AddLabel(115, 320, LabelHue, "CREW");
										AddButton(80, 340, 0xFA5, 0xFA6, GetButtonID(7, 3), GumpButtonType.Reply, 0); 
										AddLabel(115, 340, LabelHue, "OFFICER");
										AddButton(80, 360, 0xFA5, 0xFA6, GetButtonID(7, 4), GumpButtonType.Reply, 0); 
										AddLabel(115, 360, LabelHue, "CAPTAIN");
										AddButton(80, 380, 0xFA5, 0xFA6, GetButtonID(7, 5), GumpButtonType.Reply, 0); 
										AddLabel(115, 380, LabelHue, "DENY ACCESS");

											
										break;
									}
									
								case (2):
									{
										AddLabel(80, 120, 68, "CREW");
										AddLabel(80, 260, LabelHue, String.Format(selectedPlayer.Name + " Access: "));
										AddButton(80, 280, 0xFA5, 0xFA6, GetButtonID(7, 0), GumpButtonType.Reply, 0); 
										AddLabel(115, 280, LabelHue, "N/A");
										AddButton(80, 300, 0xFA5, 0xFA6, GetButtonID(7, 1), GumpButtonType.Reply, 0); 
										AddLabel(115, 300, LabelHue, "PASSENGER");
										AddButton(80, 320, 0xFA6, 0xFA6, GetButtonID(7, 2), GumpButtonType.Reply, 0); 
										AddLabel(115, 320, 68, "CREW");
										AddButton(80, 340, 0xFA5, 0xFA6, GetButtonID(7, 3), GumpButtonType.Reply, 0); 
										AddLabel(115, 340, LabelHue, "OFFICER");
										AddButton(80, 360, 0xFA5, 0xFA6, GetButtonID(7, 4), GumpButtonType.Reply, 0); 
										AddLabel(115, 360, LabelHue, "CAPTAIN");
										AddButton(80, 380, 0xFA5, 0xFA6, GetButtonID(7, 5), GumpButtonType.Reply, 0); 
										AddLabel(115, 380, LabelHue, "DENY ACCESS");


										break;
									}
									
								case (3):
									{
										AddLabel(80, 120, 53, "OFFICER");
										AddLabel(80, 260, LabelHue, String.Format(selectedPlayer.Name + " Access: "));
										AddButton(80, 280, 0xFA5, 0xFA6, GetButtonID(7, 0), GumpButtonType.Reply, 0); 
										AddLabel(115, 280, LabelHue, "N/A");
										AddButton(80, 300, 0xFA5, 0xFA6, GetButtonID(7, 1), GumpButtonType.Reply, 0); 
										AddLabel(115, 300, LabelHue, "PASSENGER");
										AddButton(80, 320, 0xFA5, 0xFA6, GetButtonID(7, 2), GumpButtonType.Reply, 0); 
										AddLabel(115, 320, LabelHue, "CREW");
										AddButton(80, 340, 0xFA6, 0xFA6, GetButtonID(7, 3), GumpButtonType.Reply, 0); 
										AddLabel(115, 340, 53, "OFFICER");
										AddButton(80, 360, 0xFA5, 0xFA6, GetButtonID(7, 4), GumpButtonType.Reply, 0); 
										AddLabel(115, 360, LabelHue, "CAPTAIN");
										AddButton(80, 380, 0xFA5, 0xFA6, GetButtonID(7, 5), GumpButtonType.Reply, 0); 
										AddLabel(115, 380, LabelHue, "DENY ACCESS");


										break;
									}
					
								case (4):
									{
										AddLabel(80, 120, 43, "CAPTAIN");
										AddLabel(80, 260, LabelHue, String.Format(selectedPlayer.Name + " Access: "));
										AddButton(80, 280, 0xFA5, 0xFA6, GetButtonID(7, 0), GumpButtonType.Reply, 0); 
										AddLabel(115, 280, LabelHue, "N/A");
										AddButton(80, 300, 0xFA5, 0xFA6, GetButtonID(7, 1), GumpButtonType.Reply, 0); 
										AddLabel(115, 300, LabelHue, "PASSENGER");
										AddButton(80, 320, 0xFA5, 0xFA6, GetButtonID(7, 2), GumpButtonType.Reply, 0); 
										AddLabel(115, 320, LabelHue, "CREW");
										AddButton(80, 340, 0xFA5, 0xFA6, GetButtonID(7, 3), GumpButtonType.Reply, 0); 
										AddLabel(115, 340, LabelHue, "OFFICER");
										AddButton(80, 360, 0xFA5, 0xFA6, GetButtonID(7, 4), GumpButtonType.Reply, 0); 
										AddLabel(115, 360, 43, "CAPTAIN");
										AddButton(80, 380, 0xFA5, 0xFA6, GetButtonID(7, 5), GumpButtonType.Reply, 0); 
										AddLabel(115, 380, LabelHue, "DENY ACCESS");


										break;
									}
									
								case (5):
									{
										AddLabel(80, 120, 38, "DENY ACCESS");
										AddLabel(80, 260, LabelHue, String.Format(selectedPlayer.Name + " Access: "));
										AddButton(80, 280, 0xFA5, 0xFA6, GetButtonID(7, 0), GumpButtonType.Reply, 0); 
										AddLabel(115, 280, LabelHue, "N/A");
										AddButton(80, 300, 0xFA5, 0xFA6, GetButtonID(7, 1), GumpButtonType.Reply, 0); 
										AddLabel(115, 300, LabelHue, "PASSENGER");
										AddButton(80, 320, 0xFA5, 0xFA6, GetButtonID(7, 2), GumpButtonType.Reply, 0); 
										AddLabel(115, 320, LabelHue, "CREW");
										AddButton(80, 340, 0xFA5, 0xFA6, GetButtonID(7, 3), GumpButtonType.Reply, 0); 
										AddLabel(115, 340, LabelHue, "OFFICER");
										AddButton(80, 360, 0xFA6, 0xFA6, GetButtonID(7, 4), GumpButtonType.Reply, 0); 
										AddLabel(115, 360, LabelHue, "CAPTAIN");
										AddButton(80, 380, 0xFA5, 0xFA6, GetButtonID(7, 5), GumpButtonType.Reply, 0); 
										AddLabel(115, 380, 38, "DENY ACCESS");

										break;
									}
							}											
						}
						
						break;						
				}
			}														
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (_falleon.Deleted)
                return;

            Mobile from = sender.Mobile;

            if (!from.CheckAlive())
                return;

            int val = info.ButtonID - 1;

            if (val < 0)
                return;

            int type = val % 15;
            int index = val / 15;

            switch ( type )
            {
                case 0:
                    {
                        switch ( index )
                        {
                            case 0: // Never
                                {       
									_falleon.CanModifySecurity = 0;
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));

                                    break;
                                }
                            case 1: // Leader
                                {
									_falleon.CanModifySecurity = 1;
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));

                                    break;
                                }
                            case 2: // Member
                                {
									_falleon.CanModifySecurity = 2;
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));
									
                                    break;
                                }
						}
                        
                        break;
                    }
					
                case 1:
                    {
                        switch ( index )
                        {
                            case 0: // Public
                                {      
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Public, from, _falleon, PlayersAboard, 1, null));

                                    break;
                                }
                            case 1: // Party
                                {      
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Party, from, _falleon, PlayersAboard, 1, null));

                                    break;
                                }
                            case 2: // Guild
                                {      
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Guild, from, _falleon, PlayersAboard, 1, null));

                                    break;
                                }                                                       
						}
                        
                        break;
                    }	

				case 2:
					{
                        switch ( index )
                        {
							case 0: // N/A
								{
									_falleon.Public = 0;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}
								
							case 1: // Passenger
								{
									_falleon.Public = 1;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}
								
							case 2: // Crew
								{
									_falleon.Public = 2;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}
								
							case 3: // Officer
								{
									_falleon.Public = 3;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}

							case 4: // Deny Access
								{
									_falleon.Public = 5;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}	
									
						}
						
						break;
						
					}
					
					case 3:
					{
                        switch ( index )
                        {
							case 0: // N/A
								{
									_falleon.Party = 0;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}
								
							case 1: // Passenger
								{
									_falleon.Party = 1;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}
								
							case 2: // Crew
								{
									_falleon.Party = 2;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}
								
							case 3: // Officer
								{
									_falleon.Party = 3;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}

							case 4: // Deny Access
								{
									_falleon.Party = 5;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}									
						}
						
						break;
						
					}
					
					case 4:
					{
                        switch ( index )
                        {
							case 0: // N/A
								{
									_falleon.Guild = 0;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}
								
							case 1: // Passenger
								{
									_falleon.Guild = 1;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}
								
							case 2: // Crew
								{
									_falleon.Guild = 2;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}
								
							case 3: // Officer
								{
									_falleon.Guild = 3;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}

							case 4: // Deny Access
								{
									_falleon.Guild = 5;
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));		

									break;
								}									
						}
						
						break;						
					}
					
                case 5:
                    {   	
						int selectedPlayerKey = (_currentAccessListPage -1) * 10 + index;
						
						foreach(KeyValuePair<int, PlayerMobile> entry in _playersAboard)						
							if (entry.Key == selectedPlayerKey)
								_selectedPlayer = entry.Value;
						
						from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListPlayer, from, _falleon, PlayersAboard, 1, _selectedPlayer));					
                        
                        break;
                    }					
					
					
                case 6:
                    {
                        switch ( index )
                        {
                            case 0: // Main Menu
                                {       									
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, from, _falleon, PlayersAboard, 1, null));

                                    break;
                                }
                            case 1: // Access List
                                {									
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, _falleon, PlayersAboard, 1, null));

                                    break;
                                }
                            case 2: // Previous Page
                                {       									
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListPage, from, _falleon, PlayersAboard, --_currentAccessListPage, null));

                                    break;
                                }
                            case 3: // Next Page
                                {									
                                    from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListPage, from, _falleon, PlayersAboard, ++_currentAccessListPage, null));

                                    break;
                                }
						}
                        
                        break;
                    }	

					case 7:
					{
                        switch ( index )
                        {
							case 0: // N/A
								{
									if (_selectedPlayer == null)
										break;
										
									if (_falleon.PlayerAccess != null)
										if (_falleon.PlayerAccess.ContainsKey(_selectedPlayer))
											_falleon.PlayerAccess[_selectedPlayer] = 0;
										else
											_falleon.PlayerAccess.Add(_selectedPlayer, 0);
											
									if (_falleon.PlayerAccess == null)
										_falleon.PlayerAccess.Add(_selectedPlayer, 0);
									
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, _falleon, PlayersAboard, 1, null));		

									break;
								}
								
							case 1: // Passenger
								{
									if (_selectedPlayer == null)
										break;
										
									if (_falleon.PlayerAccess != null)
										if (_falleon.PlayerAccess.ContainsKey(_selectedPlayer))
											_falleon.PlayerAccess[_selectedPlayer] = 1;
										else
											_falleon.PlayerAccess.Add(_selectedPlayer, 1);
											
									if (_falleon.PlayerAccess == null)
										_falleon.PlayerAccess.Add(_selectedPlayer, 1);
									
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, _falleon, PlayersAboard, 1, null));		

									break;
								}
								
							case 2: // Crew
								{
									if (_selectedPlayer == null)
										break;
										
									if (_falleon.PlayerAccess != null)
										if (_falleon.PlayerAccess.ContainsKey(_selectedPlayer))
											_falleon.PlayerAccess[_selectedPlayer] = 2;
										else
											_falleon.PlayerAccess.Add(_selectedPlayer, 2);
											
									if (_falleon.PlayerAccess == null)
										_falleon.PlayerAccess.Add(_selectedPlayer, 2);
									
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, _falleon, PlayersAboard, 1, null));		

									break;
								}
								
							case 3: // Officer
								{
									if (_selectedPlayer == null)
										break;
									
									if (_falleon.PlayerAccess != null)
										if (_falleon.PlayerAccess.ContainsKey(_selectedPlayer))
											_falleon.PlayerAccess[_selectedPlayer] = 3;
										else
											_falleon.PlayerAccess.Add(_selectedPlayer, 3);
											
									if (_falleon.PlayerAccess == null)
										_falleon.PlayerAccess.Add(_selectedPlayer, 3);
									
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, _falleon, PlayersAboard, 1, null));		

									break;
								}

							case 4: // Captain
								{
									if (_selectedPlayer == null)
										break;
										
									if (_falleon.PlayerAccess != null)
										if (_falleon.PlayerAccess.ContainsKey(_selectedPlayer))
											_falleon.PlayerAccess[_selectedPlayer] = 4;
										else
											_falleon.PlayerAccess.Add(_selectedPlayer, 4);
											
									if (_falleon.PlayerAccess == null)
										_falleon.PlayerAccess.Add(_selectedPlayer, 4);
									
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, _falleon, PlayersAboard, 1, null));		

									break;
								}	

							case 5: // DENY ACCESS
								{
									if (_selectedPlayer == null)
										break;
										
									if (_falleon.PlayerAccess != null)
										if (_falleon.PlayerAccess.ContainsKey(_selectedPlayer))
											_falleon.PlayerAccess[_selectedPlayer] = 5;
										else
											_falleon.PlayerAccess.Add(_selectedPlayer, 5);
											
									if (_falleon.PlayerAccess == null)
										_falleon.PlayerAccess.Add(_selectedPlayer, 5);
									
									from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.AccessListDefault, from, _falleon, PlayersAboard, 1, null));		

									break;
								}	
								
						}
						
						break;						
					}					
            }			      					
        }
    }
}