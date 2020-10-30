using System;

namespace Server.Items
{
    public class ShackledHeartOfThePumpkinKing : Item
    {
        public override int LabelNumber => 1157653;  // Shackled Heart of the Pumpkin King

        private Timer Timer { get; set; }

        [Constructable]
        public ShackledHeartOfThePumpkinKing()
            : base(0x4A9C)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (Timer == null)
            {
                m.SendLocalizedMessage(1157631); // Thou shall know the pain of a chained heart...
                m.SendSound(Utility.RandomMinMax(0x423, 0x427));

                Timer = new InternalTimer(m, this);
            }
            else
            {
                m.SendLocalizedMessage(1157630); // You are already bleeding!
            }
        }

        private class InternalTimer : Timer
        {
            public Mobile Owner { get; }
            public ShackledHeartOfThePumpkinKing Heart { get; }

            public int Ticks { get; set; }

            public InternalTimer(Mobile m, ShackledHeartOfThePumpkinKing heart)
                : base(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2))
            {
                Owner = m;
                Heart = heart;

                Start();
            }

            protected override void OnTick()
            {
                if (!Owner.Alive || Owner.Map == Map.Internal || Ticks++ >= 5)
                {
                    Stop();
                    Heart.Timer = null;
                }
                else
                {
                    Owner.PlaySound(0x133);

                    Blood blood = new Blood
                    {
                        ItemID = Utility.Random(0x122A, 5)
                    };
                    blood.MoveToWorld(Owner.Location, Owner.Map);
                }
            }
        }

        public ShackledHeartOfThePumpkinKing(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }
}
