using Server.Commands;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    public class JosefSkimmons : Mobile
    {
        public virtual bool IsInvulnerable => true;

        [Constructable]
        public JosefSkimmons()
        {
            Name = "Josef Skimmons";
            Title = "The Master Blacksmith";
            Female = false;
            Race = Race.Human;
            Blessed = true;

            CantWalk = true;
            Hue = Utility.RandomSkinHue();
            Utility.AssignRandomHair(this);

            AddItem(new Backpack());
            AddItem(new Sandals(2017));
            AddItem(new LongPants(2017));
            AddItem(new FullApron(1322));

            Item gloves = new LeatherGloves
            {
                Hue = 1
            };
            AddItem(gloves);

            Item weapon = new SmithHammer
            {
                Hue = 1
            };
            AddItem(weapon);
        }

        public JosefSkimmons(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)from;

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
            {
                if (!from.HasGump(typeof(JosefSkimmonsGump)))
                {
                    from.SendGump(new JosefSkimmonsGump(from));
                }
            }
            else
            {
                from.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile m = from as PlayerMobile;

            if (m != null)
            {
                if (m.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
                {
                    if (dropped is OrcishSchematics)
                    {
                        dropped.Delete();

                        if (from.Race == Race.Gargoyle)
                        {
                            from.AddToBackpack(new GargishNictitatingLens());
                        }
                        else
                        {
                            from.AddToBackpack(new NictitatingLens());
                        }

                        if (!m.HasGump(typeof(JosefSkimmonsCompleteGump)))
                        {
                            m.SendGump(new JosefSkimmonsCompleteGump(m));
                        }
                    }
                    else
                    {
                        PublicOverheadMessage(MessageType.Regular, 0x3B2, 501550); // I am not interested in this.
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
            writer.Write(0); // version

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
            CommandSystem.Register("JosefSkimmonsComplete", AccessLevel.GameMaster, JosefSkimmonsCompleteGump_OnCommand);
        }

        private static void JosefSkimmonsCompleteGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new JosefSkimmonsCompleteGump(e.Mobile));
        }

        public JosefSkimmonsCompleteGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 140, 300, 150, 1154299, 0x7FFF, false, true); // Great work! *Reading the schematic*  Excellent!  This is just what I need to give me a leg up on the competition! As promised here are those lenses you requested!

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
        }

        public override void OnResponse(NetState sender, RelayInfo info) //Function for GumpButtonType.Reply Buttons 
        {
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
            CommandSystem.Register("JosefSkimmons", AccessLevel.GameMaster, JosefSkimmonsGump_OnCommand);
        }

        private static void JosefSkimmonsGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new JosefSkimmonsGump(e.Mobile));
        }

        public JosefSkimmonsGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 140, 300, 150, 1154294, 0x7FFF, false, true); // Hello there...what’s this? *reads the note* Look at these lenses! Cousteau drew these up did she...hrmm…well in any case I might be able to craft these lenses if you would be willing to assist me...

            AddHtmlLocalized(145, 300, 250, 24, 1154295, 0x7FFF, false, false); // What do you need assistance with?
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(2);
            AddHtmlLocalized(107, 140, 300, 150, 1154296, 0x7FFF, false, true); // I’m looking to expand my operation here, before I can do that though I need a more reliable source of raw ore.  Rumor has it the Orcs have a new machine they are using to drill huge quantities of ore!

            AddHtmlLocalized(145, 300, 250, 24, 1154297, 0x7FFF, false, false); // An orcish machine?
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 3);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(3);
            AddHtmlLocalized(107, 140, 300, 150, 1154298, 0x7FFF, false, true); // Yes! That’s exactly what I’m talking about! Drill baby drill! You get me some schematics to make that machine and I’ll strike your lenses for you...I bet those Orcs are holed up in their cave outside Yew.

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
        }

        public override void OnResponse(NetState sender, RelayInfo info) //Function for GumpButtonType.Reply Buttons 
        {
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
