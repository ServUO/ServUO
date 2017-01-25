using System;
using Server.Commands;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Engines.Quests;

namespace Server.Mobiles
{    
	public class CousteauPerron : Mobile
	{	
        public virtual bool IsInvulnerable {  get { return true; } }
		
		[Constructable]
		public CousteauPerron()
        {
            this.Name = "Cousteau Perron";
            this.Title = "The Master Tinker";
            this.Female = true;
            this.Race = Race.Human;
            this.Blessed = true;

            this.CantWalk = true;
            this.Hue = Utility.RandomSkinHue();
            Utility.AssignRandomHair(this);

            this.AddItem(new Backpack());
            this.AddItem(new FurBoots(2017));
            this.AddItem(new LongPants(2017));
            this.AddItem(new Doublet(1326));
            this.AddItem(new LongHair(2213));
            this.AddItem(new Cloak(2017));
            this.AddItem(new Cap(398));

            Item gloves = new LeatherGloves();
            gloves.Hue = 2213;
            AddItem(gloves);
        }

        public CousteauPerron(Serial serial): base(serial)
		{		
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072269); // Quest Giver
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!(m is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)m;

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.HeplerPaulson)
            {
                if (!m.HasGump(typeof(CousteauPerronGump)))
                {
                    m.SendGump(new CousteauPerronGump(m));
                }
            }
            else if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CusteauPerron)
            {
                if (!m.HasGump(typeof(CousteauPerronCompleteGump)))
                {
                    m.SendGump(new CousteauPerronCompleteGump(m));
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
                if (dropped is IceWyrmScale && m.ExploringTheDeepQuest == ExploringTheDeepQuestChain.HeplerPaulson)
                {
                    m.ExploringTheDeepQuest = ExploringTheDeepQuestChain.CusteauPerron;
                    dropped.Delete();
                }
                else if (dropped is SalvagerSuitPlans && m.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CusteauPerron)
                {
                    m.ExploringTheDeepQuest = ExploringTheDeepQuestChain.CusteauPerron;
                    dropped.Delete();

                    m.SendGump(new CousteauPerronPlansGump(m));                    
                }
                else
                {
                    this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501550); // I am not interested in this.
                }
            }
            return false;
        }

        public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();	
		}        
	}
}

namespace Server.Gumps
{
    public class CousteauPerronGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("CousteauPerron", AccessLevel.GameMaster, new CommandEventHandler(CousteauPerronGump_OnCommand));
        }

        private static void CousteauPerronGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new CousteauPerronGump(e.Mobile));
        }

        public CousteauPerronGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 110, 300, 230, 1154287, 0x7FFF, false, true); // *She looks up from the slowly rotating rabbit on a spit* Oh hello there, what brings you way out to this frozen tundra?

            AddHtmlLocalized(155, 260, 250, 24, 1154288, 0x7FFF, false, false); // Hepler sent me to ask about Shipwrecks...
            AddButton(125, 260, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(2);
            AddHtmlLocalized(107, 110, 300, 230, 1154289, 0x7FFF, false, true); // Ahh, been listening to old Hepler have you?  So I've heard something of these shipwrecks, even all the way out here.  I'm guessing you want me to tell you of one of these suits to help you explore beneath the waves?

            AddHtmlLocalized(155, 260, 250, 24, 1154290, 0x7FFF, false, false); // Do you know of the suit?            
            AddButton(125, 280, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 3);//Suit

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(3);
            AddHtmlLocalized(107, 110, 300, 230, 1154291, 0x7FFF, false, true); // Before I will help you with this perhaps you can assist me with a small task?  I require an Ice Wyrm Scale to assist me in my research, but sadly I am not strong enough to slay one alone.  Do you think you could collect this for me? Yes? Oh grand! Return to me with the ice wyrm scale and I will assist thee further.

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

    public class CousteauPerronCompleteGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("CousteauPerronComplete", AccessLevel.GameMaster, new CommandEventHandler(CousteauPerronCompleteGump_OnCommand));
        }

        private static void CousteauPerronCompleteGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new CousteauPerronCompleteGump(e.Mobile));
        }

        public CousteauPerronCompleteGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 110, 300, 230, 1154292, 0x7FFF, false, true); // You've got one! I don't know how but you did!  This will prove invaluable in my research! I will do as promised.  Long ago I learned of a set of items known as the Salvager's Suit.  It had been quite some time since anyone had seen the actual plans - or a suit for that matter, but legend tells any who wear the suit will be aided in undersea exploration.  Last anyone heard the plans could be found deep within the Sorcerer's Dungeon in Ilshenar.

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
        }

        public override void OnResponse(NetState state, RelayInfo info)
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

    public class CousteauPerronPlansGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("CousteauPerronPlans", AccessLevel.GameMaster, new CommandEventHandler(CousteauPerronPlansGump_OnCommand));
        }

        private static void CousteauPerronPlansGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new CousteauPerronPlansGump(e.Mobile));
        }

        public CousteauPerronPlansGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 110, 300, 230, 1154293, 0x7FFF, false, true); // This is exactly what I was talking about! How did you ever find such a thing! No matter! *reads the plans carefully*  It’s all here by golly, detailed instructions on how to craft each item.  There are a number of professionals throughout the realm who will be able to assist you in crafting such things – I’ve written them down on this list here *hands you a note* Simply seek the professionals I have listed and you should be well on your way!

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        from.AddToBackpack(new CusteauPerronNote());
                        break;
                    }
            }
        }
    }
}