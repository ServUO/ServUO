using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class ShipwreckBook : Item
    {
        public override int LabelNumber => 1123597;  // Book

        [Constructable]
        public ShipwreckBook()
            : base(0xFF4)
        {
            Movable = false;
            Hue = 1150;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 1))
            {
                if (FellowshipMedallion.IsDressed(from))
                {
                    Gump g = new Gump(100, 100);
                    g.AddImage(0, 0, 0x761C);
                    g.AddHtmlLocalized(115, 30, 350, 600, 1159311, "#1159311", 0x1, false,
                        true); // My inner voice guided me but has fallen silent...and to that end I have failed my purpose. We set sail for Iver's Rounding West of Skara Brae...I fear the end has come though, we are far off course and provisions are low and the weather has turned. The Love I had is my only Truth now. All that is left is Courage. When will my inner voice return?

                    from.SendGump(g);
                    from.PlaySound(1664);
                }
                else
                {
                    from.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1159310,
                        from.NetState); // * You attempt to read the journal but the characters are jumbled in your field of vision. Now your head hurts. *
                }
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public ShipwreckBook(Serial serial) : base(serial)
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
            reader.ReadInt();
        }
    }
}
