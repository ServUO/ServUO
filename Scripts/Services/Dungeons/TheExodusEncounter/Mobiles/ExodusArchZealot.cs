#region Header
//Exodus Encounter by Redmoon
#endregion Header
using Server.Commands;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class ExodusArchZealot : Mobile
    {
        public virtual bool IsInvulnerable => true;

        [Constructable]
        public ExodusArchZealot()
        {
            Name = "Hunter";
            Title = "the Arch Zealot";
            Body = 0x190;
            CantWalk = true;
            Hue = 0;
            Blessed = true;

            AddItem(new HoodedShroudOfShadows(0xA91));
            AddItem(new ThighBoots());

            Item beard = new Item(0x2040)
            {
                Hue = 902,
                Layer = Layer.FacialHair,
                Movable = false
            };

            AddItem(beard);
        }

        public ExodusArchZealot(Serial serial) : base(serial)
        {
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (from.InRange(Location, 2) && from.Race == Race.Gargoyle && dropped.GetType() == typeof(ExodusSacrificalDagger))
            {
                dropped.Delete();
                from.AddToBackpack(new ExodusSacrificalGargishDagger());

                return true;
            }

            return base.OnDragDrop(from, dropped);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            list.Add(new ExodusArchZealotGumpEntry(from, this));
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

        public class ExodusArchZealotGumpEntry : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;

            public ExodusArchZealotGumpEntry(Mobile from, Mobile giver) : base(6146, 3)
            {
                m_Mobile = from;
            }

            public override void OnClick()
            {
                if (!(m_Mobile is PlayerMobile))
                    return;

                PlayerMobile mobile = (PlayerMobile)m_Mobile;
                {
                    if (!mobile.HasGump(typeof(ExodusArchZealotGump)))
                    {
                        mobile.SendGump(new ExodusArchZealotGump(mobile));
                    }
                }
            }
        }
    }

    public class ExodusArchZealotGump : Gump
    {
        public const int White = 0x7FFF;

        public static void Initialize()
        {
            CommandSystem.Register("ExodusArchZealotGump", AccessLevel.GameMaster, ExodusArchZealotGump_OnCommand);
        }

        private static void ExodusArchZealotGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new ExodusArchZealotGump(e.Mobile));
        }

        public ExodusArchZealotGump(Mobile owner) : base(50, 50)
        {
            //----------------------------------------------------------------------------------------------------

            AddPage(0);
            AddImageTiled(54, 33, 369, 400, 2624);
            AddAlphaRegion(54, 33, 369, 400);

            AddImageTiled(416, 39, 44, 389, 203);
            //--------------------------------------Window size bar--------------------------------------------

            AddImage(97, 49, 9005);
            AddImageTiled(58, 39, 29, 390, 10460);
            AddImageTiled(412, 37, 31, 389, 10460);
            AddHtmlLocalized(140, 60, 250, 24, 1153670, White, false, false); // Exodus Arch Zealot
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
            AddHtmlLocalized(107, 110, 300, 230, 1153671, White, false, true); // Greetings Traveler *sly grin* Can you feel it? Can you feel the awesome power that emanates from this place...surely Lord Exodus will join us again soon, why – that’s why you’re here isn’t it? To perform the Ritual?

            AddHtmlLocalized(155, 260, 250, 24, 1153672, White, false, false);
            AddButton(125, 260, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 2);//Ritual?

            AddHtmlLocalized(155, 280, 250, 24, 1153674, White, false, false);
            AddButton(125, 280, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 3);//Summoning Altar?

            AddHtmlLocalized(155, 300, 250, 24, 1153676, White, false, false);
            AddButton(125, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 4);//Robe of Rite?

            AddHtmlLocalized(155, 320, 250, 24, 1153678, White, false, false);
            AddButton(125, 320, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 5);//Summoning Rite?

            AddHtmlLocalized(155, 340, 250, 24, 1153680, White, false, false);
            AddButton(125, 340, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 6);//Sacrifical Dagger?

            AddHtmlLocalized(155, 360, 250, 24, 1153682, White, false, false);
            AddButton(125, 360, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 7);//The Keys?

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK

            //// START PAGE 2 Ritual
            AddPage(2);
            AddHtmlLocalized(107, 110, 300, 230, 1153673, White, false, true); // The Ritual can only be performed by the most devoted of followers...and even then, only when the Summoning Altar has been built can the Robe of Rite be worn, the Summoning Rite be read and the Sacrificial dagger used to declare thy intention...only then can the great Lord Exodus be summoned to this plane...for these are the Keys to his return *laughs manically*

            AddHtmlLocalized(155, 280, 250, 24, 1153674, White, false, false);
            AddButton(125, 280, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 3);//Summoning Altar?

            AddHtmlLocalized(155, 300, 250, 24, 1153676, White, false, false);
            AddButton(125, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 4);//Robe of Rite?

            AddHtmlLocalized(155, 320, 250, 24, 1153678, White, false, false);
            AddButton(125, 320, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 5);//Summoning Rite?

            AddHtmlLocalized(155, 340, 250, 24, 1153680, White, false, false);
            AddButton(125, 340, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 6);//Sacrifical Dagger?

            AddHtmlLocalized(155, 360, 250, 24, 1153682, White, false, false);
            AddButton(125, 360, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 7);//The Keys?

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
            ///// END PAGE 2 Ritual


            //// START PAGE 3 Summoning Alter
            AddPage(3);
            AddHtmlLocalized(107, 110, 300, 230, 1153675, White, false, true); // The Summoning Altar must be built upon a shrine, within Trammel or Felucca it matters not...

            AddHtmlLocalized(155, 300, 250, 24, 1153676, White, false, false);
            AddButton(125, 300, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 4);//Robe of Rite?

            AddHtmlLocalized(155, 320, 250, 24, 1153678, White, false, false);
            AddButton(125, 320, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 5);//Summoning Rite?

            AddHtmlLocalized(155, 340, 250, 24, 1153680, White, false, false);
            AddButton(125, 340, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 6);//Sacrifical Dagger?

            AddHtmlLocalized(155, 360, 250, 24, 1153682, White, false, false);
            AddButton(125, 360, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 7);//The Keys?

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
            ///// END PAGE 3 Summoning Alter


            //// START PAGE 4 Robe of Rite

            AddPage(4);
            AddHtmlLocalized(107, 110, 300, 230, 1153677, White, false, true); // These vestments must be worn as a sign of devotion to the ritual, for without them thou art not true to Lord Exodus.

            AddHtmlLocalized(155, 320, 250, 24, 1153678, White, false, false);
            AddButton(125, 320, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 5);//Summoning Rite?

            AddHtmlLocalized(155, 340, 250, 24, 1153680, White, false, false);
            AddButton(125, 340, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 6);//Sacrifical Dagger?

            AddHtmlLocalized(155, 360, 250, 24, 1153682, White, false, false);
            AddButton(125, 360, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 7);//The Keys?

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
            ///// END PAGE 4 Robe of Rite


            //// START PAGE 5 Summoning Rite
            AddPage(5);
            AddHtmlLocalized(107, 110, 300, 230, 1153679, White, false, true); // From the tongue of the summoner these words must flow, for without them thou art not true! 

            AddHtmlLocalized(155, 340, 250, 24, 1153680, White, false, false);
            AddButton(125, 340, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 6);//Sacrifical Dagger?

            AddHtmlLocalized(155, 360, 250, 24, 1153682, White, false, false);
            AddButton(125, 360, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 7);//The Keys?

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
            ///// END PAGE 5 Summoning Rite


            //// START PAGE 6 Sacrifical Dagger
            AddPage(6);
            AddHtmlLocalized(107, 110, 300, 230, 1153681, White, false, true); // This dagger, infused with black rock, is the instrument by which thou shall prove thy intention before the altar.  If you wish a dagger to fit the talons of a gargoyle, simply hand to me the human tool and I shall alter it for you.

            AddHtmlLocalized(155, 360, 250, 24, 1153682, White, false, false);
            AddButton(125, 360, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 7);//The Keys?

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
            ///// END PAGE 6 Sacrifical Dagger


            //// START PAGE 7 Keys
            AddPage(7);
            AddHtmlLocalized(107, 110, 300, 230, 1153683, White, false, true); // Without the Keys the ritual cannot be performed.  To obtain these Keys you may follow the path of the Warrior, the Rogue, or the Craftsman.

            AddHtmlLocalized(155, 320, 250, 24, 1153684, White, false, false);
            AddButton(125, 320, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 8);//The Warrior?

            AddHtmlLocalized(155, 340, 250, 24, 1153686, White, false, false);
            AddButton(125, 340, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 9);//The Rogue?

            AddHtmlLocalized(155, 360, 250, 24, 1153688, White, false, false);
            AddButton(125, 360, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 10);//The Craftsman?

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
            ////END PAGE 7 Keys


            //// START PAGE 8 WARRIOR
            AddPage(8);
            AddHtmlLocalized(107, 110, 300, 230, 1153685, White, false, true); // Deep within Exodus Dungeon, minions of Lord Exodus battle fiercely against Lord Dupre’s men...slay either one to collect the Keys.

            AddHtmlLocalized(155, 340, 250, 24, 1153686, White, false, false);
            AddButton(125, 340, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 9);//The Rogue?

            AddHtmlLocalized(155, 360, 250, 24, 1153688, White, false, false);
            AddButton(125, 360, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 10);//The Craftsman?

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
            ////END PAGE 8 WARRIOR


            ////START PAGE 9 ROGUE
            AddPage(9);
            AddHtmlLocalized(107, 110, 300, 230, 1153687, White, false, true); // Hidden deep inside the dungeon are supply caches...within them the intrepid rogue can find the Keys to the ritual.  Look keenly and be deft with thy touch...as these caches are likely to be trapped.  For the light of touch the Keys may be stolen from Zealots within the depths of the Dungeon...

            AddHtmlLocalized(155, 360, 250, 24, 1153688, White, false, false);
            AddButton(125, 360, 0x26B0, 0x26B1, 0, GumpButtonType.Page, 10);//The Craftsman?

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
            ////END PAGE 9 ROGUE

            ////START PAGE 10 CRAFTSMAN
            AddPage(10);
            AddHtmlLocalized(107, 110, 300, 230, 1153689, White, false, true); // From the hands of the skilled artisan all of these Keys may be built by means of Tailoring, Alchemy, Carpentry, and Smithy.

            AddButton(325, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);//OK
            ////END PAGE 10 CRAFTSMAN

        }

        public override void OnResponse(NetState sender, RelayInfo info) //Function for GumpButtonType.Reply Buttons 
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        //Cancel 
                        from.SendMessage("Good Luck!");
                        break;
                    }
            }
        }
    }
}
