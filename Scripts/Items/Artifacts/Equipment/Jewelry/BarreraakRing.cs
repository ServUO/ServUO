namespace Server.Items
{
    [TypeAlias("Server.Items.BarreraakRing")]
    public class BarreraaksRing : GoldRing
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1095049;  // Barreraak’s Old Beat Up Ring

        [Constructable]
        public BarreraaksRing()
        {
            //TODO: Get Hue
            LootType = LootType.Blessed;
        }

        public override bool CanEquip(Mobile from)
        {
            if (!base.CanEquip(from))
            {
                return false;
            }
            else if (from.Mounted)
            {
                from.SendLocalizedMessage(1010097); // You cannot use this while mounted.
                return false;
            }
            else if (from.Flying)
            {
                from.SendLocalizedMessage(1113414); // You can't use this while flying!
                return false;
            }
            else if (from.IsBodyMod)
            {
                from.SendLocalizedMessage(1111896); // You may only change forms while in your original body.
                return false;
            }

            return true;
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
                ((Mobile)parent).BodyMod = 334;
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
                ((Mobile)parent).BodyMod = 0;
        }

        public BarreraaksRing(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Parent is Mobile)
            {
                Mobile m = (Mobile)Parent;

                Timer.DelayCall(() =>
                    {
                        if (!m.Mounted && !m.Flying && !m.IsBodyMod)
                        {
                            m.BodyMod = 334;
                        }
                    });
            }

            if (version == 0)
            {
                reader.ReadInt();
            }
        }
    }
}
