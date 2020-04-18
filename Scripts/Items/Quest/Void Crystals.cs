namespace Server.Items
{
    public class VoidCrystalOfCorruptedArcaneEssence : Item
    {
        public override int LabelNumber => 1150321;  // Void Crystal of Corrupted Arcane Essence

        [Constructable]
        public VoidCrystalOfCorruptedArcaneEssence() : base(0x1F19)
        {
            Hue = 1267;
        }

        public VoidCrystalOfCorruptedArcaneEssence(Serial serial) : base(serial)
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

    public class VoidCrystalOfCorruptedSpiritualEssence : Item
    {
        public override int LabelNumber => 1150322;  // Void Crystal of Corrupted Spiritual Essence

        [Constructable]
        public VoidCrystalOfCorruptedSpiritualEssence()
            : base(0x1F19)
        {
            Hue = 1269;
        }

        public VoidCrystalOfCorruptedSpiritualEssence(Serial serial)
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

    public class VoidCrystalOfCorruptedMysticalEssence : Item
    {
        public override int LabelNumber => 1150323;  // Void Crystal of Corrupted Mystical Essence

        [Constructable]
        public VoidCrystalOfCorruptedMysticalEssence() : base(0x1F19)
        {
            Hue = 2717;
        }

        public VoidCrystalOfCorruptedMysticalEssence(Serial serial) : base(serial)
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