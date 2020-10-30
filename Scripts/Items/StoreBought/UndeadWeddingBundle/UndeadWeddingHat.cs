namespace Server.Items
{
    [Flipable(0x9F47, 0x9F48)]
    public class UndeadWeddingHat : BaseHat
    {
        public override int LabelNumber => 1124799; // Undead Wedding Hat

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Transformed { get; set; }

        [Constructable]
        public UndeadWeddingHat()
            : this(0)
        {
        }

        [Constructable]
        public UndeadWeddingHat(int hue)
            : base(0x9F47, hue)
        {
			LootType = LootType.Blessed;
			Weight = 3.0;
        }

        private bool EnMask(Mobile from)
        {
            if (from.IsBodyMod || from.HueMod > -1)
            {
                from.SendLocalizedMessage(1158010); // You cannot use that item in this form.
            }
            else
            {
                from.HueMod = 1150;
                Transformed = true;

                return true;
            }

            return false;
        }

        private void DeMask(Mobile from)
        {
            from.HueMod = -1;
            Transformed = false;
        }

        public override bool OnEquip(Mobile from)
        {
            if (!Transformed)
            {
                if (EnMask(from))
                    return true;

                return false;
            }

            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile m && Transformed)
            {
                DeMask(m);
            }

            base.OnRemoved(parent);
        }

        public UndeadWeddingHat(Serial serial)
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
            reader.ReadInt();

            if (RootParent is Mobile && ((Mobile)RootParent).Items.Contains(this))
            {
                EnMask((Mobile)RootParent);
            }
        }
    }
}
