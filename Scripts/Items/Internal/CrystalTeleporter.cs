#region Header
// **********
// ServUO - Teleporter.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Text;

using Server.Mobiles;
using Server.Network;
using Server.Spells;
#endregion

namespace Server.Items
{
    public class CrystalTeleporter : ClickTeleporter
    {
        public override int LabelNumber { get { return 1027961; } } // magical crystal

        [Constructable]
        public CrystalTeleporter()
            : this(0x1F19, new Point3D(0, 0, 0), null)
        { }

        [Constructable]
        public CrystalTeleporter(int itemID, Point3D pointDest, Map mapDest)
            : base(itemID, pointDest, mapDest)
        {
        }

        public CrystalTeleporter(Serial serial)
            : base(serial)
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