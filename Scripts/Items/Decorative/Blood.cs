#region References
using System;
#endregion

namespace Server.Items
{
    public class Blood : Item
    {
        public static int[] BloodIDs = { 0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F };
        public static int RandomID => Utility.RandomList(BloodIDs);
        public static TimeSpan Decay = TimeSpan.FromSeconds(3.0);
        private static readonly string _TimerID = "BloodTimer";

        public override TimeSpan DecayTime => Decay;

        [Constructable]
        public Blood()
            : this(RandomID)
        { }

        [Constructable]
        public Blood(int itemID)
            : base(itemID)
        {
            Movable = false;

            TimerRegistry.Register(_TimerID, this, DecayTime, blood => blood.Delete());
        }

        public Blood(Serial serial)
            : base(serial)
        {
            TimerRegistry.Register(_TimerID, this, DecayTime, blood => blood.Delete());
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
    }
}
