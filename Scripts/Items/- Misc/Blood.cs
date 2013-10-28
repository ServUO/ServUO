using System;

namespace Server.Items
{
    public class Blood : Item
    {
        [Constructable]
        public Blood()
            : this(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F))
        {
        }

        [Constructable]
        public Blood(int itemID)
            : base(itemID)
        {
            this.Movable = false;

            new InternalTimer(this).Start();
        }

        public Blood(Serial serial)
            : base(serial)
        {
            new InternalTimer(this).Start();
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

        private class InternalTimer : Timer
        {
            private readonly Item m_Blood;
            public InternalTimer(Item blood)
                : base(TimeSpan.FromSeconds(5.0))
            {
                this.Priority = TimerPriority.OneSecond;

                this.m_Blood = blood;
            }

            protected override void OnTick()
            {
                this.m_Blood.Delete();
            }
        }
    }
}