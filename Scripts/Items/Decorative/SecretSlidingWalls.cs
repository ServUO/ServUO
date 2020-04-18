namespace Server.Items
{
    public class SecretShadowWallNS : BaseSliding
    {
        [Constructable]
        public SecretShadowWallNS()
            : base(0x363A, 0x3619)
        {
            Name = "secret door";
        }

        public SecretShadowWallNS(Serial serial)
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

    public class SecretDungeonWallNS : BaseSliding
    {
        [Constructable]
        public SecretDungeonWallNS()
            : base(0x0242, 0x0244)
        {
            Name = "secret door";
        }

        public SecretDungeonWallNS(Serial serial)
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

    public class SecretStoneWallNS : BaseSliding
    {
        [Constructable]
        public SecretStoneWallNS()
            : base(0x3C9, 0x3CA)
        {
            Name = "secret door";
            Hue = 744;
        }

        public SecretStoneWallNS(Serial serial)
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