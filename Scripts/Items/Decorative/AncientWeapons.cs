namespace Server.Items
{
    [Flipable(0xA33B, 0xA33C)]
    public class AncientWeapon1 : Item
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1125811;  // ancient weapon

        [Constructable]
        public AncientWeapon1()
            : base(0xA33B)
        {
        }

        public AncientWeapon1(Serial serial)
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

    [Flipable(0xA33D, 0xA33E)]
    public class AncientWeapon2 : Item
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1125811;  // ancient weapon

        [Constructable]
        public AncientWeapon2()
            : base(0xA33D)
        {
        }

        public AncientWeapon2(Serial serial)
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

    [Flipable(0xA33F, 0xA340)]
    public class AncientWeapon3 : Item
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1125811;  // ancient weapon

        [Constructable]
        public AncientWeapon3()
            : base(0xA33F)
        {
        }

        public AncientWeapon3(Serial serial)
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
