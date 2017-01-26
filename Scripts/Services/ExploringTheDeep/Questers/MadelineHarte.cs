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

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.HeplerPaulson)
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
                if (m.ExploringTheDeepQuest == ExploringTheDeepQuestChain.HeplerPaulson)
                {
                    if (dropped is WillemHartesHat)
                    {
                        m.ExploringTheDeepQuest = ExploringTheDeepQuestChain.CusteauPerron;
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
        AddHtmlLocalized(107, 110, 300, 230, 1154302, 0x7FFF, false, true); // Oh! You’ve found his hat! Did you...*pauses and appears to begin to cry but regains her composure” Oh, I see.  At least he showed Valor...I thank you for give me closure.  I had a chance to read the note from Cousteau, this should be what you need.

        AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
    }

    public override void OnResponse(NetState state, RelayInfo info) //Function for GumpButtonType.Reply Buttons 
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
        AddHtmlLocalized(107, 110, 300, 230, 1154300, 0x7FFF, false, true); // *You notice a woman whimpering as she struggles through manipulating the spinning wheel, you smile at her and hand her the note* Oh, hello, *wipes tear*...I’m sorry did you need something...It’s just that...*begins crying again*...my son Willem has been killed Destard! If only I had something to remember him by...

        AddHtmlLocalized(155, 260, 250, 24, 1154301, 0x7FFF, false, false); // Willem went to Destard?
        AddButton(125, 260, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

        AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

        AddPage(2);
        AddHtmlLocalized(107, 110, 300, 230, 1154335, 0x7FFF, false, true); // That's right! I told him not to go but he didn't listen! If only I had a bit of something of his to remember him by, you look brave...would you venture to Destard and find it for me? Please?
                
        AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
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