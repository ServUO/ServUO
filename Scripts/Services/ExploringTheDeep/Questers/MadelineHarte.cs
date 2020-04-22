using Server.Commands;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Network;
using MadelineHarteGump = Server.Gumps.MadelineHarteGump;

namespace Server.Mobiles
{
    public class MadelineHarte : Mobile
    {
        public virtual bool IsInvulnerable => true;

        [Constructable]
        public MadelineHarte()
        {
            Name = "Madeline Harte";
            Title = "The Seamstress";
            Hue = Utility.RandomSkinHue();
            Blessed = true;

            Utility.AssignRandomHair(this);

            Female = true;
            Race = Race.Human;

            AddItem(new LongHair(2213));
            AddItem(new Backpack());
            AddItem(new Sandals(2017));
            AddItem(new FullApron(1313));
            AddItem(new Skirt(1202));
            AddItem(new FancyShirt(2017));
        }

        public MadelineHarte(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)from;

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
            {
                if (!from.HasGump(typeof(MadelineHarteGump)))
                {
                    from.SendGump(new MadelineHarteGump(from));
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
                    if (dropped is WillemHartesHat)
                    {
                        dropped.Delete();
                        from.AddToBackpack(new CanvassRobe());

                        if (!m.HasGump(typeof(MadelineHarteCompleteGump)))
                        {
                            m.SendGump(new MadelineHarteCompleteGump(m));
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
            writer.Write(0);
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
    public class MadelineHarteCompleteGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("MadelineHarteComplete", AccessLevel.GameMaster,
                MadelineHarteCompleteGump_OnCommand);
        }

        private static void MadelineHarteCompleteGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new MadelineHarteCompleteGump(e.Mobile));
        }

        public MadelineHarteCompleteGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 140, 300, 150, 1154302, 0x7FFF, false,
                true); // Oh! You’ve found his hat! Did you...*pauses and appears to begin to cry but regains her composure” Oh, I see.  At least he showed Valor...I thank you for give me closure.  I had a chance to read the note from Cousteau, this should be what you need.

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0); //OK
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0:
                {
                    break;
                }
            }
        }
    }

    public class MadelineHarteGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("MadelineHarte", AccessLevel.GameMaster, MadelineHarteGump_OnCommand);
        }

        private static void MadelineHarteGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new MadelineHarteGump(e.Mobile));
        }

        public MadelineHarteGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 140, 300, 150, 1154300, 0x7FFF, false,
                true); // *You notice a woman whimpering as she struggles through manipulating the spinning wheel, you smile at her and hand her the note* Oh, hello, *wipes tear*...I’m sorry did you need something...It’s just that...*begins crying again*...my son Willem has been killed Destard! If only I had something to remember him by...

            AddHtmlLocalized(145, 300, 250, 24, 1154301, 0x7FFF, false, false); // Willem went to Destard?
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0); //OK

            AddPage(2);
            AddHtmlLocalized(107, 140, 300, 150, 1154335, 0x7FFF, false,
                true); // That's right! I told him not to go but he didn't listen! If only I had a bit of something of his to remember him by, you look brave...would you venture to Destard and find it for me? Please?

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0); //OK
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
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
