namespace Server.Items
{
    [Flipable(0x1765, 0x1767)]
    public class AbyssalCloth : Item, ICommodity, IScissorable
    {
        [Constructable]
        public AbyssalCloth()
            : this(1)
        {
        }

        [Constructable]
        public AbyssalCloth(int amount)
            : base(0x1767)
        {
            Stackable = true;
            Amount = amount;
            Hue = 2075;
        }

        public AbyssalCloth(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113350;// abyssal cloth

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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
