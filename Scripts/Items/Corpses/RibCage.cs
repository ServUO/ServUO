namespace Server.Items
{
    [Flipable(0x1B17, 0x1B18)]
    public class RibCage : Item, IScissorable
    {
        [Constructable]
        public RibCage()
            : base(0x1B17 + Utility.Random(2))
        {
            Stackable = false;
            Weight = 5.0;
        }

        public RibCage(Serial serial)
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

        public bool Scissor(Mobile from, Scissors scissors)
        {
            if (Deleted || !from.CanSee(this))
                return false;

            base.ScissorHelper(from, new Bone(), Utility.RandomMinMax(3, 5));

            return true;
        }
    }
}