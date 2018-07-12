using System;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class HeplerPaulson : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBHepler());
        }        

        [Constructable]
        public HeplerPaulson()
            : base("The Salvage Master")
        {
            Name = "Hepler Paulson";
            Race = Race.Human;
            CantWalk = true;
            Hue = Utility.RandomSkinHue();
            Blessed = true;

            Utility.AssignRandomHair(this);
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Shoes(0x737));
            AddItem(new LongPants(0x1BB));
            AddItem(new FancyShirt(0x535));
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

            if(pm.Young)
            {
                m.SendLocalizedMessage(502593); // Thou art too young to choose this fate.
                return;
            }

            Item boots = m.Backpack.FindItemByType(typeof(BootsOfBallast));
            Item robe = m.Backpack.FindItemByType(typeof(CanvassRobe));
            Item neck = m.Backpack.FindItemByType(typeof(AquaPendant));
            Item lens = m.Backpack.FindItemByType(m.Race == Race.Gargoyle ? typeof(GargishNictitatingLens) : typeof(NictitatingLens));

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.None)
            {
                if (!m.HasGump(typeof(HeplerPaulsonGump)))
                {
                    m.SendGump(new HeplerPaulsonGump(m));
                    pm.ExploringTheDeepQuest = ExploringTheDeepQuestChain.HeplerPaulson;
                }
            }
            else if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent && boots != null && robe != null && neck != null && lens != null)
            {
                pm.ExploringTheDeepQuest = ExploringTheDeepQuestChain.CollectTheComponentComplete;
                m.AddToBackpack(new UnknownShipwreck());
            }
            else if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponentComplete)
            {
                pm.SendGump(new HeplerPaulsonCollectCompleteGump(m));
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
                if (dropped is BrokenShipwreckRemains)
                {
                    if (m.ExploringTheDeepQuest == ExploringTheDeepQuestChain.HeplerPaulson)
                    {
                        dropped.Delete();
                        m.SendGump(new HeplerPaulsonCompleteGump(m));
                        m.ExploringTheDeepQuest = ExploringTheDeepQuestChain.HeplerPaulsonComplete;
                    }
                    else if (m.ExploringTheDeepQuest >= ExploringTheDeepQuestChain.HeplerPaulsonComplete)
                    {
                        m.SendLocalizedMessage(1154320); // You've already completed this task.
                    }
                    else
                    {
                        m.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
                    }
                }
                else
                {
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, 501550); // I am not interested in 
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
            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddImageTiled(50, 20, 400, 460, 0x1404);
            AddImageTiled(50, 29, 30, 450, 0x28DC);
            AddImageTiled(34, 140, 17, 339, 0x242F);
            AddImage(48, 135, 0x28AB);
            AddImage(-16, 285, 0x28A2);
            AddImage(0, 10, 0x28B5);
            AddImage(25, 0, 0x28B4);
            AddImageTiled(83, 15, 350, 15, 0x280A);
            AddImage(34, 479, 0x2842);
            AddImage(442, 479, 0x2840);
            AddImageTiled(51, 479, 392, 17, 0x2775);
            AddImageTiled(415, 29, 44, 450, 0xA2D);
            AddImageTiled(415, 29, 30, 450, 0x28DC);
            AddImage(370, 50, 0x589);

            AddImage(379, 60, 0x15A9);
            AddImage(425, 0, 0x28C9);
            AddImage(90, 33, 0x232D);
            AddImageTiled(130, 65, 175, 1, 0x238D);

            AddHtmlLocalized(140, 45, 250, 24, 1154327, 0x7FFF, false, false); // Exploring the Deep

            AddPage(1);
            AddHtmlLocalized(107, 140, 300, 150, 1154279, 0x7FFF, false, true); // Greetings! My have my pockets become burdened with gold! Not that this is a bad thing of course...*grins*  So ye seek to cash in salvaging many wrecks that now litter the sea floor? Well I'll tell you what...buy some of this here salvage gear and go find yerself a wreck.  Return to me when ye have a piece o' wreckage.
            
            AddHtmlLocalized(145, 300, 250, 24, 1154280, 0x7FFF, false, false); // What of all these shipwrecks?
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

            AddHtmlLocalized(145, 320, 250, 24, 1154282, 0x7FFF, false, false); // How do I use salvage gear?
            AddButton(115, 320, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 3);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(2);
            AddHtmlLocalized(107, 140, 300, 150, 1154281, 0x7FFF, false, true); // Almost uncanny how many wrecks now dot the seascape, none can say for sure why there are so many as of late...sure a drunk skipper here or there will bring a ship down quick...but ..well why question good fortune I always say!

            AddHtmlLocalized(145, 300, 250, 24, 1154282, 0x7FFF, false, false); // How do I use salvage gear?
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 4);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(3);
            AddHtmlLocalized(107, 140, 300, 150, 1154283, 0x7FFF, false, true); // Buy a salvage hook from me, head to deep water and hoist the hook overboard.  With any luck you'll pull out a bit o' wreckage and if fortune smiles on you, a bit o' treasure too!

            AddHtmlLocalized(145, 300, 250, 24, 1154280, 0x7FFF, false, false); // What of all these shipwrecks?
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 5);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(4);
            AddHtmlLocalized(107, 140, 300, 150, 1154283, 0x7FFF, false, true); // Buy a salvage hook from me, head to deep water and hoist the hook overboard.  With any luck you'll pull out a bit o' wreckage and if fortune smiles on you, a bit o' treasure too!

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(5);
            AddHtmlLocalized(107, 140, 300, 150, 1154281, 0x7FFF, false, true); // Almost uncanny how many wrecks now dot the seascape, none can say for sure why there are so many as of late...sure a drunk skipper here or there will bring a ship down quick...but ..well why question good fortune I always say!

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

        }

        public override void OnResponse(NetState state, RelayInfo info) //Function for GumpButtonType.Reply Buttons 
        {
            PlayerMobile pm = state.Mobile as PlayerMobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
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
            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddImageTiled(50, 20, 400, 460, 0x1404);
            AddImageTiled(50, 29, 30, 450, 0x28DC);
            AddImageTiled(34, 140, 17, 339, 0x242F);
            AddImage(48, 135, 0x28AB);
            AddImage(-16, 285, 0x28A2);
            AddImage(0, 10, 0x28B5);
            AddImage(25, 0, 0x28B4);
            AddImageTiled(83, 15, 350, 15, 0x280A);
            AddImage(34, 479, 0x2842);
            AddImage(442, 479, 0x2840);
            AddImageTiled(51, 479, 392, 17, 0x2775);
            AddImageTiled(415, 29, 44, 450, 0xA2D);
            AddImageTiled(415, 29, 30, 450, 0x28DC);
            AddImage(370, 50, 0x589);

            AddImage(379, 60, 0x15A9);
            AddImage(425, 0, 0x28C9);
            AddImage(90, 33, 0x232D);
            AddImageTiled(130, 65, 175, 1, 0x238D);

            AddHtmlLocalized(140, 45, 250, 24, 1154327, 0x7FFF, false, false); // Exploring the Deep

            AddPage(1);
            AddHtmlLocalized(107, 140, 300, 150, 1154284, 0x7FFF, false, true); // Well well! Lookie here! That's a fine pull indeed!  Shame we can only salvage with hooks and the like...if only we could peak beneath the waves and score the big hauls! Say, I heard once of a Master Tinker that may be able to tell of a suit specially made for exploring deep ship wrecks...if you manage to get your hands on one of those suits return to me and I'll share a map with you showing the location of a secret wreck only I know about!

            AddHtmlLocalized(145, 300, 250, 24, 1154285, 0x7FFF, false, false); // Who is the Master Tinker?
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(2);
            AddHtmlLocalized(107, 140, 300, 150, 1154286, 0x7FFF, false, true); // I've only heard they live in East Britain, if ye find them I bet ye may convince em to tell you of the suit!

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

    public class HeplerPaulsonCollectCompleteGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("HeplerPaulsonCollectComplete", AccessLevel.GameMaster, new CommandEventHandler(HeplerPaulsonCollectCompleteGump_OnCommand));
        }

        private static void HeplerPaulsonCollectCompleteGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new HeplerPaulsonCollectCompleteGump(e.Mobile));
        }

        public HeplerPaulsonCollectCompleteGump(Mobile owner) : base(50, 50)
        {
            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddImageTiled(50, 20, 400, 460, 0x1404);
            AddImageTiled(50, 29, 30, 450, 0x28DC);
            AddImageTiled(34, 140, 17, 339, 0x242F);
            AddImage(48, 135, 0x28AB);
            AddImage(-16, 285, 0x28A2);
            AddImage(0, 10, 0x28B5);
            AddImage(25, 0, 0x28B4);
            AddImageTiled(83, 15, 350, 15, 0x280A);
            AddImage(34, 479, 0x2842);
            AddImage(442, 479, 0x2840);
            AddImageTiled(51, 479, 392, 17, 0x2775);
            AddImageTiled(415, 29, 44, 450, 0xA2D);
            AddImageTiled(415, 29, 30, 450, 0x28DC);
            AddImage(370, 50, 0x589);

            AddImage(379, 60, 0x15A9);
            AddImage(425, 0, 0x28C9);
            AddImage(90, 33, 0x232D);
            AddImageTiled(130, 65, 175, 1, 0x238D);

            AddHtmlLocalized(140, 45, 250, 24, 1154327, 0x7FFF, false, false); // Exploring the Deep

            AddPage(1);
            AddHtmlLocalized(107, 140, 300, 150, 1154319, 0x7FFF, false, true); // Neptune's trident! I can't believe ye got the suit!  Well best be off then!  Here's a map to the location where I heard a big wreck might be.  Best of luck!

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0); //OK
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