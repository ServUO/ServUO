using System;
using Server.Mobiles;
using Server;

namespace Server.Items
{
    public class XenrrFishingPole : FishingPole
    {
        public override bool IsArtifact { get { return true; } }
        private int m_BodyInit;
        private int m_BodyHue;

        [CommandProperty(AccessLevel.Administrator)]
        public int BodyInit
        {
            get
            {
                return m_BodyInit;
            }
            set
            {
                m_BodyInit = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.Administrator)]
        public int BodyHue
        {
            get
            {
                return m_BodyHue;
            }
            set
            {
                m_BodyHue = value;
                InvalidateProperties();
            }
        }

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
                BodyInit = from.BodyMod;
                BodyHue = from.HueMod;
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

                m.BodyMod = BodyInit;
                m.HueMod = BodyHue;
                m.FixedParticles(0x3728, 1, 13, 5042, EffectLayer.Waist);
            }
        }
        public override int LabelNumber { get { return 1095066; } }

        [Constructable]
        public XenrrFishingPole()
        {
            this.LootType = LootType.Blessed;

            this.Attributes.SpellChanneling = 1;
            this.Attributes.CastSpeed = -1;
        }

        public XenrrFishingPole(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)m_BodyInit);
            writer.Write((int)m_BodyHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_BodyInit = reader.ReadInt();
            m_BodyHue = reader.ReadInt();
        }
    }
}