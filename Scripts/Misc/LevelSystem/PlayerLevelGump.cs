using System;
using Server;
using System.Collections;
using Server.Network;
using Server.Misc;
using Server.Mobiles;
using Server.Gumps;
using Server.Commands;


namespace Server.Gumps
{
    public class PlayerLevelGump : Gump
    {
        private Mobile m_From;
        private GumpPage m_Page;
        private SkillCategory m_Cat;
        
        private enum GumpPage
        {
            None,
            SkillList
        }

        public enum SkillCategory
        {
            Misc,
            Combat,
            Trade,
            Magic,
            Wild,
            Bard,
            Thief

        }

        //public SkillCategory m_Category;

        private const int LabelHue = 0x480;
        private const int TitleHue = 0x12B;

		public static void Initialize()
		{
         CommandSystem.Register( "level", AccessLevel.Player, new CommandEventHandler( level_OnCommand ) );
        }
    
		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "level" )]
		[Description( "Opens Level Gump." )]
		public static void level_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( PlayerLevelGump ) );
			from.SendGump( new PlayerLevelGump( from, GumpPage.None, SkillCategory.Misc ) );
				
        }
        //public PlayerLevelGump( Mobile from, GumpPage page, SkillCategory cat ) : this( from, GumpPage.None, cat )
		//{
		//}

        
		private PlayerLevelGump ( Mobile from, GumpPage page, SkillCategory cat ) : base( 40, 40 )
		{
            m_From = from;
            m_Page = page;
            m_Cat = cat;


            PlayerMobile pm = from as PlayerMobile;

			m_From.CloseGump( typeof( PlayerLevelGump ) );

			Closable=true;
			Disposable=true;
			Dragable=true;
			Resizable=false;

			AddPage(0);
			AddBackground(50, 35, 540, 382, 9270);
			AddAlphaRegion(66, 91, 219, 180);
			AddAlphaRegion(66, 49, 508, 34);
			AddAlphaRegion(292, 91, 283, 279);
			AddAlphaRegion(66, 269, 219, 101);

			AddLabel(262, 56, TitleHue, @"Player Level Gump");

            AddLabel(136, 93, TitleHue, @"Categories");
			AddButton(75, 116, 4005, 4007, GetButtonID( 1, 0 ), GumpButtonType.Reply, 0);
            AddLabel(112, 117, LabelHue, @"Miscelaneous");
			AddButton(75, 138, 4005, 4007, GetButtonID( 1, 1 ), GumpButtonType.Reply, 0);
            AddLabel(112, 139, LabelHue, @"Combat");
			AddButton(75, 160, 4005, 4007, GetButtonID( 1, 2 ), GumpButtonType.Reply, 0);
            AddLabel(112, 161, LabelHue, @"Trade Skills");
			AddButton(75, 182, 4005, 4007, GetButtonID( 1, 3 ), GumpButtonType.Reply, 0);
            AddLabel(112, 183, LabelHue, @"Magic");
            AddButton(75, 204, 4005, 4007, GetButtonID( 1, 4 ), GumpButtonType.Reply, 0);
            AddLabel(112, 205, LabelHue, @"Wilderness");
			AddButton(75, 226, 4005, 4007, GetButtonID( 1, 5 ), GumpButtonType.Reply, 0);
            AddLabel(112, 227, LabelHue, @"Thieving");
            AddButton(75, 248, 4005, 4007, GetButtonID( 1, 6 ), GumpButtonType.Reply, 0);
            AddLabel(112, 249, LabelHue, @"Bard");

			AddImage(0, 4, 10440);
			AddImage(554, 4, 10441);

            CreatePlayerExpList(from);

            int totalBaseStats;

            totalBaseStats = pm.RawStr + pm.RawInt + pm.RawDex;

			AddButton(75, 379, 241, 243, 0, GumpButtonType.Reply, 0); //Cancel
            AddButton(150, 379, 2027, 2028, GetButtonID(1,65), GumpButtonType.Reply, 0);
            AddLabel(300, 380, LabelHue, @"Stat Total: ");
            AddLabel(370, 380, LabelHue, totalBaseStats.ToString());
            AddLabel(430, 380, LabelHue, @"Skill Total: ");
            int totalSkill = pm.SkillsTotal;
            totalSkill = totalSkill / 10;
            AddLabel(500, 380, LabelHue, totalSkill.ToString());

            if (page == GumpPage.SkillList)
            {
                CreateSkillList(from, cat);
                return;
            }
            else
            {
                CreatePlayerStats(from); 
                return;
            }
            

		}
		
        public void CreatePlayerExpList( Mobile from )
		{
			PlayerMobile pm = from as PlayerMobile;
           		
            AddLabel(75, 275, LabelHue, @"Current Level:");
            AddLabel(164, 275, LabelHue, pm.charLevel.ToString());

            AddLabel(75, 297, LabelHue, @"Experience:");
            AddLabel(144, 297, LabelHue, pm.charExp.ToString());

            AddLabel(75, 319, LabelHue, @"Exp. to next level:");
            AddLabel(191, 319, LabelHue, pm.charToLevel.ToString());

            AddLabel(75, 341, LabelHue, @"Skill Points Avail:");
            AddLabel(185, 341, LabelHue, pm.charSKPoints.ToString());

            return;
        }

        public void CreatePlayerStats(Mobile from)
        {
            //int index = 0;
            //int pageindex;

            PlayerMobile pm = from as PlayerMobile;
            AddLabel(394, 93, TitleHue, @"Stats");

            AddButton(300, 116, 4005, 4007, GetButtonID(1, 7), GumpButtonType.Reply, 0);
            AddLabel(337, 117, LabelHue, @"STR: ");
            AddLabel(374, 117, LabelHue, pm.RawStr.ToString());

            AddButton(300, 138, 4005, 4007, GetButtonID(1, 8), GumpButtonType.Reply, 0);
            AddLabel(337, 139, LabelHue, @"INT: ");
            AddLabel(374, 139, LabelHue, pm.RawInt.ToString());

            AddButton(300, 160, 4005, 4007, GetButtonID(1, 9), GumpButtonType.Reply, 0);
            AddLabel(337, 161, LabelHue, @"DEX: ");
            AddLabel(374, 161, LabelHue, pm.RawDex.ToString());

            AddLabel(300, 183, LabelHue, @"Available stat points: ");
            AddLabel(435, 183, LabelHue, pm.charStatPoints.ToString());

            return;

        }
        public void CreateSkillList(Mobile from, SkillCategory cat)
        {
            m_From = from;
            m_Cat = cat;

            PlayerMobile pm = from as PlayerMobile;

            //int index = 0;
			//int pageindex;
			//int attrvalue;

            AddLabel(394, 93, TitleHue, @"Skills");
                    if (m_Cat == SkillCategory.Misc)
                    {
                        AddButton(300, 116, 4005, 4007, GetButtonID(1, 10), GumpButtonType.Reply, 0);
                        AddLabel(337, 117, LabelHue, pm.Skills.ArmsLore.Name.ToString());
                        AddLabel(475, 117, LabelHue, pm.Skills.ArmsLore.Base.ToString());

                        AddButton(300, 138, 4005, 4007, GetButtonID(1, 11), GumpButtonType.Reply, 0);
                        AddLabel(337, 139, LabelHue, pm.Skills.Begging.Name.ToString());
                        AddLabel(475, 139, LabelHue, pm.Skills.Begging.Base.ToString());

                        AddButton(300, 160, 4005, 4007, GetButtonID(1, 12), GumpButtonType.Reply, 0);
                        AddLabel(337, 161, LabelHue, pm.Skills.Camping.Name.ToString());
                        AddLabel(475, 161, LabelHue, pm.Skills.Camping.Base.ToString());

                        AddButton(300, 182, 4005, 4007, GetButtonID(1, 13), GumpButtonType.Reply, 0);
                        AddLabel(337, 183, LabelHue, pm.Skills.Cartography.Name.ToString());
                        AddLabel(475, 183, LabelHue, pm.Skills.Cartography.Base.ToString());

                        AddButton(300, 204, 4005, 4007, GetButtonID(1, 14), GumpButtonType.Reply, 0);
                        AddLabel(337, 205, LabelHue, pm.Skills.Forensics.Name.ToString());
                        AddLabel(475, 205, LabelHue, pm.Skills.Forensics.Base.ToString());

                        AddButton(300, 226, 4005, 4007, GetButtonID(1, 15), GumpButtonType.Reply, 0);
                        AddLabel(337, 227, LabelHue, pm.Skills.ItemID.Name.ToString());
                        AddLabel(475, 227, LabelHue, pm.Skills.ItemID.Base.ToString());

                        AddButton(300, 248, 4005, 4007, GetButtonID(1, 16), GumpButtonType.Reply, 0);
                        AddLabel(337, 249, LabelHue, pm.Skills.TasteID.Name.ToString());
                        AddLabel(475, 249, LabelHue, pm.Skills.TasteID.Base.ToString());

                    
                    }
                    if (m_Cat == SkillCategory.Combat)
                    {
                        AddButton(300, 116, 4005, 4007, GetButtonID(1, 17), GumpButtonType.Reply, 0);
                        AddLabel(337, 117, LabelHue, pm.Skills.Anatomy.Name.ToString());
                        AddLabel(475, 117, LabelHue, pm.Skills.Anatomy.Base.ToString());

                        AddButton(300, 138, 4005, 4007, GetButtonID(1, 18), GumpButtonType.Reply, 0);
                        AddLabel(337, 139, LabelHue, pm.Skills.Archery.Name.ToString());
                        AddLabel(475, 139, LabelHue, pm.Skills.Archery.Base.ToString());

                        AddButton(300, 160, 4005, 4007, GetButtonID(1, 19), GumpButtonType.Reply, 0);
                        AddLabel(337, 161, LabelHue, pm.Skills.Fencing.Name.ToString());
                        AddLabel(475, 161, LabelHue, pm.Skills.Fencing.Base.ToString());

                        AddButton(300, 182, 4005, 4007, GetButtonID(1, 20), GumpButtonType.Reply, 0);
                        AddLabel(337, 183, LabelHue, pm.Skills.Focus.Name.ToString());
                        AddLabel(475, 183, LabelHue, pm.Skills.Focus.Base.ToString());

                        AddButton(300, 204, 4005, 4007, GetButtonID(1, 66), GumpButtonType.Reply, 0);
                        AddLabel(337, 205, LabelHue, pm.Skills.Healing.Name.ToString());
                        AddLabel(475, 205, LabelHue, pm.Skills.Healing.Base.ToString());

                        AddButton(300, 226, 4005, 4007, GetButtonID(1, 21), GumpButtonType.Reply, 0);
                        AddLabel(337, 227, LabelHue, pm.Skills.Macing.Name.ToString());
                        AddLabel(475, 227, LabelHue, pm.Skills.Macing.Base.ToString());

                        AddButton(300, 248, 4005, 4007, GetButtonID(1, 22), GumpButtonType.Reply, 0);
                        AddLabel(337, 249, LabelHue, pm.Skills.Parry.Name.ToString());
                        AddLabel(475, 249, LabelHue, pm.Skills.Parry.Base.ToString());
                                                
                        AddButton(300, 270, 4005, 4007, GetButtonID(1, 23), GumpButtonType.Reply, 0);
                        AddLabel(337, 271, LabelHue, pm.Skills.Swords.Name.ToString());
                        AddLabel(475, 271, LabelHue, pm.Skills.Swords.Base.ToString());

                        AddButton(300, 292, 4005, 4007, GetButtonID(1, 24), GumpButtonType.Reply, 0);
                        AddLabel(337, 293, LabelHue, pm.Skills.Tactics.Name.ToString());
                        AddLabel(475, 293, LabelHue, pm.Skills.Tactics.Base.ToString());
											
						AddButton(300, 314, 4005, 4007, GetButtonID(1, 26), GumpButtonType.Reply, 0);
                        AddLabel(337, 315, LabelHue, pm.Skills.Wrestling.Name.ToString());
                        AddLabel(475, 315, LabelHue, pm.Skills.Wrestling.Base.ToString());
						
						/*
                        AddButton(300, 336, 4005, 4007, GetButtonID(1, 25), GumpButtonType.Reply, 0);
                        AddLabel(337, 337, LabelHue, pm.Skills.Throwing.Name.ToString());
                        AddLabel(475, 337, LabelHue, pm.Skills.Throwing.Base.ToString());
						*/
						
						
						
                    }
                    if (m_Cat == SkillCategory.Trade)
                    {
                        AddButton(300, 116, 4005, 4007, GetButtonID(1, 27), GumpButtonType.Reply, 0);
                        AddLabel(337, 117, LabelHue, pm.Skills.Alchemy.Name.ToString());
                        AddLabel(475, 117, LabelHue, pm.Skills.Alchemy.Base.ToString());

                        AddButton(300, 138, 4005, 4007, GetButtonID(1, 28), GumpButtonType.Reply, 0);
                        AddLabel(337, 139, LabelHue, pm.Skills.Blacksmith.Name.ToString());
                        AddLabel(475, 139, LabelHue, pm.Skills.Blacksmith.Base.ToString());

                        AddButton(300, 160, 4005, 4007, GetButtonID(1, 29), GumpButtonType.Reply, 0);
                        AddLabel(337, 161, LabelHue, pm.Skills.Fletching.Name.ToString());
                        AddLabel(475, 161, LabelHue, pm.Skills.Fletching.Base.ToString());

                        AddButton(300, 182, 4005, 4007, GetButtonID(1, 30), GumpButtonType.Reply, 0);
                        AddLabel(337, 183, LabelHue, pm.Skills.Carpentry.Name.ToString());
                        AddLabel(475, 183, LabelHue, pm.Skills.Carpentry.Base.ToString());

                        AddButton(300, 204, 4005, 4007, GetButtonID(1, 31), GumpButtonType.Reply, 0);
                        AddLabel(337, 205, LabelHue, pm.Skills.Cooking.Name.ToString());
                        AddLabel(475, 205, LabelHue, pm.Skills.Cooking.Base.ToString());

                        AddButton(300, 226, 4005, 4007, GetButtonID(1, 32), GumpButtonType.Reply, 0);
                        AddLabel(337, 227, LabelHue, pm.Skills.Inscribe.Name.ToString());
                        AddLabel(475, 227, LabelHue, pm.Skills.Inscribe.Base.ToString());

                        AddButton(300, 248, 4005, 4007, GetButtonID(1, 33), GumpButtonType.Reply, 0);
                        AddLabel(337, 249, LabelHue, pm.Skills.Lumberjacking.Name.ToString());
                        AddLabel(475, 249, LabelHue, pm.Skills.Lumberjacking.Base.ToString());

                        AddButton(300, 270, 4005, 4007, GetButtonID(1, 34), GumpButtonType.Reply, 0);
                        AddLabel(337, 271, LabelHue, pm.Skills.Mining.Name.ToString());
                        AddLabel(475, 271, LabelHue, pm.Skills.Mining.Base.ToString());

                        AddButton(300, 292, 4005, 4007, GetButtonID(1, 35), GumpButtonType.Reply, 0);
                        AddLabel(337, 293, LabelHue, pm.Skills.Tailoring.Name.ToString());
                        AddLabel(475, 293, LabelHue, pm.Skills.Tailoring.Base.ToString());
                        
                        AddButton(300, 316, 4005, 4007, GetButtonID(1, 36), GumpButtonType.Reply, 0);
                        AddLabel(337, 317, LabelHue, pm.Skills.Tinkering.Name.ToString());
                        AddLabel(475, 317, LabelHue, pm.Skills.Tinkering.Base.ToString());
						
						/*
						AddButton(300, 338, 4005, 4007, GetButtonID(1, 67), GumpButtonType.Reply, 0);
                        AddLabel(337, 339, LabelHue, pm.Skills.Imbueing.Name.ToString());
                        AddLabel(475, 339, LabelHue, pm.Skills.Imbueing.Base.ToString());
						*/
                        

                    }
                    if (m_Cat == SkillCategory.Magic)
                    {
                        AddButton(300, 116, 4005, 4007, GetButtonID(1, 37), GumpButtonType.Reply, 0);
                        AddLabel(337, 117, LabelHue, pm.Skills.Bushido.Name.ToString());
                        AddLabel(475, 117, LabelHue, pm.Skills.Bushido.Base.ToString());

                        AddButton(300, 138, 4005, 4007, GetButtonID(1, 38), GumpButtonType.Reply, 0);
                        AddLabel(337, 139, LabelHue, pm.Skills.Chivalry.Name.ToString());
                        AddLabel(475, 139, LabelHue, pm.Skills.Chivalry.Base.ToString());

                        AddButton(300, 160, 4005, 4007, GetButtonID(1, 39), GumpButtonType.Reply, 0);
                        AddLabel(337, 161, LabelHue, pm.Skills.EvalInt.Name.ToString());
                        AddLabel(475, 161, LabelHue, pm.Skills.EvalInt.Base.ToString());

                        AddButton(300, 182, 4005, 4007, GetButtonID(1, 40), GumpButtonType.Reply, 0);
                        AddLabel(337, 183, LabelHue, pm.Skills.Magery.Name.ToString());
                        AddLabel(475, 183, LabelHue, pm.Skills.Magery.Base.ToString());

                        AddButton(300, 204, 4005, 4007, GetButtonID(1, 41), GumpButtonType.Reply, 0);
                        AddLabel(337, 205, LabelHue, pm.Skills.Meditation.Name.ToString());
                        AddLabel(475, 205, LabelHue, pm.Skills.Meditation.Base.ToString());

                        AddButton(300, 226, 4005, 4007, GetButtonID(1, 42), GumpButtonType.Reply, 0);
                        AddLabel(337, 227, LabelHue, pm.Skills.Necromancy.Name.ToString());
                        AddLabel(475, 227, LabelHue, pm.Skills.Necromancy.Base.ToString());

                        AddButton(300, 248, 4005, 4007, GetButtonID(1, 43), GumpButtonType.Reply, 0);
                        AddLabel(337, 249, LabelHue, pm.Skills.Ninjitsu.Name.ToString());
                        AddLabel(475, 249, LabelHue, pm.Skills.Ninjitsu.Base.ToString());

                        AddButton(300, 270, 4005, 4007, GetButtonID(1, 44), GumpButtonType.Reply, 0);
                        AddLabel(337, 271, LabelHue, pm.Skills.MagicResist.Name.ToString());
                        AddLabel(475, 271, LabelHue, pm.Skills.MagicResist.Base.ToString());

                        AddButton(300, 292, 4005, 4007, GetButtonID(1, 45), GumpButtonType.Reply, 0);
                        AddLabel(337, 293, LabelHue, pm.Skills.Spellweaving.Name.ToString());
                        AddLabel(475, 293, LabelHue, pm.Skills.Spellweaving.Base.ToString());

                        AddButton(300, 316, 4005, 4007, GetButtonID(1, 46), GumpButtonType.Reply, 0);
                        AddLabel(337, 317, LabelHue, pm.Skills.SpiritSpeak.Name.ToString());
                        AddLabel(475, 317, LabelHue, pm.Skills.SpiritSpeak.Base.ToString());
						
						/* Comment block for non SA
						AddButton(300, 338, 4005, 4007, GetButtonID(1, 68), GumpButtonType.Reply, 0);
                        AddLabel(337, 339, LabelHue, pm.Skills.Mysticism.Name.ToString());
                        AddLabel(475, 339, LabelHue, pm.Skills.Mysticism.Base.ToString());
						*/

                    }
                    if (m_Cat == SkillCategory.Wild)
                    {
                        AddButton(300, 116, 4005, 4007, GetButtonID(1, 47), GumpButtonType.Reply, 0);
                        AddLabel(337, 117, LabelHue, pm.Skills.AnimalLore.Name.ToString());
                        AddLabel(475, 117, LabelHue, pm.Skills.AnimalLore.Base.ToString());

                        AddButton(300, 138, 4005, 4007, GetButtonID(1, 48), GumpButtonType.Reply, 0);
                        AddLabel(337, 139, LabelHue, pm.Skills.AnimalTaming.Name.ToString());
                        AddLabel(475, 139, LabelHue, pm.Skills.AnimalTaming.Base.ToString());

                        AddButton(300, 160, 4005, 4007, GetButtonID(1, 49), GumpButtonType.Reply, 0);
                        AddLabel(337, 161, LabelHue, pm.Skills.Fishing.Name.ToString());
                        AddLabel(475, 161, LabelHue, pm.Skills.Fishing.Base.ToString());

                        AddButton(300, 182, 4005, 4007, GetButtonID(1, 50), GumpButtonType.Reply, 0);
                        AddLabel(337, 183, LabelHue, pm.Skills.Herding.Name.ToString());
                        AddLabel(475, 183, LabelHue, pm.Skills.Herding.Base.ToString());

                        AddButton(300, 204, 4005, 4007, GetButtonID(1, 51), GumpButtonType.Reply, 0);
                        AddLabel(337, 205, LabelHue, pm.Skills.Tracking.Name.ToString());
                        AddLabel(475, 205, LabelHue, pm.Skills.Tracking.Base.ToString());

                        AddButton(300, 226, 4005, 4007, GetButtonID(1, 52), GumpButtonType.Reply, 0);
                        AddLabel(337, 227, LabelHue, pm.Skills.Veterinary.Name.ToString());
                        AddLabel(475, 227, LabelHue, pm.Skills.Veterinary.Base.ToString());

                    }                 
                    if (m_Cat == SkillCategory.Thief)
                    {
                        AddButton(300, 116, 4005, 4007, GetButtonID(1, 53), GumpButtonType.Reply, 0);
                        AddLabel(337, 117, LabelHue, pm.Skills.DetectHidden.Name.ToString());
                        AddLabel(475, 117, LabelHue, pm.Skills.DetectHidden.Base.ToString());

                        AddButton(300, 138, 4005, 4007, GetButtonID(1, 54), GumpButtonType.Reply, 0);
                        AddLabel(337, 139, LabelHue, pm.Skills.Hiding.Name.ToString());
                        AddLabel(475, 139, LabelHue, pm.Skills.Hiding.Base.ToString());

                        AddButton(300, 160, 4005, 4007, GetButtonID(1, 55), GumpButtonType.Reply, 0);
                        AddLabel(337, 161, LabelHue, pm.Skills.Lockpicking.Name.ToString());
                        AddLabel(475, 161, LabelHue, pm.Skills.Lockpicking.Base.ToString());

                        AddButton(300, 182, 4005, 4007, GetButtonID(1, 56), GumpButtonType.Reply, 0);
                        AddLabel(337, 183, LabelHue, pm.Skills.Poisoning.Name.ToString());
                        AddLabel(475, 183, LabelHue, pm.Skills.Poisoning.Base.ToString());

                        AddButton(300, 204, 4005, 4007, GetButtonID(1, 57), GumpButtonType.Reply, 0);
                        AddLabel(337, 205, LabelHue, pm.Skills.RemoveTrap.Name.ToString());
                        AddLabel(475, 205, LabelHue, pm.Skills.RemoveTrap.Base.ToString());

                        AddButton(300, 226, 4005, 4007, GetButtonID(1, 58), GumpButtonType.Reply, 0);
                        AddLabel(337, 227, LabelHue, pm.Skills.Snooping.Name.ToString());
                        AddLabel(475, 227, LabelHue, pm.Skills.Snooping.Base.ToString());

                        AddButton(300, 248, 4005, 4007, GetButtonID(1, 59), GumpButtonType.Reply, 0);
                        AddLabel(337, 249, LabelHue, pm.Skills.Stealing.Name.ToString());
                        AddLabel(475, 249, LabelHue, pm.Skills.Stealing.Base.ToString());

                        AddButton(300, 270, 4005, 4007, GetButtonID(1, 60), GumpButtonType.Reply, 0);
                        AddLabel(337, 271, LabelHue, pm.Skills.Stealth.Name.ToString());
                        AddLabel(475, 271, LabelHue, pm.Skills.Stealth.Base.ToString());
                    }    
                    if(m_Cat == SkillCategory.Bard)
                    {
                        AddButton(300, 116, 4005, 4007, GetButtonID(1, 61), GumpButtonType.Reply, 0);
                        AddLabel(337, 117, LabelHue, pm.Skills.Discordance.Name.ToString());
                        AddLabel(475, 117, LabelHue, pm.Skills.Discordance.Base.ToString());

                        AddButton(300, 138, 4005, 4007, GetButtonID(1, 62), GumpButtonType.Reply, 0);
                        AddLabel(337, 139, LabelHue, pm.Skills.Musicianship.Name.ToString());
                        AddLabel(475, 139, LabelHue, pm.Skills.Musicianship.Base.ToString());

                        AddButton(300, 160, 4005, 4007, GetButtonID(1, 63), GumpButtonType.Reply, 0);
                        AddLabel(337, 161, LabelHue, pm.Skills.Peacemaking.Name.ToString());
                        AddLabel(475, 161, LabelHue, pm.Skills.Peacemaking.Base.ToString());

                        AddButton(300, 182, 4005, 4007, GetButtonID(1, 64), GumpButtonType.Reply, 0);
                        AddLabel(337, 183, LabelHue, pm.Skills.Provocation.Name.ToString());
                        AddLabel(475, 183, LabelHue, pm.Skills.Provocation.Base.ToString());

                    }



                    return;
           
        }

		public static int GetButtonID( int type, int index )
		{
			return 1 + type + (index * 7);
		}

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            

            int AvlSkillPoints;
            int AvlStatPoints;

            PlayerMobile pm = m_From as PlayerMobile;

            AvlSkillPoints = pm.charSKPoints;
            AvlStatPoints = pm.charStatPoints;


            if (info.ButtonID <= 0)
                return; // Canceled

            int buttonID = info.ButtonID - 1;
            int type = buttonID % 7;
            int index = buttonID / 7;

            //int cost = 0;
            //int attrvalue = 0;

            switch (type)
            {
                case 0: // Cancel
                    {
                        break;
                    }
                case 1: // Select Attribute Type
                    {
                       

                        switch (index)
                        {
                            case 0: // Misc
                                {
                                    m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                    break;
                                }
                            case 1: // Combat
                                {
                                   m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                   break;
                                }
                            case 2: // Trade
                                {

                                    m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                    break;
                                }
                            case 3: //Magic
                                {
                                    m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                    break;
                                }
                            case 4: // Wild
                                {
                                    m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Wild));
                                    break;
                                }
                            case 5: // Bard / thief
                                {
                                    m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                    
                                    break;
                                }
                            case 6: // Bard 
                                {
                                    m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Bard));
                                   
                                    break;
                                }
                            case 7: //str
                                {
                                    if (AvlStatPoints > 0)
                                    {
                                        m_From.SendMessage("One Stat Point has been added to your Strength");
                                        pm.RawStr += 1;
                                        AvlStatPoints -= 1;
                                        pm.charStatPoints = AvlStatPoints;
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available Stat points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                        return;
                                    }
                                    
                                }
                            case 8: //int
                                {
                                    if (AvlStatPoints > 0)
                                    {
                                        m_From.SendMessage("One Stat Point has been added to your Intelligence");
                                        pm.RawInt += 1;
                                        AvlStatPoints -= 1;
                                        pm.charStatPoints = AvlStatPoints;
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available Stat points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                        return;
                                    }
                                    
                                }
                            case 9: // Dex
                                {
                                    if (AvlStatPoints > 0)
                                    {
                                        m_From.SendMessage("One Stat Point has been added to your Dexterity");
                                        pm.RawDex += 1;
                                        AvlStatPoints -= 1;
                                        pm.charStatPoints = AvlStatPoints;
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available Stat points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                        return;
                                    }
                                    
                                }
                            case 10: // ArmsLore
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.ArmsLore.Base < pm.Skills.ArmsLore.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Arms lore");
                                            pm.Skills.ArmsLore.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                           return;

                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                        return;
                                    }
                                    
                                }
                            case 11: // Begging
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if(pm.Skills.Begging.Base < pm.Skills.Begging.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to begging");
                                            pm.Skills.Begging.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                        return;
                                    }
                                    
                                    
                                }
                            case 12: // Camping
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Camping.Base < pm.Skills.Camping.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Camping");
                                            pm.Skills.Camping.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                           return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                        return;
                                    }
                                    
                                    
                                }
                            case 13: // Cart
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if(pm.Skills.Cartography.Base < pm.Skills.Cartography.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Cart");
                                            pm.Skills.Cartography.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                        return;
                                    }
                                    
                                    
                                }
                            case 14: // Forensics
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if(pm.Skills.Forensics.Base < pm.Skills.Forensics.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Forensics");
                                            pm.Skills.Forensics.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                        return;
                                    }
                                   
                                }
                            case 15: // Item ID
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.ItemID.Base < pm.Skills.ItemID.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Item ID");
                                            pm.Skills.ItemID.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                        return;
                                    }
                                  
                                    
                                }
                            case 16: // Taste ID
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.TasteID.Base < pm.Skills.TasteID.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Taste ID");
                                            pm.Skills.TasteID.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Misc));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                        return;
                                    }
                                  
                                    
                                }
                            case 17: // Anatomy
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Anatomy.Base < pm.Skills.Anatomy.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Anatomy");
                                            pm.Skills.Anatomy.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Combat));
                                        return;
                                    }
                                   
                                    
                                }
                            case 18: // Archery
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Archery.Base < pm.Skills.Archery.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Archery");
                                            pm.Skills.Archery.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Combat));
                                        return;
                                    }
                                    
                                    
                                }
                            case 19: // Fencing
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Fencing.Base < pm.Skills.Fencing.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Fencing");
                                            pm.Skills.Fencing.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Combat));
                                        return;
                                    }
                                   
                                    
                                }
                            case 20: // Focus
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Focus.Base < pm.Skills.Focus.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Focus");
                                            pm.Skills.Focus.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Combat));
                                        return;
                                    }
                                    
                                    
                                }
                            case 21: //Mace
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Macing.Base < pm.Skills.Macing.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Macing");
                                            pm.Skills.Macing.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Combat));
                                        return;
                                    }
                                    
                                    
                                }
                            case 22: // Parry
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Parry.Base < pm.Skills.Parry.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Parry");
                                            pm.Skills.Parry.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Combat));
                                        return;
                                    }
                                    
                                    
                                }
                            case 23: // Swords
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Swords.Base < pm.Skills.Swords.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Swords");
                                            pm.Skills.Swords.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Combat));
                                        return;
                                    }
                                    
                                    
                                }
                            case 24: // Tactics
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Tactics.Base < pm.Skills.Tactics.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Tactics");
                                            pm.Skills.Tactics.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Combat));
                                        return;
                                    }
                                   
                                    
                                }
								/*
                            case 25: // Throwing
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Throwing.Base < pm.Skills.Throwing.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Throwing");
                                            pm.Skills.Throwing.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Combat));
                                        return;
                                    }
								}
								*/
                            case 26: // Wrestling
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Wrestling.Base < pm.Skills.Wrestling.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Wrestling");
                                            pm.Skills.Wrestling.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Combat));
                                        return;
                                    }        
                                    
                                }
                            case 27: // Alchemy
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Alchemy.Base < pm.Skills.Alchemy.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Alchemy");
                                            pm.Skills.Alchemy.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Trade));
                                        return;
                                    }
                                    
                                    
                                }
                            case 28: // Blacksmith
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Blacksmith.Base < pm.Skills.Blacksmith.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Blacksmith");
                                            pm.Skills.Blacksmith.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Trade));
                                        return;
                                    }
                                    
                                    
                                }
                            case 29: // Fletching
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Fletching.Base < pm.Skills.Fletching.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Fletching");
                                            pm.Skills.Fletching.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Trade));
                                        return;
                                    }
                                   
                                    
                                }
                            case 30: // Carpentry
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Carpentry.Base < pm.Skills.Carpentry.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Carpentry");
                                            pm.Skills.Carpentry.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Trade));
                                        return;
                                    }
                                    
                                    
                                }
                            case 31: // Cooking
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Cooking.Base < pm.Skills.Cooking.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Cooking");
                                            pm.Skills.Cooking.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Trade));
                                        return;
                                    }
                                    
                                    
                                }
                            case 32: // Inscirbe
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Inscribe.Base < pm.Skills.Inscribe.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Inscribe");
                                            pm.Skills.Inscribe.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Trade));
                                        return;
                                    }
                                
                                    
                                }
								
								
                            case 33: // Lumberjacking
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Lumberjacking.Base < pm.Skills.Lumberjacking.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Lumberjacking");
                                            pm.Skills.Lumberjacking.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Trade));
                                        return;
                                    }
                                    
                                    
                                }
                            case 34: // Mining
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Mining.Base < pm.Skills.Mining.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Mining");
                                            pm.Skills.Mining.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Trade));
                                        return;
                                    }
                                  
                                    
                                }
                            case 35: // Tailoring
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Tailoring.Base < pm.Skills.Tailoring.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Tailoring");
                                            pm.Skills.Tailoring.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Trade));
                                        return;
                                    }
                                   
                                    
                                }
                            case 36: // Tinkering
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Tinkering.Base < pm.Skills.Tinkering.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Tinkering");
                                            pm.Skills.Tinkering.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Trade));
                                        return;
                                    }
                                    
                                    
                                }
                            case 37: // Bushido
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Bushido.Base < pm.Skills.Bushido.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Bushido");
                                            pm.Skills.Bushido.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Magic));
                                        return;
                                    }
                                    
                                    
                                }
                            case 38: // Chivalry
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Chivalry.Base < pm.Skills.Chivalry.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Chivalry");
                                            pm.Skills.Chivalry.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Magic));
                                        return;
                                    }
                                   
                                    
                                }
                            case 39: // Eval Int
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.EvalInt.Base < pm.Skills.EvalInt.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Eval Int");
                                            pm.Skills.EvalInt.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Magic));
                                        return;
                                    }
                                    
                                    
                                }
                            case 40: // Magery
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Magery.Base < pm.Skills.Magery.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Magery");
                                            pm.Skills.Magery.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Magic));
                                        return;
                                    }
                                    
                                    
                                }
                            case 41: // Meditation
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Meditation.Base < pm.Skills.Meditation.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Meditation");
                                            pm.Skills.Meditation.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Magic));
                                        return;
                                    }
                                    
                                    
                                }
                            case 42: // Necromancy
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Necromancy.Base < pm.Skills.Necromancy.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Necromancy");
                                            pm.Skills.Necromancy.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Magic));
                                        return;
                                    }
                                    
                                    
                                }
                            case 43: // Ninjitsu
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Ninjitsu.Base < pm.Skills.Ninjitsu.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Ninjitsu");
                                            pm.Skills.Ninjitsu.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Magic));
                                        return;
                                    }
                                   
                                    
                                }
                            case 44: // Magic Resist
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.MagicResist.Base < pm.Skills.MagicResist.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Magic Resist");
                                            pm.Skills.MagicResist.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Magic));
                                        return;
                                    }
                                    
                                    
                                }
                            case 45: // Spellweaving
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Spellweaving.Base < pm.Skills.Spellweaving.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Spellweaving");
                                            pm.Skills.Spellweaving.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Magic));
                                        return;
                                    }
                                   
                                    
                                }
                            case 46: // Spirit Speak
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.SpiritSpeak.Base < pm.Skills.SpiritSpeak.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Spirit Speak");
                                            pm.Skills.SpiritSpeak.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Magic));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Magic));
                                        return;
                                    }
                              
                                    
                                }
                            case 47: // Animal Lore
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.AnimalLore.Base < pm.Skills.AnimalLore.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Animal Lore");
                                            pm.Skills.AnimalLore.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Wild));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Wild));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Wild));
                                        return;
                                    }
                                   
                                    
                                }
                            case 48: // Taming
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.AnimalTaming.Base < pm.Skills.AnimalTaming.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Animal Taming");
                                            pm.Skills.AnimalTaming.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Wild));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Wild));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Wild));
                                        return;
                                    }
                                   
                                    
                                }
                            case 49: // Fishing
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Fishing.Base < pm.Skills.Fishing.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Fishing");
                                            pm.Skills.Fishing.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Wild));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Wild));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Wild));
                                        return;
                                    }
                                    
                                    
                                }
                            case 50: // Herding 
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Herding.Base < pm.Skills.Herding.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Herding");
                                            pm.Skills.Herding.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Wild));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Wild));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Wild));
                                        return;
                                    }
                                  
                                    
                                }
                            case 51: // Tracking
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Tracking.Base < pm.Skills.Tracking.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Tracking");
                                            pm.Skills.Tracking.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Wild));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Wild));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Wild));
                                        return;
                                    }
                                    
                                    
                                }
                            case 52: // Vet
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Veterinary.Base < pm.Skills.Veterinary.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Vetrinary");
                                            pm.Skills.Veterinary.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Wild));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Wild));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Wild));
                                        return;
                                    }
                                   
                                    
                                }
                            case 53: // Detect Hidden
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.DetectHidden.Base < pm.Skills.DetectHidden.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Detect_Hidden");
                                            pm.Skills.DetectHidden.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Thief));
                                        return;
                                    }
                                   
                                    
                                }
                            case 54: // Hiding
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Hiding.Base < pm.Skills.Hiding.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to hiding");
                                            pm.Skills.Hiding.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Thief));
                                        return;
                                    }
                                    
                                    
                                }
                            case 55: // Lock Picking
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Lockpicking.Base < pm.Skills.Lockpicking.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Lock Picking");
                                            pm.Skills.Lockpicking.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Thief));
                                        return;
                                    }
                                   
                                    
                                }
                            case 56: // Poisoning
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Poisoning.Base < pm.Skills.Poisoning.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Poisoning");
                                            pm.Skills.Poisoning.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Thief));
                                        return;
                                    }
                                   
                                    
                                }
                            case 57: // Remove Trap
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.RemoveTrap.Base < pm.Skills.RemoveTrap.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Remove Traps");
                                            pm.Skills.RemoveTrap.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Thief));
                                        return;
                                    }
                                    
                                    
                                }
                            case 58: // Snooping
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Snooping.Base < pm.Skills.Snooping.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Snooping");
                                            pm.Skills.Snooping.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Thief));
                                        return;
                                    }
                                    
                                    
                                }
                            case 59: // Stealing
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Stealing.Base < pm.Skills.Stealing.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Stealing");
                                            pm.Skills.Stealing.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Thief));
                                        return;
                                    }
                                    
                                    
                                }
                            case 60: // Stealth
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Stealth.Base < pm.Skills.Stealth.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Stealth");
                                            pm.Skills.Stealth.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Thief));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Thief));
                                        return;
                                    }
                                   
                                    
                                }
                            case 61: // Discord
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Discordance.Base < pm.Skills.Discordance.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Discordance");
                                            pm.Skills.Discordance.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Bard));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Bard));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Bard));
                                        return;
                                    }
                                   
                                    
                                }
                            case 62: // Musicianship
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Musicianship.Base < pm.Skills.Musicianship.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Musicianship");
                                            pm.Skills.Musicianship.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Bard));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Bard));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Bard));
                                        return;
                                    }
                                    
                                    
                                }
                            case 63: // Peacemaking
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Peacemaking.Base < pm.Skills.Peacemaking.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Peacemaking");
                                            pm.Skills.Peacemaking.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Bard));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Bard));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Bard));
                                        return;
                                    }
                                   
                                    
                                }
                            case 64: // Provocation
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Provocation.Base < pm.Skills.Provocation.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Provocation");
                                            pm.Skills.Provocation.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Bard));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Bard));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Bard));
                                        return;
                                    }
                                   
                                    
                                }
                            case 65: // Stats
                                {
                                    m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Misc));
                                    break;
                                }
                            case 66: //Healing
							{
                                if (AvlSkillPoints > 0)
                                {
                                    if (pm.Skills.Healing.Base < pm.Skills.Healing.Cap)
                                    {
                                        m_From.SendMessage("One Skill point has been added to Healing");
                                        pm.Skills.Healing.Base += 1;
                                        AvlSkillPoints -= 1;
                                        pm.charSKPoints = AvlSkillPoints;
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                        return;
                                    }
                                    pm.SendMessage("You have reached the cap in this skill");
                                    pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Combat));
                                    return;
                                }
                                else
                                {
                                    m_From.SendMessage("You do not have any available skill points left");
                                    m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Combat));
                                    return;
                                }
                                
                           

							}
							/*
						 //Comment out if your server does not support SA
							    case 67: // Imbueing
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Imbueing.Base < pm.Skills.Imbueing.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Imbueing");
                                            pm.Skills.Imbueing.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Trade));
                                        return;
                                    }
                                   
                                    
                                }
								
								//Comment out if your server does not support SA
							    case 68: // Mysticism
                                {
                                    if (AvlSkillPoints > 0)
                                    {
                                        if (pm.Skills.Mysticism.Base < pm.Skills.Mysticism.Cap)
                                        {
                                            m_From.SendMessage("One Skill point has been added to Mysticism");
                                            pm.Skills.Mysticism.Base += 1;
                                            AvlSkillPoints -= 1;
                                            pm.charSKPoints = AvlSkillPoints;
                                            m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                            return;
                                        }
                                        pm.SendMessage("You have reached the cap in this skill");
                                        pm.SendGump(new PlayerLevelGump(m_From, GumpPage.SkillList, SkillCategory.Trade));
                                        return;
                                    }
                                    else
                                    {
                                        m_From.SendMessage("You do not have any available skill points left");
                                        m_From.SendGump(new PlayerLevelGump(m_From, GumpPage.None, SkillCategory.Trade));
                                        return;
                                    }
                                   
                                    
                                }
								*/
						}		
								//End comment
                        break;
                    }
            }
        }
    
    
    
    }
   
}