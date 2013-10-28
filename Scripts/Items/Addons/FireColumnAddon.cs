using System;

namespace Server.Items
{
    public class FireColumnAddon : BaseAddon
    {
        [Constructable]
        public FireColumnAddon()
            : this(false)
        {
        }

        [Constructable]
        public FireColumnAddon(bool bloody)
        {
            this.AddComponent(new AddonComponent(0x3A5), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x3A5), 0, 0, 5);
            this.AddComponent(new AddonComponent(0x3A5), 0, 0, 10);
            this.AddComponent(new AddonComponent(0x3A5), 0, 0, 15);

            this.AddComponent(new AddonComponent(0x19BB), 0, 0, 21);
            this.AddComponent(new AddonComponent(0x19AB), 0, 0, 23);

            if (bloody)
            {
                this.AddComponent(new AddonComponent(0x122B), -2, 0, 0);
                this.AddComponent(new AddonComponent(0x122E), 0, -2, 0);
                this.AddComponent(new AddonComponent(0x122D), -1, 1, 0);
                this.AddComponent(new AddonComponent(0x122F), 1, -1, 0);
                this.AddComponent(new AddonComponent(0x122D), 0, 1, 0);
                this.AddComponent(new AddonComponent(0x122A), 1, 0, 0);
                this.AddComponent(new AddonComponent(0x122B), 2, -1, 0);
                this.AddComponent(new AddonComponent(0x122B), 0, 2, 0);
                this.AddComponent(new AddonComponent(0x122E), 1, 1, 0);
            }
        }

        public FireColumnAddon(Serial serial)
            : base(serial)
        {
        }

        public override bool ShareHue
        {
            get
            {
                return false;
            }
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
    }
}