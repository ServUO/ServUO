using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class ExodusAlterAddon : BaseAddon
    {
        [Constructable]
        public ExodusAlterAddon()
        {
            this.AddComponent(0x0782, 0, 1, 5);
            this.AddComponent(0x0783, 1, 0, 5);
            this.AddComponent(0x074E, 0, 0, 5);
            this.AddComponent(0x074F, 1, 1, 5);
        }

        public void AddComponent(int id, int x, int y, int z)
        {
            AddonComponent ac = new AddonComponent(id);

            ac.Hue = 0xA92;
            this.AddComponent(ac, x, y, z);
        }
       
        public ExodusAlterAddon(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}