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

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CusteauPerronHouse)
            {
                if (!m.HasGump(typeof(CousteauPerronGump)))
                {
                    m.SendGump(new CousteauPerronGump(m));
                    pm.ExploringTheDeepQuest = ExploringTheDeepQuestChain.CusteauPerron;
                }
            }
            else if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.Sorcerers)
            {
                if (!m.HasGump(typeof(CousteauPerronCompleteGump)))
                {
                    m.SendGump(new CousteauPerronCompleteGump(m));
                }
            }
            else if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
            {
                if (!m.HasGump(typeof(CousteauPerronPlansGump)))
                {
                    m.SendGump(new CousteauPerronPlansGump(m));
                }
            }
            else
            {
                m.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile pm = from as PlayerMobile;            

            if (pm != null)
            {
                if (pm.ExploringTheDeepQuest < ExploringTheDeepQuestChain.CusteauPerron)
                {
                    pm.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
                }
                else if (dropped is IceWyrmScale && pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CusteauPerron)
                {
                    pm.ExploringTheDeepQuest = ExploringTheDeepQuestChain.Sorcerers;
                    dropped.Delete();
                    pm.SendGump(new CousteauPerronCompleteGump(pm));
                }
                else if (dropped is SalvagerSuitPlans && pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.Sorcerers)
                {
                    pm.ExploringTheDeepQuest = ExploringTheDeepQuestChain.CollectTheComponent;
                    dropped.Delete();
                    pm.SendGump(new CousteauPerronPlansGump(pm));
                    pm.AddToBackpack(new CusteauPerronNote());
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
            this.Closable = false;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            this.AddImageTiled(50, 20, 400, 460, 0x1404);
            this.AddImageTiled(50, 29, 30, 450, 0x28DC);
            this.AddImageTiled(34, 140, 17, 339, 0x242F);
            this.AddImage(48, 135, 0x28AB);
            this.AddImage(-16, 285, 0x28A2);
            this.AddImage(0, 10, 0x28B5);
            this.AddImage(25, 0, 0x28B4);
            this.AddImageTiled(83, 15, 350, 15, 0x280A);
            this.AddImage(34, 479, 0x2842);
            this.AddImage(442, 479, 0x2840);
            this.AddImageTiled(51, 479, 392, 17, 0x2775);
            this.AddImageTiled(415, 29, 44, 450, 0xA2D);
            this.AddImageTiled(415, 29, 30, 450, 0x28DC);
            this.AddImage(370, 50, 0x589);

            this.AddImage(379, 60, 0x15A9);
            this.AddImage(425, 0, 0x28C9);
            this.AddImage(90, 33, 0x232D);
            this.AddImageTiled(130, 65, 175, 1, 0x238D);

            AddHtmlLocalized(140, 45, 250, 24, 1154327, 0x7FFF, false, false); // Exploring the Deep

            AddPage(1);
            AddHtmlLocalized(107, 140, 300, 150, 1154287, 0x7FFF, false, true); // *She looks up from the slowly rotating rabbit on a spit* Oh hello there, what brings you way out to this frozen tundra?

            AddHtmlLocalized(145, 300, 270, 24, 1154288, 0x7FFF, false, false); // Hepler sent me to ask about Shipwrecks...
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(2);
            AddHtmlLocalized(107, 140, 300, 150, 1154289, 0x7FFF, false, true); // Ahh, been listening to old Hepler have you?  So I've heard something of these shipwrecks, even all the way out here.  I'm guessing you want me to tell you of one of these suits to help you explore beneath the waves?

            AddHtmlLocalized(145, 300, 250, 24, 1154290, 0x7FFF, false, false); // Do you know of the suit?            
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 3);//Suit

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(3);
            AddHtmlLocalized(107, 140, 300, 150, 1154291, 0x7FFF, false, true); // Before I will help you with this perhaps you can assist me with a small task?  I require an Ice Wyrm Scale to assist me in my research, but sadly I am not strong enough to slay one alone.  Do you think you could collect this for me? Yes? Oh grand! Return to me with the ice wyrm scale and I will assist thee further.

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

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
            this.Closable = false;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            this.AddImageTiled(50, 20, 400, 460, 0x1404);
            this.AddImageTiled(50, 29, 30, 450, 0x28DC);
            this.AddImageTiled(34, 140, 17, 339, 0x242F);
            this.AddImage(48, 135, 0x28AB);
            this.AddImage(-16, 285, 0x28A2);
            this.AddImage(0, 10, 0x28B5);
            this.AddImage(25, 0, 0x28B4);
            this.AddImageTiled(83, 15, 350, 15, 0x280A);
            this.AddImage(34, 479, 0x2842);
            this.AddImage(442, 479, 0x2840);
            this.AddImageTiled(51, 479, 392, 17, 0x2775);
            this.AddImageTiled(415, 29, 44, 450, 0xA2D);
            this.AddImageTiled(415, 29, 30, 450, 0x28DC);
            this.AddImage(370, 50, 0x589);

            this.AddImage(379, 60, 0x15A9);
            this.AddImage(425, 0, 0x28C9);
            this.AddImage(90, 33, 0x232D);
            this.AddImageTiled(130, 65, 175, 1, 0x238D);

            AddHtmlLocalized(140, 45, 250, 24, 1154327, 0x7FFF, false, false); // Exploring the Deep

            AddPage(1);
            AddHtmlLocalized(107, 140, 300, 150, 1154292, 0x7FFF, false, true); // You've got one! I don't know how but you did!  This will prove invaluable in my research! I will do as promised.  Long ago I learned of a set of items known as the Salvager's Suit.  It had been quite some time since anyone had seen the actual plans - or a suit for that matter, but legend tells any who wear the suit will be aided in undersea exploration.  Last anyone heard the plans could be found deep within the Sorcerer's Dungeon in Ilshenar.

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
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
            this.Closable = false;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            this.AddImageTiled(50, 20, 400, 460, 0x1404);
            this.AddImageTiled(50, 29, 30, 450, 0x28DC);
            this.AddImageTiled(34, 140, 17, 339, 0x242F);
            this.AddImage(48, 135, 0x28AB);
            this.AddImage(-16, 285, 0x28A2);
            this.AddImage(0, 10, 0x28B5);
            this.AddImage(25, 0, 0x28B4);
            this.AddImageTiled(83, 15, 350, 15, 0x280A);
            this.AddImage(34, 479, 0x2842);
            this.AddImage(442, 479, 0x2840);
            this.AddImageTiled(51, 479, 392, 17, 0x2775);
            this.AddImageTiled(415, 29, 44, 450, 0xA2D);
            this.AddImageTiled(415, 29, 30, 450, 0x28DC);
            this.AddImage(370, 50, 0x589);

            this.AddImage(379, 60, 0x15A9);
            this.AddImage(425, 0, 0x28C9);
            this.AddImage(90, 33, 0x232D);
            this.AddImageTiled(130, 65, 175, 1, 0x238D);

            AddHtmlLocalized(140, 45, 250, 24, 1154327, 0x7FFF, false, false); // Exploring the Deep

            AddPage(1);
            AddHtmlLocalized(107, 140, 300, 150, 1154293, 0x7FFF, false, true); // This is exactly what I was talking about! How did you ever find such a thing! No matter! *reads the plans carefully*  It’s all here by golly, detailed instructions on how to craft each item.  There are a number of professionals throughout the realm who will be able to assist you in crafting such things – I’ve written them down on this list here *hands you a note* Simply seek the professionals I have listed and you should be well on your way!

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        break;
                    }
            }
        }
    }
}