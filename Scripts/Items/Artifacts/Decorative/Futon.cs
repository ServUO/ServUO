namespace Server.Items
{
    [Flipable]
    public class Futon : Item
    {
        public override bool IsArtifact => true;
        [Constructable]
        public Futon()
            : base(Utility.RandomDouble() > 0.5 ? 0x295C : 0x295E)
        {
        }

        public Futon(Serial serial)
            : base(serial)
        {
        }

        public void Flip()
        {
            switch (ItemID)
            {
                case 0x295C:
                    ItemID = 0x295D;
                    break;
                case 0x295E:
                    ItemID = 0x295F;
                    break;
                case 0x295D:
                    ItemID = 0x295C;
                    break;
                case 0x295F:
                    ItemID = 0x295E;
                    break;
            }
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