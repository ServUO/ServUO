using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class LuckyCoin : Item
    {
        public override int LabelNumber { get { return 1113366; } } // lucky coin

        [Constructable]
        public LuckyCoin()
            : this(1)
        {
        }

        [Constructable]
        public LuckyCoin(int amount)
            : base(0xF87)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1.0;
            this.Hue = 1174;
        }

        public LuckyCoin(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) && Amount >= 1)
            {
                from.SendLocalizedMessage(1113367); // Make a wish then toss me into sacred waters!!
                from.Target = new InternalTarget(this);
            }
        }

        private class InternalTarget : Target
        {
            private LuckyCoin m_Coin;

            public InternalTarget(LuckyCoin coin)
                : base(3, false, TargetFlags.None)
            {
                m_Coin = coin;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is AddonComponent && ((AddonComponent)targeted).Addon is FountainOfFortune)
                {
                    AddonComponent c = (AddonComponent)targeted;

                    if (c.Addon is FountainOfFortune)
                        ((FountainOfFortune)c.Addon).OnTarget(from, m_Coin);
                }
                else
                    from.SendLocalizedMessage(1113369); // That is not sacred waters. Try looking in the Underworld.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}