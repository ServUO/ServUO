using Server.Commands;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    public class ChampHuthwait : Mobile
    {
        public virtual bool IsInvulnerable => true;

        [Constructable]
        public ChampHuthwait()
            : base()
        {
            Name = "Champ Huthwait";
            Title = "The Seedy Cobbler";
            Female = false;
            Race = Race.Human;
            CantWalk = true;
            Blessed = true;

            Hue = Utility.RandomSkinHue();
            Utility.AssignRandomHair(this);

            AddItem(new Backpack());
            AddItem(new Boots(2017));
            AddItem(new LongPants(2017));
            AddItem(new FancyShirt(1432));
            AddItem(new JinBaori(1408));
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!(m is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)m;

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
            {
                if (!m.HasGump(typeof(ChampHuthwaitGump)))
                {
                    m.SendGump(new ChampHuthwaitGump(m));
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
                    if (dropped is MercutiosCutlass)
                    {
                        dropped.Delete();
                        from.AddToBackpack(new BootsOfBallast());

                        if (!m.HasGump(typeof(ChampHuthwaitCompleteGump)))
                        {
                            m.SendGump(new ChampHuthwaitCompleteGump(m));
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

        public ChampHuthwait(Serial serial) : base(serial)
        {
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
    public class ChampHuthwaitGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("ChampHuthwait", AccessLevel.GameMaster, ChampHuthwaitGump_OnCommand);
        }

        private static void ChampHuthwaitGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new ChampHuthwaitGump(e.Mobile));
        }

        public ChampHuthwaitGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 140, 300, 150, 1154303, 0x7FFF, false, true); // Just tell him I'll have the money by...*pauses*...*with a smile*...Oh, you aren't part of Mercutio's crew?  Oh well then, what's your business then? Loafers? Sandals? A Fine pair of boots then?

            AddHtmlLocalized(145, 300, 250, 24, 1154304, 0x7FFF, false, false); // I'm looking for the Boots of Ballast...
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(2);
            AddHtmlLocalized(107, 140, 300, 150, 1154305, 0x7FFF, false, true); // *reads the note*  Well I see you and Cousteau are after that suit then.  Well I'll tell you what I tell everyone else...if you want the Boots of Ballast you need to help me out with a sticky situation.

            AddHtmlLocalized(145, 300, 250, 24, 1154306, 0x7FFF, false, false); // What kind of sticky situation?
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 3);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(3);
            AddHtmlLocalized(107, 140, 300, 150, 1154307, 0x7FFF, false, true); // Yea, that's right.  Seems Mercutio's Gang is after me for a bit of a...disagreement about a small debt...if you were to...take care of Mercutio's gang I'd be willing to hook you up with those boots.

            AddHtmlLocalized(145, 300, 250, 24, 1154308, 0x7FFF, false, false); // Where is Mercutio's Gang?
            AddButton(115, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 4);

            AddButton(345, 440, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            AddPage(4);
            AddHtmlLocalized(107, 140, 300, 150, 1154309, 0x7FFF, false, true); // They usually hangout just southwest of the City in some old ruins.  Bring me evidence of Mercutio's demise and I will get you the boots you seek. 

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

    public class ChampHuthwaitCompleteGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("ChampHuthwaitComplete", AccessLevel.GameMaster, ChampHuthwaitCompleteGump_OnCommand);
        }

        private static void ChampHuthwaitCompleteGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new ChampHuthwaitCompleteGump(e.Mobile));
        }

        public ChampHuthwaitCompleteGump(Mobile owner) : base(50, 50)
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
            AddHtmlLocalized(107, 140, 300, 150, 1154310, 0x7FFF, false, true); // You did it! Hot diggity! Looks like I won't have to deal with that knave any longer! Smooth sailing from here on out...just me and the tables...oh, right here are your boots!

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
}
