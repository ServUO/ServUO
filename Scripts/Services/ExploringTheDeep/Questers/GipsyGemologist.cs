using System;
using Server.Items;
using Server.Network;
using Server.Commands;
using Server.Gumps;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class GipsyGemologist : Mobile
    {
        public virtual bool IsInvulnerable { get { return true; } }

        [Constructable]
        public GipsyGemologist() : base()
        {
            this.Name = "Zalia";
            this.Title = "The Gypsy Gemologist";
            this.Female = true;
            this.Race = Race.Human;
            this.Blessed = true;

            this.Hue = Utility.RandomSkinHue();

            this.AddItem(new LongHair(2213));
            this.AddItem(new Backpack());
            this.AddItem(new Shoes(0x737));
            this.AddItem(new Skirt(0x1BB));
            this.AddItem(new FancyShirt(0x535));
            Utility.AssignRandomHair(this);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!(m is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)m;

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
            {
                if (!m.HasGump(typeof(ZaliaQuestGump)))
                {
                    m.SendGump(new ZaliaQuestGump(m));
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
                if (m.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
                {
                    if (dropped is AquaGem)
                    {
                        dropped.Delete();
                        from.AddToBackpack(new AquaPendant());

                        if (!m.HasGump(typeof(ZaliaQuestCompleteGump)))
                        {
                            m.SendGump(new ZaliaQuestCompleteGump(m));
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

        public GipsyGemologist(Serial serial) : base(serial)
        {
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
    }
}

namespace Server.Gumps
{
    public class ZaliaQuestGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("ZaliaQuest", AccessLevel.GameMaster, new CommandEventHandler(ZaliaQuestGump_OnCommand));
        }

        private static void ZaliaQuestGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new ZaliaQuestGump(e.Mobile));
        }

        public ZaliaQuestGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 140, 300, 150, 1154311, 0x7FFF, false, true); // Hello zere my darling - looking for something shiny? Zalia has just vhat you are looking for!

            AddHtmlLocalized(145, 300, 250, 24, 1154312, 0x7FFF, false, false); // I'm looking for a special pendant...            
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(2);
            AddHtmlLocalized(107, 140, 300, 150, 1154313, 0x7FFF, false, true); // *Reads the note from Cousteau*  Oh another one of you zhen eh?  Zha Aqua pendant!  Might as well ask for zha crown jewels! I will craft zhis jewel for you if you acquire zha correct gemstone!

            AddHtmlLocalized(145, 300, 250, 24, 1154314, 0x7FFF, false, false); // Where do I find such a gemstone?
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 3);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(3);
            AddHtmlLocalized(107, 140, 300, 150, 1154315, 0x7FFF, false, true); // Zha Aqua Gem! And I vhant a loaf of bread filled with gold!  *laughs* Oh, you vhere serious?  Well zhen ye must wrestle one avay from zhe Djinn.

            AddHtmlLocalized(145, 300, 250, 24, 1154316, 0x7FFF, false, false); // You want me to wrestle a Djinn!?!
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 4);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(4);
            AddHtmlLocalized(107, 140, 300, 150, 1154317, 0x7FFF, false, true); // Zhey are usually around the winding sandy paths around zhe camp here...oddly zhey are fond of zhe water...*shrugs*

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

    public class ZaliaQuestCompleteGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("ZaliaQuestComplete", AccessLevel.GameMaster, new CommandEventHandler(ZaliaQuestCompleteGump_OnCommand));
        }

        private static void ZaliaQuestCompleteGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new ZaliaQuestCompleteGump(e.Mobile));
        }

        public ZaliaQuestCompleteGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 140, 300, 150, 1154318, 0x7FFF, false, true); // Ahah! Yes, yes, zhat is indeed zhe gem! *does some quick tinkering*  Here is your pendant as you vish...

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
}