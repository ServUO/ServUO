using System;

namespace Server.Items
{
    public class SecretShadowWallNS : BaseSliding
    {
        [Constructable]
        public SecretShadowWallNS()
            : base(0x363A, 0x3619)
        {
            this.Name = "secret door";
        }

        public SecretShadowWallNS(Serial serial)
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
    }

    public class SecretDungeonWallNS : BaseSliding
    {
        [Constructable]
        public SecretDungeonWallNS()
            : base(0x0242, 0x0244)
        {
            this.Name = "secret door";
        }

        public SecretDungeonWallNS(Serial serial)
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
    }

    public class SecretStoneWallNS : BaseSliding
    {
        [Constructable]
        public SecretStoneWallNS()
            : base(0x3C9, 0x3CA)
        {
            this.Name = "secret door";
            this.Hue = 744;
        }

        public SecretStoneWallNS(Serial serial)
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
    }
}