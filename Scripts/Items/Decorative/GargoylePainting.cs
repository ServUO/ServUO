namespace Server.Items
{
    [Flipable(0x403D, 0x403E)]
    public class GargoylePainting : Item
    {
        [Constructable]
        public GargoylePainting()
            : base(0x403D)
        {
            Weight = 10;
        }

        public GargoylePainting(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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
