using System;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.ContextMenus;
using Server;
using Server.Commands;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class HeplerPaulson : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return this.m_SBInfos; } }

        public override void InitSBInfo()
        {
            this.m_SBInfos.Add(new SBHepler());
        }        

        [Constructable]
        public HeplerPaulson()
            : base("The Salvage Master")
        {
            this.Name = "Hepler Paulson";
            this.Race = Race.Human;
            this.CantWalk = true;
            this.Hue = Utility.RandomSkinHue();
            this.Blessed = true;

            AddItem(new Backpack());
            AddItem(new Shoes(0x737));
            AddItem(new LongPants(0x1BB));
            AddItem(new FancyShirt(0x535));
            Utility.AssignRandomHair(this);
        }

        public HeplerPaulson(Serial serial) : base(serial)
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

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.None)
            {
                if (!m.HasGump(typeof(HeplerPaulsonGump)))
                {
                    m.SendGump(new HeplerPaulsonGump(m));
                }
            }
            else
            {
                if (!m.HasGump(typeof(HeplerPaulsonCompleteGump)))
                {
                    m.SendGump(new HeplerPaulsonCompleteGump(m));
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }        

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile m = from as PlayerMobile;

            if (m != null)
            {
                if (dropped is BrokenShipwreckRemains && m.ExploringTheDeepQuest == ExploringTheDeepQuestChain.None)
                {
                    m.ExploringTheDeepQuest = ExploringTheDeepQuestChain.CusteauPerron;
                    dropped.Delete();
                }
                else
                {
                    this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501550); // I am not interested in this.
                }
            }
            return false;
        }
    }

    public class HeplerPaulsonGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("HeplerPaulson", AccessLevel.GameMaster, new CommandEventHandler(HeplerPaulsonGump_OnCommand));
        }

        private static void HeplerPaulsonGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new HeplerPaulsonGump(e.Mobile));
        }

        public HeplerPaulsonGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 110, 300, 230, 1154279, 0x7FFF, false, true); // Greetings! My have my pockets become burdened with gold! Not that this is a bad thing of course...*grins*  So ye seek to cash in salvaging many wrecks that now litter the sea floor? Well I'll tell you what...buy some of this here salvage gear and go find yerself a wreck.  Return to me when ye have a piece o' wreckage.

            AddHtmlLocalized(155, 260, 250, 24, 1154280, 0x7FFF, false, false); // What of all these shipwrecks?
            AddButton(125, 260, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

            AddHtmlLocalized(155, 280, 250, 24, 1154282, 0x7FFF, false, false); // How do I use salvage gear?
            AddButton(125, 280, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 3);

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(2);
            AddHtmlLocalized(107, 110, 300, 230, 1154281, 0x7FFF, false, true); // Almost uncanny how many wrecks now dot the seascape, none can say for sure why there are so many as of late...sure a drunk skipper here or there will bring a ship down quick...but this...well why question good fortune I always say!

            AddHtmlLocalized(155, 260, 250, 24, 1154282, 0x7FFF, false, false); // How do I use salvage gear?
            AddButton(125, 260, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 4);

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(3);
            AddHtmlLocalized(107, 110, 300, 230, 1154283, 0x7FFF, false, true); // Buy a salvage hook from me, head to deep water and hoist the hook overboard.  With any luck you'll pull out a bit o' wreckage and if fortune smiles on you, a bit o' treasure too!

            AddHtmlLocalized(155, 260, 250, 24, 1154280, 0x7FFF, false, false); // What of all these shipwrecks?
            AddButton(125, 260, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 5);

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(4);
            AddHtmlLocalized(107, 110, 300, 230, 1154283, 0x7FFF, false, true); // Buy a salvage hook from me, head to deep water and hoist the hook overboard.  With any luck you'll pull out a bit o' wreckage and if fortune smiles on you, a bit o' treasure too!

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(5);
            AddHtmlLocalized(107, 110, 300, 230, 1154281, 0x7FFF, false, true); // Almost uncanny how many wrecks now dot the seascape, none can say for sure why there are so many as of late...sure a drunk skipper here or there will bring a ship down quick...but this...well why question good fortune I always say!

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

    public class HeplerPaulsonCompleteGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("HeplerPaulsonComplete", AccessLevel.GameMaster, new CommandEventHandler(HeplerPaulsonCompleteGump_OnCommand));
        }

        private static void HeplerPaulsonCompleteGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new HeplerPaulsonCompleteGump(e.Mobile));
        }

        public HeplerPaulsonCompleteGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 110, 300, 230, 1154284, 0x7FFF, false, true); // Well well! Lookie here! That's a fine pull indeed!  Shame we can only salvage with hooks and the like...if only we could peak beneath the waves and score the big hauls! Say, I heard once of a Master Tinker that may be able to tell of a suit specially made for exploring deep ship wrecks...if you manage to get your hands on one of those suits return to me and I'll share a map with you showing the location of a secret wreck only I know about!

            AddHtmlLocalized(155, 260, 250, 24, 0x7FFF, false, false); // Who is the Master Tinker?
            AddButton(125, 260, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(2);
            AddHtmlLocalized(107, 110, 300, 230, 1154284, 0x7FFF, false, true); // I've only heard they live in East Britain, if ye find them I bet ye may convince em to tell you of the suit!

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

    public class HeplerPaulsonCollectCompleteGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("HeplerPaulsonComplete", AccessLevel.GameMaster, new CommandEventHandler(HeplerPaulsonCollectCompleteGump_OnCommand));
        }

        private static void HeplerPaulsonCollectCompleteGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new HeplerPaulsonCompleteGump(e.Mobile));
        }

        public HeplerPaulsonCollectCompleteGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 110, 300, 230, 1154319, 0x7FFF, false, true); // Neptune's trident! I can't believe ye got the suit!  Well best be off then!  Here's a map to the location where I heard a big wreck might be.  Best of luck!

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
}