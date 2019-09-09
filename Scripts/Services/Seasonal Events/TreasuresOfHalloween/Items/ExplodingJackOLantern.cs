using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class ExplodingJackOLantern : Item
    {
        public override int LabelNumber { get { return 1159220; } } // Exploding Jack o' Lantern

        [Constructable]
        public ExplodingJackOLantern()
            : base(0xA407)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Mounted)
            {
                from.SendLocalizedMessage(1061130); // You can't do that while riding a mount.
            }
            else
            {
                from.SendLocalizedMessage(1113280); // Which target do you wish to throw this at?
                from.Target = new ThrowTarget(from);
            }
        }

        private class ThrowTarget : Target
        {
            private Mobile m_From;

            public ThrowTarget(Mobile from)
                : base(12, true, TargetFlags.None)
            {
                m_From = from;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is PlayerMobile)
                {
                    Mobile m = targeted as Mobile;

                    if (from == m)
                    {
                        from.SendLocalizedMessage(501588); // Verbal taunts might be more effective!
                        return;
                    }

                    Effects.SendPacket(from, from.Map, new HuedEffect(EffectType.Moving, from.Serial, m.Serial, 0xA407, from, m, 10, 0, false, false, 0, 0));
                    from.SendLocalizedMessage(1159219); // You hit your target!
                    m.SendLocalizedMessage(1159218); // You have just been hit by a Jack o' Lantern!!!	
                    m.PlaySound(519);
                }
            }
        }

        public ExplodingJackOLantern(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
