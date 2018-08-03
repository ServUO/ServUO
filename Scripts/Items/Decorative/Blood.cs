#region References
using System;
#endregion

namespace Server.Items
{
    public class Blood : Item
    {
        public static int[] BloodIDs = { 0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F };

        public static int RandomID { get { return Utility.RandomList(BloodIDs); } }

        public static TimeSpan Decay = TimeSpan.FromSeconds(3.0);

        public override TimeSpan DecayTime { get { return Decay; } }

        [Constructable]
        public Blood()
            : this(RandomID)
        { }

        [Constructable]
        public Blood(int itemID)
            : base(itemID)
        {
            Movable = false;

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

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }

        private class InternalTimer : Timer
        {
            private readonly Blood m_Blood;

            public InternalTimer(Blood blood)
                : base(blood.DecayTime)
            {
                Priority = TimerPriority.OneSecond;

                m_Blood = blood;
            }

            protected override void OnTick()
            {
                m_Blood.Delete();
            }
        }
    }
}