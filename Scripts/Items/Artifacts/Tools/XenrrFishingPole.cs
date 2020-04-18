namespace Server.Items
{
    public class XenrrFishingPole : FishingPole
    {
        public override bool IsArtifact => true;

        public override bool OnEquip(Mobile from)
        {
            if (!base.OnEquip(from))
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
            {
                Mobile from = parent as Mobile;

                from.FixedParticles(0x3728, 1, 13, 5042, EffectLayer.Waist);

                from.BodyMod = 723;
                from.HueMod = 0;
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile && !Deleted)
            {
                Mobile m = (Mobile)parent;

                m.BodyMod = 0;
                m.HueMod = -1;
                m.FixedParticles(0x3728, 1, 13, 5042, EffectLayer.Waist);
            }
        }
        public override int LabelNumber => 1095066;

        [Constructable]
        public XenrrFishingPole()
        {
            LootType = LootType.Blessed;

            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = -1;
        }

        public XenrrFishingPole(Serial serial) : base(serial)
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

            if (version == 0)
            {
                reader.ReadInt();
                reader.ReadInt();
            }

            if (Parent is Mobile)
            {
                Mobile m = (Mobile)Parent;

                Timer.DelayCall(() =>
                {
                    if (!m.Mounted && !m.Flying && !m.IsBodyMod)
                    {
                        m.BodyMod = 723;
                        m.HueMod = 0;
                    }
                });
            }
        }
    }
}