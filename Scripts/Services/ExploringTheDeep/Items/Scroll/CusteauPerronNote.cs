using System;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class CusteauPerronNote : Item
    {
        public override int LabelNumber { get { return 1023637; } } // scroll

        [Constructable]
        public CusteauPerronNote() : base(0x46B2)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1;
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1154262); // A Note from Cousteau Perron...
            list.Add(1072351); // Quest Item
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile))
                return;

            PlayerMobile mobile = (PlayerMobile)from;

            if (!from.HasGump(typeof(CusteauPerronNoteGump)))
            {
                from.SendGump(new CusteauPerronNoteGump(from));
            }
        }

        public CusteauPerronNote(Serial serial) : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }
    }

    public class CusteauPerronNoteGump : Gump
    {
        public CusteauPerronNoteGump(Mobile owner) : base(50, 50)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(104, 61, 250, 24, 1154262, 1062086, false, false); // A Note from Cousteau Perron...
            AddHtmlLocalized(42, 90, 330, 174, 1154263, 1, false, true); // Now that you have done what I asked, I shall do what you ask of me. Before you may continue your adventure you'll need to collect the components of the suit.  I have listed the components and whom you might seek to acquire them from below. Return to see Hepler Paulson in the City of Trinsic when you've got everything!<BR><BR>Nictitating Lenses - Josef Skimmons, the Master Blacksmith at the Cutlass Smithing in Bucaneer's Den will surely know how to craft such a thing.<BR><BR>Canvass Robe - Madeline Harte, Seamstress at the Adventurer's Needle in Jhelom is well known for her prowess with a needle.  If anyone can craft this for you, it is her.<BR><BR>Aqua Pendant - Zalia the Gemologist, in her craft few are better.  She will surely be able to supply the pendant. She can be found at the Gypsy camp in Northeastern Ilshenar.<BR><BR>Boots of Ballast - Champ Huthwait, a seedy man to say the least but a fine cobbler.  Seek him at the Adventurer's Supply in Vesper.
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
