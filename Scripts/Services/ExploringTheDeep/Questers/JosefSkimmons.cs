using System;
using Server.Items;
using Server.Network;
using Server.Engines.Quests;
using Server.Commands;
using Server.Gumps;

namespace Server.Mobiles
{    
	public class JosefSkimmons : Mobile
    {
        public virtual bool IsInvulnerable { get { return true; } }

        [Constructable]
		public JosefSkimmons()
        {
            Name = "Josef Skimmons";
			Title = "The Master Blacksmith";
            Female = false;
            Race = Race.Human;
            this.Blessed = true;

            CantWalk = true;
            Hue = Utility.RandomSkinHue();
            Utility.AssignRandomHair(this);

            AddItem(new Backpack());
            AddItem(new Sandals(2017));
            AddItem(new LongPants(2017));
            AddItem(new FullApron(1322));

            Item gloves = new LeatherGloves();
            gloves.Hue = 1;
            AddItem(gloves);

            Item weapon = new SmithHammer();
            weapon.Hue = 1;
            AddItem(weapon);
		}

        public JosefSkimmons(Serial serial): base(serial)
		{		
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!(m is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)m;

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.HeplerPaulson)
            {
                if (!m.HasGump(typeof(JosefSkimmonsGump)))
                {
                    m.SendGump(new JosefSkimmonsGump(m));
                }
            }
            else
            {
                m.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile m = from as PlayerMobile;

            if (m != null)
            {
                if (m.ExploringTheDeepQuest == ExploringTheDeepQuestChain.HeplerPaulson)
                {
                    if (dropped is OrcishSchematics)
                    {
                        m.ExploringTheDeepQuest = ExploringTheDeepQuestChain.CusteauPerron;
                        dropped.Delete();
                        from.AddToBackpack(new NictitatingLens());

                        if (!m.HasGump(typeof(JosefSkimmonsCompleteGump)))
                        {
                            m.SendGump(new JosefSkimmonsCompleteGump(m));
                        }
                    }
                    else
                    {
                        this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501550); // I am not interested in this.
                    }
                }
                else
                {
                    m.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
                }
            }
            return false;
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
	}
}

namespace Server.Gumps
{
    public class JosefSkimmonsCompleteGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("JosefSkimmonsComplete", AccessLevel.GameMaster, new CommandEventHandler(JosefSkimmonsCompleteGump_OnCommand));
        }

        private static void JosefSkimmonsCompleteGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new JosefSkimmonsCompleteGump(e.Mobile));
        }

        public JosefSkimmonsCompleteGump(Mobile owner) : base(50, 50)
        {
            //-------------------------------------------------------------------------------------------------
            AddPage(0);
            AddImageTiled(54, 33, 369, 400, 2624);
            AddAlphaRegion(54, 33, 369, 400);
            AddImageTiled(416, 39, 44, 389, 203);
            //--------------------------------------Window size bar--------------------------------------------

            AddImage(97, 49, 9005);
            AddImageTiled(58, 39, 29, 390, 10460);
            AddImageTiled(412, 37, 31, 389, 10460);
            AddHtmlLocalized(140, 60, 250, 24, 1154327, 0x7FFF, false, false); // Exploring the Deep
            AddImage(430, 9, 10441);
            AddImageTiled(40, 38, 17, 391, 9263);
            AddImage(6, 25, 10421);
            AddImage(34, 12, 10420);
            AddImageTiled(94, 25, 342, 15, 10304);
            AddImageTiled(40, 427, 415, 16, 10304);
            AddImage(-10, 314, 10402);
            AddImage(56, 150, 10411);
            AddImage(136, 84, 96);

            AddPage(1);
            AddHtmlLocalized(107, 110, 300, 230, 1154299, 0x7FFF, false, true); // Great work! *Reading the schematic*  Excellent!  This is just what I need to give me a leg up on the competition! As promised here are those lenses you requested!
            
            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
        }

        public override void OnResponse(NetState state, RelayInfo info) //Function for GumpButtonType.Reply Buttons 
        {
            Mobile from = state.Mobile;
            from.Frozen = false;
            switch (info.ButtonID)
            {
                case 0:
                    {
                        //Cancel
                        break;
                    }
            }
        }
    }

    public class JosefSkimmonsGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("JosefSkimmons", AccessLevel.GameMaster, new CommandEventHandler(JosefSkimmonsGump_OnCommand));
        }

        private static void JosefSkimmonsGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new JosefSkimmonsGump(e.Mobile));
        }

        public JosefSkimmonsGump(Mobile owner) : base(50, 50)
        {
            //-------------------------------------------------------------------------------------------------
            AddPage(0);
            AddImageTiled(54, 33, 369, 400, 2624);
            AddAlphaRegion(54, 33, 369, 400);
            AddImageTiled(416, 39, 44, 389, 203);
            //--------------------------------------Window size bar--------------------------------------------

            AddImage(97, 49, 9005);
            AddImageTiled(58, 39, 29, 390, 10460);
            AddImageTiled(412, 37, 31, 389, 10460);
            AddHtmlLocalized(140, 60, 250, 24, 1154327, 0x7FFF, false, false); // Exploring the Deep
            AddImage(430, 9, 10441);
            AddImageTiled(40, 38, 17, 391, 9263);
            AddImage(6, 25, 10421);
            AddImage(34, 12, 10420);
            AddImageTiled(94, 25, 342, 15, 10304);
            AddImageTiled(40, 427, 415, 16, 10304);
            AddImage(-10, 314, 10402);
            AddImage(56, 150, 10411);
            AddImage(136, 84, 96);

            AddPage(1);
            AddHtmlLocalized(107, 110, 300, 230, 1154294, 0x7FFF, false, true); // Hello there...what’s this? *reads the note* Look at these lenses! Cousteau drew these up did she...hrmm…well in any case I might be able to craft these lenses if you would be willing to assist me...

            AddHtmlLocalized(155, 260, 250, 24, 1154295, 0x7FFF, false, false); // What do you need assistance with?
            AddButton(125, 260, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(2);
            AddHtmlLocalized(107, 110, 300, 230, 1154296, 0x7FFF, false, true); // I’m looking to expand my operation here, before I can do that though I need a more reliable source of raw ore.  Rumor has it the Orcs have a new machine they are using to drill huge quantities of ore!

            AddHtmlLocalized(155, 260, 250, 24, 1154297, 0x7FFF, false, false); // An orcish machine?
            AddButton(125, 260, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 3);

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(3);
            AddHtmlLocalized(107, 110, 300, 230, 1154298, 0x7FFF, false, true); // Yes! That’s exactly what I’m talking about! Drill baby drill! You get me some schematics to make that machine and I’ll strike your lenses for you...I bet those Orcs are holed up in their cave outside Yew.
                        
            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
        }

        public override void OnResponse(NetState state, RelayInfo info) //Function for GumpButtonType.Reply Buttons 
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        //Cancel 
                        break;
                    }
            }
        }
    }
}