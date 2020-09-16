namespace Server.Items
{
    [Flipable(0x1765, 0x1767)]
    public class UncutCloth : Item, IScissorable, IDyable, ICommodity
    {
        [Constructable]
        public UncutCloth()
            : this(1)
        {
        }

        [Constructable]
        public UncutCloth(int amount)
            : base(0x1767)
        {
            Stackable = true;
            Amount = amount;
        }

        public UncutCloth(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight => 0.1;
        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;
        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
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

            base.ScissorHelper(from, new Bandage(), 1);

            return true;
        }
    }
}
