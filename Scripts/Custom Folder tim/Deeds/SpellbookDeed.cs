using System;
using Server;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Guilds;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Misc
{
    public class SpellbookDeed : Item // Create the item class which is derived from the base item class
    {
        [Constructable]
        public SpellbookDeed()
            : base(0x14F0)
        {
            Weight = 1.0;
            Name = "Spellbook Deed";
            Hue = 0x3E4;
        }

        public SpellbookDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override bool DisplayLootType { get { return false; } }

        public override void OnDoubleClick(Mobile from) // Override double click of the deed to call our target
        {
            if (!IsChildOf(from.Backpack)) // Make sure its in their pack
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                from.SendGump(new SpellbookGump(from, this));
            }
        }
    }
    public class SpellbookGump : Gump
    {
        private Item m_Deed;
        private Mobile m_Mobile;

        public SpellbookGump(Mobile from, Item deed)
            : base(20, 20)
        {
            m_Mobile = from;
            m_Deed = deed;
            {



                this.Closable = true;
                this.Disposable = false;
                this.Dragable = true;
                this.Resizable = false;
                this.AddPage(0);
                this.AddBackground(34, 17, 350, 368, 9200);
                this.AddLabel(128, 38, 0, @"Choose your Spellbook");
                this.AddTextEntry(98, 132, 226, 20, 0, (int)Buttons.TextEntry1, @"  Spellbook                       Necromancy");
                this.AddTextEntry(105, 212, 213, 20, 0, (int)Buttons.TextEntry2, @"   Chivalry                            Bushido");
                this.AddTextEntry(107, 301, 213, 20, 0, (int)Buttons.TextEntry3, @"   Ninjitsu                     Spellweaving");
                this.AddButton(298, 347, 241, 248, 0,  GumpButtonType.Reply, 0);//Close
                this.AddButton(118, 77, 2234, 248, 1, GumpButtonType.Reply, 0);//spellbook
                this.AddButton(250, 78, 11011, 248, 2, GumpButtonType.Reply, 0);//Necormancy
                this.AddButton(117, 160, 11012, 248, 3, GumpButtonType.Reply, 0);//Chivalry
                this.AddButton(250, 158, 11017, 248, 4, GumpButtonType.Reply, 0);//Bushido
                this.AddButton(117, 244, 11016, 248, 5, GumpButtonType.Reply, 0);//Ninjitsu
                this.AddButton(249, 246, 11053, 248, 6, GumpButtonType.Reply, 0);//Spellweaving
                this.AddTextEntry(225, 132, 86, 19, 0, (int)Buttons.TextEntry4, @"  Necromancy");
                this.AddTextEntry(230, 214, 78, 20, 0, (int)Buttons.TextEntry5, @"    Bushido");
                this.AddTextEntry(229, 301, 90, 20, 0, (int)Buttons.TextEntry6, @"  Spellweaving");
            }
        }

        public enum Buttons
        {
            TextEntry1,
            TextEntry2,
            TextEntry3,
            Button0,
            Button1,
            Button2,
            Button3,
            Button4,
            Button5,
            Button6,
            TextEntry4,
            TextEntry5,
            TextEntry6,
        }
        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            //EthyDeed ed = ed.Delete();
            switch (info.ButtonID)
            {
                case 0:
                    {
                        from.CloseGump(typeof(SpellbookGump));
                        break;
                    }
                case 1:
                    {
                        Item item = new FullMagerySpellbook();
                        from.AddToBackpack(item);
                        from.CloseGump(typeof(SpellbookGump));

                        break;
                    }

                case 2:
                    {
                        Item item = new FullNecroSpellbook();
                        from.AddToBackpack(item);
                        from.CloseGump(typeof(SpellbookGump));

                        break;
                    }
                case 3:
                    {
                        Item item = new FullChivalrySpellbook();
                        from.AddToBackpack(item);
                        from.CloseGump(typeof(SpellbookGump));

                        break;
                    }

                case 4:
                    {
                        Item item = new FullBushidoSpellbook();
                        from.AddToBackpack(item);
                        from.CloseGump(typeof(SpellbookGump));

                        break;
                    }
                case 5:
                    {
                        Item item = new FullNinjitsuSpellbook();
                        from.AddToBackpack(item);
                        from.CloseGump(typeof(SpellbookGump));

                        break;
                    }
                case 6:
                    {
                        Item item = new FullSpellweavingSpellbook();
                        from.AddToBackpack(item);
                        from.CloseGump(typeof(SpellbookGump));

                        break;
                    }
            }
        }
    }
}
