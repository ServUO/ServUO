using System;
using Server.Network;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public class BasicAddon : BaseAddon
    {
        [Constructable]
        public BasicAddon()
        {
            this.AddComponent(new AddonComponent(2810), 0, 0, 0);
        }
		
		public BasicAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((byte)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadByte();
        }
	}
}