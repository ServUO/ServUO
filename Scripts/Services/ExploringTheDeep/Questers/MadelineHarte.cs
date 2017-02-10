using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class MadelineHarte : Mobile
    {
        public virtual bool IsInvulnerable { get { return true; } }

        [Constructable]
        public MadelineHarte()
        {
            this.Name = "Madeline Harte";
            this.Title = "The Seamstress";
            this.Hue = Utility.RandomSkinHue();
            this.Blessed = true;

            Utility.AssignRandomHair(this);

            this.Female = true;
            this.Race = Race.Human;

            this.AddItem(new LongHair(2213));
            this.AddItem(new Backpack());
            this.AddItem(new Sandals(2017));
            this.AddItem(new FullApron(1313));
            this.AddItem(new Skirt(1202));
            this.AddItem(new FancyShirt(2017));
        }

        public MadelineHarte(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!(m is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)m;

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
            {
                if (!m.HasGump(typeof(MadelineHarteGump)))
                {
                    m.SendGump(new MadelineHarteGump(m));
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}

public class MadelineHarteCompleteGump : Gump
{
    public static void Initialize()
    {
        CommandSystem.Register("MadelineHarteComplete", AccessLevel.GameMaster, new CommandEventHandler(MadelineHarteCompleteGump_OnCommand));
    }

    private static void MadelineHarteCompleteGump_OnCommand(CommandEventArgs e)
    {
        e.Mobile.SendGump(new MadelineHarteCompleteGump(e.Mobile));
    }

    public MadelineHarteCompleteGump(Mobile owner) : base(50, 50)
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
        AddHtmlLocalized(107, 140, 300, 150, 1154302, 0x7FFF, false, true); // Oh! You’ve found his hat! Did you...*pauses and appears to begin to cry but regains her composure” Oh, I see.  At least he showed Valor...I thank you for give me closure.  I had a chance to read the note from Cousteau, this should be what you need.

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

public class MadelineHarteGump : Gump
{
    public static void Initialize()
    {
        CommandSystem.Register("MadelineHarte", AccessLevel.GameMaster, new CommandEventHandler(MadelineHarteGump_OnCommand));
    }

    private static void MadelineHarteGump_OnCommand(CommandEventArgs e)
    {
        e.Mobile.SendGump(new MadelineHarteGump(e.Mobile));
    }

    public MadelineHarteGump(Mobile owner) : base(50, 50)
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
        AddHtmlLocalized(107, 140, 300, 150, 1154300, 0x7FFF, false, true); // *You notice a woman whimpering as she struggles through manipulating the spinning wheel, you smile at her and hand her the note* Oh, hello, *wipes tear*...I’m sorry did you need something...It’s just that...*begins crying again*...my son Willem has been killed Destard! If only I had something to remember him by...

        AddHtmlLocalized(145, 300, 250, 24, 1154301, 0x7FFF, false, false); // Willem went to Destard?
        AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

        AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

        AddPage(2);
        AddHtmlLocalized(107, 140, 300, 150, 1154335, 0x7FFF, false, true); // That's right! I told him not to go but he didn't listen! If only I had a bit of something of his to remember him by, you look brave...would you venture to Destard and find it for me? Please?

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