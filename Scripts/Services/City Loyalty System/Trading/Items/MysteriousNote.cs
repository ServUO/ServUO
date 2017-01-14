using System;
using Server;
using Server.Mobiles;
using Server.Engines.CityLoyalty;
using Server.Gumps;

namespace Server.Items
{
    public class MysteriousNote : Item
    {
        public override int LabelNumber { get { return 1151753; } } // A Mysterious Note

        [Constructable]
        public MysteriousNote()
            : base(0x2831)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            Gump g = new Gump(25, 25);
            g.AddBackground(0, 0, 404, 325, 9380);
            g.AddHtmlLocalized(40, 50, 324, 225, 1151735, false, false);

            /*You unfurl the parchment, it appears to be a handwritten note*<br><br>Taking up delivery of goods fer 
             * the crown are ya? Well...maybe yer interested in a bit of something fer yerself?  Look fer me in Felucca...
             * the taverns of Ocllo, Nujelm, & Serpent's Hold if ye want to fence yer goods.<br><br>-A Friend*/

            from.SendGump(g);
        }

        public MysteriousNote(Serial serial)
            : base(serial)
        {
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