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

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.HeplerPaulson)
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
                if (m.ExploringTheDeepQuest == ExploringTheDeepQuestChain.HeplerPaulson)
                {
                    if (dropped is AquaGem)
                    {
                        m.ExploringTheDeepQuest = ExploringTheDeepQuestChain.CusteauPerron;
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
            AddHtmlLocalized(107, 110, 300, 230, 1154311, 0x7FFF, false, true); // Hello zere my darling - looking for something shiny? Zalia has just vhat you are looking for!

            AddHtmlLocalized(155, 260, 250, 24, 1154312, 0x7FFF, false, false); // I'm looking for a special pendant...            
            AddButton(125, 260, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(2);
            AddHtmlLocalized(107, 110, 300, 230, 1154313, 0x7FFF, false, true); // *Reads the note from Cousteau*  Oh another one of you zhen eh?  Zha Aqua pendant!  Might as well ask for zha crown jewels! I will craft zhis jewel for you if you acquire zha correct gemstone!

            AddHtmlLocalized(155, 260, 250, 24, 1154314, 0x7FFF, false, false); // Where do I find such a gemstone?
            AddButton(125, 260, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 3);

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(3);
            AddHtmlLocalized(107, 110, 300, 230, 1154315, 0x7FFF, false, true); // Zha Aqua Gem! And I vhant a loaf of bread filled with gold!  *laughs* Oh, you vhere serious?  Well zhen ye must wrestle one avay from zhe Djinn.

            AddHtmlLocalized(155, 260, 250, 24, 1154316, 0x7FFF, false, false); // You want me to wrestle a Djinn!?!
            AddButton(125, 260, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 4);

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(4);
            AddHtmlLocalized(107, 110, 300, 230, 1154317, 0x7FFF, false, true); // Zhey are usually around the winding sandy paths around zhe camp here...oddly zhey are fond of zhe water...*shrugs*

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
        }

        public override void OnResponse(NetState state, RelayInfo info) 
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
            AddHtmlLocalized(107, 110, 300, 230, 1154318, 0x7FFF, false, true); // Ahah! Yes, yes, zhat is indeed zhe gem! *does some quick tinkering*  Here is your pendant as you vish...
            
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